namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

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
        public override void Purge(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity.", true);

                using (SQLiteCommand cmndPurgeActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurgeActivity.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurgeActivity.CommandType = CommandType.Text;
                    cmndPurgeActivity.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(actSubType)));
                    cmndPurgeActivity.ExecuteNonQuery();
                }

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
