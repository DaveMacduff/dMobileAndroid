namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using SQLite;
    using System;
    using System.Threading.Tasks;

    public class TableActivityConvertUnit : TableBase
    {
        #region Constructors

        public TableActivityConvertUnit()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnit";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units.", true);

                string query = "DELETE FROM tActivityConvertUnit WHERE tActivityConvertUnit.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging convert units failed.", ex);
            }
        }

        #endregion
    }
}
