// <copyright file="tActivityDetailReturnsModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailReturns")]
    public class tActivityDetailReturnsModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailReturnsOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailReturnsIsConfirmed { get; set; }
    }
}
