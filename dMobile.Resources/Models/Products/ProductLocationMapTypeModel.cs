// <copyright file="ProductLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Products
{
    using dMobile.Resources.Models.LocationMap;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the mapping type between a product and its location.
    /// </summary>
    [Table("tProductLocationMapType")]
    public class ProductLocationMapTypeModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product location map type.
        /// </summary>
        [NotNull, PrimaryKey, AutoIncrement]
        public int ProductLocationMapTypeID { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the enumeration value representing the type of product location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "FK_ProductLocationMapTypeOnProductLocationMapTypeEnum", Order = 1, Unique = false)]
        public int ProductLocationMapTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}