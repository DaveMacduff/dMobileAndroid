namespace dMobile.BarCodeScanner
{
    using System;

    public class BarCodeParseEAN_UCC_8 : BarCodeParse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = this.IdentifyProductIdentifier(barCodeData);   // do not remove data
            barCodeData = this.IdentifyGTIN(barCodeData);

            this.Remainder = barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (Settings.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family)
            {
                this.ProductIdentifier = barCodeData;
            }
            else
            {
                this.ProductIdentifier = barCodeData.PadLeft(14, '0');
            }

            return barCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTIN(string barCodeData)
        {
            int len = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.GTIN = barCodeData.Substring(0, len).PadLeft(14, '0');

            barCodeData = barCodeData.Remove(0, len);

            return barCodeData;
        }
    }
}
