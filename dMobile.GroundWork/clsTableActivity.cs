// <copyright file="clsTableActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
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

                string query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity failed.", true);

                throw new Exception("Database exception occurred. Purging activity failed.", ex);
            }
        }
    }
}
