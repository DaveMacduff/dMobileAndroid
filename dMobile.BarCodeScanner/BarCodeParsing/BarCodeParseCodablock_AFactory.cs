namespace dMobile.BarCodeScanner
{
  class BarCodeParseCodablock_AFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCodablock_A();
    }
  }
}
