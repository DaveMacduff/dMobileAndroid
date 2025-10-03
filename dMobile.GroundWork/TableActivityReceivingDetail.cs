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
    using Bedrock;
    using Resources.Models.Items;
    using SQLite;

    /// <summary>
    ///     Defines the <see cref="TableActivityReceivingDetail" />.
    /// </summary>
    public class TableActivityReceivingDetail : TableBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TableActivityReceivingDetail" /> class.
        /// </summary>
        public TableActivityReceivingDetail()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.Receiving;
            this.LowerCaseName = "receiving";
            this.ProperCaseName = "Receiving";
        }

        /// <summary>
        ///     Purge the activity detail.
        /// </summary>
        /// <returns><see cref="Task" /> Task completed.</returns>
        public override async Task PurgeData()
        {
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} details.",
                true);

            await Conn.RunInTransactionAsync(
                transaction =>
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

                        query =
                            "DELETE FROM tActivityDetailExtra WHERE tActivityDetailExtra.ActivityDetailRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityDetailExtra
                            WHERE tActivityDetailExtra.ActivityDetailRefno
                                NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityDetailReceiving
                            WHERE tActivityDetailReceiving.ActivityDetailRefno <= 0
                                OR tActivityDetailReceiving.ActivityDetailFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityDetailReceiving
                            WHERE tActivityDetailReceiving.ActivityDetailRefno
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
                        DELETE FROM tActivityDetailReceiving
                            WHERE tActivityDetailReceiving.ActivityDetailRefno IN
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

                        query =
                            "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                        transaction.Execute(query, subType);

                        transaction.Commit();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} details complete.",
                            true);
                    }
                    catch (SQLiteException sqlEx)
                    {
                        transaction.Rollback();
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            "Database exception occurred.",
                            true);
                        this.DataAccessStatusOutHandler(
                            this,
                            EventArgs.Empty,
                            $"Purging {this.LowerCaseName} details failed.",
                            true);
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            sqlEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new BaseException(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            ex);
                    }
                });
        }

        /// <summary>
        /// Sends a request based on the specified activity foreign key reference number.
        /// </summary>
        /// <remarks>This method retrieves data from the database using the provided activity foreign key
        /// reference number. If a matching record is found, it creates and sends a communication request based on the
        /// retrieved data.</remarks>
        /// <param name="activityFkRefno">The foreign key reference number of the activity item. Must be a valid reference number.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string from the
        /// communication request.</returns>
        /// <exception cref="Exception">Thrown if no records are found for the specified <paramref name="activityFkRefno"/> in the database.</exception>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            const string query = "SELECT * FROM tActivityItemReceiving WHERE ActivityItemFKRefno = ?";
            var results = await Conn.QueryAsync<ActivityItemReceivingModel>(
                query,
                activityFkRefno);
            if (results.Count <= 0)
            {
                throw new BaseException($"Unable to find {activityFkRefno} in tActivityItemReceiving.");
            }

            var request = await this.CreateRequest(results[0].ActivityItemFKRefno);
            return await SendComm(request);
        }

        /// <inheritdoc cref="TableBase.Save(string, int)"/>
        protected override async Task Save(string responseData, int refno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance()
                .SqlConn.RunInTransactionAsync(
                    transaction =>
                    {
                        try
                        {
                            using var ds = new DataSet();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Reading received data.",
                                false);
                            ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving details.",
                                false);
                            if (ds.Tables.Count != 0)
                            {
                                var maxRowCount = Convert.ToInt32(
                                    ds.Tables["tActivityDetailPurchaseOrder"].Rows.Count);
                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {maxRowCount}",
                                    false);
                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    maxRowCount);
                                var results = transaction.Query<ActivityItemReceivingModel>(
                                    "SELECT * FROM tActivityItemReceiving WHERE ActivityItemRefno = ?",
                                    refno);
                                if (results == null ||
                                    results.Count == 0)
                                {
                                    errMsg =
                                        "No results in tActivityItemReceiving for ActivityItemRefno " +
                                        refno.ToString();
                                    throw new BaseException(errMsg);
                                }

                                var currentRow = 0;
                                foreach (DataRow row in ds.Tables["tActivityDetailPurchaseOrder"]
                                             .Rows)
                                {
                                    errMsg = "Arguments:";
                                    const string queryInsertDetail = "INSERT INTO tActivityDetail " +
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
                                        refno,
                                        GetReceivedDataLocation(row, transaction, false),
                                        results[0].ProductUnitRefno
                                    };

                                    parametersInsertDetail.AddRange(
                                        SaveDetail(row, transaction, false));

                                    var rowsAffected = transaction.Execute(
                                        queryInsertDetail,
                                        parametersInsertDetail.ToArray());
                                    if (rowsAffected <= 0)
                                    {
                                        errMsg =
                                            "Insert into ActivityDetail failed. Rows affected <= 0";
                                        throw new BaseException(errMsg);
                                    }

                                    var actDetailRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityDetail"));
                                    if (actDetailRefno <= 0)
                                    {
                                        errMsg =
                                            "Failed to find row id after activity detail insert.";
                                        throw new BaseException(errMsg);
                                    }

                                    var detailFkRefno =
                                        Convert.ToInt32(row["ActivityDetailFKRefno"]);

                                    errMsg += Environment.NewLine +
                                              "Detail fk refno " +
                                              detailFkRefno;

                                    rowsAffected = transaction.Execute(
                                        "INSERT INTO tActivityDetailReceiving (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                                        actDetailRefno,
                                        detailFkRefno);
                                    if (rowsAffected <= 0)
                                    {
                                        errMsg =
                                            "Insert into ActivityDetailReceiving failed. Rows affected <= 0";
                                        throw new BaseException(errMsg);
                                    }

                                    SaveCommonDetailVariant(
                                        row,
                                        transaction,
                                        actDetailRefno,
                                        false);
                                    SaveDetailExtra(row, transaction, actDetailRefno, false);

                                    currentRow++;
                                    if (currentRow % 10 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            currentRow,
                                            0,
                                            maxRowCount);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    maxRowCount,
                                    0,
                                    maxRowCount);
                            }

                            transaction.Commit();
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving details complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving details failed.",
                                true);

                            throw new BaseException(
                                "Database exception occurred. Saving receiving details failed." +
                                Environment.NewLine +
                                errMsg,
                                ex);
                        }
                        catch (Exception)
                        {
                            if (transaction.IsInTransaction)
                            {
                                transaction.Rollback();
                            }

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Exception occurred.",
                                true);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving receiving details failed.",
                                true);
                            throw;
                        }
                    });
        }

        /// <summary>
        ///     Create request activity detail string.
        /// </summary>
        /// <param name="activityFkRefno"><see cref="int" /> Activity item adjustment foreign key reference number.</param>
        /// <returns><see cref="string" /> Request xml string.</returns>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var request = new StringBuilder();
            using var writer = XmlWriter.Create(request, new XmlWriterSettings { Async = true });
            try
            {
                var xmlRequest = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST PURCHASE ORDER DETAILS</action>" +
                    await SysCommon.GetRequestDefaults(false) +
                    "<itm_refno>" +
                    activityFkRefno +
                    "</itm_refno>" +
                    "</command>" +
                    "</imclient>");

                xmlRequest.Save(writer);
                await writer.FlushAsync();
            }
            finally
            {
                writer.Close();
            }

            const string errMsg =
                "The created request activity detail xml string length has a zero length.";
            request = request.Length <= 0 ? throw new BaseException(errMsg) : request;

            return request.ToString();
        }
    }
}