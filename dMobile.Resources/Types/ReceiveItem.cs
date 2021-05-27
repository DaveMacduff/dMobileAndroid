using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class ReceiveItem
    {
        private int activityItemRefno;
        private int activityItemReceivingFKRefno;
        private string activityItemReceivingStringFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private decimal activityItemReceivingQuantityOrdered;
        private decimal activityItemReceivingBaseQuantityOrdered;
        private decimal activityItemReceivingWeightQuantityOrdered;
        private decimal activityItemReceivingQuantityReceived;
        private decimal activityItemReceivingBaseQuantityReceived;
        private decimal activityItemReceivingWeightQuantityReceived;
        private string activityItemReceivingVendorProductUnitCode;
        private string activityItemReceivingDescription;
        private int locationMapRefno;
        private int locationCode;
        private string locationName;
        private decimal activityItemReceivingQuantityReceivedToDate;
        private decimal activityItemReceivingWeightQuantityReceivedToDate;
        private string isActivityItemReceivingComment;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemReceivingFKRefno { get => activityItemReceivingFKRefno; set => activityItemReceivingFKRefno = value; }
        public string ActivityItemReceivingStringFKRefno { get => activityItemReceivingStringFKRefno; set => activityItemReceivingStringFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public decimal ActivityItemReceivingQuantityOrdered { get => activityItemReceivingQuantityOrdered; set => activityItemReceivingQuantityOrdered = value; }
        public decimal ActivityItemReceivingBaseQuantityOrdered { get => activityItemReceivingBaseQuantityOrdered; set => activityItemReceivingBaseQuantityOrdered = value; }
        public decimal ActivityItemReceivingWeightQuantityOrdered { get => activityItemReceivingWeightQuantityOrdered; set => activityItemReceivingWeightQuantityOrdered = value; }
        public decimal ActivityItemReceivingQuantityReceived { get => activityItemReceivingQuantityReceived; set => activityItemReceivingQuantityReceived = value; }
        public decimal ActivityItemReceivingBaseQuantityReceived { get => activityItemReceivingBaseQuantityReceived; set => activityItemReceivingBaseQuantityReceived = value; }
        public decimal ActivityItemReceivingWeightQuantityReceived { get => activityItemReceivingWeightQuantityReceived; set => activityItemReceivingWeightQuantityReceived = value; }
        public string ActivityItemReceivingVendorProductUnitCode { get => activityItemReceivingVendorProductUnitCode; set => activityItemReceivingVendorProductUnitCode = value; }
        public string ActivityItemReceivingDescription { get => activityItemReceivingDescription; set => activityItemReceivingDescription = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public decimal ActivityItemReceivingQuantityReceivedToDate { get => activityItemReceivingQuantityReceivedToDate; set => activityItemReceivingQuantityReceivedToDate = value; }
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get => activityItemReceivingWeightQuantityReceivedToDate; set => activityItemReceivingWeightQuantityReceivedToDate = value; }
        public string IsActivityItemReceivingComment { get => isActivityItemReceivingComment; set => isActivityItemReceivingComment = value; }
    }
}
