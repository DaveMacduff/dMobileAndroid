namespace dMobile.BarCodeScanner
{
    class BarCodeParseEAN_UCC_13Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UCC_13();
        }
    }
}
