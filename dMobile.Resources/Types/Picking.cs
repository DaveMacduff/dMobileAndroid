using System;

namespace dMobile.Resources.Types
{
    public class Picking
    {
        // tActivityPicking.ActivityRefno INT
        // tActivityPicking.ActivityPickingFKRefno INT
        // tActivityPicking.ActivityPickingStringFKRefno STRING
        // tLocationMapWarehouse.LocationMapRefno INT
        // tLocationMapWarehouse.LocationMapWarehouseFKRefno INT
        // tLocationMapWarehouse.LocationMapWarehouseName STRING
        // tActivityPicking.ActivityPickingOrderedDateTime DATETIME
        // tActivityPicking.ActivityPickingDeliveryDateTime DATETIME
        // tActivityPicking.ActivityPickingCustomerContactName STRING
        // tActivityPicking.ActivityPickingCustomerShipToInfo STRING
        // tRoute.RouteCode STRING
        // tRoute.RouteDescription STRING
        // tRoute.RouteDayOfWeek STRING
        // tActivityPickingRoute.RouteStop INT
        // tActivity.ActivityStatus INT
        // tActivityPicking.ActivityPickingIsReviewed BOOL
        
        private int activityRefno;
        private int activityPickingFKRefno;
        private string activityPickingStringFKRefno;
        private int locationMapRefno;
        private int locationMapWarehouseFKRefno;
        private string locationMapWarehouseName;
        private DateTime activityPickingOrderedDateTime;
        private DateTime activityPickingDeliveryDateTime;
        private string activityPickingCustomerContactName;
        private string activityPickingCustomerShipToInfo;
        private string routeCode;
        private string routeDescription;
        private string routeDayOfWeek;
        private int routeStop;
        private int activityStatus;
        private bool activityPickingIsReviewed;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityPickingFKRefno { get => activityPickingFKRefno; set => activityPickingFKRefno = value; }
        public string ActivityPickingStringFKRefno { get => activityPickingStringFKRefno; set => activityPickingStringFKRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public DateTime ActivityPickingOrderedDateTime { get => activityPickingOrderedDateTime; set => activityPickingOrderedDateTime = value; }
        public DateTime ActivityPickingDeliveryDateTime { get => activityPickingDeliveryDateTime; set => activityPickingDeliveryDateTime = value; }
        public string ActivityPickingCustomerContactName { get => activityPickingCustomerContactName; set => activityPickingCustomerContactName = value; }
        public string ActivityPickingCustomerShipToInfo { get => activityPickingCustomerShipToInfo; set => activityPickingCustomerShipToInfo = value; }
        public string RouteCode { get => routeCode; set => routeCode = value; }
        public string RouteDescription { get => routeDescription; set => routeDescription = value; }
        public string RouteDayOfWeek { get => routeDayOfWeek; set => routeDayOfWeek = value; }
        public int RouteStop { get => routeStop; set => routeStop = value; }
        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
        public bool ActivityPickingIsReviewed { get => activityPickingIsReviewed; set => activityPickingIsReviewed = value; }
    }
}
