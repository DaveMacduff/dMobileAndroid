// <copyright file="ProductSchemeVariantModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a product scheme variant.
    /// </summary>
    [Table("tProductSchemeVariant")]
    public class ProductSchemeVariantModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product scheme variant.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeVariantRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the product scheme variant.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantCode", Order = 1, Unique = true)]
        public string ProductSchemeVariantCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product scheme variant.
        /// </summary>
        [NotNull]
        public string ProductSchemeVariantDescription { get; set; }

        /// <summary>
        /// Gets or sets the unique number of the product scheme variant.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantNo", Order = 1, Unique = true)]
        public int ProductSchemeVariantNo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product scheme variant is the default variant.
        /// </summary>
        [NotNull]
        public bool ProductSchemeVariantIsDefault { get; set; }
    }
}