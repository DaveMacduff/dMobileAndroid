// <copyright file="ActivityItemAdjustmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemAdjustment")]
    public class ActivityItemAdjustmentModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemAdjustmentOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentWeightQuantity { get; set; }

        [NotNull]
        public string ActivityItemAdjustmentDescription { get; set; }
    }
}