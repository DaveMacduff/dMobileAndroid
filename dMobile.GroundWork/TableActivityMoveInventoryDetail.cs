// <copyright file="TableActivityMoveInventoryDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityMoveInventoryDetail" />.
    /// </summary>
    public class TableActivityMoveInventoryDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventoryDetail"/> class.
        /// </summary>
        public TableActivityMoveInventoryDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryDetail";
            this.CurrentSubType = SysCommon.ActSubTypes.MoveInventory;
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
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
                        DELETE FROM tActivityDetailMoveInventory
                            WHERE tActivityDetailMoveInventory.ActivityDetailRefno <= 0
                                OR tActivityDetailMoveInventory.ActivityDetailFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityDetailMoveInventory
                            WHERE tActivityDetailMoveInventory.ActivityDetailRefno
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
                        DELETE FROM tActivityDetailMoveInventory
                            WHERE tActivityDetailMoveInventory.ActivityDetailRefno IN
                                (SELECT tActivityDetail.ActivityDetailRefno
                                    FROM tActivityDetail
                                    WHERE tActivityDetail.ActivityDetailSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details complete.", true);
                }
                catch (SQLiteException sqlEx)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details failed.", true);
                    throw new Exception("Database exception occurred. Purging move inventory details failed.", sqlEx);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Database exception occurred. Purging move inventory details failed.", ex);
                }
            });
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
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

        protected override Task<string> CreateRequest(int activityItemFkRefno)
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

        protected override Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }
    }
}
