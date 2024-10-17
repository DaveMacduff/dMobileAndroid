// <copyright file="BarCodeParseMicro_PDF417.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Represents a parser for Micro PDF417 barcode symbols.
    /// </summary>
    public class BarCodeParseMicro_PDF417 : BarCodeParse
    {
        /// <summary>
        /// Parses the provided Micro PDF417 barcode data.
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