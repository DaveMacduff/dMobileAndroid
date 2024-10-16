// <copyright file="TableLocationMapType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models.LocationMap;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableLocationMapType" />.
    /// </summary>
    public class TableLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapType"/> class.
        /// </summary>
        public TableLocationMapType()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapType";
            this.LowerCaseName = "location map types";
            this.ProperCaseName = "Location map types";
        }

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
                const int whsLocMapRefno = 0;
                var responseData = await this.SendRequest(whsLocMapRefno);
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} complete.",
                    true);
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
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapType");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types failed.", true);

                throw new Exception("Database exception occurred. Purging location map types failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapType"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} location map types.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Location map types status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="activityFkRefno</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            return await this.SendComm(await this.CreateRequest(activityFkRefno));
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
                var locTypes = new List<LocationMapTypeModel>();
                var rowsToSave = dataSet.Tables["tLocationMapType"].Rows.Count;
                for (var i = 0; i < rowsToSave; i++)
                {
                    var row = dataSet.Tables["tLocationMapType"].Rows[i];
                    var locMapType = (SysCommon.LocMapTypes)Convert.ToInt32(row["LocationMapTypeEnum"]);

                    // Removed validation because of try to make save faster.
                    ////switch (locMapType)
                    ////{
                    ////    case SysCommon.LocMapTypes.IsFreezer:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsGroundLevel:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsHandStack:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsProcessing:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsReserved:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsStaging:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsToBeOrdered:
                    ////        break;
                    ////    case SysCommon.LocMapTypes.IsTracked:
                    ////        break;
                    ////    default:
                    ////        throw new Exception($"Undefined location map type {locMapType}");
                    ////}

                    var locMapFkRefno = Convert.ToInt32(row["LocationMapFKRefno"]);
                    var locMapSubType = (SysCommon.LocMapSubTypes)Convert.ToInt32(
                        row["LocationMapSubType"]);
                    var locMapRefno = locMapSubType switch
                    {
                        SysCommon.LocMapSubTypes.None => throw new ArgumentOutOfRangeException(),
                        SysCommon.LocMapSubTypes.Warehouse => await GetWarehouseLocMapRefno(
                            locMapFkRefno),
                        SysCommon.LocMapSubTypes.Section => await GetSectionLocMapRefno(
                            locMapFkRefno),
                        SysCommon.LocMapSubTypes.Slot => await GetSlotLocMapRefno(locMapFkRefno),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    var locType = new LocationMapTypeModel
                    {
                        LocationMapRefno = locMapRefno,
                        LocationMapTypeEnum = (int)locMapType,
                    };
                    locTypes.Add(locType);
                }

                await conn.InsertAllAsync(locTypes);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{locTypes.Count} {this.LowerCaseName}.", true);
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
            async Task<int> GetWarehouseLocMapRefno(int locMapFkRefno)
            {
                const string query = "SELECT LocationMapRefno FROM tLocationMapWarehouse" +
                                     " WHERE LocationMapWarehouseFKRefno = ?";
                var result = await conn.FindWithQueryAsync<LocationMapWarehouseModel>(
                    query, locMapFkRefno);
                if (result == null)
                {
                    var msg = $"Unable to find foreign key {locMapFkRefno} in warehouses.";
                    throw new Exception(msg);
                }

                return result.LocationMapRefno;
            }

            ///////////////////////////////////////////////////////////
            async Task<int> GetSectionLocMapRefno(int locMapFkRefno)
            {
                const string query = "SELECT LocationMapRefno FROM tLocationMapSection" +
                    " WHERE LocationMapSectionFKRefno = ?";
                var result = await conn.FindWithQueryAsync<LocationMapSectionModel>(
                    query, locMapFkRefno);
                if (result == null)
                {
                    var msg = $"Unable to find foreign key {locMapFkRefno} in sections.";
                    throw new Exception(msg);
                }

                return result.LocationMapRefno;
            }

            ///////////////////////////////////////////////////////////
            async Task<int> GetSlotLocMapRefno(int locMapFkRefno)
            {
                const string query = "SELECT LocationMapRefno FROM tLocationMapSlot" +
                    " WHERE LocationMapSlotFKRefno = ?";
                var result = await conn.FindWithQueryAsync<LocationMapSlotModel>(
                    query, locMapFkRefno);
                if (result == null)
                {
                    var msg = $"Unable to find foreign key {locMapFkRefno} in slots.";
                    throw new Exception(msg);
                }

                return result.LocationMapRefno;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var locMapWhsFKRefno = 0;
            var locMapWhs = new TableLocationMapWarehouse();

            // lookup and verify warehouse
            if (activityFkRefno > 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityFkRefno);
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST LOCATION MAP TYPES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
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

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
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
