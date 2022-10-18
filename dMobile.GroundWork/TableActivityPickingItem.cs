// <copyright file="TableActivityPickingItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityPickingItem" />.
    /// </summary>
    public class TableActivityPickingItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingItem"/> class.
        /// </summary>
        public TableActivityPickingItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingItem";
        }

        /// <summary>
        /// The RequestData.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        /// <param name="xmlCriteria">The xmlCriteria<see cref="XDocument"/>.</param>
        public override async Task RequestData(int actRefno, XDocument xmlCriteria)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                    if (results?.Count > 0)
                    {
                        foreach (var actvityModel in results)
                        {
                            actRefno = Convert.ToInt32(actvityModel.ActivityRefno);
                            reqData = await this.SendRequest(actRefno, xmlCriteria);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    var reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actRefno);
                            }
                        }
                    }
                }
                else
                {
                    reqData = await this.SendRequest(actRefno, xmlCriteria);

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);

                throw new Exception("Database exception occurred. Seeding sales order items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order items failed.", true);

                throw new Exception("Seeding sales order items failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items.", true);

                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityItemPicking WHERE tActivityItemPicking.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items completed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order items failed.", true);

                throw new Exception("Database exception occurred. Purging sales order items failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales order items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sales order items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The SendRequest.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        /// <param name="xmlCriteria">The xmlCriteria<see cref="XDocument"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actRefno, XDocument xmlCriteria)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityPickingModel>("SELECT * FROM tActivityPicking WHERE ActivityRefno = ?", actRefno);

            int actPickFKRefno;
            if (results?.Count > 0)
            {
                actPickFKRefno = Convert.ToInt32(results[0].ActivityFKRefno);
            }
            else
            {
                throw new Exception("No results in tActivityPicking for ActivityRefno " + actRefno + Environment.NewLine + this.FullClassName + "SendRequest");
            }

            return await this.SendComm(await this.CreateRequest(actPickFKRefno, xmlCriteria));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        protected override async Task Save(string reqData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityItemPicking"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actItemRefno = 0;
                            var prodUnitRefno = 0;
                            var progValue = 0;
                            var locMapRefno = 0;
                            var actItemPickComment = string.Empty;
                            var actItemPickFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPicking"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Picking), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                actItemPickFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

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
                                    false,
                                };

                                transaction.Execute(queryInsertItemPicking, parametersInsertItemPicking);

                                actItemPickComment = Convert.ToString(drow["ActivityItemPickingComment"]);

                                if (!string.IsNullOrEmpty(actItemPickComment))
                                {
                                    transaction.Execute("INSERT INTO tActivityItemPickingComment (ActivityItemRefno, ActivityItemPickingCommentComment) VALUES (?, ?)", actItemRefno, actItemPickComment);
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
        /// The CreateRequest.
        /// </summary>
        /// <param name="actPickFKRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected async override Task<string> CreateRequest(int actPickFKRefno, XDocument criteria)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            XDocument request;
            if (criteria == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER ITEMS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<refno>" + actPickFKRefno + "</refno>"
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
                    + "<refno>" + actPickFKRefno + "</refno>"
                    + "<whs_id>" + criteria.Root.Element("whs_id").Value + "</whs_id>"
                    + "<sect_refno>" + criteria.Root.Element("sect_refno").Value + "</sect_refno>"
                    + "</command>"
                    + "<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
