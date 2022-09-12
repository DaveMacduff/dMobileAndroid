// <copyright file="TableActivityProductionTicketMaterialDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketMaterialDetail" />.
    /// </summary>
    public class TableActivityProductionTicketMaterialDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketMaterialDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketMaterialDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialDetail";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="actItemRefno"></param>
        public override async Task RequestData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material details.", true);

                string reqData;
                if (actItemRefno == 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemModel>("SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

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

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials details failed.", true);

                throw new Exception("Database exception occurred. Seeding ticket materials details failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket materials details failed.", true);

                throw new Exception("Seeding ticket materials details failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(ActivityDetailRefno) FROM tActivityDetail WHERE ActivityDetailSubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial)));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} ticket material details.", rowsAffected), true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Material ticket details status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details.", true);

            try
            {
                var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef"
                            + " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN"
                            + " (SELECT tActivityDetail.ActivityDetailRefno"
                            + " FROM tActivityDetail"
                            + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetailProductionMaterial"
                    + " WHERE tActivityDetailProductionMaterial.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetailExtra"
                    + " WHERE tActivityDetailExtra.ActivityDetailRefno IN"
                    + " (SELECT tActivityDetail.ActivityDetailRefno"
                    + " FROM tActivityDetail"
                    + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material details failed.", true);

                throw new Exception("Database exception occurred. Purging ticket material details failed.", ex);
            }
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionMaterialModel>("SELECT * FROM tActivityItemProductionMaterial WHERE ActivityItemRefno = ?", actItemRefno);

            int actItemMaterialFKRefno;
            if (results?.Count > 0)
            {
                actItemMaterialFKRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new Exception("drActItemMaterial.Read() failed.");
            }

            return await this.SendComm(await this.CreateRequest(actItemMaterialFKRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="actItemProductionRefno"></param>
        protected override async Task Save(string reqData, int actItemProductionRefno)
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);

                            var actDetailRefno = 0;
                            var progValue = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            var results = transaction.Query<tActivityItemProductionModel>("SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?", actItemProductionRefno);

                            if (results == null || results?.Count == 0)
                            {
                                throw new Exception("No results in tActivityItemProduction for ActivityItemRefno " + actItemProductionRefno + Environment.NewLine + errMsg);
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailProduction"].Rows)
                            {
                                errMsg = "Arguments:";

                                var queryInsertDetail = "INSERT INTO tActivityDetail " +
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
                                var parametersInsertDetail = new List<object>();
                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketMaterial));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemProductionRefno);
                                parametersInsertDetail.Add(Convert.ToInt32(this.GetDetailLocation(drow, transaction, false)));
                                parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                var actDetailProductionFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailProductionFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailProductionMaterial (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)", actDetailRefno, actDetailProductionFKRefno);

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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details complete.", false);
                    }
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material details failed.", false);

                    throw new Exception("Database exception occurred. Saving ticket material details failed." + Environment.NewLine + errMsg, ex);
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
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PRODUCTION TICKET MATERIAL DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + actItemProductionFKRefno + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
