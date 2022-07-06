// <copyright file="PurchaseOrderWorkSheetItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class PurchaseOrderWorkSheetItem
    {
        public int ActivityItemRefno { get; set; }
        public int ActivityRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get; set; }
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get; set; }
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ActivityItemStatus { get; set; }
    }
}
