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
    /// Defines the <see cref="TableLocationMapType" />
    /// </summary>
    public class TableLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapType"/> class.
        /// </summary>
        public TableLocationMapType()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapType";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types.", true);

                string reqData = string.Empty;
                int whsLocMapRefno = 0;

                //GC.Collect();
                //GC.WaitForPendingFinalizers();

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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding location map types failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tLocationMapType";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging location map types failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapType";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} location map types.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Location map types status failed.", ex);
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

            using (SQLiteCommand cmndLocMapType = conn.CreateCommand())
            using (SQLiteCommand cmndSect = conn.CreateCommand())
            using (SQLiteCommand cmndSlot = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //cmndLocMapType.CommandText = "tLocationMapType";
                            //cmndLocMapType.CommandType = CommandType.Text;
                            //cmndLocMapType.IndexName = "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";

                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;


                            //cmndSect.CommandText = "tLocationMapSection";
                            //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                            //cmndSect.CommandType = CommandType.Text;


                            //cmndSlot.CommandText = "tLocationMapSlot";
                            //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                            //cmndSlot.CommandType = CommandType.Text;


                            //using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int locMapRefno = 0;
                            int locMapSubType = 0;
                            int locMapFKRefno = 0;
                            int progValue = 0;
                            bool isLocMapTypeFound = false;
                            int locMapTypeEnum = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["seg"].Rows)
                            {
                                locMapFKRefno = Convert.ToInt32(drow["f03"]);
                                errMsg = "Whs fk refno " + locMapFKRefno;

                                locMapTypeEnum = Convert.ToInt32(drow["f01"]);
                                switch (locMapTypeEnum)
                                {
                                    case (int)SysCommon.LocMapTypes.IsFreezer:
                                        errMsg += Environment.NewLine + "Map type \'Freezer\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsGroundLevel:
                                        errMsg += Environment.NewLine + "Map type \'Ground Level\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsHandStack:
                                        errMsg += Environment.NewLine + "Map type \'Hand Stack\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsProcessing:
                                        errMsg += Environment.NewLine + "Map type \'Processing\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsReserved:
                                        errMsg += Environment.NewLine + "Map type \'Reserved\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsStaging:
                                        errMsg += Environment.NewLine + "Map type \'Staging\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsToBeOrdered:
                                        errMsg += Environment.NewLine + "Map type \'To Be Ordered\'";
                                        break;

                                    case (int)SysCommon.LocMapTypes.IsTracked:
                                        errMsg += Environment.NewLine + "Map type \'Tracked\'";
                                        break;

                                    default:
                                        throw new Exception(string.Format("Undefined location map type {0}", locMapTypeEnum));
                                }

                                locMapSubType = Convert.ToInt32(drow["f02"]);

                                switch (locMapSubType)
                                {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                        errMsg += Environment.NewLine + "Map sub type \'Warehouse\'";

                                        cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                        //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                        cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapFKRefno));

                                        using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                        {
                                            //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            //{
                                            if (drWhs.Read())
                                            {
                                                locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
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
                                        errMsg += Environment.NewLine + "Map sub type \'Section\'";

                                        cmndSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                                        //cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                                        cmndSect.CommandType = CommandType.Text;
                                        cmndSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapFKRefno));

                                        using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
                                        {
                                            //if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                            //{
                                            if (drSect.Read())
                                            {
                                                locMapRefno = Convert.ToInt32(drSect.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
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

                                        break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                        errMsg += Environment.NewLine + "Map sub type \'Slot\'";

                                        cmndSlot.CommandText = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                                        //cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                                        cmndSlot.CommandType = CommandType.Text;
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
                                            //    throw new Exception("Unable to locate the slot fk refno in tLocationMapSlot." + Environment.NewLine + errMsg);
                                            //}

                                            drSlot.Close();
                                        }

                                        break;

                                    default:
                                        // TODO: Resource Strings
                                        throw new Exception(string.Format("Undefined Location Map SubType {0}", locMapSubType));
                                }

                                isLocMapTypeFound = false;

                                //if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                //{
                                cmndLocMapType.CommandText = "SELECT * FROM tLocationMapType WHERE LocationMapRefno = @locationMapRefno AND LocationMapTypeEnum = @locationMapTypeEnum";
                                cmndLocMapType.CommandType = CommandType.Text;
                                //cmndLocMapType.IndexName = "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";
                                cmndLocMapType.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                cmndLocMapType.Parameters.Add(new SQLiteParameter("@locationMapTypeEnum", locMapTypeEnum));

                                using (SQLiteDataReader drLocMapType = cmndLocMapType.ExecuteReader())
                                {
                                    //if (drLocMapType.Seek(DbSeekOptions.FirstEqual, new object[] { locMapRefno, locMapTypeEnum }))
                                    if (drLocMapType.Read())
                                    {
                                        isLocMapTypeFound = true;
                                    }

                                    drLocMapType.Close();
                                }
                                //}

                                if (!isLocMapTypeFound)
                                {
                                    cmndLocMapType.Parameters.Clear();
                                    cmndLocMapType.CommandText = "INSERT INTO tLocationMapType (LocationMapRefno, LocationMapTypeEnum) VALUES (@locationMapRefno, @locationMapTypeEnum)";
                                    cmndLocMapType.CommandType = CommandType.Text;
                                    //cmndLocMapType.IndexName = "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@locationMapRefno", locMapRefno));
                                    cmndLocMapType.Parameters.Add(new SQLiteParameter("@locationMapTypeEnum", locMapTypeEnum));

                                    cmndLocMapType.ExecuteNonQuery();

                                    //rec.SetInt32((int)SysCommon.LocMapTypeFields.LocMapRefno, locMapRefno);
                                    //rec.SetInt32((int)SysCommon.LocMapTypeFields.EnumType, locMapTypeEnum);

                                    //rs.Insert(rec);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);

                            //}

                            //drSlot.Close();
                            //}
                            //}

                            //drSect.Close();

                            //drWhs.Close();
                            //}
                            //}
                            //drLocMapType.Close();
                            //}
                            //}
                        }

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback(); 

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource Strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving location map types failed." + Environment.NewLine + errMsg, ex);
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
            int locMapWhsFKRefno = 0;
            TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();

            // lookup and verify warehouse
            if (0 < whsLocMapRefno)
            {
                locMapWhsFKRefno = locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
            }

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST LOCATION MAP TYPES</action>"
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
