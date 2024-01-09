// <copyright file="LoginStartup.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Views
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using CustomPages;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using Forms;
    using Helpers;
    using Pages;
    using Resources.Enums;
    using Resources.Models;
    using Resources.Types;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="LoginStartup" /> Login startup form.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginStartup : DMobileContentPage, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginStartup" /> class.
        /// </summary>
        public LoginStartup()
        {
            this.InitializeComponent();

            this.Title = "dMobile Login";
            this.cmdLogin.Text = Resource.Login;
            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.lblWarehouse.Text = Resource.Warehouse;
            this.lstWarehouse.Title = "Select Warehouse";
            this.SysEnv = SysEnvironment.GetInstance();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether LogInPressed.
        /// </summary>
        private bool LogInPressed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether ChangesMade.
        /// </summary>
        private bool ChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsAllowedOverridePicker.
        /// </summary>
        private bool IsAllowedOverridePicker { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsAllowedAssocBarCode.
        /// </summary>
        private bool IsAllowedAssocBarCode { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether IsOverrideAddManualDetail.
        /// </summary>
        private bool IsOverrideAddManualDetail { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        private bool IsControlLoaded { get; set; }

        /// <summary>
        ///     Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether IsEnableAllModuleBypassLicense.
        /// </summary>
        private bool IsEnableAllModuleBypassLicense { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///     Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;
            var password = string.Empty;
            var userCode = 0;

            try
            {
                // Validate captured information is complete.

                // Check for empty data.
                if (string.IsNullOrEmpty(this.txtLoginID.Text) || string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    isReadyToSave = false;
                    return isReadyToSave;
                }

                // Check if user exists.
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>(
                    "SELECT * FROM tContactUser WHERE ContactUserCode = ?",
                    Convert.ToInt32(this.txtLoginID.Text));

                // If user code is valid load the user specific system preferences.
                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;
                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail =
                        results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
                }

                // Override the contact login password with the system one if bypass licenses is enabled.
                if (this.IsEnableAllModuleBypassLicense)
                {
                    password = SysCommon.SystemLoginPassword;
                }

                if (userCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    isReadyToSave = false;
                }

                if (password != this.txtLoginPassword.Text)
                {
                    isReadyToSave = false;
                }

                if (this.lstWarehouse.Items.Count > 0 && Convert.ToInt32(this.lstWarehouse.SelectedValue) <= 0)
                {
                    isReadyToSave = false;
                }
            }
            catch (SQLiteException ex)
            {
                var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }
            catch (Exception ex)
            {
                var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}" /> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            if (!this.LogInPressed)
            {
                // attempt to record login
                if (!this.SysEnv.SysSettings.IsDisconnectedMode
                    && this.SysEnv.CommSetup.HostIPAddressComponent1
                    + this.SysEnv.CommSetup.HostIPAddressComponent2
                    + this.SysEnv.CommSetup.HostIPAddressComponent3 != 0)
                {
                    if (this.SysEnv.ContactLog.IsLoggedIn)
                    {
                        this.SysEnv.ContactLog.IsLoggedIn = false;
                        var tempContactLogin = this.SysEnv.ContactLog;
                        _ = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);

                        try
                        {
                            this.SysEnv.ContactLog.SubmitContactLogOut();
                        }
                        catch (SocketException)
                        {
                            // do nothing if login failed
                            // FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            // Device.BeginInvokeOnMainThread(async () => await errReport.ShowDialog());
                        }
                        catch (Exception ex)
                        {
                            var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }
                }
            }

            return !this.ChangesMade;
        }

        /// <summary>
        ///     Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool" /> True if a save was performed successfully else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;
            var locMap = LocationMap.GetInstance();

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SysEnv.ContactLog.LoginID = int.Parse(this.txtLoginID.Text);

                    if (Debugger.IsAttached)
                    {
                        this.SysEnv.ContactLog.LoginPassword = this.txtLoginPassword.Text;
                    }
                    else
                    {
                        this.SysEnv.ContactLog.LoginPassword = string.Empty;
                    }

                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = Convert.ToInt32(this.lstWarehouse.SelectedValue);
                    this.SysEnv.ContactLog.IsLoggedIn = true;
                    this.SysEnv.ContactLog.IsAllowedAssociateProductBarCode = this.IsAllowedAssocBarCode;
                    this.SysEnv.ContactLog.IsAllowedPickingModuleOverridePicker = this.IsAllowedOverridePicker;
                    this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail =
                        this.IsOverrideAddManualDetail;

                    // attempt to record login
                    this.LogInPressed = true;

                    if (!this.SysEnv.SysSettings.IsDisconnectedMode
                        && this.SysEnv.CommSetup.HostIPAddressComponent1
                        + this.SysEnv.CommSetup.HostIPAddressComponent2
                        + this.SysEnv.CommSetup.HostIPAddressComponent3 != 0)
                    {
                        try
                        {
                            this.SysEnv.ContactLog.SubmitContactLogIn();
                        }
                        catch (SocketException)
                        {
                            // do nothing if login failed
                        }
                        catch (Exception ex)
                        {
                            var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }

                    this.SysEnv.IsEnableAllModuleBypassLicense = this.IsEnableAllModuleBypassLicense;

                    var tempContactLogin = this.SysEnv.ContactLog;
                    saveComplete = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                    locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;
                    locMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMap.LoginWhsLocMapRefno);

                    this.ChangesMade = false;
                }
                else
                {
                    this.SysEnv.ContactLog.IsLoggedIn = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await errReport.ShowDialog().ContinueWith(result => { }));

                saveComplete = false;
            }

            return saveComplete;
        }

        /// <summary>
        ///     Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool" />. True if the changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    await this.LoadData();
                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                var errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await errReport.ShowDialog().ContinueWith(result => { });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation containing an XDocument.</returns>
        public async Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            await Task.Run(async () =>
            {
                try
                {
                    base.OnAppearing();
                    this.IsIndicatorRunning = true;
                    SubscribeKeyUpHandlers();
                    await this.LoginStartup_GotFocus();
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = "This activity unexpectedly failed to initialize correctly." +
                                  " Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            });
            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                {
                    try
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Enter)
                        {
                            _ = await this.IsReadyToClose();
                            this.cmdLogin_Click(this, EventArgs.Empty);
                        }

                        await this.LoginStartup_GotFocus();
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                msg,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var msg = "This activity unexpectedly failed to initialize correctly." +
                                      " Please exit and try again.";
                            var errorReport = new FormErrorReport(msg, ex);
                            _ = await errorReport.ShowDialog();
                        });
                    }
                    finally
                    {
                        this.IsIndicatorRunning = false;
                    }
                });
            }
        }

        /// <summary>
        ///     Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Load the warehouses to display for selection.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task LoadData()
        {
            this.txtLoginID.Text = this.SysEnv.ContactLog.LoginID.ToString();
            this.txtLoginPassword.Text = this.SysEnv.ContactLog.LoginPassword;

            var resultsWarehouse = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Warehouse>(
                "SELECT LocationMapRefno, LocationMapWarehouseCode, LocationMapWarehouseName FROM tLocationMapWarehouse");

            if (resultsWarehouse?.Count > 0)
            {
                foreach (var warehouse in resultsWarehouse)
                {
                    warehouse.DisplayName =
                        warehouse.LocationMapWarehouseCode + " " + warehouse.LocationMapWarehouseName;
                }

                this.lstWarehouse.ItemsSource = resultsWarehouse;
                this.lstWarehouse.ItemDisplayBinding = new Binding("DisplayName");
                var selectedWarehouse = resultsWarehouse.Where(warehouse => warehouse.LocationMapRefno ==
                                                                            this.SysEnv.ContactLog
                                                                                .WarehouseLocationMapRefno)
                    .FirstOrDefault();

                if (selectedWarehouse != null)
                {
                    this.lstWarehouse.SelectedIndex = this.lstWarehouse.ItemsSource.IndexOf(selectedWarehouse);
                    this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
                }
            }
        }

        /// <summary>
        ///     The LoginStartup_GotFocus.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task LoginStartup_GotFocus()
        {
            if (!this.IsControlLoaded)
            {
                await this.LoadData();
                this.IsControlLoaded = true;

                if (this.SysEnv.ContactLog.LoginID == 0)
                {
                    _ = this.txtLoginID.Focus();
                }
                else if (this.SysEnv.ContactLog.LoginPassword == string.Empty)
                {
                    _ = this.txtLoginPassword.Focus();
                }
                else
                {
                    _ = this.cmdLogin.Focus();
                }
            }
        }

        /// <summary>
        ///     The IdentifyChangesMade.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.ChangesMade = true;
            }
        }

        /// <summary>
        ///     The ValidateEntry.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task ValidateEntry(object sender, CancelEventArgs e)
        {
            var userCode = 0;
            var password = string.Empty;

            try
            {
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>(
                    "SELECT * FROM tContactUser WHERE ContactUserCode = ?",
                    Convert.ToInt32(this.txtLoginID.Text));

                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;

                    // TODO: Where are these used anymore?
                    // March 28, 2016 - IsAllowedAssocBarCode is set later in this file  in function SaveChanges()
                    // and used in SysShared.IdentifyProductScanned.
                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail =
                        results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
                }

                if (string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    this.txtLoginPassword.Text = string.Empty;
                    password = string.Empty;
                    throw new MessageOnlyException("Please enter a user ID.");
                }

                if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    password = string.Empty;
                    throw new MessageOnlyException("Please enter a password.");
                }

                if (userCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    throw new MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
                }

                if (password != this.txtLoginPassword.Text)
                {
                    throw new MessageOnlyException(Resource.InvalidLoginPassword);
                }

                if (this.lstWarehouse.Items.Count > 0)
                {
                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = Convert.ToInt32(this.lstWarehouse.SelectedValue);

                    if (Convert.ToInt32(this.lstWarehouse.SelectedValue) <= 0)
                    {
                        throw new MessageOnlyException(Resource.SelectedWarehouseRequired);
                    }
                }
                else
                {
                    this.SysEnv.ContactLog.WarehouseLocationMapRefno = 0;
                }

                var locMap = LocationMap.GetInstance();
                locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;

                if (e.Cancel == false)
                {
                    var tempContactLogin = this.SysEnv.ContactLog;
                    _ = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                e.Cancel = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;
                var errReport = new FormErrorReport(ex);
                await errReport.ShowDialog().ContinueWith(result => { });
            }
            catch (Exception ex)
            {
                e.Cancel = false;
                var errReport = new FormErrorReport(ex);
                _ = await errReport.ShowDialog();
            }
        }

        /// <summary>
        ///     The cmdLogin_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsIndicatorRunning = true;
                ////SysShared.PlaySound("InterfaceOptionSelect.wav");

                await Task.Run(async () =>
                {
                    await this.ValidateEntry(sender, new CancelEventArgs());

                    if (await this.SaveChanges())
                    {
                        // <WO ID="14734" Comment="Show message after login when data is older than 48 hours" />
                        if (this.SysEnv == null
                            || this.SysEnv.RefreshOpt == null
                            || this.SysEnv.RefreshOpt.RefreshedDataLast == null
                            || (DateTime.Now - this.SysEnv.RefreshOpt.RefreshedDataLast).TotalHours > 48)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                Resource.RefreshRequired,
                                Resource.DataIsOutOfDatePleaseRefreshImmediately,
                                Resource.OK));
                        }

                        _ = await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = new MainPage());
                    }
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
            finally
            {
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     The PictureBoxLogo_DoubleClick.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void PictureBoxLogo_DoubleClick(object sender, EventArgs e)
        {
            if (this.IsEnableAllModuleBypassLicense)
            {
                // this.txtLoginPassword.BackColor = SystemColors.Window;
                this.IsEnableAllModuleBypassLicense = false;
            }
            else
            {
                this.txtLoginPassword.BackgroundColor = Color.Aqua;
                this.IsEnableAllModuleBypassLicense = true;
            }

            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                Resource.DMobile,
                Resource.AllModulesEnabledBypassingDeviceLicenses,
                Resource.OK));
        }

        /// <summary>
        ///     The GetControlDescription.
        /// </summary>
        /// <returns>The <see cref="string" />.</returns>
        private string GetControlDescription()
        {
            return "dMobile Login";
        }

        /// <summary>
        ///     The LstWarehouse_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private void LstWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedWarehouse = (Warehouse)this.lstWarehouse.SelectedItem;
            this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
        }
    }
}