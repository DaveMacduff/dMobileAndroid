namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using dMobile.Bedrock;
    using System.Threading.Tasks;

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
        public override async Task Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemPickingComment");

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
