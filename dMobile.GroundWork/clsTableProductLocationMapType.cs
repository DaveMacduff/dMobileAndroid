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

    public class TableProductLocationMapType : TableBase
    {
        #region Constructors

        public TableProductLocationMapType()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableProductLocationMapType";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations.", true);
#endif

                using (SqlCeCommand cmndLocMapWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                    cmndLocMapWhs.CommandType = CommandType.TableDirect;
                    cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                    using (SqlCeDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                    {
                        string whsLocMapName = string.Empty;
                        int whsLocMapFKRefno = 0;
                        int whsLocMapRefno = 0;
                        string reqData = string.Empty;

                        while (drLocMapWhs.Read())
                        {
                            whsLocMapFKRefno = drLocMapWhs.GetInt32((int)SysCommon.LocMapWhsFields.FKRefno);
                            whsLocMapName = drLocMapWhs.GetString((int)SysCommon.LocMapWhsFields.Name);
                            whsLocMapRefno = drLocMapWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
#if MULTILANGUAGE
                  string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ForWhs), whsLocMapName),
#else
 string.Format("For whs \'{0}\'.", whsLocMapName),
#endif
 true);

                            ////GC.Collect();
                            ////GC.WaitForPendingFinalizers();
                            ////this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default floor locations.", true);
                            ////reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Floor, whsLocMapFKRefno);
                            ////if (0 < reqData.Length)
                            ////{
                            ////  if (reqData.Contains("<message>ERROR</message>"))
                            ////  {
                            ////    XmlDocument reqErr = new XmlDocument();
                            ////    reqErr.LoadXml(reqData);
                            ////    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                            ////  }

                            ////  this.Save(reqData, whsLocMapRefno);
                            ////}

                            //GC.Collect();
                            //GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
              this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultPickingLocations), true);
#else
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);
#endif
                            reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Picking, whsLocMapFKRefno);
                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                //this.Save(reqData, whsLocMapRefno);
                                this.SaveOpt(reqData, whsLocMapRefno);
                            }

                            //GC.Collect();
                            //GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
              this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultReceivingLocations), true);
#else
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);
#endif
                            reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Receiving, whsLocMapFKRefno);
                            if (0 < reqData.Length)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                //this.Save(reqData, whsLocMapRefno);
                                this.SaveOpt(reqData, whsLocMapRefno);
                            }
                        }
                    }
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductLocationsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations complete.", true);
#endif
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductLocationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingProductLocationsFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding product locations failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductLocationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding product locations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingProductLocationsFailed), ex);
#else
                throw new Exception("Seeding product locations failed.", ex);
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
                using (SqlCeCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tProductLocationMapType";
                    cmndStatus.CommandType = CommandType.Text;
                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ProductLocations), rowsAffected),
#else
 string.Format("{0} product locations.", rowsAffected),
#endif
 true);
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ProductLocationsStatusFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "product locations status failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredProductLocationStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Product location status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);
#endif

                using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tProductLocationMapType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product locations.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductLocations), ex);
#else
                throw new Exception("Database exception occurred. Purging product locations.", ex);
