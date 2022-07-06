// <copyright file="ReceiveItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReceiveItem
    {
        private int ActivityItemFKRefno;
        private int productRefno;
        private string productUnitSizeDescription;
        private decimal activityItemReceivingWeightQuantityReceived;
        private string activityItemReceivingDescription;
        private int locationCode;
        private decimal activityItemReceivingWeightQuantityReceivedToDate;

        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get => this.ActivityItemFKRefno; set => this.ActivityItemFKRefno = value; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get => this.productRefno; set => this.productRefno = value; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get => this.productUnitSizeDescription; set => this.productUnitSizeDescription = value; }
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingQuantityReceived { get; set; }
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }
        public decimal ActivityItemReceivingWeightQuantityReceived { get => this.activityItemReceivingWeightQuantityReceived; set => this.activityItemReceivingWeightQuantityReceived = value; }
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }
        public string ActivityItemReceivingDescription { get => this.activityItemReceivingDescription; set => this.activityItemReceivingDescription = value; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get => this.locationCode; set => this.locationCode = value; }
        public string LocationName { get; set; }
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get => this.activityItemReceivingWeightQuantityReceivedToDate; set => this.activityItemReceivingWeightQuantityReceivedToDate = value; }
        public string IsActivityItemReceivingComment { get; set; }
    }
}
