// <copyright file="Picking.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Headers
{
    using System;

    public class Picking
    {
        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public DateTime ActivityPickingOrderedDateTime { get; set; }
        public DateTime ActivityPickingDeliveryDateTime { get; set; }
        public string ActivityPickingCustomerContactName { get; set; }
        public string ActivityPickingCustomerShipToInfo { get; set; }
        public string RouteCode { get; set; }
        public string RouteDescription { get; set; }
        public string RouteDayOfWeek { get; set; }
        public int RouteStop { get; set; }
        public int ActivityStatus { get; set; }
        public bool ActivityPickingIsReviewed { get; set; }
    }
}
