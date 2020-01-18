namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

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
        public override void Purge()
        {
            // TODO: Resource strings
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory)));

                    cmndPurge.CommandText = "DELETE FROM tActivityItemMoveInventory WHERE tActivityItemMoveInventory.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurge.ExecuteNonQuery();
                }

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
