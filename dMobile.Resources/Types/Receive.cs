// <copyright file="Receive.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Receive
    {
        public int ActivityRefno { get; set; }
        public int ActivitySubType { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public string ActivityReceivingPONumber { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public DateTime ActivityReceivingIssuedDateTime { get; set; }
        public DateTime ActivityReceivingArrivalDateTime { get; set; }
        public string ActivityReceivingVendorContactName { get; set; }
        public string ActivityReceivingVendorSupplierContactName { get; set; }
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get; set; }
    }
}
