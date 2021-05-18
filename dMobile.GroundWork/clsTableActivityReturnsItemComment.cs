namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using dMobile.Bedrock;
    using System.Threading.Tasks;

    public class TableActivityReturnsItemComment : TableBase
    {
        #region Constructors

        public TableActivityReturnsItemComment()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItemComment";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityReturnsItemComment");

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging sales return item comments failed.", ex);
            }
        }

        #endregion
    }
}
