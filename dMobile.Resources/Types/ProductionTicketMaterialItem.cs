using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class ProductionTicketMaterialItem
    {
        //tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
        //        tActivityItemProductionMaterial.ActivityItemProductionMaterialFKRefno AS ActivityItemProductionMaterialFKRefno,");
        //        tActivityItemProductionMaterial.ActivityItemProductionMaterialStringFKRefno AS ActivityItemProductionMaterialStringFKRefno,");
        //        tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
        //        tProduct.ProductRefno AS ProductRefno,");
        //        tProduct.ProductCode AS ProductCode,");
        //        tProduct.ProductDescription AS ProductDescription,");
        //        tUnit.UnitCode AS BaseUnitCode,");
        //        tProductUnit.ProductUnitBC AS ProductUnitBC,");
        //        tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
        //        tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
        //        tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
        //        tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
        //        tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
        //        tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
        //        tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
        //        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
        //        tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        //        tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        //        tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
        //        FROM tActivityItemProduction ");

        private int activityItemRefno;
        private int activityItemProductionMaterialFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private int locationMapRefno;
        private int locationCode;
        private int activityItemStatus;
        private decimal activityItemProductionQuantity;
        private decimal activityItemProductionBaseQuantity;
        private decimal activityItemProductionWeightQuantity;
        private decimal activityItemProductionQuantityExpected;
        private decimal activityItemProductionBaseQuantityExpected;
        private decimal activityItemProductionWeightQuantityExpected;
        private string activityItemProductionMaterialStringFKRefno;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private string productCode;
        private string productDescription;
        private string baseUnitCode;
        private string locationName;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemProductionMaterialFKRefno { get => activityItemProductionMaterialFKRefno; set => activityItemProductionMaterialFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
        public decimal ActivityItemProductionQuantity { get => activityItemProductionQuantity; set => activityItemProductionQuantity = value; }
        public decimal ActivityItemProductionBaseQuantity { get => activityItemProductionBaseQuantity; set => activityItemProductionBaseQuantity = value; }
        public decimal ActivityItemProductionWeightQuantity { get => activityItemProductionWeightQuantity; set => activityItemProductionWeightQuantity = value; }
        public decimal ActivityItemProductionQuantityExpected { get => activityItemProductionQuantityExpected; set => activityItemProductionQuantityExpected = value; }
        public decimal ActivityItemProductionBaseQuantityExpected { get => activityItemProductionBaseQuantityExpected; set => activityItemProductionBaseQuantityExpected = value; }
        public decimal ActivityItemProductionWeightQuantityExpected { get => activityItemProductionWeightQuantityExpected; set => activityItemProductionWeightQuantityExpected = value; }
        public string ActivityItemProductionMaterialStringFKRefno { get => activityItemProductionMaterialStringFKRefno; set => activityItemProductionMaterialStringFKRefno = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
    }
}
