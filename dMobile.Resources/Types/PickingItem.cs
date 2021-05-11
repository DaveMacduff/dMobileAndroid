namespace dMobile.Resources.Types
{
    public class PickingItem
    {
        //activityItemRefno int
        //activityItemPickingFKRefno int
        //activityItemPickingStringFKRefno string
        //productUnitRefno int
        // productRefno int
        // productCode string
        // productDescription string 
        // productBrandDescription string
        // unitCode string
        // productUnitBC string
        // productUnitSizeDescription string 
        // activityItemPickingQuantityOrdered deciaml
        // activityItemPickingBaseQuantityOrdered decimal
        // activityItemPickingWeightQuantityOrdered decimal
        // activityItemPickingQuantityShipped decimal
        // activityItemPickingBaseQuantityShipped decimal
        // activityItemPickingWeightQuantityShipped decimal
        // activityItemPickingCustomerProductUnitCode string
        // locationMapRefno int 
        // locationCode int 
        // locationName string
        // activityItemPickingQuantityAvailable decimal
        // activityItemPickingBaseQuantityAvailable decimal
        // activityItemPickingWeightQuantityAvailable decimal
        // isActivityItemPickingComment string
        // activityItemStatus int

        private int activityItemRefno;
        private int activityItemPickingFKRefno;
        private string activityItemPickingStringFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private string productCode;
        private string productDescription;
        private string productBrandDescription;
        private string unitCode;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private decimal activityItemPickingQuantityOrdered;
        private decimal activityItemPickingBaseQuantityOrdered;
        private decimal activityItemPickingWeightQuantityOrdered;
        private decimal activityItemPickingQuantityShipped;
        private decimal activityItemPickingBaseQuantityShipped;
        private decimal activityItemPickingWeightQuantityShipped;
        private string activityItemPickingCustomerProductUnitCode;
        private int locationMapRefno;
        private int locationCode;
        private string locationName;
        private decimal activityItemPickingQuantityAvailable;
        private decimal activityItemPickingBaseQuantityAvailable;
        private decimal activityItemPickingWeightQuantityAvailable;
        private string isActivityItemPickingComment;
        private int activityItemStatus;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemPickingFKRefno { get => activityItemPickingFKRefno; set => activityItemPickingFKRefno = value; }
        public string ActivityItemPickingStringFKRefno { get => activityItemPickingStringFKRefno; set => activityItemPickingStringFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductBrandDescription { get => productBrandDescription; set => productBrandDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityItemPickingQuantityOrdered { get => activityItemPickingQuantityOrdered; set => activityItemPickingQuantityOrdered = value; }
        public decimal ActivityItemPickingBaseQuantityOrdered { get => activityItemPickingBaseQuantityOrdered; set => activityItemPickingBaseQuantityOrdered = value; }
        public decimal ActivityItemPickingWeightQuantityOrdered { get => activityItemPickingWeightQuantityOrdered; set => activityItemPickingWeightQuantityOrdered = value; }
        public decimal ActivityItemPickingQuantityShipped { get => activityItemPickingQuantityShipped; set => activityItemPickingQuantityShipped = value; }
        public decimal ActivityItemPickingBaseQuantityShipped { get => activityItemPickingBaseQuantityShipped; set => activityItemPickingBaseQuantityShipped = value; }
        public decimal ActivityItemPickingWeightQuantityShipped { get => activityItemPickingWeightQuantityShipped; set => activityItemPickingWeightQuantityShipped = value; }
        public string ActivityItemPickingCustomerProductUnitCode { get => activityItemPickingCustomerProductUnitCode; set => activityItemPickingCustomerProductUnitCode = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public decimal ActivityItemPickingQuantityAvailable { get => activityItemPickingQuantityAvailable; set => activityItemPickingQuantityAvailable = value; }
        public decimal ActivityItemPickingBaseQuantityAvailable { get => activityItemPickingBaseQuantityAvailable; set => activityItemPickingBaseQuantityAvailable = value; }
        public decimal ActivityItemPickingWeightQuantityAvailable { get => activityItemPickingWeightQuantityAvailable; set => activityItemPickingWeightQuantityAvailable = value; }
        public string IsActivityItemPickingComment { get => isActivityItemPickingComment; set => isActivityItemPickingComment = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
    }
}
