using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class ProductionTicketByProduct
    {
        //sql.Append("SELECT tActivityProduction.ActivityRefno AS ActivityRefno,");
        //sql.Append("tActivityProductionByProduct.ActivityProductionFKRefno AS ActivityProductionFKRefno,");
        //sql.Append("tActivityProductionByProduct.ActivityProductionStringFKRefno AS ActivityProductionStringFKRefno,");
        //sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
        //sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
        //sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
        //sql.Append("tActivityProduction.ActivityProductionDateTime AS ActivityProductionDateTime,");
        //sql.Append("tActivityProduction.ActivityProductionDescription AS ActivityProductionDescription,");
        //sql.Append("tActivity.ActivityStatus AS ActivityStatus ");

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
