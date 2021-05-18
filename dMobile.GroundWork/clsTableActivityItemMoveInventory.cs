namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using dMobile.Bedrock;
    using System.Threading.Tasks;

    public class TableActivityMoveInventoryItem : TableBase
    {
        #region Constructors

        public TableActivityMoveInventoryItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryItem";
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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items.", true);

            try
            {
                string query = "DELETE FROM tActivityItemMoveInventory WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging move inventory items failed.", ex);
            }
        }

        #endregion
    }
}
