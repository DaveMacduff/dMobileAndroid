// <copyright file="BarCodeParseEAN_UPC_Symbol.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseEAN_UPC_Symbol : BarCodeParse
    {
        /// <summary>
        /// Parsing rules reviewed
        /// Pos 1 Number System Character
        /// Pos 2-6 Company Prefix
        /// Pos 7-11 Item Reference
        /// Pos 12 Check Digit.
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);   // does not remove data
            barCodeData = this.IdentifyGTIN(barCodeData);   // does not remove data

            // parse the GTIN
            barCodeData = this.IdentifyNumberSystemCharacter(barCodeData);
            barCodeData = this.IdentifyCompanyPrefix(barCodeData);
            barCodeData = this.IdentifyItemReference(barCodeData);

            // remove the check digit if it is there
            if (barCodeData.Length > 0)
            {
                barCodeData = barCodeData.Remove(0, 1);
            }
            // end parse GTIN
            this.Remainder = barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyNumberSystemCharacter(string barCodeData)
        {
            string strBarCodeData = barCodeData;
            this.NumberSystemCharacter = Convert.ToInt32(strBarCodeData.Substring(0, 1));

            // remove the Number System Character from the bar code data
            strBarCodeData = strBarCodeData.Remove(0, 1);

            return strBarCodeData;
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
            string strBarCodeData = barCodeData;
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
        protected override string IdentifyItemReference(string barCodeData)
        {
            string strBarCodeData = barCodeData;

            this.ItemReference = strBarCodeData.Substring(0, 5);

            // remove the Item Reference from the bar code data
            strBarCodeData = strBarCodeData.Remove(0, this.ItemReference.Length);

            return strBarCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTIN(string barCodeData)
        {
            int iLen = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.GTIN = barCodeData.Substring(0, iLen).PadLeft(14, '0');

            // remove the GTIN from the bar code data
            // barCodeData = barCodeData.Remove(0, strGTIN.Length);
            return this.Data;
        }
    }
}
