namespace dMobile.Resources.QueryResults
{
    //string queryProductUnitBarcode = "SELECT tProductUnit.ProductUnitRefno,"
    // + "tProductUnitBarCode.ProductUnitBarCodeRefno,"
    // + "tLocationMapWarehouseProductScheme.ProductSchemeRefno,"
    // + "tProduct.ProductSchemeRefno,"
    // + "tProduct.ProductCode,"
    // + "tProduct.ProductDescription,"
    // + "tProductUnit.ProductUnitBC,"
    // + "tUnit.UnitCode "
    // + "FROM tProductUnitBarCode "
    // + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno "
    // + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
    // + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
    // + "INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno "
    // + "WHERE tProductUnitBarCode.ProductUnitBarCodeCode = ? "
    // + "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ? "
    // + "AND tProductUnitBarCode.ProductUnitBarCodeModifier1 = ?";

    public class ProductUnitBarcodeQueryResult
    {
        private int productUnitRefno;
        private int productUnitBarCodeRefno;
        private int warehouseProductSchemeRefno;
        private int productSchemeRefno;
        private string productCode;
        private string productDescription;
        private string productUnitBC;
        private string unitCode;

        public int ProductUnitRefno { get => productUnitRefno; set => productUnitRefno = value; }
        public int ProductUnitBarCodeRefno { get => productUnitBarCodeRefno; set => productUnitBarCodeRefno = value; }
        public int WarehouseProductSchemeRefno { get => warehouseProductSchemeRefno; set => warehouseProductSchemeRefno = value; }
        public int ProductSchemeRefno { get => productSchemeRefno; set => productSchemeRefno = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string ProductDescription { get => productDescription; set => productDescription = value; }
        public string ProductUnitBC { get => productUnitBC; set => productUnitBC = value; }
        public string UnitCode { get => unitCode; set => unitCode = value; }
    }
}

//"SELECT tProductUnit.ProductUnitRefno,"
//                                    + "tLocationMapWarehouseProductScheme.ProductSchemeRefno,"
//                                    + "tProduct.ProductCode,"
//                                    + "tProductUnit.ProductUnitBC,"
//                                    + "tUnit.UnitCode "
//                                    + "FROM tProductUnit "