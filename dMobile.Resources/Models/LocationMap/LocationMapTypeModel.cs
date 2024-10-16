// <copyright file="LocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.LocationMap
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the type of a location map.
    /// </summary>
    [Table("tLocationMapType")]
    public class LocationMapTypeModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the location map type.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int LocationMapTypeRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_LocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the enumeration value representing the type of location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "PK_LocationMapTypeOnLocationMapTypeEnum", Order = 1, Unique = false)]
        public int LocationMapTypeEnum { get; set; }
    }
}