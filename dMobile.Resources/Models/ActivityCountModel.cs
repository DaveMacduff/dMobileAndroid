// <copyright file="ActivityCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a count activity.
    /// </summary>
    [Table("tActivityCount")]
    public class ActivityCountModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [Indexed(Name = "FK_ActivityCountOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the type of the count activity.
        /// </summary>
        [NotNull]
        public int ActivityCountType { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the count activity.
        /// </summary>
        [NotNull]
        public DateTime ActivityCountDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the count type.
        /// </summary>
        [NotNull]
        public string ActivityCountTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of the count activity.
        /// </summary>
        [NotNull]
        public string ActivityCountDescription { get; set; }
    }
}