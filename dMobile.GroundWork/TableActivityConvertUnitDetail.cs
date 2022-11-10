// <copyright file="TableActivityConvertUnitDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityConvertUnitDetail : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityConvertUnitDetail"/> class.
        /// </summary>
        public TableActivityConvertUnitDetail()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnitDetail";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details.", true);

                var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                            + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                            + "(SELECT tActivityDetail.ActivityDetailRefno "
                            + "FROM tActivityDetail "
                            + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivityDetailProductSchemeVariantXRef "
                     + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
                     + "(SELECT tActivityDetail.ActivityDetailRefno "
                     + "FROM tActivityDetail "
                     + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivityDetailConvertUnit "
                        + "WHERE tActivityDetailConvertUnit.ActivityDetailRefno IN "
                        + "(SELECT tActivityDetail.ActivityDetailRefno "
                        + "FROM tActivityDetail "
                        + "WHERE tActivityDetail.ActivityDetailSubType = ?)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivityDetail WHERE tActivityDetail.ActivityDetailSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit details failed.", true);

                throw new Exception("Database exception occurred. Purging convert unit details failed.", ex);
            }
        }
    }
}
