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
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
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
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations.", true);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);

                var responseData = await this.SendComm(await this.CreateRequestProdLoc(
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking)));
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var reqErr = new XmlDocument();
                    reqErr.LoadXml(responseData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                }

                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);
                responseData = await this.SendComm(await this.CreateRequestProdLoc(
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving)));
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var reqErr = new XmlDocument();
                    reqErr.LoadXml(responseData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                }

                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations failed.", true);

                throw new Exception("Seeding warehouse locations failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tWarehouseLocationMapType"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} warehouse locations.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "warehouse locations status failed.", false);

                throw new Exception("Database exception occurred. Warehouse location status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tWarehouseLocationMapType");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);

                throw new Exception("Database exception occurred. Purging warehouse locations.", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var locMapRefno = 0;
            var whsLocMapType = 0;
            var locMapFKRefno = 0;
            var locMapSubType = 0;
            var currWhsLocMapRefno = 0;
            var whsLocMapFKRefno = 0;
            var whsLocMapRefno = 0;
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var progValue = 0;
                        var isLocMapTypeFound = false;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["seg"].Rows)
                        {
                            whsLocMapFKRefno = Convert.ToInt32(drow["WarehouseLocationMapFKRefno"]);
                            errMsg += Environment.NewLine + "Warehouse fk refno " + whsLocMapFKRefno;

                            var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", whsLocMapFKRefno);

                            if (resultsLocationMapWarehouse?.Count > 0)
                            {
                                whsLocMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                            }
                            else
                            {
                                throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                            }

                            locMapFKRefno = Convert.ToInt32(drow["LocationMapFKRefno"]);
                            errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                            // locMapSubType = Warehouse, Section, Slot.
                            // Create error message for locMapSubType.
                            locMapSubType = Convert.ToInt32(drow["LocationMapSubType"]);
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                        + ", 'Warehouse'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                        + ", 'Section'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                        + ", 'Slot'.";
                                    break;

                                default:

                                    throw new Exception($"Undefined location map subtype {locMapSubType}");
                            }

                            // whsLocMapType = Picking, Receiving, Floor.
                            whsLocMapType = Convert.ToInt32(drow["WarehouseLocationMapType"]);
                            errMsg += Environment.NewLine + "Map type " + whsLocMapType;

                            // Find the key for the locMapSubType.
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    var resultsWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapFKRefno);

                                    if (resultsWarehouse?.Count > 0)
                                    {
                                        locMapRefno = resultsWarehouse[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapFKRefno);

                                    if (resultsLocationMapSection?.Count > 0)
                                    {
                                        locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    var resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapFKRefno);

                                    if (resultsLocationMapSlot?.Count > 0)
                                    {
                                        locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                    }

                                    break;

                                default:
                                    throw new Exception($"Undefined location map subtype {locMapSubType}");
                            }

                            isLocMapTypeFound = false;
                            currWhsLocMapRefno = 0;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var resultsWarehouseLocationMapType = transaction.Query<tWarehouseLocationMapTypeModel>("SELECT * FROM tWarehouseLocationMapType WHERE WarehouseLocationMapRefno = ? AND WarehouseLocationMapTypeEnum = ?", whsLocMapRefno, whsLocMapType);

                                if (resultsWarehouseLocationMapType?.Count > 0)
                                {
                                    currWhsLocMapRefno = resultsWarehouseLocationMapType[0].LocationMapRefno;
                                    isLocMapTypeFound = true;
                                }
                                else
                                {
                                    throw new Exception("drLocMapType.Read() failed." + Environment.NewLine + errMsg);
                                }
                            }

                            if (isLocMapTypeFound)
                            {
                                if (currWhsLocMapRefno != locMapRefno)
                                {
                                    // Update the LocMapRefno if different
                                    transaction.Execute("UPDATE tWarehouseLocationMapType SET LocationMapRefno = ? WHERE WarehouseLocationMapRefno = ? AND WarehouseLocationMapTypeEnum = ?", locMapRefno, whsLocMapRefno, whsLocMapType);
                                }
                            }
                            else
                            {
                                transaction.Execute("INSERT INTO tWarehouseLocationMapType (WarehouseLocationMapRefno, WarehouseLocationMapTypeEnum, LocationMapRefno) VALUES (?, ?, ?)", whsLocMapRefno, whsLocMapType, locMapRefno);
                            }

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations failed.", false);

                    throw new Exception("Database exception occurred. Saving warehouse locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The CreateRequest.
        /// </summary>
        /// <param name="whsLocMapType">The whsLocMapType<see cref="int"/>.</param>
        /// <returns></returns>
        protected async override Task<string> CreateRequestProdLoc(int whsLocMapType)
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
                + "<action>REQUEST WAREHOUSE LOCATION MAP TYPES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<warehouse_location_map_type>" + whsLocMapType + "</warehouse_location_map_type>"
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

        protected override Task<string> CreateRequest(int activityItemFkRefno)
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

        protected override Task Save(string responseData, int refno)
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
    }
}
