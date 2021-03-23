using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrTransferFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private TransferSettings transSettings = new TransferSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private ArrayList m_aSortBy3 = new ArrayList();
        private ArrayList m_aSortBy4 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrTransferFilter";
        private bool m_bChangesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ucCallingControl"></param>
        /// <remarks></remarks>
        public ucActivityHdrTransferFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblTransferDate.Text = "Transfer Date";
            this.lblApplyDate.Text = "Apply Transfer Date Filter";
            this.lblSort.Text = "SortBy";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";
            this.lbl4.Text = "4";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            //At least one sort option must be selected
            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add(new ComboBoxMembers("Transfer # Ascending", "ActivityTransferFKRefno ASC"));
            aSortBy1.Add(new ComboBoxMembers("Transfer # Descending", "ActivityTransferFKRefno DESC"));
            aSortBy1.Add(new ComboBoxMembers("Date Ascending", "ActivityTransferDateTime ASC"));
            aSortBy1.Add(new ComboBoxMembers("Date Descending", "ActivityTransferDateTime DESC"));
            aSortBy1.Add(new ComboBoxMembers("Whs # Ascending", "LocationMapWarehouseFKRefno ASC"));
            aSortBy1.Add(new ComboBoxMembers("Whs # Descending", "LocationMapWarehouseFKRefno DESC"));
            aSortBy1.Add(new ComboBoxMembers("Arrival Ascending", "LocationMapArrivalDateTime ASC"));
            aSortBy1.Add(new ComboBoxMembers("Arrival Descending", "LocationMapArrivalDateTime DESC"));
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(new ComboBoxMembers("", ""));
            aSortBy2.Add(new ComboBoxMembers("Transfer # Ascending", "ActivityTransferFKRefno ASC"));
            aSortBy2.Add(new ComboBoxMembers("Transfer # Descending", "ActivityTransferFKRefno DESC"));
            aSortBy2.Add(new ComboBoxMembers("Date Ascending", "ActivityTransferDateTime ASC"));
            aSortBy2.Add(new ComboBoxMembers("Date Descending", "ActivityTransferDateTime DESC"));
            aSortBy2.Add(new ComboBoxMembers("Whs # Ascending", "LocationMapWarehouseFKRefno ASC"));
            aSortBy2.Add(new ComboBoxMembers("Whs # Descending", "LocationMapWarehouseFKRefno DESC"));
            aSortBy2.Add(new ComboBoxMembers("Arrival Ascending", "LocationMapArrivalDateTime ASC"));
            aSortBy2.Add(new ComboBoxMembers("Arrival Descending", "LocationMapArrivalDateTime DESC"));
            this.cboSortBy2.ItemsSource = this.aSortBy2;
            this.cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;

            // TODO: Resource strings
            aSortBy3.Clear();
            aSortBy3.Add(new ComboBoxMembers("", ""));
            aSortBy3.Add(new ComboBoxMembers("Transfer # Ascending", "ActivityTransferFKRefno ASC"));
            aSortBy3.Add(new ComboBoxMembers("Transfer # Descending", "ActivityTransferFKRefno DESC"));
            aSortBy3.Add(new ComboBoxMembers("Date Ascending", "ActivityTransferDateTime ASC"));
            aSortBy3.Add(new ComboBoxMembers("Date Descending", "ActivityTransferDateTime DESC"));
            aSortBy3.Add(new ComboBoxMembers("Whs # Ascending", "LocationMapWarehouseFKRefno ASC"));
            aSortBy3.Add(new ComboBoxMembers("Whs # Descending", "LocationMapWarehouseFKRefno DESC"));
            aSortBy3.Add(new ComboBoxMembers("Arrival Ascending", "LocationMapArrivalDateTime ASC"));
            aSortBy3.Add(new ComboBoxMembers("Arrival Descending", "LocationMapArrivalDateTime DESC"));
            this.cboSortBy3.ItemsSource = this.aSortBy3;
            this.cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;

            // TODO: Resource strings
            aSortBy4.Clear();
            aSortBy4.Add(new ComboBoxMembers("", ""));
            aSortBy4.Add(new ComboBoxMembers("Transfer # Ascending", "ActivityTransferFKRefno ASC"));
            aSortBy4.Add(new ComboBoxMembers("Transfer # Descending", "ActivityTransferFKRefno DESC"));
            aSortBy4.Add(new ComboBoxMembers("Date Ascending", "ActivityTransferDateTime ASC"));
            aSortBy4.Add(new ComboBoxMembers("Date Descending", "ActivityTransferDateTime DESC"));
            aSortBy4.Add(new ComboBoxMembers("Whs # Ascending", "LocationMapWarehouseFKRefno ASC"));
            aSortBy4.Add(new ComboBoxMembers("Whs # Descending", "LocationMapWarehouseFKRefno DESC"));
            aSortBy4.Add(new ComboBoxMembers("Arrival Ascending", "LocationMapArrivalDateTime ASC"));
            aSortBy4.Add(new ComboBoxMembers("Arrival Descending", "LocationMapArrivalDateTime DESC"));
            this.cboSortBy4.ItemsSource = this.aSortBy4;
            this.cboSortBy4.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;

            this.filters.Add("", "");
            this.filters.Add("Transfer # Ascending", "ActivityTransferFKRefno ASC");
            this.filters.Add("Transfer # Descending", "ActivityTransferFKRefno DESC");
            this.filters.Add("Date Ascending", "ActivityTransferDateTime ASC");
            this.filters.Add("Date Descending", "ActivityTransferDateTime DESC");
            this.filters.Add("Whs # Ascending", "LocationMapWarehouseFKRefno ASC");
            this.filters.Add("Whs # Descending", "LocationMapWarehouseFKRefno DESC");
            this.filters.Add("Arrival Ascending", "LocationMapArrivalDateTime ASC");
            this.filters.Add("Arrival Descending", "LocationMapArrivalDateTime DESC");
        }

        #endregion

        #region OnAppearing/OnDisappearing

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

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape || keycode == Keycode.F1)
                    {
                        if (await SaveChanges())
                        {
                            ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                        else
                        {
                            await CancelChanges();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityFilter_GotFocus threw an error " + ex, "OK");
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

        public ArrayList aSortBy1
        {
            get
            {
                return m_aSortBy1;
            }
            set
            {
                m_aSortBy1 = value;
            }
        }

        public ArrayList aSortBy2
        {
            get
            {
                return m_aSortBy2;
            }
            set
            {
                m_aSortBy2 = value;
            }
        }

        public ArrayList aSortBy3
        {
            get
            {
                return m_aSortBy3;
            }
            set
            {
                m_aSortBy3 = value;
            }
        }

        public ArrayList aSortBy4
        {
            get
            {
                return m_aSortBy4;
            }
            set
            {
                m_aSortBy4 = value;
            }
        }

        public bool bEnterTrapped
        {
            get
            {
                return m_bEnterTrapped;
            }
            set
            {
                m_bEnterTrapped = value;
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

        public TransferSettings TransSettings
        {
            get
            {
                return transSettings;
            }
            set
            {
                transSettings = value;
            }
        }

        public string strSQLSortBy
        {
            get
            {
                return m_strSQLSortBy;
            }
            set
            {
                m_strSQLSortBy = value;
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

        public bool bChangesMade
        {
            get
            {
                return m_bChangesMade;
            }
            set
            {
                m_bChangesMade = value;
            }
        }

        #endregion

        #region Private Members

        private void MakeSQLSortBy()
        {
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            TransSettings.strSortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (cboSortBy2.SelectedIndex >= 0)
            {
                TransSettings.strSortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(TransSettings.strSortBy2Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.strSortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                TransSettings.strSortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(TransSettings.strSortBy3Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.strSortBy3Value;
                }
            }

            if (cboSortBy4.SelectedIndex >= 0)
            {
                TransSettings.strSortBy4Value = this.filters[cboSortBy4.Items[cboSortBy4.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(TransSettings.strSortBy4Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.strSortBy4Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            TransSettings = TransSettings.LoadData();

            if (SysCommon.IsEmptyDate(TransSettings.TransferDateFilter))
            {
                TransSettings.TransferDateFilter = DateTime.Now;
            }

            IsControlLoaded = true;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload oTransferSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!IsControlLoaded)
                {
                    LoadData();
                }

                chkApplyDate.IsToggled = TransSettings.IsApplyTransferDateFilter;
                dtpDate.Date = TransSettings.TransferDateFilter;

                cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;
                cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;
                cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;

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

            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "IsReadyToSave", Ex) == false)
                {
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
            return !bChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Activity Transfer Filter";
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
                    if (chkApplyDate.IsToggled)
                    {
                        TransSettings.TransferDateFilter = dtpDate.Date;
                    }

                    TransSettings.IsApplyTransferDateFilter = chkApplyDate.IsToggled;

                    TransSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    TransSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    TransSettings.iSortBy3Index = cboSortBy3.SelectedIndex;
                    TransSettings.iSortBy4Index = cboSortBy4.SelectedIndex;
                    MakeSQLSortBy();

                    TransferSettings temp_oTransferSettings = TransSettings;
                    TransSettings.SaveData(ref temp_oTransferSettings);
                    TransSettings = temp_oTransferSettings;

                    bSaveComplete = true;
                    bChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK");
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "SaveChanges", Ex) == false)
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
            bool bCancelChanges = true;

            try
            {
                if (!await IsReadyToClose())
                {
                    LoadData();

                    cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;
                    cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;
                    MakeSQLSortBy();

                    dtpDate.Date = TransSettings.TransferDateFilter;
                    chkApplyDate.IsToggled = TransSettings.IsApplyTransferDateFilter;

                    bCancelChanges = false;
                    bChangesMade = false;
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "CancelChanges", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }

                bCancelChanges = false;
            }

            return bCancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public XDocument GetXML()
        {
            XDocument request = System.Xml.Linq.XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                      "<params>" +
                      ("<transfer_date>" + TransSettings.TransferDateFilter.ToShortDateString() + "</transfer_date>") +
                      ("<sortby>" + strSQLSortBy + "</sortby>") +
                      "</params>");

            return request;
        }

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
                    bChangesMade = true;
                }
            }
            catch (Exception Ex)
            {

                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }
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

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}