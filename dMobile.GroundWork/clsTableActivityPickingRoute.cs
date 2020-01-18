namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

    public class TableActivityPickingRoute : TableBase
    {
        #region Constructors

        public TableActivityPickingRoute()
          : base()
        {
            this.FullClassName = "TableActivityPickingRoute";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityPickingRoute";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales order routes failed.", ex);
            }
        }

        #endregion
    }
}
