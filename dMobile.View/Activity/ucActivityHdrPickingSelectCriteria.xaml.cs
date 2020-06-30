using dMobile.View.CustomPages.Modal;
using System;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Types;
using Color = Xamarin.Forms.Color;
using dMobile.Resources.Services;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrPickingSelectCriteria : DMobileModal, IAppArea, IAppPublisher
    {
        // TODO: Form
        #region Fields

        //private UserControl callingControl = null;
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private LocationMap locMap;
        private BarCodeParse barParse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SLocMap locMapInfo;
        private DataView dataViewWhs;
        private DataView dataViewSections;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private ObservableCollection<object> warehouseList;
        private ObservableCollection<object> sectionList;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ucCallingControl"></param>
        /// <remarks></remarks>
        public ucActivityHdrPickingSelectCriteria()
        {
            this.InitializeComponent();

            this.lblSectionData.Text = "";
            this.lblWarehouseData.Text = "";
            this.lblSearch.Text = "";
            this.lblWarehouse.Text = "";
            this.lblSection.Text = "";
            this.tabWarehouse.Text = "";
            this.tabSection.Text = "";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.PickingSelectCriteria;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMapInfo = new SLocMap();
            }
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.dgWhs.ItemsSource = this.WarehouseList;
            this.dgSections.ItemsSource = this.SectionList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
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
                }
            });

            try
            {
                this.ucSetLocationMap_GotFocus(this, new EventArgs());
                this.tabLocation_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucSetLocationMap_GotFocus/tabLocation_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ScannerDisable();

            base.OnDisappearing();
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

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

        private BarCodeParse BarParse
        {
            get
            {
                return this.barParse;
            }

            set
            {
                this.barParse = value;
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

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        public ObservableCollection<object> WarehouseList { get => warehouseList; set => warehouseList = value; }
        public ObservableCollection<object> SectionList { get => sectionList; set => sectionList = value; }

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
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            return "Picking Selection Criteria";
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
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
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
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    if (!this.SysEnv.SysSettings.IsPickingModuleDisallowLoadingAllOrders)
                    {
                        if (!await DisplayAlert(this.GetControlDescription(), "All orders for all sections will be loaded. Continue?", "Yes", "No"))
                        {
                            cancelChanges = false;
                            this.IsChangesMade = false;
                        }
                    }

                    // Clear any properties.
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
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
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
            int locMapRefno = 0;

            if (0 < this.LocMapInfo.SectLocMapRefno)
            {
                locMapRefno = this.LocMapInfo.SectLocMapRefno;
            }
            else if (0 < this.LocMapInfo.WhsLocMapRefno)
            {
                locMapRefno = this.LocMapInfo.WhsLocMapRefno;
            }

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<LocationMapRefno>" + locMapRefno + "</LocationMapRefno>")
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
        /// <remarks></remarks>
        public void InitCboSearchSection(object sender, EventArgs e)
        {
            this.cboSectionSearch.SelectedItem = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void tabLocation_GotFocus(object sender, EventArgs e)
        {
            //switch (this.tabLocation.SelectedIndex)
            //{
            //    case 1:
            //        if (this.DataViewWhs == null || this.LocMapInfo.WhsLocMapRefno == 0)
            //        {
            //            // Resource strings
            //            await DisplayAlert(this.GetControlDescription(), "Select the warehouse first!", "OK");
            //            this.tabLocation.SelectedIndex = 0;
            //        }
            //        else
            //        {
            //            if (this.DataViewSections == null)
            //            {
            //                this.tabLocation.SelectedIndex = 0;
            //            }
            //            else
            //            {
            this.DataViewSections.RowFilter = "WarehouseLocationMapRefno = " + this.LocMapInfo.WhsLocMapRefno;
            this.SelectSection();

            if (this.DataViewSections.Count > 0)
            {
                //this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.dgSections[this.dgSections.CurrentRowIndex, 2]));

                if (this.dgSections.SelectedItem is Section section)
                {
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(section.LocationMapRefno));
                }
            }

            this.UpdateLocationDisplay();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucSetLocationMap_LostFocus(object sender, EventArgs e)
        {
            this.ScannerDisable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucSetLocationMap_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.ScannerEnable();
                    this.LoadData();
                    this.SelectWarehouse();
                    this.UpdateLocationDisplay();

                    // WO 10190 LEKKER wants to default to the section tab, Lekker only has one warehouse so we can assume it.
                    // Update Oct 9,2014 Lekker now has 2 warehouse this code will not work anymore. 
                    // John said "do not fix until they complain". We could default the warehouse to the login warehouse.
                    if (this.DataViewWhs.Count == 1 && this.DataViewSections.Count > 0)
                    {
                        //this.tabLocation.SelectedIndex = 1;
                        this.tabSection.Focus();
                    }

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = true;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchSection_TextChanged(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // By Code
                this.IsChangesMade = true;

                if (this.cboSectionSearch.SelectedIndex == 0)
                {
                    if ((string.Compare(this.txtSearchSection.Text, string.Empty) > 0))
                    {
                        this.DataViewSections.RowFilter = "LocationMapSectionCode = " + this.txtSearchSection.Text.Trim();
                    }
                }
                else
                {
                    this.DataViewSections.RowFilter = "LocationMapSectionName LIKE \'" + this.txtSearchSection.Text.Trim() + "%\'";
                }

                //this.dgSections.DataSource = this.dataViewSections;

            this.PopulateSectionList();
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
            this.DesignGridSections();

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
        private void LoadData()
        {
            SLocMap tempLocMapInfo = this.LocMap.CurrLocMap;

            // Default to the login warehouse
            tempLocMapInfo.WhsLocMapRefno = this.LocMap.LoginWhsLocMapRefno;
            
            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                tempLocMapInfo.WhsLocMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;
            }

            tempLocMapInfo.SectLocMapRefno = this.LocMap.CurrLocMap.SectLocMapRefno;

            if (this.LocMap.CurrLocMap.IsSectionTracked && tempLocMapInfo.SectLocMapRefno != 0)
            {
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(tempLocMapInfo.SectLocMapRefno);
            }
            else
            {
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(tempLocMapInfo.WhsLocMapRefno);
            }

            // Load the dataviews and apply row filters base on the current sLocationMapInfo values
            this.LoadWhsList();
            this.LoadSections();
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
                for (int rowIdx = 0; rowIdx <= this.DataViewWhs.Count - 1; rowIdx++)
                {
                    if (Convert.ToInt32(this.DataViewWhs[rowIdx]["LocationMapRefno"]) == this.LocMapInfo.WhsLocMapRefno)
                    {
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
                this.lblWarehouseData.Text = this.LocMapInfo.WhsCode + " " + this.LocMapInfo.WhsName;
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
                this.lblSectionData.Text = this.LocMapInfo.SectCode + " " + this.LocMapInfo.SectName;
                this.lblSectionData.TextColor = Color.Blue;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void SelectSection()
        {
            // Attempt to find last selected section in section view
            if (this.LocMapInfo.SectLocMapRefno != 0)
            {
                if (Convert.ToInt32(this.DataViewSections[this.SectionList.IndexOf(this.dgSections.SelectedItem as Section)]["LocationMapRefno"]) != this.LocMapInfo.SectLocMapRefno)
                {
                    for (int rowIdx = 0; rowIdx <= this.DataViewSections.Count - 1; rowIdx++)
                    {
                        if (Convert.ToInt32(this.DataViewSections[rowIdx]["LocationMapRefno"]) == this.LocMapInfo.SectLocMapRefno)
                        {
                            if (this.SectionList.Count > rowIdx)
                            {
                                this.dgSections.SelectedItem = this.SectionList[rowIdx];
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if (this.SectionList.Count > 0)
                {
                    this.dgSections.SelectedItem = this.SectionList[0];
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                // No need to disable/enable scanner because only messagebox are called. 
                // GoFocus and LostFocus events will handle the scanner
                this.BarParse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an activity header reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning a product bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    int locMapRefno = LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarParse.LocationIdentifier);
                    this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                    
                    this.SelectWarehouse();
                    this.SelectSection();
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanned bar code type is not supported here.");
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DesignGridSections()
        {
            this.PopulateSectionList();

            Label labelCodeHeader = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelNameHeader = new Label { Text = "Name", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Calculate text size
            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCodeHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelNameHeader.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelCodeHeader);
            gridHeader.Children.Add(labelNameHeader);

            Grid.SetColumn(labelCodeHeader, 0);
            Grid.SetColumn(labelNameHeader, 1);

            Grid.SetRow(gridHeader, 0);

            this.dataGridSectionsHeader.Children.Add(gridHeader);

            foreach (Section section in this.SectionList)
            {
                double dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(section.LocationMapSectionCode));
                double dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(section.LocationMapSectionName);

                if (dataColumnWidth0 > columnWidth0)
                {
                    columnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (dataColumnWidth1 > columnWidth1)
                {
                    columnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }
            }

            // Data grid
            DataTemplate dataTemplateCount = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    //ColumnSpacing = 15,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) }
                    }
                };

                Label labelCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelName = new Label { VerticalOptions = LayoutOptions.Center };

                labelCode.SetBinding(Label.TextProperty, new Binding("LocationMapSectionCode"));
                labelName.SetBinding(Label.TextProperty, new Binding("LocationMapSectionName"));

                gridData.Children.Add(labelCode, 0, 0);
                gridData.Children.Add(labelName, 0, 0);

                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelName, 1);

                return new ViewCell { View = gridData };
            });

            this.dgSections.ItemTemplate = dataTemplateCount;

            //if (this.dgSections.TableStyles.Count > 0)
            //{
            //    this.dgSections.TableStyles.Clear();
            //}

            //DataGridTableStyle dataGridStyle = new DataGridTableStyle();
            //dataGridStyle.MappingName = "Sections";

            //DataGridTextBoxColumn dataGridCol1 = new DataGridTextBoxColumn();
            //dataGridCol1.MappingName = "LocationMapSectionCode";
            //dataGridCol1.HeaderText = "Code";
            //dataGridCol1.Width = 50;
            //dataGridStyle.GridColumnStyles.Add(dataGridCol1);

            //DataGridTextBoxColumn dataGridCol2 = new DataGridTextBoxColumn();
            //dataGridCol2.MappingName = "LocationMapSectionName";
            //dataGridCol2.HeaderText = "Name";
            //dataGridCol2.Width = 173;
            //dataGridStyle.GridColumnStyles.Add(dataGridCol2);

            //DataGridTextBoxColumn dataGridCol3 = new DataGridTextBoxColumn();
            //dataGridCol3.MappingName = "LocationMapRefno";
            //dataGridCol3.HeaderText = "LocationMapRefno";
            //dataGridCol3.Width = 0;
            //dataGridStyle.GridColumnStyles.Add(dataGridCol3);

            //this.dgSections.TableStyles.Add(dataGridStyle);
            //this.dgSections.RowHeadersVisible = false;
            //this.dgSections.ColumnHeadersVisible = true;
        }

        #endregion

        private void dgWhs_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded && e.SelectedItem is Warehouse warehouse)
            {
                this.IsChangesMade = true;
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
                this.UpdateLocationDisplay();
            }
        }

        private void dgSections_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.IsControlLoaded && e.SelectedItem is Warehouse warehouse)
            {
                this.IsChangesMade = true;
                this.LocMapInfo = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(warehouse.LocationMapRefno));
                this.UpdateLocationDisplay();
            }
        }

        private void PopulateWarehouseList()
        {
            this.WarehouseList = new ObservableCollection<object>();

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
            this.SectionList = new ObservableCollection<object>();

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
    }
}