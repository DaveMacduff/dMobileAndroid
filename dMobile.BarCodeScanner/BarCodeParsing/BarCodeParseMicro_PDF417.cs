namespace dMobile.BarCodeScanner
{
    using System;

    public class BarCodeParseMicro_PDF417 : BarCodeParse
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
    }
}
