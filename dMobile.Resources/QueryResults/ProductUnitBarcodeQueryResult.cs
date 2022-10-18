// <copyright file="ProductUnitBarcodeQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.QueryResults
{
    // string queryProductUnitBarcode = "SELECT tProductUnit.ProductUnitRefno,"
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
        private int productSchemeRefno;
        private string unitCode;

        public int ProductUnitRefno { get; set; }
        public int ProductUnitBarCodeRefno { get; set; }
        public int WarehouseProductSchemeRefno { get; set; }
        public int ProductSchemeRefno { get => this.productSchemeRefno; set => this.productSchemeRefno = value; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductUnitBC { get; set; }
        public string UnitCode { get => this.unitCode; set => this.unitCode = value; }
    }
}

// "SELECT tProductUnit.ProductUnitRefno,"
//                                    + "tLocationMapWarehouseProductScheme.ProductSchemeRefno,"
//                                    + "tProduct.ProductCode,"
//                                    + "tProductUnit.ProductUnitBC,"
//                                    + "tUnit.UnitCode "
//                                    + "FROM tProductUnit "