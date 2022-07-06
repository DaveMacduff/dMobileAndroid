// <copyright file="PurchaseOrderWorkSheetItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class PurchaseOrderWorkSheetItem
    {
        private int ActivityItemFKRefno;
        private int productRefno;
        private string productUnitSizeDescription;
        private decimal activityItemPurchaseOrderWorkSheetWeightQuantityOrdered;
        private int locationCode;
        private int activityItemStatus;

        public int ActivityItemRefno { get; set; }
        public int ActivityRefno { get; set; }
        public int ActivityItemFKRefno { get => this.ActivityItemFKRefno; set => this.ActivityItemFKRefno = value; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get => this.productRefno; set => this.productRefno = value; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitSizeDescription { get => this.productUnitSizeDescription; set => this.productUnitSizeDescription = value; }
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get; set; }
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get; set; }
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get => this.activityItemPurchaseOrderWorkSheetWeightQuantityOrdered; set => this.activityItemPurchaseOrderWorkSheetWeightQuantityOrdered = value; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get => this.locationCode; set => this.locationCode = value; }
        public string LocationName { get; set; }
        public int ActivityItemStatus { get => this.activityItemStatus; set => this.activityItemStatus = value; }
    }
}
