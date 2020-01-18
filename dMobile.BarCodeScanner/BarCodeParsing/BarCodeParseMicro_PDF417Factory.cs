namespace dMobile.BarCodeScanner
{
    class BarCodeParseMicro_PDF417Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseMicro_PDF417();
        }
    }
}
