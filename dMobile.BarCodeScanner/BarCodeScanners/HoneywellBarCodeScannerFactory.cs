// <copyright file="HoneywellBarCodeScannerFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// Defines the Honeywell barcode scanner factory.
    /// </summary>
    public class HoneywellBarCodeScannerFactory : BarCodeScannerFactory
    {
        /// <summary>
        /// Gets the Honeywell barcode scanner instance.
        /// </summary>
        /// <returns>A <see cref="BarCodeScanner"/> instance.</returns>
        public override BarCodeScanner GetBarCodeScanner()
        {
            return new HoneywellBarCodeScanner();
        }
    }
}