namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using SQLite;
    using System;
    using System.Threading.Tasks;

    public class TableActivityMoveInventoryDetail : TableBase
    {
        #region Constructors

        public TableActivityMoveInventoryDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryDetail";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            // TODO: Resource strings
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

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging move inventory details failed.", ex);
            }
        }

        #endregion
    }
}
