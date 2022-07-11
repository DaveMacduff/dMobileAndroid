// <copyright file="TableActivityPickingItemComment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityPickingItemComment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingItemComment"/> class.
        /// </summary>
        public TableActivityPickingItemComment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingItemComment";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemPickingComment");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purge sales order item comments failed.", true);

                throw new Exception("Database exception occurred. Purging sales order item comments failed.", ex);
            }
        }
    }
}
