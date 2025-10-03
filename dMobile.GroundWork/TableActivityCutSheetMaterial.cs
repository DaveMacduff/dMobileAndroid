// <copyright file="TableActivityCutSheetMaterial.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCutSheetMaterial" />.
    /// </summary>
    public class TableActivityCutSheetMaterial : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetMaterial"/> class.
        /// </summary>
        public TableActivityCutSheetMaterial()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetMaterial;
            this.LowerCaseName = "sheet raw material products";
            this.ProperCaseName = "Sheet raw material products";
        }

        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} details.",
                true);
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        var query = @"
                        DELETE FROM tActivityCutSheetMaterial
                            WHERE tActivityCutSheetMaterial.ActivityRefno <= ?
                                OR tActivityCutSheetMaterial.ActivityFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityCutSheetMaterial
                            WHERE tActivityCutSheetMaterial.ActivityRefno
                                NOT IN (SELECT tActivityCutSheet.ActivityRefno FROM tActivityCutSheet)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityCutSheetMaterial
                            WHERE tActivityCutSheetMaterial.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityCutSheet
                            WHERE tActivityCutSheet.ActivityRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityCutSheet
                            WHERE tActivityCutSheet.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = "DELETE FROM tActivityCutSheetMaterial";
                        transaction.Execute(query, subType);

                        query =
                            "DELETE FROM tActivityCutSheet WHERE tActivityCutSheet.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} details complete.",
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
                            $"Purging {this.LowerCaseName} details failed.",
                            true);
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            sqlEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            ex);
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
                            using var ds = new DataSet();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received responseData.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving ticket materials.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityCutSheet"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actRefno = 0;
                                var progValue = 0;
                                var whsLocMapRefno = 0;
                                var actCutSheetFKRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityCutSheet"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)",
                                        0,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CutSheetMaterial));
                                    actRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivity"));
                                    whsLocMapRefno =
                                        GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                    // To remove time - convert twice
                                    var dt = Convert.ToDateTime(drow["ActivityCutSheetDateTime"]);

                                    const string queryActivityCutSheet =
                                        "INSERT INTO tActivityCutSheet (" +
                                        "ActivityRefno, " +
                                        "ActivitySubType, " +
                                        "WarehouseLocationMapRefno, " +
                                        "ActivityCutSheetDateTime, " +
                                        "ActivityCutSheetDescription" +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
                                        "?, " +
                                        "?, " +
                                        "?" +
                                        ")";
                                    var parametersActivityCutSheet = new object[]
                                    {
                                        actRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CutSheetMaterial),
                                        whsLocMapRefno,
                                        DateTime.Parse(dt.ToShortDateString(), CultureInfo.CurrentCulture),
                                        Convert.ToString(drow["ActivityCutSheetDescription"])
                                    };

                                    transaction.Execute(
                                        queryActivityCutSheet,
                                        parametersActivityCutSheet);

                                    actCutSheetFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                    const string queryActivityCutSheetMaterial =
                                        "INSERT INTO tActivityCutSheetMaterial (" +
                                        "ActivityRefno, " +
                                        "ActivityFKRefno, " +
                                        "ActivityStringFKRefno " +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
                                        "?" +
                                        ")";
                                    var parametersActivityCutSheetMaterial = new object[]
                                    {
                                        actRefno,
                                        actCutSheetFKRefno,
                                        Convert.ToString(actCutSheetFKRefno)
                                    };

                                    transaction.Execute(
                                        queryActivityCutSheetMaterial,
                                        parametersActivityCutSheetMaterial);

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
                                "Saving cut sheet materials complete.",
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
                                "Saving sheet materials failed.",
                                false);
                            throw new BaseException(
                                "Database exception occurred. Saving cut sheet materials failed.",
                                ex);
                        }
                    });
        }

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
                "<action>REQUEST CUT SHEET MATERIAL</action>" +
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