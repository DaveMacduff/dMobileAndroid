// <copyright file="ZebraIntentScannerService.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;

    /// <summary>
    /// Platform-agnostic singleton service that receives scan events
    /// raised by the Android layer via <see cref="IZebraIntentScanner"/>.
    /// </summary>
    public class ZebraIntentScannerService : IZebraIntentScanner
    {
        private static ZebraIntentScannerService instance;

        public static ZebraIntentScannerService Instance => instance ??= new ZebraIntentScannerService();

        public event EventHandler<ZebraScanEventArgs> BarcodeScanned;

        // Called by MainActivity.OnNewIntent (Android layer only)
        public void RaiseBarcodeScanned(string data, string type)
        {
            this.BarcodeScanned?.Invoke(this, new ZebraScanEventArgs(data, type));
        }
    }
}