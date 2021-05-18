namespace dMobile.GroundWork
{
    using System;
    using SQLite;
    using dMobile.Bedrock;
    using System.Threading.Tasks;

    public class TableActivityItem : TableBase
    {
        #region Constructors

        public TableActivityItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityItem";
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actSubType"></param>
        public override async Task Purge(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items.", true);

                string query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging activity items failed.", ex);
            }
        }

        #endregion
    }
}
