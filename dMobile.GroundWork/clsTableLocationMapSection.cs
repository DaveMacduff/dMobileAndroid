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
    /// Defines the <see cref="TableLocationMapSection" />
    /// </summary>
    public class TableLocationMapSection : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSection"/> class.
        /// </summary>
        public TableLocationMapSection()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSection";
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override void Request(int whsLocMapRefno)
        {
            try
            {
                // TODO: Resource String
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections.", true);

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
                                // TODO: Resource String
                                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For whs \'{0}\'.", drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)), true);
                            }
                            else
                            {
                                // TODO: Resource String
                                throw new Exception(string.Format("Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.", whsLocMapRefno));
                            }

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

                    this.Save(reqData);
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSectionsFailed), ex);
#else
                throw new Exception("Database exception occurred. Seeding sections failed.", ex);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), ex);
#else
                throw new Exception("Seeding sections failed.", ex);
#endif
            }
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override void PurgeData(int whsLocMapRefno)
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSections), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;

                    if (whsLocMapRefno == 0)
                    {
                        cmndPurge.CommandText = "DELETE FROM tLocationMapSection";
                    }
                    else
                    {
                        cmndPurge.CommandText = "DELETE FROM tLocationMapSection "
                            + "WHERE WarehouseLocationMapRefno = " + whsLocMapRefno.ToString();
                    }

                    cmndPurge.ExecuteNonQuery();
                }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSectionsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections complete.", true);
#endif
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSectionsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSectionsFailed), ex);
#else
                throw new Exception("Database exception occurred. Purging sections failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapSection";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Sections), rowsAffected), 
