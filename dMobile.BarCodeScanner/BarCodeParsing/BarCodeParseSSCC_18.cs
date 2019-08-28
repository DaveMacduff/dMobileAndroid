namespace dMobile.BarCodeScanner
{
  using System;

  public class BarCodeParseSSCC_18 : BarCodeParse
  {
    /// <summary>
    /// Data structure is always 18 digits
    /// Position 1 is indicator digit
    /// Position 2-16 company prefix and manufacturer serial number
    /// Position 18 is a check digit
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      // do not remove data
      barCodeData = this.IdentifySSCC18(barCodeData);

      // must follow IdentifySSCC18(), also do not remove data
      barCodeData = this.IdentifyProductIdentifier(barCodeData);

      barCodeData = this.IdentifyPackagingType(barCodeData);
      barCodeData = this.IdentifyCompanyPrefix(barCodeData);
      barCodeData = this.IdentifyManufacturerSerialNumber(barCodeData);

      // remove the check digit if it is there
      if (barCodeData.Length > 0)
      {
        barCodeData = this.Data.Remove(0, 1);
      }

      this.Remainder = barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyManufacturerSerialNumber(string barCodeData)
    {
      this.ManufacturerSerialNumber = Convert.ToInt64(barCodeData.Substring(0, 9));

      // remove the Manufacturer Serial Number from the bar code data
      barCodeData = barCodeData.Remove(0, 9);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyPackagingType(string barCodeData)
    {
      this.PackagingType = Convert.ToInt32(barCodeData.Substring(0, 1));

      // remove the Packaging Type from the bar code data
      barCodeData = barCodeData.Remove(0, 1);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyProductIdentifier(string barCodeData)
    {
      this.ProductIdentifier = barCodeData;

      //barCodeData = barCodeData.Remove(0, strProductIdentifier.Length);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyCompanyPrefix(string barCodeData)
    {
      this.CompanyPrefix = barCodeData.Substring(0, 7);

      // remove the Company Prefix from the bar code data
      barCodeData = barCodeData.Remove(0, CompanyPrefix.Length);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifySSCC18(string barCodeData)
    {
      if (barCodeData.StartsWith("00") && barCodeData.Length >= 18)
      {
        // remove the AI
        barCodeData = barCodeData.Remove(0, 2);

        // assign the SSCC-18
        this.SSCC18 = barCodeData.Substring(0, 18);

        // remove the SSCC-18 from the bar code data
        // barCodeData = barCodeData.Remove(0, strSSCC18.Length);
      }
      return barCodeData;
    }
  }
}
