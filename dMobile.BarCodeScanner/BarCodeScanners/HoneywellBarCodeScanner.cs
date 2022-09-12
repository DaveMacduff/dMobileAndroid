// <copyright file="HoneywellBarCodeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Android.Util;
    using dMobile.Resources.Services;
    using Honeywell.AIDC.CrossPlatform;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="HoneywellBarCodeScanner" />.
    /// </summary>
    public partial class HoneywellBarCodeScanner : BarCodeScanner
    {
        /// <summary>
        /// Gets or sets the Honeywell bar code reader instance.
        /// </summary>
        private BarcodeReader HoneywellReader { get; set; } = new BarcodeReader();

        /// <summary>
        /// Gets or sets a value indicating whether the symbology settings are set.
        /// </summary>
        private bool IsSymbologySettingsSet { get; set; } = false;

        /// <summary>
        /// Gets or sets the honey well scanner service.
        /// </summary>
        private IHoneywellScannerService HoneywellScannerService { get; set; } =
            DependencyService.Get<IHoneywellScannerService>();

        /// <summary>
        /// Gets or sets the scanner profile data event handler.
        /// </summary>
        private EventHandler ProfileDataHandler { get; set; }

        /// <summary>
        /// Initialize the scanner.
        /// </summary>
        /// <returns>The <see cref="bool"/> True if the scanner was initialized else false.</returns>
        public override bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Open the scanner reader and load symbology settings.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Start()
        {
            // KLUDGE: There is probably a better way to do this but we need to make sure the scan
            // isn't still reading.
            ////int retry = 0;
            ////while (this.HoneywellReader.IsReaderOpened)
            ////{
            ////    ////Thread.Sleep(100);
            ////    Thread.Sleep(1000);
            ////    retry++;

            ////    ////if (retry > 5)
            ////    ////{
            ////    ////    break;
            ////    ////}

            ////    if (retry > 10)
            ////    {
            ////        break;
            ////    }
            ////}

            if (this.HoneywellReader.IsReaderOpened)
            {
                return;
            }

            if (this.HoneywellReader == null)
            {
                _ = Log.Error("HoneywellBarcodeScanner", "Unexpected this.HoneywellReader == null");
                this.HoneywellReader = new BarcodeReader();
            }

            var result = await this.HoneywellReader.OpenAsync();
            if (result.Code == BarcodeReaderBase.Result.Codes.SUCCESS ||
                result.Code == BarcodeReaderBase.Result.Codes.READER_ALREADY_OPENED)
            {
                this.SetSymbologySettings();
                this.HoneywellReader.BarcodeDataReady += this.HoneywellReader_BarcodeDataReady;
            }
            else
            {
                var errMsg = $"HoneywellBarcodeScanner, OpenAsync failed, Code: {result.Code} Message: {result.Message}.";
                _ = Log.Error("HoneywellBarcodeScanner", errMsg);
                throw new Exception(errMsg);
            }
        }

        /// <summary>
        /// Close the scanner reader.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Stop()
        {
            if (!this.HoneywellReader.IsReaderOpened)
            {
                return;
            }

            if (this.HoneywellReader == null)
            {
                _ = Log.Error("HoneywellBarcodeScanner", "Unexpected this.HoneywellReader == null");
                this.HoneywellReader = new BarcodeReader();
            }

            var result = await this.HoneywellReader.CloseAsync();
            if (result.Code == BarcodeReaderBase.Result.Codes.SUCCESS)
            {
                this.HoneywellReader.BarcodeDataReady -= this.HoneywellReader_BarcodeDataReady;
            }
            else
            {
                var errMsg = $"HoneywellBarcodeScanner, CloseAsync failed, Code: {result.Code} Message: {result.Message}.";
                _ = Log.Error("HoneywellBarcodeScanner", errMsg);
                throw new Exception(errMsg);
            }
        }

        /// <summary>
        /// Terminates the scanner.
        /// </summary>
        public override void Terminate()
        {
            this.HoneywellReader = null;
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        /// <param name="disposing">The disposing<see cref="bool"/>.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to clean up managed resources
                this.Terminate();
            }
        }

        /// <summary>
        /// The HoneywellReader_BarcodeDataReady.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="BarcodeDataArgs"/>.</param>
        private async void HoneywellReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            try
            {
                await this.OnBarCodeScan(new BarCodeScannerEventArgs(e.Data));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
                _ = Log.Error("HoneywellBarcodeScanner", errMsg);
                throw new Exception(errMsg);
            }
        }

        /// <summary>
        /// The SetSymbologySettings.
        /// </summary>
        private void SetSymbologySettings()
        {
            if (!this.IsSymbologySettingsSet)
            {
                if (this.HoneywellReader.IsReaderOpened)
                {
                    this.HoneywellScannerService.ProfileDataReady += this.ProfileDataHandler = async (s, e) =>
                    {
                        try
                        {
                            this.HoneywellScannerService.ProfileDataReady -= this.ProfileDataHandler;
                            var profileSettings = this.HoneywellScannerService.BarcodeReaderSettings;

                            var result = await this.HoneywellReader.SetAsync(profileSettings);

                            if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                            {
                                var errMsg = $"Could not set symbology settings {result.Message}.";
                                throw new Exception(errMsg);
                            }
                            else
                            {
                                this.IsSymbologySettingsSet = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            var errMsg = $"SetSymbologySettings - Error setting symbology settings {ex.Message}";
                            Log.Error("SetSymbologySettings", errMsg);
                            throw new Exception(errMsg);
                        }
                    };

                    this.HoneywellScannerService.GetScannerProfile();
                }
            }
        }
    }
}
