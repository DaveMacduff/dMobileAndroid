// <copyright file="BarCodeParseTelepen.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Bar code parsing for Telepen symbology.
    /// </summary>
    public class BarCodeParseTelepen : BarCodeParse
    {
        /// <summary>
        /// Parses the provided bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            // Identify the product identifier from the bar code data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            // Set the remainder of the bar code data
            this.Remainder = barCodeData;
        }
    }
}