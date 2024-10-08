// <copyright file="ProductionTicketMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class ProductionTicketMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionTicketMenu"/> class.
        /// </summary>
        public ProductionTicketMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdFinishedGoods.Text = "1) Finished Goods";
            this.cmdIngredientMaterial.Text = "2) Ingredient Materials";
            this.cmdByProduct.Text = "3) Additional Products";
            this.cmdDebox.Text = "4) Debox";
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

                if (keycode == Keycode.Num1 && this.cmdFinishedGoods.IsEnabled)
                {
                    this.DisableControls();
                    this.UnloadControl();

                    var ticketFinished = new ActivityHdr(SysCommon.ActSubTypes.TicketFinished);
                    await ticketFinished.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdIngredientMaterial.IsEnabled)
                {
                    this.DisableControls();
                    this.UnloadControl();

                    var ticketMaterial = new ActivityHdr(SysCommon.ActSubTypes.TicketMaterial);
                    await ticketMaterial.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdByProduct.IsEnabled)
                {
                    this.DisableControls();
                    this.UnloadControl();

                    var ticketAddProduct = new ActivityHdr(SysCommon.ActSubTypes.TicketAddProduct);
                    await ticketAddProduct.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdDebox.IsEnabled)
                {
                    this.DisableControls();
                    this.UnloadControl();

                    var adjustment = new ActivityHdr(SysCommon.ActSubTypes.Adjustment);
                    await adjustment.Show();
                }
            });

            try
            {
                this.ucProductionTicketMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucProductionTicketMenu_GotFocus threw an error " + ex, "OK"));
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

        private bool IsControlLoaded { get; set; } = false;

        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

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
            return "Production Ticket Menu";
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
        private void ucProductionTicketMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdDebox.IsEnabled = true;

                this.cmdFinishedGoods.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsTicketFinishedGoodModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdFinishedGoods.IsEnabled = true;
                    }
                }

                this.cmdIngredientMaterial.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsTicketMaterialModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdIngredientMaterial.IsEnabled = true;
                    }
                }

                this.cmdByProduct.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsTicketByProductModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdByProduct.IsEnabled = true;
                    }
                }

                // Override all checks if module bypass Isenabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.cmdFinishedGoods.IsEnabled = true;
                    this.cmdIngredientMaterial.IsEnabled = true;
                    this.cmdByProduct.IsEnabled = true;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdFinishedGoods_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketFinished = new ActivityHdr(SysCommon.ActSubTypes.TicketFinished);
            await ticketFinished.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdIngredientMaterial_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketMaterial = new ActivityHdr(SysCommon.ActSubTypes.TicketMaterial);
            await ticketMaterial.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdByProduct_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketAddProduct = new ActivityHdr(SysCommon.ActSubTypes.TicketAddProduct);
            await ticketAddProduct.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdByProduct.IsEnabled = false;
            this.cmdFinishedGoods.IsEnabled = false;
            this.cmdIngredientMaterial.IsEnabled = false;
            this.cmdDebox.IsEnabled = false;
        }

        /// <summary>
        /// Fires close event and sets IsControlLoaded to false.
        /// </summary>
        private void UnloadControl()
        {
            this.IsControlLoaded = false;
        }

        /// <summary>
        /// Sets the adjustment settings for deboxing.
        /// </summary>
        private void SetDeboxAdjustmentSettings()
        {
            var adjustmentSettings = new AdjustmentSettings();
            adjustmentSettings.AdjustmentType = SysCommon.AdjTypes.Debox;
            adjustmentSettings.IsApplyAdjustmentType = true;
            adjustmentSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(SysCommon.AdjTypes.Debox);
            _ = adjustmentSettings.SaveData(ref adjustmentSettings);
        }

        private async void cmdDebox_Click(object sender, EventArgs e)
        {
            // <WO ID="14748" comment="Adding Debox adjustment to Production Ticket menu." />
            // Set adjustment form settings.
            this.SetDeboxAdjustmentSettings();

            // Show an adjustment form
            this.DisableControls();
            this.UnloadControl();

            var adjustment = new ActivityHdr(SysCommon.ActSubTypes.Adjustment);
            await adjustment.Show();
        }
    }
}