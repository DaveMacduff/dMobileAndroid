// <copyright file="clsTableActivityReceivingDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityReceivingDetail" />.
    /// </summary>
    public class TableActivityReceivingDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceivingDetail"/> class.
        /// </summary>
        public TableActivityReceivingDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingDetail";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                    if (results?.Count > 0)
                    {
                        foreach (tActivityItemModel activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItemModel.ActivityItemRefno);
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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);

                throw new Exception("Database exception occurred. Seeding receiving details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving details failed.", true);

                throw new Exception("Seeding receiving details failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                    + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivityDetailReceiving "
                    + "WHERE tActivityDetailReceiving.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivityDetailExtra "
                    + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Receiving));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details failed.", true);

                throw new Exception("Database exception occurred. Purging receiving details failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Receiving)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} receiving details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Receiving details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRecvRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRecvRefno)
        {
            List<tActivityItemReceivingModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReceivingModel>("SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = ?", actItemRecvRefno);
            int actItemRecvFKRefno;
            if (results?.Count > 0)
            {
                actItemRecvFKRefno = results[0].ActivityItemReceivingFKRefno;
            }
            else
            {
                throw new Exception("drItemRecv.Read() failed.");
            }

            string reqData = await this.SendComm(await this.CreateRequest(actItemRecvFKRefno));

            return reqData;
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemRecvRefno"></param>
        protected override async Task Save(string reqData, int actItemRecvRefno)
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPurchaseOrder"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int productUnitRefno = 0;
                            int progValue = 0;
                            int recvDetailFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemReceivingModel> results = transaction.Query<tActivityItemReceivingModel>("SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = ?", actItemRecvRefno);

                            if (results == null || results?.Count == 0)
                            {
                                throw new Exception("No results in tActivityItemReceiving for ActivityItemRefno " + actItemRecvRefno + " " + this.FullClassName);
                            }

                            productUnitRefno = results[0].ProductUnitRefno;

                            foreach (DataRow drow in ds.Tables["tActivityDetailPurchaseOrder"].Rows)
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
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.Receiving));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemRecvRefno);
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(productUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                recvDetailFKRefno = Convert.ToInt32(drow["ActivityDetailPurchaseOrderFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + recvDetailFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailReceiving (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)", actDetailRefno, recvDetailFKRefno);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, false);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, false);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }

                        transaction.Commit();

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details failed.", true);

                    throw new Exception("Database exception occurred. Saving receiving details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="iActivityItemReceivingFKRefno">The iActivityItemReceivingFKRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int iActivityItemReceivingFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PURCHASE ORDER DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + iActivityItemReceivingFKRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
