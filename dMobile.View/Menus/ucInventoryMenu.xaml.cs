using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Activity;
using Android.Media;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucInventoryMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdMovement.Text = "1) Movement";
            this.cmdConvertUnits.Text = "2) Convert Units (Break Case)";
            this.cmdInventoryCounts.Text = "3) Counts";
            this.cmdSkidMaintenance.Text = "4) Skids";
            this.cmdInquiry.Text = "5) Inquiry";
            this.cmdAdjustments.Text = "6) Adjustments";

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

                if (keycode == Keycode.Enter && this.cmdMovement.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucInventoryMovementMenu inventoryMovementMenu = new ucInventoryMovementMenu();
                    await inventoryMovementMenu.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdMovement.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucInventoryMovementMenu inventoryMovementMenu = new ucInventoryMovementMenu();
                    await inventoryMovementMenu.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdConvertUnits.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityConvertUnit convertUnit = new ucActivityConvertUnit();
                    await convertUnit.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdInventoryCounts.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucInventoryCountsMenu countsMenu = new ucInventoryCountsMenu();
                    await countsMenu.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdSkidMaintenance.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityHdrSkidInfo skidInfo = new ucActivityHdrSkidInfo();
                    await skidInfo.ShowDialog();
                }
                else if (keycode == Keycode.Num5 && this.cmdInquiry.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityInventoryInquiry inquiry = new ucActivityInventoryInquiry();
                    await inquiry.ShowDialog();
                }
                else if (keycode == Keycode.Num6 && this.cmdAdjustments.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucInventoryAdjustmentsMenu adjustmentsMenu = new ucInventoryAdjustmentsMenu();
                    await adjustmentsMenu.Show();
                }
            });

            try
            {
                this.ucInventoryMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucInventoryMenu_GotFocus threw an error " + ex, "OK"));
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
            return "Inventory Menu";
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
        public void ucInventoryMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.cmdMovement.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsMovesModule || this.SysEnv.SysSettings.IsReplenishmentModule
                            || this.SysEnv.SysSettings.IsTransMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdMovement.IsEnabled = true;
                        }
                    }

                    this.cmdConvertUnits.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsConvertUnitsModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdConvertUnits.IsEnabled = true;
                        }
                    }

                    this.cmdInventoryCounts.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.SysSettings.IsCycleModule
                          || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdInventoryCounts.IsEnabled = true;
                        }
                    }

                    this.cmdSkidMaintenance.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsSkidTrackingModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdSkidMaintenance.IsEnabled = true;
                        }
                    }

                    this.cmdAdjustments.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdAdjustments.IsEnabled = true;
                        }
                    }

                    this.cmdInquiry.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdInquiry.IsEnabled = true;
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdMovement.IsEnabled = true;
                        this.cmdConvertUnits.IsEnabled = true;
                        this.cmdInventoryCounts.IsEnabled = true;
                        this.cmdSkidMaintenance.IsEnabled = true;
                        this.cmdAdjustments.IsEnabled = true;
                        this.cmdInquiry.IsEnabled = true;
                    }

                    this.cmdMovement.Focus();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdMovement_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucInventoryMovementMenu inventoryMovementMenu = new ucInventoryMovementMenu();
            await inventoryMovementMenu.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdConvertUnits_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucActivityConvertUnit convertUnit = new ucActivityConvertUnit();
            await convertUnit.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdInventoryCounts_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucInventoryCountsMenu countsMenu = new ucInventoryCountsMenu();
            await countsMenu.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSkidMaintenance_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucActivityHdrSkidInfo skidInfo = new ucActivityHdrSkidInfo();
            await skidInfo.ShowDialog();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdInquiry_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucActivityInventoryInquiry inquiry = new ucActivityInventoryInquiry();
            await inquiry.ShowDialog();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdjustments_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucInventoryAdjustmentsMenu adjustmentsMenu = new ucInventoryAdjustmentsMenu();
            await adjustmentsMenu.Show();
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdAdjustments.IsEnabled = false;
            this.cmdConvertUnits.IsEnabled = false;
            this.cmdInquiry.IsEnabled = false;
            this.cmdInventoryCounts.IsEnabled = false;
            this.cmdMovement.IsEnabled = false;
            this.cmdSkidMaintenance.IsEnabled = false;
        }

        #endregion
    }
}