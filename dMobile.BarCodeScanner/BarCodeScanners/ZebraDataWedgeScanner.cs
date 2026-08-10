// <copyright file="ZebraDataWedgeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System.Threading.Tasks;

    public class ZebraDataWedgeScanner : BarCodeScanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZebraDataWedgeScanner"/> class.
        /// </summary>
        public ZebraDataWedgeScanner()
        {
            ZebraIntentScannerService.Instance.BarcodeScanned += this.OnZebraIntentBarcodeScanned;
        }

        /// <summary>
        /// Handles the BarcodeScanned event from the ZebraIntentScannerService.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the scanned barcode.</param>
        private async void OnZebraIntentBarcodeScanned(object sender, ZebraScanEventArgs e)
        {
            try
            {
                await this.RaiseBarCodeScan(new BarCodeScannerEventArgs(e.BarcodeData));
            }
            catch
            {
                // ignored
            }
        }

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
            ZebraIntentScannerService.Instance.BarcodeScanned -= this.OnZebraIntentBarcodeScanned;
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
