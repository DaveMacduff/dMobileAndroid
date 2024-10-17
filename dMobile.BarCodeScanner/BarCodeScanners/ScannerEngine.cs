// <copyright file="ScannerEngine.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// Represents the scanner engine which provides access to the barcode scanner.
    /// </summary>
    public sealed class ScannerEngine
    {
        /// <summary>
        /// Defines the barcode scanner instance.
        /// </summary>
        private static BarCodeScanner scanner = BarCodeScannerFacade.GetBarCodeScanner();

        /// <summary>
        /// Prevents a default instance of the <see cref="ScannerEngine"/> class from being created.
        /// </summary>
        private ScannerEngine()
        {
        }

        /// <summary>
        /// Gets or sets the barcode scanner instance.
        /// </summary>
        public BarCodeScanner Scanner
        {
            get
            {
                scanner ??= BarCodeScannerFacade.GetBarCodeScanner();
                return scanner;
            }

            set => scanner = value;
        }

        /// <summary>
        /// Gets or sets the scanner engine instance.
        /// </summary>
        private static ScannerEngine Instance { get; set; } = new ScannerEngine();

        /// <summary>
        /// Gets the scanner engine instance.
        /// </summary>
        /// <returns>The <see cref="ScannerEngine"/> instance.</returns>
        public static ScannerEngine GetInstance()
        {
            return Instance ??= new ScannerEngine();
        }
    }
}