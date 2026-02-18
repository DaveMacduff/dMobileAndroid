// <copyright file="ActivityDetail.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using dMobile.Resources.Types.Headers;
    using dMobile.Resources.Types.Items;
    using Forms;
    using Resources.Enums;
    using Resources.Types.Details;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Activity detail control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetail : IAppArea, IDataLayerEventListener
    {
        private bool cmdAddSkidIsEnabled;

        private string cmdAddSkidText = string.Empty;

        private bool cmdLocationIsEnabled;

        private string cmdLocationText = string.Empty;

        private bool cmdSelectSkidIsEnabled;

        private string cmdSelectSkidText = string.Empty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityDetail" /> class.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem" /> Activity item.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity subtypes.</param>
        public ActivityDetail(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.CmdLocationIsEnabled = false;
            this.CmdSelectSkidIsEnabled = false;
            this.CmdAddSkidIsEnabled = false;
            this.cmdAddDetail.IsEnabled = false;

            this.CmdLocationText = "Set Location";
            this.CmdSelectSkidText = "Select Skid";
            this.CmdAddSkidText = "Add Skid";
            this.cmdAddDetail.Text = "Add Detail";

            this.MainActivityItems = new ObservableCollection<object>();
            this.ExtraInfoItems = new ObservableCollection<string>();

            this.SysEnv = SysEnvironment.GetInstance();
            this.SysEnv.SysSettings.IsNewActivityDetailsAdded = false;

            this.LocMap = LocationMap.GetInstance();

            actItem.ActHdr.SkidInfo ??= new SysSkidInfo();
            this.ActSubType = actSubTypes;
            this.ActDetail = ActDetailFacade.GetActDetail(actItem, actSubTypes);
            this.ActDetail.ActItem = actItem;

            this.Title = this.ActDetail.GetControlDescription();
            this.DataCollector = new DataCollector(this.ActSubType, actItem);
            this.Timer = new SysTimer(this.TimerTickCallback);
            this.IsFirstAppearance = true;
        }

        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        private SysEnvironment SysEnv { get; }

        private LocationMap LocMap { get; }

        private SysCommon.ActSubTypes ActSubType { get; }

        private ActDetail ActDetail { get; }

        private bool IsShowDataValidMsg { get; set; } = true;

        private int SelectedActivityDetailFkRefno { get; set; }

        private List<string> OptionsMenuItems { get; } = new List<string>();

        private ObservableCollection<string> ExtraInfoItems { get; }

        private ObservableCollection<object> MainActivityItems { get; set; }

        private bool IsSearchEntryFocused { get; set; }

        private bool IsDataLoading { get; set; }

        private DataCollector DataCollector { get; set; }

        /// <summary>
        ///     Gets or sets the command location text.
        /// </summary>
        private string CmdLocationText
        {
            get => this.cmdLocationText;
            set
            {
                this.cmdLocationText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether command location is enabled.
        /// </summary>
        private bool CmdLocationIsEnabled
        {
            get => this.cmdLocationIsEnabled;
            set
            {
                this.cmdLocationIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the command select skid text.
        /// </summary>
        private string CmdSelectSkidText
        {
            get => this.cmdSelectSkidText;
            set
            {
                this.cmdSelectSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command select skid is enabled.
        /// </summary>
        private bool CmdSelectSkidIsEnabled
        {
            get => this.cmdSelectSkidIsEnabled;
            set
            {
                this.cmdSelectSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the command add skid text.
        /// </summary>
        private string CmdAddSkidText
        {
            get => this.cmdAddSkidText;
            set
            {
                this.cmdAddSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command add skid is enabled.
        /// </summary>
        private bool CmdAddSkidIsEnabled
        {
            get => this.cmdAddSkidIsEnabled;
            set
            {
                this.cmdAddSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        private SysTimer Timer { get; }

        private string SearchTextOrig { get; set; } = string.Empty;

        private bool IsFirstAppearance { get; set; }

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
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (!await this.IsReadyToSave())
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catchall
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException(
                            "Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }

                if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                    this.SysEnv.SysSettings.CanPrintProductLabelReceiving &&
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                    !this.SysEnv.SysSettings.IsDisconnectedMode &&
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch && this.ActDetail.ListDetails.Count > 0)
                {
                    if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                "Print Label",
                                "Print Product Labels",
                                "Yes",
                                "No")))
                    {
                        await PrintBatchedProductLabels();
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return isSaveComplete;

            async Task PrintBatchedProductLabels()
            {
                // For each detail marked for printing
                var detail = this.ActDetail.ListDetails;

                for (var rowIdx = 0; rowIdx <= detail.Count - 1; rowIdx++)
                {
                    if (Convert.ToBoolean(
                            detail[rowIdx]
                                .GetType()
                                .GetProperty(
                                    "IsBatchProductLabel")
                                ?.GetValue(detail[rowIdx])))
                    {
                        var request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<serial>" + detail[rowIdx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")
                                ?.GetValue(detail[rowIdx]) + "</serial>" +
                            "<pdate>" + detail[rowIdx].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")
                                ?.GetValue(detail[rowIdx]) + "</pdate>" +
                            "<lot_no>" + detail[rowIdx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")
                                ?.GetValue(detail[rowIdx]) + "</lot_no>" +
                            "<bdate>" + detail[rowIdx].GetType().GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")
                                ?.GetValue(detail[rowIdx]) + "</bdate>" +
                            "<variant_code>" + detail[rowIdx].GetType().GetProperty("ProductSchemeVariantCode")
                                ?.GetValue(detail[rowIdx]) + "</variant_code>" +
                            "<variant_no>" + detail[rowIdx].GetType().GetProperty("ProductSchemeVariantNo")
                                ?.GetValue(detail[rowIdx]) + "</variant_no>" +
                            "<alt_qty>" + detail[rowIdx].GetType().GetProperty("ActivityDetailQuantity")
                                ?.GetValue(detail[rowIdx]) + "</alt_qty>" +
                            "<wt_qty>" + detail[rowIdx].GetType().GetProperty("ActivityDetailBaseQuantity")
                                ?.GetValue(detail[rowIdx]) + "</wt_qty>" +
                            "<base_qty>" + detail[rowIdx].GetType().GetProperty("ActivityDetailWeightQuantity")
                                ?.GetValue(detail[rowIdx]) + "</base_qty>" +
                            "</params>");

                        // CreateSubmitProductLabelXML assumes actItem and
                        // Loc.sSelectedLocationMap is correct
                        var sendReq = await this.ActDetail.CreateSubmitProductLabelXml(request);
                        try
                        {
                            _ = SysCommon.SubmitComm(sendReq);
                            detail[rowIdx]
                                .GetType()
                                .GetProperty("IsBatchProductLabel")
                                ?.SetValue(detail[rowIdx], false);
                        }
                        catch (MessageOnlyException msgEx)
                        {
                            var msg = SysCommon.GetMessageFromMessageException(msgEx);
                            await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        GetControlDescription(),
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
                }
            }
        }

        /// <summary>
        ///     Cancel any pending activity item changes.
        /// </summary>
        /// <returns>True if the pending changes where canceled, else false.</returns>
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
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
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
        ///     Get the activity detail return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity detail return information.</returns>
        public async Task<XDocument> GetXml()
        {
            var skidFillQty = new SFillQuantity
            {
                Quantity = 0,
                BaseQuantity = 0,
                WeightQuantity = 0
            };
            var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
            {
                skidFillQty = this.ActDetail.GetTotalSkidQuantityFromListDetail(
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid);
            }

            return XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "<qty>" + fillQty.Quantity + "</qty>" +
                "<base_qty>" + fillQty.BaseQuantity + "</base_qty>" +
                "<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>" +
                "<skid_qty>" + skidFillQty.Quantity + "</skid_qty>" +
                "</params>");
        }

        /// <summary>
        ///     Data layer controller event handler. Used to handle status information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher" />.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs" />.</param>
        public void OnLayerStatOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        ///     Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher" />.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs" />.</param>
        public void OnLayerProgressOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        ///     Timer tick call back.
        /// </summary>
        public void TimerTickCallback()
        {
            this.Timer.Stop();
            if (this.IsSearchEntryFocused &&
                this.txtSearchDetail.Text != this.SearchTextOrig)
            {
                this.SearchTextOrig = this.txtSearchDetail.Text;
                ////this.LoadAndBindData().FireAndForgetSafeAsync();
                this.DesignGrid().FireAndForgetSafeAsync();
            }

            // Did not start the timer again here because the timer is started in the OnTextChange event.
        }

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformCancelChanges);

            return true;

            async void PerformCancelChanges()
            {
                if (await this.CancelChanges())
                {
                    // Set the dialog result to OK to force the line item total to recalculate.
                    // Even though the back button is meant to be a cancel we need to update
                    // because data still could have changed.
                    this.DialogResult = DialogResult.OK;
                    BackButtonCallback();
                }
            }

            async void BackButtonCallback()
            {
                this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                this.SysEnv.SysSettings.IsNewActivityDetailsAdded = false;
                await this.ScannerDisable();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }

        /// <summary>
        ///     Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string" /> Property name to check if changed.</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName == nameof(this.CmdLocationText) ||
                propertyName == nameof(this.CmdLocationIsEnabled) ||
                propertyName == nameof(this.CmdSelectSkidText) ||
                propertyName == nameof(this.CmdSelectSkidIsEnabled) ||
                propertyName == nameof(this.CmdAddSkidText) ||
                propertyName == nameof(this.CmdAddSkidIsEnabled))
            {
                PopulateOptionsMenu();
            }

            if (propertyName == nameof(this.CmdSelectSkidText))
            {
                if (this.CmdSelectSkidText != "Select Skid")
                {
                    this.lblSkidNumber.Text = this.CmdSelectSkidText;
                    this.lblSkidNumber.IsVisible = true;
                }
                else
                {
                    this.lblSkidNumber.Text = string.Empty;
                    this.lblSkidNumber.IsVisible = false;
                }
            }

            return;

            void PopulateOptionsMenu()
            {
                this.OptionsMenuItems.Clear();

                if (this.CmdLocationIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdLocationText);
                }

                if (this.CmdSelectSkidIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdSelectSkidText);
                }

                if (this.CmdAddSkidIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdAddSkidText);
                }
            }
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.SubscribeKeyUpHandlers();

            if (this.IsFirstAppearance)
            {
                this.IsFirstAppearance = false;
                await this.LoadAndBindData();
                await this.ScannerEnable();
            }

            if (!this.IsSearchEntryFocused)
            {
                this.ListViewMainActivity.Focus();
                await this.FindSelectedItemAsync();
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static string GetControlDescription()
        {
            return "Activity Detail";
        }

        private async Task LoadAndBindData()
        {
            try
            {
                this.cmdAddDetail.IsEnabled = true;
                if (!this.IsDataLoading)
                {
                    this.IsDataLoading = true;

                    // The activity indicator control messes up the search text control focus. Disable for searching.
                    if (!this.IsSearchEntryFocused)
                    {
                        this.IsIndicatorRunning = true;
                    }

                    await this.RefreshBySubType();
                    await this.DesignGrid();

                    // No need to invoke DisplayData() selected item will do so, unless the list is empty.
                    if (this.MainActivityItems == null ||
                        this.MainActivityItems.Count == 0)
                    {
                        await this.DisplayDataItem();
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

        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(
                this,
                nameof(EventTypes.OnKeyUp),
                this.KeyUpHandlerCallback);
        }

        private async void KeyUpHandlerCallback(string keyPressed)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();

                // TODO: Verify key codes.
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                switch (keycode)
                {
                    case Keycode.Del:
                    case Keycode.Clear:
                    {
                        if (!this.IsSearchEntryFocused)
                        {
                            await AppearingDelKey();
                        }

                        break;
                    }

                    case Keycode.F1:
                        await AppearingF1Key();
                        break;

                    case Keycode.F3:
                        await AppearingF3Key();
                        break;

                    case Keycode.F4:
                        await AppearingF4Key();
                        break;

                    case Keycode.DpadDown:
                        AppearingDpadDownKey();
                        break;

                    case Keycode.DpadUp:
                        AppearingDpadUp();
                        break;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
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

            async Task AppearingDelKey()
            {
                try
                {
                    // Check if there is an item in the data grid selected to delete.
                    IsItemSelected();

                    // Ask if the deletion is a entire skid or item.
                    var isDelItem = false;
                    var isDelSkid = false;
                    if (string.IsNullOrEmpty(this.ActDetail.SchemeData.LogisticUnitSkidNumber))
                    {
                        if (await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                "Do you wish to delete this single detail?",
                                "Yes",
                                "No"))
                        {
                            isDelItem = true;
                        }
                    }
                    else
                    {
                        var msg = "Do you wish to delete all detail belonging to this skid" +
                                  $" {this.ActDetail.SchemeData.LogisticUnitSkidNumber}?";
                        if (await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                "Yes",
                                "No"))
                        {
                            isDelSkid = true;
                        }
                        else
                        {
                            if (await this.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    "Do you wish to delete this single detail?",
                                    "Yes",
                                    "No"))
                            {
                                isDelItem = true;
                            }
                        }
                    }

                    if (isDelItem)
                    {
                        await AppearingDelKeyDetail();
                    }
                    else if (isDelSkid)
                    {
                        await AppearingDelKeySkid();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                        ? msgEx.Message
                        : msgEx.Message["MSGBOX-".Length..];
                    await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
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

                return;

                void IsItemSelected()
                {
                    var isItemSelected = this.ListViewMainActivity.SelectedItem != null &&
                                         Convert.ToInt32(
                                             this.MainActivityItems.IndexOf(
                                                 this.ListViewMainActivity.SelectedItem)) >=
                                         0;

                    if (!isItemSelected)
                    {
                        throw new MessageOnlyException("Nothing selected to delete.");
                    }
                }

                async Task AppearingDelKeyDetail()
                {
                    // Read the selected detail.
                    await this.ActDetail.AssignActivityDetail(
                        (Resources.Types.Details.ActivityDetail)this.ListViewMainActivity.SelectedItem);
                    this.ActDetail.ActStatus = SysCommon.ActStatus.Deleted;

                    // Submit the request, and delete it from the data table.
                    _ = await this.ActDetail.Delete();

                    // Update the activity line item quantities.
                    this.ActDetail.ActItem.AddToQuantityInActivityItem(
                        this.ActDetail.ActItem.SelectedItem,
                        this.ActDetail.Quantity * -1,
                        this.ActDetail.BaseQuantity * -1,
                        this.ActDetail.WeightQuantity * -1);

                    // Update the table item quantity.
                    await this.ActDetail.ActItem.UpdateItemTableQuantity(this.ActDetail.ActItem.SelectedItem);
                    await this.LoadAndBindData();
                }

                async Task AppearingDelKeySkid()
                {
                    // Determine the selected detail skid code.
                    var currIdx = this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem);
                    var skidCode = Convert.ToString(
                        this.ActDetail.ListDetails[currIdx]
                            .GetType()
                            .GetProperty(
                                "ActivityDetailLogisticUnitSkidNumber")
                            ?.GetValue(
                                this.ActDetail.ListDetails[currIdx]));
                    var skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(skidCode);

                    if (skidInfo.CurrSkid.SkidFkRefno == 0)
                    {
                        throw new Exception(
                            $"Unable to determine the skid '{skidCode}'" +
                            " information to delete.");
                    }

                    // Submit the request, and delete it from the data table.
                    await this.ActDetail.DeleteSkid(skidInfo);
                    var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                    this.ActDetail.ActItem.ReplaceQuantityInActivityItem(
                        this.ActDetail.ActItem.SelectedItem,
                        fillQty.Quantity,
                        fillQty.BaseQuantity,
                        fillQty.WeightQuantity);
                    await this.ActDetail.ActItem.UpdateItemTableQuantity(
                        this.ActDetail.ActItem.SelectedItem);
                    this.ActDetail.ActItem.ListItems[this.ActDetail.ActItem.SelectedListIdx] =
                        this.ActDetail.ActItem.SelectedItem;
                    await this.LoadAndBindData();
                }
            }

            async Task AppearingF1Key()
            {
                try
                {
                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;

                        // TODO: DEV-5619
                        // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                        this.SysEnv.SysSettings.IsNewActivityDetailsAdded = false;
                        await this.ScannerDisable();
                        await this.Close();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
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

            async Task AppearingF3Key()
            {
                // TODO: DEV-5619
                // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile
                try
                {
                    var qty = await this.ActDetail.GetTotalQuantityFromListDetail();
                    var msg = $"Total:\r{qty.Quantity}" +
                        $" {this.ActDetail.ActItem.ProdInfo.UnitCode}" +
                        $" @ {qty.WeightQuantity}" +
                        $" {this.ActDetail.ActItem.ProdInfo.WeightUnitCode}";

                    if (!string.IsNullOrEmpty(msg))
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                "OK"));
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
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
                    _ = this.ListViewMainActivity.Focus();
                }
            }

            async Task AppearingF4Key()
            {
                try
                {
                    await CheckIfActivityCanBeConfirmed();
                    await MarkDetailAsConfirmed();
                    await this.DisplayDataItem();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errorReport = new FormErrorReport(this.ActSubType, ex);
                            _ = await errorReport.ShowDialog();
                        });
                }

                return;

                Task MarkDetailAsConfirmed()
                {
                    if (this.ListViewMainActivity.SelectedItem is PickingDetail pickDetail)
                    {
                        var idx = this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem);
                        pickDetail = (PickingDetail)this.ActDetail.ListDetails[idx];
                        if (pickDetail.ActivityDetailIsConfirmed)
                        {
                            pickDetail.ActivityDetailIsConfirmed = false;
                            pickDetail.ActivityDetailIsConfirmedString = " ";
                        }
                        else
                        {
                            pickDetail.ActivityDetailIsConfirmed = true;
                            pickDetail.ActivityDetailIsConfirmedString = "C";
                        }

                        ((PickingActDetail)this.ActDetail).UpdateConfirmedStatus(idx);

                        this.MainActivityItems[idx] = pickDetail;
                        Device.BeginInvokeOnMainThread(() =>
                            {
                                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                                this.ListViewMainActivity.ScrollTo(
                                    this.ListViewMainActivity.SelectedItem,
                                    ScrollToPosition.MakeVisible,
                                    false);
                            });
                    }
                    else
                    {
                        if (this.ListViewMainActivity.SelectedItem is ReturnDetail returnDetail)
                        {
                            var idx = this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem);
                            returnDetail = (ReturnDetail)this.ActDetail.ListDetails[idx];
                            if (returnDetail.ActivityDetailIsConfirmed)
                            {
                                returnDetail.ActivityDetailIsConfirmed = false;
                                returnDetail.ActivityDetailIsConfirmedString = " ";
                            }
                            else
                            {
                                returnDetail.ActivityDetailIsConfirmed = true;
                                returnDetail.ActivityDetailIsConfirmedString = "C";
                            }

                            this.MainActivityItems[idx] = returnDetail;
                            Device.BeginInvokeOnMainThread(() =>
                                {
                                    this.ListViewMainActivity.SelectedItem = returnDetail;
                                    this.ListViewMainActivity.ScrollTo(
                                        this.ListViewMainActivity.SelectedItem,
                                        ScrollToPosition.MakeVisible,
                                        false);
                                });
                        }
                        else if (this.ListViewMainActivity.SelectedItem is TransferDetail transferDetail)
                        {
                            transferDetail = (TransferDetail)this.ActDetail.ListDetails[
                                this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)];
                            ////transferDetail.ActivityDetailIsConfirmed = true;
                            ////transferDetail.ActivityDetailIsConfirmedString = "C";
                        }
                    }

                    return Task.CompletedTask;
                }

                async Task CheckIfActivityCanBeConfirmed()
                {
                    var xmlString = new StringBuilder();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = false
                    };
                    string action;

                    if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    {
                        action = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                            <imclient>
                                <command>
                                    <action>SUBMIT SALES ORDER DETAIL CONFIRM</action>
                                </command>
                                {await SysCommon.GetRequestDefaults(false)}
                                <refno>{this.ActDetail.DetailFkRefno}</refno>
                            </imclient>";
                    }
                    else if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                    {
                        action = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                            <imclient>
                                <command>
                                    <action>SUBMIT TRANSFER FROM DETAIL CONFIRM</action>
                                </command>
                                {await SysCommon.GetRequestDefaults(false)}
                                <refno>{this.ActDetail.DetailFkRefno}</refno>
                            </imclient>";
                    }
                    else if (this.ActSubType == SysCommon.ActSubTypes.TransferTo)
                    {
                        action = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                            <imclient>
                                <command>
                                    <action>SUBMIT TRANSFER TO DETAIL CONFIRM</action>
                                </command>
                                {await SysCommon.GetRequestDefaults(false)}
                                <refno>{this.ActDetail.DetailFkRefno}</refno>
                            </imclient>";
                    }
                    else
                    {
                        throw new MessageOnlyException(
                            "Confirmations are not supported with this activity.");
                    }

                    xmlString.Append(action);
                    _ = XmlWriter.Create(xmlString, settings);
                    _ = SysCommon.SubmitComm(xmlString.ToString());
                }
            }

            void AppearingDpadDownKey()
            {
                if (this.ListViewMainActivity.SelectedItem != null &&
                    Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) <
                        this.MainActivityItems.Count - 1)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem) + 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }

            void AppearingDpadUp()
            {
                if (this.ListViewMainActivity.SelectedItem != null &&
                    Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) > 0)
                {
                    this.ListViewMainActivity.SelectedItem =
                        this.MainActivityItems[this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem) - 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }
        }

        private void TxtSearchDetail_Focused(object sender, EventArgs e)
        {
            this.IsSearchEntryFocused = true;
            this.Timer.Start();
        }

        private void TxtSearchDetail_Unfocused(object sender, EventArgs e)
        {
            this.IsSearchEntryFocused = false;
            this.Timer.Stop();
        }

        private void TxtSearchDetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Timer was started in the focused event handler.
            // Timer was stopped in the unfocused event handler.
            // Timer was stopped in the timer tick call back event.
            // Start the timer when the text changes. This will reset the timer and force a load data.
            this.Timer.Start();
        }

        private async void ListViewMainActivity_ItemSelected(
            object sender,
            SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.AssignActivityDetail(e.SelectedItem);
            this.ActDetail.SelectedListIdx = e.SelectedItemIndex;
            this.SelectedActivityDetailFkRefno = this.ActDetail.DetailFkRefno;
            await this.DisplayDataItem();
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
                {
                    try
                    {
                        await this.ScannerDisable();
                        if (scanArgs == null)
                        {
                            const string msg = "Scanner event fired but no data was received.";
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    msg,
                                    Resource.OK));
                            throw new TaskCanceledException();
                        }

                        var parse = scanArgs.ParseInfo;
                        var isLocationBarCodeScanned = !string.IsNullOrEmpty(parse.LocationIdentifier);
                        var isProductScanned = !string.IsNullOrEmpty(parse.ProductIdentifier);
                        var isFkRefnoScanned = !string.IsNullOrEmpty(parse.ActFkRefno);
                        var isFkItemRefnoScanned = !string.IsNullOrEmpty(parse.ActItemFkRefno);
                        var isFkDetailRefnoScanned = !string.IsNullOrEmpty(parse.ActDetailFkRefno);
                        var isSkidScanned = !string.IsNullOrEmpty(parse.Sscc18);

                        // GLOBAL LOCATION IDENTIFIER SCANNED
                        if (isLocationBarCodeScanned)
                        {
                            await BarCodeHandlerLocationIdentifier(parse);
                        }
                        else if (isFkRefnoScanned)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    "Scanning a header reference # is not supported here.",
                                    "OK"));
                        }
                        else if (isFkItemRefnoScanned)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    "Scanning an item reference # is not supported here.",
                                    "OK"));
                        }
                        else if (isFkDetailRefnoScanned)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    "Scanning a detail reference # is not supported here.",
                                    "OK"));
                        }
                        else if (isProductScanned)
                        {
                            await HandleProductScanned(parse);
                        }
                        else if (isSkidScanned)
                        {
                            await HandleSkidScanned();
                        }
                        else
                        {
                            throw new MessageOnlyException("Scanned data is not recognized.");
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // This exception is also caught if the scan event arguments are null.
                    }
                    catch (IsReadyToApplyException)
                    {
                    }
                    catch (GatherProductDataException ex)
                    {
                        if (!ex.SuppressError)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                                    _ = await errRpt.ShowDialog();
                                });
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
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
                        if (scanArgs != null)
                        {
                            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                        }

                        await this.ScannerEnable();
                    }
                });

            return;

            async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
            {
                var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (currSkid.SkidFkRefno != 0)
                {
                    var skidLocComplete = await LocationMapTasks.IsCompleteLocMap(currSkid.LocMap);
                    if (skidLocComplete)
                    {
                        this.LocMap.CurrLocMap = currSkid.LocMap;
                        this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                        var currentSkidReadableLocation =
                            LocationMapTasks.GetCurrentReadableLocation();
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.CmdLocationText = "Location: " +
                            Environment.NewLine +
                            currentSkidReadableLocation);
                        throw new MessageOnlyException(
                            "Scanning a location is not supported when a skid with a location is selected.");
                    }

                    // If the skid location is not complete, then the location is being set for the skid when a
                    // product is scanned and added to the skid.
                }

                var scanLocRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier);
                var scanLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(scanLocRefno);
                if (scanLoc.WhsLocMapRefno != this.ActDetail.ActItem.LocMap.WhsLocMapRefno)
                {
                    const string msg = "Scanned warehouse does not match the line item warehouse.";
                    throw new MessageOnlyException(msg);
                }

                this.LocMap.CurrLocMap = scanLoc;
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                _ = await Device.InvokeOnMainThreadAsync(() =>
                    this.CmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
            }

            async Task HandleSkidScanned()
            {
                if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                }

                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    const string msg = "Scanning a shipping container bar code is not supported in disconnected mode.";
                    throw new MessageOnlyException(msg);
                }

                var skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(scanArgs.ParseInfo.Sscc18);
                if (skidInfo.CurrSkid.SkidFkRefno == 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.CmdSelectSkidText = "Select Skid");
                    throw new MessageOnlyException("Skid does not exist.");
                }

                if (skidInfo.CurrSkid.ProdInfo.UnitRefno !=
                    this.ActDetail.ActItem.ProdInfo.UnitRefno)
                {
                    const string msg = "Identified line item product does not match the selected skid product.";
                    throw new MessageOnlyException(msg);
                }

                if (this.ActDetail.ActItem.LocMap.WhsLocMapRefno != skidInfo.CurrSkid.LocMap.WhsLocMapRefno)
                {
                    const string msg = "Skid warehouse does not match the line item warehouse.";
                    throw new MessageOnlyException(msg);
                }

                if (this.ActDetail.ActItem.ProdInfo.UnitRefno != skidInfo.CurrSkid.ProdInfo.UnitRefno)
                {
                    const string msg = "Skid product does not match the line item product.";
                    throw new MessageOnlyException(msg);
                }

                // Insert a new activity detail record
                this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                        this.ActDetail.ActItem.ProdInfo.Code,
                        this.ActDetail.ActItem.ProdInfo.Scheme,
                        this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<itm_refno>" +
                    this.ActDetail.ActItem.ItemFkRefno +
                    "</itm_refno>" +
                    "<alt_qty>" +
                    skidInfo.CurrSkid.Quantity +
                    "</alt_qty>" +
                    "<wt_qty>" +
                    skidInfo.CurrSkid.WeightQuantity +
                    "</wt_qty>" +
                    "<ActivityDetailUniqueSchemeID>" +
                    this.ActDetail.UniqueSchemeId +
                    "</ActivityDetailUniqueSchemeID>" +
                    "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
                await this.ActDetail.InsertSkid(skidInfo);
                this.ActDetail.ActItem.ActHdr.SkidInfo = skidInfo;

                await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;
                        this.CmdSelectSkidText = "Skid # " + skidInfo.CurrSkid.SkidCode;
                    });

                // Update the activity line item quantities.
                var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                this.ActDetail.ActItem.AddToQuantityInActivityItem(
                    this.ActDetail.ActItem.SelectedItem,
                    fillQty.Quantity,
                    fillQty.BaseQuantity,
                    fillQty.WeightQuantity);

                await this.LoadAndBindData();
            }

            async Task HandleProductScanned(BarCodeParse parse)
            {
                // Check if location is set, if defaulting location is not enabled.
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                    !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    throw new MessageOnlyException("Location must be set first!");
                }

                var prodUnitRefno = await this.DataCollector.IdentifyProductScanned(parse);
                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                // Check if the scanned product matches the select activity line item product.
                if (prodUnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                {
                    var itemProdInfo = this.ActDetail.ActItem.ProdInfo;
                    var msg =
                        $"Scanned product '{prodInfo.Code}' BC '{prodInfo.UnitBc}' Unit '{prodInfo.UnitCode}'" +
                        " does not match the selected line item product" +
                        $" '{itemProdInfo.Code}' BC '{itemProdInfo.UnitBc}' Unit '{itemProdInfo.UnitCode}'.";
                    throw new MessageOnlyException(msg);
                }

                // If there is a selected skid check if the scanned product matches.
                var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (currSkid.SkidFkRefno > 0 &&
                    currSkid.ProdInfo.Refno > 0)
                {
                    if (currSkid.ProdInfo.UnitRefno != prodUnitRefno)
                    {
                        var msg = $"Scanned product '{prodInfo.Code}' BC '{prodInfo.UnitBc}' Unit '{prodInfo.UnitCode}'" +
                            " does not match the selected skid product" +
                            $" '{currSkid.ProdInfo.Code}' BC '{currSkid.ProdInfo.UnitBc}' Unit" +
                            $" '{currSkid.ProdInfo.UnitCode}'.";
                        throw new MessageOnlyException(msg);
                    }
                }

                // A skid may contain 'default' scheme data, fill the activity detail
                // with the default skid schema.
                var defaultItemSchemeData = new ActItemSchemeData();
                var initSchemeData = this.InitDetailBeforeGather(this.ActDetail);
                defaultItemSchemeData.ProdSchemeData = initSchemeData;
                this.ActDetail.SchemeData = initSchemeData;
                await DataCollector.GatherProductData(
                    parse,
                    defaultItemSchemeData,
                    this.ActDetail,
                    isForceStopForEditing: false);
                await this.InsertActivityDetail(this.ActDetail);
                await this.FindSelectedItemAsync();
            }
        }

        private ProductSchemeData InitDetailBeforeGather(ActDetail actDetail)
        {
            // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
            // because of the length of the code and the fact isn't human-readable friendly.
            var schemeData = actDetail.SchemeData;
            if (actDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
            {
                var curSkid = actDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                schemeData.LogisticUnitSkidNumber = curSkid.SkidCode;
                schemeData.LogisticUnitBatchLotNumber = curSkid.SchemeData.LogisticUnitBatchLotNumber;
                schemeData.LogisticUnitBestBeforeSellByDate = curSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                schemeData.LogisticUnitProductionDate = curSkid.SchemeData.LogisticUnitProductionDate;
                schemeData.VariantCode = curSkid.SchemeData.VariantCode;
                schemeData.VariantNumber = curSkid.SchemeData.VariantNumber;
                schemeData.VariantDescription = curSkid.SchemeData.VariantDescription;
            }

            if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues &&
                this.ActDetail.ActItem.ProdInfo.Scheme.IsLotTrackedDefault &&
                string.IsNullOrEmpty(schemeData.LogisticUnitBatchLotNumber) &&
                this.ActDetail.ActItem.ActSubType == SysCommon.ActSubTypes.Receiving)
            {
                var poNummber = ((Receive)this.ActDetail.ActItem.ActHdr.SelectedActivity).ActivityReceivingPONumber;
                var lineNumber = ((ReceiveItem)this.ActDetail.ActItem.SelectedItem).ActivityItemStringFKRefno;
                schemeData.LogisticUnitBatchLotNumber = poNummber + "-" + lineNumber;
            }

            this.ActDetail.SchemeData = schemeData;

            return schemeData;
        }

        /// <summary>
        ///     Call refresh data, unique to the activity subtype.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task RefreshBySubType()
        {
            await RefreshData();

            return;

            async Task RefreshData()
            {
                try
                {
                    this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor =
                        await ProductInfoTasks.GetUnitRelationFactor(this.ActDetail.ActItem.ProdInfo);

                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            this.ActDetail.RegLayerListenerProgOut(this);
                            this.ActDetail.RegLayerListenerStatOut(this);
                            await this.ActDetail.Refresh();
                            await Device.InvokeOnMainThreadAsync(() => this.ActDetail.ListDetails.Clear());
                            this.ActDetail.ListDetails = await this.ActDetail.GetActivityDetailList(false);
                        });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(
                                this.ActSubType,
                                ex,
                                this.ActDetail.GetControlDescription(),
                                "Refresh");
                            _ = await errRpt.ShowDialog();
                        });
                }
                finally
                {
                    this.ActDetail.UnregLayerListenerProgOut(this);
                    this.ActDetail.UnregLayerListenerStatOut(this);
                }
            }
        }

        /// <summary>
        ///     Find the selected item in the data list using the SelectedActivityDetailFkRefno.
        /// </summary>
        private Task FindSelectedItemAsync()
        {
            if (this.ActDetail.ListDetails.Count > 0 &&
                this.MainActivityItems.Count > 0)
            {
                var currSelectedListItemIdx = this.ActDetail.SelectedListIdx;

                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                this.ActDetail.SelectedListIdx = -1;
                object selectedItem = null;
                if (this.SelectedActivityDetailFkRefno != 0)
                {
                    foreach (var item in this.ActDetail.ListDetails)
                    {
                        if (this.SelectedActivityDetailFkRefno == this.ActDetail.GetFkRefno(item))
                        {
                            selectedItem = item;
                            break;
                        }

                        this.ActDetail.SelectedListIdx++;
                    }
                }

                // If not found try to find the previous one.
                if (selectedItem == null &&
                    currSelectedListItemIdx > 0)
                {
                    // Logic to handle multiple detail deleted, perhaps by a skid deletion.
                    if (this.ActDetail.ListDetails.Count - 1 <= currSelectedListItemIdx)
                    {
                        // if the index is less that half go to the top, else the bottom.
                        if (currSelectedListItemIdx <
                            Convert.ToInt32(this.ActDetail.ListDetails.Count / 2) - 1)
                        {
                            currSelectedListItemIdx = 0;
                        }
                        else
                        {
                            currSelectedListItemIdx = this.ActDetail.ListDetails.Count - 1;
                        }
                    }
                    else
                    {
                        currSelectedListItemIdx--;
                    }

                    var item = this.ActDetail.ListDetails[currSelectedListItemIdx];
                    if (item != null)
                    {
                        this.ActDetail.SelectedListIdx = currSelectedListItemIdx;
                        this.SelectedActivityDetailFkRefno = this.ActDetail.GetFkRefno(item);
                        selectedItem = item;
                    }
                }

                // Goto the first one in the list if nothing found.
                Device.BeginInvokeOnMainThread(() =>
                    {
                        if (selectedItem == null)
                        {
                            this.ActDetail.SelectedListIdx = 0;
                            this.ListViewMainActivity.SelectedItem =
                                this.ActDetail.ListDetails[this.ActDetail.SelectedListIdx];
                        }
                        else
                        {
                            this.ListViewMainActivity.SelectedItem = selectedItem;
                        }

                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            false);
                    });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedListIdx = -1;
                this.ActDetail.SelectedDetail = null;
                this.ActDetail.DetailFkRefno = 0;
                this.ActDetail.DetailRefno = 0;
                this.SelectedActivityDetailFkRefno = 0;
                Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = null; });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Design the main activity grid / list. Also perform text search filtering.
        /// </summary>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task DesignGrid()
        {
            await Device.InvokeOnMainThreadAsync(() =>
                {
                    // Redesign the grid / list and rebind it to the refreshed list items.
                    this.MainActivityItems = this.ActDetail.DesignGrid(
                        ref this.GridMainActivity,
                        ref this.ListViewMainActivity);

                    if (!string.IsNullOrEmpty(this.txtSearchDetail.Text))
                    {
                        this.MainActivityItems = this.ActDetail.Search(this.txtSearchDetail.Text);
                    }

                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });
        }

        /// <summary>
        ///     Insert an activity detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            actDetail.DetailFkRefno = 0;
            actDetail.ActStatus = SysCommon.ActStatus.Inserted;

            var xmlDoc = actDetail.GetXml(actDetail);

            // actDetail.DetailFkRefno and actDetail.DetailRefno is assigned in Insert().
            await actDetail.Insert(xmlDoc);

            var insertedDetailType = this.ActDetail.GatherDetail();

            this.ActDetail.ListDetails.Add(insertedDetailType);
            this.SelectedActivityDetailFkRefno = insertedDetailType.ActivityDetailFKRefno;
            this.SysEnv.SysSettings.IsNewActivityDetailsAdded = true;
            await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ListViewMainActivity.ItemTemplate = this.ActDetail.DesignGrid(
                        ref this.GridMainActivity,
                        insertedDetailType,
                        this.MainActivityItems);
                });

            ////this.FindSelectedItemAsync();
            ////await AddOrUpdateDetailList();
        }

        /// <summary>
        ///     Display the item list data and set other controls.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task DisplayDataItem()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ExtraInfoItems.Clear();

                    var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                    this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;

                    if (currSkid.SkidFkRefno > 0)
                    {
                        this.CmdAddSkidIsEnabled = true;
                        this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                    }
                    else
                    {
                        this.CmdAddSkidIsEnabled = false;
                        this.CmdSelectSkidText = "Select Skid";
                    }

                    this.CmdLocationIsEnabled = currSkid.SkidFkRefno <= 0;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno > 0)
                    {
                        this.CmdLocationText = "Location: " +
                                               Environment.NewLine +
                                               LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        this.CmdLocationText = "Set Location";
                    }

                    var extra = await this.ActDetail.GetExtraInfoAsync(this.ListViewMainActivity.SelectedItem);
                    this.ExtraInfoItems.Add(extra.Text1);
                    this.ExtraInfoItems.Add(extra.Text2);
                    this.ExtraInfoItems.Add(extra.Text3);
                    this.ExtraInfoItems.Add(extra.Text4);

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                });

            return;
        }

        /// <summary>
        ///     Set add detail command button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        private async void CmdAddDetail_Click(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();
                this.IsIndicatorRunning = true;
                if (await CheckAllowedToAddDetail())
                {
                    var locMap = LocationMap.GetInstance();
                    if (!await LocationMapTasks.IsCompleteLocMap(locMap.CurrLocMapRefno))
                    {
                        throw new MessageOnlyException("Location must be set first.");
                    }

                    if (locMap.CurrLocMap.WhsLocMapRefno !=
                        this.ActDetail.ActItem.LocMap.WhsLocMapRefno)
                    {
                        throw new MessageOnlyException(
                            "Detail warehouse must match the line item warehouse.");
                    }

                    await PerformAddDetail();
                }
                else
                {
                    throw new MessageOnlyException("Permission denied to add detail.");
                }
            }
            catch (GatherProductDataException gatherEx)
            {
                if (!gatherEx.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, gatherEx);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", ex.Message, "OK"));
            }
            finally
            {
                await Task.Run(this.LoadAndBindData);
                await this.ScannerEnable();
            }

            return;

            ActItemSchemeData InitDetailBeforeGather()
            {
                this.ActDetail.Quantity = 0;
                this.ActDetail.BaseQuantity = 0;
                this.ActDetail.WeightQuantity = 0;
                this.ActDetail.SchemeId = string.Empty;
                this.ActDetail.UniqueSchemeId = string.Empty;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime1 = SysCommon.EmptyDateTime;

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of
                // the length of the code and the fact isn't human-readable friendly.
                var tempItemProdSchemeData = new ActItemSchemeData();
                var schemeData = this.ActDetail.SchemeData;
                var skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (skidInfo.SkidFkRefno == 0)
                {
                    // Set to empty values, otherwise the detail edit screen may not be shown.
                    schemeData.LogisticUnitSkidNumber = string.Empty;
                    schemeData.LogisticUnitBatchLotNumber = string.Empty;
                    ////schemeData.LogisticUnitBestBeforeSellByDate = SysCommon.EmptyDateTime;
                    ////schemeData.LogisticUnitProductionDate = SysCommon.EmptyDateTime;
                    schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                    schemeData.LogisticUnitProductionDate = DateTime.Now;
                    schemeData.VariantCode = string.Empty;
                    schemeData.VariantNumber = 0;
                    schemeData.VariantDescription = string.Empty;
                }
                else
                {
                    schemeData.LogisticUnitSkidNumber = skidInfo.SkidCode;
                    schemeData.LogisticUnitBatchLotNumber = skidInfo.SchemeData.LogisticUnitBatchLotNumber;
                    schemeData.LogisticUnitBestBeforeSellByDate = skidInfo.SchemeData.LogisticUnitBestBeforeSellByDate;
                    schemeData.LogisticUnitProductionDate = skidInfo.SchemeData.LogisticUnitProductionDate;
                    schemeData.VariantCode = skidInfo.SchemeData.VariantCode;
                    schemeData.VariantNumber = skidInfo.SchemeData.VariantNumber;
                    schemeData.VariantDescription = skidInfo.SchemeData.VariantDescription;
                }

                tempItemProdSchemeData.ProdSchemeData = schemeData;
                this.ActDetail.SchemeData = schemeData;
                return tempItemProdSchemeData;
            }

            async Task<bool> CheckAllowedToAddDetail()
            {
                var isAllowedAddManualDetail = true;
                if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                    this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    var form = new FormAddManualDetailOverride();

                    isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                        await form.ShowDialog() == DialogResult.OK);
                }

                return isAllowedAddManualDetail;
            }

            async Task PerformAddDetail()
            {
                ////var gatherActDetail = ActDetailFacade.GetActDetail(
                ////    this.ActDetail.ActItem,
                ////    this.ActSubType);

                // Not allowed to count individual boxes on a skid. Skids must be
                // scanned as a whole unit or built in skid maintenance.

                // DEV-827 This change to allow counting partial skids is a bad idea.
                ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                ////{
                ////    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                ////    {
                ////        this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                ////    }
                ////}
                var tempItemProdSchemeData = InitDetailBeforeGather();
                await DataCollector.GatherProductData(
                    null,
                    tempItemProdSchemeData,
                    this.ActDetail,
                    isForceStopForEditing: true);
                await this.InsertActivityDetail(this.ActDetail);
            }
        }

        /// <summary>
        ///     Option menu click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.CmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                });

            var result = await this.DisplayActionSheet(
                "ActivityDetail Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            if (result == this.CmdLocationText &&
                this.CmdLocationIsEnabled)
            {
                CmdLocationClick();
            }
            else if (result == this.CmdSelectSkidText &&
                     this.CmdSelectSkidIsEnabled)
            {
                CmdSelectSkidClick();
            }
            else if (result == this.CmdAddSkidText &&
                     this.CmdAddSkidIsEnabled)
            {
                CmdAddSkidClick();
            }

            return;

            async void CmdLocationClick()
            {
                try
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.SelectLocationMap,
                        this.ActDetail.ActItem.LocMap.WhsLocMapRefno,
                        true);
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var actItemLocRefno = this.ActDetail.ActItem.LocMap.WhsLocMapRefno;
                        if (this.LocMap.ChosenLocMap.WhsLocMapRefno != actItemLocRefno)
                        {
                            throw new MessageOnlyException(
                                "Chosen warehouse must match the activity item warehouse.");
                        }

                        // Special validation for picking and filtering by section.
                        // Check if the scanned section matched the filtered one.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            var sectRefno = ((PickingActivity)this.ActDetail.ActItem.ActHdr)
                                .PickingLocationMapFilter
                                .SectLocMapRefno;
                            if (sectRefno != 0 &&
                                this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                            {
                                var sectName = ((PickingActivity)this.ActDetail.ActItem.ActHdr)
                                    .PickingLocationMapFilter
                                    .SectName;
                                var msg =
                                    $"Warning selected location section '{this.LocMap.CurrLocMap.SectName}' does" +
                                    $" not match the filtered picking section '{sectName}'.";

                                await Device.InvokeOnMainThreadAsync(async () =>
                                        await Application.Current.MainPage.DisplayAlert(
                                            this.ActDetail.ActItem.GetControlDescription(),
                                            msg,
                                            Resource.OK));
                            }
                        }

                        this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    }

                    await Device.InvokeOnMainThreadAsync(() =>
                        Task.FromResult(
                            this.CmdLocationText = "Location: " + Environment.NewLine +
                            LocationMapTasks.GetCurrentReadableLocation()));
                    await this.DisplayDataItem();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
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

            async void CmdSelectSkidClick()
            {
                try
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.SkidInfo,
                        this.ActDetail.ActItem,
                        this.ActSubType,
                        SysCommon.ActSubTypes.SkidInfo,
                        SysShared.ActivityType.Details);
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var xmlDoc = await form.GetXml();
                        this.ActDetail.ActItem.ActHdr.SkidInfo =
                            await SysSkidInfo.GetSkidInfoFromXml(xmlDoc);

                        if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                        {
                            var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                            var prodInfo = this.ActDetail.ActItem.ProdInfo;
                            if (prodInfo.Code == currSkid.ProdInfo.Code &&
                                prodInfo.UnitBc == currSkid.ProdInfo.UnitBc &&
                                prodInfo.UnitCode == currSkid.ProdInfo.UnitCode &&
                                currSkid.LocMap.WhsLocMapRefno ==
                                this.ActDetail.LocMap.CurrLocMap.WhsLocMapRefno)
                            {
                                this.LocMap.CurrLocMap = currSkid.LocMap;

                                this.CmdAddSkidIsEnabled = true;
                                this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                                this.CmdLocationIsEnabled = false;
                            }
                            else
                            {
                                this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                                this.CmdSelectSkidText = "Select Skid";
                                this.CmdAddSkidIsEnabled = false;
                                await Device.InvokeOnMainThreadAsync(async () =>
                                    await this.DisplayAlert(
                                        "Skid",
                                        "Skid product must match the selected line item product and warehouse.",
                                        "OK"));
                            }
                        }
                        else
                        {
                            this.CmdSelectSkidText = "Select Skid";
                            this.CmdAddSkidIsEnabled = false;
                            this.CmdLocationIsEnabled = true;
                        }
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
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

            async void CmdAddSkidClick()
            {
                try
                {
                    if (await CheckAllowedToAddSkid() && await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            "Add the available skid quantity?",
                            "Yes",
                            "No")))
                    {
                        this.ActDetail.UniqueSchemeId =
                            ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                                this.ActDetail.ActItem.ProdInfo.Code,
                                this.ActDetail.ActItem.ProdInfo.Scheme,
                                this.ActDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity
                        // validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<imclient>" +
                            "<itm_refno>" +
                            this.ActDetail.ActItem.ItemFkRefno +
                            "</itm_refno>" +
                            "<alt_qty>" +
                            this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity +
                            "</alt_qty>" +
                            "<wt_qty>" +
                            this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity +
                            "</wt_qty>" +
                            "<ActivityDetailUniqueSchemeID>" +
                            this.ActDetail.UniqueSchemeId +
                            "</ActivityDetailUniqueSchemeID>" +
                            "</imclient>");

                        await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
                        await this.ActDetail.InsertSkid(this.ActDetail.ActItem.ActHdr.SkidInfo);
                        var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedItem,
                            fillQty.Quantity,
                            fillQty.BaseQuantity,
                            fillQty.WeightQuantity);

                        await Task.Run(this.LoadAndBindData);
                        this.ListViewMainActivity.Focus();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
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

                return;

                async Task<bool> CheckAllowedToAddSkid()
                {
                    var isAllowedAddManualDetail = true;
                    if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                        this.ActSubType == SysCommon.ActSubTypes.Picking && !this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail)
                    {
                        var form = new FormAddManualDetailOverride();
                        isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                            await form.ShowDialog() == DialogResult.OK);
                    }

                    return isAllowedAddManualDetail;
                }
            }
        }
    }
}