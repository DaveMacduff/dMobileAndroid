// <copyright file="TableActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivity" /> Table activity class.
    /// </summary>
    public class TableActivity : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivity"/> class.
        /// </summary>
        public TableActivity()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivity";
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <param name="actSubType">The actSubType<see cref="SysCommon.ActSubTypes"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity.", true);

                const string query = "DELETE FROM tActivity WHERE tActivity.ActivitySubType = ?";
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query, Convert.ToInt32(actSubType));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity complete.", true);
            }
            catch (SQLiteException sqlEx)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{sqlEx.Message}", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity failed.", true);
                throw;
            }
        }

        public override Task PurgeData()
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
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

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
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
