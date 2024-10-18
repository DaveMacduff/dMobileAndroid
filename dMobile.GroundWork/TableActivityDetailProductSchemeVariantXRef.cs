// <copyright file="TableActivityDetailProductSchemeVariantXRef.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using SQLite;

    public class TableActivityDetailProductSchemeVariantXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityDetailProductSchemeVariantXRef"/> class.
        /// </summary>
        public TableActivityDetailProductSchemeVariantXRef()
          : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityDetailProductSchemeVariantXRef";
            this.LowerCaseName = "product scheme variant xref";
            this.ProperCaseName = "Product scheme variant xref";
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);

                const string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef";
                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details failed.", true);

                throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} details failed.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred. Purging {this.LowerCaseName} details failed.", ex);
            }
        }
    }
}
