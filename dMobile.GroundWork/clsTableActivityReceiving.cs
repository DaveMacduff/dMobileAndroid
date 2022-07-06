// <copyright file="clsTableActivityReceiving.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityReceiving" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings.", true);

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receivings failed.", true);

                throw new Exception("Seeding receivings failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receivings.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Receivings status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings.", true);

                string query = "DELETE FROM tActivityReceiving";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings completed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receivings failed.", true);

                throw new Exception("Database exception occurred. Purging receivings failed.", ex);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityPurchaseOrder"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int recvFKRefno = 0;
                            string recvPONumber = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                recvFKRefno = Convert.ToInt32(drow["ActivityPurchaseOrderFKRefno"]);
                                recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                string query = "INSERT INTO tActivityReceiving (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityFKRefno, " +
                                    "ActivityStringFKRefno, " +
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
                                object[] parameters = new object[]
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receivings failed.", false);
                    throw new Exception("Database exception occurred. Saving receivings failed.", ex);
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
                  + "<action>REQUEST PURCHASE ORDERS</action>"
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
