namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using SQLite;
    using dMobile.Bedrock;
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

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityConvertUnit WHERE tActivityConvertUnit.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?", Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

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
