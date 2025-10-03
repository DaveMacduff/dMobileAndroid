// <copyright file="TableActivityDetailProductSchemeVariantXRef.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using Bedrock;
    using SQLite;

    /// <summary>
    /// The Activity Detail Product Scheme Variant XRef table.
    /// </summary>
    public class TableActivityDetailProductSchemeVariantXRef : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityDetailProductSchemeVariantXRef"/> class.
        /// </summary>
        public TableActivityDetailProductSchemeVariantXRef()
          : base()
        {
            this.LowerCaseName = "product scheme variant xref";
            this.ProperCaseName = "Product scheme variant xref";
        }

        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details.", true);

                const string query = "DELETE FROM tActivityDetailProductSchemeVariantXRef";
                await Conn.ExecuteAsync(query);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} details failed.", true);

                throw new BaseException($"Database exception occurred. Purging {this.LowerCaseName} details failed.", ex);
            }
            catch (Exception ex)
            {
                throw new BaseException($"Exception occurred. Purging {this.LowerCaseName} details failed.", ex);
            }
        }
    }
}
