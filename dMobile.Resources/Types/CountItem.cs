// <copyright file="CountItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class CountItem
    {
        private int activityItemCountFKRefno;
        private int locationMapRefno;
        private decimal activityItemCountWeightQuantity;
        private string locationName;

        public int ActivityItemRefno { get; set; }
        public int ActivityItemCountFKRefno { get => this.activityItemCountFKRefno; set => this.activityItemCountFKRefno = value; }
        public string ActivityItemCountStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int LocationMapRefno { get => this.locationMapRefno; set => this.locationMapRefno = value; }
        public decimal ActivityItemCountQuantityOnHand { get; set; }
        public decimal ActivityItemCountBaseQuantityOnHand { get; set; }
        public decimal ActivityItemCountWeightQuantityOnHand { get; set; }
        public decimal ActivityItemCountQuantity { get; set; }
        public decimal ActivityItemCountBaseQuantity { get; set; }
        public decimal ActivityItemCountWeightQuantity { get => this.activityItemCountWeightQuantity; set => this.activityItemCountWeightQuantity = value; }
        public int ProductRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get => this.locationName; set => this.locationName = value; }
        public int ActivityItemStatus { get; set; }
    }
}
