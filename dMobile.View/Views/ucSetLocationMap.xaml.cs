// <copyright file="ucSetLocationMap.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Color = Xamarin.Forms.Color;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Set location map view model.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSetLocationMap : DMobileTabbedModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ucSetLocationMap"/> class.
        /// </summary>
        public ucSetLocationMap()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.tabWarehouse.Text = "Warehouse";
            this.tabSection.Text = "Section";
            this.lblSearch.Text = "Search";
            this.tabSlot.Text = "Slot";
            this.lblSlotCoordinate1.Text = "Coord 1";
            this.lblSlotCoordinate2.Text = "Coord 2";
            this.lblSlotCoordinate3.Text = "Coord 3";
            this.warehouseTab.Title = "Warehouse";
            this.sectionTab.Title = "Section";
            this.slotTab.Title = "Slot";
            this.cboSectionSearch.Items.Add("Code");
            this.cboSectionSearch.Items.Add("Description");
            this.cboSectionSearch.SelectedItem = "Code";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = ScannerEngine.GetInstance().Scanner;

                this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();
                this.SectionObservableCollection = new ObservableCollection<Section>();
                this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();
                this.LocMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                this.LocMap.ChosenLocMapRefno = this.LocMap.CurrLocMapRefno;
            }

            this.ApplyEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ucSetLocationMap"/> class.
        /// </summary>
        /// <param name="defaultWhsLocMapRefno"><see cref="int"/> Default warehouse location map reference number.</param>
        /// <param name="lockWhsSelection"><see cref="bool"/> Warehouse selection lock enabler.</param>
        public ucSetLocationMap(int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.tabWarehouse.Text = "Warehouse";
            this.tabSection.Text = "Section";
            this.lblSearch.Text = "Search";
            this.tabSlot.Text = "Slot";
            this.lblSlotCoordinate1.Text = "Coord 1";
            this.lblSlotCoordinate2.Text = "Coord 2";
            this.lblSlotCoordinate3.Text = "Coord 3";
            this.warehouseTab.Title = "Warehouse";
            this.sectionTab.Title = "Section";
            this.slotTab.Title = "Slot";
            this.cboSectionSearch.Items.Add("Code");
            this.cboSectionSearch.Items.Add("Description");
            this.cboSectionSearch.SelectedItem = "Code";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = ScannerEngine.GetInstance().Scanner;

                this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();
                this.SectionObservableCollection = new ObservableCollection<Section>();
                this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();
                this.LocMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                this.LocMap.ChosenLocMapRefno = this.LocMap.CurrLocMapRefno;
                this.DefaultWhsLocMapRefno = defaultWhsLocMapRefno;
                this.LockWhsSelection = lockWhsSelection;

                if (this.LockWhsSelection)
                {
                    this.dgWhs.IsEnabled = false;
                }
            }

            this.ApplyEvents();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warehouse selection is locked from change.
        /// </summary>
        private bool LockWhsSelection { get; set; } = false;

        /// <summary>
        /// Gets or sets the default warehouse location map reference number.
        /// </summary>
        private int DefaultWhsLocMapRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.SelectLocation;

        /// <summary>
        /// Gets or sets the location map information.
        /// </summary>
        private SLocMap LocMapInfo { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        /// Gets or sets the slot coordinate 3 list.
        /// </summary>
        private List<Slot> ListSlotCoord3 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets the slot coordinate 2 list.
        /// </summary>
        private List<Slot> ListSlotCoord2 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets the slot coordinate 1 list.
        /// </summary>
        private List<Slot> ListSlotCoord1 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets the list of sections.
        /// </summary>
        private List<Section> ListSections { get; set; } = new List<Section>();

        /// <summary>
        /// Gets or sets the warehouse list.
        /// </summary>
        private List<Warehouse> ListWarehouse { get; set; } = new List<Warehouse>();

        /// <summary>
        /// Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control / view has been loaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code parsed data.
        /// </summary>
        private BarCodeParse Parse { get; set; }

        /// <summary>
        /// Gets or sets the warehouse observable collection.
        /// </summary>
        private ObservableCollection<Warehouse> WarehouseObservableCollection { get; set; } =
            new ObservableCollection<Warehouse>();

        /// <summary>
        /// Gets or sets the section observable collection.
        /// </summary>
        private ObservableCollection<Section> SectionObservableCollection { get; set; } =
            new ObservableCollection<Section>();

        /// <summary>
        /// Gets or sets the slot coordinate 1 observable collection.
        /// </summary>
        private ObservableCollection<Slot> SlotCoord1ObservableCollection { get; set; } =
            new ObservableCollection<Slot>();

        /// <summary>
        /// Gets or sets the slot coordinate 2 observable collection.
        /// </summary>
        private ObservableCollection<Slot> SlotCoord2ObservableCollection { get; set; } =
            new ObservableCollection<Slot>();

        /// <summary>
        /// Gets or sets the slot coordinate 3 observable collection.
        /// </summary>
        private ObservableCollection<Slot> SlotCoord3ObservableCollection { get; set; } =
            new ObservableCollection<Slot>();

        /// <summary>
        /// Gets or sets the list of filtered warehouses.
        /// </summary>
        private List<Warehouse> FilteredListWarehouse { get; set; } = new List<Warehouse>();

        /// <summary>
        /// Gets or sets the list of filtered sections.
        /// </summary>
        private List<Section> FilteredListSections { get; set; } = new List<Section>();

        /// <summary>
        /// Gets or sets the list of filtered slot coordinates 1.
        /// </summary>
        private List<Slot> FilteredListSlotCoord1 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets the list of filtered slot coordinates 2.
        /// </summary>
        private List<Slot> FilteredListSlotCoord2 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets the list of filtered slot coordinates 3.
        /// </summary>
        private List<Slot> FilteredListSlotCoord3 { get; set; } = new List<Slot>();

        /// <summary>
        /// Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; } = false;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {

            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool"/> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerEnable();
                isReadyToSave = true;
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
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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

            return isReadyToSave;
        }

        /// <summary>
        /// Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}"/> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = !this.IsChangesMade;

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
                }
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
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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

            return isReadyToClose;
        }

        /// <summary>
        /// Get control activity description.
        /// </summary>
        /// <returns><see cref="string"/> Control activity description.</returns>
        public string GetControlDescription()
        {
            return "Select Location";
        }

        /// <summary>
        /// Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool"/> True if a save was performed successfully else false.</returns>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
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
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
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

                saveComplete = false;
            }
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }

            return saveComplete;
        }

        /// <summary>
        /// Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool"/>. True if the changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (await this.IsReadyToClose())
                {
                    // Reset any properties if needed.
                }
                else
                {
                    await this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
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

                cancelChanges = false;
            }
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }

            return cancelChanges;
        }

        /// <summary>
        /// Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation containing an XDocument.</returns>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<LocationMapRefno>" + LocationMapTasks.GetLocMapRefnoFromSLocMap(this.LocMap.ChosenLocMap) + "</LocationMapRefno>"
                + "</params>");

            return request;
        }

        /// <summary>
        /// Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                SubscribeKeyUpHandlers();

                this.Loading = true;
                this.IsEnabled = false;
                if (!this.IsControlLoaded)
                {
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

                    this.IsControlLoaded = true;
                    await this.UpdateLocationDisplay();
                }

                await TabLocation_GotFocus(this, new EventArgs());
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
                    string msg = "This activity unexpectedly failed to initialize correctly." +
                        " Please exit and try again.";
                    FormErrorReport errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.IsEnabled = true;
                this.Loading = false;
            }

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    bool svdIsScanReg = this.IsBarCodeHandlerRegistered;
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
                        if (this.CurrentPage.Title == this.warehouseTab.Title)
                        {
                            if (this.dgWhs.SelectedItem != null && Convert.ToInt32(
                                this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) <
                                this.WarehouseObservableCollection.Count - 1)
                            {
                                this.dgWhs.SelectedItem = this.WarehouseObservableCollection[
                                    this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) + 1];
                                this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                            }
                        }
                        else if (this.CurrentPage.Title == this.sectionTab.Title)
                        {
                            if (this.dgSections.SelectedItem != null && Convert.ToInt32(
                                this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) <
                                this.SectionObservableCollection.Count - 1)
                            {
                                this.dgSections.SelectedItem = this.SectionObservableCollection[
                                    this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) + 1];
                                this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible, true);
                            }
                        }
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        if (this.CurrentPage.Title == this.warehouseTab.Title)
                        {
                            if (this.dgWhs.SelectedItem != null && Convert.ToInt32(
                                this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) > 0)
                            {
                                this.dgWhs.SelectedItem = this.WarehouseObservableCollection[
                                    this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) - 1];
                                this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                            }
                        }
                        else if (this.CurrentPage.Title == this.sectionTab.Title)
                        {
                            if (this.dgSections.SelectedItem != null && Convert.ToInt32(
                                this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) > 0)
                            {
                                this.dgSections.SelectedItem = this.SectionObservableCollection[
                                    this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) - 1];
                                this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible, true);
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
                if (this.FilteredListWarehouse == null || this.LocMapInfo.WhsLocMapRefno == 0)
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
                            if (this.dgSections.SelectedItem is Section selectedSection &&
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
                    if (this.FilteredListSlotCoord1 != null && this.FilteredListSlotCoord2 != null &&
                        this.FilteredListSlotCoord3 != null)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();

                        await this.SelectSlotCoordinate();

                        if (this.FilteredListSlotCoord1.Count > 0)
                        {
                            int slotLocMapRefno = 0;

                            if (this.dgCoord1.SelectedItem is Slot slotCoord1 &&
                                this.dgCoord2.SelectedItem is Slot slotCoord2 &&
                                this.dgCoord3.SelectedItem is Slot slotCoord3)
                            {
                                slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
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
        /// Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        /// <summary>
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();
                    _ = base.OnBackButtonPressed();

                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        await this.Close();
                    }
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
                        string msg = "This activity unexpectedly failed to initialize correctly." +
                            " Please exit and try again.";
                        FormErrorReport errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    if (this.IsSvdBarScannerEnabled)
                    {
                        await this.ScannerEnable();
                    }
                }
            });

            return true;
        }

        /// <summary>
        /// Turn on form event handlers.
        /// </summary>
        private void ApplyEvents()
        {
            this.warehouseLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.sectionLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.slotLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;

            this.warehouseLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.sectionLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.slotLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;

            this.CurrentPageChanged += this.UcSetLocationMap_CurrentPageChanged;
        }

        /// <summary>
        /// Set location map, current page changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void UcSetLocationMap_CurrentPageChanged(object sender, EventArgs e)
        {
            if (this.CurrentPage.Title == this.sectionTab.Title)
            {
                if (this.dgSections.SelectedItem == null && this.SectionObservableCollection.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionObservableCollection[0];
                }
            }
            else if (this.CurrentPage.Title == this.slotTab.Title)
            {
                if (this.dgSections.SelectedItem == null && this.SectionObservableCollection.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionObservableCollection[0];
                }

                if (this.dgCoord1.SelectedItem == null && this.SlotCoord1ObservableCollection.Count > 0)
                {
                    this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0];
                }

                if (this.dgCoord2.SelectedItem == null && this.SlotCoord2ObservableCollection.Count > 0)
                {
                    this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0];
                }

                if (this.dgCoord3.SelectedItem == null && this.SlotCoord3ObservableCollection.Count > 0)
                {
                    this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0];
                }
            }
        }

        /// <summary>
        /// Search section text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private async void TxtSearchSection_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

                // By Code
                if (this.cboSectionSearch.SelectedIndex == 0)
                {
                    if (string.Compare(this.txtSearchSection.Text, string.Empty) > 0)
                    {
                        this.FilteredListSections = this.ListSections.Where(
                            section => Convert.ToString(section.LocationMapSectionCode) ==
                            this.txtSearchSection.Text.Trim()).ToList();
                    }
                    else
                    {
                        this.FilteredListSections = this.ListSections;
                    }
                }
                else
                {
                    this.FilteredListSections = this.ListSections.Where(
                        section => section.LocationMapSectionName.Contains(this.txtSearchSection.Text.Trim())).ToList();
                }
            }
            catch (EvaluateException)
            {
                this.FilteredListSections = this.ListSections.Where(
                    section => Convert.ToString(section.LocationMapSectionCode) == Convert.ToString(-1)).ToList();
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSectionList();
            }
        }

        /// <summary>
        /// Slot coordinate 1 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private async void TxtSlotCoordinate1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.txtSlotCoordinate1.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.txtSlotCoordinate1.Text.Trim().ToUpper())
                        && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.txtSlotCoordinate1.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1;
                    }
                }
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord1List();
            }
        }

        /// <summary>
        /// Populate slot coordinate 1 list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task PopulateSlotCoord1List()
        {
            this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord1)
            {
                this.SlotCoord1ObservableCollection.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.ItemsSource = this.SlotCoord1ObservableCollection);
        }

        /// <summary>
        /// Slot coordinate 2 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private async void TxtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.txtSlotCoordinate2.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate2.Text.Trim().ToUpper())
                            && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.txtSlotCoordinate2.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2;
                    }
                }
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord2List();
            }
        }

        /// <summary>
        /// Populate slot coordinate 2 list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task PopulateSlotCoord2List()
        {
            this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord2)
            {
                this.SlotCoord2ObservableCollection.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.ItemsSource = this.SlotCoord2ObservableCollection);
        }

        /// <summary>
        /// Slot coordinate 3 text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private async void TxtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (string.Compare(this.txtSlotCoordinate3.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate3.Text.Trim().ToUpper())
                            && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where(
                            slot => slot.LocationMapSlotCoordinateCode.Contains(
                                this.txtSlotCoordinate3.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where(
                            slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3;
                    }
                }
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }

                await this.PopulateSlotCoord3List();
            }
        }

        /// <summary>
        /// Populate slot coordinate 3 list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task PopulateSlotCoord3List()
        {
            this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord3)
            {
                this.SlotCoord3ObservableCollection.Add(slot);
            }

            _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.ItemsSource = this.SlotCoord3ObservableCollection);
        }

        /// <summary>
        /// Load the list of warehouses.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task LoadWhsList()
        {
            this.ListWarehouse = await LocationMapTasks.GetWhsList();
            this.FilteredListWarehouse = this.ListWarehouse;
            await PopulateWarehouseList();

            async Task PopulateWarehouseList()
            {
                this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();

                foreach (Warehouse warehouse in this.FilteredListWarehouse)
                {
                    this.WarehouseObservableCollection.Add(warehouse);
                }

                _ = await Device.InvokeOnMainThreadAsync(() => this.dgWhs.ItemsSource = this.WarehouseObservableCollection);
            }
        }

        /// <summary>
        /// Load the list of sections.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task LoadSections()
        {
            this.ListSections = await LocationMapTasks.GetSectList();
            this.FilteredListSections = this.ListSections.Where(
                section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();
            await this.PopulateSectionList();
        }

        /// <summary>
        /// Load the list of slots.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task LoadSlots()
        {
            this.ListSlotCoord1 = await LocationMapTasks.GetSlotCoord1List();
            this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord1List();

            this.ListSlotCoord2 = await LocationMapTasks.GetSlotCoord2List();
            this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord2List();

            this.ListSlotCoord3 = await LocationMapTasks.GetSlotCoord3List();
            this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where(
                slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord3List();
        }

        /// <summary>
        /// Load the data for the form controls.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
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
                        locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                            this.DefaultWhsLocMapRefno,
                            0,
                            SysCommon.ProdLocMapTypes.Picking);
                    }
                }
                else if (this.LocMap.ChosenLocMap.WhsLocMapRefno <= 0)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Select the initial warehouse.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task SelectWarehouse()
        {
            // Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                Warehouse selectedWarehouse = this.WarehouseObservableCollection.FirstOrDefault(
                    warehouse => warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno);

                if (selectedWarehouse != null)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.dgWhs.SelectedItem = selectedWarehouse);
                }
                else if (this.WarehouseObservableCollection.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0]);
                }
            }
            else
            {
                if (this.WarehouseObservableCollection.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0]);
                }
            }
        }

        /// <summary>
        /// Update the location display.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task UpdateLocationDisplay()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.warehouseLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.sectionLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.slotLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " + this.LocMapInfo.WhsName);
                    this.warehouseLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                    this.sectionLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " + this.LocMapInfo.WhsName);
                    this.sectionLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                    this.slotLocationMapInfo.SetWarehouseDataText(this.LocMapInfo.WhsCode + "  " + this.LocMapInfo.WhsName);
                    this.slotLocationMapInfo.SetWarehouseDataTextColor(Color.Blue);
                }

                if (this.LocMapInfo.SectLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetSectionDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetSectionDataText("Unknown");
                    this.sectionLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetSectionDataText("Unknown");
                    this.slotLocationMapInfo.SetSectionDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.sectionLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.sectionLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.slotLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.slotLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                }

                if (this.LocMapInfo.SlotLocMapRefno == 0)
                {
                    this.warehouseLocationMapInfo.SetSlotDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetSlotDataText("Unknown");
                    this.sectionLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetSlotDataText("Unknown");
                    this.slotLocationMapInfo.SetSlotDataTextColor(Color.Black);
                }
                else
                {
                    this.warehouseLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.sectionLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.sectionLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.slotLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.slotLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                }

                if (this.CurrentPage.TabIndex == 1)
                {
                    this.cboSectionSearch.SelectedIndex = 0;
                }

                if (this.CurrentPage.TabIndex == 2)
                {
                    if (this.FilteredListSlotCoord1.Count > 0)
                    {
                        if (this.dgCoord1.SelectedItem is Slot slot1)
                        {
                            this.lblSlotCoordinate1.Text = "Coord1: " + slot1.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.FilteredListSlotCoord2.Count > 0)
                    {
                        if (this.dgCoord2.SelectedItem is Slot slot2)
                        {
                            this.lblSlotCoordinate2.Text = "Coord2: " + slot2.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.FilteredListSlotCoord3.Count > 0)
                    {
                        if (this.dgCoord3.SelectedItem is Slot slot3)
                        {
                            this.lblSlotCoordinate3.Text = "Coord3: " + slot3.LocationMapSlotCoordinateCode;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Select the section.
        /// </summary>
        /// <remarks><see cref="Task"/> Task operation.</remarks>
        private async Task SelectSection()
        {
            if (this.LocMap.ChosenLocMap.IsSectionTracked)
            {
                // attempt to find last selected section in section view
                if (this.LocMapInfo.SectLocMapRefno != 0)
                {
                    Section section = this.SectionObservableCollection.FirstOrDefault(
                        section => section.LocationMapRefno == this.LocMapInfo.SectLocMapRefno);

                    if (section != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgSections.SelectedItem = section);
                    }
                    else if (this.SectionObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgSections.SelectedItem = this.SectionObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SectionObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgSections.SelectedItem = this.SectionObservableCollection[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Select the slot coordinate.
        /// </summary>
        /// <remarks><see cref="Task"/> Task operation.</remarks>
        private async Task SelectSlotCoordinate()
        {
            if (this.LocMap.ChosenLocMap.IsSlotTracked)
            {
                if (this.FilteredListSlotCoord1.Count > this.SlotCoord1ObservableCollection.Count)
                {
                    await this.PopulateSlotCoord1List();
                }

                if (this.FilteredListSlotCoord2.Count > this.SlotCoord2ObservableCollection.Count)
                {
                    await this.PopulateSlotCoord2List();
                }

                if (this.FilteredListSlotCoord3.Count > this.SlotCoord3ObservableCollection.Count)
                {
                    await this.PopulateSlotCoord3List();
                }

                // Attempt to find last selected slot coordinate in slot coordinate 1 view
                if (this.LocMapInfo.SlotCoord1LocMapRefno != 0)
                {
                    Slot slotCoord1 = this.SlotCoord1ObservableCollection.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno);

                    if (slotCoord1 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.SelectedItem = slotCoord1);
                    }
                    else if (this.SlotCoord1ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord1ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0]);
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    Slot slotCoord2 = this.SlotCoord2ObservableCollection.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno);

                    if (slotCoord2 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.SelectedItem = slotCoord2);
                    }
                    else if (this.SlotCoord2ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord2ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0]);
                    }
                }

                // Attempt to find last selected slot coordinate in slot coordinate 3 view
                if (this.LocMapInfo.SlotCoord3LocMapRefno > 0)
                {
                    Slot slotCoord3 = this.SlotCoord3ObservableCollection.FirstOrDefault(
                        slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno);

                    if (slotCoord3 != null)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.SelectedItem = slotCoord3);
                    }
                    else if (this.SlotCoord3ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord3ObservableCollection.Count > 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Scan event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            try
            {
                await this.ScannerDisable();
                this.Loading = true;

                // No need to disable/enable scanner because only message box are called.
                // GoFocus and LostFocus events will handle the scanner
                this.Parse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning an header reference # is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
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
                else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Scanning a shipping container bar code is not supported here!",
                            "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    int locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
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
                this.Loading = false;
            }
        }

        /// <summary>
        /// Warehouse list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Selected item changed event arguments.</param>
        private async void Whs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    if (e.SelectedItem is Warehouse warehouse)
                    {
                        this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            Convert.ToInt32(warehouse.LocationMapRefno));
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.dgSections.SelectedItem = null;
                            this.dgCoord1.SelectedItem = null;
                            this.dgCoord2.SelectedItem = null;
                            this.dgCoord3.SelectedItem = null;
                        });
                        await this.LoadSections();
                        await this.UpdateLocationDisplay();
                    }
                }
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Section list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Selected item changed event arguments.</param>
        private async void Sections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsControlLoaded)
                {
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Slot coordinate 1 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Selected item changed event arguments.</param>
        private async void Coord1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (e.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 &&
                        this.dgCoord3.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Slot coordinate 2 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Selected item changed event arguments.</param>
        private async void Coord2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (this.dgCoord1.SelectedItem is Slot slot1 && e.SelectedItem is Slot slot2 &&
                        this.dgCoord3.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Slot coordinate 3 list selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Selected item changed event arguments.</param>
        private async void Coord3_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (this.dgCoord1.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 &&
                        e.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(
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
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// Populate section list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task PopulateSectionList()
        {
            this.SectionObservableCollection = new ObservableCollection<Section>();

            foreach (Section section in this.FilteredListSections)
            {
                this.SectionObservableCollection.Add(section);
            }

            _ = await Device.InvokeOnMainThreadAsync(() => this.dgSections.ItemsSource = this.SectionObservableCollection);
        }

        /// <summary>
        /// Save location button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        /// Cancel location button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
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