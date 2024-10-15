// <copyright file="ProductSchemeLotNumberModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a product scheme lot number.
    /// </summary>
    [Table("tProductSchemeLotNumber")]
    public class ProductSchemeLotNumberModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product scheme lot number.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductSchemeLotNumberRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the product scheme lot number.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode", Order = 1, Unique = true)]
        public string ProductSchemeLotNumberCode { get; set; }

        /// <summary>
        /// Gets or sets the default code of the product scheme lot number.
        /// </summary>
        [NotNull]
        public string ProductSchemeLotNumberCodeDefault { get; set; }
    }
}