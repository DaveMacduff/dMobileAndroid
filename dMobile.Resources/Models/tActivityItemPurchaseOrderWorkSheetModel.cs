// <copyright file="tActivityItemPurchaseOrderWorkSheetModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemPurchaseOrderWorkSheet")]
    public class tActivityItemPurchaseOrderWorkSheetModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityItemPurchaseOrderWorkSheetOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get; set; }
    }
}