#endif
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <param name="whsLocMapFKRefno"></param>
        /// <returns></returns>
        protected override string SendRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            return this.SendComm(this.CreateRequest(prodLocMapType, whsLocMapFKRefno));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="whsLocMapRefno"></param>
        protected override void Save(string reqData, int whsLocMapRefno)
        {
            int prodRefno = 0;
            int locMapRefno = 0;
            int prodLocMapType = 0;
            int locMapFKRefno = 0;
            int locMapSubType = 0;
            int prodFKRefno = 0;
            int currProdLocMapRefno = 0;
            string errMsg = string.Empty;
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

            try
            {
                using (DataSet ds = new DataSet())
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocations), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
 string.Format("Records {0}", progMax),
#endif
 false);

                        using (SqlCeCommand cmndLocMapType = conn.CreateCommand())
                        {
                            cmndLocMapType.CommandText = "tProductLocationMapType";
                            cmndLocMapType.CommandType = CommandType.TableDirect;
                            cmndLocMapType.IndexName =
                                  "FK_ProductLocationMapTypeOnWarehouseLocationMapRefnoAndProductRefnoAndProductLocationMapTypeEnum";

                            using (SqlCeCommand cmndLocMapTypeUpdate = conn.CreateCommand())
                            {
                                cmndLocMapTypeUpdate.CommandText = string.Empty;
                                cmndLocMapTypeUpdate.CommandType = CommandType.Text;
                                cmndLocMapTypeUpdate.IndexName = string.Empty;

                                using (SqlCeDataReader drLocMapType = cmndLocMapType.ExecuteReader())
                                {
                                    using (SqlCeCommand cmndProd = conn.CreateCommand())
                                    {
                                        cmndProd.CommandText = "tProduct";
                                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                        cmndProd.CommandType = CommandType.TableDirect;

                                        using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                                        {
                                            using (SqlCeCommand cmndWhs = conn.CreateCommand())
                                            {
                                                cmndWhs.CommandText = "tLocationMapWarehouse";
                                                cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                                cmndWhs.CommandType = CommandType.TableDirect;

                                                using (SqlCeDataReader drWhs = cmndWhs.ExecuteReader())
                                                {
                                                    using (SqlCeCommand cmndSect = conn.CreateCommand())
                                                    {
                                                        cmndSect.CommandText = "tLocationMapSection";
                                                        cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                                        cmndSect.CommandType = CommandType.TableDirect;

                                                        using (SqlCeDataReader drSect = cmndSect.ExecuteReader())
                                                        {
                                                            using (SqlCeCommand cmndSlot = conn.CreateCommand())
                                                            {
                                                                cmndSlot.CommandText = "tLocationMapSlot";
                                                                cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                                                cmndSlot.CommandType = CommandType.TableDirect;

                                                                using (SqlCeDataReader drSlot = cmndSlot.ExecuteReader())
                                                                {
                                                                    using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(
                                                                        ResultSetOptions.Updatable))
                                                                    {
                                                                        SqlCeUpdatableRecord rec = rs.CreateRecord();
                                                                        int progValue = 0;
                                                                        bool isLocMapTypeFound = false;

                                                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                                                        foreach (DataRow drow in ds.Tables["seg"].Rows)
                                                                        {
                                                                            prodFKRefno = Convert.ToInt32(drow["f04"]);
                                                                            errMsg = "FK refno " + prodFKRefno;
                                                                            if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                                                            {
                                                                                if (drProd.Read())
                                                                                {
                                                                                    prodRefno = Convert.ToInt32(
                                                                                        drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                                                                }
                                                                                else
                                                                                {
#if MULTILANGUAGE
                                          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodreadFailed) + Environment.NewLine + errMsg);
#else
                                                                                    throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
#endif
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
#else
"Unable to locate the product refno from the product fk refno in tProduct."
#endif
 + Environment.NewLine + errMsg);
                                                                            }

                                                                            locMapFKRefno = Convert.ToInt32(drow["f01"]);
                                                                            errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                                                                            // locMapSubType = Warehouse, Section, Slot.
                                                                            // Create error message for locMapSubType.
                                                                            locMapSubType = Convert.ToInt32(drow["f02"]);
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
                                                                                    throw new Exception(
                                                                                        string.Format(
#if MULTILANGUAGE
                                              Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
"Undefined location map subtype {0}",
#endif
 locMapSubType));
                                                                            }

                                                                            // prodLocMapType = Picking, Receiving, Floor.
                                                                            prodLocMapType = Convert.ToInt32(drow["f03"]);
                                                                            errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                                                                            // Find the key for the locMapSubType.
                                                                            switch (locMapSubType)
                                                                            {
                                                                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                                                                    if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                                                                    {
                                                                                        if (drWhs.Read())
                                                                                        {
                                                                                            locMapRefno = Convert.ToInt32(drWhs.GetValue(Convert.ToInt32(
                                                                                                SysCommon.LocMapWhsFields.LocMapRefno)));
                                                                                        }
                                                                                        else
                                                                                        {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine 
#else
                                                                                            throw new Exception("drWhs.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse)
#else
"Unable to locate the warehouse fk refno in tLocationMapWarehouse."
#endif
 + Environment.NewLine + errMsg);
                                                                                    }

                                                                                    break;

                                                                                case (int)SysCommon.LocMapSubTypes.Section:
                                                                                    if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                                                                    {
                                                                                        if (drSect.Read())
                                                                                        {
                                                                                            locMapRefno = Convert.ToInt32(
                                                                                                drSect.GetValue(
                                                                                                Convert.ToInt32(SysCommon.LocMapSectFields.LocMapRefno)));
                                                                                        }
                                                                                        else
                                                                                        {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine
#else
                                                                                            throw new Exception("drSect.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSectionFkRefnoInTlocationmapsection)
#else
"Unable to locate section fk refno in tLocationMapSection."
#endif
 + Environment.NewLine + errMsg);
                                                                                    }

                                                                                    break;

                                                                                case (int)SysCommon.LocMapSubTypes.Slot:
                                                                                    if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                                                                    {
                                                                                        if (drSlot.Read())
                                                                                        {
                                                                                            locMapRefno = Convert.ToInt32(
                                                                                                drSlot.GetValue(
                                                                                                Convert.ToInt32(SysCommon.LocMapSlotFields.LocMapRefno)));
                                                                                        }
                                                                                        else
                                                                                        {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrslotreadFailed) + Environment.NewLine 
#else
                                                                                            throw new Exception("drSlot.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSlotFkRefnoInTlocationmapslot)
#else
"Unable to locate slot fk refno in tLocationMapSlot."
#endif
 + Environment.NewLine + errMsg);
                                                                                    }

                                                                                    break;

                                                                                default:
                                                                                    throw new Exception(
                                                                                        string.Format(
#if MULTILANGUAGE
                                              Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
"Undefined location map subtype {0}",
#endif
 locMapSubType));
                                                                            }

                                                                            isLocMapTypeFound = false;

                                                                            currProdLocMapRefno = 0;
                                                                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                                                            {
                                                                                if (drLocMapType.Seek(
                                                                                    DbSeekOptions.FirstEqual,
                                                                                    new object[] { whsLocMapRefno, prodRefno, prodLocMapType }))
                                                                                {
                                                                                    if (drLocMapType.Read())
                                                                                    {
                                                                                        currProdLocMapRefno = Convert.ToInt32(
                                                                                            drLocMapType.GetValue(
                                                                                            Convert.ToInt32(SysCommon.ProdLocMapTypeFields.LocMapRefno)));
                                                                                        isLocMapTypeFound = true;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmaptypereadFailed) + Environment.NewLine + errMsg);
#else
"drLocMapType.Read() failed." + Environment.NewLine + errMsg);
#endif
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (isLocMapTypeFound)
                                                                            {
                                                                                if (currProdLocMapRefno != locMapRefno)
                                                                                {
                                                                                    // Update the LocMapRefno if different
                                                                                    cmndLocMapTypeUpdate.CommandText = "UPDATE tProductLocationMapType"
                                                                                      + " SET LocationMapRefno = " + locMapRefno
                                                                                      + " WHERE ProductRefno = " + prodRefno
                                                                                      + " AND WarehouseLocationMapRefno = " + whsLocMapRefno
                                                                                      + " AND ProductLocationMapTypeEnum = " + prodLocMapType;
                                                                                    cmndLocMapTypeUpdate.ExecuteNonQuery();
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                rec.SetInt32(
                                                                                    Convert.ToInt32(SysCommon.ProdLocMapTypeFields.ProdRefno),
                                                                                    prodRefno);

                                                                                rec.SetInt32(
                                                                                    Convert.ToInt32(SysCommon.ProdLocMapTypeFields.WhsLocMapRefno),
                                                                                    whsLocMapRefno);

                                                                                rec.SetInt32(
                                                                                    Convert.ToInt32(SysCommon.ProdLocMapTypeFields.EnumType),
                                                                                    prodLocMapType);

                                                                                rec.SetInt32(
                                                                                    Convert.ToInt32(SysCommon.ProdLocMapTypeFields.LocMapRefno),
                                                                                    locMapRefno);

                                                                                rs.Insert(rec);
                                                                            }

                                                                            progValue++;
                                                                            if (progValue % 100 == 0)
                                                                            {
                                                                                this.DataAccessProgressOutHandler(
                                                                                    this,
                                                                                    EventArgs.Empty,
                                                                                    progValue,
                                                                                    0,
                                                                                    progMax);
                                                                            }
                                                                        }

                                                                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                                                    }

                                                                    drSlot.Close();
                                                                }
                                                            }

                                                            drSect.Close();
                                                        }
                                                    }

                                                    drWhs.Close();
                                                }
                                            }

                                            drProd.Close();
                                        }
                                    }

                                    drLocMapType.Close();
                                }
                            }
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocationsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocationsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);
#endif

                // Duplicate key violations
                if (ex.NativeError == 25016)
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
                }

                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductLocationsFailed) + Environment.NewLine + errMsg,
