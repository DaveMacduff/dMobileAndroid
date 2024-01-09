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
            this.CurrentSubType = SysCommon.ActSubTypes.Returns;
            this.LowerCaseName = "sales order return";
            this.ProperCaseName = "Sales order return";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging return details.", true);

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
                    query = "DELETE FROM tActivityDetailReturns WHERE tActivityDetailReturns.ActivityDetailRefno <= 0" +
                            " OR tActivityDetailReturns.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailReturns WHERE tActivityDetailReturns.ActivityDetailRefno " +
                            "NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
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

                    query = "DELETE FROM tActivityDetailReturns "
                        + "WHERE tActivityDetailReturns.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetailExtra "
                        + "WHERE tActivityDetailExtra.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                    transaction.Execute(query, subType);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details complete.", true);
                    transaction.Commit();
                }
                catch (SQLiteException sqlEx)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging return details failed.", true);
                    throw new Exception("Database exception occurred. Purging return details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw new Exception("Database exception occurred. Purging receiving details failed.", ex);
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
            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReturnsModel>("SELECT * FROM tActivityItemReturns WHERE ActivityItemRefno = ?", Convert.ToInt32(actItemRefno));

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
        /// <param name="responseData"></param>
        /// <param name="actItemReturnsRefno"></param>
        /// <inheritdoc/>
        protected override async Task Save(string responseData, int actItemReturnsRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sales return details.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["tActivityDetailReturns"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

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
                + "<action>REQUEST SALES RETURN DETAILS</action>"
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
