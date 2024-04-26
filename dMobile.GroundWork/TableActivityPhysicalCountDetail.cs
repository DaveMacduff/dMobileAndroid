// <copyright file="TableActivityPhysicalCountDetail.cs" company="InterSect Business Systems Inc">
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
    using Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityPhysicalCountDetail" />.
    /// </summary>
    public class TableActivityPhysicalCountDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPhysicalCountDetail"/> class.
        /// </summary>
        public TableActivityPhysicalCountDetail()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPhysicalCountDetail";
            this.CurrentSubType = SysCommon.ActSubTypes.PhysicalCounts;
            this.LowerCaseName = "physical count";
            this.ProperCaseName = "Physical count";
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
        ///     Send the request to the comm server.
        /// </summary>
        /// <param name="actItemRefno">Activity item reference number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            const string query = "SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?";
            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<tActivityItemCountModel>(
                query,
                actItemRefno);

            int actItemCountFkRefno;
            if (results != null && results.Count > 0)
            {
                actItemCountFkRefno = results[0].ActivityItemFKRefno;
            }
            else
            {
                throw new Exception("No results in tActivityItemCount for ActivityItemRefno " + actItemRefno);
            }

            return await this.SendComm(await this.CreateRequest(actItemCountFkRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"> Response data.</param>
        /// <param name="actItemCountRefno"> Activity item count reference number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details.", false);

                        if (ds.Tables.Count != 0)
                        {
                            var progMax = Convert.ToInt32(ds.Tables["tActivityDetailCount"].Rows.Count);
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}.", false);
                            var actDetailRefno = 0;
                            var progValue = 0;
                            var actDetailPhysicalFKRefno = 0;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            var resultCountItems = transaction.Query<tActivityItemCountModel>("SELECT * FROM tActivityItemCount WHERE ActivityItemRefno = ?", actItemCountRefno);

                            if (resultCountItems == null || (resultCountItems != null && resultCountItems.Count == 0))
                            {
                                throw new Exception("drItemCount.Read() failed.");
                            }

                            foreach (DataRow drow in ds.Tables["tActivityDetailCount"].Rows)
                            {
                                errMsg = "Arguments:";
                                var parametersInsertDetail = new List<object>();
                                const string queryInsertDetail = @"
                                    INSERT INTO tActivityDetail (
                                            ActivityDetailSubType, 
                                            ActivityDetailStatus, 
                                            ActivityItemRefno, 
                                            LocationMapRefno, 
                                            ProductUnitRefno, 
                                            ActivityDetailUniqueSchemeID, 
                                            ActivityDetailSchemeID, 
                                            ActivityDetailQuantity, 
                                            ActivityDetailBaseQuantity, 
                                            ActivityDetailWeightQuantity, 
                                            ActivityDetailSingleTradeItemSerialNumber, 
                                            ActivityDetailLogisticUnitBatchLotNumber, 
                                            ActivityDetailLogisticUnitBestBeforeSellByDate, 
                                            ActivityDetailLogisticUnitExpiryUseByDate, 
                                            ActivityDetailLogisticUnitProductionDate, 
                                            ActivityDetailLogisticUnitDueDate, 
                                            ActivityDetailLogisticUnitPackagingDate, 
                                            ActivityDetailLogisticUnitSkidNumber
                                        ) VALUES (
                                            ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
                                        )";

                                parametersInsertDetail.Add(Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts));
                                parametersInsertDetail.Add(Convert.ToInt32(0));
                                parametersInsertDetail.Add(actItemCountRefno);
                                parametersInsertDetail.Add(this.GetDetailLocation(drow, transaction, true));
                                parametersInsertDetail.Add(resultCountItems[0].ProductUnitRefno);
                                parametersInsertDetail.AddRange(this.SaveDetail(drow, transaction, true));

                                transaction.Execute(queryInsertDetail, parametersInsertDetail.ToArray());

                                actDetailRefno = Convert.ToInt32(transaction.ExecuteScalar<int>(
                                    "SELECT last_insert_rowid() FROM tActivityDetail"));
                                actDetailPhysicalFKRefno = Convert.ToInt32(drow["f01"]);
                                errMsg += Environment.NewLine + "Detail fk refno " + actDetailPhysicalFKRefno;

                                transaction.Execute(@"INSERT INTO tActivityDetailCount
                                        (ActivityDetailRefno, ActivityDetailFKRefno) 
                                        VALUES 
                                        (?, ?)",
                                    actDetailRefno,
                                    actDetailPhysicalFKRefno);

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

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving physical count details failed.", true);

                    throw new Exception("Database exception occurred. Saving physical count details failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno"></param>
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

            var request = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                <imclient>
                <command>
                <action>REQUEST PHYSICAL COUNT DETAILS</action>
                {await SysCommon.GetRequestDefaults(false)}
                <itm_refno>{activityItemFkRefno}</itm_refno>
                </command>
                </imclient>");

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
