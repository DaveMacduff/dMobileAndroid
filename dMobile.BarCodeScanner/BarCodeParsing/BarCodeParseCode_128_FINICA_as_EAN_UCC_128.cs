// <copyright file="BarCodeParseCode_128_FINICA_as_EAN_UCC_128.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Provides parsing functionality for Code 128 barcodes specific to FINICA as EAN/UCC 128.
    /// </summary>
    public class BarCodeParseCode_128_FINICA_as_EAN_UCC_128 : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for FINICA as EAN/UCC 128.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyGtin(barCodeData);    // Do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);
            barCodeData = this.IdentifyLogisticUnitBestBeforeSellByDate(barCodeData);
            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
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
        /// Identifies the GTIN (Global Trade Item Number) from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the GTIN.</returns>
        protected override string IdentifyGtin(string barCodeData)
        {
            this.Data = barCodeData;
            this.Gtin = this.Data[..14];

            // Do not remove the GTIN from the data
            return this.Data;
        }

        /// <summary>
        /// Identifies the logistic unit best before/sell by date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the best before/sell by date.</returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            // Assign the best before/sell by date
            var year = Convert.ToInt32(barCodeData[..2]);
            if (year < 80)
            {
                year += 2000;
            }
            else
            {
                year += 1900;
            }

            var dt = new DateTime(year, Convert.ToInt32(barCodeData.Substring(2, 2)),
                Convert.ToInt32(barCodeData.Substring(4, 2)));   // 101026
            this.LogisticUnitBestBeforeSellByDate = dt;

            // Remove the best before/sell by date from the barcode data
            barCodeData = barCodeData.Remove(0, 6);

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
            var wholeNumber = barCodeData[..6];
            this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^places] + "." + wholeNumber[^places..]);
            ////if (places != 0)
            ////{
            ////    this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^places] + "." + wholeNumber[^places..]);
            ////}
            ////else
            ////{
            ////    this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            ////}

            // Remove the net weight in metric kilograms from the barcode data
            barCodeData = barCodeData.Remove(0, 6);

            return barCodeData;
        }
    }
}
