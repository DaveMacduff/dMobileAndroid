// <copyright file="clsTableActivityProductionTicketFinishedItem.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketFinishedItem" />.
    /// </summary>
    public class TableActivityProductionTicketFinishedItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinishedItem"/> class.
        /// </summary>
        public TableActivityProductionTicketFinishedItem()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinishedItem";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        public override async Task RequestData(int actRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items.", true);

                string reqData;
                if (actRefno == 0)
                {
                    List<tActivityModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>("SELECT ActivityRefno FROM tActivity WHERE ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityModel activityModel in results)
                        {
                            actRefno = Convert.ToInt32(activityModel.ActivityRefno);
                            reqData = await this.SendRequest(actRefno);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actRefno);
                            }
                        }
                    }
                }
                else
                {
                    reqData = await this.SendRequest(actRefno);

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items failed.", true);

                throw new Exception("Database exception occurred. Seeding finished ticket items failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket items failed.", true);

                throw new Exception("Seeding finished ticket items failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items.", true);

                string query = "DELETE FROM tActivityItemBarCodeAssociationXRef"
                    + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN"
                    + " (SELECT tActivityItem.ActivityItemRefno"
                    + " FROM tActivityItem"
                    + " WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityItemProductionFinished"
                    + " WHERE tActivityItemProductionFinished.ActivityItemRefno IN"
                    + " (SELECT tActivityItem.ActivityItemRefno"
                    + " FROM tActivityItem"
                    + " WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityItemProduction"
                    + " WHERE tActivityItemProduction.ActivityItemRefno IN"
                    + " (SELECT tActivityItem.ActivityItemRefno"
                    + " FROM tActivityItem"
                    + " WHERE tActivityItem.ActivityItemSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket items failed.", true);

                throw new Exception("Database exception occurred. Purging finished ticket items failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityItemRefno) FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} finished ticket items.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Finished ticket items status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actRefno)
        {
            List<tActivityProductionFinishedModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityProductionFinishedModel>("SELECT * FROM tActivityProductionFinished WHERE ActivityRefno = ?", Convert.ToInt32(actRefno));

            int actProductionFKRefno;
            if (results?.Count > 0)
            {
                actProductionFKRefno = results[0].ActivityFKRefno;
            }
            else
            {
                throw new Exception("drFinished.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(actProductionFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actRefno">The actRefno<see cref="int"/>.</param>
        protected override async Task Save(string reqData, int actRefno)
        {
            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actItemRefno = 0;
                            int progValue = 0;
                            int prodUnitRefno = 0;
                            int locMapRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
                            {
                                transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished), Convert.ToInt32(SysCommon.ActStatus.NoChange), Convert.ToInt32(actRefno));
                                actItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                                prodUnitRefno = this.GetProductUnitRefnoFromDataRow(drow, transaction, false);

                                string queryActivityItemProduction = "INSERT INTO tActivityItemProduction (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ProductUnitRefno, " +
                                    "LocationMapRefno, " +
                                    "ActivityItemProductionQuantity, " +
                                    "ActivityItemProductionBaseQuantity, " +
                                    "ActivityItemProductionWeightQuantity, " +
                                    "ActivityItemProductionDescription, " +
                                    "ActivityItemProductionQuantityExpected, " +
                                    "ActivityItemProductionBaseQuantityExpected, " +
                                    "ActivityItemProductionWeightQuantityExpected" +
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
                                object[] parametersActivityItemProduction = new object[]
                                {
                                    actItemRefno,
                                    Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
                                    prodUnitRefno,
                                    this.GetWhsLocMapRefnoFromDataRow(drow, transaction),
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]),
                                    drow["ActivityItemProductionDescription"].ToString(),
                                    Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]),
                                    Convert.ToDecimal(drow["ActivityItemProductionWeightQuantityExpected"]),
                                };

                                transaction.Execute(queryActivityItemProduction, parametersActivityItemProduction);

                                int actItemProductionFKRefno = Convert.ToInt32(drow["ActivityItemProductionFKRefno"]);
                                transaction.Execute("INSERT INTO tActivityItemProductionFinished (ActivityItemRefno, ActivityItemFKRefno, ActivityItemStringFKRefno) VALUES (?, ?, ?)", actItemRefno, actItemProductionFKRefno, Convert.ToString(actItemProductionFKRefno));

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket items failed.", false);

                    throw new Exception("Database exception occurred. Saving finished ticket items failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="actProductionFKRefno">The actProductionFKRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int actProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET FINISHED ITEMS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<refno>" + actProductionFKRefno.ToString() + "</refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
