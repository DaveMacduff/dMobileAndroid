// <copyright file="ActivityItemReceivingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Items
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a comment on an activity item receiving.
    /// </summary>
    [Table("tActivityItemReceivingComment")]
    public class ActivityItemReceivingCommentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemReceivingModel))]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the comment for the activity item receiving.
        /// </summary>
        [NotNull]
        public string ActivityItemReceivingCommentComment { get; set; }
    }
}