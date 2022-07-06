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
        public int ActivityItemRefno { get; set; }
        public int ActivityItemFKRefno { get; set; }
        public string ActivityItemStringFKRefno { get; set; }
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string ProductUnitBC { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }
        public decimal ActivityItemReceivingQuantityReceived { get; set; }
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }
        public decimal ActivityItemReceivingWeightQuantityReceived { get; set; }
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }
        public string ActivityItemReceivingDescription { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationCode { get; set; }
        public string LocationName { get; set; }
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get; set; }
        public string IsActivityItemReceivingComment { get; set; }
    }
}
