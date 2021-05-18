namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

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
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfers to.", true);

                reqData = await this.Send("activity transfers to");

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
        public override async Task<int> DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} transfers to.", rowsAffected), true);
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
        public override async Task Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfers to.", true);

            try
            {
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityTransfer WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

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
        protected override async Task Save(string reqData)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
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
                                transDesc = Convert.ToString(drow["ActivityTransferDescription"]);
                                errMsg += Environment.NewLine + "Trans desc \'" + transDesc + "\'";
                                transDateTime = Convert.ToDateTime(drow["ActivityTransferDateTime"]);
                                errMsg += Environment.NewLine + "Trans date " + transDateTime.ToShortDateString();
                                arrivalDateTime = Convert.ToDateTime(drow["ActivityTransferArrivalDateTime"]);
                                errMsg += Environment.NewLine + "Trans arrival date " + arrivalDateTime.ToShortDateString();
                                locMapWhsFKRefno = Convert.ToInt32(drow["TransferToLocationMapWarehouseFKRefno"]);
                                errMsg += Environment.NewLine + "Whs fk refno " + locMapWhsFKRefno;

                                List<tLocationMapWarehouseModel> results = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                                if (results?.Count > 0)
                                {
                                    whsLocMapRefno = results[0].LocationMapRefno;
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Unable to locate the warehouse fk refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                }

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));

                                actRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivity"));

                                string query = "INSERT INTO tActivityTransfer (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityTransferFKRefno, " +
                                    "ActivityTransferStringFKRefno, " +
                                    "TransferToWarehouseLocationMapRefno, " +
                                    "ActivityTransferDescription, " +
                                    "ActivityTransferDateTime, " +
                                    "ActivityTransferArrivalDateTime" +
                                    ") VALUES (" +
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
                                    Convert.ToInt32(actRefno),
                                    Convert.ToInt32(SysCommon.ActSubTypes.TransferTo),
                                    Convert.ToInt32(transFKRefno),
                                    Convert.ToString(transFKRefno),
                                    Convert.ToInt32(whsLocMapRefno),
                                    transDesc,
                                    Convert.ToDateTime(transDateTime),
                                    Convert.ToDateTime(arrivalDateTime)
                                };

                                transaction.Execute(query, parameters);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();
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

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving transfers to failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> Create()
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