namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    /// <summary>
    /// Defines the <see cref="TableLocationMapSlot" />
    /// </summary>
    public class TableLocationMapSlot : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSlot"/> class.
        /// </summary>
        public TableLocationMapSlot()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlot";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task Request(int whsLocMapRefno)
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots.", true);

                string reqData = string.Empty;

                if (0 < whsLocMapRefno)
                {
                    List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", whsLocMapRefno);

                    if (results?.Count > 0)
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For whs \'{0}\'.", results[0].LocationMapWarehouseName), true);
                    }
                    else
                    {
                        // TODO: Resource Strings
                        throw new Exception("No results in tLocationMapWarehouse for LocationMapRefno " + whsLocMapRefno + Environment.NewLine + this.FullClassName + " Request");
                    }
                }

                reqData = await this.Send(whsLocMapRefno);

                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.SaveOpt(reqData);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding slots failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding slots failed.", ex);
            }
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="sectRefno"></param>
        public override async Task PurgeData(int sectRefno)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots.", true);

                if (sectRefno == 0)
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSlot");
                }
                else
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSlot WHERE SectionLocationMapRefno = ?", sectRefno);
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging slots failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSlot"));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} slots.", rowsAffected), false);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Slots status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Send(int whsLocMapRefno)
        {
            return await this.SendComm(await this.Create(whsLocMapRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="recvData"></param>
        protected override Task Save(string recvData)
        {
            throw new Exception("clsTableLocationMapSlot Save is not implemented");
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="recvData"></param>
        protected async Task SaveOpt(string recvData)
        {
            string errMsg = string.Empty;
            int recordCount = 0;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    int startIndex = recvData.IndexOf("<VFPData");

                    using (StringReader stringReader = new StringReader(recvData.Substring(startIndex)))
                    using (XmlReader reader = XmlReader.Create(stringReader))
                    {
                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots.", false);

                        int locMapRefno = 0;
                        bool isLocMapSlotFound = false;
                        int sectLocMapRefno = 0;
                        int whsLocMapRefno = 0;
                        int coord1Refno = 0;
                        int coord2Refno = 0;
                        int coord3Refno = 0;

                        // f01
                        string slotName = string.Empty;

                        // f02
                        string slotBarCode = string.Empty;

                        // f03
                        int slotFKRefno = 0;

                        // f04
                        int sectFKRefno = 0;

                        // f05
                        string slotCoord1 = string.Empty;

                        // f06
                        string slotCoord2 = string.Empty;

                        // f07
                        string slotCoord3 = string.Empty;

                        Dictionary<string, int> slot1Lookup = new Dictionary<string, int>();
                        Dictionary<string, int> slot2Lookup = new Dictionary<string, int>();
                        Dictionary<string, int> slot3Lookup = new Dictionary<string, int>();

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                        while (!reader.EOF)
                        {
                            bool recordFilled = false;
                            string elementName = string.Empty;

                            while (!recordFilled && reader.Read())
                            {
                                // Fill one record
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    elementName = reader.Name;
                                }
                                else if (reader.NodeType == XmlNodeType.Text)
                                {
                                    switch (elementName)
                                    {
                                        case "seg":
                                            // beginning a segment
                                            break;
                                        case "f01":
                                            slotName = reader.Value;
                                            break;
                                        case "f02":
                                            slotBarCode = reader.Value;
                                            break;
                                        case "f03":
                                            slotFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f04":
                                            sectFKRefno = reader.ReadContentAsInt();
                                            break;
                                        case "f05":
                                            slotCoord1 = reader.Value;
                                            break;
                                        case "f06":
                                            slotCoord2 = reader.Value;
                                            break;
                                        case "f07":
                                            slotCoord3 = reader.Value;
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                                {
                                    recordFilled = true; // we are full
                                }
                            } // end of fill one record

                            if (!recordFilled)
                            {
                                continue;
                            }

                            errMsg = "Slot fk refno " + slotFKRefno;
                            errMsg += Environment.NewLine + "Slot name \'" + slotName + "\'";
                            errMsg += Environment.NewLine + "Slot bar code \'" + slotBarCode + "\'";
                            errMsg += Environment.NewLine + "Sect fk refno " + sectFKRefno;

                            List<tLocationMapSectionModel> resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", sectFKRefno);

                            if (resultsLocationMapSection?.Count > 0)
                            {
                                sectLocMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                whsLocMapRefno = resultsLocationMapSection[0].WarehouseLocationMapRefno;
                            }
                            else
                            {
                                // TODO: Resource Strings
                                throw new Exception("No results in tLocationMapSection for LocationMapSectionFKRefno " + sectFKRefno + Environment.NewLine + errMsg);
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 1 \'" + slotCoord1 + "\'";

                            if (!slot1Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord1, out coord1Refno))
                            {
                                string query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                                List<tLocationMapSlotCoordinateModel> resultsLocationMapSlotCoordinate1 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1), slotCoord1, whsLocMapRefno);

                                if (resultsLocationMapSlotCoordinate1?.Count > 0)
                                {
                                    coord1Refno = resultsLocationMapSlotCoordinate1[0].LocationMapSlotCoordinateRefno;
                                    slot1Lookup.Add(whsLocMapRefno.ToString() + slotCoord1, coord1Refno);
                                }
                                else
                                {
                                    throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 1. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord1 + " " + slotCoord1 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                                }
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 2 \'" + slotCoord2 + "\'";

                            if (!slot2Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord2, out coord2Refno))
                            {
                                string query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                                List<tLocationMapSlotCoordinateModel> resultsLocationMapSlotCoordinate2 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2), slotCoord2, whsLocMapRefno);

                                if (resultsLocationMapSlotCoordinate2?.Count > 0)
                                {
                                    coord2Refno = resultsLocationMapSlotCoordinate2[0].LocationMapSlotCoordinateRefno;
                                    slot2Lookup.Add(whsLocMapRefno.ToString() + slotCoord2, coord2Refno);
                                }
                                else
                                {
                                    throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 2. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord2 + " " + slotCoord2 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                                }
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 3 \'" + slotCoord3 + "\'";

                            if (!slot3Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord3, out coord3Refno))
                            {
                                string query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                                List<tLocationMapSlotCoordinateModel> resultsLocationMapSlotCoordinate3 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3), slotCoord3, whsLocMapRefno);

                                if (resultsLocationMapSlotCoordinate3?.Count > 0)
                                {
                                    coord3Refno = resultsLocationMapSlotCoordinate3[0].LocationMapSlotCoordinateRefno;
                                    slot3Lookup.Add(whsLocMapRefno.ToString() + slotCoord3, coord3Refno);
                                }
                                else
                                {
                                    throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 3. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord3 + " " + slotCoord3 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                                }
                            }

                            isLocMapSlotFound = false;

                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                List<tLocationMapSlotModel> results = transaction.Query<tLocationMapSlotModel>("SELECT LocationMapSlotFKRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", slotFKRefno);

                                if (results?.Count > 0)
                                {
                                    isLocMapSlotFound = true;

                                    string queryUpdate = "UPDATE tLocationMapSlot "
                                        + "SET SectionLocationMapRefno = " + sectLocMapRefno + ","
                                        + "LocationMapSlotCode = " + slotFKRefno.ToString()
                                        + "LocationMapSlotName = \'" + slotName + "\',"
                                        + "SlotCoord1LocationMapRefno = " + coord1Refno + ","
                                        + "SlotCoord2LocationMapRefno = " + coord2Refno + ","
                                        + "SlotCoord3LocationMapRefno = " + coord3Refno + ","
                                        + "LocationMapSlotBarCode = \'" + slotBarCode + "\' "
                                        + "WHERE LocationMapSlotFKRefno = " + slotFKRefno;

                                    transaction.Execute(queryUpdate);
                                }
                            }

                            if (!isLocMapSlotFound)
                            {
                                transaction.Execute("INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)", Convert.ToInt32(SysCommon.LocMapSubTypes.Slot));

                                locMapRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tLocationMap"));

                                string queryInsert = "INSERT INTO tLocationMapSlot " +
                                "(" +
                                "LocationMapRefno, " +
                                "LocationMapSubType, " +
                                "SectionLocationMapRefno, " +
                                "LocationMapSlotCode, " +
                                "LocationMapSlotName, " +
                                "SlotCoord1LocationMapRefno, " +
                                "SlotCoord2LocationMapRefno, " +
                                "SlotCoord3LocationMapRefno, " +
                                "LocationMapSlotFKRefno, " +
                                "LocationMapSlotBarCode, " +
                                "WarehouseLocationMapRefno" +
                                ") VALUES (" +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                                List<object> parametersInsert = new List<object>()
                                {
                                    locMapRefno,
                                    Convert.ToInt32(SysCommon.LocMapSubTypes.Slot),
                                    sectLocMapRefno,
                                    slotFKRefno,
                                    slotName,
                                    coord1Refno,
                                    coord2Refno,
                                    coord3Refno,
                                    slotFKRefno,
                                    slotBarCode,
                                    whsLocMapRefno
                                };

                                transaction.Execute(queryInsert, parametersInsert);
                            }

                            ++recordCount;
                            if (recordCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        } // end of record data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots failed.", false);

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving slots failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override async Task<string> Create(int whsLocMapRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();
            int locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (whsLocMapRefno != 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
            }

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SLOTS</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno.ToString() + "</LocationMapWarehouseFKRefno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
