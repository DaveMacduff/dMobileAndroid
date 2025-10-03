// <copyright file="TableActivityPicking.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
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

    /// <summary>
    ///     Defines the <see cref="TableActivityPicking" />.
    /// </summary>
    public class TableActivityPicking : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityPicking" /> class.
        /// </summary>
        public TableActivityPicking()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales orders";
            this.ProperCaseName = "Sales orders";
        }

        /// <summary>
        ///     Purge the activity header data.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);
                    var query = @"DELETE FROM tActivityPickingRoute
                    WHERE tActivityPickingRoute.PickingActivityRefno <= 0
                        OR tActivityPickingRoute.RouteRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityPickingRoute
                    WHERE tActivityPickingRoute.PickingActivityRefno
                        NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityPickingRoute
                    WHERE tActivityPickingRoute.RouteRefno
                        NOT IN (SELECT tRoute.RouteRefno FROM tRoute)";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityPicking
                    WHERE tActivityPicking.ActivityRefno <= 0
                        OR tActivityPicking.ActivityFKRefno <= 0";
                await Conn.ExecuteAsync(query);
                query = @"DELETE FROM tActivityPicking
                    WHERE tActivityPicking.ActivityRefno
                        NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                await Conn.ExecuteAsync(query);
                query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = @"DELETE FROM tActivityPickingRoute
                    WHERE PickingActivityRefno IN
                        (SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?)";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = "DELETE FROM tActivityPicking WHERE ActivitySubType = ?";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await Conn.ExecuteAsync(query, this.CurrentSubType);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} complete.",
                    true);
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName} failed.",
                    true);
                throw;
            }
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    try
                    {
                        var orders = new List<Seg>();
                        foreach (DataRow row in rows)
                        {
                            var refno = await GetActivityRefno();
                            var order = new Seg
                            {
                                ActivityRefno = refno,
                                ActivitySubType = Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                ActivityFKRefno = Convert.ToInt32(row["ActivityFKRefno"]),
                                ActivityStringFKRefno = Convert.ToString(row["ActivityFKRefno"]),
                                LocationMapWarehouseFKRefno =
                                    Convert.ToInt32(row["LocationMapWarehouseFKRefno"]),
                                ActivityPickingOrderedDateTime =
                                    Convert.ToDateTime(row["ActivityPickingOrderedDateTime"]),
                                ActivityPickingDeliveryDateTime =
                                    Convert.ToDateTime(row["ActivityPickingDeliveryDateTime"]),
                                ActivityPickingCustomerContactName =
                                    Convert.ToString(row["ActivityPickingCustomerContactName"]),
                                ActivityPickingCustomerShipToInfo =
                                    Convert.ToString(row["ActivityPickingCustomerShipToInfo"]),
                                ActivityPickingIsReviewed = false,
                                ActivityPickingRouteCode =
                                    Convert.ToString(row["ActivityPickingRouteCode"]),
                                ActivityPickingRouteStop =
                                    Convert.ToInt32(row["ActivityPickingRouteStop"])
                            };
                            orders.Add(order);
                        }

                        const string query = "CREATE TEMP TABLE Seg" +
                                             "(" +
                                             "ActivityRefno INTEGER NOT NULL," +
                                             "ActivitySubType INTEGER NOT NULL," +
                                             "ActivityFKRefno INTEGER NOT NULL," +
                                             "ActivityStringFKRefno VARCHAR(10) NOT NULL," +
                                             "LocationMapWarehouseFKRefno INTEGER NOT NULL," +
                                             "ActivityPickingOrderedDateTime DATETIME NOT NULL," +
                                             "ActivityPickingDeliveryDateTime DATETIME NOT NULL," +
                                             "ActivityPickingCustomerContactName VARCHAR(40) NOT NULL," +
                                             "ActivityPickingCustomerShipToInfo VARCHAR(40) NOT NULL," +
                                             "ActivityPickingIsReviewed BIT NOT NULL," +
                                             "ActivityPickingRouteCode VARCHAR(5) NOT NULL," +
                                             "ActivityPickingRouteStop INTEGER NOT NULL" +
                                             ")";

                        await Conn.ExecuteAsync(query);
                        await Conn.InsertAllAsync(orders);

                        const string query1 = "INSERT INTO tActivityPicking " +
                                              "(" +
                                              "ActivityRefno," +
                                              "ActivitySubType," +
                                              "ActivityFKRefno," +
                                              "ActivityStringFKRefno," +
                                              "WarehouseLocationMapRefno," +
                                              "ActivityPickingOrderedDateTime," +
                                              "ActivityPickingDeliveryDateTime," +
                                              "ActivityPickingCustomerContactName," +
                                              "ActivityPickingCustomerShipToInfo," +
                                              "ActivityPickingIsReviewed" +
                                              ")" +
                                              "SELECT Seg.ActivityRefno," +
                                              "Seg.ActivitySubType," +
                                              "Seg.ActivityFKRefno," +
                                              "Seg.ActivityStringFKRefno," +
                                              "whs.LocationMapRefno as WarehouseLocationMapRefno," +
                                              "Seg.ActivityPickingOrderedDateTime," +
                                              "Seg.ActivityPickingDeliveryDateTime," +
                                              "Seg.ActivityPickingCustomerContactName," +
                                              "Seg.ActivityPickingCustomerShipToInfo," +
                                              "Seg.ActivityPickingIsReviewed" +
                                              " FROM Seg" +
                                              " INNER JOIN tLocationMapWarehouse whs ON " +
                                              "whs.LocationMapWarehouseFKRefno = Seg.LocationMapWarehouseFKRefno";

                        const string query2 = "INSERT INTO tActivityPickingRoute " +
                                              "(" +
                                              "PickingActivityRefno," +
                                              "RouteRefno," +
                                              "RouteStop" +
                                              ")" +
                                              "SELECT Seg.ActivityRefno," +
                                              "tRoute.RouteRefno," +
                                              "Seg.ActivityPickingRouteStop" +
                                              " FROM Seg" +
                                              " INNER JOIN tRoute ON tRoute.RouteCode = Seg.ActivityPickingRouteCode";

                        await Task.WhenAll(Conn.ExecuteAsync(query1), Conn.ExecuteAsync(query2));
                    }
                    finally
                    {
                        // Drop the temporary table
                        const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                        await Conn.ExecuteAsync(dropTempTableQuery);
                    }
                }
            }

            return;

            async Task<int> GetActivityRefno()
            {
                var query = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)";
                await Conn.ExecuteAsync(query, 0, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "SELECT last_insert_rowid()";
                ////query = "SELECT tActivity.ActivityRefno FROM tActivity" +
                ////        $" WHERE tActivity.ActivitySubType = {Convert.ToInt32(SysCommon.ActSubTypes.Picking)}" +
                ////        " ORDER BY tActivity.ActivityRefno DESC LIMIT 1";
                return Convert.ToInt32(await Conn.ExecuteScalarAsync<int>(query));
            }
        }

        /// <summary>
        ///     The CreateRequest.
        /// </summary>
        /// <param name="criteriaFilter"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(XDocument criteriaFilter)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (criteriaFilter == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDERS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<whs_id></whs_id>" +
                    "<sect_refno></sect_refno>" +
                    "</command>" +
                    "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST SALES ORDERS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<whs_id>" +
                    criteriaFilter.Root?.Element("whs_id")?.Value +
                    "</whs_id>" +
                    "<sect_refno>" +
                    criteriaFilter.Root?.Element("sect_refno")?.Value +
                    "</sect_refno>" +
                    "</command>" +
                    "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <inheritdoc/>
        protected override async Task<string> CreateRequest()
        {
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings
                {
                    Async = false,
                    Indent = false,
                    OmitXmlDeclaration = false
                }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                writer.WriteElementString("action", "REQUEST SALES ORDERS");

                // Insert request defaults directly as XML
                var requestDefaults = await SysCommon.GetRequestDefaults(false);
                if (!string.IsNullOrEmpty(requestDefaults))
                {
                    var defaultsDoc = XDocument.Parse("<root>" + requestDefaults + "</root>");
                    foreach (var elem in defaultsDoc.Root.Elements())
                    {
                        elem.WriteTo(writer);
                    }
                }

                writer.WriteElementString("whs_id", string.Empty);
                writer.WriteElementString("sect_refno", string.Empty);
                writer.WriteEndElement(); // command
                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            /// <summary>
            ///     Gets or sets the activity reference identifier.
            /// </summary>
            public int ActivityRefno { get; set; }

            /// <summary>
            ///    Gets or sets the activity subtype.
            /// </summary>
            public int ActivitySubType { get; set; }

            /// <summary>
            ///    Gets or sets the activity foreign key reference number.
            /// </summary>
            public int ActivityFKRefno { get; set; }

            /// <summary>
            ///   Gets or sets the activity string foreign key reference number.
            /// </summary>
            public string ActivityStringFKRefno { get; set; }

            /// <summary>
            ///   Gets or sets the location map warehouse foreign key reference number.
            /// </summary>
            public int LocationMapWarehouseFKRefno { get; set; }

            /// <summary>
            ///  Gets or sets the activity picking ordered date time.
            /// </summary>
            public DateTime ActivityPickingOrderedDateTime { get; set; }

            /// <summary>
            ///     Gets or sets the activity picking delivery date time.
            /// </summary>
            public DateTime ActivityPickingDeliveryDateTime { get; set; }

            /// <summary>
            ///     Gets or sets the activity picking customer contact name.
            /// </summary>
            public string ActivityPickingCustomerContactName { get; set; }

            /// <summary>
            ///     Gets or sets the activity picking customer ship to information.
            /// </summary>
            public string ActivityPickingCustomerShipToInfo { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether the activity picking is reviewed.
            /// </summary>
            public bool ActivityPickingIsReviewed { get; set; }

            /// <summary>
            ///    Gets or sets the activity picking route code.
            /// </summary>
            public string ActivityPickingRouteCode { get; set; }

            /// <summary>
            ///   Gets or sets the activity picking route stop.
            /// </summary>
            public int ActivityPickingRouteStop { get; set; }
        }
    }
}