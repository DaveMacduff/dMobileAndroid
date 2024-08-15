// <copyright file="BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// 
    /// </summary>
    public class BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre : BarCodeParseUCC_EAN_128_Symbol
    {
        public static readonly string KiiNaturalsCrispsVendorNumber = "0678629";

        /// <inheritdoc />
        protected override string IdentifyGtin(string barCodeData)
        {
            var data = barCodeData;

            // they have their GTIN's ID'd 02 instead of 01
            if (data.StartsWith("02") &&
                data.Length >= 16)
            {
                data = "01" + data[2..];
            }

            return base.IdentifyGtin(data);
        }

        /// <inheritdoc />
        protected override string IdentifyGtiNinLogisticUnit(string barCodeData)
        {
            // They have their GTIN's ID'd 02 instead of 01
            // We will simply ignore their 02 here and recognize it in IdentifyGTIN only
            return barCodeData;
        }

        /// <inheritdoc />
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("15") &&
                tempData.Length >= 8)
            {
                // For Vendor # 0678629
                // stored as DDMMYY and it should be YYMMDD
                if (this.Gtin != null &&
                    this.Gtin[1..].StartsWith(KiiNaturalsCrispsVendorNumber))
                {
                    // Reverse
                    var years = tempData.Substring(2, 2);
                    var months = tempData.Substring(4, 2);
                    var days = tempData.Substring(6, 2);

                    // switch days and years
                    var identifier = tempData[..2];
                    tempData =
                        identifier +
                        days +
                        months +
                        years +
                        tempData[8..]; // Rest of string
                }

                return base.IdentifyLogisticUnitBestBeforeSellByDate(tempData);
            }

            return tempData; // it isn't so just return
        }
    }
}