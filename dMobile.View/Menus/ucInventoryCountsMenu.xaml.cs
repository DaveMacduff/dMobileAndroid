using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.Activity;
using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryCountsMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;

        #endregion

        #region Constructors

        public ucInventoryCountsMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdPhysicalCount.Text = "1) Physical Count";
            this.cmdCycleCount.Text = "2) Cycle Count";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }
        }

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter && this.cmdPhysicalCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityHdr physicalCountHdr = new ucActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
                    await physicalCountHdr.Show();
                }
                else if (keycode == Keycode.Num1 && this.cmdPhysicalCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityHdr physicalCountHdr = new ucActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
                    await physicalCountHdr.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdCycleCount.IsEnabled)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;

                    ucActivityHdr cycleCountHdr = new ucActivityHdr(SysCommon.ActSubTypes.CycleCounts);
                    await cycleCountHdr.Show();
                }
            });

            try
            {
                this.ucInventoryCountsMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucInventoryCountsMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                this.DisableControls();
                this.IsControlLoaded = false;
                await this.Close();
            });

            return true;
        }

        #region Properties

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        #endregion

        #region Public Members

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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Inventory Counts Menu";
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
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        #endregion

        #region Private Members

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

            ucActivityHdr physicalCountActivityHdr = new ucActivityHdr(SysCommon.ActSubTypes.PhysicalCounts);
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

            ucActivityHdr cycleCountActivityHdr = new ucActivityHdr(SysCommon.ActSubTypes.CycleCounts);
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
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsPhysicalModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                    {
                        this.cmdPhysicalCount.IsEnabled = true;
                    }
                }

                this.cmdCycleCount.IsEnabled = false;
                if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                    && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                    && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                {
                    if (this.SysEnv.SysSettings.IsCycleModule || this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
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

                this.cmdPhysicalCount.Focus();
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

        #endregion
    }
}