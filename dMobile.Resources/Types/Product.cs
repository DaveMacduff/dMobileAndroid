namespace dMobile.Resources.Types
{
    public class Product
    {
        private int productRefno;
        private string productDescription;
        private string productCode;

        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
    }
}
