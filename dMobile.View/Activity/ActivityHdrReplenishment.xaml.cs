// <copyright file="ActivityHdrReplenishment.xaml.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="ActivityHdrReplenishment" /> Activity replenishment header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrReplenishment : DMobileContentPage, IAppArea, IDataLayerEventListener
    {
        private bool cmdShowAllMoveTypesIsEnabled = false;

        private string cmdShowAllMoveTypes = string.Empty;

        private bool cmdShowOnlyPullDownMoveTypeIsEnabled = false;

        private string cmdShowOnlyPullDownMoveType = string.Empty;

        private bool cmdShowOnlyRestockMoveTypeIsEnabled = false;

        private string cmdShowOnlyRestockMoveType = string.Empty;

        private bool cmdShowOnlyManualMoveTypeIsEnabled = false;

        private string cmdShowOnlyManualMoveType = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdrReplenishment"/> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <remarks>
        /// Page refresh does not invoke this constructor.
        /// </remarks>
        public ActivityHdrReplenishment(SysCommon.ActSubTypes activitySubType)
        {
            this.InitializeComponent();

            this.cmdPerformMove.Text = "Perform Move";
            this.cmdPerformMove.IsEnabled = false;
            this.cmdSetToLocation.Text = "Confirm To Location";
            this.cmdSetToLocation.IsEnabled = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = activitySubType;
            this.ActHdr = ActivityFacade.GetActivity(this.ActSubType);
            this.Title = this.ActHdr.GetControlDescription();

            this.cmdSetToLocation.IsEnabled = true;
            this.cmdPerformMove.IsEnabled = true;
            this.CmdShowAllMoveTypesIsEnabled = true;
            this.CmdShowAllMoveTypes = "All Move Types";
            this.CmdShowOnlyRestockMoveTypeIsEnabled = true;
            this.CmdShowOnlyRestockMoveType = "Restock Only";
            this.CmdShowOnlyPullDownMoveTypeIsEnabled = true;
            this.CmdShowOnlyPullDownMoveType = "Pull Down Only";
            this.CmdShowOnlyManualMoveTypeIsEnabled = true;
            this.CmdShowOnlyManualMoveType = "Manual Only";

            this.MainMoveToItems.Clear();
            this.MainMoveToItems.Add("To Location:");
            this.MainMoveToItems.Add("Tracking:");
            this.ListViewMoveTo.ItemsSource = this.MainMoveToItems;

            // If the activity header object does not have a skid already initialize the instance.
            if (this.ActHdr.SkidInfo == null)
            {
                this.ActHdr.SkidInfo = new SysSkidInfo();
            }
        }

        /// <summary>
        /// Gets or sets the current location map.
        /// </summary>
        public LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        /// Gets or sets the selected to location map.
        /// </summary>
        public SLocMap SelToLocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the selected to current location reference number.
        /// </summary>
        public int SelToCurrLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether to show all move type is enabled.
        /// </summary>
        public bool CmdShowAllMoveTypesIsEnabled
        {
            get => this.cmdShowAllMoveTypesIsEnabled;
            set
            {
                this.cmdShowAllMoveTypesIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show all moves types.
        /// </summary>
        public string CmdShowAllMoveTypes
        {
            get => this.cmdShowAllMoveTypes;
            set
            {
                this.cmdShowAllMoveTypes = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only pull down move types is enabled.
        /// </summary>
        public bool CmdShowOnlyPullDownMoveTypeIsEnabled
        {
            get => this.cmdShowOnlyPullDownMoveTypeIsEnabled;
            set
            {
                this.cmdShowOnlyPullDownMoveTypeIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only pull down types.
        /// </summary>
        public string CmdShowOnlyPullDownMoveType
        {
            get => this.cmdShowOnlyPullDownMoveType;
            set
            {
                this.cmdShowOnlyPullDownMoveType = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only restock move types is enabled.
        /// </summary>
        public bool CmdShowOnlyRestockMoveTypeIsEnabled
        {
            get => this.cmdShowOnlyRestockMoveTypeIsEnabled;
            set
            {
                this.cmdShowOnlyRestockMoveTypeIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only restock move types.
        /// </summary>
        public string CmdShowOnlyRestockMoveType
        {
            get => this.cmdShowOnlyRestockMoveType;
            set
            {
                this.cmdShowOnlyRestockMoveType = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only restock move types is enabled.
        /// </summary>
        public bool CmdShowOnlyManualMoveTypeIsEnabled
        {
            get => this.cmdShowOnlyManualMoveTypeIsEnabled;
            set
            {
                this.cmdShowOnlyManualMoveTypeIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only manual move types.
        /// </summary>
        public string CmdShowOnlyManualMoveType
        {
            get => this.cmdShowOnlyManualMoveType;
            set
            {
                this.cmdShowOnlyManualMoveType = value;
                this.OnPropertyChanged();
            }
        }

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
        /// Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainMoveFromItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainMoveToItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the OptionsMenuItems.
        /// </summary>
        private List<string> OptionsMenuItems { get; set; } = new List<string>();

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
        /// Gets or sets a value indicating whether the menu options is in focus.
        /// </summary>
        private bool IsMenuOptionsFocused { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the list view move from is in focus.
        /// </summary>
        private bool IsListViewMoveFromFocused { get; set;} = false;

        /// <summary>
        /// Gets or sets a value indicating whether the list view move to is in focus.
        /// </summary>
        private bool IsListViewMoveToFocused { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the perform move button is in focus.
        /// </summary>
        private bool IsCmdPerformMoveFocused { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the set to location button is in focus.
        /// </summary>
        private bool IsCmdSetToLocationFocused { get; set; } = false;

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
            return isReadyToSave;
        }

        /// <summary>
        /// Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}"/> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
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
            if (propertyName == nameof(this.CmdShowAllMoveTypesIsEnabled) ||
                propertyName == nameof(this.CmdShowAllMoveTypes) ||
                propertyName == nameof(this.CmdShowOnlyPullDownMoveTypeIsEnabled) ||
                propertyName == nameof(this.CmdShowOnlyPullDownMoveType) ||
                propertyName == nameof(this.CmdShowOnlyRestockMoveTypeIsEnabled) ||
                propertyName == nameof(this.CmdShowOnlyRestockMoveType) ||
                propertyName == nameof(this.CmdShowOnlyManualMoveTypeIsEnabled) ||
                propertyName == nameof(this.CmdShowOnlyManualMoveType))
            {
                PopulateOptionsMenu();
            }

            //////////////////////////////////////////////////////////////////////////
            void PopulateOptionsMenu()
            {
                this.OptionsMenuItems.Clear();
                if (this.CmdShowAllMoveTypesIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdShowAllMoveTypes);
                }

                if (this.CmdShowOnlyPullDownMoveTypeIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdShowOnlyPullDownMoveType);
                }

                if (this.CmdShowOnlyRestockMoveTypeIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdShowOnlyRestockMoveType);
                }

                if (this.CmdShowOnlyManualMoveTypeIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdShowOnlyManualMoveType);
                }
            }
        }

        /// <summary>
        /// Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                this.Loading = true;
                this.NumTaps = 0;
                this.SubscribeKeyUpHandlers();
                this.ListViewMoveFrom.ItemTapped += this.ListViewMoveFrom_ItemTapped;
                this.ListViewMoveTo.ItemTapped += this.ListViewMoveTo_ItemTapped;
                await this.ScannerEnable();

                this.ActHdr.XmlDataCriteria = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<whs_id>0</whs_id>" +
                            "<sect_refno>0</sect_refno>" +
                            "</params>");

                await this.RefreshDataBySubType();
                this.ListViewMoveFrom.ItemsSource = this.MainMoveFromItems;

                // If not headers where loaded clear the move to items list view.
                if (this.MainMoveFromItems == null || this.MainMoveFromItems.Count == 0)
                {
                    Device.BeginInvokeOnMainThread(() => this.MainMoveFromItems.Clear());
                }
                else
                {
                    await this.FindSelectedItemAsyncMoveFrom();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
                    ? msgEx.Message
                    : msgEx.Message["MSGBOX-".Length..];
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActHdr.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (CommunicationException commEx)
            {
                // Question should a communication error, be reported as an error or just a message box?
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load replenishment data.{Environment.NewLine}" +
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
                    var msg = $"Unable to load replenishment data.{Environment.NewLine}" +
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
            }
        }

        /// <summary>
        /// Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override async void OnDisappearing()
        {
            this.ListViewMoveFrom.ItemTapped -= this.ListViewMoveFrom_ItemTapped;
            this.ListViewMoveTo.ItemTapped -= this.ListViewMoveTo_ItemTapped;
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
                    case Keycode.F1:
                        this.Loading = true;
                        await KeyUpF1Handler();
                        break;

                    case Keycode.DpadDown:
                        if (this.IsListViewMoveFromFocused)
                        {
                            MoveFromAppearingDpadDownKey();
                        }

                        if (this.IsListViewMoveToFocused)
                        {
                            MoveToAppearingDpadDownKey();
                        }

                        break;

                    case Keycode.DpadUp:
                        if (this.IsListViewMoveFromFocused)
                        {
                            MoveFromAppearingDpadUpKey();
                        }

                        if (this.IsListViewMoveToFocused)
                        {
                            MoveToAppearingDpadUpKey();
                        }

                        break;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
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
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            void MoveFromAppearingDpadDownKey()
            {
                var selItem = this.ListViewMoveFrom.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainMoveFromItems.IndexOf(selItem);
                if (selItemIdx >= this.MainMoveFromItems.Count - 1)
                {
                    return;
                }

                this.ListViewMoveFrom.SelectedItem = this.MainMoveFromItems[selItemIdx + 1];
                this.ListViewMoveFrom.ScrollTo(
                    item: this.ListViewMoveFrom.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            void MoveToAppearingDpadDownKey()
            {
                var selItem = this.ListViewMoveTo.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainMoveToItems.IndexOf(selItem);
                if (selItemIdx >= this.MainMoveToItems.Count - 1)
                {
                    return;
                }

                this.ListViewMoveTo.SelectedItem = this.MainMoveToItems[selItemIdx + 1];
                this.ListViewMoveTo.ScrollTo(
                    item: this.ListViewMoveTo.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            void MoveFromAppearingDpadUpKey()
            {
                var selItem = this.ListViewMoveFrom.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainMoveFromItems.IndexOf(selItem);
                if (selItemIdx <= 0)
                {
                    return;
                }

                this.ListViewMoveFrom.SelectedItem = this.MainMoveFromItems[selItemIdx - 1];
                this.ListViewMoveFrom.ScrollTo(
                    item: this.ListViewMoveFrom.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            void MoveToAppearingDpadUpKey()
            {
                var selItem = this.ListViewMoveTo.SelectedItem;
                if (selItem == null)
                {
                    return;
                }

                var selItemIdx = this.MainMoveToItems.IndexOf(selItem);
                if (selItemIdx <= 0)
                {
                    return;
                }

                this.ListViewMoveTo.SelectedItem = this.MainMoveToItems[selItemIdx - 1];
                this.ListViewMoveTo.ScrollTo(
                    item: this.ListViewMoveTo.SelectedItem,
                    position: ScrollToPosition.MakeVisible,
                    animated: true);
            }

            //////////////////////////////////////////////////////////////////////////
            async Task KeyUpF1Handler()
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
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
                    throw new MessageOnlyException("Scanning an activity reference # is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                {
                    throw new MessageOnlyException("Scanning an activity item reference # is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new MessageOnlyException("Scanning a location identifier bar code is not supported here!");
                }
                else
                {
                    throw new MessageOnlyException("Scanned bar code type is not supported here!");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
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
                this.Loading = false;
            }
        }

        /// <summary>
        /// Handle double click on list view item.
        /// Using a tap gesture recognizer in the designGrid would be optimal, however there is
        /// currently a known bug that prevents selectedItemChanged from firing.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e">The e<see cref="ItemTappedEventArgs"/> Item tapped event arguments.</param>
        private async void ListViewMoveFrom_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            this.NumTaps++;

            if (this.PreviousSender == e.Item)
            {
                if (this.NumTaps == 2)
                {
                    this.NumTaps = 0;
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
        /// Handle double click on list view item.
        /// Using a tap gesture recognizer in the designGrid would be optimal, however there is
        /// currently a known bug that prevents selectedItemChanged from firing.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e">The e<see cref="ItemTappedEventArgs"/> Item tapped event arguments.</param>
        private async void ListViewMoveTo_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            this.NumTaps++;

            if (this.PreviousSender == e.Item)
            {
                if (this.NumTaps == 2)
                {
                    this.NumTaps = 0;
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
                this.MainMoveFromItems = this.ActHdr.DesignGrid(ref this.GridMoveFrom, ref this.ListViewMoveFrom);
                this.ListViewMoveFrom.ItemsSource = this.MainMoveFromItems;
            });
        }

        /// <summary>
        /// Find the selected item in the move from data list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task FindSelectedItemAsyncMoveFrom()
        {
            if (this.ActHdr.ListHdrs.Count > 0 && this.MainMoveFromItems.Count > 0)
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
                    this.ListViewMoveFrom.SelectedItem = this.MainMoveFromItems[this.ActHdr.SelectedListItemIdx];
                    await this.ActHdr.SelectActivity(this.ListViewMoveFrom.SelectedItem);
                }
                else
                {
                    await this.ActHdr.SelectActivity(selectedItem);
                    this.ListViewMoveFrom.SelectedItem = this.ActHdr.SelectedActivity;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMoveFrom.ScrollTo(
                        this.ListViewMoveFrom.SelectedItem,
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
                    this.ListViewMoveFrom.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListViewMoveFrom_Focused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveFromFocused = true;
        }

        /// <summary>
        /// Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListViewMoveFrom_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveFromFocused = false;
        }

        /// <summary>
        /// Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListViewMoveTo_Focused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveToFocused = true;
        }

        /// <summary>
        /// Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="FocusEventArgs"/> Focus event arguments.</param>
        private void ListViewMoveTo_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveToFocused = false;
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
        }

        /// <summary>
        /// Option menu click handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            ////if (this.Loading)
            ////{
            ////    return;
            ////}

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                var result = await this.DisplayActionSheet(
                    "Move Types",
                    "Cancel",
                    null,
                    this.OptionsMenuItems.ToArray());

                if (result != null)
                {
                    if (result == "All Move Types")
                    {
                        this.lblMoveType.Text = result;
                    }
                    else if (result == "Restock Only")
                    {
                        this.lblMoveType.Text = result;

                    }
                    else if (result == "Pull Down Only")
                    {
                        this.lblMoveType.Text = result;

                    }
                    else if (result == "Manual Only")
                    {
                        this.lblMoveType.Text = result;
                    }
                    else
                    {
                        this.lblMoveType.Text = string.Empty;
                    }
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

        private void ListViewMoveFrom_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void ListViewMoveTo_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private async void CmdSetToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    return;
                }

                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                this.SelToLocMap = this.LocMap.CurrLocMap.Clone();
                this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                // Display the selected from location.
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetToLocation.Text = "To: " +
                        LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetToLocation.Text = "Confirm To Location");
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "Error selecting to location." + Environment.NewLine + ex.Message,
                    "OK"));
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        private void CmdSetToLocation_Focused(object sender, FocusEventArgs e)
        {

        }

        private void CmdSetToLocation_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private async void CmdPerformMove_Click(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                "Perform Move",
                "Replenishment move complete",
                "OK"));
        }

        private void CmdPerformMove_Focused(object sender, FocusEventArgs e)
        {

        }

        private void CmdPerformMove_Unfocused(object sender, FocusEventArgs e)
        {

        }
    }
}
