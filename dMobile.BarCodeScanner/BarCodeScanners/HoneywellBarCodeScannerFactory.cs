// <copyright file="HoneywellBarCodeScannerFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// Defines the <see cref="HoneywellBarCodeScannerFactory" /> Honeywell scanner factory.
    /// </summary>
    public class HoneywellBarCodeScannerFactory : BarCodeScannerFactory
    {
        /// <summary>
        /// Get the Honeywell bar code scanner instance.
        /// </summary>
        /// <returns>The <see cref="BarCodeScanner"/>.</returns>
        public override BarCodeScanner GetBarCodeScanner()
        {
            return new HoneywellBarCodeScanner();
        }
    }
}
