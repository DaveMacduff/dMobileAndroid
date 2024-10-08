// <copyright file="ActivityDetailReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailReceiving")]
    public class ActivityDetailReceivingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailReceivingOnActivityDetailFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityDetailFKRefno { get; set; }
    }
}
