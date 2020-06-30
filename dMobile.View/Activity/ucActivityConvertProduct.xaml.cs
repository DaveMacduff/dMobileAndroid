using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.CustomPages;
using System.Xml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using System.Data.SQLite;
using Android.Views;
using System.Collections.ObjectModel;

namespace dMobile.View.Activity
{
    public partial class ucActivityConvertProduct : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucInventoryMenu();
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private int fromProdRefno = 0;
        private int toProdRefno = 0;
        private ProductInfo fromProductInfo;
        private ProductInfo toProductInfo;
        private SysCommon.ActSubTypes actSubType;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ucActivityConvertProduct()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectToProduct.Text = "Select To Product";
            this.cmdSelectFromProduct.Text = "Select From Product";
            this.cmdDetail.Text = "Detail";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

                InitBarcodeModifier();

                this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertProduct);
                this.Activity.SkidInfo = new SysSkidInfo();
                this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertProduct;

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.ConvertProduct);
                this.ActItem.ActSubType = this.Activity.ActSubType;
                this.ActItem.GetDataView(false);

                this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertProduct);
                this.ActDetail.ActSubType = this.ActItem.ActSubType;

                this.ActDetail.GetDataView(false);

                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        /// Initializes the barcode modifier.
        /// </summary>
        private void InitBarcodeModifier()
        {
            this.updnBarCodeModifier1.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.updnBarCodeModifier1.Items.Add(" ");
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
                    if (keycode == Keycode.Escape)
                    {
                        await this.CancelChanges();
                    }
                    else if (keycode == Keycode.F1)
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
                this.ucActivityConvertUnit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityConvertUnit_GotFocus threw an error " + ex, "OK");
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void KeyUpHandler(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter || e.KeyValue == 134)
        //    {
        //        if (sender == this.cmdDetail)
        //        {
        //            ShowConvertProductDetailsForm();
        //        }
        //        else if (sender == this.cmdLocation)
        //        {
        //            ShowSelectLocationForm();
        //        }
        //        else if (sender == this.cmdSelectFromProduct)
        //        {
        //            ShowSelectFromProductForm();
        //        }
        //        else if (sender == this.cmdSelectToProduct)
        //        {
        //            ShowSelectToProductForm();
        //        }
        //        else if (this.SaveChanges())
        //        {
        //            if (this.ReqCloseEvent != null)
        //            {
        //                this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //    }
        //    else if (e.KeyCode == Keys.Escape)
        //    {
        //        this.CancelChanges();
        //    }
        //    else if (e.KeyCode == Keys.F1)
        //    {
        //        if (this.SaveChanges())
        //        {
        //            if (this.ReqCloseEvent != null)
        //            {
        //                this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //    }
        //}

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

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

        private int FromProdRefno
        {
            get
            {
                return this.fromProdRefno;
            }

            set
            {
                this.fromProdRefno = value;
                (this.ActItem as ConvertProductActItem).FromProdRefno = value;
            }
        }

        private int ToProdRefno
        {
            get
            {
                return this.toProdRefno;
            }

            set
            {
                this.toProdRefno = value;
                (this.ActItem as ConvertProductActItem).ToProdRefno = value;
            }
        }

        private ProductInfo FromProdInfo
        {
            get
            {
                return this.fromProductInfo;
            }
            set
            {
                this.fromProductInfo = value;
                this.FromProdRefno = value.Refno;
            }
        }


        private ProductInfo ToProdInfo
        {
            get
            {
                return this.toProductInfo;
            }
            set
            {
                this.toProductInfo = value;
                this.ToProdRefno = value.Refno;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

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

        #endregion



        #region IAppArea

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
        /// <remarks>Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        /// Must have a try catch block.</remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                AssertLocationSet();
                AssertFromProductSet();

                AssertToProductSet();
                AssertDetailsSet();
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
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
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
                isReadyToClose = false;
                
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        ///// <summary>
        ///// Get the Menu/Screen to return to after this screen is closed.
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return new ucInventoryAdjustmentsMenu();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Convert Product";
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
                    if (this.FromProdRefno != 0 && this.ActItem.DataViewItems.Count != 0 && this.ActDetail.DataViewDetails.Count != 0)
                    {
                        saveComplete = true;
                        this.IsChangesMade = false;
                    }
                    else
                    {
                        AssertLocationSet();
                        AssertFromProductSet();
                        AssertToProductSet();
                        AssertDetailsSet();
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
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

        //private static DialogResult AskToPrintProductLabels()
        //{
        //    return MessageBox.Show(
        //    "Unit conversion complete." + Environment.NewLine +
        //      "Do you wish to print product labels?",
        //      "Unit Conversion",
        //     MessageBoxButtons.YesNo,
        //      MessageBoxIcon.Question,
        //      MessageBoxDefaultButton.Button2);
        //}

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
                    this.ClearContents();
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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
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
        public XDocument GetXML()
        {
            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<from_prod_fk_refno>" + ((this.FromProdInfo != null) ? this.FromProdInfo.Refno.ToString() : string.Empty) + "</from_prod_fk_refno>"
                + "<to_prod_fk_refno>" + ((this.ToProdInfo != null) ? this.ToProdInfo.Refno.ToString() : string.Empty) + "</to_prod_fk_refno>"
                + "<location_map_refno>" + this.LocMap.CurrLocMapRefno + "</location_map_refno>"
                + "</params>");

            return xmlDoc;
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


        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            return; // no barcode handler
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ValidateEntry(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = false;

        //    try
        //    {
        //        // buttons ignore cancel = true.
        //        if (sender == this.cmdLocation)
        //        {
        //        }

        //        if (sender == this.cmdSelectFromProduct)
        //        {
        //        }

        //        if (sender == this.cmdSelectToProduct)
        //        {
        //        }

        //        if (sender == this.updnBarCodeModifier1)
        //        {
        //        }

        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        e.Cancel = true;
        //        ex.Show(this, this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Cancel = false;
        //        using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
        //        {
        //            errRpt.ShowDialog();
        //        }
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ClearContents()
        {
            //this.SuspendLayout();
            this.FromProdRefno = 0;
            this.ToProdRefno = 0;
            this.LocMap.CurrLocMap = new SLocMap();
            this.LocMap = LocationMap.GetInstance();
            this.ActItem.Delete();
            this.ActDetail.Delete();
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectFromProduct.Text = "Select From Product";
            this.cmdSelectToProduct.Text = "Select To Product";
            //this.ResumeLayout(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private string MakeProductLabelsSubmitString(int idx)
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)detail[idx]["ProductUnitRefno"]);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
            req.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
            req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
            req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNumber"]);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
            req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
            req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            await ShowSelectLocationForm();
        }

        /// <summary>
        /// Shows the select location form.
        /// </summary>
        private async Task ShowSelectLocationForm()
        {
            this.ScannerDisable();
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdLocation.Text = "Set Location";
                }
            }

            this.ScannerEnable();
            DisplaySummary();
        }




        /// <summary>
        /// Throws an exception if Location is not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertLocationSet()
        {
            if (!IsLocationSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Location must be set.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }

        }

        /// <summary>
        /// Checks if the location is selected.
        /// </summary>
        /// <returns></returns>
        private bool IsLocationSet()
        {
            return LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno);
        }


        /// <summary>
        /// Throws an exception if FROM Product is not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertFromProductSet()
        {
            if (!IsFromProductSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Please set the from product unit first.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }


        /// <summary>
        /// Checks if the TO product matches the FROM product.
        /// If they match, throw a Exceptions.MessageOnlyException.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException"/>
        private void AssertToDoesNotEqualFrom()
        {
            if (this.ToProdRefno == this.FromProdRefno)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("TO Product cannot be the same as FROM Product.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }

        /// <summary>
        /// Checks if the FROM product is selected.
        /// </summary>
        /// <returns></returns>
        private bool IsFromProductSet()
        {
            return this.FromProdRefno != 0;
        }

        /// <summary>
        /// Checks if the convert TO product item is set.
        /// Throws if not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertToProductSet()
        {

            if (!IsToProductSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("No convert to product has be set.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }
        /// <summary>
        /// Checks if the TO product is selected.
        /// </summary>
        /// <returns></returns>
        private bool IsToProductSet()
        {
            return this.ToProdRefno != 0;
        }

        /// <summary>
        /// Checks if the convert TO product item details is set.
        /// Throws if not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertDetailsSet()
        {
            if (0 >= this.ActDetail.DataViewDetails.Count)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("No convert to product details have be set.");
            }
        }


        /// <summary>
        /// Selects a Product and returns the ProdRefno if found.
        /// </summary>
        /// <param name="filterAllowProductChange">True to filter the list to only products with AllowChangeProduct</param>
        /// <returns>ProdRefno if found; otherwise 0.</returns>
        private async Task<int> SelectProduct(bool filterAllowChangeProduct)
        {
            try
            {
                FormDialogContainer frmSelectProduct = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProduct, SysCommon.ActSubTypes.ConvertProduct);

                if (filterAllowChangeProduct)
                {
                    (frmSelectProduct.DisplayControl as ucSelectProduct).FilterOnAllowConvertProduct = true;
                }
                
                if (await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    XDocument xmlDocProduct = frmSelectProduct.DisplayControlData;
                    return Convert.ToInt32(xmlDocProduct.Root.Element("ProductRefno").Value);
                }
            }
            catch (FormatException)
            {
                // failed to convert to an integer
                // This is ignored and returned as a -1
            }
            catch (OverflowException)
            {
                // failed to convert to an integer.
                // This is ignored and returned as a -1
            }
            catch (SQLiteException ex)
            {   
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
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

            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            await ShowSelectFromProductForm();
        }

        /// <summary>
        /// Shows the Select FROM Product Form.
        /// </summary>
        private async Task ShowSelectFromProductForm()
        {
            this.ScannerDisable();

            try
            {
                AssertLocationSet();

                this.FromProdRefno = await SelectProduct(true);

                if (this.FromProdRefno != 0)
                {
                    ProductInfo prodInfo = ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);
                    prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.FromProdRefno);
                    FromProdInfo = prodInfo;

                    if (!FromProdInfo.IsAllowConvertProduct)
                    {
                        FromProdRefno = 0;
                        FromProdInfo = new ProductInfo();

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Cannot convert a product without the Product Flag: Convert Label Adjustment Allow Product Change.", null, SysCommon.ShowErrorMessageType.MessageBox);
                    }

                    AssignFromProductText();
                }

                //this.cmdSelectToProduct.Refresh();
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
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

            this.ScannerEnable();
            DisplaySummary();
        }

        /// <summary>
        /// Assigns the FROM Product button text to reflect the product selected.
        /// </summary>
        private void AssignFromProductText()
        {
            using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdBCUnit.CommandType = CommandType.Text;
                cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProductUnit.ProductUnitBC,"
                    + "tUnit.UnitCode "
                    + "FROM tProductUnit "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                    + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                    + "WHERE tProductUnit.ProductUnitRefno = " + this.FromProdInfo.UnitRefno;

                using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                {
                    if (drProdBCUnit.Read())
                    {
                        this.cmdSelectFromProduct.Text =
                            Convert.ToString(drProdBCUnit["ProductCode"]).TrimEnd()
                            + " " + Convert.ToString(drProdBCUnit["ProductDescription"]).TrimEnd();

                    }

                    drProdBCUnit.Close();
                }
            }
        }


        /// <summary>
        /// Assigns the TO Product button text to reflect the product selected.
        /// </summary>
        private void AssignToProductText()
        {
            using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdBCUnit.CommandType = CommandType.Text;
                cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode,"
                + "tProduct.ProductDescription,"
                + "tProductUnit.ProductUnitBC,"
                + "tUnit.UnitCode "
                + "FROM tProductUnit "
                + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                + "WHERE tProductUnit.ProductUnitRefno = " + this.ToProdInfo.UnitRefno;

                using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                {
                    if (drProdBCUnit.Read())
                    {
                        this.cmdSelectToProduct.Text =
                            Convert.ToString(drProdBCUnit["ProductCode"]).TrimEnd()
                            + " " + Convert.ToString(drProdBCUnit["ProductDescription"]).TrimEnd();

                    }

                    drProdBCUnit.Close();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectToUnit_Click(object sender, EventArgs e)
        {
            await ShowSelectToProductForm();
        }

        /// <summary>
        /// Shows the Select TO Product Form.
        /// </summary>
        private async Task ShowSelectToProductForm()
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl causes a click event to fire if it is the next control
                this.ScannerDisable();

                AssertLocationSet();

                this.ToProdRefno = await SelectProduct(false);

                this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                if (this.ToProdRefno != 0)
                {
                    ProductInfo prodInfo = ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    ToProdInfo = prodInfo;

                    AssignToProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
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

            DisplaySummary();
        }


        /// <summary>
        /// Launches the ConvertProductDetails Form.
        /// </summary>
        private async Task ShowConvertProductDetailsForm()
        {
            try
            {
                this.ScannerDisable();
                AssertLocationSet();
                AssertFromProductSet();
                AssertToProductSet();

                AssertToDoesNotEqualFrom();

                this.ActItem.Delete();

                this.ActItem.LocMap = this.LocMap;
                this.ActItem.ProdInfo = this.ToProdInfo;
                await this.ActItem.Insert(GetXML(), false);
                this.ActDetail = this.ActDetail.GetInfo(0, true);
                this.ActDetail.ActItem = this.ActItem;

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityConvertProductDetail, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    // Detail Save
                    // If save, we send to comm server
                }
                else
                {
                    // Detail Cancel
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            finally
            {
                this.ScannerEnable();
            }

            DisplaySummary();
        }

        /// <summary>
        /// Displays the listbox summary data.
        /// </summary>
        private void DisplaySummary()
        {
            //DataRowCollection rows = this.ActDetail.DataViewDetails.Table.Rows;
            //SysCommon.SFillQuantity fillQty = GetActivityItemQuantity();

            this.ItemsList.Clear();

            if (FromProdInfo != null)
            {
                // TODO: Resource strings
                this.ItemsList.Add(string.Format("From {0} {1} {2}", FromProdInfo.BaseUnitCode, FromProdInfo.UnitCode, FromProdInfo.UnitSizeDesc));
            }

            if (ToProdInfo != null)
            {
                // TODO: Resource strings
                this.ItemsList.Add(string.Format("To {0} {1} {2}", ToProdInfo.BaseUnitCode, ToProdInfo.UnitCode, ToProdInfo.UnitSizeDesc ));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private SysCommon.SFillQuantity GetActivityItemQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();
            DataView dv = this.ActDetail.DataViewDetails;
            
            if (dv != null)
            {
                int count = dv.Count;
                
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        fillQty.Quantity += Convert.ToDecimal(dv[i]["ActivityDetailQuantity"]);
                        fillQty.BaseQuantity += Convert.ToDecimal(dv[i]["ActivityDetailBaseQuantity"]);
                        fillQty.WeightQuantity += Convert.ToDecimal(dv[i]["ActivityDetailWeightQuantity"]);
                    }
                }
            }

            return fillQty;
        }

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityConvertUnit_GotFocus(object sender, EventArgs e)
        {
            Load();
        }

        /// <summary>
        /// Enables the form to be edited and loads.
        /// </summary>
        private void Load()
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdLocation.Text = "Set Location";
                    }

                    this.IsControlLoaded = true;
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.DisplaySummary();
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

                if (sender == this.updnBarCodeModifier1)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
                }
            }
        }

        #endregion

        private async void cmdDetail_Click(object sender, EventArgs e)
        {
            await ShowConvertProductDetailsForm();
        }

        private void updnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
        }
    }
}