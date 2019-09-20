namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.IO;

    public class TableLocationMapSlot : TableBase
    {
        #region Constructors

        public TableLocationMapSlot()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlot";
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override void Request(int whsLocMapRefno)
        {
//            try
//            {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlots), true);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots.", true);
//#endif

//                string reqData = string.Empty;

//                GC.Collect();
//                GC.WaitForPendingFinalizers();

//                if (0 < whsLocMapRefno)
//                {
//                    using (SQLiteCommand cmndWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//                    {
//                        cmndWhs.CommandText = "tLocationMapWarehouse";
//                        cmndWhs.CommandType = CommandType.TableDirect;
//                        cmndWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";

//                        using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
//                        {
//                            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapRefno }))
//                            {
//                                if (drWhs.Read())
//                                {
//                                    this.DataAccessStatusOutHandler(
//                                        this,
//                                        EventArgs.Empty,
//#if MULTILANGUAGE
//                      string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ForWhs), drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)),
//#else
// string.Format("For whs \'{0}\'.", drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)),
//#endif
// true);
//                                }
//                                else
//                                {
//#if MULTILANGUAGE
//                  throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
//#else
//                                    throw new Exception("drWhs.Read() failed.");
//#endif
//                                }
//                            }
//                            else
//                            {
//                                throw new Exception(
//                                    string.Format(
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseNameUsingTheWarehouseRefnoInTlocationmapwarehouse),
//#else
//"Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.",
//#endif
// whsLocMapRefno));
//                            }

//                            drWhs.Close();
//                        }
//                    }
//                }

//                reqData = this.Send(whsLocMapRefno);
//                if (0 < reqData.Length)
//                {
//                    if (reqData.Contains("<message>ERROR</message>"))
//                    {
//                        XmlDocument reqErr = new XmlDocument();
//                        reqErr.LoadXml(reqData);
//                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
//                    }

//                    this.SaveOpt(reqData);
//                }

//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsComplete), true);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots complete.", true);
//#endif
//            }
//            catch (SQLiteException ex)
//            {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), true);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);
//#endif

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSlotsFailed), ex);
//#else
//                throw new Exception("Database exception occurred. Seeding slots failed.", ex);
//#endif
//            }
//            catch (Exception ex)
//            {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), true);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);
//#endif

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotsFailed), ex);
//#else
//                throw new Exception("Seeding slots failed.", ex);
//#endif
//            }
        }

        /// <summary>
        /// 
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
        ///
        /// </summary>
        /// <remarks></remarks>
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

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <returns></returns>
        protected override string Send(int whsLocMapRefno)
        {
            return this.SendComm(this.Create(whsLocMapRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recvData"></param>
        protected override void Save(string recvData)
        {
//            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//            string errMsg = string.Empty;

//            try
//            {
//                using (DataSet ds = new DataSet())
//                {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif
//                    ds.ReadXml(SysCommon.GetXmlReader(ref recvData), XmlReadMode.Auto);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlots), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots.", false);
//#endif
//                    if (ds.Tables.Count != 0)
//                    {
//                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

//                        this.DataAccessStatusOutHandler(
//                            this,
//                            EventArgs.Empty,
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
//#else
// string.Format("Records {0}", progMax),
//#endif
// false);

//                        using (SQLiteCommand cmndSlot = conn.CreateCommand())
//                        {
//                            cmndSlot.CommandText = "tLocationMapSlot";
//                            cmndSlot.CommandType = CommandType.TableDirect;
//                            cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                            using (SQLiteDataReader drSlot = cmndSlot.ExecuteReader())
//                            {
//                                using (SQLiteCommand cmndSect = conn.CreateCommand())
//                                {
//                                    cmndSect.CommandText = "tLocationMapSection";
//                                    cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
//                                    cmndSect.CommandType = CommandType.TableDirect;

//                                    using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
//                                    {
//                                        using (SQLiteCommand cmndCoord1 = conn.CreateCommand())
//                                        {
//                                            cmndCoord1.CommandText = "tLocationMapSlotCoordinate";
//                                            cmndCoord1.IndexName =
//                                                "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";
//                                            cmndCoord1.CommandType = CommandType.TableDirect;

//                                            using (SQLiteCommand cmndLocMap = conn.CreateCommand())
//                                            {
//                                                cmndLocMap.CommandText = "tLocationMap";
//                                                cmndLocMap.CommandType = CommandType.TableDirect;

//                                                using (SQLiteCommand cmndID = conn.CreateCommand())
//                                                {
//                                                    cmndID.CommandText = "SELECT @@IDENTITY";
//                                                    cmndID.CommandType = CommandType.Text;

//                                                    using (SqlCeResultSet rsID = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
//                                                    {
//                                                        SqlCeUpdatableRecord recID = rsID.CreateRecord();

//                                                        using (SQLiteDataReader drCoord = cmndCoord1.ExecuteReader())
//                                                        {
//                                                            using (SqlCeResultSet rs = cmndSlot.ExecuteResultSet(ResultSetOptions.Updatable))
//                                                            {
//                                                                SqlCeUpdatableRecord rec = rs.CreateRecord();
//                                                                int locMapRefno = 0;
//                                                                int sectFKRefno = 0;
//                                                                string slotCoord = string.Empty;
//                                                                int progValue = 0;
//                                                                bool isLocMapSlotFound = false;
//                                                                int slotFKRefno = 0;
//                                                                int sectLocMapRefno = 0;
//                                                                int whsLocMapRefno = 0;
//                                                                int coord1Refno = 0;
//                                                                int coord2Refno = 0;
//                                                                int coord3Refno = 0;
//                                                                string slotName = string.Empty;
//                                                                string slotBarCode = string.Empty;

//                                                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                                                foreach (DataRow drow in ds.Tables["seg"].Rows)
//                                                                {
//                                                                    slotFKRefno = Convert.ToInt32(Convert.ToInt32(drow["f03"]));
//                                                                    errMsg = "Slot fk refno " + slotFKRefno;
//                                                                    slotName = Convert.ToString(drow["f01"]);
//                                                                    errMsg += Environment.NewLine + "Slot name \'" + slotName + "\'";
//                                                                    slotBarCode = Convert.ToString(drow["f02"]);
//                                                                    errMsg += Environment.NewLine + "Slot bar code \'" + slotBarCode + "\'";
//                                                                    sectFKRefno = Convert.ToInt32(drow["f04"]);
//                                                                    errMsg += Environment.NewLine + "Sect fk refno " + sectFKRefno;

//                                                                    if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { sectFKRefno }))
//                                                                    {
//                                                                        if (drSect.Read())
//                                                                        {
//                                                                            sectLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.LocMapRefno);
//                                                                            whsLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.WhsLocMapRefno);
//                                                                        }
//                                                                        else
//                                                                        {
//#if MULTILANGUAGE
//                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine + errMsg);
//#else
//                                                                            throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
//#endif
//                                                                        }
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        throw new Exception(
//#if MULTILANGUAGE
//                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSectionFkRefnoInTlocationmapsection)
//#else
//"Unable to locate the section fk refno in tLocationMapSection."
//#endif
// + Environment.NewLine + errMsg);
//                                                                    }

//                                                                    slotCoord = Convert.ToString(drow["f05"]);
//                                                                    errMsg += Environment.NewLine + "Slot coordinate 1 \'" + slotCoord + "\'";
//                                                                    if (drCoord.Seek(
//                                                                        DbSeekOptions.FirstEqual,
//                                                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord1, slotCoord }))
//                                                                    {
//                                                                        if (drCoord.Read())
//                                                                        {
//                                                                            coord1Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                                                        }
//                                                                        else
//                                                                        {
//                                                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                                                        }
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        throw new Exception(
//#if MULTILANGUAGE
//                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinate)
//#else
//"Unable to locate the slot coordinate 1 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                                                    }

//                                                                    slotCoord = drow["f06"].ToString();
//                                                                    errMsg += Environment.NewLine + "Slot coordinate 2 \'" + slotCoord + "\'";
//                                                                    if (drCoord.Seek(
//                                                                        DbSeekOptions.FirstEqual,
//                                                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord2, slotCoord }))
//                                                                    {
//                                                                        if (drCoord.Read())
//                                                                        {
//                                                                            coord2Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                                                        }
//                                                                        else
//                                                                        {
//                                                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                                                        }
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        throw new Exception(
//#if MULTILANGUAGE
//                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatez)
//#else
//"Unable to locate the slot coordinate 2 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                                                    }

//                                                                    slotCoord = drow["f07"].ToString();
//                                                                    errMsg += Environment.NewLine + "Slot coordinate 3 \'" + slotCoord + "\'";
//                                                                    if (drCoord.Seek(
//                                                                        DbSeekOptions.FirstEqual,
//                                                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord3, slotCoord }))
//                                                                    {
//                                                                        if (drCoord.Read())
//                                                                        {
//                                                                            coord3Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                                                        }
//                                                                        else
//                                                                        {
//                                                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                                                        }
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        throw new Exception(
//#if MULTILANGUAGE
//                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatezz)
//#else
//"Unable to locate the slot coordinate 3 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                                                    }

//                                                                    isLocMapSlotFound = false;
//                                                                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                                                                    {
//                                                                        cmndSlot.CommandText = "tLocationMapSlot";
//                                                                        cmndSlot.CommandType = CommandType.TableDirect;
//                                                                        cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                                                                        if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { slotFKRefno }))
//                                                                        {
//                                                                            isLocMapSlotFound = true;
//                                                                            cmndSlot.CommandType = CommandType.Text;
//                                                                            cmndSlot.IndexName = string.Empty;
//                                                                            cmndSlot.CommandText = "UPDATE tLocationMapSlot "
//                                                                                + "SET SectionLocationMapRefno = " + sectLocMapRefno + ","
//                                                                                + "LocationMapSlotCode = " + slotFKRefno.ToString()
//                                                                                + "LocationMapSlotName = \'" + slotName + "\',"
//                                                                                + "SlotCoord1LocationMapRefno = " + coord1Refno + ","
//                                                                                + "SlotCoord2LocationMapRefno = " + coord2Refno + ","
//                                                                                + "SlotCoord3LocationMapRefno = " + coord3Refno + ","
//                                                                                + "LocationMapSlotBarCode = \'" + slotBarCode + "\' "
//                                                                                + "WHERE LocationMapSlotFKRefno = " + slotFKRefno;
//                                                                            cmndSlot.ExecuteNonQuery();
//                                                                        }
//                                                                    }

//                                                                    if (!isLocMapSlotFound)
//                                                                    {
//                                                                        cmndSlot.CommandText = "tLocationMapSlot";
//                                                                        cmndSlot.CommandType = CommandType.TableDirect;
//                                                                        cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                                                                        recID.SetInt32(
//                                                                            (int)SysCommon.LocMapFields.SubType,
//                                                                            (int)SysCommon.LocMapSubTypes.Slot);

//                                                                        rsID.Insert(recID);

//                                                                        locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.SectLocMapRefno, sectLocMapRefno);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.WhsLocMapRefno, whsLocMapRefno);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord1LocMapRefno, coord1Refno);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord2LocMapRefno, coord2Refno);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord3LocMapRefno, coord3Refno);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.LocMapRefno, locMapRefno);

//                                                                        rec.SetInt32(
//                                                                            (int)SysCommon.LocMapSlotFields.LocMapSubType,
//                                                                            (int)SysCommon.LocMapSubTypes.Slot);

//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.Code, slotFKRefno);
//                                                                        rec.SetString((int)SysCommon.LocMapSlotFields.Name, slotName);
//                                                                        rec.SetInt32((int)SysCommon.LocMapSlotFields.FKRefno, slotFKRefno);
//                                                                        rec.SetString((int)SysCommon.LocMapSlotFields.BarCode, slotBarCode);

//                                                                        rs.Insert(rec);
//                                                                    }

//                                                                    progValue++;
//                                                                    if (progValue % 100 == 0)
//                                                                    {
//                                                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                                                                    }
//                                                                }

//                                                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                                                            }

//                                                            drCoord.Close();
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }

//                                        drSect.Close();
//                                    }
//                                }

//                                drSlot.Close();
//                            }
//                        }
//                    }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotsComplete), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots complete.", false);
//#endif
//                }
//            }
//            catch (SQLiteException ex)
//            {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotsFailed), false);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots failed.", false);
//#endif

//                // Duplicate key violations
//                if (ex.NativeError == 25016)
//                {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//                }

//                throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSlotsFailed) + Environment.NewLine + errMsg,
//#else
//"Database exception occurred. Saving slots failed." + Environment.NewLine + errMsg,
//#endif
// ex);
//            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recvData"></param>
        protected void SaveOpt(string recvData)
        {
//            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//            string errMsg = string.Empty;
//            int recordCount = 0;
//            try
//            {
//                int startIndex = recvData.IndexOf("<VFPData");
//                using (StringReader stringReader = new StringReader(recvData.Substring(startIndex)))
//                using (XmlReader reader = XmlReader.Create(stringReader))
//                {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlots), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots.", false);
//#endif


//                    using (SQLiteCommand cmndSlot = conn.CreateCommand())
//                    using (SQLiteCommand cmndSect = conn.CreateCommand())
//                    using (SQLiteCommand cmndCoord1 = conn.CreateCommand())
//                    using (SQLiteCommand cmndLocMap = conn.CreateCommand())
//                    using (SQLiteCommand cmndID = conn.CreateCommand())
//                    {
//                        cmndSlot.CommandText = "tLocationMapSlot";
//                        cmndSlot.CommandType = CommandType.TableDirect;
//                        cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                        cmndSect.CommandText = "tLocationMapSection";
//                        cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
//                        cmndSect.CommandType = CommandType.TableDirect;

//                        cmndCoord1.CommandText = "tLocationMapSlotCoordinate";
//                        cmndCoord1.IndexName =
//                            "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";
//                        cmndCoord1.CommandType = CommandType.TableDirect;


//                        cmndLocMap.CommandText = "tLocationMap";
//                        cmndLocMap.CommandType = CommandType.TableDirect;

//                        cmndID.CommandText = "SELECT @@IDENTITY";
//                        cmndID.CommandType = CommandType.Text;

//                        using (SQLiteDataReader drSlot = cmndSlot.ExecuteReader())
//                        using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
//                        using (SqlCeResultSet rsID = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
//                        using (SQLiteDataReader drCoord = cmndCoord1.ExecuteReader())
//                        using (SqlCeResultSet rs = cmndSlot.ExecuteResultSet(ResultSetOptions.Updatable))
//                        {
//                            SqlCeUpdatableRecord recID = rsID.CreateRecord();
//                            SqlCeUpdatableRecord rec = rs.CreateRecord();

//                            int locMapRefno = 0;
                            
                            
//                            bool isLocMapSlotFound = false;

//                            int sectLocMapRefno = 0;
//                            int whsLocMapRefno = 0;
//                            int coord1Refno = 0;
//                            int coord2Refno = 0;
//                            int coord3Refno = 0;

//                            // f01
//                            string slotName = string.Empty;

//                            // f02
//                            string slotBarCode = string.Empty;

//                            // f03
//                            int slotFKRefno = 0;

//                            // f04
//                            int sectFKRefno = 0;

//                            // f05
//                            string slotCoord1 = string.Empty;

//                            // f06
//                            string slotCoord2 = string.Empty;

//                            // f07
//                            string slotCoord3 = string.Empty;


//                            System.Collections.Generic.Dictionary<string, int> slot1Lookup = new System.Collections.Generic.Dictionary<string, int>();
//                            System.Collections.Generic.Dictionary<string, int> slot2Lookup = new System.Collections.Generic.Dictionary<string, int>();
//                            System.Collections.Generic.Dictionary<string, int> slot3Lookup = new System.Collections.Generic.Dictionary<string, int>();

//                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

//                            while (!reader.EOF)
//                            {
//                                bool recordFilled = false;
//                                string elementName = string.Empty;

//                                while (!recordFilled && reader.Read())
//                                {
//                                    // Fill one record
//                                    if (reader.NodeType == XmlNodeType.Element)
//                                    {
//                                        elementName = reader.Name;
//                                    }
//                                    else if (reader.NodeType == XmlNodeType.Text)
//                                    {
//                                        switch (elementName)
//                                        {
//                                            case "seg":
//                                                // beginning a segment
//                                                break;
//                                            case "f01":
//                                                slotName = reader.Value;
//                                                break;
//                                            case "f02":
//                                                slotBarCode = reader.Value;
//                                                break;
//                                            case "f03":
//                                                slotFKRefno = reader.ReadContentAsInt();
//                                                break;
//                                            case "f04":
//                                                sectFKRefno = reader.ReadContentAsInt();
//                                                break;
//                                            case "f05":
//                                                slotCoord1 = reader.Value;
//                                                break;
//                                            case "f06":
//                                                slotCoord2 = reader.Value;
//                                                break;
//                                            case "f07":
//                                                slotCoord3 = reader.Value;
//                                                break;
//                                        }
//                                    }
//                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
//                                    {
//                                        recordFilled = true; // we are full
//                                    }
//                                } // end of fill one record

//                                if (!recordFilled)
//                                    continue;

//                                errMsg = "Slot fk refno " + slotFKRefno;
//                                errMsg += Environment.NewLine + "Slot name \'" + slotName + "\'";
                                
//                                errMsg += Environment.NewLine + "Slot bar code \'" + slotBarCode + "\'";
//                                errMsg += Environment.NewLine + "Sect fk refno " + sectFKRefno;

//                                if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { sectFKRefno }))
//                                {
//                                    if (drSect.Read())
//                                    {
//                                        sectLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.LocMapRefno);
//                                        whsLocMapRefno = drSect.GetInt32((int)SysCommon.LocMapSectFields.WhsLocMapRefno);
//                                    }
//                                    else
//                                    {
//#if MULTILANGUAGE
//                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine + errMsg);
//#else
//                                        throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
//#endif
//                                    }
//                                }
//                                else
//                                {
//                                    throw new Exception(
//#if MULTILANGUAGE
//                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSectionFkRefnoInTlocationmapsection)
//#else
//"Unable to locate the section fk refno in tLocationMapSection."
//#endif
// + Environment.NewLine + errMsg);
//                                }

                                
//                                errMsg += Environment.NewLine + "Slot coordinate 1 \'" + slotCoord1 + "\'";
//                                if (!slot1Lookup.TryGetValue(slotCoord1, out coord1Refno))
//                                {
//                                    if (drCoord.Seek(
//                                        DbSeekOptions.FirstEqual,
//                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord1, slotCoord1 }))
//                                    {
//                                        if (drCoord.Read())
//                                        {
//                                            coord1Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                            slot1Lookup.Add(slotCoord1, coord1Refno);
//                                        }
//                                        else
//                                        {
//                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        throw new Exception(
//#if MULTILANGUAGE
//                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinate)
//#else
//"Unable to locate the slot coordinate 1 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                    }
//                                }

                                
//                                errMsg += Environment.NewLine + "Slot coordinate 2 \'" + slotCoord2 + "\'";
//                                if (!slot2Lookup.TryGetValue(slotCoord2, out coord2Refno))
//                                {
//                                    if (drCoord.Seek(
//                                        DbSeekOptions.FirstEqual,
//                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord2, slotCoord2 }))
//                                    {
//                                        if (drCoord.Read())
//                                        {
//                                            coord2Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                            slot2Lookup.Add(slotCoord2, coord2Refno);
//                                        }
//                                        else
//                                        {
//                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        throw new Exception(
//#if MULTILANGUAGE
//                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatez)
//#else
//"Unable to locate the slot coordinate 2 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                    }
//                                }

//                                errMsg += Environment.NewLine + "Slot coordinate 3 \'" + slotCoord3 + "\'";
//                                if (!slot3Lookup.TryGetValue(slotCoord3, out coord3Refno))
//                                {
//                                    if (drCoord.Seek(
//                                        DbSeekOptions.FirstEqual,
//                                        new object[] { SysCommon.LocMapSlotCoordTypes.Coord3, slotCoord3 }))
//                                    {
//                                        if (drCoord.Read())
//                                        {
//                                            coord3Refno = drCoord.GetInt32((int)SysCommon.LocMapSlotCoordFields.Refno);
//                                            slot3Lookup.Add(slotCoord3, coord3Refno);
//                                        }
//                                        else
//                                        {
//                                            throw new Exception("drCoord.Read()" + Environment.NewLine + errMsg);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        throw new Exception(
//#if MULTILANGUAGE
//                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotCoordinateCodeInTlocationmapslotcoordinatezz)
//#else
//"Unable to locate the slot coordinate 3 code in tLocationMapSlotCoordinate."
//#endif
// + Environment.NewLine + errMsg);
//                                    }
//                                }

//                                isLocMapSlotFound = false;
//                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                                {
//                                    cmndSlot.CommandText = "tLocationMapSlot";
//                                    cmndSlot.CommandType = CommandType.TableDirect;
//                                    cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                                    if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { slotFKRefno }))
//                                    {
//                                        isLocMapSlotFound = true;
//                                        cmndSlot.CommandType = CommandType.Text;
//                                        cmndSlot.IndexName = string.Empty;
//                                        cmndSlot.CommandText = "UPDATE tLocationMapSlot "
//                                            + "SET SectionLocationMapRefno = " + sectLocMapRefno + ","
//                                            + "LocationMapSlotCode = " + slotFKRefno.ToString()
//                                            + "LocationMapSlotName = \'" + slotName + "\',"
//                                            + "SlotCoord1LocationMapRefno = " + coord1Refno + ","
//                                            + "SlotCoord2LocationMapRefno = " + coord2Refno + ","
//                                            + "SlotCoord3LocationMapRefno = " + coord3Refno + ","
//                                            + "LocationMapSlotBarCode = \'" + slotBarCode + "\' "
//                                            + "WHERE LocationMapSlotFKRefno = " + slotFKRefno;
//                                        cmndSlot.ExecuteNonQuery();
//                                    }
//                                }

//                                if (!isLocMapSlotFound)
//                                {
//                                    cmndSlot.CommandText = "tLocationMapSlot";
//                                    cmndSlot.CommandType = CommandType.TableDirect;
//                                    cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";

//                                    recID.SetInt32(
//                                        (int)SysCommon.LocMapFields.SubType,
//                                        (int)SysCommon.LocMapSubTypes.Slot);

//                                    rsID.Insert(recID);

//                                    locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.SectLocMapRefno, sectLocMapRefno);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.WhsLocMapRefno, whsLocMapRefno);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord1LocMapRefno, coord1Refno);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord2LocMapRefno, coord2Refno);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.SlotCoord3LocMapRefno, coord3Refno);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.LocMapRefno, locMapRefno);

//                                    rec.SetInt32(
//                                        (int)SysCommon.LocMapSlotFields.LocMapSubType,
//                                        (int)SysCommon.LocMapSubTypes.Slot);

//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.Code, slotFKRefno);
//                                    rec.SetString((int)SysCommon.LocMapSlotFields.Name, slotName);
//                                    rec.SetInt32((int)SysCommon.LocMapSlotFields.FKRefno, slotFKRefno);
//                                    rec.SetString((int)SysCommon.LocMapSlotFields.BarCode, slotBarCode);

//                                    rs.Insert(rec);
//                                }

//                                ++recordCount;

//                                if (recordCount % 100 == 0)
//                                {
//                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
//                                }
//                            } // end of record data

//                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

//                            drCoord.Close();
//                            drSect.Close();
//                            drSlot.Close();
//                        }
//                    }


//                    this.DataAccessStatusOutHandler(
//                        this,
//                        EventArgs.Empty,
//#if MULTILANGUAGE
// string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), recordCount),
//#else
// string.Format("Records {0}", recordCount),
//#endif
// false);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotsComplete), false);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots complete.", false);
//#endif
//                }
//            }
//            catch (SQLiteException ex)
//            {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotsFailed), false);
//#else
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots failed.", false);
//#endif

//                // Duplicate key violations
//                if (ex.NativeError == 25016)
//                {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//                }

//                throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSlotsFailed) + Environment.NewLine + errMsg,
//#else
//"Database exception occurred. Saving slots failed." + Environment.NewLine + errMsg,
//#endif
// ex);
//            }
        }


        /// <summary>
        /// 
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

        #endregion
    }
}
