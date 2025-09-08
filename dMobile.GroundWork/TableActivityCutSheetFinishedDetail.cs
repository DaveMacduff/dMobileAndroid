// <copyright file="TableActivityCutSheetFinishedDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityCutSheetFinishedDetail" />.
    /// </summary>
    public class TableActivityCutSheetFinishedDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityCutSheetFinishedDetail"/> class.
        /// </summary>
        public TableActivityCutSheetFinishedDetail()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.CutSheetFinished;
            this.LowerCaseName = "cut sheet finished product";
            this.ProperCaseName = "Cut sheet finished product";
        }

        /// <inheritdoc cref="TableBase.PurgeData()"/>
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
                        DELETE FROM tActivityDetailCutSheetFinished
                            WHERE tActivityDetailCutSheetFinished.ActivityDetailRefno <= 0
                                OR tActivityDetailCutSheetFinished.ActivityDetailFKRefno <= 0";
                        transaction.Execute(query);

                        query = @"
                        DELETE FROM tActivityDetailCutSheetFinished
                            WHERE tActivityDetailCutSheetFinished.ActivityDetailRefno
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
                        DELETE FROM tActivityDetailCutSheetFinished
                            WHERE tActivityDetailCutSheetFinished.ActivityDetailRefno IN
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

        /// <inheritdoc cref="TableBase.SendRequest(int)"/>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ActivityItemCutSheetFinishedModel>(
                    "SELECT * FROM tActivityItemCutSheetFinished WHERE ActivityItemFKRefno = ?",
                    activityFkRefno);
            int actItemCutSheetFKRefno;
            if (results?.Count > 0)
            {
                actItemCutSheetFKRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new Exception("drItemFinished.Read() failed.");
            }

            return await SendComm(await this.CreateRequest(actItemCutSheetFKRefno));
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
                                "Saving finished cut sheet details.",
                                false);

                            if (ds.Tables.Count != 0)
                            {
                                var progMax = Convert.ToInt32(
                                    ds.Tables["tActivityDetailCutSheet"].Rows.Count);

                                this.DataAccessStatusOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    $"Records {progMax}",
                                    false);

                                var actDetailRefno = 0;
                                var progValue = 0;
                                var actDetailFKRefno = 0;

                                this.DataAccessProgressOutHandler(
                                    this,
                                    EventArgs.Empty,
                                    0,
                                    0,
                                    progMax);

                                var results = transaction.Query<ActivityItemCutSheetModel>(
                                    "SELECT * FROM tActivityItemCutSheet WHERE ActivityItemRefno = ?",
                                    refno);

                                if (results == null || results.Count == 0)
                                {
                                    throw new Exception(
                                        "No results for ActivityItemRefno ." +
                                        refno +
                                        " in tActivityItemCutSheet." +
                                        Environment.NewLine +
                                        errMsg);
                                }

                                foreach (DataRow drow in
                                         ds.Tables["tActivityDetailCutSheet"].Rows)
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
                                        "ActivityDetailLogisticUnitCutSheetDate, " +
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
                                        Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
                                        Convert.ToInt32(0),
                                        refno,
                                        GetReceivedDataLocation(drow, transaction, false),
                                        results[0].ProductUnitRefno
                                    };
                                    parametersInsertDetail.AddRange(
                                        SaveDetail(drow, transaction, false));

                                    transaction.Execute(
                                        queryInsertDetail,
                                        parametersInsertDetail.ToArray());

                                    actDetailRefno = Convert.ToInt32(
                                        transaction.ExecuteScalar<int>(
                                            "SELECT last_insert_rowid() FROM tActivityDetail"));
                                    actDetailFKRefno = Convert.ToInt32(
                                        drow["ActivityDetailCutSheetFKRefno"]);

                                    errMsg = "FK detail refno " + actDetailFKRefno;

                                    transaction.Execute(
                                        "INSERT INTO tActivityDetailCutSheetFinished (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)",
                                        actDetailRefno,
                                        actDetailFKRefno);

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
                                "Saving finished cut sheet details complete.",
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
                                "Saving finished cut sheet details failed.",
                                false);

                            throw new Exception(
                                "Database exception occurred. Saving finished cut sheet details failed." +
                                Environment.NewLine +
                                errMsg,
                                ex);
                        }
                    });
        }

        /// <inheritdoc cref="TableBase.CreateRequest(int)"/>
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
                "<action>REQUEST CUT SHEET FINISHED DETAILS</action>" +
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