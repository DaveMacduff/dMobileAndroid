using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.Activity;
using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucPurchaseOrdersMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;

        #endregion

        #region Constructors

        public ucPurchaseOrdersMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdReceiving.Text = "1) Receiving";
            this.cmdPutAway.Text = "2) Put Away";
            this.cmdWorkSheet.Text = "3) Work Sheet";

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

                if (keycode == Keycode.Enter && this.cmdWorkSheet.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    ucActivityHdr worksheet = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdReceiving.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    ucActivityHdr receiving = new ucActivityHdr(SysCommon.ActSubTypes.Receiving);
                    await receiving.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdPutAway.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    ucActivitySkidPutAway putAway = new ucActivitySkidPutAway();
                    await putAway.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdWorkSheet.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    ucActivityHdr worksheet = new ucActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                }
            });

            try
            {
                this.ucPurchaseOrdersMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucPurchaseOrdersMenu_GotFocus threw an error " + ex, "OK"));
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

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                this.DisableControls();
                this.IsControlLoaded = false;
                await this.Close();
            });

            return true;
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
            return "Purchase Orders Menu";
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
            this.cmdPutAway.IsEnabled = false;
            this.cmdReceiving.IsEnabled = false;
            this.cmdWorkSheet.IsEnabled = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucPurchaseOrdersMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.cmdReceiving.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsReceivingModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdReceiving.IsEnabled = true;
                        }
                    }

                    this.cmdPutAway.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPutAwayModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            if (this.SysEnv.SysSettings.IsSkidTrackingModule)
                            {
                                this.cmdPutAway.IsEnabled = true;
                            }
                        }
                    }

                    this.cmdWorkSheet.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPOWorkSheetModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdWorkSheet.IsEnabled = true;
                        }
                    }

                    // Override all checks if module bypass Isenabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.IsEnableAllModuleBypassLicense && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdReceiving.IsEnabled = true;
                        this.cmdPutAway.IsEnabled = true;
                        this.cmdWorkSheet.IsEnabled = true;
                    }

                    this.cmdReceiving.Focus();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdReceiving_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;
            ucActivityHdr receiving = new ucActivityHdr(SysCommon.ActSubTypes.Receiving);
            await receiving.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPutAway_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;
            ucActivitySkidPutAway putAway = new ucActivitySkidPutAway();
            await putAway.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdWorkSheet_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;
            ucActivityPurchaseOrderWorkSheet worksheet = new ucActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
            await worksheet.Show();
        }

        #endregion
    }
}