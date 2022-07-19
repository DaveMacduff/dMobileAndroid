// <copyright file="tActivityAdjustmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityAdjustment")]
    public class tActivityAdjustmentModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityModel))]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityAdjustmentOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        public int ActivityAdjustmentType { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityAdjustmentOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityAdjustmentDateTime { get; set; }

        [NotNull]
        public string ActivityAdjustmentDescription { get; set; }

        [NotNull]
        public string ActivityAdjustmentTypeDescription { get; set; }
    }
}
