using System;

namespace dMobile.Resources.Types
{
    public class Adjustment: object
    {
        private int activityRefno;
        private int activityAdjustmentFKRefno;
        private int activityAdjustmentType;
        private int locationMapWarehouseFKRefno;
        private int locationMapRefno;
        private int activityStatus;
        private string activityAdjustmentStringFKRefno;
        private string activityAdjustmentDescription;
        private string activityAdjustmentTypeDescription;
        private string locationMapWarehouseName;
        private DateTime activityAdjustmentDateTime;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityAdjustmentFKRefno { get => activityAdjustmentFKRefno; set => activityAdjustmentFKRefno = value; }
        public int ActivityAdjustmentType { get => activityAdjustmentType; set => activityAdjustmentType = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
        public string ActivityAdjustmentStringFKRefno { get => activityAdjustmentStringFKRefno; set => activityAdjustmentStringFKRefno = value; }
        public string ActivityAdjustmentDescription { get => activityAdjustmentDescription; set => activityAdjustmentDescription = value; }
        public string ActivityAdjustmentTypeDescription { get => activityAdjustmentTypeDescription; set => activityAdjustmentTypeDescription = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public DateTime ActivityAdjustmentDateTime { get => activityAdjustmentDateTime; set => activityAdjustmentDateTime = value; }
    }
}
