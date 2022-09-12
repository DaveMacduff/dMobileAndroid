// <copyright file="TableActivityReturnsDetail.cs" company="InterSect Business Systems Inc">
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

    public class TableActivityReturnsDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReturnsDetail"/> class.
        /// </summary>
        public TableActivityReturnsDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsDetail";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                    if (results?.Count > 0)
                    {
                        foreach (var activityItemModel in results)
                        {
                            actItemRefno = Convert.ToInt32(activityItemModel.ActivityItemRefno);
                            reqData = await this.SendRequest(actItemRefno);

                            if (reqData.Length > 0)
                            {
                                if (reqData.Contains("<message>ERROR</message>"))
                                {
                                    var reqErr = new XmlDocument();
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
                            var reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        await this.Save(reqData, actItemRefno);
                    }
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);

                throw new Exception("Database exception occurred. Seeding sales return details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sales return details failed.", true);

                throw new Exception("Seeding sales return details failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details.", true);

                var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                            + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                            + "(SELECT tActivityDetail.ActivityDetailRefno "
                            + "FROM tActivityDetail "
                            + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityDetailReturns "
                        + "WHERE tActivityDetailReturns.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityDetailExtra "
                    + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                    + "(SELECT tActivityDetail.ActivityDetailRefno "
                    + "FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                query = "DELETE FROM tActivityDetail "
                    + "WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.Returns));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return details failed.", true);

                throw new Exception("Database exception occurred. Purging sales return details failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.Returns)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} sales return details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sales return details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemRetRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRetRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReturnsModel>("SELECT * FROM tActivityItemReturns WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemRetRefno));

            int actItemRetFKRefno;
            if (results?.Count > 0)
            {
                actItemRetFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
            }
            else
            {
                throw new Exception("drItemReturns.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(actItemRetFKRefno));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemReturnsRefno"></param>
        protected override async Task Save(string reqData, int actItemReturnsRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailReturns"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actDetailRefno = 0;
                            var detailRetFKRefno = 0;
                            var progValue = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            var results = transaction.Query<tActivityItemReturnsModel>("SELECT * FROM tActivityItemReturns WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemReturnsRefno));

                            if (results == null || results?.Count == 0)
                            {
                                throw new Exception("No results in tActivityItemReturns for ActivityItemRefno " + actItemReturnsRefno + " " + this.FullClassName);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailReturns"].Rows)
                            {
                                errMsg = "Arguments:";

                                var queryInsertDetail = "INSERT INTO tActivityDetail (" +
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
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?, " +
                                                        "?" +
                                                        ")";
                                var parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.Returns));
                                parametersInsertDetail.Add(0);
                                parametersInsertDetail.Add(Convert.ToInt32(actItemReturnsRefno));
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, false));
                                parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                detailRetFKRefno = Convert.ToInt32(drow["ActivityDetailFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + detailRetFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailReturns (ActivityDetailRefno, ActivityDetailFKRefno, ActivityDetailReturnsIsConfirmed) VALUES (?, ?, ?)",
                                    Convert.ToInt32(actDetailRefno),
                                    Convert.ToInt32(detailRetFKRefno),
                                    Convert.ToBoolean(drow["ActivityDetailReturnsIsConfirmed"]));

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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details failed.", false);

                    throw new Exception("Database exception occurred. Saving sales return details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override async Task<string> CreateRequest(int iActivityItemFKRefno)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES RETURN DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + iActivityItemFKRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
