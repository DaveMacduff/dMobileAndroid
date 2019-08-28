namespace dMobile.BarCodeScanner
{
  using System;

  /// <summary>
  /// This is the event arguments for the BarCode Scanner class event BarCodeScan.
  /// </summary>
  public class BarCodeScannerEventArgs : EventArgs
  {
    #region Fields

    private BarCodeParse parseInfo = null;

    #endregion

    #region Constructors

    public BarCodeScannerEventArgs(string strPData)
    {
      this.ParseInfo = BarCodeParseFacade.ParseData(strPData);
    }

    #endregion

    #region Properties

    public BarCodeParse ParseInfo
    {
      get
      {
        return this.parseInfo;
      }

      set
      {
        this.parseInfo = value;
      }
    }

    #endregion
  }
}
