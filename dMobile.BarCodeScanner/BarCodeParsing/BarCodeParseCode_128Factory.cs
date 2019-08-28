namespace dMobile.BarCodeScanner
{
  class BarCodeParseCode_128Factory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseCode_128();
    }
  }
}
