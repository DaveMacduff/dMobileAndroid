using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.CustomPages.Modal;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrReturnsFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ReturnsSettings retSettings = new ReturnsSettings();
        private ArrayList sortBy1 = new ArrayList();
        private ArrayList sortBy2 = new ArrayList();
        private ArrayList sortBy3 = new ArrayList();
        private string sqlSortBy = string.Empty;
        private bool isEnterTrapped = false;
        private bool isControlLoaded = false;
        private string fullClassName = "dMobile.ucActivityHdrReturnsFilter";
        private bool isChangesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.Returns;
        private Dictionary<string, string> filters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        public ucActivityHdrReturnsFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyReturnedDate.Text = "Apply Returned Date Filter";
            this.lblReturnedDate.Text = "Returned";
            this.lblApplyEnteredDate.Text = "Apply Entered Date Filter";
            this.lblEnteredDate.Text = "Entered";
            this.lblSort.Text = "Sort By";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            // TODO: Resource strings
            this.SortBy1.Clear();
            this.SortBy1.Add(new ComboBoxMembers("Order # Ascending", "ActivityReturnsFKRefno ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Order # Descending", "ActivityReturnsFKRefno DESC"));
            this.SortBy1.Add(new ComboBoxMembers("Entered Date Ascending", "ActivityReturnsOrderedDateTime ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Entered Date Descending", "ActivityReturnsOrderedDateTime DESC"));
            this.SortBy1.Add(new ComboBoxMembers("Returned Ascending", "ActivityReturnsDeliveryDateTime ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Returned Descending", "ActivityReturnsDeliveryDateTime DESC"));
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;

            // TODO: Resource strings
            this.SortBy2.Clear();
            this.SortBy2.Add(new ComboBoxMembers(string.Empty, string.Empty));
            this.SortBy2.Add(new ComboBoxMembers("Order # Ascending", "ActivityReturnsFKRefno ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Order # Descending", "ActivityReturnsFKRefno DESC"));
            this.SortBy2.Add(new ComboBoxMembers("Entered Date Ascending", "ActivityReturnsOrderedDateTime ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Entered Date Descending", "ActivityReturnsOrderedDateTime DESC"));
            this.SortBy2.Add(new ComboBoxMembers("Returned Ascending", "ActivityReturnsDeliveryDateTime ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Returned Descending", "ActivityReturnsDeliveryDateTime DESC"));
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;

            // TODO: Resource strings
            this.SortBy3.Clear();
            this.SortBy3.Add(new ComboBoxMembers(string.Empty, string.Empty));
            this.SortBy3.Add(new ComboBoxMembers("Order # Ascending", "ActivityReturnsFKRefno ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Order # Descending", "ActivityReturnsFKRefno DESC"));
            this.SortBy3.Add(new ComboBoxMembers("Entered Date Ascending", "ActivityReturnsOrderedDateTime ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Entered Date Descending", "ActivityReturnsOrderedDateTime DESC"));
            this.SortBy3.Add(new ComboBoxMembers("Returned Ascending", "ActivityReturnsDeliveryDateTime ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Returned Descending", "ActivityReturnsDeliveryDateTime DESC"));
            this.cboSortBy3.ItemsSource = this.SortBy3;
            this.cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;

            filters.Add("", "");
            this.filters.Add("Order # Ascending", "ActivityReturnsFKRefno ASC");
            this.filters.Add("Order # Descending", "ActivityReturnsFKRefno DESC");
            this.filters.Add("Entered Date Ascending", "ActivityReturnsOrderedDateTime ASC");
            this.filters.Add("Entered Date Descending", "ActivityReturnsOrderedDateTime DESC");
            this.filters.Add("Returned Ascending", "ActivityReturnsDeliveryDateTime ASC");
            this.filters.Add("Returned Descending", "ActivityReturnsDeliveryDateTime DESC");
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
                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                }
            });

            try
            {
                this.ucActivityReturnsFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityReturnsFilter_GotFocus threw an error " + ex, "OK");
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

        private ArrayList SortBy3
        {
            get
            {
                return this.sortBy3;
            }

            set
            {
                this.sortBy3 = value;
            }
        }

        private bool IsEnterTrapped
        {
            get
            {
                return this.isEnterTrapped;
            }

            set
            {
                this.isEnterTrapped = value;
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

        private ReturnsSettings RetSettings
        {
            get
            {
                return this.retSettings;
            }

            set
            {
                this.retSettings = value;
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

        #endregion

        #region Public Members

        #region IAppArea Members

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
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Returns Filter";
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
                    if (this.chkApplyReturnedDate.IsToggled)
                    {
                        this.RetSettings.ReturnedDateFilter = this.dtpReturnedDate.Date;
                    }

                    this.RetSettings.IsApplyReturnedDateFilter = this.chkApplyReturnedDate.IsToggled;

                    if (this.chkApplyEnteredDate.IsToggled)
                    {
                        this.RetSettings.ReturnedDateFilter = this.dtpEnteredDate.Date;
                    }

                    this.RetSettings.IsApplyEnteredDateFilter = this.chkApplyEnteredDate.IsToggled;

                    this.RetSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.RetSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.RetSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    ReturnsSettings tempRetSettings = this.RetSettings;
                    this.RetSettings.SaveData(ref tempRetSettings);
                    this.RetSettings = tempRetSettings;

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
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
                    // Clear any properties.
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
                    this.chkApplyReturnedDate.IsToggled = this.RetSettings.IsApplyReturnedDateFilter;
                    this.dtpReturnedDate.Date = this.RetSettings.ReturnedDateFilter;
                    this.chkApplyEnteredDate.IsToggled = this.RetSettings.IsApplyEnteredDateFilter;
                    this.dtpEnteredDate.Date = this.RetSettings.EnteredDateFilter;
                    this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;
                    this.MakeSQLSortBy();

                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

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
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<returned_date>" + this.RetSettings.ReturnedDateFilter.ToShortDateString() + "</returned_date>")
                + ("<entered_date>" + this.RetSettings.EnteredDateFilter.ToShortDateString() + "</entered_date>")
                + ("<sortby>" + this.SQLSortBy + "</sortby>")
                + "</params>");

            return request;
        }

        #endregion

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
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityReturnsFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                // Reload oReturnsSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                }

                this.chkApplyEnteredDate.IsToggled = this.RetSettings.IsApplyEnteredDateFilter;
                this.dtpEnteredDate.Date = this.RetSettings.EnteredDateFilter;

                this.chkApplyReturnedDate.IsToggled = this.RetSettings.IsApplyReturnedDateFilter;
                this.dtpReturnedDate.Date = this.RetSettings.ReturnedDateFilter;

                this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;
                this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;
                this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            //((ComboBoxMembers)(this.cboSortBy1.Items[cboSortBy1.SelectedIndex])).ValueMember.Trim();
            this.SQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            this.RetSettings.strSortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.RetSettings.strSortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.RetSettings.strSortBy2Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.RetSettings.strSortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.RetSettings.strSortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.RetSettings.strSortBy3Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.RetSettings.strSortBy3Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.RetSettings = this.RetSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.RetSettings.EnteredDateFilter))
            {
                this.RetSettings.EnteredDateFilter = DateTime.Now;
            }

            if (SysCommon.IsEmptyDate(this.RetSettings.ReturnedDateFilter))
            {
                this.RetSettings.ReturnedDateFilter = DateTime.Now;
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