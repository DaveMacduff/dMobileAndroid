// <copyright file="ucLoginStartup.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucLoginStartup : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ucLoginStartup"/> class.
        /// </summary>
        public ucLoginStartup()
        {
            this.InitializeComponent();

            this.Title = "dMobile Login";
            this.cmdLogin.Text = Resource.Login;
            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.lblWarehouse.Text = Resource.Warehouse;
            this.lstWarehouse.Title = "Select Warehouse";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
            }
        }

        private bool LogInPressed { get; set; } = false;

        private bool ChangesMade { get; set; } = false;

        private bool IsAllowedOverridePicker { get; set; } = false;

        private bool IsAllowedAssocBarCode { get; set; } = true;

        private bool IsOverrideAddManualDetail { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private SysEnvironment SysEnv { get; set; }

        private bool IsEnableAllModuleBypassLicense { get; set; } = false;

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                try
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter)
                    {
                        _ = await this.IsReadyToClose();
                        this.cmdLogin_Click(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            try
            {
                await this.LoginStartup_GotFocus();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.Error, "LoginStartup_GotFocus threw an error: " + ex.Message, Resource.OK));
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
            bool isReadyToSave = true;
            string password = string.Empty;
            int userCode = 0;

            try
            {
                if (string.IsNullOrEmpty(this.txtLoginID.Text) || string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    isReadyToSave = false;
                    return isReadyToSave;
                }

                List<tContactUserModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>("SELECT * FROM tContactUser WHERE ContactUserCode = ?", Convert.ToInt32(this.txtLoginID.Text));

                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;

                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail = results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
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
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                isReadyToSave = false;
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            if (!this.LogInPressed)
            {
                // attempt to record login
                if (!this.SysEnv.SysSettings.IsDisconnectedMode
                    && (this.SysEnv.CommSetup.HostIPAddressComponent1
                    + this.SysEnv.CommSetup.HostIPAddressComponent2
                    + this.SysEnv.CommSetup.HostIPAddressComponent3) != 0)
                {
                    if (this.SysEnv.ContactLog.IsLoggedIn)
                    {
                        this.SysEnv.ContactLog.IsLoggedIn = false;
                        ContactLogin tempContactLogin = this.SysEnv.ContactLog;
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
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }
                }
            }

            return !this.ChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;
            LocationMap locMap = LocationMap.GetInstance();

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
                    this.SysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail = this.IsOverrideAddManualDetail;

                    // attempt to record login
                    this.LogInPressed = true;

                    if (!this.SysEnv.SysSettings.IsDisconnectedMode
                        && (this.SysEnv.CommSetup.HostIPAddressComponent1
                        + this.SysEnv.CommSetup.HostIPAddressComponent2
                        + this.SysEnv.CommSetup.HostIPAddressComponent3) != 0)
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
                            FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                            _ = await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog());
                        }
                    }

                    this.SysEnv.IsEnableAllModuleBypassLicense = this.IsEnableAllModuleBypassLicense;

                    ContactLogin tempContactLogin = this.SysEnv.ContactLog;
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
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await Device.InvokeOnMainThreadAsync(async () => await errReport.ShowDialog().ContinueWith(result =>
                {
                }));

                saveComplete = false;
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

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
                FormErrorReport errReport = new FormErrorReport(SysCommon.ActSubTypes.LoginStartup, ex);
                await errReport.ShowDialog().ContinueWith(result =>
                {
                });
            }

            return cancelChanges;
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

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            this.txtLoginID.Text = this.SysEnv.ContactLog.LoginID.ToString();
            this.txtLoginPassword.Text = this.SysEnv.ContactLog.LoginPassword;

            List<Warehouse> resultsWarehouse = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Warehouse>("SELECT LocationMapRefno, LocationMapWarehouseCode, LocationMapWarehouseName FROM tLocationMapWarehouse");

            if (resultsWarehouse?.Count > 0)
            {
                foreach (Warehouse warehouse in resultsWarehouse)
                {
                    warehouse.DisplayName = warehouse.LocationMapWarehouseCode + " " + warehouse.LocationMapWarehouseName;
                }

                this.lstWarehouse.ItemsSource = resultsWarehouse;
                this.lstWarehouse.ItemDisplayBinding = new Binding("DisplayName");
                Warehouse selectedWarehouse = resultsWarehouse.Where(warehouse => warehouse.LocationMapRefno == this.SysEnv.ContactLog.WarehouseLocationMapRefno).FirstOrDefault();

                if (selectedWarehouse != null)
                {
                    this.lstWarehouse.SelectedIndex = this.lstWarehouse.ItemsSource.IndexOf(selectedWarehouse);
                    this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
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
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.ChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ValidateEntry(object sender, CancelEventArgs e)
        {
            int userCode = 0;
            string password = string.Empty;

            try
            {
                List<tContactUserModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>("SELECT * FROM tContactUser WHERE ContactUserCode = ?", Convert.ToInt32(this.txtLoginID.Text));

                if (results?.Count > 0)
                {
                    userCode = results[0].ContactUserCode;
                    password = results[0].ContactUserPassword;

                    // TODO: Where are these used anymore?
                    // March 28, 2016 - IsAllowedAssocBarCode is set later in this file  in function SaveChanges(); and used in SysShared.IdentifyProductScanned.
                    this.IsAllowedAssocBarCode = results[0].ContactUserIsAllowedAssociateProductBarCode;
                    this.IsAllowedOverridePicker = results[0].ContactUserIsAllowedPickingModuleOverridePicker;
                    this.IsOverrideAddManualDetail = results[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;

                }

                if (string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    this.txtLoginPassword.Text = string.Empty;
                    password = string.Empty;
                    throw new MessageOnlyException("Please enter a user ID.");
                }
                else if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    password = string.Empty;
                    throw new MessageOnlyException("Please enter a password.");
                }
                else if (userCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    throw new MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
                }
                else if (password != this.txtLoginPassword.Text)
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

                LocationMap locMap = LocationMap.GetInstance();
                locMap.LoginWhsLocMapRefno = this.SysEnv.ContactLog.WarehouseLocationMapRefno;

                if (e.Cancel == false)
                {
                    ContactLogin tempContactLogin = this.SysEnv.ContactLog;
                    _ = this.SysEnv.ContactLog.SaveData(ref tempContactLogin);
                }
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;
                FormErrorReport errReport = new FormErrorReport(ex);
                await errReport.ShowDialog().ContinueWith(result =>
                {
                });
            }
            catch (Exception ex)
            {
                e.Cancel = false;
                FormErrorReport errReport = new FormErrorReport(ex);
                _ = await errReport.ShowDialog();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;

                await Task.Run(async () =>
                {
                    await this.ValidateEntry(sender, new CancelEventArgs());

                    if (await this.SaveChanges())
                    {
                        // <WO ID="14734" Comment="Show message after login when data is older than 48 hours" />
                        if (this.SysEnv == null || this.SysEnv.RefreshOpt == null || this.SysEnv.RefreshOpt.RefreshedDataLast == null || (DateTime.Now - this.SysEnv.RefreshOpt.RefreshedDataLast).TotalHours > 48)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.RefreshRequired, Resource.DataIsOutOfDatePleaseRefreshImmediately, Resource.OK));
                        }

                        _ = await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage = new MainPage());
                    }
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.DMobile, Resource.AllModulesEnabledBypassingDeviceLicenses, Resource.OK));
        }

        public string GetControlDescription()
        {
            throw new NotImplementedException();
        }

        private void LstWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            Warehouse selectedWarehouse = (Warehouse)this.lstWarehouse.SelectedItem;
            this.lstWarehouse.SelectedValue = selectedWarehouse.LocationMapRefno;
        }
    }
}