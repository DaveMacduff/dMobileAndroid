// <copyright file="BarCodeCommon.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using Resources.Models.Products;

    /// <summary>
    /// Common bar code scanner class methods.
    /// </summary>
    public class BarCodeCommon
    {
        /// <summary>
        /// The GetSymbologyPrefixFromSymbologyEnum.
        /// </summary>
        /// <param name="symbology">The barcode symbology enum.</param>
        /// <returns>The symbology prefix string.</returns>
        public static string GetSymbologyPrefixFromSymbologyEnum(BarCodeSymbology symbology)
        {
            // Determine the symbology prefix based on the provided symbology enum value.
            var symbologyPrefix = symbology switch
            {
                // or ']O4'
                BarCodeSymbology.UNDEFINED => "]01",
                BarCodeSymbology.UCC_12_UPC_E => "]X0",
                BarCodeSymbology.EAN_UCC_14 => string.Empty,
                BarCodeSymbology.EAN_UCC_13 => "]E0",
                BarCodeSymbology.EAN_UCC_8 => "]E4",
                BarCodeSymbology.ITF_14_Symbol => "]I",
                BarCodeSymbology.UCC_EAN_128_Symbol => "]C1",
                BarCodeSymbology.Code_128 => "]C",
                BarCodeSymbology.SSCC_18 => string.Empty,
                BarCodeSymbology.Code_39 => "]A",
                BarCodeSymbology.EAN_UPC_Symbol => "]E",
                BarCodeSymbology.Code_93 => "]G",
                BarCodeSymbology.Codabar => "]F",
                BarCodeSymbology.Code_2_of_5 => string.Empty,
                BarCodeSymbology.MSI => "]M",
                BarCodeSymbology.Plessey => string.Empty,
                BarCodeSymbology.Code_11 => "]H",
                BarCodeSymbology.Matrix_2_of_5 => string.Empty,
                BarCodeSymbology.Telepen => "]B",
                BarCodeSymbology.PDF417 => "]L",
                BarCodeSymbology.Micro_PDF417 => string.Empty,
                BarCodeSymbology.RSS_14 => string.Empty,
                BarCodeSymbology.RSS_Limited => string.Empty,
                BarCodeSymbology.RSS_Expanded => "]e0",
                BarCodeSymbology.Codablock_A => "]O6",
                BarCodeSymbology.Codablock_F => "]O5",
                BarCodeSymbology.TLC_39 => string.Empty,
                BarCodeSymbology.Codablock => "]O",
                BarCodeSymbology.Codablock256 => "]02",
                _ => string.Empty,
            };
            return symbologyPrefix;
        }

        /// <summary>
        /// Request product with deli information using a product bar code.
        /// </summary>
        /// <param name="productUnitBarCodeCode"><see cref="string"/> Bar code data.</param>
        /// <param name="productUnitBarCodeModifier1"><see cref="string"/> Bar code modifier character.</param>
        /// <param name="barCodeSymbology">Used for identify deli items.</param>
        /// <param name="deliScaleAmount"><see cref="decimal"/> Used for calculating the item weight.</param>
        /// <returns><see cref="DeliProductInfo"/> Product information including deli item information.</returns>
        public static async Task<DeliProductInfo> GetDeliProdInfoFromBarCode(
            string productUnitBarCodeCode,
            string productUnitBarCodeModifier1,
            BarCodeSymbology barCodeSymbology,
            decimal deliScaleAmount)
        {
            // Get the system environment instance.
            var sysEnv = SysEnvironment.GetInstance();
            var unitRefnoFromBarCode = new DeliProductInfo();

            // Create an XML string builder and writer settings.
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false
            };
            var xml = XmlWriter.Create(xmlString, settings);

            // Get the revision and current date-time strings.
            var revisionDateTime = sysEnv.RefreshOpt.RefreshedDataLast.ToString(
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var currentDateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");

            // Create an XML document for the request.
            var result = new XmlDocument();
            var request = new XDocument(
                new XDeclaration("1.0", "utf-16", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST PRODUCT FROM BARCODE")),
                    new XElement("device_id", sysEnv.DeviceId),
                    new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                    new XElement("revision_datetime", revisionDateTime),
                    new XElement("stamp", currentDateTime),
                    new XElement("barcode", productUnitBarCodeCode),
                    new XElement("modifier1", productUnitBarCodeModifier1),
                    new XElement("symbology", Convert.ToInt32(barCodeSymbology)),
                    new XElement("barcode_amount", deliScaleAmount)));

            // Save the request XML and flush the writer.
            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            // Submit the request and load the response XML.
            var data = SysCommon.SubmitComm(xmlString.ToString());
            result.LoadXml(data);

            // Parse the response XML to get the product and deli information.
            if (result.SelectSingleNode("/VFPData/ProductFKRefno") !.HasChildNodes)
            {
                unitRefnoFromBarCode.ProdInfo.FkRefno = Convert.ToInt32(
                    result.SelectSingleNode("/VFPData/ProductFKRefno") !.FirstChild.Value);
            }

            if (result.SelectSingleNode("/VFPData/DeliScaleWeight") !.HasChildNodes)
            {
                unitRefnoFromBarCode.DeliScaleWeight = Convert.ToDecimal(
                    result.SelectSingleNode("/VFPData/DeliScaleWeight") !.FirstChild.Value);
            }

            if (result.SelectSingleNode("/VFPData/DeliScaleWeightUnit") !.HasChildNodes)
            {
                unitRefnoFromBarCode.DeliScaleWeightUnit = Convert.ToString(
                    result.SelectSingleNode("/VFPData/DeliScaleWeightUnit") !.FirstChild.Value);
            }

            // If the product reference number is not zero, get additional product information.
            if (unitRefnoFromBarCode.ProdInfo.FkRefno != 0)
            {
                unitRefnoFromBarCode.ProdInfo.UnitIsBc = Convert.ToBoolean(
                    result.SelectSingleNode("/VFPData/ProductUnitIsBC") !.FirstChild.Value);
                unitRefnoFromBarCode.ProdInfo.UnitCode = result.SelectSingleNode(
                    "/VFPData/ProductUnitCode") !.FirstChild.Value;

                // Query the database for product information.
                var productList = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<ProductModel>(
                        "SELECT * FROM tProduct WHERE ProductFKRefno = ?",
                        Convert.ToInt32(unitRefnoFromBarCode.ProdInfo.FkRefno));

                if (productList != null &&
                    productList.Count > 0)
                {
                    unitRefnoFromBarCode.ProdInfo.Refno = productList[0].ProductRefno;

                    // Query the database for unit information.
                    var unitList = await DatabaseConnection.GetInstance()
                        .SqlConn.QueryAsync<UnitModel>(
                            "SELECT * FROM tUnit WHERE UnitCode = ?",
                            Convert.ToString(unitRefnoFromBarCode.ProdInfo.UnitCode));

                    if (unitList != null &&
                        unitList.Count > 0)
                    {
                        var unitRefno = unitList[0].UnitRefno;

                        // Query the database for product unit information.
                        var productUnitList = await DatabaseConnection.GetInstance()
                            .SqlConn.QueryAsync<ProductUnitModel>(
                                "SELECT * FROM tProductUnit WHERE ProductRefno = ?" +
                                " AND UnitRefno = ? AND ProductUnitIsBC = ?",
                                unitRefnoFromBarCode.ProdInfo.Refno,
                                unitRefno,
                                unitRefnoFromBarCode.ProdInfo.UnitIsBc);

                        if (productUnitList != null &&
                            productUnitList.Count > 0)
                        {
                            unitRefnoFromBarCode.ProdInfo.UnitRefno =
                                productUnitList[0].ProductUnitRefno;
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
            // Get the assembly version information.
            var major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            var minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            var build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var revision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}