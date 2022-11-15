// <copyright file="BarCodeParseITF_14_Symbol.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseITF_14_Symbol : BarCodeParse
    {
        /// <summary>
        /// Parsing rules reviewed
        ///
        /// Data structure is always 14 digits
        /// Position 1 is package indicator digit
        /// Position 2-3 is UPC numbering system or EAN prefix
        /// Position 4-8 company prefix
        /// Position 9-13 item reference
        /// Position 14 is a check digit.
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
                barCodeData = this.IdentifyGTIN(barCodeData);   // does not remove data

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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyPackagingType(string barCodeData)
        {
            var strBarCodeData = barCodeData;

            this.PackagingType = Convert.ToInt32(strBarCodeData.Substring(0, 1));

            // remove the packagiing indicator
            strBarCodeData = strBarCodeData.Remove(0, 1);

            return strBarCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            var strBarCodeData = barCodeData;

            this.CompanyPrefix = strBarCodeData.Substring(0, 5);

            // remove the Company Prefix from the bar code data
            strBarCodeData = strBarCodeData.Remove(0, this.CompanyPrefix.Length);

            return strBarCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyItemReference(string barCodeData)
        {
            this.ItemReference = barCodeData.Substring(0, 5);

            // remove the Item Reference from the bar code data
            barCodeData = barCodeData.Remove(0, this.ItemReference.Length);

            return barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGTIN(string barCodeData)
        {
            this.GTIN = barCodeData.Substring(0, 14);

            // remove the GTIN from the bar code data
            // barCodeData = barCodeData.Remove(0, strGTIN.Length);
            return barCodeData;
        }
    }
}
