// <copyright file="TableActivityPhysicalCountItem.cs" company="InterSect Business Systems Inc">
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
                    var query = "SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?";
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityModel>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                    if (results != null && results.Count > 0)
                    {
                        actRefno = results[0].ActivityRefno;
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
                var query = "SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} physical count items.", true);
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

                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
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
            var actCountFKRefno = 0;

            var query = "SELECT * FROM tActivityCount WHERE ActivityRefno = ?";
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityCountModel>(query, actRefno);

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
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count items.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityItemCount"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);
                        var actItemRefno = 0;
                        var progValue = 0;
                        var prodUnitRefno = 0;
                        var locMapRefno = 0;
                        var locMapWhsFKRefno = 0;
                        var locMapSectFKRefno = 0;
                        var locMapSlotFKRefno = 0;
                        var countItemFKRefno = 0;

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

                                var locationMapSlotResults = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapSlotFKRefno);

                                if (locationMapSlotResults != null && locationMapSlotResults.Count > 0)
                                {
                                    locMapRefno = locationMapSlotResults[0].LocationMapRefno;
                                }
                            }

                            if (locMapRefno == 0)
                            {
                                locMapSectFKRefno = Convert.ToInt32(drow["f07"]);

                                var locationMapSectionResults = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapSectFKRefno);

                                if (locationMapSectionResults != null && locationMapSectionResults.Count > 0)
                                {
                                    locMapRefno = locationMapSectionResults[0].LocationMapRefno;
                                }
                            }

                            if (locMapRefno == 0)
                            {
                                locMapWhsFKRefno = Convert.ToInt32(drow["f06"]);

                                var locationMapWarehouseResults = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                if (locationMapWarehouseResults != null && locationMapWarehouseResults.Count > 0)
                                {
                                    locMapRefno = locationMapWarehouseResults[0].LocationMapRefno;
                                }
                            }

                            if (locMapRefno == 0)
                            {
                                throw new Exception(
                                    $"Unable to indentify the location map reference number.{Environment.NewLine} Whs FK Refno {locMapWhsFKRefno}{Environment.NewLine} Section FK Refno {locMapSectFKRefno}{Environment.NewLine} Slot FK Refno {locMapSlotFKRefno}");
                            }

                            countItemFKRefno = Convert.ToInt32(drow["f02"]);

                            transaction.Execute(
                                "INSERT INTO tActivityItemCount (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemFKRefno, " +
                                "ActivityItemStringFKRefno, " +
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
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
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
