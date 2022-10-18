// <copyright file="SalesOrdersMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class SalesOrdersMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesOrdersMenu"/> class.
        /// </summary>
        public SalesOrdersMenu()
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

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Num1 && this.cmdPicking.IsEnabled)
                {
                    this.DisableControls();

                    var picking = new ActivityHdr(SysCommon.ActSubTypes.Picking);
                    await picking.Show();

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num2 && this.cmdReturns.IsEnabled)
                {
                    this.DisableControls();

                    var returns = new ActivityHdr(SysCommon.ActSubTypes.Returns);
                    await returns.Show();

                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num3 && this.cmdReleaseByOrder.IsEnabled)
                {
                    this.DisableControls();

                    var releaseByOrder = new ActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);
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
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucSalesOrdersMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());

            base.OnDisappearing();
        }

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
                var pickingSingle = new ActivityHdr(SysCommon.ActSubTypes.PickingSingle);
                await pickingSingle.Show();
            }
            else
            {
                var picking = new ActivityHdr(SysCommon.ActSubTypes.Picking);
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

            var returns = new ActivityHdr(SysCommon.ActSubTypes.Returns);
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

                _ = this.cmdPicking.Focus();
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
                var releaseByOrder = new ActivityHdrReleaseByOrder(SysCommon.ActSubTypes.PickingSingle);
                await releaseByOrder.Show();
            }
            else
            {
                var releaseByOrder = new ActivityHdrReleaseByOrder(SysCommon.ActSubTypes.Picking);
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
    }
}