// <copyright file="InventoryMovementMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using Resources.Enums;
    using Activity;
    using CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.Bedrock.Settings;

    /// <summary>
    /// Inventory movement menu.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryMovementMenu : DMobileContentPage, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryMovementMenu"/> class.
        /// </summary>
        public InventoryMovementMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdMoveProduct.Text = "1) Move Product";
            this.cmdTransferFrom.Text = "2) Transfer From";
            this.cmdTransferTo.Text = "3) Transfer To";
            this.cmdMoveSkid.Text = "4) Move Skid";
            this.cmdReplenishment.Text = "5) Replenishment";
        }

        /// <summary>
        /// Gets or sets the transfer settings.
        /// </summary>
        private TransferSettings TransSettings { get; set; } = new TransferSettings();

        /// <summary>
        /// Gets or sets a value indicating whether the control is loaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the system settings environment.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Is ready to save.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Is ready to close.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Get the control description.</returns>
        public string GetControlDescription()
        {
            return "Inventory Movement Menu";
        }

        /// <summary>
        /// Save changes.
        /// </summary>
        /// <returns><see cref="bool"/> True.</returns>
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Cancel changes.
        /// </summary>
        /// <returns><see cref="bool"/> True.</returns>
        public Task<bool> CancelChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the return xml data.
        /// </summary>
        /// <returns><see cref="XDocument"/> Xml data.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// The on appearing event handler.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Num1 && this.cmdMoveProduct.IsEnabled)
                {
                    this.DisableControls();
                    var ucActivityMoveInventory = new ActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
                    await ucActivityMoveInventory.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdTransferFrom.IsEnabled)
                {
                    this.DisableControls();
                    var transferFrom = new ActivityHdr(SysCommon.ActSubTypes.TransferFrom);
                    await transferFrom.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdTransferTo.IsEnabled)
                {
                    this.DisableControls();
                    var transferTo = new ActivityHdr(SysCommon.ActSubTypes.TransferTo);
                    await transferTo.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdMoveSkid.IsEnabled)
                {
                    await this.MoveSkid();
                }
                else if (keycode == Keycode.Num5)
                {
                    await this.Replenishment();
                }
            });

            try
            {
                this.InventoryMovementMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.ucInventoryMovementMenu_GotFocus threw an error " + ex,
                    "OK"));
            }
        }

        /// <summary>
        /// On disappearing event handler.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// On back button pressed.
        /// </summary>
        /// <returns><see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                this.DisableControls();
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// Disable the menu controls.
        /// </summary>
        private void DisableControls()
        {
            this.cmdMoveProduct.IsEnabled = false;
            this.cmdMoveSkid.IsEnabled = false;
            this.cmdTransferFrom.IsEnabled = false;
            this.cmdTransferTo.IsEnabled = false;
            this.cmdReplenishment.IsEnabled = false;
            this.IsControlLoaded = false;
        }

        /// <summary>
        /// Inventory movement menu got focus event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void InventoryMovementMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdTransferFrom.IsEnabled = false;
                this.cmdTransferTo.IsEnabled = false;
                this.cmdMoveProduct.IsEnabled = false;
                this.cmdMoveSkid.IsEnabled = false;
                this.cmdReplenishment.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdTransferFrom.IsEnabled = true;
                        this.cmdTransferTo.IsEnabled = true;
                        this.cmdMoveProduct.IsEnabled = true;
                        this.cmdMoveSkid.IsEnabled = true;
                        this.cmdReplenishment.IsEnabled = true;
                    }
                    else
                    {
                        this.cmdTransferFrom.IsEnabled = this.SysEnv.SysSettings.IsTransMod;
                        this.cmdTransferTo.IsEnabled = this.SysEnv.SysSettings.IsTransMod;
                        this.cmdMoveProduct.IsEnabled = this.SysEnv.SysSettings.IsMovesModule;
                        this.cmdMoveSkid.IsEnabled = this.SysEnv.SysSettings.IsMoveSkidModule;
                        this.cmdReplenishment.IsEnabled = this.SysEnv.SysSettings.IsReplenishmentModule;
                    }
                }

                // Override all checks if module bypass enabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.cmdMoveProduct.IsEnabled = true;
                    this.cmdMoveSkid.IsEnabled = true;
                    this.cmdTransferFrom.IsEnabled = true;
                    this.cmdTransferTo.IsEnabled = true;
                    this.cmdReplenishment.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Moves menu item click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void cmdMoves_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            var ucActivityMoveInventory = new ActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
            await ucActivityMoveInventory.Show();
        }

        /// <summary>
        /// Transfer from menu item click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void cmdTransferFrom_Click(object sender, EventArgs e)
        {
            this.SetSaveTransSettings();
            this.DisableControls();

            var transferFrom = new ActivityHdr(SysCommon.ActSubTypes.TransferFrom);
            await transferFrom.Show();
        }

        /// <summary>
        /// Transfer to menu item click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void cmdTransferTo_Click(object sender, EventArgs e)
        {
            this.SetSaveTransSettings();
            this.DisableControls();

            var transferTo = new ActivityHdr(SysCommon.ActSubTypes.TransferTo);
            await transferTo.Show();
        }

        /// <summary>
        /// Move skid menu item click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void cmdMoveSkid_Click(object sender, EventArgs e)
        {
            await this.MoveSkid();
        }

        /// <summary>
        /// Opens the move skid screen and disables controls of this screen.
        /// </summary>
        private async Task MoveSkid()
        {
            this.DisableControls();

            var moveSkid = new ActivityMoveSkid(SysCommon.ActSubTypes.MoveSkid);
            await moveSkid.Show();
        }

        /// <summary>
        /// Set save transfer settings.
        /// </summary>
        private void SetSaveTransSettings()
        {
            var tempTransSettings = this.TransSettings;
            _ = this.TransSettings.SaveData(ref tempTransSettings);
            this.TransSettings = tempTransSettings;
        }

        /// <summary>
        /// Replenishment menu item click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void cmdReplenishment_Click(object sender, EventArgs e)
        {
            await this.Replenishment();
        }

        /// <summary>
        /// Opens the replenishment screen and disables controls of this screen.
        /// </summary>
        private async Task Replenishment()
        {
            this.DisableControls();
            var replenishment = new ActivityHdrReplenishment(SysCommon.ActSubTypes.InventoryReplenishment);
            await replenishment.Show();
        }
    }
}