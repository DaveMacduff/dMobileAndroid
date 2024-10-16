// <copyright file="BarcodeAssociationItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents an item associated with a barcode.
    /// </summary>
    public class BarcodeAssociationItem
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit has a barcode.
        /// </summary>
        public bool ProductUnitIsBC { get; set; }

        /// <summary>
        /// Gets or sets the code of the unit.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit barcode.
        /// </summary>
        public int ProductUnitBarCodeRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product unit barcode.
        /// </summary>
        public string ProductUnitBarCodeCode { get; set; }

        /// <summary>
        /// Gets or sets the first modifier of the product unit barcode.
        /// </summary>
        public string ProductUnitBarCodeModifier1 { get; set; }

        /// <summary>
        /// Gets or sets the symbology enum of the product unit barcode.
        /// </summary>
        public int ProductUnitBarCodeSymbologyEnum { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity item.
        /// </summary>
        public int ActivityItemStatus { get; set; }
    }
}