namespace dMobile.BarCodeScanner
{
  class BarCodeParseCodablockFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCodablock();
    }
  }
}
