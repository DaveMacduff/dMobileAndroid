// <copyright file="BarCodeParseCode_128_NorthCoopMeat_Product256912.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Provides parsing functionality for Code 128 barcodes specific to North Coop Meat Product 256912.
    /// </summary>
    public class BarCodeParseCode_128_NorthCoopMeat_Product256912 : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for North Coop Meat Product 256912.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);
            barCodeData = this.IdentifyLogisticUnitProductionDate(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
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
        /// Identifies the net weight in metric kilograms from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the net weight in metric kilograms.</returns>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            this.NetWeightMetricKgFoundAi = true;

            // Get the number of decimal places
            var places = 2;

            // Assign the net weight in metric kilograms
            var wholeNumber = barCodeData[..4];
            if (places != 0)
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^places] + "." + wholeNumber[^places..]);
            }
            else
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            }

            // Remove the net weight in metric kilograms from the barcode data
            barCodeData = barCodeData.Remove(0, 4);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the logistic unit production date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit production date.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.Length >= 8)
            {
                // Assign the production date
                var year = Convert.ToInt32(tempData[..2]);
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(
                    year,
                    Convert.ToInt32(tempData.Substring(2, 2)),
                    Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                this.LogisticUnitProductionDate = dt;

                // Remove the production date from the barcode data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }
    }
}
