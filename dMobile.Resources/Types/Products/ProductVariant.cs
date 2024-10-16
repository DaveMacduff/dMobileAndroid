// <copyright file="ProductVariant.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Products
{
    public class ProductVariant
    {
        public string ProductSchemeVariantCode { get; set; }
        public int ProductSchemeVariantNo { get; set; }
        public string ProductSchemeVariantDisplay { get; set; }
    }
}


// s.Append("SELECT tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
// s.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
// s.Append("tProductSchemeVariant.ProductSchemeVariantCode + \'  \' ");
// s.Append("+ tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDisplay ");
// s.Append("FROM tProduct ");
// s.Append("INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno ");
// s.Append("LEFT JOIN tProductSchemeVariantXRef ON tProductSchemeVariantXRef.ProductSchemeRefno = ");
// s.Append("tLocationMapWarehouseProductScheme.ProductSchemeRefno ");
// s.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = ");
// s.Append("tProductSchemeVariantXRef.ProductSchemeVariantRefno ");
// s.AppendFormat("WHERE tProduct.ProductRefno = {0}", prodRefno);
// s.AppendFormat(" AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = {0}", currLocMap.WhsLocMapRefno);
// s.Append(" ORDER BY tProductSchemeVariant.ProductSchemeVariantCode");