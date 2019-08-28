namespace dMobile.BarCodeScanner
{
  class BarCodeParseCodablock256Factory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCodablock256();
    }
  }
}
