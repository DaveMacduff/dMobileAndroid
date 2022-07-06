// <copyright file="Picking.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Picking
    {
        private int locationMapWarehouseFKRefno;
        private DateTime activityPickingDeliveryDateTime;
        private string routeDayOfWeek;
        private int activityStatus;

        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapWarehouseFKRefno { get => this.locationMapWarehouseFKRefno; set => this.locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseName { get; set; }
        public DateTime ActivityPickingOrderedDateTime { get; set; }
        public DateTime ActivityPickingDeliveryDateTime { get => this.activityPickingDeliveryDateTime; set => this.activityPickingDeliveryDateTime = value; }
        public string ActivityPickingCustomerContactName { get; set; }
        public string ActivityPickingCustomerShipToInfo { get; set; }
        public string RouteCode { get; set; }
        public string RouteDescription { get; set; }
        public string RouteDayOfWeek { get => this.routeDayOfWeek; set => this.routeDayOfWeek = value; }
        public int RouteStop { get; set; }
        public int ActivityStatus { get => this.activityStatus; set => this.activityStatus = value; }
        public bool ActivityPickingIsReviewed { get; set; }
    }
}
