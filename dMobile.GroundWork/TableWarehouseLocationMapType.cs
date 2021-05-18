namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableWarehouseLocationMapType" />
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
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations.", true);

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);

                string reqData = await this.SendRequest(Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking));

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);

                reqData = await this.SendRequest(Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving));

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding warehouse locations failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tWarehouseLocationMapType"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} warehouse locations.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "warehouse locations status failed.", false);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Warehouse location status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tWarehouseLocationMapType");
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging warehouse locations.", ex);
            }
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="whsLocMapType"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int whsLocMapType)
        {
            return await this.SendComm(this.CreateRequest(whsLocMapType));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            int locMapRefno = 0;
            int whsLocMapType = 0;
            int locMapFKRefno = 0;
            int locMapSubType = 0;
            int currWhsLocMapRefno = 0;
            int whsLocMapFKRefno = 0;
            int whsLocMapRefno = 0;
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);


                            int progValue = 0;
                            bool isLocMapTypeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                whsLocMapFKRefno = Convert.ToInt32(drow["WarehouseLocationMapFKRefno"]);
                                errMsg += Environment.NewLine + "Warehouse fk refno " + whsLocMapFKRefno;

                                List<tLocationMapWarehouseModel> resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", whsLocMapFKRefno);

                                if (resultsLocationMapWarehouse?.Count > 0)
                                {
                                    whsLocMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                }
                                else
                                {
                                    // TODO: Resource Strings
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
                                        // TODO: Resource Strings
                                        throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                                }

                                // whsLocMapType = Picking, Receiving, Floor.
                                whsLocMapType = Convert.ToInt32(drow["WarehouseLocationMapType"]);
                                errMsg += Environment.NewLine + "Map type " + whsLocMapType;

                                // Find the key for the locMapSubType.
                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                        List<tLocationMapWarehouseModel> resultsWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapFKRefno);

                                        if (resultsWarehouse?.Count > 0)
                                        {
                                            locMapRefno = resultsWarehouse[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        List<tLocationMapSectionModel> resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSection?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        List<tLocationMapSlotModel> resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapFKRefno);

                                        if (resultsLocationMapSlot?.Count > 0)
                                        {
                                            locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                        }

                                        break;

                                    default:
                                        throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                                }

                                isLocMapTypeFound = false;
                                currWhsLocMapRefno = 0;

                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    List<tWarehouseLocationMapTypeModel> resultsWarehouseLocationMapType = transaction.Query<tWarehouseLocationMapTypeModel>("SELECT * FROM tWarehouseLocationMapType WHERE WarehouseLocationMapRefno = ? AND WarehouseLocationMapTypeEnum = ?", whsLocMapRefno, whsLocMapType);

                                    if (resultsWarehouseLocationMapType?.Count > 0)
                                    {
                                        currWhsLocMapRefno = resultsWarehouseLocationMapType[0].LocationMapRefno;
                                        isLocMapTypeFound = true;
                                    }
                                    else
                                    {
                                        // TODO: Resource Strings
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

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving warehouse locations failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="whsLocMapType">The whsLocMapType<see cref="int"/></param>
        /// <returns></returns>
        protected override string CreateRequest(int whsLocMapType)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSE LOCATION MAP TYPES</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<warehouse_location_map_type>" + whsLocMapType + "</warehouse_location_map_type>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
