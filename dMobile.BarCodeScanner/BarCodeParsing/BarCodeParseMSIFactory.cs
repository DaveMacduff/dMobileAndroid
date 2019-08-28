namespace dMobile.BarCodeScanner
{
  class BarCodeParseMSIFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseMSI();
    }
  }
}
