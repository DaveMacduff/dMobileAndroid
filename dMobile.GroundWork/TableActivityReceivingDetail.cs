// <copyright file="TableActivityReceivingDetail.cs" company="InterSect Business Systems Inc">
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
            this.CurrentSubType = SysCommon.ActSubTypes.Receiving;
            this.LowerCaseName = "receiving";
            this.ProperCaseName = "Receiving";
        }

        /// <summary>
        /// Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging details.", true);

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
                    query = "DELETE FROM tActivityDetailReceiving WHERE tActivityDetailReceiving.ActivityDetailRefno <= 0" +
                            " OR tActivityDetailReceiving.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    // Corruption recovery cleanup.
                    query = "DELETE FROM tActivityDetailReceiving WHERE tActivityDetailReceiving.ActivityDetailRefno " +
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

                    query = "DELETE FROM tActivityDetailReceiving "
                        + "WHERE tActivityDetailReceiving.ActivityDetailRefno IN "
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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving details failed.", true);
                    throw new Exception("Database exception occurred. Purging receiving details failed.", sqlEx);
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
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"> Activity item reference number.</param>
        /// <returns><see cref="string"/> Send request data result.</returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var query = "SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = ?";
            var results = await conn.QueryAsync<tActivityItemReceivingModel>(
                query,
                actItemRefno);
            if (results.Count <= 0)
            {
                throw new Exception($"Unable to find {actItemRefno} in tActivityItemReceiving.");
            }

            var request = await this.CreateRequest(results[0].ActivityItemFKRefno);
            return await this.SendComm(request);
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Response data received.</param>
        /// <param name="actItemRefno"><see cref="int"/> Activity item reference number.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        protected override async Task Save(string responseData, int actItemRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details.", false);
                    if (ds.Tables.Count != 0)
                    {
                        var maxRowCount = Convert.ToInt32(ds.Tables["tActivityDetailPurchaseOrder"].Rows.Count);
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {maxRowCount}", false);
                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, maxRowCount);
                        var results = transaction.Query<tActivityItemReceivingModel>(
                            "SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = ?",
                            actItemRefno);
                        if (results == null || results?.Count == 0)
                        {
                            errMsg = "No results in tActivityItemReceiving for ActivityItemRefno " + actItemRefno +
                                " " + this.FullClassName;
                            throw new Exception(errMsg);
                        }

                        var currentRow = 0;
                        foreach (DataRow row in ds.Tables["tActivityDetailPurchaseOrder"].Rows)
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

                            var parametersInsertDetail = new List<object>
                            {
                                Convert.ToInt32(SysCommon.ActSubTypes.Receiving),
                                Convert.ToInt32(0),
                                actItemRefno,
                                this.GetDetailLocation(row, transaction, false),
                                results[0].ProductUnitRefno,
                            };

                            parametersInsertDetail.AddRange(this.SaveDetail(row, transaction, false));

                            var rowsAffected = transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());
                            if (rowsAffected <= 0)
                            {
                                errMsg = "Insert into ActivityDetail failed. Rows affected <= 0";
                                throw new Exception(errMsg);
                            }

                            var actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                                "SELECT last_insert_rowid() FROM tActivityDetail"));
                            if (actDetailRefno <= 0)
                            {
                                errMsg = "Failed to find row id after activity detail insert.";
                                throw new Exception(errMsg);
                            }

                            var detailFkRefno = Convert.ToInt32(row["ActivityDetailFKRefno"]);

                            errMsg += Environment.NewLine + "Detail fk refno " + detailFkRefno;

                            rowsAffected = transaction.Execute(
                                "INSERT INTO tActivityDetailReceiving (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                                actDetailRefno,
                                detailFkRefno);
                            if (rowsAffected <= 0)
                            {
                                errMsg = "Insert into ActivityDetailReceiving failed. Rows affected <= 0";
                                throw new Exception(errMsg);
                            }

                            this.SaveCommonDetailVariant(row, transaction, actDetailRefno, false);
                            this.SaveDetailExtra(row, transaction, actDetailRefno, false);

                            currentRow++;
                            if (currentRow % 10 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, currentRow, 0, maxRowCount);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, maxRowCount, 0, maxRowCount);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details failed.", true);

                    throw new Exception(
                        "Database exception occurred. Saving receiving details failed."
                        + Environment.NewLine
                        + errMsg, ex);
                }
                catch (Exception)
                {
                    if (transaction.IsInTransaction)
                    {
                        transaction.Rollback();
                    }

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving details failed.", true);
                    throw;
                }
            });
        }

        /// <summary>
        /// Create request activity detail string.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item adjustment foreign key reference number.</param>
        /// <returns><see cref="string"/> Request xml string.</returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var request = new StringBuilder();
            using var writer = XmlWriter.Create(request, new XmlWriterSettings() { Async = true });
            try
            {
                var xmlRequest = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST PURCHASE ORDER DETAILS</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<itm_refno>" + activityItemFkRefno.ToString() + "</itm_refno>"
                    + "</command>"
                    + "</imclient>");

                xmlRequest.Save(writer);
                await writer.FlushAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.Close();
            }

            const string errMsg = "The created request activity detail xml string length has a zero length.";
            request = (request.Length <= 0) ? throw new Exception(errMsg) : request;

            return request.ToString();
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
