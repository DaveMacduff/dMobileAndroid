// <copyright file="ActivityItem.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using Android.Util;

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
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
    /// Activity items control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItem : DMobileModal, IAppArea, IDataLayerEventListener
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
        /// Define the activity filter text information.
        /// </summary>
        private string cmdFilterText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the filter selection is enabled.
        /// </summary>
        private bool cmdFilterIsEnabled = false;

        /// <summary>
        /// Define the select skid text information.
        /// </summary>
        private string cmdSelectSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the skid selection is enabled.
        /// </summary>
        private bool cmdSelectSkidIsEnabled = false;

        /// <summary>
        /// Define the add a product text information.
        /// </summary>
        private string cmdAddProductText = string.Empty;

        /// <summary>
        /// Define a value indicating whether adding a product is enabled.
        /// </summary>
        private bool cmdAddProductIsEnabled = false;

        /// <summary>
        /// Define the add skid text information.
        /// </summary>
        private string cmdAddSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether adding a skid is enabled.
        /// </summary>
        private bool cmdAddSkidIsEnabled = false;

        /// <summary>
        /// Define the clear selected skid text information.
        /// </summary>
        private string cmdClearSelectedSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether clear selected a skid is enabled.
        /// </summary>
        private bool cmdClearSelectedSkidIsEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItem"/> class.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> Activity header instance.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubType"/> Activity sub type.</param>
        public ActivityItem(Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.CmdLocationText = "Set Location";
            this.CmdFilterText = "Filter";
            this.CmdSelectSkidText = "Select Skid";
            this.CmdClearSelectedSkidText = "Clear Skid";
            this.CmdAddProductText = "Add Prod";
            this.CmdAddSkidText = "Add Skid";

            this.LocMap = LocationMap.GetInstance();
            this.SysEnv = SysEnvironment.GetInstance();
            this.ActSubType = actSubTypes;
            this.ActItem = ActItemFacade.GetActItem(activity, this.ActSubType);
            this.ActItem.ActHdr = activity;

            this.CmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;

            // If the skid was not set on the header activity define an empty one.
            activity.SkidInfo ??= new SysSkidInfo();

            this.ParseAndSeedBarCodeModifier();

            // Page title.
            this.Title = this.ActItem.GetControlDescription();
        }

        /// <summary>
        /// Gets or sets the selected activity item foreign key reference number.
        /// </summary>
        private int SelectedActivityItemFkRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether to show a data valid message or raise an error.
        /// </summary>
        private bool IsShowDataValidMsg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether data changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        /// Gets or sets the warehouse location map instance.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the activity item instance.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler has been registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets a list of extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets a list of list view activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the options menu items list.
        /// </summary>
        private List<string> OptionsMenuItems { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a counter that tracks the number of screen taps, for double click.
        /// </summary>
        private int NumTaps { get; set; } = 0;

        /// <summary>
        /// Gets or sets the sender for tracking the number of screen taps, for double click.
        /// </summary>
        private object PreviousSender { get; set; }

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
        /// Gets or sets the command filter text.
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
        /// Gets or sets a value indicating whether the command filter is enabled.
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
        /// Gets or sets the command add product text.
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
        /// Gets or sets a value indicating whether the command add product is enabled.
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
        /// Gets or sets the command clear selected skid text.
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
        /// Gets or sets a value indicating whether the command clear selected skid is enabled.
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
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return "Activity Item";
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (isReadyToSave && (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                    this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
                {
                    isReadyToSave = await this.IsReadyToSavePicking();
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
            var isReadyToClose = !this.IsChangesMade;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                // Prairie meats like Lekker requirement to check quantity under even though we are
                // canceling / closing.
                // See WO# 12080
                if (isReadyToClose && (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                       this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
                {
                    isReadyToClose = await this.IsReadyToClosePicking();
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
            var isSaveComplete = false;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (await this.IsReadyToSave())
                {
                    ////ClearContents();

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
                        // IsReadyToSave will raise errors for invalid data. This is a catch all
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException(
                            "Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (MessageOnlyException ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var doc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>" + "</params>");

            return doc;
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle status information.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// event publisher.</param>
        /// <param name="dataLayerEventArgs"><see cref="DataLayerEventArgs"/> Data layer event
        /// arguments.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            ////ListViewDisplayExtraInfo.Items.Clear();
            ////const int lineLength = 30;
            ////List<string> lines = SysCommon.WordWrap2List(dataLayerEventArgs.StatusOutMsg, lineLength);
            ////for (int i = 0; i < lines.Count; i++)
            ////{
            ////    ListViewDisplayExtraInfo.Items.Add(lines[i]);
            ////}
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// event publisher.</param>
        /// <param name="dataLayerEventArgs">><see cref="DataLayerEventArgs"/> Data layer event
        /// arguments.</param>
        public void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            dataLayerEventArgs = dataLayerEventArgs ?? throw new ArgumentNullException(nameof(dataLayerEventArgs));
            if (dataLayerEventArgs.CurrentValue > 0 && dataLayerEventArgs.MaxValue > 0)
            {
                // ReSharper disable once PossibleLossOfFraction
                double percent = (100 * dataLayerEventArgs.CurrentValue) / dataLayerEventArgs.MaxValue;
            }
        }

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
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
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(this.BackButtonCallback);
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
        /// Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                this.Loading = true;
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.ScannerStart();
                    this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                    this.CmdClearSelectedSkidIsEnabled = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0
                        && this.CmdAddSkidIsEnabled && this.CmdSelectSkidIsEnabled;

                    this.ActItem.RegLayerListenerProgOut(this);
                    await this.RefreshBySubType();
                    this.ActItem.UnregLayerListenerProgOut(this);
                    await this.DesignGrid();

                    if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(() => this.ExtraInfoItems.Clear());
                    }

                    await this.ActAppearing();
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    const string msg = "This activity unexpectedly failed to initialize correctly." +
                              " Please exit and try again.";
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
        /// Populate the options menu items.
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
        /// Start, initialize the bar code scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Subscribe key up handlers, other events and find the selected item using this.SelectedActivityItemFKRefno.
        /// </summary>
        private async Task ActAppearing()
        {
            await this.ScannerEnable();
            this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.SubscribeKeyUpHandlers();
            await this.FindSelectedItemAsync();
        }

        /// <summary>
        /// Subscribe the key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(
                this,
                EventTypes.OnKeyUp.ToString(),
                this.KeyUpHandlerCallback);
        }

        /// <summary>
        /// Key up handler call back.
        /// </summary>
        /// <param name="keyPressed"><see cref="string"/> Key pressed value.</param>
        private async void KeyUpHandlerCallback(string keyPressed)
        {
            if (!this.Loading)
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                switch (keycode)
                {
                    case Keycode.Clear:
                        await this.AppearingClearKey();
                        break;

                    case Keycode.Del:
                        await this.AppearingDelKey();
                        break;

                    case Keycode.DpadDown:
                        this.AppearingDpadDown();
                        break;

                    case Keycode.DpadUp:
                        this.AppearingDpadUp();
                        break;

                    case Keycode.Enter:
                        await this.AppearingEnterKey();
                        break;

                    case Keycode.F1:
                        await this.AppearingF1Key();
                        break;

                    case Keycode.F2:
                        await this.OnAppearingF2Key();
                        break;

                    case Keycode.F3:
                        await this.OnAppearingF3Key();
                        break;

                    case Keycode.F4:
                        await this.OnAppearingF4Key();
                        break;
                }
            }
        }

        /// <summary>
        /// Enter key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task AppearingEnterKey()
        {
            if (this.ListViewMainActivity.SelectedItem == null ||
                Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        "Please select an item.",
                        Resource.OK));
            }
            else
            {
                await this.ListViewMainActivity_KeyUpEnter();
            }
        }

        /// <summary>
        /// Clear key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task AppearingClearKey()
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;

                // TODO: DEV-5619
                // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                await this.ActDisappearing();
                await this.Close();
            }
        }

        /// <summary>
        /// F1 key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task AppearingF1Key()
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;

                // TODO: DEV-5619
                // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                await this.ActDisappearing();
                await this.Close();
            }
        }

        /// <summary>
        /// Dpad down key up handler.
        /// </summary>
        private void AppearingDpadDown()
        {
            if (this.ListViewMainActivity.SelectedItem != null && Convert.ToInt32(this.MainActivityItems.IndexOf(
                this.ListViewMainActivity.SelectedItem)) < this.MainActivityItems.Count - 1)
            {
                this.ListViewMainActivity.SelectedItem =
                    this.MainActivityItems[this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) + 1];
                this.ListViewMainActivity.ScrollTo(
                    this.ListViewMainActivity.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
            }
        }

        /// <summary>
        /// Dpad up key up handler.
        /// </summary>
        private void AppearingDpadUp()
        {
            if (this.ListViewMainActivity.SelectedItem != null &&
                Convert.ToInt32(this.MainActivityItems.IndexOf(
                    this.ListViewMainActivity.SelectedItem)) > 0)
            {
                this.ListViewMainActivity.SelectedItem =
                    this.MainActivityItems[this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) - 1];
                this.ListViewMainActivity.ScrollTo(
                    this.ListViewMainActivity.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
            }
        }

        /// <summary>
        /// Delete key handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        /// <exception cref="MessageOnlyException"> Selected skid warehouse and product does not match line item.</exception>
        private async Task AppearingDelKey()
        {
            var isItemSelected =
                Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) >= 0;
            if (isItemSelected)
            {
                // If there is a skid selected
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                {
                    // Ask to delete the skid
                    var msg = $"Do you wish to delete all this skid" +
                              $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' detail?";
                    if (await this.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            "Yes",
                            "No"))
                    {
                        // Make sure that the warehouse, product, bc, and unit match.
                        var skidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                        if (this.ActItem.ProdInfo.FKRefno != skidInfo.ProdInfo.FKRefno ||
                            this.ActItem.ProdInfo.UnitIsBC != skidInfo.ProdInfo.UnitIsBC ||
                            this.ActItem.ProdInfo.UnitRefno != skidInfo.ProdInfo.UnitRefno ||
                            this.ActItem.LocMap.WhsFKRefno != skidInfo.LocMap.WhsFKRefno)
                        {
                            msg = "The selected skid warehouse and product must be the same as the line item" +
                                  " warehouse and product before the skid can be deleted.";
                            throw new MessageOnlyException(msg);
                        }

                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        await actDetail.DeleteSkid(this.ActItem.ActHdr.SkidInfo);

                        var fillQty = await actDetail.GetTotalQuantityFromListDetail();
                        this.ActItem.ReplaceQuantityInActivityItem(
                            this.ActItem.SelectedActivityItem,
                            fillQty.Quantity,
                            fillQty.BaseQuantity,
                            fillQty.WeightQuantity);
                        await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedActivityItem);
                        this.ActItem.ListItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                            // This line is needed to display the new updated quantity visually.
                            this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                        });
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                "Item deletion needs to be performed on the desktop.",
                                Resource.OK));
                    }
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            "Item deletion needs to be performed on the desktop.",
                            Resource.OK));
                }
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        "Nothing selected to delete.",
                        Resource.OK));
            }
        }

        /// <summary>
        /// F2 key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task OnAppearingF2Key()
        {
            try
            {
                this.ListViewMainActivity.IsEnabled = false;
                if (this.ListViewMainActivity.SelectedItem != null &&
                    this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem) < 0)
                {
                    throw new MessageOnlyException(
                        "Please select a line item to show the comments for.");
                }

                string comment = await this.ActItem.GetComment(this.ListViewMainActivity.SelectedItem as dynamic);
                if (!string.IsNullOrEmpty(comment))
                {
                    await this.DisplayAlert("Item Comment", comment, "OK");
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
                this.ListViewMainActivity.IsEnabled = true;
            }
        }

        /// <summary>
        /// F3 key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task OnAppearingF3Key()
        {
            try
            {
                var msg = await this.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            "OK"));
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        /// F4 key up handler.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task OnAppearingF4Key()
        {
            try
            {
                this.ListViewMainActivity.IsEnabled = false;
                await this.ActDisappearing();

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
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
                this.ListViewMainActivity.IsEnabled = true;
            }
        }

        /// <summary>
        /// Check if there are any skids to print.
        /// </summary>
        /// <returns><see cref="bool"/> True if there are skids to be printed, else alse.</returns>
        /// <exception cref="MessageOnlyException"> Request skid check failed.</exception>
        /// <exception cref="Exception"> Error returned request skid check.</exception>
        private async Task<bool> CheckIfSkids2Print()
        {
            var isExist2Print = false;
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings() { Async = true });
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + await SysCommon.GetRequestDefaults(false)
                + "<command>"
                + "<action>REQUEST CHECK IF SKIDS 2 PRINT</action>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<fk_refno>" + this.ActItem.ActHdr.HdrFkRefno + "</fk_refno>"
                + "<fk_source>PO</fk_source>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            var reqData = SysCommon.SubmitComm(xmlString.ToString());

            if (reqData.Length > 0)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);
                if (xmlDoc.DocumentElement == null)
                {
                    throw new Exception("Request to check if there are skids to print failed.");
                }

                if (reqData.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = xmlDoc.DocumentElement["detail"]?.InnerText;
                    if (errorDetail != null && errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException("Check skids to print error." + Environment.NewLine + errorDetail);
                    }
                    else
                    {
                        throw new Exception("Check skids to print error." + Environment.NewLine + errorDetail);
                    }
                }
                else if (reqData.Contains("<detail>True</detail>"))
                {
                    isExist2Print = true;
                }
            }
            else
            {
                throw new Exception("Request to check if there are skids to print failed.");
            }

            return isExist2Print;
        }

        /// <summary>
        /// Unsubscribe from KeyUpHandler, other event hooks and turn off the scanner.
        /// </summary>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
            await this.ScannerDisable();
        }

        /// <summary>
        /// Double click on list view item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="ItemTappedEventArgs"/> Item tapped event arguments.</param>
        private async void ListViewMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            this.NumTaps++;

            if (this.PreviousSender == e.Item)
            {
                if (this.NumTaps == 2)
                {
                    this.NumTaps = 0;

                    if (this.ListViewMainActivity.SelectedItem != null &&
                        (Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > -1))
                    {
                        await this.ListViewMainActivity_KeyUpEnter();
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
        /// Check if the activity item has been reviewed.
        /// </summary>
        private void IsItemReviewed()
        {
            var isReviewed = true;

            // TODO: Looks like confirmations have not been implemented. Currently picking and
            // warehouse transfers can use confirmations.
            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                if (this.ActItem.SelectedActivityItem != null)
                {
                    // TODO: test
                    isReviewed = Convert.ToBoolean(
                        (this.ActItem.SelectedActivityItem as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// Check if ALL the activity items has been reviewed.
        /// </summary>
        private void IsAllItemsReviewed()
        {
            var isReviewed = true;

            // TODO: Looks like confirmations have not been implemented. Currently picking and
            // warehouse transfers can use confirmations.
            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                for (var idx = 0; idx <= this.ActItem.ListItems.Count - 1; idx++)
                {
                    // TODO: test
                    isReviewed = isReviewed && Convert.ToBoolean(
                        (this.ActItem.ListItems[idx] as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }

            ////if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel &&
            ////    (this.ActSubType == SysCommon.ActSubTypes.Picking ||
            ////    this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
            ////{
            ////    bool isReviewed = true;
            ////    foreach (object item in this.ActItem.ListItems)
            ////    {
            ////        isReviewed = isReviewed && ((PickingItem)item).ActivityItemPickingIsReviewed;
            ////    }

            ////    if (isReviewed)
            ////    {
            ////        ((Picking)this.ActItem.ActHdr.SelectedActivity).ActivityPickingIsReviewed = true;
            ////    }

            ////    if (!await ((PickingActivity)this.ActItem.ActHdr).ClearSalesOrderBeingPicked(true))
            ////    {
            ////        await Device.InvokeOnMainThreadAsync(async () =>
            ////            await Application.Current.MainPage.DisplayAlert(
            ////                this.ActItem.GetControlDescription(),
            ////                "Unable to clear the current picker.",
            ////                Resource.OK));
            ////    }
            ////}
        }

        /// <summary>
        /// Perform quantity under check for ALL line items.
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
                        if (available > 0 && shipped < ordered)
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
        /// Perform quantity under check for one line item.
        /// </summary>
        /// <remarks>Line items must exist, one needs to be selected.</remarks>
        private async void ItemQuantityUnderCheck()
        {
            if (this.ActItem.ListItems != null
                && this.ActItem.SelectedActivityItem != null
                && this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                var selectedItem = (PickingItem)this.ActItem.SelectedActivityItem;
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
                                Environment.NewLine + "On Hand {0} {1}",
                                Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable),
                                prodInfo.BaseUnitCode);
                        }
                        else
                        {
                            msg = string.Format(
                                "The quantity shipped is less than the quantity ordered." +
                                Environment.NewLine + "On Hand {0} {1}",
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
                        throw new MessageOnlyException(
                            "The quantity shipped is less than the quantity ordered.");
                    }
                }
            }
        }

        /// <summary>
        /// Main data grid key up enter event handler.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        private async Task ListViewMainActivity_KeyUpEnter()
        {
            if (this.Loading ||
                this.ListViewMainActivity.SelectedItem == null ||
                Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
            {
                return;
            }

            await Task.Run(async () =>
            {
                try
                {
                    await this.ActDisappearing();
                    await this.ActItem.AssignActivityItem(this.ListViewMainActivity.SelectedItem);
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                        {
                            throw new MessageOnlyException(
                                "Selected line item product must match the selected skid product.");
                        }
                    }

                    var form = new FormDialogContainer(EDialogTypes.ActivityDetail, this.ActItem, this.ActSubType);
                    this.IsChangesMade = false;

                    // There currently no way to cancel from the detail screen. Always returns DialogResult.OK.
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var xmlDoc = await form.GetXML();
                        var qty = xmlDoc?.Root?.Element("qty")?.Value ?? "0";
                        var baseQty = xmlDoc?.Root?.Element("base_qty")?.Value ?? "0";
                        var weightQty = xmlDoc?.Root?.Element("wt_qty")?.Value ?? "0";

                        this.ActItem.ReplaceQuantityInActivityItem(
                            this.ActItem.SelectedActivityItem,
                            Math.Round(decimal.Parse(qty), 3),
                            Math.Round(decimal.Parse(baseQty), 3),
                            Math.Round(decimal.Parse(weightQty), 3));
                        await this.ActItem.UpdateItemTableQuantity(this.ListViewMainActivity.SelectedItem);
                        this.ActItem.ListItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
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
            });
        }

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Sender bar code scanner event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            this.Loading = true;

            await this.ActDisappearing();

            object selectedItem = null;
            var actItemFoundRefno = 0;
            var tempItemProdSchemeData = new ActItemSchemeData();

            try
            {
                var parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    await this.BarCodeHandlerLocationIdentifier(parse);
                }
                else
                {
                    var prodUnitRefno = 0;
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        // Identify the product from the parse info.
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);
                        var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        await this.SetLocation(scannedProdInfo);

                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        if (actItemFoundRefno > 0)
                        {
                            this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                            selectedItem = this.ActItem.SelectedActivityItem;
                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
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
                                    await this.ActItem.Insert(ProductInfoTasks.GetXML(scannedProdInfo), false);
                                    selectedItem = this.ActItem.SelectedActivityItem;
                                    this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                                    actItemFoundRefno = this.ActItem.ItemRefno;

                                    // Reload all the line items from the ActItem.ListItem collection and populate the
                                    // List view.
                                    // NOTE: Change this to add one only the ListView / Grid at a time on the end.
                                    // For example a physical / cycle count could have 1000 plus line on a document.
                                    // Plus add new line items consistently with already many existing will get
                                    // progressively slower.
                                    await this.DesignGrid();
                                }
                                else
                                {
                                    throw new GatherProductDataException(
                                        this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo), true);
                                }
                            }
                            else
                            {
                                throw new MessageOnlyException(this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo));
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        selectedItem = this.ActItem.ListItems.OfType<object>().FirstOrDefault(activityItem =>
                            Convert.ToString(activityItem.GetType().GetProperty(
                                this.ActItem.GetDataViewSortFkRefnoIdx())?.GetValue(activityItem)) ==
                                Convert.ToString(scanArgs.ParseInfo.ActItemFKRefno));
                        if (selectedItem == null)
                        {
                            throw new Exception(this.ActItem.GetUnableToFindItemFkRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                        }
                        else
                        {
                            await this.ActItem.AssignActivityItem(selectedItem);
                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                            actItemFoundRefno = this.ActItem.ItemRefno;
                        }
                    }
                    else if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException(
                            "Scanning an line item detail bar code is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                            parse.SSCC18);

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            throw new MessageOnlyException("Skid does not exist.");
                        }

                        prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        if (actItemFoundRefno > 0)
                        {
                            this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                            selectedItem = this.ActItem.SelectedActivityItem;
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                var data = ProductInfoTasks.GetXML(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);

                                // Additional found matching line items are not allowed to be created during
                                // scanning and inserting. I am not expecting matches to be found because
                                // that check has already been performed at this point.
                                await this.ActItem.Insert(data, askToInsertAsNewItem: false);
                                selectedItem = this.ActItem.SelectedActivityItem;
                                this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                throw new MessageOnlyException(
                                    this.ActItem.GetItemNotFoundMessage(
                                        this.LocMap,
                                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }
                        }
                    }

                    // Check the location again because of product location defaulting.
                    await this.VerifyLocationIsValid();

                    // Check that the skid product matches the identified product unit.
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno !=
                            this.ActItem.ProdInfo.UnitRefno)
                        {
                            throw new MessageOnlyException(
                                "Identified line item product does not match the selected skid product.");
                        }
                    }

                    // If NOT a container, evaluate
                    if (string.IsNullOrEmpty(parse.SSCC18))
                    {
                        await this.CheckCurrentLocationMatchProductLocation(selectedItem);

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        //// if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        //// {
                        ////    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////    {
                        ////        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////        this.cmdSelectSkid.Text = Translator.GetInstance().Translate(
                        ////        TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////        this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////    }
                        //// }

                        var isInsertReady = false;
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        var schemeData = actDetail.SchemeData;
                        schemeData = this.DefaultSchemeDataFromCurrentSkidSchemeDetail(actDetail);
                        tempItemProdSchemeData.ProdSchemeData = schemeData;
                        actDetail.SchemeData = schemeData;

                        isInsertReady = await SysShared.GatherProductData(
                            parse,
                            this.ActSubType,
                            this.ActItem,
                            tempItemProdSchemeData,
                            false,
                            actDetail,
                            false);

                        if (isInsertReady)
                        {
                            var xmlDoc = actDetail.GetXml(actDetail);
                            await actDetail.Insert(xmlDoc);

                            // TODO check if actually new, if not delete and change counter
                            // Does the row exist? If not, then it has been removed due to not needing it
                            // Skip update qty.
                            if (selectedItem != null)
                            {
                                // Upon a save of completed data update the data view to to show new item and
                                // quantity change.
                                this.ActItem.AddToQuantityInActivityItem(
                                    selectedItem,
                                    actDetail.Quantity,
                                    actDetail.BaseQuantity,
                                    actDetail.WeightQuantity);
                                await this.ActItem.UpdateItemTableQuantity(selectedItem);
                                this.ActItem.SelectedActivityItem = selectedItem;
                                this.ActItem.ListItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                                    // This line is needed to display the new updated quantity visually.
                                    this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                                });
                            }
                        }
                    }
                    else
                    {
                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        actDetail.SchemeId = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);
                        actDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks
                        // and quantity validations.
                        var xmlParam = XDocument.Parse(
                                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                    + "<imclient>"
                                    + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                                    + "<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                                    + "<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                                    + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                                    + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        if (actDetail.Quantity == 0)
                        {
                            throw new MessageOnlyException($"No quantity exists on skid" +
                                $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' in warehouse" +
                                $" {LocationMap.GetInstance().CurrLocMap.WhsCode}.");
                        }

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            selectedItem,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);
                        await this.ActItem.UpdateItemTableQuantity(selectedItem);
                        this.ActItem.SelectedActivityItem = selectedItem;
                        this.ActItem.ListItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                            // This line is needed to display the new updated quantity visually.
                            this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                        });
                    }

                    // IF AN ACTIVITY LINE ITEM WAS FOUND
                    if (actItemFoundRefno == 0 || selectedItem == null)
                    {
                        throw new MessageOnlyException("Line item not found.");
                    }
                    else
                    {
                        this.ActItem.SelectedActivityItem = selectedItem;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (GatherProductDataException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (IsReadyToApplyException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

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

            this.Loading = false;
        }

        /// <summary>
        /// Print skid labels.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
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
                    var reqData = await this.SubmitPrintSkidLabelsRequest();
                    if (reqData.Length > 0)
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var errorDetail = xmlDoc.DocumentElement?["detail"]?.InnerText ?? string.Empty;
                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new MessageOnlyException(
                                    "Print all skid label(s) error." + Environment.NewLine +
                                    errorDetail);
                            }
                            else
                            {
                                throw new Exception("Print all skid label(s) error." +
                                    Environment.NewLine + errorDetail);
                            }
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    "Skid labels printed successfully.",
                                    Resource.OK));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform print skid labels request.
        /// </summary>
        /// <returns><see cref="Task{T}"/> Request result.</returns>
        private async Task<string> SubmitPrintSkidLabelsRequest()
        {
            var xmlString = await this.MakeSkidLabelsRequestString();
            return SysCommon.SubmitComm(xmlString);
        }

        /// <summary>
        /// Make the print labels request string.
        /// </summary>
        /// <returns><see cref="string"/> Print labels request string.</returns>
        private async Task<string> MakeSkidLabelsRequestString()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, new XmlWriterSettings() { Async = true });
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + await SysCommon.GetRequestDefaults(false)
                + "<command>"
                + "<action>SUBMIT PRINT ALL SKID LABELS</action>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<fk_refno>" + this.ActItem.ActHdr.HdrFkRefno + "</fk_refno>"
                + "<fk_source>PO</fk_source>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();
            return xmlString.ToString();
        }

        /// <summary>
        /// Verify the current location matches the default location.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        /// <exception cref="MessageOnlyException"> Location must be set first.</exception>
        private async Task VerifyLocationIsValid()
        {
            // Check the location again because of product location defaulting.
            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                throw new MessageOnlyException("Location must be set first.");
            }
        }

        /// <summary>
        /// Check if the current location matches the product default location.
        /// </summary>
        /// <param name="selectedItem"><see cref="object"/> Selected activity item instance.</param>
        /// <returns><see cref="Task"/> Task oeration.</returns>
        /// <exception cref="Exception"> Invalid warehouse on picking activity item.</exception>
        /// <exception cref="Exception"> Invalid product on picking activity item.</exception>
        /// <exception cref="MessageOnlyException"> Current location does not match product default location.</exception>
        private async Task CheckCurrentLocationMatchProductLocation(object selectedItem)
        {
            if (await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                var isVerifyActivity = this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                       this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                                       this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                                       this.ActSubType == SysCommon.ActSubTypes.TransferTo;

                // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                if (this.SysEnv.SysSettings.IsVerifyProductLocation && isVerifyActivity)
                {
                    // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                    // Picking activity items only have the warehouse, not section or slot.
                    var whsLocMapRefno =
                        Convert.ToInt32(
                            selectedItem.GetType().GetProperty("LocationMapRefno")?.GetValue(selectedItem) ?? "0");
                    if (whsLocMapRefno <= 0)
                    {
                        throw new Exception("Invalid warehouse on picking activity item. LocationMapRefno == 0");
                    }

                    var prodRefno =
                        Convert.ToInt32(selectedItem.GetType().GetProperty("ProductRefno")?.GetValue(selectedItem) ?? "0");
                    if (prodRefno <= 0)
                    {
                        throw new Exception("Invalid product on picking activity item. ProductRefno == 0");
                    }

                    var locMapRefno = await this.ActItem.GetDefaultLocMapRefno(whsLocMapRefno, prodRefno);
                    if (this.LocMap.CurrLocMapRefno != locMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The current location does not match the product home location.");
                    }
                }
            }
        }

        /// <summary>
        /// Default scheme data from the current skid scheme detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail containing the skid information.</param>
        /// <returns><see cref="ActDetail"/> Scheme data from current skid.</returns>
        private ProductSchemeData DefaultSchemeDataFromCurrentSkidSchemeDetail(ActDetail actDetail)
        {
            // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
            // because of the length of the code and the fact is isn't human readable friendly.
            var schemeData = actDetail.SchemeData;
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
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
        /// Bar code handler location identifier.
        /// </summary>
        /// <param name="parse"></param>
        /// <returns></returns>
        private async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
        {
            // If section filtering was set in the activity header. Do we want to enforce
            // the scanned section location.
            var tmpLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                    parse.LocationIdentifier));
            this.LocMap.CurrLocMap = tmpLoc;
            this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();

            await this.CheckIfScannedSectionMatchesFilteredOne(tmpLoc);
        }

        /// <summary>
        /// Check if the scanned location section matches the currently set filtered picking one.
        /// </summary>
        /// <param name="scannedLocMap"><see cref="SLocMap"/> Scanned location map.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task CheckIfScannedSectionMatchesFilteredOne(SLocMap scannedLocMap)
        {
            // Special validation for picking and filtering by section.
            // Check if the scanned section matched the filtered one.
            if (this.ActSubType == SysCommon.ActSubTypes.Picking
                || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
            {
                var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                if (sectRefno != 0 && scannedLocMap.SectLocMapRefno != sectRefno)
                {
                    var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                    var msg = $"Warning scanned location section '{scannedLocMap.SectName}' does" +
                              $" not match the filtered picking section '{sectName}'.";

                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
            }
        }

        /// <summary>
        /// Ask to add new activity item.
        /// </summary>
        /// <param name="scannedProdInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="Task{T}"/> True if allowed to create a new activity item.</returns>
        private async Task<bool> AskToAddNewItem(ProductInfo scannedProdInfo)
        {
            var isAddNewItem = false;
            if (this.ActItem.IsAskCreateNewItem)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    if (await this.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        string.Format(
                            "A line item does not exist for this warehouse {0}" +
                                " and product \'{1}.\'" + Environment.NewLine +
                                "Do you wish to create one?",
                            this.LocMap.CurrLocMap.WhsCode,
                            ProductInfoTasks.GetCurrReadableProdBCUnit(scannedProdInfo)),
                        "Yes",
                        "No"))
                    {
                        isAddNewItem = true;
                    }
                });
            }
            else
            {
                isAddNewItem = true;
            }

            return isAddNewItem;
        }

        /// <summary>
        /// Set the current location.
        /// </summary>
        /// <param name="productInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        /// <exception cref="MessageOnlyException"> Location must be set first. Location is incomplete.</exception>
        private async Task SetLocation(ProductInfo productInfo)
        {
            if (this.IsAllowedDefaultLocationOverride())
            {
                this.SetCurrentLocationWithChosen();
            }
            else
            {
                await this.DefaultToFloorLocation(productInfo).ConfigureAwait(false);
            }

            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
            {
                throw new MessageOnlyException("Location must be set first!");
            }
        }

        /// <summary>
        /// Set the current location with the chosen location.
        /// </summary>
        private void SetCurrentLocationWithChosen()
        {
            // This should not be needed, current location should have been already set using the location picker.
            if (this.LocMap.CurrLocMapRefno != this.LocMap.ChosenLocMapRefno)
            {
                // Set the current location to the chosen one, overriding and defaulting location behaviour.
                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
            }
        }

        /// <summary>
        /// Default the location to the floor for a product.
        /// </summary>
        /// <param name="productInfo"><see cref="ProductInfo"/> Product information.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DefaultToFloorLocation(ProductInfo productInfo)
        {
            var whsLocMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(locationMapRefno: 0);

            // We can't determine the product location map base on the sub type and quantity.
            // We don't have the quantity yet, so default to the floor.
            var tmpLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                whsLocMapRefno,
                productInfo.Refno,
                SysCommon.ProdLocMapTypes.Floor);
            if (await LocationMapTasks.IsCompleteLocMap(tmpLocMapRefno))
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(tmpLocMapRefno);
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
            }
        }

        /// <summary>
        /// Check if default location is allowed to be overridden.
        /// </summary>
        /// <returns><see cref="bool"/> True if the location override is allowed else false.</returns>
        private bool IsAllowedDefaultLocationOverride()
        {
            var isAllowedLocationOverride = false;
            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                     this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                    || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                        this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                {
                    isAllowedLocationOverride = true;
                }
            }

            return isAllowedLocationOverride;
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
                    // Do not refresh product list for counts. Except on first load. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as
                    // MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    if (this.IsFirstAppearance)
                    {
                        await this.RefreshData();
                    }

                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    // Do not refresh product list for counts. Except on first load. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as
                    // MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    if (this.IsFirstAppearance)
                    {
                        await this.RefreshData();
                    }

                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Returns:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.LoginStartup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SplashStartup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PrintProductLabel:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSelectCriteria:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingFilter:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SubmitBatch:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.CommunicationSetup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SetupDefaults:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquirySummary:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    await this.RefreshData();
                    break;

                default:
                    // Performing a refresh data will remove and partially added new items or quantity.
                    await this.RefreshData();
                    break;
            }
        }

        /// <summary>
        /// Refresh the activity item data.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        private async Task RefreshData()
        {
            try
            {
                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ActItem.RegLayerListenerProgOut(this);
                    await this.ActItem.Refresh(this.ActItem.ActHdr.XmlDataCriteria);
                    await Device.InvokeOnMainThreadAsync(() => this.ActItem.ListItems.Clear());
                    this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(
                        this.ActSubType,
                        ex,
                        this.ActItem.GetControlDescription(),
                        "Refresh");
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.ActItem.UnregLayerListenerProgOut(this);
                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }
        }

        /// <summary>
        /// Design the main activity grid / list.
        /// </summary>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
        private async Task DesignGrid()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                // Redesign the grid / list and rebind it to the refreshed list items.
                this.MainActivityItems = this.ActItem.DesignGrid(
                    ref this.GridMainActivity,
                    ref this.ListViewMainActivity);

                if (!string.IsNullOrEmpty(this.txtSearchItem.Text))
                {
                    this.MainActivityItems = this.ActItem.Search(this.txtSearchItem.Text);
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
            if (this.ActItem.ListItems.Count > 0)
            {
                this.ActItem.SelectedListItemIdx = 0;
                object selectedItem = null;
                foreach (var item in this.ActItem.ListItems)
                {
                    if (this.SelectedActivityItemFkRefno == this.ActItem.GetFkRefno(item))
                    {
                        selectedItem = item;
                        break;
                    }

                    this.ActItem.SelectedListItemIdx++;
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActItem.SelectedListItemIdx = 0;
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActItem.SelectedListItemIdx];
                    this.SelectedActivityItemFkRefno = await this.ActItem.AssignActivityItem(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    this.SelectedActivityItemFkRefno = await this.ActItem.AssignActivityItem(selectedItem);
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                    ////Device.BeginInvokeOnMainThread(() =>
                    ////{
                    ////    this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                    ////});
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.ScrollTo(this.ListViewMainActivity.SelectedItem, ScrollToPosition.MakeVisible, false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActItem.SelectedListItemIdx = -1;
                this.ActItem.SelectedActivityItem = null;
                this.ActItem.ItemFkRefno = 0;
                this.ActItem.ItemRefno = 0;
                this.SelectedActivityItemFkRefno = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// Text search activity items text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Sender object text changed event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void TxtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
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
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        /// Bar code modifier selected index changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
        }

        /// <summary>
        /// Data grid activity item selected item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Sender selected item changed
        /// event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActItem.AssignActivityItem(e.SelectedItem);
            this.ActItem.SelectedListItemIdx = e.SelectedItemIndex;
            this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
            await this.DisplayDataItem();
        }

        /// <summary>
        /// Activity item display data.
        /// </summary>
        private async Task DisplayDataItem()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                this.ExtraInfoItems.Clear();

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                {
                    this.CmdAddSkidIsEnabled = true;
                    this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    ////this.ExtraInfoItems.Add("Selected Skid #: " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                }
                else
                {
                    this.CmdAddSkidIsEnabled = false;
                    this.CmdSelectSkidText = "Select Skid";
                }

                this.ExtraInfoItems.Add("Selected Location: " + LocationMapTasks.GetCurrentReadableLocation());

                if (this.LocMap.CurrLocMap.WhsLocMapRefno > 0)
                {
                    this.CmdLocationText = "Location: " + Environment.NewLine +
                        LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.CmdLocationText = "Set Location";
                }

                this.CmdLocationIsEnabled = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno <= 0;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        this.LocMap.LoginWhsLocMapRefno);
                }

                switch (this.ActItem.ActSubType)
                {
                    case SysCommon.ActSubTypes.Adjustment:
                        await this.DisplayDataItemAdjustment();
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
                        await this.DisplayDataItemCycleCount();

                        break;

                    case SysCommon.ActSubTypes.DataStatus:
                        break;

                    case SysCommon.ActSubTypes.InventoryInquiry:
                        this.DisplayDataItemInventoryInquiry();
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
                        await this.DisplayDataItemPhysicalCount();
                        break;

                    case SysCommon.ActSubTypes.Picking:
                        await this.DisplayDataItemPicking();
                        break;

                    case SysCommon.ActSubTypes.PickingSingle:
                        await this.DisplayDataItemPickingSingle();
                        break;

                    case SysCommon.ActSubTypes.TicketAddProduct:
                        await this.DisplayDataItemTicketAddProduct();
                        break;

                    case SysCommon.ActSubTypes.TicketFinished:
                        await this.DisplayDataItemTicketFinished();
                        break;

                    case SysCommon.ActSubTypes.TicketMaterial:
                        await this.DisplayDataItemTicketMaterial();
                        break;

                    case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                        await this.DisplayDataItemPurchaseOrderWorkSheet();
                        break;

                    case SysCommon.ActSubTypes.PurgeActivityData:
                        break;

                    case SysCommon.ActSubTypes.PutAwaySkid:
                        break;

                    case SysCommon.ActSubTypes.Receiving:
                        await this.DisplayDataItemReceiving();
                        break;

                    case SysCommon.ActSubTypes.RefreshData:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByItemSection:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByOrder:
                        break;

                    case SysCommon.ActSubTypes.Returns:
                        await this.DisplayDataItemReturns();
                        break;

                    case SysCommon.ActSubTypes.SelectLocation:
                        break;

                    case SysCommon.ActSubTypes.SelectProduct:
                        break;

                    case SysCommon.ActSubTypes.SkidInfo:
                        break;

                    case SysCommon.ActSubTypes.TransferFrom:
                        await this.DisplayDataItemTransferFrom();
                        break;

                    case SysCommon.ActSubTypes.TransferTo:
                        await this.DisplayDataItemTransferTo();
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
        /// Display the activity item transfer to data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemTransferTo()
        {
            if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemTo)
            {
                ////var prodLocCode = Convert.ToString(transferItemTo.LocationCode);
                //// prodLocName = transferItemTo.LocationName;
                var prodDesc = transferItemTo.ProductDescription;
                var prodUnitSizeDesc = transferItemTo.ProductUnitSizeDescription;
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    transferItemTo.LocationCode,
                    transferItemTo.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item transfer from data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemTransferFrom()
        {
            if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemFrom)
            {
                var prodLocCode = Convert.ToString(transferItemFrom.LocationCode);
                var prodLocName = transferItemFrom.LocationName;
                var prodDesc = transferItemFrom.ProductDescription;
                var prodUnitSizeDesc = transferItemFrom.ProductUnitSizeDescription;

                // Get product default location info
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    transferItemFrom.LocationCode,
                    transferItemFrom.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item returns data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemReturns()
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
                    returnItem.LocationCode,
                    returnItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:
                        this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", BRAND: " + prodBrandDesc.Trim());
                        break;

                    default:
                        this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " + prodUnitSizeDesc.Trim());
                        break;
                }

                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:
                        this.ExtraInfoItems.Add("SIZE: Unknown, BRAND: Unknown");
                        break;

                    default:
                        this.ExtraInfoItems.Add("DESC: Unknown");
                        break;
                }

                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item receiving data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemReceiving()
        {
            if (this.ListViewMainActivity.SelectedItem is ReceiveItem receiveItem)
            {
                ////var prodLocCode = Convert.ToString(receiveItem.LocationCode);
                ////var prodLocName = receiveItem.LocationName;
                var prodDesc = receiveItem.ProductDescription;
                var prodUnitSizeDesc = receiveItem.ProductUnitSizeDescription;

                // Get product default location info
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    receiveItem.LocationCode,
                    receiveItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item purchase order work sheet data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemPurchaseOrderWorkSheet()
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
                    purchaseOrderWorkSheetItem.LocationCode,
                    purchaseOrderWorkSheetItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item production ticket raw material data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemTicketMaterial()
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
                    productionTicketMaterialItem.LocationCode,
                    productionTicketMaterialItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item production ticket finished goods data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemTicketFinished()
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
                    productionTicketFinishedItem.LocationCode,
                    productionTicketFinishedItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item production ticket additional products data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemTicketAddProduct()
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
                    productionTicketByProductItem.LocationCode,
                    productionTicketByProductItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item picking single data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemPickingSingle()
        {
            if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItemSingle)
            {
                ////var prodLocCode = Convert.ToString(pickingItemSingle.LocationCode);
                ////var prodLocName = pickingItemSingle.LocationName;
                var prodDesc = pickingItemSingle.ProductDescription;
                var prodUnitSizeDesc = pickingItemSingle.ProductUnitSizeDescription;
                var prodBrandDesc = pickingItemSingle.ProductBrandDescription;

                // Get product default location info
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    pickingItemSingle.LocationCode,
                    pickingItemSingle.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);

                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:
                        prodDesc = pickingItemSingle.ProductDescription;
                        prodUnitSizeDesc = pickingItemSingle.ProductUnitSizeDescription;
                        this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", BRAND: " +
                            prodBrandDesc.Trim());
                        break;

                    default:
                        this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " + prodUnitSizeDesc.Trim());
                        break;
                }

                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:
                        this.ExtraInfoItems.Add("SIZE: Unknown, BRAND: Unknown");
                        break;

                    default:
                        this.ExtraInfoItems.Add("DESC: Unknown");
                        break;
                }

                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item picking data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemPicking()
        {
            if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItem)
            {
                ////var prodLocCode = Convert.ToString(pickingItem.LocationCode);
                ////var prodLocName = pickingItem.LocationName;
                var prodCode = pickingItem.ProductCode;
                var prodDesc = pickingItem.ProductDescription;
                var prodUnitSizeDesc = pickingItem.ProductUnitSizeDescription;
                var prodBrandDesc = pickingItem.ProductBrandDescription;
                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:

                        this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " +
                            prodBrandDesc.Trim());
                        break;

                    default:

                        this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " +
                            prodUnitSizeDesc.Trim());
                        break;
                }

                // Get product default location info
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    pickingItem.LocationCode,
                    pickingItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                {
                    case 1:
                        this.ExtraInfoItems.Add("SIZE: Unknown, BRAND: Unknown");
                        break;

                    default:
                        this.ExtraInfoItems.Add("DESC: Unknown");
                        break;
                }

                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item receiving data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemPhysicalCount()
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

                var prodDesc = physicalCountItem.ProductDescription.ToString();
                var prodUnitSizeDesc = physicalCountItem.ProductUnitSizeDescription.ToString();
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item receiving data.
        /// </summary>
        private void DisplayDataItemInventoryInquiry()
        {
            if (this.ActItem.ProdInfo.Refno <= 0)
            {
                this.ExtraInfoItems.Add($"PRODUCT: {"All"}");
            }
            else
            {
                this.ExtraInfoItems.Add($"PRODUCT: {ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo)}");
            }

            this.ExtraInfoItems.Add($"LOCATION: {LocationMapTasks.GetCurrentReadableLocation()}");
        }

        /// <summary>
        /// Display the activity item cycle count data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemCycleCount()
        {
            if (this.ListViewMainActivity.SelectedItem is CountItem cycleCountItem)
            {
                // Get the cycle count line item location.
                var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    Convert.ToInt32(cycleCountItem.LocationMapRefno));
                var countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                // Using the cycle count line item warehouse, get the product default
                // picking location.
                var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    countItemLocMap.WhsLocMapRefno,
                    Convert.ToInt32(cycleCountItem.ProductRefno),
                    SysCommon.ProdLocMapTypes.Picking);
                var defProdLoc = LocationMapTasks.GetReadableLocation(
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                var prodDesc = cycleCountItem.ProductDescription.ToString();
                var prodUnitSizeDesc = cycleCountItem.ProductUnitSizeDescription.ToString();
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                this.ExtraInfoItems.Add("Count Location: " + countLoc.Trim());
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
                this.ExtraInfoItems.Add("Count Location: Unknown");
            }
        }

        /// <summary>
        /// Display the activity item receiving data.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task DisplayDataItemAdjustment()
        {
            if (this.ListViewMainActivity.SelectedItem is AdjustmentItem adjustmentItem)
            {
                var prodDesc = adjustmentItem.ProductDescription;
                var prodUnitSizeDesc = adjustmentItem.ProductUnitSizeDescription;
                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                // Get product default location info
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                    adjustmentItem.LocationCode,
                    adjustmentItem.ProductRefno);
                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    defLocMapRefno);
                var defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                this.ExtraInfoItems.Add("Default Location: " + defLoc);
            }
            else
            {
                this.ExtraInfoItems.Add("No product description.");
                this.ExtraInfoItems.Add("Default Location: Unknown");
            }
        }

        /// <summary>
        /// If activity is picking check if ready to close items.
        /// </summary>
        /// <returns><see cref="Task{T}"/> True if picking quantity checks are all good.</returns>
        /// <exception cref="NotImplementedException"><see cref="NotImplementedException"/> Undefined quantity check type.</exception>
        private async Task<bool> IsReadyToClosePicking()
        {
            var isReadyToClose = true;

            try
            {
                this.IsShowDataValidMsg = true;
                this.ItemQuantityUnderCheck();

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
                                "No") == false))
                        {
                            isReadyToClose = false;
                            this.IsShowDataValidMsg = false;
                        }

                        break;

                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                        isReadyToClose = false;
                        this.IsShowDataValidMsg = false;
                        break;

                    case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                        break;

                    default:
                        throw new NotImplementedException(
                            "Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                }
            }
            catch
            {
                throw;
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Check if ready to save.
        /// </summary>
        /// <returns><see cref="Task{T}"/> True if the activity items is ready to save.</returns>
        /// <exception cref="NotImplementedException"> Picking quantity check type is not implemented.</exception>
        private async Task<bool> IsReadyToSavePicking()
        {
            var isReadyToSave = true;

            try
            {
                this.ItemQuantityUnderCheck();

                // Added extra check for Lekker, check all items.
                this.AllItemsQuantityUnderCheck();
            }
            catch (MessageOnlyException ex)
            {
                switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                {
                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                        if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                "Quantity Validation",
                                ex.Message + Environment.NewLine + "Continue?",
                                "Yes",
                                "No") == false))
                        {
                            isReadyToSave = false;
                            this.IsShowDataValidMsg = false;
                        }

                        break;

                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                        await Device.InvokeOnMainThreadAsync(
                            async () => await this.DisplayAlert(
                                "Quantity Validation",
                                ex.Message,
                                "OK"));
                        isReadyToSave = false;
                        this.IsShowDataValidMsg = false;

                        break;

                    case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                        break;

                    default:

                        throw new NotImplementedException(
                            "Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                }
            }
            catch
            {
                throw;
            }

            return isReadyToSave;
        }

        /// <summary>
        /// Option menu click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
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
                "ActivityItem Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            if (result == this.CmdLocationText && this.CmdLocationIsEnabled)
            {
                await this.CmdLocationMenuClicked();
            }
            else if (result == this.CmdFilterText && this.CmdFilterIsEnabled)
            {
            }
            else if (result == this.CmdSelectSkidText && this.CmdSelectSkidIsEnabled)
            {
                await this.CmdSelectSkidClicked();
            }
            else if (result == this.CmdAddProductText && this.CmdAddProductIsEnabled)
            {
                await this.CmdAddProductClicked();
            }
            else if (result == this.CmdAddSkidText && this.CmdAddSkidIsEnabled)
            {
                await this.CmdAddSkidClicked();
            }
            else if (result == this.CmdClearSelectedSkidText && this.CmdClearSelectedSkidIsEnabled)
            {
                await this.CmdClearSelectedSkidClicked();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        async Task CmdLocationMenuClicked()
        {
            try
            {
                var dspOverrideMsg = false;
                await this.ActDisappearing();
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
                            this.ActItem.GetControlDescription(),
                            "Default locations are enabled, location changes may be overridden.",
                            Resource.OK));
                }

                var form = new FormDialogContainer(
                    EDialogTypes.SelectLocationMap,
                    SysCommon.ActSubTypes.NotApplicable);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                    // Special validation for picking and filtering by section.
                    // Check if the scanned section matched the filtered one.
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                        this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                        {
                            var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                      $" not match the filtered picking section '{sectName}'.";

                            await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        msg,
                                        Resource.OK));
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        async Task CmdSelectSkidClicked()
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                await this.ActDisappearing();
                var form = new FormDialogContainer(
                    EDialogTypes.SkidInfo,
                    this.ActItem,
                    this.ActSubType);
                _ = await form.DisplayControl.ShowDialog();

                // Current location was assigned in the form.
                var xmlDoc = await form.GetXML();
                this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                {
                    if (this.ActItem.SelectedActivityItem != null)
                    {
                        // Needed to refresh the selected item location, called SelectItem.
                        await this.ActItem.AssignActivityItem(this.ActItem.SelectedActivityItem);
                        this.CmdClearSelectedSkidIsEnabled = false;
                    }
                }
                else
                {
                    var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                    if (this.ActItem.ProdInfo.Code == currSkid.ProdInfo.Code &&
                        this.ActItem.ProdInfo.UnitBC == currSkid.ProdInfo.UnitBC &&
                        this.ActItem.ProdInfo.UnitCode == currSkid.ProdInfo.UnitCode)
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                        this.CmdLocationIsEnabled = false;
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            "Skid",
                            "Skid product must match the selected line item product.",
                            "OK"));

                        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        this.CmdSelectSkidText = "Select Skid";
                        this.CmdAddSkidIsEnabled = false;
                    }

                    // Special validation for picking and filtering by section.
                    // Check if the scanned section matched the filtered one.
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                        this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                        {
                            var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                      $" not match the filtered picking section '{sectName}'.";

                            await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        msg,
                                        Resource.OK));
                        }
                    }

                    this.CmdClearSelectedSkidIsEnabled = true;
                    var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno);

                    // TODO: Sorting example
                    this.ActItem.ListItems = this.ActItem.ListItems.OrderBy(item =>
                        item.GetType().GetProperty("ActivityItemRefno")?.GetValue(item)).ToList();

                    if (actItemFoundRefno > 0)
                    {
                        ////int foundRowIdx = this.ActItem.ListItems.Find(actItemFoundRefno);
                        var selectedItem = this.ActItem.ListItems.FirstOrDefault(act =>
                            Convert.ToString(act.GetType().GetProperty("ActivityItemRefno")?.GetValue(act)) ==
                            Convert.ToString(actItemFoundRefno));

                        if (selectedItem != null)
                        {
                            await this.ActItem.AssignActivityItem(selectedItem);
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        async Task CmdClearSelectedSkidClicked()
        {
            try
            {
                await this.ActDisappearing();
                this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                this.CmdSelectSkidIsEnabled = true;
                this.CmdClearSelectedSkidIsEnabled = false;
                this.CmdAddSkidIsEnabled = false;

                // Expected menu to be recreated.
                this.CmdSelectSkidText = "Select Skid";
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        async Task CmdAddProductClicked()
        {
            // Needed to check if the enter key was pressed last, because it seems that
            // SelectNextControl in the key press event causes a click event to fire
            // if a command button is the next control rather than just giving the button focus.
            try
            {
                await this.ActDisappearing();

                XDocument xmlDoc = null;
                ProductInfo prodInfo = null;
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.SelectProductUnit,
                        this.ActSubType);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        xmlDoc = await form.GetXML();
                        var prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                        if (prodUnitRefno == 0)
                        {
                            throw new MessageOnlyException("A product unit must be chosen.");
                        }

                        prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    }
                    else
                    {
                        throw new CancelOperationException();
                    }
                }
                else
                {
                    prodInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo;
                    var unitSizeDesc = SysCommon.AddXMLReservedChars(prodInfo.UnitSizeDesc);

                    xmlDoc = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<params>"
                        + "<ProductUnitRefno>" + prodInfo.UnitRefno + "</ProductUnitRefno>"
                        + "<ProductRefno>" + prodInfo.Refno + "</ProductRefno>"
                        + "<ProductCode>" + prodInfo.Code + "</ProductCode>"
                        + "<ProductDescription>" + SysCommon.AddXMLReservedChars(prodInfo.Desc) + "</ProductDescription>"
                        + "<UnitCode>" + prodInfo.UnitCode + "</UnitCode>"
                        + "<BaseUnitCode>" + prodInfo.BaseUnitCode + "</BaseUnitCode>"
                        + "<ProductUnitBC>" + prodInfo.UnitBC + "</ProductUnitBC>"
                        + "<ProductUnitSizeDescription>" + unitSizeDesc + "</ProductUnitSizeDescription>"
                        + "</params>");
                }

                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMap))
                {
                    throw new MessageOnlyException("Location must be set.");
                }

                var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodInfo.UnitRefno);
                if (actItemFoundRefno > 0)
                {
                    this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                }
                else
                {
                    if (!this.ActItem.IsAllowAddItem)
                    {
                        throw new MessageOnlyException(
                            "Adding new line items for this activity is currently disabled.");
                    }

                    // After insert this.ActItem.ItemFKRefno may be a negative value if a new line item was created.
                    await this.ActItem.Insert(xmlDoc, false);
                    this.ActItem.SetSelectedActivityItem(this.ActItem.ItemRefno);
                    this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                    // This is a little dangerous we assume that the new row idx will be the last one
                    // selectedItem = this.ActItem.ListItems.Last();
                    // Try not to refresh all the items.
                    ////this.ActItem.RegLayerListenerProgOut(this);
                    ////await this.RefreshBySubType();
                    ////this.ActItem.UnregLayerListenerProgOut(this);
                    await this.DesignGrid();

                    await this.ActAppearing();

                }
            }
            catch (CancelOperationException)
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

                await this.RefreshBySubType();
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });

                await this.RefreshBySubType();
            }
            finally
            {
                if (this.ActItem.ListItems.Count > 0)
                {
                    this.ActItem.SelectedActivityItem = this.ActItem.ListItems.LastOrDefault();
                }

                await this.ActAppearing();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task CmdAddSkidClicked()
        {
            // Needed to check if the enter key was pressed last, because it seems that
            // SelectNextControl in the key press event causes a click event to fire
            // if a command button is the next control rather than just giving the button focus.
            try
            {
                await this.ActDisappearing();
                if (await this.DisplayAlert(
                    "Add Skid",
                    "Add the ENTIRE skid to this activity?",
                    "Yes",
                    "No"))
                {
                    var prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                    var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                    object selectedItem = null;
                    if (actItemFoundRefno > 0)
                    {
                        ////foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                        selectedItem = this.ActItem.ListItems.FirstOrDefault(actItem =>
                            Convert.ToString(actItem.GetType().GetProperty(
                                "ActivityItemRefno")?.GetValue(actItem)) ==
                            Convert.ToString(actItemFoundRefno));
                    }
                    else
                    {
                        if (this.ActItem.IsAllowAddItem)
                        {
                            var xml = ProductInfoTasks.GetXML(
                                this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);
                            await this.ActItem.Insert(xml, false);

                            // This is a little dangerous we assume that the new row idx
                            // will be the last one
                            ////foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                            selectedItem = this.ActItem.ListItems.LastOrDefault();
                            actItemFoundRefno = this.ActItem.ItemRefno;
                        }
                        else
                        {
                            throw new MessageOnlyException(
                                this.ActItem.GetItemNotFoundMessage(
                                    this.LocMap,
                                    this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                        }
                    }

                    this.ActItem.SelectedActivityItem = selectedItem;

                    // INSERT A NEW ACTIVITY DETAIL RECORD
                    var actDetail = ActDetailFacade.GetActDetail(
                        this.ActItem,
                        this.ActSubType);
                    actDetail.ActItem = this.ActItem;

                    // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                    // because of the length of the code and the fact is isn't human readable friendly.
                    var schemeData = actDetail.SchemeData;
                    schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    actDetail.SchemeData = schemeData;
                    actDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                        this.ActItem.ProdInfo.Code,
                        this.ActItem.ProdInfo.Scheme,
                        actDetail.SchemeData);
                    actDetail.SchemeId = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                        this.ActItem.ProdInfo.Code,
                        this.ActItem.ProdInfo.Scheme,
                        actDetail.SchemeData);

                    // IsReadyToApply will perform any validations, such as duplicate
                    // checks and quantity validations.
                    var xmlParam = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                        + "<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                        + "<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                        + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                        + "</imclient>");

                    await this.ActItem.IsReadyToApply(xmlParam);
                    await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                    // Upon a save of completed data update the data view to to show new
                    // item and quantity change
                    this.ActItem.AddToQuantityInActivityItem(
                        this.ActItem.SelectedActivityItem,
                        actDetail.Quantity,
                        actDetail.BaseQuantity,
                        actDetail.WeightQuantity);

                    this.ListViewMainActivity.SelectedItem = selectedItem;
                    this.MainActivityItems[this.ActItem.SelectedListItemIdx] = selectedItem;
                }
            }
            catch (IsReadyToApplyException)
            {
                // Do nothing.
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
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
        /// Parse and seed the bar code modifier picker.
        /// </summary>
        private void ParseAndSeedBarCodeModifier()
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

        /// <summary>
        /// Back button call back.
        /// </summary>
        private async void BackButtonCallback()
        {
            if (await this.CancelChanges())
            {
                await this.ActDisappearing();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }
    }
}
