using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.Resources.Types;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Resource = dMobile.Resources.Properties.Resources;
using dMobile.Resources.Models;

namespace dMobile.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucLoginStartup : DMobileContentPage, IAppArea, IAppPublisher
    {
        private bool changesMade = false;
        private bool isControlLoaded = false;
        private SysEnvironment sysEnv;
        private bool isAllowedAssocBarCode = true;
        private bool isAllowedOverridePicker = false;
        private bool isOverrideAddManualDetail = false;
        private bool logInPressed = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private bool isEnableAllModuleBypassLicense = false;

        public ucLoginStartup()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = "dMobile Login";
            this.cmdLogin.Text = Resource.Login;
            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.lblWarehouse.Text = Resource.Warehouse;
            this.lstWarehouse.Title = "Select Warehouse";

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();
            }
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

        private bool LogInPressed
        {
            get
            {
                return this.logInPressed;
            }

            set
            {
                this.logInPressed = value;
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

        private bool IsAllowedOverridePicker
        {
            get
            {
                return this.isAllowedOverridePicker;
            }

            set
            {
                this.isAllowedOverridePicker = value;
            }
        }

        private bool IsAllowedAssocBarCode
        {
            get
            {
                return this.isAllowedAssocBarCode;
            }

            set
            {
                this.isAllowedAssocBarCode = value;
            }
        }

        private bool IsOverrideAddManualDetail
        {
            get
            {
                return this.isOverrideAddManualDetail;
            }

            set
            {
                this.isOverrideAddManualDetail = value;
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

        private bool IsEnableAllModuleBypassLicense
        {
            get
            {
                return this.isEnableAllModuleBypassLicense;
            }

            set
            {
                this.isEnableAllModuleBypassLicense = value;
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                try
                {
                    Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter)
                    {
                        await this.IsReadyToClose();
                        this.cmdLogin_Click(this, new EventArgs());
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        //this.CallingControl = new ucMainMenu();
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                    else if (keycode == Keycode.F1)
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await Device.InvokeOnMainThreadAsync(async () => await this.Close());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            try
            {
                await this.LoginStartup_GotFocus();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.Error, "LoginStartup_GotFocus threw an error: " + ex.Message, Resource.OK));
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
            bool isReadyToSave = true;
            string password = string.Empty;
            int userCode = 0;

            try
            {
                if (string.IsNullOrEmpty(this.txtLoginID.Text) || string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    isReadyToSave = false;
                    return isReadyToSave;
                }

                List<tContactUserModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>("SELECT * FROM tContactUser WHERE ContactUserCode = ?", Convert.ToInt32(this.txtLoginID.Text));

                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;

                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail = results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
                }

                // Override the contact login password with the system one if bypass licenses is enabled.
                if (this.IsEnableAllModuleBypassLicense)
                {
                    password = SysCommon.SystemLoginPassword;
                }

                if (userCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    isReadyToSave = false;
                }

                if (password != this.txtLoginPassword.Text)
                {
                    isReadyToSave = false;
                }

                if (this.lstWarehouse.Items.Count > 0 && Convert.ToInt32(this.lstWarehouse.SelectedValue) <= 0)
                {
                    isReadyToSave = false;
                }
            }
            catch (SQLiteException ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
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
            if (!this.LogInPressed)
            {
                // attempt to record login
                if (!this.SysEnv.SysSettings.IsDisconnectedMode
                    && (this.SysEnv.CommSetup.iHostIPAddressComponent1
                    + this.SysEnv.CommSetup.iHostIPAddressComponent2
                    + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
                {
                    if (this.SysEnv.ContactLog.IsLoggedIn)
                    {
                        this.SysEnv.ContactLog.IsLoggedIn = false;
                        ContactLogin tempContactLogin = this.SysEnv.ContactLog;
                        this.SysEnv.ContactLog.SaveData(ref tempContactLogin);

                        try
                        {
                            this.SysEnv.ContactLog.SubmitContactLogOut();
                        }
                        catch (SocketException)
                        {
                            // do nothing if login failed
                            //FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            //Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                        }
                        catch (Exception ex)
                        {
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }
                }
            }

            return !this.ChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;
            LocationMap locMap = LocationMap.GetInstance();

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SysEnv.ContactLog.LoginID = int.Parse(this.txtLoginID.Text);

                    if (Debugger.IsAttached)
                    {
                        this.SysEnv.ContactLog.LoginPassword = this.txtLoginPassword.Text;
                    }
                    else
                    {
                        this.SysEnv.ContactLog.LoginPassword = string.Empty;
                    }

                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = Convert.ToInt32(this.lstWarehouse.SelectedValue);
                    this.SysEnv.ContactLog.IsLoggedIn = true;
                    this.SysEnv.ContactLog.IsAllowedAssociateProductBarCode = this.IsAllowedAssocBarCode;
                    this.SysEnv.ContactLog.IsAllowedPickingModuleOverridePicker = this.IsAllowedOverridePicker;
                    this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail = this.IsOverrideAddManualDetail;

                    // attempt to record login
                    this.LogInPressed = true;

                    if (!this.SysEnv.SysSettings.IsDisconnectedMode
                        && (this.SysEnv.CommSetup.iHostIPAddressComponent1
                        + this.SysEnv.CommSetup.iHostIPAddressComponent2
                        + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
                    {
                        try
                        {
                            this.SysEnv.ContactLog.SubmitContactLogIn();
                        }
                        catch (SocketException)
                        {
                            // do nothing if login failed
                        }
                        catch (Exception ex)
                        {
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }

                    this.SysEnv.IsEnableAllModuleBypassLicense = this.IsEnableAllModuleBypassLicense;

                    ContactLogin tempContactLogin = this.SysEnv.ContactLog;
                    saveComplete = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                    locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;
                    locMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMap.LoginWhsLocMapRefno);

                    this.ChangesMade = false;
                }
                else
                {
                    this.SysEnv.ContactLog.IsLoggedIn = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.LoginStartup);
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog().ContinueWith(result =>
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }));

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
                    await this.LoadData();
                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await errReport.ShowDialog().ContinueWith(result =>
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
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
        /// <remarks></remarks>
        private async Task LoadData()
        {
            this.txtLoginID.Text = this.SysEnv.ContactLog.LoginID.ToString();
            this.txtLoginPassword.Text = this.SysEnv.ContactLog.LoginPassword;

            List<Warehouse> resultsWarehouse = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Warehouse>("SELECT LocationMapRefno, LocationMapWarehouseCode, LocationMapWarehouseName FROM tLocationMapWarehouse");

            if (resultsWarehouse?.Count > 0)
            {
                foreach (Warehouse warehouse in resultsWarehouse)
                {
                    warehouse.DisplayName = warehouse.LocationMapWarehouseCode + " " + warehouse.LocationMapWarehouseName;
                }

                this.lstWarehouse.ItemsSource = resultsWarehouse;
                this.lstWarehouse.ItemDisplayBinding = new Binding("DisplayName");
                Warehouse selectedWarehouse = resultsWarehouse.Where(warehouse => warehouse.LocationMapRefno == this.SysEnv.ContactLog.WarehouseLocationMapRefno).FirstOrDefault();

                if (selectedWarehouse != null)
                {
                    this.lstWarehouse.SelectedIndex = this.lstWarehouse.ItemsSource.IndexOf(selectedWarehouse);
                    this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task LoginStartup_GotFocus()
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    await this.LoadData();
                    this.IsControlLoaded = true;

                    if (this.SysEnv.ContactLog.LoginID == 0)
                    {
                        this.txtLoginID.Focus();
                    }
                    else if (this.SysEnv.ContactLog.LoginPassword == string.Empty)
                    {
                        this.txtLoginPassword.Focus();
                    }
                    else
                    {
                        this.cmdLogin.Focus();
                    }
                }

                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
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
                this.ChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ValidateEntry(object sender, CancelEventArgs e)
        {
            int userCode = 0;
            string password = string.Empty;

            try
            {
                List<tContactUserModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>("SELECT * FROM tContactUser WHERE ContactUserCode = ?", Convert.ToInt32(this.txtLoginID.Text));

                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;

                    // TODO: Where are these used anymore?
                    // March 28, 2016 - IsAllowedAssocBarCode is set later in this file  in function SaveChanges(); and used in SysShared.IdentifyProductScanned.
                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail = results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;

                }

                if (string.IsNullOrEmpty(txtLoginID.Text))
                {
                    this.txtLoginPassword.Text = string.Empty;
                    password = string.Empty;
                    throw new Exceptions.MessageOnlyException("Please enter a user ID.");
                }
                else if (string.IsNullOrEmpty(txtLoginPassword.Text))
                {
                    password = string.Empty;
                    throw new Exceptions.MessageOnlyException("Please enter a password.");
                }
                else if (userCode != Convert.ToInt32(txtLoginID.Text))
                {
                    throw new Exceptions.MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
                }
                else if (password != txtLoginPassword.Text)
                {
                    throw new Exceptions.MessageOnlyException(Resource.InvalidLoginPassword);
                }

                if (this.lstWarehouse.Items.Count > 0)
                {
                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = Convert.ToInt32(this.lstWarehouse.SelectedValue);

                    if (Convert.ToInt32(this.lstWarehouse.SelectedValue) <= 0)
                    {
                        throw new Exceptions.MessageOnlyException(Resource.SelectedWarehouseRequired);
                    }
                }
                else
                {
                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = 0;
                }

                LocationMap locMap = LocationMap.GetInstance();
                locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;

                if (e.Cancel == false)
                {
                    ContactLogin tempContactLogin = this.SysEnv.ContactLog;
                    this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(SysCommon.ActSubTypes.LoginStartup);
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;
                FormErrorReport errReport = new FormErrorReport(ex);
                await errReport.ShowDialog().ContinueWith(result =>
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                });
            }
            catch (Exception ex)
            {
                e.Cancel = false;
                FormErrorReport errReport = new FormErrorReport(ex);
                await errReport.ShowDialog();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;

                await Task.Run(async () =>
                {
                    await this.ValidateEntry(sender, new CancelEventArgs());

                    if (await this.SaveChanges())
                    {
                        // <WO ID="14734" Comment="Show message after login when data is older than 48 hours" />
                        if (SysEnv == null || SysEnv.RefreshOpt == null || SysEnv.RefreshOpt.RefreshedDataLast == null || (DateTime.Now - this.SysEnv.RefreshOpt.RefreshedDataLast).TotalHours > 48)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.RefreshRequired, Resource.DataIsOutOfDatePleaseRefreshImmediately, Resource.OK));
                        }

                        await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = new MainPage());
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtLoginID_GotFocus(object sender, FocusEventArgs e)
        {
            if (this.txtLoginID.Text != null)
            {
                this.txtLoginID.CursorPosition = 0;
                this.txtLoginID.SelectionLength = this.txtLoginID.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtLoginPassword_GotFocus(object sender, FocusEventArgs e)
        {
            if (this.txtLoginPassword.Text != null)
            {
                this.txtLoginPassword.CursorPosition = 0;
                this.txtLoginPassword.SelectionLength = this.txtLoginPassword.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PictureBoxLogo_DoubleClick(object sender, EventArgs e)
        {
            if (this.IsEnableAllModuleBypassLicense)
            {
                //this.txtLoginPassword.BackColor = SystemColors.Window;
                this.IsEnableAllModuleBypassLicense = false;
            }
            else
            {
                this.txtLoginPassword.BackgroundColor = Color.Aqua;
                this.IsEnableAllModuleBypassLicense = true;
            }

            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.DMobile, Resource.AllModulesEnabledBypassingDeviceLicenses, Resource.OK));
        }

        public string GetControlDescription()
        {
            throw new NotImplementedException();
        }

        private void LstWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            Warehouse selectedWarehouse = (Warehouse)this.lstWarehouse.SelectedItem;
            this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
        }
    }
}