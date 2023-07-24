// <copyright file="ActivityHdr.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Controller.Activity;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ActivityHdr" /> Activity header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdr : DMobileContentPage, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Defines the text filter.
        /// </summary>
        private string filterText = string.Empty;

        /// <summary>
        /// Define the filter text was enabled.
        /// </summary>
        private bool filterIsEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdr"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <remarks>
        /// Page refresh does not invoke this constructor.
        /// </remarks>
        public ActivityHdr(SysCommon.ActSubTypes activitySubType)
        {
            this.InitializeComponent();

            this.cmdAdd.Text = "Add";
            this.cmdAdd.IsEnabled = false;
            this.FilterText = "Filter";
            this.FilterIsEnabled = true;
            this.SysEnv = SysEnvironment.GetInstance();
            this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = activitySubType;
            this.ActHdr = ActivityFacade.GetActivity(this.ActSubType);
            this.Title = this.ActHdr.GetControlDescription();

            // If the activity header object does not have a skid already initialize the instance.
            if (this.ActHdr.SkidInfo == null)
            {
                this.ActHdr.SkidInfo = new SysSkidInfo();
            }
        }

        ////protected override async void OnElementChanged(VisualElementChangedEventArgs e)
        ////{
        ////    base.OnElementChanged(e);

        ////    this.ScrollbarFadingEnabled = false;
        ////}

        /// <summary>
        /// Gets or sets the selected activity foreign key reference number.
        /// </summary>
        private int SelectedActivityFkRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether any data changes have be made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code parsed data.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets the global system environment object.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bar code handler is registered / enabled.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        /// Gets or sets the activity header instance.
        /// </summary>
        private Controller.Activity.Activity ActHdr { get; set; }

        /// <summary>
        /// Gets or sets the activity header sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.InventoryReplenishment;

        /// <summary>
        /// Gets or sets the list of items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the OptionsMenuItems.
        /// </summary>
        private List<string> OptionsMenuItems { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the filter text.
        /// </summary>
        private string FilterText
        {
            get => this.filterText;
            set
            {
                this.filterText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether entering a filter was performed.
        /// </summary>
        private bool FilterIsEnabled
        {
            get => this.filterIsEnabled;
            set
            {
                this.filterIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a number of screen taps tracker for Xamarin bug that prevents
        /// selectedItemChanged from firing.
        /// </summary>
        private int NumTaps { get; set; } = 0;

        /// <summary>
        /// Gets or sets the previous sender used in tracking the number of screen taps for
        /// Xamarin bug that prevents selectedItemChanged from firing.
        /// </summary>
        private object PreviousSender { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the header listview is in focus.
        /// </summary>
        private bool IsHdrListViewInFocus { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the list display data is in focus.
        /// </summary>
        private bool IsDisplayListViewInFocus { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the search text box is in focus.
        /// </summary>
        private bool IsSearchEntryFocused { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the add button is in focus.
        /// </summary>
        private bool IsAddButtonFocused { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the menu options is in focus.
        /// </summary>
        private bool IsMenuOptionsFocused { get; set; } = false;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    await this.Scanner.Stop();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool"/> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;
            if (isReadyToSave && (this.ActSubType == SysCommon.ActSubTypes.Picking
                || this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
            {
                isReadyToSave = await IsReadyToSavePicking(isReadyToSave);
            }

            return isReadyToSave;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToSavePicking(bool isReadyToSave)
            {
                // If activity is picking and confirm to exit form is enabled.
                // This code should be moved into an picking header class inherited from ucActivityHdr.
                if (this.SysEnv.SysSettings.IsPickConfirmExit)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        isReadyToSave = await this.AskConfirmExit();
                    });
                }

                return isReadyToSave;
            }
        }

        /// <summary>
        /// Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}"/> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            // If activity is picking and confirm to exit form is enabled.
            // This code should be moved into an picking header class inherited from ucActivityHdr.
            if (isReadyToClose
                && this.SysEnv.SysSettings.IsPickConfirmExit
                && (this.ActSubType == SysCommon.ActSubTypes.Picking
                    || this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
            {
                isReadyToClose = await this.AskConfirmExit();
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool"/> True if a save was performed successfully else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new Exception("Data failed validation. Check your data.");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool"/>. True if the changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
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
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
        /// Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation containing an XDocument.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// Data layer status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// publisher delegate.</param>
        /// <param name="dataLayerEventArgs"><see cref="DataGridEnableEventArgs"/> Data layer
        /// publisher event arguments.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            // NOTE: Do not set the scanner off or on here. Scan events can occur even though
            // this method could be invoked asynchronously in the future.
            ////this.ListViewDisplayExtraInfo.Items.Clear();
            this.ExtraInfoItems.Clear();
            if (!string.IsNullOrEmpty(dataLayerEventArgs.StatusOutMsg))
            {
                this.ExtraInfoItems.Add(dataLayerEventArgs.StatusOutMsg);
            }
        }

        /// <summary>
        /// Data layer progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// publisher delegate.</param>
        /// <param name="dataLayerEventArgs"><see cref="DataGridEnableEventArgs"/> Data layer
        /// publisher event arguments.</param>
        public void OnLayerProgressOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            if (dataLayerEventArgs.CurrentValue > 0 && dataLayerEventArgs.MaxValue > 0)
            {
                ////double percent = 100 * dataLayerEventArgs.CurrentValue / dataLayerEventArgs.MaxValue;
                ////this.ProgBarMain.Progress = percent / 100;
            }
        }

        /// <summary>
        /// Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string"/> Option property name..</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(this.FilterIsEnabled) || propertyName == nameof(this.FilterText))
            {
                this.OptionsMenuItems.Clear();

                if (this.FilterIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.FilterText);
                }
            }
        }

        /// <summary>
        /// Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            var performRefreshData = true;
            base.OnAppearing();
            try
            {
                this.Loading = true;
                this.NumTaps = 0;
                this.SubscribeKeyUpHandlers();
                this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
                await this.ScannerEnable();
                this.ExtraInfoItems.Clear();

                this.ActHdr.XmlDataCriteria = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<whs_id>0</whs_id>" +
                            "<sect_refno>0</sect_refno>" +
                            "</params>");

                // Note: This filter request will be asked for upon a page refresh.
                // IsShowActivityFilterSelection flag stored in singleton so it is not reset on a page refresh.
                if ((this.ActSubType == SysCommon.ActSubTypes.Picking
                    || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                     && this.SysEnv.SysSettings.IsPrePickCrit && this.SysEnv.SysSettings.IsShowActivityFilterSelection)
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = false;

                    // Override performRefreshData, WO#12906 do not load any orders if no section was selected.
                    performRefreshData = await HandlePrePickCriteria();
                }

                if (performRefreshData)
                {
                    await this.RefreshDataBySubType();
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                }

                // If not headers where loaded clear the extra info list.
                if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                {
                    Device.BeginInvokeOnMainThread(() => this.ExtraInfoItems.Clear());
                }
                else
                {
                    await this.FindSelectedItemAsync();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (CommunicationException commEx)
            {
                // Question should a communication error, be reported as an error or just a messagebox?
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load activity headers.{Environment.NewLine}" +
                        $"Because of a communication problem.{Environment.NewLine}" +
                        $"Detail:{commEx.Message}{Environment.NewLine}" +
                        "Please exit and try again.";

                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK);
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load activity headers.{Environment.NewLine}" +
                        $"General exception.{Environment.NewLine}" +
                        $"Detail:{ex.Message}{Environment.NewLine}" +
                        "Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
                if (!performRefreshData)
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    await this.Close();
                }

                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> HandlePrePickCriteria()
            {
                var performRefreshData = true;

                // This code should be moved into an picking header class inherited from ucActivityHdr.
                var sectFKRefno = 0;

                // Scanning in the picking selection criteria form is possible, which means the
                // the last parse bar code data would be overwritten.
                // However because the bar code parse object at this point does not contain any
                // valuable data we do not need to preserve it before calling the picking
                // selection form.
                var formPickSelect = new FormDialogContainer(
                    EDialogTypes.ActivityPickingSelectCriteria,
                    this.ActSubType);

                if (await Device.InvokeOnMainThreadAsync(async () =>
                    await formPickSelect.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    var xmlResult = await formPickSelect.GetXML();
                    var locMap = LocationMap.GetInstance();
                    var tmpLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        int.Parse(xmlResult.Root.Element("LocationMapRefno").Value));
                    locMap.CurrLocMap = tmpLocMap;
                    ((PickingActivity)this.ActHdr).PickingLocationMapFilter = tmpLocMap;
                    sectFKRefno = tmpLocMap.SectFKRefno;

                    this.ActHdr.XmlDataCriteria = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + "<whs_id>" + tmpLocMap.WhsFKRefno + "</whs_id>"
                                + "<sect_refno>" + tmpLocMap.SectFKRefno + "</sect_refno>"
                                + "</params>");
                }

                // See WO#12906 do not load any orders if no section was selected, rather than
                // load all the orders.
                if (this.SysEnv.SysSettings.IsPickingModuleDisallowLoadingAllOrders && sectFKRefno == 0)
                {
                    performRefreshData = false;
                }

                return performRefreshData;
            }
        }

        /// <summary>
        /// Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override async void OnDisappearing()
        {
            this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
        }

        /// <summary>
        /// Subscribe key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), this.KeyUpHandlerCallback);
        }

        /// <summary>
        /// Key up handler call back.
        /// </summary>
        /// <param name="keyPressed"><see cref="string"/> Key pressed value.</param>
        private async void KeyUpHandlerCallback(string keyPressed)
        {
            if (this.Loading)
            {
                return;
            }

            try
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
                switch (keycode)
                {
                    case Keycode.Enter:
                        if (!this.IsSearchEntryFocused)
                        {
                            this.Loading = true;
                            await KeyUpEnterHandler();
                        }

                        break;

                    case Keycode.F1:
                        this.Loading = true;
                        await KeyUpF1Handler();
                        break;

                    case Keycode.F4 when !this.SysEnv.SysSettings.IsDisconnectedMode:
                        if (!this.IsSearchEntryFocused)
                        {
                            this.Loading = true;
                            await KeyUpF4HandlerReleaseOrder();
                        }

                        break;

                    case Keycode.F5:
                        this.Loading = true;
                        await KeyUpF5HandlerChangePickCriteria();
                        break;

                    case Keycode.DpadDown:
                        if (!this.IsSearchEntryFocused)
                        {
                            AppearingDpadDownKey();
                        }

                        break;

                    case Keycode.DpadUp:
                        if (!this.IsSearchEntryFocused)
                        {
                            AppearingDpadUpKey();
                        }

                        break;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadDownKey()
            {
                var selItem = this.ListViewMainActivity.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainActivityItems.IndexOf(selItem);
                if (selItemIdx >= this.MainActivityItems.Count - 1)
                {
                    return;
                }

                this.ListViewMainActivity.SelectedItem = this.MainActivityItems[selItemIdx + 1];
                this.ListViewMainActivity.ScrollTo(
                    item: this.ListViewMainActivity.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadUpKey()
            {
                var selItem = this.ListViewMainActivity.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainActivityItems.IndexOf(selItem);
                if (selItemIdx <= 0)
                {
                    return;
                }

                this.ListViewMainActivity.SelectedItem = this.MainActivityItems[selItemIdx - 1];
                this.ListViewMainActivity.ScrollTo(
                    item: this.ListViewMainActivity.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpF1Handler()
            {
                if (await this.SaveChanges())
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    await this.Close();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpF4HandlerReleaseOrder()
            {
                try
                {
                    IsItemSelected();

                    // Important! Need to ask for label quantity before the change status AND set
                    // this.ActHdr.Refno
                    this.ActHdr.HdrRefno = this.ActHdr.GetRefnoFromRowIdx(this.ListViewMainActivity.SelectedItem);

                    var labelQuantity = new SLabelQuantity();

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle &&
                        this.SysEnv.SysSettings.CanPrintShippingLabel &&
                        !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        labelQuantity = await this.ActHdr.GetLabelQuantity(
                            this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity);
                    }

                    if (await Device.InvokeOnMainThreadAsync(async () =>
                        await this.ActHdr.ChangeStatus(this.ListViewMainActivity.SelectedItem)))
                    {
                        // Print labels after release
                        // Lekker Foods wants to be ask to print labels after the order has been released.
                        // There is no opportunity to reprint after the release.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            // Shipping labels
                            if (this.SysEnv.SysSettings.CanPrintShippingLabel &&
                                !this.SysEnv.SysSettings.IsDisconnectedMode)
                            {
                                labelQuantity = await PrintLabelsAfterRelease(labelQuantity);
                            }

                            if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                            {
                                await this.RefreshDataBySubType();
                            }
                            else
                            {
                                _ = this.ActHdr.ListHdrs.Remove(this.ListViewMainActivity.SelectedItem);
                                await this.DesignGrid();
                            }
                        }
                        else if (this.ActSubType == SysCommon.ActSubTypes.Adjustment)
                        {
                            // <WO ID="14748" comment="Printing after deboxing" />
                            // If DEBOX, prompt for label
                            if (this.ActHdr is AdjActivity adjustment)
                            {
                                if (adjustment.AdjSettings != null &&
                                    adjustment.AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox)
                                {
                                    await this.ScannerDisable();

                                    // Show pop up screen
                                    var form = new FormDialogContainer(
                                        EDialogTypes.ActivityDeboxLabelCount,
                                        this.ActSubType);

                                    if (await Device.InvokeOnMainThreadAsync(async () =>
                                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                                    {
                                        var doc = await form.GetXML();
                                        await this.ActHdr.PrintLabel(doc);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await this.FindSelectedItemAsync();
                }

                //////////////////////////////////////////////////////////////////////////
                async Task<SLabelQuantity> PrintLabelsAfterRelease(
                    SLabelQuantity labelQuantity)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                        this.ActSubType == SysCommon.ActSubTypes.Picking)
                    {
                        if (this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity)
                        {
                            var form = new FormDialogContainer(
                                EDialogTypes.ActivityShippingLabelEdit,
                                labelQuantity,
                                this.ActHdr,
                                ShippingLabelQuantitySelectionMode.RepackOnly);

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog() == DialogResult.OK))
                            {
                                var request = await form.GetXML();

                                // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                // If itm_refno is zero then no line is updated as printed.
                                request.Element("params").Element("whs_id").AddAfterSelf(
                                    new XElement("itm_refno", 0));
                                await this.ActHdr.PrintLabel(request);
                            }
                        }
                        else
                        {
                            var form = new FormDialogContainer(
                                EDialogTypes.ActivityShippingLabelEdit,
                                labelQuantity,
                                this.ActHdr,
                                ShippingLabelQuantitySelectionMode.BothFullRepack);

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog() == DialogResult.OK))
                            {
                                var request = await form.GetXML();

                                // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                // If itm_refno is zero then no line is updated as printed.
                                request.Element("params").Element("whs_id").AddAfterSelf(
                                    new XElement("itm_refno", 0));
                                await this.ActHdr.PrintLabel(request);
                            }
                        }
                    }

                    return labelQuantity;
                }

                //////////////////////////////////////////////////////////////////////////
                void IsItemSelected()
                {
                    var isItemSelected = this.ListViewMainActivity.SelectedItem != null
                                         && Convert.ToInt32(this.MainActivityItems.IndexOf(
                                             this.ListViewMainActivity.SelectedItem)) >= 0;

                    if (!isItemSelected)
                    {
                        throw new MessageOnlyException("Nothing selected to release.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpF5HandlerChangePickCriteria()
            {
                // Change pick selection.
                await Task.Run(async () =>
                {
                    try
                    {
                        if ((this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle) &&
                            this.SysEnv.SysSettings.IsPrePickCrit)
                        {
                            // NOTE: oBarCodeParse is not initialized yet so there is not reason to
                            // preserve its values
                            var form = new FormDialogContainer(
                                EDialogTypes.ActivityPickingSelectCriteria,
                                this.ActSubType);

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog() == DialogResult.OK))
                            {
                                var locMap = LocationMap.GetInstance();
                                var result = await form.GetXML();
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    int.Parse(result.Root.Element("LocationMapRefno").Value));
                                locMap.CurrLocMap = sLocMap;

                                this.ActHdr.XmlDataCriteria = XDocument.Parse(
                                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                    + "<params>"
                                    + "<whs_id>" + sLocMap.WhsFKRefno + "</whs_id>"
                                    + "<sect_refno>" + sLocMap.SectFKRefno + "</sect_refno>"
                                    + "</params>");
                            }

                            await this.RefreshDataBySubType();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpEnterHandler()
            {
                this.ActHdr.HdrRefno = this.ActHdr.GetRefnoFromRowIdx(this.ListViewMainActivity.SelectedItem);
                if (this.SysEnv.SysSettings.IsPickSingleProd &&
                    this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    await this.ListViewMainActivity_KeyUpEnter_PickingSingle();
                }
                else
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityItem,
                        this.ActHdr,
                        this.ActSubType);
                    this.IsChangesMade = false;
                    _ = await Device.InvokeOnMainThreadAsync(async () =>
                            await form.DisplayControl.ShowDialog());
                }
            }
        }

        /// <summary>
        /// Ask to confirm to exit.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task<bool> AskConfirmExit()
        {
            var isReadyToClose = !this.IsChangesMade;
            await this.DisplayAlert(this.ActHdr.GetControlDescription(), "Confirm exit?", "Yes", "No")
                .ContinueWith((result) =>
                {
                    if (!result.Result)
                    {
                        isReadyToClose = false;
                    }
                });

            return isReadyToClose;
        }

        /// <summary>
        /// Bar code event handler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Bar code information.</param>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading)
            {
                return;
            }

            ////if (scanArgs == null)
            ////{
            ////    var msg = "Scanner event fired but not data was received.";
            ////    await Device.InvokeOnMainThreadAsync(async () =>
            ////        await Application.Current.MainPage.DisplayAlert(
            ////            this.ActHdr.GetControlDescription(),
            ////            msg,
            ////            Resource.OK));
            ////    return;
            ////}

            var message = string.Empty;

            try
            {
                this.Loading = true;
                await this.ScannerDisable();
                this.BarParse = scanArgs.ParseInfo;
                if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                {
                    // this.BarParse.ActFKRefno e.g. format is 'OE:12345678'.
                    // Start at 3th position for the foreign key reference number.
                    await BarCodeHandlerFKRefno(this.BarParse.ActFKRefno);
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                {
                    throw new MessageOnlyException(
                        "Scanning an activity item reference # is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new MessageOnlyException(
                        "Scanning a product bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                {
                    throw new MessageOnlyException(
                        "Scanning a shipping container bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new MessageOnlyException(
                        "Scanning a location identifier bar code is not supported here!");
                }
                else
                {
                    throw new MessageOnlyException(
                        "Scanned bar code type is not supported here!");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerFKRefno(string actHdrStringFKRefno)
            {
                // Remember that the headers have been filtered by the login warehouse.
                var selectedActivity = this.ActHdr.ListHdrs.OfType<object>().FirstOrDefault(
                    activity => Convert.ToString(activity.GetType().GetProperty(
                        this.ActHdr.GetFkRefnoIdx()).GetValue(activity)) ==
                        actHdrStringFKRefno);
                if (selectedActivity != null)
                {
                    this.ActHdr.HdrRefno = this.ActHdr.GetRefnoFromRowIdx(selectedActivity);
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var form = new FormDialogContainer(
                            EDialogTypes.ActivityItem,
                            this.ActHdr,
                            this.ActSubType);
                        this.IsChangesMade = false;
                        _ = await form.DisplayControl.ShowDialog();
                    });
                }
                else
                {
                    this.ActHdr.HdrFkRefno = 0;
                    this.ActHdr.HdrRefno = 0;
                    throw new MessageOnlyException(
                        $"Unable to find reference # {actHdrStringFKRefno}. Perhaps change your login warehouse.");
                }
            }
        }

        /// <summary>
        /// Handle double click on list view item.
        /// Using a tap gesture recognizer in the designGrid would be optimal, however there is
        /// currently a known bug that prevents selectedItemChanged from firing.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e">The e<see cref="ItemTappedEventArgs"/> Item tapped event arguments.</param>
        private async void ListViewMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            this.NumTaps++;

            if (this.PreviousSender == e.Item)
            {
                if (this.NumTaps == 2)
                {
                    this.NumTaps = 0;

                    try
                    {
                        this.Loading = true;
                        await this.ScannerDisable();
                        this.ActHdr.HdrRefno = this.ActHdr.GetRefnoFromRowIdx(this.ListViewMainActivity.SelectedItem);
                        if (this.SysEnv.SysSettings.IsPickSingleProd &&
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            await this.ListViewMainActivity_KeyUpEnter_PickingSingle();
                        }
                        else
                        {
                            var form = new FormDialogContainer(
                                EDialogTypes.ActivityItem,
                                this.ActHdr,
                                this.ActSubType);
                            this.IsChangesMade = false;
                            _ = await Device.InvokeOnMainThreadAsync(async () =>
                                    await form.DisplayControl.ShowDialog());
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                            ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActHdr.GetControlDescription(),
                                msg,
                                Resource.OK));
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
                        this.Loading = false;
                    }
                }
            }
            else
            {
                this.PreviousSender = e.Item;
                this.NumTaps = 1;
            }

            await Task.Delay(500);
            this.NumTaps = 0;
        }

        /// <summary>
        /// Call refresh data, unique to the activity sub type.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task RefreshDataBySubType()
        {
            // NOTE: So far all sub types will be refreshed.
            switch (this.ActSubType)
            {
                default:
                    // Performing a refresh data will remove and partially added new items or quantity.
                    await RefreshData();
                    break;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task RefreshData()
            {
                // NOTE: So far all sub types will be refreshed.
                try
                {
                    this.ActHdr.RegLayerListenerStatOut(this);
                    this.ActHdr.RegLayerListenerProgOut(this);
                    await this.ActHdr.Refresh(this.ActHdr.XmlDataCriteria);
                    this.ActHdr.ListHdrs = await this.ActHdr.GetActivityList(isSubmitBatch: false);
                    await this.DesignGrid();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    this.ActHdr.UnregLayerListenerStatOut(this);
                    this.ActHdr.UnregLayerListenerProgOut(this);
                }
            }
        }

        /// <summary>
        /// Design the main activity grid / list.
        /// </summary>
        /// <remarks>Search text is applied as a filter to list of headers.</remarks>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
        private async Task DesignGrid()
        {
            // Bind the filtered activity items to the list view.
            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.MainActivityItems = this.ActHdr.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);

                if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                {
                    this.MainActivityItems = this.ActHdr.Search(this.txtSearchHdr.Text);
                }

                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            });
        }

        /// <summary>
        /// Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActHdr.ListHdrs.Count > 0 && this.MainActivityItems.Count > 0)
            {
                // Iterate through the list of headers looking for a foreign key match.
                // Faster than LINQ.
                this.ActHdr.SelectedListItemIdx = 0;
                object selectedItem = null;
                foreach (var hdr in this.ActHdr.ListHdrs)
                {
                    if (this.SelectedActivityFkRefno == this.ActHdr.GetFkRefno(hdr))
                    {
                        selectedItem = hdr;
                        break;
                    }

                    this.ActHdr.SelectedListItemIdx++;
                }

                // Select the first header if no match was found, else select the item.
                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActHdr.SelectedListItemIdx = 0;
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActHdr.SelectedListItemIdx];
                    await this.ActHdr.SelectActivity(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActHdr.SelectActivity(selectedItem);
                    this.ListViewMainActivity.SelectedItem = this.ActHdr.SelectedActivity;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActHdr.SelectedListItemIdx = -1;
                this.ActHdr.SelectedActivity = null;
                this.ActHdr.HdrFkRefno = 0;
                this.ActHdr.HdrRefno = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// Add item command button event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdAdd_Click(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire if a
                // command button is the next control rather than just giving the button focus.
                XDocument xmlDoc = null;
                _ = await this.ActHdr.Insert(xmlDoc);
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];

                // Do not set the IsChangesMade. MessageOnlyException is really not
                // and error.
                // Allow the validation to handle the setting for IsChangesMade.
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
                this.Loading = false;
            }
        }

        /// <summary>
        /// Main data grid key up enter event handler.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        private async Task ListViewMainActivity_KeyUpEnter()
        {
            if (this.Loading)
            {
                return;
            }

            if (this.ListViewMainActivity.SelectedItem == null)
            {
                return;
            }

            if (Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
            {
                return;
            }

            try
            {
                this.Loading = true;
                await this.ScannerDisable();
                this.ActHdr.HdrRefno = this.ActHdr.GetRefnoFromRowIdx(this.ListViewMainActivity.SelectedItem);
                if (this.SysEnv.SysSettings.IsPickSingleProd &&
                    this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    await this.ListViewMainActivity_KeyUpEnter_PickingSingle();
                }
                else
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityItem,
                        this.ActHdr,
                        this.ActSubType);
                    this.IsChangesMade = false;
                    _ = await Device.InvokeOnMainThreadAsync(async () =>
                            await form.DisplayControl.ShowDialog());
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
                this.Loading = false;
            }
        }

        /// <summary>
        /// Perform key up enter picking single.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task ListViewMainActivity_KeyUpEnter_PickingSingle()
        {
            var isAllowedToPick = true;
            if (await ((PickingActivity)this.ActHdr).IsSalesOrderBeingPicked())
            {
                isAllowedToPick = false;
                if (this.SysEnv.ContactLog.IsAllowedPickingModuleOverridePicker)
                {
                    // TODO: Test
                    if (await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            "Picker Override",
                            "Do you wish to clear the current picker?",
                            "Yes",
                            "No")))
                    {
                        if (await ((PickingActivity)this.ActHdr).ClearSalesOrderBeingPicked(false))
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    "Picker Override",
                                    "Picker cleared.",
                                    "OK"));
                            isAllowedToPick = true;
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    "Picker Override",
                                    "Unable to clear the current picker.",
                                    "OK"));
                        }
                    }
                }
            }

            if (isAllowedToPick)
            {
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityItemSingle,
                    this.ActHdr,
                    this.ActSubType);
                this.IsChangesMade = false;
                _ = await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog());
            }
        }

        /// <summary>
        /// Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListView_Focused(object sender, FocusEventArgs e)
        {
            this.IsHdrListViewInFocus = true;
        }

        /// <summary>
        /// Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListView_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsHdrListViewInFocus = false;
        }

        /// <summary>
        /// Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListDisplay_Focused(object sender, FocusEventArgs e)
        {
            this.IsDisplayListViewInFocus = true;
        }

        /// <summary>
        /// Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListDisplay_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsDisplayListViewInFocus = false;
        }

        /// <summary>
        /// Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void CmdAdd_Focused(object sender, FocusEventArgs e)
        {
            this.IsAddButtonFocused = true;
        }

        /// <summary>
        /// Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void CmdAdd_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsAddButtonFocused = false;
        }

        /// <summary>
        /// The ListViewMainActivity_ItemSelected.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Select item changed event arguments.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActHdr.SelectedListItemIdx = e.SelectedItemIndex;
            await this.ActHdr.SelectActivity(e.SelectedItem);
            this.SelectedActivityFkRefno = this.ActHdr.HdrFkRefno;
            await DisplayData();

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayData()
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ExtraInfoItems.Clear();

                    // TODO: Test this function after SQL changes
                    // This code should be moved into an activity header classes inherited from ucActivityHdr
                    // for each subtype.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.Adjustment:
                            DisplayDataAdjustments();

                            break;

                        case SysCommon.ActSubTypes.AssignDefaultLocation:
                            break;

                        case SysCommon.ActSubTypes.BarCodeAssociation:
                            DisplayDataBarCodeAssoc();

                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            DisplayDataCycleCounts();

                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            DisplayDataPhysicalCounts();

                            break;

                        case SysCommon.ActSubTypes.Picking:
                            DisplayDataPicking();

                            break;

                        case SysCommon.ActSubTypes.PickingSingle:
                            DisplayDataPickingSingle();

                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            DisplayDataAddProduct();

                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            DisplayDataTicketFinished();

                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            DisplayDataTicketMaterial();

                            break;

                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                            break;

                        case SysCommon.ActSubTypes.Receiving:
                            DisplayDataReceiving();
                            break;

                        case SysCommon.ActSubTypes.ReleaseByOrder:
                            DisplayDataReleaseByOrder();

                            break;

                        case SysCommon.ActSubTypes.Returns:
                            DisplayDataReturns();

                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            DisplayDataTransferFrom();

                            break;

                        case SysCommon.ActSubTypes.TransferTo:
                            DisplayDataTransferFrom();

                            break;

                        case SysCommon.ActSubTypes.NotApplicable:
                            throw new NotImplementedException("SysCommon.ActSubTypes.NotApplicable");

                        case SysCommon.ActSubTypes.RefreshData:
                            throw new NotImplementedException("SysCommon.ActSubTypes.RefreshData");

                        case SysCommon.ActSubTypes.DataStatus:
                            throw new NotImplementedException("SysCommon.ActSubTypes.DataStatus");

                        case SysCommon.ActSubTypes.SelectProduct:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SelectProduct");

                        case SysCommon.ActSubTypes.SelectLocation:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SelectLocation");

                        case SysCommon.ActSubTypes.ConvertUnit:
                            throw new NotImplementedException("SysCommon.ActSubTypes.ConvertUnit");

                        case SysCommon.ActSubTypes.MoveInventory:
                            throw new NotImplementedException("SysCommon.ActSubTypes.MoveInventory");

                        case SysCommon.ActSubTypes.PurgeActivityData:
                            throw new NotImplementedException("SysCommon.ActSubTypes.PurgeActivityData");

                        case SysCommon.ActSubTypes.ReleaseByItemSection:
                            throw new NotImplementedException("SysCommon.ActSubTypes.ReleaseByItemSection");

                        case SysCommon.ActSubTypes.SkidInfo:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SkidInfo");

                        case SysCommon.ActSubTypes.MoveSkid:
                            throw new NotImplementedException("SysCommon.ActSubTypes.MoveSkid");

                        case SysCommon.ActSubTypes.BreakSkid:
                            throw new NotImplementedException("SysCommon.ActSubTypes.BreakSkid");

                        case SysCommon.ActSubTypes.PutAwaySkid:
                            throw new NotImplementedException("SysCommon.ActSubTypes.PutAwaySkid");

                        case SysCommon.ActSubTypes.InventoryInquiry:
                            throw new NotImplementedException("SysCommon.ActSubTypes.InventoryInquiry");

                        case SysCommon.ActSubTypes.InventoryReplenishment:
                            throw new NotImplementedException("SysCommon.ActSubTypes.InventoryReplenishment");

                        case SysCommon.ActSubTypes.LoginStartup:
                            throw new NotImplementedException("SysCommon.ActSubTypes.LoginStartup");

                        case SysCommon.ActSubTypes.SplashStartup:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SplashStartup");

                        case SysCommon.ActSubTypes.PrintProductLabel:
                            throw new NotImplementedException("SysCommon.ActSubTypes.PrintProductLabel");

                        case SysCommon.ActSubTypes.PickingSelectCriteria:
                            throw new NotImplementedException("SysCommon.ActSubTypes.PickingSelectCriteria");

                        case SysCommon.ActSubTypes.PickingFilter:
                            throw new NotImplementedException("SysCommon.ActSubTypes.PickingFilter");

                        case SysCommon.ActSubTypes.SubmitBatch:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SubmitBatch");

                        case SysCommon.ActSubTypes.CommunicationSetup:
                            throw new NotImplementedException("SysCommon.ActSubTypes.CommunicationSetup");

                        case SysCommon.ActSubTypes.SetupDefaults:
                            throw new NotImplementedException("SysCommon.ActSubTypes.SetupDefaults");

                        case SysCommon.ActSubTypes.InventoryInquirySummary:
                            throw new NotImplementedException("SysCommon.ActSubTypes.InventoryInquirySummary");

                        case SysCommon.ActSubTypes.ConvertProduct:
                            throw new NotImplementedException("SysCommon.ActSubTypes.ConvertProduct");

                        default:
                            break;
                    }
                });

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataAdjustments()
                {
                    if (this.ActHdr.SelectedActivity is Adjustment adjustment)
                    {
                        this.ExtraInfoItems.Add("Adjustment type: " + adjustment.ActivityAdjustmentTypeDescription);
                        this.ExtraInfoItems.Add("Warehouse:  " + adjustment.LocationMapWarehouseName.ToString());
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataBarCodeAssoc()
                {
                    if (this.ActHdr.SelectedActivity is BarcodeAssociation barcodeAssociation)
                    {
                        this.ExtraInfoItems.Add(barcodeAssociation.ProductUnitBarCodeCode.TrimEnd());
                        this.ExtraInfoItems.Add(barcodeAssociation.ProductUnitBarCodeModifier1.TrimEnd());
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataCycleCounts()
                {
                    if (this.ActHdr.SelectedActivity is Count count)
                    {
                        const int LineLength = 30;
                        var lines = SysCommon.WordWrap2List(
                            count.ActivityCountDescription.TrimEnd(),
                            LineLength);

                        for (var i = 0; i < lines.Count; i++)
                        {
                            this.ExtraInfoItems.Add(lines[i]);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataPhysicalCounts()
                {
                    if (this.ActHdr.SelectedActivity is Count count)
                    {
                        const int LineLength = 30;
                        var lines = SysCommon.WordWrap2List(
                            count.ActivityCountDescription.TrimEnd(),
                            LineLength);

                        for (var i = 0; i < lines.Count; i++)
                        {
                            this.ExtraInfoItems.Add(lines[i]);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataPicking()
                {
                    if (this.ActHdr.SelectedActivity is Picking picking)
                    {
                        this.ExtraInfoItems.Add("Customer: " + picking.ActivityPickingCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " + picking.ActivityPickingCustomerShipToInfo);
                        this.ExtraInfoItems.Add($"Order warehouse:   {picking.LocationMapWarehouseFKRefno} - {picking.LocationMapWarehouseName}");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataPickingSingle()
                {
                    if (this.ActHdr.SelectedActivity is Picking pickingSingle)
                    {
                        this.ExtraInfoItems.Add("Customer: " + pickingSingle.ActivityPickingCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " + pickingSingle.ActivityPickingCustomerShipToInfo);
                        this.ExtraInfoItems.Add($"Order warehouse:   {pickingSingle.LocationMapWarehouseFKRefno} - {pickingSingle.LocationMapWarehouseName}");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataAddProduct()
                {
                    if (this.ActHdr.SelectedActivity is ProductionTicketByProduct productionTicketByProduct)
                    {
                        this.ExtraInfoItems.Add(productionTicketByProduct.ActivityProductionDescription);
                        this.ExtraInfoItems.Add("Warehouse:  " + productionTicketByProduct.LocationMapWarehouseName);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataTicketFinished()
                {
                    if (this.ActHdr.SelectedActivity is ProductionTicketFinished productionTicketFinished)
                    {
                        this.ExtraInfoItems.Add(productionTicketFinished.ActivityProductionDescription);
                        this.ExtraInfoItems.Add("Warehouse:  " + productionTicketFinished.LocationMapWarehouseName);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataTicketMaterial()
                {
                    if (this.ActHdr.SelectedActivity is ProductionTicketMaterial productionTicketMaterial)
                    {
                        this.ExtraInfoItems.Add(productionTicketMaterial.ActivityProductionDescription);
                        this.ExtraInfoItems.Add("Warehouse:  " + productionTicketMaterial.LocationMapWarehouseName);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataReceiving()
                {
                    if (this.ActHdr.SelectedActivity is Receive receive)
                    {
                        this.ExtraInfoItems.Add("Vendor:  " + receive.ActivityReceivingVendorContactName);
                        this.ExtraInfoItems.Add("Supplier:  " + receive.ActivityReceivingVendorSupplierContactName);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataReleaseByOrder()
                {
                    if (this.ActHdr.SelectedActivity is Picking pickingReleaseByOrder)
                    {
                        this.ExtraInfoItems.Add("Customer: " + pickingReleaseByOrder.ActivityPickingCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " + pickingReleaseByOrder.ActivityPickingCustomerShipToInfo);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataReturns()
                {
                    if (this.ActHdr.SelectedActivity is Return returns)
                    {
                        this.ExtraInfoItems.Add("Customer: " + returns.ActivityReturnsCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " + returns.ActivityReturnsCustomerShipToInfo);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataTransferFrom()
                {
                    if (this.ActHdr.SelectedActivity is Transfer transfer)
                    {
                        this.ExtraInfoItems.Add(transfer.ActivityTransferDescription);
                        this.ExtraInfoItems.Add("To Warehouse:  " + transfer.LocationMapWarehouseName);
                    }
                }
            }
        }

        /// <summary>
        /// Text box search item focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void TxtSearchItem_Focused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = true;
        }

        /// <summary>
        /// Text box search item unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void TxtSearchItem_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = false;
        }

        /// <summary>
        /// Text search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                await this.DesignGrid();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// Option menu click handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                var result = await this.DisplayActionSheet(
                    "Activity Options",
                    "Close",
                    null,
                    this.OptionsMenuItems.ToArray());

                if (result == this.FilterText && this.FilterIsEnabled)
                {
                    EDialogTypes dlgType = 0;
                    dlgType = this.ActSubType switch
                    {
                        SysCommon.ActSubTypes.Picking => EDialogTypes.ActivityHdrPickingFilter,
                        SysCommon.ActSubTypes.PickingSingle => EDialogTypes.ActivityHdrPickingFilter,
                        SysCommon.ActSubTypes.Receiving => EDialogTypes.ActivityHdrReceivingFilter,
                        SysCommon.ActSubTypes.TransferFrom => EDialogTypes.ActivityHdrTransferFilter,
                        SysCommon.ActSubTypes.TransferTo => EDialogTypes.ActivityHdrTransferFilter,
                        SysCommon.ActSubTypes.PhysicalCounts => EDialogTypes.ActivityHdrPhysicalCountFilter,
                        SysCommon.ActSubTypes.CycleCounts => EDialogTypes.ActivityHdrCycleCountFilter,
                        SysCommon.ActSubTypes.Adjustment => EDialogTypes.ActivityAdjustmentFilter,
                        SysCommon.ActSubTypes.Returns => EDialogTypes.ActivityHdrReturnsFilter,
                        SysCommon.ActSubTypes.TicketFinished => EDialogTypes.ActivityProductionTicketFinishedFilter,
                        SysCommon.ActSubTypes.TicketMaterial => EDialogTypes.ActivityProductionTicketMaterialFilter,
                        SysCommon.ActSubTypes.TicketAddProduct => EDialogTypes.ActivityProductionTicketByProductFilter,
                        SysCommon.ActSubTypes.NotApplicable => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.BarCodeAssociation => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.RefreshData => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.DataStatus => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SelectProduct => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SelectLocation => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.ConvertUnit => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.MoveInventory => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PurgeActivityData => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.ReleaseByOrder => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.ReleaseByItemSection => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PurchaseOrderWorkSheet => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SkidInfo => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.MoveSkid => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.BreakSkid => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PutAwaySkid => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.AssignDefaultLocation => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.InventoryInquiry => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.InventoryReplenishment => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.LoginStartup => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SplashStartup => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PrintProductLabel => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PickingSelectCriteria => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.PickingFilter => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SubmitBatch => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.CommunicationSetup => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.SetupDefaults => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.InventoryInquirySummary => throw new NotImplementedException(),
                        SysCommon.ActSubTypes.ConvertProduct => throw new NotImplementedException(),
                        _ => throw new NotImplementedException("eActivitySubType"),
                    };

                    var form = new FormDialogContainer(dlgType, this.ActSubType);
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            var xmlDoc = await form.GetXML();
                            if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                            {
                                this.MainActivityItems = this.ActHdr.Search(this.txtSearchHdr.Text);
                            }
                        }
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];

                // Do not set the IsChangesMade. MessageOnlyException is really not
                // an error.
                // Allow the validation to handle the setting for IsChangesMade.
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
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
                this.Loading = false;
            }
        }
    }
}
