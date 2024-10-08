// <copyright file="ActivityDetailProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailProductionByProduct")]
    public class ActivityDetailProductionByProductModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailProductionByProductOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }
    }
}
