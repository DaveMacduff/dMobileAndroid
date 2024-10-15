// <copyright file="ProductUnitBarCodeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a product unit barcode.
    /// </summary>
    [Table("tProductUnitBarCode")]
    public class ProductUnitBarCodeModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product unit barcode.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ProductUnitBarCodeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        [Indexed(Name = "FK_ProductUnitBarCodeOnProductUnitRefno", Order = 1, Unique = false)]
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the product unit barcode.
        /// </summary>
        [NotNull]
        [Indexed(
            Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1",
            Order = 1,
            Unique = true)]
        public string ProductUnitBarCodeCode { get; set; }

        /// <summary>
        /// Gets or sets the raw barcode of the product unit.
        /// </summary>
        [NotNull]
        public string ProductUnitBarCodeRaw { get; set; }

        /// <summary>
        /// Gets or sets the symbology enum of the product unit barcode.
        /// </summary>
        [NotNull]
        public int ProductUnitBarCodeSymbologyEnum { get; set; }

        /// <summary>
        /// Gets or sets the first modifier of the product unit barcode.
        /// </summary>
        [NotNull]
        [Indexed(
            Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1",
            Order = 2,
            Unique = true)]
        public string ProductUnitBarCodeModifier1 { get; set; }

        /// <summary>
        /// Gets or sets the status of the product unit barcode.
        /// </summary>
        [NotNull]
        public int ProductUnitBarCodeStatus { get; set; }
    }
}
