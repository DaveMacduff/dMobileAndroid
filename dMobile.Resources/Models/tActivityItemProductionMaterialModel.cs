// <copyright file="tActivityItemProductionMaterialModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemProductionMaterial")]
    public class tActivityItemProductionMaterialModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionMaterialOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }
    }
}
