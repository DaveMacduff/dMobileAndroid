// <copyright file="clsTableActivityProductionTicketFinishedDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketFinishedDetail" />.
    /// </summary>
    public class TableActivityProductionTicketFinishedDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketFinishedDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketFinishedDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketFinishedDetail";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityItemModel activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                            reqData = await this.SendRequest(actItemRefno);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    XmlDocument reqErr = new XmlDocument();
                                    reqErr.LoadXml(reqData);
                                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                                }

                                await this.Save(reqData, actItemRefno);
                            }
                        }
                    }
                }
                else
                {
                    reqData = await this.SendRequest(actItemRefno);

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actItemRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);

                throw new Exception("Database exception occurred. Seeding finished ticket details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding finished ticket details failed.", true);

                throw new Exception("Seeding finished ticket details failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                    + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityDetailProductionFinished"
                    + " WHERE tActivityDetailProductionFinished.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityDetailExtra"
                    + " WHERE tActivityDetailExtra.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                query = "DELETE FROM tActivityDetail WHERE ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging finished ticket details failed.", true);

                throw new Exception("Database exception occurred. Purging finished ticket details failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} finished ticket details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Finished ticket details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            List<tActivityItemProductionFinishedModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionFinishedModel>("SELECT * FROM tActivityItemProductionFinished WHERE ActivityItemRefno = ?", actItemRefno);

            int actItemProductionFKRefno;
            if (results?.Count > 0)
            {
                actItemProductionFKRefno = results[0].ActivityItemProductionFinishedFKRefno;
            }
            else
            {
                throw new Exception("drItemFinished.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(actItemProductionFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemProductionRefno"></param>s
        protected override async Task Save(string reqData, int actItemProductionRefno)
        {
            string errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemProductionModel> results = transaction.Query<tActivityItemProductionModel>("SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?", actItemProductionRefno);

                            if (results == null || (results != null && results.Count == 0))
                            {
                                throw new Exception("No results for ActivityItemRefno ." + actItemProductionRefno + " in tActivityItemProduction." + Environment.NewLine + errMsg);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                string queryInsertDetail = "INSERT INTO tActivityDetail " +
                                "(" +
                                "ActivityDetailSubType, " +
                                "ActivityDetailStatus, " +
                                "ActivityItemRefno, " +
                                "LocationMapRefno, " +
                                "ProductUnitRefno, " +
                                "ActivityDetailUniqueSchemeID, " +
                                "ActivityDetailSchemeID, " +
                                "ActivityDetailQuantity, " +
                                "ActivityDetailBaseQuantity, " +
                                "ActivityDetailWeightQuantity, " +
                                "ActivityDetailSingleTradeItemSerialNumber, " +
                                "ActivityDetailLogisticUnitBatchLotNumber, " +
                                "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                                "ActivityDetailLogisticUnitExpiryUseByDate, " +
                                "ActivityDetailLogisticUnitProductionDate, " +
                                "ActivityDetailLogisticUnitDueDate, " +
                                "ActivityDetailLogisticUnitPackagingDate, " +
                                "ActivityDetailLogisticUnitSkidNumber" +
                                ") " +
                                "VALUES" +
                                "(" +
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
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                                List<object> parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemProductionRefno);
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);

                                errMsg = "FK detail refno " + actDetailFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailProductionFinished (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)", actDetailRefno, actDetailFKRefno);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, false);

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving finished ticket details failed.", false);

                    throw new Exception("Database exception occurred. Saving finished ticket details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="actItemProductionFKRefno">The actItemProductionFKRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int actItemProductionFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET FINISHED DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + actItemProductionFKRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
