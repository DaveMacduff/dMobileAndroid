// <copyright file="InventoryInquiryItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class InventoryInquiryItem
    {
        private string weightUnitCode;
        private decimal activityitemweightquantity;
        private string skidCode;

        public string Productcode { get; set; }
        public string Productunitbc { get; set; }
        public string UnitCode { get; set; }
        public string WeightUnitCode { get => this.weightUnitCode; set => this.weightUnitCode = value; }
        public decimal ActivityItemQuantity { get; set; }
        public decimal ActivityItemWeightQuantity { get => this.activityitemweightquantity; set => this.activityitemweightquantity = value; }
        public string ActivityDetailLogisticUnitProductionDate { get; set; }
        public string ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }
        public string LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapSectionFKRefno { get; set; }
        public string LocationMapSlotFKRefno { get; set; }
        public string SkidCode { get => this.skidCode; set => this.skidCode = value; }
    }
}
