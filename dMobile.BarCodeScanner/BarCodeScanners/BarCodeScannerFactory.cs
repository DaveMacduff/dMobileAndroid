namespace dMobile.BarCodeScanner
{
  /// <summary>
  /// This is the abstract barcode scanner class factory.
  /// </summary>
  public abstract class BarCodeScannerFactory
  {
    public abstract BarCodeScanner GetBarCodeScanner();
  }
}
