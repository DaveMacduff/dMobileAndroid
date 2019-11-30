namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

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
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses.", true);

                string reqData = string.Empty;

                // TODO: Uncomment GC calls
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding warehouses failed.", ex);
            }
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tLocationMapWarehouse WHERE LocationMapSubType = @locationMapSubType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses failed.", true);

                throw new Exception("Database exception occurred. Purging warehouses failed.", ex);
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

            // TODO: SQL
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

            // TODO: SQL
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

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} warehouses.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Warehouses status failed.", ex);
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
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses.", false);

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouse"].Rows.Count);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                        using (SQLiteCommand cmndWhs = conn.CreateCommand())
                        using (SQLiteCommand cmndLocMap = conn.CreateCommand())
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            cmndLocMap.CommandText = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (@locationMapSubType)";
                            cmndLocMap.CommandType = CommandType.Text;
                            cmndLocMap.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));

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

                                cmndWhs.CommandText = "SELECT LocationMapWarehouseFKRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locMapWhsFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locMapWhsFKRefno", locMapWhsFKRefno));
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    if (drWhs.Read())
                                    {
                                        isLocMapWhsFound = true;

                                        if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                        {
                                            cmndWhs.CommandType = CommandType.Text;
                                            cmndWhs.CommandText = "UPDATE tLocationMapWarehouse " +
                                                "SET LocationMapWarehouseCode = " + locMapWhsCode.ToString() + ", " +
                                                "LocationMapWarehouseName = \'" + locMapWhsName + "\', " +
                                                "LocationMapWarehouseBarCode = \'" + locMapWhsBarCode + "\' " +
                                                "WHERE LocationMapWarehouseFKRefno = " + locMapWhsFKRefno.ToString();
                                            cmndWhs.ExecuteScalar();
                                        }
                                    }

                                    drWhs.Close();
                                }

                                if (!isLocMapWhsFound)
                                {
                                    cmndLocMap.ExecuteNonQuery();

                                    cmndWhs.Parameters.Clear();
                                    cmndWhs.CommandType = CommandType.Text;
                                    cmndWhs.CommandText = "INSERT INTO tLocationMapWarehouse " +
                                    "(LocationMapSubType, " +
                                    "LocationMapWarehouseName, " +
                                    "LocationMapWarehouseCode, " +
                                    "LocationMapWarehouseFKRefno, " +
                                    "LocationMapWarehouseBarCode) " +
                                    "VALUES " +
                                    "(@locationMapSubType, " +
                                    "@locationMapWarehouseName, " +
                                    "@locationMapWarehouseCode, " +
                                    "@locationMapWarehouseFKRefno, " +
                                    "@locationMapWarehouseBarCode)";
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

                            transaction.Commit();
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses complete.", false);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses failed.", false);

                // TODO: SQLite native error codes?
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

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Saving warehouses failed." + Environment.NewLine + errMsg, ex);
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

            return xmlString.ToString();
        }
    }
}
