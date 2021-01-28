using System;
namespace dMobile.Resources.Types
{
    public class InventoryInquiryItem
    {
        private string productcode;
        private string productunitbc;
        private string unitcode;
        private string weightunitcode;
        private decimal activityitemquantity;
        private decimal activityitemweightquantity;

        public string Productcode { get => productcode; set => productcode = value; }
        public string Productunitbc { get => productunitbc; set => productunitbc = value; }
        public string Unitcode { get => unitcode; set => unitcode = value; }
        public string Weightunitcode { get => weightunitcode; set => weightunitcode = value; }
        public decimal Activityitemquantity { get => activityitemquantity; set => activityitemquantity = value; }
        public decimal Activityitemweightquantity { get => activityitemweightquantity; set => activityitemweightquantity = value; }
    }
}
