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
    /// Defines the <see cref="TableActivityCycleCount" />
    /// </summary>
    public class TableActivityCycleCount : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCount"/> class.
        /// </summary>
        public TableActivityCycleCount()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCount";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle counts.", true);

                reqData = this.Send("cycle counts");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle counts complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding cycle counts failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding cycle counts failed.", ex);
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
                    cmndStatus.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} cycle count(s)", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Cycle counts status failed.", ex);
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
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle counts.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                    cmndPurge.CommandType = CommandType.Text;

                    cmndPurge.CommandText = "DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle counts complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle counts failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging cycle counts failed.", ex);
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

            using (SQLiteCommand cmndCount = conn.CreateCommand())
            using (SQLiteCommand cmndAct = conn.CreateCommand())
            using (SQLiteCommand cmndID = conn.CreateCommand())
            using (SQLiteCommand cmndWhs = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    using (DataSet ds = new DataSet())
                    {
                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle counts.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityCount"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            //using (SQLiteCommand cmndCount = conn.CreateCommand())
                            //{
                            //cmndCount.CommandText = "tActivityCount";
                            //cmndCount.CommandType = CommandType.Text;
                            //cmndCount.IndexName = "UQ_ActivityCountOnActivityCountFKRefno";

                            //using (SQLiteCommand cmndAct = conn.CreateCommand())
                            //{
                            //cmndAct.CommandText = "tActivity";
                            //cmndAct.CommandType = CommandType.Text;

                            //using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                            //using (SQLiteCommand cmndID = conn.CreateCommand())
                            //{
                            //cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                            //cmndID.CommandType = CommandType.Text;

                            //using (SQLiteCommand cmndWhs = conn.CreateCommand())
                            //{
                            //cmndWhs.CommandText = "tLocationMapWarehouse";
                            //cmndWhs.CommandType = CommandType.Text;
                            //cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            //using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
                            //{
                            //using (SqlCeResultSet rs = cmndCount.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            string actCountTypeDesc = string.Empty;
                            int countFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityCount"].Rows)
                            {
                                // TODO: Resource strings
                                errMsg = "Arguments:";

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.CycleCounts);
                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);

                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                cmndCount.CommandText = "INSERT INTO tActivityCount (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityCountFKRefno, " +
                                    "ActivityCountStringFKRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityCountType, " +
                                    "ActivityCountDateTime, " +
                                    "ActivityCountTypeDescription, " +
                                    "ActivityCountDescription" +
                                    ") VALUES (" +
                                    "@activityRefno, " +
                                    "@activitySubType, " +
                                    "@activityCountFKRefno, " +
                                    "@activityCountStringFKRefno, " +
                                    "@warehouseLocationMapRefno, " +
                                    "@activityCountType, " +
                                    "@activityCountDateTime, " +
                                    "@activityCountTypeDescription, " +
                                    "@activityCountDescription" +
                                    ")";
                                cmndCount.CommandType = CommandType.Text;
                                //cmndCount.IndexName = "UQ_ActivityCountOnActivityCountFKRefno";
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                                //rec.SetInt32((int)SysCommon.ActCountFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActCountFields.ActSubType, (int)SysCommon.ActSubTypes.CycleCounts);

                                countFKRefno = Convert.ToInt32(drow["ActivityCountFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + countFKRefno;
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndWhs);
                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);

                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountFKRefno", countFKRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountStringFKRefno", countFKRefno.ToString()));
                                cmndCount.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountType", Convert.ToInt32(drow["ActivityCountType"])));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountDateTime", DateTime.Parse(dt.ToShortDateString())));

                                //rec.SetInt32(Convert.ToInt32(SysCommon.ActCountFields.FKRefno), countFKRefno);
                                //rec.SetString((int)SysCommon.ActCountFields.StringFKRefno, countFKRefno.ToString());
                                //rec.SetInt32((int)SysCommon.ActCountFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetDateTime((int)SysCommon.ActCountFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                //rec.SetInt32((int)SysCommon.ActCountFields.Type, Convert.ToInt32(drow["ActivityCountType"]));

                                switch (Convert.ToInt32(drow["ActivityCountType"]))
                                {
                                    case (int)SysCommon.ActCountTypeFields.CycleCountByWarehouse:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Cycle Count By Warehouse";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.CycleCountBySection:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Cycle Count By Section";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.CycleCountBySlot:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Cycle Count By Slot";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.BaseCycleCountByWarehouse:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Base Cycle Count By Warehouse";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.PhysicalCountByWarehouse:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Standard Physical Count By Warehouse";
                                        break;
                                }

                                // TODO: Resource strings
                                errMsg += Environment.NewLine + "Type \'" + actCountTypeDesc + "\'";

                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountTypeDescription", actCountTypeDesc));
                                //rec.SetString((int)SysCommon.ActCountFields.TypeDescription, actCountTypeDesc);

                                // <wo="14563" comment="receiving strings up to 75 in length when database wants max 60. This is the viewable description when selecting line item." />
                                string incomingDescription = ReduceWhitespace(drow["ActivityCountDescription"].ToString());

                                if (incomingDescription.Length > 60)
                                {
                                    incomingDescription = incomingDescription.Replace(" ", string.Empty); // remove all spaces
                                }

                                if (incomingDescription.Length > 60)
                                {
                                    incomingDescription = incomingDescription.Substring(0, 60); // truncate the end if we need to.
                                }

                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountDescription", incomingDescription));
                                //rec.SetString((int)SysCommon.ActCountFields.Description, incomingDescription);

                                cmndCount.ExecuteNonQuery();

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
                        //}
                        //}
                        //}
                        //}
                        //}
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle counts complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle counts failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving cycle counts failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        /// Reduces excess whitespace in a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReduceWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            bool prevIsWhitespace = false;
            char[] charArray = input.ToCharArray();
            for (int i = 0; i < input.Length; ++i)
            {
                if (Char.IsWhiteSpace(charArray[i]))
                {
                    if (prevIsWhitespace)
                        continue;
                    prevIsWhitespace = true;
                }
                else
                {
                    prevIsWhitespace = false;
                }

                sb.Append(charArray[i]);
            }
            return sb.ToString();
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
                + "<action>REQUEST CYCLE COUNTS</action>"
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
