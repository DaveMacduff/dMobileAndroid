namespace dMobile.BarCodeScanner
{
    using System;

    public class BarCodeParseMatrix_2_of_5 : BarCodeParse
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
            this.ProductIdentifier = barCodeData;

            barCodeData = barCodeData.Remove(0, ProductIdentifier.Length);

            return barCodeData;
        }
    }
}
