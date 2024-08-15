// <copyright file="SetLocationMap.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using CustomPages.Modal;
    using Forms;
    using Helpers;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Set location map view model.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetLocationMap : DMobileTabbedModal, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SetLocationMap" /> class.
        /// </summary>
        public SetLocationMap()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.TabWarehouse.Text = "Warehouse";
            this.TabSection.Text = "Section";
            this.LblSearch.Text = "Search";
            this.TabSlot.Text = "Slot";
            this.LblSlotCoordinate1.Text = "Coord 1";
            this.LblSlotCoordinate2.Text = "Coord 2";
            this.LblSlotCoordinate3.Text = "Coord 3";
            this.WarehouseTab.Title = "Warehouse";
            this.SectionTab.Title = "Section";
            this.SlotTab.Title = "Slot";
            this.CboSectionSearch.Items.Add("Code");
            this.CboSectionSearch.Items.Add("Description");
            this.CboSectionSearch.SelectedItem = "Code";

            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Warehouses = new ObservableCollection<Warehouse>();
            this.Sections = new ObservableCollection<Section>();
            this.Slot1Coordinates = new ObservableCollection<Slot>();
            this.Slot2Coordinates = new ObservableCollection<Slot>();
            this.Slot3Coordinates = new ObservableCollection<Slot>();
            this.LocMap = LocationMap.GetInstance();
            this.LocMapInfo = this.LocMap.CurrLocMap.Clone();
            this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
            this.LocMap.ChosenLocMapRefno = this.LocMap.CurrLocMapRefno;
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.ApplyEvents();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetLocationMap" /> class.
        /// </summary>
        /// <param name="defaultWhsLocMapRefno"><see cref="int" /> Default warehouse location map reference number.</param>
        /// <param name="lockWhsSelection"><see cref="bool" /> Warehouse selection lock enabler.</param>
        public SetLocationMap(int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.TabWarehouse.Text = "Warehouse";
            this.TabSection.Text = "Section";
            this.LblSearch.Text = "Search";
            this.TabSlot.Text = "Slot";
            this.LblSlotCoordinate1.Text = "Coord 1";
            this.LblSlotCoordinate2.Text = "Coord 2";
            this.LblSlotCoordinate3.Text = "Coord 3";
            this.WarehouseTab.Title = "Warehouse";
            this.SectionTab.Title = "Section";
            this.SlotTab.Title = "Slot";
            this.CboSectionSearch.Items.Add("Code");
            this.CboSectionSearch.Items.Add("Description");
            this.CboSectionSearch.SelectedItem = "Code";

            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Warehouses = new ObservableCollection<Warehouse>();
            this.Sections = new ObservableCollection<Section>();
            this.Slot1Coordinates = new ObservableCollection<Slot>();
            this.Slot2Coordinates = new ObservableCollection<Slot>();
            this.Slot3Coordinates = new ObservableCollection<Slot>();
            this.LocMap = LocationMap.GetInstance();
            this.LocMapInfo = this.LocMap.CurrLocMap.Clone();
            this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
            this.LocMap.ChosenLocMapRefno = this.LocMap.CurrLocMapRefno;
            this.DefaultWhsLocMapRefno = defaultWhsLocMapRefno;
            this.LockWhsSelection = lockWhsSelection;

            if (this.LockWhsSelection)
            {
                this.WarehousesView.IsEnabled = false;
            }

            this.ApplyEvents();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the warehouse selection is locked from change.
        /// </summary>
        private bool LockWhsSelection { get; }

        /// <summary>
        ///     Gets or sets the default warehouse location map reference number.
        /// </summary>
        private int DefaultWhsLocMapRefno { get; }

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.SelectLocation;

        /// <summary>
        ///     Gets or sets the location map information.
        /// </summary>
        private SLocMap LocMapInfo { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; } = LocationMap.GetInstance();

        /// <summary>
        ///     Gets or sets the slot coordinate 3 list.
        /// </summary>
        private List<Slot> ListSlotCoord3 { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets the slot coordinate 2 list.
        /// </summary>
        private List<Slot> ListSlotCoord2 { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets the slot coordinate 1 list.
        /// </summary>
        private List<Slot> ListSlotCoord1 { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets the list of sections.
        /// </summary>
        private List<Section> ListSections { get; set; } = new List<Section>();

        /// <summary>
        ///     Gets or sets the warehouse list.
        /// </summary>
        private List<Warehouse> ListWarehouse { get; set; } = new List<Warehouse>();

        /// <summary>
        ///     Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the bar code parsed data.
        /// </summary>
        private BarCodeParse Parse { get; set; }

        /// <summary>
        ///     Gets or sets the warehouse observable collection.
        /// </summary>
        private ObservableCollection<Warehouse> Warehouses { get; set; }

        /// <summary>
        ///     Gets or sets the section observable collection.
        /// </summary>
        private ObservableCollection<Section> Sections { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 1 observable collection.
        /// </summary>
        private ObservableCollection<Slot> Slot1Coordinates { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 2 observable collection.
        /// </summary>
        private ObservableCollection<Slot> Slot2Coordinates { get; set; }

        /// <summary>
        ///     Gets or sets the slot coordinate 3 observable collection.
        /// </summary>
        private ObservableCollection<Slot> Slot3Coordinates { get; set; }

        /// <summary>
        ///     Gets or sets the list of filtered warehouses.
        /// </summary>
        private List<Warehouse> FilteredWarehouses { get; set; } = new List<Warehouse>();

        /// <summary>
        ///     Gets or sets the list of filtered sections.
        /// </summary>
        private List<Section> FilteredListSections { get; set; } = new List<Section>();

        /// <summary>
        ///     Gets or sets the list of filtered slot coordinates 1.
        /// </summary>
        private List<Slot> FilteredSlot1Coordinates { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets the list of filtered slot coordinates 2.
        /// </summary>
        private List<Slot> FilteredSlot2Coordinates { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets the list of filtered slot coordinates 3.
        /// </summary>
        private List<Slot> FilteredSlot3Coordinates { get; set; } = new List<Slot>();

        /// <summary>
        ///     Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; }

        /// <summary>
        ///     Un-register the bar code scan event handler and disable the scanner.
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
                        this.Scanner.BarCodeScan += this.BarCodeHandler;
                    }

                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        ///     Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;

            try
            {
                isReadyToSave = true;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
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
        ///     Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}" /> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                    isReadyToClose = true;
                }

                if (isReadyToClose)
                {
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
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
                    this.IsChangesMade = false;
                    saveComplete = true;
                    this.LocMap.ChosenLocMap = this.LocMapInfo.Clone();
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "Data failed validation. Check your data.",
                        "OK"));
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
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

            try
            {
                if (await this.IsReadyToClose())
                {
                    // Reset any properties if needed.
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation containing an XDocument.</returns>
        public async Task<XDocument> GetXml()
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<LocationMapRefno>" + LocationMapTasks.GetLocMapRefnoFromSLocMap(this.LocMap.ChosenLocMap) +
                "</LocationMapRefno>"
                + "</params>");

            return request;
        }

        /// <summary>
        ///     Get control activity description.
        /// </summary>
        /// <returns><see cref="string" /> Control activity description.</returns>
        public string GetControlDescription()
        {
            return "Select Location";
        }

        /// <summary>
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                if (!this.IsIndicatorRunning)
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    SubscribeKeyUpHandlers();
                    this.IsEnabled = false;
                    await this.ScannerEnable();
                    await this.LoadData();
                    await this.SelectWarehouse();

                    // Selecting the section and slot are required here even if the section / slot grids are not display
                    // because we want to set the grid(s).CurrentRowIdx
                    if (this.LocMap.ChosenLocMap.IsSectionTracked)
                    {
                        await this.SelectSection();
                        await this.SelectSlotCoordinate();
                    }

                    await this.UpdateLocationDisplay();
                    await TabLocation_GotFocus(this, EventArgs.Empty);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                    const string msg = "This activity unexpectedly failed to initialize correctly." +
                                       " Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.IsEnabled = true;
                this.IsIndicatorRunning = false;
            }

            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                {
                    if (this.IsIndicatorRunning)
                    {
                        return;
                    }

                    var svdIsScanReg = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();

                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                    }
                    else if (keycode == Keycode.DpadDown)
                    {
                        if (this.CurrentPage.Title == this.WarehouseTab.Title)
                        {
                            if (this.WarehousesView.SelectedItem != null && Convert.ToInt32(
                                    this.Warehouses.IndexOf(this.WarehousesView.SelectedItem as Warehouse)) <
                                this.Warehouses.Count - 1)
                            {
                                this.WarehousesView.SelectedItem = this.Warehouses[
                                    this.Warehouses.IndexOf(this.WarehousesView.SelectedItem as Warehouse) +
                                    1];
                                this.WarehousesView.ScrollTo(this.WarehousesView.SelectedItem,
                                    ScrollToPosition.MakeVisible, true);
                            }
                        }
                        else if (this.CurrentPage.Title == this.SectionTab.Title)
                        {
                            if (this.SectionsView.SelectedItem != null && Convert.ToInt32(
                                    this.Sections.IndexOf(this.SectionsView.SelectedItem as Section)) <
                                this.Sections.Count - 1)
                            {
                                this.SectionsView.SelectedItem = this.Sections[
                                    this.Sections.IndexOf(this.SectionsView.SelectedItem as Section) +
                                    1];
                                this.SectionsView.ScrollTo(this.SectionsView.SelectedItem, ScrollToPosition.MakeVisible,
                                    true);
                            }
                        }
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        if (this.CurrentPage.Title == this.WarehouseTab.Title)
                        {
                            if (this.WarehousesView.SelectedItem != null && Convert.ToInt32(
                                    this.Warehouses.IndexOf(this.WarehousesView.SelectedItem as Warehouse)) >
                                0)
                            {
                                this.WarehousesView.SelectedItem = this.Warehouses[
                                    this.Warehouses.IndexOf(this.WarehousesView.SelectedItem as Warehouse) -
                                    1];
                                this.WarehousesView.ScrollTo(this.WarehousesView.SelectedItem,
                                    ScrollToPosition.MakeVisible, true);
                            }
                        }
                        else if (this.CurrentPage.Title == this.SectionTab.Title)
                        {
                            if (this.SectionsView.SelectedItem != null && Convert.ToInt32(
                                    this.Sections.IndexOf(this.SectionsView.SelectedItem as Section)) >
                                0)
                            {
                                this.SectionsView.SelectedItem = this.Sections[
                                    this.Sections.IndexOf(this.SectionsView.SelectedItem as Section) -
                                    1];
                                this.SectionsView.ScrollTo(this.SectionsView.SelectedItem, ScrollToPosition.MakeVisible,
                                    true);
                            }
                        }
                    }

                    if (svdIsScanReg)
                    {
                        await this.ScannerEnable();
                    }
                });
            }

            async Task TabLocation_GotFocus(object sender, EventArgs e)
            {
                if (this.FilteredWarehouses == null || this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "Select the warehouse first!",
                        "OK"));
                }
                else
                {
                    if (this.FilteredListSections != null)
                    {
                        this.FilteredListSections = this.ListSections.Where(
                            section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();

                        await this.SelectSection();

                        if (this.FilteredListSections.Count > 0)
                        {
                            if (this.SectionsView.SelectedItem is Section selectedSection &&
                                this.LocMapInfo.SectLocMapRefno != selectedSection.LocationMapRefno)
                            {
                                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    selectedSection.LocationMapRefno);
                            }
                        }
                    }
                }

                if (this.FilteredListSections == null || this.LocMapInfo.SectLocMapRefno == 0)
                {
                    //// await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(
                    //// this.GetControlDescription(), "Select the section first!", "OK"));
                }
                else
                {
                    if (this.FilteredSlot1Coordinates != null && this.FilteredSlot2Coordinates != null &&
                        this.FilteredSlot3Coordinates != null)
                    {
                        this.FilteredSlot1Coordinates = this.ListSlotCoord1.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredSlot2Coordinates = this.ListSlotCoord2.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredSlot3Coordinates = this.ListSlotCoord3.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();

                        await this.SelectSlotCoordinate();

                        if (this.FilteredSlot1Coordinates.Count > 0)
                        {
                            var slotLocMapRefno = 0;

                            if (this.dgCoord1.SelectedItem is Slot slotCoord1 &&
                                this.dgCoord2.SelectedItem is Slot slotCoord2 &&
                                this.dgCoord3.SelectedItem is Slot slotCoord3)
                            {
                                slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
                                    this.LocMapInfo,
                                    this.LocMapInfo.SectLocMapRefno,
                                    slotCoord1.LocationMapSlotCoordinateRefno,
                                    slotCoord2.LocationMapSlotCoordinateRefno,
                                    slotCoord3.LocationMapSlotCoordinateRefno);
                            }

                            if (slotLocMapRefno != 0)
                            {
                                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                            }
                        }
                    }
                }

                await this.UpdateLocationDisplay();
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

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    _ = base.OnBackButtonPressed();
                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        await this.Close();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
                        var msg = "This activity unexpectedly failed to initialize correctly." +
                                  " Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
            });

            return true;
        }

        /// <summary>
        ///     Turn on form event handlers.
        /// </summary>
        private void ApplyEvents()
        {
            this.warehouseLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.SectionLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.SlotLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;

            this.warehouseLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.SectionLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.SlotLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;

            this.CurrentPageChanged += this.SetLocationMap_CurrentPageChanged;
        }

        /// <summary>
        ///     Set location map, current page changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void SetLocationMap_CurrentPageChanged(object sender, EventArgs e)
        {
            if (this.CurrentPage.Title == this.SectionTab.Title)
            {
                if (this.SectionsView.SelectedItem == null && this.Sections.Count > 0)
                {
                    this.SectionsView.SelectedItem = this.Sections[0];
                }
            }
            else if (this.CurrentPage.Title == this.SlotTab.Title)
            {
                if (this.SectionsView.SelectedItem == null && this.Sections.Count > 0)
                {
                    this.SectionsView.SelectedItem = this.Sections[0];
                }

                if (this.dgCoord1.SelectedItem == null && this.Slot1Coordinates.Count > 0)
                {
                    this.dgCoord1.SelectedItem = this.Slot1Coordinates[0];
                }

                if (this.dgCoord2.SelectedItem == null && this.Slot2Coordinates.Count > 0)
                {
                    this.dgCoord2.SelectedItem = this.Slot2Coordinates[0];
                }

                if (this.dgCoord3.SelectedItem == null && this.Slot3Coordinates.Count > 0)
                {
                    this.dgCoord3.SelectedItem = this.Slot3Coordinates[0];
                }
            }
        }

        /// <summary>
        ///     Search section text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private async void TxtSearchSection_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

                // By Code
                if (this.CboSectionSearch.SelectedIndex == 0)
                {
                    if (string.Compare(this.TxtSearchSection.Text, string.Empty) > 0)
                    {
                        this.FilteredListSections = this.ListSections.Where(
                            section => Convert.ToString(section.LocationMapSectionCode) ==
                                       this.TxtSearchSection.Text.Trim()).ToList();
                    }
                    else
                    {
                        this.FilteredListSections = this.ListSections;
                    }
                }
                else
                {
                    this.FilteredListSections = this.ListSections.Where(
                        section => section.LocationMapSectionName.Contains(this.TxtSearchSection.Text.Trim())).ToList();
                }
            }
            catch (EvaluateException)
            {
                this.FilteredListSections = this.ListSections.Where(
                    section => Convert.ToString(section.LocationMapSectionCode) == Convert.ToString(-1)).ToList();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSectionList();
            }
        }

        /// <summary>
        ///     Slot coordinate 1 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private async void TxtSlotCoordinate1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.TxtSlotCoordinate1.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot1Coordinates = this.ListSlotCoord1.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                        this.TxtSlotCoordinate1.Text.Trim().ToUpper())
                                    && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot1Coordinates = this.ListSlotCoord1.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.TxtSlotCoordinate1.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot1Coordinates = this.ListSlotCoord1.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot1Coordinates = this.ListSlotCoord1;
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord1List();
            }
        }

        /// <summary>
        ///     Populate slot coordinate 1 list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task PopulateSlotCoord1List()
        {
            this.Slot1Coordinates = new ObservableCollection<Slot>();

            foreach (var slot in this.FilteredSlot1Coordinates)
            {
                this.Slot1Coordinates.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() =>
                this.dgCoord1.ItemsSource = this.Slot1Coordinates);
        }

        /// <summary>
        ///     Slot coordinate 2 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private async void TxtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.TxtSlotCoordinate2.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot2Coordinates = this.ListSlotCoord2.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(this.TxtSlotCoordinate2.Text.Trim()
                                        .ToUpper())
                                    && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot2Coordinates = this.ListSlotCoord2.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.TxtSlotCoordinate2.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot2Coordinates = this.ListSlotCoord2.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot2Coordinates = this.ListSlotCoord2;
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord2List();
            }
        }

        /// <summary>
        ///     Populate slot coordinate 2 list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task PopulateSlotCoord2List()
        {
            this.Slot2Coordinates = new ObservableCollection<Slot>();

            foreach (var slot in this.FilteredSlot2Coordinates)
            {
                this.Slot2Coordinates.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() =>
                this.dgCoord2.ItemsSource = this.Slot2Coordinates);
        }

        /// <summary>
        ///     Slot coordinate 3 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private async void TxtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.TxtSlotCoordinate3.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot3Coordinates = this.ListSlotCoord3.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(this.TxtSlotCoordinate3.Text.Trim()
                                        .ToUpper())
                                    && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot3Coordinates = this.ListSlotCoord3.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.TxtSlotCoordinate3.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredSlot3Coordinates = this.ListSlotCoord3.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredSlot3Coordinates = this.ListSlotCoord3;
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord3List();
            }
        }

        /// <summary>
        ///     Populate slot coordinate 3 list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task PopulateSlotCoord3List()
        {
            this.Slot3Coordinates = new ObservableCollection<Slot>();

            foreach (var slot in this.FilteredSlot3Coordinates)
            {
                this.Slot3Coordinates.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() =>
                this.dgCoord3.ItemsSource = this.Slot3Coordinates);
        }

        /// <summary>
        ///     Load the list of warehouses.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task LoadWhsList()
        {
            this.ListWarehouse = await LocationMapTasks.GetWhsList();
            this.FilteredWarehouses = this.ListWarehouse;
            await PopulateWarehouseList();
            return;

            async Task PopulateWarehouseList()
            {
                this.Warehouses = new ObservableCollection<Warehouse>();

                foreach (var warehouse in this.FilteredWarehouses)
                {
                    this.Warehouses.Add(warehouse);
                }

                _ = await Device.InvokeOnMainThreadAsync(() =>
                    this.WarehousesView.ItemsSource = this.Warehouses);
            }
        }

        /// <summary>
        ///     Load the list of sections.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task LoadSections()
        {
            this.ListSections = await LocationMapTasks.GetSectList();
            this.FilteredListSections = this.ListSections.Where(
                section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();
            await this.PopulateSectionList();
        }

        /// <summary>
        ///     Load the list of slots.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task LoadSlots()
        {
            this.ListSlotCoord1 = await LocationMapTasks.GetSlotCoord1List();
            this.FilteredSlot1Coordinates = this.ListSlotCoord1.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord1List();

            this.ListSlotCoord2 = await LocationMapTasks.GetSlotCoord2List();
            this.FilteredSlot2Coordinates = this.ListSlotCoord2.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord2List();

            this.ListSlotCoord3 = await LocationMapTasks.GetSlotCoord3List();
            this.FilteredSlot3Coordinates = this.ListSlotCoord3.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord3List();
        }

        /// <summary>
        ///     Load the data for the form controls.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task LoadData()
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

                // Default to the login warehouse
                int locMapRefno;

                if (this.DefaultWhsLocMapRefno > 0)
                {
                    // If the default warehouse match the current
                    if (this.LocMap.ChosenLocMap.WhsLocMapRefno == this.DefaultWhsLocMapRefno)
                    {
                        locMapRefno = this.LocMap.ChosenLocMapRefno;
                    }
                    else
                    {
                        locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                            this.DefaultWhsLocMapRefno,
                            0,
                            SysCommon.ProdLocMapTypes.Picking);
                    }
                }
                else if (this.LocMap.ChosenLocMap.WhsLocMapRefno <= 0)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        this.LocMap.LoginWhsLocMapRefno,
                        0,
                        SysCommon.ProdLocMapTypes.Picking);
                }
                else
                {
                    locMapRefno = this.LocMap.ChosenLocMapRefno;
                }

                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                // Load the data views and apply row filters base on the current sLocationMapInfo values
                await this.LoadWhsList();
                await this.LoadSections();
                await this.LoadSlots();
                await this.SelectWarehouse();
                await this.SelectSection();
                await this.SelectSlotCoordinate();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Select the initial warehouse.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SelectWarehouse()
        {
            // Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                var selectedWarehouse = this.Warehouses.FirstOrDefault(
                    warehouse => warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno);

                if (selectedWarehouse != null)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.WarehousesView.SelectedItem = selectedWarehouse);
                }
                else if (this.Warehouses.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.WarehousesView.SelectedItem = this.Warehouses[0]);
                }
            }
            else
            {
                if (this.Warehouses.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.WarehousesView.SelectedItem = this.Warehouses[0]);
                }
            }
        }

        /// <summary>
        ///     Update the location display.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task UpdateLocationDisplay()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.warehouseLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.SectionLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.SectionLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.SlotLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.SlotLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " +
                                                                       this.LocMapInfo.WhsName);
                    this.warehouseLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                    this.SectionLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " +
                                                                     this.LocMapInfo.WhsName);
                    this.SectionLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                    this.SlotLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " +
                                                                  this.LocMapInfo.WhsName);
                    this.SlotLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                }

                if (this.LocMapInfo.SectLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetSectionDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.SectionLocationMapInfo.SetSectionDataText("Unknown");
                    this.SectionLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.SlotLocationMapInfo.SetSectionDataText("Unknown");
                    this.SlotLocationMapInfo.SetSectionDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " +
                                                                     this.LocMapInfo.SectName);
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.SectionLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " +
                                                                   this.LocMapInfo.SectName);
                    this.SectionLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.SlotLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " +
                                                                this.LocMapInfo.SectName);
                    this.SlotLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                }

                if (this.LocMapInfo.SlotLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetSlotDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.SectionLocationMapInfo.SetSlotDataText("Unknown");
                    this.SectionLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.SlotLocationMapInfo.SetSlotDataText("Unknown");
                    this.SlotLocationMapInfo.SetSlotDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.SectionLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.SectionLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.SlotLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.SlotLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                }

                if (this.CurrentPage.TabIndex == 1)
                {
                    this.CboSectionSearch.SelectedIndex = 0;
                }

                if (this.CurrentPage.TabIndex == 2)
                {
                    if (this.FilteredSlot1Coordinates.Count > 0)
                    {
                        if (this.dgCoord1.SelectedItem is Slot slot1)
                        {
                            this.LblSlotCoordinate1.Text = "Coord1: " + slot1.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.FilteredSlot2Coordinates.Count > 0)
                    {
                        if (this.dgCoord2.SelectedItem is Slot slot2)
                        {
                            this.LblSlotCoordinate2.Text = "Coord2: " + slot2.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.FilteredSlot3Coordinates.Count > 0)
                    {
                        if (this.dgCoord3.SelectedItem is Slot slot3)
                        {
                            this.LblSlotCoordinate3.Text = "Coord3: " + slot3.LocationMapSlotCoordinateCode;
                        }
                    }
                }
            });
        }

        /// <summary>
        ///     Select the section.
        /// </summary>
        /// <remarks><see cref="Task" /> Task operation.</remarks>
        private async Task SelectSection()
        {
            if (this.LocMap.ChosenLocMap.IsSectionTracked)
            {
                // attempt to find last selected section in section view
                if (this.LocMapInfo.SectLocMapRefno != 0)
                {
                    var section = this.Sections.FirstOrDefault(
                        section => section.LocationMapRefno == this.LocMapInfo.SectLocMapRefno);

                    if (section != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.SectionsView.SelectedItem = section);
                    }
                    else if (this.Sections.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.SectionsView.SelectedItem = this.Sections[0]);
                    }
                }
                else
                {
                    if (this.Sections.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.SectionsView.SelectedItem = this.Sections[0]);
                    }
                }
            }
        }

        /// <summary>
        ///     Select the slot coordinate.
        /// </summary>
        /// <remarks><see cref="Task" /> Task operation.</remarks>
        private async Task SelectSlotCoordinate()
        {
            if (this.LocMap.ChosenLocMap.IsSlotTracked)
            {
                if (this.FilteredSlot1Coordinates.Count > this.Slot1Coordinates.Count)
                {
                    await this.PopulateSlotCoord1List();
                }

                if (this.FilteredSlot2Coordinates.Count > this.Slot2Coordinates.Count)
                {
                    await this.PopulateSlotCoord2List();
                }

                if (this.FilteredSlot3Coordinates.Count > this.Slot3Coordinates.Count)
                {
                    await this.PopulateSlotCoord3List();
                }

                // Attempt to find last selected slot coordinate in slot coordinate 1 view
                if (this.LocMapInfo.SlotCoord1LocMapRefno != 0)
                {
                    var slotCoord1 = this.Slot1Coordinates.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno);

                    if (slotCoord1 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.SelectedItem = slotCoord1);
                    }
                    else if (this.Slot1Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord1.SelectedItem = this.Slot1Coordinates[0]);
                    }
                }
                else
                {
                    if (this.Slot1Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord1.SelectedItem = this.Slot1Coordinates[0]);
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    var slotCoord2 = this.Slot2Coordinates.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord2LocMapRefno);

                    if (slotCoord2 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.SelectedItem = slotCoord2);
                    }
                    else if (this.Slot2Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord2.SelectedItem = this.Slot2Coordinates[0]);
                    }
                }
                else
                {
                    if (this.Slot2Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord2.SelectedItem = this.Slot2Coordinates[0]);
                    }
                }

                // Attempt to find last selected slot coordinate in slot coordinate 3 view
                if (this.LocMapInfo.SlotCoord3LocMapRefno > 0)
                {
                    var slotCoord3 = this.Slot3Coordinates.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord3LocMapRefno);

                    if (slotCoord3 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.SelectedItem = slotCoord3);
                    }
                    else if (this.Slot3Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord3.SelectedItem = this.Slot3Coordinates[0]);
                    }
                }
                else
                {
                    if (this.Slot3Coordinates.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord3.SelectedItem = this.Slot3Coordinates[0]);
                    }
                }
            }
        }

        /// <summary>
        ///     Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Scan event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;

                // No need to disable/enable scanner because only message box are called.
                // GoFocus and LostFocus events will handle the scanner
                this.Parse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.Parse.ActFkRefno))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning an header reference # is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ActItemFkRefno))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning an item reference # is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning a product bar code is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.Sscc18))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning a shipping container bar code is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        this.Parse.LocationIdentifier);
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                    await this.SelectWarehouse();
                    await this.SelectSection();
                    await this.SelectSlotCoordinate();
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "Scanned bar code type is not supported here!",
                        "OK"));
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Warehouse list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void Whs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                this.IsChangesMade = true;

                if (e.SelectedItem is Warehouse warehouse)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        Convert.ToInt32(warehouse.LocationMapRefno));
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.SectionsView.SelectedItem = null;
                        this.dgCoord1.SelectedItem = null;
                        this.dgCoord2.SelectedItem = null;
                        this.dgCoord3.SelectedItem = null;
                    });
                    await this.LoadSections();
                    await this.UpdateLocationDisplay();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Section list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void Sections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                this.IsChangesMade = true;
                if (e.SelectedItem is Section section)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        Convert.ToInt32(section.LocationMapRefno));
                    this.dgCoord1.SelectedItem = null;
                    this.dgCoord2.SelectedItem = null;
                    this.dgCoord3.SelectedItem = null;
                    await this.LoadSlots();
                    await this.UpdateLocationDisplay();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Slot coordinate 1 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void Coord1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                this.IsChangesMade = true;
                var slotLocMapRefno = 0;

                if (e.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 &&
                    this.dgCoord3.SelectedItem is Slot slot3)
                {
                    slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
                        this.LocMapInfo,
                        this.LocMapInfo.SectLocMapRefno,
                        slot1.LocationMapSlotCoordinateRefno,
                        slot2.LocationMapSlotCoordinateRefno,
                        slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                }

                await this.UpdateLocationDisplay();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Slot coordinate 2 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void Coord2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                this.IsChangesMade = true;
                var slotLocMapRefno = 0;

                if (this.dgCoord1.SelectedItem is Slot slot1 && e.SelectedItem is Slot slot2 &&
                    this.dgCoord3.SelectedItem is Slot slot3)
                {
                    slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
                        this.LocMapInfo,
                        this.LocMapInfo.SectLocMapRefno,
                        slot1.LocationMapSlotCoordinateRefno,
                        slot2.LocationMapSlotCoordinateRefno,
                        slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                }

                await this.UpdateLocationDisplay();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Slot coordinate 3 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event arguments.</param>
        private async void Coord3_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                this.IsChangesMade = true;
                var slotLocMapRefno = 0;

                if (this.dgCoord1.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 &&
                    e.SelectedItem is Slot slot3)
                {
                    slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
                        this.LocMapInfo,
                        this.LocMapInfo.SectLocMapRefno,
                        slot1.LocationMapSlotCoordinateRefno,
                        slot2.LocationMapSlotCoordinateRefno,
                        slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                }

                await this.UpdateLocationDisplay();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Populate section list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task PopulateSectionList()
        {
            this.Sections = new ObservableCollection<Section>();

            foreach (var section in this.FilteredListSections)
            {
                this.Sections.Add(section);
            }

            _ = await Device.InvokeOnMainThreadAsync(() =>
                this.SectionsView.ItemsSource = this.Sections);
        }

        /// <summary>
        ///     Save location button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        ///     Cancel location button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.Cancel;
                await this.Close();
            }
        }
    }
}