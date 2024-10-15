// <copyright file="ActivityDetailReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a receiving detail activity.
    /// </summary>
    [Table("tActivityDetailReceiving")]
    public class ActivityDetailReceivingModel
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
        [Indexed(Name = "UQ_ActivityDetailReceivingOnActivityDetailFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityDetailFKRefno { get; set; }
    }
}