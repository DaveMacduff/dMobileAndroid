// <copyright file="ActivityItemTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemTransfer")]
    public class ActivityItemTransferModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemRefno", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemSubType", Order = 1, Unique = false)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemTransferOnActivityItemFKRefnoAndActivitySubType", Order = 1, Unique = false)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(ProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int TransferFromWarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemTransferRequiredQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferWeightQuantity { get; set; }
    }
}
