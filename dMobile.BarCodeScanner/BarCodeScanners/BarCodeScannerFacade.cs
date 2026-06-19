// <copyright file="BarCodeScannerFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using Bedrock;

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
            var scannerType = BarCodeScannerType.ZebraDataWedge;
            BarCodeScannerFactory scannerFactory = scannerType switch
            {
                BarCodeScannerType.Honeywell => new HoneywellBarCodeScannerFactory(),
                BarCodeScannerType.ZebraDataWedge => new ZebraDataWedgeScannerFactory(),
                _ => throw new BaseException("Undefined barcode scanner type."),
            };
            return scannerFactory.GetBarCodeScanner();
        }
    }
}