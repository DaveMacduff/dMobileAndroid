// <copyright file="ActivityItemModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity item.
    /// </summary>
    [Table("tActivityItem")]
    public class ActivityItemModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the activity item.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity item.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity item.
        /// </summary>
        [NotNull]
        public int ActivityItemStatus { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the associated activity.
        /// </summary>
        [Indexed(Name = "FK_ActivityItemOnActivityRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity item is added.
        /// </summary>
        [NotNull]
        public bool ActivityItemIsAdded { get; set; } = false;
    }
}