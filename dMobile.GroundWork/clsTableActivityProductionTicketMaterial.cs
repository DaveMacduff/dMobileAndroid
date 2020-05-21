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
    /// Defines the <see cref="TableActivityProductionTicketMaterial" />
    /// </summary>
    public class TableActivityProductionTicketMaterial : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterial"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterial()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterial";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override void Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials.", true);

                reqData = this.Send("ticket materials");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding ticket materials failed.", ex);
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
                    cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndStatus.CommandType = CommandType.Text;
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket materials.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Material tickets status failed.", ex);
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
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials.", true);

                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                    cmndPurge.CommandText = "DELETE FROM tActivityProductionMaterial"
                        + " WHERE tActivityProductionMaterial.ActivityRefno IN"
                        + " (SELECT tActivityProduction.ActivityRefno"
                        + " FROM tActivityProduction"
                        + " WHERE tActivityProduction.ActivitySubType = @activitySubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket materials failed.", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndProduction = conn.CreateCommand())
            using (SQLiteCommand cmndMaterial = conn.CreateCommand())
            using (SQLiteCommand cmndActivity = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndProduction = conn.CreateCommand())
                            //{
                            //cmndProduction.CommandText = "tActivityProduction";
                            //cmndProduction.CommandType = CommandType.Text;
                            //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";

                            //using (SQLiteCommand cmndMaterial = conn.CreateCommand())
                            //{
                            //cmndMaterial.CommandText = "tActivityProductionMaterial";
                            //cmndMaterial.CommandType = CommandType.Text;
                            //cmndMaterial.IndexName = "UQ_ActivityProductionMaterialOnActivityProductionFKRefno";

                            //using (SqlCeResultSet rsMaterial = cmndMaterial.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recMaterial = rsMaterial.CreateRecord();

                            //using (SQLiteCommand cmndActivity = conn.CreateCommand())
                            //{
                            //cmndActivity.CommandText = "tActivity";
                            //cmndActivity.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsActivity = cmndActivity.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActivity = rsActivity.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT @@IDENTITY";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rsProduction = cmndProduction.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recProduction = rsProduction.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int actProductionFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                cmndActivity.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndActivity.CommandType = CommandType.Text;
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                                cmndActivity.ExecuteNonQuery();

                                //recActivity.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.TicketMaterial);
                                //recActivity.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //rsActivity.Insert(recActivity);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndWhs);

                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityProductionDateTime"]);

                                cmndProduction.CommandText = "INSERT INTO tActivityProduction (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityProductionDateTime, " +
                                    "ActivityProductionDescription" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityProductionDateTime, " +
                                    "@activityProductionDescription" +
                                    ")";
                                cmndProduction.CommandType = CommandType.Text;
                                //cmndProduction.IndexName = "PK_ActivityProductionOnActivityRefno";
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDateTime", DateTime.Parse(dt.ToShortDateString())));
                                cmndProduction.Parameters.Add(new SQLiteParameter("@activityProductionDescription", drow["ActivityProductionDescription"].ToString()));

                                cmndProduction.ExecuteNonQuery();

                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActRefno, actRefno);
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.ActSubType, (int)SysCommon.ActSubTypes.TicketMaterial);
                                //recProduction.SetInt32((int)SysCommon.ActProdFields.WhsLocMapRefno, whsLocMapRefno);
                                //recProduction.SetDateTime((int)SysCommon.ActProdFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                //recProduction.SetString((int)SysCommon.ActProdFields.Description, drow["ActivityProductionDescription"].ToString());
                                //rsProduction.Insert(recProduction);

                                actProductionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + actProductionFKRefno;

                                cmndMaterial.CommandText = "INSERT INTO tActivityProductionMaterial (" +
                                    "ActivityRefno, " +
                                    "ActivityProductionFKRefno, " +
                                    "ActivityProductionStringFKRefno " +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activityProductionFKRefno, " +
                                    "@activityProductionStringFKRefno" +
                                    ")";
                                cmndMaterial.CommandType = CommandType.Text;
                                //cmndMaterial.IndexName = "UQ_ActivityProductionMaterialOnActivityProductionFKRefno";
                                cmndMaterial.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndMaterial.Parameters.Add(new SQLiteParameter("@activityProductionFKRefno", actProductionFKRefno));
                                cmndMaterial.Parameters.Add(new SQLiteParameter("@activityProductionStringFKRefno", actProductionFKRefno.ToString()));

                                cmndMaterial.ExecuteNonQuery();

                                //recMaterial.SetInt32((int)SysCommon.ActProdMaterialFields.ActRefno, actRefno);
                                //recMaterial.SetInt32((int)SysCommon.ActProdMaterialFields.FKRefno, actProductionFKRefno);
                                //recMaterial.SetString((int)SysCommon.ActProdMaterialFields.StringFKRefno, actProductionFKRefno.ToString());
                                //rsMaterial.Insert(recMaterial);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        //drWhs.Close();
                        //}
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket materials failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST PRODUCTION TICKET MATERIAL</action>"
                + SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
