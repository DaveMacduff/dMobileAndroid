using System;

namespace dMobile.Resources.Types
{
    public class ConvertUnitDetail
    {
        private int countNumber;
        private int productUnitRefno;
        private int productRefno;
        private int productSchemeVariantNumber;
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

        public int CountNumber { get => countNumber; set => countNumber = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int ProductSchemeVariantNumber { get => productSchemeVariantNumber; set => productSchemeVariantNumber = value; }
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
