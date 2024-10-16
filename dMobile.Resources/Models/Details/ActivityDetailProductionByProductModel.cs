// <copyright file="ActivityDetailProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Details
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a production by-product detail activity.
    /// </summary>
    [Table("tActivityDetailProductionByProduct")]
    public class ActivityDetailProductionByProductModel
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
        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailProductionByProductOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }
    }
}