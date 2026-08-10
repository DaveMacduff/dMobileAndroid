// <copyright file="ZebraDataWedgeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using Android;
    using Android.Content;
    using Android.OS;
    using System;
    using System.Threading.Tasks;

    public class ZebraDataWedgeScanner : BarCodeScanner
    {
        /// <summary>
        /// Opens the scanner reader and loads symbology settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task OpenBarcodeReader()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the scanner reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task CloseBarcodeReader()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Enables scanning on the barcode reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task EnableScanning()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disables scanning on the barcode reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task DisableScanning()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Terminates the scanner.
        /// </summary>
        public override void Terminate()
        {
        }

        /// <summary>
        /// Disposes the resources used by the scanner.
        /// </summary>
        /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to clean up managed resources
                this.Terminate();
            }
        }
    }
}
