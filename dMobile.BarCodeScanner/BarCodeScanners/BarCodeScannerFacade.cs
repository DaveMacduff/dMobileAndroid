// <copyright file="BarCodeScannerFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;

    /// <summary>
    /// This is the barcode scanner facade class that return a generic barcode scanner object.
    /// </summary>
    public class BarCodeScannerFacade
    {
        /// <summary>
        /// Creates and returns a generic (device independent) barcode scanner object.
        /// </summary>
        /// <returns>Generic barcode scanner object.</returns>
        public static BarCodeScanner GetBarCodeScanner()
        {
            BarCodeScannerFactory scannerFactory = new HoneywellBarCodeScannerFactory();
            BarCodeScanner scanner;

            if (scannerFactory != null)
            {
                scanner = scannerFactory.GetBarCodeScanner();
            }
            else
            {
                throw new NotSupportedException("Unidentified bar code scanner type.");
            }

            return scanner;
        }
    }
}
