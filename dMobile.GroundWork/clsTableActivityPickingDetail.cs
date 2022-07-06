// <copyright file="clsTableActivityPickingDetail.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityPickingDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingDetail"/> class.
        /// </summary>
        public TableActivityPickingDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingDetail";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    List<tActivityItemModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT * FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Picking));

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding picking details failed.", true);

                throw new Exception("Database exception occurred. Seeding picking details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales order details failed.", true);

                throw new Exception("Seeding sales order details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                    + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityDetailPicking "
                        + "WHERE tActivityDetailPicking.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityDetailExtra "
                    + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Picking));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order details failed.", true);

                throw new Exception("Database exception occurred. Purging sales order details failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} Sales order details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sales order details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemPickRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemPickRefno)
        {
            List<tActivityItemPickingModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemPickingModel>("SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = ?", actItemPickRefno);

            int actItemPickFKRefno;
            if (results?.Count > 0)
            {
                actItemPickFKRefno = Convert.ToInt32(results[0].ActivityItemPickingFKRefno);
            }
            else
            {
                throw new Exception("drItemPick.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(actItemPickFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemPickRefno"></param>
        protected override async Task Save(string reqData, int actItemPickRefno)
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tActivityDetailPicking"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            int actDetailRefno = 0;
                            int progValue = 0;
                            int actDetailPickFKRefno = 0;
                            int productUnitRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            List<tActivityItemPickingModel> resultPickingItems = transaction.Query<tActivityItemPickingModel>("SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = ?", actItemPickRefno);

                            if (resultPickingItems == null || (resultPickingItems != null && resultPickingItems.Count == 0))
                            {
                                throw new Exception("No results in tActivityItemPicking for ActivityItemRefno " + actItemPickRefno);
                            }

                            productUnitRefno = resultPickingItems[0].ProductUnitRefno;

                            foreach (DataRow drow in ds.Tables["tActivityDetailPicking"].Rows)
                            {
                                bool useFNo = false;
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
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.Picking));
                                parametersInsertDetail.Add(0);
                                parametersInsertDetail.Add(actItemPickRefno);
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, useFNo));
                                parametersInsertDetail.Add(productUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, useFNo));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailPickFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailPickFKRefno;

                                string queryInsertPickingDetail = "INSERT INTO tActivityDetailPicking (" +
                                    "ActivityDetailRefno, " +
                                    "ActivityDetailFKRefno, " +
                                    "ActivityDetailPickingIsConfirmed" +
                                    ") VALUES (" +
                                    "?, " +
                                    "?, " +
                                    "?" +
                                    ")";

                                transaction.Execute(queryInsertPickingDetail, actDetailRefno, actDetailPickFKRefno, Convert.ToBoolean(drow["ActivityDetailPickingIsConfirmed"]));

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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details completed.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details failed.", false);

                    throw new Exception("Database exception occurred. Saving sales order details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemPickFKRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int actItemPickFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + actItemPickFKRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
