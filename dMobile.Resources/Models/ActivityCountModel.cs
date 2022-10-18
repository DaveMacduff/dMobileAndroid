// <copyright file="ActivityCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityCount")]
    public class ActivityCountModel
    {
        [PrimaryKey]
        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityCountOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        [Indexed(Name = "FK_ActivityCountOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(tLocationMapWarehouseModel))]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public int ActivityCountType { get; set; }

        [NotNull]
        public DateTime ActivityCountDateTime { get; set; }

        [NotNull]
        public string ActivityCountTypeDescription { get; set; }

        [NotNull]
        public string ActivityCountDescription { get; set; }
    }
}
