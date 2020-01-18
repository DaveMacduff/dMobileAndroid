namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

    public class TableActivityMoveInventory : TableBase
    {
        #region Constructors

        public TableActivityMoveInventory()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventory";
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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory.", true);

            try
            {
                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.MoveInventory)));

                    cmndPurge.CommandText = "DELETE FROM tActivityMoveInventory WHERE tActivityMoveInventory.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging move inventory failed.", ex);
            }
        }

        #endregion
    }
}
