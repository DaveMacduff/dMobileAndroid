// <copyright file="ProductUnitModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Products
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a product unit.
    /// </summary>
    [Table("tProductUnit")]
    public class ProductUnitModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the product unit.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductModel))]
        [Indexed(Name = "FK_ProductUnitOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnUnitRefno", Order = 1, Unique = false)]
        public int UnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the base unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnBaseUnitRefno", Order = 1, Unique = false)]
        public int BaseUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the weight unit.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(UnitModel))]
        [Indexed(Name = "FK_ProductUnitOnWtUnitRefno", Order = 1, Unique = false)]
        public int WtUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is a barcode.
        /// </summary>
        [NotNull]
        public bool ProductUnitIsBC { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        [NotNull]
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the weight of the product unit.
        /// </summary>
        [NotNull]
        public decimal ProductUnitWeight { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        [NotNull]
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is a sell unit.
        /// </summary>
        [NotNull]
        public bool ProductUnitIsSellUnit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is the default unit.
        /// </summary>
        [NotNull]
        public bool ProductUnitIsDefault { get; set; }

        /// <summary>
        /// Gets or sets the box count of the product unit.
        /// </summary>
        [NotNull]
        public decimal ProductUnitBoxCount { get; set; }

        /// <summary>
        /// Gets or sets the weight tolerance of the product unit.
        /// </summary>
        [NotNull]
        public decimal ProductUnitWeightTolerance { get; set; }
    }
}
