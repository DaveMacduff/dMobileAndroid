// <copyright file="IZebraIntentScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;

    /// <summary>
    /// Abstraction for receiving Zebra intent-based scan data.
    /// Implemented in the Android layer, consumed in .NET Standard.
    /// </summary>
    public interface IZebraIntentScanner
    {
        /// <summary>Raised when a barcode is scanned via Zebra DataWedge intent.</summary>
        event EventHandler<ZebraScanEventArgs> BarcodeScanned;
    }

    public class ZebraScanEventArgs : EventArgs
    {
        public string BarcodeData { get; }

        public string BarcodeType { get; }

        public ZebraScanEventArgs(string barcodeData, string barcodeType)
        {
            this.BarcodeData = barcodeData;
            this.BarcodeType = barcodeType;
        }
    }
}