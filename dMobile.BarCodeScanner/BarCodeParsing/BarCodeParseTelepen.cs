namespace dMobile.BarCodeScanner
{
  using System;

  public class BarCodeParseTelepen : BarCodeParse
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      barCodeData = this.IdentifyProductIdentifier(barCodeData);

      Remainder = barCodeData;
    }
  }
}
