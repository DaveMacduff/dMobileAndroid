namespace dMobile.View.Menus
{
    using dMobile.View.CustomPages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.View.Activity;
    using dMobile.Resources.Enums;
    using Android.Views;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSalesOrdersMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;

        #endregion

        #region Constructors

        public ucSalesOrdersMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdPicking.Text = "1) " + "Picking";
            this.cmdReturns.Text = "2) " + "Returns";
            this.cmdReleaseByOrder.Text = "3) " + "Release By Order #";
            this.cmdReleaseBySection.Text = "4) " + "Release By Section";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }
        }

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Num1)
                {
                    this.DisableControls();

                    ucActivityHdr picking = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
                    await picking.Show();

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num2)
                {
                    this.DisableControls();

                    ucActivityHdr returns = new ucActivityHdr(SysCommon.ActSubTypes.Returns);
                    await returns.Show();

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num3)
                {
                    this.DisableControls();

                    ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);
                    await releaseByOrder.Show();

                    this.IsControlLoaded = false;
                }
            });

            try
            {
                this.ucSalesOrdersMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucSalesOrdersMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());

            base.OnDisappearing();
        }

        #region Properties

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
            return "Sales Orders Menu";
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

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdPicking.IsEnabled = false;
            this.cmdReleaseByOrder.IsEnabled = false;
            this.cmdReleaseBySection.IsEnabled = false;
            this.cmdReturns.IsEnabled = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPicking_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            if (this.SysEnv.SysSettings.IsPickSingleProd)
            {
                ucActivityHdr pickingSingle = new ucActivityHdr(SysCommon.ActSubTypes.PickingSingle);
                await pickingSingle.Show();
            }
            else
            {
                ucActivityHdr picking = new ucActivityHdr(SysCommon.ActSubTypes.Picking);
                await picking.Show();
            }

            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdReturns_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            ucActivityHdr returns = new ucActivityHdr(SysCommon.ActSubTypes.Returns);
            await returns.Show();

            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSalesOrdersMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                    this.cmdPicking.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPickMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdPicking.IsEnabled = true;
                        }
                    }

                    this.cmdReturns.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRetMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReturns.IsEnabled = true;
                        }
                    }

                    this.cmdReleaseByOrder.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRelByOrder || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReleaseByOrder.IsEnabled = true;
                        }
                    }

                    this.cmdReleaseBySection.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsRelBySect || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReleaseBySection.IsEnabled = false;   // Disabled intentionally
                        }
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdPicking.IsEnabled = true;
                        this.cmdReturns.IsEnabled = true;
                        this.cmdReleaseByOrder.IsEnabled = true;
                        this.cmdReleaseBySection.IsEnabled = true;
                    }

                    this.cmdPicking.Focus();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdReleaseByOrder_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            if (this.SysEnv.SysSettings.IsPickSingleProd)
            {
                ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.PickingSingle);
                await releaseByOrder.Show();
            }
            else
            {
                ucActivityHdrReleaseByOrder releaseByOrder = new ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes.Picking);
                await releaseByOrder.Show();
            }

            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void cmdReleaseBySection_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;
        }

        #endregion
    }
}