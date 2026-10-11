// <copyright file="ActivityItemInventoryInquiry.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Resources.Enums;
    using CustomPages.Modal;
    using Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// This is the activity item inventory inquiry class.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemInventoryInquiry : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemInventoryInquiry"/> class.
        /// </summary>
        /// <param name="actItem">Activity item instance.</param>
        /// <param name="actSubTypes">Activity sub type.</param>
        public ActivityItemInventoryInquiry(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;

            this.ActSubType = actSubTypes;
            this.ActItem = actItem;

            this.Title = this.GetControlDescription();
        }

        private bool IsEnterTrapped { get; set; }

        private SysEnvironment SysEnv { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; }

        private ActItem ActItem { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private BarCodeParse BarParse { get; set; }

        private BarCodeScanner Scanner { get; set; }

        private int TotalTicks { get; set; } = 0;

        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning && keycode == Keycode.Enter)
                        {
                            var svdProdInfo = this.ActItem.ProdInfo;

                            try
                            {
                                this.IsEnterTrapped = true;
                                this.IsEnabled = false;

                                await this.ScannerDisable();
                                var index = 0;

                                if (this.ListViewMainActivity.SelectedItem != null &&
                                    this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) >= 0)
                                {
                                    index = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);

                                    // TODO: test
                                    this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromCodeBcUnit(
                                        Convert.ToString((this.ActItem.ListItems[index] as dynamic).productcode),
                                        Convert.ToString((this.ActItem.ListItems[index] as dynamic).productunitbc),
                                        Convert.ToString((this.ActItem.ListItems[index] as dynamic).unitcode));

                                    var actDetail = ActDetailFacade.GetActDetail(this.ActItem,
                                        SysCommon.ActSubTypes.InventoryInquiry);

                                    var form = new FormDialogContainer(EDialogTypes.ActivityInventoryInquiryDetail,
                                        actDetail, this.ActSubType);
                                    _ = await form.DisplayControl.ShowDialog();
                                }
                            }
                            catch (Exception ex)
                            {
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                                    _ = await errRpt.ShowDialog();
                                });
                            }
                            finally
                            {
                                this.ActItem.ProdInfo = svdProdInfo;
                                await this.ScannerEnable();

                                this.IsEnabled = true;
                            }
                        }
                    });
                    await this.ActItemInventoryInquiry_GotFocus(this, EventArgs.Empty);
                    await this.RefreshData();
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error", "this.ActItemInventoryInquiry_GotFocus threw an error " + ex,
                            "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = true;
                }
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            SysCommon.ShutdownConnection();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    _ = await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                await this.Scanner.EnableScanning();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Inventory Inquiry Summary";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(this.IsValidData());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            if (isReadyToClose)
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    // this.ProdUnitRefno = 0;

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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task RefreshData()
        {
            await this.ScannerDisable();
            await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

            try
            {
                var xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "</params>");

                await this.ActItem.Refresh(xmldocParams);
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems = this.ActItem.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            if (this.ActItem.ListItems.Count > 0)
            {
                await this.ActItem.AssignActivityItem(this.ActItem.ListItems[0]);
            }

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task ActItemInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        "OK"));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;
                this.BarParse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new Exception("Scanning an activity reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new Exception("Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new Exception("Scanning an product identifier is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new Exception("Scanning a shipping container bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new Exception("Scanning a global location identifier is not supported here.");
                }
                else
                {
                    throw new Exception("Scanned bar code type is not supported here.");
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing.
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }
    }
}