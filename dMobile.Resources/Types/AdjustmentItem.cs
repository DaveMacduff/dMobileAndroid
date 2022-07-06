// <copyright file="AdjustmentItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AdjustmentItem
    {
        private int activityItemStatus;
        private decimal activityItemAdjustmentWeightQuantity;
        private string locationName;

        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public int ActivityItemStatus { get => this.activityItemStatus; set => this.activityItemStatus = value; }
        public decimal ActivityItemAdjustmentQuantity { get; set; }
        public decimal ActivityItemAdjustmentBaseQuantity { get; set; }
        public decimal ActivityItemAdjustmentWeightQuantity { get => this.activityItemAdjustmentWeightQuantity; set => this.activityItemAdjustmentWeightQuantity = value; }
        public string ActivityItemStringFKRefno { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string LocationName { get => this.locationName; set => this.locationName = value; }
    }
}
