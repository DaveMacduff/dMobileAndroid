using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.BarCodeScanner;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using System.Collections.ObjectModel;
using Java.Security;
using dMobile.View.CustomPages.Modal;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertProductDetail : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private int fromProductRefno;
        private int toProductRefno;
        private ProductInfo fromProduct;
        private ProductInfo toProduct;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private bool isControlLoaded = false;
        //private UserControl callingControl = null;
        private ActDetail actDetail = null;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityConvertProductDetail(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            InitializeComponent();

            // TODO: Resource strings
            this.cmdAdd.Text = "Add";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;

                this.ActSubType = actSubTypes;
                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.ActDetail.GetDataView(false);
                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                this.LocMap = this.ActDetail.ActItem.LocMap;

                this.ActDetail.ActItem.CurrRowIdx = -1;

                DataRow dr = this.ActDetail.ActItem.DataViewItems.Table.Rows[0];
                FromProdRefno = Convert.ToInt32(dr["from_prod_fk_refno"]);
                ToProdRefno = Convert.ToInt32(dr["to_prod_fk_refno"]);
                ProductInfo prodInfo = new ProductInfo();
                ToProduct = ProductInfoTasks.GetInfoFromRefno(ToProdRefno);
                FromProduct = ProductInfoTasks.GetInfoFromRefno(FromProdRefno);
                // fill in all the activity data
                this.ActDetail.ActItem.ProdInfo = ToProduct;

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

            this.lstDisplay.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        await AddManually();
                    }
                    else if (keycode == Keycode.F1 || keycode == Keycode.Escape)
                    {
                        await this.CloseForm();
                    }
                    else if (keycode == Keycode.Del)
                    {
                        // Commented out because we currently dont support it
                        // but wanted to. Uncomment to add most of this functionality
                        // back in.
                        //DeleteAdjustment();
                    }
                }
            });

            try
            {
                this.ucActivityConvertProductDetail_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityConvertProductDetail_GotFocus threw an error " + ex, "OK");
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

        /// <summary>
        /// The product we are changing out of.
        /// </summary>
        private int FromProdRefno
        {
            get
            {
                return this.fromProductRefno;
            }
            set
            {
                this.fromProductRefno = value;
            }
        }

        /// <summary>
        /// The desired product to change into.
        /// </summary>
        private int ToProdRefno
        {
            get
            {
                return this.toProductRefno;
            }
            set
            {
                this.toProductRefno = value;
            }
        }

        private ProductInfo ToProduct
        {
            get
            {
                return this.toProduct;
            }
            set
            {
                this.toProduct = value;
            }
        }

        private ProductInfo FromProduct
        {
            get
            {
                return this.fromProduct;
            }
            set
            {
                this.fromProduct = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.ActDetail.LocMap;
            }
            set
            {
                this.ActDetail.LocMap = value;
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

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
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = this.IsValidData();
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
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

                await Device.InvokeOnMainThreadAsync(async () =>
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
            return "Convert Products";
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
                    if (HasData() &&
                        //this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&
                        this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch && // batch means print at save instead of by item
                        !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // weren't allowed to print on add or scan, so print now

                        // TODO: Resource strings
                        if (await DisplayAlert("Print Label", "Print Product Labels?", "Yes", "No"))
                        {
                            await PrintBatchProductLabels();
                        }
                    }

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    //MessageBox.Show("Data failed validation. Check your data.");

                    // TODO: Resource strings
                    await DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK");
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

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
                if (await this.IsReadyToClose())
                {
                    // Change any values back that you need
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
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
        /// <returns></returns>
        /// <remarks></remarks>
        public XDocument GetXML()
        {
            string unqScheme = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            string scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            SProductSchemeData schemeData = this.ActDetail.SchemeData;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + schemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + schemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>")
                + ("<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>")
                + ("<scheme_id>" + scheme + "</scheme_id>")
                + ("<unq_scheme_id>" + unqScheme + "</unq_scheme_id>")
                + "</params>");

            return request;
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
            BarCodeParse parse = null;
            bool isInsertReady = false;

            try
            {
                this.ScannerDisable();

                SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));
                }
                else
                {
                    AssertNotLineItem(parse);
                    AssertNotLineItemDetail(parse);
                    AssertNotShippingContainer(parse);

                    int prodUnitRefno = 0;
                    prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, prodUnitRefno);
                    // Check if product scanned matches the FROM product
                    // Location is set in ConvertProduct screen

                    // Product identifier
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        int tempFromProdUnitRefno = 0;

                        tempFromProdUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, tempFromProdUnitRefno);

                        ProductInfo scannedProduct = new ProductInfo();
                        scannedProduct = ProductInfoTasks.GetInfoFromUnitRefno(tempFromProdUnitRefno);

                        //this.FromProdRefno = tempFromProdRefno;

                        // Insert a new activity detail record
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


                        // Assigns all data to the ActivityDetail
                        isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

                        // Confirm that scanned matches from data. If not, then what is wrong
                        if (scannedProduct.Code != FromProduct.Code)
                        {
                            throw new Exceptions.MessageOnlyException("Cannot scan a different \"From\" product than what was set.", null, SysCommon.ShowErrorMessageType.MessageBox);
                        }

                        XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                        await CreateAdjustment(xmlDoc);
                        // We check if we are allowed to print on scan
                        if (//this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct && // TODO possibly remove this.
                            !this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                            !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await PrintProductLabel();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();
            }
        }

        private static void AssertNotShippingContainer(BarCodeParse parse)
        {
            // Shipping container scanned, skid
            if (!string.IsNullOrEmpty(parse.SSCC18))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported here!");
            }
        }

        private static void AssertNotLineItemDetail(BarCodeParse parse)
        {
            // Activity detail fk refno
            if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here!");
            }
        }

        private static void AssertNotLineItem(BarCodeParse parse)
        {
            // Activity item fk refno
            if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning a line item bar code is not supported here!");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            return true;
        }


        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySummary()
        {
            this.ItemsList.Clear();

            if (this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                // TODO: Resource strings
                this.ItemsList.Add("Batch printing enabled.");
            }
            else
            {
                // TODO: Resource strings
                this.ItemsList.Add("Print labels on scan.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
        }


        /// <summary>
        /// Adding products manually instead of scanning.
        /// </summary>
        private async Task AddManually()
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = form.DisplayControlData;

                try
                {
                    this.Loading = true;

                    await Task.Run(async () =>
                    {
                        await CreateAdjustment(xmlDoc);

                    // We check if we are allowed to print on scan
                    if (
                            //(this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&// TODO remove true || (workaround com server not updated with value yet)
                            !this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                            !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await PrintProductLabel();
                        }

                    });
                }
                catch (Exception ex)
                {
                    // TODO: logging
                    // TODO: Resource strings
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    this.Loading = false;
                }
            }
        }

        /// <summary>
        /// Creates an adjustment for the product.
        /// </summary>
        /// <remarks>
        /// This sends what we wish to be an activity detail to the
        /// COM server. The COM server sees it doesn't exist and creates it,
        /// then it sends back three fk_refnos for activity, activity item, and activity detail.
        /// Once received it will then need to update the table values with the fk_refnos.
        /// </remarks>
        /// <param name="data">Data used for the row to be created.</param>
        private async Task CreateAdjustment(XDocument data)
        {
            try
            {
                // Create adjustment
                string request = this.ActDetail.CreateSubmitXML(data);
                string response = SysCommon.SubmitComm(request);
                SysCommon.CheckCommResponseForErrors(response);

                // Parse response for the updated fk_refnos
                // Modify and change the ActivityDetail/Item/Header to be up to date with the reponse fk_refnos
                XDocument responseData = XDocument.Parse(response);
                this.ActDetail.ActItem.ActHdr.FKRefno = Convert.ToInt32(responseData.Root.Element("fk_refno").Value);
                this.ActDetail.ActItem.FKRefno = Convert.ToInt32(responseData.Root.Element("fkit_refno").Value);
                this.ActDetail.FKRefno = Convert.ToInt32(responseData.Root.Element("ser_refno").Value);

                data.Root.Add(new XElement("hdr_refno", this.ActDetail.ActItem.ActHdr.FKRefno));
                data.Root.Add(new XElement("itm_refno", this.ActDetail.ActItem.FKRefno));
                data.Root.Element("ser_refno").SetValue(this.ActDetail.FKRefno);

                this.ActDetail.Insert(data);

                SelectRow(this.ActDetail.DataViewDetails.Count - 1);
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
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
        /// Removes the adjustment from happening.
        /// </summary>
        /// <remarks>
        /// Sends the fk_refno of the detail to remove to the COM server.
        /// </remarks>
        private async Task DeleteAdjustment()
        {
            if (NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                XDocument data = new XDocument();
                // TODO send the fk_refno to delete over
                string sendRequest = this.ActDetail.CreateDeleteXML(data);
                string response = SysCommon.SubmitComm(sendRequest);
                SysCommon.CheckCommResponseForErrors(response);

                // Remove from grid
                int selectedIndex = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
                this.ActDetail.DataViewDetails.Table.Rows.RemoveAt(selectedIndex);
                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }

                if (this.ActDetail.DataViewDetails.Table.Rows.Count > 0 && this.DataGridMainActivityItemsList.Count > selectedIndex)
                {
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[selectedIndex];
                }
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
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
        /// Prints all product labels if batch.
        /// </summary>
        private async Task PrintBatchProductLabels()
        {
            if (!this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                return;
            }

            int count = this.ActDetail.DataViewDetails.Count;

            for (int i = 0; i < count; ++i)
            {
                // Set the detail, item, header, product, scheme, scheme data info
                this.ActDetail.SelectDetail(i);

                // Print using that data
                await PrintProductLabel();
            }
        }

        /// <summary>
        /// Causes all current adjustments to pring prooduct labels.
        /// </summary>
        /// <remarks>
        /// Sends an xml string to the COM server to print the product labels.
        /// </remarks>
        private async Task PrintProductLabel()
        {
            if (NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                XDocument data = new XDocument();
                string sendRequest = this.ActDetail.CreateSubmitProductLabelXML(data);
                string response = SysCommon.SubmitComm(sendRequest);
                SysCommon.CheckCommResponseForErrors(response);
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
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
        /// Checks if the Activity Detail has 0 data rows.
        /// </summary>
        /// <returns>True if there are no data rows.</returns>
        private bool NoData()
        {
            return this.ActDetail.DataViewDetails.Count == 0;
        }

        private bool HasData()
        {
            return !NoData();
        }

        #endregion

        /// <summary>
        /// Saves or cancels changes and exits.
        /// </summary>
        private async Task CloseForm()
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

        /// <summary>
        /// Click handler for the Add button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            await AddManually();
        }

        private void ucActivityConvertProductDetail_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    // this.updnBarCodeModifier1.Text = this.SysEnv.CurrentBarCodeModifier1;

                    this.IsControlLoaded = true;
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        private void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectRow(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
        }

        private void SelectRow(int row)
        {
            if (row < 0)
            {
                return;
            }

            //this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);
            //this.dataGridMainActivity.Select(row);
            this.ActDetail.ActItem.CurrRowIdx = row;
            this.DisplaySummary();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.DataGridMainActivityItemsList = this.ActDetail.Search(e.NewTextValue);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
        }
    }
}