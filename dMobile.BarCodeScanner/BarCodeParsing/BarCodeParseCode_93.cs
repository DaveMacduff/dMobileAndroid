namespace dMobile.BarCodeScanner
{
  using System;

  /// <summary>
  /// Parsing rules reviewed
  /// </summary>
  public class BarCodeParseCode_93 : BarCodeParse
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
