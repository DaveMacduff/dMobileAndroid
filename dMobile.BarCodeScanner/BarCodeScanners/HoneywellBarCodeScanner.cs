// <copyright file="HoneywellBarCodeScanner.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeScanners
{
    using System;
    using System.Threading.Tasks;
    using Honeywell.AIDC.CrossPlatform;
    using Resources.Services;
    using Xamarin.Forms;

    /// <summary>
    /// Represents a barcode scanner for Honeywell devices.
    /// </summary>
    public class HoneywellBarCodeScanner : BarCodeScanner
    {
        /// <summary>
        /// Gets or sets the Honeywell barcode reader instance.
        /// </summary>
        private BarcodeReader HoneywellReader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the symbology settings are set.
        /// </summary>
        private bool IsSymbologySettingsSet { get; set; }

        /// <summary>
        /// Gets or sets the Honeywell scanner service.
        /// </summary>
        private IHoneywellScannerService HoneywellScannerService { get; set; } =
            DependencyService.Get<IHoneywellScannerService>();

        /// <summary>
        /// Gets or sets the scanner profile data event handler.
        /// </summary>
        private EventHandler ProfileDataHandler { get; set; }

        /// <summary>
        /// Opens the scanner reader and loads symbology settings.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the barcode reader cannot be opened.</exception>
        public override async Task OpenBarcodeReader()
        {
            this.HoneywellReader ??= new BarcodeReader();

            ////if (this.HoneywellReader.IsReaderOpened)
            ////{
            ////    ////throw new Exception("Barcode reader cannot be opened while the reader is open.");
            ////}

            if (!this.HoneywellReader.IsReaderOpened)
            {
                var result = await this.HoneywellReader.OpenAsync();
                if (result.Code == BarcodeReaderBase.Result.Codes.SUCCESS ||
                    result.Code == BarcodeReaderBase.Result.Codes.READER_ALREADY_OPENED)
                {
                    this.SetSymbologySettings();
                    this.HoneywellReader.BarcodeDataReady += this.HoneywellReader_BarcodeDataReady;
                }
                else
                {
                    var errMsg =
                        $"HoneywellBarcodeScanner, OpenAsync failed, Code: {result.Code} Message: {result.Message}.";
                    ////_ = Log.Error("HoneywellBarcodeScanner", errMsg);
                    throw new Exception(errMsg);
                }
            }
        }

        /// <summary>
        /// Closes the scanner reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the barcode reader cannot be closed.</exception>
        public override async Task CloseBarcodeReader()
        {
            if (this.HoneywellReader.IsReaderOpened)
            {
                ////throw new Exception("Barcode reader cannot be closed while the reader is open.");
            }
            else
            {
                await this.HoneywellReader.SoftwareTriggerAsync(false);
                var resultFirst = await this.HoneywellReader.CloseAsync();
                if (resultFirst.Code == BarcodeReaderBase.Result.Codes.SUCCESS)
                {
                    this.HoneywellReader.BarcodeDataReady -= this.HoneywellReader_BarcodeDataReady;
                }
                else
                {
                    var errMsg = $"HoneywellBarcodeScanner, CloseAsync failed, Code: {resultFirst.Code} Message: {resultFirst.Message}.";
                    throw new Exception(errMsg);
                }

                this.HoneywellReader = null;
            }
        }

        /// <summary>
        /// Enables scanning on the barcode reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task EnableScanning()
        {
            // Software trigger turns off and on the scanner light and enables scanning.
            // We will rely on the hardware trigger to enable scanning.
            ////await this.HoneywellReader.SoftwareTriggerAsync(true);
            await this.HoneywellReader.EnableAsync(true);
        }

        /// <summary>
        /// Disables scanning on the barcode reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task DisableScanning()
        {
            // Software trigger turns off and on the scanner light and enables scanning.
            // We will rely on the hardware trigger to enable scanning.
            ////await this.HoneywellReader.SoftwareTriggerAsync(false);
            await this.HoneywellReader.EnableAsync(false);
        }

        /// <summary>
        /// Terminates the scanner.
        /// </summary>
        public override void Terminate()
        {
            this.HoneywellReader = null;
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

        /// <summary>
        /// Handles the BarcodeDataReady event of the HoneywellReader.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        /// <exception cref="Exception">Thrown when an error occurs while processing the barcode data.</exception>
        private async void HoneywellReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            try
            {
                await this.RaiseBarCodeScan(new BarCodeScannerEventArgs(e.Data));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
                ////_ = Log.Error("HoneywellBarcodeScanner", errMsg);
                throw new Exception(errMsg);
            }
        }

        /// <summary>
        /// Sets the symbology settings for the barcode reader.
        /// </summary>
        /// <exception cref="Exception">Thrown when an error occurs while setting the symbology settings.</exception>
        private void SetSymbologySettings()
        {
            if (!this.IsSymbologySettingsSet)
            {
                if (this.HoneywellReader.IsReaderOpened)
                {
                    this.HoneywellScannerService.ProfileDataReady += this.ProfileDataHandler = SetSettings;

                    this.HoneywellScannerService.GetScannerProfile();
                }
            }

            return;

            async void SetSettings(object s, EventArgs e)
            {
                try
                {
                    this.HoneywellScannerService.ProfileDataReady -= this.ProfileDataHandler;
                    var profileSettings = this.HoneywellScannerService.BarcodeReaderSettings;
                    profileSettings[this.HoneywellReader.SettingKeys.TriggerScanMode] =
                        this.HoneywellReader.SettingValues.TriggerScanMode_OneShot;
                    var result = await this.HoneywellReader.SetAsync(profileSettings);
                    if (result.Code != BarcodeReaderBase.Result.Codes.SUCCESS)
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
                    throw new Exception(errMsg);
                }
            }
        }
    }
}