// <copyright file="tProductLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tProductLocationMapType")]
    public class tProductLocationMapTypeModel
    {
        [NotNull]
        [ForeignKey(typeof(tProductModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "FK_ProductLocationMapTypeOnProductLocationMapTypeEnum", Order = 1, Unique = false)]
        public int ProductLocationMapTypeEnum { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        [Indexed(Name = "FK_ProductLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}
