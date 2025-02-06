// <copyright file="ActivityItem.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Resources.Types.Items;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Activity items control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItem : IAppArea
    {
        /// <summary>
        ///     Define a value indicating whether adding a product is enabled.
        /// </summary>
        private bool cmdAddProductIsEnabled;

        /// <summary>
        ///     Define the add a product text information.
        /// </summary>
        private string cmdAddProductText = string.Empty;

        /// <summary>
        ///     Define a value indicating whether adding a skid is enabled.
        /// </summary>
        private bool cmdAddSkidIsEnabled;

        /// <summary>
        ///     Define the add skid text information.
        /// </summary>
        private string cmdAddSkidText = string.Empty;

        /// <summary>
        ///     Define a value indicating whether clear selected a skid is enabled.
        /// </summary>
        private bool cmdClearSelectedSkidIsEnabled;

        /// <summary>
        ///     Define the clear selected skid text information.
        /// </summary>
        private string cmdClearSelectedSkidText = string.Empty;

        /// <summary>
        ///     Define a value indicating whether the filter selection is enabled.
        /// </summary>
        private bool cmdFilterIsEnabled;

        /// <summary>
        ///     Define the activity filter text information.
        /// </summary>
        private string cmdFilterText = string.Empty;

        /// <summary>
        ///     Define a value indicating whether the location selection is enabled.
        /// </summary>
        private bool cmdLocationIsEnabled;

        /// <summary>
        ///     Define the warehouse map location text information.
        /// </summary>
        private string cmdLocationText = string.Empty;

        /// <summary>
        ///     Define a value indicating whether the skid selection is enabled.
        /// </summary>
        private bool cmdSelectSkidIsEnabled;

        /// <summary>
        ///     Define the select skid text information.
        /// </summary>
        private string cmdSelectSkidText = string.Empty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityItem" /> class.
        /// </summary>
        /// <param name="activity"><see cref="Activity" /> Activity header instance.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity subtype.</param>
        public ActivityItem(Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // If the skid was not set on the header activity define an empty one.
            activity.SkidInfo ??= new SysSkidInfo();

            this.ActSubType = actSubTypes;
            this.ActItem = ActItemFacade.GetActItem(activity, this.ActSubType);
            this.ActItem.ActHdr = activity;

            this.SysEnv.SysSettings.IsCountActivityItemRefreshData = true;
            this.IsDataLoading = false;
            this.IsIndicatorRunning = false;

            this.lblSkidNumber.Text = "No skid selected";
            this.CmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;
            this.CmdLocationText = "Set Location";
            this.CmdFilterText = "Filter";
            this.CmdSelectSkidText = "Select Skid";
            this.CmdClearSelectedSkidText = "Clear Skid";
            this.CmdAddProductText = "Add Product";
            this.CmdAddSkidText = "Add Skid";
            this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
            this.CmdClearSelectedSkidIsEnabled =
                this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0 &&
                this.CmdAddSkidIsEnabled &&
                this.CmdSelectSkidIsEnabled;

            ParseAndSeedBarCodeModifier();
            this.Title = this.ActItem.GetControlDescription();
            this.SearchTimer = new SysTimer(this.SearchTimerTickCallback);
            this.IsFirstAppearance = true;
            return;

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();
                if (!this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    return;
                }

                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
                foreach (var item in modifiers)
                {
                    this.BarCodeModifier1Picker.Items.Add(item);
                }

                if (this.BarCodeModifier1Picker.Items.Count > 0)
                {
                    this.BarCodeModifier1Picker.IsVisible = true;
                    this.BarCodeModifier1Picker.IsEnabled = true;
                }

                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
            }
        }

        /// <summary>
        ///     Gets or sets the selected activity item foreign key reference number.
        /// </summary>
        private int SelectedActivityItemFkRefno { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show a data valid message or raise an error.
        /// </summary>
        private bool IsShowDataValidMsg { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether data changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets the warehouse location map instance.
        /// </summary>
        private LocationMap LocMap { get; } = LocationMap.GetInstance();

        /// <summary>
        ///     Gets or sets the activity item instance.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets a list of extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraLineItemInfo { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets a list of line item detail.
        /// </summary>
        private ObservableCollection<string> LineItemInfo { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets a list of list view activity items.
        /// </summary>
        private ObservableCollection<object> ActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        ///     Gets the options menu items list.
        /// </summary>
        private List<string> OptionsMenuItems { get; } = new List<string>();

        /// <summary>
        ///     Gets or sets a counter that tracks the number of screen taps, for double click.
        /// </summary>
        private int NumberOfScreenTapsForDoubleClick { get; set; }

        /// <summary>
        ///     Gets or sets the sender for tracking the number of screen taps, for double click.
        /// </summary>
        private object PreviousSenderForDoubleClick { get; set; }

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
        ///     Gets or sets the command filter text.
        /// </summary>
        private string CmdFilterText
        {
            get => this.cmdFilterText;
            set
            {
                this.cmdFilterText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command filter is enabled.
        /// </summary>
        private bool CmdFilterIsEnabled
        {
            get => this.cmdFilterIsEnabled;
            set
            {
                this.cmdFilterIsEnabled = value;
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
        ///     Gets or sets the command add product text.
        /// </summary>
        private string CmdAddProductText
        {
            get => this.cmdAddProductText;
            set
            {
                this.cmdAddProductText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command add product is enabled.
        /// </summary>
        private bool CmdAddProductIsEnabled
        {
            get => this.cmdAddProductIsEnabled;
            set
            {
                this.cmdAddProductIsEnabled = value;
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

        /// <summary>
        ///     Gets or sets the command clear selected skid text.
        /// </summary>
        private string CmdClearSelectedSkidText
        {
            get => this.cmdClearSelectedSkidText;
            set
            {
                this.cmdClearSelectedSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command clear selected skid is enabled.
        /// </summary>
        private bool CmdClearSelectedSkidIsEnabled
        {
            get => this.cmdClearSelectedSkidIsEnabled;
            set
            {
                this.cmdClearSelectedSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the search text box is in focus.
        /// </summary>
        private bool IsSearchEntryFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code modifier picker is in focus.
        /// </summary>
        private bool IsBarCodeModifierPickerFocus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data is being loaded.
        /// </summary>
        private bool IsDataLoading { get; set; }

        /// <summary>
        ///     Gets or sets the search text box original text.
        /// </summary>
        private string SearchTextOrig { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the system timer.
        /// </summary>
        private SysTimer SearchTimer { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this is the first appearance of the form.
        /// </summary>
        private bool IsFirstAppearance { get; set; }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;
            if (isReadyToSave && this.ActSubType == SysCommon.ActSubTypes.Picking)
            {
                isReadyToSave = await IsReadyToSavePicking();
            }

            return isReadyToSave;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToSavePicking()
            {
                var isReadyToSavePicking = true;
                try
                {
                    await this.ItemQuantityUnderCheck();

                    // Added extra check for Lekker Foods, check all items.
                    this.AllItemsQuantityUnderCheck();
                }
                catch (MessageOnlyException ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                    {
                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                            if (await Device.InvokeOnMainThreadAsync(
                                    async () =>
                                        await this.DisplayAlert(
                                            "Quantity Validation",
                                            ex.Message + Environment.NewLine + "Continue?",
                                            "Yes",
                                            "No") ==
                                        false))
                            {
                                isReadyToSavePicking = false;
                                this.IsShowDataValidMsg = false;
                            }

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                            await Device.InvokeOnMainThreadAsync(
                                async () => await this.DisplayAlert(
                                    "Quantity Validation",
                                    ex.Message,
                                    "OK"));
                            isReadyToSavePicking = false;
                            this.IsShowDataValidMsg = false;

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                return isReadyToSavePicking;
            }
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (await this.IsReadyToSave())
                {
                    // Print all receiving skid labels.
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                        !this.SysEnv.SysSettings.IsDisconnectedMode &&
                        this.SysEnv.SysSettings.IsSkidTrackingModule &&
                        this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
                    {
                        await this.PrintSkidLabels();
                    }

                    this.IsChangesMade = false;
                    isSaveComplete = true;
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch-all
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException("Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///     Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;
            try
            {
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                cancelChanges = false;
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            // Prairie meats like Lekker requirement to check quantity under even though we are
            // canceling / closing.
            // See WO# 12080
            if (isReadyToClose && this.ActSubType == SysCommon.ActSubTypes.Picking)
            {
                isReadyToClose = await IsReadyToClosePicking();
            }

            return isReadyToClose;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToClosePicking()
            {
                var isReadyToClosePicking = true;

                try
                {
                    this.IsShowDataValidMsg = true;
                    await this.ItemQuantityUnderCheck();

                    // Added extra check for Lekker, check all items.
                    this.AllItemsQuantityUnderCheck();
                }
                catch (MessageOnlyException ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                    {
                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                            if (await Device.InvokeOnMainThreadAsync(
                                    async () => await this.DisplayAlert(
                                                    "Quantity Validation",
                                                    ex.Message + Environment.NewLine + "Continue?",
                                                    "Yes",
                                                    "No") ==
                                                false))
                            {
                                isReadyToClosePicking = false;
                                this.IsShowDataValidMsg = false;
                            }

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                    await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                            isReadyToClosePicking = false;
                            this.IsShowDataValidMsg = false;
                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                            break;

                        default:
                            throw new NotImplementedException(
                                "Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                    }
                }

                return isReadyToClosePicking;
            }
        }

        /// <summary>
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var doc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");

            return doc;
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
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            //////////////////////////////////////////////////////////////////////////
            void BackButtonCallback()
            {
                Task.Run(
                    async () =>
                    {
                        if (await this.CancelChanges())
                        {
                            this.SysEnv.SysSettings.IsCountActivityItemRefreshData = true;
                            this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                            await this.ScannerDisable();
                            _ = base.OnBackButtonPressed();
                            await this.Close();
                        }
                    });
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
                propertyName == nameof(this.CmdFilterText) ||
                propertyName == nameof(this.CmdFilterIsEnabled) ||
                propertyName == nameof(this.CmdSelectSkidText) ||
                propertyName == nameof(this.CmdSelectSkidIsEnabled) ||
                propertyName == nameof(this.CmdAddProductText) ||
                propertyName == nameof(this.CmdAddProductIsEnabled) ||
                propertyName == nameof(this.CmdAddSkidText) ||
                propertyName == nameof(this.CmdAddSkidIsEnabled) ||
                propertyName == nameof(this.CmdClearSelectedSkidText) ||
                propertyName == nameof(this.CmdClearSelectedSkidIsEnabled))
            {
                this.PopulateOptionsMenu();
            }

            if (propertyName == nameof(this.CmdSelectSkidText))
            {
                this.lblSkidNumber.IsVisible = true;
                this.lblSkidNumber.Text = this.CmdSelectSkidText != "Select Skid"
                    ? this.CmdSelectSkidText
                    : "No skid selected";
            }
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                this.NumberOfScreenTapsForDoubleClick = 0;
                this.SubscribeKeyUpHandlers();
                this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
                await this.EnableMenuItems();
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.LoadAndBind();
                    await this.ScannerEnable();
                }

                if (!this.IsSearchEntryFocused)
                {
                    this.ListViewMainActivity.Focus();
                    await this.FindSelectedItemAsync();
                    await this.DisplayLineItemDetail();
                    await this.DisplayDataItem();
                }
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(this.ActSubType, ex);
                _ = await errRpt.ShowDialog();
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Get the control description.
        /// </summary>
        /// <returns><see cref="string" /> Control description.</returns>
        private static string GetControlDescription()
        {
            return "Activity Item";
        }

        /// <summary>
        ///     SearchTimer tick call back.
        /// </summary>
        private void SearchTimerTickCallback()
        {
            this.SearchTimer.Stop();
            if (this.IsSearchEntryFocused &&
                this.txtSearchItem.Text != this.SearchTextOrig)
            {
                this.SearchTextOrig = this.txtSearchItem.Text;
                this.LoadAndBind().FireAndForgetSafeAsync();
            }

            // Did not start the timer again here because the timer is started in the OnTextChange event.
        }

        /// <summary>
        ///     Load and bind the data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task LoadAndBind()
        {
            try
            {
                // We only want to load data if it is not already loading.
                if (!this.IsDataLoading)
                {
                    this.IsDataLoading = true;

                    // The activity indicator control messes up the search text control focus.
                    // Disable it for searching.
                    if (!this.IsSearchEntryFocused)
                    {
                        this.IsIndicatorRunning = true;
                    }

                    await RefreshData();

                    // It is possible because of disconnected mode to have unsubmitted data. We will sort the items
                    // and place the new items at the bottom of the list.
                    this.ActItem.PlaceNewItemsAtBottomOfList(this.ActItem.ListItems);
                    var mainActivityItems =
                        this.ActItem.CreateObservableCollectionFromListItems(this.ActItem.ListItems);
                    var gridItems = this.ActItem.DefineGrid();
                    this.ActItem.CalcMaxDataColWidth(gridItems, this.ActItem.ListItems);
                    this.GridMainActivity.Children.Add(gridItems);
                    this.ListViewMainActivity.ItemTemplate = this.ActItem.GetDataTemplate();
                    this.ActivityItems = mainActivityItems;
                    this.ListViewMainActivity.ItemsSource = this.ActivityItems;
                    if (this.ActivityItems == null || this.ActivityItems.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(() => this.ExtraLineItemInfo.Clear());
                    }
                    else
                    {
                        await this.FindSelectedItemAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
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

            return;

            ///////////////////////////////////////////////////////////
            async Task RefreshData()
            {
                // If not in disconnected mode, refresh data from the communication server.
                if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    await this.ActItem.Refresh(this.ActItem.ActHdr.XmlDataCriteria);
                }

                // Get the updated list from the database.
                this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);
            }
        }

        /// <summary>
        /// Find the selected item.
        /// </summary>
        private async Task FindSelectedItemAsync()
        {
            object selectedItem = null;
            var selectedIndex = -1;
            if (this.ActItem.ListItems.Count > 0)
            {
                if (this.ActItem.SelectedListIdx >= 0)
                {
                    selectedIndex = this.ActItem.SelectedListIdx;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }

                if (selectedItem == null &&
                    this.SelectedActivityItemFkRefno > 0)
                {
                    selectedIndex = 0;
                    foreach (var item in this.ActItem.ListItems)
                    {
                        if (this.SelectedActivityItemFkRefno == this.ActItem.GetFkRefno(item))
                        {
                            selectedItem = item;
                            break;
                        }

                        selectedIndex++;
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    selectedIndex = 0;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }
            }

            this.ActItem.SelectedListIdx = selectedIndex;
            await this.ActItem.AssignActivityItem(selectedItem);
            Device.BeginInvokeOnMainThread(
                () =>
                {
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });
        }

        /// <summary>
        /// Find the selected item.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int" /> Activity item foreign key reference number.</param>
        private async Task FindSelectedItemAsync(int activityItemFkRefno)
        {
            object selectedItem = null;
            var selectedIndex = -1;
            if (this.ActItem.ListItems.Count > 0)
            {
                if (this.ActItem.SelectedListIdx >= 0)
                {
                    selectedIndex = this.ActItem.SelectedListIdx;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }

                if (selectedItem == null &&
                    activityItemFkRefno > 0)
                {
                    selectedIndex = 0;
                    foreach (var item in this.ActItem.ListItems)
                    {
                        if (activityItemFkRefno == this.ActItem.GetFkRefno(item))
                        {
                            selectedItem = item;
                            break;
                        }

                        selectedIndex++;
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    selectedIndex = 0;
                    selectedItem = this.ActItem.ListItems[selectedIndex];
                }
            }

            this.ActItem.SelectedListIdx = selectedIndex;
            await this.ActItem.AssignActivityItem(selectedItem);
            Device.BeginInvokeOnMainThread(
                () =>
                {
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });
        }

        /// <summary>
        ///     Populate the options menu items.
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();

            if (this.CmdLocationIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdLocationText);
            }

            if (this.CmdFilterIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdFilterText);
            }

            if (this.CmdSelectSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdSelectSkidText);
            }

            if (this.CmdClearSelectedSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdClearSelectedSkidText);
            }

            if (this.CmdAddProductIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdAddProductText);
            }

            if (this.CmdAddSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdAddSkidText);
            }
        }

        /// <summary>
        ///     Subscribe the key up handlers.
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

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (keycode)
                {
                    case Keycode.Clear:
                        await AppearingClearKey();
                        break;

                    case Keycode.Del:
                        if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                        {
                            await AppearingDelKey();
                        }

                        break;

                    case Keycode.DpadDown:
                        AppearingDpadDown();
                        break;

                    case Keycode.DpadUp:
                        AppearingDpadUp();
                        break;

                    case Keycode.Enter:
                        if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                        {
                            await AppearingEnterKey();
                        }

                        break;

                    case Keycode.F1:
                        await AppearingF1Key();
                        break;

                    case Keycode.F2:
                        if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                        {
                            await AppearingF2Key();
                        }

                        break;

                    case Keycode.F3:
                        if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                        {
                            await AppearingF3Key();
                        }

                        break;

                    case Keycode.F4:
                        if (!(this.IsSearchEntryFocused || this.IsBarCodeModifierPickerFocus))
                        {
                            ////await Task.Run(() =>
                            ////{
                            ////    // Explicit GC, Xamarin forms not disposing of objects properly
                            ////    GC.Collect();
                            ////    GC.WaitForPendingFinalizers();
                            ////});
                            await AppearingF4Key();
                        }

                        break;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
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
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingEnterKey()
            {
                if (this.ListViewMainActivity.SelectedItem == null ||
                    Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
                                "Please select an item.",
                                Resource.OK));
                }
                else
                {
                    await this.ListViewMainActivity_KeyUpEnter();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingClearKey()
            {
                if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;

                    // TODO: DEV-5619
                    // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                    this.SysEnv.SysSettings.IsCountActivityItemRefreshData = true;
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    await this.ScannerDisable();
                    await this.Close();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingF1Key()
            {
                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;

                    // TODO: DEV-5619
                    // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                    this.SysEnv.SysSettings.IsCountActivityItemRefreshData = true;
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    await this.ScannerDisable();
                    await this.Close();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadDown()
            {
                if (this.ListViewMainActivity.SelectedItem != null &&
                    Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) <
                    this.ActivityItems.Count - 1)
                {
                    this.ListViewMainActivity.SelectedItem =
                        this.ActivityItems[
                            this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) + 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadUp()
            {
                if (this.ListViewMainActivity.SelectedItem != null &&
                    Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) > 0)
                {
                    this.ListViewMainActivity.SelectedItem =
                        this.ActivityItems[
                            this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) - 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingDelKey()
            {
                var isItemSelected =
                    Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) >= 0;
                if (isItemSelected)
                {
                    // If there is a skid selected
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
                    {
                        // Ask to delete the skid
                        var msg = "Do you wish to delete all this skid" +
                                  $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' detail?";
                        if (await this.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "Yes",
                                "No"))
                        {
                            // Make sure that the warehouse, product, bc, and unit match.
                            var skidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                            if (this.ActItem.ProdInfo.FkRefno != skidInfo.ProdInfo.FkRefno ||
                                this.ActItem.ProdInfo.UnitIsBc != skidInfo.ProdInfo.UnitIsBc ||
                                this.ActItem.ProdInfo.UnitRefno != skidInfo.ProdInfo.UnitRefno ||
                                this.ActItem.LocMap.WhsFkRefno != skidInfo.LocMap.WhsFkRefno)
                            {
                                msg = "The selected skid warehouse and product must be the same as the line item" +
                                      " warehouse and product before the skid can be deleted.";
                                throw new MessageOnlyException(msg);
                            }

                            var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                            await actDetail.DeleteSkid(this.ActItem.ActHdr.SkidInfo);

                            var fillQty = await actDetail.GetTotalQuantityFromListDetail();
                            this.ActItem.ReplaceQuantityInActivityItem(
                                this.ActItem.SelectedItem,
                                fillQty.Quantity,
                                fillQty.BaseQuantity,
                                fillQty.WeightQuantity);
                            await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);
                            this.ActItem.ListItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                            Device.BeginInvokeOnMainThread(
                                () =>
                                {
                                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;

                                    // This line is needed to display the new updated quantity visually.
                                    this.ActivityItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                                });
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        "Item deletion needs to be performed on the desktop.",
                                        Resource.OK));
                        }
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    "Item deletion needs to be performed on the desktop.",
                                    Resource.OK));
                    }
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
                                "Nothing selected to delete.",
                                Resource.OK));
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingF2Key()
            {
                try
                {
                    this.ListViewMainActivity.IsEnabled = false;
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) < 0)
                    {
                        throw new MessageOnlyException("Please select a line item to show the comments for.");
                    }

                    string comment = await this.ActItem.GetComment(this.ListViewMainActivity.SelectedItem as dynamic);
                    if (!string.IsNullOrEmpty(comment))
                    {
                        await this.DisplayAlert("Item Comment", comment, "OK");
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                            return Task.CompletedTask;
                        });
                }
                finally
                {
                    this.ListViewMainActivity.IsEnabled = true;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingF3Key()
            {
                try
                {
                    var msg = await this.ActItem.GetTotalQuantity();

                    if (!string.IsNullOrEmpty(msg))
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                                await this.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    msg,
                                    "OK"));
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                            return Task.CompletedTask;
                        });
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AppearingF4Key()
            {
                try
                {
                    this.ListViewMainActivity.IsEnabled = false;
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        throw new MessageOnlyException(
                            "Inventory Inquiry Summary is unavailable in disconnected mode.");
                    }

                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityInventoryInquirySummary,
                        this.ActItem,
                        this.ActSubType);
                    _ = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                    // No need to refresh the data here after call to inquiry form.
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                            return Task.CompletedTask;
                        });
                }
                finally
                {
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
                    this.ListViewMainActivity.IsEnabled = true;
                }
            }
        }

        /// <summary>
        ///     Check if there are any skids to print.
        /// </summary>
        /// <returns><see cref="bool" /> True if there are skids to be printed, else alse.</returns>
        /// <exception cref="MessageOnlyException"> Request skid check failed.</exception>
        /// <exception cref="Exception"> Error returned request skid check.</exception>
        private async Task<bool> CheckIfSkids2Print()
        {
            var isExist2Print = false;
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings { Async = true });
            var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                <imclient>
                {await SysCommon.GetRequestDefaults(false)}
                <command>
                <action>REQUEST CHECK IF SKIDS 2 PRINT</action>
                <whs_id>{this.LocMap.CurrLocMap.WhsFkRefno}</whs_id>
                <fk_refno>{this.ActItem.ActHdr.HdrFkRefno}</fk_refno>
                <fk_source>PO</fk_source>
                </command>
                </imclient>";
            var request = XDocument.Parse(xmlRequest);

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            var responseData = SysCommon.SubmitComm(xmlString.ToString());
            SysCommon.CheckCommResponseForErrors(responseData);
            if (responseData.Contains("<detail>True</detail>"))
            {
                isExist2Print = true;
            }

            return isExist2Print;
        }

        /// <summary>
        ///     Double-click on list view item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="ItemTappedEventArgs" /> Item tapped event arguments.</param>
        private async void ListViewMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                this.NumberOfScreenTapsForDoubleClick++;

                if (this.PreviousSenderForDoubleClick == e.Item)
                {
                    if (this.NumberOfScreenTapsForDoubleClick == 2)
                    {
                        this.NumberOfScreenTapsForDoubleClick = 0;
                        try
                        {
                            await this.ScannerDisable();
                            if (this.ListViewMainActivity.SelectedItem != null &&
                                Convert.ToInt32(
                                    this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) >
                                -1)
                            {
                                await this.ListViewMainActivity_KeyUpEnter();
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            throw;
                        }
                        finally
                        {
                            await this.ScannerEnable();
                        }
                    }
                }
                else
                {
                    this.PreviousSenderForDoubleClick = e.Item;
                    this.NumberOfScreenTapsForDoubleClick = 1;
                }

                await Task.Delay(500);
                this.NumberOfScreenTapsForDoubleClick = 0;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }
        }

        /// <summary>
        ///     Perform quantity under check for ALL line items.
        /// </summary>
        private void AllItemsQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                for (var idx = 0; idx < this.ActItem.ListItems.Count; idx++)
                {
                    // TODO: Test
                    var selectedItem = (PickingItem)this.ActItem.ListItems[idx];
                    var available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                    var shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                    var ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);
                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (available > 0 &&
                            shipped < ordered)
                        {
                            throw new MessageOnlyException(
                                "Some line items exists where the shipped quantity is less that" +
                                " the quantity ordered.");
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            throw new MessageOnlyException(
                                "Some line items exists where the shipped quantity is less that" +
                                " the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Perform quantity under check for one line item.
        /// </summary>
        /// <remarks>Line items must exist, one needs to be selected.</remarks>
        private async Task ItemQuantityUnderCheck()
        {
            if (this.ActItem.ListItems != null &&
                this.ActItem.SelectedItem != null &&
                this.SysEnv.SysSettings.PickQtyUnderWarn !=
                SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                var selectedItem = (PickingItem)this.ActItem.SelectedItem;
                ////decimal available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                var shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                var ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    // Do not warn if product is out of stock WO# 12054 was only applied
                    // to ucActivityItemSingle if (0 < available && shipped < ordered)
                    if (shipped < ordered)
                    {
                        string msg;
                        if (this.SysEnv.SysSettings.IsPickQtyUnderShowBaseQtyAvail)
                        {
                            var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(selectedItem.ProductUnitRefno);
                            msg = string.Format(
                                "The quantity shipped is less than the quantity ordered." +
                                Environment.NewLine +
                                "On Hand {0} {1}",
                                Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable),
                                prodInfo.BaseUnitCode);
                        }
                        else
                        {
                            msg = string.Format(
                                "The quantity shipped is less than the quantity ordered." +
                                Environment.NewLine +
                                "On Hand {0} {1}",
                                Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityAvailable),
                                Convert.ToString(selectedItem.UnitCode));
                        }

                        throw new MessageOnlyException(msg);
                    }
                }
                else
                {
                    if (shipped < ordered)
                    {
                        throw new MessageOnlyException("The quantity shipped is less than the quantity ordered.");
                    }
                }
            }
        }

        /// <summary>
        ///     Main data grid key up enter event handler.
        /// </summary>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        private async Task ListViewMainActivity_KeyUpEnter()
        {
            if (this.IsDataLoading ||
                this.ListViewMainActivity.SelectedItem == null ||
                Convert.ToInt32(this.ActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
            {
                return;
            }

            try
            {
                // Do not turn off or on scanner here, caller did.
                await this.ActItem.AssignActivityItem(this.ListViewMainActivity.SelectedItem);

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                    {
                        throw new MessageOnlyException(
                            "Selected line item product must match the selected skid product.");
                    }
                }

                await this.ScannerDisable();
                var form = new FormDialogContainer(EDialogTypes.ActivityDetail, this.ActItem, this.ActSubType);
                this.IsChangesMade = false;

                // There currently no way to cancel from the detail screen. Always returns DialogResult.OK.
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXml();
                    var qty = xmlDoc?.Root?.Element("qty")?.Value ?? "0";
                    var baseQty = xmlDoc?.Root?.Element("base_qty")?.Value ?? "0";
                    var weightQty = xmlDoc?.Root?.Element("wt_qty")?.Value ?? "0";

                    this.ActItem.ReplaceQuantityInActivityItem(
                        this.ActItem.SelectedItem,
                        Math.Round(decimal.Parse(qty), 3),
                        Math.Round(decimal.Parse(baseQty), 3),
                        Math.Round(decimal.Parse(weightQty), 3));
                    await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
                    this.ActItem.ListItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                    this.ActivityItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                    await this.DisplayLineItemDetail();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }
            finally
            {
                await this.ScannerEnable();
                this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
            }
        }

        /// <summary>
        ///     Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Sender bar code scanner event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(
                async () =>
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
                                    Resource.OK));
                            throw new TaskCanceledException();
                        }

                        object selectedItem = null;
                        var actItemFoundRefno = 0;
                        var tempItemProdSchemeData = new ActItemSchemeData();
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
                        else
                        {
                            if (isProductScanned)
                            {
                                // Identify the product from the parse info.
                                var prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);
                                var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                                // If there is a skid selected, then the product scanned must match the skid product.
                                await CheckIfSkidProductMatchesScanned(prodUnitRefno, scannedProdInfo);

                                await this.SetDefaultLocationBeforeKnownActivityItem(scannedProdInfo);

                                // Attempt to find the activity line item.
                                actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                                    prodUnitRefno,
                                    this.LocMap.CurrLocMap);
                                if (actItemFoundRefno > 0)
                                {
                                    this.ActItem.SetSelectedActivityItemFromList(actItemFoundRefno);
                                    if (this.ActItem.SelectedItem == null)
                                    {
                                        throw new Exception("Failed to set the found selected item in the list.");
                                    }

                                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                                    selectedItem = this.ActItem.SelectedItem;
                                    this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                                    await AssignDefaultProductLocation();
                                    await AssignSkidLocation();
                                }
                                else
                                {
                                    if (this.ActItem.IsAllowAddItem)
                                    {
                                        if (await this.AskToAddNewItem(scannedProdInfo))
                                        {
                                            // Additional found matching line items are not allowed to be created during
                                            // scanning and inserting. I am not expecting matches to be found because
                                            // that check has already been performed at this point.
                                            await this.ActItem.Insert(ProductInfoTasks.GetXml(scannedProdInfo));
                                            this.ActItem.SetSelectedActivityItemFromList(this.ActItem.ItemRefno);
                                            selectedItem = this.ActItem.SelectedItem;
                                            await this.ActItem.AssignActivityItem(selectedItem);
                                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;

                                            actItemFoundRefno = this.ActItem.ItemRefno;

                                            await AssignSkidLocation();

                                            this.ActivityItems.Add(this.ActItem);
                                            await this.DesignGrid();
                                            await this.FindSelectedItemAsync(this.ActItem.ItemFkRefno);
                                        }
                                        else
                                        {
                                            throw new GatherProductDataException(
                                                this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo),
                                                true);
                                        }
                                    }
                                    else
                                    {
                                        throw new MessageOnlyException(
                                            this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo));
                                    }
                                }
                            }
                            else if (isFkRefnoScanned)
                            {
                                throw new MessageOnlyException(
                                    "Scanning an activity header bar code is not supported here.");
                            }
                            else if (isFkItemRefnoScanned)
                            {
                                selectedItem = this.ActItem.ListItems.FirstOrDefault(
                                    activityItem =>
                                        Convert.ToString(
                                            activityItem.GetType()
                                                .GetProperty(this.ActItem.GetDataViewSortFkRefnoIdx())
                                                ?.GetValue(activityItem)) ==
                                        Convert.ToString(scanArgs.ParseInfo.ActItemFkRefno));
                                if (selectedItem == null)
                                {
                                    throw new Exception(
                                        this.ActItem.GetUnableToFindItemFkRefnoMessage(
                                            scanArgs.ParseInfo.ActItemFkRefno));
                                }

                                await this.ActItem.AssignActivityItem(selectedItem);
                                this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else if (isFkDetailRefnoScanned)
                            {
                                throw new MessageOnlyException(
                                    "Scanning an line item detail bar code is not supported here.");
                            }
                            else if (isSkidScanned)
                            {
                                // First tuple for me.
                                var result = await HandleSkidScanned(parse);
                                actItemFoundRefno = result.Item1;
                                selectedItem = result.Item2;
                            }

                            // Check the location again because of product location defaulting.
                            await this.VerifyLocationIsValid();

                            // Check that the skid product matches the identified product unit.
                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                            {
                                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno !=
                                    this.ActItem.ProdInfo.UnitRefno)
                                {
                                    throw new MessageOnlyException(
                                        "Identified line item product does not match the selected skid product.");
                                }
                            }

                            if (isSkidScanned)
                            {
                                // INSERT A NEW ACTIVITY DETAIL RECORD
                                var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                                actDetail.ActItem = this.ActItem;

                                actDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                                    this.ActItem.ProdInfo.Code,
                                    this.ActItem.ProdInfo.Scheme,
                                    actDetail.SchemeData);
                                actDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                                    this.ActItem.ProdInfo.Code,
                                    this.ActItem.ProdInfo.Scheme,
                                    actDetail.SchemeData);

                                // IsReadyToApply will perform any validations, such as duplicate checks
                                // and quantity validations.
                                var xmlString = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                                    <imclient>
                                    <itm_refno>{this.ActItem.ItemFkRefno}</itm_refno>
                                    <alt_qty>{this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity}</alt_qty>
                                    <wt_qty>{this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity}</wt_qty>
                                    <ActivityDetailUniqueSchemeID>{actDetail.UniqueSchemeId}
                                        </ActivityDetailUniqueSchemeID>
                                    </imclient>";
                                var xmlParam = XDocument.Parse(xmlString);

                                await this.ActItem.IsReadyToApply(xmlParam);

                                this.ActItem = actDetail.ActItem;
                                this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                                this.ActItem.SelectedItem = this.ActItem;
                                selectedItem = this.ActItem.SelectedItem;
                                actItemFoundRefno = this.ActItem.ItemRefno;

                                Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        var isAddAllSkidDetail = false;
                                        var skidMsg = "Do you wish to add all the available skid quantity?";
                                        var result = await this.DisplayAlert("Skid", skidMsg, "Yes", "No");
                                        if (result)
                                        {
                                            isAddAllSkidDetail = true;
                                        }

                                        if (isAddAllSkidDetail)
                                        {
                                            await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);
                                            if (selectedItem != null)
                                            {
                                                this.ActItem.AddToQuantityInActivityItem(
                                                    selectedItem,
                                                    actDetail.Quantity,
                                                    actDetail.BaseQuantity,
                                                    actDetail.WeightQuantity);
                                                await this.ActItem.UpdateItemTableQuantity(selectedItem);
                                                this.ActItem.SelectedItem = selectedItem;
                                                await this.LoadAndBind();
                                            }
                                        }
                                    });

                                ////if (actDetail.Quantity == 0)
                                ////{
                                ////    throw new MessageOnlyException("No quantity exists on skid" +
                                ////                                   $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' in warehouse" +
                                ////                                   $" {LocationMap.GetInstance().CurrLocMap.WhsCode}.");
                                ////}

                                // Does the row exist? If not, then it has been removed due to not needing it
                                // Skip update qty.
                                ////if (selectedItem != null)
                                ////{
                                ////    // Upon a save of completed data update the data view to show new item and quantity change
                                ////    this.ActItem.AddToQuantityInActivityItem(
                                ////        selectedItem,
                                ////        actDetail.Quantity,
                                ////        actDetail.BaseQuantity,
                                ////        actDetail.WeightQuantity);
                                ////    await this.ActItem.UpdateItemTableQuantity(selectedItem);
                                ////    this.ActItem.SetSelectedActivityItemFromList(this.ActItem.ItemRefno);
                                ////    this.ActItem.SelectedItem = selectedItem;
                                ////}
                            }
                            else
                            {
                                await this.CheckCurrentLocationMatchDefaultProductLocation(
                                    this.ActItem.ProdInfo,
                                    this.LocMap.CurrLocMap);

                                // Not allowed to count individual boxes on a skid. Skids must be
                                // scanned as a whole unit or built in skid maintenance.

                                // DEV-827 This change to allow counting partial skids is a bad idea.
                                //// if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                                ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                                //// {
                                ////    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                                ////    {
                                ////        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                                ////        this.cmdSelectSkid.Text = "Select Skid";

                                var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                                var schemeData = this.DefaultSchemeDataFromCurrentSkidSchemeDetail(actDetail);
                                tempItemProdSchemeData.ProdSchemeData = schemeData;
                                actDetail.SchemeData = schemeData;

                                var isInsertReady = await SysShared.GatherProductData(
                                    parse,
                                    this.ActSubType,
                                    this.ActItem,
                                    tempItemProdSchemeData,
                                    actDetail);

                                if (isInsertReady)
                                {
                                    var xmlDoc = actDetail.GetXml(actDetail);
                                    await actDetail.Insert(xmlDoc);
                                    this.ActItem = actDetail.ActItem;

                                    // TODO check if actually new, if not delete and change counter
                                    // Does the row exist? If not, then it has been removed due to not needing it
                                    // Skip update qty.
                                    if (selectedItem != null)
                                    {
                                        // Upon a save of completed data update the data view to show new item and
                                        // quantity change.
                                        this.ActItem.AddToQuantityInActivityItem(
                                            selectedItem,
                                            actDetail.Quantity,
                                            actDetail.BaseQuantity,
                                            actDetail.WeightQuantity);
                                        await this.ActItem.UpdateItemTableQuantity(selectedItem);
                                        this.ActItem.SelectedItem = selectedItem;
                                        ////this.ActItem.ListItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                                    }
                                }
                            }

                            // IF AN ACTIVITY LINE ITEM WAS FOUND
                            if (actItemFoundRefno == 0 ||
                                selectedItem == null)
                            {
                                throw new MessageOnlyException("Line item not found.");
                            }

                            this.ActItem.SelectedItem = selectedItem;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Do nothing as task is being canceled from IdentifyProductScanned when user
                        // declines to assign bar code to product.

                        // This exception is also caught if the scan event arguments are null.
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    msg,
                                    Resource.OK));
                    }
                    catch (GatherProductDataException ex)
                    {
                        if (!ex.SuppressError)
                        {
                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                {
                                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                                    _ = await errRpt.ShowDialog();
                                    return Task.CompletedTask;
                                });
                        }
                    }
                    catch (IsReadyToApplyException ex)
                    {
                        if (!ex.SuppressError)
                        {
                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                {
                                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                                    _ = await errRpt.ShowDialog();
                                    return Task.CompletedTask;
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                            {
                                var errRpt = new FormErrorReport(this.ActSubType, ex);
                                _ = await errRpt.ShowDialog();
                                return Task.CompletedTask;
                            });
                    }
                    finally
                    {
                        await this.ScannerEnable();
                        if (scanArgs != null)
                        {
                            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
                            await this.DesignGrid();
                            this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                            await this.DisplayLineItemDetail();
                        }
                    }
                });

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
            {
                // If section filtering was set in the activity header. Do we want to enforce
                // the scanned section location?
                var tmpLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));
                this.LocMap.CurrLocMap = tmpLoc;
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();

                await Device.InvokeOnMainThreadAsync(
                    () =>
                    {
                        this.lblSelectedLocation.Text = "Selected Location: " +
                                                        LocationMapTasks.GetCurrentReadableLocation();
                    });

                await CheckIfScannedSectionMatchesFilteredOne(tmpLoc);
                return;

                //////////////////////////////////////////////////////////////////////////
                async Task CheckIfScannedSectionMatchesFilteredOne(SLocMap scannedLocMap)
                {
                    // Special validation for picking and filtering by section.
                    // Check if the scanned section matched the filtered one.
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    {
                        var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 &&
                            scannedLocMap.SectLocMapRefno != sectRefno)
                        {
                            var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            var msg = $"Warning scanned location section '{scannedLocMap.SectName}' does" +
                                      $" not match the filtered picking section '{sectName}'.";

                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        msg,
                                        Resource.OK));
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<(int, object)> HandleSkidScanned(BarCodeParse parse)
            {
                if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                {
                    const string msg = "Scanning a shipping container bar code is not enabled.";
                    throw new MessageOnlyException(msg);
                }

                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    const string msg = "Scanning a shipping container bar code is not supported in disconnected mode.";
                    throw new MessageOnlyException(msg);
                }

                this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.Sscc18);

                // The current location will supersede the skid location on counts.
                if (!(this.ActItem.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                      this.ActItem.ActSubType == SysCommon.ActSubTypes.CycleCounts))
                {
                    if (await LocationMapTasks.IsCompleteLocMap(this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap))
                    {
                        this.LocMap.CurrLocMap = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap;
                    }
                }

                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    throw new MessageOnlyException("The current location must be complete.");
                }

                var prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                if (prodUnitRefno == 0)
                {
                    throw new MessageOnlyException("Skid does have a product assigned to it.");
                }

                object selectedItem;

                // Does the skid product and location exist as a line item.
                var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                    prodUnitRefno,
                    LocationMap.GetInstance().CurrLocMap);
                if (actItemFoundRefno > 0)
                {
                    this.ActItem.SetSelectedActivityItemFromList(actItemFoundRefno);
                    if (this.ActItem.SelectedItem == null)
                    {
                        throw new Exception("Failed to set the found selected item in the list.");
                    }

                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    selectedItem = this.ActItem.SelectedItem;
                    this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                }
                else
                {
                    if (this.ActItem.IsAllowAddItem)
                    {
                        if (await this.AskToAddNewItem(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo))
                        {
                            var data = ProductInfoTasks.GetXml(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);

                            // Additional found matching line items are not allowed to be created during
                            // scanning and inserting. I am not expecting matches to be found because
                            // that check has already been performed at this point.
                            await this.ActItem.Insert(data);
                            this.ActItem.SetSelectedActivityItemFromList(this.ActItem.ItemRefno);
                            selectedItem = this.ActItem.SelectedItem;
                            await this.ActItem.AssignActivityItem(selectedItem);
                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                            actItemFoundRefno = this.ActItem.ItemRefno;
                            this.ActivityItems.Add(this.ActItem);
                            await this.DesignGrid();
                            await this.FindSelectedItemAsync(this.ActItem.ItemFkRefno);
                        }
                        else
                        {
                            throw new GatherProductDataException(
                                this.ActItem.GetItemNotFoundMessage(
                                    this.LocMap,
                                    this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo),
                                true);
                        }
                    }
                    else
                    {
                        // Because a new line item is not allowed to be created, the skid will be cleared.
                        var msg = this.ActItem.GetItemNotFoundMessage(
                            this.LocMap,
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);

                        this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                        throw new MessageOnlyException(msg);
                    }
                }

                return (actItemFoundRefno, selectedItem);
            }

            ///////////////////////////////////////////////////////////
            async Task CheckIfSkidProductMatchesScanned(int prodUnitRefno, ProductInfo scannedProdInfo)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0)
                    {
                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            const string msg = "Selected skid product does not match" +
                                               " the scanned product. Please scan the skid assigned" +
                                               " product or clear the selected skid.";
                            throw new MessageOnlyException(msg);
                        }
                    }
                    else
                    {
                        // If the skid product is not set, then set it.
                        if (!await this.AskToAssignProductToSkid(scannedProdInfo))
                        {
                            const string msg = "Assigning product to a skid cancelled. Scan aborted.";
                            throw new MessageOnlyException(msg);
                        }

                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo = scannedProdInfo;
                        SysSkidInfo.SendUpdate(this.ActItem.ActHdr.SkidInfo);
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            async Task AssignDefaultProductLocation()
            {
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc &&
                    !this.IsAllowedDefaultLocationOverride())
                {
                    var itemLocMap =
                        await this.ActItem.GetSelectedItemLocation(this.ActItem.LocMapRefno);
                    if (await LocationMapTasks.IsCompleteLocMap(itemLocMap))
                    {
                        this.LocMap.CurrLocMap = itemLocMap;
                        this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            async Task AssignSkidLocation()
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0 &&
                    await LocationMapTasks.IsCompleteLocMap(
                        this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap))
                {
                    this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap = this.LocMap.CurrLocMap.Clone();
                }
            }
        }

        /// <summary>
        ///     Set the activity item location for adding new or identify line items.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo" /> Product information.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SetDefaultLocationBeforeKnownActivityItem(ProductInfo prodInfo)
        {
            // Need to get a warehouse to determine default location if turned on, we don't
            // have the line item warehouse yet. We could assume the header warehouse or even
            // the login warehouse.
            // We can't determine the default location until after identifying the line item.
            var assignDefaultLocation = true;
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
            {
                if (!(this.ActItem.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                      this.ActItem.ActSubType == SysCommon.ActSubTypes.CycleCounts))
                {
                    if (await LocationMapTasks.IsCompleteLocMap(this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap))
                    {
                        this.LocMap.CurrLocMap = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap;
                        assignDefaultLocation = false;
                    }
                }
            }

            if (assignDefaultLocation)
            {
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // This will be the default location if the line item location is not complete.
                    var whsLocMapRefno =
                        await LocationMapTasks.GetWhsLocMapRefnoFromLocMapRefno(this.LocMap.CurrLocMapRefno);
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(whsLocMapRefno, prodInfo.Refno);

                    if (this.IsAllowedDefaultLocationOverride())
                    {
                        if (await LocationMapTasks.IsCompleteLocMap(this.LocMap.ChosenLocMap))
                        {
                            SetCurrentLocationWithChosen();
                        }
                        else
                        {
                            if (!await LocationMapTasks.IsCompleteLocMap(defLocMapRefno))
                            {
                                const string msg = "Default locations is enabled, and for this activity and product" +
                                                   " the location is not fully complete.";
                                throw new MessageOnlyException(msg);
                            }

                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                            this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                        }
                    }
                    else
                    {
                        if (!await LocationMapTasks.IsCompleteLocMap(defLocMapRefno))
                        {
                            const string msg = "Default locations is enabled, and for this activity and product" +
                                               " the location is not fully complete.";
                            throw new MessageOnlyException(msg);
                        }

                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                        this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                    }
                }
                else
                {
                    SetCurrentLocationWithChosen();
                }
            }

            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
            {
                throw new MessageOnlyException("Location must be set first!");
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            // Set the current location with the chosen location.
            void SetCurrentLocationWithChosen()
            {
                // This should not be needed, current location should have been already set using the location picker.
                if (this.LocMap.CurrLocMapRefno != this.LocMap.ChosenLocMapRefno)
                {
                    // Set the current location to the chosen one, overriding and defaulting location behaviour.
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                }
            }
        }

        /// <summary>
        ///     Check if default location is allowed to be overridden.
        /// </summary>
        /// <returns><see cref="bool" /> True if the location override is allowed else false.</returns>
        private bool IsAllowedDefaultLocationOverride()
        {
            var isAllowedLocationOverride = false;
            var useProductDefaultLocation = this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
            if (useProductDefaultLocation)
            {
                var allowLocationOverridePhysical = this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                                                    this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation;
                var allowLocationOverrideCycle = this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                                                 this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation;
                if (allowLocationOverridePhysical || allowLocationOverrideCycle)
                {
                    isAllowedLocationOverride = true;
                }
            }

            return isAllowedLocationOverride;
        }

        /// <summary>
        ///     Verify the current location matches the default location.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        /// <exception cref="MessageOnlyException"> Location must be set first.</exception>
        private async Task VerifyLocationIsValid()
        {
            var useProductDefaultLocation = this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;

            // Check the location again because of product location defaulting.
            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) && useProductDefaultLocation)
            {
                throw new MessageOnlyException("Location must be set first.");
            }
        }

        /// <summary>
        ///     Check if the current location matches the product default location.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task CheckCurrentLocationMatchDefaultProductLocation(ProductInfo prodInfo, SLocMap locMap)
        {
            var isLocationComplete = await LocationMapTasks.IsCompleteLocMap(locMap);
            var useSelectedLocation = !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
            if (isLocationComplete && useSelectedLocation)
            {
                // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                var isVerifyProductLocation = this.SysEnv.SysSettings.IsVerifyProductLocation;
                var isActivityTypeToVerify = this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                             this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                                             this.ActSubType == SysCommon.ActSubTypes.TransferTo;
                if (isVerifyProductLocation && isActivityTypeToVerify)
                {
                    // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                    // Picking activity items only have the warehouse, not section or slot.
                    if (locMap.WhsLocMapRefno <= 0)
                    {
                        throw new Exception("Invalid warehouse on activity item. LocationMapRefno == 0");
                    }

                    if (prodInfo.Refno <= 0)
                    {
                        throw new Exception("Invalid product on activity item. ProductRefno == 0");
                    }

                    var defLocMapRefno =
                        await this.ActItem.GetDefaultLocMapRefno(locMap.WhsLocMapRefno, prodInfo.Refno);
                    if (LocationMap.GetInstance().CurrLocMapRefno != defLocMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The current location does not match the product home location.");
                    }
                }
            }
        }

        /// <summary>
        ///     Print skid labels.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        /// <exception cref="MessageOnlyException"> Print skid labels message.</exception>
        /// <exception cref="Exception"> Unhandled print skid labels error.</exception>
        private async Task PrintSkidLabels()
        {
            if (await this.CheckIfSkids2Print())
            {
                if (await this.DisplayAlert(
                        "Skid Labels",
                        "Do you wish to print all skid label(s)?",
                        "Yes",
                        "No"))
                {
                    await this.SubmitPrintSkidLabelsRequest();
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            "Skid labels printed successfully.",
                            Resource.OK));
                }
            }
        }

        /// <summary>
        ///     Perform print skid labels request.
        /// </summary>
        /// <returns><see cref="Task{T}" /> Request result.</returns>
        private async Task SubmitPrintSkidLabelsRequest()
        {
            var xmlString = await this.MakeSkidLabelsRequestString();
            var responseData = SysCommon.SubmitComm(xmlString);
            SysCommon.CheckCommResponseForErrors(responseData);
        }

        /// <summary>
        ///     Make the print labels request string.
        /// </summary>
        /// <returns><see cref="string" /> Print labels request string.</returns>
        private async Task<string> MakeSkidLabelsRequestString()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings { Async = true });
            var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                <imclient>
                {await SysCommon.GetRequestDefaults(false)}
                <command>
                <action>SUBMIT PRINT ALL SKID LABELS</action>
                <whs_id>{this.LocMap.CurrLocMap.WhsFkRefno}</whs_id>
                <fk_refno>{this.ActItem.ActHdr.HdrFkRefno}</fk_refno>
                <fk_source>PO</fk_source>
                </command>
                </imclient>";
            var request = XDocument.Parse(xmlRequest);

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();
            return xmlString.ToString();
        }

        /// <summary>
        ///     Default scheme data from the current skid scheme detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail containing the skid information.</param>
        /// <returns><see cref="ActDetail" /> Scheme data from current skid.</returns>
        private ProductSchemeData DefaultSchemeDataFromCurrentSkidSchemeDetail(ActDetail actDetail)
        {
            // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
            // because of the length of the code and the fact isn't human-readable friendly.
            var schemeData = actDetail.SchemeData;
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
            {
                var curSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                schemeData.LogisticUnitSkidNumber = curSkid.SkidCode;
                schemeData.LogisticUnitBatchLotNumber = curSkid.SchemeData.LogisticUnitBatchLotNumber;
                schemeData.LogisticUnitBestBeforeSellByDate = curSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                schemeData.LogisticUnitProductionDate = curSkid.SchemeData.LogisticUnitProductionDate;
                schemeData.VariantCode = curSkid.SchemeData.VariantCode;
                schemeData.VariantNumber = curSkid.SchemeData.VariantNumber;
                schemeData.VariantDescription = curSkid.SchemeData.VariantDescription;
            }

            return schemeData;
        }

        /// <summary>
        ///     Ask to add new activity item.
        /// </summary>
        /// <param name="scannedProdInfo"><see cref="ProductInfo" /> Product information.</param>
        /// <returns><see cref="Task{T}" /> True if allowed to create a new activity item.</returns>
        private async Task<bool> AskToAddNewItem(ProductInfo scannedProdInfo)
        {
            var isAddNewItem = false;
            if (this.ActItem.IsAskCreateNewItem)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        if (await this.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                string.Format(
                                    "A line item does not exist for this warehouse {0}" +
                                    " and product \'{1}.\'" +
                                    Environment.NewLine +
                                    "Do you wish to create one?",
                                    this.LocMap.CurrLocMap.WhsCode,
                                    ProductInfoTasks.GetCurrReadableProdBcUnit(scannedProdInfo)),
                                "Yes",
                                "No"))
                        {
                            isAddNewItem = true;
                        }

                        return Task.CompletedTask;
                    });
            }
            else
            {
                isAddNewItem = true;
            }

            return isAddNewItem;
        }

        /// <summary>
        ///    Ask to assign product to skid.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="bool"/> True to assign the product to the skid, else do not assign.</returns>
        private async Task<bool> AskToAssignProductToSkid(ProductInfo prodInfo)
        {
            var isAssignProductToSkid = false;
            await Device.InvokeOnMainThreadAsync(
                async () =>
                {
                    var msg = "The current selected skid does not have a product assigned to it yet." +
                              $"Do you wish to assign '{prodInfo.Code} {prodInfo.UnitBc.Trim()} {prodInfo.UnitCode}'" +
                              $" to the skid?";
                    if (await this.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            "Yes",
                            "No"))
                    {
                        isAssignProductToSkid = true;
                    }

                    return Task.CompletedTask;
                });

            return isAssignProductToSkid;
        }

        /// <summary>
        ///     Design the main activity grid / list.
        /// </summary>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task DesignGrid()
        {
            await Device.InvokeOnMainThreadAsync(
                () =>
                {
                    this.ActivityItems = null;
                    this.ListViewMainActivity.ItemsSource = null;

                    // Redesign the grid / list and rebind it to the refreshed list items.
                    this.ActivityItems = this.ActItem.DesignGrid(
                        ref this.GridMainActivity,
                        ref this.ListViewMainActivity);

                    if (!string.IsNullOrEmpty(this.txtSearchItem.Text))
                    {
                        this.ActivityItems = this.ActItem.Search(this.txtSearchItem.Text);
                    }

                    this.ListViewMainActivity.ItemsSource = this.ActivityItems;
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        ///     Text search activity items text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Sender object text changed event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private void TxtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            // SearchTimer was started in the focused event handler.
            // SearchTimer was stopped in the unfocused event handler.
            // SearchTimer was stopped in the timer tick call back event.
            // Start the timer when the text changes. This will reset the timer and force a load data.
            this.SearchTimer.Start();
        }

        /// <summary>
        ///     Bar code modifier selected index changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
        }

        /// <summary>
        ///     Data grid activity item selected item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e">
        ///     <see cref="SelectedItemChangedEventArgs" /> Sender selected item changed
        ///     event arguments.
        /// </param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActItem.AssignActivityItem(e.SelectedItem);
            this.ActItem.SelectedListIdx = e.SelectedItemIndex;
            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
            await this.DisplayLineItemDetail();
            await this.DisplayDataItem();
        }

        /// <summary>
        ///     Display selected item extra information.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task DisplayLineItemDetail()
        {
            await Device.InvokeOnMainThreadAsync(
                async () =>
                {
                    this.LineItemInfo.Clear();
                    switch (this.ActItem.ActSubType)
                    {
                        case SysCommon.ActSubTypes.Picking:
                            if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(pickingItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                          $" Ordered: {pickingItem.ActivityItemPickingQuantityOrdered:F3}" +
                                          $" Shipped: {pickingItem.ActivityItemPickingQuantityShipped:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            if (this.ListViewMainActivity.SelectedItem is CountItem cycleCountItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(cycleCountItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                          $" Counted: {cycleCountItem.ActivityItemCountQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            if (this.ListViewMainActivity.SelectedItem is CountItem physicalCountItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(physicalCountItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Counted: {physicalCountItem.ActivityItemCountQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.Receiving:
                            if (this.ListViewMainActivity.SelectedItem is ReceiveItem receivingItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(receivingItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                          $" Ordered: {receivingItem.ActivityItemReceivingQuantityOrdered:F3}" +
                                          $" Received: {receivingItem.ActivityItemReceivingQuantityReceived:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            if (this.ListViewMainActivity.SelectedItem is TransferItem transferFromItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(transferFromItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                          $" Required: {transferFromItem.ActivityItemTransferRequiredQuantity:F3}" +
                                          $" Transferred: {transferFromItem.ActivityItemTransferQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.Returns:
                            if (this.ListViewMainActivity.SelectedItem is ReturnItem returnsItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(returnsItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Returned: {returnsItem.ActivityItemReturnsQuantityReturned:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.Adjustment:
                            if (this.ListViewMainActivity.SelectedItem is AdjustmentItem adjustmentItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(adjustmentItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Adjusted: {adjustmentItem.ActivityItemAdjustmentQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketFinishedItem ticketFinishedItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(ticketFinishedItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Finished: {ticketFinishedItem.ActivityItemProductionQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketMaterialItem ticketMaterialItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(ticketMaterialItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Used: {ticketMaterialItem.ActivityItemProductionQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketByProductItem ticketByProductItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(ticketByProductItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Finished: {ticketByProductItem.ActivityItemProductionQuantity:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                            if (this.ListViewMainActivity.SelectedItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
                            {
                                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(purchaseOrderWorkSheetItem.ProductUnitRefno);
                                var prodBcUnit = ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo);
                                var msg = $"{prodBcUnit} -" +
                                    $" Ordered: {purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered:F3}";
                                this.LineItemInfo.Add(msg);
                            }

                            break;

                        case SysCommon.ActSubTypes.ConvertUnit:
                        case SysCommon.ActSubTypes.MoveInventory:
                        case SysCommon.ActSubTypes.PurgeActivityData:
                        case SysCommon.ActSubTypes.BarCodeAssociation:
                        case SysCommon.ActSubTypes.RefreshData:
                        case SysCommon.ActSubTypes.DataStatus:
                        case SysCommon.ActSubTypes.ReleaseByOrder:
                        case SysCommon.ActSubTypes.ReleaseByItemSection:
                        case SysCommon.ActSubTypes.SelectProduct:
                        case SysCommon.ActSubTypes.SelectLocation:
                        case SysCommon.ActSubTypes.SkidInfo:
                        case SysCommon.ActSubTypes.MoveSkid:
                        case SysCommon.ActSubTypes.BreakSkid:
                        case SysCommon.ActSubTypes.PickingSingle:
                        case SysCommon.ActSubTypes.PutAwaySkid:
                        case SysCommon.ActSubTypes.AssignDefaultLocation:
                        case SysCommon.ActSubTypes.InventoryInquiry:
                        case SysCommon.ActSubTypes.InventoryReplenishment:
                        case SysCommon.ActSubTypes.LoginStartup:
                        case SysCommon.ActSubTypes.SplashStartup:
                        case SysCommon.ActSubTypes.PrintProductLabel:
                        case SysCommon.ActSubTypes.PickingSelectCriteria:
                        case SysCommon.ActSubTypes.PickingFilter:
                        case SysCommon.ActSubTypes.SubmitBatch:
                        case SysCommon.ActSubTypes.CommunicationSetup:
                        case SysCommon.ActSubTypes.SetupDefaults:
                        case SysCommon.ActSubTypes.InventoryInquirySummary:
                        case SysCommon.ActSubTypes.ConvertProduct:
                        case SysCommon.ActSubTypes.TransferTo:
                        case SysCommon.ActSubTypes.NotApplicable:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    this.ListViewLineDetail.ItemsSource = this.LineItemInfo;
                });
        }

        /// <summary>
        ///     Display selected item extra information.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task DisplayDataItem()
        {
            await Device.InvokeOnMainThreadAsync(
                async () =>
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        this.CmdAddSkidIsEnabled = true;
                        this.CmdSelectSkidIsEnabled = false;
                        this.CmdClearSelectedSkidIsEnabled = true;
                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity == 0)
                        {
                            this.cmdAddProductIsEnabled = true;
                        }
                        else
                        {
                            this.cmdAddProductIsEnabled = false;
                        }

                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                            this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        {
                            this.CmdLocationIsEnabled = true;
                        }
                        else
                        {
                            this.CmdLocationIsEnabled = false;
                        }

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(
                            () =>
                            {
                                this.CmdAddSkidIsEnabled = false;
                                this.CmdAddProductIsEnabled = true;
                                this.CmdSelectSkidIsEnabled = true;
                                this.CmdClearSelectedSkidIsEnabled = false;
                                this.CmdLocationIsEnabled = true;
                                this.CmdSelectSkidText = "Select Skid";
                            });
                    }

                    this.ExtraLineItemInfo.Clear();

                    switch (this.ActItem.ActSubType)
                    {
                        case SysCommon.ActSubTypes.Adjustment:
                            await DisplayDataItemAdjustment();
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            await DisplayDataItemCycleCount();
                            break;

                        case SysCommon.ActSubTypes.InventoryInquiry:
                            DisplayDataItemInventoryInquiry();
                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            await DisplayDataItemPhysicalCount();
                            break;

                        case SysCommon.ActSubTypes.Picking:
                            await DisplayDataItemPicking();
                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            await DisplayDataItemTicketAddProduct();
                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            await DisplayDataItemTicketFinished();
                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            await DisplayDataItemTicketMaterial();
                            break;

                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                            await DisplayDataItemPurchaseOrderWorkSheet();
                            break;

                        case SysCommon.ActSubTypes.Receiving:
                            await DisplayDataItemReceiving();
                            break;

                        case SysCommon.ActSubTypes.Returns:
                            await DisplayDataItemReturns();
                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            await DisplayDataItemTransferFrom();
                            break;

                        case SysCommon.ActSubTypes.TransferTo:
                            await DisplayDataItemTransferTo();
                            break;

                        case SysCommon.ActSubTypes.PickingSingle:
                        case SysCommon.ActSubTypes.NotApplicable:
                        case SysCommon.ActSubTypes.BarCodeAssociation:
                        case SysCommon.ActSubTypes.RefreshData:
                        case SysCommon.ActSubTypes.DataStatus:
                        case SysCommon.ActSubTypes.SelectProduct:
                        case SysCommon.ActSubTypes.SelectLocation:
                        case SysCommon.ActSubTypes.ConvertUnit:
                        case SysCommon.ActSubTypes.MoveInventory:
                        case SysCommon.ActSubTypes.PurgeActivityData:
                        case SysCommon.ActSubTypes.ReleaseByOrder:
                        case SysCommon.ActSubTypes.ReleaseByItemSection:
                        case SysCommon.ActSubTypes.SkidInfo:
                        case SysCommon.ActSubTypes.MoveSkid:
                        case SysCommon.ActSubTypes.BreakSkid:
                        case SysCommon.ActSubTypes.PutAwaySkid:
                        case SysCommon.ActSubTypes.AssignDefaultLocation:
                        case SysCommon.ActSubTypes.InventoryReplenishment:
                        case SysCommon.ActSubTypes.LoginStartup:
                        case SysCommon.ActSubTypes.SplashStartup:
                        case SysCommon.ActSubTypes.PrintProductLabel:
                        case SysCommon.ActSubTypes.PickingSelectCriteria:
                        case SysCommon.ActSubTypes.PickingFilter:
                        case SysCommon.ActSubTypes.SubmitBatch:
                        case SysCommon.ActSubTypes.CommunicationSetup:
                        case SysCommon.ActSubTypes.SetupDefaults:
                        case SysCommon.ActSubTypes.InventoryInquirySummary:
                        case SysCommon.ActSubTypes.ConvertProduct:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ////this.ExtraLineItemInfo.Add("Selected Location: " + LocationMapTasks.GetCurrentReadableLocation());
                    this.lblSelectedLocation.Text =
                        "Selected Location: " + LocationMapTasks.GetCurrentReadableLocation();

                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
                });
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemTransferTo()
            {
                if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemTo)
                {
                    ////var prodLocCode = Convert.ToString(transferItemTo.LocationCode);
                    //// prodLocName = transferItemTo.LocationName;
                    var prodDesc = transferItemTo.ProductDescription;
                    var prodUnitSizeDesc = transferItemTo.ProductUnitSizeDescription;
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        transferItemTo.LocationMapRefno,
                        transferItemTo.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemTransferFrom()
            {
                if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemFrom)
                {
                    ////var prodLocCode = Convert.ToString(transferItemFrom.LocationCode);
                    ////var prodLocName = transferItemFrom.LocationName;
                    var prodDesc = transferItemFrom.ProductDescription;
                    var prodUnitSizeDesc = transferItemFrom.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        transferItemFrom.LocationMapRefno,
                        transferItemFrom.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemReturns()
            {
                if (this.ListViewMainActivity.SelectedItem is ReturnItem returnItem)
                {
                    ////var prodLocCode = Convert.ToString(returnItem.LocationCode);
                    ////var prodLocName = returnItem.LocationName;
                    ////var prodCode = returnItem.ProductCode;
                    var prodDesc = returnItem.ProductDescription;
                    var prodUnitSizeDesc = returnItem.ProductUnitSizeDescription;
                    var prodBrandDesc = returnItem.ProductBrandDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        returnItem.LocationMapRefno,
                        returnItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    switch (this.SysEnv.SysSettings.PickItemDescOpt)
                    {
                        case 1:
                            this.ExtraLineItemInfo.Add(
                                "SIZE: " +
                                prodUnitSizeDesc.Trim() +
                                ", BRAND: " +
                                prodBrandDesc.Trim());
                            break;

                        default:
                            this.ExtraLineItemInfo.Add("DESC: " + prodDesc.Trim() + ", " + prodUnitSizeDesc.Trim());
                            break;
                    }

                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    switch (this.SysEnv.SysSettings.PickItemDescOpt)
                    {
                        case 1:
                            this.ExtraLineItemInfo.Add("SIZE: Unknown, BRAND: Unknown");
                            break;

                        default:
                            this.ExtraLineItemInfo.Add("DESC: Unknown");
                            break;
                    }

                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemReceiving()
            {
                if (this.ListViewMainActivity.SelectedItem is ReceiveItem receiveItem)
                {
                    ////var prodLocCode = Convert.ToString(receiveItem.LocationCode);
                    ////var prodLocName = receiveItem.LocationName;
                    var prodDesc = receiveItem.ProductDescription;
                    var prodUnitSizeDesc = receiveItem.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        receiveItem.LocationMapRefno,
                        receiveItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemPurchaseOrderWorkSheet()
            {
                if (this.ListViewMainActivity.SelectedItem is PurchaseOrderWorkSheetItem
                    purchaseOrderWorkSheetItem)
                {
                    ////var prodLocCode = Convert.ToString(purchaseOrderWorkSheetItem.LocationCode);
                    ////var prodLocName = purchaseOrderWorkSheetItem.LocationName;
                    var prodDesc = purchaseOrderWorkSheetItem.ProductDescription;
                    var prodUnitSizeDesc = purchaseOrderWorkSheetItem.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        purchaseOrderWorkSheetItem.LocationMapRefno,
                        purchaseOrderWorkSheetItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemTicketMaterial()
            {
                if (this.ListViewMainActivity.SelectedItem is ProductionTicketMaterialItem
                    productionTicketMaterialItem)
                {
                    ////var prodLocCode = Convert.ToString(productionTicketMaterialItem.LocationCode);
                    ////var prodLocName = productionTicketMaterialItem.LocationName;
                    var prodDesc = productionTicketMaterialItem.ProductDescription;
                    var prodUnitSizeDesc = productionTicketMaterialItem.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        productionTicketMaterialItem.LocationMapRefno,
                        productionTicketMaterialItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemTicketFinished()
            {
                if (this.ListViewMainActivity.SelectedItem is ProductionTicketFinishedItem
                    productionTicketFinishedItem)
                {
                    ////var prodLocCode = Convert.ToString(productionTicketFinishedItem.LocationCode);
                    ////var prodLocName = productionTicketFinishedItem.LocationName;
                    var prodDesc = productionTicketFinishedItem.ProductDescription;
                    var prodUnitSizeDesc = productionTicketFinishedItem.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        productionTicketFinishedItem.LocationMapRefno,
                        productionTicketFinishedItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemTicketAddProduct()
            {
                if (this.ListViewMainActivity.SelectedItem is ProductionTicketByProductItem
                    productionTicketByProductItem)
                {
                    ////var prodLocCode = Convert.ToString(productionTicketByProductItem.LocationCode);
                    ////var prodLocName = productionTicketByProductItem.LocationName;
                    var prodDesc = productionTicketByProductItem.ProductDescription;
                    var prodUnitSizeDesc = productionTicketByProductItem.ProductUnitSizeDescription;

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        productionTicketByProductItem.LocationMapRefno,
                        productionTicketByProductItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemPicking()
            {
                if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItem)
                {
                    ////var prodLocCode = Convert.ToString(pickingItem.LocationCode);
                    ////var prodLocName = pickingItem.LocationName;
                    ////var prodCode = pickingItem.ProductCode;
                    var prodDesc = pickingItem.ProductDescription;
                    var prodUnitSizeDesc = pickingItem.ProductUnitSizeDescription;
                    var prodBrandDesc = pickingItem.ProductBrandDescription;
                    switch (this.SysEnv.SysSettings.PickItemDescOpt)
                    {
                        case 1:

                            this.ExtraLineItemInfo.Add(
                                "SIZE: " +
                                prodUnitSizeDesc.Trim() +
                                ", " +
                                prodBrandDesc.Trim());
                            break;

                        default:

                            this.ExtraLineItemInfo.Add(
                                "DESC: " +
                                prodDesc.Trim() +
                                ", " +
                                prodUnitSizeDesc.Trim());
                            break;
                    }

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        pickingItem.LocationMapRefno,
                        pickingItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    switch (this.SysEnv.SysSettings.PickItemDescOpt)
                    {
                        case 1:
                            this.ExtraLineItemInfo.Add("SIZE: Unknown, BRAND: Unknown");
                            break;

                        default:
                            this.ExtraLineItemInfo.Add("DESC: Unknown");
                            break;
                    }

                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemPhysicalCount()
            {
                if (this.ListViewMainActivity.SelectedItem is CountItem physicalCountItem)
                {
                    // Do not check if default location is enabled. Always show the default location.
                    var tempLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        Convert.ToInt32(physicalCountItem.LocationMapRefno));
                    var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        tempLocMap.WhsLocMapRefno,
                        Convert.ToInt32(physicalCountItem.ProductRefno),
                        SysCommon.ProdLocMapTypes.Picking);
                    var defProdLoc = LocationMapTasks.GetReadableLocation(
                        await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                    var prodDesc = physicalCountItem.ProductDescription;
                    var prodUnitSizeDesc = physicalCountItem.ProductUnitSizeDescription;
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defProdLoc.Trim());
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void DisplayDataItemInventoryInquiry()
            {
                this.ExtraLineItemInfo.Add(
                    this.ActItem.ProdInfo.Refno <= 0
                        ? "PRODUCT: All"
                        : $"PRODUCT: {ProductInfoTasks.GetCurrReadableProdBcUnit(this.ActItem.ProdInfo)}");

                this.ExtraLineItemInfo.Add($"LOCATION: {LocationMapTasks.GetCurrentReadableLocation()}");
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemCycleCount()
            {
                if (this.ListViewMainActivity.SelectedItem is CountItem cycleCountItem)
                {
                    // Get the cycle count line item location.
                    var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        Convert.ToInt32(cycleCountItem.LocationMapRefno));
                    ////var countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                    // Using the cycle count line item warehouse, get the product default
                    // picking location.
                    var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        countItemLocMap.WhsLocMapRefno,
                        Convert.ToInt32(cycleCountItem.ProductRefno),
                        SysCommon.ProdLocMapTypes.Picking);
                    var defProdLoc = LocationMapTasks.GetReadableLocation(
                        await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                    var prodDesc = cycleCountItem.ProductDescription;
                    var prodUnitSizeDesc = cycleCountItem.ProductUnitSizeDescription;
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                    this.ExtraLineItemInfo.Add("Default Location: " + defProdLoc.Trim());
                    ////this.ExtraLineItemInfo.Add("Count Location: " + countLoc.Trim());
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                    ////this.ExtraLineItemInfo.Add("Count Location: Unknown");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayDataItemAdjustment()
            {
                if (this.ListViewMainActivity.SelectedItem is AdjustmentItem adjustmentItem)
                {
                    var prodDesc = adjustmentItem.ProductDescription;
                    var prodUnitSizeDesc = adjustmentItem.ProductUnitSizeDescription;
                    this.ExtraLineItemInfo.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                    // Get product default location info
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        adjustmentItem.LocationMapRefno,
                        adjustmentItem.ProductRefno);
                    var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                    this.ExtraLineItemInfo.Add("Default Location: " + defLoc);
                }
                else
                {
                    this.ExtraLineItemInfo.Add("No product description.");
                    this.ExtraLineItemInfo.Add("Default Location: Unknown");
                }
            }
        }

        /// <summary>
        ///     Enable menu items.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task EnableMenuItems()
        {
            await Device.InvokeOnMainThreadAsync(
                () =>
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
                    {
                        var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                        this.CmdAddSkidIsEnabled = true;
                        this.CmdClearSelectedSkidIsEnabled = true;
                        this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;

                        // If the skid is a new skid or empty, the location is enabled.
                        // There is no new skid flag, so we check if the quantity is greater than 0.
                        this.CmdLocationIsEnabled = currSkid.Quantity == 0;

                        // When defaulting is turned on a new or empty skid will have the default location.
                        // The current location will be the default location.
                        var location = LocationMapTasks.GetCurrentReadableLocation();
                        this.CmdLocationText = "Location: " + Environment.NewLine + location;
                    }
                    else
                    {
                        this.CmdAddSkidIsEnabled = false;
                        this.CmdClearSelectedSkidIsEnabled = false;
                        this.CmdSelectSkidText = "Select Skid";
                        this.CmdLocationIsEnabled = true;
                    }

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

                    return Task.CompletedTask;
                });
        }

        /// <summary>
        ///     Option menu click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsDataLoading)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        _ = await Device.InvokeOnMainThreadAsync(
                            () => this.CmdLocationText =
                                "Location: " + Environment.NewLine + currentReadableLocation);
                        return Task.CompletedTask;
                    });

                var result = await this.DisplayActionSheet(
                    "ActivityItem Options",
                    "Close",
                    null,
                    this.OptionsMenuItems.ToArray());

                if (result == this.CmdLocationText &&
                    this.CmdLocationIsEnabled)
                {
                    await CmdLocationMenuClicked();
                }
                else if (result == this.CmdFilterText &&
                         this.CmdFilterIsEnabled)
                {
                    // TODO: Filter by form.
                }
                else if (result == this.CmdSelectSkidText &&
                         this.CmdSelectSkidIsEnabled)
                {
                    await CmdSelectSkidClicked();
                }
                else if (result == this.CmdAddProductText &&
                         this.CmdAddProductIsEnabled)
                {
                    await CmdAddProductClicked();
                }
                else if (result == this.CmdAddSkidText &&
                         this.CmdAddSkidIsEnabled)
                {
                    await CmdAddSkidClicked();
                }
                else if (result == this.CmdClearSelectedSkidText &&
                         this.CmdClearSelectedSkidIsEnabled)
                {
                    CmdClearSelectedSkidClicked();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                        return Task.CompletedTask;
                    });
            }
            finally
            {
                await this.ScannerEnable();
                await this.FindSelectedItemAsync();
                this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraLineItemInfo;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task CmdLocationMenuClicked()
            {
                var dspOverrideMsg = false;
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                         this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation) ||
                        (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                         this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                    {
                    }
                    else
                    {
                        dspOverrideMsg = true;
                    }
                }

                if (dspOverrideMsg)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                "Default locations are enabled, location changes cannot be overridden here.",
                                Resource.OK));
                }

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                    // Special validation for picking and filtering by section.
                    // Check if the scanned section matched the filtered one.
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    {
                        var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 &&
                            this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                        {
                            var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                      $" not match the filtered picking section '{sectName}'.";

                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        msg,
                                        Resource.OK));
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdSelectSkidClicked()
            {
                try
                {
                    // Do we need to have a line item selected to invoke the skid form?
                    // We are not allowing creation of new skids.
                    // Scanning a skid with attempt to find a matching line item for the skid product and location.
                    // The question is, do we need to have a line item selected to invoke the skid form?
                    // I am thinking that we do not need to have a line item selected to invoke the skid form. Because
                    // a line item can be created for some activity types.
                    // A new skid cannot be created, if it does not exist. The skid must be created in the detail
                    // activity level, which implies that a line item will be selected.
                    // Invoke the skid form without any line item information.
                    this.CmdClearSelectedSkidIsEnabled = false;
                    var form = new FormDialogContainer(
                        EDialogTypes.SkidInfo,
                        this.ActItem.ActHdr,
                        this.ActSubType,
                        SysShared.ActivityType.Items);
                    _ = await form.DisplayControl.ShowDialog();

                    // Current location was assigned in the form.
                    var xmlDoc = await form.GetXml();
                    var skidInfo = await SysSkidInfo.GetSkidInfoFromXml(xmlDoc);
                    if (skidInfo.CurrSkid.SkidFkRefno != 0 &&
                        skidInfo.CurrSkid.ProdInfo.Refno > 0 &&
                        skidInfo.CurrSkid.LocMapRefno > 0)
                    {
                        // Special validation for picking and filtering by section.
                        // Check if the skid section matched the activity filtered one.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            var filterSectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter
                                .SectLocMapRefno;
                            if (filterSectRefno != 0)
                            {
                                var skidLocMapRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMapRefno;
                                var skidLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(skidLocMapRefno);
                                if (skidLoc != null &&
                                    skidLoc.SectLocMapRefno > 0 &&
                                    skidLoc.SectLocMapRefno != filterSectRefno)
                                {
                                    var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter
                                        .SectName;
                                    var msg = $"Warning selected location section '{skidLoc.SectName}' does" +
                                              $" not match the filtered picking section '{sectName}'.";
                                    await Device.InvokeOnMainThreadAsync(
                                        async () =>
                                            await Application.Current.MainPage.DisplayAlert(
                                                this.ActItem.GetControlDescription(),
                                                msg,
                                                Resource.OK));
                                }
                            }
                        }

                        // Try to find a matching line item for the skid product and location.
                        var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            skidInfo.CurrSkid.ProdInfo.UnitRefno,
                            skidInfo.CurrSkid.LocMap);
                        if (actItemFoundRefno > 0)
                        {
                            this.CmdSelectSkidText = "Skid# " + skidInfo.CurrSkid.SkidCode;
                            this.CmdAddSkidIsEnabled = true;
                            this.CmdLocationIsEnabled = false;
                            this.ActItem.SetSelectedActivityItemFromList(actItemFoundRefno);
                            if (this.ActItem.SelectedItem == null)
                            {
                                throw new Exception("Failed to set the found selected item in the list.");
                            }

                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(
                                async () => await this.DisplayAlert(
                                    "Skid",
                                    "Skid product must match the selected line item product and the warehouse.",
                                    "OK"));

                            skidInfo = new SysSkidInfo();
                            this.CmdSelectSkidText = "Select Skid";
                            this.CmdAddSkidIsEnabled = false;
                            this.CmdLocationIsEnabled = true;
                        }

                        this.ActItem.ActHdr.SkidInfo.CurrSkid = skidInfo.CurrSkid;
                        await this.LoadAndBind();
                    }
                }
                finally
                {
                    this.CmdClearSelectedSkidIsEnabled = true;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void CmdClearSelectedSkidClicked()
            {
                this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                this.CmdSelectSkidIsEnabled = true;
                this.CmdClearSelectedSkidIsEnabled = false;
                this.CmdAddSkidIsEnabled = false;

                // Expected menu to be recreated.
                this.CmdSelectSkidText = "Select Skid";
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdAddProductClicked()
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                try
                {
                    XDocument xmlDoc;
                    ProductInfo prodInfo = null;

                    // If no skid is selected, then a product unit must be chosen.
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno == 0)
                    {
                        var form = new FormDialogContainer(
                            EDialogTypes.SelectProductUnit,
                            this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            xmlDoc = await form.GetXml();
                            if (xmlDoc.Root != null)
                            {
                                var prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno")?.Value);
                                if (prodUnitRefno == 0)
                                {
                                    throw new MessageOnlyException("A product unit must be chosen.");
                                }

                                prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            }
                        }
                        else
                        {
                            throw new CancelOperationException();
                        }
                    }
                    else
                    {
                        prodInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo;

                        if (prodInfo == null ||
                            prodInfo.UnitRefno == 0)
                        {
                            var form = new FormDialogContainer(
                                EDialogTypes.SelectProductUnit,
                                this.ActSubType);

                            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                            {
                                xmlDoc = await form.GetXml();
                                if (xmlDoc.Root != null)
                                {
                                    var prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno")?.Value);
                                    if (prodUnitRefno == 0)
                                    {
                                        throw new MessageOnlyException("A product unit must be chosen.");
                                    }

                                    prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                                    this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo = prodInfo;
                                }
                            }
                            else
                            {
                                throw new CancelOperationException();
                            }
                        }
                    }

                    if (prodInfo != null)
                    {
                        // Location used in finding match line item.
                        await this.SetDefaultLocationBeforeKnownActivityItem(prodInfo);

                        var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            prodInfo.UnitRefno,
                            LocationMap.GetInstance().CurrLocMap);
                        if (actItemFoundRefno > 0)
                        {
                            const string msg = "A line item already exists for this location and product.";
                            throw new MessageOnlyException(msg);
                        }

                        if (!this.ActItem.IsAllowAddItem)
                        {
                            const string msg = "Adding new line items is currently disabled.";
                            throw new MessageOnlyException(msg);
                        }

                        await this.CheckCurrentLocationMatchDefaultProductLocation(
                            prodInfo,
                            LocationMap.GetInstance().CurrLocMap);

                        // Insert into activity item table, and add new activity item to the list.
                        // Inserted item uses the current location.
                        // this.ActItem.ItemRefno assigned in Insert();
                        // this.ActItem.ItemFKRefno assigned in Insert();
                        // this.ActItem.ProdInfo assigned in Insert();
                        // this.ActItem.ProdInfo.Scheme assigned in Insert();
                        var unitSizeDesc = SysCommon.AddXmlReservedChars(prodInfo.UnitSizeDesc);
                        var xmlString = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                            <params>
                            <ProductUnitRefno>{prodInfo.UnitRefno}</ProductUnitRefno>
                            <ProductRefno>{prodInfo.Refno}</ProductRefno>
                            <ProductCode>{prodInfo.Code}</ProductCode>
                            <ProductDescription>{SysCommon.AddXmlReservedChars(prodInfo.Desc)}</ProductDescription>
                            <UnitCode>{prodInfo.UnitCode}</UnitCode>
                            <BaseUnitCode>{prodInfo.BaseUnitCode}</BaseUnitCode>
                            <ProductUnitBC>{prodInfo.UnitBc}</ProductUnitBC>
                            <ProductUnitSizeDescription>{unitSizeDesc}</ProductUnitSizeDescription>
                            </params>";
                        xmlDoc = XDocument.Parse(xmlString);

                        await this.ActItem.Insert(xmlDoc);
                        this.ActItem.SetSelectedActivityItemFromList(this.ActItem.ItemRefno);
                        if (this.ActItem.SelectedItem == null)
                        {
                            throw new Exception("Failed to set the found selected item in the list.");
                        }

                        await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                        this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0 &&
                            await LocationMapTasks.IsCompleteLocMap(this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap))
                        {
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap = this.LocMap.CurrLocMap.Clone();
                        }

                        this.ActivityItems.Add(this.ActItem);
                        await this.DesignGrid();
                        await this.FindSelectedItemAsync(this.ActItem.ItemFkRefno);
                    }
                }
                catch (CancelOperationException)
                {
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (GatherProductDataException ex)
                {
                    if (!ex.SuppressError)
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                            {
                                var errRpt = new FormErrorReport(this.ActSubType, ex);
                                _ = await errRpt.ShowDialog();
                                return Task.CompletedTask;
                            });
                    }

                    await this.LoadAndBind();
                }
                finally
                {
                    ////await this.FindSelectedItemAsync();
                    if (this.ActItem.ListItems.Count > 0)
                    {
                        this.ActItem.SelectedItem = this.ActItem.ListItems.LastOrDefault();
                        this.ActItem.SelectedListIdx = this.ActItem.ListItems.Count - 1;
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdAddSkidClicked()
            {
                try
                {
                    var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                    if (currSkid == null ||
                        currSkid.SkidFkRefno == 0)
                    {
                        const string msg = "Please select a skid to add.";
                        throw new MessageOnlyException(msg);
                    }

                    if (await this.DisplayAlert(
                            "Add Skid",
                            "Add the available skid quantity to this activity?",
                            "Yes",
                            "No"))
                    {
                        // Use the skid location to find the activity line item.
                        var prodUnitRefno = currSkid.ProdInfo.UnitRefno;
                        var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            prodUnitRefno,
                            currSkid.LocMap);

                        if (actItemFoundRefno > 0)
                        {
                            this.ActItem.SetSelectedActivityItemFromList(actItemFoundRefno);
                            if (this.ActItem.SelectedItem == null)
                            {
                                throw new Exception("Failed to set the found selected item in the list.");
                            }

                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                        }
                        else
                        {
                            if (!this.ActItem.IsAllowAddItem)
                            {
                                const string msg = "Adding new line items is currently disabled.";
                                throw new MessageOnlyException(msg);
                            }

                            await this.CheckCurrentLocationMatchDefaultProductLocation(
                                currSkid.ProdInfo,
                                currSkid.LocMap);

                            if (!this.ActItem.IsAllowAddItem)
                            {
                                throw new MessageOnlyException(
                                    this.ActItem.GetItemNotFoundMessage(
                                        this.LocMap,
                                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }

                            var xmlDoc = ProductInfoTasks.GetXml(currSkid.ProdInfo);

                            // Insert into activity item table, and add new activity item to the list.
                            // Inserted item uses the current location.
                            // this.ActItem.ItemRefno assigned in Insert();
                            // this.ActItem.ItemFKRefno assigned in Insert();
                            // this.ActItem.ProdInfo assigned in Insert();
                            // this.ActItem.ProdInfo.Scheme assigned in Insert();
                            await this.ActItem.Insert(xmlDoc);
                            this.ActItem.SetSelectedActivityItemFromList(this.ActItem.ItemRefno);
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);

                            if (actItemFoundRefno == 0)
                            {
                                await this.DesignGrid();
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(
                                    () =>
                                    {
                                        this.ListViewMainActivity.SelectedItem = this.ActItem;
                                        this.ListViewMainActivity.ScrollTo(
                                            this.ActItem,
                                            ScrollToPosition.MakeVisible,
                                            false);
                                    });
                                await this.DisplayLineItemDetail();
                                await this.DisplayDataItem();
                            }
                        }

                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        var actDetail = ActDetailFacade.GetActDetail(
                            this.ActItem,
                            this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                        // because of the length of the code and the fact isn't human-readable friendly.
                        var schemeData = actDetail.SchemeData;
                        schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        actDetail.SchemeData = schemeData;
                        actDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);
                        actDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate
                        // checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<imclient>" +
                            "<itm_refno>" +
                            this.ActItem.ItemFkRefno +
                            "</itm_refno>" +
                            "<alt_qty>" +
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity +
                            "</alt_qty>" +
                            "<wt_qty>" +
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity +
                            "</wt_qty>" +
                            "<ActivityDetailUniqueSchemeID>" +
                            actDetail.UniqueSchemeId +
                            "</ActivityDetailUniqueSchemeID>" +
                            "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to show new
                        // item and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            this.ActItem.SelectedItem,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        await this.LoadAndBind();
                        ////this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                        ////this.ActivityItems[this.ActItem.SelectedListIdx] = this.ActItem.SelectedItem;
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
                }
            }
        }

        /// <summary>
        ///     Text box search item focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void TxtSearchItem_Focused(object sender, EventArgs e)
        {
            this.IsSearchEntryFocused = true;
            this.SearchTimer.Start();
        }

        /// <summary>
        ///     Text box search item unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void TxtSearchItem_Unfocused(object sender, EventArgs e)
        {
            this.IsSearchEntryFocused = false;
            this.SearchTimer.Stop();
        }

        /// <summary>
        ///     Barcode modifier focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void BarCodeModifierPicker_Focused(object sender, FocusEventArgs e)
        {
            this.IsBarCodeModifierPickerFocus = true;
        }

        /// <summary>
        ///     Barcode modifier unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void BarCodeModifierPicker_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsBarCodeModifierPickerFocus = false;
        }
    }
}