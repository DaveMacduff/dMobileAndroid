namespace dMobile.Bedrock
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Data;
    using SQLite;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using dMobile.Resources.QueryResults;

    public static class ProductInfoTasks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetCurrReadableProdBCUnit(ProductInfo prodInfo)
        {
            if (prodInfo == null)
            {
                return string.Empty;
            }

            return prodInfo.Code.TrimEnd() + (" " + prodInfo.UnitBC).TrimEnd() + " " + prodInfo.UnitCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static XDocument GetXML(ProductInfo prodInfo)
        {
            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement("params",
                  new XElement("ProductUnitRefno", prodInfo.UnitRefno),
                  new XElement("ProductRefno", prodInfo.Refno),
                  new XElement("ProductFKRefno", prodInfo.FKRefno),
                  new XElement("ProductCode", prodInfo.Code),
                  new XElement("ProductDescription", prodInfo.Desc),
                  new XElement("UnitCode", prodInfo.UnitCode),
                  new XElement("BaseUnitCode", prodInfo.UnitCode),
                  new XElement("WeightUnitCode", prodInfo.UnitCode),
                  new XElement("ProductUnitBC", prodInfo.UnitBC),
                  new XElement("ProductUnitIsBC", prodInfo.UnitIsBC.ToString()),
                  new XElement("ProductUnitSizeDescription", prodInfo.UnitSizeDesc),
                  new XElement("ProductIsCatchAsk", prodInfo.IsCatchAsk.ToString()),
                  new XElement("ProductIsCatchAskZero", prodInfo.IsCatchAskZero.ToString()),
                  new XElement("ProductAllowConvertProduct", prodInfo.IsAllowConvertProduct.ToString()),
                  new XElement("ProductIsCatchCount", prodInfo.IsCatchCount.ToString()),
                  new XElement("ProductIsCatchWeight", prodInfo.IsCatchWeight.ToString()),
                  new XElement("ProductBrandDescription", prodInfo.BrandDesc),
                  new XElement("ProductUnitIsSellUnit", prodInfo.UnitIsSellUnit.ToString()),
                  new XElement("ProductUnitIsDefault", prodInfo.UnitIsDefault.ToString()),
                  new XElement("ProductUnitBoxCount", prodInfo.BoxCount.ToString()),
                  new XElement("ProductUnitWeightTolerance", prodInfo.WeightTolerance.ToString()),
                  new XElement("ProductUnitWeight", prodInfo.UnitWeight.ToString())));

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="productUnitBC"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public static async Task<ProductInfo> GetInfoFromCodeBCUnit(string productCode, string productUnitBC, string unitCode)
        {
            ProductInfo prodInfo = new ProductInfo();
            StringBuilder sql = new StringBuilder();

            // Private this.m_decProductUnit2ProductBaseUnitFactor As Decimal = 0
            if (unitCode == "SK")
            {
                string query = "SELECT tProduct.ProductFKRefno, " +
                "tProduct.ProductRefno, " +
                "tProduct.ProductCode, " +
                "tProduct.ProductDescription, " +
                "tProduct.ProductIsCatchCount, " +
                "tProduct.ProductIsCatchWeight, " +
                "tProduct.ProductIsCatchAsk, " +
                "tProduct.ProductIsCatchAskZero, " +
                "tProduct.ProductBrandDescription, " +
                "tProduct.ProductAllowConvertProduct " +
                "FROM tProduct " +
                "WHERE tProduct.ProductCode = ?";

                List<tProductModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductModel>(query, productCode);

                if (results != null && results.Count > 0)
                {
                    prodInfo.Refno = Convert.ToInt32(results[0].ProductRefno);
                    prodInfo.FKRefno = Convert.ToInt32(results[0].ProductFKRefno);
                    prodInfo.Code = Convert.ToString(results[0].ProductCode);
                    prodInfo.Desc = Convert.ToString(results[0].ProductDescription);
                    prodInfo.UnitIsBC = Convert.ToBoolean(string.IsNullOrEmpty(productUnitBC) ? false : true);
                    prodInfo.UnitBC = productUnitBC;
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
                string query = "SELECT tProduct.ProductFKRefno, " +
                "tUnit.UnitCode, " +
                "aUnitBase.UnitCode AS BaseUnitCode, " +
                "aUnitWeight.UnitCode AS WeightUnitCode, " +
                "tProduct.ProductRefno, " +
                "tProduct.ProductCode, " +
                "tProduct.ProductDescription, " +
                "tProduct.ProductIsCatchCount, " +
                "tProduct.ProductIsCatchWeight, " +
                "tProduct.ProductIsCatchAsk, " +
                "tProduct.ProductIsCatchAskZero, " +
                "tProduct.ProductAllowConvertProduct, " +
                "tProductUnit.ProductUnitRefno, " +
                "tProductUnit.ProductUnitBC, " +
                "tProductUnit.ProductUnitIsBC, " +
                "tProductUnit.ProductUnitWeight, " +
                "tProductUnit.ProductUnitSizeDescription, " +
                "tProduct.ProductBrandDescription, " +
                "tProductUnit.ProductUnitIsSellUnit, " +
                "tProductUnit.ProductUnitIsDefault, " +
                "tProductUnit.ProductUnitBoxCount, " +
                "tProductUnit.ProductUnitWeightTolerance " +
                "FROM tProductUnit " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
                "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "WHERE tProduct.ProductCode = ? " +
                "AND tProductUnit.ProductUnitBC = ? " +
                "AND tUnit.UnitCode = ?";

                List<ProductInfoQueryResult> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductInfoQueryResult>(query, productCode, productUnitBC, unitCode);

                if (results != null && results.Count > 0)
                {
                    prodInfo.UnitRefno = results[0].ProductUnitRefno;
                    prodInfo.Refno = results[0].ProductRefno;
                    prodInfo.FKRefno = results[0].ProductFKRefno;
                    prodInfo.Code = results[0].ProductCode;
                    prodInfo.Desc = results[0].ProductDescription;
                    prodInfo.UnitCode = results[0].UnitCode;
                    prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                    prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                    prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                    prodInfo.UnitIsBC = results[0].ProductUnitIsBC;
                    prodInfo.UnitBC = results[0].ProductUnitBC;
                    prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                    prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                    prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                    prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                    prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                    prodInfo.UnitWeight = results[0].ProductUnitWeight;
                    prodInfo.BrandDesc = results[0].ProductBrandDescription;
                    prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                    prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                    prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                    prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
                }

                prodInfo.Unit2BaseUnitFactor = await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
                prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);
            }

            return prodInfo;
        }

        public class UnitRefnoFromBarCode
        {
            private decimal deliScaleWeight = 0;
            private string deliScaleWeightUnit = string.Empty;
            private ProductInfo prodInfo = new ProductInfo();

            public decimal DeliScaleWeight { get => this.deliScaleWeight; set => this.deliScaleWeight = value; }
            public string DeliScaleWeightUnit { get => this.deliScaleWeightUnit; set => this.deliScaleWeightUnit = value; }
            public ProductInfo ProdInfo { get => this.prodInfo; set => this.prodInfo = value; }
        }

        public static async Task<UnitRefnoFromBarCode> GetUnitRefnoFromBarCode(
               string productUnitBarCodeCode,
               string productUnitBarCodeModifier1,
               SysCommon.BarCodeSymbologies barCodeSymbology,
               decimal deliScaleAmount
            )
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();
            UnitRefnoFromBarCode unitRefnoFromBarCode = new UnitRefnoFromBarCode();

            if (!sysEnv.SysSettings.IsDisconnectedMode)
            {
                string sendRequest = string.Empty;
                string data = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                string deviceID = sysEnv.DeviceID;
                string revisionDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                string loginID = sysEnv.ContactLog.LoginID.ToString();
                string currentDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                XmlDocument result = new XmlDocument();

                XDocument request = new XDocument(
                    new XDeclaration("1.0", "utf-16", "yes"),
                    string.Empty,
                    new XElement("imclient",
                    new XElement("command",
                    new XElement("action", "REQUEST PRODUCT FROM BARCODE")),
                    new XElement("device_id", deviceID),
                    new XElement("login_id", loginID),
                    new XElement("revision_datetime", revisionDateTime),
                    new XElement("stamp", currentDateTime),
                    new XElement("barcode", productUnitBarCodeCode),
                    new XElement("modifier1", productUnitBarCodeModifier1),
                    new XElement("symbology", Convert.ToInt32(barCodeSymbology)),
                    new XElement("barcode_amount", deliScaleAmount)));

                request.Save(xml);
                xml.Flush();
                xml.Close();

                sendRequest = xmlString.ToString();
                data = SysCommon.SubmitComm(sendRequest);

                if (data.Length > 0)
                {
                    result.LoadXml(data);

                    // check if result is an error message
                    if (data.Contains("<message>ERROR</message>"))
                    {
                        throw new Exception(result.DocumentElement["detail"].InnerText);
                    }
                    else
                    {
                        if (result.SelectSingleNode("/VFPData/ProductFKRefno").HasChildNodes)
                        {
                            unitRefnoFromBarCode.ProdInfo.FKRefno = Convert.ToInt32(result.SelectSingleNode("/VFPData/ProductFKRefno").FirstChild.Value);
                        }

                        if (result.SelectSingleNode("/VFPData/DeliScaleWeight").HasChildNodes)
                        {
                            unitRefnoFromBarCode.DeliScaleWeight = Convert.ToDecimal(result.SelectSingleNode("/VFPData/DeliScaleWeight").FirstChild.Value);
                        }

                        if (result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").HasChildNodes)
                        {
                            unitRefnoFromBarCode.DeliScaleWeightUnit = Convert.ToString(result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").FirstChild.Value);
                        }

                        if (unitRefnoFromBarCode.ProdInfo.FKRefno != 0)
                        {
                            unitRefnoFromBarCode.ProdInfo.UnitIsBC = Convert.ToBoolean(result.SelectSingleNode("/VFPData/ProductUnitIsBC").FirstChild.Value);
                            unitRefnoFromBarCode.ProdInfo.UnitCode = result.SelectSingleNode("/VFPData/ProductUnitCode").FirstChild.Value;

                            List<tProductModel> productList = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(unitRefnoFromBarCode.ProdInfo.FKRefno));

                            if (productList != null && productList.Count > 0)
                            {
                                unitRefnoFromBarCode.ProdInfo.Refno = productList[0].ProductRefno;

                                List<tUnitModel> unitList = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", Convert.ToString(unitRefnoFromBarCode.ProdInfo.UnitCode));

                                if (unitList != null && unitList.Count > 0)
                                {
                                    int unitRefno = unitList[0].UnitRefno;

                                    List<tProductUnitModel> productUnitList = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", unitRefnoFromBarCode.ProdInfo.Refno, unitRefno, unitRefnoFromBarCode.ProdInfo.UnitIsBC);

                                    if (productUnitList != null && productUnitList.Count > 0)
                                    {
                                        unitRefnoFromBarCode.ProdInfo.UnitRefno = productUnitList[0].ProductUnitRefno;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return unitRefnoFromBarCode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public static async Task<ProductInfo> GetInfoFromUnitRefno(int productUnitRefno)
        {
            ProductInfo prodInfo = new ProductInfo();

            string query = "SELECT tProduct.ProductFKRefno AS ProductFKRefno, " +
            "tUnit.UnitCode AS UnitCode, " +
            "aUnitBase.UnitCode AS BaseUnitCode, " +
            "aUnitWeight.UnitCode AS WeightUnitCode, " +
            "tProduct.ProductRefno AS ProductRefno, " +
            "tProduct.ProductCode AS ProductCode, " +
            "tProduct.ProductDescription AS ProductDescription, " +
            "tProduct.ProductIsCatchCount AS ProductIsCatchCount, " +
            "tProduct.ProductIsCatchWeight AS ProductIsCatchWeight, " +
            "tProduct.ProductIsCatchAsk AS ProductIsCatchAsk, " +
            "tProduct.ProductIsCatchAskZero AS ProductIsCatchAskZero, " +
            "tProduct.ProductAllowConvertProduct AS ProductAllowConvertProduct, " +
            "tProductUnit.ProductUnitRefno AS ProductUnitRefno, " +
            "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
            "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
            "tProductUnit.ProductUnitWeight AS ProductUnitWeight, " +
            "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
            "tProduct.ProductBrandDescription AS ProductBrandDescription, " +
            "tProductUnit.ProductUnitIsSellUnit AS ProductUnitIsSellUnit, " +
            "tProductUnit.ProductUnitIsDefault AS ProductUnitIsDefault, " +
            "tProductUnit.ProductUnitBoxCount AS ProductUnitBoxCount," +
            "tProductUnit.ProductUnitWeightTolerance AS ProductUnitWeightTolerance " +
            "FROM tProductUnit " +
            "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
            "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
            "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
            "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
            "WHERE tProductUnit.ProductUnitRefno = ?";

            List<ProductInfoQueryResult> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductInfoQueryResult>(query, Convert.ToInt32(productUnitRefno));

            if (results != null && results.Count > 0)
            {
                prodInfo.UnitRefno = results[0].ProductUnitRefno;
                prodInfo.Refno = results[0].ProductRefno;
                prodInfo.FKRefno = results[0].ProductFKRefno;
                prodInfo.Code = results[0].ProductCode;
                prodInfo.Desc = results[0].ProductDescription;
                prodInfo.UnitCode = results[0].UnitCode;
                prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                prodInfo.UnitIsBC = results[0].ProductUnitIsBC;
                prodInfo.UnitBC = results[0].ProductUnitBC;
                prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                prodInfo.UnitWeight = results[0].ProductUnitWeight;
                prodInfo.BrandDesc = results[0].ProductBrandDescription;
                prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
            }

            prodInfo.Unit2BaseUnitFactor = await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            return prodInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productRefno"></param>
        /// <returns></returns>
        public static async Task<ProductInfo> GetInfoFromRefno(int productRefno)
        {
            ProductInfo prodInfo = new ProductInfo();

            string query = "SELECT tProduct.ProductFKRefno, " +
            "tUnit.UnitCode, " +
            "aUnitBase.UnitCode AS BaseUnitCode, " +
            "aUnitWeight.UnitCode AS WeightUnitCode, " +
            "tProduct.ProductRefno, " +
            "tProduct.ProductCode," +
            "tProduct.ProductDescription," +
            "tProduct.ProductIsCatchCount," +
            "tProduct.ProductIsCatchWeight, " +
            "tProduct.ProductIsCatchAsk, " +
            "tProduct.ProductIsCatchAskZero, " +
            "tProduct.ProductAllowConvertProduct, " +
            "tProductUnit.ProductUnitRefno, " +
            "tProductUnit.ProductUnitBC, " +
            "tProductUnit.ProductUnitIsBC, " +
            "tProductUnit.ProductUnitWeight, " +
            "tProductUnit.ProductUnitSizeDescription, " +
            "tProduct.ProductBrandDescription, " +
            "tProductUnit.ProductUnitIsSellUnit, " +
            "tProductUnit.ProductUnitIsDefault, " +
            "tProductUnit.ProductUnitBoxCount," +
            "tProductUnit.ProductUnitWeightTolerance " +
            "FROM tProductUnit " +
            "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
            "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
            "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
            "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
            "WHERE tProduct.ProductRefno = ? " +
            "AND tProductUnit.ProductUnitIsDefault = 1"; // true

            List<ProductInfoQueryResult> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductInfoQueryResult>(query, Convert.ToInt32(productRefno));

            if (results != null && results.Count > 0)
            {
                prodInfo.UnitRefno = results[0].ProductUnitRefno;
                prodInfo.Refno = results[0].ProductRefno;
                prodInfo.FKRefno = results[0].ProductFKRefno;
                prodInfo.Code = results[0].ProductCode;
                prodInfo.Desc = results[0].ProductDescription;
                prodInfo.UnitCode = results[0].UnitCode;
                prodInfo.BaseUnitCode = results[0].BaseUnitCode;
                prodInfo.WeightUnitCode = results[0].WeightUnitCode;
                prodInfo.UnitSizeDesc = results[0].ProductUnitSizeDescription;
                prodInfo.UnitIsBC = results[0].ProductUnitIsBC;
                prodInfo.UnitBC = results[0].ProductUnitBC;
                prodInfo.IsCatchAsk = results[0].ProductIsCatchAsk;
                prodInfo.IsCatchAskZero = results[0].ProductIsCatchAskZero;
                prodInfo.IsAllowConvertProduct = results[0].ProductAllowConvertProduct;
                prodInfo.IsCatchCount = results[0].ProductIsCatchCount;
                prodInfo.IsCatchWeight = results[0].ProductIsCatchWeight;
                prodInfo.UnitWeight = results[0].ProductUnitWeight;
                prodInfo.BrandDesc = results[0].ProductBrandDescription;
                prodInfo.UnitIsDefault = results[0].ProductUnitIsDefault;
                prodInfo.UnitIsSellUnit = results[0].ProductUnitIsSellUnit;
                prodInfo.BoxCount = results[0].ProductUnitBoxCount;
                prodInfo.WeightTolerance = results[0].ProductUnitWeightTolerance;
            }

            prodInfo.Unit2BaseUnitFactor = await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            return prodInfo;
        }

        /// <summary>
        /// Get the product scheme from the product refno and the current location map instance.
        /// </summary>
        /// <param name="productRefno">Product refno</param>
        /// <returns></returns>
        public static async Task<ProdScheme> GetSchemeComponentsFromRefno(int productRefno)
        {
            ProdScheme scheme = new ProdScheme();
            ProdSchemeLabelSettings labelScheme = new ProdSchemeLabelSettings().LoadData();

            string query = "SELECT tProductScheme.ProductSchemeRefno AS ProductSchemeRefno, " +
            "tProductScheme.ProductSchemeIsSerialTracked AS ProductSchemeIsSerialTracked, " +
            "tProductScheme.ProductSchemeIsProductionDateTracked AS ProductSchemeIsProductionDateTracked, " +
            "tProductScheme.ProductSchemeIsLotTracked AS ProductSchemeIsLotTracked, " +
            "tProductScheme.ProductSchemeIsBestBeforeDateTracked AS ProductSchemeIsBestBeforeDateTracked, " +
            "tProductScheme.ProductSchemeIsVariantTracked AS ProductSchemeIsVariantTracked, " +
            "tProductScheme.ProductSchemeIsLotUniqueComponent AS ProductSchemeIsLotUniqueComponent, " +
            "tProductScheme.ProductSchemeIsProductionDateUniqueComponent AS ProductSchemeIsProductionDateUniqueComponent, " +
            "tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent AS ProductSchemeIsBestBeforeDateUniqueComponent, " +
            "tProductScheme.ProductSchemeIsVariantUniqueComponent AS ProductSchemeIsVariantUniqueComponent, " +
            "tProductScheme.ProductSchemeExtra1Type AS ProductSchemeExtra1Type, " +
            "tProductScheme.ProductSchemeExtra2Type AS ProductSchemeExtra2Type, " +
            "tProductScheme.ProductSchemeExtra3Type AS ProductSchemeExtra3Type, " +
            "tProductScheme.ProductSchemeExtra4Type AS ProductSchemeExtra4Type, " +
            "tProductScheme.ProductSchemeExtra5Type AS ProductSchemeExtra5Type, " +
            "tProductScheme.ProductSchemeExtra1Caption AS ProductSchemeExtra1Caption, " +
            "tProductScheme.ProductSchemeExtra2Caption AS ProductSchemeExtra2Caption, " +
            "tProductScheme.ProductSchemeExtra3Caption AS ProductSchemeExtra3Caption, " +
            "tProductScheme.ProductSchemeExtra4Caption AS ProductSchemeExtra4Caption, " +
            "tProductScheme.ProductSchemeExtra5Caption AS ProductSchemeExtra5Caption, " +
            "tProductScheme.ProductSchemeExtra1IsRequired AS ProductSchemeExtra1IsRequired, " +
            "tProductScheme.ProductSchemeExtra2IsRequired AS ProductSchemeExtra2IsRequired, " +
            "tProductScheme.ProductSchemeExtra3IsRequired AS ProductSchemeExtra3IsRequired, " +
            "tProductScheme.ProductSchemeExtra4IsRequired AS ProductSchemeExtra4IsRequired, " +
            "tProductScheme.ProductSchemeExtra5IsRequired AS ProductSchemeExtra5IsRequired, " +
            "tProductScheme.ProductSchemeIsLotTrackedDefault AS ProductSchemeIsLotTrackedDefault, " +
            "tProductScheme.ProductSchemeIsProductionDateTrackedDefault AS ProductSchemeIsProductionDateTrackedDefault, " +
            "tProductScheme.ProductSchemeIsBestBeforeDateTrackedDefault AS ProductSchemeIsBestBeforeDateTrackedDefault " +
            "FROM tProduct " +
            "INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno " +
            "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ? " +
            "INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tLocationMapWarehouseProductScheme.ProductSchemeRefno " +
            "WHERE tProduct.ProductRefno = ?";

            List<tProductSchemeModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductSchemeModel>(query, Convert.ToInt32(LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno), Convert.ToInt32(productRefno));

            if (results != null && results.Count > 0)
            {
                scheme.SchemeRefno = Convert.ToInt32(results[0].ProductSchemeRefno);

                if (LocationMap.GetInstance().CurrLocMap.IsWhsWMS)
                {
                    scheme.IsSerialTracked = results[0].ProductSchemeIsSerialTracked;
                    scheme.IsProductionDateTracked = results[0].ProductSchemeIsProductionDateTracked;
                    scheme.IsLotTracked = results[0].ProductSchemeIsLotTracked;
                    scheme.IsBestBeforeDateTracked = results[0].ProductSchemeIsBestBeforeDateTracked;
                    scheme.IsVariantTracked = results[0].ProductSchemeIsVariantTracked;
                    scheme.IsLotUniqueComponent = results[0].ProductSchemeIsLotUniqueComponent;
                    scheme.IsProductionDateUniqueComponent = results[0].ProductSchemeIsProductionDateUniqueComponent;
                    scheme.IsBestBeforeDateUniqueComponent = results[0].ProductSchemeIsBestBeforeDateUniqueComponent;
                    scheme.IsVariantUniqueComponent = results[0].ProductSchemeIsVariantUniqueComponent;
                    scheme.Extra1Type = results[0].ProductSchemeExtra1Type;
                    scheme.Extra2Type = results[0].ProductSchemeExtra2Type;
                    scheme.Extra3Type = results[0].ProductSchemeExtra3Type;
                    scheme.Extra4Type = results[0].ProductSchemeExtra4Type;
                    scheme.Extra5Type = results[0].ProductSchemeExtra5Type;
                    scheme.Extra1Caption = results[0].ProductSchemeExtra1Caption;
                    scheme.Extra2Caption = results[0].ProductSchemeExtra2Caption;
                    scheme.Extra3Caption = results[0].ProductSchemeExtra3Caption;
                    scheme.Extra4Caption = results[0].ProductSchemeExtra4Caption;
                    scheme.Extra5Caption = results[0].ProductSchemeExtra5Caption;
                    scheme.Extra1IsRequired = results[0].ProductSchemeExtra1IsRequired;
                    scheme.Extra2IsRequired = results[0].ProductSchemeExtra2IsRequired;
                    scheme.Extra3IsRequired = results[0].ProductSchemeExtra3IsRequired;
                    scheme.Extra4IsRequired = results[0].ProductSchemeExtra4IsRequired;
                    scheme.Extra5IsRequired = results[0].ProductSchemeExtra5IsRequired;
                    scheme.IsLotTrackedDefault = results[0].ProductSchemeIsLotTrackedDefault;
                    scheme.IsProductionDateTrackedDefault = results[0].ProductSchemeIsProductionDateTrackedDefault;
                    scheme.IsBestBeforeDateTrackedDefault = results[0].ProductSchemeIsBestBeforeDateTrackedDefault;

                    scheme.LabelInfo = labelScheme.GetSchemeInfoFromSchemeRefno(scheme.SchemeRefno);
                }
            }

            return scheme;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public static async Task<decimal> GetUnitRelationFactor(ProductInfo productInfo)
        {
            decimal prodUnitFactor = 0;

            if (productInfo.UnitRefno != 0)
            {
                string query = "SELECT tProductUnitRelation.ProductUnitRelationFactor AS ProductUnitRelationFactor " +
                "FROM tProductUnitRelation " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit1Refno " +
                "INNER JOIN tProductUnit AS tProductBaseUnit ON tProductBaseUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit2Refno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tUnit AS tBaseUnit ON tBaseUnit.UnitRefno = tProductBaseUnit.BaseUnitRefno " +
                "WHERE tProductUnit.ProductRefno = ? " +
                "AND tUnit.UnitCode = ? " +
                "AND tBaseUnit.UnitCode = ?";

                prodUnitFactor = Convert.ToDecimal(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<decimal>(query, Convert.ToInt32(productInfo.Refno), Convert.ToString(productInfo.UnitCode), Convert.ToString(productInfo.BaseUnitCode)));

                if (prodUnitFactor == 0)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Unable to find product '{0}' unit relation factor between {1} and {2}", productInfo.Code, productInfo.UnitCode, productInfo.BaseUnitCode));
                }
            }

            return prodUnitFactor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productRefno"></param>
        /// <param name="productUnit1Code"></param>
        /// <param name="productUnit2Code"></param>
        /// <returns></returns>
        public static async Task<decimal> GetUnitRelationFactor(int prodRefno, string prodUnit1Code, string prodUnit2Code)
        {
            decimal prodUnitFactor = 0;

            string query = "SELECT tProductUnitRelation.ProductUnitRelationFactor AS ProductUnitRelationFactor " +
            "FROM tProductUnitRelation " +
            "INNER JOIN tProductUnit AS aProductUnit1 ON aProductUnit1.ProductUnitRefno = tProductUnitRelation.ProductUnit1Refno " +
            "INNER JOIN tProductUnit AS aProductUnit2 ON aProductUnit2.ProductUnitRefno = tProductUnitRelation.ProductUnit2Refno " +
            "INNER JOIN tUnit AS aUnit1 ON aUnit1.UnitRefno = aProductUnit1.UnitRefno " +
            "INNER JOIN tUnit AS aUnit2 ON aUnit2.UnitRefno = aProductUnit2.UnitRefno " +
            "WHERE tProductUnit.ProductRefno = ? " +
            "AND aUnit1.UnitCode = ? " +
            "AND aUnit2.UnitCode = ?";

            prodUnitFactor = Convert.ToDecimal(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<decimal>(query, prodRefno, prodUnit1Code, prodUnit2Code));

            if (prodUnitFactor == 0)
            {
                // TODO: Resource strings
                throw new Exception(string.Format("Unable to find product '{0}' unit relation factor between {1} and {2}", prodRefno, prodUnit1Code, prodUnit2Code));
            }

            return prodUnitFactor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetProdSchemeSingleTradeItemSerialNumber()
        {
            string singleTradeItemSerialNumber = string.Empty;

            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            if (!sysEnv.SysSettings.IsDisconnectedMode)
            {
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                string deviceID = sysEnv.DeviceID;
                string revision = sysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
                string loginID = sysEnv.ContactLog.LoginID.ToString();
                string current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
                XDocument request = null;

                request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<command>" +
                    "<action>REQUEST PRODUCT SCHEME SERIAL NUMBER</action>" +
                    "</command>" +
                    ("<device_id>" + deviceID + "</device_id>") +
                    ("<login_id>" + loginID + "</login_id>") +
                    ("<revision_datetime>" + revision + "</revision_datetime>") +
                    ("<stamp>" + current + "</stamp>") +
                    "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                string reqData = SysCommon.SubmitComm(xmlString.ToString());
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        throw new Exception(docError.DocumentElement["detail"].InnerText);
                    }

                    XmlDocument xmlResult = new XmlDocument();
                    xmlResult.LoadXml(reqData);

                    singleTradeItemSerialNumber = xmlResult.SelectSingleNode("/VFPData/SingleTradeUnitSerialNumber").FirstChild.Value.PadRight(20);
                }
            }

            return singleTradeItemSerialNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableProdUnit"></param>
        public static async Task<List<ProductUnit>> GetProductUnitList()
        {
            string sql = @"SELECT tProductUnit.ProductUnitRefno AS ProductUnitRefno,
                tProduct.ProductCode AS ProductCode,
                tProduct.ProductDescription AS ProductDescription,
                tUnit.UnitCode AS UnitCode,
                aUnitBase.UnitCode AS BaseUnitCode,
                aUnitWeight.UnitCode AS WeightUnitCode,
                tProductUnit.ProductUnitBC AS ProductUnitBC,
                tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,
                tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                tProduct.ProductRefno AS ProductRefno,
                tProductUnit.ProductUnitIsSellUnit AS ProductUnitIsSellUnit,
                tProductUnit.ProductUnitIsDefault AS ProductUnitIsDefault 
                FROM tProductUnit
                INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                ORDER BY tProduct.ProductCode, tProductUnit.ProductUnitBC, tUnit.UnitCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnit>(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableProd"></param>
        public static async Task<List<Product>> GetProductList()
        {
            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Product>("SELECT ProductRefno, ProductCode, ProductDescription FROM tProduct ORDER BY ProductCode");
        }

        /// <summary>
        /// Gets the product list with an extra column for ProductAllowConvertProduct flag.
        /// </summary>
        /// <param name="dataTableProd"></param>
        public static async Task<List<Product>> GetProductListAllowConvertProduct()
        {
            string sql = "SELECT ProductRefno, " +
            "ProductCode, " +
            "ProductDescription, " +
            "ProductAllowConvertProduct " +
            "FROM tProduct " +
            "WHERE ProductAllowConvertProduct = 1 " +
            "ORDER BY ProductCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Product>(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodCode"></param>
        /// <param name="prodSchemeComponents"></param>
        /// <param name="prodSchemeData"></param>
        /// <returns></returns>
        public static string GetActUniqueSchemeIDFromSProdSchemeData(string prodCode, ProdScheme prodSchemeComponents, SProductSchemeData prodSchemeData)
        {
            string schemeID = string.Empty;

            if (prodSchemeComponents.IsSerialTracked)
            {
                schemeID = schemeID + prodCode.TrimEnd() + ".";

                if (!string.IsNullOrEmpty(prodSchemeData.SingleTradeItemSerialNumber))
                {
                    schemeID = schemeID + "SN:" + prodSchemeData.SingleTradeItemSerialNumber.TrimEnd() + ".";
                }

                if (prodSchemeComponents.IsLotUniqueComponent)
                {
                    schemeID = schemeID + "LT:" + prodSchemeData.LogisticUnitBatchLotNumber.TrimEnd() + ".";
                }

                if (prodSchemeComponents.IsProductionDateUniqueComponent)
                {
                    schemeID = schemeID + "PD:" + prodSchemeData.LogisticUnitProductionDate.ToString("yyyyMMdd").TrimEnd() + ".";
                }

                if (prodSchemeComponents.IsBestBeforeDateUniqueComponent)
                {
                    schemeID = schemeID + "BB:" + prodSchemeData.LogisticUnitBestBeforeSellByDate.ToString("yyyyMMdd").TrimEnd()
                        + ".";
                }

                if (prodSchemeComponents.IsVariantUniqueComponent)
                {
                    schemeID = schemeID + "PV:" + prodSchemeData.VariantCode.TrimEnd() + ".";
                }

                if (SysEnvironment.GetInstance().SysSettings.IsSkidTrackingModule)
                {
                    schemeID = schemeID + "SK:" + prodSchemeData.LogisticUnitSkidNumber.TrimEnd() + ".";
                }

                LocationMap locMap = LocationMap.GetInstance();

                if (locMap.CurrLocMap.WhsFKRefno != 0)
                {
                    schemeID = schemeID + "BL:" + LocationMapTasks.GetCurrentLocationForUniquenessChecking().TrimEnd() + ".";
                }
            }

            return schemeID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodCode"></param>
        /// <param name="prodSchemeComponents"></param>
        /// <param name="prodSchemeData"></param>
        /// <returns></returns>
        public static string GetActSchemeIDFromSProdSchemeData(string prodCode, ProdScheme prodSchemeComponents, SProductSchemeData prodSchemeData)
        {
            string schemeID = string.Empty;

            if (!string.IsNullOrEmpty(prodSchemeData.SingleTradeItemSerialNumber))
            {
                schemeID = schemeID + "SN:" + prodSchemeData.SingleTradeItemSerialNumber.TrimEnd() + ".";
            }

            if (prodSchemeComponents.IsLotTracked)
            {
                schemeID = schemeID + "LT:" + prodSchemeData.LogisticUnitBatchLotNumber + ".";
            }

            if (prodSchemeComponents.IsProductionDateTracked)
            {
                schemeID = schemeID + "PD:" + prodSchemeData.LogisticUnitProductionDate.ToString("yyyy/MM/dd") + ".";
            }

            if (prodSchemeComponents.IsBestBeforeDateTracked)
            {
                schemeID = schemeID + "BB:" + prodSchemeData.LogisticUnitBestBeforeSellByDate.ToString("yyyy/MM/dd") + ".";
            }

            if (prodSchemeComponents.IsVariantTracked)
            {
                schemeID = schemeID + "PV:" + prodSchemeData.VariantCode + ".";
            }

            if (SysEnvironment.GetInstance().SysSettings.IsSkidTrackingModule)
            {
                schemeID = schemeID + "SK:" + prodSchemeData.LogisticUnitSkidNumber.TrimEnd() + ".";
            }

            LocationMap locationMap = LocationMap.GetInstance();

            if (locationMap.CurrLocMap.WhsFKRefno != 0)
            {
                schemeID = schemeID + "BL:" + LocationMapTasks.GetCurrentReadableLocation() + ".";
            }

            return schemeID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueSchemeID"></param>
        /// <returns></returns>
        public static async Task<bool> IsDuplicateUniqueSchemeID(string unqSchemeID)
        {
            bool isDuplicate = false;

            string query = "SELECT ActivityDetailUniqueSchemeID FROM tActivityDetail WHERE ActivityDetailUniqueSchemeID = ?";
            List<tActivityDetailModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityDetailModel>(query, unqSchemeID);

            if (results != null && results.Count > 0)
            {
                isDuplicate = true;
            }

            return isDuplicate;
        }
    }
}
