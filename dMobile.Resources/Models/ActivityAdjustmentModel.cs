// <copyright file="ActivityAdjustmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents the header model for an activity adjustment.
    /// </summary>
    [Table("tActivityAdjustment")]
    public class ActivityAdjustmentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityFKRefno", Order = 1, Unique = true)]

        // ReSharper disable once InconsistentNaming
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]

        // ReSharper disable once InconsistentNaming
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the type of the activity adjustment.
        /// </summary>
        [NotNull]
        public int ActivityAdjustmentType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityAdjustmentOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the activity adjustment.
        /// </summary>
        [NotNull]
        public DateTime ActivityAdjustmentDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the activity adjustment.
        /// </summary>
        [NotNull]
        public string ActivityAdjustmentDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of the activity adjustment type.
        /// </summary>
        [NotNull]
        public string ActivityAdjustmentTypeDescription { get; set; }
    }
}