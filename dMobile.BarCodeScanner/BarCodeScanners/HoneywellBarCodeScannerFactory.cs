using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    public class HoneywellBarCodeScannerFactory : BarCodeScannerFactory
    {
        public override BarCodeScanner GetBarCodeScanner()
        {
            return new HoneywellBarCodeScanner();
        }
    }
}
