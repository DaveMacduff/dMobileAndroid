// <copyright file="TableActivityProductionTicketByProduct.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketByProduct" />.
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
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products.", true);

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket add-products failed.", true);

                throw new Exception("Database exception occurred. Seeding ticket add-products failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket add-products.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Ticket add-product status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products.", true);

                var query = "DELETE FROM tActivityProductionByProduct"
                            + " WHERE tActivityProductionByProduct.ActivityRefno IN"
                            + " (SELECT tActivityProduction.ActivityRefno FROM tActivityProduction WHERE tActivityProduction.ActivitySubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivityProduction WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-products complete.", true);

                throw new Exception("Database exception occurred. Purging ticket add-products complete.", ex);
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityProduction"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actRefno = 0;
                            var progValue = 0;
                            var whsLocMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityProduction"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                // To remove time - convert twice
                                var productionDateTime = Convert.ToDateTime(drow["ActivityProductionDateTime"]).ToShortDateString();
                                var productionDesc = drow["ActivityProductionDescription"].ToString();
                                var queryActivityProduction = "INSERT INTO tActivityProduction (" +
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
                                var parametersActivityProduction = new object[]
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct),
                                    whsLocMapRefno,
                                    DateTime.Parse(productionDateTime),
                                    productionDesc,
                                };

                                transaction.Execute(queryActivityProduction, parametersActivityProduction);

                                var productionFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                var queryActivityProductionByProduct = "INSERT INTO tActivityProductionByProduct (" +
                                                                       "ActivityRefno, " +
                                                                       "ActivityFKRefno, " +
                                                                       "ActivityStringFKRefno" +
                                                                       ") VALUES (" +
                                                                       "?, " +
                                                                       "?, " +
                                                                       "?" +
                                                                       ")";
                                var parametersActivityProductionByProduct = new object[]
                                {
                                    actRefno,
                                    productionFKRefno,
                                    Convert.ToString(productionFKRefno),
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-products failed.", false);
                    throw new Exception("Database exception occurred. Saving ticket add-products failed.", ex);
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
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT</action>"
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
