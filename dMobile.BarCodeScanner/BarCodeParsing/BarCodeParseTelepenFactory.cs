namespace dMobile.BarCodeScanner
{
    class BarCodeParseTelepenFactory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseTelepen();
        }
    }
}
