// <copyright file="BarCodeParseUCC_12_UPC_E.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseUCC_12_UPC_E : BarCodeParse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyGTIN(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGTIN(string barCodeData)
        {
            var iLen = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.GTIN = barCodeData.Substring(0, iLen).PadLeft(14, '0');

            // remove the GTIN from the bar code data
            barCodeData = barCodeData.Remove(0, iLen);

            return barCodeData;
        }
    }
}
