namespace dMobile.BarCodeScanner
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using Honeywell.AIDC.CrossPlatform;

    /// <summary>
    /// This is the barcode scanner facade class that return a generic barcode scanner object.
    /// </summary>
    public class BarCodeScannerFacade
    {
        ///// <summary>
        ///// Creates and returns a generic (device independent) barcode scanner object.
        ///// </summary>
        ///// <returns>Generic barcode scanner object</returns>
        //public static BarCodeScanner GetBarCodeScanner()
        //{
        //    BarCodeScanner scanner = null;
        //    BarCodeScannerFactory scannerFactory = null;

        //    string oemInfo = GetOemInfo();
        //    string lowerOEM = oemInfo.ToLower();
        //    // Is this a Symbol device?
        //    if (lowerOEM.IndexOf("symbol") > -1)
        //    {
        //        scannerFactory = new SymbolBarCodeScannerFactory();
        //    }

        //    // Is this an Intermec device?
        //    if (lowerOEM.IndexOf("intermec") > -1 || oemInfo.Length == 50)
        //    {
        //        scannerFactory = new IntermecBarCodeScannerFactory();
        //    }

        //    // Is this an LXE device?
        //    if (lowerOEM.IndexOf("lxe") > -1)
        //    {
        //        scannerFactory = new LXEBarCodeScannerFactory();
        //    }

        //    // Is this an LXE WinCE6 device?
        //    if (lowerOEM.IndexOf("venture") > -1)
        //    {
        //        scannerFactory = new LXEBarCodeScannerFactory();
        //    }

        //    if (lowerOEM.IndexOf("psion") != -1)
        //    {
        //        scannerFactory = new PsionBarCodeScannerFactory();
        //    }

        //    if (lowerOEM.IndexOf("motorola wt41n0") > -1)
        //    {
        //        scannerFactory = new SymbolBarCodeScannerFactory();
        //    }

        //    // Create generic barcode reader object
        //    if (scannerFactory != null)
        //    {
        //        scanner = scannerFactory.GetBarCodeScanner();
        //    }
        //    else
        //    {
        //        // TODO: Resource strings
        //        throw new NotSupportedException("Unidentified bar code scanner type");
        //    }

        //    return scanner;
        //}

        /// <summary>
        /// Creates and returns a generic (device independent) barcode scanner object.
        /// </summary>
        /// <returns>Generic barcode scanner object</returns>
        public static BarCodeScanner GetBarCodeScanner()
        {
            // TODO: Get device type
            BarCodeScanner scanner = null;
            BarCodeScannerFactory scannerFactory = null;

            scannerFactory = new HoneywellBarCodeScannerFactory();

            // Create generic barcode reader object
            if (scannerFactory != null)
            {
                scanner = scannerFactory.GetBarCodeScanner();
            }
            else
            {
                // TODO: Resource strings
                throw new NotSupportedException("Unidentified bar code scanner type");
            }

            return scanner;
        }
    }
}
