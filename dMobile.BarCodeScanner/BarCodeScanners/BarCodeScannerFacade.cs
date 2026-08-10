// <copyright file="BarCodeScannerFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using Android.OS;

    /// <summary>
    /// This is the barcode scanner facade class that returns a generic barcode scanner object.
    /// </summary>
    public static class BarCodeScannerFacade
    {
        /// <summary>
        /// Creates and returns a generic (device independent) barcode scanner object.
        /// </summary>
        /// <returns>A generic barcode scanner object.</returns>
        /// <exception cref="NotSupportedException">Thrown when the barcode scanner type is unidentified.</exception>
        public static BarCodeScanner GetBarCodeScanner()
        {
            BarCodeScannerFactory scannerFactory = Build.Manufacturer switch
            {
                "Zebra Technologies" => new ZebraDataWedgeScannerFactory(),
                "Honeywell" => new HoneywellBarCodeScannerFactory(),
                _ => throw new NotSupportedException(
                    $"Barcode scanner type is not supported for manufacturer: {Build.Manufacturer}")
            };

            return scannerFactory.GetBarCodeScanner();
        }
    }
}