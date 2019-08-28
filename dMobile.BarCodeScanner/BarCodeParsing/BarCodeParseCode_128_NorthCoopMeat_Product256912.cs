namespace dMobile.BarCodeScanner
{
  using System;

  class BarCodeParseCode_128_NorthCoopMeat_Product256912 : BarCodeParse
  {
    #region Public Members

    /// <summary>
    /// Parsing rules reviewed
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    public override void ParseBarCodeData(string barCodeData)
    {
      barCodeData = this.IdentifyProductIdentifier(barCodeData);
      barCodeData = this.IdentifyNetWeightMetricKG(barCodeData);
      barCodeData = this.IdentifyLogisticUnitProductionDate(barCodeData);
      this.Remainder = barCodeData;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyProductIdentifier(string barCodeData)
    {
      if (barCodeData.Length < 7)
      {
        this.ProductIdentifier = barCodeData.PadLeft(6, '0');
      }
      else if (barCodeData.Length > 6)
      {
        this.ProductIdentifier = barCodeData.Substring(0, 6);
      }
      else
      {
        this.ProductIdentifier = barCodeData;
      }

      barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyNetWeightMetricKG(string barCodeData)
    {
      this.NetWeightMetricKGFoundAI = true;

      // Get the number of decimal places
      int places = 2;

      // assign the METRIC NET WEIGHT KG
      string wholeNumber = barCodeData.Substring(0, 4);
      if (places != 0)
      {
        this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber.Substring(0,
            wholeNumber.Length - places) + "." + wholeNumber.Substring(wholeNumber.Length - places));
      }
      else
      {
        this.NetWeightMetricKG = Convert.ToDecimal(wholeNumber);
      }

      // Remove the METRIC NET WEIGHT KG from the bar code data
      barCodeData = barCodeData.Remove(0, 4);

      return barCodeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barCodeData"></param>
    /// <returns></returns>
    protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
    {
      string tempData = barCodeData;

      if (tempData.Length >= 8)
      {
        // assign the PRODUCTION DATE
        int year = Convert.ToInt32(tempData.Substring(0, 2));
        if (year < 80)
        {
          year = year + 2000;
        }
        else
        {
          year = year + 1900;
        }

        DateTime dt = new DateTime(
            year,
            Convert.ToInt32(tempData.Substring(2, 2)),
            Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

        this.LogisticUnitProductionDate = dt;

        // remove the PRODUCTION DATE from the bar code data
        tempData = tempData.Remove(0, 6);
      }

      return tempData;
    }

    #endregion
  }
}
