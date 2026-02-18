// <copyright file="PurchaseOrdersMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    using Bedrock;
    using Resources.Enums;
    using Activity;
    using CustomPages;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PurchaseOrdersMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrdersMenu"/> class.
        /// </summary>
        public PurchaseOrdersMenu()
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

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter && this.cmdWorkSheet.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    var worksheet = new ActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdReceiving.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    var receiving = new ActivityHdr(SysCommon.ActSubTypes.Receiving);
                    await receiving.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdPutAway.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    var putAway = new ActivitySkidPutAway();
                    await putAway.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdWorkSheet.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                    var worksheet = new ActivityHdr(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    await worksheet.Show();
                }
            });

            try
            {
                this.ucPurchaseOrdersMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucPurchaseOrdersMenu_GotFocus threw an error " + ex, "OK"));
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

        private bool IsControlLoaded { get; set; }

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
            return "Purchase Orders Menu";
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
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdReceiving.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsReceivingModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdReceiving.IsEnabled = true;
                    }
                }

                this.cmdPutAway.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPutAwayModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        if (this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            this.cmdPutAway.IsEnabled = true;
                        }
                    }
                }

                this.cmdWorkSheet.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode) && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPOWorkSheetModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
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

                _ = this.cmdReceiving.Focus();
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
            var receiving = new ActivityHdr(SysCommon.ActSubTypes.Receiving);
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
            var putAway = new ActivitySkidPutAway();
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
            var worksheet = new ActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
            await worksheet.Show();
        }
    }
}