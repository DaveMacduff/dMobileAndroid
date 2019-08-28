namespace dMobile.BarCodeScanner
{
  class BarCodeParseUnDefinedFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseUnDefined();
    }
  }
}
