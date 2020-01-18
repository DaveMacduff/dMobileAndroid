namespace dMobile.BarCodeScanner
{
    class BarCodeParseEAN_UCC_14Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UCC_14();
        }
    }
}
