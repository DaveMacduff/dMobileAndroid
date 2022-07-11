// <copyright file="TableActivityMoveInventoryDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityMoveInventoryDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventoryDetail"/> class.
        /// </summary>
        public TableActivityMoveInventoryDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryDetail";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task PurgeData()
        {
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details.", true);

            try
            {
                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                     + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                     + "(SELECT tActivityDetail.ActivityDetailRefno "
                     + "FROM tActivityDetail "
                     + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                query = "DELETE FROM tActivityDetailMoveInventory "
                         + "WHERE tActivityDetailMoveInventory.ActivityDetailRefno IN "
                         + "(SELECT tActivityDetail.ActivityDetailRefno "
                         + "FROM tActivityDetail "
                         + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details failed.", true);

                throw new Exception("Database exception occurred. Purging move inventory details failed.", ex);
            }
        }
    }
}
