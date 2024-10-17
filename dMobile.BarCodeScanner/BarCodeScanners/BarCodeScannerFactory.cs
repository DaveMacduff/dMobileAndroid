// <copyright file="BarCodeScannerFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// Represents the abstract factory for creating barcode scanner instances.
    /// </summary>
    public abstract class BarCodeScannerFactory
    {
        /// <summary>
        /// Gets the barcode scanner instance.
        /// </summary>
        /// <returns>A <see cref="BarCodeScanner"/> instance.</returns>
        public abstract BarCodeScanner GetBarCodeScanner();
    }
}