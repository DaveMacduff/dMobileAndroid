// <copyright file="TableActivityMoveInventoryItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using Bedrock;
    using SQLite;

    public class TableActivityMoveInventoryItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventoryItem"/> class.
        /// </summary>
        public TableActivityMoveInventoryItem()
          : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.MoveInventory;
            this.LowerCaseName = "move inventory item";
            this.ProperCaseName = "Move inventory item";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);
            await Conn.RunInTransactionAsync(transaction =>
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

                    throw new BaseException("Database exception occurred. Purging move inventory items failed.", ex);
                }
            });
        }
    }
}
