// <copyright file="LocationMapWarehouseProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.LocationMap
{
    using dMobile.Resources.Models.Products;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the mapping between a warehouse location, product, and product scheme.
    /// </summary>
    [Table("tLocationMapWarehouseProductScheme")]
    public class LocationMapWarehouseProductSchemeModel
    {
        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product scheme.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ProductSchemeModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }
    }
}