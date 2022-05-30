// <copyright file="BarCodeParseEAN_UCC_13.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseEAN_UCC_13 : BarCodeParse
    {
        /// <summary>
        /// Parsing rules reviewed BarCodeParseEAN_UCC_13.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Data structure is always 13 or 14 digits
        /// Position 1 is an implied zero
        /// Position 2-13 company prefix and item reference
        /// Position 14 is a check digit.
        /// </para><para>
        /// Because we do not know the length of either the company prefix or the item reference they
        /// cannot be parsed.</para></remarks>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            if (this.Settings.IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E)
            {
                this.ClearResetBarCodeProperties(false);
                BarCodeParseUCC_12_UPC_E barCodeParse = new BarCodeParseUCC_12_UPC_E();

                // Remove any leading extra zero padding, max bar code length will be 12.
                if (barCodeData.Length > 12)
                {
                    barCodeData = barCodeData.Substring(barCodeData.Length - 12);
                }

                barCodeParse.ParseBarCodeData(barCodeData);
            }
            else
            {
                barCodeData = this.IdentifyGTIN(barCodeData);     // does not remove data
                barCodeData = this.IdentifyProductIdentifier(barCodeData);   // does not remove data

                barCodeData = barCodeData.Remove(0, 1);   // remove implied zero

                barCodeData = this.IdentifyCompanyPrefix(barCodeData);
                barCodeData = this.IdentifyItemReference(barCodeData);

                if (barCodeData.Length > 0)
                {
                    barCodeData = barCodeData.Remove(0, 1);   // remove check digit
                }

                this.Remainder = barCodeData;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            this.CompanyPrefix = barCodeData.Substring(0, 5);

            // remove the Company Prefix from the bar code data
            barCodeData = barCodeData.Remove(0, this.CompanyPrefix.Length);

            return barCodeData;
        }

        /// <summary>
        /// Product Code.
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
        protected override string IdentifyGTIN(string barCodeData)
        {
            int len = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.GTIN = barCodeData.Substring(0, len).PadLeft(14, '0');

            // remove the GTIN from the bar code data
            // barCodeData = barCodeData.Remove(0, barCodeData.Length);
            return barCodeData;
        }
    }
}
