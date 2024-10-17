// <copyright file="BarCodeParseEAN_UCC_8.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Class for parsing EAN/UCC-8 barcodes.
    /// </summary>
    public class BarCodeParseEAN_UCC_8 : BarCodeParse
    {
        /// <summary>
        /// Parses the EAN/UCC-8 barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);   // do not remove data
            barCodeData = this.IdentifyGtin(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = this.Settings.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family
                ? barCodeData : barCodeData.PadLeft(14, '0');

            return barCodeData;
        }

        /// <summary>
        /// Identifies the GTIN from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after extracting the GTIN.</returns>
        /// <inheritdoc/>
        protected override string IdentifyGtin(string barCodeData)
        {
            var len = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.Gtin = barCodeData[..len].PadLeft(14, '0');

            barCodeData = barCodeData.Remove(0, len);

            return barCodeData;
        }
    }
}