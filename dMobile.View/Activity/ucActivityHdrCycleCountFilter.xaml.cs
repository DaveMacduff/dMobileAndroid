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
    public partial class ucActivityHdrCycleCountFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private CycleCountSettings countSettings = new CycleCountSettings();
        private ArrayList sortBy1 = new ArrayList();
        private ArrayList sortBy2 = new ArrayList();
        private string sqlSortBy = string.Empty;
        private bool enterTrapped = false;
        private bool isControlLoaded = false;
        private string fullClassName = "dMobile.ucActivityHdrCycleCountFilter";
        private bool changesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        public ucActivityHdrCycleCountFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyCountDate.Text = "Apply Date Filter";
            this.lblDate.Text = "Count Date";
            this.lblSort.Text = "Sort By";            
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            //At least one sort option must be selected
            // TODO: Resource strings
            this.SortBy1.Clear();
            this.SortBy1.Add(new ComboBoxMembers("Count # Ascending", "ActivityCountFKRefno ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Count # Descending", "ActivityCountFKRefno DESC"));
            this.SortBy1.Add(new ComboBoxMembers("Count Date Ascending", "ActivityCountDateTime ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Count Date Descending", "ActivityCountDateTime DESC"));
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;

            // TODO: Resource strings
            this.SortBy2.Clear();
            this.SortBy2.Add(new ComboBoxMembers(string.Empty, string.Empty));
            this.SortBy2.Add(new ComboBoxMembers("Count # Ascending", "ActivityCountFKRefno ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Count # Descending", "ActivityCountFKRefno DESC"));
            this.SortBy2.Add(new ComboBoxMembers("Count Date Ascending", "ActivityCountDateTime ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Count Date Descending", "ActivityCountDateTime DESC"));
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;

            filters.Add("", "");
            filters.Add("Count # Ascending", "ActivityCountFKRefno ASC");
            filters.Add("Count # Descending", "ActivityCountFKRefno DESC");
            filters.Add("Count Date Ascending", "ActivityCountDateTime ASC");
            filters.Add("Count Date Descending", "ActivityCountDateTime DESC");
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

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
                this.ucActivityHdrCycleCountFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityHdrCycleCountFilter_GotFocus threw an error " + ex, "OK");
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

        private ArrayList SortBy1
        {
            get
            {
                return this.sortBy1;
            }

            set
            {
                this.sortBy1 = value;
            }
        }

        private ArrayList SortBy2
        {
            get
            {
                return this.sortBy2;
            }

            set
            {
                this.sortBy2 = value;
            }
        }

        private bool EnterTrapped
        {
            get
            {
                return this.enterTrapped;
            }

            set
            {
                this.enterTrapped = value;
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

        private CycleCountSettings CountSettings
        {
            get
            {
                return this.countSettings;
            }

            set
            {
                this.countSettings = value;
            }
        }

        private string SQLSortBy
        {
            get
            {
                return this.sqlSortBy;
            }

            set
            {
                this.sqlSortBy = value;
            }
        }

        private string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        private bool ChangesMade
        {
            get
            {
                return this.changesMade;
            }

            set
            {
                this.changesMade = value;
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
        public void ucActivityHdrCycleCountFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload oCycleCountSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                }

                this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;
                this.dtpCountDate.Date = this.CountSettings.dtCountDate;

                this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
                this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "IsReadyToSave", ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }

                isReadyToSave = false;
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
            return !this.ChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Cycle Count Filter";
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
                    if (this.chkApplyCountDate.IsToggled)
                    {
                        this.CountSettings.dtCountDate = this.dtpCountDate.Date;
                    }

                    this.CountSettings.IsApplyCountDate = this.chkApplyCountDate.IsToggled;

                    this.CountSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.CountSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;

                    this.MakeSQLSortBy();

                    CycleCountSettings tempCountSettings = this.CountSettings;
                    this.CountSettings.SaveData(ref tempCountSettings);
                    this.CountSettings = tempCountSettings;

                    saveComplete = true;
                    this.ChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Data failed validation. Check your data.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "SaveChanges", ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }

                saveComplete = false;
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
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();

                    this.dtpCountDate.Date = this.CountSettings.dtCountDate;
                    this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;

                    this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;
                    this.MakeSQLSortBy();

                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    await errRpt.ShowDialog();
                });

                cancelChanges = false;
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
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                ("<count_date>" + this.CountSettings.dtCountDate.ToShortDateString() + "</count_date>") +
                ("<sortby>" + this.SQLSortBy + "</sortby>") +
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
                if (this.IsControlLoaded)
                {
                    this.ChangesMade = true;
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "IdentifyChangesMade", ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }
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
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            //(ComboBoxMembers)this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex];
            this.SQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            this.CountSettings.strSortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.CountSettings.strSortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.CountSettings.strSortBy2Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.CountSettings.strSortBy2Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.CountSettings = this.CountSettings.LoadData();

            // default count date to today's date if it is empty
            if (SysCommon.IsEmptyDate(this.CountSettings.dtCountDate))
            {
                this.CountSettings.dtCountDate = DateTime.Now;
            }

            this.IsControlLoaded = true;
        }

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

        #endregion
    }
}