namespace dMobile.Resources.Types
{
    public class PurchaseOrderWorkSheetItem
    {
        private int activityItemRefno;
        private int activityRefno;
        private int activityItemPurchaseOrderWorkSheetFKRefno;
        private string activityItemPurchaseOrderWorkSheetStringFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string productUnitSizeDescription;
        private decimal activityItemPurchaseOrderWorkSheetQuantityOrdered;
        private decimal activityItemPurchaseOrderWorkSheetBaseQuantityOrdered;
        private decimal activityItemPurchaseOrderWorkSheetWeightQuantityOrdered;
        private int locationMapRefno;
        private int locationCode;
        private string locationName;
        private int activityItemStatus;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityRefno { get => activityRefno; set => activityRefno = value; }
        public int ActivityItemPurchaseOrderWorkSheetFKRefno { get => activityItemPurchaseOrderWorkSheetFKRefno; set => activityItemPurchaseOrderWorkSheetFKRefno = value; }
        public string ActivityItemPurchaseOrderWorkSheetStringFKRefno { get => activityItemPurchaseOrderWorkSheetStringFKRefno; set => activityItemPurchaseOrderWorkSheetStringFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get => activityItemPurchaseOrderWorkSheetQuantityOrdered; set => activityItemPurchaseOrderWorkSheetQuantityOrdered = value; }
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get => activityItemPurchaseOrderWorkSheetBaseQuantityOrdered; set => activityItemPurchaseOrderWorkSheetBaseQuantityOrdered = value; }
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get => activityItemPurchaseOrderWorkSheetWeightQuantityOrdered; set => activityItemPurchaseOrderWorkSheetWeightQuantityOrdered = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
    }
}
