// <copyright file="ActivityItemReceivingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemReceivingComment")]
    public class ActivityItemReceivingCommentModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemReceivingModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemReceivingCommentComment { get; set; }
    }
}
