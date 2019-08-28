namespace dMobile.BarCodeScanner
{
  class BarCodeParseUCC_12_UPC_EFactory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseUCC_12_UPC_E();
    }
  }
}
