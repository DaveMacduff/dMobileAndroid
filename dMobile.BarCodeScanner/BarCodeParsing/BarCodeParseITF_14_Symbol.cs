// <copyright file="BarCodeParseITF_14_Symbol.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Represents a parser for ITF-14 barcode symbols.
    /// </summary>
    public class BarCodeParseITF_14_Symbol : BarCodeParse
    {
        /// <summary>
        /// Parses the provided ITF-14 barcode data.
        ///
        /// Data structure is always 14 digits
        /// Position 1 is package indicator digit
        /// Position 2-3 is UPC numbering system or EAN prefix
        /// Position 4-8 company prefix
        /// Position 9-13 item reference
        /// Position 14 is a check digit.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            if (barCodeData.Length < 14)
            {
                barCodeData = this.IdentifyProductIdentifier(barCodeData);   // does not remove data
                barCodeData = barCodeData.Remove(0, barCodeData.Length);
            }
            else
            {
                barCodeData = this.IdentifyProductIdentifier(barCodeData);   // does not remove data
                barCodeData = this.IdentifyGtin(barCodeData);   // does not remove data

                // parse the GTIN
                barCodeData = this.IdentifyPackagingType(barCodeData);
                barCodeData = barCodeData.Remove(0, 2);   // remove the UPC numbering system or EAN prefix
                barCodeData = this.IdentifyCompanyPrefix(barCodeData);
                barCodeData = this.IdentifyItemReference(barCodeData);

                // remove the check digit if it is there
                if (barCodeData.Length > 0)
                {
                    barCodeData = barCodeData.Remove(0, 1);    // remove the check digit
                }

                // end parse GTIN
            }

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the packaging type from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the packaging type.</returns>
        /// <inheritdoc/>
        protected override string IdentifyPackagingType(string barCodeData)
        {
            var strBarCodeData = barCodeData;

            this.PackagingType = Convert.ToInt32(strBarCodeData[..1]);

            // remove the packaging indicator
            strBarCodeData = strBarCodeData.Remove(0, 1);

            return strBarCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (this.Settings.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family)
            {
                this.ProductIdentifier = barCodeData;
            }
            else
            {
                this.ProductIdentifier = barCodeData.PadLeft(14, '0');
            }

            // remove the Product Identifier from the bar code data
            // barCodeData = barCodeData.Remove(0, strProductIdentifier.Length);
            return barCodeData;
        }

        /// <summary>
        /// Identifies the company prefix from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the company prefix.</returns>
        /// <inheritdoc/>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            var strBarCodeData = barCodeData;

            this.CompanyPrefix = strBarCodeData[..5];

            // remove the Company Prefix from the bar code data
            strBarCodeData = strBarCodeData.Remove(0, this.CompanyPrefix.Length);

            return strBarCodeData;
        }

        /// <summary>
        /// Identifies the item reference from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the item reference.</returns>
        /// <inheritdoc/>
        protected override string IdentifyItemReference(string barCodeData)
        {
            this.ItemReference = barCodeData[..5];

            // remove the Item Reference from the bar code data
            barCodeData = barCodeData.Remove(0, this.ItemReference.Length);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the GTIN from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data.</param>
        /// <returns>The remaining barcode data after identifying the GTIN.</returns>
        /// <inheritdoc/>
        protected override string IdentifyGtin(string barCodeData)
        {
            this.Gtin = barCodeData[..14];

            // remove the GTIN from the bar code data
            // barCodeData = barCodeData.Remove(0, strGTIN.Length);
            return barCodeData;
        }
    }
}
