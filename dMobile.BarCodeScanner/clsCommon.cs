// <copyright file="clsCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner
{
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using Org.Apache.Http.Protocol;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Common bar code scanner class methods.
    /// </summary>
    public class clsCommon
    {
        /// <summary>
        /// The GetSymbologyPrefixFromSymbologyEnum.
        /// </summary>
        /// <param name="symbology">.</param>
        /// <returns>.</returns>
        public static string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbologies symbology)
        {
            string symbologyPrefix;
            switch (symbology)
            {
                case BarCodeSymbologies.UNDEFINED:

                    // or ']O4'
                    symbologyPrefix = "]01";
                    break;

                case BarCodeSymbologies.UCC_12_UPC_E:
                    symbologyPrefix = "]X0";
                    break;

                case BarCodeSymbologies.EAN_UCC_14:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.EAN_UCC_13:
                    symbologyPrefix = "]E0";
                    break;

                case BarCodeSymbologies.EAN_UCC_8:
                    symbologyPrefix = "]E4";
                    break;

                case BarCodeSymbologies.ITF_14_Symbol:
                    symbologyPrefix = "]I";
                    break;

                case BarCodeSymbologies.UCC_EAN_128_Symbol:
                    symbologyPrefix = "]C1";
                    break;

                case BarCodeSymbologies.Code_128:
                    symbologyPrefix = "]C";
                    break;

                case BarCodeSymbologies.SSCC_18:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Code_39:
                    symbologyPrefix = "]A";
                    break;

                case BarCodeSymbologies.EAN_UPC_Symbol:
                    symbologyPrefix = "]E";
                    break;

                case BarCodeSymbologies.Code_93:
                    symbologyPrefix = "]G";
                    break;

                case BarCodeSymbologies.Codabar:
                    symbologyPrefix = "]F";
                    break;

                case BarCodeSymbologies.Code_2_of_5:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.MSI:
                    symbologyPrefix = "]M";
                    break;

                case BarCodeSymbologies.Plessey:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Code_11:
                    symbologyPrefix = "]H";
                    break;

                case BarCodeSymbologies.Matrix_2_of_5:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Telepen:
                    symbologyPrefix = "]B";
                    break;

                case BarCodeSymbologies.PDF417:
                    symbologyPrefix = "]L";
                    break;

                case BarCodeSymbologies.Micro_PDF417:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_14:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_Limited:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.RSS_Expanded:
                    symbologyPrefix = "]e0";
                    break;

                case BarCodeSymbologies.Codablock_A:
                    symbologyPrefix = "]O6";
                    break;

                case BarCodeSymbologies.Codablock_F:
                    symbologyPrefix = "]O5";
                    break;

                case BarCodeSymbologies.TLC_39:
                    symbologyPrefix = string.Empty;
                    break;

                case BarCodeSymbologies.Codablock:
                    symbologyPrefix = "]O";
                    break;

                case BarCodeSymbologies.Codablock256:
                    symbologyPrefix = "]02";
                    break;

                default:
                    symbologyPrefix = string.Empty;
                    break;
            }

            return symbologyPrefix;
        }

        /// <summary>
        /// Request product with deli information using a product bar code.
        /// </summary>
        /// <param name="productUnitBarCodeCode"><see cref="string"/> Bar code data.</param>
        /// <param name="productUnitBarCodeModifier1"><see cref="string"/> Bar code modifier character.</param>
        /// <param name="barCodeSymbology"><see cref="SysCommon.BarCodeSymbologies"/> Used for idenitify deli items.</param>
        /// <param name="deliScaleAmount"><see cref="decimal"/> Used for calculating the item weight.</param>
        /// <returns><see cref="DeliProductInfo"/> Product information including deli item information.</returns>
        public static async Task<DeliProductInfo> GetDeliProdInfoFromBarCode(
               string productUnitBarCodeCode,
               string productUnitBarCodeModifier1,
               BarCodeSymbologies barCodeSymbology,
               decimal deliScaleAmount)
        {
            var sysEnv = SysEnvironment.GetInstance();
            var unitRefnoFromBarCode = new DeliProductInfo();

            if (!sysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);
                var revisionDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString(
                    "yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                var currentDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                var result = new XmlDocument();

                var request = new XDocument(
                    new XDeclaration("1.0", "utf-16", "yes"),
                    string.Empty,
                    new XElement(
                        "imclient",
                        new XElement("command", new XElement("action", "REQUEST PRODUCT FROM BARCODE")),
                        new XElement("device_id", sysEnv.DeviceID),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revisionDateTime),
                        new XElement("stamp", currentDateTime),
                        new XElement("barcode", productUnitBarCodeCode),
                        new XElement("modifier1", productUnitBarCodeModifier1),
                        new XElement("symbology", Convert.ToInt32(barCodeSymbology)),
                        new XElement("barcode_amount", deliScaleAmount)));

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                var data = SysCommon.SubmitComm(xmlString.ToString());
                result.LoadXml(data);
                if (result.SelectSingleNode("/VFPData/ProductFKRefno").HasChildNodes)
                {
                    unitRefnoFromBarCode.ProdInfo.FkRefno = Convert.ToInt32(
                        result.SelectSingleNode("/VFPData/ProductFKRefno").FirstChild.Value);
                }

                if (result.SelectSingleNode("/VFPData/DeliScaleWeight").HasChildNodes)
                {
                    unitRefnoFromBarCode.DeliScaleWeight = Convert.ToDecimal(
                        result.SelectSingleNode("/VFPData/DeliScaleWeight").FirstChild.Value);
                }

                if (result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").HasChildNodes)
                {
                    unitRefnoFromBarCode.DeliScaleWeightUnit = Convert.ToString(
                        result.SelectSingleNode("/VFPData/DeliScaleWeightUnit").FirstChild.Value);
                }

                if (unitRefnoFromBarCode.ProdInfo.FkRefno != 0)
                {
                    unitRefnoFromBarCode.ProdInfo.UnitIsBc = Convert.ToBoolean(
                        result.SelectSingleNode("/VFPData/ProductUnitIsBC").FirstChild.Value);
                    unitRefnoFromBarCode.ProdInfo.UnitCode = result.SelectSingleNode(
                        "/VFPData/ProductUnitCode").FirstChild.Value;

                    var productList = await DatabaseConnection.GetInstance().
                        SqlConn.QueryAsync<tProductModel>(
                        "SELECT * FROM tProduct WHERE ProductFKRefno = ?",
                        Convert.ToInt32(unitRefnoFromBarCode.ProdInfo.FkRefno));

                    if (productList != null && productList.Count > 0)
                    {
                        unitRefnoFromBarCode.ProdInfo.Refno = productList[0].ProductRefno;

                        var unitList = await DatabaseConnection.GetInstance().
                            SqlConn.QueryAsync<tUnitModel>(
                            "SELECT * FROM tUnit WHERE UnitCode = ?",
                            Convert.ToString(unitRefnoFromBarCode.ProdInfo.UnitCode));

                        if (unitList != null && unitList.Count > 0)
                        {
                            var unitRefno = unitList[0].UnitRefno;

                            var productUnitList = await DatabaseConnection.
                                GetInstance().SqlConn.QueryAsync<tProductUnitModel>(
                                "SELECT * FROM tProductUnit WHERE ProductRefno = ?" +
                                    " AND UnitRefno = ? AND ProductUnitIsBC = ?",
                                unitRefnoFromBarCode.ProdInfo.Refno,
                                unitRefno,
                                unitRefnoFromBarCode.ProdInfo.UnitIsBc);

                            if (productUnitList != null && productUnitList.Count > 0)
                            {
                                unitRefnoFromBarCode.ProdInfo.UnitRefno =
                                    productUnitList[0].ProductUnitRefno;
                            }
                        }
                    }
                }
            }

            return unitRefnoFromBarCode;
        }

        /// <summary>
        /// Get the bar code scanner assembly information.
        /// </summary>
        /// <returns><see cref="string"/> Assembly information.</returns>
        public string GetAssemblyVersion()
        {
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}
