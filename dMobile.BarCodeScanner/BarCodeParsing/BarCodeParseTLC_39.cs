// <copyright file="BarCodeParseTLC_39.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Bar code parsing for TLC 39 symbology.
    /// </summary>
    public class BarCodeParseTLC_39 : BarCodeParse
    {
        /// <summary>
        /// Parses the provided bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            // Identify the product identifier from the bar code data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            // Set the remainder of the bar code data
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            // Set the product identifier
            this.ProductIdentifier = barCodeData;

            // Remove the product identifier from the bar code data
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }
    }
}