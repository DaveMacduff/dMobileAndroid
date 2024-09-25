// <copyright file="TableActivityCycleCountItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCycleCountItem" />.
    /// </summary>
    public class TableActivityCycleCountItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCountItem"/> class.
        /// </summary>
        public TableActivityCycleCountItem()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountItem";
            this.CurrentSubType = SysCommon.ActSubTypes.CycleCounts;
            this.LowerCaseName = "cycle count";
            this.ProperCaseName = "Cycle count";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging existing {this.LowerCaseName} items.",
                true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(
                transaction =>
                {
                    try
                    {
                        // Corruption recovery cleanup.
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName}.",
                            true);
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
                        DELETE FROM tActivityItemCount
                            WHERE tActivityItemCount.ActivityItemRefno <= 0
                                OR tActivityItemCount.ActivityItemFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItemCount
                            WHERE tActivityItemCount.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query);

                        query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                        transaction.Execute(query);

                        // Regular purge.
                        query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem
                                    WHERE tActivityItem.ActivityItemSubType = ?)";
                        transaction.Execute(query, this.CurrentSubType);

                        query =
                            "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        query =
                            "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                        transaction.Execute(query, this.CurrentSubType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items complete.",
                            true);
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Database exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items failed.",
                            true);
                        throw new Exception(
                            $"Database exception occurred.Purging {this.LowerCaseName} items failed.",
                            ex);
                    }
                    catch
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} items failed.",
                            true);
                        throw;
                    }
                });
        }

        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityCountModel>(
                    "SELECT * FROM tActivityCount WHERE ActivityFKRefno = ?",
                    activityFkRefno);

            int actCountFkRefno;
            if (results != null &&
                results.Count > 0)
            {
                actCountFkRefno = Convert.ToInt32(results[0].ActivityFKRefno);
            }
            else
            {
                throw new Exception(
                    "No results in tActivityCount for ActivityRefno " + activityFkRefno);
            }

            return await this.SendComm(await this.CreateRequest(actCountFkRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actRefno">The actItemRefno<see cref="int"/>.</param>
        protected override async Task Save(string responseData, int actRefno)
        {
            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using var ds = new DataSet();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received data.",
                                false);

                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle count items.",
                                false);

                            // Receiving schema but no data results in no table being created in the dataset.
                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityItemCount"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var progValue = 0;
                                var locMapWhsFkRefno = 0;
                                var locMapSectFkRefno = 0;
                                var locMapSlotFkRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                foreach (DataRow drow in ds.Tables["tActivityItemCount"].Rows)
                                {
                                    transaction.Execute(
                                        "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno," +
                                        " ActivityItemIsAdded) VALUES (?, ?, ?, ?)",
                                        Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                        actRefno,
                                        0);

                                    var actItemRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityItem"));
                                    var prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                        drow,
                                        transaction,
                                        true);

                                    // Cannot use this.SaveDetailLocation() f00 tags are not the same.
                                    var locMapRefno = 0;
                                    if (locMapRefno == 0)
                                    {
                                        locMapSlotFkRefno = Convert.ToInt32(drow["f08"]);
                                        var resultSlots = transaction.Query<tLocationMapSlotModel>(
                                            "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                                            locMapSlotFkRefno);

                                        if (resultSlots != null &&
                                            resultSlots.Count > 0)
                                        {
                                            locMapRefno = Convert.ToInt32(
                                                resultSlots[0].LocationMapRefno);
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        locMapSectFkRefno = Convert.ToInt32(drow["f07"]);
                                        var resultSections =
                                            transaction.Query<tLocationMapSectionModel>(
                                                "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                                                locMapSectFkRefno);

                                        if (resultSections != null &&
                                            resultSections.Count > 0)
                                        {
                                            locMapRefno = Convert.ToInt32(
                                                resultSections[0].LocationMapRefno);
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        locMapWhsFkRefno = Convert.ToInt32(drow["f06"]);
                                        var resultWarehouses =
                                            transaction.Query<tLocationMapWarehouseModel>(
                                                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                                                locMapWhsFkRefno);

                                        if (resultWarehouses != null &&
                                            resultWarehouses.Count > 0)
                                        {
                                            locMapRefno = Convert.ToInt32(
                                                resultWarehouses[0].LocationMapRefno);
                                        }
                                    }

                                    if (locMapRefno == 0)
                                    {
                                        throw new Exception(
                                            $"Unable to identify the location map reference number." +
                                            $"{Environment.NewLine}Whs FK Refno {locMapWhsFkRefno}" +
                                            $"{Environment.NewLine} Section FK Refno {locMapSectFkRefno}" +
                                            $"{Environment.NewLine} Slot FK Refno {locMapSlotFkRefno}");
                                    }

                                    var countItemFkRefno = Convert.ToInt32(drow["f02"]);
                                    const string query = "INSERT INTO tActivityItemCount (" +
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

                                    var parameters = new object[]
                                    {
                                        actItemRefno,
                                        Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                        countItemFkRefno,
                                        Convert.ToString(countItemFkRefno),
                                        prodUnitRefno,
                                        locMapRefno,
                                        Convert.ToDecimal(drow["f09"]),
                                        Convert.ToDecimal(drow["f10"]),
                                        Convert.ToDecimal(drow["f11"]),
                                        Convert.ToDecimal(drow["f12"]),
                                        Convert.ToDecimal(drow["f13"]),
                                        Convert.ToDecimal(drow["f14"])
                                    };

                                    transaction.Execute(query, parameters);

                                    progValue++;
                                    if (progValue % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    progMax,
                                    0,
                                    progMax);
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle count items complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle count items failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving cycle count items failed.",
                                ex);
                        }
                    });
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
                "<action>REQUEST CYCLE COUNT ITEMS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<refno>" +
                activityFkRefno.ToString() +
                "</refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
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

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}