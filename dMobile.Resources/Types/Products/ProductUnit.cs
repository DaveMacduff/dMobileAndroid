// <copyright file="ProductUnit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Products
{
    /// <summary>
    /// Represents a unit of a product.
    /// </summary>
    public class ProductUnit
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
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the unit code.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the base unit code.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the weight unit code.
        /// </summary>
        public string WeightUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit has a barcode.
        /// </summary>
        public bool ProductUnitIsBC { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is a sell unit.
        /// </summary>
        public bool ProductUnitIsSellUnit { get; set; }
    }
}