// <copyright file="TableActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityItem"/> class.
        /// </summary>
        public TableActivityItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityItem";
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actSubType"></param>
        /// <inheritdoc/>
        public override async Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items.", true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                    transaction.Execute(query, actSubType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items complete.", true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items failed.", true);
                    throw new Exception("Database exception occurred. Purging activity items failed.", ex);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items failed.", true);
                    throw new Exception("Database exception occurred. Purging activity items failed.", ex);
                }
            });
        }

        public override Task PurgeData()
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
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