#else
"Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="whsLocMapRefno"></param>
        protected void SaveOpt(string reqData, int whsLocMapRefno)
        {
            string errMsg = string.Empty;
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

            try
            {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif

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


#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocations), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations.", false);
#endif



                    using (SqlCeCommand cmndLocMapType = conn.CreateCommand())
                    using (SqlCeCommand cmndLocMapTypeUpdate = conn.CreateCommand())
                    using (SqlCeCommand cmndProd = conn.CreateCommand())
                    using (SqlCeCommand cmndWhs = conn.CreateCommand())
                    using (SqlCeCommand cmndSect = conn.CreateCommand())
                    using (SqlCeCommand cmndSlot = conn.CreateCommand())
                    {
                        cmndLocMapType.CommandText = "tProductLocationMapType";
                        cmndLocMapType.CommandType = CommandType.TableDirect;
                        cmndLocMapType.IndexName =
                              "FK_ProductLocationMapTypeOnWarehouseLocationMapRefnoAndProductRefnoAndProductLocationMapTypeEnum";


                        cmndLocMapTypeUpdate.CommandText = string.Empty;
                        cmndLocMapTypeUpdate.CommandType = CommandType.Text;
                        cmndLocMapTypeUpdate.IndexName = string.Empty;

                        cmndProd.CommandText = "tProduct";
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        cmndProd.CommandType = CommandType.TableDirect;

                        cmndWhs.CommandText = "tLocationMapWarehouse";
                        cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                        cmndWhs.CommandType = CommandType.TableDirect;

                        cmndSect.CommandText = "tLocationMapSection";
                        cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                        cmndSect.CommandType = CommandType.TableDirect;

                        cmndSlot.CommandText = "tLocationMapSlot";
                        cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                        cmndSlot.CommandType = CommandType.TableDirect;

                        using (SqlCeDataReader drLocMapType = cmndLocMapType.ExecuteReader())
                        using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                        using (SqlCeDataReader drWhs = cmndWhs.ExecuteReader())
                        using (SqlCeDataReader drSect = cmndSect.ExecuteReader())
                        using (SqlCeDataReader drSlot = cmndSlot.ExecuteReader())
                        using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(
                            ResultSetOptions.Updatable))
                        {
                            SqlCeUpdatableRecord rec = rs.CreateRecord();
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
                                    continue;

                                errMsg = "FK refno " + prodFKRefno;
                                if (drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
                                {
                                    if (drProd.Read())
                                    {
                                        prodRefno = Convert.ToInt32(
                                            drProd.GetValue((int)SysCommon.ProductFields.Refno));
                                    }
                                    else
                                    {
#if MULTILANGUAGE
                                          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drProd.Read() failed." + Environment.NewLine + errMsg);
#endif
                                    }
                                }
                                else
                                {
                                    throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
#else
"Unable to locate the product refno from the product fk refno in tProduct."
#endif
 + Environment.NewLine + errMsg);
                                }

                                errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;

                                // locMapSubType = Warehouse, Section, Slot.
                                // Create error message for locMapSubType.
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
                                        throw new Exception(
                                            string.Format(
#if MULTILANGUAGE
                                              Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
"Undefined location map subtype {0}",
#endif
 locMapSubType));
                                }

                                // prodLocMapType = Picking, Receiving, Floor.
                                errMsg += Environment.NewLine + "Map type " + prodLocMapType;

                                // Find the key for the locMapSubType.
                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                        if (!warehouseLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                        {
                                            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            {
                                                if (drWhs.Read())
                                                {
                                                    locMapRefno = Convert.ToInt32(drWhs.GetValue(Convert.ToInt32(
                                                        SysCommon.LocMapWhsFields.LocMapRefno)));
                                                    warehouseLookup.Add(locMapFKRefno, locMapRefno);
                                                }
                                                else
                                                {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine 
#else
                                                    throw new Exception("drWhs.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse)
#else
"Unable to locate the warehouse fk refno in tLocationMapWarehouse."
#endif
 + Environment.NewLine + errMsg);
                                            }
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        if (!sectionLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                        {
                                            if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            {
                                                if (drSect.Read())
                                                {
                                                    locMapRefno = Convert.ToInt32(
                                                        drSect.GetValue(
                                                        Convert.ToInt32(SysCommon.LocMapSectFields.LocMapRefno)));
                                                    sectionLookup.Add(locMapFKRefno, locMapRefno);
                                                }
                                                else
                                                {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine
#else
                                                    throw new Exception("drSect.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSectionFkRefnoInTlocationmapsection)
#else
"Unable to locate section fk refno in tLocationMapSection."
#endif
 + Environment.NewLine + errMsg);
                                            }
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        if (!slotLookup.TryGetValue(locMapFKRefno, out locMapRefno))
                                        {
                                            if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            {
                                                if (drSlot.Read())
                                                {
                                                    locMapRefno = Convert.ToInt32(
                                                        drSlot.GetValue(
                                                        Convert.ToInt32(SysCommon.LocMapSlotFields.LocMapRefno)));
                                                    slotLookup.Add(locMapFKRefno, locMapRefno);
                                                }
                                                else
                                                {
#if MULTILANGUAGE
                                              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrslotreadFailed) + Environment.NewLine 
#else
                                                    throw new Exception("drSlot.Read() failed." + Environment.NewLine
#endif
 + errMsg);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSlotFkRefnoInTlocationmapslot)
#else
"Unable to locate slot fk refno in tLocationMapSlot."
#endif
 + Environment.NewLine + errMsg);
                                            }
                                        }

                                        break;

                                    default:
                                        throw new Exception(
                                            string.Format(
#if MULTILANGUAGE
                                              Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
"Undefined location map subtype {0}",
#endif
 locMapSubType));
                                }

                                isLocMapTypeFound = false;

                                currProdLocMapRefno = 0;
                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    if (drLocMapType.Seek(
                                        DbSeekOptions.FirstEqual,
                                        new object[] { whsLocMapRefno, prodRefno, prodLocMapType }))
                                    {
                                        if (drLocMapType.Read())
                                        {
                                            currProdLocMapRefno = Convert.ToInt32(
                                                drLocMapType.GetValue(
                                                Convert.ToInt32(SysCommon.ProdLocMapTypeFields.LocMapRefno)));
                                            isLocMapTypeFound = true;
                                        }
                                        else
                                        {
                                            throw new Exception(
#if MULTILANGUAGE
                                                Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmaptypereadFailed) + Environment.NewLine + errMsg);
#else
"drLocMapType.Read() failed." + Environment.NewLine + errMsg);
#endif
                                        }
                                    }
                                }

                                if (isLocMapTypeFound)
                                {
                                    if (currProdLocMapRefno != locMapRefno)
                                    {
                                        // Update the LocMapRefno if different
                                        cmndLocMapTypeUpdate.CommandText = "UPDATE tProductLocationMapType"
                                          + " SET LocationMapRefno = " + locMapRefno
                                          + " WHERE ProductRefno = " + prodRefno
                                          + " AND WarehouseLocationMapRefno = " + whsLocMapRefno
                                          + " AND ProductLocationMapTypeEnum = " + prodLocMapType;
                                        cmndLocMapTypeUpdate.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.ProdLocMapTypeFields.ProdRefno),
                                        prodRefno);

                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.ProdLocMapTypeFields.WhsLocMapRefno),
                                        whsLocMapRefno);

                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.ProdLocMapTypeFields.EnumType),
                                        prodLocMapType);

                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.ProdLocMapTypeFields.LocMapRefno),
                                        locMapRefno);

                                    rs.Insert(rec);
                                }

                                ++recordCount;
                                if (recordCount % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, (recordCount / 10) % 100, 0, 100);
                                }
                            } // end of data

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), recordCount),
#else
 string.Format("Records {0}", recordCount),
#endif
 false);
                            warehouseLookup.Clear();
                            sectionLookup.Clear();
                            slotLookup.Clear();

                            warehouseLookup = null;
                            sectionLookup = null;
                            slotLookup = null;

                            drSlot.Close();
                            drSect.Close();
                            drWhs.Close();
                            drProd.Close();
                            drLocMapType.Close();
                        }
                    }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocationsComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations complete.", false);
#endif
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingProductLocationsFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving product locations failed.", false);
#endif

                // Duplicate key violations
                if (ex.NativeError == 25016)
                {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
                }

                throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingProductLocationsFailed) + Environment.NewLine + errMsg,
#else
"Database exception occurred. Saving product locations failed." + Environment.NewLine + errMsg,
#endif
 ex);
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCT LOCATION MAP TYPES</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<product_location_map_type>" + prodLocMapType + "</product_location_map_type>")
                + ("<whs_id>" + whsLocMapFKRefno + "</whs_id>")
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
