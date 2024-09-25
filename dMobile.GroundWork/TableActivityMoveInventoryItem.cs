// <copyright file="TableActivityMoveInventoryItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityMoveInventoryItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventoryItem"/> class.
        /// </summary>
        public TableActivityMoveInventoryItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryItem";
            this.CurrentSubType = SysCommon.ActSubTypes.MoveInventory;
            this.LowerCaseName = "move inventory item";
            this.ProperCaseName = "Move inventory item";
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
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Corruption recovery cleanup.
                    var query = @"
                        DELETE FROM tActivityItemMoveInventory
                            WHERE tActivityItemMoveInventory.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemMoveInventory
                            WHERE tActivityItemMoveInventory.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItem
                            WHERE tActivityItem.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityItemMoveInventory WHERE ActivityItemSubType = ?";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                    transaction.Execute(query, subType);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items complete.", true);
                }
                catch (SQLiteException ex)
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items failed.", true);

                    throw new Exception("Database exception occurred. Purging move inventory items failed.", ex);
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
