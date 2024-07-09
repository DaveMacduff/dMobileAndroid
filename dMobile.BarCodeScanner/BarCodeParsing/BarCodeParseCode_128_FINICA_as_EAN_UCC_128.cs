// <copyright file="BarCodeParseCode_128_FINICA_as_EAN_UCC_128.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Defines the <see cref="BarCodeParseCode_128_FINICA_as_EAN_UCC_128" />.
    /// </summary>
    public class BarCodeParseCode_128_FINICA_as_EAN_UCC_128 : BarCodeParse
    {
        /// <summary>
        /// Parsing rules reviewed.
        /// </summary>
        /// <param name="barCodeData">.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyGtin(barCodeData);    // do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);
            barCodeData = this.IdentifyLogisticUnitBestBeforeSellByDate(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// The IdentifyProductIdentifier.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (barCodeData.Length < 14)
            {
                this.ProductIdentifier = barCodeData.PadLeft(14, '0');
            }
            else if (barCodeData.Length > 14)
            {
                this.ProductIdentifier = barCodeData[..14];
            }
            else
            {
                this.ProductIdentifier = barCodeData;
            }

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }

        /// <summary>
        /// The IdentifyGTIN.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyGtin(string barCodeData)
        {
            this.Data = barCodeData;

            this.Gtin = this.Data[..14];

            // Do not remove the gtin from the data
            return this.Data;
        }

        /// <summary>
        /// The IdentifyLogisticUnitBestBeforeSellByDate.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            // Assign the BEST BEFORE SELL BY DATE
            var year = Convert.ToInt32(barCodeData[..2]);
            if (year < 80)
            {
                year = year + 2000;
            }
            else
            {
                year = year + 1900;
            }

            var dt = new DateTime(year, Convert.ToInt32(barCodeData.Substring(2, 2)),
                Convert.ToInt32(barCodeData.Substring(4, 2)));   // 101026
            this.LogisticUnitBestBeforeSellByDate = dt;

            // remove the BEST BEFORE SELL BY DATE from the bar code data
            barCodeData = barCodeData.Remove(0, 6);

            return barCodeData;
        }

        /// <summary>
        /// The IdentifyNetWeightMetricKG.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            this.NetWeightMetricKgFoundAi = true;

            // Get the number of decimal places
            var places = 2;

            // assign the METRIC NET WEIGHT KG
            var wholeNumber = barCodeData[..6];
            if (places != 0)
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^places] + "." + wholeNumber[^places..]);
            }
            else
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            }

            // Remove the METRIC NET WEIGHT KG from the bar code data
            barCodeData = barCodeData.Remove(0, 6);

            return barCodeData;
        }
    }
}
