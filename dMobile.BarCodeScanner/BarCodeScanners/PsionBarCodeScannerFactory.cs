namespace dMobile.BarCodeScanner
{
    public class PsionBarCodeScannerFactory : BarCodeScannerFactory
    {
        public override BarCodeScanner GetBarCodeScanner()
        {
            return new PsionBarCodeScanner();
        }
    }
}
