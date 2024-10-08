// <copyright file="ActivityItemReturnsCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemReturnsComment")]
    public class ActivityItemReturnsCommentModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemReturnsModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemReturnsCommentComment { get; set; }
    }
}
