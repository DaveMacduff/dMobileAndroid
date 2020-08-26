using System;

namespace dMobile.Resources.Types
{
    public class ProductionTicketFinished
    {
        private int activityStatus;
        private int activityRefno;
        private int activityProductionFKRefno;
        private int locationMapRefno;
        private int locationMapWarehouseFKRefno;
        private DateTime activityProductionDateTime;
        private string activityProductionDescription;
        private string activityProductionStringFKRefno;
        private string locationMapWarehouseName;

        public int ActivityStatus { get => activityStatus; set => activityStatus = value; }
        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityProductionFKRefno { get => activityProductionFKRefno; set => activityProductionFKRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public DateTime ActivityProductionDateTime { get => activityProductionDateTime; set => activityProductionDateTime = value; }
        public string ActivityProductionDescription { get => activityProductionDescription; set => activityProductionDescription = value; }
        public string ActivityProductionStringFKRefno { get => activityProductionStringFKRefno; set => activityProductionStringFKRefno = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
    }
}
