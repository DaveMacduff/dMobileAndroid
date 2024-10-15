// <copyright file="ActivityDetailReplenishmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Table definition activity detail replenishment model.
    /// </summary>
    [Table("tActivityDetailReplenishment")]
    public class ActivityDetailReplenishmentModel
    {
        /// <summary>
        /// Gets or sets the activity detail reference number.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the activity detail reference number.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailReplenishmentOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity detail replenishment is confirmed.
        /// </summary>
        [NotNull]
        public bool ActivityDetailReplenishmentIsConfirmed { get; set; }
    }
}
