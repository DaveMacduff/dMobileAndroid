// <copyright file="LocationMapModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a location map.
    /// </summary>
    [Table("tLocationMap")]
    public class LocationMapModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the location map.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        [Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }
    }
}