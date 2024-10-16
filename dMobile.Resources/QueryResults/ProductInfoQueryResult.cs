// <copyright file="ProductInfoQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.QueryResults
{
    /// <summary>
    /// Represents the result of a product information query.
    /// </summary>
    public class ProductInfoQueryResult
    {
        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the product.
        /// </summary>
        public int ProductFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the unit code of the product.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the base unit code of the product.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the weight unit code of the product.
        /// </summary>
        public string WeightUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is a barcode.
        /// </summary>
        public bool ProductUnitIsBC { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch ask.
        /// </summary>
        public bool ProductIsCatchAsk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch ask zero.
        /// </summary>
        public bool ProductIsCatchAskZero { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows conversion.
        /// </summary>
        public bool ProductAllowConvertProduct { get; set; }

        /// <summary>
        /// Gets or sets the number of days the product is accepted.
        /// </summary>
        public int ProductAcceptDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch count.
        /// </summary>
        public bool ProductIsCatchCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is catch weight.
        /// </summary>
        public bool ProductIsCatchWeight { get; set; }

        /// <summary>
        /// Gets or sets the weight of the product unit.
        /// </summary>
        public decimal ProductUnitWeight { get; set; }

        /// <summary>
        /// Gets or sets the description of the product brand.
        /// </summary>
        public string ProductBrandDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is the default unit.
        /// </summary>
        public bool ProductUnitIsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is the sell unit.
        /// </summary>
        public bool ProductUnitIsSellUnit { get; set; }

        /// <summary>
        /// Gets or sets the box count of the product unit.
        /// </summary>
        public decimal ProductUnitBoxCount { get; set; }

        /// <summary>
        /// Gets or sets the weight tolerance of the product unit.
        /// </summary>
        public decimal ProductUnitWeightTolerance { get; set; }
    }
}
