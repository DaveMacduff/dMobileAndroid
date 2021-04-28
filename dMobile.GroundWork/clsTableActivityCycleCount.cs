namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

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
        public override async Task Request()
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

                    await this.Save(reqData);
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
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(tActivity.ActivityRefno) FROM tActivity WHERE tActivity.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} cycle count(s)", rowsAffected), true);
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
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging cycle counts.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

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
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
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

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                                actRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivity"));

                                countFKRefno = Convert.ToInt32(drow["ActivityCountFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + countFKRefno;
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);
                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);

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

                                string sql = "INSERT INTO tActivityCount (" +
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
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";

                                List<object> parameters = new List<object>()
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                    countFKRefno,
                                    countFKRefno.ToString(),
                                    whsLocMapRefno,
                                    Convert.ToInt32(drow["ActivityCountType"]),
                                    DateTime.Parse(dt.ToShortDateString()),
                                    actCountTypeDesc,
                                    incomingDescription
                                };

                                transaction.Execute(sql, parameters);

                                //cmndCount.CommandType = CommandType.Text;
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityRefno", actRefno));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountFKRefno", countFKRefno));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountStringFKRefno", countFKRefno.ToString()));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", whsLocMapRefno));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountType", Convert.ToInt32(drow["ActivityCountType"])));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountDateTime", DateTime.Parse(dt.ToShortDateString())));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountTypeDescription", actCountTypeDesc));
                                //cmndCount.Parameters.Add(new SQLiteParameter("@activityCountDescription", incomingDescription));
                                //cmndCount.ExecuteNonQuery();

                                //rec.SetInt32((int)SysCommon.ActCountFields.ActRefno, actRefno);
                                //rec.SetInt32((int)SysCommon.ActCountFields.ActSubType, (int)SysCommon.ActSubTypes.CycleCounts);
                                //rec.SetInt32(Convert.ToInt32(SysCommon.ActCountFields.FKRefno), countFKRefno);
                                //rec.SetString((int)SysCommon.ActCountFields.StringFKRefno, countFKRefno.ToString());
                                //rec.SetInt32((int)SysCommon.ActCountFields.WhsLocMapRefno, whsLocMapRefno);
                                //rec.SetDateTime((int)SysCommon.ActCountFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                //rec.SetInt32((int)SysCommon.ActCountFields.Type, Convert.ToInt32(drow["ActivityCountType"]));
                                //rec.SetString((int)SysCommon.ActCountFields.TypeDescription, actCountTypeDesc);
                                //rec.SetString((int)SysCommon.ActCountFields.Description, incomingDescription);

                                //rs.Insert(rec);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

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

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving cycle counts failed." + Environment.NewLine + errMsg, ex);
                }
            });
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
