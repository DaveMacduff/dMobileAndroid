namespace dMobile.BarCodeScanner
{
    using Android.Util;
    using dMobile.Resources.Services;
    using Honeywell.AIDC.CrossPlatform;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="HoneywellBarCodeScanner" />
    /// </summary>
    public partial class HoneywellBarCodeScanner : BarCodeScanner
    {
        /// <summary>
        /// Defines the honeywellReader
        /// </summary>
        private BarcodeReader honeywellReader = null;

        /// <summary>
        /// Defines the mUIContext
        /// </summary>
        private SynchronizationContext mUIContext = SynchronizationContext.Current;
        private IHoneywellScannerService honeywellScannerService = DependencyService.Get<IHoneywellScannerService>();
        private EventHandler profileDataHandler = null;
        private bool symbologySettingsSet = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoneywellBarCodeScanner"´/> class.
        /// </summary>
        public HoneywellBarCodeScanner()
        {
        }

        /// <summary>
        /// Gets or sets the HoneywellReader
        /// </summary>
        private BarcodeReader HoneywellReader
        {
            get
            {
                return this.honeywellReader;
            }

            set
            {
                this.honeywellReader = value;
            }
        }

        /// <summary>
        /// The Initialize
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public override bool Initialize()
        {
            if (this.HoneywellReader == null)
            {
                this.HoneywellReader = new BarcodeReader();
            }

            return true;
        }

        /// <summary>
        /// The Start
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task Start()
        {
            if (!this.HoneywellReader.IsReaderOpened)
            {
                BarcodeReader.Result result = await this.HoneywellReader.OpenAsync();

                if (result.Code == BarcodeReader.Result.Codes.SUCCESS || result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
                {
                    this.SetSymbologySettings();
                    this.HoneywellReader.BarcodeDataReady += HoneywellReader_BarcodeDataReady;
                    await this.HoneywellReader.EnableAsync(true);
                }
                else
                {
                    Console.WriteLine("Error", "OpenAsync failed, Code:" + result.Code + " Message:" + result.Message, "OK");
                }
            }
        }

        /// <summary>
        /// The Stop
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task Stop()
        {
            if (this.HoneywellReader.IsReaderOpened)
            {
                BarcodeReader.Result result = await this.HoneywellReader.CloseAsync();

                if (result.Code == BarcodeReader.Result.Codes.SUCCESS)
                {
                    this.HoneywellReader.BarcodeDataReady -= HoneywellReader_BarcodeDataReady;
                    await this.HoneywellReader.EnableAsync(false);
                }
                else
                {
                    Console.WriteLine("Error", "CloseAsync failed, Code:" + result.Code + " Message:" + result.Message, "OK");
                }
            }
        }

        /// <summary>
        /// Terminates the scanner.
        /// </summary>
        public override void Terminate()
        {
            if (this.HoneywellReader != null)
            {
                this.HoneywellReader = null;
            }
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        /// <param name="disposing">The disposing<see cref="bool"/></param>
        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to clean up managed resources
                this.Terminate();
            }
        }

        /// <summary>
        /// The HoneywellReader_BarcodeDataReady
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="BarcodeDataArgs"/></param>
        private void HoneywellReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            try
            {
                if (this.mUIContext == null)
                {
                    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                    this.mUIContext = SynchronizationContext.Current;
                }

                this.mUIContext.Post(_ => { this.OnBarCodeScan(new BarCodeScannerEventArgs(e.Data)); }, null);
            }
            catch (Exception ex)
            {
                Log.Error("HoneywellBarcodeScanner", ex.Message);
            }
        }

        /// <summary>
        /// The SetSymbologySettings
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private void SetSymbologySettings()
        {
            if (!this.symbologySettingsSet)
            {
                if (HoneywellReader.IsReaderOpened)
                {
                    this.honeywellScannerService.ProfileDataReady += this.profileDataHandler = async (s, e) =>
                    {
                        try
                        {
                            this.honeywellScannerService.ProfileDataReady -= this.profileDataHandler;
                            Dictionary<string, object> profileSettings = this.honeywellScannerService.BarcodeReaderSettings;

                            BarcodeReader.Result result = await this.HoneywellReader.SetAsync(profileSettings);

                            if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                            {
                                await Application.Current.MainPage.DisplayAlert("Error", "Could not set symbology settings " + result.Message, "OK");
                            }
                            else
                            {
                                this.symbologySettingsSet = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("SetSymbologySettings", "Error setting symbology settings " + ex.Message);
                        }
                    };

                    this.honeywellScannerService.GetScannerProfile();
                }
            }
        }
    }
}
