// <copyright file="TableActivityItemBarCodeAssociationXRef.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="TableActivityItemBarCodeAssociationXRef" />.
    /// </summary>
    public class TableActivityItemBarCodeAssociationXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityItemBarCodeAssociationXRef"/> class.
        /// </summary>
        public TableActivityItemBarCodeAssociationXRef()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityItemBarCodeAssociationXRef";
            this.CurrentSubType = SysCommon.ActSubTypes.BarCodeAssociation;
        }

        public override Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityItemBarCodeAssociationXRef");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs failed.", true);

                throw new Exception("Database exception occurred. Purging item bar code association xrefs failed.", ex);
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
