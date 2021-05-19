using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using SQLite;
using System.Collections.ObjectModel;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using dMobile.View.CustomPages.Modal;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityInventoryInquiry : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private Controller.Activity activity;
        private ActItem actItem;
        private SysCommon.ActSubTypes actSubTypes;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private bool isEnterTrapped = false;
        //private UserControl callingControl = null;
        private LocationMap locMap;
        private int totalTicks = 0;
        private BarCodeParse barParse;
        private BarCodeScanner.BarCodeScanner scanner;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private string mainGridMappingName = "tLocationMapDefault";
        private List<object> listItems;
        private bool isControlLoaded = false;
        private ObservableCollection<object> dataGridMainItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityInventoryInquiry()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

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

                this.updnBarCodeModifier1.SelectedItem = Convert.ToString(this.SysEnv.CurrentBarCodeModifier1);

                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;
                //this.CallingControl = new ucInventoryMenu();
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquiry);
                this.ActItem.ProdInfo = new ProductInfo();
                this.ActItem.LocMap = this.LocMap;
                this.ActItem.SysEnv = this.SysEnv;

                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityInventoryInquiry(ProductInfo prodInfo)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

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

                this.updnBarCodeModifier1.SelectedItem = Convert.ToString(this.SysEnv.CurrentBarCodeModifier1);

                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;
                //this.CallingControl = new ucInventoryMenu();
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquiry);
                this.ActItem.ProdInfo = prodInfo;
                this.ActItem.LocMap = this.LocMap;
                this.ActItem.SysEnv = this.SysEnv;

                this.cmdSelectProduct.Text = prodInfo.Code;
                this.Title = this.GetControlDescription();
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

            this.dataGridMain.ItemsSource = this.DataGridMainItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
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
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                }
            });

            try
            {
                await this.ucActivityInventoryInquiry_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityInventoryInquiry_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

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

        private int TotalTicks
        {
            get
            {
                return this.totalTicks;
            }

            set
            {
                this.totalTicks = value;
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
                return this.actSubTypes;
            }

            set
            {
                this.actSubTypes = value;
            }
        }

        private string MainGridMappingName
        {
            get
            {
                return this.mainGridMappingName;
            }

            set
            {
                this.mainGridMappingName = value;
            }
        }

        private List<object> ListItems
        {
            get
            {
                return this.listItems;
            }

            set
            {
                this.listItems = value;
            }
        }

        public ObservableCollection<object> DataGridMainItemsList { get => dataGridMainItemsList; set => dataGridMainItemsList = value; }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
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
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                isReadyToSave = false;

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Inventory Inquiry", "Quantity inquiry is not supported in disconnected mode.", "OK"));
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

            if (isReadyToClose)
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
                {
                    await this.ScannerDisable();
                }
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            try
            {
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityInventoryInquirySummary, this.ActItem, this.ActSubType);
                await form.DisplayControl.ShowDialog();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return false;
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
            }
            else
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            }

            this.ActItem.LocMap = this.LocMap;
            //this.dataGridMain.DataSource = null;
            //this.dataGridMain.Refresh();

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSelectProduct_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            ProductInfo prodInfo = new ProductInfo();
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProduct, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                //await form.SaveChanges(sender, e);
                XDocument xmlArgs = form.DisplayControlData;
                prodInfo = await ProductInfoTasks.GetInfoFromRefno(Convert.ToInt32(xmlArgs.Root.Element("ProductRefno").Value));
                this.cmdSelectProduct.Text = prodInfo.Code;
            }
            else
            {
                // TODO: Resource strings
                this.cmdSelectProduct.Text = "Select Product";
            }

            this.ActItem.ProdInfo = prodInfo;
            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void tmrActivityInventoryInquiry_Tick(object sender, EventArgs e)
        {
            this.IsControlLoaded = true;

            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            this.cmdLocation.IsEnabled = true;
            this.cmdSelectProduct.IsEnabled = true;
            this.cmdDisplayDefault.IsEnabled = true;
            this.updnBarCodeModifier1.IsEnabled = true;

            this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
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
        /// <param name="scanArgs"></param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
            {
                this.Loading = true;
                this.BarParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                try
                {
                    if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning an activity reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning an activity item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                    {
                        int prodUnitRefno = 0;
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarParse, prodUnitRefno);

                        if (prodUnitRefno != 0)
                        {
                            ProductInfo tempProdInfo = new ProductInfo();
                            tempProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ActItem.ProdInfo = tempProdInfo;
                            //<issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(this.ActItem.ProdInfo, this.BarParse);
                            await Device.InvokeOnMainThreadAsync(() => this.cmdSelectProduct.Text = this.ActItem.ProdInfo.Code);
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdSelectProduct.Text = "Select Product");
                        }
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanning a shipping container bar code is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarParse.LocationIdentifier));

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanned bar code type is not supported here.");
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task ucActivityInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.tmrActivityInventoryInquiry_Tick(sender, e);
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridMain"></param>
        private void DesignDataGridProductOnly()
        {
            this.DataGridMainItemsList = new ObservableCollection<object>();

            foreach (ProductLocationMapType location in this.ListItems)
            {
                //ProductLocationMapType location = new ProductLocationMapType
                //{
                //    ProductCode = Convert.ToString(row["ProductCode"]),
                //    LocationMapType = Convert.ToString(row["LocationMapType"]),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"])
                //};

                this.DataGridMainItemsList.Add(location);
            }

            // TODO: Resource strings
            Label labelProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapTypeHeader = new Label { Text = "Default", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductDescriptionHeader = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductBrandDescriptionHeader = new Label { Text = "Brand", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapTypeHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductDescriptionHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductBrandDescriptionHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelProductCodeHeader);
            gridHeader.Children.Add(labelLocationMapTypeHeader);
            gridHeader.Children.Add(labelProductDescriptionHeader);
            gridHeader.Children.Add(labelProductBrandDescriptionHeader);

            Grid.SetColumn(labelProductCodeHeader, 0);
            Grid.SetColumn(labelLocationMapTypeHeader, 1);
            Grid.SetColumn(labelProductDescriptionHeader, 2);
            Grid.SetColumn(labelProductBrandDescriptionHeader, 3);

            Grid.SetRow(gridHeader, 0);

            this.dataGridMainHeader.Children.Add(gridHeader);

            foreach (ProductLocationMapType location in this.DataGridMainItemsList)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(location.LocationMapType);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductDescription);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductBrandDescription);

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                    }
                };

                Label labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductDescription = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductBrandDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelLocationMapType.SetBinding(Label.TextProperty, new Binding("LocationMapType"));
                labelProductDescription.SetBinding(Label.TextProperty, new Binding("ProductDescription"));
                labelProductBrandDescription.SetBinding(Label.TextProperty, new Binding("ProductBrandDescription"));

                gridData.Children.Add(labelProductCode);
                gridData.Children.Add(labelLocationMapType);
                gridData.Children.Add(labelProductDescription);
                gridData.Children.Add(labelProductBrandDescription);

                Grid.SetColumn(labelProductCode, 0);
                Grid.SetColumn(labelLocationMapType, 1);
                Grid.SetColumn(labelProductDescription, 2);
                Grid.SetColumn(labelProductBrandDescription, 3);

                return new ViewCell { View = gridData };
            });

            this.dataGridMain.ItemTemplate = dataTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridMain"></param>
        private void DesignDataGridLocationOnly()
        {
            this.DataGridMainItemsList = new ObservableCollection<object>();

            foreach (dynamic obj in this.ListItems)
            {
                //var location = new
                //{
                //    LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                //    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                //    LocationMapSlotName = Convert.ToString(row["LocationMapSlotName"]),
                //    LocationMapType = Convert.ToString(row["LocationMapType"])
                //};

                this.DataGridMainItemsList.Add(obj);
            }

            // TODO: Resource strings
            Label labelLocationMapWarehouseCodeHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapSectionNameHeader = new Label { Text = "Section", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapSlotNameHeader = new Label { Text = "Slot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapTypeHeader = new Label { Text = "Default", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapWarehouseCodeHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapSectionNameHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapSlotNameHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapTypeHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelLocationMapWarehouseCodeHeader);
            gridHeader.Children.Add(labelLocationMapSectionNameHeader);
            gridHeader.Children.Add(labelLocationMapSlotNameHeader);
            gridHeader.Children.Add(labelLocationMapTypeHeader);

            Grid.SetColumn(labelLocationMapWarehouseCodeHeader, 0);
            Grid.SetColumn(labelLocationMapSectionNameHeader, 1);
            Grid.SetColumn(labelLocationMapSlotNameHeader, 2);
            Grid.SetColumn(labelLocationMapTypeHeader, 3);

            Grid.SetRow(gridHeader, 0);

            this.dataGridMainHeader.Children.Add(gridHeader);

            foreach (dynamic location in this.DataGridMainItemsList)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapWarehouseCode));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapSectionName));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapSlotName));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapType));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                    }
                };

                Label labelLocationMapWarehouseCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapSectionName = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapSlotName = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };

                labelLocationMapWarehouseCode.SetBinding(Label.TextProperty, new Binding("LocationMapWarehouseCode"));
                labelLocationMapSectionName.SetBinding(Label.TextProperty, new Binding("LocationMapSectionName"));
                labelLocationMapSlotName.SetBinding(Label.TextProperty, new Binding("LocationMapSlotName"));
                labelLocationMapType.SetBinding(Label.TextProperty, new Binding("LocationMapType"));

                gridData.Children.Add(labelLocationMapWarehouseCode);
                gridData.Children.Add(labelLocationMapSectionName);
                gridData.Children.Add(labelLocationMapSlotName);
                gridData.Children.Add(labelLocationMapType);

                Grid.SetColumn(labelLocationMapWarehouseCode, 0);
                Grid.SetColumn(labelLocationMapSectionName, 1);
                Grid.SetColumn(labelLocationMapSlotName, 2);
                Grid.SetColumn(labelLocationMapType, 3);

                return new ViewCell { View = gridData };
            });

            this.dataGridMain.ItemTemplate = dataTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetDataViewProductOnlyPickingQuery()
        {
            string sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked && 0 < this.LocMap.CurrLocMap.SlotLocMapRefno)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked && 0 >= this.LocMap.CurrLocMap.SlotLocMapRefno)
            {
                // find all the slots for the selected section
                string sub = "(SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE (tLocationMapSlot.SectionLocationMapRefno = " + this.LocMap.CurrLocMapRefno + "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE (tProductLocationMapType.LocationMapRefno IN " + sub + ") "
                    + "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking
                    + ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked && !this.LocMap.CurrLocMap.IsSlotTracked && 0 < this.LocMap.CurrLocMap.SectLocMapRefno)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode, "
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking;
            }

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetDataViewProductOnlyReceivingQuery()
        {
            string sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked && 0 < this.LocMap.CurrLocMap.SlotLocMapRefno)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked && 0 >= this.LocMap.CurrLocMap.SlotLocMapRefno)
            {
                // find all the slots for the selected section
                string sub = "(SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE (tLocationMapSlot.SectionLocationMapRefno = " + this.LocMap.CurrLocMapRefno + "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE (tProductLocationMapType.LocationMapRefno IN " + sub + ") "
                    + "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving
                    + ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked && !this.LocMap.CurrLocMap.IsSlotTracked && 0 < this.LocMap.CurrLocMap.SectLocMapRefno)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving;
            }

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewProductOnly()
        {
            DataTable dataTableItems = new DataTable("tLocation");
            dataTableItems.Columns.Add("ProductCode", Type.GetType("System.String"));
            dataTableItems.Columns.Add("LocationMapType", Type.GetType("System.String"));
            dataTableItems.Columns.Add("ProductDescription", Type.GetType("System.String"));
            dataTableItems.Columns.Add("ProductBrandDescription", Type.GetType("System.String"));

            List<object> listLocationMapTypes = dataTableItems.Select().ToList<object>();

            string queryPickingOnly = this.GetDataViewProductOnlyPickingQuery();

            if (!string.IsNullOrEmpty(queryPickingOnly))
            {
                List<ProductLocationMapType> resultsPickingProductLocationMapType = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductLocationMapType>(queryPickingOnly);

                if (resultsPickingProductLocationMapType?.Count > 0)
                {
                    listLocationMapTypes.AddRange(resultsPickingProductLocationMapType);

                    //foreach (ProductLocationMapType productLocationMapType in resultsPickingProductLocationMapType)
                    //{
                    //    DataRow row = dataTableItems.NewRow();
                    //    row["ProductCode"] = Convert.ToString(drProdLocType["ProductCode"]);
                    //    row["LocationMapType"] = "Picking";
                    //    row["ProductBrandDescription"] = Convert.ToString(drProdLocType["ProductBrandDescription"]);
                    //    row["ProductDescription"] = Convert.ToString(drProdLocType["ProductDescription"]);
                    //    dataTableItems.Rows.Add(row);
                    //}
                }
            }

            string queryReceivingOnly = this.GetDataViewProductOnlyReceivingQuery();

            if (!string.IsNullOrEmpty(queryReceivingOnly))
            {
                List<ProductLocationMapType> resultsReceivingProductLocationMapType = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductLocationMapType>(queryReceivingOnly);

                if (resultsReceivingProductLocationMapType?.Count > 0)
                {
                    listLocationMapTypes.AddRange(resultsReceivingProductLocationMapType);

                    //while (drProdLocType.Read())
                    //{
                    //    DataRow row = dataTableItems.NewRow();
                    //    row["ProductCode"] = Convert.ToString(drProdLocType["ProductCode"]);
                    //    row["LocationMapType"] = "Receiving";
                    //    row["ProductBrandDescription"] = Convert.ToString(drProdLocType["ProductBrandDescription"]);
                    //    row["ProductDescription"] = Convert.ToString(drProdLocType["ProductDescription"]);
                    //    dataTableItems.Rows.Add(row);
                    //}
                }
            }

            //this.DataViewItems = dataTableItems.DefaultView;
            //this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //this.DataViewItems.AllowNew = false;
            //this.DataViewItems.AllowEdit = false;
            //this.DataViewItems.AllowDelete = false;
            //this.DataViewItems.Sort = "ProductCode,LocationMapType";

            this.ListItems = (List<object>)listLocationMapTypes.OfType<ProductLocationMapType>().OrderBy(locMapType => locMapType.ProductCode).ThenBy(locMapType => locMapType.LocationMapType);
            return this.ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewLocationOnly()
        {
            DataTable dataTableItems = new DataTable("tLocation");

            dataTableItems.Columns.Add("LocationMapWarehouseCode", Type.GetType("System.Int32"));
            dataTableItems.Columns.Add("LocationMapSectionName", Type.GetType("System.String"));
            dataTableItems.Columns.Add("LocationMapSlotName", Type.GetType("System.String"));
            dataTableItems.Columns.Add("LocationMapType", Type.GetType("System.String"));

            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
            SLocMap locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            DataRow row = dataTableItems.NewRow();
            row["LocationMapWarehouseCode"] = locMap.WhsCode;
            row["LocationMapSectionName"] = locMap.SectName;
            row["LocationMapSlotName"] = locMap.SlotName;
            row["LocationMapType"] = "Picking";
            dataTableItems.Rows.Add(row);

            locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);
            locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            row = dataTableItems.NewRow();
            row["LocationMapWarehouseCode"] = locMap.WhsCode;
            row["LocationMapSectionName"] = locMap.SectName;
            row["LocationMapSlotName"] = locMap.SlotName;
            row["LocationMapType"] = "Receiving";
            dataTableItems.Rows.Add(row);

            //this.DataViewItems = dataTableItems.DefaultView;
            //this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //this.DataViewItems.AllowNew = false;
            //this.DataViewItems.AllowEdit = false;
            //this.DataViewItems.AllowDelete = false;
            //this.DataViewItems.Sort = "LocationMapWarehouseCode,LocationMapSectionName,LocationMapSlotName,LocationMapType";

            this.ListItems = (List<object>)dataTableItems.Select().ToList().OfType<dynamic>().OrderBy(obj => obj.LocationMapWarehouseCode).ThenBy(obj => obj.LocationMapSectionName).ThenBy(obj => obj.LocationMapSlotName).ThenBy(obj => obj.LocationMapType);
            return this.ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdDisplayDefault_Click(object sender, EventArgs e)
        {
            await DisplayLocationDefaults();
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DisplayLocationDefaults()
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    if (0 < this.ActItem.ProdInfo.Refno)
                    {
                        this.ActItem.ListItems = await this.GetDataViewLocationOnly();

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.DesignDataGridLocationOnly();
                            this.dataGridMain.ItemsSource = this.DataGridMainItemsList;
                        });
                    }
                    else
                    {
                        // A warehouse only query will result in to much data. Restrict it to at least a section.
                        if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                        {
                            // TODO: Resource strings
                            throw new Exception("The set location must have a warehouse selected.");
                        }

                        if (this.LocMap.CurrLocMap.IsSectionTracked && 0 >= this.LocMap.CurrLocMap.SectLocMapRefno)
                        {
                            // TODO: Resource strings
                            throw new Exception("The set location must have a section selected.");
                        }

                        this.ActItem.ListItems = await this.GetDataViewProductOnly();

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.DesignDataGridProductOnly();
                            this.dataGridMain.ItemsSource = this.DataGridMainItemsList;
                        });
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.Loading = false;
                }
            });
        }

        #endregion

        private void updnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
        }

        private async void cmdView_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }
    }
}