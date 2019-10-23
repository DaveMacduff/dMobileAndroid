using System;

namespace dMobile.BarCodeScanner
{
  /// <summary>
  /// This is the barcode scanner class factory for Intermec devices.
  /// </summary>
  public class IntermecBarCodeScannerFactory : BarCodeScannerFactory
  {
    public override BarCodeScanner GetBarCodeScanner()
    {
        throw new NotImplementedException("No Implemented");
      //return new IntermecBarCodeScanner();
    }
  }
}
