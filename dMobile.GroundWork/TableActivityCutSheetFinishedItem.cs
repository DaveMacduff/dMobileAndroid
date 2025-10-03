// <copyright file="TableActivityCutSheetFinishedItem.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models.Headers;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityCutSheetFinishedItem" />.
    /// </summary>
    public class TableActivityCutSheetFinishedItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetFinishedItem"/> class.
        /// </summary>
        public TableActivityCutSheetFinishedItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetFinished;
            this.LowerCaseName = "cut sheet finished product";
            this.ProperCaseName = "Cut sheet finished product";
        }

        /// <inheritdoc cref="TableBase.PurgeData()"/>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} items.",
                true);
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        var query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemCutSheetFinished WHERE tActivityItemCutSheetFinished.ActivityItemRefno <= 0" +
                            " OR tActivityItemCutSheetFinished.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemCutSheetFinished WHERE tActivityItemCutSheetFinished.ActivityItemRefno " +
                            "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemCutSheet WHERE tActivityItemCutSheet.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemCutSheet
                            WHERE tActivityItemCutSheet.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                        transaction.Execute(query);

                        // Regular purge.
                        query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno
                                    FROM tActivityItem
                                    WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItemCutSheetFinished";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItemCutSheet WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

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

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityCutSheetFinishedModel>(
                    "SELECT * FROM tActivityCutSheetFinished WHERE ActivityFKRefno = ?",
                    Convert.ToInt32(activityFkRefno));

            int actCutSheetFKRefno;
            if (results?.Count > 0)
            {
                actCutSheetFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new BaseException("drFinished.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(actCutSheetFKRefno));
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int refno)
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
                                "Reading received data.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving finished cut sheet items.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemCutSheet"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var progValue = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemCutSheet"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(this.CurrentSubType),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        Convert.ToInt32(refno),
                                        0);
                                    var actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    var prodUnitRefno = GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        false);

                                    const string queryActivityItemCutSheet =
                                        "INSERT INTO tActivityItemCutSheet (" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemSubType, " +
                                        "ProductUnitRefno, " +
                                        "LocationMapRefno, " +
                                        "ActivityItemCutSheetQuantity, " +
                                        "ActivityItemCutSheetBaseQuantity, " +
                                        "ActivityItemCutSheetWeightQuantity, " +
                                        "ActivityItemCutSheetDescription, " +
                                        "ActivityItemCutSheetQuantityExpected, " +
                                        "ActivityItemCutSheetBaseQuantityExpected, " +
                                        "ActivityItemCutSheetWeightQuantityExpected" +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
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
                                    var parametersActivityItemCutSheet = new object[]
                                    {
                                        actItemRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CutSheetFinished),
                                        prodUnitRefno,
                                        GetWhsLocMapRefnoFromDataRow(drow, transaction),
                                        Convert.ToDecimal(drow["ActivityItemCutSheetQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetBaseQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetWeightQuantity"]),
                                        drow["ActivityItemCutSheetDescription"].ToString(),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetBaseQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetWeightQuantityExpected"])
                                    };

                                    transaction.Execute(
                                        queryActivityItemCutSheet,
                                        parametersActivityItemCutSheet);

                                    var actItemCutSheetFKRefno =
                                        Convert.ToInt32(drow["ActivityItemCutSheetFKRefno"]);
                                    transaction.Execute(
                                        "INSERT INTO tActivityItemCutSheetFinished (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                                        actItemRefno,
                                        actItemCutSheetFKRefno,
                                        Convert.ToString(actItemCutSheetFKRefno));

                                    progValue++;
                                    if (progValue % 100 == 0)
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
                                "Saving finished cut sheet items complete.",
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
                                "Saving finished cut sheet items failed.",
                                false);

                            throw new BaseException(
                                "Database exception occurred. Saving finished cut sheet items failed.",
                                ex);
                        }
                    });
        }

        /// <inheritdoc cref="TableBase.CreateRequest(int)"/>
        protected override async Task<string> CreateRequest(int activityFkRefno)
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
                "<action>REQUEST CUT SHEET FINISHED ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno.ToString() +
                "</refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}