namespace dMobile.View.Menus
{
    using dMobile.View.CustomPages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.View.Activity;
    using dMobile.Resources.Enums;
    using Android.Views;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSalesOrdersMenu : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucMainMenu();
        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        #endregion

        #region Constructors

        public ucSalesOrdersMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdPicking.Text = "1) " + "Picking";
            this.cmdReturns.Text = "2) " + "Returns";
            this.cmdReleaseByOrder.Text = "3) " + "Release By Order #";
            this.cmdReleaseBySection.Text = "4) " + "Release By Section";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
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

                if (keycode == Keycode.Num1)
                {
                    this.DisableControls();
                    //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Picking);

                    ucActivityHdr picking = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
                    await picking.Show();

                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num2)
                {
                    this.DisableControls();
                    //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Returns);

                    ucActivityHdr returns = new ucActivityHdr(SysCommon.ActSubTypes.Returns);
                    await returns.Show();

                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num3)
                {
                    this.DisableControls();
                    //this.CallingControl = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);

                    ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);
                    await releaseByOrder.Show();

                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
            });

            try
            {
                this.ucSalesOrdersMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucSalesOrdersMenu_GotFocus threw an error " + ex, "OK"));
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

        //private UserControl CallingControl
        //{
        //    get
        //    {
        //        return this.callingControl;
        //    }

        //    set
        //    {
        //        this.callingControl = value;
        //    }
        //}

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
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Sales Orders Menu";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            return true;
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

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdPicking.IsEnabled = false;
            this.cmdReleaseByOrder.IsEnabled = false;
            this.cmdReleaseBySection.IsEnabled = false;
            this.cmdReturns.IsEnabled = false;

            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdReturns_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter || e.KeyValue == 134)
        //    {
        //        this.DisableControls();

        //        this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Returns);

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //    else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.F1)
        //    {
        //        this.DisableControls();

        //        this.CallingControl = new ucMainMenu();

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //    else if (e.KeyCode == Keys.D1)
        //    {
        //        this.DisableControls();

        //        if (this.SysEnv.SysSettings.IsPickSingleProd)
        //        {
        //            this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.PickingSingle);
        //        }
        //        else
        //        {
        //            this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
        //        }

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //    else if (e.KeyCode == Keys.D2)
        //    {
        //        this.DisableControls();

        //        this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Returns);

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //    else if (e.KeyCode == Keys.D3)
        //    {
        //        this.DisableControls();

        //        this.CallingControl = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //    else if (e.KeyCode == Keys.D4)
        //    {
        //        this.DisableControls();

        //        this.CallingControl = new ucMainMenu();

        //        if (this.ReqCloseEvent != null)
        //        {
        //            this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
        //        }

        //        this.IsControlLoaded = false;
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPicking_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            if (this.SysEnv.SysSettings.IsPickSingleProd)
            {
                //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.PickingSingle);
                ucActivityHdr pickingSingle = new ucActivityHdr(SysCommon.ActSubTypes.PickingSingle);
                await pickingSingle.Show();
            }
            else
            {
                //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
                ucActivityHdr picking = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
                await picking.Show();
            }

            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdReturns_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Returns);

            ucActivityHdr returns = new ucActivityHdr(SysCommon.ActSubTypes.Returns);
            await returns.Show();

            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSalesOrdersMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                    this.cmdPicking.IsEnabled = false;
                    
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPickMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdPicking.IsEnabled = true;
                        }
                    }

                    this.cmdReturns.IsEnabled = false;
                    
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRetMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReturns.IsEnabled = true;
                        }
                    }

                    this.cmdReleaseByOrder.IsEnabled = false;
                    
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRelByOrder || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReleaseByOrder.IsEnabled = true;
                        }
                    }

                    this.cmdReleaseBySection.IsEnabled = false;
                    
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRelBySect || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReleaseBySection.IsEnabled = false;   // Disabled intentionally
                        }
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdPicking.IsEnabled = true;
                        this.cmdReturns.IsEnabled = true;
                        this.cmdReleaseByOrder.IsEnabled = true;
                        this.cmdReleaseBySection.IsEnabled = true;
                    }

                    this.cmdPicking.Focus();

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdReleaseByOrder_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            if (this.SysEnv.SysSettings.IsPickSingleProd)
            {
                //this.CallingControl = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.PickingSingle);
                ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.PickingSingle);
                await releaseByOrder.Show();
            }
            else
            {
                //this.CallingControl = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.Picking);
                ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.Picking);
                await releaseByOrder.Show();
            }

            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void cmdReleaseBySection_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            //this.CallingControl = new ucMainMenu();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
        }

        #endregion
    }
}