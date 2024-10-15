// <copyright file="ActivityDetailCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a count detail activity.
    /// </summary>
    [Table("tActivityDetailCount")]
    public class ActivityDetailCountModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity detail.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity detail.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailCountOnActivityDetailFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityDetailFKRefno { get; set; }
    }
}