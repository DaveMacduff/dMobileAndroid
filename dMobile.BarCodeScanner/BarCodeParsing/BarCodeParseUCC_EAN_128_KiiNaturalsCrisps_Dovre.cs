// <copyright file="BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre : BarCodeParseUCC_EAN_128_Symbol
    {
        public static readonly string KiiNaturalsCrispsVendorNumber = "0678629";

        /// <inheritdoc/>
        protected override string IdentifyGTIN(string barCodeData)
        {
            var data = barCodeData;

            // they have their GTIN's ID'd 02 instead of 01
            if (data.StartsWith("02") && data.Length >= 16)
            {
                data = "01" + data.Substring(2);
            }
            return base.IdentifyGTIN(data);
        }

        /// <inheritdoc/>
        protected override string IdentifyGTINinLogisticUnit(string barCodeData)
        {
            // They have their GTIN's ID'd 02 instead of 01
            // We will simply ignore their 02 here and recognize it in IdentifyGTIN only
            return barCodeData;
        }

        /// <inheritdoc/>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // For Vendor # 0678629
                // stored as DDMMYY and it should be YYMMDD
                if (this.GTIN != null && this.GTIN.Substring(1).StartsWith(KiiNaturalsCrispsVendorNumber))
                {
                    // Reverse
                    var years = tempData.Substring(2, 2);
                    var months = tempData.Substring(4, 2);
                    var days = tempData.Substring(6, 2);

                    // switch days and years
                    var identifier = tempData.Substring(0, 2);
                    var daysAlt = years;
                    var monthsAlt = months;
                    var yearsAlt = days;
                    tempData =
                        identifier +
                        yearsAlt +
                        monthsAlt +
                        daysAlt +
                        tempData.Substring(8);     // Rest of string
                }

                return base.IdentifyLogisticUnitBestBeforeSellByDate(tempData);
            }

            return tempData; // it isn't so just return
        }
    }
}
