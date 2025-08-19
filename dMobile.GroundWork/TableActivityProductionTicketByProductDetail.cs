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
    using Bedrock;
    using Resources.Models.Items;
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
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
                $"Purging {this.LowerCaseName} details.",
                true);

            await conn.RunInTransactionAsync(
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
                        DELETE FROM tActivityDetailProductionByProduct
                            WHERE tActivityDetailProductionByProduct.ActivityDetailRefno <= 0
                                OR tActivityDetailProductionByProduct.ActivityDetailFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityDetailProductionByProduct
                            WHERE tActivityDetailProductionByProduct.ActivityDetailRefno
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
                        DELETE FROM tActivityDetailProductionByProduct
                            WHERE tActivityDetailProductionByProduct.ActivityDetailRefno IN
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
                        throw new Exception(
                            $"Database exception occurred. Purging {this.LowerCaseName} details failed.",
                            sqlEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(
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
            try
            {
                const int actItemFKRefno = 0;

                const string query =
                    "SELECT * FROM tActivityItemProductionByProduct WHERE ActivityItemFKRefno = ?";
                var results = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<ActivityItemProductionByProductModel>(
                        query,
                        activityFkRefno);

                if (results != null &&
                    results.Count > 0)
                {
                    activityFkRefno = results[0].ActivityItemFKRefno;
                }
                else
                {
                    throw new Exception("No results found in tActivityItemProductionByProduct");
                }

                return await SendComm(await this.CreateRequest(actItemFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception(
                    "Database Exception occurred. ActivityItemProductionByProductModel Send. ",
                    ex);
            }
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
                                "Saving ticket add-product details.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityDetailProduction"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actDetailRefno = 0;
                                var progValue = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                var results = transaction.Query<ActivityItemProductionModel>(
                                    "SELECT * FROM tActivityItemProduction WHERE ActivityItemRefno = ?",
                                    refno);

                                if (results == null ||
                                    (results != null && results.Count == 0))
                                {
                                    throw new Exception(
                                        "No results found in tActivityItemProduction for ActivityItemRefno " +
                                        refno +
                                        Environment.NewLine +
                                        errMsg);
                                }

                                foreach (DataRow drow in
                                         ds.Tables["tActivityDetailProduction"].Rows)
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
                                    var parametersInsertDetail = new List<object>();
                                    parametersInsertDetail.Add(
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                                    parametersInsertDetail.Add(0);
                                    parametersInsertDetail.Add(refno);
                                    parametersInsertDetail.Add(
                                        GetReceivedDataLocation(drow, transaction, false));
                                    parametersInsertDetail.Add(results[0].ProductUnitRefno);
                                    parametersInsertDetail.AddRange(
                                        SaveDetail(drow, transaction, false));

                                    transaction.Execute(
                                        queryInsertDetail,
                                        parametersInsertDetail.ToArray());

                                    actDetailRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityDetail"));
                                    var actDetailProductionFkRefno =
                                        Convert.ToInt32(drow["ActivityDetailProductionFKRefno"]);
                                    errMsg += Environment.NewLine +
                                              "Detail fk refno " +
                                              actDetailProductionFkRefno;

                                    transaction.Execute(
                                        "INSERT INTO tActivityDetailProductionByProduct (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                                        actDetailRefno,
                                        actDetailProductionFkRefno);

                                    SaveCommonDetailVariant(
                                        drow,
                                        transaction,
                                        actDetailRefno,
                                        false);
                                    SaveDetailExtra(drow, transaction, actDetailRefno, false);

                                    progValue++;
                                    if (progValue % 100 == 0)
                                    {
                                        this.DataAccessProgressOutHandler(
                                            this,
                                            EventArgs.Empty,
                                            progValue,
                                            0,
                                            progMax);
                                    }
                                }

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    progMax,
                                    0,
                                    progMax);
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving ticket add-product details complete.",
                                false);
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Database exception occurred.",
                                false);
                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving ticket add-product details failed.",
                                false);

                            throw new Exception(
                                "Database exception occurred. Saving ticket add-product details failed." +
                                Environment.NewLine +
                                errMsg,
                                ex);
                        }
                    });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno">The activityFkRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                "<imclient>" +
                "<command>" +
                "<action>REQUEST PRODUCTION TICKET BYPRODUCT DETAILS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<itm_refno>" +
                activityFkRefno +
                "</itm_refno>" +
                "</command>" +
                "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}