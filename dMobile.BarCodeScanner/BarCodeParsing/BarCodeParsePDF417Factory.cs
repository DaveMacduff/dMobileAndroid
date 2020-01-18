namespace dMobile.BarCodeScanner
{
    class BarCodeParsePDF417Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParsePDF417();
        }
    }
}
