using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Activity;

namespace dMobile.View.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucInventoryMovementMenu : DMobileContentPage, IAppArea
    {
        #region Fields

        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;
        private TransferSettings transSettings;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucInventoryMovementMenu()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdMoveProduct.Text = "1) Move Product";
            this.cmdTransferFrom.Text = "2) Transfer From";
            this.cmdTransferTo.Text = "3) Transfer To";
            this.cmdMoveSkid.Text = "4) Move Skid";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.TransSettings = new TransferSettings();
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

                if (keycode == Keycode.Num1 && this.cmdMoveProduct.IsEnabled)
                {
                    this.DisableControls();
                    ucActivityMoveInventory ucActivityMoveInventory = new ucActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
                    await ucActivityMoveInventory.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdTransferFrom.IsEnabled)
                {
                    this.DisableControls();
                    ucActivityHdr transferFrom = new ucActivityHdr(SysCommon.ActSubTypes.TransferFrom);
                    await transferFrom.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdTransferTo.IsEnabled)
                {
                    this.DisableControls();
                    ucActivityHdr transferTo = new ucActivityHdr(SysCommon.ActSubTypes.TransferTo);
                    await transferTo.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdMoveSkid.IsEnabled)
                {
                    await MoveSkid();
                }
            });

            try
            {
                this.ucInventoryMovementMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucInventoryMovementMenu_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();
                this.DisableControls();
                await this.Close();
            });

            return true;
        }

        #endregion

        #region Properties

        private TransferSettings TransSettings
        {
            get
            {
                return this.transSettings;
            }

            set
            {
                this.transSettings = value;
            }
        }

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
            return "Inventory Movement Menu";
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
        /// <remarks></remarks>
        private void DisableControls()
        {
            this.cmdMoveProduct.IsEnabled = false;
            this.cmdMoveSkid.IsEnabled = false;
            this.cmdTransferFrom.IsEnabled = false;
            this.cmdTransferTo.IsEnabled = false;
            this.IsControlLoaded = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucInventoryMovementMenu_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;

                    this.cmdTransferFrom.IsEnabled = false;
                    this.cmdTransferTo.IsEnabled = false;
                    this.cmdMoveProduct.IsEnabled = false;
                    this.cmdMoveSkid.IsEnabled = false;

                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.ContactLog.LoginID != SysCommon.SetupLoginID
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {

                        if (this.SysEnv.ContactLog.LoginID == SysCommon.SystemLoginID)
                        {
                            this.cmdTransferFrom.IsEnabled = true;
                            this.cmdTransferTo.IsEnabled = true;
                            this.cmdMoveProduct.IsEnabled = true;
                            this.cmdMoveSkid.IsEnabled = true;
                        }
                        else
                        {
                            this.cmdTransferFrom.IsEnabled = this.SysEnv.SysSettings.IsTransMod;
                            this.cmdTransferTo.IsEnabled = this.SysEnv.SysSettings.IsTransMod;
                            this.cmdMoveProduct.IsEnabled = this.SysEnv.SysSettings.IsMovesModule;
                            this.cmdMoveSkid.IsEnabled = this.SysEnv.SysSettings.IsMoveSkidModule;
                        }
                    }

                    // Override all checks if module bypass enabled.
                    if ((this.SysEnv.IsCommunicationTestSuccessful || this.SysEnv.SysSettings.IsDisconnectedMode)
                        && this.SysEnv.IsEnableAllModuleBypassLicense
                        && !this.SysEnv.RefreshOpt.IsRefreshDataFailed)
                    {
                        this.cmdMoveProduct.IsEnabled = true;
                        this.cmdMoveSkid.IsEnabled = true;
                        this.cmdTransferFrom.IsEnabled = true;
                        this.cmdTransferTo.IsEnabled = true;
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
        private async void cmdMoves_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            ucActivityMoveInventory ucActivityMoveInventory = new ucActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
            await ucActivityMoveInventory.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdTransferFrom_Click(object sender, EventArgs e)
        {
            this.SetSaveTransSettings();
            this.DisableControls();

            ucActivityHdr transferFrom = new ucActivityHdr(SysCommon.ActSubTypes.TransferFrom);
            await transferFrom.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdTransferTo_Click(object sender, EventArgs e)
        {
            this.SetSaveTransSettings();
            this.DisableControls();

            ucActivityHdr transferTo = new ucActivityHdr(SysCommon.ActSubTypes.TransferTo);
            await transferTo.Show();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdMoveSkid_Click(object sender, EventArgs e)
        {
            await MoveSkid();
        }

        /// <summary>
        /// Opens the move skid screen and disables controls of this screen.
        /// </summary>
        private async Task MoveSkid()
        {
            this.DisableControls();

            ucActivityMoveSkid moveSkid = new ucActivityMoveSkid(SysCommon.ActSubTypes.MoveSkid);
            await moveSkid.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetSaveTransSettings()
        {
            TransferSettings tempTransSettings = this.TransSettings;
            this.TransSettings.SaveData(ref tempTransSettings);
            this.TransSettings = tempTransSettings;
        }

        #endregion    
    }
}