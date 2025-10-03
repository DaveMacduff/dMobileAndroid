// <copyright file="TableProductLocationMapType.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models.LocationMap;
    using SQLite;

    /// <summary>
    ///     Table product location map type CRUD methods.
    /// </summary>
    public class TableProductLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductLocationMapType"/> class.
        /// </summary>
        public TableProductLocationMapType()
        {
            this.LowerCaseName = "product locations";
            this.ProperCaseName = "Product locations";
        }

        /// <inheritdoc/>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);
                const string query = "SELECT tLocationMapWarehouse.LocationMapWarehouseFKRefno FROM tLocationMapWarehouse";
                var resultsLocationMapWarehouse = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<LocationMapWarehouseModel>(query);

                if (resultsLocationMapWarehouse?.Count > 0)
                {
                    foreach (var whs in resultsLocationMapWarehouse)
                    {
                        var responseData = await this.SendRequest(
                            0,
                            whs.LocationMapWarehouseFKRefno);
                        await this.SaveOpt(responseData, whs.LocationMapWarehouseFKRefno);
                    }
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.LowerCaseName} seeded.",
                    true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);

                throw new BaseException($"Seeding {this.LowerCaseName} failed.", ex);
            }
        }

        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(
                    await DatabaseConnection.GetInstance()
                        .SqlConn.ExecuteScalarAsync<int>(
                            "SELECT COUNT(*) FROM tProductLocationMapType"));

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} product locations.",
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
                    $"{this.ProperCaseName} status failed.",
                    false);

                throw new BaseException(
                    $"Database exception occurred. {this.ProperCaseName} status failed.",
                    ex);
            }

            return rowsAffected;
        }

        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Purging {this.LowerCaseName}.",
                    true);

                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tProductLocationMapType");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Database exception occurred. Purging {this.LowerCaseName}.",
                    true);

                throw new BaseException($"Database exception occurred. Purging {this.LowerCaseName}.", ex);
            }
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="locMapWhsFKRefno">Location map warehouse foreign key reference number.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected async Task SaveOpt(string responseData, int locMapWhsFKRefno)
        {
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var prodLoc = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        prodLoc.Add(
                            new Seg
                            {
                                LocationMapFKRefno = Convert.ToInt32(row["LocationMapFKRefno"]),
                                LocationMapSubType = Convert.ToInt32(row["LocationMapSubType"]),
                                ProductUnitLocationMapType =
                                    Convert.ToInt32(row["ProductUnitLocationMapType"]),
                                ProductFKRefno = Convert.ToInt32(row["ProductFKRefno"])
                            });
                    }

                    var query = "CREATE TEMP TABLE Seg " +
                                "(" +
                                "LocationMapFKRefno INTEGER, " +
                                "LocationMapSubType INTEGER, " +
                                "ProductUnitLocationMapType INTEGER, " +
                                "ProductFKRefno INTEGER" +
                                ")";
                    await Conn.ExecuteAsync(query);
                    await Conn.InsertAllAsync(prodLoc);
                    query = "INSERT INTO tProductLocationMapType (" +
                            "ProductRefno" +
                            ",WarehouseLocationMapRefno" +
                            ",ProductLocationMapTypeEnum" +
                            ",LocationMapRefno) " +
                            "SELECT prod.ProductRefno" +
                            ",whs.LocationMapRefno as WarehouseLocationMapRefno" +
                            ",seg.ProductUnitLocationMapType as ProductLocationMapTypeEnum" +
                            ",whs.LocationMapRefno" +
                            " FROM Seg" +
                            " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                            " INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapWarehouseFKRefno = Seg.LocationMapFKRefno" +
                            $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Warehouse}" +
                            " UNION " +
                            "SELECT prod.ProductRefno" +
                            ",whs.LocationMapRefno as WarehouseLocationMapRefno" +
                            ",seg.ProductUnitLocationMapType as ProductLocationMapTypeEnum" +
                            ",sect.LocationMapRefno" +
                            " FROM Seg" +
                            " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                            " INNER JOIN tLocationMapSection sect ON sect.LocationMapSectionFKRefno = Seg.LocationMapFKRefno" +
                            " INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapRefno = sect.WarehouseLocationMapRefno" +
                            $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Section}" +
                            " UNION " +
                            "SELECT prod.ProductRefno" +
                            ",whs.LocationMapRefno as WarehouseLocationMapRefno" +
                            ",Seg.ProductUnitLocationMapType as ProductLocationMapTypeEnum" +
                            ",slot.LocationMapRefno" +
                            " FROM Seg" +
                            " INNER JOIN tProduct prod ON prod.ProductFKRefno = Seg.ProductFKRefno" +
                            " INNER JOIN tLocationMapSlot slot ON slot.LocationMapSlotFKRefno = Seg.LocationMapFKRefno" +
                            " INNER JOIN tLocationMapWarehouse whs ON whs.LocationMapRefno = slot.WarehouseLocationMapRefno" +
                            $" WHERE Seg.LocationMapSubType = {(int)SysCommon.LocMapSubTypes.Slot}";
                    var rowsAffected = await Conn.ExecuteAsync(query);
                    if (prodLoc.Count != rowsAffected)
                    {
                        throw new BaseException(
                            "Failed to insert all rows into tProductLocationMapType");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await Conn.ExecuteAsync(dropTempTableQuery);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rows.Count} {this.LowerCaseName} for warehouse {locMapWhsFKRefno}.",
                    true);
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} for warehouse {locMapWhsFKRefno} to save.",
                    true);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest(
            int prodLocMapType,
            int whsLocMapFkRefno)
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
                "<action>REQUEST PRODUCT LOCATION MAP TYPES</action>" +
                await SysCommon.GetRequestDefaults(true) +
                "<product_location_map_type>" +
                prodLocMapType +
                "</product_location_map_type>" +
                "<whs_id>" +
                whsLocMapFkRefno +
                "</whs_id>" +
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

            public int ProductUnitLocationMapType { get; set; }

            public int ProductFKRefno { get; set; }
        }
    }
}