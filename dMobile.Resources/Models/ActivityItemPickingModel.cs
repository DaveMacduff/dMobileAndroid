// <copyright file="ActivityItemPickingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemPicking")]
    public class ActivityItemPickingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemPickingOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityShipped { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityShipped { get; set; }

        [NotNull]
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }

        [NotNull]
        public string ActivityItemPickingDescription { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityAvailable { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }

        [NotNull]
        public bool ActivityItemPickingIsReviewed { get; set; }
    }
}
