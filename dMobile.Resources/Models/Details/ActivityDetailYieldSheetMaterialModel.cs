// <copyright file="ActivityDetailYieldSheetMaterialModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable UnusedMember.Global
namespace dMobile.Resources.Models.Details
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a cut sheet material detail activity.
    /// </summary>
    [Table("tActivityDetailYieldSheetMaterial")]
    public class ActivityDetailYieldSheetMaterialModel
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
        [Indexed(Name = "UQ_ActivityDetailYieldSheetMaterialOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }
    }
}