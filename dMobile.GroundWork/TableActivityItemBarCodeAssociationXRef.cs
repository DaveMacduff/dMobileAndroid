// <copyright file="TableActivityItemBarCodeAssociationXRef.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityItemBarCodeAssociationXRef" />.
    /// </summary>
    public class TableActivityItemBarCodeAssociationXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityItemBarCodeAssociationXRef"/> class.
        /// </summary>
        public TableActivityItemBarCodeAssociationXRef()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityItemBarCodeAssociationXRef";
            this.CurrentSubType = SysCommon.ActSubTypes.BarCodeAssociation;
            this.LowerCaseName = "item bar code association xrefs";
            this.ProperCaseName = "Item Bar Code Association Xrefs";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging existing {this.LowerCaseName} items.", true);
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Regular purge.
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                    const string query = @"DELETE FROM tActivityItemBarCodeAssociationXRef";
                    transaction.Execute(query);

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
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw new Exception($"Purging {this.LowerCaseName} failed.", ex);
                }
            });
        }
    }
}
