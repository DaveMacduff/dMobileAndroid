using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.Resources.Types
{
    public class InventoryInquiryDetail
    {
        private string productCode;
        private string productUnitBC;
        private string unitCode;
        private string weightUnitCode;
        private string activityDetailSchemeID;
        private decimal activityDetailQuantity;
        private decimal activityDetailWeightQuantity;

        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public string ActivityDetailSchemeID { get => activityDetailSchemeID; set => activityDetailSchemeID = value; }
        public decimal ActivityDetailQuantity { get => activityDetailQuantity; set => activityDetailQuantity = value; }
        public decimal ActivityDetailWeightQuantity { get => activityDetailWeightQuantity; set => activityDetailWeightQuantity = value; }
    }
}
