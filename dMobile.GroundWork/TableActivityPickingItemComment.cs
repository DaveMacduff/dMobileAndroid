// <copyright file="TableActivityPickingItemComment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using Bedrock;
    using SQLite;

    public class TableActivityPickingItemComment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingItemComment"/> class.
        /// </summary>
        public TableActivityPickingItemComment()
          : base()
        {
            this.CurrentSubType = SysCommon.ActSubTypes.Picking;
            this.LowerCaseName = "sales order";
            this.ProperCaseName = "Sales order";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments.", true);

                await Conn.ExecuteAsync("DELETE FROM tActivityItemPickingComment");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purge sales order item comments failed.", true);

                throw new BaseException("Database exception occurred. Purging sales order item comments failed.", ex);
            }
        }
    }
}
