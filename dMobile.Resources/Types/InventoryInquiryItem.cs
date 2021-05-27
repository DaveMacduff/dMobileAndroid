using System;
namespace dMobile.Resources.Types
{
    public class InventoryInquiryItem
    {
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string weightUnitCode;
        private decimal activityItemQuantity;
        private decimal activityitemweightquantity;
        private string activityDetailLogisticUnitProductionDate;
        private string activityDetailLogisticUnitBestBeforeSellByDate;
        private string locationMapWarehouseFKRefno;
        private string locationMapSectionFKRefno;
        private string locationMapSlotFKRefno;
        private string skidCode;

        public string Productcode { get => productCode; set => productCode = value; }
        public string Productunitbc { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public decimal ActivityItemQuantity { get => activityItemQuantity; set => activityItemQuantity = value; }
        public decimal ActivityItemWeightQuantity { get => activityitemweightquantity; set => activityitemweightquantity = value; }
        public string ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
        public string ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public string LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapSectionFKRefno { get => locationMapSectionFKRefno; set => locationMapSectionFKRefno = value; }
        public string LocationMapSlotFKRefno { get => locationMapSlotFKRefno; set => locationMapSlotFKRefno = value; }
        public string SkidCode { get => skidCode; set => skidCode = value; }
    }
}
