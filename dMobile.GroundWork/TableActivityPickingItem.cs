// <copyright file="TableActivityPickingItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
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
    using Resources.Models.Headers;
    using SQLite;

    /// <summary>
    ///     Defines the <see cref="TableActivityPickingItem" />.
    /// </summary>
    public class TableActivityPickingItem : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityPickingItem" /> class.
        /// </summary>
        public TableActivityPickingItem()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales order items";
            this.ProperCaseName = "Sales order items";
        }

        /// <summary>
        ///     Purge the activity header data.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);
                    var query = @"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemBarCodeAssociationXRef
                    WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                        NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemReasonCodeXRef
                    WHERE tActivityItemReasonCodeXRef.ActivityItemRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemReasonCodeXRef
                    WHERE tActivityItemReasonCodeXRef.ActivityItemRefno
                        NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemPickingComment
                    WHERE tActivityItemPickingComment.ActivityItemRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemPickingComment
                    WHERE tActivityItemPickingComment.ActivityItemRefno
                        NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemPicking
                    WHERE tActivityItemPicking.ActivityItemRefno <= 0
                        OR tActivityItemPicking.ActivityItemFKRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItemPicking
                    WHERE tActivityItemPicking.ActivityItemRefno
                        NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityItem
                    WHERE tActivityItem.ActivityRefno
                        NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                await Conn.ExecuteAsync(query);
                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = "DELETE FROM tActivityItemBarCodeAssociationXRef " +
                    "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN " +
                    "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = @"DELETE FROM tActivityItemReasonCodeXRef
                    WHERE tActivityItemReasonCodeXRef.ActivityItemRefno IN
                    (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = @"DELETE FROM tActivityItemPickingComment
                    WHERE tActivityItemPickingComment.ActivityItemRefno IN
                    (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = "DELETE FROM tActivityItemPicking WHERE tActivityItemPicking.ActivityItemSubType = ?";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} complete.",
                    true);
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        ///     Send a request for the sales order activity line items.
        /// </summary>
        /// <param name="activityFkRefno">The activity foreign key reference number.</param>
        /// <param name="criteria"><see cref="XDocument" /> Line item filter criteria.</param>
        /// <returns><see cref="string" /> Request response data.</returns>
        protected override async Task<string> SendRequest(int activityFkRefno, XDocument criteria)
        {
            string response;
            try
            {
                int actPickFkRefno;
                    var results = await Conn.QueryAsync<ActivityPickingModel>(
                    "SELECT * FROM tActivityPicking WHERE ActivityFKRefno = ?",
                    activityFkRefno);
                if (results?.Count > 0)
                {
                    actPickFkRefno = Convert.ToInt32(results[0].ActivityFKRefno);
                }
                else
                {
                    var msg = $"No results in tActivityPicking for ActivityRefno {activityFkRefno}";
                    throw new BaseException(msg);
                }

                response = await SendComm(await this.CreateRequest(actPickFkRefno, criteria));
            }
            catch (SQLiteException sqlEx)
            {
                throw new BaseException(
                    "Database exception occurred. Unable to get the sales order items.",
                    sqlEx);
            }
            catch (Exception ex)
            {
                throw new BaseException(
                    "Database exception occurred. Unable to get the sales order items.",
                    ex);
            }

            return response;
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int refno)
        {
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    try
                    {
                        var items = new List<Seg>();
                        foreach (DataRow row in rows)
                        {
                            var itemRefno = await GetActivityItemRefno();
                            var item = new Seg
                            {
                                ActivityItemRefno = itemRefno,
                                ActivityFKRefno = refno,
                                ActivityPickingFKRefno = Convert.ToInt32(row["ActivityPickingFKRefno"]),
                                ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"]),
                                ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemFKRefno"]),
                                ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"]),
                                ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                                UnitCode = Convert.ToString(row["UnitCode"]),
                                LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"]),
                                ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"]),
                                ActivityItemPickingDescription = Convert.ToString(row["ActivityItemPickingDescription"]),
                                ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"]),
                                ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"]),
                                ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"]),
                                ActivityItemPickingQuantityPicked = Convert.ToDecimal(row["ActivityItemPickingQuantityPicked"]),
                                ActivityItemPickingBaseQuantityPicked = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityPicked"]),
                                ActivityItemPickingWeightQuantityPicked = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityPicked"]),
                                ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"]),
                                ActivityItemPickingBaseQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityAvailable"]),
                                ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"]),
                                ActivityItemPickingComment = Convert.ToString(row["ActivityItemPickingComment"])
                            };

                            items.Add(item);
                        }

                        var query = "CREATE TEMP TABLE Seg (" +
                            "ActivityItemRefno INTEGER PRIMARY KEY," +
                            "ActivityFKRefno INTEGER," +
                            "ActivityPickingFKRefno INTEGER," +
                            "ActivityItemFKRefno INTEGER," +
                            "ActivityItemStringFKRefno VARCHAR(10)," +
                            "ActivityItemPickingFKRefno INTEGER," +
                            "ProductFKRefno INTEGER," +
                            "ProductUnitIsBC INTEGER," +
                            "UnitCode VARCHAR(2)," +
                            "LocationMapWarehouseFKRefno INTEGER," +
                            "ActivityItemPickingCustomerProductUnitCode VARCHAR(10)," +
                            "ActivityItemPickingDescription VARCHAR(40)," +
                            "ActivityItemPickingQuantityOrdered DECIMAL(11, 3)," +
                            "ActivityItemPickingBaseQuantityOrdered DECIMAL(11, 3)," +
                            "ActivityItemPickingWeightQuantityOrdered DECIMAL(11, 3)," +
                            "ActivityItemPickingQuantityPicked DECIMAL(11, 3)," +
                            "ActivityItemPickingBaseQuantityPicked DECIMAL(11, 3)," +
                            "ActivityItemPickingWeightQuantityPicked DECIMAL(11, 3)," +
                            "ActivityItemPickingQuantityAvailable DECIMAL(11, 3)," +
                            "ActivityItemPickingBaseQuantityAvailable DECIMAL(11, 3)," +
                            "ActivityItemPickingWeightQuantityAvailable DECIMAL(11, 3)," +
                            "ActivityItemPickingComment VARCHAR(254)" +
                            ")";
                        await Conn.ExecuteAsync(query);
                        await Conn.InsertAllAsync(items);

                        query = "INSERT INTO tActivityItemPicking (" +
                            "ActivityItemRefno," +
                            "ActivityItemSubType," +
                            "ActivityItemFKRefno," +
                            "ActivityItemStringFKRefno," +
                            "ProductUnitRefno," +
                            "WarehouseLocationMapRefno," +
                            "ActivityItemPickingQuantityOrdered," +
                            "ActivityItemPickingBaseQuantityOrdered," +
                            "ActivityItemPickingWeightQuantityOrdered," +
                            "ActivityItemPickingQuantityShipped," +
                            "ActivityItemPickingBaseQuantityShipped," +
                            "ActivityItemPickingWeightQuantityShipped," +
                            "ActivityItemPickingCustomerProductUnitCode," +
                            "ActivityItemPickingDescription," +
                            "ActivityItemPickingQuantityAvailable," +
                            "ActivityItemPickingBaseQuantityAvailable," +
                            "ActivityItemPickingWeightQuantityAvailable," +
                            "ActivityItemPickingIsReviewed" +
                            ") SELECT " +
                            "Seg.ActivityItemRefno," +
                            $"{(int)SysCommon.ActSubTypes.Picking} as ActivityItemSubType," +
                            "Seg.ActivityItemFKRefno," +
                            "Seg.ActivityItemStringFKRefno," +
                            "prodUnit.ProductUnitRefno," +
                            "whs.LocationMapRefno as WarehouseLocationMapRefno," +
                            "Seg.ActivityItemPickingQuantityOrdered," +
                            "Seg.ActivityItemPickingBaseQuantityOrdered," +
                            "Seg.ActivityItemPickingWeightQuantityOrdered," +
                            "Seg.ActivityItemPickingQuantityPicked," +
                            "Seg.ActivityItemPickingBaseQuantityPicked," +
                            "Seg.ActivityItemPickingWeightQuantityPicked," +
                            "Seg.ActivityItemPickingCustomerProductUnitCode," +
                            "Seg.ActivityItemPickingDescription," +
                            "Seg.ActivityItemPickingQuantityAvailable," +
                            "Seg.ActivityItemPickingBaseQuantityAvailable," +
                            "Seg.ActivityItemPickingWeightQuantityAvailable," +
                            "0 AS ActivityItemPickingIsReviewed" +
                            " FROM Seg" +
                            " INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapWarehouseFKRefno =" +
                            " Seg.LocationMapWarehouseFKRefno" +
                            " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                            " INNER JOIN tUnit ON tUnit.UnitCode = Seg.UnitCode" +
                            " INNER JOIN tProductUnit prodUnit ON prodUnit.ProductRefno = prod.ProductRefno" +
                            " AND prodUnit.UnitRefno = tUnit.UnitRefno" +
                            " AND prodUnit.ProductUnitIsBC = Seg.ProductUnitIsBC";
                        await Conn.ExecuteAsync(query);

                        query = "INSERT INTO tActivityItemPickingComment (" +
                            "ActivityItemRefno," +
                            "ActivityItemPickingCommentComment" +
                            ") SELECT " +
                            "Seg.ActivityItemRefno," +
                            "Seg.ActivityItemPickingComment as ActivityItemPickingCommentComment" +
                            " FROM Seg" +
                            " WHERE COALESCE(Seg.ActivityItemPickingComment, '') != ''";
                        await Conn.ExecuteAsync(query);
                    }
                    finally
                    {
                        // Drop the temporary table
                        const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                        await Conn.ExecuteAsync(dropTempTableQuery);
                    }
                }
            }

            return;

            async Task<int> GetActivityItemRefno()
            {
                var query = "INSERT INTO tActivityItem (" +
                    "ActivityItemSubType" +
                    ",ActivityItemStatus" +
                    ",ActivityRefno" +
                    ",ActivityItemIsAdded) VALUES (?, ?, ?, ?)";
                await Conn.ExecuteAsync(
                    query,
                    Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                    Convert.ToInt32(SysCommon.ActStatus.NoChange),
                    refno,
                    0);
                query = "SELECT last_insert_rowid()";
                ////query = "SELECT tActivityItem.ActivityItemRefno" +
                ////    " FROM tActivityItem" +
                ////    $" WHERE tActivityItem.ActivityItemSubType = {Convert.ToInt32(SysCommon.ActSubTypes.Picking)}" +
                ////    " ORDER BY tActivityItem.ActivityItemRefno DESC LIMIT 1";
                return Convert.ToInt32(await Conn.ExecuteScalarAsync<int>(query));
            }
        }

        protected override async Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (criteria == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDER ITEMS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<refno>" +
                    activityFkRefno +
                    "</refno>" +
                    "<whs_id></whs_id>" +
                    "<sect_refno></sect_refno>" +
                    "</command>" +
                    "<is_disconnected>" +
                    this.SysEnv.SysSettings.IsDisconnectedMode +
                    "</is_disconnected>" +
                    "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDER ITEMS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<refno>" +
                    activityFkRefno +
                    "</refno>" +
                    "<whs_id>" +
                    criteria.Root?.Element("whs_id") !.Value +
                    "</whs_id>" +
                    "<sect_refno>" +
                    criteria.Root?.Element("sect_refno") !.Value +
                    "</sect_refno>" +
                    "</command>" +
                    "<is_disconnected>" +
                    this.SysEnv.SysSettings.IsDisconnectedMode +
                    "</is_disconnected>" +
                    "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

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
                "<action>REQUEST SALES ORDER ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno +
                "</refno>" +
                "<whs_id></whs_id>" +
                "<sect_refno></sect_refno>" +
                "</command>" +
                "<is_disconnected>" +
                this.SysEnv.SysSettings.IsDisconnectedMode +
                "</is_disconnected>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            /// <summary>
            ///    Gets or sets the activity item reference number.
            /// </summary>
            public int ActivityItemRefno { get; set; }

            // Gets or sets the activity foreign key reference number.
            public int ActivityFKRefno { get; set; }

            // Gets or sets the activity picking foreign key reference number.
            public int ActivityPickingFKRefno { get; set; }

            // Gets or sets the activity item foreign key reference number.
            public int ActivityItemFKRefno { get; set; }

            // Gets or sets the activity item foreign key reference number as a string.
            public string ActivityItemStringFKRefno { get; set; }

            // Gets or sets the product foreign key reference number.
            public int ProductFKRefno { get; set; }

            // Gets or sets a value indicating whether the product unit is BC.
            public bool ProductUnitIsBC { get; set; }

            // Gets or sets the unit code.
            public string UnitCode { get; set; }

            // Gets or sets the location map warehouse foreign key reference number.
            public int LocationMapWarehouseFKRefno { get; set; }

            // Gets or sets the activity item picking customer product unit code.
            public string ActivityItemPickingCustomerProductUnitCode { get; set; }

            // Gets or sets the activity item picking description.
            public string ActivityItemPickingDescription { get; set; }

            // Gets or sets the activity item picking quantity ordered.
            public decimal ActivityItemPickingQuantityOrdered { get; set; }

            // Gets or sets the activity item picking base quantity ordered.
            public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }

            // Gets or sets the activity item picking weight quantity ordered.
            public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }

            // Gets or sets the activity item picking quantity picked.
            public decimal ActivityItemPickingQuantityPicked { get; set; }

            // Gets or sets the activity item picking base quantity picked.
            public decimal ActivityItemPickingBaseQuantityPicked { get; set; }

            // Gets or sets the activity item picking weight quantity picked.
            public decimal ActivityItemPickingWeightQuantityPicked { get; set; }

            // Gets or sets the activity item picking quantity available.
            public decimal ActivityItemPickingQuantityAvailable { get; set; }

            // Gets or sets the activity item picking base quantity available.
            public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }

            // Gets or sets the activity item picking weight quantity available.
            public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }

            // Gets or sets the activity item picking comment.
            public string ActivityItemPickingComment { get; set; }
        }
    }
}