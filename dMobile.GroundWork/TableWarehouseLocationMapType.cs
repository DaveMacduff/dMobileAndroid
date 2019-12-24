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
        public override void Request()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations.", true);
#endif

                // TODO: Uncomment GC calls
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultPickingLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);
#endif
                string reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Picking);
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

                // TODO: Uncomment GC calls
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultReceivingLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);
#endif
                reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Receiving);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsComplete), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations complete.", true);
#endif
            }
            catch (Exception ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsFailed), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsFailed), ex);
#else
                throw new Exception("Seeding warehouse locations failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tWarehouseLocationMapType";
                    cmndStatus.CommandType = CommandType.Text;
                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.WarehouseLocations), rowsAffected),
#else
              string.Format("{0} warehouse locations.", rowsAffected),
#endif
              true);
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.WarehouseLocationsStatusFailed), false);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "warehouse locations status failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredWarehouseLocationStatusFailed), ex);
#else
                throw new Exception("Database exception occurred. Warehouse location status failed.", ex);
#endif
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouseLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);
#endif

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tWarehouseLocationMapType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouseLocations), true);
#else
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingWarehouseLocations), ex);
#else
                throw new Exception("Database exception occurred. Purging warehouse locations.", ex);
#endif
            }
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="whsLocMapType"></param>
        /// <returns></returns>
        protected override string SendRequest(int whsLocMapType)
        {
            return this.SendComm(this.CreateRequest(whsLocMapType));
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            int locMapRefno = 0;
            int whsLocMapType = 0;
            int locMapFKRefno = 0;
            int locMapSubType = 0;
            int currWhsLocMapRefno = 0;
            int whsLocMapFKRefno = 0;
            int whsLocMapRefno = 0;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndLocMapType = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapTypeUpdate = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndSect = conn.CreateCommand())
            using (SQLiteCommand cmndSlot = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
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

                            //cmndLocMapType.CommandText = "tWarehouseLocationMapType";
                            //cmndLocMapType.CommandType = CommandType.TableDirect;
                            //cmndLocMapType.IndexName = "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum";

                            //{
                            //cmndLocMapTypeUpdate.CommandText = string.Empty;
                            //cmndLocMapTypeUpdate.CommandType = CommandType.Text;
                            //cmndLocMapTypeUpdate.IndexName = string.Empty;

                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.TableDirect;

                            //cmndSect.CommandText = "tLocationMapSection";
                            //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                            //cmndSect.CommandType = CommandType.TableDirect;

                            //cmndSlot.CommandText = "tLocationMapSlot";
                            //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                            //cmndSlot.CommandType = CommandType.TableDirect;


                            //using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(
                            //    ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int progValue = 0;
                            bool isLocMapTypeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                whsLocMapFKRefno = Convert.ToInt32(drow["WarehouseLocationMapFKRefno"]);
                                errMsg += Environment.NewLine + "Warehouse fk refno " + whsLocMapFKRefno;

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", whsLocMapFKRefno));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapFKRefno }))
                                    //{
                                    if (drWhs.Read())
                                    {
                                        whsLocMapRefno = Convert.ToInt32(drWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
                                    }
                                    else
                                    {
                                        // TODO: Resource Strings
                                        throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource Strings
                                    //    throw new Exception("Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                    //}

                                    drWhs.Close();
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
                                        cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                        cmndWhs.CommandType = CommandType.Text;
                                        //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                        cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapFKRefno));

                                        using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                        {
                                            //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            //{
                                            if (drWhs.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
                                            }
                                            else
                                            {
                                                // TODO: Resource Strings
                                                throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                            }
                                            //}
                                            //else
                                            //{
                                            //    // TODO: Resource Strings
                                            //    throw new Exception("Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                            //}

                                            drWhs.Close();
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                        cmndSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                        cmndSect.CommandType = CommandType.Text;
                                        //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                        cmndSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapFKRefno));

                                        using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
                                        {
                                            //if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            //{
                                            if (drSect.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(
                                                    drSect.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
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
                                            //    throw new Exception("Unable to locate section fk refno in tLocationMapSection." + Environment.NewLine + errMsg);
                                            //}

                                            drSect.Close();
                                        }

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        cmndSlot.CommandText = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                                        cmndSlot.CommandType = CommandType.Text;
                                        //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                        cmndSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", locMapFKRefno));

                                        using (SQLiteDataReader drSlot = cmndSlot.ExecuteReader())
                                        {
                                            //if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            //{
                                            if (drSlot.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drSlot.GetValue((int)SysCommon.LocMapSlotFields.LocMapRefno));
                                            }
                                            else
                                            {
                                                // TODO: Resource Strings
                                                throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
                                            }
                                            //}
                                            //else
                                            //{
                                            //    // TODO: Resource Strings
                                            //    throw new Exception("Unable to locate slot fk refno in tLocationMapSlot." + Environment.NewLine + errMsg);
                                            //}

                                            drSlot.Close();
                                        }

                                        break;

                                    default:
                                        throw new Exception(string.Format("Undefined location map subtype {0}", locMapSubType));
                                }

                                isLocMapTypeFound = false;

                                currWhsLocMapRefno = 0;
                                if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    cmndLocMapType.CommandText = "SELECT * FROM tWarehouseLocationMapType WHERE WarehouseLocationMapRefno = @warehouseLocationMapRefno AND WarehouseLocationMapTypeEnum = @warehouseLocationMapTypeEnum";
                                    cmndLocMapType.CommandType = CommandType.Text;
                                    //cmndLocMapType.IndexName = "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum";
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@warehouseLocationMapTypeEnum", whsLocMapType));

                                    using (SQLiteDataReader drLocMapType = cmndLocMapType.ExecuteReader())
                                    {
                                        //if (drLocMapType.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapRefno, whsLocMapType }))
                                        //{
                                        if (drLocMapType.Read())
                                        {
                                            currWhsLocMapRefno = Convert.ToInt32(drLocMapType.GetValue((int)SysCommon.WhsLocMapTypeFields.LocMapRefno));
                                            isLocMapTypeFound = true;
                                        }
                                        else
                                        {
                                            // TODO: Resource Strings
                                            throw new Exception("drLocMapType.Read() failed." + Environment.NewLine + errMsg);
                                        }
                                        //}

                                        drLocMapType.Close();
                                    }
                                }

                                if (isLocMapTypeFound)
                                {
                                    if (currWhsLocMapRefno != locMapRefno)
                                    {
                                        // Update the LocMapRefno if different
                                        cmndLocMapTypeUpdate.CommandText = "UPDATE tWarehouseLocationMapType"
                                          + " SET LocationMapRefno = " + locMapRefno
                                          + " WHERE WarehouseLocationMapRefno = " + whsLocMapRefno
                                          + " AND WarehouseLocationMapTypeEnum = " + whsLocMapType;
                                        cmndLocMapTypeUpdate.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    cmndLocMapType.CommandText = "INSERT INTO tWarehouseLocationMapType (WarehouseLocationMapRefno, WarehouseLocationMapTypeEnum, LocationMapRefno) " +
                                    "VALUES (@warehouseLocationMapRefno, @warehouseLocationMapTypeEnum, @locationMapRefno)";
                                    cmndLocMapType.CommandType = CommandType.Text;
                                    //cmndLocMapType.IndexName = "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum";
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@warehouseLocationMapTypeEnum", whsLocMapType));
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    cmndLocMapType.ExecuteNonQuery();

                                    //rec.SetInt32(Convert.ToInt32(SysCommon.WhsLocMapTypeFields.WhsLocMapRefno), whsLocMapRefno);
                                    //rec.SetInt32(Convert.ToInt32(SysCommon.WhsLocMapTypeFields.EnumType), whsLocMapType);
                                    //rec.SetInt32(Convert.ToInt32(SysCommon.WhsLocMapTypeFields.LocMapRefno), locMapRefno);

                                    //rs.Insert(rec);
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

                        //         drSlot.Close();
                        //}
                        //}

                        //    drSect.Close();
                        //}
                        //}

                        //drWhs.Close();
                        //}
                        //}
                        //}
                        //}
                        //}
                        //}

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving warehouse locations failed." + Environment.NewLine + errMsg, ex);
                }
            }
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
