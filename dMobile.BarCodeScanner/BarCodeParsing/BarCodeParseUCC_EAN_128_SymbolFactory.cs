namespace dMobile.BarCodeScanner
{
    class BarCodeParseUCC_EAN_128_SymbolFactory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseUCC_EAN_128_Symbol();
        }
    }
}
