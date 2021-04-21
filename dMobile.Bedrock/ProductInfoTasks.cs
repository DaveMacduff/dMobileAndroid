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

    public static class ProductInfoTasks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetCurrReadableProdBCUnit(ProductInfo prodInfo)
        {
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

                List<object> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(query, productCode);

                if (results != null && results.Count > 0)
                {
                    prodInfo.Refno = Convert.ToInt32(drProdInfo["ProductRefno"]);
                    prodInfo.FKRefno = Convert.ToInt32(drProdInfo["ProductFKRefno"]);
                    prodInfo.Code = Convert.ToString(drProdInfo["ProductCode"]);
                    prodInfo.Desc = Convert.ToString(drProdInfo["ProductDescription"]);
                    prodInfo.UnitIsBC = Convert.ToBoolean(string.IsNullOrEmpty(productUnitBC) ? false : true);
                    prodInfo.UnitBC = productUnitBC;
                    prodInfo.UnitCode = unitCode;
                    prodInfo.IsCatchAsk = Convert.ToBoolean(drProdInfo["ProductIsCatchAsk"]);
                    prodInfo.IsCatchAskZero = Convert.ToBoolean(drProdInfo["ProductIsCatchAskZero"]);
                    prodInfo.IsAllowConvertProduct = Convert.ToBoolean(drProdInfo["ProductAllowConvertProduct"]);
                    prodInfo.IsCatchCount = Convert.ToBoolean(drProdInfo["ProductIsCatchCount"]);
                    prodInfo.IsCatchWeight = Convert.ToBoolean(drProdInfo["ProductIsCatchWeight"]);
                    prodInfo.BrandDesc = Convert.ToString(drProdInfo["ProductBrandDescription"]);
                }
            }
            else
            {
                using (SQLiteCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndProdInfo.CommandType = CommandType.Text;
                    cmndProdInfo.CommandText = "SELECT tProduct.ProductFKRefno, " +
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
                    "tProductUnit.ProductUnitBoxCount " +
                    "FROM tProductUnit " +
                    "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                    "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
                    "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
                    "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                    "WHERE tProduct.ProductCode = @productCode " +
                    "AND tProductUnit.ProductUnitBC = @productUnitBC " +
                    "AND tUnit.UnitCode = @unitCode";
                    cmndProdInfo.Parameters.Add(new SQLiteParameter("@productCode", productCode));
                    cmndProdInfo.Parameters.Add(new SQLiteParameter("@productUnitBC", productUnitBC));
                    cmndProdInfo.Parameters.Add(new SQLiteParameter("@unitCode", unitCode));

                    using (SQLiteDataReader drProdInfo = cmndProdInfo.ExecuteReader())
                    {
                        if (drProdInfo.Read())
                        {
                            prodInfo.UnitRefno = Convert.ToInt32(drProdInfo["ProductUnitRefno"]);
                            prodInfo.Refno = Convert.ToInt32(drProdInfo["ProductRefno"]);
                            prodInfo.FKRefno = Convert.ToInt32(drProdInfo["ProductFKRefno"]);
                            prodInfo.Code = Convert.ToString(drProdInfo["ProductCode"]);
                            prodInfo.Desc = Convert.ToString(drProdInfo["ProductDescription"]);
                            prodInfo.UnitCode = Convert.ToString(drProdInfo["UnitCode"]);
                            prodInfo.BaseUnitCode = Convert.ToString(drProdInfo["BaseUnitCode"]);
                            prodInfo.WeightUnitCode = Convert.ToString(drProdInfo["WeightUnitCode"]);
                            prodInfo.UnitSizeDesc = Convert.ToString(drProdInfo["ProductUnitSizeDescription"]);
                            prodInfo.UnitIsBC = Convert.ToBoolean(drProdInfo["ProductUnitIsBC"]);
                            prodInfo.UnitBC = Convert.ToString(drProdInfo["ProductUnitBC"]);
                            prodInfo.IsCatchAsk = Convert.ToBoolean(drProdInfo["ProductIsCatchAsk"]);
                            prodInfo.IsCatchAskZero = Convert.ToBoolean(drProdInfo["ProductIsCatchAskZero"]);
                            prodInfo.IsAllowConvertProduct = Convert.ToBoolean(drProdInfo["ProductAllowConvertProduct"]);
                            prodInfo.IsCatchCount = Convert.ToBoolean(drProdInfo["ProductIsCatchCount"]);
                            prodInfo.IsCatchWeight = Convert.ToBoolean(drProdInfo["ProductIsCatchWeight"]);
                            prodInfo.UnitWeight = Convert.ToDecimal(drProdInfo["ProductUnitWeight"]);
                            prodInfo.BrandDesc = Convert.ToString(drProdInfo["ProductBrandDescription"]);
                            prodInfo.UnitIsDefault = Convert.ToBoolean(drProdInfo["ProductUnitIsDefault"]);
                            prodInfo.UnitIsSellUnit = Convert.ToBoolean(drProdInfo["ProductUnitIsSellUnit"]);
                            prodInfo.BoxCount = Convert.ToDecimal(drProdInfo["ProductUnitBoxCount"]);
                        }
                    }
                }

                prodInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(prodInfo);
                prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);
            }

            return prodInfo;
        }

        public static int GetUnitRefnoFromBarCode(
               string productUnitBarCodeCode,
               string productUnitBarCodeModifier1,
               SysCommon.BarCodeSymbologies barCodeSymbology,
               decimal deliScaleAmount,
               ref decimal deliScaleWeight,
               ref string deliScaleWeightUnit,
               ref ProductInfo prodInfo)
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

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
                            prodInfo.FKRefno = Convert.ToInt32(result.SelectSingleNode("/VFPData/ProductFKRefno").FirstChild.Value);
                        }

                        if (result.SelectSingleNode("/VFPData/DeliScaleWeight").HasChildNodes)
                        {
                            deliScaleWeight = Convert.ToDecimal(result.SelectSingleNode("/VFPData/DeliScaleWeight").FirstChild.Value);
                        }

                        if (result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").HasChildNodes)
                        {
                            deliScaleWeightUnit = Convert.ToString(result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").FirstChild.Value);
                        }

                        if (prodInfo.FKRefno != 0)
                        {
                            prodInfo.UnitIsBC = Convert.ToBoolean(result.SelectSingleNode("/VFPData/ProductUnitIsBC").FirstChild.Value);
                            prodInfo.UnitCode = result.SelectSingleNode("/VFPData/ProductUnitCode").FirstChild.Value;

                            using (SQLiteCommand cmndProd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            using (SQLiteCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            using (SQLiteCommand cmndProdUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            {
                                cmndProd.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                                cmndProd.CommandType = CommandType.Text;
                                //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                cmndProd.Parameters.Add(new SQLiteParameter("@productFKRefno", Convert.ToInt32(prodInfo.FKRefno)));

                                using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
                                {
                                    if (drProd.Read())
                                    {
                                        prodInfo.Refno = drProd.GetInt32(Convert.ToInt32(SysCommon.ProductFields.Refno));

                                        cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                                        cmndUnit.CommandType = CommandType.Text;
                                        cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", Convert.ToString(prodInfo.UnitCode)));

                                        using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                                        {
                                            if (drUnit.Read())
                                            {
                                                int unitRefno = drUnit.GetInt32(Convert.ToInt32(SysCommon.UnitFields.UnitRefno));

                                                cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                                                cmndProdUnit.CommandType = CommandType.Text;
                                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(prodInfo.Refno)));
                                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", Convert.ToInt32(unitRefno)));
                                                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", prodInfo.UnitIsBC));

                                                using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                                                {
                                                    if (drProdUnit.Read())
                                                    {
                                                        prodInfo.UnitRefno = drProdUnit.GetInt32(Convert.ToInt32(SysCommon.ProdUnitFields.Refno));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return prodInfo.UnitRefno;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public static ProductInfo GetInfoFromUnitRefno(int productUnitRefno)
        {
            ProductInfo prodInfo = new ProductInfo();

            using (SQLiteCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdInfo.CommandType = CommandType.Text;
                cmndProdInfo.CommandText = "SELECT tProduct.ProductFKRefno AS ProductFKRefno, " +
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
                "tProductUnit.ProductUnitBoxCount AS ProductUnitBoxCount " +
                "FROM tProductUnit " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
                "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "WHERE tProductUnit.ProductUnitRefno = @productUnitRefno";
                cmndProdInfo.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(productUnitRefno)));

                using (SQLiteDataReader drProdInfo = cmndProdInfo.ExecuteReader())
                {
                    if (drProdInfo.Read())
                    {
                        prodInfo.UnitRefno = Convert.ToInt32(drProdInfo["ProductUnitRefno"]);
                        prodInfo.Refno = Convert.ToInt32(drProdInfo["ProductRefno"]);
                        prodInfo.FKRefno = Convert.ToInt32(drProdInfo["ProductFKRefno"]);
                        prodInfo.Code = Convert.ToString(drProdInfo["ProductCode"]);
                        prodInfo.Desc = Convert.ToString(drProdInfo["ProductDescription"]);
                        prodInfo.UnitCode = Convert.ToString(drProdInfo["UnitCode"]);
                        prodInfo.BaseUnitCode = Convert.ToString(drProdInfo["BaseUnitCode"]);
                        prodInfo.WeightUnitCode = Convert.ToString(drProdInfo["WeightUnitCode"]);
                        prodInfo.UnitSizeDesc = Convert.ToString(drProdInfo["ProductUnitSizeDescription"]);
                        prodInfo.UnitIsBC = Convert.ToBoolean(drProdInfo["ProductUnitIsBC"]);
                        prodInfo.UnitBC = Convert.ToString(drProdInfo["ProductUnitBC"]);
                        prodInfo.IsCatchAsk = Convert.ToBoolean(drProdInfo["ProductIsCatchAsk"]);
                        prodInfo.IsCatchAskZero = Convert.ToBoolean(drProdInfo["ProductIsCatchAskZero"]);
                        prodInfo.IsAllowConvertProduct = Convert.ToBoolean(drProdInfo["ProductAllowConvertProduct"]);
                        prodInfo.IsCatchCount = Convert.ToBoolean(drProdInfo["ProductIsCatchCount"]);
                        prodInfo.IsCatchWeight = Convert.ToBoolean(drProdInfo["ProductIsCatchWeight"]);
                        prodInfo.UnitWeight = Convert.ToDecimal(drProdInfo["ProductUnitWeight"]);
                        prodInfo.BrandDesc = Convert.ToString(drProdInfo["ProductBrandDescription"]);
                        prodInfo.UnitIsDefault = Convert.ToBoolean(drProdInfo["ProductUnitIsDefault"]);
                        prodInfo.UnitIsSellUnit = Convert.ToBoolean(drProdInfo["ProductUnitIsSellUnit"]);
                        prodInfo.BoxCount = Convert.ToDecimal(drProdInfo["ProductUnitBoxCount"]);
                    }

                    drProdInfo.Close();
                }
            }

            prodInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(prodInfo);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            return prodInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productRefno"></param>
        /// <returns></returns>
        public static ProductInfo GetInfoFromRefno(int productRefno)
        {
            ProductInfo prodInfo = new ProductInfo();

            using (SQLiteCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdInfo.CommandType = CommandType.Text;
                cmndProdInfo.CommandText = "SELECT tProduct.ProductFKRefno, " +
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
                "tProductUnit.ProductUnitBoxCount " +
                "FROM tProductUnit " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno " +
                "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "WHERE tProduct.ProductRefno = @productRefno " +
                "AND tProductUnit.ProductUnitIsDefault = 1";       // true
                cmndProdInfo.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(productRefno)));

                using (SQLiteDataReader drProdInfo = cmndProdInfo.ExecuteReader())
                {
                    if (drProdInfo.Read())
                    {
                        prodInfo.UnitRefno = Convert.ToInt32(drProdInfo["ProductUnitRefno"]);
                        prodInfo.Refno = Convert.ToInt32(drProdInfo["ProductRefno"]);
                        prodInfo.FKRefno = Convert.ToInt32(drProdInfo["ProductFKRefno"]);
                        prodInfo.Code = Convert.ToString(drProdInfo["ProductCode"]);
                        prodInfo.Desc = Convert.ToString(drProdInfo["ProductDescription"]);
                        prodInfo.UnitCode = Convert.ToString(drProdInfo["UnitCode"]);
                        prodInfo.BaseUnitCode = Convert.ToString(drProdInfo["BaseUnitCode"]);
                        prodInfo.WeightUnitCode = Convert.ToString(drProdInfo["WeightUnitCode"]);
                        prodInfo.UnitSizeDesc = Convert.ToString(drProdInfo["ProductUnitSizeDescription"]);
                        prodInfo.UnitIsBC = Convert.ToBoolean(drProdInfo["ProductUnitIsBC"]);
                        prodInfo.UnitBC = Convert.ToString(drProdInfo["ProductUnitBC"]);
                        prodInfo.IsCatchAsk = Convert.ToBoolean(drProdInfo["ProductIsCatchAsk"]);
                        prodInfo.IsCatchAskZero = Convert.ToBoolean(drProdInfo["ProductIsCatchAskZero"]);
                        prodInfo.IsAllowConvertProduct = Convert.ToBoolean(drProdInfo["ProductAllowConvertProduct"]);
                        prodInfo.IsCatchCount = Convert.ToBoolean(drProdInfo["ProductIsCatchCount"]);
                        prodInfo.IsCatchWeight = Convert.ToBoolean(drProdInfo["ProductIsCatchWeight"]);
                        prodInfo.UnitWeight = Convert.ToDecimal(drProdInfo["ProductUnitWeight"]);
                        prodInfo.BrandDesc = Convert.ToString(drProdInfo["ProductBrandDescription"]);
                        prodInfo.UnitIsDefault = Convert.ToBoolean(drProdInfo["ProductUnitIsDefault"]);
                        prodInfo.UnitIsSellUnit = Convert.ToBoolean(drProdInfo["ProductUnitIsSellUnit"]);
                        prodInfo.BoxCount = Convert.ToDecimal(drProdInfo["ProductUnitBoxCount"]);
                    }

                    drProdInfo.Close();
                }
            }

            prodInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(prodInfo);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            return prodInfo;
        }

        /// <summary>
        /// Get the product scheme from the product refno and the current location map instance.
        /// </summary>
        /// <param name="productRefno">Product refno</param>
        /// <returns></returns>
        public static ProdScheme GetSchemeComponentsFromRefno(int productRefno)
        {
            ProdScheme scheme = new ProdScheme();
            ProdSchemeLabelSettings labelScheme = new ProdSchemeLabelSettings().LoadData();

            using (SQLiteCommand cmndProductSchemeComponents = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProductSchemeComponents.CommandType = CommandType.Text;
                cmndProductSchemeComponents.CommandText = "SELECT tProductScheme.ProductSchemeRefno AS ProductSchemeRefno, " +
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
                "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = @whsLocMapRefno " +
                "INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tLocationMapWarehouseProductScheme.ProductSchemeRefno " +
                "WHERE tProduct.ProductRefno = @productRefno";
                cmndProductSchemeComponents.Parameters.Add(new SQLiteParameter("@whsLocMapRefno", Convert.ToInt32(LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno)));
                cmndProductSchemeComponents.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(productRefno)));

                using (SQLiteDataReader dr = cmndProductSchemeComponents.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        scheme.SchemeRefno = Convert.ToInt32(dr["ProductSchemeRefno"]);
                        
                        if (LocationMap.GetInstance().CurrLocMap.IsWhsWMS)
                        {
                            scheme.IsSerialTracked = Convert.ToBoolean(dr["ProductSchemeIsSerialTracked"]);
                            scheme.IsProductionDateTracked = Convert.ToBoolean(dr["ProductSchemeIsProductionDateTracked"]);
                            scheme.IsLotTracked = Convert.ToBoolean(dr["ProductSchemeIsLotTracked"]);
                            scheme.IsBestBeforeDateTracked = Convert.ToBoolean(dr["ProductSchemeIsBestBeforeDateTracked"]);
                            scheme.IsVariantTracked = Convert.ToBoolean(dr["ProductSchemeIsVariantTracked"]);
                            scheme.IsLotUniqueComponent = Convert.ToBoolean(dr["ProductSchemeIsLotUniqueComponent"]);
                            scheme.IsProductionDateUniqueComponent = Convert.ToBoolean(dr["ProductSchemeIsProductionDateUniqueComponent"]);
                            scheme.IsBestBeforeDateUniqueComponent = Convert.ToBoolean(dr["ProductSchemeIsBestBeforeDateUniqueComponent"]);
                            scheme.IsVariantUniqueComponent = Convert.ToBoolean(dr["ProductSchemeIsVariantUniqueComponent"]);
                            scheme.Extra1Type = Convert.ToInt32(dr["ProductSchemeExtra1Type"]);
                            scheme.Extra2Type = Convert.ToInt32(dr["ProductSchemeExtra2Type"]);
                            scheme.Extra3Type = Convert.ToInt32(dr["ProductSchemeExtra3Type"]);
                            scheme.Extra4Type = Convert.ToInt32(dr["ProductSchemeExtra4Type"]);
                            scheme.Extra5Type = Convert.ToInt32(dr["ProductSchemeExtra5Type"]);
                            scheme.Extra1Caption = Convert.ToString(dr["ProductSchemeExtra1Caption"]);
                            scheme.Extra2Caption = Convert.ToString(dr["ProductSchemeExtra2Caption"]);
                            scheme.Extra3Caption = Convert.ToString(dr["ProductSchemeExtra3Caption"]);
                            scheme.Extra4Caption = Convert.ToString(dr["ProductSchemeExtra4Caption"]);
                            scheme.Extra5Caption = Convert.ToString(dr["ProductSchemeExtra5Caption"]);
                            scheme.Extra1IsRequired = Convert.ToBoolean(dr["ProductSchemeExtra1IsRequired"]);
                            scheme.Extra2IsRequired = Convert.ToBoolean(dr["ProductSchemeExtra2IsRequired"]);
                            scheme.Extra3IsRequired = Convert.ToBoolean(dr["ProductSchemeExtra3IsRequired"]);
                            scheme.Extra4IsRequired = Convert.ToBoolean(dr["ProductSchemeExtra4IsRequired"]);
                            scheme.Extra5IsRequired = Convert.ToBoolean(dr["ProductSchemeExtra5IsRequired"]);
                            scheme.IsLotTrackedDefault = Convert.ToBoolean(dr["ProductSchemeIsLotTrackedDefault"]);
                            scheme.IsProductionDateTrackedDefault = Convert.ToBoolean(dr["ProductSchemeIsProductionDateTrackedDefault"]);
                            scheme.IsBestBeforeDateTrackedDefault = Convert.ToBoolean(dr["ProductSchemeIsBestBeforeDateTrackedDefault"]);

                            scheme.LabelInfo = labelScheme.GetSchemeInfoFromSchemeRefno(scheme.SchemeRefno);
                        }
                    }

                    dr.Close();
                }
            }

            return scheme;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public static decimal GetUnitRelationFactor(ProductInfo productInfo)
        {
            decimal prodUnitFactor = 0;

            if (productInfo.UnitRefno != 0)
            {
                using (SQLiteCommand cmndProdUnitRelation = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndProdUnitRelation.CommandType = CommandType.Text;
                    cmndProdUnitRelation.CommandText = "SELECT tProductUnitRelation.ProductUnitRelationFactor AS ProductUnitRelationFactor " +
                    "FROM tProductUnitRelation " +
                    "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit1Refno " +
                    "INNER JOIN tProductUnit AS tProductBaseUnit ON tProductBaseUnit.ProductUnitRefno = tProductUnitRelation.ProductUnit2Refno " +
                    "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                    "INNER JOIN tUnit AS tBaseUnit ON tBaseUnit.UnitRefno = tProductBaseUnit.BaseUnitRefno " +
                    "WHERE tProductUnit.ProductRefno = @refno " +
                    "AND tUnit.UnitCode = @unitCode " +
                    "AND tBaseUnit.UnitCode = @baseUnitCode";
                    cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@refno", Convert.ToInt32(productInfo.Refno)));
                    cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@unitCode", Convert.ToString(productInfo.UnitCode)));
                    cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@baseUnitCode", Convert.ToString(productInfo.BaseUnitCode)));

                    //var test = cmndProdUnitRelation.ExecuteScalar();
                    prodUnitFactor = Convert.ToDecimal(cmndProdUnitRelation.ExecuteScalar());

                    if (prodUnitFactor == 0)
                    {
                        // TODO: Resource strings
                        throw new Exception(string.Format("Unable to find product '{0}' unit relation factor between {1} and {2}", productInfo.Code, productInfo.UnitCode, productInfo.BaseUnitCode));
                    }
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
        public static decimal GetUnitRelationFactor(int prodRefno, string prodUnit1Code, string prodUnit2Code)
        {
            decimal prodUnitFactor = 0;

            using (SQLiteCommand cmndProdUnitRelation = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdUnitRelation.CommandType = CommandType.Text;
                cmndProdUnitRelation.CommandText = "SELECT tProductUnitRelation.ProductUnitRelationFactor AS ProductUnitRelationFactor " +
                "FROM tProductUnitRelation " +
                "INNER JOIN tProductUnit AS aProductUnit1 ON aProductUnit1.ProductUnitRefno = tProductUnitRelation.ProductUnit1Refno " +
                "INNER JOIN tProductUnit AS aProductUnit2 ON aProductUnit2.ProductUnitRefno = tProductUnitRelation.ProductUnit2Refno " +
                "INNER JOIN tUnit AS aUnit1 ON aUnit1.UnitRefno = aProductUnit1.UnitRefno " +
                "INNER JOIN tUnit AS aUnit2 ON aUnit2.UnitRefno = aProductUnit2.UnitRefno " +
                "WHERE tProductUnit.ProductRefno = @prodRefno " +
                "AND aUnit1.UnitCode = @prodUnit1Code " +
                "AND aUnit2.UnitCode = @prodUnit2Code";
                cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@prodRefno", prodRefno));
                cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@prodUnit1Code", prodUnit1Code));
                cmndProdUnitRelation.Parameters.Add(new SQLiteParameter("@prodUnit2Code", prodUnit2Code));

                prodUnitFactor = Convert.ToDecimal(cmndProdUnitRelation.ExecuteScalar());

                if (prodUnitFactor == 0)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Unable to find product '{0}' unit relation factor between {1} and {2}", prodRefno, prodUnit1Code, prodUnit2Code));
                }
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

                    singleTradeItemSerialNumber = xmlResult.SelectSingleNode(
                        "/VFPData/SingleTradeUnitSerialNumber").FirstChild.Value.PadRight(20);
                }
            }

            return singleTradeItemSerialNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableProdUnit"></param>
        public static void GetProductUnitList(ref DataTable dataTableProdUnit)
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

            using (SQLiteDataAdapter dataAdapterProdUnit = new SQLiteDataAdapter(sql, DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetProdUnit = new DataSet())
                {
                    dataAdapterProdUnit.Fill(dataSetProdUnit);
                    dataTableProdUnit = dataSetProdUnit.Tables[0];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableProd"></param>
        public static void GetProductList(ref DataTable dataTableProd)
        {
            string sql = @"SELECT ProductRefno, ProductCode, ProductDescription FROM tProduct ORDER BY ProductCode";

            using (SQLiteDataAdapter dataAdapterProd = new SQLiteDataAdapter(sql, DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetProd = new DataSet())
                {
                    dataAdapterProd.Fill(dataSetProd);
                    dataTableProd = dataSetProd.Tables[0];
                }
            }
        }

        /// <summary>
        /// Gets the product list with an extra column for ProductAllowConvertProduct flag.
        /// </summary>
        /// <param name="dataTableProd"></param>
        public static void GetProductListAllowConvertProduct(ref DataTable dataTableProd)
        {
            string sql = "SELECT ProductRefno, " +
            "ProductCode, " +
            "ProductDescription, " +
            "ProductAllowConvertProduct " +
            "FROM tProduct " +
            "WHERE ProductAllowConvertProduct = 1 " +
            "ORDER BY ProductCode";

            DataSet ds = new DataSet();
            using (SQLiteDataAdapter dataAdapterProd = new SQLiteDataAdapter(sql, DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetProd = new DataSet())
                {
                    dataAdapterProd.Fill(dataSetProd);
                    dataTableProd = dataSetProd.Tables[0];
                }
            }
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
        public static bool IsDuplicateUniqueSchemeID(string unqSchemeID)
        {
            bool isDuplicate = false;

            using (SQLiteCommand cmnd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmnd.CommandText = "SELECT ActivityDetailUniqueSchemeID FROM tActivityDetail WHERE ActivityDetailUniqueSchemeID = @activityDetailUniqueSchemeID";
                cmnd.CommandType = CommandType.Text;
                cmnd.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", unqSchemeID));

                using (SQLiteDataReader dr = cmnd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        isDuplicate = true;
                    }
                }
            }

            return isDuplicate;
        }
    }
}
