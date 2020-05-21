namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class TableActivityTransferTo : TableBase
    {
        #region Constructors

        public TableActivityTransferTo()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityTransferTo";
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
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers to.", true);

                reqData = this.Send("activity transfers to");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers to complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                throw new Exception("Seeding transfers to failed.", ex);
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
                        + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferTo;
                    cmndStatus.CommandType = CommandType.Text;

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers to.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Transfers to status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityTransfer "
                        + "WHERE tActivityTransfer.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferTo;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity "
                        + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TransferTo;
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to failed.", true);

                throw new Exception("Database exception occurred. Purging transfers to failed.", ex);
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityTransfer"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            cmndID.CommandType = CommandType.Text;

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
                                    //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                    //{
                                    if (drWhs.Read())
                                    {
                                        whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drWhs.Read() failed. Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
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
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.TransferTo);
                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
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
                                //cmndTrans.IndexName = "UQ_ActivityTransferOnActivityTransferFKRefnoAndActivitySubType";
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(actRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferFKRefno", Convert.ToInt32(transFKRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferStringFKRefno", Convert.ToString(transFKRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@transferToWarehouseLocationMapRefno", Convert.ToInt32(whsLocMapRefno)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDescription", transDesc));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferDateTime", Convert.ToDateTime(transDateTime)));
                                cmndTrans.Parameters.Add(new SQLiteParameter("@activityTransferArrivalDateTime", Convert.ToDateTime(arrivalDateTime)));

                                cmndTrans.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.ActSubType, (int)SysCommon.ActSubTypes.TransferTo);
                                //rec.SetInt32((int)SysCommon.ActTransferFields.FKRefno, transFKRefno);
                                //rec.SetString((int)SysCommon.ActTransferFields.StringFKRefno, Convert.ToString(transFKRefno));
                                //rec.SetInt32((int)SysCommon.ActTransferFields.TransferToWhsLocMapRefno, whsLocMapRefno);
                                //rec.SetString((int)SysCommon.ActTransferFields.Description, transDesc);
                                //rec.SetDateTime((int)SysCommon.ActTransferFields.DateTime, transDateTime);
                                //rec.SetDateTime((int)SysCommon.ActTransferFields.ArrivalDateTime, arrivalDateTime);
                                //rs.Insert(rec);

                                progValue++;

                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfers to failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness Violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfers to failed.", ex);
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