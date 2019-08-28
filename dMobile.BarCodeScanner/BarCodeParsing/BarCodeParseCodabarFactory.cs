namespace dMobile.BarCodeScanner
{
  class BarCodeParseCodabarFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCodabar();
    }
  }
}
