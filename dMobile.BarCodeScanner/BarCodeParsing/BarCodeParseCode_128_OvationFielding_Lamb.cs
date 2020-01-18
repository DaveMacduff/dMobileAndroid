namespace dMobile.BarCodeScanner
{
    using System;

    public class BarCodeParseCode_128_OvationFielding_Lamb : BarCodeParse
    {
        /// <summary>
        /// Parsing rule class for Ovation Field New Zealand Lamb.
        /// </summary>
        /// <remarks>
        /// See WO # 11127
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
        /// 87 = Pack Location
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
            barCodeData = this.IdentifyGTIN(barCodeData);    // do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = barCodeData.Remove(0, 1);     // check digit
            barCodeData = this.IdentifyNetWeightMetricKG(barCodeData);
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
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            ProductIdentifier = barCodeData.Substring(0, 5);
            barCodeData = barCodeData.Remove(0, ProductIdentifier.Length);

            return barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTIN(string barCodeData)
        {
            Data = barCodeData;

            GTIN = Data.Substring(0, 5).PadLeft(13, '0');

            // strBarCodeData = strBarCodeData.Remove(0, 5);

            return Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            int year = Convert.ToInt32(barCodeData.Substring(0, 2));
            if (year < 80)
            {
                year = year + 2000;
            }
            else
            {
                year = year + 1900;
            }

            int dayOfTheYear = Convert.ToInt32(barCodeData.Substring(2, 3));

            DateTime dt = new DateTime(year, 1, 1);

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
        protected override string IdentifyNetWeightMetricKG(string barCodeData)
        {
            NetWeightMetricKGFoundAI = false;

            // get the number of decimal places
            int iDecPlaces = 2;

            // assign the METRIC NET WEIGHT KG
            string wholeNumber = barCodeData.Substring(0, 4);
            if (iDecPlaces != 0)
            {
                this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber.Substring(0,
                    wholeNumber.Length - iDecPlaces) + "." + wholeNumber.Substring(
                    wholeNumber.Length - iDecPlaces));
            }
            else
            {
                this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber);
            }

            // remove the METRIC NET WEIGHT KG from the bar code data
            barCodeData = barCodeData.Remove(0, 4);

            return barCodeData;
        }
    }
}
