// <copyright file="ActivityProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityProductionByProduct")]
    public class ActivityProductionByProductModel
    {
        [NotNull]
        [ForeignKey(typeof(ActivityProductionModel))]
        [Indexed(Name = "PK_ActivityProductionByProductOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityProductionByProductOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }
    }
}
