// <copyright file="TableActivityConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityConvertUnitItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityConvertUnitItem"/> class.
        /// </summary>
        public TableActivityConvertUnitItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnitItem";
            this.CurrentSubType = SysCommon.ActSubTypes.ConvertUnit;
            this.LowerCaseName = "convert unit";
            this.ProperCaseName = "Convert unit";
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
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items.", true);

                var query = "DELETE FROM tActivityItemConvertUnit WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(SysCommon.ActSubTypes.ConvertUnit));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items failed.", true);

                throw new Exception("Database exception occurred. Purging convert unit items failed.", ex);
            }
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
