// <copyright file="ucOptionsMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Options;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucOptionsMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ucOptionsMenu"/> class.
        /// </summary>
        public ucOptionsMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }

            this.cmdSetupDefaults.Text = "1) " + Resource.SetupDefaults;
            this.cmdCommunicationSetup.Text = "2) " + Resource.CommSetup;
            this.cmdRefreshData.Text = "3) " + Resource.RefreshData;
            this.cmdSubmitBatch.Text = "4) " + Resource.SubmitBatch;
            this.cmdUtilities.Text = "5) " + Resource.Utilities;
            this.cmdAbout.Text = "6) " + Resource.About;
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                this.EnableControls();
                this.ucOptionsMenu_GotFocus(this, new EventArgs());
                this.cmdRefreshData_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "ucOptionsMenu_GotFocus exception: " + ex.Message, "OK"));
            }

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter && this.cmdSetupDefaults.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucSetupDefaults setupDefaults = new ucSetupDefaults();
                    await setupDefaults.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdSetupDefaults.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucSetupDefaults setupDefaults = new ucSetupDefaults();
                    await setupDefaults.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdCommunicationSetup.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucCommunicationSetup commSetup = new ucCommunicationSetup();
                    await commSetup.Show();

                }
                else if (keycode == Keycode.Num3 && this.cmdRefreshData.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucRefreshDataOptions refreshDataOption = new ucRefreshDataOptions();
                    await refreshDataOption.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdSubmitBatch.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucSubmitBatch submitBatch = new ucSubmitBatch();
                    await submitBatch.Show();
                }
                else if (keycode == Keycode.Num5 && this.cmdUtilities.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucUtilitiesMenu utilities = new ucUtilitiesMenu();
                    await utilities.Show();
                }
                else if (keycode == Keycode.Num6 && this.cmdAbout.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucAbout about = new ucAbout();
                    await about.Show();
                }
            });
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                this.DisableControls();
                this.IsControlLoaded = false;
                await this.Close();
            });

            return true;
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
            return Resource.OptionsMenu;
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
        public async void cmdSetupDefaults_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucSetupDefaults();
            ucSetupDefaults setupDefaults = new ucSetupDefaults();
            await setupDefaults.Show();

            this.DisableControls();
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdCommunicationSetup_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucCommunicationSetup();
            this.DisableControls();
            this.IsControlLoaded = false;

            ucCommunicationSetup commSetup = new ucCommunicationSetup();
            await commSetup.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdRefreshData_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucRefreshDataOptions();
            this.DisableControls();
            this.IsControlLoaded = false;

            ucRefreshDataOptions refreshDataOption = new ucRefreshDataOptions();
            await refreshDataOption.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSubmitBatch_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucSubmitBatch();
            this.DisableControls();
            this.IsControlLoaded = false;

            ucSubmitBatch submitBatch = new ucSubmitBatch();
            await submitBatch.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAbout_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucAbout();
            this.DisableControls();
            this.IsControlLoaded = false;

            ucAbout about = new ucAbout();
            await about.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdUtilities_Click(object sender, EventArgs e)
        {
            // this.CallingControl = new ucUtilitiesMenu();
            this.DisableControls();
            this.IsControlLoaded = false;

            ucUtilitiesMenu utilities = new ucUtilitiesMenu();
            await utilities.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void cmdRefreshData_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                CommunicationSetup commSetup = new CommunicationSetup();

                if (File.Exists(commSetup.FileASCIIZ))
                {
                    this.cmdRefreshData.IsEnabled = true;
                }
                else
                {
                    this.cmdRefreshData.IsEnabled = false;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucOptionsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                if (this.SysEnv.IsCommunicationTestSuccessful)
                {
                    this.cmdRefreshData.IsEnabled = true;
                }
                else
                {
                    this.cmdRefreshData.IsEnabled = false;
                }

                if (this.SysEnv.IsCommunicationTestSuccessful && this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    this.cmdSubmitBatch.IsEnabled = true;
                }
                else
                {
                    this.cmdSubmitBatch.IsEnabled = false;
                }

                _ = this.cmdSetupDefaults.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdAbout.IsEnabled = false;
            this.cmdCommunicationSetup.IsEnabled = false;
            this.cmdRefreshData.IsEnabled = false;
            this.cmdSetupDefaults.IsEnabled = false;
            this.cmdSubmitBatch.IsEnabled = false;
            this.cmdUtilities.IsEnabled = false;
        }

        private void EnableControls()
        {
            this.cmdAbout.IsEnabled = true;
            this.cmdCommunicationSetup.IsEnabled = true;
            this.cmdRefreshData.IsEnabled = true;
            this.cmdSetupDefaults.IsEnabled = true;
            this.cmdSubmitBatch.IsEnabled = true;
            this.cmdUtilities.IsEnabled = true;
        }
    }
}
