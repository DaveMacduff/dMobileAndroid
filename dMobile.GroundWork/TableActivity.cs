// <copyright file="TableActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivity" /> Table activity class.
    /// </summary>
    public class TableActivity : TableBase
    {
        /// <inheritdoc/>
        public override async Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity.", true);

                const string query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity complete.", true);
            }
            catch (SQLiteException sqlEx)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity failed.", true);
                throw;
            }
        }
    }
}
