// <copyright file="tActivityItemProductionModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemProduction")]
    public class tActivityItemProductionModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemProductionOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        [Indexed(Name = "FK_ActivityItemProductionOnLocationMapRefno", Order = 2, Unique = false)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemProductionQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemProductionBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemProductionWeightQuantity { get; set; }

        [NotNull]
        public string ActivityItemProductionDescription { get; set; }

        [NotNull]
        public decimal ActivityItemProductionQuantityExpected { get; set; }

        [NotNull]
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }

        [NotNull]
        public decimal ActivityItemProductionWeightQuantityExpected { get; set; }
    }
}
