namespace dMobile.BarCodeScanner
{
  class BarCodeParseITF_14_SymbolFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseITF_14_Symbol();
    }
  }
}
