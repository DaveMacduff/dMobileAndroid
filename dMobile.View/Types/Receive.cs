using System;

namespace dMobile.View.Types
{
    class Receive
    {
        private int activityRefno;
        private int activitySubType;
        private int activityReceivingFKRefno;
        private string activityReceivingStringFKRefno;
        private string activityReceivingPONumber;
        private int warehouseLocationMapRefno;
        private DateTime activityReceivingIssuedDateTime;
        private DateTime activityReceivingArrivalDateTime;
        private string activityReceivingVendorContactName;
        private string activityReceivingVendorSupplierContactName;
        private string activityReceivingVendorSupplierGTINCompanyPrefix;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivitySubType { get => activitySubType; set => activitySubType = value; }
        public int ActivityReceivingFKRefno { get => activityReceivingFKRefno; set => activityReceivingFKRefno = value; }
        public string ActivityReceivingStringFKRefno { get => activityReceivingStringFKRefno; set => activityReceivingStringFKRefno = value; }
        public string ActivityReceivingPONumber { get => activityReceivingPONumber; set => activityReceivingPONumber = value; }
        public int WarehouseLocationMapRefno { get => warehouseLocationMapRefno; set => warehouseLocationMapRefno = value; }
        public DateTime ActivityReceivingIssuedDateTime { get => activityReceivingIssuedDateTime; set => activityReceivingIssuedDateTime = value; }
        public DateTime ActivityReceivingArrivalDateTime { get => activityReceivingArrivalDateTime; set => activityReceivingArrivalDateTime = value; }
        public string ActivityReceivingVendorContactName { get => activityReceivingVendorContactName; set => activityReceivingVendorContactName = value; }
        public string ActivityReceivingVendorSupplierContactName { get => activityReceivingVendorSupplierContactName; set => activityReceivingVendorSupplierContactName = value; }
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get => activityReceivingVendorSupplierGTINCompanyPrefix; set => activityReceivingVendorSupplierGTINCompanyPrefix = value; }
    }
}
