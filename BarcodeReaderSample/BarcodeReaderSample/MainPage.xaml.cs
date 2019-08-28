using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using Honeywell.AIDC.CrossPlatform;

namespace BarcodeReaderSample
{
    public partial class MainPage : ContentPage
    {
        private const string DEFAULT_READER_KEY = "default";
        private Dictionary<string, BarcodeReader> mBarcodeReaders;
        private bool mContinuousScan = false, mOpenReader = false;
        private BarcodeReader mSelectedReader = null;
        private SynchronizationContext mUIContext = SynchronizationContext.Current;
        private int mTotalContinuousScanCount = 0;
        private bool mSoftContinuousScanStarted = false;
        private bool mSoftOneShotScanStarted = false;

        public MainPage()
        {
            InitializeComponent();
            PopulateReaderPicker();
            mBarcodeReaders = new Dictionary<string, BarcodeReader>();
        }

        #region Manage UI Controls

        private async void PopulateReaderPicker()
        {
            try
            {
                // Queries the list of readers that are connected to the mobile computer.
                IList<BarcodeReaderInfo> readerList = await BarcodeReader.GetConnectedBarcodeReaders();
                if (readerList.Count > 0)
                {
                    foreach (BarcodeReaderInfo reader in readerList)
                    {
                        mReaderPicker.Items.Add(reader.ScannerName);
                    }
                }
                else
                {
                    mReaderPicker.Items.Add(DEFAULT_READER_KEY);
                }
            }
            catch (Exception ex)
            {
                mReaderPicker.Items.Add(DEFAULT_READER_KEY);
                await DisplayAlert("Error", "Failed to query connected readers, " + ex.Message, "OK");
            }

            mReaderPicker.SelectedIndex = 0;
        }

        private void UpdateBarcodeInfo(string data, string symbology, DateTime timestamp)
        {
            mScanDataEditor.Text = data;
            mSymbologyLabel.Text = "Symbology: " + symbology;
            mTimestampLabel.Text = "Timestamp: " + timestamp.ToString();
        }

        private string GetSelectedReaderName()
        {
            int selIndex = mReaderPicker.SelectedIndex;
            if (selIndex >= 0)
            {
                return mReaderPicker.Items[selIndex];
            }
            else
            {
                return null;
            }
        }

        #endregion // Manage UI Controls

        /// <summary>
        /// Get an instance of BarcodeReader from the mBarcodeReaders collection
        /// if a matching key (reader name) is found; otherwise, create a new
        /// instance of BarcodeReader and add it to the mBarcodeReaders collection.
        /// </summary>
        /// <param name="readerName">A string containing the barcode reader name.</param>
        /// <returns>An instance of BarcodeReader object.</returns>
        public BarcodeReader GetBarcodeReader(string readerName)
        {
            BarcodeReader reader = null;

            if (readerName == DEFAULT_READER_KEY)
            { // This name was added to the Open Reader picker list if the
              // query for connected barcode readers failed. It is not a
              // valid reader name. Set the readerName to null to default
              // to internal scanner.
                readerName = null;
            }

            if (null == readerName)
            {
                if (mBarcodeReaders.ContainsKey(DEFAULT_READER_KEY))
                {
                    reader = mBarcodeReaders[DEFAULT_READER_KEY];
                }
            }
            else
            {
                if (mBarcodeReaders.ContainsKey(readerName))
                {
                    reader = mBarcodeReaders[readerName];
                }
            }

            if (null == reader)
            {
                // Create a new instance of BarcodeReader object.
                reader = new BarcodeReader(readerName);

                // Add an event handler to receive barcode data.
                // Even though we may have multiple reader sessions, we only
                // have one event handler. In this app, no matter which reader
                // the data come frome it will update the same UI controls.
                reader.BarcodeDataReady += MBarcodeReader_BarcodeDataReady;

                // Add the BarcodeReader object to mBarcodeReaders collection.
                if (null == readerName)
                {
                    mBarcodeReaders.Add(DEFAULT_READER_KEY, reader);
                }
                else
                {
                    mBarcodeReaders.Add(readerName, reader);
                }
            }

            return reader;
        }

        // Event handler for the BarcodeDataReady event.
        private async void MBarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            // Update the barcode information in the UI thread.
            mUIContext.Post(_ => {
                UpdateBarcodeInfo(e.Data, e.SymbologyName, e.TimeStamp);
            }
                , null);

