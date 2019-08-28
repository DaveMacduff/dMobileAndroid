namespace dMobile.BarCodeScanner
{
  class BarCodeParseCode_39Factory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCode_39();
    }
  }
}
