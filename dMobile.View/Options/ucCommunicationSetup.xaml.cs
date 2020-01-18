namespace dMobile.View.Options
{
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucCommunicationSetup : DMobileContentPage, IAppArea, ISysSocketEventListener, IAppPublisher
    {
        //private UserControl callingControl = new ucOptionsMenu();
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private int minIPAdress = 0;
        private int maxIPAdress = 255;
        private int minPort = 0;
        private int maxPort = 20000;
        private int maxNumberOfPorts = 100;
        private SysEnvironment sysEnv;
        private SysSocket sysSock;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList;

        public ucCommunicationSetup()
        {
            InitializeComponent();

            this.Title = this.GetControlDescription();

            // TODO: Resource strings
            this.lblMobileServerIP.Text = "Mobile Server IP Address";
            this.lblBasePort.Text = "Base Port";
            this.lblMaxPorts.Text = "Max Ports";
            this.lblDefaultPort.Text = "Default Port";
            this.cmdAdvanced.Text = "Advanced";
            this.cmdConnect.Text = "Connect";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.SysSock = new SysSocket();
                this.ItemsList = new ObservableCollection<string>();

                try
                {
                    this.ucCommunicationSetup_GotFocus(this, null);
                }
                catch (Exception ex)
                {
                    // TODO: Logging
                    Console.WriteLine("ucCommunicationSetup_GotFocus threw an error" + ex);
                }
            }
        }

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

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
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



        private SysSocket SysSock
        {
            get
            {
                return this.sysSock;
            }

            set
            {
                this.sysSock = value;
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

        public ObservableCollection<string> ItemsList
        {
            get
            {
                return this.itemsList;
            }
            set
            {
                this.itemsList = value;
            }
        }

        private int MinIPAddress
        {
            get
            {
                return this.minIPAdress;
            }
        }

        private int MaxIPAddress
        {
            get
            {
                return this.maxIPAdress;
            }
        }

        private int MinPort
        {
            get
            {
                return this.minPort;
            }
        }

        private int MaxPort
        {
            get
            {
                return this.maxPort;
            }
        }

        private int MaxNumberOfPorts
        {
            get
            {
                return this.maxNumberOfPorts;
            }
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
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
            return "Communication Setup";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                int maxPort = Convert.ToInt32(int.Parse(this.updnBasePort.Text) + int.Parse(this.updnMaxPorts.Text) - 1);

                isReadyToSave = isReadyToSave && (int.Parse(this.updnBasePort.Text) >= this.MinPort && int.Parse(this.updnBasePort.Text) <= this.MaxPort);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnMaxPorts.Text) >= this.MinPort && int.Parse(this.updnMaxPorts.Text) <= this.MaxNumberOfPorts);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnIP1.Text) >= this.MinIPAddress && int.Parse(this.updnIP1.Text) <= this.MaxIPAddress);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnIP2.Text) >= this.MinIPAddress && int.Parse(this.updnIP2.Text) <= this.MaxIPAddress);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnIP3.Text) >= this.MinIPAddress && int.Parse(this.updnIP3.Text) <= this.MaxIPAddress);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnIP4.Text) >= this.MinIPAddress && int.Parse(this.updnIP4.Text) <= this.MaxIPAddress);
                isReadyToSave = isReadyToSave && (int.Parse(this.updnDefaultPort.Text) >= int.Parse(this.updnBasePort.Text) && int.Parse(this.updnDefaultPort.Text) <= this.MaxPort);
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            bool isSaveComplete = false;
            try
            {
                if (this.IsReadyToSave())
                {
                    this.SysEnv.CommSetup.iHostIPAddressComponent1 = int.Parse(this.updnIP1.Text);
                    this.SysEnv.CommSetup.iHostIPAddressComponent2 = int.Parse(this.updnIP2.Text);
                    this.SysEnv.CommSetup.iHostIPAddressComponent3 = int.Parse(this.updnIP3.Text);
                    this.SysEnv.CommSetup.iHostIPAddressComponent4 = int.Parse(this.updnIP4.Text);
                    this.SysEnv.CommSetup.iBaseSocketPort = int.Parse(this.updnBasePort.Text);
                    this.SysEnv.CommSetup.iDefaultSocketPort = int.Parse(this.updnDefaultPort.Text);
                    this.SysEnv.CommSetup.iMaxSocketPorts = int.Parse(this.updnMaxPorts.Text);

                    CommunicationSetup tempCommSetup = this.SysEnv.CommSetup;
                    isSaveComplete = this.SysEnv.CommSetup.SaveData(ref tempCommSetup);

                    this.IsChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isSaveComplete = false;
                ex.Show(/*this,*/ SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(/*this,*/ SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                //using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                //{
                //    errRpt.ShowDialog();
                //}

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
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
                // IsReadyToClose checks if any changes where made.
                if (this.IsReadyToClose())
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
                ex.Show(/*this,*/ SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                //using (FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                //{
                //    errorReport.ShowDialog();
                //}

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
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
        public void ucCommunicationSetup_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.LoadData();
                this.IsControlLoaded = true;
                this.updnIP1.Focus();

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
        public void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                // Socket Base Port
                //if (sender == this.updnBasePort)
                //{
                if (true == (int.Parse(this.updnBasePort.Text) < this.MinPort || int.Parse(this.updnBasePort.Text) > this.MaxPort))
                {
                    e.Cancel = true;
                    throw new Exceptions.MessageOnlyException(string.Format("Base port must be between {0} and {1}", this.MinPort, this.MaxPort));
                }
                //}

                // Socket Max Ports
                //if (sender == this.updnMaxPorts)
                //{
                if (true == (int.Parse(this.updnMaxPorts.Text) < this.MinPort || int.Parse(this.updnMaxPorts.Text) > this.MaxNumberOfPorts))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("port must be between {0} and {1}", this.MinPort, this.MaxNumberOfPorts));
                }
                //}

                // IP Address Component 1
                //if (sender == this.updnIP1)
                //{
                if (true == (int.Parse(this.updnIP1.Text) < this.MinIPAddress || int.Parse(this.updnIP1.Text) > this.MaxIPAddress))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("IP address component must be between {0} and {1}", this.MinIPAddress, this.MaxIPAddress));
                }
                //}

                // IP Address Component 2
                //if (sender == this.updnIP2)
                //{
                if (true == (int.Parse(this.updnIP2.Text) < this.MinIPAddress || int.Parse(this.updnIP2.Text) > this.MaxIPAddress))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("IP address component must be between {0} and {1}", this.MinIPAddress, this.MaxIPAddress));
                }
                //}

                // IP Address Component 3
                //if (sender == this.updnIP3)
                //{
                if (true == (int.Parse(this.updnIP3.Text) < this.MinIPAddress || int.Parse(this.updnIP3.Text) > this.MaxIPAddress))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("IP address component must be between {0} and {1}", this.MinIPAddress, this.MaxIPAddress));
                }
                //}

                // IP Address Component 4
                //if (sender == this.updnIP4)
                //{
                if (true == (int.Parse(this.updnIP4.Text) < this.MinIPAddress || int.Parse(this.updnIP4.Text) > this.MaxIPAddress))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("IP address component must be between {0} and {1}", this.MinIPAddress, this.MaxIPAddress));
                }
                //}

                // Default Socket Port
                //if (sender == this.updnDefaultPort)
                //{
                int maxPort = (int)(int.Parse(this.updnBasePort.Text) + int.Parse(this.updnMaxPorts.Text) - 1);

                if (true == (int.Parse(this.updnDefaultPort.Text) < int.Parse(this.updnBasePort.Text) || int.Parse(this.updnDefaultPort.Text) > this.MaxPort))
                {
                    e.Cancel = true;

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException(string.Format("Default port must be between {0} and {1}", this.updnBasePort.Text, this.MaxPort));
                }
                //}
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(/*this,*/ SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                //using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                //{
                //    errRpt.ShowDialog();
                //}

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
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
                    this.IsChangesMade = true;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(/*this,*/ SysCommon.ActSubTypes.CommunicationSetup);
            }
            catch (Exception ex)
            {
                //using (FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, this, ex))
                //{
                //    errRpt.ShowDialog();
                //}

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.CommunicationSetup, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdvanced_Click(object sender, EventArgs e)
        {
            // Needed to check if the enter key was pressed last, because it seems that
            // SelectNextControl causes a click event to fire if it is the next control
            this.DisableControls();
            //this.CallingControl = new ucCommunicationSetupAdvanced();

            ucCommunicationSetupAdvanced setupAdvanced = new ucCommunicationSetupAdvanced();
            await setupAdvanced.Show();

            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdConnect_Click(object sender, EventArgs e)
        {
            this.ValidateEntry(this, new CancelEventArgs());

            if (this.SaveChanges())
            {
                await this.Connect();
                ucCommunicationSetupStatus status = new ucCommunicationSetupStatus();
                status.ItemsList = this.ItemsList;
                await status.ShowDialog();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        //public void SelectValueGotFocus(object sender, EventArgs e)
        //{
        //    //SysCommon.SelectValueInNumericUpDown((UpDownBase)sender);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sysSocketEventPublisher"></param>
        /// <param name="sysSocketEventArgs"></param>
        /// <remarks></remarks>
        public void OnSysSocketStatusOut(ISysSocketEventPublisher sysSocketEventPublisher, SysSocketEventArgs sysSocketEventArgs)
        {
            this.AddCommMsg(this, null, sysSocketEventArgs.StatusOutMsg);
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



        private void DisableControls()
        {
            ////cmdAdvanced.Enabled = False
            ////cmdConnect.Enabled = False
            ////lstCommMsg.Enabled = False
            ////updnBasePort.Enabled = False
            ////updnDefaultPort.Enabled = False
            ////updnIP1.Enabled = False
            ////updnIP2.Enabled = False
            ////updnIP3.Enabled = False
            ////updnIP4.Enabled = False
            ////updnMaxPorts.Enabled = False

            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.SysEnv.CommSetup = this.SysEnv.CommSetup.LoadData();

            this.updnIP1.Text = this.SysEnv.CommSetup.iHostIPAddressComponent1.ToString();
            this.updnIP2.Text = this.SysEnv.CommSetup.iHostIPAddressComponent2.ToString();
            this.updnIP3.Text = this.SysEnv.CommSetup.iHostIPAddressComponent3.ToString();
            this.updnIP4.Text = this.SysEnv.CommSetup.iHostIPAddressComponent4.ToString();
            this.updnBasePort.Text = this.SysEnv.CommSetup.iBaseSocketPort.ToString();
            this.updnDefaultPort.Text = this.SysEnv.CommSetup.iDefaultSocketPort.ToString();
            this.updnMaxPorts.Text = this.SysEnv.CommSetup.iMaxSocketPorts.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task Connect()
        {
            this.Loading = true;

            await Task.Run(() =>
            {
                string sendReq;
                Socket socket;
                int bytesSent;
                List<byte> bytRecvData;
                string reqData = string.Empty;

                this.ItemsList.Clear();
                //this.lstCommMsg.ItemsSource = null;

                this.SysEnv.SysSock.BasePort = this.SysEnv.CommSetup.iBaseSocketPort;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Base socket port: {0}", this.SysEnv.SysSock.BasePort));

                this.SysEnv.SysSock.DefSocketPort = this.SysEnv.CommSetup.iDefaultSocketPort;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Default socket port: {0}", this.SysEnv.SysSock.DefSocketPort));

                this.SysEnv.SysSock.MaxPorts = this.SysEnv.CommSetup.iMaxSocketPorts;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Max socket port: {0}", this.SysEnv.SysSock.MaxPorts));

                this.SysEnv.SysSock.ConnTotalRetries = this.SysEnv.CommSetup.iConnectionAttempts;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Connection retries: {0}", this.SysEnv.SysSock.ConnTotalRetries));

                this.SysEnv.SysSock.SendTotalRetries = this.SysEnv.CommSetup.iSendAttempts;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Socket send retries: {0}", this.SysEnv.SysSock.SendTotalRetries));

                this.SysEnv.SysSock.RecvTotalRetries = this.SysEnv.CommSetup.iReceiveAttempts;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Socket receive retries: {0}", this.SysEnv.SysSock.RecvTotalRetries));

                this.SysEnv.SysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.iSendTimeOutSeconds * 1000;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Send time out: {0} seconds.", this.SysEnv.CommSetup.iSendTimeOutSeconds));

                this.SysEnv.SysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.iReceiveTimeOutSeconds * 1000;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Receive time out: {0} seconds.", this.SysEnv.CommSetup.iReceiveTimeOutSeconds));

                this.SysEnv.SysSock.RecvBuffSize = this.SysEnv.CommSetup.iiReceiveBufferSize;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Receive buffer size: {0} ", this.SysEnv.SysSock.RecvBuffSize));

                this.SysEnv.SysSock.ServerIP = this.SysEnv.CommSetup.iHostIPAddressComponent1
                    + "." + this.SysEnv.CommSetup.iHostIPAddressComponent2 + "."
                    + this.SysEnv.CommSetup.iHostIPAddressComponent3 + "."
                    + this.SysEnv.CommSetup.iHostIPAddressComponent4;

                // TODO: Resource strings
                this.AddCommMsg(this, EventArgs.Empty, string.Format("Server host ip: {0}", this.SysEnv.SysSock.ServerIP));
                this.AddCommMsg(this, EventArgs.Empty, "Creating connection request.");

                sendReq = CommunicationSetup.CreateXMLSubmitDeviceID();

                try
                {
                    this.SysSock.RegSockListenerStatusOut((ISysSocketEventListener)this);

                    this.SysEnv.IsCommunicationTestSuccessful = false;

                    //// If DateAndTime.Hour(DateTime.Now) Mod 2 = 0 And 
                    //// DateAndTime.Minute(DateTime.Now) Mod 2 = 0 Then
                    //// Dim strAppDir As String = Path.GetDirectoryName(
                    //// Assembly.GetExecutingAssembly().GetName().CodeBase)
                    //// Dim strFullPathToMyFile As String = Path.Combine(
                    //// strAppDir, "dMobileResource03.wav")    'modem sound
                    //// Dim oSnd As clsSound = New clsSound(strFullPathToMyFile)
                    //// oSnd.Play()
                    //// Thread.Sleep(25000)
                    //// End If

                    socket = this.SysEnv.SysSock.Connect();

                    if (socket == null)
                    {
                        // TODO: Resource strings
                        this.AddCommMsg(this, EventArgs.Empty, "Connection failed.");
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.AddCommMsg(this, EventArgs.Empty, "Sending device id.");
                        bytesSent = this.SysEnv.SysSock.SendRequest(socket, sendReq);

                        this.AddCommMsg(this, EventArgs.Empty, string.Format("Bytes sent: {0}", bytesSent));

                        // TODO: Resource strings
                        this.AddCommMsg(this, EventArgs.Empty, "Waiting for response.");
                        bytRecvData = this.SysEnv.SysSock.ReceiveResponse(socket);

                        socket.Close();

                        // TODO: Resource strings
                        this.AddCommMsg(this, EventArgs.Empty, "Socket closed.");

                        reqData = Compress.UnZip(bytRecvData.ToArray());

                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            string errorDetail = docError.DocumentElement["detail"].InnerText;

                            // TODO: Resource strings
                            this.AddCommMsg(this, EventArgs.Empty, "Submit device id error.");
                            this.AddCommMsg(this, EventArgs.Empty, errorDetail);
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddCommMsg(this, EventArgs.Empty, "Connection succeeded.");
                            this.SysEnv.IsCommunicationTestSuccessful = true;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    // TODO: Resource strings
                    this.AddCommMsg(this, EventArgs.Empty, "Socket exception occurred.");
                    this.AddCommMsg(this, EventArgs.Empty, "Connection failed.");

                    //MessageBox.Show(string.Format("Code {0}, {1}", ex.ErrorCode, ex.Message));

                    // TODO: Resource strings
                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", string.Format("Code {0}, {1}", ex.ErrorCode, ex.Message), "OK"));
                }
                finally
                {
                    this.SysSock.UnregSockListenerStatusOut((ISysSocketEventListener)this);
                }
            });

            this.Loading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <remarks></remarks>
        private void AddCommMsg(object sender, EventArgs e, string message)
        {
            this.ItemsList.Add(message);
        }

        private void UpdnIP1_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP1.CursorPosition = 0;
            this.updnIP1.SelectionLength = this.updnIP1.Text.Length;
        }

        private void UpdnIP2_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP2.CursorPosition = 0;
            this.updnIP2.SelectionLength = this.updnIP2.Text.Length;
        }

        private void UpdnIP3_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP3.CursorPosition = 0;
            this.updnIP3.SelectionLength = this.updnIP3.Text.Length;
        }

        private void UpdnIP4_Focused(object sender, FocusEventArgs e)
        {
            this.updnIP4.CursorPosition = 0;
            this.updnIP4.SelectionLength = this.updnIP4.Text.Length;
        }

        private void UpdnBasePort_Focused(object sender, FocusEventArgs e)
        {
            this.updnBasePort.CursorPosition = 0;
            this.updnBasePort.SelectionLength = this.updnBasePort.Text.Length;
        }

        private void UpdnMaxPorts_Focused(object sender, FocusEventArgs e)
        {
            this.updnMaxPorts.CursorPosition = 0;
            this.updnMaxPorts.SelectionLength = this.updnMaxPorts.Text.Length;
        }

        private void UpdnDefaultPort_Focused(object sender, FocusEventArgs e)
        {
            this.updnDefaultPort.CursorPosition = 0;
            this.updnDefaultPort.SelectionLength = this.updnDefaultPort.Text.Length;
        }
    }
}
