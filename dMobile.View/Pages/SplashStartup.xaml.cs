namespace dMobile.View
{
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using dMobile.View.Themes;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashStartup : DMobileContentPage, IAppArea, IAppPublisher, IDataNubListener
    {
        //private int totalTicks = 0;
        private bool isErrorRaised = false;
        //private UserControl callingControl = null;
        private SysEnvironment sysEnv = null;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList;

        public SplashStartup()
        {
            this.InitializeComponent();

            this.Title = GetControlDescription();
            this.ItemsList = new ObservableCollection<string>();
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.listBoxStatus.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                try
                {
                    Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Escape || keycode == Keycode.Grave)
                    {
                        //Application.Current.Quit();
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    }
                    else if (keycode == Keycode.Enter)
                    {
                        if (this.btnClose.IsEnabled)
                        {
                            this.BtnClose_Clicked(null, null);
                        }
                    }
                    else
                    {
                        // clear any pending key events
                        //Application.DoEvents();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            await Task.Run(async () => await this.LoadData(this, new EventArgs()));
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

        //public int TotalTicks
        //{
        //    get
        //    {
        //        return this.totalTicks;
        //    }

        //    set
        //    {
        //        this.totalTicks = value;
        //    }
        //}

        public bool IsErrorRaised
        {
            get
            {
                return this.isErrorRaised;
            }

            set
            {
                this.isErrorRaised = value;
            }
        }

        //public UserControl CallingControl
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
            bool isReadyToSave = true;
            return isReadyToSave;
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
        //    if (this.IsErrorRaised)
        //    {
        //        return this.CallingControl;
        //    }
        //    else
        //    {
        //        return new LoginStartup();
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "dMobile Startup";
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
            XDocument doc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return doc;
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

        public void OnNubStatOut(
            IDataNubPublisher databaseNubEventPublisher,
            DataNubEventArgs databaseNubEventArgs)
        {
            this.StartUpStatusOut(this, new EventArgs(), databaseNubEventArgs.StatusOutMsg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void LoadLocationMap(object sender, EventArgs e)
        {
            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Loading location map.");

            LocationMap locMap = LocationMap.GetInstance();
            locMap.Load();

            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Loading location map completed.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void DataIntegrityCheck()
        {
            if (SysEnv.RefreshOpt.IsRefreshDataFailed)
            {
                //MessageBox.Show(
                //    "The last data refresh did not complete. You must refresh the data again.",
                //    this.GetControlDescription(),
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Exclamation,
                //    MessageBoxDefaultButton.Button1);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    // TODO: Resource Strings
                    await DisplayAlert(this.GetControlDescription(), "The last data refresh did not complete. You must refresh the data again.", "OK");
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool SyncDateTime(object sender, EventArgs e)
        {
            bool isDateTimeChanged = false;

            //if (!this.SysEnv.SysSettings.IsDisconnectedMode
            //    && (this.SysEnv.CommSetup.iHostIPAddressComponent1
            //    + this.SysEnv.CommSetup.iHostIPAddressComponent2
            //    + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
            //{


            //    this.StartUpStatusOut(sender, e, "Attempting to sync date & time.");

            //    SysSocket sysSock = new SysSocket();
            //    sysSock.BasePort = this.SysEnv.CommSetup.iBaseSocketPort;
            //    sysSock.DefSocketPort = this.SysEnv.CommSetup.iDefaultSocketPort;
            //    sysSock.MaxPorts = this.SysEnv.CommSetup.iMaxSocketPorts;
            //    sysSock.ConnTotalRetries = this.SysEnv.CommSetup.iConnectionAttempts;
            //    sysSock.SendTotalRetries = this.SysEnv.CommSetup.iSendAttempts;
            //    sysSock.RecvTotalRetries = this.SysEnv.CommSetup.iReceiveAttempts;
            //    sysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.iSendTimeOutSeconds * 1000;
            //    sysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.iReceiveTimeOutSeconds * 1000;
            //    sysSock.RecvBuffSize = this.SysEnv.CommSetup.iiReceiveBufferSize;
            //    sysSock.ServerIP = Convert.ToString(this.SysEnv.CommSetup.iHostIPAddressComponent1.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent2.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent3.ToString() + "."
            //        + this.SysEnv.CommSetup.iHostIPAddressComponent4.ToString());
            //    try
            //    {
            //        System.Net.Sockets.Socket socket;
            //        socket = sysSock.Connect();

            //        if (socket == null)
            //        {
            //            // TODO: Resource strings
            //            this.StartUpStatusOut(sender, e, "Connection failed.");
            //        }
            //        else
            //        {
            //            // TODO: Resource strings
            //            this.StartUpStatusOut(sender, e, "Requesting date & time.");

            //            string sendRequest = this.CreateXMLSyncDateTime();
            //            int bytesSent = sysSock.SendRequest(socket, sendRequest);
            //            List<byte> recvData = sysSock.ReceiveResponse(socket);
            //            socket.Close();

            //            string data = Compress.UnZip(recvData.ToArray());
            //            if (data.Contains("<message>ERROR</message>"))
            //            {
            //                XmlDocument docError = new XmlDocument();
            //                docError.LoadXml(data);

            //                string errorDetail = docError.DocumentElement["detail"].InnerText;

            //                // TODO: Resource strings
            //                this.StartUpStatusOut(sender, e, "Request date & time error.");
            //                this.StartUpStatusOut(sender, e, errorDetail);
            //            }
            //            else
            //            {
            //                using (DataSet ds = new DataSet())
            //                {
            //                    ds.ReadXml(SysCommon.GetXmlReader(ref data), XmlReadMode.Auto);
            //                    if (ds.Tables.Count != 0)
            //                    {
            //                        //OpenNETCF.Runtime.InteropServices.SYSTEMTIME sysTime = new OpenNETCF.Runtime.InteropServices.SYSTEMTIME();
            //                        //OpenNETCF.Runtime.InteropServices.SYSTEMTIME sysTimeLocal = new OpenNETCF.Runtime.InteropServices.SYSTEMTIME();

            //                        foreach (DataRow drow in ds.Tables["tDateTime"].Rows)
            //                        {
            //                            //sysTime.wYear = Convert.ToInt16(drow["system_year"]);
            //                            //sysTime.wMonth = Convert.ToInt16(drow["system_month"]);
            //                            //sysTime.wDay = Convert.ToInt16(drow["system_day"]);
            //                            //sysTime.wHour = Convert.ToInt16(drow["system_hour"]);
            //                            //sysTime.wMinute = Convert.ToInt16(drow["system_minute"]);
            //                            //sysTime.wSecond = Convert.ToInt16(drow["system_second"]);
            //                            //sysTime.wDayOfWeek = Convert.ToInt16(drow["system_dayofweek"]);
            //                            //DateTimeHelper.SystemTime = sysTime.ToDateTime();

            //                            //sysTimeLocal.wYear = Convert.ToInt16(drow["local_year"]);
            //                            //sysTimeLocal.wMonth = Convert.ToInt16(drow["local_month"]);
            //                            //sysTimeLocal.wDay = Convert.ToInt16(drow["local_day"]);
            //                            //sysTimeLocal.wHour = Convert.ToInt16(drow["local_hour"]);
            //                            //sysTimeLocal.wMinute = Convert.ToInt16(drow["local_minute"]);
            //                            //sysTimeLocal.wSecond = Convert.ToInt16(drow["local_second"]);
            //                            //sysTime.wDayOfWeek = Convert.ToInt16(drow["local_dayofweek"]);
            //                            //DateTimeHelper.LocalTime = sysTimeLocal.ToDateTime();
            //                        }

            //                        this.StartUpStatusOut(sender, e, "Date & time synchronized.");
            //                        isDateTimeChanged = true;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (System.Net.Sockets.SocketException)
            //    {
            //        // TODO: Resource strings
            //        this.StartUpStatusOut(sender, e, "Socket exception occurred.");
            //        this.StartUpStatusOut(sender, e, "Sync date & time failed.");
            //    }
            //}

            return isDateTimeChanged;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateXMLSyncDateTime()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string deviceID = SysDeviceID.GetDeviceID();

            XDocument request = System.Xml.Linq.XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST TIME</action>"
                + ("<device_id>" + deviceID + "</device_id>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void lstStatus_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (this.timerStatus.Enabled == false)
        //    {
        //        if (e.KeyCode == Keys.Escape)
        //        {
        //            Application.Current.Exit();
        //        }
        //    }
        //    else
        //    {
        //        // clear any pending key events
        //        Application.DoEvents();
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task InitializeDatabase(object sender, EventArgs e)
        {
            DatabaseNub db = new DatabaseNub();

            this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = false;

            db.RegNubListenerStatOut(this);

            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Locating database.");

            if (File.Exists(DatabaseConnection.GetInstance().DBASCIIZ) && new FileInfo(DatabaseConnection.GetInstance().DBASCIIZ).Length > 0)
            {
                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, "Database found.");
                this.StartUpStatusOut(sender, e, DatabaseConnection.GetInstance().DBASCIIZ);
            }
            else
            {
                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, "Database missing.");
                this.StartUpStatusOut(sender, e, "Creating database.");

                db.CreateNewDatabase();
                db.CreateTables();

                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, "Seeding system users.");

                TableContactUsers users = new TableContactUsers();
                users.SeedContactUsersWithSystemUsers();

                this.IsErrorRaised = db.ErrorRaised;

                if (this.IsErrorRaised)
                {
                    //                    MessageBox.Show(
                    //#if MULTILANGUAGE
                    //                        Translator.GetInstance().Translate(TranslatorVocabulary.DBCreationFailed),
                    //#else
                    //                        "Database creation failed.",
                    //#endif
                    //                        this.GetControlDescription(),
                    //                        MessageBoxButtons.OK,
                    //                        MessageBoxIcon.Exclamation,
                    //                        MessageBoxDefaultButton.Button1);

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        //TODO: Resource Strings
                        await DisplayAlert(this.GetControlDescription(), "Database creation failed.", "OK");
                    });
                }
                else
                {
                    // TODO: Resource Strings
                    this.StartUpStatusOut(sender, e, "Database creation completed.");

                    this.SysEnv.RefreshOpt.IsDataCreatedInStartUp = true;
                }
            }

            RefreshDataOptions tempRefreshDataOptions = this.SysEnv.RefreshOpt;
            this.SysEnv.RefreshOpt.SaveData(ref tempRefreshDataOptions);
            db.UnregNubListenerStatOut(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void InitializeScanner(object sender, EventArgs e)
        {
            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Identifying bar code scanner.");

            BarCodeScanner scanner = BarCodeSingleton.GetInstance().Scanner;

            if (scanner == null)
            {
                //MessageBox.Show(
                //    "Unable to identify bar code scanner.",
                //    this.GetControlDescription(),
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Exclamation,
                //    MessageBoxDefaultButton.Button1);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    //TODO: Resource Strings
                    await DisplayAlert(this.GetControlDescription(), "Unable to identify bar code scanner.", "OK", "Cancel");
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void LoadProducts(object sender, EventArgs e)
        {
            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Loading products.");

            ////Products productList = Products.GetInstance();
            ////productList.GetProductList();
            ////productList.GetProductUnitList();

            // TODO: Resource Strings
            this.StartUpStatusOut(sender, e, "Loading products completed.");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="strMessage"></param>
        /// <remarks></remarks>
        private void StartUpStatusOut(object sender, EventArgs e, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ItemsList.Add(message);
                this.listBoxStatus.ScrollTo(message, ScrollToPosition.MakeVisible, false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task LoadData(object sender, EventArgs e)
        {
            this.Loading = true;

            await Task.Run(async () =>
            {                
                //this.TotalTicks = this.TotalTicks + this.timerStatus.Interval;

                //if (this.Enabled && this.TotalTicks >= 1000)
                //if (this.IsEnabled)
                //{
                //this.timerStatus.Enabled = false;

                try
                {
                    // TODO: Resource Strings
                    this.StartUpStatusOut(sender, e, "Initializing environment.");

                    this.SysEnv = SysEnvironment.GetInstance();

                    this.SysEnv.IsCommunicationTestSuccessful = this.CommunicationTest(sender, e);

                    if (this.SysEnv.IsCommunicationTestSuccessful)
                    {
                        // Cannot do this
                        //this.SyncDateTime(sender, e);
                    }

                    this.InitializeScanner(sender, e);

                    await this.InitializeDatabase(sender, e);

                    if (!this.IsErrorRaised)
                    {
                        this.LoadProducts(sender, e);
                    }

                    if (!this.IsErrorRaised)
                    {
                        this.LoadLocationMap(sender, e);
                    }

                    DataIntegrityCheck();

                    this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.SplashStartup, ex, this.GetControlDescription(), "timerStatus_Tick");
                        await errReport.ShowDialog();
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    });
                }
                //}
            });

            this.Loading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CommunicationTest(object sender, EventArgs e)
        {
            bool testSuccessful = false;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode && (this.SysEnv.CommSetup.iHostIPAddressComponent1 + this.SysEnv.CommSetup.iHostIPAddressComponent2 + this.SysEnv.CommSetup.iHostIPAddressComponent3) != 0)
            {
                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, "Attempting communication test.");

                SysSocket sysSock = new SysSocket();

                sysSock.BasePort = this.SysEnv.CommSetup.iBaseSocketPort;
                sysSock.DefSocketPort = this.SysEnv.CommSetup.iDefaultSocketPort;
                sysSock.MaxPorts = this.SysEnv.CommSetup.iMaxSocketPorts;
                sysSock.ConnTotalRetries = this.SysEnv.CommSetup.iConnectionAttempts;
                sysSock.SendTotalRetries = this.SysEnv.CommSetup.iSendAttempts;
                sysSock.RecvTotalRetries = this.SysEnv.CommSetup.iReceiveAttempts;

                this.StartUpStatusOut(sender, e, string.Format("Base socket port: {0}", sysSock.BasePort));
                this.StartUpStatusOut(sender, e, string.Format("Default socket port: {0}", sysSock.DefSocketPort));
                this.StartUpStatusOut(sender, e, string.Format("Max socket port: {0}", sysSock.MaxPorts));
                this.StartUpStatusOut(sender, e, string.Format("Connection retries: {0}", sysSock.ConnTotalRetries));
                this.StartUpStatusOut(sender, e, string.Format("Socket send retries: {0}", sysSock.SendTotalRetries));
                this.StartUpStatusOut(sender, e, string.Format("Socket receive retries: {0}", sysSock.RecvTotalRetries));

                sysSock.SendTimeOutMilliseconds = this.SysEnv.CommSetup.iSendTimeOutSeconds * 1000;

                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, string.Format("Send time out: {0} seconds.", this.SysEnv.CommSetup.iSendTimeOutSeconds));

                sysSock.ReceiveTimeOutMilliseconds = this.SysEnv.CommSetup.iReceiveTimeOutSeconds * 1000;

                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, string.Format("Receive time out: {0} seconds.", this.SysEnv.CommSetup.iReceiveTimeOutSeconds));

                sysSock.RecvBuffSize = this.SysEnv.CommSetup.iiReceiveBufferSize;

                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, string.Format("Receive buffer size: {0} ", sysSock.RecvBuffSize));

                sysSock.ServerIP = this.SysEnv.CommSetup.iHostIPAddressComponent1 + "."
                    + this.SysEnv.CommSetup.iHostIPAddressComponent2 + "."
                    + this.SysEnv.CommSetup.iHostIPAddressComponent3 + "."
                    + this.SysEnv.CommSetup.iHostIPAddressComponent4;

                // TODO: Resource Strings
                this.StartUpStatusOut(sender, e, string.Format("Server host IP: {0}", sysSock.ServerIP));
                this.StartUpStatusOut(sender, e, "Creating connection request.");

                try
                {
                    System.Net.Sockets.Socket socket;
                    socket = sysSock.Connect();

                    if (socket == null)
                    {
                        // TODO: Resource Strings
                        this.StartUpStatusOut(sender, e, "Connection failed.");
                    }
                    else
                    {
                        // TODO: Resource Strings
                        this.StartUpStatusOut(sender, e, "Sending device ID.");

                        string sendReq = CommunicationSetup.CreateXMLSubmitDeviceID();
                        int bytesSent = sysSock.SendRequest(socket, sendReq);

                        // TODO: Resource Strings
                        this.StartUpStatusOut(sender, e, string.Format("Bytes sent: {0}", bytesSent));
                        this.StartUpStatusOut(sender, e, "Waiting for response.");

                        List<byte> bytRecvData = sysSock.ReceiveResponse(socket);

                        socket.Close();

                        // TODO: Resource Strings
                        this.StartUpStatusOut(sender, e, "Socket closed.");

                        string recvData = Compress.UnZip(bytRecvData.ToArray());
                        if (recvData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(recvData);

                            string errorDetail = docError.DocumentElement["detail"].InnerText;

                            // TODO: Resource Strings
                            this.StartUpStatusOut(sender, e, "Submit device id error.");
                            this.StartUpStatusOut(sender, e, errorDetail);
                        }
                        else
                        {
                            this.StartUpStatusOut(sender, e, "Connection succeeded.");
                            testSuccessful = true;
                        }
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    // TODO: Resource Strings
                    this.StartUpStatusOut(sender, e, "Socket exception occurred.");
                    this.StartUpStatusOut(sender, e, "Connection failed.");

                    //MessageBox.Show("Communication test failed.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //TODO: Resource Strings
                        await DisplayAlert(this.GetControlDescription(), "Communication test failed.", "OK");
                    });
                }
            }

            return testSuccessful;
        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new NavigationPage(new ucLoginStartup()));
        }
    }
}
