// <copyright file="ActivityTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityTransfer")]
    public class ActivityTransferModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityTransferOnActivityFKRefno", Order = 1, Unique = false)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        public int TransferWarehouseLocationMapFKRefno { get; set; }

        [NotNull]
        public string TransferWarehouseLocationMapDescription { get; set; }

        [NotNull]
        public string ActivityTransferDescription { get; set; }

        [NotNull]
        public DateTime ActivityTransferDateTime { get; set; }

        [NotNull]
        public DateTime ActivityTransferArrivalDateTime { get; set; }
    }
}
