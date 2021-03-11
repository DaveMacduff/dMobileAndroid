using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Views;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityMoveSkid : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucInventoryMovementMenu();
        private bool isControlLoaded = false;
        private bool isEnterTrapped = false;
        private string fullClassName = typeof(ucActivityMoveSkid).FullName;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private bool isNumericTrapped = false;
        private bool isEscapeTrapped = false;
        private bool isEditTrapped = false;
        private int prodUnitRefno = 0;
        private SysSkidInfo skidInfo;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SkidInfo;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityMoveSkid(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.cmdToLocation.Text = "Set To Location";
            this.lblProductBCUnit.Text = "Product";
            this.lblSkidCode.Text = "Skid #";

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SkidInfo = new SysSkidInfo();

            //this.CallingControl = callingControl;
            this.ActSubType = actSubType;

            this.Scanner = BarCodeSingleton.GetInstance().Scanner;

            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.SkidInfo);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.SkidInfo);
            this.ActItem.ActHdr = this.Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.SkidInfo);
            this.ActDetail.ActItem = this.ActItem;

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

            this.lstSkidStats.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Escape || keycode == Keycode.F1)
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
                else if (keycode == Keycode.Enter)
                {
                    await this.SkidCodeEnter();
                }
            });

            try
            {
                this.ucActivityMoveSkid_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucActivityMoveSkid_GotFocus threw an error " + ex, "OK");
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

        public bool IsBarCodeHandlerRegistered
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

        public bool IsControlLoaded
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

        public bool IsChangesMade
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

        public bool IsEnterTrapped
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

        //public UserControl CallingControl
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

        private SysSkidInfo SkidInfo
        {
            get
            {
                return this.skidInfo;
            }

            set
            {
                this.skidInfo = value;
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

        private bool IsNumericTrapped
        {
            get
            {
                return this.isNumericTrapped;
            }

            set
            {
                this.isNumericTrapped = value;
            }
        }

        private bool IsEscapeTrapped
        {
            get
            {
                return this.isEscapeTrapped;
            }

            set
            {
                this.isEscapeTrapped = value;
            }
        }

        private bool IsEditTrapped
        {
            get
            {
                return this.isEditTrapped;
            }

            set
            {
                this.isEditTrapped = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
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

            isReadyToSave = isReadyToSave && LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno);
            isReadyToSave = isReadyToSave && this.SkidInfo.CurrSkid.SkidFKRefno != 0;

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            if (this.Scanner != null)
            {
                this.ScannerDisable();
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
            return "Skid Info";
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
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
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
            XDocument xmlDoc = null;

            if (this.SkidInfo.CurrSkid.SkidFKRefno == 0)
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno></skid_fk_refno>"
                    + "<chep></chep>"
                    + "<hold></hold>"
                    + "<base_quantity></base_quantity>"
                    + "<cube></cube>"
                    + "<height></height>"
                    + "<quantity></quantity>"
                    + "<weight_quantity></weight_quantity>"
                    + "<hi></hi>"
                    + "<ti></ti>"
                    + "<cube_unit></cube_unit>"
                    + "<height_unit></height_unit>"
                    + "<logistic_unit_skid_code></logistic_unit_skid_code>"
                    + "<skid_code></skid_code>"
                    + "<location_map_refno></location_map_refno>"
                    + "<product_unit_refno></product_unit_refno>"
                    + "</params>");
            }
            else
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + ("<skid_fk_refno>" + this.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                    + ("<chep>" + this.SkidInfo.CurrSkid.IsCHEP + "</chep>")
                    + ("<hold>" + this.SkidInfo.CurrSkid.IsHold + "</hold>")
                    + ("<base_quantity>" + this.SkidInfo.CurrSkid.BaseQuantity + "</base_quantity>")
                    + ("<cube>" + this.SkidInfo.CurrSkid.Cube + "</cube>")
                    + ("<height>" + this.SkidInfo.CurrSkid.Height + "</height>")
                    + ("<quantity>" + this.SkidInfo.CurrSkid.Quantity + "</quantity>")
                    + ("<weight_quantity>" + this.SkidInfo.CurrSkid.WeightQuantity + "</weight_quantity>")
                    + ("<hi>" + this.SkidInfo.CurrSkid.Hi + "</hi>")
                    + ("<ti>" + this.SkidInfo.CurrSkid.Ti + "</ti>")
                    + ("<cube_unit>" + this.SkidInfo.CurrSkid.CubeUnit + "</cube_unit>")
                    + ("<height_unit>" + this.SkidInfo.CurrSkid.HeightUnit + "</height_unit>")
                    + ("<logistic_unit_skid_code>" + this.SkidInfo.CurrSkid.LogisticUnitSkidCode + "</logistic_unit_skid_code>")
                    + ("<skid_code>" + this.SkidInfo.CurrSkid.SkidCode + "</skid_code>")
                    + ("<location_map_refno>" + this.SkidInfo.CurrSkid.LocMapRefno + "</location_map_refno>")
                    + ("<product_unit_refno>" + this.SkidInfo.CurrSkid.ProdInfo.UnitRefno + "</product_unit_refno>")
                    + "</params>");
            }

            return xmlDoc;
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
                    this.SkidInfo = SysSkidInfo.Move(this.SkidInfo, this.LocMap.CurrLocMap);

                    //if (MessageBox.Show("Do you wish to move any more skid(s)?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    // TODO: Resource strings
                    if (!await DisplayAlert(this.GetControlDescription(), "Do you wish to move any more skid(s)?", "Yes", "No"))
                    {
                        saveComplete = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), ex.Message, "OK");
            }

            return saveComplete;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void KeyPressHander(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    if (this.IsEnterTrapped)
        //    {
        //        e.Handled = true;
        //        this.SelectNextControl((System.Windows.Forms.Control)sender, true, false, false, true);
        //        this.IsEnterTrapped = false;
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityMoveSkid_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

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
        public async void cmdToLocation_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdToLocation.Text = "Set Location";
                }
            }

            this.ScannerEnable();
        }

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
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
            {
                BarCodeParse parse = null;

                try
                {
                    this.Loading = true;

                    this.ScannerDisable();

                    parse = scanArgs.ParseInfo;

                    // GLOBAL LOCATION IDENTIFIER SCANNED
                    if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                    {
                        int locMapRefno = LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier);

                        if (locMapRefno > 0)
                        {
                            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                        }

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set Location");
                        }
                    }

                    // ACTIVITY ITEM FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning a line item bar code is not supported here!");
                    }

                    // ACTIVITY DETAIL FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning an line item detail bar code is not supported here!");
                    }

                    // SHIPPING CONAINTER SCANNED, SKID
                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.SSCC18);

                        if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.SkidInfo.CurrSkid.LocMapRefno);
                        }

                        await Device.InvokeOnMainThreadAsync(() => this.DisplaySkidInfo());

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set Location");
                        }
                    }

                    // PRODUCT IDENTIFIER
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning a shipping container bar code is not supported here!");
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), ex.Message, "OK"));
                }
                finally
                {
                    this.ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySkidInfo()
        {
            this.ItemsList.Clear();

            if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode.ToString();

                // TODO: Resource strings
                this.lblProductBCUnit.Text = string.Format("Product: {0} {1} {2}", this.SkidInfo.CurrSkid.ProdInfo.Code, this.SkidInfo.CurrSkid.ProdInfo.UnitBC, this.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                // TODO: Resource strings
                this.ItemsList.Add(string.Format("Bar Code: {0}", this.SkidInfo.CurrSkid.LogisticUnitSkidCode));

                // TODO: Resource strings
                string qty = string.Format("Qty: {0} {1}", Math.Round(this.SkidInfo.CurrSkid.Quantity, 3), this.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                this.ItemsList.Add(qty);

                // TODO: Resource strings
                this.ItemsList.Add(string.Format("Weight: {0} {1}", Math.Round(this.SkidInfo.CurrSkid.WeightQuantity, 3), this.SkidInfo.CurrSkid.ProdInfo.WeightUnitCode));
                this.ItemsList.Add(string.Format("Ti: {0} Hi: {1}", this.SkidInfo.CurrSkid.Ti, this.SkidInfo.CurrSkid.Hi));
                this.ItemsList.Add(string.Format("Height: {0} {1}", this.SkidInfo.CurrSkid.Height, this.SkidInfo.CurrSkid.HeightUnit));
                this.ItemsList.Add(string.Format("Cube: {0} {1}", this.SkidInfo.CurrSkid.Cube, this.SkidInfo.CurrSkid.CubeUnit));
                this.ItemsList.Add(string.Format("CHEP: {0}", this.SkidInfo.CurrSkid.IsCHEP));
                this.ItemsList.Add(string.Format("Hold: {0}", this.SkidInfo.CurrSkid.IsHold));
            }
            else
            {
                // TODO: Resource strings
                this.ItemsList.Add("Skid does not exist.");
            }

            //this.lstSkidStats.SelectedIndex = 0;
        }

        /// <summary>
        /// Handle Enter press 
        /// </summary>
        /// <returns></returns>
        private async Task SkidCodeEnter()
        {
            try
            {
                this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(this.txtSkidCode.Text);

                if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.SkidInfo.CurrSkid.LocMapRefno);
                }

                this.DisplaySkidInfo();
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), ex.Message, "OK");
            }

            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            }
            else
            {
                // TODO: Resource strings
                this.cmdToLocation.Text = "Set Location";
            }
        }

        private async void txtSkidCode_Completed(object sender, EventArgs e)
        {
            await this.SkidCodeEnter();
        }

        #endregion
    }
}