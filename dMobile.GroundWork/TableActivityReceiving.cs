// <copyright file="TableActivityReceiving.cs" company="InterSect Business Systems Inc">
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving.", true);

                var reqData = await this.SendRequest();

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving failed.", true);

                throw new Exception("Seeding receiving failed.", ex);
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receiving.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Receiving status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving.", true);

                var query = "DELETE FROM tActivityReceiving";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving completed.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving failed.", true);

                throw new Exception("Database exception occurred. Purging receiving failed.", ex);
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
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityPurchaseOrder"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actRefno = 0;
                            var progValue = 0;
                            var whsLocMapRefno = 0;
                            var recvFKRefno = 0;
                            var recvPONumber = string.Empty;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPurchaseOrder"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                recvFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                recvPONumber = drow["ActivityPurchaseOrderPONumber"].ToString();
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                var query = "INSERT INTO tActivityReceiving (" +
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
                                var parameters = new object[]
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving failed.", false);
                    throw new Exception("Database exception occurred. Saving receiving failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
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
