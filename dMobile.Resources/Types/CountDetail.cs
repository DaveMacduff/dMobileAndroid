using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class CountDetail
    {
        //        tActivityDetail.ActivityDetailRefno
        //tActivityDetail.LocationMapRefno
        //tActivityDetailCount.ActivityDetailCountFKRefno
        //tUnit.UnitCode
        //WtUnitCode
        //BaseUnitCode
        //tProduct.ProductCode
        //tProduct.ProductRefno
        //tProduct.ProductDescription
        //tProductUnit.ProductUnitBC
        //tProductUnit.ProductUnitIsBC
        //tProductUnit.ProductUnitSizeDescription
        //tActivityDetail.ActivityDetailQuantity
        //tActivityDetail.ActivityDetailBaseQuantity
        //tActivityDetail.ActivityDetailWeightQuantity
        //tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate
        //tActivityDetail.ActivityDetailSchemeID
        //tActivityDetail.ActivityDetailUniqueSchemeID
        //tActivityDetail.ActivityDetailSingleTradeItemSerialNumber
        //tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber
        //tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate
        //tActivityDetail.ActivityDetailLogisticUnitProductionDate
        //tActivityDetail.ActivityDetailLogisticUnitSkidNumber
        //LocationCode
        //LocationName
        //Location
        //tProductSchemeVariant.ProductSchemeVariantCode
        //tProductSchemeVariant.ProductSchemeVariantNo
        //tProductSchemeVariant.ProductSchemeVariantDescription
        //tActivityDetail.ActivityDetailStatus
        //tActivityDetailExtra.ActivityDetailExtra1Text
        //tActivityDetailExtra.ActivityDetailExtra2Text
        //tActivityDetailExtra.ActivityDetailExtra3Text
        //tActivityDetailExtra.ActivityDetailExtra4Text
        //tActivityDetailExtra.ActivityDetailExtra5Date

        private int activityDetailRefno;
        private int locationMapRefno;
        private int activityDetailCountFKRefno;
        private string unitCode;
        private string wtUnitCode;
        private string baseUnitCode;
        private string productCode;
        private int productRefno;
        private string productDescription;
        private string productBrandDescription;
        private string productUnitBC;
        private bool productUnitIsBC;
        private string productUnitSizeDescription;
        private decimal activityDetailQuantity;
        private decimal activityDetailBaseQuantity;
        private decimal activityDetailWeightQuantity;
        private DateTime activityDetailLogisticUnitBestBeforeSellByDate;
        private string activityDetailSchemeID;
        private string activityDetailUniqueSchemeID;
        private string activityDetailSingleTradeItemSerialNumber;
        private string activityDetailLogisticUnitBatchLotNumber;
        private DateTime activityDetailLogisticUnitExpiryUseByDate;
        private DateTime activityDetailLogisticUnitProductionDate;
        private string activityDetailLogisticUnitSkidNumber;
        private int locationCode;
        private string locationName;
        private string location;
        private string productSchemeVariantCode;
        private int productSchemeVariantNo;
        private string productSchemeVariantDescription;
        private int activityDetailStatus;
        private string activityDetailExtra1Text;
        private string activityDetailExtra2Text;
        private string activityDetailExtra3Text;
        private string activityDetailExtra4Text;
        private DateTime activityDetailExtra5Date;

        public int ActivityDetailRefno { get => activityDetailRefno; set => activityDetailRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int ActivityDetailCountFKRefno { get => activityDetailCountFKRefno; set => activityDetailCountFKRefno = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string WtUnitCode { get => wtUnitCode; set => wtUnitCode = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductBrandDescription { get => productBrandDescription; set => productBrandDescription = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailBaseQuantity { get => activityDetailBaseQuantity; set => activityDetailBaseQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public string ActivityDetailSchemeID { get => activityDetailSchemeID; set => activityDetailSchemeID = value; }
        public string ActivityDetailUniqueSchemeID { get => activityDetailUniqueSchemeID; set => activityDetailUniqueSchemeID = value; }
        public string ActivityDetailSingleTradeItemSerialNumber { get => activityDetailSingleTradeItemSerialNumber; set => activityDetailSingleTradeItemSerialNumber = value; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get => activityDetailLogisticUnitBatchLotNumber; set => activityDetailLogisticUnitBatchLotNumber = value; }
        public DateTime ActivityDetailLogisticUnitExpiryUseByDate { get => activityDetailLogisticUnitExpiryUseByDate; set => activityDetailLogisticUnitExpiryUseByDate = value; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
        public string ActivityDetailLogisticUnitSkidNumber { get => activityDetailLogisticUnitSkidNumber; set => activityDetailLogisticUnitSkidNumber = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public string Location { get => location; set => location = value; }
        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public int ProductSchemeVariantNo { get => productSchemeVariantNo; set => productSchemeVariantNo = value; }
        public string ProductSchemeVariantDescription { get => productSchemeVariantDescription; set => productSchemeVariantDescription = value; }
        public int ActivityDetailStatus { get => activityDetailStatus; set => activityDetailStatus = value; }
        public string ActivityDetailExtra1Text { get => activityDetailExtra1Text; set => activityDetailExtra1Text = value; }
        public string ActivityDetailExtra2Text { get => activityDetailExtra2Text; set => activityDetailExtra2Text = value; }
        public string ActivityDetailExtra3Text { get => activityDetailExtra3Text; set => activityDetailExtra3Text = value; }
        public string ActivityDetailExtra4Text { get => activityDetailExtra4Text; set => activityDetailExtra4Text = value; }
        public DateTime ActivityDetailExtra5Date { get => activityDetailExtra5Date; set => activityDetailExtra5Date = value; }
    }
}
