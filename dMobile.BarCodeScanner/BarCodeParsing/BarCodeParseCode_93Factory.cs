namespace dMobile.BarCodeScanner
{
    class BarCodeParseCode_93Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_93();
        }
    }
}
