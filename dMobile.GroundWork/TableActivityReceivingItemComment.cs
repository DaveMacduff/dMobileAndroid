// <copyright file="TableActivityReceivingItemComment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityReceivingItemComment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReceivingItemComment"/> class.
        /// </summary>
        public TableActivityReceivingItemComment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItemComment";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemReceivingComment");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments failed.", true);

                throw new Exception("Database exception occurred. Purging receiving item comments failed.", ex);
            }
        }
    }
}
