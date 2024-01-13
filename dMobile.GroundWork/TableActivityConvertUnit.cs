// <copyright file="TableActivityConvertUnit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityConvertUnit : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityConvertUnit"/> class.
        /// </summary>
        public TableActivityConvertUnit()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnit";
            this.CurrentSubType = SysCommon.ActSubTypes.ConvertUnit;
            this.LowerCaseName = "convert units";
            this.ProperCaseName = "Convert units";
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            var conn = DatabaseConnection.GetInstance().SQLConn;
            var subType = Convert.ToInt32(this.CurrentSubType);
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units.", true);

                    // Corruption recovery cleanup.
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                    var query = @"
                        DELETE FROM tActivityConvertUnit
                            WHERE tActivityConvertUnit.ActivityRefno <= 0
                                OR tActivityConvertUnit.ActivityFKRefno <= 0";
                    transaction.Execute(query);

                    query = @"
                        DELETE FROM tActivityConvertUnit
                            WHERE tActivityConvertUnit.ActivityRefno
                                NOT IN (SELECT tActivity.ActivityRefno FROM tActivity)";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivityRefno <= 0";
                    transaction.Execute(query);

                    // Regular purge.
                    query = "DELETE FROM tActivityConvertUnit WHERE tActivityConvertUnit.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                    transaction.Execute(query, subType);

                    transaction.Commit();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
                }
                catch
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw;
                }
            });
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }
    }
}
