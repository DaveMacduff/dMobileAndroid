// <copyright file="clsTableActivityProductionTicketMaterial.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketMaterial" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials.", true);

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials failed.", true);

                throw new Exception("Seeding ticket materials failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket materials.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Material tickets status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials.", true);

                string query = "DELETE FROM tActivityProductionMaterial"
                    + " WHERE tActivityProductionMaterial.ActivityRefno IN"
                    + " (SELECT tActivityProduction.ActivityRefno"
                    + " FROM tActivityProduction"
                    + " WHERE tActivityProduction.ActivitySubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket materials failed.", true);

                throw new Exception("Database exception occurred. Purging ticket materials failed.", ex);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int actProductionFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                // To remove time - convert twice
                                DateTime dt = Convert.ToDateTime(drow["ActivityProductionDateTime"]);

                                string queryActivityProduction = "INSERT INTO tActivityProduction (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityProductionDateTime, " +
                                    "ActivityProductionDescription" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";
                                object[] parametersActivityProduction = new object[]
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial),
                                    whsLocMapRefno,
                                    DateTime.Parse(dt.ToShortDateString()),
                                    Convert.ToString(drow["ActivityProductionDescription"]),
                                };

                                transaction.Execute(queryActivityProduction, parametersActivityProduction);

                                actProductionFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                string queryActivityProductionMaterial = "INSERT INTO tActivityProductionMaterial (" +
                                    "ActivityRefno, " +
                                    "ActivityFKRefno, " +
                                    "ActivityStringFKRefno " +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";
                                object[] parametersActivityProductionMaterial = new object[]
                                {
                                    actRefno,
                                    actProductionFKRefno,
                                    Convert.ToString(actProductionFKRefno),
                                };

                                transaction.Execute(queryActivityProductionMaterial, parametersActivityProductionMaterial);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket materials failed.", false);
                    throw new Exception("Database exception occurred. Saving ticket materials failed.", ex);
                }
            });
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
                + "<action>REQUEST PRODUCTION TICKET MATERIAL</action>"
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
