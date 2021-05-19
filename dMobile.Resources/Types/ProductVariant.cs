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


//s.Append("SELECT tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
//s.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
//s.Append("tProductSchemeVariant.ProductSchemeVariantCode + \'  \' ");
//s.Append("+ tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDisplay ");
//s.Append("FROM tProduct ");
//s.Append("INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno ");
//s.Append("LEFT JOIN tProductSchemeVariantXRef ON tProductSchemeVariantXRef.ProductSchemeRefno = ");
//s.Append("tLocationMapWarehouseProductScheme.ProductSchemeRefno ");
//s.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = ");
//s.Append("tProductSchemeVariantXRef.ProductSchemeVariantRefno ");
//s.AppendFormat("WHERE tProduct.ProductRefno = {0}", prodRefno);
//s.AppendFormat(" AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = {0}", currLocMap.WhsLocMapRefno);
//s.Append(" ORDER BY tProductSchemeVariant.ProductSchemeVariantCode");