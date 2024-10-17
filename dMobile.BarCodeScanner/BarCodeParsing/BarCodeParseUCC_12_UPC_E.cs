// <copyright file="BarCodeParseUCC_12_UPC_E.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Bar code parsing for UCC 12 / UPC E symbology.
    /// </summary>
    public class BarCodeParseUCC_12_UPC_E : BarCodeParse
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

            // Identify the GTIN from the bar code data
            barCodeData = this.IdentifyGtin(barCodeData);

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
            var isDeliScaleBarCode = false;

            if (this.Settings.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU)
            {
                // KLUDGE: PLU (price look up) codes always start with the number 2 according to
                // Fedor March 12 / 2013 - Warning this will bite us later
                var s = Convert.ToString(Convert.ToInt64(barCodeData));

                // 2 = deli
                // 9 = organic
                // 3 or 4 = produce conventionally grown
                // if (s.StartsWith("2") || s.StartsWith("9") || s.StartsWith("3") || s.StartsWith("4"))
                if (s.StartsWith("2"))
                {
                    isDeliScaleBarCode = true;
                    this.ProductIdentifier = Convert.ToString(Convert.ToInt32(
                        barCodeData.Substring(1, 5)));     // 0 index should be a '2'

                    if (this.Settings.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount)
                    {
                        // 4 digit amount
                        this.DeliScaleAmount = Convert.ToDecimal(barCodeData.Substring(7, 4)) / 100;
                    }
                    else
                    {
                        // 5 digit amount
                        this.DeliScaleAmount = Convert.ToDecimal(barCodeData.Substring(6, 5)) / 100;
                    }

                    // this.Remainder = barCodeData.Substring(7);
                }
            }

            if (!isDeliScaleBarCode)
            {
                if (this.Settings.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family)
                {
                    this.ProductIdentifier = barCodeData;
                }
                else
                {
                    this.ProductIdentifier = barCodeData.PadLeft(14, '0');
                }
            }

            // remove the Product Identifier from the bar code data
            // barCodeData = barCodeData.Remove(0, strProductIdentifier.Length);

            // (zero suppressed UPC)
            // CString tmpstr = bcode.Left(3);
            // CString s4Zeros('0', 4);
            // CString s5Zeros('0', 5);
            // TCHAR pch = bcode[6];

            // switch (pch)
            // {
            //  case '0':
            //  case '1':
            //  case '2':
            //    tmpstr += bcode[6] + s4Zeros + bcode.Mid(3,3);
            //    break;
            //  case '3':
            //    tmpstr += bcode[6] + s5Zeros + bcode.Mid(4,2);
            //    break;
            //  case '4':
            //    tmpstr += bcode.Mid(3,2) + s5Zeros + bcode[5];
            //    break;
            //  case '5':
            //  case '6':
            //  case '7':
            //  case '8':
            //  case '9':
            //    tmpstr += bcode.Mid(3,3) + s4Zeros + bcode[6];
            // }
            // tmpstr += bcode[7];
            return barCodeData;
        }

        /// <summary>
        /// Identifies the GTIN from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The remaining bar code data after identifying the GTIN.</returns>
        /// <inheritdoc/>
        protected override string IdentifyGtin(string barCodeData)
        {
            var iLen = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.Gtin = barCodeData[..iLen].PadLeft(14, '0');

            // remove the GTIN from the bar code data
            barCodeData = barCodeData.Remove(0, iLen);

            return barCodeData;
        }
    }
}