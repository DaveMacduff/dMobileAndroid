namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

    public class TableActivityPickingItemComment : TableBase
    {
        #region Constructors

        public TableActivityPickingItemComment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityPickingItemComment";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemPickingComment";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purge sales order item comments failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales order item comments failed.", ex);
            }
        }

        #endregion
    }
}
