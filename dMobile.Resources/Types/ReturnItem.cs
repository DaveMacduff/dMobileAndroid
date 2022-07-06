// <copyright file="ReturnItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class ReturnItem
    {
        private int productRefno;
        private string productUnitSizeDescription;
        private decimal activityItemReturnsWeightQuantityReturned;
        private int locationCode;
        private string isActivityItemReturnsComment;

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
        public decimal ActivityItemReturnsQuantityReturned { get; set; }
        public decimal ActivityItemReturnsBaseQuantityReturned { get; set; }
        public decimal ActivityItemReturnsWeightQuantityReturned { get => this.activityItemReturnsWeightQuantityReturned; set => this.activityItemReturnsWeightQuantityReturned = value; }
        public string ActivityItemReturnsCustomerProductUnitCode { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get => this.locationCode; set => this.locationCode = value; }
        public string LocationName { get; set; }
        public string IsActivityItemReturnsComment { get => this.isActivityItemReturnsComment; set => this.isActivityItemReturnsComment = value; }
        public int ActivityItemStatus { get; set; }
    }
}
