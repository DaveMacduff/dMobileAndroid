// <copyright file="LocationMapSectionModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapSection")]
    public class LocationMapSectionModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        [NotNull]
        [Indexed(Name = "FK_LocationMapSectionOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionCode", Order = 1, Unique = true)]
        public int LocationMapSectionCode { get; set; }

        [Indexed(Name = "UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName", Order = 1, Unique = false)]
        public string LocationMapSectionName { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionFKRefno", Order = 1, Unique = true)]
        public int LocationMapSectionFKRefno { get; set; }

        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionBarCode", Order = 1, Unique = true)]
        public string LocationMapSectionBarCode { get; set; }
    }
}
