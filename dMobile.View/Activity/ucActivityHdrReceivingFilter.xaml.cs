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
using dMobile.Resources.Types;

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
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

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
                // TODO: Resource strings
                //At least one sort option must be selected
                aSortBy1.Clear();
                aSortBy1.Add("Purchase Order # Ascending");
                aSortBy1.Add("Purchase Order # Descending");
                aSortBy1.Add("Issued Date Ascending");
                aSortBy1.Add("Issued Date Descending");
                aSortBy1.Add("Arrival Ascending");
                aSortBy1.Add("Arrival Descending");

                cboSortBy1.ItemsSource = aSortBy1;
                cboSortBy1.SelectedIndex = RecvSettings.iSortBy1Index;

                aSortBy2.Clear();
                aSortBy2.Add(string.Empty);
                aSortBy2.Add("Purchase Order # Ascending");
                aSortBy2.Add("Purchase Order # Descending");
                aSortBy2.Add("Issued Date Ascending");
                aSortBy2.Add("Issued Date Descending");
                aSortBy2.Add("Arrival Ascending");
                aSortBy2.Add("Arrival Descending");

                cboSortBy2.ItemsSource = aSortBy2;
                cboSortBy2.SelectedIndex = RecvSettings.iSortBy2Index;

                aSortBy3.Clear();
                aSortBy3.Add(string.Empty);
                aSortBy3.Add("Purchase Order # Ascending");
                aSortBy3.Add("Purchase Order # Descending");
                aSortBy3.Add("Issued Date Ascending");
                aSortBy3.Add("Issued Date Descending");
                aSortBy3.Add("Arrival Ascending");
                aSortBy3.Add("Arrival Descending");

                cboSortBy3.ItemsSource = aSortBy3;
                cboSortBy3.SelectedIndex = RecvSettings.iSortBy3Index;

                filters.Add(string.Empty, null);
                filters.Add("Purchase Order # Ascending", new Sort() { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.ASC });
                filters.Add("Purchase Order # Descending", new Sort() { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.DESC });
                filters.Add("Issued Date Ascending", new Sort() { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.ASC });
                filters.Add("Issued Date Descending", new Sort() { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.DESC });
                filters.Add("Arrival Ascending", new Sort() { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.ASC });
                filters.Add("Arrival Descending", new Sort() { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.DESC });
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
                        if (await SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs()); ;
                        }
                        else
                        {
                            await CancelChanges();
                        }
                    }
                    else if (keycode == Keycode.Grave)
                    {
                        if (await SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs()); ;
                        }
                        else
                        {
                            await CancelChanges();
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
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityReceivingFilter_GotFocus threw an error " + ex, "OK"));
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
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].SortValue;
            RecvSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]];

            if (cboSortBy2.SelectedIndex >= 0)
            {
                RecvSettings.SortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]];

                if (RecvSettings.SortBy2Value != null && !string.IsNullOrEmpty(RecvSettings.SortBy2Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + RecvSettings.SortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                RecvSettings.SortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]];

                if (RecvSettings.SortBy3Value != null && !string.IsNullOrEmpty(RecvSettings.SortBy3Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + RecvSettings.SortBy3Value;
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
        public async Task<bool> IsReadyToSave()
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
            return "Activity Receiving Filter";
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
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Validation Failed", "Data Failed Validation. Check Your Data.", "OK"));
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
        public async Task<bool> CancelChanges()
        {
            bool bCancelChanges = true;

            try
            {
                if (!await IsReadyToClose())
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
                bCancelChanges = false;
            }

            return bCancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
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
            if (await SaveChanges())
            {
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            await CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}