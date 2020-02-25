using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.View.Types
{
    class Count
    {
        private int activityRefno;
        private int activitySubType;
        private int activityCountFKRefno;
        private string activityCountStringFKRefno;
        private int warehouseLocationMapRefno;
        private int activityCountType;
        private DateTime activityCountDateTime;
        private string activityCountTypeDescription;
        private string activityCountDescription;

        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivitySubType { get => activitySubType; set => activitySubType = value; }
        public int ActivityCountFKRefno { get => activityCountFKRefno; set => activityCountFKRefno = value; }
        public string ActivityCountStringFKRefno { get => activityCountStringFKRefno; set => activityCountStringFKRefno = value; }
        public int WarehouseLocationMapRefno { get => warehouseLocationMapRefno; set => warehouseLocationMapRefno = value; }
        public int ActivityCountType { get => activityCountType; set => activityCountType = value; }
        public DateTime ActivityCountDateTime { get => activityCountDateTime; set => activityCountDateTime = value; }
        public string ActivityCountTypeDescription { get => activityCountTypeDescription; set => activityCountTypeDescription = value; }
        public string ActivityCountDescription { get => activityCountDescription; set => activityCountDescription = value; }
    }
}
