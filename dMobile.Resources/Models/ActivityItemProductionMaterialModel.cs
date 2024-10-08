// <copyright file="ActivityItemProductionMaterialModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemProductionMaterial")]
    public class ActivityItemProductionMaterialModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemProductionMaterialOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }
    }
}
