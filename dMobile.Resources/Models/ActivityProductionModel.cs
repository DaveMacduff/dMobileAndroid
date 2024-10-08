// <copyright file="ActivityProductionModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityProduction")]
    public class ActivityProductionModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityProductionOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityProductionOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityProductionOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityProductionDateTime { get; set; }

        [NotNull]
        public string ActivityProductionDescription { get; set; }
    }
}
