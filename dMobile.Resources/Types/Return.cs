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
        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public DateTime ActivityReturnsOrderedDateTime { get; set; }
        public DateTime ActivityReturnsDeliveryDateTime { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public string ActivityReturnsCustomerContactName { get; set; }
        public string ActivityReturnsCustomerShipToInfo { get; set; }
        public int ActivityStatus { get; set; }
    }
}
