// <copyright file="MainMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        public MainMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }

            this.cmdPurchaseOrders.Text = "1) " + Resource.ReceivingAndPutAway;
            this.cmdSalesOrders.Text = "2) " + Resource.SalesOrders;
            this.cmdInventory.Text = "3) " + Resource.Inventory;
            this.cmdProduction.Text = "4) " + Resource.Production;
            this.cmdOptions.Text = "5) " + Resource.Options;
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

                if (keycode == Keycode.Num1 && this.cmdPurchaseOrders.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var purchaseOrders = new PurchaseOrdersMenu();
                    await purchaseOrders.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdSalesOrders.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var salesOrders = new SalesOrdersMenu();
                    await salesOrders.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdInventory.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var inventoryMenu = new InventoryMenu();
                    await inventoryMenu.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdProduction.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var production = new ProductionTicketMenu();
                    await production.Show();
                }
                else if (keycode == Keycode.Num5 && this.cmdOptions.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var optionsMenu = new OptionsMenu();
                    await optionsMenu.Show();
                }
            });

            try
            {
                this.ucMainMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucMainMenu_GotFocus threw an error " + ex, "OK"));
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

        // protected override bool OnBackButtonPressed()
        // {
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        base.OnBackButtonPressed();

        // this.DisableControls();
        //        this.IsControlLoaded = false;

        // });

        // return true;
        // }
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
        /// <inheritdoc/>
        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc/>
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
            return Resource.MainMenu;
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
        public async Task<XDocument> GetXml()
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
            this.cmdInventory.IsEnabled = false;
            this.cmdOptions.IsEnabled = false;
            this.cmdProduction.IsEnabled = false;
            this.cmdPurchaseOrders.IsEnabled = false;
            this.cmdSalesOrders.IsEnabled = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void EnableControls()
        {
            this.cmdInventory.IsEnabled = true;
            this.cmdOptions.IsEnabled = true;
            this.cmdProduction.IsEnabled = true;
            this.cmdPurchaseOrders.IsEnabled = true;
            this.cmdSalesOrders.IsEnabled = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucMainMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdPurchaseOrders.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsReceivingModule || this.SysEnv.SysSettings.IsPutAwayModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdPurchaseOrders.IsEnabled = true;
                    }
                }

                this.cmdSalesOrders.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPickMod || this.SysEnv.SysSettings.IsRetMod
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdSalesOrders.IsEnabled = true;
                    }
                }

                this.cmdInventory.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsTransMod || this.SysEnv.SysSettings.IsConvertUnitsModule
                        || this.SysEnv.SysSettings.IsMovesModule || this.SysEnv.SysSettings.IsCycleModule
                        || this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.SysSettings.IsReplenishmentModule
                        || this.SysEnv.SysSettings.IsAddToSkidModule || this.SysEnv.SysSettings.IsBreakSkidModule
                        || this.SysEnv.SysSettings.IsCreateSkidModule || this.SysEnv.SysSettings.IsRemoveFromSkidModule
                        || this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdInventory.IsEnabled = true;
                    }
                }

                this.cmdProduction.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsTicketMaterialModule || this.SysEnv.SysSettings.IsTicketByProductModule
                        || this.SysEnv.SysSettings.IsTicketFinishedGoodModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdProduction.IsEnabled = true;
                    }
                }

                // Override all checks if module bypass enabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.cmdPurchaseOrders.IsEnabled = true;
                    this.cmdSalesOrders.IsEnabled = true;
                    this.cmdInventory.IsEnabled = true;
                    this.cmdProduction.IsEnabled = true;
                }

                this.cmdOptions.IsEnabled = true;

                // attempt to give focus to first control
                _ = this.cmdPurchaseOrders.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdPurchaseOrders_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var purchaseOrders = new PurchaseOrdersMenu();
            await purchaseOrders.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSalesOrders_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var salesOrdersMenu = new SalesOrdersMenu();
            await salesOrdersMenu.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdInventory_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var inventoryMenu = new InventoryMenu();
            await inventoryMenu.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProduction_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var production = new ProductionTicketMenu();
            await production.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdOptions_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var optionsMenu = new OptionsMenu();
            await optionsMenu.Show();
        }
    }
}
