// <copyright file="tLocationMapWarehouseProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapWarehouseProductScheme")]
    public class tLocationMapWarehouseProductSchemeModel
    {
        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_WhsProdSchemeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductModel)), Indexed(Name = "FK_WhsProdSchemeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductSchemeModel)), Indexed(Name = "FK_WhsProdSchemeOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }
    }
}
