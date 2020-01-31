using dMobile.View.CustomPages.Modal;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.View.Types;
using System.Linq;

namespace dMobile.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSetLocationMap : DMobileModal, IAppArea, IAppPublisher
    {
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SelectLocation;
        //private UserControl callingControl = null;
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private LocationMap locMap;
        private BarCodeParse parse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
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
        private ObservableCollection<Section> sectionsList;
        private ObservableCollection<Section> filteredSectionsList;
        private ObservableCollection<Slot> slotCoord1List;
        private ObservableCollection<Slot> filteredSlotCoord1List;
        private ObservableCollection<Slot> slotCoord2List;
        private ObservableCollection<Slot> filteredSlotCoord2List;
        private ObservableCollection<Slot> slotCoord3List;
        private ObservableCollection<Slot> filteredSlotCoord3List;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        public ucSetLocationMap()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblWarehouse.Text = "Whs";
            this.lblSection.Text = "Section";
            this.lblSlot.Text = "Slot";
            this.tabWarehouse.Text = "Warehouse";
            this.tabSection.Text = "Section";
            this.lblSearch.Text = "Search";
            this.tabSlot.Text = "Slot";
            this.lblSlotCoordinate1.Text = "Coord 1";
            this.lblSlotCoordinate2.Text = "Coord 2";
            this.lblSlotCoordinate3.Text = "Coord 3";
            this.lblWarehouseData.Text = "Unknown";
            this.lblSectionData.Text = "Unknown";
            this.lblSlotData.Text = "Unknown";

            this.cboSectionSearch.Items.Add("Code");
            this.cboSectionSearch.Items.Add("Description");

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.WarehouseList = new ObservableCollection<Warehouse>();
                this.SectionsList = new ObservableCollection<Section>();
                this.SlotCoord1List = new ObservableCollection<Slot>();
                this.SlotCoord2List = new ObservableCollection<Slot>();
                this.SlotCoord3List = new ObservableCollection<Slot>();

                this.locMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.LoadData();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        public ucSetLocationMap(int defaultWhsLocMapRefno, bool lockWhsSelection)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblWarehouse.Text = "Whs";
            this.lblSection.Text = "Section";
            this.lblSlot.Text = "Slot";
            this.tabWarehouse.Text = "Warehouse";
            this.tabSection.Text = "Section";
            this.lblSearch.Text = "Search";
            this.tabSlot.Text = "Slot";
            this.lblSlotCoordinate1.Text = "Coord 1";
            this.lblSlotCoordinate2.Text = "Coord 2";
            this.lblSlotCoordinate3.Text = "Coord 3";
            this.lblWarehouseData.Text = "Unknown";
            this.lblSectionData.Text = "Unknown";
            this.lblSlotData.Text = "Unknown";

            this.cboSectionSearch.Items.Add("Code");
            this.cboSectionSearch.Items.Add("Description");

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.WarehouseList = new ObservableCollection<Warehouse>();
                this.SectionsList = new ObservableCollection<Section>();
                this.SlotCoord1List = new ObservableCollection<Slot>();
                this.SlotCoord2List = new ObservableCollection<Slot>();
                this.SlotCoord3List = new ObservableCollection<Slot>();

                this.locMapInfo = new SLocMap();
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.DefaultWhsLocMapRefno = defaultWhsLocMapRefno;
                this.LockWhsSelection = lockWhsSelection;

                if (this.LockWhsSelection)
                {
                    this.dgWhs.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        await this.Close();
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
            });

            try
            {
                this.ucSetLocationMap_GotFocus(this, new EventArgs());
                this.tabLocation_GotFocus(this, new EventArgs());
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
        protected override void OnDisappearing()
        {
            this.ucSetLocationMap_LostFocus(this, new EventArgs());
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #region Events

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

        #endregion

        #region Properties

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

        private BarCodeScanner.BarCodeScanner Scanner
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
        public ObservableCollection<Section> SectionsList { get => sectionsList; set => sectionsList = value; }
        public ObservableCollection<Slot> SlotCoord1List { get => slotCoord1List; set => slotCoord1List = value; }
        public ObservableCollection<Slot> SlotCoord2List { get => slotCoord2List; set => slotCoord2List = value; }
        public ObservableCollection<Slot> SlotCoord3List { get => slotCoord3List; set => slotCoord3List = value; }
        public ObservableCollection<Section> FilteredSectionsList { get => filteredSectionsList; set => filteredSectionsList = value; }
        public ObservableCollection<Slot> FilteredSlotCoord1List { get => filteredSlotCoord1List; set => filteredSlotCoord1List = value; }
        public ObservableCollection<Slot> FilteredSlotCoord2List { get => filteredSlotCoord2List; set => filteredSlotCoord2List = value; }
        public ObservableCollection<Slot> FilteredSlotCoord3List { get => filteredSlotCoord3List; set => filteredSlotCoord3List = value; }

        #endregion

        #region Public Members
        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
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
                    this.ScannerDisable();
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

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
        public bool SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    this.LocMap.CurrLocMap = this.LocMapInfo;
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    //MessageBox.Show("Data failed validation. Check your data.");

                    // TODO: Resource strings
                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Validation Error", "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
        public bool CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                if (this.IsReadyToClose())
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
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
        private void ucSetLocationMap_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.ScannerEnable();
                    this.LoadData();
                    this.SelectWarehouse();

                    // Selecting the section and slot are required here even if the section / slot grids are not display 
                    // because we want to set the grid(s).CurrentRowIdx
                    if (this.LocMap.IsSectLocMapTracked)
                    {
                        this.SelectSection();
                        this.SelectSlotCoordinate();
                    }

                    this.IsControlLoaded = true;
                    this.UpdateLocationDisplay();

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
        private void tabLocation_GotFocus(object sender, EventArgs e)
        {
            try
            {
                //switch (this.tabLocation.SelectedIndex)
                //{
                //    case 1:
                if (this.DataViewWhs == null || this.LocMapInfo.WhsLocMapRefno == 0)
                {
                    //MessageBox.Show("Select the warehouse first!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Select the warehouse first!", "OK"));
                }
                else
                {
                    if (this.DataViewSections == null)
                    {
                        //this.tabLocation.SelectedIndex = 0;
                    }
                    else
                    {
                        //this.DataViewSections.RowFilter = "WarehouseLocationMapRefno = " + this.LocMapInfo.WhsLocMapRefno;

                        this.SelectSection();

                        if (this.DataViewSections.Count > 0)
                        {
                            if (this.dgSections.SelectedItem is Section section && this.LocMapInfo.SectLocMapRefno != section.LocationMapRefno)
                            {
                                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(section.LocationMapRefno);
                            }
                        }
                    }
                }

                //    break;

                //case 2:
                if (this.DataViewSections == null || this.LocMapInfo.SectLocMapRefno == 0)
                {
                    //MessageBox.Show("Select the section first!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Select the section first!", "OK"));

                    //this.tabLocation.SelectedIndex = 1;
                }
                else
                {
                    if (this.DataViewSlotCoord1 == null)
                    {
                        //this.tabLocation.SelectedIndex = 1;
                    }
                    else
                    {
                        //this.DataViewSlotCoord1.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
                        //this.DataViewSlotCoord2.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
                        //this.DataViewSlotCoord3.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;

                        this.SelectSlotCoordinate();

                        if (this.DataViewSlotCoord1.Count > 0)
                        {
                            Slot slotCoord1 = this.dgCoord1.SelectedItem as Slot;
                            Slot slotCoord2 = this.dgCoord2.SelectedItem as Slot;
                            Slot slotCoord3 = this.dgCoord3.SelectedItem as Slot;

                            //int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(
                            //    this.LocMapInfo.SectLocMapRefno,
                            //    (int)this.dgCoord1[this.dgCoord1.CurrentRowIndex, 1],
                            //    (int)this.dgCoord2[this.dgCoord2.CurrentRowIndex, 1],
                            //    (int)this.dgCoord3[this.dgCoord3.CurrentRowIndex, 1]);

                            if (slotCoord1 != null && slotCoord2 != null && slotCoord3 != null)
                            {
                                int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(
                                    this.LocMapInfo.SectLocMapRefno,
                                    slotCoord1.LocationMapSlotCoordinateRefno,
                                    slotCoord2.LocationMapSlotCoordinateRefno,
                                    slotCoord3.LocationMapSlotCoordinateRefno);

                                if (slotLocMapRefno != 0)
                                {
                                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                                }
                            }
                        }
                    }
                    //        }
                    //        break;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            this.UpdateLocationDisplay();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSetLocationMap_LostFocus(object sender, EventArgs e)
        {
            if (this.Scanner != null)
            {
                this.ScannerDisable();
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

            this.WarehouseList.Clear();

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
            //this.DataViewSections.RowFilter = "WarehouseLocationMapRefno = " + this.LocMapInfo.WhsLocMapRefno;

            this.SectionsList.Clear();

            foreach (DataRow row in this.DataViewSections.ToTable().Rows)
            {
                Section section = new Section
                {
                    LocationMapSectionCode = Convert.ToInt32(row["LocationMapSectionCode"]),
                    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                    WarehouseLocationMapRefno = Convert.ToInt32(row["WarehouseLocationMapRefno"])
                };

                this.SectionsList.Add(section);
            }

            this.FilteredSectionsList = new ObservableCollection<Section>(this.SectionsList.Where((section) => section.WarehouseLocationMapRefno == this.LocMapInfo.WhsLocMapRefno));
            this.dgSections.ItemsSource = this.FilteredSectionsList;
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
            //this.DataViewSlotCoord1.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;

            this.SlotCoord1List.Clear();

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

            this.FilteredSlotCoord1List = new ObservableCollection<Slot>(this.SlotCoord1List.Where((slot) => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno));
            this.dgCoord1.ItemsSource = this.FilteredSlotCoord1List;

            LocationMapTasks.GetSlotCoord2List(ref dataTableSlots);
            dataTableSlots.TableName = "SlotCoord2";
            this.DataViewSlotCoord2 = dataTableSlots.DefaultView;
            //this.DataViewSlotCoord2.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
            //this.dgCoord2.ItemsSource = this.DataViewSlotCoord2;

            this.SlotCoord2List.Clear();

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

            this.FilteredSlotCoord2List = new ObservableCollection<Slot>(this.SlotCoord2List.Where((slot) => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno));
            this.dgCoord2.ItemsSource = this.FilteredSlotCoord2List;

            LocationMapTasks.GetSlotCoord3List(ref dataTableSlots);
            dataTableSlots.TableName = "SlotCoord3";
            this.DataViewSlotCoord3 = dataTableSlots.DefaultView;
            //this.DataViewSlotCoord3.RowFilter = "SectionLocationMapRefno = " + this.LocMapInfo.SectLocMapRefno;
            //this.dgCoord3.ItemsSource = this.DataViewSlotCoord3;

            this.SlotCoord3List.Clear();

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

            this.FilteredSlotCoord3List = new ObservableCollection<Slot>(this.SlotCoord3List.Where((slot) => slot.SectionLocationMapRefno == this.LocMapInfo.SectLocMapRefno));
            this.dgCoord3.ItemsSource = this.FilteredSlotCoord3List;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            // Default to the login warehouse
            int locMapRefno = 0;

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

            this.SelectSlotCoordinate();
            this.SelectSection();
            this.SelectWarehouse();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void SelectWarehouse()
        {
            //Attempt to find last selected warehouse in warehouse view
            if (this.LocMapInfo.WhsLocMapRefno != 0)
            {
                //for (int rowIdx = 0; rowIdx <= this.DataViewWhs.Count - 1; rowIdx++)
                foreach (var item in this.WarehouseList)
                {
                    //if ((int)this.DataViewWhs[rowIdx]["LocationMapRefno"] == this.LocMapInfo.WhsLocMapRefno)

                    if (item is Warehouse warehouse && warehouse.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno)
                    {
                        //this.dgWhs.CurrentRowIndex = rowIdx;
                        //this.dgWhs.Select(rowIdx);
                        this.dgWhs.SelectedItem = warehouse;

                        break;
                    }
                }
            }
            else
            {
                //dgWhs.CurrentRowIndex = 0;
                //dgWhs.Select(0);

                if (this.WarehouseList != null && this.WarehouseList.Count > 0)
                {
                    this.dgWhs.SelectedItem = this.WarehouseList[0] as Warehouse;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void UpdateLocationDisplay()
        {
            if (this.LocMapInfo.WhsLocMapRefno == 0)
            {
                // TODO: Resource strings
                this.lblWarehouseData.Text = "Unknown";
                this.lblWarehouseData.TextColor = Color.Black;
            }
            else
            {
                this.lblWarehouseData.Text = this.LocMapInfo.WhsCode + "  " + this.LocMapInfo.WhsName;
                this.lblWarehouseData.TextColor = Color.Blue;
            }

            if (this.LocMapInfo.SectLocMapRefno == 0)
            {
                // TODO: Resource strings
                this.lblSectionData.Text = "Unknown";
                this.lblSectionData.TextColor = Color.Black;
            }
            else
            {
                this.lblSectionData.Text = this.LocMapInfo.SectCode + "  " + this.LocMapInfo.SectName;
                this.lblSectionData.TextColor = Color.Blue;
            }

            if (this.LocMapInfo.SlotLocMapRefno == 0)
            {
                // TODO: Resource strings
                this.lblSlotData.Text = "Unknown";
                this.lblSlotData.TextColor = Color.Black;
            }
            else
            {
                this.lblSlotData.Text = this.LocMapInfo.SlotName;
                this.lblSlotData.TextColor = Color.Blue;
            }

            //if (this.tabLocation.SelectedIndex == 1)
            //{
            this.cboSectionSearch.SelectedIndex = 0;

            // TODO: Resource strings
            //this.cboSectionSearch.Text = "Description";
            //}

            //if (this.tabLocation.SelectedIndex == 2)
            //{
            if (this.DataViewSlotCoord1.Count > 0)
            {
                // TODO: Resource strings
                //this.lblSlotCoordinate1.Text = "Crd1: " + this.dgCoord1[this.dgCoord1.CurrentRowIndex, 0].ToString();

                if (this.dgCoord1.SelectedItem is Slot coord1)
                {
                    this.lblSlotCoordinate1.Text = "Crd1: " + coord1.LocationMapSlotCoordinateCode;
                }
            }

            if (this.DataViewSlotCoord2.Count > 0)
            {
                // TODO: Resource strings
                //this.lblSlotCoordinate2.Text = "Crd2: " + this.dgCoord2[this.dgCoord2.CurrentRowIndex, 0].ToString();

                if (this.dgCoord2.SelectedItem is Slot coord2)
                {
                    this.lblSlotCoordinate2.Text = "Crd2: " + coord2.LocationMapSlotCoordinateCode;
                }
            }

            if (this.DataViewSlotCoord3.Count > 0)
            {
                // TODO: Resource strings
                //this.lblSlotCoordinate3.Text = "Crd3: " + this.dgCoord3[this.dgCoord3.CurrentRowIndex, 0].ToString();

                if (this.dgCoord3.SelectedItem is Slot coord3)
                {
                    this.lblSlotCoordinate3.Text = "Crd3: " + coord3.LocationMapSlotCoordinateCode;
                }

            }
            //}
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void SelectSection()
        {
            if (this.LocMap.IsSectLocMapTracked)
            {
                // attempt to find last selected section in section view
                if (this.LocMapInfo.SectLocMapRefno != 0)
                {
                    if (this.dgSections.SelectedItem is Section section && section.LocationMapRefno != this.LocMapInfo.SectLocMapRefno)
                    {
                        //for (int rowIdx = 0; rowIdx <= this.DataViewSections.Count - 1; rowIdx++)
                        //{
                        //    if ((int)this.DataViewSections[rowIdx]["LocationMapRefno"] == this.LocMapInfo.SectLocMapRefno)
                        //    {
                        //        this.dgSections.CurrentRowIndex = rowIdx;
                        //        this.dgSections.Select(rowIdx);

                        //        break;
                        //    }
                        //}

                        foreach (var item in this.FilteredSectionsList)
                        {
                            if (item is Section sect && section.LocationMapRefno == this.LocMapInfo.WhsLocMapRefno)
                            {
                                this.dgWhs.SelectedItem = sect;

                                break;
                            }
                        }
                    }
                    else
                    {
                        //dgSections.Select(dgSections.CurrentRowIndex);

                        if (this.FilteredSectionsList != null && this.FilteredSectionsList.Count > 0)
                        {
                            this.dgSections.SelectedItem = this.FilteredSectionsList[0] as Section;
                        }
                    }
                }
                else
                {
                    //dgSections.CurrentRowIndex = 0;
                    //dgSections.Select(0);

                    if (this.FilteredSectionsList != null && this.FilteredSectionsList.Count > 0)
                    {
                        this.dgSections.SelectedItem = this.FilteredSectionsList[0] as Section;
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
            if (this.LocMap.IsSlotLocMapTracked)
            {
                // Attempt to find last selected slot coordinate in slot coordinate 1 view
                if (this.LocMapInfo.SlotCoord1LocMapRefno != 0)
                {
                    if (this.dgCoord1.SelectedItem is Slot slot && slot.LocationMapSlotCoordinateRefno != this.LocMapInfo.SlotCoord1LocMapRefno)
                    //if ((int)this.DataViewSlotCoord1[this.dgCoord1.CurrentRowIndex]["LocationMapSlotCoordinateRefno"] != this.LocMapInfo.SlotCoord1LocMapRefno)
                    {
                        //for (int rowIdx = 0; rowIdx <= this.DataViewSlotCoord1.Count - 1; rowIdx++)
                        //{
                        //    if ((int)this.DataViewSlotCoord1[rowIdx]["LocationMapSlotCoordinateRefno"] == this.LocMapInfo.SlotCoord1LocMapRefno)
                        //    {
                        //        this.dgCoord1.CurrentRowIndex = rowIdx;

                        //        break;
                        //    }
                        //}

                        foreach (var item in this.FilteredSlotCoord1List)
                        {
                            //if ((int)this.DataViewWhs[rowIdx]["LocationMapRefno"] == this.LocMapInfo.WhsLocMapRefno)

                            if (item is Slot slotCoord1 && slotCoord1.LocationMapRefno == this.LocMapInfo.SlotCoord1LocMapRefno)
                            {
                                //this.dgWhs.CurrentRowIndex = rowIdx;
                                //this.dgWhs.Select(rowIdx);
                                this.dgCoord1.SelectedItem = slotCoord1;

                                break;
                            }
                        }
                    }
                    else
                    {
                        //this.dgCoord1.Select(this.dgCoord1.CurrentRowIndex);

                        if (this.FilteredSlotCoord1List != null && this.FilteredSlotCoord1List.Count > 0)
                        {
                            this.dgCoord1.SelectedItem = this.FilteredSlotCoord1List[0] as Slot;
                        }
                    }
                }
                else
                {
                    if (this.DataViewSlotCoord1.Count > 0)
                    {
                        //this.dgCoord1.CurrentRowIndex = 0;
                        //this.dgCoord1.Select(0);

                        if (this.FilteredSlotCoord1List != null && this.FilteredSlotCoord1List.Count > 0)
                        {
                            this.dgCoord1.SelectedItem = this.FilteredSlotCoord1List[0] as Slot;
                        }
                    }
                }

                // attempt to find last selected slot coordinate in slot coordinate 2 view
                if (this.LocMapInfo.SlotCoord2LocMapRefno != 0)
                {
                    //if ((int)this.DataViewSlotCoord2[this.dgCoord2.CurrentRowIndex]["LocationMapSlotCoordinateRefno"] != this.LocMapInfo.SlotCoord2LocMapRefno)
                    if (this.dgCoord2.SelectedItem is Slot slot && slot.LocationMapSlotCoordinateRefno != this.LocMapInfo.SlotCoord2LocMapRefno)
                    {
                        //for (int rowIdx = 0; rowIdx <= this.DataViewSlotCoord2.Count - 1; rowIdx++)
                        //{
                        //    if ((int)this.DataViewSlotCoord2[rowIdx]["LocationMapSlotCoordinateRefno"] == this.LocMapInfo.SlotCoord2LocMapRefno)
                        //    {
                        //        this.dgCoord2.CurrentRowIndex = rowIdx;
                        //        this.dgCoord2.Select(rowIdx);

                        //        break;
                        //    }
                        //}

                        foreach (var item in this.FilteredSlotCoord2List)
                        {
                            //if ((int)this.DataViewWhs[rowIdx]["LocationMapRefno"] == this.LocMapInfo.WhsLocMapRefno)

                            if (item is Slot slotCoord2 && slotCoord2.LocationMapRefno == this.LocMapInfo.SlotCoord2LocMapRefno)
                            {
                                //this.dgWhs.CurrentRowIndex = rowIdx;
                                //this.dgWhs.Select(rowIdx);
                                this.dgCoord1.SelectedItem = slotCoord2;

                                break;
                            }
                        }

                    }
                    else
                    {
                        //this.dgCoord2.Select(dgCoord2.CurrentRowIndex);

                        if (this.FilteredSlotCoord2List != null && this.FilteredSlotCoord2List.Count > 0)
                        {
                            this.dgCoord2.SelectedItem = this.FilteredSlotCoord2List[0] as Slot;
                        }
                    }
                }
                else
                {
                    if (this.DataViewSlotCoord2.Count > 0)
                    {
                        //this.dgCoord2.CurrentRowIndex = 0;
                        //this.dgCoord2.Select(0);

                        if (this.FilteredSlotCoord2List != null && this.FilteredSlotCoord2List.Count > 0)
                        {
                            this.dgCoord2.SelectedItem = this.FilteredSlotCoord2List[0] as Slot;
                        }
                    }
                }

                //// Attempt to find last selected slot coordinate in slot coordinate 3 view
                //if (0 < this.LocMapInfo.SlotCoord3LocMapRefno)
                //{
                //    if ((int)this.DataViewSlotCoord3[this.dgCoord3.CurrentRowIndex]["LocationMapSlotCoordinateRefno"] != this.LocMapInfo.SlotCoord3LocMapRefno)
                //    {
                //        for (int rowIdx = 0; rowIdx <= this.DataViewSlotCoord3.Count - 1; rowIdx++)
                //        {
                //            if ((int)this.DataViewSlotCoord3[rowIdx]["LocationMapSlotCoordinateRefno"] == this.LocMapInfo.SlotCoord3LocMapRefno)
                //            {
                //                this.dgCoord3.CurrentRowIndex = rowIdx;
                //                this.dgCoord3.Select(rowIdx);

                //                break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        this.dgCoord3.Select(dgCoord3.CurrentRowIndex);
                //    }
                //}
                //else
                //{
                //    if (this.DataViewSlotCoord3.Count > 0)
                //    {
                //        this.dgCoord3.CurrentRowIndex = 0;
                //        this.dgCoord3.Select(0);
                //    }
                //}

                if (this.LocMapInfo.SlotCoord3LocMapRefno != 0)
                {
                    if (this.dgCoord3.SelectedItem is Slot slot && slot.LocationMapSlotCoordinateRefno != this.LocMapInfo.SlotCoord3LocMapRefno)
                    //if ((int)this.DataViewSlotCoord1[this.dgCoord1.CurrentRowIndex]["LocationMapSlotCoordinateRefno"] != this.LocMapInfo.SlotCoord1LocMapRefno)
                    {
                        //for (int rowIdx = 0; rowIdx <= this.DataViewSlotCoord1.Count - 1; rowIdx++)
                        //{
                        //    if ((int)this.DataViewSlotCoord1[rowIdx]["LocationMapSlotCoordinateRefno"] == this.LocMapInfo.SlotCoord1LocMapRefno)
                        //    {
                        //        this.dgCoord1.CurrentRowIndex = rowIdx;

                        //        break;
                        //    }
                        //}

                        foreach (var item in this.FilteredSlotCoord3List)
                        {
                            //if ((int)this.DataViewWhs[rowIdx]["LocationMapRefno"] == this.LocMapInfo.WhsLocMapRefno)

                            if (item is Slot slotCoord3 && slotCoord3.LocationMapRefno == this.LocMapInfo.SlotCoord3LocMapRefno)
                            {
                                //this.dgWhs.CurrentRowIndex = rowIdx;
                                //this.dgWhs.Select(rowIdx);
                                this.dgCoord3.SelectedItem = slotCoord3;

                                break;
                            }
                        }
                    }
                    else
                    {
                        if (this.FilteredSlotCoord3List != null && this.FilteredSlotCoord3List.Count > 0)
                        {
                            this.dgCoord3.SelectedItem = this.FilteredSlotCoord3List[0] as Slot;
                        }
                    }
                }
                else
                {
                    if (this.DataViewSlotCoord3.Count > 0)
                    {
                        //this.dgCoord3.CurrentRowIndex = 0;
                        //this.dgCoord3.Select(0);

                        if (this.FilteredSlotCoord3List != null && this.FilteredSlotCoord3List.Count > 0)
                        {
                            this.dgCoord3.SelectedItem = this.FilteredSlotCoord3List[0] as Slot;
                        }
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
        private void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            // No need to disable/enable scanner because only messagebox are called. 
            // GoFocus and LostFocus events will handle the scanner
            this.Parse = scanArgs.ParseInfo;

            if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an header reference # is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an item reference # is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a product bar code is not supported here!", "OK"));
            }
            else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
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
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here!", "OK"));
            }
        }

        #endregion

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            await this.Close();
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private void DgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.IsChangesMade = true;
            //this.dgSections.Select(this.dgSections.CurrentRowIndex);

            if (this.IsControlLoaded)
            {
                //this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno((int)this.dgSections[this.dgSections.CurrentRowIndex, 2]);

                Section section = this.dgSections.SelectedItem as Section;
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));

                this.UpdateLocationDisplay();
            }
        }

        private void DgCoord1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
                //int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno,
                //    (int)this.dgCoord1[this.dgCoord1.CurrentRowIndex, 1],
                //    (int)this.dgCoord2[this.dgCoord2.CurrentRowIndex, 1],
                //    (int)this.dgCoord3[this.dgCoord3.CurrentRowIndex, 1]);

                Slot slotCoord1 = this.dgCoord1.SelectedItem as Slot;
                Slot slotCoord2 = this.dgCoord2.SelectedItem as Slot;
                Slot slotCoord3 = this.dgCoord3.SelectedItem as Slot;

                if (slotCoord1 != null && slotCoord2 != null && slotCoord3 != null)
                {
                    int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slotCoord1.LocationMapSlotCoordinateRefno, slotCoord2.LocationMapSlotCoordinateRefno, slotCoord3.LocationMapSlotCoordinateRefno);

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    this.UpdateLocationDisplay();
                }
            }
        }

        private void DgCoord2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
                //int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(
                //    this.LocMapInfo.SectLocMapRefno,
                //    (int)this.dgCoord1[this.dgCoord1.CurrentRowIndex, 1],
                //    (int)this.dgCoord2[this.dgCoord2.CurrentRowIndex, 1],
                //    (int)this.dgCoord3[this.dgCoord3.CurrentRowIndex, 1]);

                Slot slotCoord1 = this.dgCoord1.SelectedItem as Slot;
                Slot slotCoord2 = this.dgCoord2.SelectedItem as Slot;
                Slot slotCoord3 = this.dgCoord3.SelectedItem as Slot;

                if (slotCoord1 != null && slotCoord2 != null && slotCoord3 != null)
                {
                    int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slotCoord1.LocationMapSlotCoordinateRefno, slotCoord2.LocationMapSlotCoordinateRefno, slotCoord3.LocationMapSlotCoordinateRefno);

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    this.UpdateLocationDisplay();
                }
            }
        }

        private void DgCoord3_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
                //int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(
                //    this.LocMapInfo.SectLocMapRefno,
                //    (int)this.dgCoord1[this.dgCoord1.CurrentRowIndex, 1],
                //    (int)this.dgCoord2[this.dgCoord2.CurrentRowIndex, 1],
                //    (int)this.dgCoord3[this.dgCoord3.CurrentRowIndex, 1]);

                Slot slotCoord1 = this.dgCoord1.SelectedItem as Slot;
                Slot slotCoord2 = this.dgCoord2.SelectedItem as Slot;
                Slot slotCoord3 = this.dgCoord3.SelectedItem as Slot;

                if (slotCoord1 != null && slotCoord2 != null && slotCoord3 != null)
                {
                    int slotLocMapRefno = LocationMapTasks.GetSlotRefnoFromCoordinates(this.LocMapInfo.SectLocMapRefno, slotCoord1.LocationMapSlotCoordinateRefno, slotCoord2.LocationMapSlotCoordinateRefno, slotCoord3.LocationMapSlotCoordinateRefno);

                    if (slotLocMapRefno != 0)
                    {
                        this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(slotLocMapRefno);
                    }

                    this.UpdateLocationDisplay();
                }
            }
        }

        private void DgWhs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.isControlLoaded)
            {
                this.IsChangesMade = true;
                //this.dgWhs.Select(this.dgWhs.CurrentRowIndex);

                if (this.IsControlLoaded)
                {
                    //this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno((int)this.dgWhs[this.dgWhs.CurrentRowIndex, 2]);

                    Warehouse warehouse = this.dgWhs.SelectedItem as Warehouse;

                    if (warehouse != null)
                    {
                        this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(warehouse.LocationMapRefno);
                        this.UpdateLocationDisplay();
                    }
                }
            }
        }

        private void txtSearchSection_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.cboSectionSearch.SelectedIndex == 0)
            {
                if (string.Compare(txtSearchSection.Text, string.Empty) > 0)
                {
                    //this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
                    this.FilteredSectionsList = new ObservableCollection<Section>(this.SectionsList.Where((section) => section.LocationMapSectionCode.ToString().ToLower().Contains(this.txtSearchSection.Text.ToLower().Trim())));
                }
                else
                {
                    this.FilteredSectionsList = this.SectionsList;
                }
            }
            else
            {
                //this.DataViewSections.RowFilter = "LocationMapSectionName LIKE \'" + this.txtSearchSection.Text.Trim() + "%\'";
                this.FilteredSectionsList = new ObservableCollection<Section>(this.SectionsList.Where((section) => section.LocationMapSectionName.ToLower().Contains(this.txtSearchSection.Text.ToLower().Trim())));
            }

            this.dgSections.ItemsSource = this.FilteredSectionsList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.DataViewSlotCoord1.RowFilter = "LocationMapSlotCoordinateCode LIKE \'" + this.txtSlotCoordinate1.Text.Trim() + "%\'";
            this.FilteredSlotCoord1List = new ObservableCollection<Slot>(this.SlotCoord1List.Where((slot) => slot.LocationMapSlotCoordinateCode.ToString().ToLower().Contains(this.txtSlotCoordinate1.Text.ToLower().Trim())));
            this.dgCoord1.ItemsSource = this.FilteredSlotCoord1List;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.DataViewSlotCoord2.RowFilter = "LocationMapSlotCoordinateCode LIKE \'" + this.txtSlotCoordinate2.Text.Trim() + "%\'";
            this.FilteredSlotCoord2List = new ObservableCollection<Slot>(this.SlotCoord2List.Where((slot) => slot.LocationMapSlotCoordinateCode.ToString().ToLower().Contains(this.txtSlotCoordinate2.Text.ToLower().Trim())));
            this.dgCoord2.ItemsSource = this.FilteredSlotCoord2List;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSlotCoordinate3_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.DataViewSlotCoord3.RowFilter = "LocationMapSlotCoordinateCode LIKE \'" + this.txtSlotCoordinate3.Text.Trim() + "%\'";
            this.FilteredSlotCoord3List = new ObservableCollection<Slot>(this.SlotCoord3List.Where((slot) => slot.LocationMapSlotCoordinateCode.ToString().ToLower().Contains(this.txtSlotCoordinate3.Text.ToLower().Trim())));
            this.dgCoord3.ItemsSource = this.FilteredSlotCoord3List;
        }
    }
}