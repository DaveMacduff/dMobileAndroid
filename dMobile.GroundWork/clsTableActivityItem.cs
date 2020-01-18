namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

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
        public override void Purge(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items.", true);

                using (SQLiteCommand cmndPurgeActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurgeActItem.CommandText = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = @activityItemSubType";
                    cmndPurgeActItem.CommandType = CommandType.Text;
                    cmndPurgeActItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(actSubType)));
                    cmndPurgeActItem.ExecuteNonQuery();
                }

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
