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
    using dMobile.Resources.Custom;

    /// <summary>
    /// Defines the <see cref="TableLocationMapWarehouse" />
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
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouses), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses.", true);
#endif

                string reqData = string.Empty;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                reqData = this.Send();
                if (0 < reqData.Length)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    this.Save(reqData);
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses complete.", true);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesFailed), ex);
#else
                throw new Exception("Seeding warehouses failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouses), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tLocationMap WHERE LocationMapSubType = "
                        + SysCommon.LocMapSubTypes.Warehouse.ToString();
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehousesComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehousesFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingWarehousesFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging warehouses failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The GetLocMapWhsRefnoFromLocWhsFKRefno
        /// </summary>
        /// <param name="locMapWhsFKRefno"></param>
        /// <returns></returns>
        public int GetLocMapWhsRefnoFromLocWhsFKRefno(int locMapWhsFKRefno)
        {
            int whsLocMapRefno = 0;

            //      try
            //      {
            //        using (SQLiteCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //        {
            //          cmndGetWhs.CommandText = "tLocationMapWarehouse";
            //          cmndGetWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
            //          cmndGetWhs.CommandType = CommandType.Text;

            //          using (SQLiteDataReader drWhs = cmndGetWhs.ExecuteReader())
            //          {
            //            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
            //            {
            //              if (drWhs.Read())
            //              {
            //                whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
            //              }
            //              else
            //              {
            //#if MULTILANGUAGE
            //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
            //#else
            //                throw new Exception("drWhs.Read() failed.");
            //#endif
            //              }
            //            }
            //            else
            //            {
            //              throw new Exception(
            //                string.Format(
            //#if MULTILANGUAGE
            //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateWarehouseRefnoUsingTheWarehouseFkRefnoInTlocationmapwarehouse),
            //#else
            //                  "Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.",
            //#endif
            //                  locMapWhsFKRefno));
            //            }

            //            drWhs.Close();
            //          }
            //        }
            //      }
            //      catch (SQLiteException ex)
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetlocmapwhsrefnofromlocwhsfkrefnoFailed), ex);
            //#else
            //        throw new Exception("Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.", ex);
            //#endif
            //      }

            return whsLocMapRefno;
        }

        /// <summary>
        /// The GetLocMapWhsFKRefnoFromLocMapWhsRefno
        /// </summary>
        /// <param name="locMapWhsRefno"></param>
        /// <returns></returns>
        public int GetLocMapWhsFKRefnoFromLocMapWhsRefno(int locMapWhsRefno)
        {
            int locMapWhsFKRefno = 0;

            //      try
            //      {
            //        using (SQLiteCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            //        {
            //          cmndGetWhs.CommandText = "tLocationMapWarehouse";
            //          cmndGetWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";
            //          cmndGetWhs.CommandType = CommandType.Text;

            //          using (SQLiteDataReader drWhs = cmndGetWhs.ExecuteReader())
            //          {
            //            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsRefno }))
            //            {
            //              if (drWhs.Read())
            //              {
            //                locMapWhsFKRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.FKRefno);
            //              }
            //              else
            //              {
            //#if MULTILANGUAGE
            //                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
            //#else
            //                throw new Exception("drWhs.Read() failed.");
            //#endif
            //              }
            //            }
            //            else
            //            {
            //              throw new Exception(
            //                string.Format(
            //#if MULTILANGUAGE
            //                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateWarehouseFkRefnoUsingTheWarehouseRefnoInTlocationmapwarehouse),
            //#else
            //                  "Unable to locate warehouse fk refno using the warehouse refno {0} in tLocationMapWarehouse.",
            //#endif
            //                  locMapWhsRefno));
            //            }

            //            drWhs.Close();
            //          }
            //        }
            //      }
            //      catch (SQLiteException ex)
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetlocmapwhsfkrefnofromlocmapwhsrefnoFailed), ex);
            //#else
            //        throw new Exception("Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.", ex);
            //#endif
            //      }

            return locMapWhsFKRefno;
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapWarehouse";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Warehouses), rowsAffected), 
#else
              string.Format("{0} warehouses.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredWarehousesStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Warehouses status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <returns></returns>
        protected override string Send()
        {
            return this.SendComm(this.Create());
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

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
                      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehouses), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses.", false);
#endif
                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouse"].Rows.Count);

                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
#if MULTILANGUAGE
                            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                            string.Format("Records {0}", progMax),
#endif
                            false);

                        using (SQLiteCommand cmndWhs = conn.CreateCommand())
                        {
                            using (SQLiteCommand cmndLocMap = conn.CreateCommand())
                            {
                                cmndLocMap.CommandText = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (@locationMapSubType)";
                                cmndLocMap.CommandType = CommandType.Text;

                                using (SQLiteCommand cmndID = conn.CreateCommand())
                                {
                                    cmndID.CommandText = "SELECT last_insert_rowid()";
                                    cmndID.CommandType = CommandType.Text;

                                    int locMapRefno = 0;
                                    int progValue = 0;
                                    bool isLocMapWhsFound = false;
                                    int locMapWhsFKRefno = 0;
                                    int locMapWhsCode = 0;
                                    string locMapWhsName = string.Empty;
                                    string locMapWhsBarCode = string.Empty;

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                    foreach (DataRow drow in ds.Tables["tLocationMapWarehouse"].Rows)
                                    {
                                        locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);

                                        errMsg = "Whs fk refno " + locMapWhsFKRefno;
                                        locMapWhsCode = Convert.ToInt32(drow["LocationMapWarehouseCode"]);
                                        errMsg += Environment.NewLine + "Whs code " + locMapWhsCode;
                                        locMapWhsName = drow["LocationMapWarehouseName"].ToString();
                                        errMsg += Environment.NewLine + "Whs name \'" + locMapWhsCode + "\'";
                                        locMapWhsBarCode = drow["LocationMapWarehouseBarCode"].ToString();
                                        errMsg += Environment.NewLine + "Whs bar code \'" + locMapWhsBarCode + "\'";

                                        isLocMapWhsFound = false;


                                        if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        {
                                            cmndWhs.CommandText = "SELECT LocationMapWarehouseFKRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locMapWhsFKRefno";
                                            cmndWhs.CommandType = CommandType.Text;
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locMapWhsFKRefno", locMapWhsFKRefno));
                                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                            using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                            {
                                                if (drWhs.Read())
                                                {
                                                    isLocMapWhsFound = true;
                                                    cmndWhs.CommandType = CommandType.Text;
                                                    cmndWhs.CommandText = "UPDATE tLocationMapWarehouse " +
                                                        "SET LocationMapWarehouseCode = " + locMapWhsCode.ToString() + ", " +
                                                        "LocationMapWarehouseName = \'" + locMapWhsName + "\', " +
                                                        "LocationMapWarehouseBarCode = \'" + locMapWhsBarCode + "\' " +
                                                        "WHERE LocationMapWarehouseFKRefno = " + locMapWhsFKRefno.ToString();
                                                    cmndWhs.ExecuteScalar();
                                                }

                                                drWhs.Close();
                                            }
                                        }

                                        if (!isLocMapWhsFound)
                                        {
                                            locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                            cmndLocMap.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));
                                            cmndLocMap.ExecuteNonQuery();

                                            // TODO: Insert here
                                            cmndWhs.Parameters.Clear();
                                            cmndWhs.CommandType = CommandType.Text;
                                            cmndWhs.CommandText = "INSERT INTO tLocationMapWarehouse " +
                                            "(LocationMapRefno, " +
                                            "LocationMapSubType, " +
                                            "LocationMapWarehouseName, " +
                                            "LocationMapWarehouseCode, " +
                                            "LocationMapWarehouseFKRefno, " +
                                            "LocationMapWarehouseBarCode) " +
                                            "VALUES " +
                                            "(@locationMapRefno, " +
                                            "@locationMapSubType, " +
                                            "@locationMapWarehouseName, " +
                                            "@locationMapWarehouseCode, " +
                                            "@locationMapWarehouseFKRefno, " +
                                            "@locationMapWarehouseBarCode)";
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseName", locMapWhsName));
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseCode", locMapWhsCode));
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));
                                            cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseBarCode", locMapWhsBarCode));

                                            cmndWhs.ExecuteNonQuery();
                                        }

                                        progValue++;
                                        if (progValue % 100 == 0)
                                        {
                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                        }
                                    }

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                }
                            }

                        }
                    }

#if MULTILANGUAGE
                      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehousesComplete), false);
#else
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses complete.", false);
#endif
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehousesFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses failed.", false);
#endif

                //                // Duplicate key violations
                //                if (ex.NativeError == 25016)
                //                {
                //#if MULTILANGUAGE
                //                      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
                //                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
                //#else
                //                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                //                    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                //#endif
                //                }

                throw new Exception(
#if MULTILANGUAGE
                        Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingWarehousesFailed) + Environment.NewLine + errMsg,
#else
                        "Database exception occurred. Saving warehouses failed." + Environment.NewLine + errMsg,
#endif
                        ex);
            }
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override string Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSES</action>"
                + SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            string test = xmlString.ToString();
            return xmlString.ToString();
        }
    }
}
