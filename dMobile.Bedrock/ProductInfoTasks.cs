namespace dMobile.Bedrock
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using SQLite;
    using System.Data;

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
        public static ProductInfo GetInfoFromCodeBCUnit(string productCode, string productUnitBC, string unitCode)
        {
            ProductInfo prodInfo = new ProductInfo();
            StringBuilder sql = new StringBuilder();

            // Private this.m_decProductUnit2ProductBaseUnitFactor As Decimal = 0
            if (unitCode == "SK")
            {
                sql.Append("SELECT tProduct.ProductFKRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductIsCatchCount,");
                sql.Append("tProduct.ProductIsCatchWeight,");
                sql.Append("tProduct.ProductIsCatchAsk,");
                sql.Append("tProduct.ProductIsCatchAskZero,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProduct.ProductAllowConvertProduct");
                sql.Append(" FROM tProduct");
                sql.AppendFormat(" WHERE tProduct.ProductCode = '{0}'", productCode);

                using (SQLiteConnection cmndProdInfo = DatabaseConnection.GetInstance().SQLConn)
                {
                    cmndProdInfo.CommandType = CommandType.Text;
                    cmndProdInfo.CommandText = sql.ToString();
                    using (SqlCeDataReader drProdInfo = cmndProdInfo.ExecuteReader())
                    {
                        if (drProdInfo.Read())
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

                        drProdInfo.Close();
                    }
                }
            }
            else
            {
                sql.Append("SELECT tProduct.ProductFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WeightUnitCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductIsCatchCount,");
                sql.Append("tProduct.ProductIsCatchWeight,");
                sql.Append("tProduct.ProductIsCatchAsk,");
                sql.Append("tProduct.ProductIsCatchAskZero,");
                sql.Append("tProduct.ProductAllowConvertProduct,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitWeight,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitIsSellUnit,");
                sql.Append("tProductUnit.ProductUnitIsDefault,");
                sql.Append("tProductUnit.ProductUnitBoxCount");
                sql.Append(" FROM tProductUnit");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
                sql.Append(" INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno");
                sql.Append(" INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
                sql.AppendFormat(" WHERE tProduct.ProductCode = '{0}'", productCode);
                sql.AppendFormat(" AND tProductUnit.ProductUnitBC = '{0}'", productUnitBC);
                sql.AppendFormat(" AND tUnit.UnitCode = '{0}'", unitCode);

                using (SqlCeCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndProdInfo.CommandType = CommandType.Text;
                    cmndProdInfo.CommandText = sql.ToString();
                    using (SqlCeDataReader drProdInfo = cmndProdInfo.ExecuteReader())
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
            }

            return prodInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitBarCodeCode"></param>
        /// <param name="productUnitBarCodeModifier1"></param>
        /// <param name="barCodeSymbology"></param>
        /// <param name="deliScaleAmount"></param>
        /// <param name="deliScaleWeight"></param>
        /// <param name="deliScaleWeightUnit"></param>
        /// <returns></returns>
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
                string revisionDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString(
                    "yyyy'-'MM'-'dd'T'HH':'mm':'ss");
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
                            deliScaleWeightUnit = Convert.ToString(result.SelectSingleNode(
                                "/VFPData/DeliScaleWeightUnit").FirstChild.Value);
                        }

                        if (prodInfo.FKRefno != 0)
                        {
                            prodInfo.UnitIsBC = Convert.ToBoolean(result.SelectSingleNode("/VFPData/ProductUnitIsBC").FirstChild.Value);
                            prodInfo.UnitCode = result.SelectSingleNode("/VFPData/ProductUnitCode").FirstChild.Value;

                            using (SqlCeCommand cmndProdUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            {
                                cmndProdUnit.CommandText = "tProductUnit";
                                cmndProdUnit.CommandType = CommandType.TableDirect;
                                cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                                using (SqlCeDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                                {
                                    using (SqlCeCommand cmndProd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                    {
                                        cmndProd.CommandText = "tProduct";
                                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                                        cmndProd.CommandType = CommandType.TableDirect;

                                        using (SqlCeDataReader drProd = cmndProd.ExecuteReader())
                                        {
                                            using (SqlCeCommand cmndUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                            {
                                                cmndUnit.CommandText = "tUnit";
                                                cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                                                cmndUnit.CommandType = CommandType.TableDirect;

                                                using (SqlCeDataReader drUnit = cmndUnit.ExecuteReader())
                                                {
                                                    drProd.Seek(DbSeekOptions.FirstEqual, new object[] { prodInfo.FKRefno });
                                                    if (drProd.Read())
                                                    {
                                                        prodInfo.Refno = drProd.GetInt32((int)SysCommon.ProductFields.Refno);

                                                        drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodInfo.UnitCode });
                                                        if (drUnit.Read())
                                                        {
                                                            int unitRefno = drUnit.GetInt32((int)SysCommon.UnitFields.UnitRefno);
                                                            if (drProdUnit.Seek(
                                                                DbSeekOptions.FirstEqual, new object[] { prodInfo.Refno, unitRefno, prodInfo.UnitIsBC }))
                                                            {
                                                                if (drProdUnit.Read())
                                                                {
                                                                    prodInfo.UnitRefno = drProdUnit.GetInt32((int)SysCommon.ProdUnitFields.Refno);
                                                                }
                                                            }
                                                        }
                                                    }

                                                    drUnit.Close();
                                                }
                                            }

                                            drProd.Close();
                                        }
                                    }

                                    drProdUnit.Close();
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
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT tProduct.ProductFKRefno,");
            sql.Append("tUnit.UnitCode,");
            sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
            sql.Append("aUnitWeight.UnitCode AS WeightUnitCode,");
            sql.Append("tProduct.ProductRefno,");
            sql.Append("tProduct.ProductCode,");
            sql.Append("tProduct.ProductDescription,");
            sql.Append("tProduct.ProductIsCatchCount,");
            sql.Append("tProduct.ProductIsCatchWeight,");
            sql.Append("tProduct.ProductIsCatchAsk,");
            sql.Append("tProduct.ProductIsCatchAskZero,");
            sql.Append("tProduct.ProductAllowConvertProduct,");
            sql.Append("tProductUnit.ProductUnitRefno,");
            sql.Append("tProductUnit.ProductUnitBC,");
            sql.Append("tProductUnit.ProductUnitIsBC,");
            sql.Append("tProductUnit.ProductUnitWeight,");
            sql.Append("tProductUnit.ProductUnitSizeDescription,");
            sql.Append("tProduct.ProductBrandDescription,");
            sql.Append("tProductUnit.ProductUnitIsSellUnit,");
            sql.Append("tProductUnit.ProductUnitIsDefault,");
            sql.Append("tProductUnit.ProductUnitBoxCount");
            sql.Append(" FROM tProductUnit");
            sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
            sql.Append(" INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno");
            sql.Append(" INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno");
            sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
            sql.AppendFormat(" WHERE tProductUnit.ProductUnitRefno = {0}", productUnitRefno);

            using (SqlCeCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdInfo.CommandType = CommandType.Text;
                cmndProdInfo.CommandText = sql.ToString();
                using (SqlCeDataReader drProdInfo = cmndProdInfo.ExecuteReader())
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
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT tProduct.ProductFKRefno,");
            sql.Append("tUnit.UnitCode,");
            sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
            sql.Append("aUnitWeight.UnitCode AS WeightUnitCode,");
            sql.Append("tProduct.ProductRefno,");
            sql.Append("tProduct.ProductCode,");
            sql.Append("tProduct.ProductDescription,");
            sql.Append("tProduct.ProductIsCatchCount,");
            sql.Append("tProduct.ProductIsCatchWeight,");
            sql.Append("tProduct.ProductIsCatchAsk,");
            sql.Append("tProduct.ProductIsCatchAskZero,");
            sql.Append("tProduct.ProductAllowConvertProduct,");
            sql.Append("tProductUnit.ProductUnitRefno,");
            sql.Append("tProductUnit.ProductUnitBC,");
            sql.Append("tProductUnit.ProductUnitIsBC,");
            sql.Append("tProductUnit.ProductUnitWeight,");
            sql.Append("tProductUnit.ProductUnitSizeDescription,");
            sql.Append("tProduct.ProductBrandDescription,");
            sql.Append("tProductUnit.ProductUnitIsSellUnit,");
            sql.Append("tProductUnit.ProductUnitIsDefault,");
            sql.Append("tProductUnit.ProductUnitBoxCount");
            sql.Append(" FROM tProductUnit");
            sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno");
            sql.Append(" INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno");
            sql.Append(" INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno");
            sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
            sql.AppendFormat(" WHERE tProduct.ProductRefno = {0}", productRefno);
            sql.Append(" AND tProductUnit.ProductUnitIsDefault = 1");       // true

            using (SqlCeCommand cmndProdInfo = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdInfo.CommandType = CommandType.Text;
                cmndProdInfo.CommandText = sql.ToString();
                using (SqlCeDataReader drProdInfo = cmndProdInfo.ExecuteReader())
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
        public static ProdScheme GetSchemeComponentsFromRefno(int productRefno)
        {
            ProdScheme scheme = new ProdScheme();
            StringBuilder sql = new StringBuilder();
            ProdSchemeLabelSettings labelScheme = new ProdSchemeLabelSettings().LoadData();

            sql.Append("SELECT tProductScheme.ProductSchemeRefno,");
            sql.Append("tProductScheme.ProductSchemeIsSerialTracked,");
            sql.Append("tProductScheme.ProductSchemeIsProductionDateTracked,");
            sql.Append("tProductScheme.ProductSchemeIsLotTracked,");
            sql.Append("tProductScheme.ProductSchemeIsBestBeforeDateTracked,");
            sql.Append("tProductScheme.ProductSchemeIsVariantTracked,");
            sql.Append("tProductScheme.ProductSchemeIsLotUniqueComponent,");
            sql.Append("tProductScheme.ProductSchemeIsProductionDateUniqueComponent,");
            sql.Append("tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent,");
            sql.Append("tProductScheme.ProductSchemeIsVariantUniqueComponent,");
            sql.Append("tProductScheme.ProductSchemeExtra1Type,");
            sql.Append("tProductScheme.ProductSchemeExtra2Type,");
            sql.Append("tProductScheme.ProductSchemeExtra3Type,");
            sql.Append("tProductScheme.ProductSchemeExtra4Type,");
            sql.Append("tProductScheme.ProductSchemeExtra5Type,");
            sql.Append("tProductScheme.ProductSchemeExtra1Caption,");
            sql.Append("tProductScheme.ProductSchemeExtra2Caption,");
            sql.Append("tProductScheme.ProductSchemeExtra3Caption,");
            sql.Append("tProductScheme.ProductSchemeExtra4Caption,");
            sql.Append("tProductScheme.ProductSchemeExtra5Caption,");
            sql.Append("tProductScheme.ProductSchemeExtra1IsRequired,");
            sql.Append("tProductScheme.ProductSchemeExtra2IsRequired,");
            sql.Append("tProductScheme.ProductSchemeExtra3IsRequired,");
            sql.Append("tProductScheme.ProductSchemeExtra4IsRequired,");
            sql.Append("tProductScheme.ProductSchemeExtra5IsRequired,");
            sql.Append("tProductScheme.ProductSchemeIsLotTrackedDefault,");
            sql.Append("tProductScheme.ProductSchemeIsProductionDateTrackedDefault,");
            sql.Append("tProductScheme.ProductSchemeIsBestBeforeDateTrackedDefault ");
            sql.Append("FROM tProduct");
            sql.Append(" INNER JOIN tProductScheme ON tProductScheme.ProductSchemeRefno = tProduct.ProductSchemeRefno");
            sql.AppendFormat(" WHERE tProduct.ProductRefno = {0}", productRefno);

            using (SqlCeCommand cmndProductSchemeComponents = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProductSchemeComponents.CommandText = sql.ToString();
                cmndProductSchemeComponents.CommandType = CommandType.Text;

                using (SqlCeDataReader dr = cmndProductSchemeComponents.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        scheme.SchemeRefno = Convert.ToInt32(dr["ProductSchemeRefno"]);
                        scheme.IsSerialTracked = Convert.ToBoolean(dr["ProductSchemeIsSerialTracked"]);
                        scheme.IsProductionDateTracked = Convert.ToBoolean(dr["ProductSchemeIsProductionDateTracked"]);
                        scheme.IsLotTracked = Convert.ToBoolean(dr["ProductSchemeIsLotTracked"]);
                        scheme.IsBestBeforeDateTracked = Convert.ToBoolean(dr["ProductSchemeIsBestBeforeDateTracked"]);
                        scheme.IsVariantTracked = Convert.ToBoolean(dr["ProductSchemeIsVariantTracked"]);
                        scheme.IsLotUniqueComponent = Convert.ToBoolean(dr["ProductSchemeIsLotUniqueComponent"]);
                        scheme.IsProductionDateUniqueComponent = Convert.ToBoolean(
                            dr["ProductSchemeIsProductionDateUniqueComponent"]);
                        scheme.IsBestBeforeDateUniqueComponent = Convert.ToBoolean(
                            dr["ProductSchemeIsBestBeforeDateUniqueComponent"]);
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
                        scheme.IsProductionDateTrackedDefault = Convert.ToBoolean(
                            dr["ProductSchemeIsProductionDateTrackedDefault"]);
                        scheme.IsBestBeforeDateTrackedDefault = Convert.ToBoolean(
                            dr["ProductSchemeIsBestBeforeDateTrackedDefault"]);

                        scheme.LabelInfo = labelScheme.GetSchemeInfoFromSchemeRefno(scheme.SchemeRefno);
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
                using (SqlCeCommand cmndProdUnitRelation = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT tProductUnitRelation.ProductUnitRelationFactor ");
                    sql.Append("FROM tProductUnitRelation ");
                    sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                    sql.Append("= tProductUnitRelation.ProductUnit1Refno ");
                    sql.Append(" INNER JOIN tProductUnit AS tProductBaseUnit ON tProductBaseUnit.ProductUnitRefno ");
                    sql.Append("= tProductUnitRelation.ProductUnit2Refno ");
                    sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                    sql.Append("INNER JOIN tUnit AS tBaseUnit ON tBaseUnit.UnitRefno = tProductBaseUnit.BaseUnitRefno ");
                    sql.AppendFormat("WHERE tProductUnit.ProductRefno = {0}", productInfo.Refno);
                    sql.AppendFormat(" AND tUnit.UnitCode = '{0}'", productInfo.UnitCode);
                    sql.AppendFormat(" AND tBaseUnit.UnitCode = '{0}'", productInfo.BaseUnitCode);

                    cmndProdUnitRelation.CommandType = CommandType.Text;
                    cmndProdUnitRelation.CommandText = sql.ToString();

                    prodUnitFactor = Convert.ToDecimal(cmndProdUnitRelation.ExecuteScalar());
                    if (prodUnitFactor == 0)
                    {
                        throw new Exception(string.Format(
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindProductUnitRelationFactorBetweenAnd),
#else
                            "Unable to find product '{0}' unit relation factor between {1} and {2}",
#endif
                            productInfo.Code,
                            productInfo.UnitCode,
                            productInfo.BaseUnitCode));
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

            using (SqlCeCommand cmndProdUnitRelation = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT tProductUnitRelation.ProductUnitRelationFactor");
                sql.Append(" FROM tProductUnitRelation");
                sql.Append(" INNER JOIN tProductUnit AS aProductUnit1 ON aProductUnit1.ProductUnitRefno ");
                sql.Append("= tProductUnitRelation.ProductUnit1Refno");
                sql.Append(" INNER JOIN tProductUnit AS aProductUnit2 ON aProductUnit2.ProductUnitRefno ");
                sql.Append("= tProductUnitRelation.ProductUnit2Refno");
                sql.Append(" INNER JOIN tUnit AS aUnit1 ON aUnit1.UnitRefno = aProductUnit1.UnitRefno");
                sql.Append(" INNER JOIN tUnit AS aUnit2 ON aUnit2.UnitRefno = aProductUnit2.UnitRefno");
                sql.AppendFormat(" WHERE tProductUnit.ProductRefno = {0}", prodRefno);
                sql.AppendFormat(" AND aUnit1.UnitCode = '{0}'", prodUnit1Code);
                sql.AppendFormat(" AND aUnit2.UnitCode = '{0}'", prodUnit2Code);

                cmndProdUnitRelation.CommandType = CommandType.Text;
                cmndProdUnitRelation.CommandText = sql.ToString();

                prodUnitFactor = Convert.ToDecimal(cmndProdUnitRelation.ExecuteScalar());
                if (prodUnitFactor == 0)
                {
                    throw new Exception(string.Format(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindProductUnitRelationFactorBetweenAnd),
#else
"Unable to find product '{0}' unit relation factor between {1} and {2}",
#endif
 prodRefno,
                        prodUnit1Code,
                        prodUnit2Code));
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
            string sql = @"SELECT tProductUnit.ProductUnitRefno,
                tProduct.ProductCode,
                tProduct.ProductDescription,
                tUnit.UnitCode,
                aUnitBase.UnitCode AS BaseUnitCode,
                aUnitWeight.UnitCode AS WeightUnitCode,
                tProductUnit.ProductUnitBC,
                tProductUnit.ProductUnitIsBC,
                tProductUnit.ProductUnitSizeDescription,
                tProduct.ProductRefno,
                tProductUnit.ProductUnitIsSellUnit,
                tProductUnit.ProductUnitIsDefault
                FROM tProductUnit
                INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                ORDER BY tProduct.ProductCode, tProductUnit.ProductUnitBC, tUnit.UnitCode";

            using (SqlCeDataAdapter dataAdapterProdUnit = new SqlCeDataAdapter(
                sql,
                DatabaseConnection.GetInstance().SQLConn))
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
            string sql = @"SELECT tProduct.ProductRefno,
                                            tProduct.ProductCode,
                                            tProduct.ProductDescription
                                            FROM tProduct
                                            ORDER BY tProduct.ProductCode";

            using (SqlCeDataAdapter dataAdapterProd = new SqlCeDataAdapter(
                sql,
                DatabaseConnection.GetInstance().SQLConn))
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
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tProduct.ProductRefno,");
            sql.Append("tProduct.ProductCode,");
            sql.Append("tProduct.ProductDescription,");
            sql.Append("tProduct.ProductAllowConvertProduct ");
            sql.Append("FROM tProduct ");
            sql.Append("WHERE tProduct.ProductAllowConvertProduct = 1 ");
            sql.Append("ORDER BY tProduct.ProductCode");

            DataSet ds = new DataSet();
            using (SqlCeDataAdapter dataAdapterProd = new SqlCeDataAdapter(
                sql.ToString(),
                DatabaseConnection.GetInstance().SQLConn))
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
        public static string GetActUniqueSchemeIDFromSProdSchemeData(
            string prodCode,
            ProdScheme prodSchemeComponents,
            SProductSchemeData prodSchemeData)
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
        public static string GetActSchemeIDFromSProdSchemeData(
            string prodCode,
            ProdScheme prodSchemeComponents,
            SProductSchemeData prodSchemeData)
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

            using (SqlCeCommand cmnd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmnd.CommandText = "tActivityDetail";
                cmnd.CommandType = CommandType.TableDirect;
                cmnd.IndexName = "ActivityDetailOnActivityDetailUniqueSchemeID";

                using (SqlCeDataReader dr = cmnd.ExecuteReader())
                {
                    if (dr.Seek(DbSeekOptions.FirstEqual, new object[] { unqSchemeID }))
                    {
                        isDuplicate = true;
                    }
                }
            }

            return isDuplicate;
        }
    }
}
