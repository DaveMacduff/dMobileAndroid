using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.Activity;
using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucPurchaseOrdersMenu : DMobileContentPage, IAppArea, IAppPublisher
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

        public ucPurchaseOrdersMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdReceiving.Text = "1) Receiving";
            this.cmdPutAway.Text = "2) Put Away";
            this.cmdWorkSheet.Text = "3) Work Sheet";

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

                if (keycode == Keycode.Enter)
                {
                    //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);                            
                    ucActivityHdr worksheet = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Escape)
                {
                    //this.CallingControl = new ucMainMenu();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num1)
                {
                    ucActivityHdr receiving = new ucActivityHdr(SysCommon.ActSubTypes.Receiving);
                    await receiving.Show();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num2)
                {
                    //this.CallingControl = new ucActivitySkidPutAway()
                    ucActivitySkidPutAway putAway = new ucActivitySkidPutAway();
                    await putAway.Show();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num3)
                {
                    //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);                    
                    ucActivityHdr worksheet = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
            });

            try
            {
                this.ucPurchaseOrdersMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucPurchaseOrdersMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
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
            // TODO: Resource strings
            return "Purchase Orders Menu";
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
            this.cmdPutAway.IsEnabled = false;
            this.cmdReceiving.IsEnabled = false;
            this.cmdWorkSheet.IsEnabled = false;

            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucPurchaseOrdersMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.cmdReceiving.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsReceivingModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReceiving.IsEnabled = true;
                        }
                    }

                    this.cmdPutAway.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPutAwayModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            if (this.SysEnv.SysSettings.IsSkidTrackingModule)
                            {
                                this.cmdPutAway.IsEnabled = true;
                            }
                        }
                    }

                    this.cmdWorkSheet.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPOWorkSheetModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdWorkSheet.IsEnabled = true;
                        }
                    }

                    // Override all checks if module bypass Isenabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.IsEnableAllModuleBypassLicense && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdReceiving.IsEnabled = true;
                        this.cmdPutAway.IsEnabled = true;
                        this.cmdWorkSheet.IsEnabled = true;
                    }

                    this.cmdReceiving.Focus();

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
        private async void cmdReceiving_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Receiving);
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
            ucActivityHdr receiving = new ucActivityHdr(SysCommon.ActSubTypes.Receiving);
            await receiving.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPutAway_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucActivitySkidPutAway();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
            ucActivitySkidPutAway putAway = new ucActivitySkidPutAway();
            await putAway.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdWorkSheet_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);                    
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;
            ucActivityPurchaseOrderWorkSheet worksheet = new ucActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);        
            await worksheet.Show();
        }

        #endregion
    }
}