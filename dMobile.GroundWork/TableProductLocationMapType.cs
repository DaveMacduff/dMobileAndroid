// <copyright file="TableProductLocationMapType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    public class TableProductLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableProductLocationMapType"/> class.
        /// </summary>
        public TableProductLocationMapType() : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductLocationMapType";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations.", true);

                var whsLocMapName = string.Empty;
                var whsLocMapFkRefno = 0;
                var whsLocMapRefno = 0;
                var responseData = string.Empty;

                var resultsLocationMapWarehouse = await DatabaseConnection.GetInstance().
                    SqlConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse");

                if (resultsLocationMapWarehouse?.Count > 0)
                {
                    foreach (var locationMapWarehouse in resultsLocationMapWarehouse)
                    {
                        whsLocMapFkRefno = locationMapWarehouse.LocationMapWarehouseFKRefno;
                        whsLocMapName = locationMapWarehouse.LocationMapWarehouseName;
                        whsLocMapRefno = locationMapWarehouse.LocationMapRefno;

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"For whs \'{whsLocMapName}\'.", true);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);

                        responseData = await this.SendRequest((int)SysCommon.ProdLocMapTypes.Floor, whsLocMapFkRefno);
                        await this.SaveOpt(responseData, whsLocMapRefno);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);

                        responseData = await this.SendRequest((int)SysCommon.ProdLocMapTypes.Picking, whsLocMapFkRefno);
                        await this.SaveOpt(responseData, whsLocMapRefno);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);

                        responseData = await this.SendRequest((int)SysCommon.ProdLocMapTypes.Receiving, whsLocMapFkRefno);
                        await this.SaveOpt(responseData, whsLocMapRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);

                throw new Exception("Database exception occurred. Seeding product locations failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);

                throw new Exception("Seeding product locations failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductLocationMapType"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} product locations.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "product locations status failed.", false);

                throw new Exception("Database exception occurred. Product location status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tProductLocationMapType");
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);

                throw new Exception("Database exception occurred. Purging product locations.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="whsLocMapRefno"></param>
        /// <inheritdoc/>
        protected override async Task Save(string responseData, int whsLocMapRefno)
        {
            var prodRefno = 0;
            var locMapRefno = 0;
            var prodLocMapType = 0;
            var locMapFKRefno = 0;
            var locMapSubType = 0;
            var prodFKRefno = 0;
            var currProdLocMapRefno = 0;
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                            var progValue = 0;
                            var isLocMapTypeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                prodFKRefno = Convert.ToInt32(drow["f04"]);
                                errMsg = "FK refno " + prodFKRefno;

                                var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

                                if (resultsProduct?.Count > 0)
                                {
                                    prodRefno = resultsProduct[0].ProductRefno;
                                }
                                else
                                {
                                    throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                                }

                                locMapFKRefno = Convert.ToInt32(drow["f01"]);
                                errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                                // locMapSubType = Warehouse, Section, Slot.
                                // Create error message for locMapSubType.
                                locMapSubType = Convert.ToInt32(drow["f02"]);
                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:

                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Warehouse'.";
                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:

                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Section'.";
                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:

                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Slot'.";
                                        break;

                                    default:

                                        throw new Exception($"Undefined location map subtype {locMapSubType}");
                                }

                                // prodLocMapType = Picking, Receiving, Floor.
                                prodLocMapType = Convert.ToInt32(drow["f03"]);
                                errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                                // Find the key for the locMapSubType.
                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                        var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapFKRefno));

                                        if (resultsLocationMapWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToInt32(locMapFKRefno));

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
                                        var resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", Convert.ToInt32(locMapFKRefno));

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
                                currProdLocMapRefno = 0;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    var query = "SELECT * FROM tProductLocationMapType WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductLocationMapTypeEnum = ?";
                                    var resultsProductLocationMapType = transaction.Query<tProductLocationMapTypeModel>(query, Convert.ToInt32(whsLocMapRefno), Convert.ToInt32(prodRefno), Convert.ToInt32(prodLocMapType));

                                    if (resultsProductLocationMapType?.Count > 0)
                                    {
                                        currProdLocMapRefno = resultsProductLocationMapType[0].LocationMapRefno;
                                        isLocMapTypeFound = true;
                                    }
                                }

                                if (isLocMapTypeFound)
                                {
                                    if (currProdLocMapRefno != locMapRefno)
                                    {
                                        // Update the LocMapRefno if different
                                        var queryUpdate = "UPDATE tProductLocationMapType"
                                                          + " SET LocationMapRefno = " + locMapRefno
                                                          + " WHERE ProductRefno = " + prodRefno
                                                          + " AND WarehouseLocationMapRefno = " + whsLocMapRefno
                                                          + " AND ProductLocationMapTypeEnum = " + prodLocMapType;

                                        transaction.Execute(queryUpdate);
                                    }
                                }
                                else
                                {
                                    var queryInsert = "INSERT INTO tProductLocationMapType (" +
                                                      "ProductRefno, " +
                                                      "WarehouseLocationMapRefno, " +
                                                      "ProductLocationMapTypeEnum, " +
                                                      "LocationMapRefno" +
                                                      ") VALUES (" +
                                                      "?, " +
                                                      "?, " +
                                                      "?, " +
                                                      "?" +
                                                      ")";
                                    var parametersInsert = new object[]
                                    {
                                        Convert.ToInt32(prodRefno),
                                        Convert.ToInt32(whsLocMapRefno),
                                        Convert.ToInt32(prodLocMapType),
                                        Convert.ToInt32(locMapRefno),
                                    };

                                    transaction.Execute(queryInsert, parametersInsert);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
                    }

                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);

                    throw new Exception("Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="whsLocMapRefno"></param>
        /// <summary>
        ///
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="whsLocMapRefno"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected async Task SaveOpt(string responseData, int whsLocMapRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    var startIndex = responseData.IndexOf("<VFPData", StringComparison.Ordinal);
                    var sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (var stringReader = new StringReader(responseData.Substring(startIndex)))
                    using (var reader = XmlReader.Create(stringReader, sett))
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);

                        var recordCount = 0;
                        var isLocMapTypeFound = false;

                        var prodRefno = 0;
                        var locMapRefno = 0;

                        // f01
                        var locMapFKRefno = 0;

                        // f02
                        var locMapSubType = 0;

                        // f03
                        var prodLocMapType = 0;

                        // f04
                        var prodFKRefno = 0;
                        var currProdLocMapRefno = 0;

                        var warehouseLookup = new Dictionary<int, int>();
                        var sectionLookup = new Dictionary<int, int>();
                        var slotLookup = new Dictionary<int, int>();

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            var recordFilled = false;
                            var elementName = string.Empty;
                            while (!recordFilled && reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    elementName = reader.Name;
                                }
                                else if (reader.NodeType == XmlNodeType.Text)
                                {
                                    switch (elementName)
                                    {
                                        case "f01":
                                            locMapFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f02":
                                            locMapSubType = reader.ReadContentAsInt();
                                            break;
                                        case "f03":
                                            prodLocMapType = reader.ReadContentAsInt();
                                            break;
                                        case "f04":
                                            prodFKRefno = reader.ReadContentAsInt();
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                                {
                                    recordFilled = true; // we are full;
                                }
                            } // end of fill record

                            if (!recordFilled)
                            {
                                continue;
                            }

                            errMsg = "FK refno " + prodFKRefno;

                            var resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                prodRefno = resultsProduct[0].ProductRefno;
                            }
                            else
                            {
                                throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                            }

                            errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                            // locMapSubType = Warehouse, Section, Slot.
                            // Create error message for locMapSubType.
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:

                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Warehouse'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:

                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Section'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:

                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Slot'.";
                                    break;

                                default:

                                    throw new Exception($"Undefined location map subtype {locMapSubType}");
                            }

                            // prodLocMapType = Picking, Receiving, Floor.
                            errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                            // Find the key for the locMapSubType.
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    if (!warehouseLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                            warehouseLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    if (!sectionLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSection?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                            sectionLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    if (!slotLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        var resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSlot?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                            slotLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                default:
                                    throw new Exception($"Undefined location map subtype {locMapSubType}");
                            }

                            isLocMapTypeFound = false;
                            currProdLocMapRefno = 0;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var querySelect = "SELECT * FROM tProductLocationMapType WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductLocationMapTypeEnum = ?";
                                var resultsProductLocationMapType = transaction.Query<tProductLocationMapTypeModel>(querySelect, whsLocMapRefno, prodRefno, prodLocMapType);

                                if (resultsProductLocationMapType?.Count > 0)
                                {
                                    currProdLocMapRefno = resultsProductLocationMapType[0].LocationMapRefno;
                                    isLocMapTypeFound = true;
                                }
                            }

                            if (isLocMapTypeFound)
                            {
                                if (currProdLocMapRefno != locMapRefno)
                                {
                                    // Update the LocMapRefno if different
                                    transaction.Execute("UPDATE tProductLocationMapType SET LocationMapRefno = ? WHERE ProductRefno = ? AND WarehouseLocationMapRefno = ? AND ProductLocationMapTypeEnum = ?", locMapRefno, prodRefno, whsLocMapRefno, prodLocMapType);
                                }
                            }
                            else
                            {
                                transaction.Execute("INSERT INTO tProductLocationMapType (ProductRefno, WarehouseLocationMapRefno, ProductLocationMapTypeEnum, LocationMapRefno) VALUES (?, ?, ?, ?)", prodRefno, whsLocMapRefno, prodLocMapType, locMapRefno);
                            }

                            ++recordCount;
                            if (recordCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, recordCount / 10 % 100, 0, 100);
                            }
                        } // end of data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {recordCount}", false);

                        warehouseLookup = null;
                        sectionLookup = null;
                        slotLookup = null;
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);

                    throw new Exception("Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        protected async override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
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
                + "<action>REQUEST PRODUCT LOCATION MAP TYPES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<product_location_map_type>" + prodLocMapType + "</product_location_map_type>"
                + "<whs_id>" + whsLocMapFkRefno + "</whs_id>"
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

        protected override Task Save(string responseData)
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

        protected override Task<string> CreateRequest(int activityFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
