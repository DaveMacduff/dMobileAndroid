using System;

namespace dMobile.Resources.Types
{
    public class MoveInventoryDetail
    {
        //"productUnitRefno", Type.GetType("System.Int32")
        //"productRefno", Type.GetType("System.Int32")
        //"productSchemeVariantNo", Type.GetType("System.Int32")
        //"activityDetailFKRefno", Type.GetType("System.Int32")
        //"activityDetailRefno", Type.GetType("System.Int32")
        //"productCode", Type.GetType("System.String")
        //"productUnitBC", Type.GetType("System.String")
        //"unitCode", Type.GetType("System.String")
        //"productDescription", Type.GetType("System.String")
        //"productUnitSizeDescription", Type.GetType("System.String")
        //"activityDetailSchemeID", Type.GetType("System.String")
        //"activityDetailUniqueSchemeID", Type.GetType("System.String")
        //"activityDetailSingleTradeItemSerialNumber", Type.GetType("System.String")
        //"activityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String")
        //"productSchemeVariantCode", Type.GetType("System.String")
        //"activityDetailQuantity", Type.GetType("System.Decimal")
        //"activityDetailBaseQuantity", Type.GetType("System.Decimal")
        //"activityDetailWeightQuantity", Type.GetType("System.Decimal")
        //"activityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime")
        //"activityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime")

        private int productUnitRefno;
        private int productRefno;
        private int productSchemeVariantNo;
        private int activityDetailFKRefno;
        private int activityDetailRefno;        
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string productDescription;
        private string productUnitSizeDescription;
        private string activityDetailSchemeID;
        private string activityDetailUniqueSchemeID;
        private string activityDetailSingleTradeItemSerialNumber;
        private string activityDetailLogisticUnitBatchLotNumber;
        private string productSchemeVariantCode;
        private decimal activityDetailQuantity;
        private decimal activityDetailBaseQuantity;
        private decimal activityDetailWeightQuantity;
        private DateTime activityDetailLogisticUnitBestBeforeSellByDate;
        private DateTime activityDetailLogisticUnitProductionDate;

        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int ProductSchemeVariantNo { get => productSchemeVariantNo; set => productSchemeVariantNo = value; }
        public int ActivityDetailFKRefno { get => activityDetailFKRefno; set => activityDetailFKRefno = value; }
        public int ActivityDetailRefno { get => activityDetailRefno; set => activityDetailRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string ActivityDetailSchemeID { get => activityDetailSchemeID; set => activityDetailSchemeID = value; }
        public string ActivityDetailUniqueSchemeID { get => activityDetailUniqueSchemeID; set => activityDetailUniqueSchemeID = value; }
        public string ActivityDetailSingleTradeItemSerialNumber { get => activityDetailSingleTradeItemSerialNumber; set => activityDetailSingleTradeItemSerialNumber = value; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get => activityDetailLogisticUnitBatchLotNumber; set => activityDetailLogisticUnitBatchLotNumber = value; }
        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailBaseQuantity { get => activityDetailBaseQuantity; set => activityDetailBaseQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
    }
}
