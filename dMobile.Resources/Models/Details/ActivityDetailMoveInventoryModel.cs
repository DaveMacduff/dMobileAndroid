// <copyright file="ActivityDetailMoveInventoryModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Details
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a move inventory detail activity.
    /// </summary>
    [Table("tActivityDetailMoveInventory")]
    public class ActivityDetailMoveInventoryModel
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
        [Indexed(Name = "UQ_ActivityDetailMoveInventoryOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }
    }
}