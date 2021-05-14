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
    /// Defines the <see cref="TableActivityReceiving" />
    /// </summary>
    public class TableActivityReceiving : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceiving"/> class.
        /// </summary>
        public TableActivityReceiving()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceiving";
        }

        /// <summary>
        /// The Request
        /// </summary>
        public override async Task Request()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings.", true);

                string reqData = await this.Send("receiving");

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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings complete.", true);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings failed.", true);

                // TODO: Resource strings
                throw new Exception("Seeding receivings failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receivings.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Receivings status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings.", true);

                string query = "DELETE FROM tActivityReceiving";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings completed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receivings failed.", ex);
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
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityPurchaseOrder"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int recvFKRefno = 0;
                            string recvPONumber = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                            {
                                errMsg = "Arguments:";

                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                                actRefno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivity"));
                                recvFKRefno = Convert.ToInt32(drow["ActivityPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Recv fk refno " + recvFKRefno;
                                recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                                errMsg += Environment.NewLine + "PO number " + recvPONumber;
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, transaction);

                                string query = "INSERT INTO tActivityReceiving (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityReceivingFKRefno, " +
                                    "ActivityReceivingStringFKRefno, " +
                                    "ActivityReceivingPONumber, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityReceivingIssuedDateTime, " +
                                    "ActivityReceivingArrivalDateTime, " +
                                    "ActivityReceivingVendorContactName, " +
                                    "ActivityReceivingVendorSupplierContactName, " +
                                    "ActivityReceivingVendorSupplierGTINCompanyPrefix" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
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
                                    Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                    recvFKRefno,
                                    Convert.ToString(recvFKRefno),
                                    recvPONumber,
                                    whsLocMapRefno,
                                    Convert.ToDateTime(drow["ActivityPurchaseOrderIssuedDateTime"]),
                                    Convert.ToDateTime(drow["ActivityPurchaseOrderArrivalDateTime"]),
                                    drow["ActivityPurchaseOrderVendorContactName"].ToString(),
                                    drow["ActivityPurchaseOrderVendorSupplierContactName"].ToString(),
                                    drow["ActivityPurchaseOrderVendorSupplierGTINCompanyPrefix"].ToString(),
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

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings failed.", false);

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving receivings failed." + Environment.NewLine + errMsg, ex);
                }
            });
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
                  + "<action>REQUEST PURCHASE ORDERS</action>"
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
