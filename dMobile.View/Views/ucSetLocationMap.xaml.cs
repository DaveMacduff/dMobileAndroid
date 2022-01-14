namespace dMobile.View
{
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Color = Xamarin.Forms.Color;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSetLocationMap : DMobileTabbedModal, IAppArea
    {
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SelectLocation;
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private LocationMap locMap;
        private BarCodeParse parse = null;
        private BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SLocMap locMapInfo;
        private int defaultWhsLocMapRefno = 0;
        private bool lockWhsSelection = false;

        private List<Warehouse> listWarehouse;
        private List<Section> listSections;
        private List<Slot> listSlotCoord1;
        private List<Slot> listSlotCoord2;
        private List<Slot> listSlotCoord3;

        private List<Warehouse> filteredListWarehouse;
        private List<Section> filteredListSections;
        private List<Slot> filteredListSlotCoord1;
        private List<Slot> filteredListSlotCoord2;
        private List<Slot> filteredListSlotCoord3;

        private ObservableCollection<Warehouse> warehouseObservableCollection;
        private ObservableCollection<Section> sectionObservableCollection;
        private ObservableCollection<Slot> slotCoord1ObservableCollection;
        private ObservableCollection<Slot> slotCoord2ObservableCollection;
        private ObservableCollection<Slot> slotCoord3ObservableCollection;

        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        public ucSetLocationMap()
        {
            this.InitializeComponent();

            // TODO: Resource strings
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
                this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();
                this.SectionObservableCollection = new ObservableCollection<Section>();
                this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();

                this.LocMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            }

            this.ApplyEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        public ucSetLocationMap(int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            // TODO: Resource strings
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
                this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();
                this.SectionObservableCollection = new ObservableCollection<Section>();
                this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();
                this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();

                this.LocMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
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
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

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
                        if (this.dgWhs.SelectedItem != null && Convert.ToInt32(this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) < this.WarehouseObservableCollection.Count - 1)
                        {
                            this.dgWhs.SelectedItem = this.WarehouseObservableCollection[this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) + 1];
                            this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (this.CurrentPage.Title == this.sectionTab.Title)
                    {
                        if (this.dgSections.SelectedItem != null && Convert.ToInt32(this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) < this.SectionObservableCollection.Count - 1)
                        {
                            this.dgSections.SelectedItem = this.SectionObservableCollection[this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) + 1];
                            this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.CurrentPage.Title == this.warehouseTab.Title)
                    {
                        if (this.dgWhs.SelectedItem != null && Convert.ToInt32(this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse)) > 0)
                        {
                            this.dgWhs.SelectedItem = this.WarehouseObservableCollection[this.WarehouseObservableCollection.IndexOf(this.dgWhs.SelectedItem as Warehouse) - 1];
                            this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (this.CurrentPage.Title == this.sectionTab.Title)
                    {
                        if (this.dgSections.SelectedItem != null && Convert.ToInt32(this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section)) > 0)
                        {
                            this.dgSections.SelectedItem = this.SectionObservableCollection[this.SectionObservableCollection.IndexOf(this.dgSections.SelectedItem as Section) - 1];
                            this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
            });

            try
            {
                this.Loading = true;
                this.IsEnabled = false;
                await this.ucSetLocationMap_GotFocus(this, new EventArgs());
                await this.tabLocation_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine("this.ucSetLocationMap_GotFocus/tabLocation_GotFocus threw error " + ex);
            }
            finally
            {
                this.IsEnabled = true;
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ucSetLocationMap_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        private void ApplyEvents()
        {
            this.warehouseLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.sectionLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;
            this.slotLocationMapInfo.BtnSaveClick += this.BtnSave_Clicked;

            this.warehouseLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.sectionLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;
            this.slotLocationMapInfo.BtnCancelClick += this.BtnCancel_Clicked;

            this.CurrentPageChanged += UcSetLocationMap_CurrentPageChanged;
        }

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

        private bool LockWhsSelection
        {
            get
            {
                return this.lockWhsSelection;
            }

            set
            {
                this.lockWhsSelection = value;
            }
        }

        private int DefaultWhsLocMapRefno
        {
            get
            {
                return this.defaultWhsLocMapRefno;
            }

            set
            {
                this.defaultWhsLocMapRefno = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private SLocMap LocMapInfo
        {
            get
            {
                return this.locMapInfo;
            }

            set
            {
                this.locMapInfo = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private List<Slot> ListSlotCoord3
        {
            get
            {
                return this.listSlotCoord3;
            }

            set
            {
                this.listSlotCoord3 = value;
            }
        }

        private List<Slot> ListSlotCoord2
        {
            get
            {
                return this.listSlotCoord2;
            }

            set
            {
                this.listSlotCoord2 = value;
            }
        }

        private List<Slot> ListSlotCoord1
        {
            get
            {
                return this.listSlotCoord1;
            }

            set
            {
                this.listSlotCoord1 = value;
            }
        }

        private List<Section> ListSections
        {
            get
            {
                return this.listSections;
            }

            set
            {
                this.listSections = value;
            }
        }

        private List<Warehouse> ListWarehouse
        {
            get
            {
                return this.listWarehouse;
            }

            set
            {
                this.listWarehouse = value;
            }
        }

        private bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private bool IsBarCodeHandlerRegistered
        {
            get
            {
                return this.isBarCodeHandlerRegistered;
            }

            set
            {
                this.isBarCodeHandlerRegistered = value;
            }
        }

        private BarCodeScanner Scanner
        {
            get
            {
                return this.scanner;
            }

            set
            {
                this.scanner = value;
            }
        }

        private BarCodeParse Parse
        {
            get
            {
                return this.parse;
            }

            set
            {
                this.parse = value;
            }
        }

        public ObservableCollection<Warehouse> WarehouseObservableCollection { get => warehouseObservableCollection; set => warehouseObservableCollection = value; }
        public ObservableCollection<Section> SectionObservableCollection { get => sectionObservableCollection; set => sectionObservableCollection = value; }
        public ObservableCollection<Slot> SlotCoord1ObservableCollection { get => slotCoord1ObservableCollection; set => slotCoord1ObservableCollection = value; }
        public ObservableCollection<Slot> SlotCoord2ObservableCollection { get => slotCoord2ObservableCollection; set => slotCoord2ObservableCollection = value; }
        public ObservableCollection<Slot> SlotCoord3ObservableCollection { get => slotCoord3ObservableCollection; set => slotCoord3ObservableCollection = value; }
        public List<Warehouse> FilteredListWarehouse { get => filteredListWarehouse; set => filteredListWarehouse = value; }
        public List<Section> FilteredListSections { get => filteredListSections; set => filteredListSections = value; }
        public List<Slot> FilteredListSlotCoord1 { get => filteredListSlotCoord1; set => filteredListSlotCoord1 = value; }
        public List<Slot> FilteredListSlotCoord2 { get => filteredListSlotCoord2; set => filteredListSlotCoord2 = value; }
        public List<Slot> FilteredListSlotCoord3 { get => filteredListSlotCoord3; set => filteredListSlotCoord3 = value; }

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
                    await this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings            
            return "Select Location";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.LocMap.CurrLocMap = this.LocMapInfo;
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });

                saveComplete = false;
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });

                cancelChanges = false;
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<LocationMapRefno>" + LocationMapTasks.GetLocMapRefnoFromSLocMap() + "</LocationMapRefno>")
                + "</params>");

            return request;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucSetLocationMap_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    await this.ScannerEnable();
                    await this.LoadData();
                    await this.SelectWarehouse();

                    // Selecting the section and slot are required here even if the section / slot grids are not display 
                    // because we want to set the grid(s).CurrentRowIdx
                    if (this.LocMap.CurrLocMap.IsSectionTracked)
                    {
                        await this.SelectSection();
                        await this.SelectSlotCoordinate();
                    }

                    this.IsControlLoaded = true;
                    await this.UpdateLocationDisplay();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void txtSearchSection_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // By Code
                if (this.cboSectionSearch.SelectedIndex == 0)
                {
                    if (string.Compare(txtSearchSection.Text, string.Empty) > 0)
                    {
                        //this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
                        this.FilteredListSections = this.ListSections.Where<Section>(section => Convert.ToString(section.LocationMapSectionCode) == this.txtSearchSection.Text.Trim()).ToList();
                    }
                    else
                    {
                        //this.DataViewSections.RowFilter = string.Empty;
                        this.FilteredListSections = this.ListSections;
                    }
                }
                else
                {
                    //this.DataViewSections.RowFilter = "LocationMapSectionName LIKE \'" + this.txtSearchSection.Text.Trim() + "%\'";
                    this.FilteredListSections = this.ListSections.Where<Section>(section => section.LocationMapSectionName.Contains(this.txtSearchSection.Text.Trim())).ToList();
                }
            }
            // Setting this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
            // when txt is not numeric blows up. Handle here
            catch (EvaluateException ex)
            {
                Console.WriteLine(ex);
                //this.DataViewSections.RowFilter = "LocationMapSectionCode = -1";
                this.FilteredListSections = this.ListSections.Where<Section>(section => Convert.ToString(section.LocationMapSectionCode) == Convert.ToString(-1)).ToList();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while searching." + ex.Message, "OK"));
                this.FilteredListSections = this.ListSections;
            }
            finally
            {
                await this.PopulateSectionList();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void txtSlotCoordinate1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.Compare(txtSlotCoordinate1.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate1.Text.Trim().ToUpper())
                        && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate1.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    } 
                    else
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1;
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while searching." + ex.Message, "OK"));
                this.filteredListSlotCoord1 = this.ListSlotCoord1;
            }
            finally
            {
                await this.PopulateSlotCoord1List();
            }
        }

        private async Task PopulateSlotCoord1List()
        {
            this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord1)
            {
                this.SlotCoord1ObservableCollection.Add(slot);
            }

            await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.ItemsSource = this.SlotCoord1ObservableCollection);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void txtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.Compare(txtSlotCoordinate2.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate2.Text.Trim().ToUpper())
                        && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate2.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2;
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while searching." + ex.Message, "OK"));
                this.filteredListSlotCoord2 = this.ListSlotCoord2;
            }
            finally
            {
                await this.PopulateSlotCoord2List();
            }
        }

        private async Task PopulateSlotCoord2List()
        {
            this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord2)
            {
                this.SlotCoord2ObservableCollection.Add(slot);
            }

            await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.ItemsSource = this.SlotCoord2ObservableCollection);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void txtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.Compare(txtSlotCoordinate3.Text, string.Empty) > 0)
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate3.Text.Trim().ToUpper())
                        && slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate3.Text.Trim().ToUpper())).ToList();
                    }
                }
                else
                {
                    if (this.LocMapInfo.SectLocMapRefno != 0)
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                    }
                    else
                    {
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3;
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while searching." + ex.Message, "OK"));
                this.filteredListSlotCoord3 = this.ListSlotCoord3;
            }
            finally
            {
                await this.PopulateSlotCoord3List();
            }
        }

        private async Task PopulateSlotCoord3List()
        {
            this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord3)
            {
                this.SlotCoord3ObservableCollection.Add(slot);
            }

            await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.ItemsSource = this.SlotCoord3ObservableCollection);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task tabLocation_GotFocus(object sender, EventArgs e)
        {
            try
            {
                if (this.FilteredListWarehouse == null || this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Select the warehouse first!", "OK"));
                }
                else
                {
                    if (this.FilteredListSections != null)
                    {
                        this.FilteredListSections = this.ListSections.Where<Section>(section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();

                        await this.SelectSection();

                        if (this.FilteredListSections.Count > 0)
                        {
                            if (this.dgSections.SelectedItem is Section selectedSection && this.LocMapInfo.SectLocMapRefno != selectedSection.LocationMapRefno)
                            {
                                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(selectedSection.LocationMapRefno);
                            }
                        }
                    }
                }

                if (this.FilteredListSections == null || this.LocMapInfo.SectLocMapRefno == 0)
                {
                    //await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Select the section first!", "OK"));
                }
                else
                {
                    if (this.FilteredListSlotCoord1 != null && this.FilteredListSlotCoord2 != null && this.FilteredListSlotCoord3 != null)
                    {
                        this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
                        this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();

                        await this.SelectSlotCoordinate();

                        if (this.FilteredListSlotCoord1.Count > 0)
                        {
                            int slotLocMapRefno = 0;

                            if (this.dgCoord1.SelectedItem is Slot slotCoord1 && this.dgCoord2.SelectedItem is Slot slotCoord2 && this.dgCoord3.SelectedItem is Slot slotCoord3)
                            {
                                slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slotCoord1.LocationMapSlotCoordinateRefno, slotCoord2.LocationMapSlotCoordinateRefno, slotCoord3.LocationMapSlotCoordinateRefno);
                            }

                            if (slotLocMapRefno != 0)
                            {
                                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                            }
                        }
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            await this.UpdateLocationDisplay();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucSetLocationMap_LostFocus(object sender, EventArgs e)
        {
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadWhsList()
        {
            this.ListWarehouse = await LocationMapTasks.GetWhsList();
            this.FilteredListWarehouse = this.ListWarehouse;
            await this.PopulateWarehouseList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadSections()
        {
            this.ListSections = await LocationMapTasks.GetSectList();
            this.FilteredListSections = this.ListSections.Where(section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();
            await this.PopulateSectionList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadSlots()
        {
            this.ListSlotCoord1 = await LocationMapTasks.GetSlotCoord1List();
            this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord1List();

            this.ListSlotCoord2 = await LocationMapTasks.GetSlotCoord2List();
            this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord2List();

            this.ListSlotCoord3 = await LocationMapTasks.GetSlotCoord3List();
            this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            await this.PopulateSlotCoord3List();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            try
            {
                // Default to the login warehouse
                int locMapRefno;

                if (0 < this.DefaultWhsLocMapRefno)
                {
                    // If the default warehouse match the current
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno == this.DefaultWhsLocMapRefno)
                    {
                        locMapRefno = this.LocMap.CurrLocMapRefno;
                    }
                    else
                    {
                        locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.DefaultWhsLocMapRefno, 0, SysCommon.ProdLocMapTypes.Picking);
                    }
                }
                else if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.LoginWhsLocMapRefno, 0, SysCommon.ProdLocMapTypes.Picking);
                }
                else
                {
                    locMapRefno = this.LocMap.CurrLocMapRefno;
                }

                this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                // Load the dataviews and apply row filters base on the current sLocationMapInfo values
                await this.LoadWhsList();
                await this.LoadSections();
                await this.LoadSlots();

                await this.SelectWarehouse();
                await this.SelectSection();
                await this.SelectSlotCoordinate();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while loading data." + ex.Message, "OK"));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task SelectWarehouse()
        {
            // Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                Warehouse selectedWarehouse = this.WarehouseObservableCollection.Where<Warehouse>(warehouse => warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno).FirstOrDefault();

                if (selectedWarehouse != null)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dgWhs.SelectedItem = selectedWarehouse);
                }
                else if (this.WarehouseObservableCollection.Count > 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0]);
                }
            }
            else
            {
                if (this.WarehouseObservableCollection.Count > 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0]);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task UpdateLocationDisplay()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    // TODO: Resource strings
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
                    // TODO: Resource strings
                    //this.lblSectionData.Text = "Unknown";
                    //this.lblSectionData.TextColor = Color.Black;

                    this.warehouseLocationMapInfo.SetSectionDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetSectionDataText("Unknown");
                    this.sectionLocationMapInfo.SetSectionDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetSectionDataText("Unknown");
                    this.slotLocationMapInfo.SetSectionDataTextColor(Color.Black);
                }
                else
                {
                    //this.lblSectionData.Text = this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName;
                    //this.lblSectionData.TextColor = Color.Blue;

                    this.warehouseLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.warehouseLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.sectionLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.sectionLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                    this.slotLocationMapInfo.SetSectionDataText(this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName);
                    this.slotLocationMapInfo.SetSectionDataTextColor(Color.Blue);
                }

                if (this.LocMapInfo.SlotLocMapRefno == 0)
                {
                    // TODO: Resource strings
                    //this.lblSlotData.Text = "Unknown";
                    //this.lblSlotData.TextColor = Color.Black;

                    this.warehouseLocationMapInfo.SetSlotDataText("Unknown");
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetSlotDataText("Unknown");
                    this.sectionLocationMapInfo.SetSlotDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetSlotDataText("Unknown");
                    this.slotLocationMapInfo.SetSlotDataTextColor(Color.Black);
                }
                else
                {
                    //this.lblSlotData.Text = this.LocMapInfo.SlotName;
                    //this.lblSlotData.TextColor = Color.Blue;

                    this.warehouseLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.warehouseLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.sectionLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.sectionLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                    this.slotLocationMapInfo.SetSlotDataText(this.LocMapInfo.SlotName);
                    this.slotLocationMapInfo.SetSlotDataTextColor(Color.Blue);
                }

                //if (this.tabLocation.SelectedIndex == 1)
                //{
                if (this.CurrentPage.TabIndex == 1)
                {
                    this.cboSectionSearch.SelectedIndex = 0;
                    //this.cboSectionSearch.Text = "Description";
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
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task SelectSection()
        {
            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                // attempt to find last selected section in section view
                if (this.LocMapInfo.SectLocMapRefno != 0)
                {
                    Section section = this.SectionObservableCollection.Where<Section>(section => section.LocationMapRefno == this.LocMapInfo.SectLocMapRefno).FirstOrDefault();

                    if (section != null)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgSections.SelectedItem = section);
                    }
                    else if (this.SectionObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgSections.SelectedItem = this.SectionObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SectionObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgSections.SelectedItem = this.SectionObservableCollection[0]);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task SelectSlotCoordinate()
        {
            if (this.LocMap.CurrLocMap.IsSlotTracked)
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
                    Slot slotCoord1 = this.SlotCoord1ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord1 != null)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.SelectedItem = slotCoord1);
                    }
                    else if (this.SlotCoord1ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord1ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0]);
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    Slot slotCoord2 = this.SlotCoord2ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord2 != null)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.SelectedItem = slotCoord2);
                    }
                    else if (this.SlotCoord2ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord2ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0]);
                    }
                }

                // Attempt to find last selected slot coordinate in slot coordinate 3 view
                if (0 < this.LocMapInfo.SlotCoord3LocMapRefno)
                {
                    Slot slotCoord3 = this.SlotCoord3ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord3 != null)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.SelectedItem = slotCoord3);
                    }
                    else if (this.SlotCoord3ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0]);
                    }
                }
                else
                {
                    if (this.SlotCoord3ObservableCollection.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0]);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            try
            {
                this.Loading = true;

                // No need to disable/enable scanner because only messagebox are called. 
                // GoFocus and LostFocus events will handle the scanner
                this.Parse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an header reference # is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an item reference # is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a product bar code is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    int locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.Parse.LocationIdentifier);
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                    await this.SelectWarehouse();
                    await this.SelectSection();
                    await this.SelectSlotCoordinate();
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here!", "OK"));
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while processing barcode." + ex.Message, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        #endregion

        private async void dgWhs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (this.isControlLoaded)
                {
                    this.IsChangesMade = true;

                    if (this.IsControlLoaded && e.SelectedItem is Warehouse warehouse)
                    {
                        this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
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
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while selecting a warehouse." + ex.Message, "OK"));
            }
        }

        private async void dgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                this.IsChangesMade = true;

                if (this.IsControlLoaded && e.SelectedItem is Section section)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));
                    this.dgCoord1.SelectedItem = null;
                    this.dgCoord2.SelectedItem = null;
                    this.dgCoord3.SelectedItem = null;
                    await this.LoadSlots();
                    await this.UpdateLocationDisplay();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while selecting a section." + ex.Message, "OK"));
            }
        }

        private async void dgCoord1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (e.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 && this.dgCoord3.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                    }

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    await this.UpdateLocationDisplay();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while selecting slot coordinate 1." + ex.Message, "OK"));
            }
        }

        private async void dgCoord2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (this.dgCoord1.SelectedItem is Slot slot1 && e.SelectedItem is Slot slot2 && this.dgCoord3.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                    }

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    await this.UpdateLocationDisplay();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while selecting slot coordinate 2." + ex.Message, "OK"));
            }
        }

        private async void dgCoord3_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    int slotLocMapRefno = 0;

                    if (this.dgCoord1.SelectedItem is Slot slot1 && this.dgCoord2.SelectedItem is Slot slot2 && e.SelectedItem is Slot slot3)
                    {
                        slotLocMapRefno = await LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                    }

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    await this.UpdateLocationDisplay();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "An error occurred while selecting slot coordinate 3." + ex.Message, "OK"));
            }
        }

        private async Task PopulateWarehouseList()
        {
            this.WarehouseObservableCollection = new ObservableCollection<Warehouse>();

            foreach (Warehouse warehouse in this.FilteredListWarehouse)
            {
                //Warehouse warehouse = new Warehouse
                //{
                //    LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                //    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"]),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"])
                //};

                this.WarehouseObservableCollection.Add(warehouse);
            }

            await Device.InvokeOnMainThreadAsync(() => this.dgWhs.ItemsSource = this.WarehouseObservableCollection);
        }

        private async Task PopulateSectionList()
        {
            this.SectionObservableCollection = new ObservableCollection<Section>();

            foreach (Section section in this.FilteredListSections)
            {
                //Section section = new Section
                //{
                //    LocationMapSectionCode = Convert.ToInt32(row["LocationMapSectionCode"]),
                //    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                //    WarehouseLocationMapRefno = Convert.ToInt32(row["WarehouseLocationMapRefno"])
                //};

                this.SectionObservableCollection.Add(section);
            }

            await Device.InvokeOnMainThreadAsync(() => this.dgSections.ItemsSource = this.SectionObservableCollection);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

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