namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

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
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref details.", true);

                using (SQLiteCommand cmndPurgeProdSchemeVarXRef = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurgeProdSchemeVarXRef.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef";
                    cmndPurgeProdSchemeVarXRef.CommandType = CommandType.Text;
                    cmndPurgeProdSchemeVarXRef.ExecuteNonQuery();
                }

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
