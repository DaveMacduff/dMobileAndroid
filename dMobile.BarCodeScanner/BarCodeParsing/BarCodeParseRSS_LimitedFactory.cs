namespace dMobile.BarCodeScanner
{
  class BarCodeParseRSS_LimitedFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseRSS_Limited();
    }
  }
}
