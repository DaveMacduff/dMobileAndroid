// <copyright file="TableActivityPicking.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityPicking" />.
    /// </summary>
    public class TableActivityPicking : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPicking"/> class.
        /// </summary>
        public TableActivityPicking()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPicking";
        }

        /// <summary>
        /// The RequestData.
        /// </summary>
        /// <param name="criteria"></param>
        public override async Task RequestData(XDocument criteria)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders.", true);

                string reqData = await this.SendRequest(criteria);
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales orders failed.", true);

                throw new Exception("Seeding sales orders failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityRefno) FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sales orders.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sales orders status failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders.", true);

                string query = "DELETE FROM tActivityPickingRoute WHERE PickingActivityRefno IN (SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityPicking WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales orders failed.", true);

                throw new Exception("Database exception occurred. Purging sales orders failed.", ex);
            }
        }

        /// <summary>
        /// The SendRequest.
        /// </summary>
        /// <param name="criteria">The criteria<see cref="XDocument"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(XDocument criteria)
        {
            return await this.SendComm(await this.CreateRequest(criteria));
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityPicking"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actRefno = 0;
                            int progValue = 0;
                            int whsLocMapRefno = 0;
                            int routeRefno = 0;
                            string pickRouteCode = string.Empty;
                            int actPickFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityPicking"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", 0, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                                actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                                actPickFKRefno = Convert.ToInt32(drow["ActivityFKRefno"]);
                                whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, transaction);

                                string queryInsertPicking = "INSERT INTO tActivityPicking (" +
                                    "ActivityRefno, " +
                                    "ActivitySubType, " +
                                    "ActivityFKRefno, " +
                                    "ActivityStringFKRefno, " +
                                    "WarehouseLocationMapRefno, " +
                                    "ActivityPickingOrderedDateTime, " +
                                    "ActivityPickingDeliveryDateTime, " +
                                    "ActivityPickingCustomerContactName, " +
                                    "ActivityPickingCustomerShipToInfo, " +
                                    "ActivityPickingIsReviewed" +
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
                                    "?" +
                                    ")";
                                object[] parametersInsertPicking = new object[]
                                {
                                    actRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                    actPickFKRefno,
                                    Convert.ToString(actPickFKRefno),
                                    whsLocMapRefno,
                                    Convert.ToDateTime(drow["ActivityPickingOrderedDateTime"]),
                                    Convert.ToDateTime(drow["ActivityPickingDeliveryDateTime"]),
                                    Convert.ToString(drow["ActivityPickingCustomerContactName"]),
                                    Convert.ToString(drow["ActivityPickingCustomerShipToInfo"]),
                                    false,
                                };

                                transaction.Execute(queryInsertPicking, parametersInsertPicking);

                                routeRefno = 0;
                                pickRouteCode = Convert.ToString(drow["ActivityPickingRouteCode"]);
                                if (!string.IsNullOrEmpty(pickRouteCode))
                                {
                                    List<tRouteModel> resultRoutes = transaction.Query<tRouteModel>("SELECT * FROM tRoute WHERE RouteCode = ?", pickRouteCode);

                                    if (resultRoutes?.Count > 0)
                                    {
                                        routeRefno = resultRoutes[0].RouteRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("drRoute.Read() failed.");
                                    }
                                }

                                if (routeRefno != 0)
                                {
                                    transaction.Execute("INSERT INTO tActivityPickingRoute (PickingActivityRefno, RouteRefno, RouteStop) VALUES (?, ?, ?)", actRefno, routeRefno, Convert.ToInt32(drow["ActivityPickingRouteStop"]));
                                }

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales orders failed.", true);

                    throw new Exception("Database exception occurred. Saving sales orders failed.", ex);
                }
            });
        }

        /// <summary>
        /// The CreateRequest.
        /// </summary>
        /// <param name="critFilter"></param>
        /// <returns></returns>
        protected async override Task<string> CreateRequest(XDocument critFilter)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request;
            if (critFilter == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDERS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<whs_id></whs_id>"
                    + "<sect_refno></sect_refno>"
                    + "</command>"
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDERS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<whs_id>" + critFilter.Root.Element("whs_id").Value + "</whs_id>"
                    + "<sect_refno>" + critFilter.Root.Element("sect_refno").Value + "</sect_refno>"
                    + "</command>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
