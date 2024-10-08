// <copyright file="ActivityItemReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemReceiving")]
    public class ActivityItemReceivingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityReceived { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceived { get; set; }

        [NotNull]
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }

        [NotNull]
        public string ActivityItemReceivingDescription { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceivedToDate { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get; set; }
    }
}
