// <copyright file="clsTableActivityPhysicalCount.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityPhysicalCount" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts.", true);

                string reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding physical counts failed.", true);

                throw new Exception("Seeding physical counts failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                string query = "SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} physical counts.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Physical counts status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts.", true);

                string query = "DELETE FROM tActivityCount WHERE tActivityCount.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging physical counts failed.", true);

                throw new Exception("Database exception occurred. Purging physical counts failed.", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        protected override async Task Save(string reqData)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityCount"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            string actCountTypeDesc = string.Empty;
                            int countFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityCount"].Rows)
                            {
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                countFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                string query = "INSERT INTO tActivityCount (" +
                                "ActivityRefno, " +
                                "ActivitySubType, " +
                                "ActivityFKRefno, " +
                                "ActivityStringFKRefno, " +
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
                                    Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts),
                                    countFKRefno,
                                    countFKRefno.ToString(),
                                    whsLocMapRefno,
                                    Convert.ToInt32(drow["ActivityCountType"]),
                                };

                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityCountDateTime"]);
                                // rec.SetDateTime((int)SysCommon.ActCountFields.DateTime, DateTime.Parse(dt.ToShortDateString()));
                                parameters.Add(DateTime.Parse(dt.ToShortDateString()));

                                switch (Convert.ToInt32(drow["ActivityCountType"]))
                                {
                                    case (int)SysCommon.ActCountTypeFields.PhysicalCountByWarehouse:

                                        actCountTypeDesc = "Standard Physical Count By Warehouse";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.CycleCountByWarehouse:

                                        actCountTypeDesc = "Cycle Count By Warehouse";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.CycleCountBySection:

                                        actCountTypeDesc = "Cycle Count By Section";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.CycleCountBySlot:

                                        actCountTypeDesc = "Cycle Count By Slot";
                                        break;

                                    case (int)SysCommon.ActCountTypeFields.BaseCycleCountByWarehouse:

                                        actCountTypeDesc = "Base Cycle Count By Warehouse";
                                        break;
                                }

                                parameters.Add(actCountTypeDesc);

                                // <wo="14563" comment="receiving strings up to 75 in length when database wants max 60. This is the viewable description when selecting line item." />
                                string incomingDescription = this.ReduceWhitespace(drow["ActivityCountDescription"].ToString());

                                if (incomingDescription.Length > 60)
                                {
                                    incomingDescription = incomingDescription.Replace(" ", string.Empty); // remove all spaces
                                }

                                if (incomingDescription.Length > 60)
                                {
                                    incomingDescription = incomingDescription.Substring(0, 60); // truncate the end if we need to.
                                }

                                parameters.Add(incomingDescription);

                                transaction.Execute(query, parameters.ToArray());

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical counts failed.", true);

                    throw new Exception("Database exception occurred. Saving physical counts failed.", ex);
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
                    {
                        continue;
                    }

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
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PHYSICAL COUNTS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
