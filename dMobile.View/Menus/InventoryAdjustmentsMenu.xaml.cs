// <copyright file="InventoryAdjustmentsMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.Activity;
    using dMobile.View.CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryAdjustmentsMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryAdjustmentsMenu"/> class.
        /// </summary>
        public InventoryAdjustmentsMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdRework.Text = "Rework";
            this.cmdProduction.Text = "Production";
            this.cmdInventory.Text = "Inventory";
            this.cmdRequisition.Text = "Material Requisition";
            this.cmdDisposal.Text = "Disposal";
            this.cmdConvertProduct.Text = "Convert Label";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.AdjSettings = new AdjustmentSettings();
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

                if (keycode == Keycode.F1)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num1 && this.cmdInventory.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.Inventory);
                }
                else if (keycode == Keycode.Num2 && this.cmdProduction.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.Production);
                }
                else if (keycode == Keycode.Num3 && this.cmdRework.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.Rework);
                }
                else if (keycode == Keycode.Num4 && this.cmdRequisition.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.Requisition);
                }
                else if (keycode == Keycode.Num5 && this.cmdDisposal.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.Disposal);
                }
                else if (keycode == Keycode.Num6 && this.cmdConvertProduct.IsEnabled)
                {
                    await this.SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
                }
            });

            try
            {
                this.ucInventoryAdjustmentsMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucInventoryAdjustmentsMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
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

        private AdjustmentSettings AdjSettings { get; set; }

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
            var IsReadyToSave = true;

            return IsReadyToSave;
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
            return "Adjustments Menu";
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
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucInventoryAdjustmentsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.AdjSettings = this.AdjSettings.LoadData();

                this.cmdInventory.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdInventory.IsEnabled = true;
                    }
                }

                this.cmdProduction.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdProduction.IsEnabled = true;
                    }
                }

                this.cmdRework.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdRework.IsEnabled = true;
                    }
                }

                this.cmdRequisition.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdRequisition.IsEnabled = true;
                    }
                }

                this.cmdDisposal.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdDisposal.IsEnabled = true;
                    }
                }

                this.cmdConvertProduct.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsInventoryAdjustmentModule
                        || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdConvertProduct.IsEnabled = true;
                    }
                }

                // Override all checks if module bypass enabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.cmdInventory.IsEnabled = true;
                    this.cmdProduction.IsEnabled = true;
                    this.cmdRework.IsEnabled = true;
                    this.cmdRequisition.IsEnabled = true;
                    this.cmdDisposal.IsEnabled = true;
                    this.cmdConvertProduct.IsEnabled = true;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdInventory_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.Inventory);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProduction_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.Production);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRework_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.Rework);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDisposal_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.Disposal);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRequisition_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.Requisition);
        }

        /// <summary>
        /// Click handler for Convert Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdConvertProduct_Click(object sender, EventArgs e)
        {
            await this.SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
        }

        /// <summary>
        /// Displays the Convert Product form.
        /// </summary>
        private async Task ShowConvertProductForm()
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var convertProduct = new ActivityConvertProduct();
            await convertProduct.Show();
        }

        /// <summary>
        /// Sets the adjustment type and saves adjustment settings.
        /// </summary>
        /// <param name="adjType"></param>
        private async Task SetAdjustmentType(SysCommon.AdjTypes adjType)
        {
            if (adjType == SysCommon.AdjTypes.ConvertProduct)
            {
                // Placed here to get the default case of keyhandler as well.
                await this.ShowConvertProductForm();
            }
            else
            {
                this.AdjSettings.AdjustmentType = adjType;
                await this.SetSaveAdjSettings();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdDisposal.IsEnabled = false;
            this.cmdInventory.IsEnabled = false;
            this.cmdProduction.IsEnabled = false;
            this.cmdRework.IsEnabled = false;
            this.cmdDisposal.IsEnabled = false;
            this.cmdRequisition.IsEnabled = false;
            this.cmdConvertProduct.IsEnabled = false;
        }

        /// <summary>
        ///
        /// </summary>
        private async Task SetSaveAdjSettings()
        {
            this.DisableControls();
            this.AdjSettings.IsApplyAdjustmentType = true;
            this.AdjSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
            var tempAdjSettings = this.AdjSettings;
            _ = this.AdjSettings.SaveData(ref tempAdjSettings);
            this.AdjSettings = tempAdjSettings;
            this.IsControlLoaded = false;

            var ucActivityHdr = new ActivityHdr(SysCommon.ActSubTypes.Adjustment);
            await ucActivityHdr.Show();
        }
    }
}