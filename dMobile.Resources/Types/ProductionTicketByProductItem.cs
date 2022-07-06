// <copyright file="ProductionTicketByProductItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProductionTicketByProductItem
    {
        private int activityItemStatus;
        private decimal activityItemProductionWeightQuantityExpected;
        private string locationName;

        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public int ActivityItemStatus { get => this.activityItemStatus; set => this.activityItemStatus = value; }
        public decimal ActivityItemProductionQuantity { get; set; }
        public decimal ActivityItemProductionBaseQuantity { get; set; }
        public decimal ActivityItemProductionWeightQuantity { get; set; }
        public decimal ActivityItemProductionQuantityExpected { get; set; }
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }
        public decimal ActivityItemProductionWeightQuantityExpected { get => this.activityItemProductionWeightQuantityExpected; set => this.activityItemProductionWeightQuantityExpected = value; }
        public string ActivityItemStringFKRefno { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string BaseUnitCode { get; set; }
        public string LocationName { get => this.locationName; set => this.locationName = value; }
    }
}
