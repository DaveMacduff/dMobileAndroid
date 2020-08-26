using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class ProductionTicketByProductItem
    {
        //sql.Append("SELECT tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
        //sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductFKRefno AS ActivityItemProductionByProductFKRefno,");
        //sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductStringFKRefno AS ActivityItemProductionByProductStringFKRefno,");
        //sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
        //sql.Append("tProduct.ProductRefno AS ProductRefno,");
        //sql.Append("tProduct.ProductCode AS ProductCode,");
        //sql.Append("tProduct.ProductDescription AS ProductDescription,");
        //sql.Append("tUnit.UnitCode AS BaseUnitCode,");
        //sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
        //sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
        //sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
        //sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
        //sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        //sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        //sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
        //sql.Append("FROM tActivityItemProduction ");

        private int activityItemRefno;
        private int activityItemProductionByProductFKRefno;
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
        private string activityItemProductionByProductStringFKRefno;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private string productCode;
        private string productDescription;
        private string baseUnitCode;
        private string locationName;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemProductionByProductFKRefno { get => activityItemProductionByProductFKRefno; set => activityItemProductionByProductFKRefno = value; }
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
        public string ActivityItemProductionByProductStringFKRefno { get => activityItemProductionByProductStringFKRefno; set => activityItemProductionByProductStringFKRefno = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
    }
}
