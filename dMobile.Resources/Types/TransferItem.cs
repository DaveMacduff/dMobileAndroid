using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class TransferItem
    {
        private int activityItemRefno;
        private int activityItemTransferFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private int locationMapRefno;
        private int locationCode;
        private int activityItemStatus;
        private decimal activityItemTransferRequiredQuantity;
        private decimal activityItemTransferQuantity;
        private decimal activityItemTransferBaseQuantity;
        private decimal activityItemTransferWeightQuantity;
        private string activityItemTransferStringFKRefno;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string locationName;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemTransferFKRefno { get => activityItemTransferFKRefno; set => activityItemTransferFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
        public decimal ActivityItemTransferRequiredQuantity { get => activityItemTransferRequiredQuantity; set => activityItemTransferRequiredQuantity = value; }
        public decimal ActivityItemTransferQuantity { get => activityItemTransferQuantity; set => activityItemTransferQuantity = value; }
        public decimal ActivityItemTransferBaseQuantity { get => activityItemTransferBaseQuantity; set => activityItemTransferBaseQuantity = value; }
        public decimal ActivityItemTransferWeightQuantity { get => activityItemTransferWeightQuantity; set => activityItemTransferWeightQuantity = value; }
        public string ActivityItemTransferStringFKRefno { get => activityItemTransferStringFKRefno; set => activityItemTransferStringFKRefno = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
    }
}
