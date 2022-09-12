// <copyright file="ucInventoryMovementMenu.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class ucInventoryMovementMenu : DMobileContentPage, IAppArea
    {

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// Initializes a new instance of the <see cref="ucInventoryMovementMenu"/> class.
        /// </summary>
        public ucInventoryMovementMenu()
        {
            this.InitializeComponent();

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

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Num1 && this.cmdMoveProduct.IsEnabled)
                {
                    this.DisableControls();
                    var ucActivityMoveInventory = new ActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
                    await ucActivityMoveInventory.Show();
                }
                else if (keycode == Keycode.Num2 && this.cmdTransferFrom.IsEnabled)
                {
                    this.DisableControls();
                    var transferFrom = new ActivityHdr(SysCommon.ActSubTypes.TransferFrom);
                    await transferFrom.Show();
                }
                else if (keycode == Keycode.Num3 && this.cmdTransferTo.IsEnabled)
                {
                    this.DisableControls();
                    var transferTo = new ActivityHdr(SysCommon.ActSubTypes.TransferTo);
                    await transferTo.Show();
                }
                else if (keycode == Keycode.Num4 && this.cmdMoveSkid.IsEnabled)
                {
                    await this.MoveSkid();
                }
            });

            try
            {
                this.ucInventoryMovementMenu_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucInventoryMovementMenu_GotFocus threw an error " + ex, "OK"));
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

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                this.DisableControls();
                await this.Close();
            });

            return true;
        }

        private TransferSettings TransSettings { get; set; }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdMoves_Click(object sender, EventArgs e)
        {
            this.DisableControls();

            var ucActivityMoveInventory = new ActivityMoveInventory(SysCommon.ActSubTypes.MoveInventory);
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

            var transferFrom = new ActivityHdr(SysCommon.ActSubTypes.TransferFrom);
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

            var transferTo = new ActivityHdr(SysCommon.ActSubTypes.TransferTo);
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
            await this.MoveSkid();
        }

        /// <summary>
        /// Opens the move skid screen and disables controls of this screen.
        /// </summary>
        private async Task MoveSkid()
        {
            this.DisableControls();

            var moveSkid = new ActivityMoveSkid(SysCommon.ActSubTypes.MoveSkid);
            await moveSkid.Show();
        }

        /// <summary>
        ///
        /// </summary>
        private void SetSaveTransSettings()
        {
            var tempTransSettings = this.TransSettings;
            _ = this.TransSettings.SaveData(ref tempTransSettings);
            this.TransSettings = tempTransSettings;
        }
    }
}