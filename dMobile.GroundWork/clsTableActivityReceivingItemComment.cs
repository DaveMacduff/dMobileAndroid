namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using System.Threading.Tasks;
    using dMobile.Bedrock;

    public class TableActivityReceivingItemComment : TableBase
    {
        #region Constructors

        public TableActivityReceivingItemComment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItemComment";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemReceivingComment");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging receiving item comments failed.", ex);
            }
        }

        #endregion
    }
}
