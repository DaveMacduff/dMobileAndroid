// <copyright file="ActivityItemReturnsCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Items
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a comment on an activity item return.
    /// </summary>
    [Table("tActivityItemReturnsComment")]
    public class ActivityItemReturnsCommentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemReturnsModel))]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the comment for the activity item return.
        /// </summary>
        [NotNull]
        public string ActivityItemReturnsCommentComment { get; set; }
    }
}