// <copyright file="BarCodeParseCode_2_of_5.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Provides parsing functionality for Code 2 of 5 barcodes.
    /// </summary>
    public class BarCodeParseCode_2_of_5 : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for Code 2 of 5 barcodes.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;

            // Remove the Product Identifier from the barcode data
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }
    }
}