using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Activity;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryAdjustmentsMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private AdjustmentSettings adjSettings;
        private bool isControlLoaded = false;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucInventoryAdjustmentsMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
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

        #endregion

        #region OnAppearing/OnDisappearing

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

                if (keycode == Keycode.F1)
                {
                    this.DisableControls();
                    this.IsControlLoaded = false;
                }
                else if (keycode == Keycode.Num1 && this.cmdInventory.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Inventory);
                }
                else if (keycode == Keycode.Num2 && this.cmdProduction.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Production);
                }
                else if (keycode == Keycode.Num3 && this.cmdRework.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Rework);
                }
                else if (keycode == Keycode.Num4 && this.cmdRequisition.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Requisition);
                }
                else if (keycode == Keycode.Num5 && this.cmdDisposal.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.Disposal);
                }
                else if (keycode == Keycode.Num6 && this.cmdConvertProduct.IsEnabled)
                {
                    await SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
                }
            });

            try
            {
                this.ucInventoryAdjustmentsMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucInventoryAdjustmentsMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
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

        #endregion

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

        private AdjustmentSettings AdjSettings
        {
            get
            {
                return this.adjSettings;
            }

            set
            {
                this.adjSettings = value;
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

        #region IAppArea Members

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
            bool IsReadyToSave = true;

            return IsReadyToSave;
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
            // TODO: Resource strings
            return "Adjustments Menu";
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

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucInventoryAdjustmentsMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
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
                    }
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
            await SetAdjustmentType(SysCommon.AdjTypes.Inventory);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProduction_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Production);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRework_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Rework);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDisposal_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Disposal);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdRequisition_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.Requisition);
        }

        /// <summary>
        /// Click handler for Convert Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdConvertProduct_Click(object sender, EventArgs e)
        {
            await SetAdjustmentType(SysCommon.AdjTypes.ConvertProduct);
        }

        /// <summary>
        /// Displays the Convert Product form.
        /// </summary>
        private async Task ShowConvertProductForm()
        {
            this.DisableControls();
            this.IsControlLoaded = false;

            ucActivityConvertProduct convertProduct = new ucActivityConvertProduct();
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
                await ShowConvertProductForm();
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
            AdjustmentSettings tempAdjSettings = this.AdjSettings;
            this.AdjSettings.SaveData(ref tempAdjSettings);
            this.AdjSettings = tempAdjSettings;
            this.IsControlLoaded = false;

            ucActivityHdr ucActivityHdr = new ucActivityHdr(SysCommon.ActSubTypes.Adjustment);
            await ucActivityHdr.Show();
        }

        #endregion
    }
}