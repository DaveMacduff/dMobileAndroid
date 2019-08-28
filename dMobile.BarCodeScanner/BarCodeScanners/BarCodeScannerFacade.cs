namespace dMobile.BarCodeScanner
{
  using System;
  using dMobile.Bedrock;

  /// <summary>
  /// This is the barcode scanner facade class that return a generic barcode scanner object.
  /// </summary>
  public class BarCodeScannerFacade
  {
    #region Public Members

    /// <summary>
    /// Creates and returns a generic (device independent) barcode scanner object.
    /// </summary>
    /// <returns>Generic barcode scanner object</returns>
    public static BarCodeScanner GetBarCodeScanner()
    {
      BarCodeScanner scanner = null;
      BarCodeScannerFactory scannerFactory = null;

      string oemInfo = GetOemInfo();
      string lowerOEM = oemInfo.ToLower();
      // Is this a Symbol device?
      if (lowerOEM.IndexOf("symbol") > -1)
      {
        scannerFactory = new SymbolBarCodeScannerFactory();
      }

      // Is this an Intermec device?
      if (lowerOEM.IndexOf("intermec") > -1 || oemInfo.Length == 50)
      {
        scannerFactory = new IntermecBarCodeScannerFactory();
      }

      // Is this an LXE device?
      if (lowerOEM.IndexOf("lxe") > -1)
      {
        scannerFactory = new LXEBarCodeScannerFactory();
      }

      // Is this an LXE WinCE6 device?
      if (lowerOEM.IndexOf("venture") > -1)
      {
        scannerFactory = new LXEBarCodeScannerFactory();
      }

      if (lowerOEM.IndexOf("psion") != -1)
      {
          scannerFactory = new PsionBarCodeScannerFactory();
      }

      if (lowerOEM.IndexOf("motorola wt41n0") > -1)
      {
        scannerFactory = new SymbolBarCodeScannerFactory();
      }

      // Create generic barcode reader object
      if (scannerFactory != null)
      {
        scanner = scannerFactory.GetBarCodeScanner();
      }
      else
      {
#if MULTILANGUAGE
        throw new NotSupportedException(Translator.GetInstance().Translate(TranslatorVocabulary.UnidentifiedBarCodeScannerType));
#else
        throw new NotSupportedException("Unidentified bar code scanner type");
#endif
      }

      return scanner;
    }

    #endregion

    #region Private Members

    #region Get OEM Info (incl API declarations)

    /// <summary>
    /// Retrieves the OEM information from the device.
    /// </summary>
    /// <returns>OEM Information (manufacturer and model)</returns>
    private static string GetOemInfo()
    {
      int oemIdx = 0;
      string oemInfo = new string(' ', 50);
      int result = SystemParametersInfo(SPI_GETOEMINFO, 50, oemInfo, 0);
      oemIdx = oemInfo.IndexOf('\0');

      if (oemIdx > 0)
      {
        oemInfo = oemInfo.Substring(0, oemIdx);
      }

      return oemInfo;
    }

    [System.Runtime.InteropServices.DllImport("coredll.dll")]
    private static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, int fWinIni);

    private const int SPI_GETOEMINFO = 258;

    #endregion

    #endregion
  }
}
