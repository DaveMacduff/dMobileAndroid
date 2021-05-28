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
    public partial class ucSetLocationMap : DMobileTabbedModal, IAppArea, IAppPublisher
    {
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SelectLocation;
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private LocationMap locMap;
        private BarCodeParse parse = null;
        private BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SLocMap locMapInfo;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
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

                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
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
                await this.ucSetLocationMap_GotFocus(this, new EventArgs());
                await this.tabLocation_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine("this.ucSetLocationMap_GotFocus/tabLocation_GotFocus threw error " + ex);
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

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        public event ReqCloseHandler ReqClose
        {
            add
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(this.ReqCloseEvent, value);
            }

            remove
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(this.ReqCloseEvent, value);
            }
        }

        public event ReqSaveEnableHandler ReqSaveEnable
        {
            add
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Combine(this.ReqSaveEnableEvent, value);
            }

            remove
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Remove(this.ReqSaveEnableEvent, value);
            }
        }

        public event ReqSaveDisableHandler ReqSaveDisable
        {
            add
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Combine(this.ReqSaveDisableEvent, value);
            }

            remove
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Remove(this.ReqSaveDisableEvent, value);
            }
        }

        public event ReqCancelEnableHandler ReqCancelEnable
        {
            add
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Combine(this.ReqCancelEnableEvent, value);
            }

            remove
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Remove(this.ReqCancelEnableEvent, value);
            }
        }

        public event ReqCancelDisableHandler ReqCancelDisable
        {
            add
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Combine(this.ReqCancelDisableEvent, value);
            }

            remove
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Remove(this.ReqCancelDisableEvent, value);
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

        private ReqCloseHandler ReqCloseEvent
        {
            get
            {
                return this.reqCloseEvent;
            }

            set
            {
                this.reqCloseEvent = value;
            }
        }

        private ReqSaveEnableHandler ReqSaveEnableEvent
        {
            get
            {
                return this.reqSaveEnableEvent;
            }

            set
            {
                this.reqSaveEnableEvent = value;
            }
        }

        private ReqSaveDisableHandler ReqSaveDisableEvent
        {
            get
            {
                return this.reqSaveDisableEvent;
            }

            set
            {
                this.reqSaveDisableEvent = value;
            }
        }

        private ReqCancelEnableHandler ReqCancelEnableEvent
        {
            get
            {
                return this.reqCancelEnableEvent;
            }

            set
            {
                this.reqCancelEnableEvent = value;
            }
        }

        private ReqCancelDisableHandler ReqCancelDisableEvent
        {
            get
            {
                return this.reqCancelDisableEvent;
            }

            set
            {
                this.reqCancelDisableEvent = value;
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

        #region IAppPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
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
                    this.SelectWarehouse();

                    // Selecting the section and slot are required here even if the section / slot grids are not display 
                    // because we want to set the grid(s).CurrentRowIdx
                    if (this.LocMap.CurrLocMap.IsSectionTracked)
                    {
                        this.SelectSection();
                        this.SelectSlotCoordinate();
                    }

                    this.IsControlLoaded = true;
                    await this.UpdateLocationDisplay();

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchSection_TextChanged(object sender, TextChangedEventArgs e)
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
                Console.WriteLine(ex);
                this.FilteredListSections = this.ListSections;
            }
            finally
            {
                this.PopulateSectionList();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate1_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate1.Text.Trim())).ToList();
            this.PopulateSlotCoord1List();
        }

        private void PopulateSlotCoord1List()
        {
            this.SlotCoord1ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord1)
            {
                //Slot slot = new Slot
                //{
                //    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                //    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                //    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                //};

                this.SlotCoord1ObservableCollection.Add(slot);
            }

            this.dgCoord1.ItemsSource = this.SlotCoord1ObservableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.DataViewSlotCoord2.RowFilter = "LocationMapSlotCoordinateCode LIKE \'%" + this.txtSlotCoordinate2.Text.Trim() + "%\'";
            this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate1.Text.Trim())).ToList();
            this.PopulateSlotCoord2List();
        }

        private void PopulateSlotCoord2List()
        {
            this.SlotCoord2ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord2)
            {
                //Slot slot = new Slot
                //{
                //    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                //    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                //    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                //};

                this.SlotCoord2ObservableCollection.Add(slot);
            }

            this.dgCoord2.ItemsSource = this.SlotCoord2ObservableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.DataViewSlotCoord3.RowFilter = "LocationMapSlotCoordinateCode LIKE \'%" + this.txtSlotCoordinate3.Text.Trim() + "%\'";
            this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.LocationMapSlotCoordinateCode.Contains(this.txtSlotCoordinate1.Text.Trim())).ToList();
            this.PopulateSlotCoord3List();
        }

        private void PopulateSlotCoord3List()
        {
            this.SlotCoord3ObservableCollection = new ObservableCollection<Slot>();

            foreach (Slot slot in this.FilteredListSlotCoord3)
            {
                //Slot slot = new Slot
                //{
                //    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                //    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                //    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                //};

                this.SlotCoord3ObservableCollection.Add(slot);
            }

            this.dgCoord3.ItemsSource = this.SlotCoord3ObservableCollection;
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

                        this.SelectSection();

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

                        this.SelectSlotCoordinate();

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
            this.PopulateWarehouseList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadSections()
        {
            this.ListSections = await LocationMapTasks.GetSectList();
            this.FilteredListSections = this.ListSections.Where(section => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno).ToList();
            this.PopulateSectionList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadSlots()
        {
            this.ListSlotCoord1 = await LocationMapTasks.GetSlotCoord1List();
            this.FilteredListSlotCoord1 = this.ListSlotCoord1.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            this.PopulateSlotCoord1List();

            this.ListSlotCoord2 = await LocationMapTasks.GetSlotCoord2List();
            this.FilteredListSlotCoord2 = this.ListSlotCoord2.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            this.PopulateSlotCoord2List();

            this.ListSlotCoord3 = await LocationMapTasks.GetSlotCoord3List();
            this.FilteredListSlotCoord3 = this.ListSlotCoord3.Where<Slot>(slot => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno).ToList();
            this.PopulateSlotCoord3List();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
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

            this.SelectWarehouse();
            this.SelectSection();
            this.SelectSlotCoordinate();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void SelectWarehouse()
        {
            // Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                Warehouse selectedWarehouse = this.WarehouseObservableCollection.Where<Warehouse>(warehouse => warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno).FirstOrDefault();

                if (selectedWarehouse != null)
                {
                    this.dgWhs.SelectedItem = selectedWarehouse;
                }
            }
            else
            {
                if (this.WarehouseObservableCollection.Count > 0)
                {
                    this.dgWhs.SelectedItem = this.WarehouseObservableCollection[0];
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
                //}

                //if (this.tabLocation.SelectedIndex == 2)
                //{
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
        private void SelectSection()
        {
            if (this.LocMap.CurrLocMap.IsSectionTracked)
            {
                // attempt to find last selected section in section view
                if (this.LocMapInfo.SectLocMapRefno != 0)
                {
                    Section section = this.SectionObservableCollection.Where<Section>(section => section.LocationMapRefno == this.LocMapInfo.SectLocMapRefno).FirstOrDefault();

                    if (section != null)
                    {
                        this.dgSections.SelectedItem = section;
                    }
                }
                else
                {
                    if (this.SectionObservableCollection.Count > 0)
                    {
                        this.dgSections.SelectedItem = this.SectionObservableCollection[0];
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void SelectSlotCoordinate()
        {
            if (this.LocMap.CurrLocMap.IsSlotTracked)
            {
                if (this.FilteredListSlotCoord1.Count > this.SlotCoord1ObservableCollection.Count)
                {
                    this.PopulateSlotCoord1List();
                }

                if (this.FilteredListSlotCoord2.Count > this.SlotCoord2ObservableCollection.Count)
                {
                    this.PopulateSlotCoord2List();
                }

                if (this.FilteredListSlotCoord3.Count > this.SlotCoord3ObservableCollection.Count)
                {
                    this.PopulateSlotCoord3List();
                }

                // Attempt to find last selected slot coordinate in slot coordinate 1 view
                if (this.LocMapInfo.SlotCoord1LocMapRefno != 0)
                {
                    Slot slotCoord1 = this.SlotCoord1ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord1 != null)
                    {
                        this.dgCoord1.SelectedItem = slotCoord1;
                    }
                }
                else
                {
                    if (this.SlotCoord1ObservableCollection.Count > 0)
                    {
                        this.dgCoord1.SelectedItem = this.SlotCoord1ObservableCollection[0];
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    Slot slotCoord2 = this.SlotCoord2ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord2 != null)
                    {
                        this.dgCoord2.SelectedItem = slotCoord2;
                    }
                }
                else
                {
                    if (this.SlotCoord2ObservableCollection.Count > 0)
                    {
                        this.dgCoord2.SelectedItem = this.SlotCoord2ObservableCollection[0];
                    }
                }

                // Attempt to find last selected slot coordinate in slot coordinate 3 view
                if (0 < this.LocMapInfo.SlotCoord3LocMapRefno)
                {
                    Slot slotCoord3 = this.SlotCoord3ObservableCollection.Where(slot => slot.LocationMapSlotCoordinateRefno == this.LocMapInfo.SlotCoord1LocMapRefno).FirstOrDefault();

                    if (slotCoord3 != null)
                    {
                        this.dgCoord3.SelectedItem = slotCoord3;
                    }
                }
                else
                {
                    if (this.SlotCoord3ObservableCollection.Count > 0)
                    {
                        this.dgCoord3.SelectedItem = this.SlotCoord3ObservableCollection[0];
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
                this.SelectWarehouse();
                this.SelectSection();
                this.SelectSlotCoordinate();
            }
            else
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here!", "OK"));
            }
        }

        #endregion

        private async void dgWhs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.isControlLoaded)
            {
                this.IsChangesMade = true;

                if (this.IsControlLoaded && e.SelectedItem is Warehouse warehouse)
                {
                    this.LocMapInfo = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
                    this.dgSections.SelectedItem = null;
                    this.dgCoord1.SelectedItem = null;
                    this.dgCoord2.SelectedItem = null;
                    this.dgCoord3.SelectedItem = null;
                    await this.LoadSections();
                    await this.UpdateLocationDisplay();
                }
            }
        }

        private async void dgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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

        private async void dgCoord1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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

        private async void dgCoord2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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

        private async void dgCoord3_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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

        private void PopulateWarehouseList()
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

            this.dgWhs.ItemsSource = this.WarehouseObservableCollection;
        }

        private void PopulateSectionList()
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

            this.dgSections.ItemsSource = this.SectionObservableCollection;
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