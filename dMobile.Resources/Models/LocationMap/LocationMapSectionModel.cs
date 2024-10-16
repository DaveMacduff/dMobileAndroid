// <copyright file="LocationMapSectionModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.LocationMap
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a section in a location map.
    /// </summary>
    [Table("tLocationMapSection")]
    public class LocationMapSectionModel
    {
        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "FK_LocationMapSectionOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location map section.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionCode", Order = 1, Unique = true)]
        public int LocationMapSectionCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location map section.
        /// </summary>
        [Indexed(Name = "UQ_LocationMapSectionOnWarehouseLocationMapRefnoLocationMapSectionName", Order = 1, Unique = false)]
        public string LocationMapSectionName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the location map section.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionFKRefno", Order = 1, Unique = true)]
        public int LocationMapSectionFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the location map section.
        /// </summary>
        [Indexed(Name = "UQ_LocationMapSectionOnLocationMapSectionBarCode", Order = 1, Unique = true)]
        public string LocationMapSectionBarCode { get; set; }
    }
}