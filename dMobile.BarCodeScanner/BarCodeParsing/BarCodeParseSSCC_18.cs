// <copyright file="BarCodeParseSSCC_18.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Represents a parser for SSCC-18 barcode symbols.
    /// </summary>
    public class BarCodeParseSSCC_18 : BarCodeParse
    {
        /// <summary>
        /// Parses the provided SSCC-18 barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            // Do not remove data
            barCodeData = this.IdentifySscc18(barCodeData);

            // Must follow IdentifySSCC18(), also do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            barCodeData = this.IdentifyPackagingType(barCodeData);
            barCodeData = this.IdentifyCompanyPrefix(barCodeData);
            barCodeData = this.IdentifyManufacturerSerialNumber(barCodeData);

            // Remove the check digit if it is there
            if (barCodeData.Length > 0)
            {
                barCodeData = this.Data.Remove(0, 1);
            }

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the Manufacturer Serial Number from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the Manufacturer Serial Number.</returns>
        protected override string IdentifyManufacturerSerialNumber(string barCodeData)
        {
            this.ManufacturerSerialNumber = Convert.ToInt64(barCodeData[..9]);

            // Remove the Manufacturer Serial Number from the barcode data
            barCodeData = barCodeData.Remove(0, 9);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the Packaging Type from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the Packaging Type.</returns>
        protected override string IdentifyPackagingType(string barCodeData)
        {
            this.PackagingType = Convert.ToInt32(barCodeData[..1]);

            // Remove the Packaging Type from the barcode data
            barCodeData = barCodeData.Remove(0, 1);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the Product Identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the Product Identifier.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData;

            // barCodeData = barCodeData.Remove(0, strProductIdentifier.Length);
            return barCodeData;
        }

        /// <summary>
        /// Identifies the Company Prefix from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the Company Prefix.</returns>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            this.CompanyPrefix = barCodeData[..7];

            // Remove the Company Prefix from the barcode data
            barCodeData = barCodeData.Remove(0, this.CompanyPrefix.Length);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the SSCC-18 from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the SSCC-18.</returns>
        protected override string IdentifySscc18(string barCodeData)
        {
            if (barCodeData.StartsWith("00") && barCodeData.Length >= 18)
            {
                // Remove the AI
                barCodeData = barCodeData.Remove(0, 2);

                // Assign the SSCC-18
                this.Sscc18 = barCodeData[..18];

                // barCodeData = barCodeData.Remove(0, strSSCC18.Length);
            }

            return barCodeData;
        }
    }
}