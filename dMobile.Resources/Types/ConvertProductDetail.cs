using System;

namespace dMobile.Resources.Types
{
    public class ConvertProductDetail
    {
        private int productRefno;
        private int productSchemeVariantNumber;
        private int activityDetailFKRefno;
        private int activityItemFKRefno;
        private int activityHdrFKRefno;
        private decimal activityDetailQuantity;
        private decimal activityDetailBaseQuantity;
        private decimal activityDetailWeightQuantity;
        private string activityDetailSingleTradeItemSerialNumber;
        private string activityDetailLogisticUnitBatchLotNumber;
        private string productSchemeVariantCode;
        private DateTime activityDetailLogisticUnitBestBeforeSellByDate;
        private DateTime activityDetailLogisticUnitProductionDate;

        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int ProductSchemeVariantNumber { get => productSchemeVariantNumber; set => productSchemeVariantNumber = value; }
        public int ActivityDetailFKRefno { get => activityDetailFKRefno; set => activityDetailFKRefno = value; }
        public int ActivityItemFKRefno { get => activityItemFKRefno; set => activityItemFKRefno = value; }
        public int ActivityHdrFKRefno { get => activityHdrFKRefno; set => activityHdrFKRefno = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailBaseQuantity { get => activityDetailBaseQuantity; set => activityDetailBaseQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
        public string ActivityDetailSingleTradeItemSerialNumber { get => activityDetailSingleTradeItemSerialNumber; set => activityDetailSingleTradeItemSerialNumber = value; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get => activityDetailLogisticUnitBatchLotNumber; set => activityDetailLogisticUnitBatchLotNumber = value; }
        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
    }
}
