namespace dMobile.View.Types
{
    public class ProductUnit
    {
        private int productUnitRefno;
        private int productRefno;
        private string productDescription;
        private string productCode;
        private string unitCode;
        private string baseUnitCode;
        private string weightUnitCode;
        private string productUnitBC;
        private bool productUnitIsBC;
        private string productUnitSizeDescription;
        private bool productUnitIsSellUnit;

        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public bool ProductUnitIsSellUnit { get => productUnitIsSellUnit; set => productUnitIsSellUnit = value; }
    }
}
