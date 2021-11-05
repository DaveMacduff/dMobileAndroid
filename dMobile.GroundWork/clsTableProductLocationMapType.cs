namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.IO;
    using System.Collections.Generic;
    using SQLite;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;

    public class TableProductLocationMapType : TableBase
    {
        public TableProductLocationMapType() : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductLocationMapType";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Request()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations.", true);

                string whsLocMapName = string.Empty;
                int whsLocMapFKRefno = 0;
                int whsLocMapRefno = 0;
                string reqData = string.Empty;

                List<tLocationMapWarehouseModel> resultsLocationMapWarehouse = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse");

                if (resultsLocationMapWarehouse?.Count > 0)
                {
                    foreach (tLocationMapWarehouseModel locationMapWarehouse in resultsLocationMapWarehouse)
                    {
                        whsLocMapFKRefno = locationMapWarehouse.LocationMapWarehouseFKRefno;
                        whsLocMapName = locationMapWarehouse.LocationMapWarehouseName;
                        whsLocMapRefno = locationMapWarehouse.LocationMapRefno;

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For whs \'{0}\'.", whsLocMapName), true);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);

                        reqData = await this.SendRequest((int)SysCommon.ProdLocMapTypes.Picking, whsLocMapFKRefno);

                        if (0 < reqData.Length)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.SaveOpt(reqData, whsLocMapRefno);
                        }

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);

                        reqData = await this.SendRequest((int)SysCommon.ProdLocMapTypes.Receiving, whsLocMapFKRefno);

                        if (0 < reqData.Length)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument reqErr = new XmlDocument();
                                reqErr.LoadXml(reqData);
                                throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            }

                            await this.SaveOpt(reqData, whsLocMapRefno);
                        }
                    }
                }

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);

                // TODO: Resource Strings
                throw new Exception("Database exception occurred. Seeding product locations failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);

                // TODO: Resource Strings
                throw new Exception("Seeding product locations failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tProductLocationMapType"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} product locations.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "product locations status failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Product location status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tProductLocationMapType");
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product locations.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <param name="whsLocMapFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            return await this.SendComm(await this.CreateRequest(prodLocMapType, whsLocMapFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="whsLocMapRefno"></param>
        protected override async Task Save(string reqData, int whsLocMapRefno)
        {
            int prodRefno = 0;
            int locMapRefno = 0;
            int prodLocMapType = 0;
            int locMapFKRefno = 0;
            int locMapSubType = 0;
            int prodFKRefno = 0;
            int currProdLocMapRefno = 0;
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int progValue = 0;
                            bool isLocMapTypeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                prodFKRefno = Convert.ToInt32(drow["f04"]);
                                errMsg = "FK refno " + prodFKRefno;

                                List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

                                if (resultsProduct?.Count > 0)
                                {
                                    prodRefno = resultsProduct[0].ProductRefno;
                                }
                                else
                                {
                                    // TODO: Resource strings
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
                                        // TODO: Resource strings
                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Warehouse'.";
                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        // TODO: Resource strings
                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Section'.";
                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        // TODO: Resource strings
                                        errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Slot'.";
                                        break;

                                    default:
                                        // TODO: Resource strings
                                        throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                                }

                                // prodLocMapType = Picking, Receiving, Floor.
                                prodLocMapType = Convert.ToInt32(drow["f03"]);
                                errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                                // Find the key for the locMapSubType.
                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                        List<tLocationMapWarehouseModel> resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapFKRefno));

                                        if (resultsLocationMapWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        List<tLocationMapSectionModel> resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToInt32(locMapFKRefno));

                                        if (resultsLocationMapSection?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        List<tLocationMapSlotModel> resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", Convert.ToInt32(locMapFKRefno));

                                        if (resultsLocationMapSlot?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    default:
                                        // TODO: Resource strings
                                        throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                                }

                                isLocMapTypeFound = false;
                                currProdLocMapRefno = 0;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    string query = "SELECT * FROM tProductLocationMapType WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductLocationMapTypeEnum = ?";
                                    List<tProductLocationMapTypeModel> resultsProductLocationMapType = transaction.Query<tProductLocationMapTypeModel>(query, Convert.ToInt32(whsLocMapRefno), Convert.ToInt32(prodRefno), Convert.ToInt32(prodLocMapType));

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
                                        string queryUpdate = "UPDATE tProductLocationMapType"
                                          + " SET LocationMapRefno = " + locMapRefno
                                          + " WHERE ProductRefno = " + prodRefno
                                          + " AND WarehouseLocationMapRefno = " + whsLocMapRefno
                                          + " AND ProductLocationMapTypeEnum = " + prodLocMapType;

                                        transaction.Execute(queryUpdate);
                                    }
                                }
                                else
                                {
                                    string queryInsert = "INSERT INTO tProductLocationMapType (" +
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
                                    object[] parametersInsert = new object[]
                                    {
                                        Convert.ToInt32(prodRefno),
                                        Convert.ToInt32(whsLocMapRefno),
                                        Convert.ToInt32(prodLocMapType),
                                        Convert.ToInt32(locMapRefno)
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
                    }

                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="whsLocMapRefno"></param>
        protected async Task SaveOpt(string reqData, int whsLocMapRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    int startIndex = reqData.IndexOf("<VFPData");
                    XmlReaderSettings sett = new XmlReaderSettings();
                    sett.CheckCharacters = false;
                    sett.CloseInput = false;
                    sett.ConformanceLevel = ConformanceLevel.Auto;
                    sett.IgnoreComments = true;
                    sett.IgnoreProcessingInstructions = true;
                    sett.IgnoreWhitespace = false;
                    sett.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
                    sett.ValidationType = ValidationType.None;

                    using (StringReader stringReader = new StringReader(reqData.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader, sett))
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);

                        int recordCount = 0;
                        bool isLocMapTypeFound = false;

                        int prodRefno = 0;
                        int locMapRefno = 0;
                        // f01
                        int locMapFKRefno = 0;
                        // f02
                        int locMapSubType = 0;
                        // f03
                        int prodLocMapType = 0;
                        // f04
                        int prodFKRefno = 0;
                        int currProdLocMapRefno = 0;

                        Dictionary<int, int> warehouseLookup = new Dictionary<int, int>();
                        Dictionary<int, int> sectionLookup = new Dictionary<int, int>();
                        Dictionary<int, int> slotLookup = new Dictionary<int, int>();

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            // fill one record
                            // we do this by looping until an end segment is found
                            bool recordFilled = false;
                            string elementName = string.Empty;
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

                            // TODO: Resource Strings
                            errMsg = "FK refno " + prodFKRefno;

                            List<tProductModel> resultsProduct = transaction.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", prodFKRefno);

                            if (resultsProduct?.Count > 0)
                            {
                                prodRefno = resultsProduct[0].ProductRefno;
                            }
                            else
                            {
                                // TODO: Resource Strings
                                throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
                            }

                            // TODO: Resource Strings
                            errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                            // locMapSubType = Warehouse, Section, Slot.
                            // Create error message for locMapSubType.
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    // TODO: Resource Strings
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Warehouse'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    // TODO: Resource Strings
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Section'.";
                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    // TODO: Resource Strings
                                    errMsg += Environment.NewLine + "Map sub type = " + locMapSubType + ", 'Slot'.";
                                    break;

                                default:
                                    // TODO: Resource Strings
                                    throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                            }

                            // prodLocMapType = Picking, Receiving, Floor.
                            errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                            // Find the key for the locMapSubType.
                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    if (!warehouseLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        List<tLocationMapWarehouseModel> resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                            warehouseLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    if (!sectionLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        List<tLocationMapSectionModel> resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSection?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                            sectionLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    if (!slotLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                    {
                                        List<tLocationMapSlotModel> resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSlot?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                            slotLookup.Add(locMapFKRefno, locMapRefno);
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                    }

                                    break;

                                default:
                                    throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                            }

                            isLocMapTypeFound = false;
                            currProdLocMapRefno = 0;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                string querySelect = "SELECT * FROM tProductLocationMapType WHERE WarehouseLocationMapRefno = ? AND ProductRefno = ? AND ProductLocationMapTypeEnum = ?";
                                List<tProductLocationMapTypeModel> resultsProductLocationMapType = transaction.Query<tProductLocationMapTypeModel>(querySelect, whsLocMapRefno, prodRefno, prodLocMapType);

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
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        } // end of data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        warehouseLookup = null;
                        sectionLookup = null;
                        slotLookup = null;
                    }

                    transaction.Commit();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected async override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT LOCATION MAP TYPES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + ("<product_location_map_type>" + prodLocMapType + "</product_location_map_type>")
                + ("<whs_id>" + whsLocMapFKRefno + "</whs_id>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
