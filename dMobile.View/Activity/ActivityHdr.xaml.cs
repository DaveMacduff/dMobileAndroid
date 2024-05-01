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
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller;
    using Controller.Activity;
    using Forms;
    using Helpers;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityHdr" /> Activity header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdr : IAppArea, IDataLayerEventListener
    {
        /// <summary>
        ///     Define the filter text was enabled.
        /// </summary>
        private bool filterIsEnabled;

        /// <summary>
        ///     Defines the text filter.
        /// </summary>
        private string filterText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdr"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        /// <param name="xmlDataCriteria"><see cref="XDocument"/> Activity data filter criteria.</param>
        /// <param name="pickingLocMapFilter"><see cref="SLocMap"/> Picking filter location map.</param>
        public ActivityHdr(SysCommon.ActSubTypes activitySubType, XDocument xmlDataCriteria, SLocMap pickingLocMapFilter)
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
            this.IsDataLoading = false;

            // If the activity header object does not have a skid already initialize the instance.
            this.ActHdr.SkidInfo ??= new SysSkidInfo();
            this.ActHdr.XmlDataCriteria = xmlDataCriteria;
            ((PickingActivity)this.ActHdr).PickingLocationMapFilter = pickingLocMapFilter;
            this.Timer = new SysTimer(this.TimerTickCallback, 1000);
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.LoadAndBindData().FireAndForgetSafeAsync();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdr" /> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
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
            this.IsDataLoading = false;

            // If the activity header object does not have a skid already initialize the instance.
            this.ActHdr.SkidInfo ??= new SysSkidInfo();
            this.ActHdr.XmlDataCriteria = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "<whs_id>0</whs_id>" +
                "<sect_refno>0</sect_refno>" +
                "</params>");
            this.Timer = new SysTimer(this.TimerTickCallback, 1000);
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.LoadAndBindData().FireAndForgetSafeAsync();
        }

        public ActivityHdr(BarCodeScanner scanner)
        {
            this.Scanner = scanner;
        }

        /// <summary>
        /// Delay over handler delegate.
        /// </summary>
        public delegate void DelayOverHandler();

        /// <summary>
        ///     Gets or sets the selected activity foreign key reference number.
        /// </summary>
        private int SelectedActivityFkRefno { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether any data changes have be made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets the bar code parsed data.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        ///     Gets the global system environment object.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Gets the activity header instance.
        /// </summary>
        private Activity ActHdr { get; }

        /// <summary>
        ///     Gets the activity header subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets the list of items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        ///     Gets the OptionsMenuItems.
        /// </summary>
        private List<string> OptionsMenuItems { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the system timer.
        /// </summary>
        private SysTimer Timer { get; set; }

        /// <summary>
        ///     Gets or sets the filter text.
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
        ///     Gets or sets a value indicating whether entering a filter was performed.
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
        ///     Gets or sets a number of screen taps tracker for Xamarin bug that prevents
        ///     selectedItemChanged from firing.
        /// </summary>
        private int NumTaps { get; set; }

        /// <summary>
        ///     Gets or sets the previous sender used in tracking the number of screen taps for
        ///     Xamarin bug that prevents selectedItemChanged from firing.
        /// </summary>
        private object PreviousSender { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the search text box is in focus.
        /// </summary>
        private bool IsSearchEntryFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the data is being loaded.
        /// </summary>
        private bool IsDataLoading { get; set; }

        /// <summary>
        ///     Gets or sets the search text box original text.
        /// </summary>
        private string SearchTextOrig { get; set; } = string.Empty;

        /// <summary>
        ///     Timer tick call back.
        /// </summary>
        public void TimerTickCallback()
        {
            this.Timer.Stop();
            if (this.IsSearchEntryFocused && this.txtSearchHdr.Text != this.SearchTextOrig)
            {
                this.SearchTextOrig = this.txtSearchHdr.Text;
                this.LoadAndBindData().FireAndForgetSafeAsync();
            }

            // Did not start the timer again here because the timer is started in the OnTextChange event.
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
                this.Scanner.BarCodeScan += this.BarCodeHandler;
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        ///     Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;

            if (isReadyToSave && (this.ActSubType == SysCommon.ActSubTypes.Picking
                                  || this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
            {
                isReadyToSave = await IsReadyToSavePicking();
            }

            return isReadyToSave;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToSavePicking()
            {
                var isReadyToSavePicking = true;

                // If activity is picking and confirm to exit form is enabled.
                // This code should be moved into an picking header class inherited from ucActivityHdr.
                if (this.SysEnv.SysSettings.IsPickConfirmExit)
                {
                    await Device.InvokeOnMainThreadAsync(async () => { isReadyToSavePicking = await this.AskConfirmExit(); });
                }

                return isReadyToSavePicking;
            }
        }

        /// <summary>
        ///     Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form is ready to close else false.</returns>
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
        ///     Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool" /> True if a save was performed successfully else false.</returns>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete;

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
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
        ///     Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool" /> True if the changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

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
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
        ///     Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation containing an XDocument.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///     Data layer status out event handler.
        /// </summary>
        /// <param name="publisher">
        ///     <see cref="IDataLayerEventPublisher" /> Data layer
        ///     publisher delegate.
        /// </param>
        /// <param name="dataLayerEventArgs">
        ///     <see cref="DataGridEnableEventArgs" /> Data layer
        ///     publisher event arguments.
        /// </param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            this.ExtraInfoItems.Clear();
            if (!string.IsNullOrEmpty(dataLayerEventArgs.StatusOutMsg))
            {
                this.ExtraInfoItems.Add(dataLayerEventArgs.StatusOutMsg);
            }
        }

        /// <summary>
        ///     Data layer progress out event handler.
        /// </summary>
        /// <param name="publisher">
        ///     <see cref="IDataLayerEventPublisher" /> Data layer
        ///     publisher delegate.
        /// </param>
        /// <param name="dataLayerEventArgs">
        ///     <see cref="DataGridEnableEventArgs" /> Data layer
        ///     publisher event arguments.
        /// </param>
        public void OnLayerProgressOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            if (dataLayerEventArgs.CurrentValue > 0 && dataLayerEventArgs.MaxValue > 0)
            {
                ////double percent = 100 * dataLayerEventArgs.CurrentValue / dataLayerEventArgs.MaxValue;
                ////this.ProgressBarMain.Progress = percent / 100;
            }
        }

        /// <summary>
        ///     Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformBackButton);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void PerformBackButton()
            {
                if (await this.CancelChanges())
                {
                    await this.ScannerDisable();
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            }
        }

        /// <summary>
        ///     Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string" /> Option property name..</param>
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
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.NumTaps = 0;
            this.Timer.Start();
            this.SubscribeKeyUpHandlers();
            this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
            if (!this.IsSearchEntryFocused)
            {
                this.ListViewMainActivity.Focus();
                await this.FindSelectedItemAsync();
            }
        }

        /// <summary>
        ///     Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override async void OnDisappearing()
        {
            this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
            this.Timer.Stop();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Load and bind the data.
        /// </summary>
        /// <returns><see cref="Task"/> Task complete.</returns>
        private async Task LoadAndBindData()
        {
            try
            {
                if (!this.IsDataLoading)
                {
                    this.IsDataLoading = true;

                    // The activity indicator control messes up the search text control focus. Disable for searching.
                    if (!this.IsSearchEntryFocused)
                    {
                        this.IsIndicatorRunning = true;
                    }

                    await this.RefreshDataBySubType();
                    await this.DesignGrid();
                    if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(() => this.ExtraInfoItems.Clear());
                    }
                    else
                    {
                        await this.FindSelectedItemAsync();
                    }
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
                // The activity indicator control messes up the search text control focus. Disable for searching.
                if (!this.IsSearchEntryFocused)
                {
                    this.IsIndicatorRunning = false;
                }

                this.IsDataLoading = false;
            }
        }

        /// <summary>
        ///     Subscribe key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), this.KeyUpHandlerCallback);
        }

        /// <summary>
        ///     Key up handler call back.
        /// </summary>
        /// <param name="keyPressed"><see cref="string" /> Key pressed value.</param>
        private async void KeyUpHandlerCallback(string keyPressed)
        {
            if (this.IsDataLoading)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
                switch (keycode)
                {
                    case Keycode.Enter:
                        if (!this.IsSearchEntryFocused)
                        {
                            await KeyUpEnterHandler();
                        }

                        break;

                    case Keycode.F1:
                        await KeyUpF1Handler();
                        break;

                    case Keycode.F4 when !this.SysEnv.SysSettings.IsDisconnectedMode:
                        if (!this.IsSearchEntryFocused)
                        {
                            await KeyUpF4HandlerReleaseOrder();
                        }

                        break;

                    case Keycode.F5:
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
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            }

            return;

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
                    this.ListViewMainActivity.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
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
                    this.ListViewMainActivity.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpF1Handler()
            {
                if (await this.SaveChanges())
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    await this.ScannerDisable();
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
                    this.ActHdr.AssignRefnoFromActivity(this.ListViewMainActivity.SelectedItem);
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
                        switch (this.ActSubType)
                        {
                            // Print labels after release
                            // Lekker Foods wants to be ask to print labels after the order has been released.
                            // There is no opportunity to reprint after the release.
                            case SysCommon.ActSubTypes.Picking:
                            case SysCommon.ActSubTypes.PickingSingle:
                            {
                                // Shipping labels
                                if (this.SysEnv.SysSettings.CanPrintShippingLabel &&
                                    !this.SysEnv.SysSettings.IsDisconnectedMode)
                                {
                                    await PrintLabelsAfterRelease(labelQuantity);
                                }

                                if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                                {
                                    await this.RefreshDataBySubType();
                                }
                                else
                                {
                                    _ = this.ActHdr.ListHeaders.Remove(this.ListViewMainActivity.SelectedItem);
                                    await this.DesignGrid();
                                }

                                break;
                            }

                            case SysCommon.ActSubTypes.Adjustment:
                            {
                                // <WO ID="14748" comment="Printing after deboxing" />
                                // If DEBOX, prompt for label
                                if (this.ActHdr is AdjActivity { AdjSettings: { AdjustmentType: SysCommon.AdjTypes.Debox } })
                                {
                                    var form = new FormDialogContainer(EDialogTypes.ActivityDeboxLabelCount, this.ActSubType);

                                    if (await Device.InvokeOnMainThreadAsync(async () =>
                                            await form.DisplayControl.ShowDialog() == DialogResult.OK))
                                    {
                                        var doc = await form.GetXml();
                                        await this.ActHdr.PrintLabel(doc);
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
                finally
                {
                    await this.FindSelectedItemAsync();
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                async Task PrintLabelsAfterRelease(SLabelQuantity labelQuantity)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle || this.ActSubType == SysCommon.ActSubTypes.Picking)
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
                                var request = await form.GetXml();

                                // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                // If itm_refno is zero then no line is updated as printed.
                                request.Element("params")
                                    ?.Element("whs_id")
                                    ?.AddAfterSelf(
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
                                var request = await form.GetXml();

                                // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                // If itm_refno is zero then no line is updated as printed.
                                request.Element("params")
                                    ?.Element("whs_id")
                                    ?.AddAfterSelf(
                                    new XElement("itm_refno", 0));
                                await this.ActHdr.PrintLabel(request);
                            }
                        }
                    }
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
                            var result = await form.GetXml();
                            if (result.Root == null)
                            {
                                throw new Exception("Unable to get XML from form.");
                            }

                            if (result.Root.Element("LocationMapRefno") == null)
                            {
                                throw new Exception("Unable to get LocationMapRefno from form.");
                            }

                            var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                int.Parse(result.Root.Element("LocationMapRefno") !.Value));
                            locMap.CurrLocMap = sLocMap;

                            this.ActHdr.XmlDataCriteria = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + "<whs_id>" + sLocMap.WhsFkRefno + "</whs_id>"
                                + "<sect_refno>" + sLocMap.SectFkRefno + "</sect_refno>"
                                + "</params>");
                        }

                        await this.RefreshDataBySubType();
                    }
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpEnterHandler()
            {
                this.ActHdr.AssignRefnoFromActivity(this.ListViewMainActivity.SelectedItem);
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
                    _ = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                    await this.FindSelectedItemAsync(this.ActHdr.HdrRefno);
                }
            }
        }

        /// <summary>
        ///     Ask to confirm to exit.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task<bool> AskConfirmExit()
        {
            var isReadyToClose = !this.IsChangesMade;
            await this.DisplayAlert(this.ActHdr.GetControlDescription(), "Confirm exit?", "Yes", "No")
                .ContinueWith(result =>
                {
                    if (!result.Result)
                    {
                        isReadyToClose = false;
                    }
                });

            return isReadyToClose;
        }

        /// <summary>
        ///     Bar code event handler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Bar code information.</param>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Turn off the scanner because we do not want more events to fire until we are finished with
                    // the execution path.
                    await this.ScannerDisable();
                    if (scanArgs == null)
                    {
                        const string msg = "Scanner event fired but no data was received.";
                        await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                            this.ActHdr.GetControlDescription(),
                            msg,
                            Resource.OK));
                        throw new TaskCanceledException();
                    }

                    this.BarParse = scanArgs.ParseInfo;
                    if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                    {
                        // this.BarParse.ActFKRefno e.g. format is 'OE:12345678'.
                        // EnableScanning at 3th position for the foreign key reference number.
                        await BarCodeHandlerFkRefno(this.BarParse.ActFKRefno);
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
                catch (TaskCanceledException)
                {
                    // Ignore.
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                }
            });

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerFkRefno(string actHdrStringFkRefno)
            {
                // Remember that the headers have been filtered by the login warehouse.
                var selectedActivity = this.ActHdr.ListHeaders.FirstOrDefault(
                    activity => Convert.ToString(activity.GetType().GetProperty(
                                    this.ActHdr.GetFkRefnoIdx())
                                    ?.GetValue(activity)) == actHdrStringFkRefno);
                if (selectedActivity != null)
                {
                    this.ActHdr.AssignRefnoFromActivity(selectedActivity);
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var form = new FormDialogContainer(
                            EDialogTypes.ActivityItem,
                            this.ActHdr,
                            this.ActSubType);
                        this.IsChangesMade = false;
                        _ = await form.DisplayControl.ShowDialog();
                    });
                    await this.FindSelectedItemAsync(this.ActHdr.HdrRefno);
                }
                else
                {
                    this.ActHdr.HdrFkRefno = 0;
                    this.ActHdr.HdrRefno = 0;
                    throw new MessageOnlyException(
                        $"Unable to find reference # {actHdrStringFkRefno}. Perhaps change your login warehouse.");
                }
            }
        }

        /// <summary>
        ///     Handle double click on list view item.
        ///     Using a tap gesture recognizer in the designGrid would be optimal, however there is
        ///     currently a known bug that prevents selectedItemChanged from firing.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e">The e<see cref="ItemTappedEventArgs" /> Item tapped event arguments.</param>
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
                        await this.ScannerDisable();
                        this.ActHdr.AssignRefnoFromActivity(this.ListViewMainActivity.SelectedItem);
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
                            await this.FindSelectedItemAsync(this.ActHdr.HdrRefno);
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                            ? msgEx.Message
                            : msgEx.Message["MSGBOX-".Length..];
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
        ///     Call refresh data, unique to the activity sub type.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
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

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task RefreshData()
            {
                // NOTE: So far all subtypes will be refreshed.
                try
                {
                    this.ActHdr.RegLayerListenerStatOut(this);
                    this.ActHdr.RegLayerListenerProgOut(this);
                    await this.ActHdr.Refresh(this.ActHdr.XmlDataCriteria);
                    this.ActHdr.ListHeaders = await this.ActHdr.GetActivityList(false);
                    await this.DesignGrid();
                }
                finally
                {
                    this.ActHdr.UnregLayerListenerStatOut(this);
                    this.ActHdr.UnregLayerListenerProgOut(this);
                }
            }
        }

        /// <summary>
        ///     Design the main activity grid / list.
        /// </summary>
        /// <remarks>Search text is applied as a filter to list of headers.</remarks>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task DesignGrid()
        {
            // Bind the filtered activity items to the list view.
            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.MainActivityItems =
                    this.ActHdr.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);

                if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                {
                    this.MainActivityItems = this.ActHdr.Search(this.txtSearchHdr.Text);
                }

                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            });
        }

        /// <summary>
        ///     Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActHdr.ListHeaders.Count > 0 && this.MainActivityItems.Count > 0)
            {
                // Iterate through the list of headers looking for a foreign key match.
                // Faster than LINQ.
                this.ActHdr.SelectedListItemIdx = 0;
                object selectedItem = null;
                foreach (var hdr in this.ActHdr.ListHeaders)
                {
                    this.ActHdr.AssignRefnoFromActivity(hdr);
                    if (this.SelectedActivityFkRefno == this.ActHdr.HdrFkRefno)
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
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActHdr.SelectedListItemIdx];
                    });
                    await this.ActHdr.SelectActivity(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActHdr.SelectActivity(selectedItem);
                    Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = this.ActHdr.SelectedActivity; });
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
                Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = null; });
            }
        }

        /// <summary>
        ///     Find the selected item in the data list.
        /// </summary>
        /// <param name="activityRefno"> Activity reference number to find.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task FindSelectedItemAsync(int activityRefno)
        {
            if (this.ActHdr.ListHeaders.Count > 0 && this.MainActivityItems.Count > 0)
            {
                // Iterate through the list of headers looking for a foreign key match.
                // Faster than LINQ.
                this.ActHdr.SelectedListItemIdx = 0;
                object selectedItem = null;
                foreach (var hdr in this.ActHdr.ListHeaders)
                {
                    this.ActHdr.AssignRefnoFromActivity(hdr);
                    if (activityRefno == this.ActHdr.HdrRefno)
                    {
                        selectedItem = hdr;
                        this.SelectedActivityFkRefno = this.ActHdr.HdrFkRefno;
                        break;
                    }

                    this.ActHdr.SelectedListItemIdx++;
                }

                // Select the first header if no match was found, else select the item.
                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActHdr.SelectedListItemIdx = 0;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActHdr.SelectedListItemIdx];
                    });
                    await this.ActHdr.SelectActivity(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActHdr.SelectActivity(selectedItem);
                    Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = this.ActHdr.SelectedActivity; });
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
                Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = null; });
            }
        }

        /// <summary>
        ///     Add item command button event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdAdd_Click(object sender, EventArgs e)
        {
            if (this.IsDataLoading)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();

                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire if a
                // command button is the next control rather than just giving the button focus.
                var activityRefno = await this.ActHdr.Insert(null);
                if (activityRefno > 0)
                {
                    await this.LoadAndBindData();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];

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
            }
        }

        /// <summary>
        ///     Perform key up enter picking single.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
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
                await this.FindSelectedItemAsync(this.ActHdr.HdrRefno);
            }
        }

        /// <summary>
        ///     The ListViewMainActivity_ItemSelected.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Select item changed event arguments.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActHdr.SelectedListItemIdx = e.SelectedItemIndex;
            await this.ActHdr.SelectActivity(e.SelectedItem);
            this.SelectedActivityFkRefno = this.ActHdr.HdrFkRefno;
            await DisplayData();
            return;

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
                            DisplayDataTransferTo();

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
                    }

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                });
                return;

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataAdjustments()
                {
                    if (this.ActHdr.SelectedActivity is Adjustment adjustment)
                    {
                        this.ExtraInfoItems.Add("Adjustment type: " + adjustment.ActivityAdjustmentTypeDescription);
                        this.ExtraInfoItems.Add("Warehouse:  " + adjustment.LocationMapWarehouseName);
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
                        const int lineLength = 30;
                        var lines = SysCommon.WordWrap2List(
                            count.ActivityCountDescription.TrimEnd(),
                            lineLength);

                        foreach (var line in lines)
                        {
                            this.ExtraInfoItems.Add(line);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataPhysicalCounts()
                {
                    if (this.ActHdr.SelectedActivity is Count count)
                    {
                        const int lineLength = 30;
                        var lines = SysCommon.WordWrap2List(
                            count.ActivityCountDescription.TrimEnd(),
                            lineLength);

                        foreach (var line in lines)
                        {
                            this.ExtraInfoItems.Add(line);
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
                        this.ExtraInfoItems.Add(
                            $"Order warehouse:   {picking.LocationMapWarehouseFKRefno} - {picking.LocationMapWarehouseName}");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataPickingSingle()
                {
                    if (this.ActHdr.SelectedActivity is Picking pickingSingle)
                    {
                        this.ExtraInfoItems.Add("Customer: " + pickingSingle.ActivityPickingCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " + pickingSingle.ActivityPickingCustomerShipToInfo);
                        this.ExtraInfoItems.Add(
                            $"Order warehouse:   {pickingSingle.LocationMapWarehouseFKRefno} - {pickingSingle.LocationMapWarehouseName}");
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
                        this.ExtraInfoItems.Add("Customer: " +
                                                pickingReleaseByOrder.ActivityPickingCustomerContactName);
                        this.ExtraInfoItems.Add("Ship to:   " +
                                                pickingReleaseByOrder.ActivityPickingCustomerShipToInfo);
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
                        this.ExtraInfoItems.Add("To Warehouse:  " + transfer.LocationMapWarehouseDescription);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void DisplayDataTransferTo()
                {
                    if (this.ActHdr.SelectedActivity is Transfer transfer)
                    {
                        this.ExtraInfoItems.Add(transfer.ActivityTransferDescription);
                    }
                }
            }
        }

        /// <summary>
        ///     Text box search item focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void TxtSearchHdr_Focused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = true;
            this.Timer.Start();
        }

        /// <summary>
        ///     Text box search item unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void TxtSearchHdr_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsSearchEntryFocused = false;
            this.Timer.Stop();
        }

        /// <summary>
        ///     Text search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Timer was started in the focused event handler.
            // Timer was stopped in the unfocused event handler.
            // Timer was stopped in the timer tick call back event.
            // Call timer start when the text changes to invoke the call back method passed to the timer when appropriate.
            // If the timer is already started then the call to start will be ignored, but the call back will be invoked.
            this.Timer.Start();
        }

        /// <summary>
        ///     Option menu click handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsDataLoading)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();
                var result = await this.DisplayActionSheet(
                    "Activity Options",
                    "Close",
                    null,
                    this.OptionsMenuItems.ToArray());

                if (result == this.FilterText && this.FilterIsEnabled)
                {
                    var dlgType = this.ActSubType switch
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
                        _ => throw new NotImplementedException("eActivitySubType")
                    };

                    var form = new FormDialogContainer(dlgType, this.ActSubType);
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            await form.GetXml();
                            if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                            {
                                this.MainActivityItems = this.ActHdr.Search(this.txtSearchHdr.Text);
                            }

                            this.LoadAndBindData().FireAndForgetSafeAsync();
                        }
                    });

                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

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
            }
        }
    }
}