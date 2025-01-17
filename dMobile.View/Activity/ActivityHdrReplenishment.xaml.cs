// <copyright file="ActivityHdrReplenishment.xaml.cs" company="InterSect Business Systems Inc">
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
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller;
    using Controller.Activity;
    using CustomPages;
    using Resources.Types.Headers;
    using Forms;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityHdrReplenishment" /> Activity replenishment header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrReplenishment : DMobileContentPage, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        ///     Show all move types string.
        /// </summary>
        private string cmdShowAllMoveTypes = "All Move Types";

        /// <summary>
        ///     Show all move types is enabled.
        /// </summary>
        private bool cmdShowAllMoveTypesIsEnabled;

        /// <summary>
        ///     Show only manual move type.
        /// </summary>
        private string cmdShowOnlyManualMoveType = "Manual Only";

        /// <summary>
        ///     Show only manual move type is enabled.
        /// </summary>
        private bool cmdShowOnlyManualMoveTypeIsEnabled = true;

        /// <summary>
        ///     Show only pull down move type.
        /// </summary>
        private string cmdShowOnlyPullDownMoveType = "Pull Down Only";

        /// <summary>
        ///     Show only pull down pull down move type is enabled.
        /// </summary>
        private bool cmdShowOnlyPullDownMoveTypeIsEnabled = true;

        /// <summary>
        ///     Show only restock move type.
        /// </summary>
        private string cmdShowOnlyRestockMoveType = "Restock Only";

        /// <summary>
        ///     Show only restock move type is enabled.
        /// </summary>
        private bool cmdShowOnlyRestockMoveTypeIsEnabled = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrReplenishment" /> class.
        /// </summary>
        /// <param name="activitySubType"><see cref="SysCommon.ActSubTypes" /> Activity sub type.</param>
        /// <remarks>
        ///     Page refresh does not invoke this constructor.
        /// </remarks>
        public ActivityHdrReplenishment(SysCommon.ActSubTypes activitySubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = activitySubType;
            this.ActHdr = ActivityFacade.GetActivity(this.ActSubType);
            this.Title = this.ActHdr.GetControlDescription();

            this.CmdShowAllMoveTypesIsEnabled = true;
            this.CmdShowOnlyManualMoveTypeIsEnabled = true;
            this.CmdShowOnlyPullDownMoveTypeIsEnabled = true;
            this.CmdShowOnlyRestockMoveTypeIsEnabled = true;
        }

        /// <summary>
        ///     Gets or sets the current location map.
        /// </summary>
        public LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        ///     Gets or sets the source to location map.
        /// </summary>
        public SLocMap SourceToLocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the source to location map reference number.
        /// </summary>
        public int SourceToLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the source from location map.
        /// </summary>
        public SLocMap SourceFromLocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the source from location map reference number.
        /// </summary>
        public int SourceFromLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the form activity item foreign key reference number.
        /// </summary>
        public int SourceFromActivityItemFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the to activity item foreign key reference number.
        /// </summary>
        public int SourceToActivityItemFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the selected from location map.
        /// </summary>
        public SLocMap SelFromLocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the selected from current location reference number.
        /// </summary>
        public int SelFromLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets the selected to location map.
        /// </summary>
        public SLocMap SelToLocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the selected to current location reference number.
        /// </summary>
        public int SelToLocMapRefno { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show all move type is enabled.
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
        ///     Gets or sets a value indicating whether to show all moves types.
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
        ///     Gets or sets a value indicating whether to show only pull down move types is enabled.
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
        ///     Gets or sets a value indicating whether to show only pull down types.
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
        ///     Gets or sets a value indicating whether to show only restock move types is enabled.
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
        ///     Gets or sets a value indicating whether to show only restock move types.
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
        ///     Gets or sets a value indicating whether to show only restock move types is enabled.
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
        ///     Gets or sets a value indicating whether to show only manual move types.
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
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets or sets the global system environment object.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets a value indicating whether bar code handler is registered / enabled.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the activity header instance.
        /// </summary>
        private Activity ActHdr { get; }

        /// <summary>
        ///     Gets or sets the activity header sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.InventoryReplenishment;

        /// <summary>
        ///     Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainMoveFromItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        ///     Gets or sets the list of items bound to data grid / list main activity.
        /// </summary>
        private ObservableCollection<object> MainMoveToItems { get; } = new ObservableCollection<object>();

        /// <summary>
        ///     Gets or sets the OptionsMenuItems.
        /// </summary>
        private List<string> OptionsMenuItems { get; } = new List<string>();

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
        ///     Gets or sets a value indicating whether the menu options is in focus.
        /// </summary>
        private bool IsMenuOptionsFocused { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the list view move from is in focus.
        /// </summary>
        private bool IsListViewMoveFromFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the list view move to is in focus.
        /// </summary>
        private bool IsListViewMoveToFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the perform move button is in focus.
        /// </summary>
        private bool IsCmdPerformMoveFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the set to location button is in focus.
        /// </summary>
        private bool IsCmdSetToLocationFocused { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the set from location button is in focus.
        /// </summary>
        private bool IsCmdSetFromLocationFocused { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.ClearBarCodeScanEventHandlers();
                    await this.Scanner.DisableScanning();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.EnableScanning();
                    if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                    {
                        this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                    }

                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        ///     Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if the data is ready to be saved else false.</returns>
        public Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;
            return Task.FromResult(isReadyToSave);
        }

        /// <summary>
        ///     Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}" /> True if the form is ready to close else false.</returns>
        public Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;
            return Task.FromResult(isReadyToClose);
        }

        /// <summary>
        ///     Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool" /> True if a save was performed successfully else false.</returns>
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
                var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
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
        ///     Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool" />. True if the changes where canceled, else false.</returns>
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
                var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
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
            // NOTE: Do not set the scanner off or on here. Scan events can occur even though
            // this method could be invoked asynchronously in the future.
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
        }

        /// <summary>
        ///     Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool" /> True.</returns>
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
        ///     Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string" /> Option property name..</param>
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

            return;

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
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    this.NumTaps = 0;
                    this.SubscribeKeyUpHandlers();
                    this.ListViewMoveFrom.ItemTapped += this.ListViewMoveFrom_ItemTapped;
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
                    var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
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
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <summary>
        ///     Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override async void OnDisappearing()
        {
            this.ListViewMoveFrom.ItemTapped -= this.ListViewMoveFrom_ItemTapped;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
                switch (keycode)
                {
                    case Keycode.F1:
                        this.IsIndicatorRunning = true;
                        await KeyUpF1Handler();
                        break;

                    case Keycode.DpadDown:
                        if (this.IsListViewMoveFromFocused)
                        {
                            MoveFromAppearingDpadDownKey();
                        }

                        break;

                    case Keycode.DpadUp:
                        if (this.IsListViewMoveFromFocused)
                        {
                            MoveFromAppearingDpadUpKey();
                        }

                        break;
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
                this.IsIndicatorRunning = false;
            }

            return;

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
                    this.ListViewMoveFrom.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
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
                    this.ListViewMoveFrom.SelectedItem,
                    ScrollToPosition.MakeVisible,
                    true);
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
            var message = string.Empty;

            try
            {
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

                this.IsIndicatorRunning = true;
                this.BarParse = scanArgs.ParseInfo;
                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity reference # is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity item reference # is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product bar code is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    ////throw new MessageOnlyException("Scanning a location identifier bar code is not supported here!");
                    var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        this.BarParse.LocationIdentifier);
                    if (locMapRefno > 0)
                    {
                        this.SelToLocMapRefno = locMapRefno;
                        this.SelToLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetToLocation.Text = "To: " +
                            LocationMapTasks.GetCurrentReadableLocation());
                        if (this.SourceToLocMapRefno != this.SelToLocMapRefno)
                        {
                            throw new MessageOnlyException(
                                "The move to location set does not match the scanned to location.");
                        }
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.cmdSetToLocation.Text = "Confirm To Location");
                    }
                }
                else
                {
                    throw new MessageOnlyException("Scanned bar code type is not supported here!");
                }
            }
            catch (TaskCanceledException)
            {
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
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Handle double click on list view item.
        ///     Using a tap gesture recognizer in the designGrid would be optimal, however there is
        ///     currently a known bug that prevents selectedItemChanged from firing.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e">The e<see cref="ItemTappedEventArgs" /> Item tapped event arguments.</param>
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
                // NOTE: So far all sub types will be refreshed.
                await this.ActHdr.Refresh(this.ActHdr.XmlDataCriteria);
                this.ActHdr.ListHeaders = await this.ActHdr.GetActivityList(false);
                await this.DesignGrid();
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
                this.MainMoveFromItems = this.ActHdr.DesignGrid(ref this.GridMoveFrom, ref this.ListViewMoveFrom);
                this.ListViewMoveFrom.ItemsSource = this.MainMoveFromItems;
            });
        }

        /// <summary>
        ///     Find the selected item in the move from data list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task FindSelectedItemAsyncMoveFrom()
        {
            if (this.ActHdr.ListHeaders.Count > 0 && this.MainMoveFromItems.Count > 0)
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
                Device.BeginInvokeOnMainThread(() => { this.ListViewMoveFrom.SelectedItem = null; });
            }
        }

        /// <summary>
        ///     Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void ListViewMoveFrom_Focused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveFromFocused = true;
        }

        /// <summary>
        ///     Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void ListViewMoveFrom_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveFromFocused = false;
        }

        /// <summary>
        ///     Headers list view focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void ListViewMoveTo_Focused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveToFocused = true;
        }

        /// <summary>
        ///     Headers list view unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void ListViewMoveTo_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsListViewMoveToFocused = false;
        }

        /// <summary>
        ///     Option menu click handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            ////if (this.Loading)
            ////{
            ////    return;
            ////}

            try
            {
                this.IsIndicatorRunning = true;
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
                this.IsIndicatorRunning = false;
            }
        }

        private async void CmdSetToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    return;
                }

                this.SelToLocMap = this.LocMap.ChosenLocMap.Clone();
                this.SelToLocMapRefno = this.LocMap.ChosenLocMapRefno;

                // Display the selected to location.
                if (this.LocMap.ChosenLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetToLocation.Text = "To: " +
                        LocationMapTasks.GetChosenReadableLocation());

                    if (this.SourceToLocMapRefno != this.LocMap.ChosenLocMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The move to location set does not match the selected to location.");
                    }

                    // Only set the current location to the Move To selected location if it is valid.
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetToLocation.Text = "Confirm To Location");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Message",
                    msgEx.Message,
                    "OK"));
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

        /// <summary>
        ///     Set to location focus event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Focus event arguments.</param>
        private void CmdSetToLocation_Focused(object sender, FocusEventArgs e)
        {
            this.IsCmdSetToLocationFocused = true;
        }

        /// <summary>
        ///     Set to location unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Unfocused event arguments.</param>
        private void CmdSetToLocation_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsCmdSetToLocationFocused = false;
        }

        /// <summary>
        ///     Perform move click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdPerformMove_Click(object sender, EventArgs e)
        {
            var responseData = string.Empty;
            var xmlString = new StringBuilder();

            try
            {
                if (this.ActHdr.HdrFkRefno <= 0)
                {
                    throw new MessageOnlyException("Please select a replenishment move.");
                }

                if (this.SourceFromLocMapRefno != this.SelFromLocMapRefno)
                {
                    throw new MessageOnlyException(
                        "The move from location set does not match the selected from location.");
                }

                if (this.SourceToLocMapRefno != this.SelToLocMapRefno)
                {
                    throw new MessageOnlyException("The move to location set does not match the selected to location.");
                }

                await CreateSubmitXml(xmlString);
                responseData = SysCommon.SubmitComm(xmlString.ToString());

                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Perform Move",
                    "Replenishment move complete",
                    "OK"));

                this.OnAppearing();
            }
            catch (CommunicationException commEx)
            {
                await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                    this.ActHdr.GetControlDescription(),
                    commEx.Message,
                    "OK"));
            }
            catch (MessageOnlyException msgEx)
            {
                await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                    this.ActHdr.GetControlDescription(),
                    msgEx.Message,
                    "OK"));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                    this.ActHdr.GetControlDescription(),
                    ex.Message,
                    "OK"));
            }

            return;

            ////////////////////////////////////////////////////////////////////////////////
            async Task CreateSubmitXml(StringBuilder xmlString)
            {
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false
                };
                var xml = XmlWriter.Create(xmlString, settings);
                XDocument request = null;
                var conn = DatabaseConnection.GetInstance().SqlConn;

                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT REPLENISHMENT</action>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<from_refno>" + this.SourceFromActivityItemFKRefno + "</from_refno>"
                    + "<to_refno>" + this.SourceToActivityItemFKRefno + "</to_refno>"
                    + "</command>"
                    + "</imclient>");

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();
            }
        }

        private void CmdPerformMove_Focused(object sender, FocusEventArgs e)
        {
            this.IsCmdPerformMoveFocused = true;
        }

        private void CmdPerformMove_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsCmdPerformMoveFocused = false;
        }

        /// <summary>
        ///     List view move from item selected event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void ListViewMoveFrom_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActHdr.SelectedListItemIdx = e.SelectedItemIndex;
            await this.ActHdr.SelectActivity(e.SelectedItem);
            this.SelectedActivityFkRefno = this.ActHdr.HdrFkRefno;

            this.SourceFromLocMap = new SLocMap();
            this.SourceFromLocMapRefno = 0;
            this.SourceFromLocMapRefno = this.ActHdr.LocationMapRefno;
            this.SourceFromLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.SourceFromLocMapRefno);
            this.SourceFromActivityItemFKRefno = ((Replenishment)this.ActHdr.SelectedActivity).ActivityItemFKRefno;
            this.MainMoveToItems.Clear();
            foreach (var item in this.ActHdr.ListItems)
            {
                if (item is Replenishment replen)
                {
                    if (replen.ActivityItemFKParRefno == this.ActHdr.HdrFkRefno)
                    {
                        this.SourceToLocMapRefno = replen.LocationMapRefno;
                        this.SourceToLocMap =
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(this.SourceToLocMapRefno);
                        this.SourceToActivityItemFKRefno = replen.ActivityItemFKRefno;
                        this.MainMoveToItems.Add($"To: {replen.ReadableLocationMap}");
                        this.MainMoveToItems.Add($"Scheme ID: {replen.ActivityDetailSchemeID}");
                        this.MainMoveToItems.Add($"Quantity {replen.ActivityItemReplenishmentQuantity * -1}" +
                                                 $" {(replen.ProductUnitBC + replen.ProductUnitCode).TrimStart()}");
                        break;
                    }
                }
            }

            this.ListViewMoveTo.ItemsSource = this.MainMoveToItems;
        }

        /// <summary>
        ///     Set from location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdSetFromLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    return;
                }

                this.SelFromLocMap = this.LocMap.ChosenLocMap.Clone();
                this.SelFromLocMapRefno = this.LocMap.ChosenLocMapRefno;

                // Display the selected from location.
                if (this.LocMap.ChosenLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSetFromLocation.Text = "From: " +
                        LocationMapTasks.GetChosenReadableLocation());

                    if (this.SourceFromLocMapRefno != this.LocMap.ChosenLocMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The move from location set does not match the selected from location.");
                    }
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSetToLocation.Text = "Confirm From Location");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Message",
                    msgEx.Message,
                    "OK"));
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

        /// <summary>
        ///     Set from location focused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void CmdSetFromLocation_Focused(object sender, FocusEventArgs e)
        {
            this.IsCmdSetFromLocationFocused = true;
        }

        /// <summary>
        ///     Set from location unfocused event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="FocusEventArgs" /> Focus event arguments.</param>
        private void CmdSetFromLocation_Unfocused(object sender, FocusEventArgs e)
        {
            this.IsCmdSetFromLocationFocused = false;
        }
    }
}