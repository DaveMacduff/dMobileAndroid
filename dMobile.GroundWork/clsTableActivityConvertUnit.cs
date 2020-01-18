namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

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
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit)));

                    cmndPurge.CommandText = "DELETE FROM tActivityConvertUnit WHERE tActivityConvertUnit.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = @activitySubType";
                    cmndPurge.ExecuteNonQuery();
                }

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
