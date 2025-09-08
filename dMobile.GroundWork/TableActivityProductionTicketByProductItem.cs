// <copyright file="TableActivityProductionTicketByProductItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketByProductItem" />.
    /// </summary>
    public class TableActivityProductionTicketByProductItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductItem"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.TicketAddProduct;
            this.LowerCaseName = "ticket additional product";
            this.ProperCaseName = "Ticket additional product";
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
                            "DELETE FROM tActivityItemProductionByProduct WHERE tActivityItemProductionByProduct.ActivityItemRefno <= 0" +
                            " OR tActivityItemProductionByProduct.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemProductionByProduct WHERE tActivityItemProductionByProduct.ActivityItemRefno " +
                            "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query =
                            "DELETE FROM tActivityItemProduction WHERE tActivityItemProduction.ActivityItemRefno <= 0";
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

                        query = "DELETE FROM tActivityItemProductionByProduct";
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

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityProductionByProductModel>(
                    "SELECT * FROM tActivityProductionByProduct WHERE ActivityFKRefno = ?",
                    activityFkRefno);

            int productionFKRefno;
            if (results?.Count > 0)
            {
                productionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drByProduct.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(productionFKRefno));
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
                                "Saving ticket add-product items.",
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
                                var locMapRefno = 0;
                                var actItemProductionFKRefno = 0;
                                var prodUnitRefno = 0;

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
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        refno,
                                        0);

                                    actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    actItemProductionFKRefno =
                                        Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                    prodUnitRefno = GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        false);
                                    locMapRefno = GetWhsLocMapRefnoFromDataRow(
                                        drow,
                                        transaction);

                                    const string queryActivityItemProduction =
                                        "INSERT INTO tActivityItemProduction " +
                                        "(" +
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
                                        ") " +
                                        "VALUES " +
                                        "(" +
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
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
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

                                    const string queryActivityItemProductionByProduct =
                                        "INSERT INTO tActivityItemProductionByProduct " +
                                        "(" +
                                        "ActivityItemRefno, " +
                                        "ActivityItemFKRefno, " +
                                        "ActivityItemStringFKRefno" +
                                        ") VALUES (" +
                                        "?, " +
                                        "?, " +
                                        "?" +
                                        ")";
                                    var parametersActivityItemProductionByProduct = new object[]
                                    {
                                        actItemRefno,
                                        actItemProductionFKRefno,
                                        Convert.ToString(actItemProductionFKRefno)
                                    };

                                    transaction.Execute(
                                        queryActivityItemProductionByProduct,
                                        parametersActivityItemProductionByProduct);

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
                                "Saving ticket add-product items complete.",
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
                                "Saving ticket add-product items failed.",
                                false);

                            throw new Exception(
                                "Database exception occurred. Saving ticket add-product items failed.",
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
                "<action>REQUEST PRODUCTION TICKET BYPRODUCT ITEMS</action>" +
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