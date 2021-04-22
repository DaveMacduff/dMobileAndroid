namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;
    using System;
    using System.Data;
    using System.Threading.Tasks;

    public class TableActivity : TableBase
    {
        #region Constructors

        public TableActivity()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivity";
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity.", true);

                string query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging activity failed.", ex);
            }
        }

        #endregion
    }
}
