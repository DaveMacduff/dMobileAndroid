// <copyright file="PickingItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class PickingItem
    {
        private int productRefno;
        private string productUnitSizeDescription;
        private decimal activityItemPickingWeightQuantityShipped;
        private int locationCode;
        private decimal activityItemPickingWeightQuantityAvailable;
        private bool activityItemPickingIsReviewed;

        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get => this.productRefno; set => this.productRefno = value; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductBrandDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get => this.productUnitSizeDescription; set => this.productUnitSizeDescription = value; }
        public decimal ActivityItemPickingQuantityOrdered { get; set; }
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }
        public decimal ActivityItemPickingQuantityShipped { get; set; }
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }
        public decimal ActivityItemPickingWeightQuantityShipped { get => this.activityItemPickingWeightQuantityShipped; set => this.activityItemPickingWeightQuantityShipped = value; }
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get => this.locationCode; set => this.locationCode = value; }
        public string LocationName { get; set; }
        public decimal ActivityItemPickingQuantityAvailable { get; set; }
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }
        public decimal ActivityItemPickingWeightQuantityAvailable { get => this.activityItemPickingWeightQuantityAvailable; set => this.activityItemPickingWeightQuantityAvailable = value; }
        public string IsActivityItemPickingComment { get; set; }
        public int ActivityItemStatus { get; set; }
        public bool ActivityItemPickingIsReviewed { get => this.activityItemPickingIsReviewed; set => this.activityItemPickingIsReviewed = value; }
    }
}
