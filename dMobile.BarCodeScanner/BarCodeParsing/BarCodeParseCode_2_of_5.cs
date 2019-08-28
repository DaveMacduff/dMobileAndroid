namespace dMobile.BarCodeScanner
{
  using System;


  public class BarCodeParseCode_2_of_5 : BarCodeParse
  {
    /// <summary>
    /// Parsing rules reviewed
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      barCodeData = this.IdentifyProductIdentifier(barCodeData);

      Remainder = barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyProductIdentifier(string barCodeData)
    {
      this.ProductIdentifier = barCodeData;

      // remove the Product Identifier from the bar code data
      barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

      return barCodeData;
    }
  }
}
