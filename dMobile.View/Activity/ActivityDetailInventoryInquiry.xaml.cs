// <copyright file="ActivityDetailInventoryInquiry.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using CustomPages.Modal;
    using Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityDetailInventoryInquiry" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetailInventoryInquiry : DMobileModal, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityDetailInventoryInquiry" /> class.
        /// </summary>
        /// <param name="actDetail">.</param>
        /// <param name="actSubType">.</param>
        public ActivityDetailInventoryInquiry(ActDetail actDetail, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = actSubType;
            this.ActDetail = actDetail;
            this.Title = this.GetControlDescription();
        }

        /// <summary>
        ///     Gets or sets the SysEnv.
        /// </summary>
        public SysEnvironment SysEnv { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsBarCodeHandlerRegistered.
        /// </summary>
        public bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the ActSubType.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        ///     Gets or sets the BarParse.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        ///     Gets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets the ActDetail.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets or sets the ExtraInfoItems.
        /// </summary>
        public ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets the MainActivityItems.
        /// </summary>
        public ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.ClearBarCodeScanEventHandlers();
                await this.Scanner.DisableScanning();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.EnableScanning();
                if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                {
                    this.Scanner.BarCodeScan += this.BarCodeHandler;
                }

                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///     The GetXml.
        /// </summary>
        /// <returns>.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///     The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(this.IsValidData());
        }

        /// <summary>
        ///     The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            const bool isReadyToClose = true;
            if (isReadyToClose)
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initialize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
                {
                    await this.ScannerDisable();
                }
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            try
            {
                this.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsEnabled = true;
            }

            return true;
        }

        /// <summary>
        ///     The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.

                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    // ClearProperties();
                    // DisplayData();
                    // this.LoadData();
                    cancelChanges = false;

                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    await this.ucActivityDetailInventoryInquiry_GotFocus(this, EventArgs.Empty);
                    await this.tmrActivityItemInventoryInquiry_Tick(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                        "ucActivityDetailInventoryInquiry_GotFocus/tmrActivityItemInventoryInquiry_Tick error: " +
                        ex.Message, "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Inventory Inquiry Detail";
        }

        /// <summary>
        ///     ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///     .
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task RefreshData()
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    await this.ScannerDisable();
                    await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

                    await this.ActDetail.Refresh();

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.MainActivityItems =
                            this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                        if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
                        {
                            this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                            await this.ActDetail.AssignActivityDetail(this.ActDetail.ListDetails[0]);
                        }
                    });
                }
                catch (NotImplementedException)
                {
                    // Handle this.ActDetail.SelectActivityDetail not implemented

                    Console.WriteLine(this.GetControlDescription() + " Not implemented exception");
                }
                catch (CommunicationException commEx)
                {
                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity details.{Environment.NewLine}" +
                                  $"Because of a communication problem.{Environment.NewLine}" +
                                  $"Detail:{commEx.Message}{Environment.NewLine}" +
                                  "Please exit and try again.";

                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK);
                    });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load activity details.{Environment.NewLine}" +
                                  $"General exception.{Environment.NewLine}" +
                                  $"Detail:{ex.Message}{Environment.NewLine}" +
                                  "Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                    this.IsIndicatorRunning = false;
                }
            });
        }

        /// <summary>
        ///     ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        ///     .
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task tmrActivityItemInventoryInquiry_Tick(object sender, EventArgs e)
        {
            await this.RefreshData();
        }

        /// <summary>
        ///     The BarCodeHandler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;
                this.BarParse = scanArgs.ParseInfo;

                // <issue="13609" author="RD" date="2016/02/22"/>
                if (this.ActDetail != null && this.ActDetail.ActItem != null)
                {
                    SysShared.AssertSuppressedErrorsAgainstScheme(this.ActDetail.ActItem.ProdInfo, this.BarParse);
                }

                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new Exception("Scanning an activity reference # is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new Exception("Scanning an activity item reference # is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new Exception("Scanning an product identifier is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new Exception("Scanning a shipping container bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new Exception("Scanning a global location identifier is not supported here.");
                }

                throw new Exception("Scanned bar code type is not supported here.");
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     The IsValidData.
        /// </summary>
        /// <returns>.</returns>
        private bool IsValidData()
        {
            return true;
        }

        /// <summary>
        ///     The ucActivityDetailInventoryInquiry_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task ucActivityDetailInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
        }

        /// <summary>
        ///     The ListViewMainActivity_ItemSelected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs" />.</param>
        private void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }
    }
}