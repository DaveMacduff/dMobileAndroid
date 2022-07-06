// <copyright file="clsTableActivityCycleCountItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCycleCountItem" />.
    /// </summary>
    public class TableActivityCycleCountItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCountItem"/> class.
        /// </summary>
        public TableActivityCycleCountItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountItem";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                    if (results != null && results.Count > 0)
                    {
                        foreach (tActivityModel activity in results)
                        {
                            actRefno = Convert.ToInt32(activity.ActivityRefno);
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items failed.", true);

                throw new Exception("Database exception occurred. Seeding cycle count items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle count items failed.", true);

                throw new Exception("Seeding cycle count items failed.", ex);
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
                string query = "SELECT COUNT(tActivityItem.ActivityItemRefno) FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} cycle count item(s)", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Cycle count items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items.", true);

            try
            {
                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                        + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                        + "(SELECT tActivityItem.ActivityItemRefno FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                query = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle count items failed.", true);

                throw new Exception("Database exception occurred.Purging cycle count items failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actRefno)
        {
            List<tActivityCountModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityCountModel>("SELECT * FROM tActivityCount WHERE ActivityRefno = ?", actRefno);

            int actCountFKRefno;
            if (results != null && results.Count > 0)
            {
                actCountFKRefno = Convert.ToInt32(results[0].ActivityFKRefno);
            }
            else
            {
                throw new Exception("No results in tActivityCount for ActivityRefno " + actRefno);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
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
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts), 0, actRefno);

                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, true);

                                // Cannot use this.SaveDetailLocation() f00 tags are not the same.
                                locMapRefno = 0;

                                if (locMapRefno == 0)
                                {
                                    locMapSlotFKRefno = Convert.ToInt32(drow["f08"]);
                                    List<tLocationMapSlotModel> resultSlots = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapSlotFKRefno);

                                    if (resultSlots != null && resultSlots.Count > 0)
                                    {
                                        locMapRefno = Convert.ToInt32(resultSlots[0].LocationMapRefno);
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapSectFKRefno = Convert.ToInt32(drow["f07"]);
                                    List<tLocationMapSectionModel> resultSections = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapSectFKRefno);

                                    if (resultSections != null && resultSections.Count > 0)
                                    {
                                        locMapRefno = Convert.ToInt32(resultSections[0].LocationMapRefno);
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    locMapWhsFKRefno = Convert.ToInt32(drow["f06"]);
                                    List<tLocationMapWarehouseModel> resultWarehouses = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                    if (resultWarehouses != null && resultWarehouses.Count > 0)
                                    {
                                        locMapRefno = Convert.ToInt32(resultWarehouses[0].LocationMapRefno);
                                    }
                                }

                                if (locMapRefno == 0)
                                {
                                    throw new Exception(string.Format("Unable to indentify the location map reference number.{0}Whs FK Refno {1}{2} Section FK Refno {3}{4} Slot FK Refno {5}",
                                        Environment.NewLine,
                                        locMapWhsFKRefno,
                                        Environment.NewLine,
                                        locMapSectFKRefno,
                                        Environment.NewLine,
                                        locMapSlotFKRefno));
                                }

                                countItemFKRefno = Convert.ToInt32(drow["f02"]);
                                string query = "INSERT INTO tActivityItemCount (" +
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

                                object[] parameters = new object[]
                                {
                                    actItemRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                    countItemFKRefno,
                                    Convert.ToString(countItemFKRefno),
                                    prodUnitRefno,
                                    locMapRefno,
                                    Convert.ToDecimal(drow["f09"]),
                                    Convert.ToDecimal(drow["f10"]),
                                    Convert.ToDecimal(drow["f11"]),
                                    Convert.ToDecimal(drow["f12"]),
                                    Convert.ToDecimal(drow["f13"]),
                                    Convert.ToDecimal(drow["f14"]),
                                };

                                transaction.Execute(query, parameters);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count items failed.", true);

                    throw new Exception("Database exception occurred. Saving cycle count items failed.", ex);
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
                + "<action>REQUEST CYCLE COUNT ITEMS</action>"
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
