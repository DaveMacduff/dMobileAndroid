// <copyright file="TableActivityPickingItem.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models;
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
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingItem";
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales order items";
            this.ProperCaseName = "Sales order items";
        }

        /// <summary>
        ///     The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            await conn.RunInTransactionAsync(transaction =>
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

                    query = @"
                        DELETE FROM tActivityItemReasonCodeXRef
                            WHERE tActivityItemReasonCodeXRef.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemReasonCodeXRef
                            WHERE tActivityItemReasonCodeXRef.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemPickingComment
                            WHERE tActivityItemPickingComment.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemPickingComment
                            WHERE tActivityItemPickingComment.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemPicking
                            WHERE tActivityItemPicking.ActivityItemRefno <= 0
                                OR tActivityItemPicking.ActivityItemFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemPicking
                            WHERE tActivityItemPicking.ActivityItemRefno
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
                    query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                    transaction.Execute(query, this.CurrentSubType);

                    query = @"DELETE FROM tActivityItemReasonCodeXRef
                                WHERE tActivityItemReasonCodeXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                    transaction.Execute(query, this.CurrentSubType);

                    query = @"DELETE FROM tActivityItemPickingComment
                                WHERE tActivityItemPickingComment.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                    transaction.Execute(query, this.CurrentSubType);

                    query = "DELETE FROM tActivityItemPicking WHERE tActivityItemPicking.ActivityItemSubType = ?";
                    transaction.Execute(query, this.CurrentSubType);

                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                    transaction.Execute(query, this.CurrentSubType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} completed.",
                        true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.",
                        true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.",
                        true);
                    throw;
                }
            });
        }

        /// <summary>
        ///     Send a request for the sales order activity line items.
        /// </summary>
        /// <param name="actItemRefno"><see cref="int" /> Line item activity item refno.</param>
        /// <param name="xmlCriteria"><see cref="XDocument" /> Line item filter criteria.</param>
        /// <returns><see cref="string" /> Request response data.</returns>
        protected override async Task<string> SendRequest(int actItemRefno, XDocument xmlCriteria)
        {
            string response;
            try
            {
                var actPickFKRefno = 0;
                var conn = DatabaseConnection.GetInstance().SqlConn;
                var results = await conn.QueryAsync<tActivityPickingModel>(
                    "SELECT * FROM tActivityPicking WHERE ActivityRefno = ?",
                    actItemRefno);
                if (results?.Count > 0)
                {
                    actPickFKRefno = Convert.ToInt32(results[0].ActivityFKRefno);
                }
                else
                {
                    var msg = $"No results in tActivityPicking for ActivityRefno {actItemRefno}";
                    throw new Exception(msg);
                }

                response = await this.SendComm(await this.CreateRequest(actPickFKRefno, xmlCriteria));
            }
            catch (SQLiteException sqlEx)
            {
                throw new Exception("Database exception occurred. Unable to get the sales order items.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Database exception occurred. Unable to get the sales order items.", ex);
            }

            return response;
        }

        /// <summary>
        ///     The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int" />.</param>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items.", false);

                    var progValue = 0;
                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityItemPicking"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actItemPickComment = string.Empty;
                        var actItemPickFKRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tActivityItemPicking"].Rows)
                        {
                            transaction.Execute(
                                "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                actRefno,
                                0);

                            var actItemRefno =
                                Convert.ToInt32(
                                    transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                            actItemPickFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                            var prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                            var locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                            var queryInsertItemPicking = "INSERT INTO tActivityItemPicking (" +
                                                         "ActivityItemRefno, " +
                                                         "ActivityItemSubType, " +
                                                         "ActivityItemFKRefno, " +
                                                         "ActivityItemStringFKRefno, " +
                                                         "ProductUnitRefno, " +
                                                         "WarehouseLocationMapRefno, " +
                                                         "ActivityItemPickingQuantityOrdered, " +
                                                         "ActivityItemPickingBaseQuantityOrdered, " +
                                                         "ActivityItemPickingWeightQuantityOrdered, " +
                                                         "ActivityItemPickingQuantityShipped, " +
                                                         "ActivityItemPickingBaseQuantityShipped, " +
                                                         "ActivityItemPickingWeightQuantityShipped, " +
                                                         "ActivityItemPickingCustomerProductUnitCode, " +
                                                         "ActivityItemPickingDescription, " +
                                                         "ActivityItemPickingQuantityAvailable, " +
                                                         "ActivityItemPickingBaseQuantityAvailable, " +
                                                         "ActivityItemPickingWeightQuantityAvailable," +
                                                         "ActivityItemPickingIsReviewed" +
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
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?, " +
                                                         "?," +
                                                         "?" +
                                                         ")";
                            var parametersInsertItemPicking = new object[]
                            {
                                actItemRefno,
                                Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                actItemPickFKRefno,
                                actItemPickFKRefno.ToString(),
                                prodUnitRefno,
                                locMapRefno,
                                Convert.ToDecimal(drow["ActivityItemPickingQuantityOrdered"]),
                                Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityOrdered"]),
                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityOrdered"]),
                                Convert.ToDecimal(drow["ActivityItemPickingQuantityPicked"]),
                                Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityPicked"]),
                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityPicked"]),
                                Convert.ToString(drow["ActivityItemPickingCustomerProductUnitCode"]),
                                Convert.ToString(drow["ActivityItemPickingDescription"]),
                                Convert.ToDecimal(drow["ActivityItemPickingQuantityAvailable"]),
                                Convert.ToDecimal(drow["ActivityItemPickingBaseQuantityAvailable"]),
                                Convert.ToDecimal(drow["ActivityItemPickingWeightQuantityAvailable"]),
                                false
                            };

                            transaction.Execute(queryInsertItemPicking, parametersInsertItemPicking);

                            actItemPickComment = Convert.ToString(drow["ActivityItemPickingComment"]);

                            if (!string.IsNullOrEmpty(actItemPickComment))
                            {
                                transaction.Execute(
                                    "INSERT INTO tActivityItemPickingComment (ActivityItemRefno, ActivityItemPickingCommentComment) VALUES (?, ?)",
                                    actItemRefno, actItemPickComment);
                            }

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items failed.", true);

                    throw new Exception("Database exception occurred. Saving sales order items failed.", ex);
                }
            });
        }

        /// <summary>
        ///     The CreateRequest.
        /// </summary>
        /// <param name="activityItemFkRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
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
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER ITEMS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<refno>" + activityItemFkRefno + "</refno>"
                    + "<whs_id></whs_id>"
                    + "<sect_refno></sect_refno>"
                    + "</command>" +
                    "<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>"
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER ITEMS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<refno>" + activityItemFkRefno + "</refno>"
                    + "<whs_id>" + criteria.Root.Element("whs_id").Value + "</whs_id>"
                    + "<sect_refno>" + criteria.Root.Element("sect_refno").Value + "</sect_refno>"
                    + "</command>"
                    + "<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>"
                    + "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + activityItemFkRefno + "</refno>"
                + "<whs_id></whs_id>"
                + "<sect_refno></sect_refno>"
                + "</command>" +
                "<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The Request Item Data.
        /// </summary>
        /// <param name="actHdrRefno"> Activity header refno.</param>
        /// <returns><see cref="Task" /> Task completed.</returns>
        public override async Task RequestItemData(int actHdrRefno)
        {
            if (actHdrRefno == 0)
            {
                var query = "SELECT * FROM tActivityPicking WHERE ActivitySubType = ?";
                var conn = DatabaseConnection.GetInstance().SqlConn;
                var queryResult = await conn.QueryAsync<tActivityPickingModel>(query, (int)this.CurrentSubType);
                if (queryResult != null && queryResult.Count > 0)
                {
                    foreach (var model in queryResult)
                    {
                        _ = base.RequestItemData(model.ActivityFKRefno);
                    }
                }
            }
            else
            {
                _ = base.RequestItemData(actHdrRefno);
            }
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}