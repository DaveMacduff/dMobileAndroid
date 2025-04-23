// <copyright file="InventoryMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Media;
    using Android.Views;
    using Bedrock;
    using Resources.Enums;
    using Activity;
    using CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryMenu"/> class.
        /// </summary>
        public InventoryMenu()
        {
            this.InitializeComponent();

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

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter && this.cmdMovement.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var inventoryMovementMenu = new InventoryMovementMenu();
                    await inventoryMovementMenu.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdMovement.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var inventoryMovementMenu = new InventoryMovementMenu();
                    await inventoryMovementMenu.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdConvertUnits.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var convertUnit = new ActivityConvertUnit();
                    await convertUnit.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdInventoryCounts.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var countsMenu = new InventoryCountsMenu();
                    await countsMenu.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdSkidMaintenance.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var skidInfo = new ActivityHdrSkidInfo(SysShared.ActivityType.Header);
                    _ = await skidInfo.ShowDialog();
                }
                else if (keycode == Keycode.Num5 && this.cmdInquiry.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var inquiry = new ActivityInventoryInquiry();
                    _ = await inquiry.ShowDialog();
                }
                else if (keycode == Keycode.Num6 && this.cmdAdjustments.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var adjustmentsMenu = new InventoryAdjustmentsMenu();
                    await adjustmentsMenu.Show();
                }
            });

            try
            {
                this.ucInventoryMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucInventoryMenu_GotFocus threw an error " + ex, "OK"));
            }
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

        /// <inheritdoc/>
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
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Inventory Menu";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<bool> CancelChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucInventoryMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdMovement.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsMovesModule || this.SysEnv.SysSettings.IsReplenishmentModule
                        || this.SysEnv.SysSettings.IsTransMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdMovement.IsEnabled = true;
                    }
                }

                this.cmdConvertUnits.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsConvertUnitsModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdConvertUnits.IsEnabled = true;
                    }
                }

                this.cmdInventoryCounts.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.SysSettings.IsCycleModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdInventoryCounts.IsEnabled = true;
                    }
                }

                this.cmdSkidMaintenance.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsSkidTrackingModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdSkidMaintenance.IsEnabled = true;
                    }
                }

                this.cmdAdjustments.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdAdjustments.IsEnabled = true;
                    }
                }

                this.cmdInquiry.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
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

                _ = this.cmdMovement.Focus();
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

            var inventoryMovementMenu = new InventoryMovementMenu();
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

            var convertUnit = new ActivityConvertUnit();
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

            var countsMenu = new InventoryCountsMenu();
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

            var skidInfo = new ActivityHdrSkidInfo(SysShared.ActivityType.Header);
            _ = await skidInfo.ShowDialog();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void CmdInquiry_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () => await this.DisplayAlert(
                        "Inventory Inquiry",
                        "Quantity inquiry is not supported in disconnected mode.",
                        "OK"));
            }
            else
            {
                var inquiry = new ActivityInventoryInquiry();
                _ = await inquiry.ShowDialog();
            }
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

            var adjustmentsMenu = new InventoryAdjustmentsMenu();
            await adjustmentsMenu.Show();
        }

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
    }
}