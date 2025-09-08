// <copyright file="TableActivityConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using Bedrock;
    using SQLite;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TableActivityConvertUnitItem" />
    /// </summary>
    public class TableActivityConvertUnitItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityConvertUnitItem"/> class.
        /// </summary>
        public TableActivityConvertUnitItem()
            : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.ConvertUnit;
            this.LowerCaseName = "convert unit";
            this.ProperCaseName = "Convert unit";
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items.", true);

                    // Corruption recovery cleanup.
                    var query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno
                                NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                    transaction.Execute(query);

                    query = "DELETE FROM tActivityItemConvertUnit WHERE tActivityItemConvertUnit.ActivityItemRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityItemConvertUnit
                            WHERE tActivityItemConvertUnit.ActivityItemRefno
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
                    query = @"
                        DELETE FROM tActivityItemBarCodeAssociationXRef
                            WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN
                                (SELECT tActivityItem.ActivityItemRefno
                                    FROM tActivityItem
                                    WHERE tActivityItem.ActivityItemSubType = ?)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityItemConvertUnit WHERE ActivityItemSubType = ?";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} items complete.", true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} items failed.", true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} items failed.", ex);
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
            });
        }
    }
}
