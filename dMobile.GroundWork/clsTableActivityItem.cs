// <copyright file="clsTableActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityItem"/> class.
        /// </summary>
        public TableActivityItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actSubType"></param>
        public override async Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items.", true);

                string query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items failed.", true);

                throw new Exception("Database exception occurred. Purging activity items failed.", ex);
            }
        }
    }
}
