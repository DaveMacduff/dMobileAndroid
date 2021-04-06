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
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Color = Xamarin.Forms.Color;

    public partial class ucSetLocationMap : DMobileTabbedModal, IAppArea, IAppPublisher
    {
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SelectLocation;
        //private UserControl callingControl = null;
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

        private DataView dataViewWhs;
        private DataView dataViewSections;
        private DataView dataViewSlotCoord1;
        private DataView dataViewSlotCoord2;
        private DataView dataViewSlotCoord3;

        private ObservableCollection<Warehouse> warehouseList;
        private ObservableCollection<Section> sectionList;
        private ObservableCollection<Slot> slotCoord1List;
        private ObservableCollection<Slot> slotCoord2List;
        private ObservableCollection<Slot> slotCoord3List;

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
                this.WarehouseList = new ObservableCollection<Warehouse>();
                this.SectionList = new ObservableCollection<Section>();
                this.SlotCoord1List = new ObservableCollection<Slot>();
                this.SlotCoord2List = new ObservableCollection<Slot>();
                this.SlotCoord3List = new ObservableCollection<Slot>();

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
                this.WarehouseList = new ObservableCollection<Warehouse>();
                this.SectionList = new ObservableCollection<Section>();
                this.SlotCoord1List = new ObservableCollection<Slot>();
                this.SlotCoord2List = new ObservableCollection<Slot>();
                this.SlotCoord3List = new ObservableCollection<Slot>();

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
                        if (this.dgWhs.SelectedItem != null && Convert.ToInt32(this.WarehouseList.IndexOf(this.dgWhs.SelectedItem as Warehouse)) < this.WarehouseList.Count - 1)
                        {
                            this.dgWhs.SelectedItem = this.WarehouseList[this.WarehouseList.IndexOf(this.dgWhs.SelectedItem as Warehouse) + 1];
                            this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if(this.CurrentPage.Title == this.sectionTab.Title)
                    {
                        if (this.dgSections.SelectedItem != null && Convert.ToInt32(this.SectionList.IndexOf(this.dgSections.SelectedItem as Section)) < this.SectionList.Count - 1)
                        {
                            this.dgSections.SelectedItem = this.SectionList[this.SectionList.IndexOf(this.dgSections.SelectedItem as Section) + 1];
                            this.dgSections.ScrollTo(this.dgSections.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.CurrentPage.Title == this.warehouseTab.Title)
                    {
                        if (this.dgWhs.SelectedItem != null && Convert.ToInt32(this.WarehouseList.IndexOf(this.dgWhs.SelectedItem as Warehouse)) > 0)
                        {
                            this.dgWhs.SelectedItem = this.WarehouseList[this.WarehouseList.IndexOf(this.dgWhs.SelectedItem as Warehouse) - 1];
                            this.dgWhs.ScrollTo(this.dgWhs.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (this.CurrentPage.Title == this.sectionTab.Title)
                    {
                        if (this.dgSections.SelectedItem != null && Convert.ToInt32(this.SectionList.IndexOf(this.dgSections.SelectedItem as Section)) > 0)
                        {
                            this.dgSections.SelectedItem = this.SectionList[this.SectionList.IndexOf(this.dgSections.SelectedItem as Section) - 1];
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
                if (this.dgSections.SelectedItem == null && this.SectionList.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionList[0];
                }
            }
            else if (this.CurrentPage.Title == this.slotTab.Title)
            {
                if (this.dgSections.SelectedItem == null && this.SectionList.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionList[0];
                }

                if (this.dgCoord1.SelectedItem == null && this.SlotCoord1List.Count > 0)
                {
                    this.dgCoord1.SelectedItem = this.SlotCoord1List[0];
                }

                if (this.dgCoord2.SelectedItem == null && this.SlotCoord2List.Count > 0)
                {
                    this.dgCoord2.SelectedItem = this.SlotCoord2List[0];
                }

                if (this.dgCoord3.SelectedItem == null && this.SlotCoord3List.Count > 0)
                {
                    this.dgCoord3.SelectedItem = this.SlotCoord3List[0];
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

        private DataView DataViewSlotCoord3
        {
            get
            {
                return this.dataViewSlotCoord3;
            }

            set
            {
                this.dataViewSlotCoord3 = value;
            }
        }

        private DataView DataViewSlotCoord2
        {
            get
            {
                return this.dataViewSlotCoord2;
            }

            set
            {
                this.dataViewSlotCoord2 = value;
            }
        }

        private DataView DataViewSlotCoord1
        {
            get
            {
                return this.dataViewSlotCoord1;
            }

            set
            {
                this.dataViewSlotCoord1 = value;
            }
        }

        private DataView DataViewSections
        {
            get
            {
                return this.dataViewSections;
            }

            set
            {
                this.dataViewSections = value;
            }
        }

        private DataView DataViewWhs
        {
            get
            {
                return this.dataViewWhs;
            }

            set
            {
                this.dataViewWhs = value;
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

        //private UserControl CallingControl
        //{
        //    get
        //    {
        //        return this.callingControl;
        //    }

        //    set
        //    {
        //        this.callingControl = value;
        //    }
        //}

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

        public ObservableCollection<Warehouse> WarehouseList { get => warehouseList; set => warehouseList = value; }
        public ObservableCollection<Section> SectionList { get => sectionList; set => sectionList = value; }
        public ObservableCollection<Slot> SlotCoord1List { get => slotCoord1List; set => slotCoord1List = value; }
        public ObservableCollection<Slot> SlotCoord2List { get => slotCoord2List; set => slotCoord2List = value; }
        public ObservableCollection<Slot> SlotCoord3List { get => slotCoord3List; set => slotCoord3List = value; }

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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

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
                    this.LoadData();
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
        public XDocument GetXML()
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
                    this.LoadData();
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
            // Did not consider selected data changed
            ////this.IsChangesMade = true;

            try
            {
                // By Code
                if (this.cboSectionSearch.SelectedIndex == 0)
                {
                    if (string.Compare(txtSearchSection.Text, string.Empty) > 0)
                    {
                        this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
                    }
                    else
                    {
                        this.DataViewSections.RowFilter = string.Empty;
                    }
                }
                else
                {
                    this.DataViewSections.RowFilter = "LocationMapSectionName LIKE \'" + this.txtSearchSection.Text.Trim() + "%\'";
                }
            }
            // Setting this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
            // when txt is not numeric blows up. Handle here
            catch (EvaluateException ex)
            {
                Console.WriteLine(ex);
                this.DataViewSections.RowFilter = "LocationMapSectionCode = -1";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.DataViewSections.RowFilter = string.Empty;
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
            // Did not consider selected data changed
            ////this.IsChangesMade = true;
            this.DataViewSlotCoord1.RowFilter = "LocationMapSlotCoordinateCode LIKE \'%" + this.txtSlotCoordinate1.Text.Trim() + "%\'";
            this.PopulateSlotCoord1List();
        }

        private void PopulateSlotCoord1List()
        {
            this.SlotCoord1List = new ObservableCollection<Slot>();

            foreach (DataRow row in this.DataViewSlotCoord1.ToTable().Rows)
            {
                Slot slot = new Slot
                {
                    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                };

                this.SlotCoord1List.Add(slot);
            }

            this.dgCoord1.ItemsSource = this.SlotCoord1List;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Did not consider selected data changed
            ////this.IsChangesMade = true;
            this.DataViewSlotCoord2.RowFilter = "LocationMapSlotCoordinateCode LIKE \'%" + this.txtSlotCoordinate2.Text.Trim() + "%\'";
            this.PopulateSlotCoord2List();
        }

        private void PopulateSlotCoord2List()
        {
            this.SlotCoord2List = new ObservableCollection<Slot>();

            foreach (DataRow row in this.DataViewSlotCoord2.ToTable().Rows)
            {
                Slot slot = new Slot
                {
                    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                };

                this.SlotCoord2List.Add(slot);
            }

            this.dgCoord2.ItemsSource = this.SlotCoord2List;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Did not consider selected data changed
            ////this.IsChangesMade = true;
            this.DataViewSlotCoord3.RowFilter = "LocationMapSlotCoordinateCode LIKE \'%" + this.txtSlotCoordinate3.Text.Trim() + "%\'";
            this.PopulateSlotCoord3List();
        }

        private void PopulateSlotCoord3List()
        {
            this.SlotCoord3List = new ObservableCollection<Slot>();

            foreach (DataRow row in this.DataViewSlotCoord3.ToTable().Rows)
            {
                Slot slot = new Slot
                {
                    LocationMapSlotCoordinateCode = Convert.ToString(row["LocationMapSlotCoordinateCode"]),
                    LocationMapSlotCoordinateRefno = Convert.ToInt32(row["LocationMapSlotCoordinateRefno"]),
                    SectionLocationMapRefno = Convert.ToInt32(row["SectionLocationMapRefno"])
                };

                this.SlotCoord3List.Add(slot);
            }

            this.dgCoord3.ItemsSource = this.SlotCoord3List;
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
                if (this.DataViewWhs == null || this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Select the warehouse first!", "OK"));
                }
                else
                {
                    if (this.DataViewSections != null)
                    {
                        this.DataViewSections.RowFilter = "WarehouseLocationMapRefno = " + this.LocMapInfo.WhsLocMapRefno;

                        this.SelectSection();

                        if (this.DataViewSections.Count > 0)
                        {
                            if (this.dgSections.SelectedItem is Section selectedSection && this.LocMapInfo.SectLocMapRefno != selectedSection.LocationMapRefno)
                            {
                                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(selectedSection.LocationMapRefno);
                            }
                        }
                    }
                }

                if (this.DataViewSections == null || this.LocMapInfo.SectLocMapRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Select the section first!", "OK"));
                }
                else
                {
                    if (this.DataViewSlotCoord1 != null && this.DataViewSlotCoord2 != null && this.DataViewSlotCoord3 != null)
                    {
                        this.DataViewSlotCoord1.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
                        this.DataViewSlotCoord2.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
                        this.DataViewSlotCoord3.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;

                        this.SelectSlotCoordinate();

                        if (this.DataViewSlotCoord1.Count > 0)
                        {
                            int slotLocMapRefno = 0;

                            if (this.dgCoord1.SelectedItem is Slot slotCoord1 && this.dgCoord2.SelectedItem is Slot slotCoord2 && this.dgCoord3.SelectedItem is Slot slotCoord3)
                            {
                                slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slotCoord1.LocationMapSlotCoordinateRefno, slotCoord2.LocationMapSlotCoordinateRefno, slotCoord3.LocationMapSlotCoordinateRefno);
                            }

                            if (slotLocMapRefno != 0)
                            {
                                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
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
        private void LoadWhsList()
        {
            DataTable dataTableWarehouses = new DataTable();
            LocationMapTasks.GetWhsList(ref dataTableWarehouses);
            dataTableWarehouses.TableName = "WhsList";
            this.DataViewWhs = dataTableWarehouses.DefaultView;
            //this.dgWhs.DataSource = this.DataViewWhs;
            this.PopulateWarehouseList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadSections()
        {
            DataTable dataTableSections = new DataTable();
            LocationMapTasks.GetSectList(ref dataTableSections);
            dataTableSections.TableName = "Sections";
            this.DataViewSections = dataTableSections.DefaultView;
            this.DataViewSections.RowFilter = "WarehouseLocationMapRefno = " + this.LocMapInfo.WhsLocMapRefno;
            //this.dgSections.DataSource = this.DataViewSections;
            this.PopulateSectionList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadSlots()
        {
            DataTable dataTableSlots = new DataTable();

            LocationMapTasks.GetSlotCoord1List(ref dataTableSlots);
            dataTableSlots.TableName = "SlotCoord1";
            this.DataViewSlotCoord1 = dataTableSlots.DefaultView;
            this.DataViewSlotCoord1.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
            this.PopulateSlotCoord1List();

            LocationMapTasks.GetSlotCoord2List(ref dataTableSlots);
            dataTableSlots.TableName = "SlotCoord2";
            this.DataViewSlotCoord2 = dataTableSlots.DefaultView;
            this.DataViewSlotCoord2.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
            this.PopulateSlotCoord2List();

            LocationMapTasks.GetSlotCoord3List(ref dataTableSlots);
            dataTableSlots.TableName = "SlotCoord3";
            this.DataViewSlotCoord3 = dataTableSlots.DefaultView;
            this.DataViewSlotCoord3.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
            this.PopulateSlotCoord3List();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
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
                    locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.DefaultWhsLocMapRefno, 0, SysCommon.ProdLocMapTypes.Picking);
                }
            }
            else if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
            {
                locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.LoginWhsLocMapRefno, 0, SysCommon.ProdLocMapTypes.Picking);
            }
            else
            {
                locMapRefno = this.LocMap.CurrLocMapRefno;
            }

            this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            // Load the dataviews and apply row filters base on the current sLocationMapInfo values
            this.LoadWhsList();
            this.LoadSections();
            this.LoadSlots();

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
                for (int rowIdx = 0; rowIdx < this.DataViewWhs.Count; rowIdx++)
                {
                    if (Convert.ToInt32(this.DataViewWhs[rowIdx]["LocationMapRefno"]) == this.LocMapInfo.WhsLocMapRefno)
                    {
                        //this.dgWhs.CurrentRowIndex = rowIdx;
                        //this.dgWhs.Select(rowIdx);

                        if (this.WarehouseList.Count > rowIdx)
                        {
                            this.dgWhs.SelectedItem = this.WarehouseList[rowIdx];
                        }

                        break;
                    }
                }
            }
            else
            {
                //dgWhs.CurrentRowIndex = 0;
                //dgWhs.Select(0);

                if (this.WarehouseList.Count > 0)
                {
                    this.dgWhs.SelectedItem = this.WarehouseList[0];
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
                    //this.lblWarehouseData.Text = "Unknown";
                    //this.lblWarehouseData.TextColor = Color.Black;

                    this.warehouseLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.warehouseLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.sectionLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.sectionLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                    this.slotLocationMapInfo.SetWarehouseDataText("Unknown");
                    this.slotLocationMapInfo.SetWarehouseDataTextColor(Color.Black);
                }
                else
                {
                    //this.lblWarehouseData.Text = this.LocMapInfo.WhsCode + "  " + this.LocMapInfo.WhsName;
                    //this.lblWarehouseData.TextColor = Color.Blue;

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
                    if (this.DataViewSlotCoord1.Count > 0)
                    {
                        if (this.dgCoord1.SelectedItem is Slot slot1)
                        {
                            this.lblSlotCoordinate1.Text = "Coord1: " + slot1.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.DataViewSlotCoord2.Count > 0)
                    {
                        if (this.dgCoord2.SelectedItem is Slot slot2)
                        {
                            this.lblSlotCoordinate2.Text = "Coord2: " + slot2.LocationMapSlotCoordinateCode;
                        }
                    }

                    if (this.DataViewSlotCoord3.Count > 0)
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
                    int index = this.dgSections.SelectedItem is Section section ? this.SectionList.IndexOf(section) : 0;

                    if (Convert.ToInt32(this.DataViewSections[index]["LocationMapRefno"]) != this.LocMapInfo.SectLocMapRefno)
                    {
                        for (int rowIdx = 0; rowIdx < this.DataViewSections.Count; rowIdx++)
                        {
                            if (Convert.ToInt32(this.DataViewSections[rowIdx]["LocationMapRefno"]) == this.LocMapInfo.SectLocMapRefno)
                            {
                                //this.dgSections.CurrentRowIndex = rowIdx;
                                //this.dgSections.Select(rowIdx);

                                if (this.SectionList.Count > rowIdx)
                                {
                                    this.dgSections.SelectedItem = this.SectionList[rowIdx];
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        //dgSections.Select(dgSections.CurrentRowIndex);

                        if (this.SectionList.Count > 0)
                        {
                            this.dgSections.SelectedItem = this.SectionList[0];
                        }
                    }
                }
                else
                {
                    //dgSections.CurrentRowIndex = 0;
                    //dgSections.Select(0);

                    if (this.SectionList.Count > 0)
                    {
                        this.dgSections.SelectedItem = this.SectionList[0];
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
                if (this.DataViewSlotCoord1.Count > this.SlotCoord1List.Count)
                {
                    this.PopulateSlotCoord1List();
                }

                if (this.DataViewSlotCoord2.Count > this.SlotCoord2List.Count)
                {
                    this.PopulateSlotCoord2List();
                }

                if (this.DataViewSlotCoord3.Count > this.SlotCoord3List.Count)
                {
                    this.PopulateSlotCoord3List();
                }

                // Attempt to find last selected slot coordinate in slot coordinate 1 view
                if (this.LocMapInfo.SlotCoord1LocMapRefno != 0)
                {
                    for (int rowIdx = 0; rowIdx < this.DataViewSlotCoord1.Count; rowIdx++)
                    {
                        if (Convert.ToInt32(this.DataViewSlotCoord1[rowIdx]["LocationMapSlotCoordinateRefno"]) == this.LocMapInfo.SlotCoord1LocMapRefno)
                        {
                            if (this.SlotCoord1List.Count > rowIdx)
                            {
                                this.dgCoord1.SelectedItem = this.SlotCoord1List[rowIdx];
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (this.SlotCoord1List.Count > 0)
                    {
                        this.dgCoord1.SelectedItem = this.SlotCoord1List[0];
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    for (int rowIdx = 0; rowIdx < this.DataViewSlotCoord2.Count; rowIdx++)
                    {
                        if (Convert.ToInt32(this.DataViewSlotCoord2[rowIdx]["LocationMapSlotCoordinateRefno"]) == this.LocMapInfo.SlotCoord2LocMapRefno)
                        {
                            if (this.SlotCoord2List.Count > rowIdx)
                            {
                                this.dgCoord2.SelectedItem = this.SlotCoord2List[rowIdx];
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (this.SlotCoord2List.Count > 0)
                    {
                        this.dgCoord2.SelectedItem = this.SlotCoord2List[0];
                    }
                }

                // Attempt to find last selected slot coordinate in slot coordinate 3 view
                if (0 < this.LocMapInfo.SlotCoord3LocMapRefno)
                {
                    for (int rowIdx = 0; rowIdx < this.DataViewSlotCoord3.Count; rowIdx++)
                    {
                        if (Convert.ToInt32(this.DataViewSlotCoord3[rowIdx]["LocationMapSlotCoordinateRefno"]) == this.LocMapInfo.SlotCoord3LocMapRefno)
                        {
                            if (this.SlotCoord3List.Count > rowIdx)
                            {
                                this.dgCoord3.SelectedItem = this.SlotCoord3List[rowIdx];
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (this.SlotCoord3List.Count > 0)
                    {
                        this.dgCoord3.SelectedItem = this.SlotCoord3List[0];
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
                //MessageBox.Show("Scanning an header reference # is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an header reference # is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
            {
                //MessageBox.Show("Scanning an item reference # is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an item reference # is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
            {
                //MessageBox.Show("Scanning a product bar code is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a product bar code is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
            {
                //MessageBox.Show("Scanning a shipping container bar code is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
            {
                int locMapRefno = LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.Parse.LocationIdentifier);
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                this.SelectWarehouse();
                this.SelectSection();
                this.SelectSlotCoordinate();
            }
            else
            {
                //MessageBox.Show("Scanned bar code type is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

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
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
                    this.dgSections.SelectedItem = null;
                    this.dgCoord1.SelectedItem = null;
                    this.dgCoord2.SelectedItem = null;
                    this.dgCoord3.SelectedItem = null;
                    this.LoadSections();
                    await this.UpdateLocationDisplay();
                }
            }
        }

        private async void dgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.IsChangesMade = true;

            if (this.IsControlLoaded && e.SelectedItem is Section section)
            {
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));
                this.dgCoord1.SelectedItem = null;
                this.dgCoord2.SelectedItem = null;
                this.dgCoord3.SelectedItem = null;
                this.LoadSlots();
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
                    slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
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
                    slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
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
                    slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slot1.LocationMapSlotCoordinateRefno, slot2.LocationMapSlotCoordinateRefno, slot3.LocationMapSlotCoordinateRefno);
                }

                if (slotLocMapRefno != 0)
                {
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                }

                await this.UpdateLocationDisplay();
            }
        }

        private void PopulateWarehouseList()
        {
            this.WarehouseList = new ObservableCollection<Warehouse>();

            foreach (DataRow row in this.DataViewWhs.ToTable().Rows)
            {
                Warehouse warehouse = new Warehouse
                {
                    LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"]),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"])
                };

                this.WarehouseList.Add(warehouse);
            }

            this.dgWhs.ItemsSource = this.WarehouseList;
        }

        private void PopulateSectionList()
        {
            this.SectionList = new ObservableCollection<Section>();

            foreach (DataRow row in this.DataViewSections.ToTable().Rows)
            {
                Section section = new Section
                {
                    LocationMapSectionCode = Convert.ToInt32(row["LocationMapSectionCode"]),
                    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                    WarehouseLocationMapRefno = Convert.ToInt32(row["WarehouseLocationMapRefno"])
                };

                this.SectionList.Add(section);
            }

            this.dgSections.ItemsSource = this.SectionList;
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