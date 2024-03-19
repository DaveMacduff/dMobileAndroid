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
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
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
      : base()
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
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno">The actItemRefno<see cref="int"/>.</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            try
            {
                var query = "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?";
                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tActivityItemCountModel>(query, actItemRefno);

                int actItemCountFKRefno;
                if (results != null && results.Count > 0)
                {
                    actItemCountFKRefno = results[0].ActivityItemFKRefno;
                }
                else
                {
                    throw new Exception("No results in tActivityItemCount for ActivityItemRefno " + actItemRefno);
                }

                return await this.SendComm(await this.CreateRequest(actItemCountFKRefno));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Send cycle count details failed. ", ex);
            }
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        /// <param name="actItemCountRefno"></param>
        protected override async Task Save(string responseData, int actItemCountRefno)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SqlConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using (var ds = new DataSet())
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details.", false);

                        // Receiving schema but no data results in no table being created in the dataset.
                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailCount"].Rows.Count);

                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}.", false);

                            var actDetailRefno = 0;
                            var progValue = 0;
                            var actDetailCycleFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            var countItems = transaction.Query<tActivityItemCountModel>("SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?", actItemCountRefno);

                            if (countItems == null || (countItems != null && countItems.Count == 0))
                            {
                                throw new Exception("drItemCount.Read() failed.");
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
                            {
                                errMsg = "Arguments:";

                                var detailParameterList = new List<object>();
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

                                detailParameterList.Add(Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                                detailParameterList.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
                                detailParameterList.Add(actItemCountRefno);
                                detailParameterList.Add(this.GetDetailLocation(drow, transaction, true));
                                detailParameterList.Add(countItems[0].ProductUnitRefno);
                                detailParameterList.AddRange(this.SaveDetail(drow, transaction, true));

                                transaction.Execute(queryInsertDetail, detailParameterList.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailCycleFKRefno = Convert.ToInt32(drow["f01"]);

                                errMsg += Environment.NewLine + "Detail fk refno" + actDetailCycleFKRefno;

                                transaction.Execute("INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)", actDetailRefno, actDetailCycleFKRefno);

                                this.SaveCommonDetailVariant(drow, transaction, actDetailRefno, true);
                                this.SaveDetailExtra(drow, transaction, actDetailRefno, true);

                                progValue++;
                                if (progValue % 10 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving cycle count details failed.", true);

                    throw new Exception("Database exception occurred. Saving cycle count details failed." + Environment.NewLine + errMsg, ex);
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
                + "<action>REQUEST CYCLE COUNT DETAILS</action>"
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
