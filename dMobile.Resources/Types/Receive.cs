// <copyright file="Receive.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Receive
    {
        private int activityReceivingFKRefno;
        private string activityReceivingPONumber;
        private DateTime activityReceivingArrivalDateTime;
        private string activityReceivingVendorSupplierGTINCompanyPrefix;

        public int ActivityRefno { get; set; }
        public int ActivitySubType { get; set; }
        public int ActivityReceivingFKRefno { get => this.activityReceivingFKRefno; set => this.activityReceivingFKRefno = value; }
        public string ActivityReceivingStringFKRefno { get; set; }
        public string ActivityReceivingPONumber { get => this.activityReceivingPONumber; set => this.activityReceivingPONumber = value; }
        public int WarehouseLocationMapRefno { get; set; }
        public DateTime ActivityReceivingIssuedDateTime { get; set; }
        public DateTime ActivityReceivingArrivalDateTime { get => this.activityReceivingArrivalDateTime; set => this.activityReceivingArrivalDateTime = value; }
        public string ActivityReceivingVendorContactName { get; set; }
        public string ActivityReceivingVendorSupplierContactName { get; set; }
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get => this.activityReceivingVendorSupplierGTINCompanyPrefix; set => this.activityReceivingVendorSupplierGTINCompanyPrefix = value; }
    }
}
