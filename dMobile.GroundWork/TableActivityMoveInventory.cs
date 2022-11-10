// <copyright file="TableActivityMoveInventory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
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
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory.", true);

            try
            {
                var query = "DELETE FROM tActivityMoveInventory WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                query = "DELETE FROM tActivity WHERE ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory failed.", true);

                throw new Exception("Database exception occurred. Purging move inventory failed.", ex);
            }
        }
    }
}
