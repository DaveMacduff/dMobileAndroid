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
    public partial class ucActivityHdrReceivingFilter : DMobileModal, IAppArea, IAppPublisher
    {        
        #region Fields

        private ReceivingSettings recvSettings = new ReceivingSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private ArrayList m_aSortBy3 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrReceivingFilter";
        private bool m_bChangesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public ucActivityHdrReceivingFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyIssuedDate.Text = "Apply Issued Date Filter";
            this.lblPODate.Text = "Issued";
            this.lblApplyArrivalDate.Text = "Apply Arrival Date Filter";
            this.lblArrivalDate.Text = "Arrival";
            this.lblSort.Text = "Sort";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";

            if (!DesignMode.IsDesignModeEnabled)
            {
                //At least one sort option must be selected
                aSortBy1.Clear();

                aSortBy1.Add(new ComboBoxMembers("Purchase Order # Ascending", "ActivityReceivingPONumber ASC"));
                aSortBy1.Add(new ComboBoxMembers("Purchase Order # Descending", "ActivityReceivingPONumber DESC"));
                aSortBy1.Add(new ComboBoxMembers("Issued Date Ascending", "ActivityReceivingIssuedDateTime ASC"));
                aSortBy1.Add(new ComboBoxMembers("Issued Date Descending", "ActivityReceivingIssuedDateTime DESC"));
                aSortBy1.Add(new ComboBoxMembers("Arrival Ascending", "ActivityReceivingArrivalDateTime ASC"));
                aSortBy1.Add(new ComboBoxMembers("Arrival Descending", "ActivityReceivingArrivalDateTime DESC"));

                cboSortBy1.ItemsSource = aSortBy1;
                cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
                //cboSortBy1.ValueMember = "ValueMember";
                cboSortBy1.SelectedIndex = RecvSettings.iSortBy1Index;

                aSortBy2.Clear();
                aSortBy2.Add(new ComboBoxMembers("", ""));

                aSortBy2.Add(new ComboBoxMembers("Purchase Order # Ascending", "ActivityReceivingPONumber ASC"));
                aSortBy2.Add(new ComboBoxMembers("Purchase Order # Descending", "ActivityReceivingPONumber DESC"));
                aSortBy2.Add(new ComboBoxMembers("Issued Date Ascending", "ActivityReceivingIssuedDateTime ASC"));
                aSortBy2.Add(new ComboBoxMembers("Issued Date Descending", "ActivityReceivingIssuedDateTime DESC"));
                aSortBy2.Add(new ComboBoxMembers("Arrival Ascending", "ActivityReceivingArrivalDateTime ASC"));
                aSortBy2.Add(new ComboBoxMembers("Arrival Descending", "ActivityReceivingArrivalDateTime DESC"));

                cboSortBy2.ItemsSource = aSortBy2;
                cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
                //cboSortBy2.ValueMember = "ValueMember";
                cboSortBy2.SelectedIndex = RecvSettings.iSortBy2Index;

                aSortBy3.Clear();
                aSortBy3.Add(new ComboBoxMembers("", ""));

                aSortBy3.Add(new ComboBoxMembers("Purchase Order # Ascending", "ActivityReceivingPONumber ASC"));
                aSortBy3.Add(new ComboBoxMembers("Purchase Order # Descending", "ActivityReceivingPONumber DESC"));
                aSortBy3.Add(new ComboBoxMembers("Issued Date Ascending", "ActivityReceivingIssuedDateTime ASC"));
                aSortBy3.Add(new ComboBoxMembers("Issued Date Descending", "ActivityReceivingIssuedDateTime DESC"));
                aSortBy3.Add(new ComboBoxMembers("Arrival Ascending", "ActivityReceivingArrivalDateTime ASC"));
                aSortBy3.Add(new ComboBoxMembers("Arrival Descending", "ActivityReceivingArrivalDateTime DESC"));

                cboSortBy3.ItemsSource = aSortBy3;
                cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
                //cboSortBy3.ValueMember = "ValueMember";
                cboSortBy3.SelectedIndex = RecvSettings.iSortBy3Index;

                filters.Add("", "");
                filters.Add("Purchase Order # Ascending", "ActivityReceivingPONumber ASC");
                filters.Add("Purchase Order # Descending", "ActivityReceivingPONumber DESC");
                filters.Add("Issued Date Ascending", "ActivityReceivingIssuedDateTime ASC");
                filters.Add("Issued Date Descending", "ActivityReceivingIssuedDateTime DESC");
                filters.Add("Arrival Ascending", "ActivityReceivingArrivalDateTime ASC");
                filters.Add("Arrival Descending", "ActivityReceivingArrivalDateTime DESC");
            }
        }

        #endregion
 
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
                    if (keycode == Keycode.Escape)
                    {
                        if (SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs()); ;
                        }
                        else
                        {
                            CancelChanges();
                        }
                    }
                    else if (keycode == Keycode.Grave)
                    {
                        if (SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs()); ;
                        }
                        else
                        {
                            CancelChanges();
                        }

                        await this.Close();
                    }
                }
            });

            try
            {
                await this.ucActivityReceivingFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityReceivingFilter_GotFocus threw an error " + ex, "OK");
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
        public ReceivingSettings RecvSettings
        {
            get
            {
                return recvSettings;
            }
            set
            {
                recvSettings = value;
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

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {    
            //((ComboBoxMembers)cboSortBy1.Items[cboSortBy1.SelectedIndex]).ValueMember.Trim();
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            RecvSettings.strSortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (cboSortBy2.SelectedIndex >= 0)
            {
                RecvSettings.strSortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(RecvSettings.strSortBy2Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + RecvSettings.strSortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                RecvSettings.strSortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(RecvSettings.strSortBy3Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + RecvSettings.strSortBy3Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            RecvSettings = RecvSettings.LoadData();

            if (SysCommon.IsEmptyDate(RecvSettings.dtArrivalDateFilter))
            {
                RecvSettings.dtArrivalDateFilter = DateTime.Now;
            }

            if (SysCommon.IsEmptyDate(RecvSettings.dtIssuedDateFilter))
            {
                RecvSettings.dtIssuedDateFilter = DateTime.Now;
            }
            IsControlLoaded = true;
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
        public async Task ucActivityReceivingFilter_GotFocus(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
                {
                    //Reload oReceivingSettings because intial values will be scewed because of SelectedIndexChanged events firing
                    if (!IsControlLoaded)
                    {
                        LoadData();
                    }

                    chkApplyArrivalDate.IsToggled = RecvSettings.IsApplyArrivalDateFilter;
                    dtpArrivalDate.Date = RecvSettings.dtArrivalDateFilter;

                    chkApplyIssuedDate.IsToggled = RecvSettings.IsApplyIssuedDateFilter;
                    dtpIssuedDate.Date = RecvSettings.dtIssuedDateFilter;

                    cboSortBy1.SelectedIndex = RecvSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = RecvSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = RecvSettings.iSortBy3Index;

                    dtpIssuedDate.Focus();

                    ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool IsReadyToSave = true;

            try
            {

            }
            catch (Exception)
            {
                //if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "IsReadyToSave", Ex) == false)
                //{
                //    Application.Exit();
                //}

                IsReadyToSave = false;
            }

            return IsReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
        {
            return !bChangesMade;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return ucCallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "dMobile Activity Receiving Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            bool bSaveComplete = false;
            try
            {
                if (IsReadyToSave())
                {
                    if (chkApplyArrivalDate.IsToggled)
                    {
                        RecvSettings.dtArrivalDateFilter = dtpArrivalDate.Date;
                    }

                    RecvSettings.IsApplyArrivalDateFilter = chkApplyArrivalDate.IsToggled;

                    if (chkApplyIssuedDate.IsToggled)
                    {
                        RecvSettings.dtIssuedDateFilter = dtpIssuedDate.Date;
                    }

                    RecvSettings.IsApplyIssuedDateFilter = chkApplyIssuedDate.IsToggled;

                    RecvSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    RecvSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    RecvSettings.iSortBy3Index = cboSortBy3.SelectedIndex;
                    MakeSQLSortBy();

                    ReceivingSettings tempRecvSettings = RecvSettings;
                    RecvSettings.SaveData(ref tempRecvSettings);
                    RecvSettings = tempRecvSettings;

                    bSaveComplete = true;
                    bChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    DisplayAlert("Validation Failed", "Data Failed Validation. Check Your Data.", "OK");
                }
            }
            catch (Exception)
            {
                //if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "SaveChanges", Ex) == false)
                //{
                //    Application.Exit();
                //}
                bSaveComplete = false;

            }

            return bSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CancelChanges()
        {
            bool bCancelChanges = true;

            try
            {
                if (!IsReadyToClose())
                {
                    LoadData();

                    chkApplyArrivalDate.IsToggled = RecvSettings.IsApplyArrivalDateFilter;
                    dtpArrivalDate.Date = RecvSettings.dtArrivalDateFilter;

                    chkApplyIssuedDate.IsToggled = RecvSettings.IsApplyIssuedDateFilter;
                    dtpIssuedDate.Date = RecvSettings.dtIssuedDateFilter;

                    cboSortBy1.SelectedIndex = RecvSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = RecvSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = RecvSettings.iSortBy3Index;
                    MakeSQLSortBy();

                    bCancelChanges = false;
                    bChangesMade = false;
                }

            }
            catch (Exception)
            {
                //if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "CancelChanges", Ex) == false)
                //{
                //    Application.Exit();
                //}
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
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                      "<params>" +
                      ("<arrival_date>" + RecvSettings.dtArrivalDateFilter.ToShortDateString() + "</arrival_date>") +
                      ("<issued_date>" + RecvSettings.dtIssuedDateFilter.ToShortDateString() + "</issued_date>") +
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
                if (isControlLoaded)
                {
                    m_bChangesMade = true;
                }
            }
            catch (Exception)
            {
                //if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "IdentifyChangesMade", Ex) == false)
                //{
                //    Application.Exit();
                //}
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

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}