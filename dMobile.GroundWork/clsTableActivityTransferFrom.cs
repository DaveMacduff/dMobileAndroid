namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class TableActivityTransferFrom : TableBase
    {
        #region Constructors

        public TableActivityTransferFrom()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferFrom";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            string reqData = string.Empty;

            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers from.", true);

                reqData = this.Send("activity transfers from");

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers from complete.", true);
            }
            catch (Exception ex)
            {
                throw new Exception("Seeding transfers from failed.", ex);
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
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(tActivity.ActivityRefno)"
                        + " FROM tActivity"
                        + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers from.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Transfers from status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityTransfer WHERE tActivityTransfer.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferFrom;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers from failed.", true);

                throw new Exception("Database exception occurred. Purging transfers from failed.", ex);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData)
        {
            string errMsg = string.Empty;
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

            using (SQLiteCommand cmndTrans = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{

                            //SQLiteUpdatableRecord recAct = rsAct.CreateRecord();

                            //using (SQLiteResultSet rs = cmndTrans.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SQLiteUpdatableRecord rec = rs.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int transFKRefno = 0;
                            string transDesc = string.Empty;
                            DateTime transDateTime = new DateTime(1900, 1, 1);
                            DateTime arrivalDateTime = new DateTime(1900, 1, 1);

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityTransfer"].Rows)
                            {
                                errMsg = "Arguments:";

                                transFKRefno = Convert.ToInt32(drow["ActivityTransferFKRefno"]);
                                errMsg = "Trans fk refno " + transFKRefno;
                                transDesc = drow["ActivityTransferDescription"].ToString();
                                errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                                transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                                errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                                arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);
                                errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();

                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                                errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;

                                cmndWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndWhs.CommandType = CommandType.Text;
                                //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(locMapWhsFKRefno)));

                                using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                    if (drWhs.Read())
                                    {
                                        whsLocMapRefno = drWhs.GetInt32(Convert.ToInt32(SysCommon.LocMapWhsFields.LocMapRefno));
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                }

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.TransferFrom);
                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

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
                                //cmndTrans.IndexName = "UQ_ActivityTransferOnActivityTransferFKRefnoAndActivitySubType";
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferFKRefno", Convert.ToInt32(transFKRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferStringFKRefno", Convert.ToString(transFKRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDescription", transDesc));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@transferToWarehouseLocationMapRefno", Convert.ToInt32(whsLocMapRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDateTime", Convert.ToDateTime(transDateTime)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferArrivalDateTime", Convert.ToDateTime(arrivalDateTime)));

                                cmndTrans.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActSubType, (int)SysCommon.ActSubTypes.TransferFrom);
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
                            //drWhs.Close();
                            //}

                            transaction.Commit();
                        }
                    }
                    //}

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from complete.", false);
                    //}
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers from failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness Violation." + Environment.NewLine + errMsg, ex);
                    //}

                    throw new Exception("Database exception occurred. Saving transfers from failed.", ex);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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

        #endregion
    }
}