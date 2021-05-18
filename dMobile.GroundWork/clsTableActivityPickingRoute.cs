namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using dMobile.Bedrock;
    using System.Threading.Tasks;

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
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityPickingRoute");

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
