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
    /// Defines the <see cref="TableActivityProductionTicketByProduct" />
    /// </summary>
    public class TableActivityProductionTicketByProduct : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProduct"/> class.
        /// </summary>
        public TableActivityProductionTicketByProduct()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProduct";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            string reqData = string.Empty;

            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products.", true);

                reqData = await this.Send("production ticket add-product(s)");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Seeding ticket add-products failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-products.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Ticket add-product status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products.", true);

                string query = "DELETE FROM tActivityProductionByProduct"
                    + " WHERE tActivityProductionByProduct.ActivityRefno IN"
                    + " (SELECT tActivityProduction.ActivityRefno FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityProduction WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging ticket add-products complete.", ex);
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
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                                actRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivity"));
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                // To remove time - convert twice
                                string productionDateTime = Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString();
                                errMsg += Environment.NewLine + "Date time " + productionDateTime;

                                string productionDesc = drow["ActivityProductionDescription"].ToString();
                                errMsg += Environment.NewLine + "Desc \'" + productionDesc + "\'";

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
                                List<object> parametersActivityProduction = new List<object>()
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                                    whsLocMapRefno,
                                    DateTime.Parse(productionDateTime),
                                    productionDesc
                                };

                                transaction.Execute(queryActivityProduction, parametersActivityProduction);

                                int productionFKRefno = Convert.ToInt32(drow["ActivityProductionFKRefno"]);
                                errMsg += Environment.NewLine + "FK refno " + productionFKRefno;

                                string queryActivityProductionByProduct = "INSERT INTO tActivityProductionByProduct (" +
                                    "ActivityRefno, " +
                                    "ActivityProductionFKRefno, " +
                                    "ActivityProductionStringFKRefno" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";
                                List<object> parametersActivityProductionByProduct = new List<object>()
                                {
                                    actRefno,
                                    productionFKRefno,
                                    Convert.ToString(productionFKRefno)
                                };

                                transaction.Execute(queryActivityProductionByProduct, parametersActivityProductionByProduct);

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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving ticket add-products failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> Create()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT</action>"
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
