namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using SQLite;
    using System;
    using System.Threading.Tasks;

    public class TableActivityDetailProductSchemeVariantXRef : TableBase
    {
        #region Constructors

        public TableActivityDetailProductSchemeVariantXRef()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityDetailProductSchemeVariantXRef";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref details.", true);

                string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging product scheme variant xref details failed.", ex);
            }
        }

        #endregion
    }
}
