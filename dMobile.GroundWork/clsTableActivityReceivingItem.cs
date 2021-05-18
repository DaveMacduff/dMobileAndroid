namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="TableActivityRecevingItem" />
    /// </summary>
    public class TableActivityRecevingItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityRecevingItem"/> class.
        /// </summary>
        public TableActivityRecevingItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItem";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        public override async Task Request(int actRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items.", true);

                if (actRefno == 0)
                {
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT * FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityModel activityModel in results)
                        {
                            actRefno = Convert.ToInt32(activityModel.ActivityRefno);
                            reqData = await this.Send(actRefno);

                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
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
                    reqData = await this.Send(actRefno);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding receiving items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding receiving items failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receiving items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Receiving items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items.", true);

                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                    + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivityItemReceiving WHERE tActivityItemReceiving.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receiving items failed.", ex);
            }
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        /// <returns></returns>
        protected override async Task<string> Send(int actRefno)
        {
            int recvFKRefno = 0;

            List<tActivityReceivingModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityReceivingModel>("SELECT * FROM tActivityReceiving WHERE ActivityRefno = ?", actRefno);

            if (results?.Count > 0)
            {
                recvFKRefno = results[0].ActivityReceivingFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drRecv.Read() failed.");
            }

            return await this.SendComm(await this.Create(recvFKRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/></param>
        protected override async Task Save(string reqData, int actRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrder"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            string recvItemComment = string.Empty;
                            int recvItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrder"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Receiving), 0, Convert.ToInt32(actRefno));

                                actItemRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                recvItemFKRefno = Convert.ToInt32(drow["ActivityItemPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + recvItemFKRefno;

                                string queryInsertActivityItemReceiving = "INSERT INTO tActivityItemReceiving (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemReceivingFKRefno, " +
                                    "ActivityItemReceivingStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemReceivingQuantityOrdered, " +
                                    "ActivityItemReceivingBaseQuantityOrdered, " +
                                    "ActivityItemReceivingWeightQuantityOrdered, " +
                                    "ActivityItemReceivingQuantityReceived, " +
                                    "ActivityItemReceivingBaseQuantityReceived, " +
                                    "ActivityItemReceivingWeightQuantityReceived, " +
                                    "ActivityItemReceivingVendorProductUnitCode, " +
                                    "ActivityItemReceivingDescription, " +
                                    "ActivityItemReceivingQuantityReceivedToDate, " +
                                    "ActivityItemReceivingBaseQuantityReceivedToDate, " +
                                    "ActivityItemReceivingWeightQuantityReceivedToDate" +
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
                                    "?" +
                                    ")";
                                List<object> parametersActivityItemReceiving = new List<object>()
                                {
                                    Convert.ToInt32(actItemRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                    Convert.ToInt32(recvItemFKRefno),
                                    Convert.ToString(recvItemFKRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(locMapRefno),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityOrdered"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceived"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceived"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceived"]),
                                    Convert.ToString(drow["ActivityItemPurchaseOrderVendorProductUnitCode"]),
                                    Convert.ToString(drow["ActivityItemPurchaseOrderDescription"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceivedToDate"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderBaseQuantityReceivedToDate"]),
                                    Convert.ToDecimal(drow["ActivityItemPurchaseOrderWeightQuantityReceivedToDate"])
                                };

                                transaction.Execute(queryInsertActivityItemReceiving, parametersActivityItemReceiving);

                                recvItemComment = Convert.ToString(drow["ActivityItemPurchaseOrderComment"]);

                                if (!string.IsNullOrEmpty(recvItemComment))
                                {
                                    transaction.Execute("INSERT INTO tActivityItemReceivingComment (ActivityItemRefno, ActivityItemReceivingCommentComment) VALUES (?, ?)", actItemRefno, recvItemComment);
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving receiving items failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="recvFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Create(int recvFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();
            string revision = new DateTime(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            string loginID = SysEnv.ContactLog.LoginID.ToString();

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER ITEMS</action>"
                + ("<device_id>" + deviceID + "</device_id>")
                + ("<login_id>" + loginID + "</login_id>")
                + ("<revision_datetime>" + revision + "</revision_datetime>")
                + ("<refno>" + recvFKRefno.ToString() + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
