namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using dMobile.Bedrock;

    public class TableActivityConvertUnitDetail : TableBase
    {
        #region Constructors

        public TableActivityConvertUnitDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnitDetail";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit)));

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                        + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetailConvertUnit "
                        + "WHERE tActivityDetailConvertUnit.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType)";
                    cmndPurge.ExecuteNonQuery();

                    cmndPurge.CommandText = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = @activityDetailSubType";
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details failed.", true);

                // TODO: Resource strings
                throw new Exception("Database exception occurred. Purging convert unit details failed.", ex);
            }
        }

        #endregion
    }
}
