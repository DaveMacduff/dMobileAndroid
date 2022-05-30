// <copyright file="clsTableActivityItemBarCodeAssociationXRef.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
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
    }
}
