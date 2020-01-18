namespace dMobile.BarCodeScanner
{
    class BarCodeParseEAN_UPC_SymbolFactory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseEAN_UPC_Symbol();
        }
    }
}
