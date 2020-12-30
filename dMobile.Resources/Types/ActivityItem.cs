using System;
namespace dMobile.Resources.Types
{
    public class ActivityItem
    {
        //DataTable dataTableItems = new DataTable("tActivityItem");

        //dataTableItems.Columns.Add("productcode", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("productunitbc", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("unitcode", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("weightunitcode", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("activitydetaillogisticunitproductiondate", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("activitydetaillogisticunitbestbeforesellbydate", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("locationmapwarehousefkrefno", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("locationmapsectionfkrefno", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("locationmapslotfkrefno", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("skidcode", Type.GetType("System.String"));
        //    dataTableItems.Columns.Add("activityitemquantity", Type.GetType("System.Decimal"));
        //    dataTableItems.Columns.Add("activityitemweightquantity", Type.GetType("System.Decimal"));

        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string weightUnitCode;
        private string activityDetailLogisticUnitProductionDate;
        private string activityDetailLogisticUnitBestbeforesellbydate;
        private string locationMapWarehouseFKRefno;
        private string locationMapSectionFKRefno;
        private string locationMapSlotFKRefno;
        private string skidCode;
        private decimal activityItemQuantity;
        private decimal activityItemWeightQuantity;

        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public string ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
        public string ActivityDetailLogisticUnitBestbeforesellbydate { get => activityDetailLogisticUnitBestbeforesellbydate; set => activityDetailLogisticUnitBestbeforesellbydate = value; }
        public string LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapSectionFKRefno { get => locationMapSectionFKRefno; set => locationMapSectionFKRefno = value; }
        public string LocationMapSlotFKRefno { get => locationMapSlotFKRefno; set => locationMapSlotFKRefno = value; }
        public string SkidCode { get => skidCode; set => skidCode = value; }
        public decimal ActivityItemQuantity { get => activityItemQuantity; set => activityItemQuantity = value; }
        public decimal ActivityItemWeightQuantity { get => activityItemWeightQuantity; set => activityItemWeightQuantity = value; }
    }
}
