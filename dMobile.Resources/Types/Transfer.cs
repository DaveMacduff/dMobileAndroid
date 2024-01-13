// <copyright file="Transfer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Transfer
    {
        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseDescription { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public DateTime ActivityTransferArrivalDateTime { get; set; }
        public string ActivityTransferDescription { get; set; }
        public DateTime ActivityTransferDateTime { get; set; }
        public int ActivityStatus { get; set; }
    }
}
