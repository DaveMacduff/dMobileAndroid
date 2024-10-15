// <copyright file="WarehouseLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the type of warehouse location map.
    /// </summary>
    [Table("tWarehouseLocationMapType")]
    public class WarehouseLocationMapTypeModel
    {
        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_WarehouseLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the enum type of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "WarehouseLocationMapTypeOnWarehouseLocationMapTypeEnum", Order = 1, Unique = false)]
        public int WarehouseLocationMapTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_WarehouseLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}
