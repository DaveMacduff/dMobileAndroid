// <copyright file="tLocationMapSlotCoordinateModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tLocationMapSlotCoordinate")]
    public class tLocationMapSlotCoordinateModel
    {
        [PrimaryKey]
        [NotNull]
        public int LocationMapSlotCoordinateRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 1, Unique = true)]
        public int LocationMapSlotCoordinateEnum { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 2, Unique = true)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 3, Unique = true)]
        public string LocationMapSlotCoordinateCode { get; set; }
    }
}