            if (mContinuousScan)
            {
                mTotalContinuousScanCount++;
 
                // Measure and display the performance.
                mUIContext.Post(_ => {
                    mCountValueLabel.Text = mTotalContinuousScanCount.ToString();
                }
                , null);
            } //endif (mContinuousScan)
            else if (mSoftOneShotScanStarted)
            {
                // Turn off the software trigger.
                await mSelectedReader.SoftwareTriggerAsync(false);
                mSoftOneShotScanStarted = false;
            }
        }

        /// <summary>
        /// Opens the barcode reader. This method should be called from the
        /// main UI thread because it also updates the button states.
        /// </summary>
        public async void OpenBarcodeReader()
        {
            if (mOpenReader) // Open Reader switch is in the On state.
            {
                mSelectedReader = GetBarcodeReader(GetSelectedReaderName());
                if (!mSelectedReader.IsReaderOpened)
                {
                    BarcodeReader.Result result = await mSelectedReader.OpenAsync();

                    if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                        result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
                    {
                        SetScannerAndSymbologySettings();

                        // Prevent user from selecting another reader.
                        mReaderPicker.IsEnabled = false;

                        // Turn on the Enable Scanning switch.
                        mEnableScanningSwitch.IsToggled = true;
                        // Enable the Enable Scanning switch.
                        mEnableScanningLabel.IsEnabled = true;
                        mEnableScanningSwitch.IsEnabled = true;

                        // Clear barcode data information
                        mScanDataEditor.Text = "";
                        mSymbologyLabel.Text = "";
                        mTimestampLabel.Text = "";
                    }
                    else
                    {
                        await DisplayAlert("Error", "OpenAsync failed, Code:" + result.Code +
                            " Message:" + result.Message, "OK");
                    }
                }
            } //endif (mOpenReader)
        }

