namespace dMobile.BarCodeScanner
{
    using System;

    class BarCodeParseCode_128_FINICA_as_EAN_UCC_128 : BarCodeParse
    {
        #region Public Members

        /// <summary>
        /// Parsing rules reviewed
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyGTIN(barCodeData);    // do not remove data
            barCodeData = this.IdentifyProductIdentifier(barCodeData);
            barCodeData = this.IdentifyNetWeightMetricKG(barCodeData);
            barCodeData = this.IdentifyLogisticUnitBestBeforeSellByDate(barCodeData);
            this.Remainder = barCodeData;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (barCodeData.Length < 14)
            {
                this.ProductIdentifier = barCodeData.PadLeft(14, '0');
            }
            else if (barCodeData.Length > 14)
            {
                this.ProductIdentifier = barCodeData.Substring(0, 14);
            }
            else
            {
                this.ProductIdentifier = barCodeData;
            }

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTIN(string barCodeData)
        {
            this.Data = barCodeData;

            this.GTIN = this.Data.Substring(0, 14);

            // Do not remove the gtin from the data

            return this.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            // Assign the BEST BEFORE SELL BY DATE
            int year = Convert.ToInt32(barCodeData.Substring(0, 2));
            if (year < 80)
            {
                year = year + 2000;
            }
            else
            {
                year = year + 1900;
            }

            DateTime dt = new DateTime(year, Convert.ToInt32(barCodeData.Substring(2, 2)),
                Convert.ToInt32(barCodeData.Substring(4, 2)));   //101026
            this.LogisticUnitBestBeforeSellByDate = dt;

            // remove the BEST BEFORE SELL BY DATE from the bar code data
            barCodeData = barCodeData.Remove(0, 6);

            return barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyNetWeightMetricKG(string barCodeData)
        {
            this.NetWeightMetricKGFoundAI = true;

            // Get the number of decimal places
            int places = 2;

            // assign the METRIC NET WEIGHT KG
            string wholeNumber = barCodeData.Substring(0, 6);
            if (places != 0)
            {
                this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber.Substring(0,
                    wholeNumber.Length - places) + "." + wholeNumber.Substring(wholeNumber.Length - places));
            }
            else
            {
                this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber);
            }

            // Remove the METRIC NET WEIGHT KG from the bar code data
            barCodeData = barCodeData.Remove(0, 6);

            return barCodeData;
        }

        #endregion
    }
}
