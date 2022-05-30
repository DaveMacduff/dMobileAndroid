// <copyright file="BarCodeScannerFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// This is the abstract barcode scanner class factory.
    /// </summary>
    public abstract class BarCodeScannerFactory
    {
        /// <summary>
        /// Get the bar code scanner instance.
        /// </summary>
        /// <returns><see cref="BarCodeScanner"/> Bar code scanner instance.</returns>
        public abstract BarCodeScanner GetBarCodeScanner();
    }
}
