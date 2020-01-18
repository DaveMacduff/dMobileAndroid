namespace dMobile.BarCodeScanner
{
    using System;

    public class BarCodeParseRSS_Limited : BarCodeParse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            ProductIdentifier = barCodeData;

            barCodeData = barCodeData.Remove(0, ProductIdentifier.Length);

            return barCodeData;
        }
    }
}
