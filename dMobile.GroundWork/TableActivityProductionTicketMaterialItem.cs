// <copyright file="TableActivityProductionTicketMaterialItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using dMobile.Resources.Models.Headers;
    using Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityProductionTicketMaterialItem" />.
    /// </summary>
    public class TableActivityProductionTicketMaterialItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterialItem"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterialItem()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialItem";
            this.CurrentSubType = SysCommon.ActSubTypes.TicketMaterial;
            this.LowerCaseName = "ticket raw material product";
            this.ProperCaseName = "Ticket raw material product";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
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
                            "DELETE FROM tActivityItemProductionMaterial WHERE tActivityItemProductionMaterial.ActivityItemRefno <= 0" +
                            " OR tActivityItemProductionMaterial.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemProductionMaterial WHERE tActivityItemProductionMaterial.ActivityItemRefno " +
                            "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query =
                            @"DELETE FROM tActivityItemProduction WHERE tActivityItemProduction.ActivityItemRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemProduction
                            WHERE tActivityItemProduction.ActivityItemRefno
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

                        query = "DELETE FROM tActivityItemProductionMaterial";
                        transaction.Execute(query, this.CurrentSubType);

                        query = "DELETE FROM tActivityItemProduction WHERE ActivityItemSubType = ?";
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

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="activityFkRefnoThe activityFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityProductionMaterialModel>(
                    "SELECT * FROM tActivityProductionMaterial WHERE ActivityFKRefno = ?",
                    activityFkRefno);

            int actProductionFKRefno;
            if (results?.Count > 0)
            {
                actProductionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception(
                    "No results in tActivityProductionMaterial for ActivityRefno " +
                    activityFkRefno +
                    Environment.NewLine +
                    this.FullClassName +
                    "Send");
            }

            return await SendComm(await this.CreateRequest(actProductionFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int"/>.</param>
        protected override async Task Save(string responseData, int actRefno)
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
                                "Saving ticket material items.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemProduction"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actItemRefno = 0;
                                var progValue = 0;
                                var prodUnitRefno = 0;
                                var locMapRefno = 0;
                                var actItemProductionFKRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(this.CurrentSubType),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        actRefno,
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
                                    var queryActivityItemProduction =
                                        "INSERT INTO tActivityItemProduction (" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemSubType, " +
                                        "ProductUnitRefno, " +
                                        "LocationMapRefno, " +
                                        "ActivityItemProductionQuantity, " +
                                        "ActivityItemProductionBaseQuantity, " +
                                        "ActivityItemProductionWeightQuantity, " +
                                        "ActivityItemProductionDescription, " +
                                        "ActivityItemProductionQuantityExpected, " +
                                        "ActivityItemProductionBaseQuantityExpected, " +
                                        "ActivityItemProductionWeightQuantityExpected" +
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
                                    var parametersActivityItemProduction = new object[]
                                    {
                                        actItemRefno,
                                        Convert.ToInt32(this.CurrentSubType),
                                        prodUnitRefno,
                                        locMapRefno,
                                        Convert.ToDecimal(drow["ActivityItemProductionQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionBaseQuantity"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionWeightQuantity"]),
                                        Convert.ToString(drow["ActivityItemProductionDescription"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionBaseQuantityExpected"]),
                                        Convert.ToDecimal(
                                            drow["ActivityItemProductionWeightQuantityExpected"])
                                    };

                                    transaction.Execute(
                                        queryActivityItemProduction,
                                        parametersActivityItemProduction);

                                    actItemProductionFKRefno =
                                        Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                    transaction.Execute(
                                        "INSERT INTO tActivityItemProductionMaterial (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)",
                                        actItemRefno,
                                        actItemProductionFKRefno,
                                        Convert.ToString(actItemProductionFKRefno));

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
                                "Saving ticket material items complete.",
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
                                "Saving ticket material items failed.",
                                false);
                            throw new Exception(
                                "Database exception occurred. Saving ticket material items failed.",
                                ex);
                        }
                    });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno">The activityFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
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
                "<action>REQUEST PRODUCTION TICKET MATERIAL ITEMS</action>" +
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