namespace dMobile.BarCodeScanner
{
    class BarCodeParseSSCC_18Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseSSCC_18();
        }
    }
}
