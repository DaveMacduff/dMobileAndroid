using System;

namespace dMobile.Resources.Types
{
    public class Transfer
    {
        //"tActivityTransfer.ActivityRefno AS ActivityRefno,    int"
        //"tActivityTransfer.ActivityTransferFKRefno AS ActivityTransferFKRefno,     int"
        //"tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,      int"
        //"tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,      int"
        //"tActivity.ActivityStatus AS ActivityStatus      int"
        //"tActivityTransfer.ActivityTransferStringFKRefno AS ActivityTransferStringFKRefno,     string"
        //"tActivityTransfer.ActivityTransferDescription AS ActivityTransferDescription,      string"
        //"tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,    string"
        //"tActivityTransfer.ActivityTransferArrivalDateTime AS ActivityTransferArrivalDateTime,   datetime"
        //"tActivityTransfer.ActivityTransferDateTime AS ActivityTransferDateTime,     datetime"

        private int activityRefno;
        private int activityTransferFKRefno;
        private int locationMapRefno;
        private int locationMapWarehouseFKRefno;
        private int activityStatus;
        private string activityTransferStringFKRefno;
        private string activityTransferDescription;
        private string locationMapWarehouseName;
        private DateTime activityTransferArrivalDateTime;
        private DateTime activityTransferDateTime;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityTransferFKRefno { get => activityTransferFKRefno; set => activityTransferFKRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
        public string ActivityTransferStringFKRefno { get => activityTransferStringFKRefno; set => activityTransferStringFKRefno = value; }
        public string ActivityTransferDescription { get => activityTransferDescription; set => activityTransferDescription = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public DateTime ActivityTransferArrivalDateTime { get => activityTransferArrivalDateTime; set => activityTransferArrivalDateTime = value; }
        public DateTime ActivityTransferDateTime { get => activityTransferDateTime; set => activityTransferDateTime = value; }
    }
}
