// <copyright file="TableActivityCycleCount.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCycleCount" />.
    /// </summary>
    public class TableActivityCycleCount : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCount"/> class.
        /// </summary>
        public TableActivityCycleCount()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCount";
            this.CurrentSubType = SysCommon.ActSubTypes.CycleCounts;
            this.LowerCaseName = "cycle counts";
            this.ProperCaseName = "Cycle counts";
        }

        /// <summary>
        /// Purge the activity headers.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName}.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                        DELETE FROM tActivityCount
                            WHERE tActivityCount.ActivityRefno <= ?
                                OR tActivityCount.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityCount
                            WHERE tActivityCount.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query =
                            "DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} complete.",
                            true);
                    }
                    catch (SQLiteException sqlEx)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Database exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"{sqlEx.Message}",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                    catch
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} failed.",
                            true);
                        throw;
                    }
                });
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received responseData.",
                                false);

                            using var ds = new DataSet();
                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle counts.",
                                false);

                            // Receiving schema but no responseData results in no table being created in the dataset.
                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityCount"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var progValue = 0;
                                var actCountTypeDesc = string.Empty;
                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityCount"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                                    var actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));

                                    var countFkRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                    var whsLocMapRefno = GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                    // To remove time - convert twice
                                    var dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);

                                    switch (Convert.ToInt32(drow["ActivityCountType"]))
                                    {
                                        case (int)SysCommon.ActCountTypeFields
                                            .CycleCountByWarehouse:

                                            actCountTypeDesc = "Cycle Count By Warehouse";
                                            break;

                                        case (int)SysCommon.ActCountTypeFields.CycleCountBySection:

                                            actCountTypeDesc = "Cycle Count By Section";
                                            break;

                                        case (int)SysCommon.ActCountTypeFields.CycleCountBySlot:

                                            actCountTypeDesc = "Cycle Count By Slot";
                                            break;

                                        case (int)SysCommon.ActCountTypeFields
                                            .BaseCycleCountByWarehouse:

                                            actCountTypeDesc = "Base Cycle Count By Warehouse";
                                            break;

                                        case (int)SysCommon.ActCountTypeFields
                                            .PhysicalCountByWarehouse:

                                            actCountTypeDesc =
                                                "Standard Physical Count By Warehouse";
                                            break;
                                    }

                                    // <wo="14563" comment="receiving strings up to 75 in length when database wants max 60. This is the viewable description when selecting line item." />
                                    var incomingDescription = this.ReduceWhitespace(
                                        drow["ActivityCountDescription"].ToString());

                                    if (incomingDescription.Length > 60)
                                    {
                                        incomingDescription =
                                            incomingDescription.Replace(
                                                " ",
                                                string.Empty); // remove all spaces
                                    }

                                    if (incomingDescription.Length > 60)
                                    {
                                        incomingDescription =
                                            incomingDescription
                                                [..60]; // truncate the end if we need to.
                                    }

                                    const string sql = "INSERT INTO tActivityCount (" +
                                                       "ActivityRefno, " +
                                                       "ActivitySubType, " +
                                                       "ActivityFKRefno, " +
                                                       "ActivityStringFKRefno, " +
                                                       "WarehouseLocationMapRefno, " +
                                                       "ActivityCountType, " +
                                                       "ActivityCountDateTime, " +
                                                       "ActivityCountTypeDescription, " +
                                                       "ActivityCountDescription" +
                                                       ") VALUES (" +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?, " +
                                                       "?" +
                                                       ")";

                                    var parameters = new object[]
                                    {
                                        actRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                        countFkRefno,
                                        countFkRefno.ToString(),
                                        whsLocMapRefno,
                                        Convert.ToInt32(drow["ActivityCountType"]),
                                        DateTime.Parse(dt.ToShortDateString()),
                                        actCountTypeDesc,
                                        incomingDescription
                                    };

                                    transaction.Execute(sql, parameters);

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    progMax,
                                    0,
                                    progMax);
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle counts complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle counts failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving cycle counts failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        /// Reduces excess whitespace in a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReduceWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var prevIsWhitespace = false;
            var charArray = input.ToCharArray();
            for (var i = 0; i < input.Length; ++i)
            {
                if (char.IsWhiteSpace(charArray[i]))
                {
                    if (prevIsWhitespace)
                    {
                        continue;
                    }

                    prevIsWhitespace = true;
                }
                else
                {
                    prevIsWhitespace = false;
                }

                sb.Append(charArray[i]);
            }

            return sb.ToString();
        }

        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST CYCLE COUNTS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}