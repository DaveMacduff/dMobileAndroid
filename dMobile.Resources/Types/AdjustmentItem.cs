using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class AdjustmentItem
    {
        private int activityItemRefno;
        private int activityItemAdjustmentFKRefno;
        private int productUnitRefno;
        private int productRefno;
        private int locationMapRefno;
        private int locationCode;
        private int activityItemStatus;
        private decimal activityItemAdjustmentQuantity;
        private decimal activityItemAdjustmentBaseQuantity;
        private decimal activityItemAdjustmentWeightQuantity;
        private string activityItemAdjustmentStringFKRefno;
        private string productUnitBC;
        private string productUnitSizeDescription;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string locationName;

        public int ActivityItemRefno { get => activityItemRefno; set => activityItemRefno = value; }
        public int ActivityItemAdjustmentFKRefno { get => activityItemAdjustmentFKRefno; set => activityItemAdjustmentFKRefno = value; }
        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationCode { get => locationCode; set => locationCode = value; }
        public int ActivityItemStatus { get => activityItemStatus; set => activityItemStatus = value; }
        public decimal ActivityItemAdjustmentQuantity { get => activityItemAdjustmentQuantity; set => activityItemAdjustmentQuantity = value; }
        public decimal ActivityItemAdjustmentBaseQuantity { get => activityItemAdjustmentBaseQuantity; set => activityItemAdjustmentBaseQuantity = value; }
        public decimal ActivityItemAdjustmentWeightQuantity { get => activityItemAdjustmentWeightQuantity; set => activityItemAdjustmentWeightQuantity = value; }
        public string ActivityItemAdjustmentStringFKRefno { get => activityItemAdjustmentStringFKRefno; set => activityItemAdjustmentStringFKRefno = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string LocationName { get => locationName; set => locationName = value; }
    }
}
