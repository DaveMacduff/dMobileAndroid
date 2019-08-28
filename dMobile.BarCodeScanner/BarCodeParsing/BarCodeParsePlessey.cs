namespace dMobile.BarCodeScanner
{
  using System;

  public class BarCodeParsePlessey : BarCodeParse
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      barCodeData = this.IdentifyProductIdentifier(barCodeData);

      this.Remainder = barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyProductIdentifier(string barCodeData)
    {
      ProductIdentifier = barCodeData;

      barCodeData = barCodeData.Remove(0, ProductIdentifier.Length);

      return barCodeData;
    }
  }
}
