// <copyright file="Transfer.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Transfer
    {
        private int activityStatus;
        private string locationMapWarehouseName;
        private DateTime activityTransferDateTime;

        public int ActivityRefno { get; set; }
        public int ActivityTransferFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public int ActivityStatus { get => this.activityStatus; set => this.activityStatus = value; }
        public string ActivityTransferStringFKRefno { get; set; }
        public string ActivityTransferDescription { get; set; }
        public string LocationMapWarehouseName { get => this.locationMapWarehouseName; set => this.locationMapWarehouseName = value; }
        public DateTime ActivityTransferArrivalDateTime { get; set; }
        public DateTime ActivityTransferDateTime { get => this.activityTransferDateTime; set => this.activityTransferDateTime = value; }
    }
}
