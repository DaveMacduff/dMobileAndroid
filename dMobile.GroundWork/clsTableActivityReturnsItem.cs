namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    public class TableActivityReturnsItem : TableBase
    {
        public TableActivityReturnsItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Request(int actRefno)
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items.", true);

                if (actRefno == 0)
                {
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns));

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding sales return items failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding sales return items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items.", true);

            try
            {
                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                    + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityItemReturnsComment";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                query = "DELETE FROM tActivityItemReturns WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales return items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales return items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Sales return items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> Send(int actRefno)
        {
            int returnsFKRefno = 0;
            List<tActivityReturnsModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityReturnsModel>("SELECT * FROM tActivityReturns WHERE ActivityRefno = ?", Convert.ToInt32(actRefno));

            if (results?.Count > 0)
            {
                returnsFKRefno = results[0].ActivityReturnsFKRefno;
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drReturns.Read() failed.");
            }

            return await this.SendComm(this.Create(returnsFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemReturns"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int actItemReturnsFKRefno = 0;

                            string actItemRetComment = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemReturns"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Returns), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, ref errMsg, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                actItemReturnsFKRefno = Convert.ToInt32(drow["ActivityItemReturnsFKRefno"]);
                                errMsg += Environment.NewLine + "Item fk refno " + actItemReturnsFKRefno;

                                string queryInsertItem = "INSERT INTO tActivityItemReturns (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemReturnsFKRefno, " +
                                    "ActivityItemReturnsStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityItemReturnsQuantityReturned, " +
                                    "ActivityItemReturnsBaseQuantityReturned, " +
                                    "ActivityItemReturnsWeightQuantityReturned, " +
                                    "ActivityItemReturnsCustomerProductUnitCode, " +
                                    "ActivityItemReturnsDescription" +
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
                                List<object> parametersInsertItem = new List<object>()
                                {
                                    Convert.ToInt32(actItemRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.Returns),
                                    Convert.ToInt32(actItemReturnsFKRefno),
                                    Convert.ToString(actItemReturnsFKRefno),
                                    Convert.ToInt32(prodUnitRefno),
                                    Convert.ToInt32(locMapRefno),
                                    Convert.ToDecimal(drow["ActivityItemReturnsQuantityReturned"]),
                                    Convert.ToDecimal(drow["ActivityItemReturnsBaseQuantityReturned"]),
                                    Convert.ToDecimal(drow["ActivityItemReturnsWeightQuantityReturned"]),
                                    Convert.ToString(drow["ActivityItemReturnsCustomerProductUnitCode"]),
                                    Convert.ToString(drow["ActivityItemReturnsDescription"])
                                };

                                transaction.Execute(queryInsertItem, parametersInsertItem);

                                actItemRetComment = Convert.ToString(drow["ActivityItemReturnsComment"]);

                                if (!string.IsNullOrEmpty(actItemRetComment))
                                {
                                    transaction.Execute("INSERT INTO tActivityItemReturnsComment (ActivityItemRefno, ActivityItemReturnsCommentComment) VALUES (?, ?)", Convert.ToInt32(actItemRefno), Convert.ToString(actItemRetComment));
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    transaction.Commit();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving sales return items failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnsFKRefno"></param>
        /// <returns></returns>
        protected override string Create(int returnsFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES RETURN ITEMS</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + returnsFKRefno + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
