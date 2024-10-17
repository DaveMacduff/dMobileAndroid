// <copyright file="BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Bar code parsing for UCC/EAN 128 symbology specific to Kii Naturals Crisps by Dovre.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre : BarCodeParseUCC_EAN_128_Symbol
    {
        /// <summary>
        /// The vendor number for Kii Naturals Crisps.
        /// </summary>
        private const string KiiNaturalsCrispsVendorNumber = "0678629";

        /// <summary>
        /// Identifies the GTIN from the bar code data, adjusting for vendor-specific formatting.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the GTIN.</returns>
        /// <inheritdoc />
        protected override string IdentifyGtin(string barCodeData)
        {
            var data = barCodeData;

            // Adjust GTIN identifier from 02 to 01 if necessary
            if (data.StartsWith("02") && data.Length >= 16)
            {
                data = "01" + data[2..];
            }

            return base.IdentifyGtin(data);
        }

        /// <summary>
        /// Identifies the GTIN in the logistic unit, ignoring vendor-specific formatting.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the GTIN in the logistic unit.</returns>
        /// <inheritdoc />
        protected override string IdentifyGtiNinLogisticUnit(string barCodeData)
        {
            // Ignore the 02 identifier here and recognize it in IdentifyGTIN only
            return barCodeData;
        }

        /// <summary>
        /// Identifies the best before/sell by date from the bar code data, adjusting for vendor-specific formatting.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the best before/sell by date.</returns>
        /// <inheritdoc />
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // For Vendor # 0678629, stored as DDMMYY and should be YYMMDD
                if (this.Gtin != null && this.Gtin[1..].StartsWith(KiiNaturalsCrispsVendorNumber))
                {
                    // Reverse the date format
                    var years = tempData.Substring(2, 2);
                    var months = tempData.Substring(4, 2);
                    var days = tempData.Substring(6, 2);

                    // Switch days and years
                    var identifier = tempData[..2];
                    tempData = identifier + days + months + years + tempData[8..]; // Rest of string
                }

                return base.IdentifyLogisticUnitBestBeforeSellByDate(tempData);
            }

            return tempData; // If not applicable, just return the original data
        }
    }
}