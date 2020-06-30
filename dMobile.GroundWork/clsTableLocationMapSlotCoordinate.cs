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
    /// Defines the <see cref="TableLocationMapSlotCoordinate" />
    /// </summary>
    public class TableLocationMapSlotCoordinate : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSlotCoordinate"/> class.
        /// </summary>
        public TableLocationMapSlotCoordinate()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlotCoordinate";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates.", true);

                string reqData = string.Empty;

                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                reqData = this.Send("slot coordinates");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding slot coordinates failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.CommandText = "DELETE FROM tLocationMapSlotCoordinate";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging slot coordinates failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapSlotCoordinate";
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} slot coordinates.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Slot coordinates status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            string errMsg = string.Empty;

            using (SQLiteCommand cmndSlotCoord = conn.CreateCommand())
            using (SQLiteCommand cmndSlotCoordInsert = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSlotCoordinate"].Rows.Count);

                            // TODO: Resource Strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            cmndSlotCoord.CommandText = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = @locationMapSlotCoordinateEnum AND LocationMapSlotCoordinateCode = @locationMapSlotCoordinateCode";
                            cmndSlotCoord.CommandType = CommandType.Text;
                            //cmndSlotCoord.IndexName = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";

                            //using (SqlCeResultSet rs = cmndSlotCoord.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //    SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int progValue = 0;
                            bool isSlotCoordFound = false;
                            int slotCoordType = (int)SysCommon.LocMapSlotCoordTypes.Coord1;
                            string slotCoordCode = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tLocationMapSlotCoordinate"].Rows)
                            {
                                slotCoordCode = drow["LocationMapSlotCoordinateCode"].ToString();
                                errMsg = "Coordinate code \'" + slotCoordCode + "\'";

                                slotCoordType = Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]);

                                switch (slotCoordType)
                                {
                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord1:
                                        errMsg += Environment.NewLine + "Type coordinate 1";
                                        break;

                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord2:
                                        errMsg += Environment.NewLine + "Type coordinate 2";
                                        break;

                                    case (int)SysCommon.LocMapSlotCoordTypes.Coord3:
                                        errMsg += Environment.NewLine + "Type coordinate 3";
                                        break;

                                    default:
                                        // TODO: Resource Strings
                                        throw new Exception("Unidentified slot coordinate enum type " + slotCoordType);
                                }

                                isSlotCoordFound = false;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    //cmndSlotCoord.CommandText = "tLocationMapSlotCoordinate";
                                    //cmndSlotCoord.CommandType = CommandType.Text;
                                    //cmndSlotCoord.IndexName = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";
                                    cmndSlotCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateEnum", slotCoordType));
                                    cmndSlotCoord.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateCode", slotCoordCode));

                                    using (SQLiteDataReader drSlotCoord = cmndSlotCoord.ExecuteReader())
                                    {
                                        //if (drSlotCoord.Seek(DbSeekOptions.FirstEqual, new object[] { slotCoordType, slotCoordCode }))
                                        if (drSlotCoord.Read())
                                        {
                                            isSlotCoordFound = true;
                                        }
                                    }
                                }

                                if (!isSlotCoordFound)
                                {
                                    cmndSlotCoordInsert.CommandText = "INSERT INTO tLocationMapSlotCoordinate (LocationMapSlotCoordinateEnum, LocationMapSlotCoordinateCode) VALUES (@locationMapSlotCoordinateEnum, @locationMapSlotCoordinateCode)";
                                    cmndSlotCoordInsert.CommandType = CommandType.Text;
                                    //cmndSlotCoordInsert.IndexName = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";
                                    cmndSlotCoordInsert.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateEnum", Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"])));
                                    cmndSlotCoordInsert.Parameters.Add(new SQLiteParameter("@locationMapSlotCoordinateCode", drow["LocationMapSlotCoordinateCode"].ToString()));

                                    cmndSlotCoordInsert.ExecuteNonQuery();

                                    //rec.SetInt32((int)SysCommon.LocMapSlotCoordFields.EnumType, Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]));
                                    //rec.SetString((int)SysCommon.LocMapSlotCoordFields.Code, drow["LocationMapSlotCoordinateCode"].ToString());

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

                            //drSlotCoord.Close();
                            //}
                            //}

                        }

                        transaction.Commit();

                        // TODO: Resource Strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource Strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource Strings
                    throw new Exception("Database exception occurred. Saving slot coordinates failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST SLOT COORDINATES</action>"
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
