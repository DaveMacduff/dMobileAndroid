namespace dMobile.BarCodeScanner
{
  class BarCodeParseMatrix_2_of_5Factory : BarCodeParseFactory
  {
    public override BarCodeParse GetBarCodeParse()
    {
      return new BarCodeParseMatrix_2_of_5();
    }
  }
}
