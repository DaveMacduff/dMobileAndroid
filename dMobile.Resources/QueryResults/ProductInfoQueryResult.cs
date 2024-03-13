// <copyright file="ProductInfoQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
    // "tProduct.ProductAcceptDays, " +
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
        public int ProductUnitRefno { get; set; }
        public int ProductRefno { get; set; }
        public int ProductFKRefno { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string UnitCode { get; set; }
        public string BaseUnitCode { get; set; }
        public string WeightUnitCode { get; set; }
        public string ProductUnitSizeDescription { get; set; }
        public bool ProductUnitIsBC { get; set; }
        public string ProductUnitBC { get; set; }
        public bool ProductIsCatchAsk { get; set; }
        public bool ProductIsCatchAskZero { get; set; }
        public bool ProductAllowConvertProduct { get; set; }
        public int ProductAcceptDays { get; set; }
        public bool ProductIsCatchCount { get; set; }
        public bool ProductIsCatchWeight { get; set; }
        public decimal ProductUnitWeight { get; set; }
        public string ProductBrandDescription { get; set; }
        public bool ProductUnitIsDefault { get; set; }
        public bool ProductUnitIsSellUnit { get; set; }
        public decimal ProductUnitBoxCount { get; set; }
        public decimal ProductUnitWeightTolerance { get; set; }
    }
}
