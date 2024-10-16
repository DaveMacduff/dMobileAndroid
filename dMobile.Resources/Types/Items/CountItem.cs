// <copyright file="CountItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Items
{
    public class CountItem
    {
        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public decimal ActivityItemCountQuantityOnHand { get; set; }
        public decimal ActivityItemCountBaseQuantityOnHand { get; set; }
        public decimal ActivityItemCountWeightQuantityOnHand { get; set; }
        public decimal ActivityItemCountQuantity { get; set; }
        public decimal ActivityItemCountBaseQuantity { get; set; }
        public decimal ActivityItemCountWeightQuantity { get; set; }
        public int ProductRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ActivityItemStatus { get; set; }
    }
}
