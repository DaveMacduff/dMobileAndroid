// <copyright file="Return.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Return
    {
        private DateTime activityReturnsDeliveryDateTime;
        private int locationMapWarehouseFKRefno;
        private string activityReturnsCustomerShipToInfo;

        public int ActivityRefno { get; set; }
        public int ActivityReturnsFKRefno { get; set; }
        public string ActivityReturnsStringFKRefno { get; set; }
        public DateTime ActivityReturnsOrderedDateTime { get; set; }
        public DateTime ActivityReturnsDeliveryDateTime { get => this.activityReturnsDeliveryDateTime; set => this.activityReturnsDeliveryDateTime = value; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get => this.locationMapWarehouseFKRefno; set => this.locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseName { get; set; }
        public string ActivityReturnsCustomerContactName { get; set; }
        public string ActivityReturnsCustomerShipToInfo { get => this.activityReturnsCustomerShipToInfo; set => this.activityReturnsCustomerShipToInfo = value; }
        public int ActivityStatus { get; set; }
    }
}
