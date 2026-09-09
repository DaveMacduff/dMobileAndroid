// <copyright file="FormAddManualDetailOverride.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Android.Views;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using CustomPages.Modal;
    using Resources.Models.Contacts;
    using Resources.Enums;
    using Resources.Models;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    ///     Add manual detail override.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormAddManualDetailOverride : DMobileModal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FormAddManualDetailOverride" /> class.
        /// </summary>
        public FormAddManualDetailOverride()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = "Manual Detail Override";
            this.cmdCancel.Text = "Cancel";
            this.lblLoginPassword.Text = "Password";
            this.lblLoginID.Text = "LoginID";
            this.cmdOK.Text = "OK";

            this.DialogResult = DialogResult.None;

            if (SysEnvironment.GetInstance().IsEnableAllModuleBypassLicense)
            {
                this.txtLoginPassword.BackgroundColor = Color.Aqua;
                this.IsEnableAllModuleBypassLicense = true;
            }
            else
            {
                // this.txtLoginPassword.BackColor = SystemColors.Window;
                this.IsEnableAllModuleBypassLicense = false;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is allowed to add manual detail.
        /// </summary>
        private bool IsAllowedAddManualDetail { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether bypass license all modules is enabled.
        /// </summary>
        private bool IsEnableAllModuleBypassLicense { get; }

        /// <summary>
        ///     Gets or sets the staff user login code.
        /// </summary>
        private int UserCode { get; set; }

        /// <summary>
        ///     Gets or sets the staff user login password.
        /// </summary>
        private string Password { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                await this.Scanner.EnableScanning();
            }
        }

        /// <summary>
        ///     On appearing event handler.
        /// </summary>
        protected override void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();

                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), keyPressed =>
                {
                    if (this.IsIndicatorRunning)
                    {
                        return;
                    }

                    // TODO: Verify key codes.
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        this.CmdOK_Click(null, EventArgs.Empty);
                    }
                });

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     On back button pressed event handler.
        /// </summary>
        /// <returns><see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        ///     On disappearing event handler.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Scan event arguments.</param>
        private Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Command OK click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdOK_Click(object sender, EventArgs e)
        {
            var isValidData = true;

            if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
            {
                isValidData = false;
            }

            if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
            {
                isValidData = false;
            }

            if (this.Password != this.txtLoginPassword.Text)
            {
                isValidData = false;
            }

            if (isValidData && this.IsAllowedAddManualDetail)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Add Manual Detail Override",
                    "Permission denied.",
                    "OK"));
            }

            await this.Close();
        }

        /// <summary>
        ///     Command cancel click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdCancel_Click(object sender, EventArgs e)
        {
            await this.Close();
        }

        /// <summary>
        ///     Validate manual detail override entry data.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="CancelEventArgs" /> Cancel event arguments.</param>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    var resultsContactUser = await DatabaseConnection.GetInstance().SqlConn
                        .QueryAsync<ContactUserModel>(
                            "SELECT * FROM tContactUser WHERE ContactUserCode = ?",
                            Convert.ToInt32(this.txtLoginID.Text));

                    if (resultsContactUser?.Count > 0)
                    {
                        this.Password = resultsContactUser[0].ContactUserPassword;
                        this.UserCode = resultsContactUser[0].ContactUserCode;
                        this.IsAllowedAddManualDetail =
                            resultsContactUser[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
                    }
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
                else if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    throw new MessageOnlyException($"Invalid login id {this.txtLoginID.Text}");
                }

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
                    throw new MessageOnlyException("Invalid login password.");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                e.Cancel = true;
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Add Manual Detail Override",
                    msg,
                    "OK"));
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }
    }
}