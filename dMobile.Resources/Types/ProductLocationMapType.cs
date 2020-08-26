namespace dMobile.Resources.Types
{
    public class ProductLocationMapType
    {
        private string productCode;
        private string locationMapType;
        private string productDescription;
        private string productBrandDescription;

        public string ProductCode { get => productCode; set => productCode = value; }
        public string LocationMapType { get => locationMapType; set => locationMapType = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductBrandDescription { get => productBrandDescription; set => productBrandDescription = value; }
    }
}
