// <copyright file="TableActivityMoveInventoryItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityMoveInventoryItem : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityMoveInventoryItem"/> class.
        /// </summary>
        public TableActivityMoveInventoryItem()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryItem";
            this.CurrentSubType = SysCommon.ActSubTypes.MoveInventory;
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
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items.", true);

            try
            {
                // Corruption recovery cleanup.
                var query = "DELETE FROM tActivityItemMoveInventory WHERE tActivityItemMoveInventory.ActivityItemRefno <= 0";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                // Corruption recovery cleanup.
                query = "DELETE FROM tActivityItem WHERE tActivityItem.ActivityRefno <= 0";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query);

                // Corruption recovery cleanup.
                query = "DELETE FROM tActivityItemMoveInventory WHERE tActivityItemMoveInventory.ActivityItemRefno " +
                        "NOT IN (SELECT tActivityItem.ActivityItemRefno FROM tActivityItem)";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                query = "DELETE FROM tActivityItemMoveInventory WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                query = "DELETE FROM tActivityItem WHERE ActivityItemSubType = ?";
                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, Convert.ToInt32(this.CurrentSubType));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items failed.", true);

                throw new Exception("Database exception occurred. Purging move inventory items failed.", ex);
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
