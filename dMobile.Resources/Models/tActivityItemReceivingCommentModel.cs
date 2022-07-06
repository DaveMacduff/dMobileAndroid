// <copyright file="tActivityItemReceivingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemReceivingComment")]
    public class tActivityItemReceivingCommentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemReceivingModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemReceivingCommentComment { get; set; }
    }
}
