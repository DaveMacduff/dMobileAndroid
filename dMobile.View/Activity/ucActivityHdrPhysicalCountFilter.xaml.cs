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
using System.Threading;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrPhysicalCountFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl m_ucCallingControl;
        private PhysicalCountSettings countSettings = new PhysicalCountSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrPhysicalCountFilter";
        private bool m_bChangesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public ucActivityHdrPhysicalCountFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblSort.Text = "Sort By";
            this.lblApplyCountDate.Text = "Apply Physical Count Date Filter";
            this.lblDate.Text = "Count Date";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add(new ComboBoxMembers("Count # Ascending", "ActivityCountFKRefno ASC"));
            aSortBy1.Add(new ComboBoxMembers("Count # Descending", "ActivityCountFKRefno DESC"));
            aSortBy1.Add(new ComboBoxMembers("Count Date Ascending", "ActivityCountDateTime ASC"));
            aSortBy1.Add(new ComboBoxMembers("Count Date Descending", "ActivityCountDateTime DESC"));
            cboSortBy1.ItemsSource = aSortBy1;
            cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(new ComboBoxMembers("", ""));
            aSortBy2.Add(new ComboBoxMembers("Count # Ascending", "ActivityCountFKRefno ASC"));
            aSortBy2.Add(new ComboBoxMembers("Count # Descending", "ActivityCountFKRefno DESC"));
            aSortBy2.Add(new ComboBoxMembers("Count Date Ascending", "ActivityCountDateTime ASC"));
            aSortBy2.Add(new ComboBoxMembers("Count Date Descending", "ActivityCountDateTime DESC"));
            cboSortBy2.ItemsSource = aSortBy2;
            cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;

            filters.Add("", "");
            filters.Add("Count # Ascending", "ActivityCountFKRefno ASC");
            filters.Add("Count # Descending", "ActivityCountFKRefno DESC");
            filters.Add("Count Date Ascending", "ActivityCountDateTime ASC");
            filters.Add("Count Date Descending", "ActivityCountDateTime DESC");
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
                        if (await IsReadyToClose())
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
                this.ucActivityHdrPhysicalCountFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdrPhysicalCountFilter_GotFocus threw an error " + ex, "OK"));
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

        public PhysicalCountSettings CountSettings
        {
            get
            {
                return countSettings;
            }
            set
            {
                countSettings = value;
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
            //((ComboBoxMembers)(cboSortBy1.Items[cboSortBy1.SelectedIndex])).ValueMember.Trim();
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            CountSettings.strSortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (cboSortBy2.SelectedIndex >= 0)
            {
                CountSettings.strSortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();
                
                if (!string.IsNullOrEmpty(CountSettings.strSortBy2Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + CountSettings.strSortBy2Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            CountSettings = CountSettings.LoadData();

            // default count date to today's date if it is empty
            if (SysCommon.IsEmptyDate(CountSettings.dtCountDate))
            {
                CountSettings.dtCountDate = DateTime.Now;
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

        public void ucActivityHdrPhysicalCountFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload CountSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!IsControlLoaded)
                {
                    LoadData();
                }
                
                chkApplyCountDate.IsToggled = CountSettings.IsApplyCountDate;
                dtpCountDate.Date = CountSettings.dtCountDate;

                cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;
                MakeSQLSortBy();

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
            return "Physical Count Filter";
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
                    if (chkApplyCountDate.IsToggled)
                    {
                        CountSettings.dtCountDate = dtpCountDate.Date;
                    }

                    CountSettings.IsApplyCountDate = chkApplyCountDate.IsToggled;

                    CountSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    CountSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    MakeSQLSortBy();

                    PhysicalCountSettings tempCountSettings = CountSettings;
                    CountSettings.SaveData(ref tempCountSettings);
                    CountSettings = tempCountSettings;

                    bSaveComplete = true;
                    bChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK"));
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

                    chkApplyCountDate.IsToggled = CountSettings.IsApplyCountDate;
                    dtpCountDate.Date = CountSettings.dtCountDate;

                    cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;
                    MakeSQLSortBy();

                    bCancelChanges = false;
                    bChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    await errRpt.ShowDialog();
                });

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
            XDocument request = System.Xml.Linq.XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<count_date>" + CountSettings.dtCountDate.ToShortDateString() + "</count_date>") + ("<sortby>" + strSQLSortBy + "</sortby>") + "</params>");

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

        private void cboSortBy1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboSortBy2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            await this.SaveChanges();
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}