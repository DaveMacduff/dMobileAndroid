// <copyright file="TableActivityTransferTo.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityTransferTo : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransferTo"/> class.
        /// </summary>
        public TableActivityTransferTo()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferTo";
            this.CurrentSubType = SysCommon.ActSubTypes.TransferTo;
            this.LowerCaseName = "to transfers";
            this.ProperCaseName = "To transfers";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from.", true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                        DELETE FROM tActivityTransfer
                            WHERE tActivityTransfer.ActivityRefno <= 0
                                OR tActivityTransfer.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityTransfer
                            WHERE tActivityTransfer.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // REGULAR PURGE
                        query =
                            "DELETE FROM tActivityAdjustment WHERE tActivityAdjustment.ActivitySubType = ?";
                        transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, Convert.ToInt32(this.CurrentSubType));

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Purging transfers from complete.",
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using (var ds = new DataSet())
                            {
                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Reading received responseData.",
                                    false);

                                ds.ReadXml(
                                    SysCommon.GetXmlReader(ref responseData),
                                    XmlReadMode.Auto);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Saving transfers to.",
                                    false);

                                if (ds.Tables.Count != 0)
                                {
                                    var progMax = Convert.ToInt32(
                                        ds.Tables["tActivityTransfer"].Rows.Count);

                                    this.DataAccessStatusOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        $"Records {progMax}",
                                        false);

                                    var actRefno = 0;
                                    var progValue = 0;
                                    var locMapWhsFKRefno = 0;
                                    var locMapWhsDescription = string.Empty;
                                    var transFKRefno = 0;
                                    var transDesc = string.Empty;
                                    var transDateTime = new DateTime(1900, 1, 1);
                                    var arrivalDateTime = new DateTime(1900, 1, 1);

                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        0,
                                        0,
                                        progMax);

                                    foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                                    {
                                        errMsg = "Arguments:";

                                        transFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                        errMsg = "Trans fk refno " + transFKRefno;
                                        transDesc = Convert.ToString(
                                            drow["ActivityTransferDescription"]);
                                        errMsg += Environment.NewLine +
                                                  "Trans desc \'" +
                                                  transDesc +
                                                  "\'";
                                        transDateTime = Convert.ToDateTime(
                                            drow["ActivityTransferDateTime"]);
                                        errMsg += Environment.NewLine +
                                                  "Trans date " +
                                                  transDateTime.ToShortDateString();
                                        arrivalDateTime = Convert.ToDateTime(
                                            drow["ActivityTransferArrivalDateTime"]);
                                        errMsg += Environment.NewLine +
                                                  "Trans arrival date " +
                                                  arrivalDateTime.ToShortDateString();
                                        locMapWhsFKRefno = Convert.ToInt32(
                                            drow["TransferLocationMapWarehouseFKRefno"]);
                                        errMsg += Environment.NewLine +
                                                  "Whs fk refno " +
                                                  locMapWhsFKRefno;

                                        locMapWhsDescription =
                                            drow["ActivityTransferDescription"].ToString();
                                        errMsg += Environment.NewLine +
                                                  "Whs desc \'" +
                                                  locMapWhsDescription +
                                                  "\'";

                                        transaction.Execute(
                                            "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                            0,
                                            Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                                        actRefno = Convert.ToInt32(
                                            transaction.ExecuteScalar<int>(
                                                "SELECT last_insert_rowid() FROM tActivity"));

                                        var query = "INSERT INTO tActivityTransfer (" +
                                                    "ActivityRefno, " +
                                                    "ActivitySubType, " +
                                                    "ActivityFKRefno, " +
                                                    "ActivityStringFKRefno, " +
                                                    "TransferWarehouseLocationMapFKRefno, " +
                                                    "TransferWarehouseLocationMapDescription, " +
                                                    "ActivityTransferDescription, " +
                                                    "ActivityTransferDateTime, " +
                                                    "ActivityTransferArrivalDateTime" +
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
                                            Convert.ToInt32(actRefno),
                                            Convert.ToInt32(SysCommon.ActSubTypes.TransferTo),
                                            Convert.ToInt32(transFKRefno),
                                            Convert.ToString(transFKRefno),
                                            Convert.ToInt32(locMapWhsFKRefno),
                                            Convert.ToString(locMapWhsDescription),
                                            transDesc,
                                            Convert.ToDateTime(transDateTime),
                                            Convert.ToDateTime(arrivalDateTime)
                                        };

                                        transaction.Execute(query, parameters);

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
                            }

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving transfers to complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                false);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving transfers to failed.",
                                false);

                            throw new Exception(
                                "Database exception occurred. Saving transfers to failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                "<action>REQUEST TRANSFERS TO</action>" +
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