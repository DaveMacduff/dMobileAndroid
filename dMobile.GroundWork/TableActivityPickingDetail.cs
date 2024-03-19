// <copyright file="TableActivityPickingDetail.cs" company="InterSect Business Systems Inc">
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
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales order";
            this.ProperCaseName = "Sales order";
        }

        /// <summary>
        /// Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = @"
                        DELETE FROM tActivityDetailProductSchemeVariantXRef
                            WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno <= 0
                                OR tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetailProductSchemeVariantXRef
                            WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno
                                NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetailExtra
                            WHERE tActivityDetailExtra.ActivityDetailRefno
                                NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetailPicking
                            WHERE tActivityDetailPicking.ActivityDetailRefno <= 0
                                OR tActivityDetailPicking.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetailPicking
                            WHERE tActivityDetailPicking.ActivityDetailRefno
                                NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetail
                            WHERE tActivityDetail.ActivityDetailSubType = ?
                                AND tActivityDetail.ActivityDetailRefno <= 0";
                    transaction.Execute(query, subType);

                    query = @"
                        DELETE FROM tActivityDetail
                            WHERE tActivityDetail.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                    transaction.Execute(query, subType);

                    // Regular purge.
                    query = @"
                        DELETE FROM tActivityDetailProductSchemeVariantXRef
                            WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno
                                    FROM tActivityDetail
                                    WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = @"
                        DELETE FROM tActivityDetailPicking
                            WHERE tActivityDetailPicking.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno
                                    FROM tActivityDetail
                                    WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = @"
                        DELETE FROM tActivityDetailExtra
                            WHERE tActivityDetailExtra.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno
                                    FROM tActivityDetail
                                    WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details failed.", true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} details failed.", ex);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tActivityItemPickingModel>("SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = ?", actItemRefno);

            int actItemPickFKRefno;
            if (results?.Count > 0)
            {
                actItemPickFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
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
        /// <param name="responseData"></param>
        /// <param name="actItemPickRefno"></param>
        /// <inheritdoc/>
        protected override async Task Save(string responseData, int actItemPickRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales order details.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityDetailPicking"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actDetailRefno = 0;
                        var progValue = 0;
                        var actDetailPickFKRefno = 0;
                        var productUnitRefno = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        var resultPickingItems = transaction.Query<tActivityItemPickingModel>("SELECT * FROM tActivityItemPicking WHERE ActivityItemRefno = ?", actItemPickRefno);

                        if (resultPickingItems == null || (resultPickingItems != null && resultPickingItems.Count == 0))
                        {
                            throw new Exception("No results in tActivityItemPicking for ActivityItemRefno " + actItemPickRefno);
                        }

                        productUnitRefno = resultPickingItems[0].ProductUnitRefno;

                        foreach (DataRow drow in ds.Tables["tActivityDetailPicking"].Rows)
                        {
                            var useFNo = false;
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

                            var queryInsertPickingDetail = "INSERT INTO tActivityDetailPicking (" +
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
        /// <param name="activityItemFkRefno"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + activityItemFkRefno.ToString() + "</itm_refno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
