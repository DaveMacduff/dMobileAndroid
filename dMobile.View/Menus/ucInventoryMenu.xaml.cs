using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryMenu : DMobileContentPage, IAppArea, IAppPublisher
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

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucInventoryMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdMovement.Text = "1) Movement";
            this.cmdConvertUnits.Text = "2) Convert Units (Break Case)";
            this.cmdInventoryCounts.Text = "3) Counts";
            this.cmdSkidMaintenance.Text = "4) Skids";
            this.cmdInquiry.Text = "5) Inquiry";
            this.cmdAdjustments.Text = "6) Adjustments";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }
        }

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter)
                {
                    //this.CallingControl = new ucInventoryMovementMenu();
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
                    //this.CallingControl = new ucInventoryMovementMenu();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num2)
                {
                    //this.CallingControl = new ucActivityConvertUnit();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num3)
                {
                    //this.CallingControl = new ucInventoryCountsMenu();
                    ucInventoryCountsMenu countsMenu = new ucInventoryCountsMenu();
                    Device.BeginInvokeOnMainThread(async () => await countsMenu.Show());
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num4)
                {
                    //this.callingControl = new ucActivityHdrSkidInfo(new ucInventoryMenu());
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num5)
                {
                    //this.CallingControl = new ucActivityInventoryInquiry();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num6)
                {
                    //this.CallingControl = new ucInventoryAdjustmentsMenu();
                    this.DisableControls();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = false;
                }
            });

            try
            {
                this.ucInventoryMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucInventoryMenu_GotFocus threw an error " + ex, "OK");
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

        #endregion

        #region Public Members

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
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
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
            return "Inventory Menu";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CancelChanges()
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
        public void ucInventoryMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.cmdMovement.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsMovesModule || this.SysEnv.SysSettings.IsReplenishmentModule
                            || this.SysEnv.SysSettings.IsTransMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdMovement.IsEnabled = true;
                        }
                    }

                    this.cmdConvertUnits.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsConvertUnitsModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdConvertUnits.IsEnabled = true;
                        }
                    }

                    this.cmdInventoryCounts.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.SysSettings.IsCycleModule
                          || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdInventoryCounts.IsEnabled = true;
                        }
                    }

                    this.cmdSkidMaintenance.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsSkidTrackingModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdSkidMaintenance.IsEnabled = true;
                        }
                    }

                    this.cmdAdjustments.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdAdjustments.IsEnabled = true;
                        }
                    }

                    this.cmdInquiry.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdInquiry.IsEnabled = true;
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdMovement.IsEnabled = true;
                        this.cmdConvertUnits.IsEnabled = true;
                        this.cmdInventoryCounts.IsEnabled = true;
                        this.cmdSkidMaintenance.IsEnabled = true;
                        this.cmdAdjustments.IsEnabled = true;
                        this.cmdInquiry.IsEnabled = true;
                    }

                    this.cmdMovement.Focus();

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
        public void cmdMovement_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucInventoryMovementMenu();
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
        public void cmdConvertUnits_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucActivityConvertUnit();
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
        public void cmdInventoryCounts_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucInventoryCountsMenu();
            ucInventoryCountsMenu countsMenu = new ucInventoryCountsMenu();
            Device.BeginInvokeOnMainThread(async () => await countsMenu.Show());
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
        public void cmdSkidMaintenance_Click(object sender, EventArgs e)
        {
            //this.callingControl = new ucActivityHdrSkidInfo(new ucInventoryMenu());
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
        public void cmdInquiry_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucActivityInventoryInquiry();
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
        public void cmdAdjustments_Click(object sender, EventArgs e)
        {
            //this.CallingControl = new ucInventoryAdjustmentsMenu();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

            this.IsControlLoaded = false;
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

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdAdjustments.IsEnabled = false;
            this.cmdConvertUnits.IsEnabled = false;
            this.cmdInquiry.IsEnabled = false;
            this.cmdInventoryCounts.IsEnabled = false;
            this.cmdMovement.IsEnabled = false;
            this.cmdSkidMaintenance.IsEnabled = false;

            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        #endregion
    }
}