using System;

namespace dMobile.BarCodeScanner
{
    public class PsionBarCodeScannerFactory : BarCodeScannerFactory
    {
        public override BarCodeScanner GetBarCodeScanner()
        {
            throw new NotImplementedException("No Implemented");
            //return new PsionBarCodeScanner();
        }
    }
}
