namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

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
        public override void Request(int whsLocMapRefno)
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots.", true);

                string reqData = string.Empty;

                // TODO: Uncomment GC calls
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                if (0 < whsLocMapRefno)
                {
                    using (SQLiteCommand cmndWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = @locationMapRefno";
                        cmndWhs.CommandType = CommandType.Text;
                        //cmndWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";
                        cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapRefno", whsLocMapRefno));

                        using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                        {
                            //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapRefno }))
                            //{
                            if (drWhs.Read())
                            {
                                // TODO: Resource Strings
                                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For whs \'{0}\'.", drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)), true);
                            }
                            else
                            {
                                // TODO: Resource Strings
                                throw new Exception("drWhs.Read() failed.");
                            }
                            //}
                            //else
                            //{
                            //    // TODO: Resource Strings
                            //    throw new Exception(string.Format("Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.", whsLocMapRefno));
                            //}

                            drWhs.Close();
                        }
                    }
                }

                reqData = this.Send(whsLocMapRefno);
                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    this.SaveOpt(reqData);
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSlotsFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding slots failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), ex);
#else
                throw new Exception("Seeding slots failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="sectRefno"></param>
        public override void PurgeData(int sectRefno)
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlots), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;

                    if (sectRefno == 0)
                    {
                        cmndPurge.CommandText = "DELETE FROM tLocationMapSlot";
                    }
                    else
                    {
                        cmndPurge.CommandText = "DELETE FROM tLocationMapSlot WHERE SectionLocationMapRefno = " + sectRefno;
                    }

                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlotsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlotsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSlotsFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging slots failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapSlot";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                      this,
                      EventArgs.Empty,
#if MULTILANGUAGE
            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Slots), rowsAffected),
#else
 string.Format("{0} slots.", rowsAffected),
