using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Activity;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryAdjustmentsMenu : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucInventoryMenu();
        private SysEnvironment sysEnv;
        private AdjustmentSettings adjSettings;
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
        public ucInventoryAdjustmentsMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdRework.Text = "Rework";
            this.cmdProduction.Text = "Production";
            this.cmdInventory.Text = "Inventory";
            this.cmdRequisition.Text = "Material Requisition";
            this.cmdDisposal.Text = "Disposal";
            this.cmdConvertProduct.Text = "Convert Label";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.AdjSettings = new AdjustmentSettings();
            }
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.F1 || keycode == Keycode.Escape)
                {
                    this.DisableControls();
                    //this.CallingControl = new ucInventoryMenu();
                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num1)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Inventory);
                }
                else if (keycode == Keycode.Num2)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Production);
                }
                else if (keycode == Keycode.Num3)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Rework);
                }
                else if (keycode == Keycode.Num4)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Requisition);
                }
                else if (keycode == Keycode.Num5)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Disposal);
                }
                else if (keycode == Keycode.Num6)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
                }
            });

            try
            {
                this.ucInventoryAdjustmentsMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucInventoryAdjustmentsMenu_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ScannerDisable();
            
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
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(this.reqCloseEvent, value);
            }

            remove
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(this.reqCloseEvent, value);
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

        private AdjustmentSettings AdjSettings
        {
            get
            {
                return this.adjSettings;
            }

            set
            {
                this.adjSettings = value;
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

        #endregion

        #region Public Members

        #region IAppArea Members

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
        public async Task<bool> IsReadyToSave()
        {
            bool IsReadyToSave = true;

            return IsReadyToSave;
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
            return "Adjustments Menu";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucInventoryAdjustmentsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.AdjSettings = this.AdjSettings.LoadData();

                    this.cmdInventory.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdInventory.IsEnabled = true;
                        }
                    }

                    this.cmdProduction.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdProduction.IsEnabled = true;
                        }
                    }

                    this.cmdRework.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdRework.IsEnabled = true;
                        }
                    }

                    this.cmdRequisition.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdRequisition.IsEnabled = true;
                        }
                    }

                    this.cmdDisposal.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdDisposal.IsEnabled = true;
                        }
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdInventory.IsEnabled = true;
                        this.cmdProduction.IsEnabled = true;
                        this.cmdRework.IsEnabled = true;
                        this.cmdRequisition.IsEnabled = true;
                        this.cmdDisposal.IsEnabled = true;
                    }

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdInventory_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.Inventory);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdInventory_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Inventory);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProduction_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Production);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdProduction_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.Production);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRework_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Rework);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdRework_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.Rework);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDisposal_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Disposal);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdDisposal_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.Disposal);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRequisition_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Requisition);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void cmdRequisition_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.Requisition);
        //}

        /// <summary>
        /// Click handler for Convert Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdConvertProduct_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
        }

        /// <summary>
        /// Displays the Convert Product form.
        /// </summary>
        private async Task ShowConvertProductForm()
        {
            //this.CallingControl = new ucActivityConvertProduct();
            this.DisableControls();
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucActivityConvertProduct convertProduct = new ucActivityConvertProduct();
            await convertProduct.Show();
        }

        ///// <summary>
        ///// Key up handler for Convert Product.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void cmdConvertProduct_KeyUp(object sender, KeyEventArgs e)
        //{
        //    HandleKeyPress(e, SysCommon.AdjTypes.ConvertProduct);
        //}

        ///// <summary>
        ///// Generic keypress handler.
        ///// </summary>
        ///// <param name="defaultAdjType">Type that will be set if enter key is pressed.</param>
        //private void HandleKeyPress(KeyEventArgs e, SysCommon.AdjTypes defaultAdjType)
        //{
        //    if (e.KeyValue == 134)
        //    {
        //        SetAdjustmentType(defaultAdjType);
        //        return;
        //    }

        //    switch (e.KeyCode)
        //    {
        //        case Keys.Enter:
        //            SetAdjustmentType(defaultAdjType);
        //            break;

        //        // Equivalent to Escape || F1
        //        case Keys.Escape:
        //        case Keys.F1:
        //            this.DisableControls();
        //            //this.CallingControl = new ucInventoryMenu();
        //            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

        //            this.IsControlLoaded = false;
        //            break;

        //        case Keys.D1:
        //            SetAdjustmentType(SysCommon.AdjTypes.Inventory);
        //            break;
        //        case Keys.D2:
        //            SetAdjustmentType(SysCommon.AdjTypes.Production);
        //            break;
        //        case Keys.D3:
        //            SetAdjustmentType(SysCommon.AdjTypes.Rework);
        //            break;
        //        case Keys.D4:
        //            SetAdjustmentType(SysCommon.AdjTypes.Requisition);
        //            break;
        //        case Keys.D5:
        //            SetAdjustmentType(SysCommon.AdjTypes.Disposal);
        //            break;
        //        case Keys.D6:
        //            SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
        //            break;
        //    }
        //}

        /// <summary>
        /// Sets the adjustment type and saves adjustment settings.
        /// </summary>
        /// <param name="adjType"></param>
        private async Task SetAdjustmentType(SysCommon.AdjTypes adjType)
        {
            if (adjType == SysCommon.AdjTypes.ConvertProduct)
            {
                // Placed here to get the default case of keyhandler as well.
                await ShowConvertProductForm();
            }
            else
            {
                this.AdjSettings.AdjustmentType = adjType;
                await this.SetSaveAdjSettings();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdDisposal.IsEnabled = false;
            this.cmdInventory.IsEnabled = false;
            this.cmdProduction.IsEnabled = false;
            this.cmdRework.IsEnabled = false;
            this.cmdDisposal.IsEnabled = false;
            this.cmdRequisition.IsEnabled = false;
            
            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task SetSaveAdjSettings()
        {
            this.DisableControls();
            this.AdjSettings.IsApplyAdjustmentType = true;
            this.AdjSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
            AdjustmentSettings tempAdjSettings = this.AdjSettings;
            this.AdjSettings.SaveData(ref tempAdjSettings);
            this.AdjSettings = tempAdjSettings;
            //this.CallingControl = new ucActivityHdr(SysCommon.ActSubTypes.Adjustment);
            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.IsControlLoaded = false;

            ucActivityHdr ucActivityHdr =  new ucActivityHdr(SysCommon.ActSubTypes.Adjustment);
            await ucActivityHdr.Show();
        }

        #endregion
    }
}