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
            this.LowerCaseName = "item bar code association xrefs";
            this.ProperCaseName = "Item Bar Code Association Xrefs";
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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging existing {this.LowerCaseName} items.", true);
            var conn = DatabaseConnection.GetInstance().SQLConn;
            await conn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    // Regular purge.
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);
                    const string query = @"DELETE FROM tActivityItemBarCodeAssociationXRef";
                    transaction.Execute(query);

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
                catch (Exception ex)
                {
                    transaction.Rollback();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);
                    throw new Exception($"Purging {this.LowerCaseName} failed.", ex);
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
