// <copyright file="BarCodeParseCode_39.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Provides parsing functionality for Code 39 barcodes.
    /// </summary>
    public class BarCodeParseCode_39 : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for Code 39 barcodes.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = barCodeData;
        }
    }
}