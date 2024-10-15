// <copyright file="LocationMapSlotCoordinateModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    /// <summary>
    /// Represents a coordinate for a slot in a location map.
    /// </summary>
    [Table("tLocationMapSlotCoordinate")]
    public class LocationMapSlotCoordinateModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the location map slot coordinate.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int LocationMapSlotCoordinateRefno { get; set; }

        /// <summary>
        /// Gets or sets the enumeration value representing the slot coordinate.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 1, Unique = true)]
        public int LocationMapSlotCoordinateEnum { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 2, Unique = true)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map slot coordinate.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 3, Unique = true)]
        public string LocationMapSlotCoordinateCode { get; set; }
    }
}