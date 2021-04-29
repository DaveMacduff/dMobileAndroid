using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class Return
    {
        // tActivityReturns.ActivityReturnsFKRefno int
        // tActivityReturns.ActivityReturnsStringFKRefno string
        // tActivityReturns.ActivityReturnsOrderedDateTime DATETIME
        // tActivityReturns.ActivityReturnsDeliveryDateTime DATETIME
        // tLocationMapWarehouse.LocationMapRefno int
        // tLocationMapWarehouse.LocationMapWarehouseFKRefno int
        // tLocationMapWarehouse.LocationMapWarehouseName string
        // tActivityReturns.ActivityReturnsCustomerContactName string
        // tActivityReturns.ActivityReturnsCustomerShipToInfo string
        // tActivity.ActivityStatus int

        //sql.Append("SELECT tActivityReturns.ActivityRefno AS ActivityRefno,");
        //        sql.Append("tActivityReturns.ActivityReturnsFKRefno AS ActivityReturnsFKRefno,");
        //        sql.Append("tActivityReturns.ActivityReturnsStringFKRefno AS ActivityReturnsStringFKRefno,");
        //        sql.Append("tActivityReturns.ActivityReturnsOrderedDateTime AS ActivityReturnsOrderedDateTime,");
        //        sql.Append("tActivityReturns.ActivityReturnsDeliveryDateTime AS ActivityReturnsDeliveryDateTime,");
        //        sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
        //        sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
        //        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
        //        sql.Append("tActivityReturns.ActivityReturnsCustomerContactName AS ActivityReturnsCustomerContactName,");
        //        sql.Append("tActivityReturns.ActivityReturnsCustomerShipToInfo AS ActivityReturnsCustomerShipToInfo,");
        //        sql.Append("tActivity.ActivityStatus AS ActivityStatus");

        private int activityRefno;
        private int activityReturnsFKRefno;
        private string activityReturnsStringFKRefno;
        private DateTime activityReturnsOrderedDateTime;
        private DateTime activityReturnsDeliveryDateTime;
        private int locationMapRefno;
        private int locationMapWarehouseFKRefno;
        private string locationMapWarehouseName;
        private string activityReturnsCustomerContactName;
        private string activityReturnsCustomerShipToInfo;
        private int activityStatus;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityReturnsFKRefno { get => activityReturnsFKRefno; set => activityReturnsFKRefno = value; }
        public string ActivityReturnsStringFKRefno { get => activityReturnsStringFKRefno; set => activityReturnsStringFKRefno = value; }
        public DateTime ActivityReturnsOrderedDateTime { get => activityReturnsOrderedDateTime; set => activityReturnsOrderedDateTime = value; }
        public DateTime ActivityReturnsDeliveryDateTime { get => activityReturnsDeliveryDateTime; set => activityReturnsDeliveryDateTime = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public string ActivityReturnsCustomerContactName { get => activityReturnsCustomerContactName; set => activityReturnsCustomerContactName = value; }
        public string ActivityReturnsCustomerShipToInfo { get => activityReturnsCustomerShipToInfo; set => activityReturnsCustomerShipToInfo = value; }
        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
    }
}
