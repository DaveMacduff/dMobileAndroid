// <copyright file="TableActivityCutSheetMaterialItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCutSheetMaterialItem" />.
    /// </summary>
    public class TableActivityCutSheetMaterialItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetMaterialItem"/> class.
        /// </summary>
        public TableActivityCutSheetMaterialItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetMaterial;
            this.LowerCaseName = "sheet raw material product";
            this.ProperCaseName = "Sheet raw material product";
        }

        /// <inheritdoc/>
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
                            "DELETE FROM tActivityItemCutSheetMaterial WHERE tActivityItemCutSheetMaterial.ActivityItemRefno <= 0" +
                            " OR tActivityItemCutSheetMaterial.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemCutSheetMaterial WHERE tActivityItemCutSheetMaterial.ActivityItemRefno " +
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

                        query = "DELETE FROM tActivityItemCutSheetMaterial";
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
                        throw new Exception(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            sqlEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            ex);
                    }
                });
        }

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityCutSheetMaterialModel>(
                    "SELECT * FROM tActivityCutSheetMaterial WHERE ActivityFKRefno = ?",
                    activityFkRefno);

            int actCutSheetFKRefno;
            if (results?.Count > 0)
            {
                actCutSheetFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception(
                    "No results in tActivityCutSheetMaterial for ActivityRefno " +
                    activityFkRefno.ToString() +
                    "Send");
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
                                "Saving sheet material items.",
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

                                var actItemRefno = 0;
                                var progValue = 0;
                                var prodUnitRefno = 0;
                                var locMapRefno = 0;
                                var actItemCutSheetFKRefno = 0;

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
                                        refno,
                                        0);
                                    actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    prodUnitRefno = GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        false);
                                    locMapRefno = GetWhsLocMapRefnoFromDataRow(
                                        drow,
                                        transaction);
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
                                        Convert.ToInt32(this.CurrentSubType),
                                        prodUnitRefno,
                                        locMapRefno,
                                        Convert.ToDecimal(drow["ActivityItemCutSheetQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetBaseQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemCutSheetWeightQuantity"]),
                                        Convert.ToString(drow["ActivityItemCutSheetDescription"]),
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

                                    actItemCutSheetFKRefno =
                                        Convert.ToInt32(drow["ActivityItemCutSheetFKRefno"]);
                                    transaction.Execute(
                                        "INSERT INTO tActivityItemCutSheetMaterial (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
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
                                "Saving sheet material items complete.",
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
                                "Saving sheet material items failed.",
                                false);
                            throw new Exception(
                                "Database exception occurred. Saving sheet material items failed.",
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
                "<action>REQUEST CUT SHEET MATERIAL ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno +
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