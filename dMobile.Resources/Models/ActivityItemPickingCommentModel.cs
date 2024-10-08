// <copyright file="ActivityItemPickingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemPickingComment")]
    public class ActivityItemPickingCommentModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemPickingModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemPickingCommentComment { get; set; }
    }
}
