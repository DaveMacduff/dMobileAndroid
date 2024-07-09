// <copyright file="BarCodeParseCode_128_OvationFielding_Lamb.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseCode_128_OvationFielding_Lamb : BarCodeParse
    {
        /// <summary>
        /// Parsing rule class for Ovation Field New Zealand Lamb.
        /// </summary>
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
        /// 51016 = Standpack Number
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
        /// <param name="barCodeData"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            this.ProductIdentifier = barCodeData[..5];
            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGtin(string barCodeData)
        {
            this.Data = barCodeData;

            this.Gtin = this.Data[..5].PadLeft(13, '0');

            // strBarCodeData = strBarCodeData.Remove(0, 5);
            return this.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            var year = Convert.ToInt32(barCodeData[..2]);
            if (year < 80)
            {
                year = year + 2000;
            }
            else
            {
                year = year + 1900;
            }

            var dayOfTheYear = Convert.ToInt32(barCodeData.Substring(2, 3));

            var dt = new DateTime(year, 1, 1);

            dt = dt.AddDays(dayOfTheYear - 1);

            this.LogisticUnitProductionDate = dt;

            barCodeData = barCodeData.Remove(0, 5);

            return barCodeData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            this.NetWeightMetricKgFoundAi = false;

            // get the number of decimal places
            var iDecPlaces = 2;

            // assign the METRIC NET WEIGHT KG
            var wholeNumber = barCodeData[..4];
            if (iDecPlaces != 0)
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber[..^iDecPlaces] + "." + wholeNumber[^iDecPlaces..]);
            }
            else
            {
                this.NetWeightMetricKg = Convert.ToDecimal(wholeNumber);
            }

            // remove the METRIC NET WEIGHT KG from the bar code data
            barCodeData = barCodeData.Remove(0, 4);

            return barCodeData;
        }
    }
}
