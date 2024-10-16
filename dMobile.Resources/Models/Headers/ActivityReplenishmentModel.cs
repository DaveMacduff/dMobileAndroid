// <copyright file="ActivityReplenishmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Headers
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity replenishment.
    /// </summary>
    [Table("tActivityReplenishment")]
    public class ActivityReplenishmentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [Indexed(Name = "UQ_ActivityReplenishmentOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReplenishmentOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        public int WarehouseLocationMapRefno { get; set; }
    }
}