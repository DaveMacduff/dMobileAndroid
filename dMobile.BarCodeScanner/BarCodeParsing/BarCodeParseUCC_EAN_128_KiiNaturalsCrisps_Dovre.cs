using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace dMobile.BarCodeScanner
{
    public class BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre : BarCodeParseUCC_EAN_128_Symbol
    {
        public static readonly string KiiNaturalsCrispsVendorNumber = "0678629";

        protected override string IdentifyGTIN(string barCodeData)
        {
            string data = barCodeData;
            // they have their GTIN's ID'd 02 instead of 01
            if (data.StartsWith("02") && data.Length >= 16)
            {
                data = "01" + data.Substring(2);
            }
            return base.IdentifyGTIN(data);
        }

        protected override string IdentifyGTINinLogisticUnit(string barCodeData)
        {
            // They have their GTIN's ID'd 02 instead of 01
            // We will simply ignore their 02 here and recognize it in IdentifyGTIN only
            return barCodeData;
        }

        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // For Vendor # 0678629
                // stored as DDMMYY and it should be YYMMDD
                if (GTIN != null && GTIN.Substring(1).StartsWith(KiiNaturalsCrispsVendorNumber))
                {
                    // Reverse
                    string years = tempData.Substring(2, 2);
                    string months = tempData.Substring(4, 2);
                    string days = tempData.Substring(6, 2);

                    // switch days and years
                    string identifier = tempData.Substring(0, 2);
                    string daysAlt = years;
                    string monthsAlt = months;
                    string yearsAlt = days;
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
