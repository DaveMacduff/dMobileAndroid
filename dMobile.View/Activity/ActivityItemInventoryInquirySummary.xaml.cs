// <copyright file="ActivityItemInventoryInquirySummary.xaml.cs" company="InterSect Business Systems Inc">
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
    using Controller;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages.Modal;
    using dMobile.Controller.Activity;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ActivityItemInventoryInquirySummary" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemInventoryInquirySummary : DMobileModal, IAppArea,
        IDataLayerEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemInventoryInquirySummary"/> class.
        /// </summary>
        /// <param name="actItem">Activity item instance.</param>
        public ActivityItemInventoryInquirySummary(ActItem actItem)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = SysCommon.ActSubTypes.InventoryInquirySummary;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);

            if (this.Activity.SkidInfo == null)
            {
                this.Activity.SkidInfo = new SysSkidInfo();
            }

            this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquirySummary);
            this.ActItem.SysEnv = this.SysEnv;
            this.ActItem.ProdInfo = actItem.ProdInfo;
            this.ActItem.LocMap = actItem.LocMap;

            // The activity item may not have a complete location thus we need to make sure that a location is assigned.
            if (this.ActItem.LocMapRefno == 0)
            {
                this.ActItem.LocMap = LocationMap.GetInstance().CurrLocMap;
            }
        }

        private SysEnvironment SysEnv { get; set; }

        private ActItem ActItem { get; set; }

        private Activity Activity { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private BarCodeParse BarParse { get; set; }

        private BarCodeScanner Scanner { get; set; }

        private ObservableCollection<string> ExtraInfoItems { get; set; } =
            new ObservableCollection<string>();

        private ObservableCollection<object> MainActivityItems { get; set; } =
            new ObservableCollection<object>();

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner
        /// </summary>
        /// <returns><see cref="Task"/> Task operation</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner = ScannerEngine.GetInstance().Scanner;
            }

            var barCodeScanner = this.Scanner;
            if (barCodeScanner != null)
            {
                barCodeScanner.OnBarCodeScan -=
                    new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await barCodeScanner.DisableScanning();
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner
        /// </summary>
        /// <returns><see cref="Task"/> Task operation</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                this.Scanner = ScannerEngine.GetInstance().Scanner;
            }

            var barCodeScanner = this.Scanner;
            if (barCodeScanner != null)
            {
                await barCodeScanner.EnableScanning();
                barCodeScanner.OnBarCodeScan +=
                    new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        /// The GetControlDescription
        /// </summary>
        /// <returns></returns>
        public string GetControlDescription()
        {
            return "Inventory Inquiry Summary";
        }

        /// <summary>
        /// The GetXml
        /// </summary>
        /// <returns></returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// The IsReadyToSave
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(this.IsValidData());
        }

        /// <summary>
        /// The IsReadyToClose
        /// </summary>
        /// <returns></returns>
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
                await this.ScannerDisable();
            }

            return isReadyToClose;
        }

        /// <summary>
        /// The SaveChanges
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChanges()
        {
            try
            {
                this.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.IsEnabled = true;
            }

            return true;
        }

        /// <summary>
        /// The CancelChanges
        /// </summary>
        /// <returns></returns>
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
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return cancelChanges;
        }

        /// <summary>
        /// /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation</returns>
        public async Task RefreshData()
        {
            try
            {
                this.ActItem.RegLayerListenerProgOut(this);
                await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());
                var xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<params>" +
                    "</params>");

                await this.ActItem.Refresh(xmldocParams);
                await Device.InvokeOnMainThreadAsync(
                    () =>
                    {
                        this.MainActivityItems = this.ActItem.DesignGrid(
                            ref this.GridMainActivity,
                            ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                        this.ExtraInfoItems.Clear();
                    });

                if (this.MainActivityItems.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(
                        () => this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0]);

                    // TODO: test
                    await this.ActItem.AssignActivityItem(this.ActItem.ListItems[0]);
                }

                await Device.InvokeOnMainThreadAsync(() => this.DisplaySummaryData());
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ActItem.UnregLayerListenerProgOut(this);
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                    SubscribeKeyUpHandlers();
                    await this.ActItemInventoryInquiry_GotFocus(this, EventArgs.Empty);
                    await this.RefreshData();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                        ? msgEx.Message
                        : msgEx.Message["MSGBOX-".Length..];
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "OK"));
                }
                catch (CommunicationException commEx)
                {
                    // Question should a communication error, be reported as an error or just a message box?
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var msg = $"Unable to load activity items.{Environment.NewLine}" +
                                      $"Because of a communication problem.{Environment.NewLine}" +
                                      $"Detail:{commEx.Message}{Environment.NewLine}" +
                                      "Please exit and try again.";

                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "OK");
                        });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var msg = $"Unable to load activity items.{Environment.NewLine}" +
                                      $"General exception.{Environment.NewLine}" +
                                      $"Detail:{ex.Message}{Environment.NewLine}" +
                                      "Please exit and try again.";
                            var errorReport = new FormErrorReport(msg, ex);
                            _ = await errorReport.ShowDialog();
                        });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                    await this.ScannerEnable();
                }
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async (keyPressed) =>
                    {
                        await KeyUpHandlerCallback(keyPressed);
                        return;

                        //////////////////////////////////////////////////////////////////////////
                        async Task KeyUpHandlerCallback(string keyPressed)
                        {
                            if (this.IsIndicatorRunning)
                            {
                                return;
                            }

                            try
                            {
                                _ = Enum.TryParse(keyPressed, out Keycode keycode);
                                if (keycode == Keycode.Enter)
                                {
                                    this.IsEnabled = false;
                                    await AppearingEnterKey();
                                }
                                else if (keycode == Keycode.F1)
                                {
                                    await OnAppearingKeyUpF1();
                                }
                            }
                            catch (Exception ex)
                            {
                                await Device.InvokeOnMainThreadAsync(
                                    async () =>
                                    {
                                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                                        _ = await errRpt.ShowDialog();
                                    });
                            }
                            finally
                            {
                                this.IsIndicatorRunning = false;
                            }

                            return;

                            //////////////////////////////////////////////////////////////////////////
                            async Task OnAppearingKeyUpF1()
                            {
                                if (await this.SaveChanges())
                                {
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                                else
                                {
                                    this.DialogResult = DialogResult.None;
                                    _ = await this.CancelChanges();
                                    await this.Close();
                                }
                            }

                            //////////////////////////////////////////////////////////////////////////
                            async Task AppearingEnterKey()
                            {
                                var svdProdInfo = this.ActItem.ProdInfo;
                                var currentIndex = this.MainActivityItems.IndexOf(
                                    this.ListViewMainActivity.SelectedItem);
                                if (currentIndex >= 0)
                                {
                                    this.ActItem.ProdInfo =
                                        await ProductInfoTasks.GetInfoFromCodeBcUnit(
                                            Convert.ToString(
                                                (this.ActItem.ListItems[currentIndex] as
                                                    Resources.Types.Items.ActivityItem).ProductCode),
                                            Convert.ToString(
                                                (this.ActItem.ListItems[currentIndex] as
                                                    Resources.Types.Items.ActivityItem).ProductUnitBC),
                                            Convert.ToString(
                                                (this.ActItem.ListItems[currentIndex] as
                                                    Resources.Types.Items.ActivityItem).UnitCode));
                                    var actDetail = ActDetailFacade.GetActDetail(
                                        this.ActItem,
                                        SysCommon.ActSubTypes.InventoryInquiry);
                                    await Device.InvokeOnMainThreadAsync(
                                        async () =>
                                        {
                                            var form = new FormDialogContainer(
                                                EDialogTypes.ActivityInventoryInquiryDetail,
                                                actDetail,
                                                this.ActSubType);
                                            _ = await form.DisplayControl.ShowDialog();
                                        });
                                }

                                this.ActItem.ProdInfo = svdProdInfo;
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// The OnBackButtonPressed
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    _ = base.OnBackButtonPressed();

                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        await this.Close();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                        _ = await this.CancelChanges();
                        await this.Close();
                    }
                });

            return true;
        }

        /// <summary>
        /// Adds data to the bottom list box to show
        /// Product ID, Qty and Weight
        /// </summary>
        private void DisplaySummaryData()
        {
            var selectedItem = this.ActItem.SelectedItem as Resources.Types.Items.ActivityItem;
            var prodInfo = this.ActItem.ProdInfo;

            this.ExtraInfoItems.Clear();

            if (selectedItem != null)
            {
                this.ExtraInfoItems.Add(
                    $"Quantity {selectedItem.ActivityItemQuantity} {selectedItem.UnitCode} @ {selectedItem.ActivityItemWeightQuantity} {selectedItem.WeightUnitCode}");

                if (this.ActItem.LocMap.IsSectionTracked)
                {
                    if (this.ActItem.LocMap.IsSlotTracked)
                    {
                        this.ExtraInfoItems.Add(
                            $"Section {selectedItem.LocationMapSectionFKRefno} Slot {selectedItem.LocationMapSlotFKRefno}");
                    }
                    else
                    {
                        this.ExtraInfoItems.Add(
                            $"Section {selectedItem.LocationMapSectionFKRefno}");
                    }
                }
            }
            else
            {
                this.ExtraInfoItems.Add("No product at selected location.");
            }

            this.ExtraInfoItems.Add($"Product ID {prodInfo.Code}");
            this.ExtraInfoItems.Add(
                $"Whs {this.ActItem.LocMap.WhsFkRefno} {this.ActItem.LocMap.WhsName}");
        }

        /// <summary>
        /// The ActItemInventoryInquiry_GotFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>The <see cref="Task"/></returns>
        private Task ActItemInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The IsValidData
        /// </summary>
        /// <returns></returns>
        private bool IsValidData()
        {
            return true;
        }

        /// <summary>
        /// The BarCodeHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs">The scanArgs<see cref="BarCodeScannerEventArgs"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
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
                    throw new Exception(
                        "Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new Exception("Scanning an product identifier is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new Exception(
                        "Scanning a shipping container bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new Exception(
                        "Scanning a global location identifier is not supported here.");
                }
                else
                {
                    throw new Exception("Scanned bar code type is not supported here.");
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
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

        /// <summary>
        /// The OnLayerStatOut
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/></param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs"/></param>
        public void OnLayerStatOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The OnLayerProgressOut
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgressOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The ListViewMainActivity_ItemSelected
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/></param>
        private void ListViewMainActivity_ItemSelected(
            object sender,
            SelectedItemChangedEventArgs e)
        {
            this.ActItem.SelectedItem = e.SelectedItem;
            this.DisplaySummaryData();
        }
    }
}
