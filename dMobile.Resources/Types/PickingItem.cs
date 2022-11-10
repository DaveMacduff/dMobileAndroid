// <copyright file="PickingItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class PickingItem
    {
        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductBrandDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public decimal ActivityItemPickingQuantityOrdered { get; set; }
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }
        public decimal ActivityItemPickingQuantityShipped { get; set; }
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }
        public decimal ActivityItemPickingWeightQuantityShipped { get; set; }
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public decimal ActivityItemPickingQuantityAvailable { get; set; }
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }
        public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }
        public string IsActivityItemPickingComment { get; set; }
        public int ActivityItemStatus { get; set; }
        public bool ActivityItemPickingIsReviewed { get; set; }
    }
}
