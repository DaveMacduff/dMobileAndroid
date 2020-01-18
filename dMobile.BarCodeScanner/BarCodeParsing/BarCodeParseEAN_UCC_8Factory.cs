namespace dMobile.BarCodeScanner
{
    class BarCodeParseEAN_UCC_8Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UCC_8();
        }
    }
}
