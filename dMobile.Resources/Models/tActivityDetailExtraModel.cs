// <copyright file="tActivityDetailExtraModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailExtra")]
    public class tActivityDetailExtraModel
    {
        [PrimaryKey]
        [Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 1, Unique = true)]
        [NotNull]
        [ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivityDetailSubType { get; set; }

        [NotNull]
        public string ActivityDetailExtra1Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra2Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra3Text { get; set; }

        [NotNull]
        public string ActivityDetailExtra4Text { get; set; }

        [NotNull]
        public DateTime ActivityDetailExtra5Date { get; set; }
    }
}
