// <copyright file="ActivityDetail.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Controller.Activity;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Controller.Activity_Item;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Activity detail control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetail : DMobileModal, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Define the warehouse map location text information.
        /// </summary>
        private string cmdLocationText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the location selection is enabled.
        /// </summary>
        private bool cmdLocationIsEnabled = false;

        /// <summary>
        /// Define the select skid text information.
        /// </summary>
        private string cmdSelectSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the skid selection is enabled.
        /// </summary>
        private bool cmdSelectSkidIsEnabled = false;

        /// <summary>
        /// Define the add skid text information.
        /// </summary>
        private string cmdAddSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether adding a skid is enabled.
        /// </summary>
        private bool cmdAddSkidIsEnabled = false;

        /// <summary>
        /// Gets or sets a value indicating whether the search text box is in focus.
        /// </summary>
        private bool IsSearchFocused { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetail"/> class.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub types.</param>
        public ActivityDetail(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.CmdLocationIsEnabled = false;
            this.CmdSelectSkidIsEnabled = false;
            this.CmdAddSkidIsEnabled = false;
            this.cmdAddDetail.IsEnabled = false;

            this.CmdLocationText = "Set Location";
            this.CmdSelectSkidText = "Select Skid";
            this.CmdAddSkidText = "Add Skid";
            this.cmdAddDetail.Text = "Add Detail";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.MainActivityItems = new ObservableCollection<object>();
                this.ExtraInfoItems = new ObservableCollection<string>();

                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = ScannerEngine.GetInstance().Scanner;

                if (actItem.ActHdr.SkidInfo == null)
                {
                    actItem.ActHdr.SkidInfo = new SysSkidInfo();
                }

                this.ActSubType = actSubTypes;
                this.ActDetail = ActDetailFacade.GetActDetail(actItem, actSubTypes);
                this.ActDetail.ActItem = actItem;

                this.Title = this.ActDetail.GetControlDescription();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether activity detail has been changed and therefore considered dirty.
        /// </summary>
        private bool IsDetailChanged { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show a data valid message or raise an error.
        /// </summary>
        private bool IsShowDataValidMsg { get; set; } = true;

        /// <summary>
        /// Gets or sets the selected activity detail foreign key reference number.
        /// </summary>
        private int SelectedActivityDetailFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the options menu items list.
        /// </summary>
        private List<string> OptionsMenuItems { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the ExtraInfoItems
        /// Gets or sets a list of extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the MainActivityItems
        /// Gets or sets a list of data grid activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the command location text.
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
        /// Gets or sets a value indicating whether command location is enabled.
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
        /// Gets or sets the command select skid text.
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
        /// Gets or sets a value indicating whether the command select skid is enabled.
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
        /// Gets or sets the command add skid text.
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
        /// Gets or sets a value indicating whether the command add skid is enabled.
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

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return "Activity Detail";
        }

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
                    this.IsBarCodeHandlerRegistered = false;
                    this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    await this.Scanner.Stop();
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
                    this.IsBarCodeHandlerRegistered = true;
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                this.IsBarCodeHandlerRegistered = true;
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (await this.IsReadyToSave())
                {
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all
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
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                {
                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            "Print Label",
                            "Print Product Labels",
                            "Yes",
                            "No")))
                        {
                            await PrintBatchedProductLabels();
                        }
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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

            //////////////////////////////////////////////////////////////////////////
            async Task PrintBatchedProductLabels()
            {
                // For each detail marked for printing
                var sendReq = string.Empty;
                var errDetail = string.Empty;
                var detail = this.ActDetail.ListDetails;

                for (var rowIdx = 0; rowIdx <= detail.Count - 1; rowIdx++)
                {
                    if (Convert.ToBoolean(detail[rowIdx].GetType().GetProperty(
                        "IsBatchProductLabel")?.GetValue(detail[rowIdx])))
                    {
                        var request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + "<serial>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailSingleTradeItemSerialNumber")?.GetValue(
                                    detail[rowIdx]) + "</serial>"
                            + "<pdate>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailLogisticUnitProductionDate")?.GetValue(
                                    detail[rowIdx]) + "</pdate>"
                            + "<lot_no>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(
                                    detail[rowIdx]) + "</lot_no>"
                            + "<bdate>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailLogisticUnitBestBeforeSellByDate")?.GetValue(
                                    detail[rowIdx]) + "</bdate>"
                            + "<variant_code>" + detail[rowIdx].GetType().GetProperty(
                                "ProductSchemeVariantCode")?.GetValue(detail[rowIdx]) + "</variant_code>"
                            + "<variant_no>" + detail[rowIdx].GetType().GetProperty(
                                "ProductSchemeVariantNo")?.GetValue(detail[rowIdx]) + "</variant_no>"
                            + "<alt_qty>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailQuantity")?.GetValue(detail[rowIdx]) + "</alt_qty>"
                            + "<wt_qty>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailBaseQuantity")?.GetValue(detail[rowIdx]) + "</wt_qty>"
                            + "<base_qty>" + detail[rowIdx].GetType().GetProperty(
                                "ActivityDetailWeightQuantity")?.GetValue(detail[rowIdx]) + "</base_qty>"
                            + "</params>");

                        // CreateSubmitProductLabelXML assumes actItem and
                        // Loc.sSelectedLocationMap is correct
                        sendReq = await this.ActDetail.CreateSubmitProductLabelXml(request);
                        errDetail = string.Empty;
                        try
                        {
                            _ = SysCommon.SubmitComm(sendReq);
                            detail[rowIdx].GetType().GetProperty("IsBatchProductLabel")?.SetValue(detail[rowIdx], false);
                        }
                        catch (MessageOnlyException msgEx)
                        {
                            var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.GetControlDescription(),
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
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Get the activity detail return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity detail return information.</returns>
        public async Task<XDocument> GetXML()
        {
            var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();

            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<qty>" + fillQty.Quantity + "</qty>"
                + "<base_qty>" + fillQty.BaseQuantity + "</base_qty>"
                + "<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle status information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            if (dataLayerEventArgs.CurrentValue > 0 && dataLayerEventArgs.MaxValue > 0)
            {
                double percent = 100 * dataLayerEventArgs.CurrentValue / dataLayerEventArgs.MaxValue;
                this.progBarMain.Progress = percent / 100;
            }
        }

        /// <summary>
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    // Set the dialog result to OK to force the line item total to recalculate.
                    // Even though the back button is meant to be a cancel we need to update
                    // because data still could have changed.
                    this.DialogResult = DialogResult.OK;
                    _ = base.OnBackButtonPressed();
                    await this.ActDisappearing();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string"/> Property name to check if changed.</param>
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

            //////////////////////////////////////////////////////////////////////////
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
        /// Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                // Note: Do we want to disable the scanner here. It may not even be enabled before at all.
                // It is my understanding that the OnAppearing event only occurs once when the control is loading.
                this.Loading = true;
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.ScannerStart();

                    // NOTE: Not sure if this is needed at this anymore.
                    this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor =
                        await ProductInfoTasks.GetUnitRelationFactor(
                            this.ActDetail.ActItem.ProdInfo);

                    this.ActDetail.RegLayerListenerProgOut(this);
                    await this.RefreshBySubType();
                    this.ActDetail.UnregLayerListenerProgOut(this);
                    await this.DesignGrid();
                    this.cmdAddDetail.IsEnabled = true;

                    await this.ActAppearing();

                    // No need to invoke DisplayData() selected item will do so, unless the list is empty.
                    if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                    {
                        await this.DisplayDataItem();
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (CommunicationException commEx)
            {
                this.IsFirstAppearance = true;

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
                this.IsFirstAppearance = true;
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
                this.Loading = false;
                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }
        }

        /// <summary>
        /// Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ActDisappearing();
            base.OnDisappearing();
        }

        /// <summary>
        /// Activity appearing.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task ActAppearing()
        {
            await this.ScannerEnable();
            SubscribeKeyUpHandlers();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

            await this.FindSelectedItemAsync();

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async (keyPressed) =>
                    {
                        if (this.Loading)
                        {
                            return;
                        }

                        // TODO: Verify key codes.
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Del || keycode == Keycode.Clear)
                        {
                            if (!this.IsSearchFocused)
                            {
                                await OnAppearingDelKey();
                            }
                        }
                        else if (keycode == Keycode.F1)
                        {
                            await OnAppearingF1Key();
                        }
                        else if (keycode == Keycode.F3)
                        {
                            await OnAppearingF3Key();
                        }
                        else if (keycode == Keycode.F4)
                        {
                            await OnAppearingF4Key();
                        }
                        else if (keycode == Keycode.DpadDown)
                        {
                            OnAppearingDpadDownKey();
                        }
                        else if (keycode == Keycode.DpadUp)
                        {
                            OnAppearingDpadUp();
                        }
                    });

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingDelKey()
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
                            var msg = $"Do you wish to delete all detail belonging to this skid" +
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

                        // Invoke the deletion.
                        if (isDelItem)
                        {
                            await OnAppearingDelKeyDetail();
                        }
                        else if (isDelSkid)
                        {
                            await OnAppearingDelKeySkid();
                        }

                        await this.FindSelectedItemAsync();
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
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

                    //////////////////////////////////////////////////////////////////////////
                    void IsItemSelected()
                    {
                        var isItemSelected = this.ListViewMainActivity.SelectedItem != null
                                             && Convert.ToInt32(this.MainActivityItems.IndexOf(
                                                 this.ListViewMainActivity.SelectedItem)) >= 0;

                        if (!isItemSelected)
                        {
                            throw new MessageOnlyException("Nothing selected to delete.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task OnAppearingDelKeyDetail()
                    {
                        // Read the selected detail.
                        await this.ActDetail.AssignThisFromDetailAsync(
                            (Resources.Types.ActivityDetail)this.ListViewMainActivity.SelectedItem,
                            false);

                        // Submit the request, and delete it from the data table.
                        _ = await this.ActDetail.Delete();

                        // Update the activity line item quantities.
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedActivityItem,
                            this.ActDetail.Quantity * -1,
                            this.ActDetail.BaseQuantity * -1,
                            this.ActDetail.WeightQuantity * -1);

                        // Update the table item quantity.
                        await this.ActDetail.ActItem.UpdateItemTableQuantity(this.ActDetail.ActItem.SelectedActivityItem);
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task OnAppearingDelKeySkid()
                    {
                        // Determine the selected detail skid code.
                        var currIdx = this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem);
                        var skidCode = Convert.ToString(this.ActDetail.ListDetails[currIdx].GetType().GetProperty(
                                "ActivityDetailLogisticUnitSkidNumber")?.GetValue(
                                    this.ActDetail.ListDetails[currIdx]));
                        var skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(skidCode);

                        if (skidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            throw new Exception($"Unable to determine the skid '{skidCode}'" +
                                $" information to delete.");
                        }

                        // Submit the request, and delete it from the data table.
                        await this.ActDetail.DeleteSkid(skidInfo);

                        await this.RefreshBySubType();
                        await this.DesignGrid();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF1Key()
                {
                    try
                    {
                        await this.ActDisappearing();
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;

                            // TODO: DEV-5619
                            // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                            await this.Close();
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                        await this.ActAppearing();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF3Key()
                {
                    // TODO: DEV-5619
                    // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile
                    try
                    {
                        await this.ActDisappearing();
                        var msg = await this.ActDetail.ActItem.GetTotalQuantity();
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                "OK"));
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                        await this.ActAppearing();
                        _ = this.ListViewMainActivity.Focus();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF4Key()
                {
                    try
                    {
                        await this.ActDisappearing();
                        var xmlString = new StringBuilder();
                        var xml = XmlWriter.Create(xmlString);
                        await CheckIfActivityCanBeConfirmed();
                        SubmitDetailConfirmation(xmlString);
                        MarkDetailAsConfirmed();
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
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
                    finally
                    {
                        await this.ActAppearing();
                        _ = this.ListViewMainActivity.Focus();
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void MarkDetailAsConfirmed()
                    {
                        if (this.ListViewMainActivity.SelectedItem is PickingDetail pickDetail)
                        {
                            pickDetail = (PickingDetail)this.ActDetail.ListDetails[
                            this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)];
                            pickDetail.ActivityDetailIsConfirmed = true;
                            pickDetail.ActivityDetailIsConfirmedString = "C";
                        }
                        else if (this.ListViewMainActivity.SelectedItem is TransferDetail transferDetail)
                        {
                            pickDetail = (PickingDetail)this.ActDetail.ListDetails[
                            this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)];
                            pickDetail.ActivityDetailIsConfirmed = true;
                            pickDetail.ActivityDetailIsConfirmedString = "C";
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    static void SubmitDetailConfirmation(StringBuilder xmlString)
                    {
                        _ = SysCommon.SubmitComm(xmlString.ToString());
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task CheckIfActivityCanBeConfirmed()
                    {
                        XDocument request = null;

                        if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<imclient>"
                                + "<command>"
                                + "<action>SUBMIT SALES ORDER DETAIL CONFIRM</action>"
                                + "</command>"
                                + await SysCommon.GetRequestDefaults(false)
                                + "<refno>" + this.ActDetail.DetailFkRefno + "</refno>"
                                + "</imclient>");
                        }
                        else if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                        {
                            request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<imclient>"
                                + "<command>"
                                + "<action>SUBMIT TRANSFER FROM DETAIL CONFIRM</action>"
                                + "</command>"
                                + await SysCommon.GetRequestDefaults(false)
                                + "<refno>" + this.ActDetail.DetailFkRefno + "</refno>"
                                + "</imclient>");
                        }
                        else if (this.ActSubType == SysCommon.ActSubTypes.TransferTo)
                        {
                            request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<imclient>"
                                + "<command>"
                                + "<action>SUBMIT TRANSFER TO DETAIL CONFIRM</action>"
                                + "</command>"
                                + await SysCommon.GetRequestDefaults(false)
                                + "<refno>" + this.ActDetail.DetailFkRefno + "</refno>"
                                + "</imclient>");
                        }
                        else
                        {
                            throw new MessageOnlyException("Confirmations are not supported with this activity.");
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadDownKey()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) <
                            this.MainActivityItems.Count - 1)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) + 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadUp()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > 0)
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
        }

        /// <summary>
        /// Activity disappearing.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
        }

        /// <summary>
        /// Text box search item focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void TxtSearchItem_Focused(object sender, EventArgs e)
        {
            this.IsSearchFocused = true;
        }

        /// <summary>
        /// Text box search item unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void TxtSearchItem_Unfocused(object sender, EventArgs e)
        {
            this.IsSearchFocused = false;
        }

        /// <summary>
        /// Search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                await this.ActDisappearing();
                await this.DesignGrid();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                await this.ActAppearing();
            }
        }

        /// <summary>
        /// Data grid activity detail selected detail event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.AssignActivityDetail(e.SelectedItem);
            this.ActDetail.SelectedListItemIdx = e.SelectedItemIndex;
            this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
            await this.DisplayDataItem();
        }

        /// <summary>
        /// Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Scan event arguments.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            this.Loading = true;
            var prodUnitRefno = 0;
            var svdActDetailFKRefno = this.ActDetail.DetailFkRefno;
            var parse = scanArgs.ParseInfo;

            try
            {
                await this.ActDisappearing();

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier();
                }
                else
                {
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        // Check if location is set, if defaulting location is not enabled.
                        if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                            !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            throw new MessageOnlyException("Location must be set first!");
                        }

                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);

                        var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        // Check if the scanned product matches the select activity line item product.
                        if (prodUnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                        {
                            throw new Exception("Scanned product does not match the" +
                                " selected line item product.");
                        }

                        // If there is a selected skid check if the scanned product matches.
                        var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                        if (currSkid.SkidFKRefno != 0)
                        {
                            if (currSkid.ProdInfo.UnitRefno != prodUnitRefno)
                            {
                                throw new Exception("Scanned product does not match the selected skid product.");
                            }
                        }

                        // Get the activity item reference number from the identified location reference number
                        // and product reference number.
                        var tempActItemRefno = await this.ActDetail.ActItem.GetActivityItemRefno(
                            prodUnitRefno);
                        if (tempActItemRefno == 0)
                        {
                            // Location and product was not found on this activity
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                Convert.ToString(this.ActDetail.ActItem.GetItemNotFoundMessage(
                                    this.LocMap,
                                    prodInfo)),
                                "OK"));
                        }
                        else
                        {
                            var isInsertReady = false;

                            // A skid may contain 'default' scheme data, fill the activity detail
                            // with the default skid schema.
                            var tempItemProdSchemeData = new ActItemSchemeData();
                            var schemeData = this.ActDetail.SchemeData;
                            if (currSkid.SkidFKRefno != 0)
                            {
                                schemeData.LogisticUnitSkidNumber = currSkid.SkidCode;
                                schemeData.LogisticUnitBatchLotNumber = currSkid.SchemeData.LogisticUnitBatchLotNumber;
                                schemeData.LogisticUnitBestBeforeSellByDate = currSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                                schemeData.LogisticUnitProductionDate = currSkid.SchemeData.LogisticUnitProductionDate;
                                schemeData.VariantCode = currSkid.SchemeData.VariantCode;
                                schemeData.VariantNumber = currSkid.SchemeData.VariantNumber;
                                schemeData.VariantDescription = currSkid.SchemeData.VariantDescription;
                            }

                            tempItemProdSchemeData.ProdSchemeData = schemeData;
                            this.ActDetail.SchemeData = schemeData;

                            // Gather data is the meat and potatoes of collecting and assembling
                            // the product scheme data.
                            isInsertReady = await SysShared.GatherProductData(
                                parse,
                                this.ActSubType,
                                this.ActDetail.ActItem,
                                tempItemProdSchemeData,
                                isInsertReady,
                                this.ActDetail,
                                false);
                            if (isInsertReady)
                            {
                                await this.InsertActivityDetail(this.ActDetail);
                            }

                            await this.FindSelectedItemAsync();
                            ////if (this.MainActivityItems.Count > currRowIdx)
                            ////{
                            ////    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[currRowIdx];
                            ////}
                        }
                    }
                    else if (!string.IsNullOrEmpty(parse.ActFKRefno))
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            "Scanning a header reference # is not supported here.",
                            "OK"));
                    }
                    else if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            "Scanning an item reference # is not supported here.",
                            "OK"));
                    }
                    else if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            "Scanning a detail reference # is not supported here.",
                            "OK"));
                    }
                    else if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            throw new MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException("Scanning a shipping container bar code is not" +
                                " supported in disconnected mode.");
                        }

                        var skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                            scanArgs.ParseInfo.SSCC18);
                        this.ActDetail.ActItem.ActHdr.SkidInfo = skidInfo;

                        if (skidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() =>
                                this.CmdSelectSkidText = "Select Skid");
                            throw new MessageOnlyException("Skid does not exist.");
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.CmdSelectSkidText =
                                "Skid# " + skidInfo.CurrSkid.SkidCode);

                            if (skidInfo.CurrSkid.ProdInfo.UnitRefno !=
                                this.ActDetail.ActItem.ProdInfo.UnitRefno)
                            {
                                throw new MessageOnlyException("Identified line item product does not match" +
                                    " the selected skid product.");
                            }
                        }

                        if (this.ActDetail.ActItem.LocMap.WhsLocMapRefno !=
                            skidInfo.CurrSkid.LocMap.WhsLocMapRefno)
                        {
                            throw new MessageOnlyException("Skid warehouse does not match the selected warehouse.");
                        }

                        if (this.ActDetail.ActItem.ProdInfo.UnitRefno !=
                            skidInfo.CurrSkid.ProdInfo.UnitRefno)
                        {
                            throw new MessageOnlyException("Skid product does not match the line item product.");
                        }

                        // Insert a new activity detail record
                        this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActDetail.ActItem.ProdInfo.Code,
                            this.ActDetail.ActItem.ProdInfo.Scheme,
                            this.ActDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                        + "<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                        + "<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                        + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                        + "</imclient>");

                        await this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                        // We should never get here.
                        await this.ActDetail.InsertSkid(skidInfo);

                        if (skidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() =>
                            {
                                this.CmdAddSkidIsEnabled = false;

                                this.CmdSelectSkidText = "Select Skid";
                            });
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(() =>
                            {
                                this.CmdAddSkidIsEnabled = true;

                                this.CmdSelectSkidText = "Skid # " + skidInfo.CurrSkid.SkidCode;
                            });
                        }

                        var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedActivityItem,
                            fillQty.Quantity,
                            fillQty.BaseQuantity,
                            fillQty.WeightQuantity);

                        // Refreshing the all the added detail makes sense even if the count
                        // is large.
                        await this.RefreshBySubType();
                    }
                    else
                    {
                        throw new MessageOnlyException("Scanned bar code type is not supported here.");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                await this.FindSelectedItemAsync();
            }
            catch (IsReadyToApplyException)
            {
                await this.FindSelectedItemAsync();
            }
            catch (GatherProductDataException ex)
            {
                await this.FindSelectedItemAsync();

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
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
                await this.ActAppearing();
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier()
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();

                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText =
                        "Location: " + Environment.NewLine + currentReadableLocation);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText = "Set Location");
                }
            }
        }

        /// <summary>
        /// Call refresh data, unique to the activity sub type.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task RefreshBySubType()
        {
            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.PhysicalCounts:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.Returns:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.LoginStartup:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SplashStartup:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PrintProductLabel:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSelectCriteria:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingFilter:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SubmitBatch:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.CommunicationSetup:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.SetupDefaults:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquirySummary:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    await RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    await RefreshData();
                    break;

                default:
                    // Performing a refresh data will remove and partially added new items or quantity.
                    await RefreshData();
                    break;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task RefreshData()
            {
                try
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.ActDetail.UnregLayerListenerProgOut(this);
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
                    ////await Task.Run(() =>
                    ////{
                    ////    // Explicit GC, Xamarin forms not disposing of objects properly
                    ////    GC.Collect();
                    ////    GC.WaitForPendingFinalizers();
                    ////});
                }
            }
        }

        /// <summary>
        /// Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActDetail.ListDetails.Count > 0)
            {
                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                this.ActDetail.SelectedListItemIdx = -1;
                object selectedItem = null;
                if (this.SelectedActivityDetailFKRefno != 0)
                {
                    foreach (var item in this.ActDetail.ListDetails)
                    {
                        if (this.SelectedActivityDetailFKRefno == this.ActDetail.GetFkRefno(item))
                        {
                            selectedItem = item;
                            this.ActDetail.SelectedListItemIdx++;
                            break;
                        }
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActDetail.SelectedListItemIdx = 0;
                    selectedItem = this.ActDetail.ListDetails[this.ActDetail.SelectedListItemIdx];
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
                }
                else
                {
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFkRefno;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = selectedItem;
                    this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedListItemIdx = -1;
                this.ActDetail.SelectedActivityDetail = null;
                this.ActDetail.DetailFkRefno = 0;
                this.ActDetail.DetailRefno = 0;
                this.SelectedActivityDetailFKRefno = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// Design the main activity grid / list. Also perform text search filtering.
        /// </summary>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
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
        /// Insert an activity detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            actDetail.DetailFkRefno = 0;
            var xmlDoc = actDetail.GetXml(actDetail);
            await actDetail.Insert(xmlDoc);

            var detailIdx = this.ActDetail.FindListDetailIndex(activityDetailRefno: actDetail.DetailRefno);
            if (detailIdx >= 0)
            {
                this.ActDetail.UpdateListDetail(detailIdx);
                var detail = (Resources.Types.ActivityDetail)this.ActDetail.ListDetails[detailIdx];
                this.SelectedActivityDetailFKRefno = detail.ActivityDetailFKRefno;
                this.MainActivityItems[detailIdx] = detail;
            }
            else
            {
                var inserted = this.ActDetail.InsertListDetail(this.MainActivityItems);
                this.SelectedActivityDetailFKRefno = inserted.ActivityDetailFKRefno;
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ListViewMainActivity.ItemTemplate = this.ActDetail.DesignGrid(
                        ref this.GridMainActivity,
                        inserted,
                        this.MainActivityItems);
                });

                await this.FindSelectedItemAsync();
            }
        }

        /// <summary>
        /// Display the item list data and set other controls.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task DisplayDataItem()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                this.ExtraInfoItems.Clear();

                var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;

                if (currSkid.SkidFKRefno > 0)
                {
                    this.CmdAddSkidIsEnabled = true;
                    this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                }
                else
                {
                    this.CmdAddSkidIsEnabled = false;
                    this.CmdSelectSkidText = "Select Skid";
                }

                if (currSkid.SkidFKRefno > 0)
                {
                    this.CmdLocationIsEnabled = false;
                }
                else
                {
                    this.CmdLocationIsEnabled = true;
                }

                if (this.LocMap.CurrLocMap.WhsLocMapRefno > 0)
                {
                    this.CmdLocationText = "Location: " + Environment.NewLine +
                        LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.CmdLocationText = "Set Location";
                }

                switch (this.ActDetail.ActSubType)
                {
                    case SysCommon.ActSubTypes.Adjustment:
                        if (this.ListViewMainActivity.SelectedItem is AdjustmentDetail adjustmentDetail)
                        {
                            // Get the cycle count line item location.
                            var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(adjustmentDetail.LocationMapRefno));
                            var countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                            // Using the cycle count line item warehouse, get the product default
                            // picking location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                countItemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(adjustmentDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));
                            var prodDesc = adjustmentDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = adjustmentDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Adjusted Location: " + countLoc.Trim());
                            ////this.ExtraInfoItems.Add(cycleDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.AssignDefaultLocation:
                        break;

                    case SysCommon.ActSubTypes.BarCodeAssociation:
                        break;

                    case SysCommon.ActSubTypes.BreakSkid:
                        break;

                    case SysCommon.ActSubTypes.ConvertUnit:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        if (this.ListViewMainActivity.SelectedItem is CountDetail cycleDetail)
                        {
                            // Get the cycle count line item location.
                            var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(cycleDetail.LocationMapRefno));
                            var countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                            // Using the cycle count line item warehouse, get the product default
                            // picking location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                countItemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(cycleDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));
                            var prodDesc = cycleDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = cycleDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Counted Location: " + countLoc.Trim());
                            ////this.ExtraInfoItems.Add(cycleDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.DataStatus:
                        break;

                    case SysCommon.ActSubTypes.InventoryInquiry:
                        if (this.ActDetail.ActItem.ProdInfo.Refno <= 0)
                        {
                            this.ExtraInfoItems.Add($"PRODUCT: {"All"}");
                        }
                        else
                        {
                            this.ExtraInfoItems.Add(
                                $"PRODUCT: {ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActDetail.ActItem.ProdInfo)}");
                        }

                        this.ExtraInfoItems.Add($"LOCATION: {LocationMapTasks.GetCurrentReadableLocation()}");
                        break;

                    case SysCommon.ActSubTypes.InventoryReplenishment:
                        break;

                    case SysCommon.ActSubTypes.MoveInventory:
                        break;

                    case SysCommon.ActSubTypes.MoveSkid:
                        break;

                    case SysCommon.ActSubTypes.NotApplicable:
                        break;

                    case SysCommon.ActSubTypes.PhysicalCounts:
                        if (this.ListViewMainActivity.SelectedItem is CountDetail physicalDetail)
                        {
                            // Get the cycle count line item location.
                            var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(physicalDetail.LocationMapRefno));
                            var countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                            // Using the cycle count line item warehouse, get the product default
                            // picking location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                countItemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(physicalDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));
                            var prodDesc = physicalDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = physicalDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Counted Location: " + countLoc.Trim());
                            ////this.ExtraInfoItems.Add(physicalDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.Picking:
                        if (this.ListViewMainActivity.SelectedItem is PickingDetail pickDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(pickDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(pickDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = pickDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = pickDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Picked Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(pickDetail.ActivityDetailSchemeID);

                            ////var qty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                            ////    ActivityItemPickingQuantityShipped;

                            ////var wtQty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                            ////    ActivityItemPickingWeightQuantityShipped;

                            ////this.ExtraInfoItems.Add("Total: " + SysCommon.NiceQuantityReadableString(
                            ////    this.ActDetail.ActItem.ProdInfo,
                            ////    qty,
                            ////    wtQty));
                        }

                        break;

                    case SysCommon.ActSubTypes.PickingSingle:
                        if (this.ListViewMainActivity.SelectedItem is PickingDetail pickSingleDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(pickSingleDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(pickSingleDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = pickSingleDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = pickSingleDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Picked Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(pickSingleDetail.ActivityDetailSchemeID);

                            ////var qty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                            ////    ActivityItemPickingQuantityShipped;
                            ////var wtQty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                            ////    ActivityItemPickingWeightQuantityShipped;

                            ////this.ExtraInfoItems.Add("Total: " + SysCommon.NiceQuantityReadableString(
                            ////    this.ActDetail.ActItem.ProdInfo,
                            ////    qty,
                            ////    wtQty));
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketAddProduct:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketByProductDetail byProductDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(byProductDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(byProductDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Receiving);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = byProductDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = byProductDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Additional Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(byProductDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketFinished:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketFinishedDetail finishedDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(finishedDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(finishedDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Receiving);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = finishedDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = finishedDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Finished Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(finishedDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketMaterial:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketMaterialDetail materialDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(materialDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(materialDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = materialDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = materialDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("ByProduct Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(materialDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                        break;

                    case SysCommon.ActSubTypes.PurgeActivityData:
                        break;

                    case SysCommon.ActSubTypes.PutAwaySkid:
                        break;

                    case SysCommon.ActSubTypes.Receiving:
                        if (this.ListViewMainActivity.SelectedItem is ReceiveDetail receiveDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(receiveDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(receiveDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Receiving);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = receiveDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = receiveDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Received Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(receiveDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.RefreshData:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByItemSection:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByOrder:
                        break;

                    case SysCommon.ActSubTypes.Returns:
                        if (this.ListViewMainActivity.SelectedItem is ReturnDetail returnDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(returnDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(returnDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Receiving);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = returnDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = returnDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Returned Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(returnDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.SelectLocation:
                        break;

                    case SysCommon.ActSubTypes.SelectProduct:
                        break;

                    case SysCommon.ActSubTypes.SkidInfo:
                        if (this.ListViewMainActivity.SelectedItem is SkidInfoDetail skidInfoDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(skidInfoDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(skidInfoDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = skidInfoDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = skidInfoDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Detail Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(skidInfoDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TransferFrom:
                        if (this.ListViewMainActivity.SelectedItem is TransferDetail transferFromDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(transferFromDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(transferFromDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = transferFromDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = transferFromDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Transferred Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(transferFromDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TransferTo:
                        if (this.ListViewMainActivity.SelectedItem is TransferDetail transferToDetail)
                        {
                            // Get the line item location.
                            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(transferToDetail.LocationMapRefno));
                            var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                            // Using the line item warehouse, get the product default location.
                            var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                itemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(transferToDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Receiving);
                            var defProdLoc = LocationMapTasks.GetReadableLocation(
                                await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            var prodDesc = transferToDetail.ProductDescription.ToString();
                            var prodUnitSizeDesc = transferToDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Transferred Location: " + detailLoc.Trim());
                            ////this.ExtraInfoItems.Add(transferToDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.LoginStartup:
                        break;

                    case SysCommon.ActSubTypes.SplashStartup:
                        break;

                    case SysCommon.ActSubTypes.PrintProductLabel:
                        break;

                    case SysCommon.ActSubTypes.PickingSelectCriteria:
                        break;

                    case SysCommon.ActSubTypes.PickingFilter:
                        break;

                    case SysCommon.ActSubTypes.SubmitBatch:
                        break;

                    case SysCommon.ActSubTypes.CommunicationSetup:
                        break;

                    case SysCommon.ActSubTypes.SetupDefaults:
                        break;

                    case SysCommon.ActSubTypes.InventoryInquirySummary:
                        break;

                    case SysCommon.ActSubTypes.ConvertProduct:
                        break;

                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// Set add detail command button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private async void CmdAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                if (await CheckAllowedToAddDetail())
                {
                    var locMap = LocationMap.GetInstance();
                    if (!await LocationMapTasks.IsCompleteLocMap(locMap.CurrLocMapRefno))
                    {
                        throw new MessageOnlyException("Location must be set first.");
                    }

                    if (locMap.CurrLocMap.WhsLocMapRefno != this.ActDetail.ActItem.LocMap.WhsLocMapRefno)
                    {
                        throw new MessageOnlyException("Detail warehouse must match the line item warehouse.");
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                ////await this.DisplayDataItem();
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
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
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of
                // the length of the code and the fact is isn't human readable friendly.
                var tempItemProdSchemeData = new ActItemSchemeData();
                var schemeData = this.ActDetail.SchemeData;
                var skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (skidInfo.SkidFKRefno == 0)
                {
                    schemeData.LogisticUnitSkidNumber = string.Empty;
                    schemeData.LogisticUnitBatchLotNumber = string.Empty;
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

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> CheckAllowedToAddDetail()
            {
                var isAllowedAddManualDetail = true;
                if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                    (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                    this.ActSubType == SysCommon.ActSubTypes.Picking))
                {
                    var form = new FormAddManualDetailOverride();

                    isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                        await form.ShowDialog() == DialogResult.OK);
                }

                return isAllowedAddManualDetail;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task PerformAddDetail()
            {
                var isInsertReady = false;

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

                var parse = BarCodeParseFacade.ParseData(string.Empty);

                try
                {
                    await this.ActDisappearing();
                    var tempItemProdSchemeData = InitDetailBeforeGather();
                    isInsertReady = await SysShared.GatherProductData(
                        parse,
                        this.ActSubType,
                        this.ActDetail.ActItem,
                        tempItemProdSchemeData,
                        isInsertReady,
                        this.ActDetail,
                        true);

                    if (isInsertReady)
                    {
                        await this.InsertActivityDetail(this.ActDetail);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await this.ActAppearing();
                }
            }
        }

        /// <summary>
        /// Option menu click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText =
                    "Location: " + Environment.NewLine + currentReadableLocation);
            });

            var result = await this.DisplayActionSheet(
                "ActivityDetail Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            if (result == this.CmdLocationText && this.CmdLocationIsEnabled)
            {
                CmdLocationClick(sender, e);
            }
            else if (result == this.CmdSelectSkidText && this.CmdSelectSkidIsEnabled)
            {
                CmdSelectSkidClick(sender, e);
            }
            else if (result == this.CmdAddSkidText && this.CmdAddSkidIsEnabled)
            {
                CmdAddSkidClick(sender, e);
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdLocationClick(object sender, EventArgs e)
            {
                try
                {
                    await this.ActDisappearing();

                    ////var dspOverrideMsg = false;
                    ////if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                    ////{
                    ////    if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                    ////            this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                    ////        || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                    ////            this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                    ////    {
                    ////        dspOverrideMsg = false;
                    ////    }
                    ////    else
                    ////    {
                    ////        dspOverrideMsg = true;
                    ////    }
                    ////}

                    ////if (dspOverrideMsg)
                    ////{
                    ////    await Device.InvokeOnMainThreadAsync(async () =>
                    ////        await Application.Current.MainPage.DisplayAlert(
                    ////            this.ActDetail.ActItem.GetControlDescription(),
                    ////            "Default locations are enabled, location changes may be overridden.",
                    ////            Resource.OK));
                    ////}

                    var form = new FormDialogContainer(
                        EDialogTypes.SelectLocationMap,
                        this.ActDetail.ActItem.LocMap.WhsLocMapRefno,
                        true);
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        if (this.LocMap.ChosenLocMap.WhsLocMapRefno != this.ActDetail.ActItem.LocMap.WhsLocMapRefno)
                        {
                            throw new MessageOnlyException("Chosen warehouse must match the activity item warehouse.");
                        }

                        // Special validation for picking and filtering by section.
                        // Check if the scanned section matched the filtered one.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            var sectRefno = ((PickingActivity)this.ActDetail.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                            if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                            {
                                var sectName = ((PickingActivity)this.ActDetail.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                                var msg = $"Warning selected location section '{this.LocMap.CurrLocMap.SectName}' does" +
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

                    await Device.InvokeOnMainThreadAsync(() => Task.FromResult(this.CmdLocationText = "Location: " +
                        Environment.NewLine + LocationMapTasks.GetCurrentReadableLocation()));
                    await this.DisplayDataItem();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdSelectSkidClick(object sender, EventArgs e)
            {
                try
                {
                    await this.ActDisappearing();
                    var form = new FormDialogContainer(
                        EDialogTypes.SkidInfo,
                        this.ActDetail.ActItem,
                        this.ActSubType);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var xmlDoc = await form.GetXML();
                        this.ActDetail.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                        if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                            if (this.ActDetail.ActItem.ProdInfo.Code == currSkid.ProdInfo.Code &&
                                this.ActDetail.ActItem.ProdInfo.UnitBC == currSkid.ProdInfo.UnitBC &&
                                this.ActDetail.ActItem.ProdInfo.UnitCode == currSkid.ProdInfo.UnitCode)
                            {
                                this.CmdAddSkidIsEnabled = true;

                                this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                                this.LocMap.CurrLocMap = currSkid.LocMap;

                                this.CmdLocationIsEnabled = false;
                            }
                            else
                            {
                                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                    "Skid",
                                    "Skid product must match the selected line item product.",
                                    "OK"));

                                this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                                this.CmdSelectSkidText = "Select Skid";
                                this.CmdAddSkidIsEnabled = false;
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
                    var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdAddSkidClick(object sender, EventArgs e)
            {
                try
                {
                    await this.ActDisappearing();
                    if (await CheckAllowedToAddSkid())
                    {
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                "Add the available skid quantity?",
                                "Yes",
                                "No")))
                        {
                            this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                                this.ActDetail.ActItem.ProdInfo.Code,
                                this.ActDetail.ActItem.ProdInfo.Scheme,
                                this.ActDetail.SchemeData);

                            // IsReadyToApply will perform any validations, such as duplicate checks and quantity
                            // validations.
                            var xmlParam = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<imclient>"
                                + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                                + "<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                                + "<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                                + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                                + "</imclient>");

                            await this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                            await this.ActDetail.InsertSkid(this.ActDetail.ActItem.ActHdr.SkidInfo);
                            var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                            this.ActDetail.ActItem.AddToQuantityInActivityItem(
                                this.ActDetail.ActItem.SelectedActivityItem,
                                fillQty.Quantity,
                                fillQty.BaseQuantity,
                                fillQty.WeightQuantity);
                        }
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                    await this.ActAppearing();
                }

                //////////////////////////////////////////////////////////////////////////
                async Task<bool> CheckAllowedToAddSkid()
                {
                    var isAllowedAddManualDetail = true;
                    if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                        (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                        this.ActSubType == SysCommon.ActSubTypes.Picking))
                    {
                        if (!this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail)
                        {
                            await this.ActDisappearing();
                            var form = new FormAddManualDetailOverride();
                            isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                                await form.ShowDialog() == DialogResult.OK);
                            await this.ActAppearing();
                        }
                    }

                    return isAllowedAddManualDetail;
                }
            }
        }
    }
}
