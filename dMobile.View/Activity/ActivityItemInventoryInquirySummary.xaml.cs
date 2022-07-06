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
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ActivityItemInventoryInquirySummary" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemInventoryInquirySummary : DMobileModal, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemInventoryInquirySummary"/> class.
        /// </summary>
        /// <param name="actItem">.</param>
        public ActivityItemInventoryInquirySummary(ActItem actItem)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquirySummary;
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquirySummary);
                this.ActItem.ProdInfo = actItem.ProdInfo;
                this.ActItem.LocMap = actItem.LocMap;
                this.ActItem.SysEnv = this.SysEnv;
            }
        }

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBarCodeHandlerRegistered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the Activity.
        /// </summary>
        private Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the BarParse.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the main activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool FirstOnAppearing { get; set; } = true;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        // public UserControl GetCallingControl()
        // {
        //    return this.CallingControl;
        // }

        /// <summary>
        ///.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Inventory Inquiry Summary";
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            return this.IsValidData();
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

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
        /// The SaveChanges.
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

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

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task RefreshData()
        {
            try
            {
                this.Loading = true;

                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

                this.ActItem.RegLayerListenerProgOut(this);
                XDocument xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "</params>");

                await this.ActItem.Refresh(xmldocParams);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems = this.ActItem.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                    this.ExtraInfoItems.Clear();
                });

                if (this.MainActivityItems.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0]);
                    // TODO: test
                    await this.ActItem.SelectActivityItem(this.ActItem.ListItems[0]);
                }

                await Device.InvokeOnMainThreadAsync(() => this.DisplaySummaryData());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ActItem.UnregLayerListenerProgOut(this);
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
            await this.ScannerDisable();

            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            SubscribeKeyUpHandlers();

            this.Loading = true;

            try
            {
                if (this.FirstOnAppearing)
                {
                    await this.ActItemInventoryInquiry_GotFocus(this, new EventArgs());
                    await this.RefreshData();
                    this.FirstOnAppearing = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActItemInventoryInquiry_GotFocus error: " + ex.Message);
            }

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    if (this.Loading)
                    {
                        return;
                    }

                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter)
                    {
                        ProductInfo svdProdInfo = this.ActItem.ProdInfo;

                        try
                        {
                            this.IsEnabled = false;
                            await this.ScannerDisable();
                            int currentIndex = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
                            if (currentIndex < 0)
                            {
                                return;
                            }

                            // TODO: Test
                            this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromCodeBCUnit(
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as Resources.Types.ActivityItem).ProductCode),
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as Resources.Types.ActivityItem).ProductUnitBC),
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as Resources.Types.ActivityItem).UnitCode));
                            ActDetail actDetail = ActDetailFacade.GetActDetail(
                                this.ActItem,
                                SysCommon.ActSubTypes.InventoryInquiry);
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormDialogContainer form = new FormDialogContainer(
                                    EDialogTypes.ActivityInventoryInquiryDetail,
                                    actDetail,
                                    this.ActSubType);
                                _ = await form.DisplayControl.ShowDialog();
                            });
                        }
                        catch (Exception ex)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                                _ = await errRpt.ShowDialog();
                            });
                        }
                        finally
                        {
                            this.ActItem.ProdInfo = svdProdInfo;
                            if (this.IsSvdBarScannerEnabled)
                            {
                                await this.ScannerEnable();
                            }

                            this.IsEnabled = true;
                        }
                    }
                    else if (keycode == Keycode.F1)
                    {
                        await OnAppearingKeyUpF1();
                    }
                });

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
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
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
                    this.DialogResult = DialogResult.None;
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Adds data to the bottom list box to show
        /// Product ID, Qty and Weight.
        /// </summary>
        private void DisplaySummaryData()
        {
            Resources.Types.ActivityItem selectedItem = this.ActItem.SelectedActivityItem as Resources.Types.ActivityItem;
            ProductInfo prodInfo = this.ActItem.ProdInfo;

            this.ExtraInfoItems.Clear();

            if (selectedItem != null)
            {
                this.ExtraInfoItems.Add(string.Format(
                    "Quantity {0} {1} @ {2} {3}",
                    selectedItem.ActivityItemQuantity,
                    selectedItem.UnitCode,
                    selectedItem.ActivityItemWeightQuantity,
                    selectedItem.WeightUnitCode));

                if (this.ActItem.LocMap.CurrLocMap.IsSectionTracked)
                {
                    if (this.ActItem.LocMap.CurrLocMap.IsSlotTracked)
                    {
                        this.ExtraInfoItems.Add(string.Format("Section {0} Slot {1}", selectedItem.LocationMapSectionFKRefno, selectedItem.LocationMapSlotFKRefno));
                    }
                    else
                    {
                        this.ExtraInfoItems.Add(string.Format("Section {0}", selectedItem.LocationMapSectionFKRefno));
                    }
                }
            }
            else
            {
                this.ExtraInfoItems.Add("No product at selected location.");
            }

            this.ExtraInfoItems.Add(string.Format("Product ID {0}", prodInfo.Code));
            this.ExtraInfoItems.Add(string.Format("Whs {0} {1}", this.ActItem.LocMap.CurrLocMap.WhsFKRefno, this.ActItem.LocMap.CurrLocMap.WhsName));
        }

        /// <summary>
        /// The ActItemInventoryInquiry_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ActItemInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
        }

        /// <summary>
        /// The IsValidData.
        /// </summary>
        /// <returns>.</returns>
        private bool IsValidData()
        {
            return true;
        }

        /// <summary>
        /// The BarCodeHandler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs">The scanArgs<see cref="BarCodeScannerEventArgs"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            this.Loading = true;
            this.BarParse = scanArgs.ParseInfo;
            await this.ScannerDisable();

            try
            {
                if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                {
                    throw new Exception("Scanning an activity reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                {
                    throw new Exception("Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new Exception("Scanning an product identifier is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
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
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnLayerStatOut.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The OnLayerProgOut.
        /// </summary>
        /// <param name="publisher">.</param>
        /// <param name="dataLayerEventArgs">.</param>
        public void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// The ListViewMainActivity_ItemSelected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActItem.SelectedActivityItem = e.SelectedItem;
            this.DisplaySummaryData();
        }
    }
}
