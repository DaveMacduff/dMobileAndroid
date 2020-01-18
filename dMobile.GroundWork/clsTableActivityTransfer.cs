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
    /// Defines the <see cref="TableActivityTransfer" />
    /// </summary>
    public class TableActivityTransfer : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityTransfer"/> class.
        /// </summary>
        public TableActivityTransfer()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransfer";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers.", true);

                reqData = this.Send("activity transfers");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                throw new Exception("Seeding transfers failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfers status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                    cmndPurge.CommandText = "DELETE FROM tActivityTransfer WHERE tActivityTransfer.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging transfers failed.", ex);
            }
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="reqData"></param>
        protected override void Save(string reqData)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmndTrans = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndTrans = conn.CreateCommand())
                            //{
                            //cmndTrans.CommandText = "tActivityTransfer";
                            //cmndTrans.CommandType = CommandType.Text;
                            //cmndTrans.IndexName = "UQ_ActivityTransferOnActivityTransferFKRefno";

                            //using (SQLiteCommand cmndAct = conn.CreateCommand())
                            //{
                            //cmndAct.CommandText = "tActivity";
                            //cmndAct.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                            //cmndWhs.CommandType = CommandType.Text;

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rs = cmndTrans.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int transFKRefno = 0;
                            string transDesc = string.Empty;
                            //DateTime transDateTime = DateAndTime.DateSerial(1900, 1, 1);
                            //DateTime arrivalDateTime = DateAndTime.DateSerial(1900, 1, 1);
                            DateTime transDateTime = new DateTime(1900, 1, 1);
                            DateTime arrivalDateTime = new DateTime(1900, 1, 1);

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                transFKRefno = Convert.ToInt32(drow["ActivityTransferFKRefno"]);
                                transDesc = drow["ActivityTransferDescription"].ToString();
                                transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                                arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);

                                errMsg = "Trans fk refno " + transFKRefno;
                                errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                                errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                                errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();

                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                                errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
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
                                        throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                    //}
                                }

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.Transfers);
                                //rsAct.Insert(recAct);

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                cmndTrans.CommandText = "INSERT INTO tActivityTransfer (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityTransferFKRefno, " +
                                    "ActivityTransferStringFKRefno, " +
                                    "TransferToWarehouseLocationMapRefno, " +
                                    "ActivityTransferDescription, " +
                                    "ActivityTransferDateTime, " +
                                    "ActivityTransferArrivalDateTime" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@activityTransferFKRefno, " +
                                    "@activityTransferStringFKRefno, " +
                                    "@transferToWarehouseLocationMapRefno, " +
                                    "@activityTransferDescription, " +
                                    "@activityTransferDateTime, " +
                                    "@activityTransferArrivalDateTime" +
                                    ")";
                                cmndTrans.CommandType = CommandType.Text;
                                //cmndTrans.IndexName = "UQ_ActivityTransferOnActivityTransferFKRefno";
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.Transfers)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferFKRefno", transFKRefno));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferStringFKRefno", Convert.ToString(transFKRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@transferToWarehouseLocationMapRefno", whsLocMapRefno));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDescription", transDesc));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDateTime", transDateTime));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferArrivalDateTime", arrivalDateTime));

                                cmndTrans.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActSubType, (int)SysCommon.ActSubTypes.Transfers);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.FKRefno, transFKRefno);
                                //rec.SetString((int)SysCommon.ActTransferFields.StringFKRefno, Convert.ToString(transFKRefno));
                                //rec.SetString((int)SysCommon.ActTransferFields.Description, transDesc);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.TransferToWhsLocMapRefno, whsLocMapRefno);
                                //rec.SetDateTime((int)SysCommon.ActTransferFields.DateTime, transDateTime);
                                //rec.SetDateTime((int)SysCommon.ActTransferFields.ArrivalDateTime, arrivalDateTime);
                                //rs.Insert(rec);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        //drWhs.Close();
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness Violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfers failed.", ex);
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
                + "<action>REQUEST TRANSFERS</action>"
                + SysCommon.GetRequestDefaults(false)
                + "</command>" + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
