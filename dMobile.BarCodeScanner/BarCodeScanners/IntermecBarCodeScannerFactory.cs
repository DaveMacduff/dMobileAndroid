namespace dMobile.BarCodeScanner
{
  /// <summary>
  /// This is the barcode scanner class factory for Intermec devices.
  /// </summary>
  public class IntermecBarCodeScannerFactory : BarCodeScannerFactory
  {
    public override BarCodeScanner GetBarCodeScanner()
    {
      return new IntermecBarCodeScanner();
    }
  }
}
