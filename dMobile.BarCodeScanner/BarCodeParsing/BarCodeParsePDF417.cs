// <copyright file="BarCodeParsePDF417.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Represents a parser for PDF417 barcode symbols.
    /// </summary>
    public class BarCodeParsePDF417 : BarCodeParse
    {
        /// <summary>
        /// Parses the provided PDF417 barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);
            return barCodeData;
        }
    }
}