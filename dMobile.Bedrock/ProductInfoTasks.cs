// <copyright file="ProductInfoTasks.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Resources.Models;
    using Resources.QueryResults;
    using Resources.Types;

    /// <summary>
    /// <see cref="ProductInfoTasks"/> Collection of product methods.
    /// </summary>
    public static class ProductInfoTasks
    {
        /// <summary>
        /// Get the current readable product bc unit.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="string"/> Product, bc and unit information.</returns>
        public static string GetCurrReadableProdBcUnit(ProductInfo prodInfo)
        {
            return prodInfo == null ? string.Empty : prodInfo.Code.TrimEnd() + (" " +
                prodInfo.UnitBc).TrimEnd() + " " + prodInfo.UnitCode;
        }

        /// <summary>
        /// Convert a product information object to a XDocument.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="XDocument"/> XDocument containing product information.</returns>
        public static XDocument GetXml(ProductInfo prodInfo)
        {
            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "params",
                    new XElement("ProductUnitRefno", prodInfo.UnitRefno),
                    new XElement("ProductRefno", prodInfo.Refno),
                    new XElement("ProductFKRefno", prodInfo.FkRefno),
                    new XElement("ProductCode", prodInfo.Code),
                    new XElement("ProductDescription", prodInfo.Desc),
                    new XElement("UnitCode", prodInfo.UnitCode),
                    new XElement("BaseUnitCode", prodInfo.UnitCode),
                    new XElement("WeightUnitCode", prodInfo.UnitCode),
                    new XElement("ProductUnitBC", prodInfo.UnitBc),
                    new XElement("ProductUnitIsBC", prodInfo.UnitIsBc.ToString()),
                    new XElement("ProductUnitSizeDescription", prodInfo.UnitSizeDesc),
                    new XElement("ProductIsCatchAsk", prodInfo.IsCatchAsk.ToString()),
                    new XElement("ProductIsCatchAskZero", prodInfo.IsCatchAskZero.ToString()),
                    new XElement("ProductAllowConvertProduct", prodInfo.IsAllowConvertProduct.ToString()),
                    new XElement("ProductIsCatchCount", prodInfo.IsCatchCount.ToString()),
                    new XElement("ProductIsCatchWeight", prodInfo.IsCatchWeight.ToString()),
                    new XElement("ProductBrandDescription", prodInfo.BrandDesc),
                    new XElement("ProductUnitIsSellUnit", prodInfo.UnitIsSellUnit.ToString()),
                    new XElement("ProductUnitIsDefault", prodInfo.UnitIsDefault.ToString()),
                    new XElement("ProductUnitBoxCount", prodInfo.BoxCount.ToString(CultureInfo.CurrentCulture)),
                    new XElement("ProductUnitWeightTolerance", prodInfo.WeightTolerance.ToString(CultureInfo.CurrentCulture)),
                    new XElement("ProductUnitWeight", prodInfo.UnitGrossWeight.ToString(CultureInfo.CurrentCulture)),
                    new XElement("ProductAcceptDays", prodInfo.ProductAcceptDays.ToString())));

            return request;
        }

        /// <summary>
        /// Get the product information from the product code, bc and unit.
        /// </summary>
        /// <param name="productCode"><see cref="string"/> Product code.</param>
        /// <param name="productUnitBc"><see cref="string"/> Broken case character.</param>
        /// <param name="unitCode"><see cref="string"/> Product unit code.</param>
        /// <returns><see cref="ProductInfo"/> Product information.</returns>
        public static async Task<ProductInfo> GetInfoFromCodeBcUnit(
            string productCode,
            string productUnitBc,
            string unitCode)
        {
            var prodInfo = new ProductInfo();
            if (unitCode == "SK")
            {
                const string query = @"
                    SELECT tProduct.ProductFKRefno, 
                            tProduct.ProductRefno, 
                            tProduct.ProductCode, 
                            tProduct.ProductDescription, 
                            tProduct.ProductIsCatchCount, 
                            tProduct.ProductIsCatchWeight, 
                            tProduct.ProductIsCatchAsk, 
                            tProduct.ProductIsCatchAskZero, 
                            tProduct.ProductBrandDescription, 
                            tProduct.ProductAllowConvertProduct 
                        FROM tProduct 
                        WHERE tProduct.ProductCode = ?";

                var results = await DatabaseConnection.GetInstance().SqlConn.
                    QueryAsync<ProductModel>(query, productCode);

                if (results != null && results.Count > 0)
                {
                    prodInfo.Refno = Convert.ToInt32(results[0].ProductRefno);
                    prodInfo.FkRefno = Convert.ToInt32(results[0].ProductFKRefno);
                    prodInfo.Code = Convert.ToString(results[0].ProductCode);
                    prodInfo.Desc = Convert.ToString(results[0].ProductDescription);
                    prodInfo.UnitIsBc = Convert.ToBoolean(!string.IsNullOrEmpty(productUnitBc));
                    prodInfo.UnitBc = productUnitBc;
                    prodInfo.UnitCode = unitCode;
                    prodInfo.IsCatchAsk = Convert.ToBoolean(results[0].ProductIsCatchAsk);
                    prodInfo.IsCatchAskZero = Convert.ToBoolean(results[0].ProductIsCatchAskZero);
                    prodInfo.IsAllowConvertProduct = Convert.ToBoolean(results[0].ProductAllowConvertProduct);
                    prodInfo.IsCatchCount = Convert.ToBoolean(results[0].ProductIsCatchCount);
                    prodInfo.IsCatchWeight = Convert.ToBoolean(results[0].ProductIsCatchWeight);
                    prodInfo.BrandDesc = Convert.ToString(results[0].ProductBrandDescription);
                }
            }
            else
            {
                const string query = @"
                    SELECT tProduct.ProductFKRefno
                            ,tUnit.UnitCode
                            ,aUnitBase.UnitCode AS BaseUnitCode
                            ,aUnitWeight.UnitCode AS WeightUnitCode
                            ,tProduct.ProductRefno
                            ,tProduct.ProductCode
                            ,tProduct.ProductDescription
                            ,tProduct.ProductIsCatchCount
                            ,tProduct.ProductIsCatchWeight
                            ,tProduct.ProductIsCatchAsk
                            ,tProduct.ProductIsCatchAskZero
                            ,tProduct.ProductAllowConvertProduct
                            ,tProduct.ProductAcceptDays
                            ,tProductUnit.ProductUnitRefno
                            ,tProductUnit.ProductUnitBC
                            ,tProductUnit.ProductUnitIsBC
                            ,tProductUnit.ProductUnitWeight
                            ,tProductUnit.ProductUnitSizeDescription
                            ,tProduct.ProductBrandDescription
                            ,tProductUnit.ProductUnitIsSellUnit
                            ,tProductUnit.ProductUnitIsDefault
                            ,tProductUnit.ProductUnitBoxCount
                            ,tProductUnit.ProductUnitWeightTolerance
                        FROM tProductUnit
                        INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                        INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                        INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                        INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                        WHERE tProduct.ProductCode = ?
                            AND tProductUnit.ProductUnitBC = ?
                            AND tUnit.UnitCode = ?";

                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductInfoQueryResult>(
                    query,
                    productCode,
                    productUnitBc,
                    unitCode);

                if (results != null && results.Count > 0)
                {
                    prodInfo.UnitRefno = results[0].ProductUnitRefno;
                    prodInfo.Refno = results[0].ProductRefno;
                    prodInfo.FkRefno = results[0].ProductFKRefno;
                    prodInfo.Code = results[0].ProductCode;
                    prodInfo.Desc = results[0].ProductDescription;
                    prodInfo.UnitCode = results[0].UnitCode;
                    prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                    prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                    prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                    prodInfo.UnitIsBc = results[0].ProductUnitIsBC;
                    prodInfo.UnitBc = results[0].ProductUnitBC;
                    prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                    prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                    prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                    prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                    prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                    prodInfo.UnitGrossWeight = results[0].ProductUnitWeight;
                    prodInfo.BrandDesc = results[0].ProductBrandDescription;
                    prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                    prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                    prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                    prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
                    prodInfo.ProductAcceptDays = results[0].ProductAcceptDays;
                }

                prodInfo.Unit2BaseUnitFactor = await GetUnitRelationFactor(prodInfo);
                prodInfo.Scheme = await GetSchemeComponentsFromRefno(prodInfo.Refno);
            }

            return prodInfo;
        }

        /// <summary>
        /// Get product information from a product unit reference number.
        /// </summary>
        /// <param name="productUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="ProductInfo"/> Product information.</returns>
        public static async Task<ProductInfo> GetInfoFromUnitRefno(int productUnitRefno)
        {
            var prodInfo = new ProductInfo();

            if (productUnitRefno > 0)
            {
                const string query = @"
                    SELECT tProduct.ProductFKRefno
                            ,tUnit.UnitCode
                            ,aUnitBase.UnitCode AS BaseUnitCode
                            ,aUnitWeight.UnitCode AS WeightUnitCode
                            ,tProduct.ProductRefno
                            ,tProduct.ProductCode
                            ,tProduct.ProductDescription
                            ,tProduct.ProductIsCatchCount
                            ,tProduct.ProductIsCatchWeight
                            ,tProduct.ProductIsCatchAsk
                            ,tProduct.ProductIsCatchAskZero
                            ,tProduct.ProductAllowConvertProduct
                            ,tProduct.ProductAcceptDays
                            ,tProductUnit.ProductUnitRefno
                            ,tProductUnit.ProductUnitBC
                            ,tProductUnit.ProductUnitIsBC
                            ,tProductUnit.ProductUnitWeight
                            ,tProductUnit.ProductUnitSizeDescription
                            ,tProduct.ProductBrandDescription
                            ,tProductUnit.ProductUnitIsSellUnit
                            ,tProductUnit.ProductUnitIsDefault
                            ,tProductUnit.ProductUnitBoxCount
                            ,tProductUnit.ProductUnitWeightTolerance
                        FROM tProductUnit
                        INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                        INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                        INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                        INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                        WHERE tProductUnit.ProductUnitRefno = ?";

                var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductInfoQueryResult>(
                    query,
                    Convert.ToInt32(productUnitRefno));

                if (results != null && results.Count > 0)
                {
                    prodInfo.UnitRefno = results[0].ProductUnitRefno;
                    prodInfo.Refno = results[0].ProductRefno;
                    prodInfo.FkRefno = results[0].ProductFKRefno;
                    prodInfo.Code = results[0].ProductCode;
                    prodInfo.Desc = results[0].ProductDescription;
                    prodInfo.UnitCode = results[0].UnitCode;
                    prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                    prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                    prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                    prodInfo.UnitIsBc = results[0].ProductUnitIsBC;
                    prodInfo.UnitBc = results[0].ProductUnitBC;
                    prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                    prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                    prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                    prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                    prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                    prodInfo.UnitGrossWeight = results[0].ProductUnitWeight;
                    prodInfo.BrandDesc = results[0].ProductBrandDescription;
                    prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                    prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                    prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                    prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
                    prodInfo.ProductAcceptDays = results[0].ProductAcceptDays;
                }

                prodInfo.Unit2BaseUnitFactor = await GetUnitRelationFactor(prodInfo);
                prodInfo.Scheme = await GetSchemeComponentsFromRefno(prodInfo.Refno);
            }

            return prodInfo;
        }

        /// <summary>
        /// Get product information from a product reference number.
        /// </summary>
        /// <param name="productRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="ProductInfo"/> Product information.</returns>
        public static async Task<ProductInfo> GetInfoFromRefno(int productRefno)
        {
            var prodInfo = new ProductInfo();

            const string query = @"
                SELECT tProduct.ProductFKRefno
                        ,tUnit.UnitCode
                        ,aUnitBase.UnitCode AS BaseUnitCode
                        ,aUnitWeight.UnitCode AS WeightUnitCode
                        ,tProduct.ProductRefno
                        ,tProduct.ProductCode
                        ,tProduct.ProductDescription
                        ,tProduct.ProductIsCatchCount
                        ,tProduct.ProductIsCatchWeight
                        ,tProduct.ProductIsCatchAsk
                        ,tProduct.ProductIsCatchAskZero
                        ,tProduct.ProductAllowConvertProduct
                        ,tProductUnit.ProductUnitRefno
                        ,tProductUnit.ProductUnitBC
                        ,tProductUnit.ProductUnitIsBC
                        ,tProductUnit.ProductUnitWeight
                        ,tProductUnit.ProductUnitSizeDescription
                        ,tProduct.ProductBrandDescription
                        ,tProductUnit.ProductUnitIsSellUnit
                        ,tProductUnit.ProductUnitIsDefault
                        ,tProductUnit.ProductUnitBoxCount
                        ,tProductUnit.ProductUnitWeightTolerance
                        ,tProduct.ProductAcceptDays
                    FROM tProductUnit
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    WHERE tProduct.ProductRefno = ?
                        AND tProductUnit.ProductUnitIsDefault = 1"; // true

            var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductInfoQueryResult>(
                query,
                Convert.ToInt32(productRefno));

            if (results != null && results.Count > 0)
            {
                prodInfo.UnitRefno = results[0].ProductUnitRefno;
                prodInfo.Refno = results[0].ProductRefno;
                prodInfo.FkRefno = results[0].ProductFKRefno;
                prodInfo.Code = results[0].ProductCode;
                prodInfo.Desc = results[0].ProductDescription;
                prodInfo.UnitCode = results[0].UnitCode;
                prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                prodInfo.UnitIsBc = results[0].ProductUnitIsBC;
                prodInfo.UnitBc = results[0].ProductUnitBC;
                prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                prodInfo.UnitGrossWeight = results[0].ProductUnitWeight;
                prodInfo.BrandDesc = results[0].ProductBrandDescription;
                prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
                prodInfo.ProductAcceptDays = results[0].ProductAcceptDays;
            }

            prodInfo.Unit2BaseUnitFactor = await GetUnitRelationFactor(prodInfo);
            prodInfo.Scheme = await GetSchemeComponentsFromRefno(prodInfo.Refno);

            return prodInfo;
        }

        /// <summary>
        /// Get the product scheme from the product reference number and the current location map instance.
        /// </summary>
        /// <param name="productRefno">Product reference number.</param>
        /// <returns><see cref="ProdScheme"/> Product scheme components.</returns>
        public static async Task<ProdScheme> GetSchemeComponentsFromRefno(int productRefno)
        {
            var scheme = new ProdScheme();
            var labelScheme = new ProdSchemeLabelSettings().LoadData();

            const string query = @"
                SELECT tProductScheme.ProductSchemeRefno AS ProductSchemeRefno
                        ,tProductScheme.ProductSchemeIsSerialTracked AS ProductSchemeIsSerialTracked
                        ,tProductScheme.ProductSchemeIsProductionDateTracked AS ProductSchemeIsProductionDateTracked
                        ,tProductScheme.ProductSchemeIsLotTracked AS ProductSchemeIsLotTracked
                        ,tProductScheme.ProductSchemeIsBestBeforeDateTracked AS ProductSchemeIsBestBeforeDateTracked
                        ,tProductScheme.ProductSchemeIsVariantTracked AS ProductSchemeIsVariantTracked
                        ,tProductScheme.ProductSchemeIsLotUniqueComponent AS ProductSchemeIsLotUniqueComponent
                        ,tProductScheme.ProductSchemeIsProductionDateUniqueComponent AS ProductSchemeIsProductionDateUniqueComponent
                        ,tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent AS ProductSchemeIsBestBeforeDateUniqueComponent
                        ,tProductScheme.ProductSchemeIsVariantUniqueComponent AS ProductSchemeIsVariantUniqueComponent
                        ,tProductScheme.ProductSchemeExtra1Type AS ProductSchemeExtra1Type
                        ,tProductScheme.ProductSchemeExtra2Type AS ProductSchemeExtra2Type
                        ,tProductScheme.ProductSchemeExtra3Type AS ProductSchemeExtra3Type
                        ,tProductScheme.ProductSchemeExtra4Type AS ProductSchemeExtra4Type
                        ,tProductScheme.ProductSchemeExtra5Type AS ProductSchemeExtra5Type
                        ,tProductScheme.ProductSchemeExtra1Caption AS ProductSchemeExtra1Caption
                        ,tProductScheme.ProductSchemeExtra2Caption AS ProductSchemeExtra2Caption
                        ,tProductScheme.ProductSchemeExtra3Caption AS ProductSchemeExtra3Caption
                        ,tProductScheme.ProductSchemeExtra4Caption AS ProductSchemeExtra4Caption
                        ,tProductScheme.ProductSchemeExtra5Caption AS ProductSchemeExtra5Caption
                        ,tProductScheme.ProductSchemeExtra1IsRequired AS ProductSchemeExtra1IsRequired
                        ,tProductScheme.ProductSchemeExtra2IsRequired AS ProductSchemeExtra2IsRequired
                        ,tProductScheme.ProductSchemeExtra3IsRequired AS ProductSchemeExtra3IsRequired
                        ,tProductScheme.ProductSchemeExtra4IsRequired AS ProductSchemeExtra4IsRequired
                        ,tProductScheme.ProductSchemeExtra5IsRequired AS ProductSchemeExtra5IsRequired
                        ,tProductScheme.ProductSchemeIsLotTrackedDefault AS ProductSchemeIsLotTrackedDefault
                        ,tProductScheme.ProductSchemeIsProductionDateTrackedDefault AS ProductSchemeIsProductionDateTrackedDefault
                        ,tProductScheme.ProductSchemeIsBestBeforeDateTrackedDefault AS ProductSchemeIsBestBeforeDateTrackedDefault
                    FROM tProduct
                    INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno
                        AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ?
                    INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tLocationMapWarehouseProductScheme.ProductSchemeRefno
                    WHERE tProduct.ProductRefno = ?";

            var prodSchemes = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductSchemeModel>(
                query,
                Convert.ToInt32(
                LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno),
                Convert.ToInt32(productRefno));

            if (prodSchemes != null && prodSchemes.Count > 0)
            {
                scheme.SchemeRefno = Convert.ToInt32(prodSchemes[0].ProductSchemeRefno);

                if (LocationMap.GetInstance().CurrLocMap.IsWhsWms)
                {
                    scheme.IsSerialTracked = prodSchemes[0].ProductSchemeIsSerialTracked;
                    scheme.IsProductionDateTracked = prodSchemes[0].ProductSchemeIsProductionDateTracked;
                    scheme.IsLotTracked = prodSchemes[0].ProductSchemeIsLotTracked;
                    scheme.IsBestBeforeDateTracked = prodSchemes[0].ProductSchemeIsBestBeforeDateTracked;
                    scheme.IsVariantTracked = prodSchemes[0].ProductSchemeIsVariantTracked;
                    scheme.IsLotUniqueComponent = prodSchemes[0].ProductSchemeIsLotUniqueComponent;
                    scheme.IsProductionDateUniqueComponent = prodSchemes[0].ProductSchemeIsProductionDateUniqueComponent;
                    scheme.IsBestBeforeDateUniqueComponent = prodSchemes[0].ProductSchemeIsBestBeforeDateUniqueComponent;
                    scheme.IsVariantUniqueComponent = prodSchemes[0].ProductSchemeIsVariantUniqueComponent;
                    scheme.Extra1Type = prodSchemes[0].ProductSchemeExtra1Type;
                    scheme.Extra2Type = prodSchemes[0].ProductSchemeExtra2Type;
                    scheme.Extra3Type = prodSchemes[0].ProductSchemeExtra3Type;
                    scheme.Extra4Type = prodSchemes[0].ProductSchemeExtra4Type;
                    scheme.Extra5Type = prodSchemes[0].ProductSchemeExtra5Type;
                    scheme.Extra1Caption = prodSchemes[0].ProductSchemeExtra1Caption;
                    scheme.Extra2Caption = prodSchemes[0].ProductSchemeExtra2Caption;
                    scheme.Extra3Caption = prodSchemes[0].ProductSchemeExtra3Caption;
                    scheme.Extra4Caption = prodSchemes[0].ProductSchemeExtra4Caption;
                    scheme.Extra5Caption = prodSchemes[0].ProductSchemeExtra5Caption;
                    scheme.Extra1IsRequired = prodSchemes[0].ProductSchemeExtra1IsRequired;
                    scheme.Extra2IsRequired = prodSchemes[0].ProductSchemeExtra2IsRequired;
                    scheme.Extra3IsRequired = prodSchemes[0].ProductSchemeExtra3IsRequired;
                    scheme.Extra4IsRequired = prodSchemes[0].ProductSchemeExtra4IsRequired;
                    scheme.Extra5IsRequired = prodSchemes[0].ProductSchemeExtra5IsRequired;
                    scheme.IsLotTrackedDefault = prodSchemes[0].ProductSchemeIsLotTrackedDefault;
                    scheme.IsProductionDateTrackedDefault = prodSchemes[0].ProductSchemeIsProductionDateTrackedDefault;
                    scheme.IsBestBeforeDateTrackedDefault = prodSchemes[0].ProductSchemeIsBestBeforeDateTrackedDefault;

                    scheme.LabelInfo = labelScheme.GetSchemeInfoFromSchemeRefno(scheme.SchemeRefno);
                }
            }

            return scheme;
        }

        /// <summary>
        /// Get the factor between two product units.
        /// </summary>
        /// <param name="productInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="decimal"/> Relationship factor.</returns>
        public static async Task<decimal> GetUnitRelationFactor(ProductInfo productInfo)
        {
            decimal prodUnitFactor = 0;

            if (productInfo.UnitRefno != 0)
            {
                // NOTE: I don't think this code is supporting broken case properly.
                const string query = @"
                    SELECT tProductUnitRelation.ProductUnitRelationFactor AS ProductUnitRelationFactor
                        FROM tProductUnitRelation
                        INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit1Refno
                        INNER JOIN tProductUnit AS tProductBaseUnit ON tProductBaseUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit2Refno
                        INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                        INNER JOIN tUnit AS tBaseUnit ON tBaseUnit.UnitRefno = tProductBaseUnit.BaseUnitRefno
                        WHERE tProductUnit.ProductRefno = ?
                        AND tUnit.UnitCode = ?
                        AND tBaseUnit.UnitCode = ?";

                prodUnitFactor = Convert.ToDecimal(await DatabaseConnection.GetInstance().SqlConn.
                    ExecuteScalarAsync<decimal>(
                    query,
                    Convert.ToInt32(productInfo.Refno),
                    Convert.ToString(productInfo.UnitCode),
                    Convert.ToString(productInfo.BaseUnitCode)));

                if (prodUnitFactor == 0)
                {
                    throw new Exception($"Unable to find product '{productInfo.Code}' unit relation factor between" +
                                        $" {productInfo.UnitCode} and {productInfo.BaseUnitCode}");
                }
            }

            return prodUnitFactor;
        }

        /// <summary>
        /// Get the next product scheme single trade item serial number.
        /// </summary>
        /// <returns><see cref="string"/> Product scheme single trade item serial number.</returns>
        public static string GetProdSchemeSingleTradeItemSerialNumber()
        {
            var singleTradeItemSerialNumber = string.Empty;

            var sysEnv = SysEnvironment.GetInstance();

            if (!sysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlString = new StringBuilder();
                var xml = XmlWriter.Create(xmlString);
                var deviceId = sysEnv.DeviceId;
                var revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString(
                    "yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
                var loginId = sysEnv.ContactLog.LoginID.ToString();
                var current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
                var request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST PRODUCT SCHEME SERIAL NUMBER</action>" +
                    "</command>" +
                    "<device_id>" + deviceId + "</device_id>" +
                    "<login_id>" + loginId + "</login_id>" +
                    "<revision_datetime>" + revision + "</revision_datetime>" +
                    "<stamp>" + current + "</stamp>" +
                    "</imclient>");
                request.Save(xml);
                xml.Flush();
                xml.Close();

                var responseData = SysCommon.SubmitComm(xmlString.ToString());
                var xmlResult = new XmlDocument();
                xmlResult.LoadXml(responseData);

                singleTradeItemSerialNumber = xmlResult.SelectSingleNode(
                    "/VFPData/SingleTradeUnitSerialNumber") !.FirstChild.Value.PadRight(20);
            }

            return singleTradeItemSerialNumber;
        }

        /// <summary>
        /// Get a list of product units.
        /// </summary>
        /// <returns><see cref="List{ProductUnit}"/> List of product units.</returns>
        public static async Task<List<ProductUnit>> GetProductUnitList()
        {
            const string query = @"
                SELECT tProductUnit.ProductUnitRefno AS ProductUnitRefno
                        ,tProduct.ProductCode AS ProductCode
                        ,tProduct.ProductDescription AS ProductDescription
                        ,tUnit.UnitCode AS UnitCode
                        ,aUnitBase.UnitCode AS BaseUnitCode
                        ,aUnitWeight.UnitCode AS WeightUnitCode
                        ,tProductUnit.ProductUnitBC AS ProductUnitBC
                        ,tProductUnit.ProductUnitIsBC AS ProductUnitIsBC
                        ,tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription
                        ,tProduct.ProductRefno AS ProductRefno
                        ,tProductUnit.ProductUnitIsSellUnit AS ProductUnitIsSellUnit
                        ,tProductUnit.ProductUnitIsDefault AS ProductUnitIsDefault 
                    FROM tProductUnit
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    ORDER BY tProduct.ProductCode, tProductUnit.ProductUnitBC, tUnit.UnitCode";

            return await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductUnit>(query);
        }

        /// <summary>
        /// Get a list of products.
        /// </summary>
        /// <returns><see cref="List{Product}"/> List of products.</returns>
        public static async Task<List<Product>> GetProductList()
        {
            const string query = @"
                SELECT ProductRefno
                        ,ProductCode
                        ,ProductDescription 
                    FROM tProduct 
                    ORDER BY ProductCode";

            return await DatabaseConnection.GetInstance().SqlConn.QueryAsync<Product>(query);
        }

        /// <summary>
        /// Gets the product list with an extra column for ProductAllowConvertProduct flag.
        /// </summary>
        /// <returns><see cref="List{Product}"/> List of products.</returns>
        public static async Task<List<Product>> GetProductListAllowConvertProduct()
        {
            const string query = @"
                SELECT ProductRefno
                        ,ProductCode
                        ,ProductDescription
                        ,ProductAllowConvertProduct
                    FROM tProduct
                    WHERE ProductAllowConvertProduct = 1
                    ORDER BY ProductCode";

            return await DatabaseConnection.GetInstance().SqlConn.QueryAsync<Product>(query);
        }

        /// <summary>
        /// Get the unique scheme id from the product scheme data.
        /// </summary>
        /// <param name="prodCode"><see cref="string"/> Product code.</param>
        /// <param name="prodSchemeComponents"><see cref="ProdScheme"/> Scheme components.</param>
        /// <param name="prodSchemeData"><see cref="ProductSchemeData"/> Scheme data.</param>
        /// <returns><see cref="string"/> Unique scheme id.</returns>
        public static string GetUniqueSchemeIdFromSProdSchemeData(
            string prodCode,
            ProdScheme prodSchemeComponents,
            ProductSchemeData prodSchemeData)
        {
            _ = prodCode ?? throw new ArgumentNullException(nameof(prodCode));
            var schemeId = string.Empty;

            if (prodSchemeComponents.IsSerialTracked)
            {
                schemeId = $"{schemeId}{prodCode.TrimEnd()}.";

                if (!string.IsNullOrEmpty(prodSchemeData.SingleTradeItemSerialNumber))
                {
                    schemeId = $"{schemeId}SN:{prodSchemeData.SingleTradeItemSerialNumber.TrimEnd()}.";
                }

                if (prodSchemeComponents.IsLotUniqueComponent)
                {
                    schemeId = $"{schemeId}LT:{prodSchemeData.LogisticUnitBatchLotNumber.TrimEnd()}.";
                }

                if (prodSchemeComponents.IsProductionDateUniqueComponent)
                {
                    schemeId = $"{schemeId}PD:{prodSchemeData.LogisticUnitProductionDate.ToString("yyyyMMdd").TrimEnd()}.";
                }

                if (prodSchemeComponents.IsBestBeforeDateUniqueComponent)
                {
                    schemeId =
                        $"{schemeId}BB:{prodSchemeData.LogisticUnitBestBeforeSellByDate.ToString("yyyyMMdd").TrimEnd()}.";
                }

                if (prodSchemeComponents.IsVariantUniqueComponent)
                {
                    schemeId = $"{schemeId}PV:{prodSchemeData.VariantCode.TrimEnd()}.";
                }

                if (SysEnvironment.GetInstance().SysSettings.IsSkidTrackingModule
                    && !string.IsNullOrWhiteSpace(prodSchemeData.LogisticUnitSkidNumber))
                {
                    schemeId = $"{schemeId}SK:{prodSchemeData.LogisticUnitSkidNumber.TrimEnd()}.";
                }

                if (LocationMap.GetInstance().CurrLocMap.WhsFkRefno != 0)
                {
                    schemeId = $"{schemeId}BL:{LocationMapTasks.GetCurrentLocationForUniquenessChecking().TrimEnd()}.";
                }
            }

            return schemeId;
        }

        /// <summary>
        /// Get the scheme id from the product scheme data.
        /// </summary>
        /// <param name="prodCode"><see cref="string"/> Product code.</param>
        /// <param name="prodSchemeComponents"><see cref="ProdScheme"/> Scheme components.</param>
        /// <param name="prodSchemeData"><see cref="ProductSchemeData"/> Scheme data.</param>
        /// <returns><see cref="string"/> Scheme id.</returns>
        public static string GetActSchemeIdFromSProdSchemeData(
            string prodCode,
            ProdScheme prodSchemeComponents,
            ProductSchemeData prodSchemeData)
        {
            _ = prodCode ?? throw new ArgumentNullException(nameof(prodCode));
            var schemeId = string.Empty;

            if (!string.IsNullOrEmpty(prodSchemeData.SingleTradeItemSerialNumber))
            {
                schemeId = $"{schemeId}SN:{prodSchemeData.SingleTradeItemSerialNumber.TrimEnd()}.";
            }

            if (prodSchemeComponents.IsLotTracked)
            {
                schemeId = $"{schemeId}LT:{prodSchemeData.LogisticUnitBatchLotNumber}.";
            }

            if (prodSchemeComponents.IsProductionDateTracked)
            {
                schemeId = $"{schemeId}PD:{prodSchemeData.LogisticUnitProductionDate:yyyy/MM/dd}.";
            }

            if (prodSchemeComponents.IsBestBeforeDateTracked)
            {
                schemeId = $"{schemeId}BB:{prodSchemeData.LogisticUnitBestBeforeSellByDate:yyyy/MM/dd}.";
            }

            if (prodSchemeComponents.IsVariantTracked)
            {
                schemeId = $"{schemeId}PV:{prodSchemeData.VariantCode}.";
            }

            if (SysEnvironment.GetInstance().SysSettings.IsSkidTrackingModule
                && !string.IsNullOrWhiteSpace(prodSchemeData.LogisticUnitSkidNumber))
            {
                schemeId = $"{schemeId}SK:{prodSchemeData.LogisticUnitSkidNumber.TrimEnd()}.";
            }

            var locationMap = LocationMap.GetInstance();

            if (locationMap.CurrLocMap.WhsFkRefno != 0)
            {
                ////schemeID = schemeID + "BL:" + LocationMapTasks.GetCurrentReadableLocation() + ".";
                schemeId = $"{schemeId}BL:{LocationMapTasks.GetCurrentLocation()}.";
            }

            return schemeId;
        }

        /// <summary>
        /// Check if the unique scheme id is a duplicate.
        /// </summary>
        /// <param name="uniqueSchemeId"><see cref="string"/> Unique scheme id.</param>
        /// <returns><see cref="bool"/> True if the unique scheme id is a duplicate, else false.</returns>
        public static async Task<bool> IsDuplicateUniqueSchemeId(string uniqueSchemeId)
        {
            var isDuplicate = false;

            const string query = @"
                SELECT ActivityDetailUniqueSchemeID
                    FROM tActivityDetail
                    WHERE ActivityDetailUniqueSchemeID = ?";

            var conn = DatabaseConnection.GetInstance().SqlConn;
            var results = await conn.QueryAsync<ActivityDetailModel>(query, uniqueSchemeId);
            if (results != null && results.Count > 0)
            {
                isDuplicate = true;
            }

            return isDuplicate;
        }
    }
}
