// <copyright file="InventoryCountsMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class InventoryCountsMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryCountsMenu"/> class.
        /// </summary>
        public InventoryCountsMenu()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdPhysicalCount.Text = "1) Physical Count";
            this.cmdCycleCount.Text = "2) Cycle Count";

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

                if (keycode == Keycode.Enter && this.cmdPhysicalCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var physicalCountHdr = new ActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
                    await physicalCountHdr.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdPhysicalCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var physicalCountHdr = new ActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
                    await physicalCountHdr.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdCycleCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    var cycleCountHdr = new ActivityHdr(SysCommon.ActSubTypes.CycleCounts);
                    await cycleCountHdr.Show();
                }
            });

            try
            {
                this.ucInventoryCountsMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucInventoryCountsMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        // public UserControl GetCallingControl()
        // {
        //    return this.CallingControl;
        // }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Inventory Counts Menu";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPhysicalCount_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var physicalCountActivityHdr = new ActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
            await physicalCountActivityHdr.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdCycleCount_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            var cycleCountActivityHdr = new ActivityHdr(SysCommon.ActSubTypes.CycleCounts);
            await cycleCountActivityHdr.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucInventoryCountsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;

                this.cmdPhysicalCount.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdPhysicalCount.IsEnabled = true;
                    }
                }

                this.cmdCycleCount.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginId
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsCycleModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginId)
                    {
                        this.cmdCycleCount.IsEnabled = true;
                    }
                }

                // Override all checks if module bypass enabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.cmdPhysicalCount.IsEnabled = true;
                    this.cmdCycleCount.IsEnabled = true;
                }

                _ = this.cmdPhysicalCount.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdCycleCount.IsEnabled = false;
            this.cmdPhysicalCount.IsEnabled = false;
        }
    }
}