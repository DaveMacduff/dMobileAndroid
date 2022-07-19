// <copyright file="tActivityProductionFinishedModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityProductionFinished")]
    public class tActivityProductionFinishedModel
    {
        [NotNull]
        [ForeignKey(typeof(tActivityProductionModel))]
        [Indexed(Name = "PK_ActivityProductionFinishedOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityProductionFinishedOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        public string ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber { get; set; }
    }
}
