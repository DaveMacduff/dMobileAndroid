// <copyright file="TableActivityCycleCountDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCycleCountDetail" />.
    /// </summary>
    public class TableActivityCycleCountDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCycleCountDetail"/> class.
        /// </summary>
        public TableActivityCycleCountDetail()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityCycleCountDetail";
            this.CurrentSubType = SysCommon.ActSubTypes.CycleCounts;
            this.LowerCaseName = "cycle count";
            this.ProperCaseName = "Cycle count";
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

                        query = @"DELETE FROM tActivityDetailCount
                                WHERE tActivityDetailCount.ActivityDetailRefno <= 0
                                    OR tActivityDetailCount.ActivityDetailFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"DELETE FROM tActivityDetailCount
                                WHERE tActivityDetailCount.ActivityDetailRefno
                                NOT IN (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail)";
                        transaction.Execute(query);

                        query = @"DELETE FROM tActivityDetail
                                WHERE tActivityDetail.ActivityDetailSubType = ?
                                    AND tActivityDetail.ActivityDetailRefno <= 0";
                        transaction.Execute(query, subType);

                        query = @"DELETE FROM tActivityDetail
                                WHERE tActivityDetail.ActivityItemRefno
                                    NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                        transaction.Execute(query, subType);

                        // Regular purge.
                        query = @"
                        DELETE FROM tActivityDetailProductSchemeVariantXRef
                            WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN
                            (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail
                                WHERE tActivityDetail.ActivityDetailSubType = ?)";
                        transaction.Execute(query, subType);

                        query = @"
                        DELETE FROM tActivityDetailCount
                            WHERE tActivityDetailCount.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail
                                    WHERE tActivityDetail.ActivityDetailSubType = ?)";
                        transaction.Execute(query, subType);

                        query = @"
                        DELETE FROM tActivityDetailExtra
                            WHERE tActivityDetailExtra.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail
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
                const string query = "SELECT * FROM tActivityItemCount WHERE ActivityItemFKRefno = ?";
                var results = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<ActivityItemCountModel>(query, activityFkRefno);

                int actItemCountFkRefno;
                if (results != null &&
                    results.Count > 0)
                {
                    actItemCountFkRefno = results[0].ActivityItemFKRefno;
                }
                else
                {
                    throw new Exception(
                        "No results in tActivityItemCount for ActivityItemRefno " +
                        activityFkRefno);
                }

                return await SendComm(await this.CreateRequest(actItemCountFkRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception(
                    "Database exception occurred. Send cycle count details failed. ",
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
                            using (var ds = new DataSet())
                            {
                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Reading received data.",
                                    false);

                                ds.ReadXml(
                                    SysCommon.GetXmlReader(ref responseData),
                                    XmlReadMode.Auto);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    "Saving cycle count details.",
                                    false);

                                // Receiving schema but no data results in no table being created in the dataset.
                                if (ds.Tables.Count != 0)
                                {
                                    var progMax = Convert.ToInt32(
                                        ds.Tables["tActivityDetailCount"].Rows.Count);

                                    this.DataAccessStatusOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        $"Records {progMax}.",
                                        false);

                                    var progValue = 0;

                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        0,
                                        0,
                                        progMax);

                                    var countItems = transaction.Query<ActivityItemCountModel>(
                                        "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?",
                                        refno);

                                    if (countItems == null ||
                                        (countItems != null && countItems.Count == 0))
                                    {
                                        throw new Exception("drItemCount.Read() failed.");
                                    }

                                    foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
                                    {
                                        errMsg = "Arguments:";
                                        var detailParameterList = new List<object>();
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

                                        detailParameterList.Add(
                                            Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                                        detailParameterList.Add(
                                            Convert.ToInt32(SysCommon.ActStatus.NoChange));
                                        detailParameterList.Add(refno);
                                        detailParameterList.Add(
                                            GetReceivedDataLocation(drow, transaction, true));
                                        detailParameterList.Add(countItems[0].ProductUnitRefno);
                                        detailParameterList.AddRange(
                                            SaveDetail(drow, transaction, true));

                                        transaction.Execute(
                                            queryInsertDetail,
                                            detailParameterList.ToArray());

                                        var actDetailRefno = Convert.ToInt32(
                                            transaction.ExecuteScalar<int>(
                                                "SELECT last_insert_rowid() FROM tActivityDetail"));
                                        var actDetailCycleFkRefno = Convert.ToInt32(drow["f01"]);

                                        errMsg += Environment.NewLine +
                                                  "Detail fk refno" +
                                                  actDetailCycleFkRefno;

                                        transaction.Execute(
                                            "INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                                            actDetailRefno,
                                            actDetailCycleFkRefno);

                                        SaveCommonDetailVariant(
                                            drow,
                                            transaction,
                                            actDetailRefno,
                                            true);
                                        SaveDetailExtra(
                                            drow,
                                            transaction,
                                            actDetailRefno,
                                            true);

                                        progValue++;
                                        if (progValue % 10 == 0)
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
                            }

                            transaction.Commit();

                            this.DataAccessStatusOutHandler(
                                this,
                                EventArgs.Empty,
                                "Saving cycle count details complete.",
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
                                "Saving cycle count details failed.",
                                true);

                            throw new Exception(
                                "Database exception occurred. Saving cycle count details failed." +
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
                "<action>REQUEST CYCLE COUNT DETAILS</action>" +
                await SysCommon.GetRequestDefaults(false) +
                "<itm_refno>" +
                activityFkRefno.ToString() +
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