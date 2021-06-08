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
    public partial class ucAbout : DMobileContentPage, IAppArea
    {
        NetworkInterface nic = null;
        private bool isControlLoaded = false;
        private ObservableCollection<IPAddr> itemsList;

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
                        await this.Close();
                    }
                }
            });
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    base.OnBackButtonPressed();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
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
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshNICList()
        {
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
            }
        }

        private class IPAddr : DMobileContentPage
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
            var test = (IPAddr)this.lstDeviceNameIP.SelectedItem;
            this.NIC = test.networkInterface;
            this.lblMACAddressData.Text = this.NIC.GetPhysicalAddress().ToString();
            this.lblSpeedData.Text = this.NIC.Speed.ToString();
            this.lblStatusData.Text = this.NIC.OperationalStatus.ToString();
        }
    }
}