#endif
 false);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSlotsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Slots status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override string Send(int whsLocMapRefno)
        {
            return this.SendComm(this.Create(whsLocMapRefno));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="recvData"></param>
        protected override void Save(string recvData)
        {
        }

        /// <summary>
        /// The SaveOpt
        /// </summary>
        /// <param name="recvData"></param>
        protected void SaveOpt(string recvData)
        {
            string errMsg = string.Empty;
            int recordCount = 0;

            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

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

                    using (SQLiteCommand cmndSlot = conn.CreateCommand())
                    using (SQLiteCommand cmndSect = conn.CreateCommand())
                    using (SQLiteCommand cmndCoord = conn.CreateCommand())
                    using (SQLiteCommand cmndLocMap = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteCommand cmndSlotUpdate = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        //cmndSlot.CommandText = "tLocationMapSlot";
                        //cmndSlot.CommandType = CommandType.Text;
                        //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

                        //cmndSect.CommandText = "tLocationMapSection";
                        //cmndSect.CommandType = CommandType.Text;
                        //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                        //cmndCoord.CommandText = "tLocationMapSlotCoordinate";
                        //cmndCoord.CommandType = CommandType.Text;
                        //cmndCoord.IndexName = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";

                        //cmndLocMap.CommandText = "tLocationMap";
                        //cmndLocMap.CommandType = CommandType.Text;

                        //cmndID.CommandText = "SELECT last_insert_rowid()";
                        //cmndID.CommandType = CommandType.Text;

                        //using (SQLiteDataReader drSlot = cmndSlot.ExecuteReader())
                        //using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
                        //using (SqlCeResultSet rsID = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
                        //using (SqlCeResultSet rs = cmndSlot.ExecuteResultSet(ResultSetOptions.Updatable))
                        //{
                        //    SqlCeUpdatableRecord recID = rsID.CreateRecord();
                        //    SqlCeUpdatableRecord rec = rs.CreateRecord();

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

                        System.Collections.Generic.Dictionary<string, int> slot1Lookup = new System.Collections.Generic.Dictionary<string, int>();
                        System.Collections.Generic.Dictionary<string, int> slot2Lookup = new System.Collections.Generic.Dictionary<string, int>();
                        System.Collections.Generic.Dictionary<string, int> slot3Lookup = new System.Collections.Generic.Dictionary<string, int>();

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

                            cmndSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                            cmndSect.CommandType = CommandType.Text;
                            cmndSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", sectFKRefno));

                            using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
                            {
                                //if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { sectFKRefno }))
                                //{
                                if (drSect.Read())
                                {
                                    sectLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.LocMapRefno);
                                    whsLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.WhsLocMapRefno);
                                }
                                else
                                {
                                    // TODO: Resource Strings
                                    throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
                                }
                                //}
                                //else
                                //{
                                //    // TODO: Resource Strings
                                //    throw new Exception("Unable to locate the section fk refno in tLocationMapSection." + Environment.NewLine + errMsg);
                                //}

                                drSect.Close();
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 1 \'" + slotCoord1 + "\'";

                            if (!slot1Lookup.TryGetValue(slotCoord1, out coord1Refno))
                            {
                                cmndCoord.CommandText = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = @locationMapSlotCoordinateEnum AND LocationMapSlotCoordinateCode = @locationMapSlotCoordinateCode ";
                                cmndCoord.CommandType = CommandType.Text;
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateEnum", SysCommon.LocMapSlotCoordTypes.Coord1));
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateCode", slotCoord1));

                                using (SQLiteDataReader drCoord = cmndCoord.ExecuteReader())
                                {
                                    //if (drCoord.Seek(DbSeekOptions.FirstEqual, new object[] { SysCommon.LocMapSlotCoordTypes.Coord1, slotCoord1 }))
                                    //{
                                    if (drCoord.Read())
                                    {
                                        coord1Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
                                        slot1Lookup.Add(slotCoord1, coord1Refno);
                                    }
                                    else
                                    {
                                        throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the slot coordinate 1 code in tLocationMapSlotCoordinate." + Environment.NewLine + errMsg);
                                    //}

                                    drCoord.Close();
                                }
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 2 \'" + slotCoord2 + "\'";

                            if (!slot2Lookup.TryGetValue(slotCoord2, out coord2Refno))
                            {
                                cmndCoord.Parameters.Clear();
                                cmndCoord.CommandText = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = @locationMapSlotCoordinateEnum AND LocationMapSlotCoordinateCode = @locationMapSlotCoordinateCode ";
                                cmndCoord.CommandType = CommandType.Text;
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateEnum", SysCommon.LocMapSlotCoordTypes.Coord2));
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateCode", slotCoord2));

                                using (SQLiteDataReader drCoord = cmndCoord.ExecuteReader())
                                {
                                    //if (drCoord.Seek(DbSeekOptions.FirstEqual, new object[] { SysCommon.LocMapSlotCoordTypes.Coord2, slotCoord2 }))
                                    //{
                                    if (drCoord.Read())
                                    {
                                        coord2Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
                                        slot2Lookup.Add(slotCoord2, coord2Refno);
                                    }
                                    else
                                    {
                                        throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the slot coordinate 2 code in tLocationMapSlotCoordinate." + Environment.NewLine + errMsg);
                                    //}

                                    drCoord.Close();
                                }
                            }

                            errMsg += Environment.NewLine + "Slot coordinate 3 \'" + slotCoord3 + "\'";

                            if (!slot3Lookup.TryGetValue(slotCoord3, out coord3Refno))
                            {
                                cmndCoord.Parameters.Clear();
                                cmndCoord.CommandText = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = @locationMapSlotCoordinateEnum AND LocationMapSlotCoordinateCode = @locationMapSlotCoordinateCode ";
                                cmndCoord.CommandType = CommandType.Text;
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateEnum", SysCommon.LocMapSlotCoordTypes.Coord3));
                                cmndCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateCode", slotCoord3));

                                using (SQLiteDataReader drCoord = cmndCoord.ExecuteReader())
                                {
                                    //if (drCoord.Seek(DbSeekOptions.FirstEqual, new object[] { SysCommon.LocMapSlotCoordTypes.Coord3, slotCoord3 }))
                                    //{
                                    if (drCoord.Read())
                                    {
                                        coord3Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
                                        slot3Lookup.Add(slotCoord3, coord3Refno);
                                    }
                                    else
                                    {
                                        throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the slot coordinate 3 code in tLocationMapSlotCoordinate." + Environment.NewLine + errMsg);
                                    //}

                                    drCoord.Close();
                                }
                            }

                            isLocMapSlotFound = false;

                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                cmndSlot.CommandText = "SELECT LocationMapSlotFKRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                                cmndSlot.CommandType = CommandType.Text;
                                //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", slotFKRefno));

                                //if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { slotFKRefno }))
                                using (SQLiteDataReader drSlot = cmndSlot.ExecuteReader())
                                {
                                    if (drSlot.Read())
                                    {
                                        isLocMapSlotFound = true;

                                        cmndSlotUpdate.CommandType = CommandType.Text;
                                        //cmndSlotUpdate.IndexName = string.Empty;
                                        cmndSlotUpdate.CommandText = "UPDATE tLocationMapSlot "
                                            + "SET SectionLocationMapRefno = " + sectLocMapRefno + ","
                                            + "LocationMapSlotCode = " + slotFKRefno.ToString()
                                            + "LocationMapSlotName = \'" + slotName + "\',"
                                            + "SlotCoord1LocationMapRefno = " + coord1Refno + ","
                                            + "SlotCoord2LocationMapRefno = " + coord2Refno + ","
                                            + "SlotCoord3LocationMapRefno = " + coord3Refno + ","
                                            + "LocationMapSlotBarCode = \'" + slotBarCode + "\' "
                                            + "WHERE LocationMapSlotFKRefno = " + slotFKRefno;
                                        cmndSlotUpdate.ExecuteNonQuery();
                                    }

                                    drSlot.Close();
                                }
                            }

                            if (!isLocMapSlotFound)
                            {
                                cmndLocMap.CommandText = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (@locationMapSubType)";
                                cmndLocMap.CommandType = CommandType.Text;
                                cmndLocMap.Parameters.Add(new SQLiteParameter("@locationMapSubType", (int)SysCommon.LocMapSubTypes.Slot));
                                cmndLocMap.ExecuteNonQuery();

                                //recID.SetInt32((int)SysCommon.LocMapFields.SubType, (int)SysCommon.LocMapSubTypes.Slot);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tLocationMap";
                                cmndID.CommandType = CommandType.Text;

                                locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                cmndSlot.CommandText = "INSERT INTO tLocationMapSlot " +
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
                                "@locationMapRefno, " +
                                "@locationMapSubType, " +
                                "@sectionLocationMapRefno, " +
                                "@locationMapSlotCode, " +
                                "@locationMapSlotName, " +
                                "@slotCoord1LocationMapRefno, " +
                                "@slotCoord2LocationMapRefno, " +
                                "@slotCoord3LocationMapRefno, " +
                                "@locationMapSlotFKRefno, " +
                                "@locationMapSlotBarCode, " +
                                "@warehouseLocationMapRefno" +
                                ")";
                                cmndSlot.CommandType = CommandType.Text;
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSubType", (int)SysCommon.LocMapSubTypes.Slot));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@sectionLocationMapRefno", sectLocMapRefno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotCode", slotFKRefno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotName", slotName));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@slotCoord1LocationMapRefno", coord1Refno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@slotCoord2LocationMapRefno", coord2Refno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@slotCoord3LocationMapRefno", coord3Refno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", slotFKRefno));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotBarCode", slotBarCode));
                                cmndSlot.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));

                                cmndSlot.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.SectLocMapRefno, sectLocMapRefno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord1LocMapRefno, coord1Refno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord2LocMapRefno, coord2Refno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord3LocMapRefno, coord3Refno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.LocMapRefno, locMapRefno);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.LocMapSubType, (int)SysCommon.LocMapSubTypes.Slot);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.Code, slotFKRefno);
                                //rec.SetString((int)SysCommon.LocMapSlotFields.Name, slotName);
                                //rec.SetInt32((int)SysCommon.LocMapSlotFields.FKRefno, slotFKRefno);
                                //rec.SetString((int)SysCommon.LocMapSlotFields.BarCode, slotBarCode);

                                //rs.Insert(rec);
                            }

                            ++recordCount;

                            if (recordCount % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                            }
                        } // end of record data

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                        //drCoord.Close();
                        //drSect.Close();
                        //drSlot.Close();
                        //}

                        transaction.Commit();
                    }


                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", recordCount), false);

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots complete.", false);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots failed.", false);

                //// Duplicate key violations
                //if (ex.NativeError == 25016)
                //{
                //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                //}

                // TODO: Resource Strings
                throw new Exception("Database exception occurred. Saving slots failed." + Environment.NewLine + errMsg, ex);
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override string Create(int whsLocMapRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();
            int locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (whsLocMapRefno != 0)
            {
                locMapWhsFKRefno = locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
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
