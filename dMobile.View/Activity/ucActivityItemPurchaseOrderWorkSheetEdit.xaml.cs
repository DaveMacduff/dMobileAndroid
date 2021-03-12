namespace dMobile.View.Activity
{
    using dMobile.View.CustomPages.Modal;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using System.Collections.ObjectModel;
    using dMobile.View.Forms;
    using dMobile.Resources.Enums;
    using Android.Views;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItemPurchaseOrderWorkSheetEdit : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ActDetailEdit m_actDetailEdit;
        private DataTable m_dtActivityDetails;
        private int m_iSelectedIndex = 0;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        //private UserControl m_ucCallingControl = null;
        private bool isEnterTrapped = false;
        private XDocument m_xmlDoc = null;
        private string m_strFullClassName = typeof(ucActivityItemPurchaseOrderWorkSheetEdit).FullName;
        private bool isNumericTrapped = false;
        private bool isEscapeTrapped = false;
        private ActDetail m_actDetail = null;
        private bool isEditTrapped = false;
        private SysCommon.ActSubTypes m_eActivitySubType = SysCommon.ActSubTypes.NotApplicable;
        private SysEnvironment m_oEnvironment;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList;

        #endregion
        
        #region Constructors

        public ucActivityItemPurchaseOrderWorkSheetEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_actDetail"></param>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityItemPurchaseOrderWorkSheetEdit(ActDetail _actDetail, SysCommon.ActSubTypes _ActivitySubTypes)
        {
            InitializeComponent();

            // Resource strings
            this.lblUnit.Text = "KG";
            this.lblQty.Text = "Quantity";
            this.btnSave.Text = "Save";
            this.btnCancel.Text = "Cancel";

            this.ItemsList = new ObservableCollection<string>();
            m_oEnvironment = SysEnvironment.GetInstance();

            eActivitySubType = _ActivitySubTypes;
            actDetailEdit = new ActDetailEdit();
            ActDetail = _actDetail;

            actDetailEdit.Quantity = ActDetail.Quantity;
            actDetailEdit.BaseQuantity = ActDetail.BaseQuantity;
            actDetailEdit.WeightQuantity = ActDetail.WeightQuantity;

            this.Title = this.GetControlDescription();
        }

        #endregion

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
                        this.DialogResult = DialogResult.None;
                        await CancelChanges();
                        ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                    else if (keycode == Keycode.Enter)
                    {
                        if (await SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                await this.ucActivityDetailEdit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK");
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

        private ReqCloseHandler ReqCloseEvent
        {
            get { return reqCloseEvent; }
            set { reqCloseEvent = value; }
        }

        private ReqSaveEnableHandler ReqSaveEnableEvent
        {
            get { return reqSaveEnableEvent; }
            set { reqSaveEnableEvent = value; }
        }

        private ReqSaveDisableHandler ReqSaveDisableEvent
        {
            get { return reqSaveDisableEvent; }
            set { reqSaveDisableEvent = value; }
        }

        private ReqCancelEnableHandler ReqCancelEnableEvent
        {
            get { return reqCancelEnableEvent; }
            set { reqCancelEnableEvent = value; }
        }

        private ReqCancelDisableHandler ReqCancelDisableEvent
        {
            get { return reqCancelDisableEvent; }
            set { reqCancelDisableEvent = value; }
        }

        public SysEnvironment oEnvironment
        {
            get
            {
                return m_oEnvironment;
            }
            set
            {
                m_oEnvironment = value;
            }
        }

        public SysCommon.ActSubTypes eActivitySubType
        {
            get
            {
                return m_eActivitySubType;
            }
            set
            {
                m_eActivitySubType = value;
            }
        }

        public bool IsEditTrapped
        {
            get
            {
                return isEditTrapped;
            }
            set
            {
                isEditTrapped = value;
            }
        }

        public bool IsEscapeTrapped
        {
            get
            {
                return isEscapeTrapped;
            }
            set
            {
                isEscapeTrapped = value;
            }
        }
        public bool IsNumericTrapped
        {
            get
            {
                return isNumericTrapped;
            }
            set
            {
                isNumericTrapped = value;
            }
        }

        public string strFullClassName
        {
            get
            {
                return m_strFullClassName;
            }
            set
            {
                m_strFullClassName = value;
            }
        }

        public XDocument xmlDoc
        {
            get
            {
                return m_xmlDoc;
            }
            set
            {
                m_xmlDoc = value;
            }
        }

        public int iSelectIndex
        {
            get
            {
                return m_iSelectedIndex;
            }
            set
            {
                m_iSelectedIndex = value;
            }
        }

        public DataTable dtActivityDetails
        {
            get
            {
                return m_dtActivityDetails;
            }
            set
            {
                m_dtActivityDetails = value;
            }
        }

        public ActDetailEdit actDetailEdit
        {
            get
            {
                return m_actDetailEdit;
            }
            set
            {
                m_actDetailEdit = value;
            }
        }

        public bool IsEnterTrapped
        {
            get
            {
                return isEnterTrapped;
            }
            set
            {
                isEnterTrapped = value;
            }
        }
        public bool IsControlLoaded
        {
            get
            {
                return isControlLoaded;
            }
            set
            {
                isControlLoaded = value;
            }
        }
        public bool IsChangesMade
        {
            get
            {
                return isChangesMade;
            }
            set
            {
                isChangesMade = value;
            }
        }

        //public UserControl ucCallingControl
        //{
        //    get
        //    {
        //        return m_ucCallingControl;
        //    }
        //    set
        //    {
        //        m_ucCallingControl = value;
        //    }
        //}

        public ActDetail ActDetail
        {
            get
            {
                return m_actDetail;
            }
            set
            {
                m_actDetail = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            //lstDisplay.Items.Clear();
            //lstDisplay.Items.Add("PROD: " + ActDetail.ActItem.ProdInfo.Code + ActDetail.ActItem.ProdInfo.UnitBC + "  " + ActDetail.ActItem.ProdInfo.Desc);
            //lstDisplay.Items.Add("SIZE: " + ActDetail.ActItem.ProdInfo.UnitSizeDesc);
            
            this.ItemsList.Clear();

            // TODO: Resource strings
            this.ItemsList.Add("PROD: " + ActDetail.ActItem.ProdInfo.Code + ActDetail.ActItem.ProdInfo.UnitBC + "  " + ActDetail.ActItem.ProdInfo.Desc);
            this.ItemsList.Add("SIZE: " + ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            SysCommon.SFillQuantity sQuantity = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)ActDetail.ActItem.ProdInfo, ActDetail.Quantity);

            actDetailEdit.Quantity = decimal.Round(sQuantity.Quantity, 3);
            actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
            actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);

            txtAltQty.Text = actDetailEdit.Quantity.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            bool IsRetVal = true;
            
            //if (!Information.IsNumeric(txtAltQty.Text))
            if (!decimal.TryParse(txtAltQty.Text, out _))
            {
                // TODO: Resource strings
                DisplayAlert("Error", "Invalid Quantity!", "OK");

                IsRetVal = false;
            }
            else
            {
                actDetailEdit.Quantity = decimal.Parse(txtAltQty.Text);
                SysCommon.SFillQuantity sQuantity = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)ActDetail.ActItem.ProdInfo, actDetailEdit.Quantity);
                actDetailEdit.Quantity = decimal.Round(sQuantity.Quantity, 3);
                actDetailEdit.BaseQuantity = decimal.Round(sQuantity.BaseQuantity, 3);
                actDetailEdit.WeightQuantity = decimal.Round(sQuantity.WeightQuantity, 3);
            }

            return IsRetVal;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async Task ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!IsControlLoaded)
                {
                    IsControlLoaded = true;
                    lblUnit.Text = ActDetail.ActItem.ProdInfo.UnitCode;

                    LoadData();
                    ShowProductInfo();
                    txtAltQty.Focus();
                }

                ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool IsReadyToSave = true;

            try
            {
                IsReadyToSave = IsReadyToSave && !(decimal.Parse(txtAltQty.Text) == 0);
                IsReadyToSave = IsReadyToSave && IsValidData();

            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "IsReadyToSave", Ex) == false)
                {
                    //Application.Exit();
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }

                IsReadyToSave = false;
            }

            return IsReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return !isChangesMade;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    ucOptionsMenu ucDisplayControl = null;
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
            return "dMobile Activity Detail Edit";
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
                    bSaveComplete = IsValidData();
                    isChangesMade = false;
                }
                else
                {
                    //MessageBox.Show("Data Failed Validation. Check Your Data.");
                    await DisplayAlert("Data Validation", "Data Failed Validation. Check Your Data.", "OK");
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "SaveChanges", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }

                bSaveComplete = false;
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
                    actDetailEdit.SchemeData = ActDetail.SchemeData;
                    actDetailEdit.Quantity = ActDetail.Quantity;
                    actDetailEdit.WeightQuantity = ActDetail.WeightQuantity;
                    actDetailEdit.BaseQuantity = ActDetail.BaseQuantity;

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
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
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
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<alt_qty>" + actDetailEdit.Quantity + "</alt_qty>") + ("<wt_qty>" + actDetailEdit.WeightQuantity + "</wt_qty>") + ("<base_qty>" + actDetailEdit.BaseQuantity + "</base_qty>") + "</params>");
            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtAltQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void ValidateEntry(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    try
        //    {
        //        if (sender == txtAltQty)
        //        {
        //            //e.Cancel = True
        //        }

        //    }
        //    catch (Exception Ex)
        //    {

        //        if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "ValidateEntry", Ex) == false)
        //        {
        //            Application.Exit();
        //        }
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (IsControlLoaded)
                {
                    isChangesMade = true;
                }
            }
            catch (Exception Ex)
            {

                if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                    //Application.Exit();
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void txtAltQty_TextChanged(object sender, EventArgs e)
        //{
        //    if (Information.IsNumeric(txtAltQty.Text))
        //    {
        //        var decQty = decimal.Parse(txtAltQty.Text);
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void NumericKeyPressHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    if (IsEnterTrapped)
        //    {
        //        e.Handled = true;
        //        SelectNextControl((System.Windows.Forms.Control)sender, true, false, false, true);
        //        IsEnterTrapped = false;

        //    }
        //    else if (IsEditTrapped)
        //    {
        //        e.Handled = false;
        //        IsEditTrapped = false;

        //    }
        //    else if (IsNumericTrapped)
        //    {
        //        e.Handled = false;
        //        IsNumericTrapped = false;

        //    }
        //    else if (IsEscapeTrapped)
        //    {
        //        e.Handled = false;
        //        IsEscapeTrapped = false;

        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

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

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            await this.Close();
        }
    }
}