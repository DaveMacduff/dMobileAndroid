// <copyright file="BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Defines the <see cref="BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack" />.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack : BarCodeParse
    {
        /// <summary>
        /// The ParseBarCodeData.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            // strip first character (unknown)
            barCodeData = barCodeData.Substring(1);

            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKG(barCodeData);

            // strip one character (unknown)
            barCodeData = barCodeData.Substring(1);

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
            this.ProductIdentifier = barCodeData.Substring(0, 5);
            return barCodeData.Substring(5);
        }

        /// <summary>
        /// The IdentifyNetWeightMetricKG.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected override string IdentifyNetWeightMetricKG(string barCodeData)
        {
            this.NetWeightMetricKGFoundAI = true;
            var fieldLength = 3;
            var decimalPlaces = 1; //
            if (decimalPlaces > 0)
            {
                this.NetWeightMetricKG = Convert.ToDecimal(
                    barCodeData.Substring(0, fieldLength - decimalPlaces)
                    + "."
                    + barCodeData.Substring(fieldLength - decimalPlaces, decimalPlaces));
            }
            else
            {
                this.NetWeightMetricKG = Convert.ToDecimal(
                    barCodeData.Substring(0, fieldLength));
            }
            return barCodeData.Substring(fieldLength);
        }

        /// <summary>
        /// 4-digit Julian to Gregorian date.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            var julianYear = Convert.ToInt32(barCodeData.Substring(0, 1));
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

            return barCodeData.Substring(4);
        }
    }
}
