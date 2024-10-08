// <copyright file="ActivityDetailTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Activity detail transfer model.
    /// </summary>
    [Table("tActivityDetailTransfer")]
    public class ActivityDetailTransferModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
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
