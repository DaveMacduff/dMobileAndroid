// <copyright file="About.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// About screen.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : DMobileContentPage, IAppArea
    {
        private ObservableCollection<IPAddr> itemsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

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
                    this.ucAbout_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in About.xaml.cs " + ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control has been loaded.
        /// </summary>
        public bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets a network interface.
        /// </summary>
        public NetworkInterface Nic { get; set; } = null;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
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
        /// Is ready to save.
        /// </summary>
        /// <returns><see cref="int"/> True if ready to save.</returns>
        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        /// <summary>
        /// Is ready to close.
        /// </summary>
        /// <returns><see cref="int"/> True if ready to close.</returns>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return "About dMobile";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<bool> CancelChanges()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public async Task<XDocument> GetXML()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// The OnAppearing.
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
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        await this.Close();
                    }
                }
            });
        }

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucAbout_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.lblDeviceIDData.Text = SysCommon.WordWrap2String(SysDeviceID.GetDeviceID(), 30);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshNICList()
        {
            var IpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (IPAddress.IsLoopback(ip.Address))
                        {
                            continue;
                        }

                        var test = new IPAddr() { networkInterface = ni };
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
                get => this.displayMember;

                set
                {
                    this.displayMember = value;
                    this.OnPropertyChanged();
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
            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Memory Usage", SysCommon.ShowMemory(), "OK"));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdVersionInfo_Click(object sender, EventArgs e)
        {
            var viewsAssemblyVersion = SysShared.GetAssemblyVersion();
            var barCodeScannerAssemblyVersion = new BarCodeScanner.clsCommon().GetAssemblyVersion();
            var groundWorkAssemblyVersion = new GroundWork.Common().GetAssemblyVersion();
            var controllerAssemblyVersion = new Controller.Common().GetAssemblyVersion();
            var bedrockAssemblyVersion = new SysCommon().GetAssemblyVersion();

            var message = string.Format("Views {0} " + Environment.NewLine +
                                        "Scan Engine {1} " + Environment.NewLine +
                                        "Ground Work {2} " + Environment.NewLine +
                                        "Controller {3} " + Environment.NewLine +
                                        "Bedrock {4} " + Environment.NewLine,
                viewsAssemblyVersion,
                barCodeScannerAssemblyVersion,
                groundWorkAssemblyVersion,
                controllerAssemblyVersion,
                bedrockAssemblyVersion);

            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Assembly Versions", message, "OK"));
        }

        private void LstDeviceNameIP_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var test = (IPAddr)this.lstDeviceNameIP.SelectedItem;
            this.Nic = test.networkInterface;
            this.lblMACAddressData.Text = this.Nic.GetPhysicalAddress().ToString();
            this.lblSpeedData.Text = this.Nic.Speed.ToString();
            this.lblStatusData.Text = this.Nic.OperationalStatus.ToString();
        }
    }
}
