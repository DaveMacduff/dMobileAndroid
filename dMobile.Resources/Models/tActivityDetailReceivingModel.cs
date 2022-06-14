// <copyright file="tActivityDetailReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailReceiving")]
    public class tActivityDetailReceivingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityDetailReceivingFKRefno { get; set; }
    }
}
