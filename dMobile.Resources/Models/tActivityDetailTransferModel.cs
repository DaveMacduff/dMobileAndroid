// <copyright file="tActivityDetailTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailTransfer")]
    public class tActivityDetailTransferModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailFKRefnoAndTransModeEnum", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailTransferIsConfirmed { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailFKRefnoAndTransModeEnum", Order = 2, Unique = true)]
        public int ActivityDetailTransferTransModeEnum { get; set; }
    }
}
