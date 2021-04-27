namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using SQLite;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="TableActivityPhysicalCount" />
    /// </summary>
    public class TableActivityPhysicalCount : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPhysicalCount"/> class.
        /// </summary>
        public TableActivityPhysicalCount()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCount";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            string reqData = string.Empty;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts.", true);

                reqData = await this.Send("physical counts");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding physical counts failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                string query = "SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} physical counts.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Physical counts status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts.", true);

                string query = "DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging physical counts failed.", ex);
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
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityCount"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);


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
                                errMsg = "Arguments:";

                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, cmndWhs);

                                cmndAct.CommandText = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (@activityStatus, @activitySubType)";
                                cmndAct.CommandType = CommandType.Text;
                                cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(0)));
                                cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts)));

                                cmndAct.ExecuteNonQuery();

                                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)SysCommon.ActSubTypes.PhysicalCounts);
                                //recAct.SetInt32((int)SysCommon.ActFields.Status, 0);
                                //rsAct.Insert(recAct);

                                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivity";
                                cmndID.CommandType = CommandType.Text;

                                actRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                countFKRefno = Convert.ToInt32(drow["ActivityCountFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + countFKRefno;

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
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts)));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountFKRefno", countFKRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountStringFKRefno", countFKRefno.ToString()));
                                cmndCount.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountType", Convert.ToInt32(drow["ActivityCountType"])));

                                //rec.SetInt32((int)SysCommon.ActCountFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActCountFields.ActSubType, (int)SysCommon.ActSubTypes.PhysicalCounts);
                                //rec.SetInt32((int)SysCommon.ActCountFields.FKRefno, countFKRefno);
                                //rec.SetString((int)SysCommon.ActCountFields.StringFKRefno, countFKRefno.ToString());
                                //rec.SetInt32((int)SysCommon.ActCountFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetInt32((int)SysCommon.ActCountFields.Type, Convert.ToInt32(drow["ActivityCountType"]));

                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);
                                //rec.SetDateTime((int)SysCommon.ActCountFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                cmndCount.Parameters.Add(new SQLiteParameter("@activityCountDateTime", DateTime.Parse(dt.ToShortDateString())));

                                switch (Convert.ToInt32(drow["ActivityCountType"]))
                                {
                                    case (int)SysCommon.ActCountTypeFields.PhysicalCountByWarehouse:
                                        // TODO: Resource strings
                                        actCountTypeDesc = "Standard Physical Count By Warehouse";
                                        break;

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
                                }

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
                            //}

                            //drWhs.Close();
                            //}
                            //}
                            //}
                            //}
                        }
                        //}
                        //}

                        transaction.Commit();

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts failed.", true);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    // TODO: Resource strings
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving physical counts failed." + Environment.NewLine + errMsg, ex);
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
            {
                return string.Empty;

            }

            StringBuilder sb = new StringBuilder();
            bool prevIsWhitespace = false;
            char[] charArray = input.ToCharArray();
            for (int i = 0; i < input.Length; ++i)
            {
                if (char.IsWhiteSpace(charArray[i]))
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
                + "<action>REQUEST PHYSICAL COUNTS</action>"
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
