namespace dMobile.Resources.QueryResults
{
    // string query = "SELECT tProduct.ProductFKRefno, " +
    // "tUnit.UnitCode, " +
    // "aUnitBase.UnitCode AS BaseUnitCode, " +
    // "aUnitWeight.UnitCode AS WeightUnitCode, " +
    // "tProduct.ProductRefno, " +
    // "tProduct.ProductCode, " +
    // "tProduct.ProductDescription, " +
    // "tProduct.ProductIsCatchCount, " +
    // "tProduct.ProductIsCatchWeight, " +
    // "tProduct.ProductIsCatchAsk, " +
    // "tProduct.ProductIsCatchAskZero, " +
    // "tProduct.ProductAllowConvertProduct, " +
    // "tProductUnit.ProductUnitRefno, " +
    // "tProductUnit.ProductUnitBC, " +
    // "tProductUnit.ProductUnitIsBC, " +
    // "tProductUnit.ProductUnitWeight, " +
    // "tProductUnit.ProductUnitSizeDescription, " +
    // "tProduct.ProductBrandDescription, " +
    // "tProductUnit.ProductUnitIsSellUnit, " +
    // "tProductUnit.ProductUnitIsDefault, " +
    // "tProductUnit.ProductUnitBoxCount " +
    // "FROM tProductUnit " +
    // "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
    // "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
    // "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
    // "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
    // "WHERE tProduct.ProductCode = ? " +
    // "AND tProductUnit.ProductUnitBC = ? " +
    // "AND tUnit.UnitCode = ?";

    public class ProductInfoQueryResult
    {
        private int productUnitRefno;
        private int productRefno;
        private int productFKRefno;
        private string productCode;
        private string productDescription;
        private string unitCode;
        private string baseUnitCode;
        private string weightUnitCode;
        private string productUnitSizeDescription;
        private bool productUnitIsBC;
        private string productUnitBC;
        private bool productIsCatchAsk;
        private bool productIsCatchAskZero;
        private bool productAllowConvertProduct;
        private bool productIsCatchCount;
        private bool productIsCatchWeight;
        private decimal productUnitWeight;
        private string productBrandDescription;
        private bool productUnitIsDefault;
        private bool productUnitIsSellUnit;
        private decimal productUnitBoxCount;
        private decimal productUnitWeightTolerance;

        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductRefno { get => productRefno; set => productRefno = value; }
        public int ProductFKRefno { get => productFKRefno; set => productFKRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
        public string BaseUnitCode { get => baseUnitCode; set => baseUnitCode = value; }
        public string WeightUnitCode { get => weightUnitCode; set => weightUnitCode = value; }
        public string ProductUnitSizeDescription { get => productUnitSizeDescription; set => productUnitSizeDescription = value; }
        public bool ProductUnitIsBC { get => productUnitIsBC; set => productUnitIsBC = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public bool ProductIsCatchAsk { get => productIsCatchAsk; set => productIsCatchAsk = value; }
        public bool ProductIsCatchAskZero { get => productIsCatchAskZero; set => productIsCatchAskZero = value; }
        public bool ProductAllowConvertProduct { get => productAllowConvertProduct; set => productAllowConvertProduct = value; }
        public bool ProductIsCatchCount { get => productIsCatchCount; set => productIsCatchCount = value; }
        public bool ProductIsCatchWeight { get => productIsCatchWeight; set => productIsCatchWeight = value; }
        public decimal ProductUnitWeight { get => productUnitWeight; set => productUnitWeight = value; }
        public string ProductBrandDescription { get => productBrandDescription; set => productBrandDescription = value; }
        public bool ProductUnitIsDefault { get => productUnitIsDefault; set => productUnitIsDefault = value; }
        public bool ProductUnitIsSellUnit { get => productUnitIsSellUnit; set => productUnitIsSellUnit = value; }
        public decimal ProductUnitBoxCount { get => productUnitBoxCount; set => productUnitBoxCount = value; }
        public decimal ProductUnitWeightTolerance { get => productUnitWeightTolerance; set => productUnitWeightTolerance = value; }
    }
}
