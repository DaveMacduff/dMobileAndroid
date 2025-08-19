// <copyright file="TableWarehouseLocationMapType.cs" company="InterSect Business Systems Inc">
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
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableWarehouseLocationMapType" />.
    /// </summary>
    public class TableWarehouseLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableWarehouseLocationMapType"/> class.
        /// </summary>
        public TableWarehouseLocationMapType()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableWarehouseLocationMapType";
            this.LowerCaseName = "warehouse location map types";
            this.ProperCaseName = "Warehouse locations map types";
        }

        /// <summary>
        ///     Gets or sets the warehouse location map type name.
        /// </summary>
        private string WarehouseMapTypeName { get; set; }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName}.",
                    true);

                await RefreshWhsLocMapTypeFloor();
                await RefreshWhsLocMapTypePicking();
                await RefreshWhsLocMapTypeReceiving();
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);

                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
            }

            return;

            ///////////////////////////////////////////////////////////
            async Task RefreshWhsLocMapTypeFloor()
            {
                this.WarehouseMapTypeName = "Floor";
                var responseData = await SendComm(
                    await this.CreateRequestProdLoc(
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Floor)));
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var reqErr = new XmlDocument();
                    reqErr.LoadXml(responseData);
                    throw new Exception(reqErr.DocumentElement!["detail"]!.InnerText);
                }

                await this.Save(responseData);
            }

            ///////////////////////////////////////////////////////////
            async Task RefreshWhsLocMapTypePicking()
            {
                this.WarehouseMapTypeName = "Picking";
                var responseData = await SendComm(
                    await this.CreateRequestProdLoc(
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking)));
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var reqErr = new XmlDocument();
                    reqErr.LoadXml(responseData);
                    throw new Exception(reqErr.DocumentElement!["detail"] !.InnerText);
                }

                await this.Save(responseData);
            }

            ///////////////////////////////////////////////////////////
            async Task RefreshWhsLocMapTypeReceiving()
            {
                this.WarehouseMapTypeName = "Receiving";
                var responseData = await SendComm(
                    await this.CreateRequestProdLoc(
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving)));
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var reqErr = new XmlDocument();
                    reqErr.LoadXml(responseData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                }

                await this.Save(responseData);
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
                rowsAffected = Convert.ToInt32(
                    await DatabaseConnection.GetInstance()
                        .SqlConn.ExecuteScalarAsync<int>(
                            "SELECT COUNT(*) FROM tWarehouseLocationMapType"));

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} warehouse locations.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    false);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "warehouse locations status failed.",
                    false);

                throw new Exception(
                    "Database exception occurred. Warehouse location status failed.",
                    ex);
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
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging warehouse locations.",
                    true);

                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tWarehouseLocationMapType");
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
                    "Purging warehouse locations.",
                    true);

                throw new Exception(
                    "Database exception occurred. Purging warehouse locations.",
                    ex);
            }
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            ////select cWhsLocation.LocationMapFKRefno, ;
            ////cWhsLocation.LocationMapSubType, ;
            ////cWhsLocation.WarehouseLocationMapType, ;
            ////cWhsLocation.WarehouseLocationMapFKRefno

            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var whsTypes = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        whsTypes.Add(
                            new Seg
                            {
                                LocationMapFKRefno = Convert.ToInt32(row["LocationMapFKRefno"]),
                                LocationMapSubType = Convert.ToInt32(row["LocationMapSubType"]),
                                WarehouseLocationMapType = Convert.ToInt32(row["WarehouseLocationMapType"]),
                                WarehouseLocationMapFKRefno = Convert.ToInt32(row["WarehouseLocationMapFKRefno"])
                            });
                    }

                    var query = "CREATE TEMP TABLE Seg " +
                        "(" +
                        "LocationMapFKRefno INTEGER NOT NULL," +
                        "LocationMapSubType INTEGER NOT NULL," +
                        "WarehouseLocationMapType INTEGER NOT NULL," +
                        "WarehouseLocationMapFKRefno INTEGER NOT NULL" +
                        ")";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(whsTypes);

                    query = "INSERT INTO tWarehouseLocationMapType (" +
                        "LocationMapRefno" +
                        ",WarehouseLocationMapTypeEnum" +
                        ",WarehouseLocationMapRefno" +
                        ") SELECT Whs.LocationMapRefno" +
                        ",Seg.WarehouseLocationMapType" +
                        ",Whs.LocationMapRefno as WarehouseLocationMapRefno" +
                        " FROM Seg" +
                        " INNER JOIN tLocationMapWarehouse Whs ON Whs.LocationMapWarehouseFKRefno = Seg.LocationMapFKRefno" +
                        $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Warehouse}" +
                        " UNION" +
                        " SELECT Sect.LocationMapRefno" +
                        ",Seg.WarehouseLocationMapType" +
                        ",Sect.WarehouseLocationMapRefno" +
                        " FROM Seg" +
                        " INNER JOIN tLocationMapSection Sect ON Sect.LocationMapSectionFKRefno = Seg.LocationMapFKRefno" +
                        $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Section}" +
                        " UNION" +
                        " SELECT Slot.LocationMapRefno" +
                        ",Seg.WarehouseLocationMapType" +
                        ",Slot.WarehouseLocationMapRefno" +
                        " FROM Seg" +
                        " INNER JOIN tLocationMapSlot Slot ON Slot.LocationMapSlotFKRefno = Seg.LocationMapFKRefno" +
                        $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Slot}";
                    var rowsAffected = await conn.ExecuteAsync(query);
                    if (whsTypes.Count != rowsAffected)
                    {
                        throw new Exception("Failed to insert all rows into tLocationMapSlot");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);

                    this.DataAccessStatusOutHandler(
                        this,
                    EventArgs.Empty,
                        $"{whsTypes.Count} warehouse map types for {this.WarehouseMapTypeName}.", true);
                }
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save for {this.WarehouseMapTypeName}.",
                    true);
            }
        }

        /// <summary>
        /// The CreateRequest.
        /// </summary>
        /// <param name="whsLocMapType">The whsLocMapType<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequestProdLoc(int whsLocMapType)
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
                "<action>REQUEST WAREHOUSE LOCATION MAP TYPES</action>" +
                await SysCommon.GetRequestDefaults(true) +
                "<warehouse_location_map_type>" +
                whsLocMapType +
                "</warehouse_location_map_type>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            public int LocationMapFKRefno { get; set; }

            public int LocationMapSubType { get; set; }

            public int WarehouseLocationMapType { get; set; }

            public int WarehouseLocationMapFKRefno { get; set; }
        }
    }
}