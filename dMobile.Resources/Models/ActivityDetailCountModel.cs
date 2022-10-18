// <copyright file="ActivityDetailCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailCount")]
    public class ActivityDetailCountModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailCountOnActivityDetailFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityDetailFKRefno { get; set; }
    }
}
