// <copyright file="FormAssociateBarCodeOverride.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Models;
    using dMobile.View.CustomPages.Modal;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="FormAssociateBarCodeOverride" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormAssociateBarCodeOverride : DMobileModal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormAssociateBarCodeOverride"/> class.
        /// </summary>
        public FormAssociateBarCodeOverride()
        {
            // TODO: Test this form
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.cmdCancel.Text = Resource.Cancel;
            this.cmdOK.Text = Resource.OK;

            if (!DesignMode.IsDesignModeEnabled)
            {
                if (SysEnvironment.GetInstance().IsEnableAllModuleBypassLicense)
                {
                    this.txtLoginPassword.BackgroundColor = Color.Aqua;
                    this.IsEnableAllModuleBypassLicense = true;
                }
                else
                {
                    // this.txtLoginPassword.BackgroundColor = SystemColors.Window;
                    this.IsEnableAllModuleBypassLicense = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether bypass license all modules is enabled.
        /// </summary>
        private bool IsEnableAllModuleBypassLicense { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether is allowed to associate a bar code.
        /// </summary>
        private bool IsAllowedAssocBarCode { get; set; } = false;

        /// <summary>
        /// Gets or sets the staff user login code.
        /// </summary>
        private int UserCode { get; set; } = 0;

        /// <summary>
        /// Gets or sets the staff user login password.
        /// </summary>
        private string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(BarCodeHandler);
                await this.Scanner.DisableScanning();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                await this.Scanner.EnableScanning();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// The on appearing event handler.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        /// <summary>
        /// Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Scan event arguments.</param>
        private static Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command OK click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdOK_Click(object sender, EventArgs e)
        {
            var isValidData = true;

            try
            {
                this.ValidateEntry(this, new CancelEventArgs());

                if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    isValidData = false;
                }
                else if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    isValidData = false;
                }
                else if (this.Password != this.txtLoginPassword.Text)
                {
                    isValidData = false;
                }
            }
            catch (Exception)
            {
                // Catch if convert to int fails.
                isValidData = false;
            }

            if (isValidData && this.IsAllowedAssocBarCode)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                });
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    Resource.BarCodeAssociation,
                    Resource.PermissionDenied,
                    Resource.OK));
            }
        }

        /// <summary>
        /// Command Cancel click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void CmdCancel_Click(object sender, EventArgs e)
        {
            async void CancelCallback()
            {
                this.DialogResult = DialogResult.Cancel;
                await this.Close();
            }

            Device.BeginInvokeOnMainThread(CancelCallback);
        }

        /// <summary>
        /// Validate bar code override entry.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    var results = await DatabaseConnection.GetInstance().SqlConn.
                        QueryAsync<ContactUserModel>(
                        "SELECT * FROM tContactUser WHERE ContactUserCode = ?",
                        Convert.ToInt32(this.txtLoginID.Text));

                    if (results?.Count > 0)
                    {
                        this.Password = results[0].ContactUserPassword;
                        this.UserCode = results[0].ContactUserCode;
                        this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    }
                }

                if (sender == this.txtLoginID)
                {
                    if (string.IsNullOrEmpty(this.txtLoginID.Text))
                    {
                        this.txtLoginPassword.Text = string.Empty;
                        this.Password = string.Empty;
                    }
                    else if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                    {
                        this.Password = string.Empty;
                    }
                    else if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
                    {
                        throw new MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
                    }
                }

                if (sender == this.txtLoginPassword)
                {
                    // Override the contact login password with the system one if bypass licenses is enabled.
                    if (this.IsEnableAllModuleBypassLicense)
                    {
                        this.Password = SysCommon.SystemLoginPassword;
                    }

                    if (string.IsNullOrEmpty(this.txtLoginID.Text))
                    {
                        this.txtLoginPassword.Text = string.Empty;
                        this.Password = string.Empty;
                    }
                    else if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                    {
                        this.Password = string.Empty;
                    }
                    else if (this.Password != this.txtLoginPassword.Text)
                    {
                        throw new MessageOnlyException(Resource.InvalidLoginPassword);
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                e.Cancel = true;
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    Resource.Barcode,
                    msg,
                    Resource.OK));
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;
                _ = await new FormErrorReport(ex).ShowDialog();
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                _ = await new FormErrorReport(ex).ShowDialog();
            }
        }

        /// <summary>
        /// The CmdOK_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void CmdOK_Clicked(object sender, EventArgs e)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                _ = await new FormErrorReport(ex).ShowDialog();
            }
        }
    }
}
