// <copyright file="TableActivityConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityConvertUnitItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityConvertUnitItem"/> class.
        /// </summary>
        public TableActivityConvertUnitItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnitItem";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items.", true);

                var query = "DELETE FROM tActivityItemConvertUnit WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items failed.", true);

                throw new Exception("Database exception occurred. Purging convert unit items failed.", ex);
            }
        }
    }
}
