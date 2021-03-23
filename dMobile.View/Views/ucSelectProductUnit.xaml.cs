namespace dMobile.View
{
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using dMobile.Resources.Types;
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSelectProductUnit : DMobileModal, IAppArea, IAppPublisher
    {
        //private UserControl callingControl = null;
        private bool isChangesMade = false;
        private DataView dataViewProdUnits = null;
        private int prodUnitRefno = 0;
        private int prodRefno = 0;
        private string prodCode = string.Empty;
        private string prodDesc = string.Empty;
        private string weightUnitCode = string.Empty;
        private string baseUnitCode = string.Empty;
        private string unitCode = string.Empty;
        private string prodUnitBC = string.Empty;
        private bool prodUnitIsBC = false;
        private string prodUnitSizeDesc = string.Empty;
        private int prodRefnoFilter = 0;
        private int prodUnitRefnoFilter = 0;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.SelectProduct;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private bool isControlLoaded = false;
        private ObservableCollection<ProductUnit> productUnitsList;

        public ucSelectProductUnit()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        /// <param name="actSubType"></param>
        public ucSelectProductUnit(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");

            this.SysEnv = SysEnvironment.GetInstance();
            this.scanner = BarCodeSingleton.GetInstance().Scanner;
            this.ProductUnitsList = new ObservableCollection<ProductUnit>();
            //this.CallingControl = callingControl;
            this.ActSubType = actSubType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        /// <param name="prodRefnoFilter"></param>
        /// <param name="prodUnitRefnoExcludeFilter"></param>
        /// <param name="actSubType"></param>
        public ucSelectProductUnit(int prodRefnoFilter, int prodUnitRefnoExcludeFilter, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");
            this.cboSearch.SelectedItem = "Code";

            this.SysEnv = SysEnvironment.GetInstance();
            this.scanner = BarCodeSingleton.GetInstance().Scanner;
            this.ProductUnitsList = new ObservableCollection<ProductUnit>();
            //this.CallingControl = callingControl;
            this.ProdRefnoFilter = prodRefnoFilter;
            this.ProdUnitRefnoFilter = prodUnitRefnoExcludeFilter;
            this.ActSubType = actSubType;
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
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await this.Close();
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.DpadDown)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductUnitsList.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) < this.ProductUnitsList.Count - 1)
                    {
                        this.dgProducts.SelectedItem = this.ProductUnitsList[this.ProductUnitsList.IndexOf(this.dgProducts.SelectedItem as ProductUnit) + 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductUnitsList.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) > 0)
                    {
                        this.dgProducts.SelectedItem = this.ProductUnitsList[this.ProductUnitsList.IndexOf(this.dgProducts.SelectedItem as ProductUnit) - 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
            });

            try
            {
                await this.ucSelectProduct_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine("this.ucSelectProduct_GotFocus threw error " + ex);
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

        private int ProdUnitRefnoFilter
        {
            get
            {
                return this.prodUnitRefnoFilter;
            }

            set
            {
                this.prodUnitRefnoFilter = value;
            }
        }

        private int ProdRefnoFilter
        {
            get
            {
                return this.prodRefnoFilter;
            }

            set
            {
                this.prodRefnoFilter = value;
            }
        }

        private string ProdUnitSizeDesc
        {
            get
            {
                return this.prodUnitSizeDesc;
            }

            set
            {
                this.prodUnitSizeDesc = value;
            }
        }

        private string ProdUnitBC
        {
            get
            {
                return this.prodUnitBC;
            }

            set
            {
                this.prodUnitBC = value;
            }
        }

        private bool ProdUnitIsBC
        {
            get
            {
                return this.prodUnitIsBC;
            }

            set
            {
                this.prodUnitIsBC = value;
            }
        }

        private string UnitCode
        {
            get
            {
                return this.unitCode;
            }

            set
            {
                this.unitCode = value;
            }
        }

        private string BaseUnitCode
        {
            get
            {
                return this.baseUnitCode;
            }

            set
            {
                this.baseUnitCode = value;
            }
        }

        private string WeightUnitCode
        {
            get
            {
                return this.weightUnitCode;
            }

            set
            {
                this.weightUnitCode = value;
            }
        }

        private string ProdDesc
        {
            get
            {
                return this.prodDesc;
            }

            set
            {
                this.prodDesc = value;
            }
        }

        private string ProdCode
        {
            get
            {
                return this.prodCode;
            }

            set
            {
                this.prodCode = value;
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

        private DataView DataViewProdUnits
        {
            get
            {
                return this.dataViewProdUnits;
            }

            set
            {
                this.dataViewProdUnits = value;
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

        private int ProdRefno
        {
            get
            {
                return this.prodRefno;
            }

            set
            {
                this.prodRefno = value;
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

        public ObservableCollection<ProductUnit> ProductUnitsList
        {
            get
            {
                return this.productUnitsList;
            }

            set
            {
                this.productUnitsList = value;
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
            bool isReadyToClose = !this.IsChangesMade;

            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
            return "Select Product";
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
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exception("Data failed validation. Check your data.");
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
                    // Clear any properties if needed.
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
                    await this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;

                    // Re-enable the scanner, IsReadToClose calls this.ScannerDisable().
                    await this.ScannerEnable();
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<ProductUnitRefno>" + this.ProdUnitRefno + "</ProductUnitRefno>")
                + ("<ProductRefno>" + this.ProdRefno + "</ProductRefno>")
                + ("<ProductCode>" + this.ProdCode + "</ProductCode>")
                + ("<ProductDescription>" + SysCommon.AddXMLReservedChars(this.ProdDesc) + "</ProductDescription>")
                + ("<UnitCode>" + this.UnitCode + "</UnitCode>")
                + ("<BaseUnitCode>" + this.BaseUnitCode + "</BaseUnitCode>")
                + ("<WeightUnitCode>" + this.WeightUnitCode + "</WeightUnitCode>")
                + ("<ProductUnitBC>" + this.ProdUnitBC + "</ProductUnitBC>")
                + ("<ProductUnitIsBC>" + this.ProdUnitIsBC + "</ProductUnitIsBC>")
                + ("<ProductUnitSizeDescription>" + SysCommon.AddXMLReservedChars(this.ProdUnitSizeDesc)
                + "</ProductUnitSizeDescription>")
                + "</params>");

            return request;
        }

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
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Start();
                this.IsBarCodeHandlerRegistered = true;
            }
        }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucSelectProduct_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();

                this.Loading = true;
                await Task.Run(async () => { await this.LoadData(); });
                this.Loading = false;

                this.cboSearch.SelectedIndex = 0;

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.IsControlLoaded = true;
                //this.dgProducts.Focus();
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void dgProducts_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    this.SelectProdUnit(dgProducts.CurrentRowIndex);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        private void ApplySearchFilter(string search)
        {
            if (!string.IsNullOrEmpty(search) || (string.Compare(search.Trim(), string.Empty) > 0))
            {
                if (this.ProdRefnoFilter == 0)
                {
                    // Product unit filter specified.
                    if (this.ProdUnitRefnoFilter != 0)
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                    + " AND ProductCode LIKE \'%%" + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                    + " AND ProductUnitIsBC";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                    + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                    + " AND ProductUnitIsBC";
                            }

                            this.DataViewProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                        }
                        else
                        {
                            if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                            {
                                // ByCode
                                if (this.cboSearch.SelectedIndex == 0)
                                {
                                    this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                        + " AND ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                        + " AND ProductUnitIsBC";
                                }
                                else
                                {
                                    this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                        + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                        + " AND ProductUnitIsBC";
                                }
                            }
                            else
                            {
                                // By Code
                                if (this.cboSearch.SelectedIndex == 0)
                                {
                                    this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                        + " AND ProductCode LIKE \'%" + search.Trim() + "%\'";
                                }
                                else
                                {
                                    this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter
                                        + " AND ProductDescription LIKE \'%" + search.Trim() + "%\'";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductCode LIKE \'%" + search.Trim()
                                    + "%\' AND ProductUnitIsSellUnit";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductDescription LIKE \'%" + search.Trim()
                                    + "%\' AND ProductUnitIsSellUnit";
                            }

                            this.DataViewProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductCode LIKE \'%" + search.Trim() + "%\'";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductDescription LIKE \'%" + search.Trim() + "%\'";
                            }
                        }
                    }
                }
                else
                {
                    // Product unit filter specified.
                    if (this.ProdUnitRefnoFilter != 0)
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%"
                                    + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                    + " AND ProductUnitIsBC";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%"
                                    + search.Trim() + "%\' AND ProductUnitIsSellUnit"
                                    + " AND ProductUnitIsBC";
                            }
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%"
                                    + search.Trim() + "%\'";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%"
                                    + search.Trim() + "%\'";
                            }
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                            }
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductCode LIKE \'%" + search.Trim() + "%\'";
                            }
                            else
                            {
                                this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                                    + " AND ProductDescription LIKE \'%" + search.Trim() + "%\'";
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                {
                    this.DataViewProdUnits.RowFilter = "ProductUnitIsSellUnit";
                }
                else
                {
                    this.DataViewProdUnits.RowFilter = string.Empty;
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
            BarCodeParse parse = null;

            try
            {
                await this.ScannerDisable();

                parse = scanArgs.ParseInfo;

                // Global location identifier
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a global location identifier is not supported here!");
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(parse.ActFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning an activity bar code is not supported here!");
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a line item bar code is not supported here!");
                }

                // Activity detail fk refno
                if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a line item detail bar code is not supported here!");
                }

                // Shipping container scanned, skid
                if (!string.IsNullOrEmpty(parse.SSCC18))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a shipping container bar code is not supported here!");
                }

                // Product identifier
                if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.cboSearch.SelectedIndex = 0;
                        int maxLen = parse.ProductIdentifier.Length > 10 ? 10 : parse.ProductIdentifier.Length;
                        this.txtSearchProduct.Text = parse.ProductIdentifier.Substring(0, maxLen);
                        //this.txtSearchProduct.SelectAll();
                        this.txtSearchProduct.CursorPosition = 0;
                        this.txtSearchProduct.SelectionLength = this.txtSearchProduct.Text.Length;
                        this.txtSearchProduct.Focus();
                    });
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            this.LoadProdUnitList();
            this.ApplyFilter();
            await Device.InvokeOnMainThreadAsync(() => this.AddProductUnitToObservableCollection());
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadProdUnitList()
        {
            DataTable dataTableProdUnit = new DataTable();
            ProductInfoTasks.GetProductUnitList(ref dataTableProdUnit);
            dataTableProdUnit.TableName = "Products";
            this.DataViewProdUnits = dataTableProdUnit.DefaultView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        private async Task SelectProdUnit(ProductUnit prodUnit)
        {
            if (prodUnit == null)
            {
                return;
            }

            try
            {
                this.ProdUnitRefno = Convert.ToInt32(prodUnit.ProductUnitRefno);
                this.ProdRefno = Convert.ToInt32(prodUnit.ProductRefno);
                this.ProdDesc = Convert.ToString(prodUnit.ProductDescription);
                this.ProdCode = Convert.ToString(prodUnit.ProductCode);
                this.UnitCode = Convert.ToString(prodUnit.UnitCode);
                this.BaseUnitCode = Convert.ToString(prodUnit.BaseUnitCode);
                this.WeightUnitCode = Convert.ToString(prodUnit.WeightUnitCode);
                this.ProdUnitBC = Convert.ToString(prodUnit.ProductUnitBC);
                this.ProdUnitIsBC = Convert.ToBoolean(prodUnit.ProductUnitIsBC);
                this.ProdUnitSizeDesc = Convert.ToString(prodUnit.ProductUnitSizeDescription);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblProductDescription.Text = this.ProdCode + "  " + this.ProdDesc;
                });
            }
            catch (Exception ex)
            {
                // TODO: Logging
                await DisplayAlert("SelectProdUnit", ex.Message, "OK");
            }
        }

        private void ApplyFilter()
        {
            if (this.ProdRefnoFilter != 0)
            {
                if (this.ProdUnitRefnoFilter != 0)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter;
                    }
                    else
                    {
                        this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter
                            + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductUnitIsSellUnit"
                            + " AND ProductUnitIsBC";
                        this.DataViewProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                    }
                }
                else
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter;
                    }
                    else
                    {
                        this.DataViewProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitIsSellUnit";
                        this.DataViewProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                    }
                }
            }
            else
            {
                if (this.ProdUnitRefnoFilter != 0)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter;
                    }
                    else
                    {
                        this.DataViewProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter.ToString()
                            + " AND ProductUnitIsSellUnit AND ProductUnitIsBC";
                    }
                }
                else
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        this.DataViewProdUnits.RowFilter = string.Empty;
                    }
                    else
                    {
                        this.DataViewProdUnits.RowFilter = "ProductUnitIsSellUnit";
                    }
                }
            }
        }

        private void AddProductUnitToObservableCollection()
        {
            this.ProductUnitsList = new ObservableCollection<ProductUnit>();

            foreach (DataRow row in this.DataViewProdUnits.ToTable().Rows)
            {
                ProductUnit prodUnit = new ProductUnit
                {
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                    ProductDescription = Convert.ToString(row["ProductDescription"]),
                    ProductCode = Convert.ToString(row["ProductCode"]),
                    UnitCode = Convert.ToString(row["UnitCode"]),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"]),
                    WeightUnitCode = Convert.ToString(row["WeightUnitCode"]),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                    ProductUnitIsSellUnit = Convert.ToBoolean(row["ProductUnitIsSellUnit"]),
                };

                this.ProductUnitsList.Add(prodUnit);
            }

            this.dgProducts.ItemsSource = this.ProductUnitsList;

            if (this.ProductUnitsList.Count > 0 && this.dgProducts.SelectedItem == null)
            {
                this.dgProducts.SelectedItem = this.ProductUnitsList[0];
            } 
        }

        private async void TxtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    Entry entry = sender as Entry;
                    int startLength = entry.Text.Length;

                    await Task.Delay(300);

                    if (startLength == entry.Text.Length)
                    {
                        this.ApplySearchFilter(this.txtSearchProduct.Text);
                        this.AddProductUnitToObservableCollection();
                    }
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
        }

        private void TxtSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.txtSearchProduct.CursorPosition = 0;
            this.txtSearchProduct.SelectionLength = this.txtSearchProduct.Text.Length;
        }

        private async void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ProductUnit prodUnit = e.SelectedItem as ProductUnit;
            await this.SelectProdUnit(prodUnit);
        }

        private async void CmdSelect_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private async void CmdClose_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }
    }
}
