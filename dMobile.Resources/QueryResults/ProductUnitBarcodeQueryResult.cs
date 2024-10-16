// <copyright file="ProductUnitBarcodeQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.QueryResults
{
    /// <summary>
    /// Represents the result of a query for product unit barcodes.
    /// </summary>
    public class ProductUnitBarcodeQueryResult
    {
        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit barcode.
        /// </summary>
        public int ProductUnitBarCodeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse product scheme.
        /// </summary>
        public int WarehouseProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme.
        /// </summary>
        public int ProductSchemeRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the unit code.
        /// </summary>
        public string UnitCode { get; set; }
    }
}