using System;
namespace dMobile.BarCodeScanner
{
  /// <summary>
  /// This is the barcode scanner class factory for LXE devices.
  /// </summary>
  public class LXEBarCodeScannerFactory : BarCodeScannerFactory
  {
    public override BarCodeScanner GetBarCodeScanner()
    {
      throw new NotImplementedException("No Implemented");
      //return new LXEBarCodeScanner();
    }
  }
}
