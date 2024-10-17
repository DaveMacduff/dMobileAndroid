// <copyright file="BarCodeParseUnDefined.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Provides parsing functionality for undefined bar code symbology.
    /// </summary>
    public class BarCodeParseUnDefined : BarCodeParse
    {
        /// <summary>
        /// Parses the bar code data and identifies the product identifier.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the product identifier.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);
            return barCodeData;
        }
    }
}