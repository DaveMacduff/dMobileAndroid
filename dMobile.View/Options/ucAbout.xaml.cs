namespace dMobile.View.Options
{
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucAbout : DMobileContentPage, IAppArea, IAppPublisher
    {
        //UserControl callingControl = new ucOptionsMenu();
        NetworkInterface nic = null;
        private bool isControlLoaded = false;
        private ObservableCollection<IPAddr> itemsList;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucAbout()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            // TODO: Resource strings
            this.lblDeviceNameIP.Text = "Device Serial Number";
            this.lblMACAddress.Text = "MAC Address:";
            this.lblStatus.Text = "Status:";
            this.lblMACAddressData.Text = "<Unknown>";
            this.lblStatusData.Text = "<Unknown>";
            this.lblSpeed.Text = "Speed:";
            this.lblSpeedData.Text = "<Unknown>";
            this.cmdShowMemory.Text = "Show Memory";
            this.cmdVersionInfo.Text = "Version Info";

            this.itemsList = new ObservableCollection<IPAddr>();

            if (!DesignMode.IsDesignModeEnabled)
            {
                try
                {
                    this.RefreshNICList();
                    this.ucAbout_GotFocus(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    // TODO: Logging
                    Console.WriteLine("Error in ucAbout.xaml.cs " + ex);
                }
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.lstDeviceNameIP.ItemsSource = this.itemsList;

            if (this.itemsList.Count > 0)
            {
                this.lstDeviceNameIP.SelectedItem = this.itemsList[0];
            }

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
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

        public bool IsControlLoaded
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

        public NetworkInterface NIC
        {
            get
            {
                return this.nic;
            }

            set
            {
                this.nic = value;
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
            return "About dMobile";
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
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }




        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucAbout_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                    this.lblDeviceIDData.Text = SysCommon.WordWrap2String(SysDeviceID.GetDeviceID(), 30);

                    //if (this.lstDeviceNameIP.SelectedIndices.Count != 1)
                    //{
                    //    this.lstDeviceNameIP.Items[0].Focused = true;
                    //}

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshNICList()
        {
            //this.lstDeviceNameIP.BeginUpdate();
            //this.lstDeviceNameIP.Items.Clear();

            IPAddress IpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (IPAddress.IsLoopback(ip.Address))
                        {
                            continue;
                        }

                        IPAddr test = new IPAddr() { networkInterface = ni };
                        test.DisplayMember = ni.Name + "    " + ip.Address.ToString();

                        this.itemsList.Add(test);
                    }
                }

                //string s = ni.Name + " " + ni.GetPhysicalAddress().ToString();

                //string[] s = new string[] { ni.Name, ni.CurrentIpAddress.ToString() };
                //ListViewItem lvi = new ListViewItem(s);
                //lvi.Tag = ni;
                //this.lstDeviceNameIP.Items.Add(lvi);
            }

            //this.lstDeviceNameIP.EndUpdate();
        }

        private class IPAddr : ContentPage
        {
            public NetworkInterface networkInterface;
            private string displayMember;

            public string DisplayMember
            {
                get
                {
                    return this.displayMember;
                }

                set
                {
                    this.displayMember = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdShowMemory_Click(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Memory Usage", SysCommon.ShowMemory(), "OK"));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdVersionInfo_Click(object sender, EventArgs e)
        {
            string viewsAssemblyVersion = new SysShared().GetAssemblyVersion();
            string barCodeScannerAssemblyVersion = new BarCodeScanner.clsCommon().GetAssemblyVersion();
            string groundWorkAssemblyVersion = new GroundWork.Common().GetAssemblyVersion();
            string controllerAssemblyVersion = new Controller.Common().GetAssemblyVersion();
            string bedrockAssemblyVersion = new SysCommon().GetAssemblyVersion();

            // TODO: Resource strings
            string message = string.Format("Views {0} " + Environment.NewLine +
                "Scan Engine {1} " + Environment.NewLine +
                "Ground Work {2} " + Environment.NewLine +
                "Controller {3} " + Environment.NewLine +
                "Bedrock {4} " + Environment.NewLine,
                viewsAssemblyVersion,
                barCodeScannerAssemblyVersion,
                groundWorkAssemblyVersion,
                controllerAssemblyVersion,
                bedrockAssemblyVersion);

            // TODO: Resource strings
            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Assembly Versions", message, "OK"));
        }


        private void LstDeviceNameIP_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (lstDeviceNameIP.SelectedIndices.Count > 0)
            //{
            //this.NIC = (NetworkInterface)(this.lstDeviceNameIP.Items[this.lstDeviceNameIP.SelectedIndices[0]].Tag);
            var test = (IPAddr)this.lstDeviceNameIP.SelectedItem;
            this.NIC = test.networkInterface;
            this.lblMACAddressData.Text = this.NIC.GetPhysicalAddress().ToString();
            this.lblSpeedData.Text = this.NIC.Speed.ToString();
            this.lblStatusData.Text = this.NIC.OperationalStatus.ToString();
            //}
        }
    }
}
