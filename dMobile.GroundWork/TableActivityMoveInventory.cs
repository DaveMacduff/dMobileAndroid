// <copyright file="TableActivityMoveInventory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityMoveInventory : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventory"/> class.
        /// </summary>
        public TableActivityMoveInventory()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventory";
            this.CurrentSubType = SysCommon.ActSubTypes.MoveInventory;
            this.LowerCaseName = "move inventory";
            this.ProperCaseName = "Move Inventory";
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                    var query = "DELETE FROM tActivityMoveInventory WHERE tActivityMoveInventory.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = @"DELETE FROM tActivityMoveInventory
                                WHERE tActivityMoveInventory.ActivityRefno
                                    NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = @"DELETE FROM tActivityItem
                                WHERE tActivityItem.ActivityRefno
                                    NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityMoveInventory WHERE ActivitySubType = ?";
                    transaction.Execute(query, this.CurrentSubType);

                    query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                    transaction.Execute(query, this.CurrentSubType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw new Exception($"Exception occurred. Purging {this.LowerCaseName} failed.", ex);
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

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
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
