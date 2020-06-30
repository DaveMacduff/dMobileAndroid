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

            try
            {
                using (SQLiteCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndGetWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                    cmndGetWhs.CommandType = CommandType.Text;
                    //cmndGetWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                    cmndGetWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(locMapWhsFKRefno)));

                    using (SQLiteDataReader drWhs = cmndGetWhs.ExecuteReader())
                    {
                        //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                        //{
                        if (drWhs.Read())
                        {
                            whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("drWhs.Read() failed. " + string.Format("Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.", locMapWhsFKRefno));
                        }
                        //}
                        //else
                        //{
                        //    throw new Exception(string.Format("Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.", locMapWhsFKRefno));
                        //}
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.", ex);
            }

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

            try
            {
                using (SQLiteCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndGetWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = @locationMapRefno";
                    cmndGetWhs.CommandType = CommandType.Text;
                    //cmndGetWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";
                    cmndGetWhs.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapWhsRefno));

                    using (SQLiteDataReader drWhs = cmndGetWhs.ExecuteReader())
                    {
                        //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsRefno }))
                        //{
                        if (drWhs.Read())
                        {
                            locMapWhsFKRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.FKRefno);
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("drWhs.Read() failed.");
                        }
                        //}
                        //else
                        //{
                        //    throw new Exception(string.Format("Unable to locate warehouse fk refno using the warehouse refno {0} in tLocationMapWarehouse.", locMapWhsRefno));
                        //}
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.", ex);
            }

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
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteCommand cmndLocMap = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
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

                            cmndLocMap.CommandText = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (@locationMapSubType)";
                            cmndLocMap.CommandType = CommandType.Text;
                            cmndLocMap.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));

                            int locMapRefno = 0;
                            int progValue = 0;
                            bool isLocMapWhsFound = false;
                            int locMapWhsFKRefno = 0;
                            int locMapWhsCode = 0;
                            string locMapWhsName = string.Empty;
                            string locMapWhsBarCode = string.Empty;
                            bool isLocMapWhsIsWMS = false;
                            string locMapWhsMask = string.Empty;
                            string locMapSectionMask = string.Empty;
                            string locMapSlotMask = string.Empty;
                            string locMapSlotCoord1Mask = string.Empty;
                            string locMapSlotCoord1Name = string.Empty;
                            string locMapSlotCoord2Mask = string.Empty;
                            string locMapSlotCoord2Name = string.Empty;
                            string locMapSlotCoord3Mask = string.Empty;
                            string locMapSlotCoord3Name = string.Empty;
                            bool isLocMapSectionTracked = false;
                            bool isLocMapSlotTracked = false;
                            bool isLocMapAdhocEnabled = false;

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
                                isLocMapWhsIsWMS = Convert.ToBoolean(drow["LocationMapWarehouseIsWMS"]);
                                errMsg += Environment.NewLine + "Whs is wms " + isLocMapWhsIsWMS.ToString();
                                locMapWhsMask = drow["LocationMapWarehouseMask"].ToString();
                                errMsg += Environment.NewLine + "Whs mask \'" + locMapWhsMask + "\'";
                                locMapSectionMask = drow["LocationMapSectionMask"].ToString();
                                errMsg += Environment.NewLine + "Whs section mask \'" + locMapSectionMask + "\'";
                                locMapSlotMask = drow["LocationMapSlotMask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot mask \'" + locMapSlotMask + "\'";
                                locMapSlotCoord1Mask = drow["LocationMapSlotCoord1Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord1 mask \'" + locMapSlotCoord1Mask + "\'";
                                locMapSlotCoord1Name = drow["LocationMapSlotCoord1Name"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord1 name \'" + locMapSlotCoord1Name + "\'";
                                locMapSlotCoord2Mask = drow["LocationMapSlotCoord2Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord2 mask \'" + locMapSlotCoord2Mask + "\'";
                                locMapSlotCoord2Name = drow["LocationMapSlotCoord2Name"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord2 name \'" + locMapSlotCoord2Name + "\'";
                                locMapSlotCoord3Name = drow["LocationMapSlotCoord3Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord3 mask \'" + locMapSlotCoord3Mask + "\'";
                                locMapSlotCoord3Mask = drow["LocationMapSlotCoord3Mask"].ToString();
                                errMsg += Environment.NewLine + "Whs slot coord3 name \'" + locMapSlotCoord3Name + "\'";
                                isLocMapSectionTracked = Convert.ToBoolean(drow["LocationMapIsSectionTracked"]);
                                errMsg += Environment.NewLine + "Whs is section tracked " + isLocMapSectionTracked.ToString();
                                isLocMapSlotTracked = Convert.ToBoolean(drow["LocationMapIsSlotTracked"]);
                                errMsg += Environment.NewLine + "Whs is slot tracked " + isLocMapSlotTracked.ToString();
                                isLocMapAdhocEnabled = Convert.ToBoolean(drow["LocationMapIsAdhocEnabled"]);
                                errMsg += Environment.NewLine + "Whs is adhoc enabled " + isLocMapAdhocEnabled.ToString();

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
                                            "LocationMapWarehouseBarCode = \'" + locMapWhsBarCode + "\', " +
                                            "LocationMapWarehouseIsWMS = " + (isLocMapWhsIsWMS ? 1 : 0).ToString() +
                                            "LocationMapWarehouseMask = \'" + locMapWhsMask + "\', " +
                                            "LocationMapWarehouseSectionMask = \'" + locMapSectionMask + "\', " +
                                            "LocationMapWarehouseSlotMask = \'" + locMapSlotMask + "\', " +
                                            "LocationMapWarehouseSlotCoordinate1Mask = \'" + locMapSlotCoord1Mask + "\', " +
                                            "LocationMapWarehouseSlotCoordinate2Mask = \'" + locMapSlotCoord2Mask + "\', " +
                                            "LocationMapWarehouseSlotCoordinate3Mask = \'" + locMapSlotCoord3Mask + "\', " +
                                            "LocationMapWarehouseSlotCoordinate1Name = \'" + locMapSlotCoord1Name + "\', " +
                                            "LocationMapWarehouseSlotCoordinate2Name = \'" + locMapSlotCoord2Name + "\', " +
                                            "LocationMapWarehouseSlotCoordinate3Name = \'" + locMapSlotCoord3Name + "\', " +
                                            "LocationMapWarehouseIsSectionTracked = " + (isLocMapSectionTracked ? 1 : 0).ToString() +
                                            "LocationMapWarehouseIsSlotTracked = " + (isLocMapSlotTracked ? 1 : 0).ToString() +
                                            "LocationMapWarehouseAdhocEnabled = " + (isLocMapAdhocEnabled ? 1 : 0).ToString() +
                                            "WHERE LocationMapWarehouseFKRefno = " + locMapWhsFKRefno.ToString();
                                            cmndWhs.ExecuteScalar();
                                        }
                                    }

                                    drWhs.Close();
                                }

                                if (!isLocMapWhsFound)
                                {
                                    cmndLocMap.ExecuteNonQuery();

                                    cmndID.CommandText = "SELECT last_insert_rowid() FROM tLocationMap";
                                    cmndID.CommandType = CommandType.Text;
                                    locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                    cmndWhs.Parameters.Clear();
                                    cmndWhs.CommandType = CommandType.Text;
                                    cmndWhs.CommandText = "INSERT INTO tLocationMapWarehouse (" +
                                    "LocationMapRefno, " +
                                    "LocationMapSubType, " +
                                    "LocationMapWarehouseName, " +
                                    "LocationMapWarehouseCode, " +
                                    "LocationMapWarehouseFKRefno, " +
                                    "LocationMapWarehouseBarCode, " +
                                    "LocationMapWarehouseIsWMS, " +
                                    "LocationMapWarehouseMask, " +
                                    "LocationMapWarehouseSectionMask, " +
                                    "LocationMapWarehouseSlotMask, " +
                                    "LocationMapWarehouseSlotCoordinate1Mask, " +
                                    "LocationMapWarehouseSlotCoordinate2Mask, " +
                                    "LocationMapWarehouseSlotCoordinate3Mask, " +
                                    "LocationMapWarehouseSlotCoordinate1Name, " +
                                    "LocationMapWarehouseSlotCoordinate2Name, " +
                                    "LocationMapWarehouseSlotCoordinate3Name, " +
                                    "LocationMapWarehouseIsSectionTracked, " +
                                    "LocationMapWarehouseIsSlotTracked, " +
                                    "LocationMapWarehouseAdhocEnabled" +
                                    ") VALUES (" +
                                    "@locationMapRefno, " +
                                    "@locationMapSubType, " +
                                    "@locationMapWarehouseName, " +
                                    "@locationMapWarehouseCode, " +
                                    "@locationMapWarehouseFKRefno, " +
                                    "@locationMapWarehouseBarCode, " +
                                    "@locationMapWarehouseIsWMS, " +
                                    "@locationMapWarehouseMask, " +
                                    "@locationMapWarehouseSectionMask, " +
                                    "@locationMapWarehouseSlotMask, " +
                                    "@locationMapWarehouseSlotCoordinate1Mask, " +
                                    "@locationMapWarehouseSlotCoordinate2Mask, " +
                                    "@locationMapWarehouseSlotCoordinate3Mask, " +
                                    "@locationMapWarehouseSlotCoordinate1Name, " +
                                    "@locationMapWarehouseSlotCoordinate2Name, " +
                                    "@locationMapWarehouseSlotCoordinate3Name, " +
                                    "@locationMapWarehouseIsSectionTracked, " +
                                    "@locationMapWarehouseIsSlotTracked, " +
                                    "@locationMapWarehouseAdhocEnabled" +
                                    ")";
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapSubType", Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse)));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseName", locMapWhsName));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseCode", locMapWhsCode));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseBarCode", locMapWhsBarCode));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseIsWMS", Convert.ToInt32(isLocMapWhsIsWMS)));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseMask", locMapWhsMask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSectionMask", locMapSectionMask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotMask", locMapSlotMask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate1Mask", locMapSlotCoord1Mask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate2Mask", locMapSlotCoord2Mask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate3Mask", locMapSlotCoord3Mask));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate1Name", locMapSlotCoord1Name));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate2Name", locMapSlotCoord2Name));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseSlotCoordinate3Name", locMapSlotCoord3Name));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseIsSectionTracked", Convert.ToInt32(isLocMapSectionTracked)));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseIsSlotTracked", Convert.ToInt32(isLocMapSlotTracked)));
                                    cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseAdhocEnabled", Convert.ToInt32(isLocMapAdhocEnabled)));

                                    cmndWhs.ExecuteNonQuery();

                                    //rec.SetBoolean(Convert.ToInt32(SysCommon.LocMapWhsFields.IsWMS), isLocMapWhsIsWMS);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.WhsMask), locMapWhsMask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SectionMask), locMapSectionMask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotMask), locMapSlotMask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord1Mask), locMapSlotCoord1Mask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord2Mask), locMapSlotCoord2Mask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord3Mask), locMapSlotCoord3Mask);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord1Name), locMapSlotCoord1Name);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord2Name), locMapSlotCoord2Name);
                                    //rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.SlotCoord3Name), locMapSlotCoord3Name);
                                    //rec.SetBoolean(Convert.ToInt32(SysCommon.LocMapWhsFields.IsSectionTracked), isLocMapSectionTracked);
                                    //rec.SetBoolean(Convert.ToInt32(SysCommon.LocMapWhsFields.IsSlotTracked), isLocMapSlotTracked);
                                    //rec.SetBoolean(Convert.ToInt32(SysCommon.LocMapWhsFields.IsAdhocEnabled), isLocMapAdhocEnabled);
                                }

                                progValue++;

                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                        }

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses failed.", false);

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
