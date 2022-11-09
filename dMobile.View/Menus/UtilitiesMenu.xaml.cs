// <copyright file="UtilitiesMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.Activity;
    using dMobile.View.CustomPages;
    using dMobile.View.Utilities;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UtilitiesMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UtilitiesMenu"/> class.
        /// </summary>
        public UtilitiesMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            this.cmdScanBarCodes.Text = "1) Scan Barcodes";
            this.cmdProductLabels.Text = "2) Product Labels";
            this.cmdAssignDefaultLocation.Text = "3) Assign Default Locations";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter && this.cmdProductLabels.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var productLabel = new ActivityProductLabel();
                    _ = await productLabel.ShowDialog();
                }
                else if (keycode == Keycode.Num1 && this.cmdScanBarCodes.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var scanBarcodes = new ActivityScanBarCodes();
                    await scanBarcodes.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdProductLabels.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var productLabel = new ActivityProductLabel();
                    _ = await productLabel.ShowDialog();
                }
                else if (keycode == Keycode.Num3 && this.cmdAssignDefaultLocation.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var assignDefaultLocation = new AssignDefaultLocation();
                    await assignDefaultLocation.Show();
                }
            });

            try
            {
                this.ucUtilitiesMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("this.ucUtilitiesMenu_GotFocus threw error " + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        private bool IsControlLoaded { get; set; } = false;

        private SysEnvironment SysEnv { get; set; }

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
            return "Utilities Menu";
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
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdProductLabels.IsEnabled = false;
            this.cmdScanBarCodes.IsEnabled = false;
            this.cmdAssignDefaultLocation.IsEnabled = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdScanBarCodes_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var scanBarcodes = new ActivityScanBarCodes();
            await scanBarcodes.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProductLabels_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var productLabel = new ActivityProductLabel();
            _ = await productLabel.ShowDialog();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdAssignDefaultLocation_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var assignDefaultLocation = new AssignDefaultLocation();
            await assignDefaultLocation.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucUtilitiesMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.cmdScanBarCodes.IsEnabled = true;

                if (this.SysEnv.IsCommunicationTestSuccessful && !this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    this.cmdProductLabels.IsEnabled = false;
                }
                else
                {
                    this.cmdProductLabels.IsEnabled = false;
                }

                if (this.SysEnv.IsCommunicationTestSuccessful && !this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    this.cmdAssignDefaultLocation.IsEnabled = true;
                }
                else
                {
                    this.cmdAssignDefaultLocation.IsEnabled = false;
                }

                _ = this.cmdScanBarCodes.Focus();
            }
        }
    }
}
