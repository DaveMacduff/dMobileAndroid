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
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivity"/> class.
        /// </summary>
        public TableActivity()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivity";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <param name="actSubType">The actSubType<see cref="SysCommon.ActSubTypes"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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
