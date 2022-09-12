// <copyright file="TableActivityReturnsItem.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityReturnsItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReturnsItem"/> class.
        /// </summary>
        public TableActivityReturnsItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData(int actRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                    if (results?.Count > 0)
                    {
                        foreach (var activityModel in results)
                        {
                            actRefno = Convert.ToInt32(activityModel.ActivityRefno);
                            reqData = await this.SendRequest(actRefno);

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
                    reqData = await this.SendRequest(actRefno);

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                throw new Exception("Database exception occurred. Seeding sales return items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return items failed.", true);

                throw new Exception("Seeding sales return items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items.", true);

            try
            {
                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityItemReturnsComment";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                query = "DELETE FROM tActivityItemReturns WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return items failed.", true);

                throw new Exception("Database exception occurred. Purging sales return items failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales return items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sales return items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> SendRequest(int actRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityReturnsModel>("SELECT * FROM tActivityReturns WHERE ActivityRefno = ?", Convert.ToInt32(actRefno));

            int returnsFKRefno;
            if (results?.Count > 0)
            {
                returnsFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drReturns.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(returnsFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityItemReturns"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actItemRefno = 0;
                            var progValue = 0;
                            var prodUnitRefno = 0;
                            var locMapRefno = 0;
                            var actItemReturnsFKRefno = 0;

                            var actItemRetComment = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemReturns"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Returns), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);
                                locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);
                                actItemReturnsFKRefno = Convert.ToInt32(drow["ActivityItemFKRefno"]);
                                var queryInsertItem = "INSERT INTO tActivityItemReturns (" +
                                                      "ActivityItemRefno, " +
                                                      "ActivityItemSubType, " +
                                                      "ActivityItemFKRefno, " +
                                                      "ActivityItemStringFKRefno, " +
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
                                var parametersInsertItem = new object[]
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
                                    Convert.ToString(drow["ActivityItemReturnsDescription"]),
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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return items failed.", false);

                    throw new Exception("Database exception occurred. Saving sales return items failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="returnsFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int returnsFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES RETURN ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + returnsFKRefno + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
