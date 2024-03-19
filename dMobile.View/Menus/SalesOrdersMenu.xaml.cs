// <copyright file="SalesOrdersMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    using CustomPages;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    ///     Sales orders menu.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesOrdersMenu : DMobileContentPage, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SalesOrdersMenu" /> class.
        /// </summary>
        public SalesOrdersMenu()
        {
            this.InitializeComponent();

            this.Title = GetControlDescription();
            this.CmdPicking.Text = "1) " + "Picking";
            this.CmdReturns.Text = "2) " + "Returns";
            this.CmdReleaseByOrder.Text = "3) " + "Release By Order #";
            this.CmdReleaseBySection.Text = "4) " + "Release By Section";
            this.SysEnv = SysEnvironment.GetInstance();
        }

        /// <summary>
        ///     Gets or sets the xml data filter.
        /// </summary>
        private XDocument XmlDataFilter { get; set; }

        /// <summary>
        ///     Gets or sets the picking location map filter.
        /// </summary>
        private SLocMap PickingLocMapFilter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the control is loading.
        /// </summary>
        private bool IsControlLoaded { get; set; }

        /// <summary>
        ///     Gets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task complete.</returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Is ready to save.
        /// </summary>
        /// <returns><see cref="Task" /> True if ready to save else false.</returns>
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     Is ready to close.
        /// </summary>
        /// <returns><see cref="Task"/> True if ready to close else false.</returns>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     Save changes.
        /// </summary>
        /// <returns><see cref="Task"/> True if save successful else false.</returns>
        public Task<bool> SaveChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     Cancel changes.
        /// </summary>
        /// <returns><see cref="Task"/> True if cancel successful else false.</returns>
        public Task<bool> CancelChanges()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     Get the xml data.
        /// </summary>
        /// <returns><see cref="Task"/> XML data.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandlers);

            try
            {
                this.SalesOrdersMenu_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.SalesOrdersMenu_GotFocus threw an error " + ex,
                    "OK"));
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async void KeyUpHandlers(string keyPressed)
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                this.DisableControls();
                switch (keycode)
                {
                    case Keycode.Num1 when this.CmdPicking.IsEnabled:
                    {
                        // Note: This filter request will be asked for upon a page refresh.
                        // IsShowActivityFilterSelection flag stored in singleton so it is not reset on a page refresh.
                        if (this.SysEnv.SysSettings.IsPrePickCrit &&
                            this.SysEnv.SysSettings.IsShowActivityFilterSelection)
                        {
                            this.XmlDataFilter = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + "<whs_id>0</whs_id>"
                                + "<sect_refno>0</sect_refno>"
                                + "</params>");

                            this.PickingLocMapFilter = new SLocMap();
                            if (await this.HandlePrePickCriteria())
                            {
                                var picking = new ActivityHdr(
                                    SysCommon.ActSubTypes.Picking,
                                    this.XmlDataFilter,
                                    this.PickingLocMapFilter);
                                await picking.Show();
                            }
                        }
                        else
                        {
                            var picking = new ActivityHdr(SysCommon.ActSubTypes.Picking);
                            await picking.Show();
                        }

                        break;
                    }

                    case Keycode.Num2 when this.CmdReturns.IsEnabled:
                    {
                        var returns = new ActivityHdr(SysCommon.ActSubTypes.Returns);
                        await returns.Show();
                        break;
                    }

                    case Keycode.Num3 when this.CmdReleaseByOrder.IsEnabled:
                    {
                        var releaseByOrder = new ActivityHdrReleaseByOrder(SysCommon.ActSubTypes.ReleaseByOrder);
                        await releaseByOrder.Show();
                        break;
                    }
                }

                this.IsControlLoaded = false;
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        private static string GetControlDescription()
        {
            return "Sales Orders Menu";
        }

        /// <summary>
        ///     Disable the controls.
        /// </summary>
        private void DisableControls()
        {
            this.CmdPicking.IsEnabled = false;
            this.CmdReleaseByOrder.IsEnabled = false;
            this.CmdReleaseBySection.IsEnabled = false;
            this.CmdReturns.IsEnabled = false;
        }

        /// <summary>
        ///     Handle Pre picking criteria.
        /// </summary>
        /// <returns><see cref="bool" /> True if a refresh should be performed.</returns>
        private async Task<bool> HandlePrePickCriteria()
        {
            var continueToHeader = true;

            // This code should be moved into an picking header class inherited from ucActivityHdr.
            var sectFkRefno = 0;

            // Scanning in the picking selection criteria form is possible, which means the
            // the last parse bar code data would be overwritten.
            // However because the bar code parse object at this point does not contain any
            // valuable data we do not need to preserve it before calling the picking
            // selection form.
            var formPickSelect = new FormDialogContainer(
                EDialogTypes.ActivityPickingSelectCriteria,
                SysCommon.ActSubTypes.Picking);

            if (await Device.InvokeOnMainThreadAsync(async () =>
                    await formPickSelect.DisplayControl.ShowDialog() == DialogResult.OK))
            {
                var xmlResult = await formPickSelect.GetXml();
                var locMapRefno = xmlResult.Root?.Element("LocationMapRefno")?.Value;
                if (locMapRefno != null)
                {
                    var tmpLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(int.Parse(locMapRefno));
                    var locMap = LocationMap.GetInstance();
                    locMap.CurrLocMap = tmpLocMap;
                    this.PickingLocMapFilter = tmpLocMap;
                    sectFkRefno = tmpLocMap.SectFkRefno;

                    this.XmlDataFilter = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<params>"
                        + "<whs_id>" + tmpLocMap.WhsFkRefno + "</whs_id>"
                        + "<sect_refno>" + tmpLocMap.SectFkRefno + "</sect_refno>"
                        + "</params>");
                }
            }

            // See WO#12906 do not load any orders if no section was selected, rather than
            // load all the orders.
            if (this.SysEnv.SysSettings.IsPickingModuleDisallowLoadingAllOrders && sectFkRefno == 0)
            {
                continueToHeader = false;
            }

            return continueToHeader;
        }

        /// <summary>
        ///     Button picking click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private async void CmdPicking_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            if (this.SysEnv.SysSettings.IsPickSingleProd)
            {
                var pickingSingle = new ActivityHdr(SysCommon.ActSubTypes.PickingSingle);
                await pickingSingle.Show();
            }
            else
            {
                // Note: This filter request will be asked for upon a page refresh.
                // IsShowActivityFilterSelection flag stored in singleton so it is not reset on a page refresh.
                if (this.SysEnv.SysSettings.IsPrePickCrit && this.SysEnv.SysSettings.IsShowActivityFilterSelection)
                {
                    this.XmlDataFilter = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<params>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</params>");

                    this.PickingLocMapFilter = new SLocMap();
                    if (await this.HandlePrePickCriteria())
                    {
                        var picking = new ActivityHdr(
                            SysCommon.ActSubTypes.Picking,
                            this.XmlDataFilter,
                            this.PickingLocMapFilter);
                        await picking.Show();
                    }
                }
                else
                {
                    var picking = new ActivityHdr(SysCommon.ActSubTypes.Picking);
                    await picking.Show();
                }
            }

            this.IsControlLoaded = false;
        }

        /// <summary>
        ///     Button returns click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private async void CmdReturns_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            var returns = new ActivityHdr(SysCommon.ActSubTypes.Returns);
            await returns.Show();

            this.IsControlLoaded = false;
        }

        /// <summary>
        ///     Sales orders menu item got focus event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private void SalesOrdersMenu_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.CmdPicking.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPickMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.CmdPicking.IsEnabled = true;
                    }
                }

                this.CmdReturns.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsRetMod || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.CmdReturns.IsEnabled = true;
                    }
                }

                this.CmdReleaseByOrder.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsRelByOrder ||
                        this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.CmdReleaseByOrder.IsEnabled = true;
                    }
                }

                this.CmdReleaseBySection.IsEnabled = false;

                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsRelBySect ||
                        this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.CmdReleaseBySection.IsEnabled = false; // Disabled intentionally
                    }
                }

                // Override all checks if module bypass enabled.
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.IsEnableAllModuleBypassLicense
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    this.CmdPicking.IsEnabled = true;
                    this.CmdReturns.IsEnabled = true;
                    this.CmdReleaseByOrder.IsEnabled = true;
                    this.CmdReleaseBySection.IsEnabled = true;
                }

                _ = this.CmdPicking.Focus();
            }
        }

        /// <summary>
        ///     Button release by order click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private async void CmdReleaseByOrder_Click(object sender, EventArgs e)
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
        ///     Button release by section click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event argument data.</param>
        private void CmdReleaseBySection_Click(object sender, EventArgs e)
        {
            this.DisableControls();
            this.IsControlLoaded = false;
        }
    }
}