// <copyright file="BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    ///     Defines the bar code parsing for UCC/EAN-128 symbology for Greenlea Premier Meats Nossack.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack : BarCodeParse
    {
        /// <summary>
        ///     Parse bar code data.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            // strip first character (unknown)
            barCodeData = barCodeData[1..];

            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);

            // strip one character (unknown)
            barCodeData = barCodeData[1..];

            // read production date (Julian -> Convert to Gregorian)
            barCodeData = this.IdentifyLogisticUnitProductionDate(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// The IdentifyProductIdentifier.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData[..5];
            return barCodeData[5..];
        }

        /// <summary>
        /// The IdentifyNetWeightMetricKG.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            this.NetWeightMetricKgFoundAi = true;
            const int fieldLength = 3;
            const int decimalPlaces = 1;
            if (decimalPlaces > 0)
            {
                this.NetWeightMetricKg = Convert.ToDecimal(
                    barCodeData[.. (fieldLength - decimalPlaces)]
                    + "."
                    + barCodeData.Substring(fieldLength - decimalPlaces, decimalPlaces));
            }
            else
            {
                this.NetWeightMetricKg = Convert.ToDecimal(
                    barCodeData[..fieldLength]);
            }

            return barCodeData[fieldLength..];
        }

        /// <summary>
        /// 4-digit Julian to Gregorian date.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            var julianYear = Convert.ToInt32(barCodeData[..1]);
            var currentYear = DateTime.Now.Year;
            int year;
            if (julianYear > currentYear % 10)
            {
                year = currentYear - 10 + (julianYear - (currentYear % 10));
            }
            else
            {
                year = currentYear - ((currentYear % 10) - julianYear);
            }

            var julianDay = Convert.ToInt32(barCodeData.Substring(1, 3));
            var gregorianDate = DateTime.Parse("1 Jan " + year);

            // -1 because 1 is the first day, and julian days start counting at 1
            gregorianDate = gregorianDate.AddDays(julianDay - 1);

            this.LogisticUnitProductionDate = gregorianDate;

            return barCodeData[4..];
        }
    }
}
