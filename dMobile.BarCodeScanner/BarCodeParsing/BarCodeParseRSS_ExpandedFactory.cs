namespace dMobile.BarCodeScanner
{
  class BarCodeParseRSS_ExpandedFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseRSS_Expanded();
    }
  }
}
