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
            this.CurrentSubType = SysCommon.ActSubTypes.PhysicalCounts;
            this.LowerCaseName = "physical count";
            this.ProperCaseName = "Physical count";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical count items.", true);

                // Corruption recovery cleanup.
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                var query = "DELETE FROM tActivityItemBarCodeAssociationXRef WHERE" +
                            " tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                // Corruption recovery cleanup.
                query = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemRefno <= 0" +
                        " OR tActivityItemCount.ActivityItemFKRefno <= 0";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                // Corruption recovery cleanup.
                query = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemRefno " +
                        "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                // Corruption recovery cleanup.
                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                query = "DELETE FROM tActivityItemBarCodeAssociationXRef "
                            + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
                            + "(SELECT tActivityItem.ActivityItemRefno "
                            + "FROM tActivityItem "
                            + "WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                query = "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

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
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var actCountFKRefno = 0;

            var query = "SELECT * FROM tActivityCount WHERE ActivityRefno = ?";
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ActivityCountModel>(query, actItemRefno);

            if (results?.Count > 0)
            {
                actCountFKRefno = results[0].ActivityFKRefno;
            }

            return await this.SendComm(await this.CreateRequest(actCountFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int"/>.</param>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

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
                            transaction.Execute(
                                "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts),
                                Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                actRefno,
                                0);

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
        /// <param name="activityItemFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PHYSICAL COUNT ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + activityItemFkRefno.ToString() + "</refno>"
                + "</command>"
                + "</imclient>");

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

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
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

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
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
