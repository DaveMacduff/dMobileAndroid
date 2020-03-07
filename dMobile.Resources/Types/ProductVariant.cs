namespace dMobile.Resources.Types
{
    public class ProductVariant
    {
        private string productSchemeVariantCode;
        private int productSchemeVariantNo;
        private string productSchemeVariantDisplay;

        public string ProductSchemeVariantCode { get => productSchemeVariantCode; set => productSchemeVariantCode = value; }
        public int ProductSchemeVariantNo { get => productSchemeVariantNo; set => productSchemeVariantNo = value; }
        public string ProductSchemeVariantDisplay { get => productSchemeVariantDisplay; set => productSchemeVariantDisplay = value; }
    }
}
