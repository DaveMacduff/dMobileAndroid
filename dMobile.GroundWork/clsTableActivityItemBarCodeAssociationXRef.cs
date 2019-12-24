namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using System.Data.SQLite;

    /// <summary>
    /// Defines the <see cref="TableActivityItemBarCodeAssociationXRef" />
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
        /// The Purge
        /// </summary>
        public override void Purge()
        {
            try
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource Strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs failed.", true);

                // TODO: Resource Strings
                throw new Exception("Database exception occurred. Purging item bar code association xrefs failed.", ex);
            }
        }
    }
}