#else
              string.Format("{0} sections.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSectionsStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Sections status failed.", ex);
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
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSect = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapInsert = conn.CreateCommand())
            using (SQLiteCommand cmndSectInsert = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSection"].Rows.Count);

                            // TODO: Resource String
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndLocMap = conn.CreateCommand())
                            //{
                            //    cmndLocMap.CommandText = "tLocationMap";
                            //    cmndLocMap.CommandType = CommandType.TableDirect;

                            //using (SqlCeResultSet rsLocMap = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord recLocMap = rsLocMap.CreateRecord();

                            //using (SqlCeResultSet rs = cmndSect.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int locMapRefno = 0;
                            int whsLocMapRefno = 0;
                            int progValue = 0;
                            bool isSectFound = false;
                            int sectFKRefno = 0;
                            int sectCode = 0;
                            string sectName = string.Empty;
                            string sectBarCode = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drowSection in ds.Tables["tLocationMapSection"].Rows)
                            {
                                sectFKRefno = Convert.ToInt32(drowSection["LocationMapSectionFKRefno"]);
                                errMsg = "Section fk refno " + sectFKRefno;
                                sectCode = Convert.ToInt32(drowSection["LocationMapSectionCode"]);
                                errMsg += Environment.NewLine + "Section code " + sectCode;
                                sectName = Convert.ToString(drowSection["LocationMapSectionName"]);
                                errMsg += Environment.NewLine + "Section name \'" + sectName + "\'";
                                sectBarCode = Convert.ToString(drowSection["LocationMapSectionBarCode"]);
                                errMsg += Environment.NewLine + "Section bar code \'" + sectBarCode + "\'";

                                cmndWhs.CommandText = "SELECT LocationMapWarehouseFKRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(drowSection["LocationMapWarehouseFKRefno"])));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drowSection, ref errMsg, drWhs);

                                    isSectFound = false;

                                    cmndSect.CommandText = "SELECT LocationMapSectionFKRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                    cmndSect.CommandType = CommandType.Text;
                                    //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                    cmndSect.Parameters.Clear();
                                    cmndSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", sectFKRefno.ToString()));

                                    using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
                                    {
                                        //cmndSect.CommandText = "tLocationMapSection";
                                        //cmndSect.CommandType = CommandType.Text;
                                        //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

                                        //if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { sectFKRefno }))

                                        if (drSect.Read())
                                        {
                                            isSectFound = true;

                                            if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                            {
                                                cmndSect.CommandType = CommandType.Text;
                                                //cmndSect.IndexName = string.Empty;
                                                cmndSect.CommandText = "UPDATE tLocationMapSection "
                                                + "SET WarehouseLocationMapRefno = " + whsLocMapRefno + ","
                                                + "LocationMapSectionCode = " + sectCode + ","
                                                + "LocationMapSectionName = \'" + sectName + "\',"
                                                + "LocationMapSectionBarCode = \'" + sectBarCode + "\' "
                                                + "WHERE LocationMapSectionFKRefno = " + sectFKRefno;

                                                cmndSect.ExecuteNonQuery();
                                            }
                                        }

                                        if (!isSectFound)
                                        {
                                            cmndLocMapInsert.CommandText = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (@locationMapSubType)";
                                            cmndLocMapInsert.CommandType = CommandType.Text;
                                            cmndLocMapInsert.Parameters.Add(new SQLiteParameter("@locationMapSubType", (int)SysCommon.LocMapSubTypes.Section));

                                            cmndLocMapInsert.ExecuteNonQuery();

                                            //recLocMap.SetInt32((int)SysCommon.LocMapFields.SubType, (int)SysCommon.LocMapSubTypes.Section);
                                            //    rsLocMap.Insert(recLocMap);

                                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tLocationMap";
                                            cmndID.CommandType = CommandType.Text;

                                            locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                            cmndSectInsert.CommandText = "INSERT INTO tLocationMapSection " +
                                            "(LocationMapRefno, " +
                                            "LocationMapSubType, " +
                                            "WarehouseLocationMapRefno, " +
                                            "LocationMapSectionCode, " +
                                            "LocationMapSectionName, " +
                                            "LocationMapSectionFKRefno, " +
                                            "LocationMapSectionBarCode " +
                                            ") " +
                                            "VALUES " +
                                            "(@locationMapRefno, " +
                                            "@locationMapSubType, " +
                                            "@warehouseLocationMapRefno, " +
                                            "@locationMapSectionCode, " +
                                            "@locationMapSectionName, " +
                                            "@locationMapSectionFKRefno, " +
                                            "@locationMapSectionBarCode" +
                                            ")";
                                            cmndSectInsert.CommandType = CommandType.Text;
                                            //cmndSectInsert.IndexName = string.Empty;
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapSubType", (int)SysCommon.LocMapSubTypes.Section));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapSectionCode", sectCode));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapSectionName", sectName));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", sectFKRefno));
                                            cmndSectInsert.Parameters.Add(new SQLiteParameter("@locationMapSectionBarCode", sectBarCode));

                                            cmndSectInsert.ExecuteNonQuery();

                                            //rec.SetInt32((int)SysCommon.LocMapSectFields.LocMapRefno, locMapRefno);
                                            //rec.SetInt32((int)SysCommon.LocMapSectFields.LocMapSubType, (int)SysCommon.LocMapSubTypes.Section);
                                            //rec.SetInt32((int)SysCommon.LocMapSectFields.WhsLocMapRefno, whsLocMapRefno);
                                            //rec.SetInt32((int)SysCommon.LocMapSectFields.Code, sectCode);
                                            //rec.SetString((int)SysCommon.LocMapSectFields.Name, sectName);
                                            //rec.SetInt32((int)SysCommon.LocMapSectFields.FKRefno, sectFKRefno);
                                            //rec.SetString((int)SysCommon.LocMapSectFields.BarCode, sectBarCode);

                                            //rs.Insert(rec);
                                        }

                                        progValue++;

                                        if (progValue % 100 == 0)
                                        {
                                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                        }

                                        drSect.Close();
                                    }

                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                                    //}

                                    drWhs.Close();
                                }
                            }
                            //}

                            //drSect.Close();
                            //}
                        }

                        transaction.Commit();

                        // TODO: Resource String
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    // TODO: Resource String
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource String
                    throw new Exception("Database exception occurred. Saving sections failed." + Environment.NewLine + errMsg, ex);
                }
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
            if (0 < whsLocMapRefno)
            {
                locMapWhsFKRefno = locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
            }

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SECTIONS</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
