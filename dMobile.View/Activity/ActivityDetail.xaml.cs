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
        private bool FirstOnAppearing { get; set; } = true;

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
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                await this.ScannerDisable();
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return isReadyToSave;
        }

        /// <summary>
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

            try
            {
                await this.ScannerDisable();
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            bool isSaveComplete = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                await this.ScannerDisable();
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
            catch (MessageOnlyException ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return isSaveComplete;

            //////////////////////////////////////////////////////////////////////////
            async Task PrintBatchedProductLabels()
            {
                // For each detail marked for printing
                string sendReq = string.Empty;
                string errDetail = string.Empty;
                string data = string.Empty;
                List<object> detail = this.ActDetail.ListDetails;

                for (int rowIdx = 0; rowIdx <= detail.Count - 1; rowIdx++)
                {
                    if (Convert.ToBoolean(detail[rowIdx].GetType().GetProperty(
                        "IsBatchProductLabel")?.GetValue(detail[rowIdx])))
                    {
                        XDocument request = XDocument.Parse(
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
                        sendReq = await this.ActDetail.CreateSubmitProductLabelXML(request);
                        errDetail = string.Empty;
                        data = SysCommon.SubmitComm(sendReq);
                        if (data.Length > 0)
                        {
                            if (data.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument docError = new XmlDocument();
                                docError.LoadXml(data);
                                string errorDetail = docError.DocumentElement["detail"].InnerText;

                                if (errorDetail.Contains("MSGBOX-"))
                                {
                                    throw new MessageOnlyException(errorDetail);
                                }
                                else
                                {
                                    throw new Exception(errorDetail);
                                }
                            }
                        }
                        else
                        {
                            errDetail = "Unexpected print product label(s) error.";
                        }

                        if (string.IsNullOrEmpty(errDetail))
                        {
                            detail[rowIdx].GetType().GetProperty("IsBatchProductLabel")?.SetValue(detail[rowIdx], false);
                        }
                        else
                        {
                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    "Printing Label(s)",
                                    errDetail,
                                    "Retry",
                                    "Abort")))
                            {
                                rowIdx--;
                            }
                            else
                            {
                                break;
                            }
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
            bool cancelChanges = true;

            try
            {
                await this.ScannerDisable();

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
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
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
            finally
            {
                await this.ScannerEnable();
            }

            return cancelChanges;
        }

        /// <summary>
        /// Get the activity detail return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity detail return information.</returns>
        public async Task<XDocument> GetXML()
        {
            SFillQuantity fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();

            XDocument xmldocParams = XDocument.Parse(
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
        public void OnLayerStatOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public void OnLayerProgOut(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs dataLayerEventArgs)
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
                try
                {
                    // Disable the scanner even though the on disappearing event will turn it off also
                    // upon closing. SaveChanges or CancelChanges may show dialogs.
                    await this.ScannerDisable();
                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                    else if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
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
                if (this.FirstOnAppearing)
                {
                    this.FirstOnAppearing = false;
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
            catch (MessageOnlyException ex)
            {
                this.FirstOnAppearing = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.FirstOnAppearing = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    string msg = "This activity unexpectedly failed to initialize correctly." +
                        " Please exit and try again.";
                    FormErrorReport errorReport = new FormErrorReport(msg, ex);
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
            ////await Device.InvokeOnMainThreadAsync(() =>
            ////{
            ////    this.ListViewMainActivity.ScrollTo(
            ////    this.ListViewMainActivity.SelectedItem,
            ////    ScrollToPosition.MakeVisible,
            ////    false);
            ////});

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
                            await OnAppearingDelKey();
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
                        await this.ScannerDisable();

                        // Check if there is an item in the data grid selected to delete.
                        IsItemSelected();

                        // Ask if the deletion is a entire skid or item.
                        bool isDelItem = false;
                        bool isDelSkid = false;
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
                            string msg = $"Do you wish to delete all detail belonging to this skid" +
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
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
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
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void IsItemSelected()
                    {
                        bool isItemSelected = this.ListViewMainActivity.SelectedItem != null
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
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task OnAppearingDelKeySkid()
                    {
                        // Determine the selected detail skid code.
                        int currIdx = this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem);
                        string skidCode = Convert.ToString(this.ActDetail.ListDetails[currIdx].GetType().GetProperty(
                                "ActivityDetailLogisticUnitSkidNumber")?.GetValue(
                                    this.ActDetail.ListDetails[currIdx]));
                        SysSkidInfo skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(skidCode);

                        if (skidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            throw new Exception($"Unable to determine the skid '{skidCode}'" +
                                $" information to delete.");
                        }

                        // Submit the request, and delete it from the data table.
                        await this.ActDetail.DeleteSkid(skidInfo);

                        await this.RefreshBySubType();
                        await this.DesignGrid();

                        // Update the activity line item quantities.
                        // Because many detail line can be affected we need to total all the details.
                        SFillQuantity fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedActivityItem,
                            fillQty.Quantity,
                            fillQty.BaseQuantity,
                            fillQty.WeightQuantity);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF1Key()
                {
                    try
                    {
                        await this.ScannerDisable();
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;

                            // TODO: DEV-5619
                            // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                            await this.Close();
                        }
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
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
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF3Key()
                {
                    // TODO: DEV-5619
                    // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile
                    try
                    {
                        await this.ScannerDisable();
                        string msg = await this.ActDetail.ActItem.GetTotalQuantity();
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                "OK"));
                        }
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
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
                        _ = this.ListViewMainActivity.Focus();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF4Key()
                {
                    try
                    {
                        await this.ScannerDisable();
                        StringBuilder xmlString = new StringBuilder();
                        XmlWriter xml = XmlWriter.Create(xmlString);
                        await CheckIfActivityCanBeConfirmed();
                        SubmitDetailConfirmation(xmlString);
                        MarkDetailAsConfirmed();
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                            _ = await errorReport.ShowDialog();
                        });
                    }
                    finally
                    {
                        await this.ScannerEnable();
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
                        string reqData = SysCommon.SubmitComm(xmlString.ToString());
                        if (reqData.Length > 0)
                        {
                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument docError = new XmlDocument();
                                docError.LoadXml(reqData);
                                string errorDetail = docError.DocumentElement["detail"].InnerText;
                                if (errorDetail.Contains("MSGBOX-"))
                                {
                                    throw new MessageOnlyException(errorDetail);
                                }
                                else
                                {
                                    throw new Exception(errorDetail);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unexpected confirm detail error.");
                        }
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
                                + "<refno>" + this.ActDetail.DetailFKRefno + "</refno>"
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
                                + "<refno>" + this.ActDetail.DetailFKRefno + "</refno>"
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
                                + "<refno>" + this.ActDetail.DetailFKRefno + "</refno>"
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
        /// Search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                await this.DesignGrid();
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
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
            }
        }

        /// <summary>
        /// Data grid activity detail selected detail event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.SelectActivityDetail(e.SelectedItem);
            this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFKRefno;
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

            await this.ScannerDisable();

            int prodUnitRefno = 0;
            int svdActDetailFKRefno = this.ActDetail.DetailFKRefno;
            BarCodeParse parse = scanArgs.ParseInfo;

            try
            {
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

                        ProductInfo prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        // Check if the scanned product matches the select activity line item product.
                        if (prodUnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                        {
                            throw new Exception("Scanned product does not match the" +
                                " selected line item product.");
                        }

                        // If there is a selected skid check if the scanned product matches.
                        SSkidInfo currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                        if (currSkid.SkidFKRefno != 0)
                        {
                            if (currSkid.ProdInfo.UnitRefno != prodUnitRefno)
                            {
                                throw new Exception("Scanned product does not match the selected skid product.");
                            }
                        }

                        // Get the activity item reference number from the identified location reference number
                        // and product reference number.
                        int tempActItemRefno = await this.ActDetail.ActItem.GetActivityItemRefno(
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
                            bool isInsertReady = false;

                            // A skid may contain 'default' scheme data, fill the activity detail
                            // with the default skid schema.
                            SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                            SProductSchemeData schemeData = this.ActDetail.SchemeData;
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

                        SysSkidInfo skidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
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
                        this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActDetail.ActItem.ProdInfo.Code,
                            this.ActDetail.ActItem.ProdInfo.Scheme,
                            this.ActDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        XDocument xmlParam = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<itm_refno>" + this.ActDetail.ActItem.ItemFKRefno + "</itm_refno>"
                        + "<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                        + "<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                        + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                        + "</imclient>");

                        await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
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

                        SFillQuantity fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
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
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException ex)
            {
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
                await this.ScannerEnable();
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier()
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier));
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();

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
                    await this.ScannerDisable();
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
                        FormErrorReport errRpt = new FormErrorReport(
                            this.ActSubType,
                            ex,
                            this.ActDetail.GetControlDescription(),
                            "Refresh");
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
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
            if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
            {
                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                object selectedItem = null;
                if (this.SelectedActivityDetailFKRefno != 0)
                {
                    foreach (object item in this.ActDetail.ListDetails)
                    {
                        if (this.SelectedActivityDetailFKRefno == this.ActDetail.GetFKRefno(item))
                        {
                            selectedItem = item;
                            this.ListViewMainActivity.SelectedItem = selectedItem;
                            break;
                        }
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                    await this.ActDetail.SelectActivityDetail(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActDetail.SelectActivityDetail(selectedItem);
                    this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFKRefno;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                    ////this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedActivityDetail = null;
                this.ActDetail.DetailFKRefno = 0;
                this.ActDetail.DetailRefno = 0;
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
            actDetail.DetailFKRefno = 0;
            XDocument xmlDoc = actDetail.GetXML(actDetail);
            await actDetail.Insert(xmlDoc);

            int detailIdx = this.ActDetail.FindListDetailIndex(activityDetailRefno: actDetail.DetailRefno);
            if (detailIdx >= 0)
            {
                this.ActDetail.UpdateListDetail(detailIdx);
                dMobile.Resources.Types.ActivityDetail detail = (Resources.Types.ActivityDetail)this.ActDetail.ListDetails[detailIdx];
                this.SelectedActivityDetailFKRefno = detail.ActivityDetailFKRefno;
                this.MainActivityItems[detailIdx] = detail;
            }
            else
            {
                Resources.Types.ActivityDetail inserted = this.ActDetail.InsertListDetail(this.MainActivityItems);
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

                SSkidInfo currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
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
                            string loc = adjustmentDetail.Location;
                            string locCode = Convert.ToString(adjustmentDetail.LocationCode);
                            string locName = adjustmentDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(adjustmentDetail.ActivityDetailSchemeID);
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
                        if (this.ListViewMainActivity.SelectedItem is CountDetail countDetail)
                        {
                            // Get the cycle count line item location.
                            SLocMap countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                Convert.ToInt32(countDetail.LocationMapRefno));
                            string countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                            // Using the cycle count line item warehouse, get the product default
                            // picking location.
                            int defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                countItemLocMap.WhsLocMapRefno,
                                Convert.ToInt32(countDetail.ProductRefno),
                                SysCommon.ProdLocMapTypes.Picking);
                            string defProdLoc = LocationMapTasks.GetReadableLocation(
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));
                            string prodDesc = countDetail.ProductDescription.ToString();
                            string prodUnitSizeDesc = countDetail.ProductUnitSizeDescription.ToString();
                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Count Location: " + countLoc.Trim());
                            this.ExtraInfoItems.Add(countDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.DataStatus:
                        break;

                    case SysCommon.ActSubTypes.InventoryInquiry:
                        if (this.ActDetail.ActItem.ProdInfo.Refno <= 0)
                        {
                            this.ExtraInfoItems.Add(string.Format("PRODUCT: {0}", "All"));
                        }
                        else
                        {
                            this.ExtraInfoItems.Add(string.Format("PRODUCT: {0}", ProductInfoTasks.GetCurrReadableProdBCUnit(
                                this.ActDetail.ActItem.ProdInfo)));
                        }

                        this.ExtraInfoItems.Add(string.Format("LOCATION: {0}", LocationMapTasks.GetCurrentReadableLocation()));
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
                        if (this.ListViewMainActivity.SelectedItem is CountDetail physicalCountDetail)
                        {
                            string loc = physicalCountDetail.Location;
                            string locCode = Convert.ToString(physicalCountDetail.LocationCode);
                            string locName = physicalCountDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(physicalCountDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.Picking:
                        if (this.ListViewMainActivity.SelectedItem is PickingDetail pickingDetail)
                        {
                            this.ExtraInfoItems.Add(LocationMapTasks.GetCurrentReadableLocation(isSeparateSlotCoords: true));

                            decimal qty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                                ActivityItemPickingQuantityShipped;

                            decimal wtQty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                                ActivityItemPickingWeightQuantityShipped;

                            this.ExtraInfoItems.Add("Total: " + SysCommon.NiceQuantityReadableString(
                                this.ActDetail.ActItem.ProdInfo,
                                qty,
                                wtQty));
                        }

                        break;

                    case SysCommon.ActSubTypes.PickingSingle:
                        if (this.ListViewMainActivity.SelectedItem is PickingDetail pickingSingleDetail)
                        {
                            this.ExtraInfoItems.Add(pickingSingleDetail.ActivityDetailSchemeID);

                            decimal qty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                                ActivityItemPickingQuantityShipped;
                            decimal wtQty = (this.ActDetail.ActItem.SelectedActivityItem as PickingItem).
                                ActivityItemPickingWeightQuantityShipped;

                            this.ExtraInfoItems.Add("Total: " + SysCommon.NiceQuantityReadableString(
                                this.ActDetail.ActItem.ProdInfo,
                                qty,
                                wtQty));
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketAddProduct:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketByProductDetail productionTicketByProductDetail)
                        {
                            string loc = productionTicketByProductDetail.Location;
                            string locCode = Convert.ToString(productionTicketByProductDetail.LocationCode);
                            string locName = productionTicketByProductDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(productionTicketByProductDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketFinished:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketFinishedDetail productionTicketFinishedDetaill)
                        {
                            string loc = productionTicketFinishedDetaill.Location;
                            string locCode = Convert.ToString(productionTicketFinishedDetaill.LocationCode);
                            string locName = productionTicketFinishedDetaill.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(productionTicketFinishedDetaill.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TicketMaterial:
                        if (this.ListViewMainActivity.SelectedItem is ProductionTicketMaterialDetail productionTicketMaterialDetail)
                        {
                            string loc = productionTicketMaterialDetail.Location;
                            string locCode = Convert.ToString(productionTicketMaterialDetail.LocationCode);
                            string locName = productionTicketMaterialDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(productionTicketMaterialDetail.ActivityDetailSchemeID);
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
                            string loc = receiveDetail.Location;
                            string locCode = Convert.ToString(receiveDetail.LocationCode);
                            string locName = receiveDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(receiveDetail.ActivityDetailSchemeID);
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
                            string loc = returnDetail.Location;
                            string locCode = Convert.ToString(returnDetail.LocationCode);
                            string locName = returnDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(returnDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.SelectLocation:
                        break;

                    case SysCommon.ActSubTypes.SelectProduct:
                        break;

                    case SysCommon.ActSubTypes.SkidInfo:
                        if (this.ListViewMainActivity.SelectedItem is SkidInfoDetail skidInfoDetail)
                        {
                            this.ExtraInfoItems.Add(skidInfoDetail.ActivityDetailSchemeID);
                            this.ExtraInfoItems.Add("PRODUCT: " + this.ActDetail.ActItem.ProdInfo.Code +
                                this.ActDetail.ActItem.ProdInfo.UnitBC + " " +
                                this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
                        }

                        break;

                    case SysCommon.ActSubTypes.TransferFrom:
                        if (this.ListViewMainActivity.SelectedItem is TransferDetail transferFromDetail)
                        {
                            string loc = transferFromDetail.Location;
                            string locCode = Convert.ToString(transferFromDetail.LocationCode);
                            string locName = transferFromDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(transferFromDetail.ActivityDetailSchemeID);
                        }

                        break;

                    case SysCommon.ActSubTypes.TransferTo:
                        if (this.ListViewMainActivity.SelectedItem is TransferDetail transferToDetail)
                        {
                            string loc = transferToDetail.Location;
                            string locCode = Convert.ToString(transferToDetail.LocationCode);
                            string locName = transferToDetail.LocationName;

                            this.ExtraInfoItems.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                            this.ExtraInfoItems.Add(transferToDetail.ActivityDetailSchemeID);
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
                    LocationMap locMap = LocationMap.GetInstance();
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
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, gatherEx);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msgEx.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", ex.Message, "OK"));
            }
            finally
            {
                await this.DisplayDataItem();
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            SActItemSchemeData InitDetailBeforeGather()
            {
                this.ActDetail.Quantity = 0;
                this.ActDetail.BaseQuantity = 0;
                this.ActDetail.WeightQuantity = 0;
                this.ActDetail.SchemeID = string.Empty;
                this.ActDetail.UniqueSchemeID = string.Empty;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of
                // the length of the code and the fact is isn't human readable friendly.
                SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                SProductSchemeData schemeData = this.ActDetail.SchemeData;
                SSkidInfo skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (skidInfo.SkidFKRefno != 0)
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
                bool isAllowedAddManualDetail = true;
                if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                    (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                    this.ActSubType == SysCommon.ActSubTypes.Picking))
                {
                    FormAddManualDetailOverride form = new FormAddManualDetailOverride();

                    isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                        await form.ShowDialog() == DialogResult.OK);
                }

                return isAllowedAddManualDetail;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task PerformAddDetail()
            {
                bool isInsertReady = false;

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

                BarCodeParse parse = BarCodeParseFacade.ParseData(string.Empty);

                try
                {
                    await this.ActDisappearing();
                    SActItemSchemeData tempItemProdSchemeData = InitDetailBeforeGather();
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

            try
            {
                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText =
                        "Location: " + Environment.NewLine + currentReadableLocation);
                });

                string result = await this.DisplayActionSheet(
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
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
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
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdLocationClick(object sender, EventArgs e)
            {
                await this.ActDisappearing();

                bool dspOverrideMsg = false;
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                            this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                        || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                            this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                    {
                        dspOverrideMsg = false;
                    }
                    else
                    {
                        dspOverrideMsg = true;
                    }
                }

                if (dspOverrideMsg)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.ActItem.GetControlDescription(),
                            "Default locations are enabled, location changes may be overridden.",
                            Resource.OK));
                }

                FormDialogContainer form = new FormDialogContainer(
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
                        int sectRefno = ((PickingActivity)this.ActDetail.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                        {
                            string sectName = ((PickingActivity)this.ActDetail.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            string msg = $"Warning selected location section '{this.LocMap.CurrLocMap.SectName}' does" +
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

                await this.ActAppearing();
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdSelectSkidClick(object sender, EventArgs e)
            {
                await this.ActDisappearing();
                FormDialogContainer form = new FormDialogContainer(
                    EDialogTypes.SkidInfo,
                    this.ActDetail.ActItem,
                    this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    XDocument xmlDoc = await form.GetXML();
                    this.ActDetail.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        SSkidInfo currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

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

                await this.ActAppearing();
            }

            //////////////////////////////////////////////////////////////////////////
            async void CmdAddSkidClick(object sender, EventArgs e)
            {
                if (await CheckAllowedToAddSkid())
                {
                    if (await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            "Add the ENTIRE skid?",
                            "Yes",
                            "No")))
                    {
                        this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActDetail.ActItem.ProdInfo.Code,
                            this.ActDetail.ActItem.ProdInfo.Scheme,
                            this.ActDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity
                        // validations.
                        XDocument xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<itm_refno>" + this.ActDetail.ActItem.ItemFKRefno + "</itm_refno>"
                            + "<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                            + "<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                            + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                            + "</imclient>");

                        await this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                        await this.ActDetail.InsertSkid(this.ActDetail.ActItem.ActHdr.SkidInfo);
                        SFillQuantity fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedActivityItem,
                            fillQty.Quantity,
                            fillQty.BaseQuantity,
                            fillQty.WeightQuantity);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task<bool> CheckAllowedToAddSkid()
                {
                    bool isAllowedAddManualDetail = true;
                    if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                        (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                        this.ActSubType == SysCommon.ActSubTypes.Picking))
                    {
                        if (!this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail)
                        {
                            await this.ActDisappearing();
                            FormAddManualDetailOverride form = new FormAddManualDetailOverride();
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
