// <copyright file="clsTableActivityPhysicalCountItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityPhysicalCountItem" />.
    /// </summary>
    public class TableActivityPhysicalCountItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPhysicalCountItem"/> class.
        /// </summary>
        public TableActivityPhysicalCountItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCountItem";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actRefno"></param>
        public override async Task RequestData(int actRefno)
        {

            // GC.Collect();
            // GC.WaitForPendingFinalizers();
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    string query = "SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?";
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                    if (results != null && results.Count > 0)
                    {
                        actRefno = results[0].ActivityRefno;
                        reqData = await this.SendRequest(actRefno);

                        if (reqData.Length > 0)
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
                else
                {
                    reqData = await this.SendRequest(actRefno);

                    if (reqData.Length > 0)
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items failed.", false);

                throw new Exception("Database exception occurred. Seeding physical count items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical count items failed.", false);

                throw new Exception("Seeding physical count items failed.", ex);
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
                string query = "SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} physical count items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Physical count items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items.", true);

                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                    + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                    + "(SELECT tActivityItem.ActivityItemRefno "
                    + "FROM tActivityItem "
                    + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                query = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items failed.", true);

                throw new Exception("Database exception occurred. Purging physical count items failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actRefno)
        {
            int actCountFKRefno = 0;

            string query = "SELECT * FROM tActivityCount WHERE ActivityRefno = ?";
            List<tActivityCountModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityCountModel>(query, actRefno);

            if (results?.Count > 0)
            {
                actCountFKRefno = results[0].ActivityFKRefno;
            }

            return await this.SendComm(await this.CreateRequest(actCountFKRefno));
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
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemCount"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int locMapSectFKRefno = 0;
                            int locMapSlotFKRefno = 0;
                            int countItemFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemCount"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, true);

                                // Cannot use this.SaveDetailLocation() f00 tags are not the same.
                                locMapRefno = 0;

                                if (locMapRefno == 0)
                                {
                                    locMapSlotFKRefno = Convert.ToInt32(drow["f08"]);

                                    List<tLocationMapSlotModel> locationMapSlotResults = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapSlotFKRefno);

                                    if (locationMapSlotResults != null && locationMapSlotResults.Count > 0)
                                    {
                                        locMapRefno = locationMapSlotResults[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapSectFKRefno = Convert.ToInt32(drow["f07"]);

                                    List<tLocationMapSectionModel> locationMapSectionResults = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapSectFKRefno);

                                    if (locationMapSectionResults != null && locationMapSectionResults.Count > 0)
                                    {
                                        locMapRefno = locationMapSectionResults[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["f06"]);

                                    List<tLocationMapWarehouseModel> locationMapWarehouseResults = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                    if (locationMapWarehouseResults != null && locationMapWarehouseResults.Count > 0)
                                    {
                                        locMapRefno = locationMapWarehouseResults[0].LocationMapRefno;
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    throw new Exception(string.Format("Unable to indentify the location map reference number.{0} Whs FK Refno {1}{2} Section FK Refno {3}{4} Slot FK Refno {5}",
                                        Environment.NewLine,
                                        locMapWhsFKRefno,
                                        Environment.NewLine,
                                        locMapSectFKRefno,
                                        Environment.NewLine,
                                        locMapSlotFKRefno));
                                }

                                countItemFKRefno = Convert.ToInt32(drow["f02"]);

                                transaction.Execute(
                                    "INSERT INTO tActivityItemCount (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemCountFKRefno, " +
                                    "ActivityItemCountStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "LocationMapRefno, " +
                                    "ActivityItemCountQuantityOnHand, " +
                                    "ActivityItemCountBaseQuantityOnHand, " +
                                    "ActivityItemCountWeightQuantityOnHand, " +
                                    "ActivityItemCountQuantity, " +
                                    "ActivityItemCountBaseQuantity, " +
                                    "ActivityItemCountWeightQuantity" +
                                    ") VALUES (" +
                                    "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                    actItemRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts),
                                    countItemFKRefno,
                                    Convert.ToString(countItemFKRefno),
                                    prodUnitRefno,
                                    locMapRefno,
                                    Convert.ToDecimal(drow["f09"]),
                                    Convert.ToDecimal(drow["f10"]),
                                    Convert.ToDecimal(drow["f11"]),
                                    Convert.ToDecimal(drow["f12"]),
                                    Convert.ToDecimal(drow["f13"]),
                                    Convert.ToDecimal(drow["f14"]));

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request physical count items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Request physical count items failed.", true);

                    throw new Exception("Database exception occurred. Saving physical count items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="actCountFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int actCountFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PHYSICAL COUNT ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + actCountFKRefno.ToString() + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
