namespace dMobile.Resources.Types
{
    public class ReturnItem
    {
        //ActivityItemRefno int
        //ActivityItemReturnsFKRefno int
        //ActivityItemReturnsStringFKRefno string 
        //ProductUnitRefno int
        //ProductRefno int
        //ProductCode string
        //ProductDescription string
        //ProductBrandDescription string
        //UnitCode string
        //ProductUnitBC string
        //ProductUnitSizeDescription string
        //ActivityItemReturnsQuantityReturned decimal
        //ActivityItemReturnsBaseQuantityReturned decimal
        //ActivityItemReturnsWeightQuantityReturned decimal
        //ActivityItemReturnsCustomerProductUnitCode string
        //LocationMapRefno int
        //LocationCode int
        //LocationName string 
        //IsActivityItemReturnsComment string
        //ActivityItemStatus int

        private int activityItemRefno;
        private int activityItemReturnsFKRefno;
        private string activityItemReturnsStringFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private string productCode;
        private string productDescription;
        private string productBrandDescription;
        private string unitCode;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private decimal activityItemReturnsQuantityReturned;
        private decimal activityItemReturnsBaseQuantityReturned;
        private decimal activityItemReturnsWeightQuantityReturned;
        private string activityItemReturnsCustomerProductUnitCode;
        private int locationMapRefno;
        private int locationCode;
        private string locationName;
        private string isActivityItemReturnsComment;
        private int activityItemStatus;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemReturnsFKRefno { get => activityItemReturnsFKRefno; set => activityItemReturnsFKRefno = value; }
        public string ActivityItemReturnsStringFKRefno { get => activityItemReturnsStringFKRefno; set => activityItemReturnsStringFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductBrandDescription { get => productBrandDescription; set => productBrandDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityItemReturnsQuantityReturned { get => activityItemReturnsQuantityReturned; set => activityItemReturnsQuantityReturned = value; }
        public decimal ActivityItemReturnsBaseQuantityReturned { get => activityItemReturnsBaseQuantityReturned; set => activityItemReturnsBaseQuantityReturned = value; }
        public decimal ActivityItemReturnsWeightQuantityReturned { get => activityItemReturnsWeightQuantityReturned; set => activityItemReturnsWeightQuantityReturned = value; }
        public string ActivityItemReturnsCustomerProductUnitCode { get => activityItemReturnsCustomerProductUnitCode; set => activityItemReturnsCustomerProductUnitCode = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public string IsActivityItemReturnsComment { get => isActivityItemReturnsComment; set => isActivityItemReturnsComment = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
    }
}
