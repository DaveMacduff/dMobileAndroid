// <copyright file="ActivityItemPickingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a comment on a picking activity item.
    /// </summary>
    [Table("tActivityItemPickingComment")]
    public class ActivityItemPickingCommentModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemPickingModel))]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the comment on the picking activity item.
        /// </summary>
        [NotNull]
        public string ActivityItemPickingCommentComment { get; set; }
    }
}