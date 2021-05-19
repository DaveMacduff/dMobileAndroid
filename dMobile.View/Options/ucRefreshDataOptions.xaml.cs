namespace dMobile.View.Options
{
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucRefreshDataOptions : DMobileContentPage, IAppArea, IAppPublisher
    {
        //private UserControl callingControl = new ucUtilitiesMenu();
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private bool isRefreshComplete = false;
        private bool isDisplayShutDownMsg = true;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        public ucRefreshDataOptions()
        {
            InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();

                try
                {
                    this.ucRefreshDataOptions_GotFocus(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "ucRefreshDataOptions_GotFocus exception: " + ex.Message, "OK"));
                }
            }

            this.cmdRefreshData.Text = Resource.RefreshData;
            this.cmdDataStatus.Text = Resource.DataStatus;
            this.lblRefreshDataChangesOnly.Text = Resource.RefreshDataChangesOnly;
            this.lblSelectAll.Text = Resource.SelectDeselectAll;
            this.lblSystemData.Text = Resource.SystemData;
            this.lblAdjustments.Text = Resource.Adjustments;
            this.lblPurchaseOrders.Text = Resource.PurchaseOrders;
            this.lblSalesOrders.Text = Resource.SalesOrders;
            this.lblProductionTickets.Text = Resource.ProductionTickets;
            this.lblReplenishment.Text = Resource.Replenishment;
            this.lblCounts.Text = Resource.Counts;
            this.lblTransfers.Text = Resource.Transfers;
            this.lblPurchaseOrderWorkSheet.Text = Resource.PurchaseOrderWorkSheet;
        }

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
                        await this.Close();
                    }
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

        //private UserControl ucCallingControl
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

        private bool IsDisplayShutDownMsg
        {
            get
            {
                return this.isDisplayShutDownMsg;
            }

            set
            {
                this.isDisplayShutDownMsg = value;
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

        private bool IsRefreshComplete
        {
            get
            {
                return this.isRefreshComplete;
            }

            set
            {
                this.isRefreshComplete = value;
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

            try
            {
                // if select all make sure all items are selected. including disconnected mode activities if applicable
                if (this.chkSelectAll.IsToggled)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        isReadyToSave = isReadyToSave && this.chkSystemData.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkRefreshDataChangesOnly.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkAdjustments.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkCounts.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkProductionTickets.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkPurchaseOrders.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkReplenishment.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkSalesOrders.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkTransfers.IsToggled;
                        isReadyToSave = isReadyToSave && this.chkPurchaseOrderWorkSheet.IsToggled;
                    }
                }

                // if we are not in disconnected mode do not allow activity refreshing
                if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    isReadyToSave = isReadyToSave && !this.chkAdjustments.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkCounts.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkProductionTickets.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkPurchaseOrders.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkReplenishment.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkSalesOrders.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkTransfers.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkPurchaseOrderWorkSheet.IsToggled;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
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
                    await this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
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
        public async Task RefreshCompelete()
        {
            if (this.IsRefreshComplete)
            {
                if (this.IsDisplayShutDownMsg)
                {
                    this.IsDisplayShutDownMsg = false;

                    // Attempt to record logout
                    if (!this.SysEnv.SysSettings.IsDisconnectedMode && (this.SysEnv.CommSetup.iHostIPAddressComponent1 + this.SysEnv.CommSetup.iHostIPAddressComponent2 + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
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
                                // TODO: Logging
                                Console.WriteLine(ex);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                Application.Current.MainPage = new NavigationPage(new SplashStartup());
                                // TODO: Resource strings
                                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Please log back in for changes to be made in effect.", "OK"));
                            }
                        }
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new SplashStartup());
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Please log back in for changes to be made in effect.", "OK"));
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return Resource.RefreshDataOptions;
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
                    this.SysEnv.RefreshOpt.RefreshDataChangesOnly = this.chkRefreshDataChangesOnly.IsToggled;
                    this.SysEnv.RefreshOpt.SystemData = this.chkSystemData.IsToggled;
                    this.SysEnv.RefreshOpt.Adjustments = Convert.ToBoolean(this.chkAdjustments.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.PurchaseOrders = Convert.ToBoolean(this.chkPurchaseOrders.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.SalesOrders = Convert.ToBoolean(this.chkSalesOrders.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.ProductionTickets = Convert.ToBoolean(this.chkProductionTickets.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Replenishment = Convert.ToBoolean(this.chkReplenishment.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Counts = Convert.ToBoolean(this.chkCounts.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Transfers = Convert.ToBoolean(this.chkTransfers.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet = Convert.ToBoolean(this.chkPurchaseOrderWorkSheet.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);

                    RefreshDataOptions tempRefreshOpt = this.SysEnv.RefreshOpt;
                    saveComplete = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt);

                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), Resource.DataFailedValidationCheckYourData, Resource.OK));
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
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
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
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
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                    await errorReport.ShowDialog();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucRefreshDataOptions_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                    this.IsControlLoaded = true;
                    this.cmdRefreshData.Focus();
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRefreshData_Click(object sender, EventArgs e)
        {
            bool isContinueRefresh = false;

            if (this.SysEnv.SysSettings.IsDisconnectedMode && await SysCommon.CheckForUnSubmittedData())
            {
                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), Resource.WarningUnsubmittedDataExistsAndWillBeLostContinueWithRefreshData, Resource.Yes, Resource.No)))
                {
                    if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), Resource.ConfirmRefreshData, Resource.Yes, Resource.No)))
                    {
                        isContinueRefresh = true;
                    }
                }
            }
            else if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Confirm refresh data?", Resource.Yes, Resource.No)))
            {
                isContinueRefresh = true;
            }

            if (isContinueRefresh)
            {
                if (await this.SaveChanges())
                {
                    this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucRefreshDataStatus, SysCommon.ActSubTypes.RefreshData);

                        await form.DisplayControl.ShowDialog();

                        if (form.DialogResult == DialogResult.None)
                        {
                            await form.SaveChanges(this, new EventArgs());
                        }

                        XDocument xmlDoc = form.DisplayControlData;
                        this.IsRefreshComplete = bool.Parse(xmlDoc.Root.Element("IsRefreshComplete").Value);
                        await this.RefreshCompelete();
                    });

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDataStatus_Click(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucRefreshDataStatus, SysCommon.ActSubTypes.DataStatus);
                Device.BeginInvokeOnMainThread(async () => await form.DisplayControl.ShowDialog());

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void chkSelectAll_Click(object sender, EventArgs e)
        {
            this.chkSystemData.IsToggled = this.chkSelectAll.IsToggled && await dMobile.GroundWork.Common.IsDataBaseDataExist();

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.chkAdjustments.IsToggled = this.chkSelectAll.IsToggled;
                this.chkPurchaseOrders.IsToggled = this.chkSelectAll.IsToggled;
                this.chkSalesOrders.IsToggled = this.chkSelectAll.IsToggled;
                this.chkProductionTickets.IsToggled = this.chkSelectAll.IsToggled;
                this.chkReplenishment.IsToggled = this.chkSelectAll.IsToggled;
                this.chkCounts.IsToggled = this.chkSelectAll.IsToggled;
                this.chkTransfers.IsToggled = this.chkSelectAll.IsToggled;
                this.chkPurchaseOrderWorkSheet.IsToggled = this.chkSelectAll.IsToggled;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsAllItemsSelected()
        {
            bool isAllSelected = false;

            isAllSelected = this.chkSystemData.IsToggled
                && Convert.ToBoolean(this.chkAdjustments.IsToggled && this.chkPurchaseOrders.IsToggled
                && this.chkSalesOrders.IsToggled && this.chkProductionTickets.IsToggled && this.chkReplenishment.IsToggled
                && this.chkCounts.IsToggled && this.chkTransfers.IsToggled
                && this.chkPurchaseOrderWorkSheet.IsToggled ? true : false);

            return isAllSelected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void ClickHandler(object sender, EventArgs e)
        {
            if (!this.chkSelectAll.IsToggled && this.IsAllItemsSelected())
            {
                this.chkSelectAll.IsToggled = await dMobile.GroundWork.Common.IsDataBaseDataExist();
            }
            else if (this.chkSelectAll.IsToggled && !this.IsAllItemsSelected())
            {
                this.chkSelectAll.IsToggled = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void IdentifyChangesMade(object sender, ToggledEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    if (sender == this.cmdDataStatus)
                    {
                    }
                    else if (sender == this.cmdRefreshData)
                    {
                    }
                    else if (sender == this.chkAdjustments)
                    {
                    }
                    else if (sender == this.chkCounts)
                    {
                    }
                    else if (sender == this.chkProductionTickets)
                    {
                    }
                    else if (sender == this.chkPurchaseOrders)
                    {
                    }
                    else if (sender == this.chkPurchaseOrderWorkSheet)
                    {
                    }
                    else if (sender == this.chkRefreshDataChangesOnly)
                    {
                        this.chkRefreshDataChangesOnly.IsToggled = this.chkRefreshDataChangesOnly.IsToggled && await dMobile.GroundWork.Common.IsDataBaseDataExist();
                    }
                    else if (sender == this.chkReplenishment)
                    {
                    }
                    else if (sender == this.chkSalesOrders)
                    {
                    }
                    else if (sender == this.chkSelectAll)
                    {
                    }
                    else if (sender == this.chkSystemData)
                    {
                    }
                    else if (sender == this.chkTransfers)
                    {
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. MessageOnlyException is really not and error. 
                // Allow the validation to handle the setting for IsChangesMade.
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;
                FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                if (sender == this.cmdDataStatus)
                {
                }
                else if (sender == this.cmdRefreshData)
                {
                }
                else if (sender == this.chkAdjustments)
                {
                }
                else if (sender == this.chkCounts)
                {
                }
                else if (sender == this.chkProductionTickets)
                {
                }
                else if (sender == this.chkPurchaseOrders)
                {
                }
                else if (sender == this.chkPurchaseOrderWorkSheet)
                {
                }
                else if (sender == this.chkRefreshDataChangesOnly)
                {
                }
                else if (sender == this.chkReplenishment)
                {
                }
                else if (sender == this.chkSalesOrders)
                {
                }
                else if (sender == this.chkSelectAll)
                {
                }
                else if (sender == this.chkSystemData)
                {
                }
                else if (sender == this.chkTransfers)
                {
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not allow the exit of control.
                e.Cancel = true;
                ex.Show(SysCommon.ActSubTypes.RefreshData);
            }
            catch (Exception ex)
            {
                // Do not allow the exit of control.
                e.Cancel = true;
                FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SysEnv.RefreshOpt.LoadData();

            this.chkRefreshDataChangesOnly.IsToggled = this.SysEnv.RefreshOpt.RefreshDataChangesOnly;
            this.chkSystemData.IsToggled = this.SysEnv.RefreshOpt.SystemData;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.chkAdjustments.IsToggled = this.SysEnv.RefreshOpt.Adjustments;
                this.chkCounts.IsToggled = this.SysEnv.RefreshOpt.Counts;
                this.chkProductionTickets.IsToggled = this.SysEnv.RefreshOpt.ProductionTickets;
                this.chkPurchaseOrders.IsToggled = this.SysEnv.RefreshOpt.PurchaseOrders;
                this.chkReplenishment.IsToggled = this.SysEnv.RefreshOpt.Replenishment;
                this.chkSalesOrders.IsToggled = this.SysEnv.RefreshOpt.SalesOrders;
                this.chkTransfers.IsToggled = this.SysEnv.RefreshOpt.Transfers;
                this.chkPurchaseOrderWorkSheet.IsToggled = this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet;
            }
            else
            {
                this.chkAdjustments.IsEnabled = false;
                this.chkCounts.IsEnabled = false;
                this.chkProductionTickets.IsEnabled = false;
                this.chkPurchaseOrders.IsEnabled = false;
                this.chkReplenishment.IsEnabled = false;
                this.chkSalesOrders.IsEnabled = false;
                this.chkTransfers.IsEnabled = false;
                this.chkPurchaseOrderWorkSheet.IsEnabled = false;
            }
        }
    }
}
