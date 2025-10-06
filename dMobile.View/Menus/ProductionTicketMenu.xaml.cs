// <copyright file="ProductionTicketMenu.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Menus
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Activity;
    using Android.Views;
    using Bedrock;
    using Bedrock.Settings;
    using CustomPages;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ProductionTicketMenu"/>.
    /// </summary>
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
            this.cmdFinishedGoods.Text = "1) Ticket Finished Goods";
            this.cmdIngredientMaterial.Text = "2) Ticket Raw Materials";
            this.cmdByProduct.Text = "3) Ticket Additional Products";
            this.cmdDebox.Text = "4) Debox";
            this.cmdCutFinished.Text = "5) Cut Finished Goods";
            this.cmdCutMaterial.Text = "6) Cut Raw Materials";
            this.cmdYieldFinished.Text = "7) Yield Finished Goods";
            this.cmdYieldMaterial.Text = "8) Yield Raw Materials";
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is loaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns>Task operation.</returns>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns>Task operation.</returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Is ready to save event.
        /// </summary>
        /// <returns>True if ready to save, else false.</returns>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Is ready to close event.
        /// </summary>
        /// <returns>True if ready to close, else false.</returns>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns>Control description.</returns>
        public string GetControlDescription()
        {
            return "Production Menu";
        }

        /// <summary>
        /// Save the changes.
        /// </summary>
        /// <returns>True if save was successful, else false.</returns>
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Cancel the changes.
        /// </summary>
        /// <returns>True if the changes were cancelled, else false.</returns>
        public Task<bool> CancelChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Get the XML control return results.
        /// </summary>
        /// <returns>XML result return data.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// On appearing event.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<string>(this, nameof(EventTypes.OnKeyUp), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                this.DisableControls();
                this.UnloadControl();
                if (keycode == Keycode.Num1 && this.cmdFinishedGoods.IsEnabled)
                {
                    var ticketFinished = new ActivityHdr(SysCommon.ActSubTypes.TicketFinished);
                    await ticketFinished.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdIngredientMaterial.IsEnabled)
                {
                    var ticketMaterial = new ActivityHdr(SysCommon.ActSubTypes.TicketMaterial);
                    await ticketMaterial.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdByProduct.IsEnabled)
                {
                    var ticketAddProduct = new ActivityHdr(SysCommon.ActSubTypes.TicketAddProduct);
                    await ticketAddProduct.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdDebox.IsEnabled)
                {
                    var adjustment = new ActivityHdr(SysCommon.ActSubTypes.Adjustment);
                    await adjustment.Show();
                }
                else if (keycode == Keycode.Num5 && this.cmdCutFinished.IsEnabled)
                {
                    var cutFinished = new ActivityHdr(SysCommon.ActSubTypes.CutSheetFinished);
                    await cutFinished.Show();
                }
                else if (keycode == Keycode.Num6 && this.cmdCutMaterial.IsEnabled)
                {
                    var cutMaterial = new ActivityHdr(SysCommon.ActSubTypes.CutSheetMaterial);
                    await cutMaterial.Show();
                }
                else if (keycode == Keycode.Num7 && this.cmdYieldFinished.IsEnabled)
                {
                    var yieldFinished = new ActivityHdr(SysCommon.ActSubTypes.YieldSheetFinished);
                    await yieldFinished.Show();
                }
                else if (keycode == Keycode.Num8 && this.cmdYieldMaterial.IsEnabled)
                {
                    var yieldMaterial = new ActivityHdr(SysCommon.ActSubTypes.YieldSheetMaterial);
                    await yieldMaterial.Show();
                }
            });

            try
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
                        if (this.SysEnv.SysSettings.IsTicketFinishedGoodModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdFinishedGoods.IsEnabled = true;
                        }
                    }

                    this.cmdIngredientMaterial.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsTicketMaterialModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdIngredientMaterial.IsEnabled = true;
                        }
                    }

                    this.cmdByProduct.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsTicketByProductModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdByProduct.IsEnabled = true;
                        }
                    }

                    this.cmdCutFinished.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsCutSheetFinishedGoodModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdCutFinished.IsEnabled = true;
                        }
                    }

                    this.cmdCutMaterial.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsCutSheetRawMaterialModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdCutMaterial.IsEnabled = true;
                        }
                    }

                    this.cmdYieldFinished.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsYieldSheetFinishedGoodModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdYieldFinished.IsEnabled = true;
                        }
                    }

                    this.cmdYieldMaterial.IsEnabled = false;
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        if (this.SysEnv.SysSettings.IsYieldSheetRawMaterialModule
                            || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                        {
                            this.cmdYieldMaterial.IsEnabled = true;
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
                        this.cmdDebox.IsEnabled = true;
                        this.cmdCutFinished.IsEnabled = true;
                        this.cmdCutMaterial.IsEnabled = true;
                        this.cmdYieldFinished.IsEnabled = true;
                        this.cmdYieldMaterial.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(
                        "Error",
                        "this.ucProductionTicketMenu_GotFocus threw an error " + ex,
                        "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async void CmdFinishedGoods_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketFinished = new ActivityHdr(SysCommon.ActSubTypes.TicketFinished);
            await ticketFinished.Show();
        }

        private async void CmdIngredientMaterial_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketMaterial = new ActivityHdr(SysCommon.ActSubTypes.TicketMaterial);
            await ticketMaterial.Show();
        }

        private async void CmdByProduct_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var ticketAddProduct = new ActivityHdr(SysCommon.ActSubTypes.TicketAddProduct);
            await ticketAddProduct.Show();
        }

        private void DisableControls()
        {
            this.cmdByProduct.IsEnabled = false;
            this.cmdFinishedGoods.IsEnabled = false;
            this.cmdIngredientMaterial.IsEnabled = false;
            this.cmdDebox.IsEnabled = false;
            this.cmdCutFinished.IsEnabled = false;
            this.cmdCutMaterial.IsEnabled = false;
            this.cmdYieldFinished.IsEnabled = false;
            this.cmdYieldMaterial.IsEnabled = false;
        }

        private void UnloadControl()
        {
            this.IsControlLoaded = false;
        }

        private void SetDeboxAdjustmentSettings()
        {
            var adjustmentSettings = new AdjustmentSettings();
            adjustmentSettings.AdjustmentType = SysCommon.AdjTypes.Debox;
            adjustmentSettings.IsApplyAdjustmentType = true;
            adjustmentSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(SysCommon.AdjTypes.Debox);
            _ = adjustmentSettings.SaveData(ref adjustmentSettings);
        }

        private async void CmdDebox_Click(object sender, EventArgs e)
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

        private async void CmdCutFinished_Clicked(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var form = new ActivityHdr(SysCommon.ActSubTypes.CutSheetFinished);
            await form.Show();
        }

        private async void CmdCutMaterial_Clicked(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var form = new ActivityHdr(SysCommon.ActSubTypes.CutSheetMaterial);
            await form.Show();
        }

        private async void CmdYieldFinished_Clicked(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var form = new ActivityHdr(SysCommon.ActSubTypes.YieldSheetFinished);
            await form.Show();
        }

        private async void CmdYieldMaterial_Clicked(object sender, EventArgs e)
        {
            this.DisableControls();
            this.UnloadControl();

            var form = new ActivityHdr(SysCommon.ActSubTypes.YieldSheetMaterial);
            await form.Show();
        }
    }
}
