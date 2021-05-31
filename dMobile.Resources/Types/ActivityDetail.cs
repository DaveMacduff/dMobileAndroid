using System;
namespace dMobile.Resources.Types
{
    //    detail = new
    //{
    //    ActivityDetailRefno = 0, int 
    //    ProductRefno = this.ActItem.ProdInfo.Refno, int
    //    ProductCode = this.ActItem.ProdInfo.Code, string
    //    ProductUnitBC = this.ActItem.ProdInfo.UnitBC, string 
    //    ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC, bool
    //    ProductDescription = this.ActItem.ProdInfo.Desc, string
    //    ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc, string
    //    UnitCode = this.ActItem.ProdInfo.UnitCode, string
    //    BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode, string
    //    WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode, string
    //    ActivityDetailQuantity = 0, decimal
    //    ActivityDetailBaseQuantity = 0, decimal
    //    ActivityDetailWeightQuantity = 0, decimal 
    //    actDetailFKRefnoFldName = this.FKRefno, int
    //    LocationMapRefno = 0, int
    //    ProductSchemeVariantCode = string.Empty, string
    //    ProductSchemeVariantNo = 0, int
    //    ActivityDetailSingleTradeItemSerialNumber = string.Empty, string
    //    ActivityDetailLogisticUnitBestBeforeSellByDate = new DateTime(), datetime
    //    ActivityDetailLogisticUnitBatchLotNumber = string.Empty, string
    //    ActivityDetailLogisticUnitProductionDate = new DateTime(), datetime
    //    ActivityDetailLogisticUnitSkidNumber = string.Empty, string
    //    ActivityDetailExtra1Text = string.Empty, string 
    //    ActivityDetailExtra2Text = string.Empty, string
    //    ActivityDetailExtra3Text = string.Empty, string 
    //    ActivityDetailExtra4Text = string.Empty, string
    //    ActivityDetailExtra5Date = new DateTime(), DateTime
    //    LocationCode = string.Empty, string
    //    LocationName = string.Empty, String
    //    ActivityDetailSchemeID = string.Empty, string
    //    ActivityDetailUniqueSchemeID = string.Empty, string
    //    ActivityDetailIsConfirmed = false, bool
    //    ActivityDetailIsConfirmedString = string.Empty, string 
    //    IsBatchProductLabel = false,
    //};

    public class ActivityDetail
    {
        private int activityDetailRefno;
        private int productRefno;
        private string productCode;
        private string productUnitBC;
        private bool productUnitIsBC;
        private string productDescription;
        private string productUnitSizeDescription;
        private string unitCode;
        private string baseUnitCode;
        private string wtUnitCode;
        private decimal activityDetailQuantity;
        private decimal activityDetailBaseQuantity;
        private decimal activityDetailWeightQuantity;
        private int actDetailFKRefnoFldName;
        private int locationMapRefno;
        private string productSchemeVariantCode;
        private int productSchemeVariantNo;
        private string activityDetailSingleTradeItemSerialNumber;
        private DateTime activityDetailLogisticUnitBestBeforeSellByDate;
        private string activityDetailLogisticUnitBatchLotNumber;
        private DateTime activityDetailLogisticUnitProductionDate;
        private string activityDetailLogisticUnitSkidNumber;
        private string activityDetailExtra1Text;
        private string activityDetailExtra2Text;
        private string activityDetailExtra3Text;
        private string activityDetailExtra4Text;
        private DateTime activityDetailExtra5Date;
        private string locationCode;
        private string locationName;
        private string activityDetailSchemeID;
        private string activityDetailUniqueSchemeID;
        private bool activityDetailIsConfirmed;
        private string activityDetailIsConfirmedString;
        private bool isBatchProductLabel;

        public int ActivityDetailRefno { get => activityDetailRefno; set => activityDetailRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string WtUnitCode { get => wtUnitCode; set => wtUnitCode = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailBaseQuantity { get => activityDetailBaseQuantity; set => activityDetailBaseQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
        public int ActDetailFKRefnoFldName { get => actDetailFKRefnoFldName; set => actDetailFKRefnoFldName = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public int ProductSchemeVariantNo { get => productSchemeVariantNo; set => productSchemeVariantNo = value; }
        public string ActivityDetailSingleTradeItemSerialNumber { get => activityDetailSingleTradeItemSerialNumber; set => activityDetailSingleTradeItemSerialNumber = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get => activityDetailLogisticUnitBatchLotNumber; set => activityDetailLogisticUnitBatchLotNumber = value; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
        public string ActivityDetailLogisticUnitSkidNumber { get => activityDetailLogisticUnitSkidNumber; set => activityDetailLogisticUnitSkidNumber = value; }
        public string ActivityDetailExtra1Text { get => activityDetailExtra1Text; set => activityDetailExtra1Text = value; }
        public string ActivityDetailExtra2Text { get => activityDetailExtra2Text; set => activityDetailExtra2Text = value; }
        public string ActivityDetailExtra3Text { get => activityDetailExtra3Text; set => activityDetailExtra3Text = value; }
        public string ActivityDetailExtra4Text { get => activityDetailExtra4Text; set => activityDetailExtra4Text = value; }
        public DateTime ActivityDetailExtra5Date { get => activityDetailExtra5Date; set => activityDetailExtra5Date = value; }
        public string LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public string ActivityDetailSchemeID { get => activityDetailSchemeID; set => activityDetailSchemeID = value; }
        public string ActivityDetailUniqueSchemeID { get => activityDetailUniqueSchemeID; set => activityDetailUniqueSchemeID = value; }
        public bool ActivityDetailIsConfirmed { get => activityDetailIsConfirmed; set => activityDetailIsConfirmed = value; }
        public string ActivityDetailIsConfirmedString { get => activityDetailIsConfirmedString; set => activityDetailIsConfirmedString = value; }
        public bool IsBatchProductLabel { get => isBatchProductLabel; set => isBatchProductLabel = value; }
    }
}
