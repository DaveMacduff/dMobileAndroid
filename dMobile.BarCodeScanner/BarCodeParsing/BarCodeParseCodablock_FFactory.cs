namespace dMobile.BarCodeScanner
{
  class BarCodeParseCodablock_FFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCodablock_F();
    }
  }
}
