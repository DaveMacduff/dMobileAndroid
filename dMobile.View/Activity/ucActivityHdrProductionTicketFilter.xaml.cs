using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages.Modal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrProductionTicketFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ProductionTicketSettings ticketSettings = new ProductionTicketSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private ArrayList m_aSortBy3 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrProductionTicketFilter";
        private bool m_bChangesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public ucActivityHdrProductionTicketFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyProductionDate.Text = "Apply Production Date Filter";
            this.lblAdjustmentDate.Text = "Date";
            this.lblSortBy.Text = "Sort By";
            this.lblSortBy1.Text = "1";
            this.lblSortBy2.Text = "2";
            this.lblSortBy3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            //At least one sort option must be selected
            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add(new ComboBoxMembers("Production # Ascending", "ActivityProductionFKRefno ASC"));
            aSortBy1.Add(new ComboBoxMembers("Production # Descending", "ActivityProductionFKRefno DESC"));
            aSortBy1.Add(new ComboBoxMembers("Production Date Ascending", "ActivityProductionDateTime ASC"));
            aSortBy1.Add(new ComboBoxMembers("Production Date Descending", "ActivityProductionDateTime DESC"));
            cboSortBy1.ItemsSource = aSortBy1;
            cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(new ComboBoxMembers("", ""));
            aSortBy2.Add(new ComboBoxMembers("Production # Ascending", "ActivityProductionFKRefno ASC"));
            aSortBy2.Add(new ComboBoxMembers("Production # Descending", "ActivityProductionFKRefno DESC"));
            aSortBy2.Add(new ComboBoxMembers("Production Date Ascending", "ActivityProductionDateTime ASC"));
            aSortBy2.Add(new ComboBoxMembers("Production Date Descending", "ActivityProductionDateTime DESC"));
            cboSortBy2.ItemsSource = aSortBy2;
            cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;

            // TODO: Resource strings
            aSortBy3.Clear();
            aSortBy3.Add(new ComboBoxMembers("", ""));
            aSortBy3.Add(new ComboBoxMembers("Production # Ascending", "ActivityProductionFKRefno ASC"));
            aSortBy3.Add(new ComboBoxMembers("Production # Descending", "ActivityProductionFKRefno DESC"));
            aSortBy3.Add(new ComboBoxMembers("Production Date Ascending", "ActivityProductionDateTime ASC"));
            aSortBy3.Add(new ComboBoxMembers("Production Date Descending", "ActivityProductionDateTime DESC"));
            cboSortBy3.ItemsSource = aSortBy3;
            cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
            cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;

            // TODO: Resource strings
            this.filters.Add("Production # Ascending", "ActivityProductionFKRefno ASC");
            this.filters.Add("Production # Descending", "ActivityProductionFKRefno DESC");
            this.filters.Add("Production Date Ascending", "ActivityProductionDateTime ASC");
            this.filters.Add("Production Date Descending", "ActivityProductionDateTime DESC");
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
                            ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs()); ;
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
                this.ucActivityHdrProductionFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdrProductionFilter_GotFocus threw an error " + ex, "OK"));
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

        public ProductionTicketSettings TicketSettings
        {
            get
            {
                return ticketSettings;
            }
            set
            {
                ticketSettings = value;
            }
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
        private void LoadData()
        {
            TicketSettings.LoadData();

            if (SysCommon.IsEmptyDate(TicketSettings.ProductionDateFilter))
            {
                TicketSettings.ProductionDateFilter = DateTime.Now;
            }

            IsControlLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            strSQLSortBy =  this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            TicketSettings.strSortBy1Value =  this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (cboSortBy2.SelectedIndex >= 0)
            {
                TicketSettings.strSortBy2Value =  this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();
                
                if (!string.IsNullOrEmpty(TicketSettings.strSortBy2Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TicketSettings.strSortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                TicketSettings.strSortBy3Value =  this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();
                if (!string.IsNullOrEmpty(TicketSettings.strSortBy3Value))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TicketSettings.strSortBy3Value;
                }
            }
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
        public void ucActivityHdrProductionFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload oProductionTicketSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!IsControlLoaded)
                {
                    LoadData();
                }

                dtpProductionDate.Date = TicketSettings.ProductionDateFilter;
                chkApplyProductionDate.IsToggled = TicketSettings.IsApplyProductionDateFilter;

                cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;
                cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;

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
            return "dMobile Activity Production Filter";
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
                      ("<production_date>" + TicketSettings.ProductionDateFilter.ToShortDateString() + "</production_date>") +
                      ("<sortby>" + strSQLSortBy + "</sortby>") +
                      "</params>");

            return request;
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

                    dtpProductionDate.Date = TicketSettings.ProductionDateFilter;
                    chkApplyProductionDate.IsToggled = TicketSettings.IsApplyProductionDateFilter;

                    cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;
                    MakeSQLSortBy();

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
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;
            
            try
            {
                if (await IsReadyToSave())
                {
                    if (chkApplyProductionDate.IsToggled)
                    {
                        TicketSettings.ProductionDateFilter = dtpProductionDate.Date;
                    }

                    TicketSettings.IsApplyProductionDateFilter = chkApplyProductionDate.IsToggled;

                    TicketSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    TicketSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    TicketSettings.iSortBy3Index = cboSortBy3.SelectedIndex;
                    MakeSQLSortBy();

                    ProductionTicketSettings tempTicketSettings = TicketSettings;
                    TicketSettings.SaveData(ref tempTicketSettings);
                    TicketSettings = tempTicketSettings;

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

        private void chkApplyProductionDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void dtpProductionDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }
    }
}