// <copyright file="WarehouseLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tWarehouseLocationMapType")]
    public class WarehouseLocationMapTypeModel
    {
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "WarehouseLocationMapTypeOnWarehouseLocationMapTypeEnum", Order = 1, Unique = false)]
        public int WarehouseLocationMapTypeEnum { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_WarehouseLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}
