// <copyright file="TableActivityCycleCountItem.cs" company="InterSect Business Systems Inc">
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
    using Bedrock;
    using Resources.Models.Headers;
    using Resources.Models.LocationMap;
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
            try
            {
                // Corruption recovery cleanup.
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);

                var queries = new List<string>
                {
                    @"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0",
                    @"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)",
                    @"DELETE FROM tActivityItemCount
                        WHERE tActivityItemCount.ActivityItemRefno <= 0
                            OR tActivityItemCount.ActivityItemFKRefno <= 0",
                    @"DELETE FROM tActivityItemCount
                        WHERE tActivityItemCount.ActivityItemRefno
                            NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)",
                    "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0",
                    @"DELETE FROM tActivityItem
                        WHERE tActivityItem.ActivityRefno
                            NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)",
                    @"DELETE FROM tActivityItemBarCodeAssociationXRef
                        WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN
                            (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem
                                WHERE tActivityItem.ActivityItemSubType = ?)",
                    "DELETE FROM tActivityItemCount WHERE tActivityItemCount.ActivityItemSubType = ?",
                    "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?"
                };

                foreach (var query in queries)
                {
                    await conn.ExecuteAsync(query, this.CurrentSubType);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} items complete.",
                    true);
            }
            catch (SQLiteException ex)
            {
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
                    $"Database exception occurred. Purging {this.LowerCaseName} items failed.",
                    ex);
            }
            catch
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// Sends a request based on the specified activity foreign key reference number.
        /// </summary>
        /// <remarks>This method retrieves data from the database using the provided activity foreign key
        /// reference number. If a matching record is found, it creates and sends a communication request based on the
        /// retrieved data.</remarks>
        /// <param name="activityFkRefno">The foreign key reference number of the activity item. Must be a valid reference number.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string from the
        /// communication request.</returns>
        /// <exception cref="Exception">Thrown if no records are found for the specified <paramref name="activityFkRefno"/> in the database.</exception>
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

            return await SendComm(await this.CreateRequest(actCountFkRefno));
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int actRefno)
        {
            try
            {
                this.StatusOut("Reading received data.");
                using var ds = new DataSet();
                ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                this.StatusOut("Saving cycle count items.");
                var conn = DatabaseConnection.GetInstance().SqlConn;
                if (ds.Tables.Count > 0)
                {
                    var rows = ds.Tables["tActivityItemCount"].Rows;
                    if (rows.Count > 0)
                    {
                        var countItems = new List<Seg>();
                        foreach (DataRow drow in rows)
                        {
                            var itemRefno = await GetActivityItemRefno();
                            var seg = new Seg
                            {
                                ActivityItemRefno = itemRefno,
                                ActivityItemSubType = Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                ActivityItemFKRefno = Convert.ToInt32(drow["f02"]),
                                ActivityItemStringFKRefno = Convert.ToString(drow["f02"]),
                                LocationMapRefno = await GetReceivedDataLocationAsync(
                                    drow,
                                    conn,
                                    true,
                                    SysCommon.ActSubTypes.CycleCounts),
                                ProductUnitRefno = await GetProductUnitRefnoFromDataRowAsync(drow, conn, true),
                                ActivityItemCountQuantityOnHand = Convert.ToDecimal(drow["f09"]),
                                ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(drow["f10"]),
                                ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(drow["f11"]),
                                ActivityItemCountQuantity = Convert.ToDecimal(drow["f12"]),
                                ActivityItemCountBaseQuantity = Convert.ToDecimal(drow["f13"]),
                                ActivityItemCountWeightQuantity = Convert.ToDecimal(drow["f14"]),
                            };
                            countItems.Add(seg);
                        }

                        var query = "CREATE TEMP TABLE Seg " +
                            "(" +
                            "ActivityItemRefno INTEGER NOT NULL," +
                            "ActivityItemSubType INTEGER NOT NULL," +
                            "ActivityItemFKRefno INTEGER NOT NULL," +
                            "ActivityItemStringFKRefno VARCHAR(10) NOT NULL," +
                            "ProductUnitRefno INTEGER NOT NULL," +
                            "LocationMapRefno INTEGER NOT NULL," +
                            "ActivityItemCountQuantityOnHand DECIMAL(11, 3) NOT NULL," +
                            "ActivityItemCountBaseQuantityOnHand DECIMAL(11, 3) NOT NULL," +
                            "ActivityItemCountWeightQuantityOnHand DECIMAL(11, 3) NOT NULL," +
                            "ActivityItemCountQuantity DECIMAL(11, 3) NOT NULL," +
                            "ActivityItemCountBaseQuantity DECIMAL(11, 3) NOT NULL," +
                            "ActivityItemCountWeightQuantity DECIMAL(11, 3) NOT NULL" +
                            ")";
                        await conn.ExecuteAsync(query);
                        await conn.InsertAllAsync(countItems);

                        query = @"
                    INSERT INTO tActivityItemCount (
                        ActivityItemRefno, 
                        ActivityItemSubType, 
                        ActivityItemFKRefno, 
                        ActivityItemStringFKRefno, 
                        ProductUnitRefno, 
                        LocationMapRefno, 
                        ActivityItemCountQuantityOnHand, 
                        ActivityItemCountBaseQuantityOnHand, 
                        ActivityItemCountWeightQuantityOnHand, 
                        ActivityItemCountQuantity, 
                        ActivityItemCountBaseQuantity, 
                        ActivityItemCountWeightQuantity
                        ) 
                    SELECT 
                        Seg.ActivityItemRefno, 
                        Seg.ActivityItemSubType, 
                        Seg.ActivityItemFKRefno, 
                        Seg.ActivityItemStringFKRefno, 
                        Seg.ProductUnitRefno, 
                        Seg.LocationMapRefno, 
                        Seg.ActivityItemCountQuantityOnHand, 
                        Seg.ActivityItemCountBaseQuantityOnHand, 
                        Seg.ActivityItemCountWeightQuantityOnHand, 
                        Seg.ActivityItemCountQuantity, 
                        Seg.ActivityItemCountBaseQuantity, 
                        Seg.ActivityItemCountWeightQuantity
                    FROM Seg";
                        var rowsAffected = await conn.ExecuteAsync(query);
                        if (countItems.Count != rowsAffected)
                        {
                            throw new Exception("Failed to insert all rows into tActivityItemCount");
                        }

                        // Drop the temporary table
                        const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                        await conn.ExecuteAsync(dropTempTableQuery);

                        this.StatusOut("Request cycle count items complete.");
                    }
                }

                ///////////////////////////////////////////////////////////
                async Task<int> GetActivityItemRefno()
                {
                    await conn.ExecuteAsync(
                            @"INSERT INTO tActivityItem
                            (ActivityItemSubType, ActivityItemStatus, ActivityRefno, ActivityItemIsAdded)
                            VALUES (?, ?, ?, ?)",
                            Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                            Convert.ToInt32(SysCommon.ActStatus.NoChange),
                            actRefno,
                            0);

                    var actItemRefno = Convert.ToInt32(
                        await conn.ExecuteScalarAsync<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                    return actItemRefno;
                }
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Request physical count items failed.",
                    true);

                throw new Exception(
                    "Database exception occurred. Saving cycle count items failed.",
                    ex);
            }
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

        /// <summary>
        /// Seg class.
        /// </summary>
        private class Seg
        {
            /// <summary>
            ///     Gets or sets the activity item reference number.
            /// </summary>
            public int ActivityItemRefno { get; set; }

            /// <summary>
            ///   Gets or sets the activity item subtype.
            /// </summary>
            public int ActivityItemSubType { get; set; }

            /// <summary>
            ///    Gets or sets the activity item foreign key reference number.
            /// </summary>
            public int ActivityItemFKRefno { get; set; }

            /// <summary>
            ///   Gets or sets the activity item foreign key reference number as a string.
            /// </summary>
            public string ActivityItemStringFKRefno { get; set; }

            /// <summary>
            ///    Gets or sets the location map reference number.
            /// </summary>
            public int LocationMapRefno { get; set; }

            /// <summary>
            ///    Gets or sets the product unit reference number.
            /// </summary>
            public int ProductUnitRefno { get; set; }

            /// <summary>
            ///   Gets or sets the activity item count quantity on hand.
            /// </summary>
            public decimal ActivityItemCountQuantityOnHand { get; set; }

            /// <summary>
            ///  Gets or sets the activity item count base quantity on hand.
            /// </summary>
            public decimal ActivityItemCountBaseQuantityOnHand { get; set; }

            /// <summary>
            /// Gets or sets the activity item count weight quantity on hand.
            /// </summary>
            public decimal ActivityItemCountWeightQuantityOnHand { get; set; }

            /// <summary>
            /// Gets or sets the activity item count quantity.
            /// </summary>
            public decimal ActivityItemCountQuantity { get; set; }

            /// <summary>
            /// Gets or sets the activity item count base quantity.
            /// </summary>
            public decimal ActivityItemCountBaseQuantity { get; set; }

            /// <summary>
            /// Gets or sets the activity item count weight quantity.
            /// </summary>
            public decimal ActivityItemCountWeightQuantity { get; set; }
        }
    }
}