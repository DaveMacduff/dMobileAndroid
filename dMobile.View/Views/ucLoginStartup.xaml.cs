using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net.Sockets;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.View.Exceptions;
using Resource = dMobile.Resources.Properties.Resources;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.CustomPages;
using dMobile.View.Menus;
using Android.Widget;
using dMobile.View.Types;

namespace dMobile.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucLoginStartup : DMobileContentPage, IAppArea, IAppPublisher
    {
        //private UserControl callingControl = null;
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

            // TODO: login title
            this.Title = "dMobile Login";

            this.cmdLogin.Text = Resource.Login;
            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.lblWarehouse.Text = Resource.Warehouse;

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();
            }
        }

        private void LstWarehouse_Unfocused(object sender, FocusEventArgs e)
        {
            this.cmdLogin.Focus();
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

        //private UserControl CallingControl
        //{
        //  get
        //  {
        //    return this.callingControl;
        //  }

        //  set
        //  {
        //    this.callingControl = value;
        //  }
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
        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                this.LoginStartup_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Alert commented out for testing
                //DisplayAlert(Resource.Error, "LoginStartup_GotFocus threw an error: " + ex.Message, Resource.OK);
                Console.WriteLine("LoginStartup_GotFocus error: " + ex.Message);
            }

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                try
                {
                    Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter)
                    {
                        if (this.IsReadyToClose())
                        {
                            this.cmdLogin_Click(this, new EventArgs());
                        }
                    }
                    else if (keycode == Keycode.Escape || keycode == Keycode.F1)
                    {
                        if (this.IsReadyToClose())
                        {
                            //this.CallingControl = new ucMainMenu();
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                            Device.BeginInvokeOnMainThread(async () => await this.Close());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

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
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = true;
            string password = string.Empty;
            int userCode = 0;

            try
            {
                using (SQLiteCommand cmndGetContactUser = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndGetContactUser.CommandText = "SELECT * FROM tContactUser WHERE ContactUserCode = @contactUserCode";
                    //cmndGetContactUser.IndexName = "UQ_ContactUserOnContactUserCode";
                    cmndGetContactUser.CommandType = CommandType.Text;
                    cmndGetContactUser.Parameters.Add(new SQLiteParameter("@contactUserCode", this.txtLoginID.Text));

                    using (SQLiteDataReader drContactUser = cmndGetContactUser.ExecuteReader())
                    {
                        //if (drContactUser.Seek(DbSeekOptions.FirstEqual, new object[] { Convert.ToInt32(this.txtLoginID.Text) }))
                        //{
                        if (drContactUser.Read())
                        {
                            userCode = drContactUser.GetInt32((int)SysCommon.ContactUserFields.Code);
                            password = drContactUser.GetString((int)SysCommon.ContactUserFields.Password);

                            this.IsAllowedAssocBarCode = drContactUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode);
                            this.IsAllowedOverridePicker = drContactUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingOverridePicker);
                            this.IsOverrideAddManualDetail = drContactUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingModuleOverrideAddManualDetail);
                        }

                        drContactUser.Close();
                        //}
                    }
                }

                // Override the contact login password with the system one if bypass licenses is enabled.
                if (this.IsEnableAllModuleBypassLicense)
                {
                    password = SysCommon.SystemLoginPassword;
                }

                if (string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    isReadyToSave = false;
                }

                if (userCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    isReadyToSave = false;
                }

                if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
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
                Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                isReadyToSave = false;

            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
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
                        catch (SocketException ex)
                        {
                            // do nothing if login failed
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                        }
                        catch (Exception ex)
                        {
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
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
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <returns></returns>
        //        /// <remarks></remarks>
        //        public string GetControlDescription()
        //        {
        //#if MULTILANGUAGE
        //            return Translator.GetInstance().Translate(TranslatorVocabulary.LoginStartup);
        //#else
        //            return "Login Startup";
        //#endif
        //        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            bool saveComplete = false;
            LocationMap locMap = LocationMap.GetInstance();

            try
            {
                if (this.IsReadyToSave())
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
                        catch (SocketException ex)
                        {
                            // do nothing if login failed
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                        }
                        catch (Exception ex)
                        {
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                        }
                    }

                    this.SysEnv.IsEnableAllModuleBypassLicense = this.IsEnableAllModuleBypassLicense;

                    ContactLogin tempContactLogin = this.SysEnv.ContactLog;
                    saveComplete = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                    locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;
                    locMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMap.LoginWhsLocMapRefno);

                    this.ChangesMade = false;
                }
                else
                {
                    this.SysEnv.ContactLog.IsLoggedIn = false;
                    throw new Exceptions.MessageOnlyException(Resource.EnteredDataFailedValidation);
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.LoginStartup);
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog().ContinueWith(result =>
                {
                        Application.Current.Quit();
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
        public bool CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                if (!this.IsReadyToClose())
                {
                    this.LoadData();
                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                errReport.ShowDialog().ContinueWith(result =>
                {
                    Application.Current.Quit();
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
        private void LoadData()
        {
            List<Warehouse> dataViewLocMapWhs = new List<Warehouse>();

            this.txtLoginID.Text = this.SysEnv.ContactLog.LoginID.ToString();
            this.txtLoginPassword.Text = this.SysEnv.ContactLog.LoginPassword;

            using (SQLiteCommand cmnd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmnd.CommandText = "SELECT LocationMapRefno, LocationMapWarehouseName FROM tLocationMapWarehouse";

                using(SQLiteDataReader reader = cmnd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataViewLocMapWhs.Add(new Warehouse(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            
            this.lstWarehouse.ItemsSource = dataViewLocMapWhs;
            this.lstWarehouse.ItemDisplayBinding = new Binding("DisplayName");
            this.lstWarehouse.SelectedIndex = this.SysEnv.ContactLog.WarehouseLocationMapRefno;
            Warehouse selectedWarehouse = (Warehouse)this.lstWarehouse.SelectedItem;
            this.lstWarehouse.SelectedValue = selectedWarehouse.Id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void LoginStartup_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                    this.IsControlLoaded = true;

                    if (this.SysEnv.ContactLog.LoginID == 0)
                    {
                        this.txtLoginID.Focus();
                    }
                    else if (this.SysEnv.ContactLog.LoginPassword == string.Empty)
                    {
                        this.txtLoginPassword.Focus();
                    }
                    else if (this.lstWarehouse.SelectedIndex != -1)
                    {
                        this.lstWarehouse.Focus();
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
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            int userCode = 0;
            string password = string.Empty;

            try
            {
                using (SQLiteCommand cmndUser = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndUser.CommandText = "SELECT * FROM tContactUser WHERE ContactUserCode = @contactUserCode";
                    //cmndUser.IndexName = "UQ_ContactUserOnContactUserCode";
                    cmndUser.CommandType = CommandType.Text;
                    cmndUser.Parameters.Add(new SQLiteParameter("@contactUserCode", this.txtLoginID.Text));

                    using (SQLiteDataReader drUser = cmndUser.ExecuteReader())
                    {
                        //if (drUser.Seek(DbSeekOptions.FirstEqual, new object[] { Convert.ToInt32(this.txtLoginID.Text) }))
                        //{
                        if (drUser.Read())
                        {
                            password = drUser.GetString((int)SysCommon.ContactUserFields.Password);
                            userCode = drUser.GetInt32((int)SysCommon.ContactUserFields.Code);

                            // TODO: Where are these used anymore?
                            // March 28, 2016 - IsAllowedAssocBarCode is set later in this file  in function SaveChanges(); and used in SysShared.IdentifyProductScanned.
                            this.IsAllowedAssocBarCode = drUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode);
                            this.IsAllowedOverridePicker = drUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingOverridePicker);
                            this.IsOverrideAddManualDetail = drUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedPickingModuleOverrideAddManualDetail);
                        }

                        drUser.Close();
                        //}
                    }

                }

                if (sender == this.txtLoginID)
                {
                    if (true == string.IsNullOrEmpty(txtLoginID.Text))
                    {
                        this.txtLoginPassword.Text = string.Empty;
                        password = string.Empty;
                    }
                    else if (true == string.IsNullOrEmpty(txtLoginPassword.Text))
                    {
                        password = string.Empty;
                    }
                    else if (true == (userCode != Convert.ToInt32(txtLoginID.Text)))
                    {
                        throw new Exceptions.MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
                    }
                }

                if (sender == this.txtLoginPassword)
                {
                    // Override the contact login password with the system one if bypass licenses is enabled.
                    if (this.IsEnableAllModuleBypassLicense)
                    {
                        password = SysCommon.SystemLoginPassword;
                    }

                    if (true == string.IsNullOrEmpty(txtLoginID.Text))
                    {
                        txtLoginPassword.Text = string.Empty;
                        password = string.Empty;
                    }
                    else if (true == string.IsNullOrEmpty(txtLoginPassword.Text))
                    {
                        password = string.Empty;
                    }
                    else if (true == (password != txtLoginPassword.Text))
                    {
                        throw new Exceptions.MessageOnlyException(Resource.InvalidLoginPassword);
                    }
                }

                if (sender == this.lstWarehouse)
                {
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
                }

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
                errReport.ShowDialog().ContinueWith(result =>
                {
                    Application.Current.Quit();
                });
            }
            catch (Exception ex)
            {
                e.Cancel = false;
                FormErrorReport errReport = new FormErrorReport(ex);
                errReport.ShowDialog().ContinueWith(result => { });
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
            this.Loading = true;

            try
            {
                await Task.Run(() =>
                {
                    this.ValidateEntry(sender, new CancelEventArgs());

                    if (this.SaveChanges())
                    {
                        // <WO ID="14734" Comment="Show message after login when data is older than 48 hours" />
                        if (SysEnv == null ||
                            SysEnv.RefreshOpt == null ||
                            SysEnv.RefreshOpt.RefreshedDataLast == null ||
                            (DateTime.Now - this.SysEnv.RefreshOpt.RefreshedDataLast).TotalHours > 48)
                        {
                            Device.BeginInvokeOnMainThread(async () => await DisplayAlert(Resource.RefreshRequired, Resource.DataIsOutOfDatePleaseRefreshImmediately, Resource.OK));
                        }

                        //this.CallingControl = new ucMainMenu();
                        Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new MainPage());

                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }

            this.Loading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtLoginID_GotFocus(object sender, FocusEventArgs e)
        {
            this.txtLoginID.CursorPosition = 0;
            this.txtLoginID.SelectionLength = this.txtLoginID.Text.Length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtLoginPassword_GotFocus(object sender, FocusEventArgs e)
        {
            //this.txtLoginPassword.SelectAll();
            this.txtLoginPassword.CursorPosition = 0;
            this.txtLoginPassword.SelectionLength = this.txtLoginPassword.Text.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PictureBoxLogo_DoubleClick(object sender, EventArgs e)
        {
            // TODO: Add logo
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

            await DisplayAlert(Resource.DMobile, Resource.AllModulesEnabledBypassingDeviceLicenses, Resource.OK);
        }

        public string GetControlDescription()
        {
            throw new NotImplementedException();
        }
    }
}
