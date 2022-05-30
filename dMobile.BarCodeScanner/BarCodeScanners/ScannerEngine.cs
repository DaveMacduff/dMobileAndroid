// <copyright file="ScannerEngine.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    /// <summary>
    /// Defines the <see cref="ScannerEngine"/> .
    /// </summary>
    public sealed class ScannerEngine
    {
        /// <summary>
        /// Defines the scanner.
        /// </summary>
        private static BarCodeScanner scanner = BarCodeScannerFacade.GetBarCodeScanner();

        /// <summary>
        /// Prevents a default instance of the <see cref="ScannerEngine"/> class from being created.
        /// </summary>
        private ScannerEngine()
        {
        }

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        public BarCodeScanner Scanner
        {
            get
            {
                if (scanner == null)
                {
                    scanner = BarCodeScannerFacade.GetBarCodeScanner();
                }

                return scanner;
            }

            set => scanner = value;
        }

        /// <summary>
        /// Gets or sets the scanner engine instance.
        /// </summary>
        private static ScannerEngine Instance { get; set; } = new ScannerEngine();

        /// <summary>
        /// Get the scanner engine instance.
        /// </summary>
        /// <returns>.</returns>
        public static ScannerEngine GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ScannerEngine();
            }

            return Instance;
        }
    }
}
