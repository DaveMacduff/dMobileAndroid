namespace dMobile.View.Menus
{
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Options;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucOptionsMenu : DMobileContentPage, IAppArea, IAppPublisher
    {
        //private UserControl callingControl = new ucMainMenu();
        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        public ucOptionsMenu()
        {
            InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();
            }

            this.cmdSetupDefaults.Text = "1) " + Resource.SetupDefaults;
            this.cmdCommunicationSetup.Text = "2) " + Resource.CommSetup;
            this.cmdRefreshData.Text = "3) " + Resource.RefreshData;
            this.cmdSubmitBatch.Text = "4) " + Resource.SubmitBatch;
            this.cmdUtilities.Text = "5) " + Resource.Utilities;
            this.cmdAbout.Text = "6) " + Resource.About;
        }

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                this.EnableControls();
                this.ucOptionsMenu_GotFocus(this, new EventArgs());
                this.cmdRefreshData_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "ucOptionsMenu_GotFocus exception: " + ex.Message, "OK");
            }

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter)
                {
                    //this.CallingControl = new ucSetupDefaults();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucSetupDefaults setupDefaults = new ucSetupDefaults();
                    await setupDefaults.Show();
                }
                else if (keycode == Keycode.Escape || keycode == Keycode.F1)
                {
                    //this.CallingControl = new ucMainMenu();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num1)
                {
                    //this.CallingControl = new ucSetupDefaults();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                    
                    ucSetupDefaults setupDefaults = new ucSetupDefaults();
                    await setupDefaults.Show();
                }
                else if (keycode == Keycode.Num2)
                {
                    //this.CallingControl = new ucCommunicationSetup();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucCommunicationSetup commSetup = new ucCommunicationSetup();
                    await commSetup.Show();

                }
                else if (keycode == Keycode.Num3)
                {
                    //this.CallingControl = new ucRefreshDataOptions();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucRefreshDataOptions refreshDataOption = new ucRefreshDataOptions();
                    await refreshDataOption.Show();
                }
                else if (keycode == Keycode.Num4)
                {                    
                    //this.CallingControl = new ucSubmitBatch();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucSubmitBatch submitBatch = new ucSubmitBatch();
                    await submitBatch.Show();
                }
                else if (keycode == Keycode.Num5)
                {
                    //this.CallingControl = new ucUtilitiesMenu();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucUtilitiesMenu utilities = new ucUtilitiesMenu();
                    await utilities.Show();
                }
                else if (keycode == Keycode.Num6)
                {
                    //this.CallingControl = new ucAbout();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;

                    ucAbout about = new ucAbout();
                    await about.Show();
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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //  return this.CallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return Resource.OptionsMenu;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSetupDefaults_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucSetupDefaults();

            ucSetupDefaults setupDefaults = new ucSetupDefaults();
            await setupDefaults.Show();

            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdCommunicationSetup_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucCommunicationSetup();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucCommunicationSetup commSetup = new ucCommunicationSetup();
            await commSetup.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdRefreshData_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucRefreshDataOptions();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucRefreshDataOptions refreshDataOption = new ucRefreshDataOptions();
            await refreshDataOption.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSubmitBatch_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucSubmitBatch();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucSubmitBatch submitBatch = new ucSubmitBatch();
            await submitBatch.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAbout_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucAbout();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucAbout about = new ucAbout();
            await about.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdUtilities_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucUtilitiesMenu();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucUtilitiesMenu utilities = new ucUtilitiesMenu();
            await utilities.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void cmdRefreshData_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                CommunicationSetup commSetup = new CommunicationSetup();

                if (File.Exists(commSetup.strFileASCIIZ))
                {
                    this.cmdRefreshData.IsEnabled = true;
                }
                else
                {
                    this.cmdRefreshData.IsEnabled = false;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucOptionsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    if (this.SysEnv.IsCommunicationTestSuccessful)
                    {
                        this.cmdRefreshData.IsEnabled = true;
                    }
                    else
                    {
                        this.cmdRefreshData.IsEnabled = false;
                    }

                    if (this.SysEnv.IsCommunicationTestSuccessful && this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        this.cmdSubmitBatch.IsEnabled = true;
                    }
                    else
                    {
                        this.cmdSubmitBatch.IsEnabled = false;
                    }

                    this.cmdSetupDefaults.Focus();

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
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
        private void DisableControls()
        {
            this.cmdAbout.IsEnabled = false;
            this.cmdCommunicationSetup.IsEnabled = false;
            this.cmdRefreshData.IsEnabled = false;
            this.cmdSetupDefaults.IsEnabled = false;
            this.cmdSubmitBatch.IsEnabled = false;
            this.cmdUtilities.IsEnabled = false;
            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        private void EnableControls()
        {
            this.cmdAbout.IsEnabled = true;
            this.cmdCommunicationSetup.IsEnabled = true;
            this.cmdRefreshData.IsEnabled = true;
            this.cmdSetupDefaults.IsEnabled = true;
            this.cmdSubmitBatch.IsEnabled = true;
            this.cmdUtilities.IsEnabled = true;
        }
    }
}
