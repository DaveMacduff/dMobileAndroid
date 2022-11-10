// <copyright file="TableActivityPickingRoute.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityPickingRoute : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityPickingRoute"/> class.
        /// </summary>
        public TableActivityPickingRoute()
          : base()
        {
            this.FullClassName = "TableActivityPickingRoute";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tActivityPickingRoute");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order routes failed.", true);

                throw new Exception("Database exception occurred. Purging sales order routes failed.", ex);
            }
        }
    }
}
