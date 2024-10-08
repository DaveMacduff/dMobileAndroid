// <copyright file="LocationMapWarehouseProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapWarehouseProductScheme")]
    public class LocationMapWarehouseProductSchemeModel
    {
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductSchemeModel))]
        [Indexed(Name = "FK_WhsProdSchemeOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }
    }
}
