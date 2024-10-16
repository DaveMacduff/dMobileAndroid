// <copyright file="ProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Products
{
    using System;
    using SQLite;

    /// <summary>
    /// Represents a product.
    /// </summary>
    [Table("tProduct")]
    public class ProductModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme.
        /// </summary>
        [NotNull]
        public int ProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the family.
        /// </summary>
        [NotNull]
        public int FamilyRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the product.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductOnProductCode", Order = 1, Unique = true)]
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the product.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ProductOnProductFKRefno", Order = 1, Unique = true)]
        public int ProductFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        [NotNull]
        [Indexed(Name = "ProductOnProductDescription", Order = 1, Unique = false)]
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch count.
        /// </summary>
        [NotNull]
        public bool ProductIsCatchCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch weight.
        /// </summary>
        [NotNull]
        public bool ProductIsCatchWeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch ask.
        /// </summary>
        [NotNull]
        public bool ProductIsCatchAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch ask zero.
        /// </summary>
        [NotNull]
        public bool ProductIsCatchAskZero { get; set; }

        /// <summary>
        /// Gets or sets the brand description of the product.
        /// </summary>
        [NotNull]
        public string ProductBrandDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows conversion.
        /// </summary>
        [NotNull]
        public bool ProductAllowConvertProduct { get; set; }

        /// <summary>
        /// Gets or sets the number of days the product is accepted.
        /// </summary>
        [NotNull]
        public int ProductAcceptDays { get; set; }
    }
}

