// <copyright file="BarCodeParseCode_128_NorthCoopMeat_Product256912.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Defines the <see cref="BarCodeParseCode_128_NorthCoopMeat_Product256912" />.
    /// </summary>
    public class BarCodeParseCode_128_NorthCoopMeat_Product256912 : BarCodeParse
    {
        /// <summary>
        /// Parsing rules reviewed.
        /// </summary>
        /// <param name="barCodeData">.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);
            barCodeData = this.IdentifyLogisticUnitProductionDate(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// The IdentifyProductIdentifier.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (barCodeData.Length < 7)
            {
                this.ProductIdentifier = barCodeData.PadLeft(6, '0');
            }
            else if (barCodeData.Length > 6)
            {
                this.ProductIdentifier = barCodeData[..6];
            }
            else
            {
                this.ProductIdentifier = barCodeData;
            }

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

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
            var wholeNumber = barCodeData[..4];
            if (places != 0)
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^places] + "." + wholeNumber[^places..]);
            }
            else
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            }

            // Remove the METRIC NET WEIGHT KG from the bar code data
            barCodeData = barCodeData.Remove(0, 4);

            return barCodeData;
        }

        /// <summary>
        /// The IdentifyLogisticUnitProductionDate.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.Length >= 8)
            {
                // assign the PRODUCTION DATE
                var year = Convert.ToInt32(tempData[..2]);
                if (year < 80)
                {
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
                }

                var dt = new DateTime(
                    year,
                    Convert.ToInt32(tempData.Substring(2, 2)),
                    Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                this.LogisticUnitProductionDate = dt;

                // remove the PRODUCTION DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }
    }
}