        /// <summary>
        /// Closes the barcode reader. This method should be called from the
        /// main UI thread because it also updates the button states.
        /// </summary>
        public async void CloseBarcodeReader()
        {
            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                if (mSoftOneShotScanStarted || mSoftContinuousScanStarted)
                {
                    // Turn off the software trigger.
                    await mSelectedReader.SoftwareTriggerAsync(false);
                    mSoftOneShotScanStarted = false;
                    mSoftContinuousScanStarted = false;
                }

                BarcodeReader.Result result = await mSelectedReader.CloseAsync();
                if (result.Code == BarcodeReader.Result.Codes.SUCCESS)
                {
                    mScanButton.IsEnabled = false;
                    // Allow user to select another reader.
                    mReaderPicker.IsEnabled = true;

                    // Disable the Enable Scanning switch.
                    mEnableScanningLabel.IsEnabled = false;
                    mEnableScanningSwitch.IsEnabled = false;
                    // Turn off the Enable Scanning switch.
                    mEnableScanningSwitch.IsToggled = false;

                    // Turn off the Continuous switch.
                    mContinuousSwitch.IsToggled = false;
                    // Disable the Continuous switch
                    mContinuousLabel.IsEnabled = false;
                    mContinuousSwitch.IsEnabled = false;
                }
                else
                {
                    await DisplayAlert("Error", "CloseAsync failed, Code:" + result.Code +
                        " Message:" + result.Message, "OK");
                }
            }
        }

        private async void SetScannerAndSymbologySettings()
        {
            try
            {
                if (mSelectedReader.IsReaderOpened)
                {
                    Dictionary<string, object> settings = new Dictionary<string, object>()
                    {
                        {mSelectedReader.SettingKeys.TriggerScanMode, mSelectedReader.SettingValues.TriggerScanMode_OneShot },
                        {mSelectedReader.SettingKeys.Code128Enabled, true },
                        {mSelectedReader.SettingKeys.Code39Enabled, true },
                        {mSelectedReader.SettingKeys.Ean8Enabled, true },
                        {mSelectedReader.SettingKeys.Ean8CheckDigitTransmitEnabled, true },
                        {mSelectedReader.SettingKeys.Ean13Enabled, true },
                        {mSelectedReader.SettingKeys.Ean13CheckDigitTransmitEnabled, true },
                        {mSelectedReader.SettingKeys.Interleaved25Enabled, true },
                        {mSelectedReader.SettingKeys.Interleaved25MaximumLength, 100 },
                        {mSelectedReader.SettingKeys.Postal2DMode, mSelectedReader.SettingValues.Postal2DMode_Usps }
                    };

                    BarcodeReader.Result result = await mSelectedReader.SetAsync(settings);
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        await DisplayAlert("Error", "Symbology settings failed, Code:" + result.Code +
                                            " Message:" + result.Message, "OK");
                    }
                }
            }
            catch (Exception exp)
            {
                await DisplayAlert("Error", "Symbology settings failed. Message: " + exp.Message, "OK");
            }
        }

        #region UI Event Handlers
        private void OpenReaderSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            mOpenReader = e.Value;
            if (mOpenReader)
            {
                OpenBarcodeReader();
            }
            else
            {
                CloseBarcodeReader();
            }
        }

        private async void EnableScanningSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            mScanButton.IsEnabled = e.Value;
            mContinuousLabel.IsEnabled = e.Value;
            mContinuousSwitch.IsEnabled = e.Value;

            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                BarcodeReader.Result result = await mSelectedReader.EnableAsync(e.Value); // Enables or disables barcode reader
                if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                {
                    await DisplayAlert("Error", "EnableAsync failed, Code:" + result.Code +
                                        " Message:" + result.Message, "OK");
                }
            }
        }

        private async void ContinuousSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                BarcodeReader.Result result;

                mContinuousScan = e.Value;
                mCountValueLabel.IsVisible = mContinuousScan;
                if (mContinuousScan)
                {
                    mTotalContinuousScanCount = 0;
                    // Clear the scan count display.
                    mCountValueLabel.Text = "";

                    // Set the trigger scan mode to continuous. In the continuous scan mode, it
                    // continuously decodes unique barcodes when the scan trigger is pressed until
                    // the trigger is released. Please see the SDK User Guide for more information
                    // if you would like to scan the same barcode multiple times.
                    // Note: These settings apply to scanning using the hardware scan button and
                    // the SCAN button on the UI. If you use the UI SCAN button to trigger
                    // the scanner to start continuous scan, this app will release the scan trigger
                    // when the Continuous switch is in the off position.
                    Dictionary<string, object> settings = new Dictionary<string, object>()
                    {
                        {mSelectedReader.SettingKeys.TriggerScanMode, mSelectedReader.SettingValues.TriggerScanMode_Continuous }
                    };

                    result = await mSelectedReader.SetAsync(settings);
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        await DisplayAlert("Error", "Failed to set continuous scan mode, Code:" + result.Code +
                                            " Message:" + result.Message, "OK");
                    }
                }
                else
                {
                    if (mSoftContinuousScanStarted)
                    {
                        mSoftContinuousScanStarted = false;
                        // Turn off the software trigger.
                        result = await mSelectedReader.SoftwareTriggerAsync(false);
                        if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                        {
                            await DisplayAlert("Error", "Failed to turn off software trigger, Code:" + result.Code +
                                " Message:" + result.Message, "OK");
                        }
                    }
                    // Set trigger scan mode to one shot.
                    Dictionary<string, object> settings = new Dictionary<string, object>()
                    {
                        {mSelectedReader.SettingKeys.TriggerScanMode, mSelectedReader.SettingValues.TriggerScanMode_OneShot }
                    };

                    result = await mSelectedReader.SetAsync(settings);
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        await DisplayAlert("Error", "Failed to set one shot scan mode, Code:" + result.Code +
                                            " Message:" + result.Message, "OK");
                    }
                }
            }
        }

        public async void OnScanButtonClicked(object sender, EventArgs args)
        {
            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                if (mContinuousScan)
                {
                    // The Continuous switch on the UI was turned on and the trigger
                    // scan mode was set to continuous scan. Pressing the SCAN button on
                    // the UI will begin the continuous scan.
                    mSoftContinuousScanStarted = true;
                }
                BarcodeReader.Result result = await mSelectedReader.SoftwareTriggerAsync(true);
                if (result.Code == BarcodeReader.Result.Codes.SUCCESS)
                {
                    // Set mSoftOneShotScanStarted to true if not in continuous scan mode.
                    // The mSoftOneShotScanStarted flag is used to turn off the software
                    // trigger after a barcode is read successfully.
                    mSoftOneShotScanStarted = !mSoftContinuousScanStarted;
                }
                else
                {
                    await DisplayAlert("Error", "Failed to turn on software trigger, Code:" + result.Code +
                        " Message:" + result.Message, "OK");
                }
            } //endif (mReaderOpened)
        }

        #endregion // UI Event Handlers
    }
}
