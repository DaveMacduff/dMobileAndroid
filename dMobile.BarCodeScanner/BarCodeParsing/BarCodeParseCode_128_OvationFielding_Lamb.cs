// <copyright file="BarCodeParseCode_128_OvationFielding_Lamb.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Provides parsing functionality for Code 128 barcodes specific to Ovation Field New Zealand Lamb.
    /// </summary>
    public class BarCodeParseCode_128_OvationFielding_Lamb : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for Ovation Field New Zealand Lamb.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <remarks>
        /// See WO # 11127.
        /// <para>
        /// 13 = Year
        /// 255 = 255th day of the year, e.g. 255 = Sept 12 (Pack date)
        /// 67 = Pack Station
        /// 0 = Check Digit
        /// 0 = Check Digit
        /// 197 = Carton Number
        /// 0 = Check Digit
        /// 51016 = Stand-pack Number
        /// 0 = Check Digit
        /// 0 = Check Digit
        /// 1492 = Weight 14.92 KG
        /// 0 = Check Digit
        /// 13 = 2013
        /// 254 = 254th day of the year, e.g. 254 = Sept 11 (Slaughter date)
        /// 87 = Pack Location.
        /// </para><para>
        /// 32 total digits.</para>
        /// </remarks>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyLogisticUnitPackagingDate(barCodeData);
            barCodeData = barCodeData.Remove(0, 2);     // pack station
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = barCodeData.Remove(0, 3);     // carton number
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = this.IdentifyGtin(barCodeData);    // do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = this.IdentifyNetWeightMetricKg(barCodeData);
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = barCodeData.Remove(0, 2);     // year
            barCodeData = barCodeData.Remove(0, 3);     // day of the year
            barCodeData = barCodeData.Remove(0, 2);     // pack location

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData[..5];
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
            this.Gtin = this.Data[..5].PadLeft(13, '0');

            // Do not remove the GTIN from the data
            return this.Data;
        }

        /// <summary>
        /// Identifies the logistic unit packaging date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the packaging date.</returns>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            var year = Convert.ToInt32(barCodeData[..2]);
            if (year < 80)
            {
                year += 2000;
            }
            else
            {
                year += 1900;
            }

            var dayOfTheYear = Convert.ToInt32(barCodeData.Substring(2, 3));
            var dt = new DateTime(year, 1, 1).AddDays(dayOfTheYear - 1);

            this.LogisticUnitProductionDate = dt;
            barCodeData = barCodeData.Remove(0, 5);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the net weight in metric kilograms from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the net weight in metric kilograms.</returns>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            this.NetWeightMetricKgFoundAi = false;

            // Get the number of decimal places
            var iDecPlaces = 2;

            // Assign the net weight in metric kilograms
            var wholeNumber = barCodeData[..4];
            this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^iDecPlaces] + "." + wholeNumber[^iDecPlaces..]);
            ////if (iDecPlaces != 0)
            ////{
            ////    this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^iDecPlaces] + "." + wholeNumber[^iDecPlaces..]);
            ////}
            ////else
            ////{
            ////    this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            ////}

            // Remove the net weight in metric kilograms from the barcode data
            barCodeData = barCodeData.Remove(0, 4);

            return barCodeData;
        }
    }
}
