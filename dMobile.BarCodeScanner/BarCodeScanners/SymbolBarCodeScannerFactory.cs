namespace dMobile.BarCodeScanner
{
  /// <summary>
  /// This is the barcode scanner class factory for Symbol devices.
  /// </summary>
  public class SymbolBarCodeScannerFactory : BarCodeScannerFactory
  {
    public override BarCodeScanner GetBarCodeScanner()
    {
      return new SymbolBarCodeScanner();
    }
  }
}
