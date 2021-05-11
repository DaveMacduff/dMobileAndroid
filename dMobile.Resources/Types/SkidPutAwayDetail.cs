using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class SkidPutAwayDetail
    {
        ////table.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
        ////table.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
        ////table.Columns.Add("ProductCode", Type.GetType("System.String"));
        //table.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
        //table.Columns.Add("UnitCode", Type.GetType("System.String"));
        //table.Columns.Add("ProductDescription", Type.GetType("System.String"));
        //table.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
        //table.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
        //table.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
        //table.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
        //table.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
        //table.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
        //table.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
        //table.Columns.Add("ProductSchemeVariantNo", Type.GetType("System.Int32"));
        //table.Columns.Add("ActivityDetailLogisticUnitSkidNumber", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
        //table.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));
        //table.Columns.Add("ActivityDetailExtra1Text", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailExtra2Text", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailExtra3Text", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailExtra4Text", Type.GetType("System.String"));
        //table.Columns.Add("ActivityDetailExtra5Date", Type.GetType("System.String"));
        //table.Columns.Add("LocationMapRefno", Type.GetType("System.Int32"));
        //table.Columns.Add("LocationCode", Type.GetType("System.String"));
        //table.Columns.Add("LocationName", Type.GetType("System.String"));

        private int productUnitRefno;
        private int productRefno;
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string productDescription;
        private string productUnitSizeDescription;
        private decimal activityDetailQuantity;
        private decimal activityDetailBaseQuantity;
        private decimal activityDetailWeightQuantity;
        private DateTime activityDetailLogisticUnitBestBeforeSellByDate;
        private DateTime activityDetailLogisticUnitProductionDate;
        private string activityDetailSingleTradeItemSerialNumber;
        private string activityDetailLogisticUnitBatchLotNumber;
        private string productSchemeVariantCode;
        private int productSchemeVariantNo;
        private string activityDetailLogisticUnitSkidNumber;
        private string activityDetailSchemeID;
        private string activityDetailUniqueSchemeID; 
        private int activityDetailFKRefno;
        private int activityDetailRefno;        
        private int activityDetailStatus;      
        private string activityDetailExtra1Text;
        private string activityDetailExtra2Text;
        private string activityDetailExtra3Text;
        private string activityDetailExtra4Text;
        private DateTime activityDetailExtra5Date;
        private int locationMapRefno;
        private string locationCode;
        private string locationName;

        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailBaseQuantity { get => activityDetailBaseQuantity; set => activityDetailBaseQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get => activityDetailLogisticUnitBestBeforeSellByDate; set => activityDetailLogisticUnitBestBeforeSellByDate = value; }
        public DateTime ActivityDetailLogisticUnitProductionDate { get => activityDetailLogisticUnitProductionDate; set => activityDetailLogisticUnitProductionDate = value; }
        public string ActivityDetailSingleTradeItemSerialNumber { get => activityDetailSingleTradeItemSerialNumber; set => activityDetailSingleTradeItemSerialNumber = value; }
        public string ActivityDetailLogisticUnitBatchLotNumber { get => activityDetailLogisticUnitBatchLotNumber; set => activityDetailLogisticUnitBatchLotNumber = value; }
        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public int ProductSchemeVariantNo { get => productSchemeVariantNo; set => productSchemeVariantNo = value; }
        public string ActivityDetailLogisticUnitSkidNumber { get => activityDetailLogisticUnitSkidNumber; set => activityDetailLogisticUnitSkidNumber = value; }
        public string ActivityDetailSchemeID { get => activityDetailSchemeID; set => activityDetailSchemeID = value; }
        public string ActivityDetailUniqueSchemeID { get => activityDetailUniqueSchemeID; set => activityDetailUniqueSchemeID = value; }
        public int ActivityDetailFKRefno { get => activityDetailFKRefno; set => activityDetailFKRefno = value; }
        public int ActivityDetailRefno { get => activityDetailRefno; set => activityDetailRefno = value; }
        public int ActivityDetailStatus { get => activityDetailStatus; set => activityDetailStatus = value; }
        public string ActivityDetailExtra1Text { get => activityDetailExtra1Text; set => activityDetailExtra1Text = value; }
        public string ActivityDetailExtra2Text { get => activityDetailExtra2Text; set => activityDetailExtra2Text = value; }
        public string ActivityDetailExtra3Text { get => activityDetailExtra3Text; set => activityDetailExtra3Text = value; }
        public string ActivityDetailExtra4Text { get => activityDetailExtra4Text; set => activityDetailExtra4Text = value; }
        public DateTime ActivityDetailExtra5Date { get => activityDetailExtra5Date; set => activityDetailExtra5Date = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public string LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
    }
}
