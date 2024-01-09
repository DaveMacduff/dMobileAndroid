// <copyright file="TableActivityProductionTicketByProductDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityProductionTicketByProductDetail" />.
    /// </summary>
    public class TableActivityProductionTicketByProductDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityProductionTicketByProductDetail"/> class.
        /// </summary>
        public TableActivityProductionTicketByProductDetail()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketByProductDetail";
            this.CurrentSubType = SysCommon.ActSubTypes.TicketAddProduct;
            this.LowerCaseName = "ticket additional product";
            this.ProperCaseName = "Ticket additional product";
        }

        /// <summary>
        /// Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef" +
                                " WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno <= 0" +
                                " OR tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailProductionByProduct WHERE tActivityDetailProductionByProduct.ActivityDetailRefno " +
                            "NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailProductionByProduct WHERE tActivityDetailProductionByProduct.ActivityDetailRefno <= 0" +
                            " OR tActivityDetailProductionByProduct.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?" +
                            " AND tActivityDetail.ActivityDetailRefno <= 0";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailProductionByProduct"
                        + " WHERE tActivityDetailProductionByProduct.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno"
                        + " FROM tActivityDetail"
                        + " WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                    transaction.Execute(query, subType);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details complete.", true);
                    transaction.Commit();
                }
                catch (SQLiteException sqlEx)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket add-product details failed.", true);
                    throw new Exception("Database exception occurred. Purging ticket add-product details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw new Exception("Database exception occurred. Purging ticket add-product details failed.", ex);
                }
            });
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            try
            {
                var actItemFKRefno = 0;

                var query = "SELECT * FROM tActivityItemProductionByProduct WHERE ActivityItemRefno = ?";
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemProductionByProductModel>(query, actItemRefno);

                if (results != null && results.Count > 0)
                {
                    actItemRefno = results[0].ActivityItemFKRefno;
                }
                else
                {
                    throw new Exception("No results found in tActivityItemProductionByProduct");
                }

                return await this.SendComm(await this.CreateRequest(actItemFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database Exception occurred. tActivityItemProductionByProductModel Send. ", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actItemProductionRefno"></param>
        protected override async Task Save(string responseData, int actItemProductionRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityDetailProduction"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var actDetailRefno = 0;
                        var progValue = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        var results = transaction.Query<tActivityItemProductionModel>("SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?", actItemProductionRefno);

                        if (results == null || (results != null && results.Count == 0))
                        {
                            throw new Exception("No results found in tActivityItemProduction for ActivityItemRefno " + actItemProductionRefno + Environment.NewLine + errMsg);
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
                            parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                            parametersInsertDetail.Add(0);
                            parametersInsertDetail.Add(actItemProductionRefno);
                            parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, false));
                            parametersInsertDetail.Add(results[0].ProductUnitRefno);
                            parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, false));

                            transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                            actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                            var actDetailProductionFKRefno = Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                            errMsg += Environment.NewLine + "Detail fk refno " + actDetailProductionFKRefno;

                            transaction.Execute("INSERT INTO tActivityDetailProductionByProduct (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)", actDetailRefno, actDetailProductionFKRefno);

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket add-product details failed.", false);

                    throw new Exception("Database exception occurred. Saving ticket add-product details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno">The activityItemFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
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
                + "<action>REQUEST PRODUCTION TICKET BYPRODUCT DETAILS</action>"
                + await SysCommon.GetRequestDefaults(false)
                + "<itm_refno>" + activityItemFkRefno + "</itm_refno>"
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
