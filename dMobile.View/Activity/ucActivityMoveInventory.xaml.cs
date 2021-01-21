using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityMoveInventory : DMobileContentPage, IAppArea, IDataLayerEventListener, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucInventoryMovementMenu();
        private bool isControlLoaded = false;
        private bool isEnterTrapped = false;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private BarCodeParse barCodeParse = null;
        private int prodUnitRefno = 0;
        private int prodUnitBarCodeRefno = 0;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private SLocMap selFromLocMap;
        private int selFromCurrLocMapRefno = 0;
        private SLocMap selToLocMap;
        private int selToCurrLocMapRefno = 0;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.MoveInventory;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        #endregion

        #region Constructors

        public ucActivityMoveInventory()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityMoveInventory(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdFromLocation.Text = "Set From Location";
            this.cmdAdd.Text = "Add";
            this.cmdToLocation.Text = "Set To Location";
            this.lblScanToLocation.Text = "Scan To Loc.";

            this.DataGridMainActivityItemsList = new ObservableCollection<object>();
            this.ItemsList = new ObservableCollection<string>();

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SelFromLocMap = new SLocMap();
            this.SelToLocMap = new SLocMap();

            this.updnBarCodeModifier1.Items.Clear();
            
            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.updnBarCodeModifier1.Items.Add(string.Empty);
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
            
                foreach (string item in modifiers)
                {
                    this.updnBarCodeModifier1.Items.Add(item);
                }
            }
            else
            {
                this.updnBarCodeModifier1.IsVisible = false;
                this.updnBarCodeModifier1.IsEnabled = false;
            }

            this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(actSubType);
            this.Activity.SkidInfo = new SysSkidInfo();
            this.ActItem = ActItemFacade.GetActItem(this.Activity, actSubType);
            this.ActItem.ActHdr = Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, actSubType);
            this.ActDetail.ActItem = ActItem;
            this.ActDetail.ActExtra = new ActDetailExtra();

            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.Title = this.GetControlDescription();
        }

        #endregion

        #region OnAppearing/OnDisappearing
        
         /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Del || keycode == Keycode.Clear || keycode == Keycode.Back)
                {
                    // TODO: Resource strings
                    if (this.dataGridMainActivity.SelectedItem != null && this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) != -1 && await DisplayAlert(this.GetControlDescription(), "Do you wish to delete this record?", "Yes", "No"))
                    {
                        this.ActDetail.DataViewDetails[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)].Delete();
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                    else
                    {
                        await this.CancelChanges();
                    }
                }
            });

            try
            {
                this.ucActivityMoveInventory_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucActivityMoveInventory_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

        #region Events

        public event ReqCloseHandler ReqClose
        {
            add
            {
                ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(ReqCloseEvent, value);
            }
            remove
            {
                ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(ReqCloseEvent, value);
            }
        }

        public event ReqSaveEnableHandler ReqSaveEnable
        {
            add
            {
                ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Combine(ReqSaveEnableEvent, value);
            }
            remove
            {
                ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Remove(ReqSaveEnableEvent, value);
            }
        }

        public event ReqSaveDisableHandler ReqSaveDisable
        {
            add
            {
                ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Combine(ReqSaveDisableEvent, value);
            }
            remove
            {
                ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Remove(ReqSaveDisableEvent, value);
            }
        }

        public event ReqCancelEnableHandler ReqCancelEnable
        {
            add
            {
                ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Combine(ReqCancelEnableEvent, value);
            }
            remove
            {
                ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Remove(ReqCancelEnableEvent, value);
            }
        }

        public event ReqCancelDisableHandler ReqCancelDisable
        {
            add
            {
                ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Combine(ReqCancelDisableEvent, value);
            }
            remove
            {
                ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Remove(ReqCancelDisableEvent, value);
            }
        }

        #endregion

        #region Properties

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

        private int SelToCurrLocMapRefno
        {
            get
            {
                return this.selToCurrLocMapRefno;
            }

            set
            {
                this.selToCurrLocMapRefno = value;
            }
        }

        private int SelFromCurrLocMapRefno
        {
            get
            {
                return this.selFromCurrLocMapRefno;
            }
            set
            {
                this.selFromCurrLocMapRefno = value;
            }
        }

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
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

        private ActItem ActItem
        {
            get
            {
                return this.actItem;
            }

            set
            {
                this.actItem = value;
            }
        }

        private Controller.Activity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
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

        private SLocMap SelToLocMap
        {
            get
            {
                return this.selToLocMap;
            }

            set
            {
                this.selToLocMap = value;
            }
        }

        private SLocMap SelFromLocMap
        {
            get
            {
                return this.selFromLocMap;
            }

            set
            {
                this.selFromLocMap = value;
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

        private bool IsEnterTrapped
        {
            get
            {
                return this.isEnterTrapped;
            }

            set
            {
                this.isEnterTrapped = value;
            }
        }

        private int ProdUnitRefno
        {
            get
            {
                return this.prodUnitRefno;
            }

            set
            {
                this.prodUnitRefno = value;
            }
        }

        private int ProdUnitBarCodeRefno
        {
            get
            {
                return this.prodUnitBarCodeRefno;
            }

            set
            {
                this.prodUnitBarCodeRefno = value;
            }
        }

        private BarCodeParse BarCodeParse
        {
            get
            {
                return this.barCodeParse;
            }

            set
            {
                this.barCodeParse = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

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
            bool isReadyToSave = true;

            if (ActDetail.DataViewDetails.Count > 0)
            {
                if (!LocationMapTasks.IsCompleteLocMapRefno(SelFromLocMap))
                {
                    isReadyToSave = false;
                    
                    // TODO: Resource strings
                    await DisplayAlert("Send Moves", "Invalid Move \'From\' Location", "OK");

                }
                else if (!LocationMapTasks.IsCompleteLocMapRefno(SelToLocMap))
                {
                    isReadyToSave = false;

                    // TODO: Resource strings
                    await DisplayAlert("Send Moves", "Invalid Move \'To\' Location", "OK");

                }
                else if (SelFromLocMap.WhsLocMapRefno != SelToLocMap.WhsLocMapRefno)
                {
                    isReadyToSave = false;

                    // TODO: Resource strings
                    await DisplayAlert("Send Moves", "The \'From\' And \'To\' Warehouse(s) Must Be The Same", "OK");
                }
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
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (Scanner != null)
            {
                ScannerDisable();
            }

            return true;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    ucInventoryMovementMenu ucDisplayControl = new ucInventoryMovementMenu();
        //    return ucDisplayControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Move Inventory";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;

            try
            {
                if (await IsReadyToSave())
                {
                    if (ActDetail.DataViewDetails != null && ActDetail.DataViewDetails.Count > 0)
                    {
                        for (int idx = 0; idx <= ActDetail.DataViewDetails.Count - 1; idx++)
                        {
                            try
                            {
                                if (ActDetail.DataViewDetails.Count > 0)
                                {
                                    ActItem.FKRefno = 0;
                                    ActItem.Refno = 0;
                                    ProductInfo oProductInfo = ActItem.ProdInfo;
                                    oProductInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(ActDetail.DataViewDetails[idx]["ProductUnitRefno"]));
                                    oProductInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(oProductInfo.Refno);
                                    ActItem.ProdInfo = oProductInfo;
                                    ActDetail = ActDetail.GetInfo(idx, false);
                                    XDocument xmlParam = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<param>" + ("<whs_id>" + SelFromLocMap.WhsFKRefno.ToString() + "</whs_id>") + ("<sect_refno>" + SelFromLocMap.SectFKRefno.ToString() + "</sect_refno>") + ("<slot_refno>" + SelFromLocMap.SlotFKRefno.ToString() + "</slot_refno>") + ("<to_sect_refno>" + SelToLocMap.SectFKRefno.ToString() + "</to_sect_refno>") + ("<to_slot_refno>" + SelToLocMap.SlotFKRefno.ToString() + "</to_slot_refno>") + "</param>");

                                    ActDetail.Submit(idx, xmlParam);
                                    idx--;
                                }
                                else
                                {
                                    goto endOfForLoop;
                                }

                            }
                            catch (Exception ex)
                            {
                                // TODO: Resource strings
                                bool result = await DisplayAlert("Move Submission", Convert.ToString("Submit Move Failed" + Environment.NewLine + ex.Message), "Abort", "Retry");

                                if (result)
                                {
                                    bSaveComplete = false;
                                    goto endOfForLoop;

                                }
                                else
                                {
                                    if (ActDetail.DataViewDetails.Count > 0)
                                    {
                                        idx--;
                                    }
                                }
                            }
                        }
                    
                    endOfForLoop:

                        if (ActDetail.DataViewDetails.Count > 0)
                        {
                            bSaveComplete = false;
                        }
                        else
                        {
                            // TODO: Resource strings
                            if (await DisplayAlert("Move Submission", "Finished Moves?", "Yes", "No"))
                            {
                                bSaveComplete = true;
                            }
                        }
                    }
                }
                else
                {
                    // TODO: Resource strings
                    await DisplayAlert("Validation", "Data Failed Validation. Check Your Data.", "OK");
                }

            }
            catch (Exception ex)
            {
                bSaveComplete = false;
                
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return bSaveComplete;
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
                    // Clear any properties.
                    // this.ProdUnitRefno = 0;

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
                    // this.LoadData();
                    cancelChanges = false;
                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.MoveInventory, ex);
                    await errorReport.ShowDialog();
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
            XDocument xmlDoc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");
            return xmlDoc;
        }

        #endregion

        #region IDataLayerEventListener Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            this.ItemsList.Clear();

            //Tokenize data longer than 30 characters for display, wrap to next line
            const int tokenLength = 30;

            if (dataLayerEventArgs.StatusOutMsg.Length > tokenLength)
            {
                int iTokens = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dataLayerEventArgs.StatusOutMsg.Length / tokenLength)));
                int iTokenLength = 0;
                
                for (var iTokenIdx = 0; iTokenIdx <= (iTokens * tokenLength) - 1; iTokenIdx++)
                {
                    iTokenLength = Convert.ToInt32(dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx > tokenLength ? tokenLength : dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx);
                    this.ItemsList.Add(dataLayerEventArgs.StatusOutMsg.Substring(Convert.ToInt32(iTokenIdx), iTokenLength));
                    iTokenIdx += tokenLength;
                }
            }
            else
            {
                this.ItemsList.Add(dataLayerEventArgs.StatusOutMsg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            //if (dataLayerEventArgs.SetMinMaxValue)
            //{
            //    pbarStatus.Minimum = dataLayerEventArgs.MinValue;
            //    pbarStatus.Maximum = dataLayerEventArgs.MaxValue;
            //}

            //pbarStatus.Value = dataLayerEventArgs.CurrentValue;
        }

        #endregion

        #region IAppPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
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
        private async void cmdFromLocation_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                this.SelFromLocMap = LocMap.CurrLocMap;
                this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdFromLocation.Text = "From " + LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdFromLocation.Text = "Set From Location";
                }
            }

            this.ScannerEnable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdToLocation_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                this.SelToLocMap = this.LocMap.CurrLocMap;
                this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdToLocation.Text = "To " + LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdToLocation.Text = "Set To Location";
                }
            }

            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityMoveInventory_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                if (this.IsControlLoaded == false)
                {
                    this.SelFromLocMap = this.LocMap.CurrLocMap;
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;
                    
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdFromLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdFromLocation.Text = "Set From Location";
                    }

                    this.SelToLocMap = this.LocMap.CurrLocMap;
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdToLocation.Text = "Set To Location";
                    }

                    this.RefreshData();

                    this.IsControlLoaded = true;
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            int prodUnitRefno = 0;

            try
            {
                this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    await form.SaveChanges(sender, e);
                    XDocument xmlDoc = form.DisplayControlData;
                    prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);

                    await this.ActItem.Insert(xmlDoc, false);
                    this.ActDetail = this.ActDetail.GetInfo(0, true);

                    SysCommon.SFillQuantity fillQty = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActItem.ProdInfo, 1);
                    SActItemSchemeData tempItemSchemeData = new SActItemSchemeData
                    {
                        ActDetailQuantity = fillQty.Quantity,
                        ActDetailBaseQuantity = fillQty.BaseQuantity,
                        ActDetailWeightQuantity = fillQty.WeightQuantity
                    };

                    bool isInsertReady = false;
                    int foundRowIdx = -1;

                    if (this.ActDetail.DataViewDetails.Count > 0)
                    {
                        foundRowIdx = this.ActDetail.DataViewDetails.Count - 1;
                    }

                    this.BarCodeParse = BarCodeParseFacade.ParseData(string.Empty);

                    this.ActDetail.Quantity = 0;
                    this.ActDetail.BaseQuantity = 0;
                    this.ActDetail.WeightQuantity = 0;
                    this.ActDetail.SchemeID = string.Empty;
                    this.ActDetail.UniqueSchemeID = string.Empty;
                    this.ActDetail.ActExtra.Text1 = string.Empty;
                    this.ActDetail.ActExtra.Text2 = string.Empty;
                    this.ActDetail.ActExtra.Text3 = string.Empty;
                    this.ActDetail.ActExtra.Text4 = string.Empty;
                    this.ActDetail.ActExtra.DateTime5 = new DateTime(1980, 1, 1);

                    isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, true);
                    this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        await errorReport.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();
                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void KeyUpHandler(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.F1)
        //    {
        //        if (this.SaveChanges())
        //        {
        //            if (this.ReqCloseEvent != null)
        //            {
        //                this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //        else
        //        {
        //            this.CancelChanges();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //private void DisplayData()
        //{
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void updnBarCodeModifier1_GotFocus(object sender, EventArgs e)
        //{
        //    SysCommon.SelectValueInDomainUpDown((UpDownBase)sender);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="iCurrentRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="oBarCodeParse"></param>
        /// <remarks></remarks>
        private void InsertActivityDetail(int rowIdx, ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                //if (this.ActDetail.DataViewDetails.Count > 0)
                //{
                //    this.dataGridMainActivity.UnSelect(rowIdx);
                //}

                XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);
                this.ActDetail.Insert(xmlDoc);

                if (this.ActDetail.DataViewDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > this.ActDetail.DataViewDetails.Count - 1)
                {
                    //this.dataGridMainActivity.CurrentRowIndex = this.ActDetail.DataViewDetails.Count - 1;
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.ActDetail.DataViewDetails.Count - 1];
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
            XDocument xmlDoc = null;
            int prodUnitRefno = 0;
            //int rowIdx = this.dataGridMainActivity.CurrentRowIndex;
            int rowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

            try
            {
                this.BarCodeParse = scanArgs.ParseInfo;

                this.ScannerDisable();

                if (!string.IsNullOrEmpty(barCodeParse.LocationIdentifier))
                {
                    if (this.chkScanToLocation.IsChecked)
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));
                        this.SelToLocMap = LocMap.CurrLocMap;
                        
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.cmdToLocation.Text = "Set To Location";
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));
                        this.SelFromLocMap = this.LocMap.CurrLocMap;
                        
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            this.cmdFromLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.cmdFromLocation.Text = "Set From Location";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                    {
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarCodeParse, prodUnitRefno);

                        xmlDoc = XDocument.Parse(
                              "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                              + "<params>"
                              + ("<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>")
                              + "</params>");

                        await this.ActItem.Insert(xmlDoc, false);
                        this.ActDetail = this.ActDetail.GetInfo(0, true);

                        SActItemSchemeData tempItemSchemeData = new SActItemSchemeData();
                        bool isInsertReady = false;

                        int foundRowIdx = -1;
                        
                        if (this.ActDetail.DataViewDetails.Count > 0)
                        {
                            foundRowIdx = this.ActDetail.DataViewDetails.Count - 1;
                        }

                        isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, false);
                        this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);

                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.ActFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an header reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code # is not supported here.");
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning bar code type is not supported here.");
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);

                if (this.ActDetail.DataViewDetails.Count > 0 && rowIdx + 1 <= this.ActDetail.DataViewDetails.Count)
                {
                    //this.dataGridMainActivity.CurrentRowIndex = rowIdx;
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[rowIdx];
                }
            }
            catch (Exception ex)
            {   
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });

                if (this.ActDetail.DataViewDetails.Count > 0 && rowIdx + 1 <= this.ActDetail.DataViewDetails.Count)
                {
                    //this.dataGridMainActivity.CurrentRowIndex = rowIdx;
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[rowIdx];
                }
            }
            finally
            {
                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshData()
        {
            this.ScannerDisable();

            //this.pbarStatus.Visible = true;
            this.ItemsList.Clear();
            //this.pbarStatus.Visible = false;

            this.ActDetail.GetDataView(false);
            this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

            if (this.ActDetail.DataViewDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
            {
                this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                //this.dataGridMainActivity.Focus();
            }

            //this.DisplayData();
            this.ScannerEnable();
        }

        #endregion
    }
}