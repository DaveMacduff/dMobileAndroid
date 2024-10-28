// <copyright file="TableLocationMapWarehouse.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableLocationMapWarehouse" />.
    /// </summary>
    public class TableLocationMapWarehouse : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapWarehouse"/> class.
        /// </summary>
        public TableLocationMapWarehouse()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapWarehouse";
            this.LowerCaseName = "warehouses";
            this.ProperCaseName = "Warehouses";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapWarehouse WHERE LocationMapSubType = ?", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses failed.", true);

                throw new Exception("Database exception occurred. Purging warehouses failed.", ex);
            }
        }

        /// <summary>
        /// The GetLocMapWhsRefnoFromLocWhsFKRefno.
        /// </summary>
        /// <param name="locMapWhsFKRefno"></param>
        /// <returns></returns>
        public async Task<int> GetLocMapWhsRefnoFromLocWhsFKRefno(int locMapWhsFKRefno)
        {
            int whsLocMapRefno;
            try
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<LocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

                if (results?.Count > 0)
                {
                    whsLocMapRefno = results[0].LocationMapRefno;
                }
                else
                {
                    throw new Exception(
                        $"Unable to locate warehouse refno using the warehouse fk refno {locMapWhsFKRefno} in tLocationMapWarehouse.");
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.", ex);
            }

            return whsLocMapRefno;
        }

        /// <summary>
        /// The GetLocMapWhsFKRefnoFromLocMapWhsRefno.
        /// </summary>
        /// <param name="locMapWhsRefno"></param>
        /// <returns></returns>
        public async Task<int> GetLocMapWhsFKRefnoFromLocMapWhsRefno(int locMapWhsRefno)
        {
            int locMapWhsFKRefno;
            try
            {
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<LocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", locMapWhsRefno);

                if (results?.Count > 0)
                {
                    locMapWhsFKRefno = results[0].LocationMapWarehouseFKRefno;
                }
                else
                {
                    throw new Exception(
                        $"Unable to locate warehouse FkRefno using the warehouse refno {locMapWhsRefno} in tLocationMapWarehouse.");
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.", ex);
            }

            return locMapWhsFKRefno;
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapWarehouse"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} warehouses.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Warehouses status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> SendRequest()
        {
            return await SendComm(await this.CreateRequest());
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var warehouses = new List<LocationMapWarehouseModel>();
                var rowsToSave = dataSet.Tables["tLocationMapWarehouse"].Rows.Count;
                for (var i = 0; i < rowsToSave; i++)
                {
                    var locMapRefno = await GetLocationMapRefno();
                    var row = dataSet.Tables["tLocationMapWarehouse"].Rows[i];
                    var warehouse = new LocationMapWarehouseModel
                    {
                        LocationMapRefno = locMapRefno,
                        LocationMapSubType = Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse),
                        LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                        LocationMapWarehouseName = row["LocationMapWarehouseName"].ToString(),
                        LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"]),
                        LocationMapWarehouseBarCode = row["LocationMapWarehouseBarCode"].ToString(),
                        LocationMapWarehouseIsWMS = Convert.ToBoolean(row["LocationMapWarehouseIsWMS"]),
                        LocationMapWarehouseMask = row["LocationMapWarehouseMask"].ToString(),
                        LocationMapWarehouseSectionMask = row["LocationMapSectionMask"].ToString(),
                        LocationMapWarehouseSlotMask = row["LocationMapSlotMask"].ToString(),
                        LocationMapWarehouseSlotCoordinate1Mask = row["LocationMapSlotCoord1Mask"].ToString(),
                        LocationMapWarehouseSlotCoordinate2Mask = row["LocationMapSlotCoord2Mask"].ToString(),
                        LocationMapWarehouseSlotCoordinate3Mask = row["LocationMapSlotCoord3Mask"].ToString(),
                        LocationMapWarehouseSlotCoordinate1Name = row["LocationMapSlotCoord1Name"].ToString(),
                        LocationMapWarehouseSlotCoordinate2Name = row["LocationMapSlotCoord2Name"].ToString(),
                        LocationMapWarehouseSlotCoordinate3Name = row["LocationMapSlotCoord3Name"].ToString(),
                        LocationMapWarehouseIsSectionTracked = Convert.ToBoolean(row["LocationMapIsSectionTracked"]),
                        LocationMapWarehouseIsSlotTracked = Convert.ToBoolean(row["LocationMapIsSlotTracked"]),
                        LocationMapWarehouseAdhocEnabled = Convert.ToBoolean(row["LocationMapIsAdhocEnabled"]),
                    };

                    warehouses.Add(warehouse);
                }

                await conn.InsertAllAsync(warehouses);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{warehouses.Count} {this.LowerCaseName}.", true);
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save.",
                    true);
            }

            return;

            ///////////////////////////////////////////////////////////
            async Task<int> GetLocationMapRefno()
            {
                var query = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)";
                await conn.ExecuteAsync(query, Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));
                query = "SELECT last_insert_rowid() FROM tLocationMap";
                var locMapRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(query));
                return locMapRefno;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
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
                + "<action>REQUEST WAREHOUSES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
