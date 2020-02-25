namespace dMobile.Resources.Types
{
    public class CountItem
    {
        private int activityItemRefno;
        private int activityItemCountFKRefno;
        private string activityItemCountStringFKRefno;
        private int productUnitRefno;
        private int locationMapRefno;
        private decimal activityItemCountQuantityOnHand;
        private decimal activityItemCountBaseQuantityOnHand;
        private decimal activityItemCountWeightQuantityOnHand;
        private decimal activityItemCountQuantity;
        private decimal activityItemCountBaseQuantity;
        private decimal activityItemCountWeightQuantity;
        private int productRefno;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private string locationCode;
        private string locationName;
        private int activityItemStatus;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemCountFKRefno { get => activityItemCountFKRefno; set => activityItemCountFKRefno = value; }
        public string ActivityItemCountStringFKRefno { get => activityItemCountStringFKRefno; set => activityItemCountStringFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public decimal ActivityItemCountQuantityOnHand { get => activityItemCountQuantityOnHand; set => activityItemCountQuantityOnHand = value; }
        public decimal ActivityItemCountBaseQuantityOnHand { get => activityItemCountBaseQuantityOnHand; set => activityItemCountBaseQuantityOnHand = value; }
        public decimal ActivityItemCountWeightQuantityOnHand { get => activityItemCountWeightQuantityOnHand; set => activityItemCountWeightQuantityOnHand = value; }
        public decimal ActivityItemCountQuantity { get => activityItemCountQuantity; set => activityItemCountQuantity = value; }
        public decimal ActivityItemCountBaseQuantity { get => activityItemCountBaseQuantity; set => activityItemCountBaseQuantity = value; }
        public decimal ActivityItemCountWeightQuantity { get => activityItemCountWeightQuantity; set => activityItemCountWeightQuantity = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
    }
}
