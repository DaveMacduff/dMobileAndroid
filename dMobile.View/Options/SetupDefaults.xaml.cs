// <copyright file="SetupDefaults.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.View.Pages.Themes;

namespace dMobile.View.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupDefaults : DMobileContentPage
    {
        private bool isChangesMade;
        public ObservableCollection<Theme> themesList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupDefaults"/> class.
        /// </summary>
        public SetupDefaults()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.IsChangesMade = false;
            this.IsControlLoaded = false;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.PrnSettings = new PrinterSettings().LoadData();
                this.ActSubType = SysCommon.ActSubTypes.SetupDefaults;
                this.themesList = new ObservableCollection<Theme>();

                foreach (var theme in (Theme[])Enum.GetValues(typeof(Theme)))
                {
                    this.themesList.Add(theme);
                }

                try
                {
                    this.ucSetupDefaults_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => this.DisplayAlert("Error", "ucSetupDefaults_GotFocus exception: " + ex.Message, "OK"));
                }
            }

            this.tabSetupDefaults.Text = "Setup";
            this.lblDisconnectMode.Text = "Disconnected Mode";
            this.lblDisconnectedModeLoadDetail.Text = "Disconnected Mode Load Detail";
            this.lblVerifyLocationWhenProductChanged.Text = "Verify Location";
            this.lblPrintProductLabelsAsBatch.Text = "Print Product Labels as a Batch";
            this.lblEnableShippingMobilePrinter.Text = "Shipping Label Printer";
            this.lblEnableShelfMobilePrinter.Text = "Shelf Label Printer";
            this.lblEnableProductMobilePrinter.Text = "Product Label Printer";
            this.btnSave.Text = "Save";
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter)
                {
                    this.BtnSave_Clicked(this, EventArgs.Empty);
                }
                else if (keycode == Keycode.F1)
                {
                    this.BtnSave_Clicked(this, EventArgs.Empty);
                }
            });

            this.themePicker.ItemsSource = this.themesList;
            this.themePicker.SelectedItem = this.SysEnv.SysSettings.Theme;
        }

        /// <summary>
        /// The OnDisappearing.
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
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private PrinterSettings PrnSettings { get; set; }

        public bool IsChangesMade
        {
            get => this.isChangesMade;

            set
            {
                this.isChangesMade = value;
                this.OnPropertyChanged();
            }
        }

        private bool IsControlLoaded { get; set; }

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
        public bool IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                _ = Device.InvokeOnMainThreadAsync(async () =>
                      await Application.Current.MainPage.DisplayAlert(
                          this.GetControlDescription(),
                          ex.Message,
                          Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this, */ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Setup Defaults";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    if (this.IsChangesMade && !(this.chkDisconnectMode.IsToggled == this.SysEnv.SysSettings.IsDisconnectedMode && this.chkDisconnectedModeLoadDetail.IsToggled == this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail))
                    {
                        _ = await this.DisplayAlert(this.GetControlDescription(), "Confirm disconnected mode changes?" + Environment.NewLine + "This will purge any un-submitted data.", "Yes", "No").ContinueWith(async (result) =>
                          {
                              if (result.Result)
                              {
                                  var form = new FormDialogContainer(EDialogTypes.RefreshDataStatus, SysCommon.ActSubTypes.PurgeActivityData);
                                  _ = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                                  var xmlDoc = await form.GetXML();
                              }
                              else
                              {
                                  await Device.InvokeOnMainThreadAsync(() =>
                                  {
                                      this.chkDisconnectMode.IsToggled = this.SysEnv.SysSettings.IsDisconnectedMode;
                                      this.chkDisconnectedModeLoadDetail.IsToggled = this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail;
                                      this.chkVerifyLocationWhenProductChanged.IsToggled = this.SysEnv.SysSettings.IsVerifyProductLocation;
                                      this.chkPrintProductLabelsAsBatch.IsToggled = this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch;
                                  });
                              }
                          });
                    }

                    this.SysEnv.SysSettings.IsDisconnectedMode = this.chkDisconnectMode.IsToggled;
                    this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail = this.chkDisconnectedModeLoadDetail.IsToggled;
                    this.SysEnv.SysSettings.IsVerifyProductLocation = this.chkVerifyLocationWhenProductChanged.IsToggled;
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch = this.chkPrintProductLabelsAsBatch.IsToggled;

                    var tempSystemSettings = this.SysEnv.SysSettings;
                    _ = SystemSettingsTasks.SaveData(ref tempSystemSettings);
                    this.SysEnv.SysSettings = tempSystemSettings;

                    this.PrnSettings.ShippingLabelPrinterName = string.Empty;
                    this.PrnSettings.IsEnabledShippingLabelPrinter = this.chkEnableShippingMobilePrinter.IsToggled;

                    if (this.PrnSettings.IsEnabledShippingLabelPrinter)
                    {
                        if (this.cboShippingMobilePrinter.Items.Count > 0 && this.cboShippingMobilePrinter.SelectedIndex > -1)
                        {
                            this.PrnSettings.ShippingLabelPrinterName = this.cboShippingMobilePrinter.SelectedItem.ToString();
                        }
                    }

                    this.PrnSettings.ShelfLabelPrinterName = string.Empty;
                    this.PrnSettings.IsEnabledShelfLabelPrinter = this.chkEnableShelfMobilePrinter.IsToggled;

                    if (this.PrnSettings.IsEnabledShelfLabelPrinter)
                    {
                        if (this.cboShelfMobilePrinter.Items.Count > 0 && this.cboShelfMobilePrinter.SelectedIndex > -1)
                        {
                            this.PrnSettings.ShelfLabelPrinterName = this.cboShelfMobilePrinter.SelectedItem.ToString();
                        }
                    }

                    this.PrnSettings.ProductLabelPrinterName = string.Empty;

                    if (this.PrnSettings.IsEnabledProductLabelPrinter)
                    {
                        this.PrnSettings.IsEnabledProductLabelPrinter = this.chkEnableProductMobilePrinter.IsToggled;

                        if (this.cboProductMobilePrinter.Items.Count > 0 && this.cboProductMobilePrinter.SelectedIndex > -1)
                        {
                            this.PrnSettings.ProductLabelPrinterName = this.cboProductMobilePrinter.SelectedItem.ToString();
                        }
                    }

                    var tempPrnSettings = this.PrnSettings;
                    _ = this.PrnSettings.SaveData(ref tempPrnSettings);
                    this.PrnSettings = tempPrnSettings;

                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    // ClearProperties();
                    // DisplayData();
                    this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSetupDefaults_GotFocus(object sender, EventArgs e)
        {
            this.LoadData();
            this.IsControlLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                }
            }
            catch (MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. MessageOnlyException is really not and error.
                // Allow the validation to handle the setting for IsChangesMade.
                _ = Device.InvokeOnMainThreadAsync(async () =>
                      await Application.Current.MainPage.DisplayAlert(
                          this.GetControlDescription(),
                          ex.Message,
                          Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            if (!this.IsControlLoaded)
            {
                this.chkDisconnectMode.IsToggled = this.SysEnv.SysSettings.IsDisconnectedMode;
                this.chkDisconnectedModeLoadDetail.IsToggled = this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail;
                this.chkVerifyLocationWhenProductChanged.IsToggled = this.SysEnv.SysSettings.IsVerifyProductLocation;
                this.chkPrintProductLabelsAsBatch.IsToggled = this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch;

                this.cboShippingMobilePrinter.Items.Clear();

                foreach (PrinterSettings.PrinterInfo prnDef in this.PrnSettings.AllPrinters)
                {
                    this.cboShippingMobilePrinter.Items.Add(prnDef.Name);
                }

                var isPrinterMatchFound = false;
                this.chkEnableShippingMobilePrinter.IsToggled = false;

                if (this.cboShippingMobilePrinter.Items.Count > 0 && !string.IsNullOrEmpty(this.PrnSettings.ShippingLabelPrinterName))
                {
                    for (var idx = 0; idx <= this.cboShippingMobilePrinter.Items.Count - 1; idx++)
                    {
                        if (this.cboShippingMobilePrinter.Items[idx].ToString() == this.PrnSettings.ShippingLabelPrinterName)
                        {
                            isPrinterMatchFound = true;
                            this.cboShippingMobilePrinter.SelectedIndex = idx;
                            this.PrnSettings.IsEnabledShippingLabelPrinter = true;
                            this.PrnSettings.ShippingLabelPrinterName = this.cboShippingMobilePrinter.Items[idx].ToString();
                            break;
                        }
                    }

                    if (!isPrinterMatchFound)
                    {
                        this.PrnSettings.ShippingLabelPrinterName = string.Empty;
                    }
                }

                this.chkEnableShippingMobilePrinter.IsToggled = this.PrnSettings.IsEnabledShippingLabelPrinter;

                this.cboShelfMobilePrinter.Items.Clear();

                foreach (PrinterSettings.PrinterInfo prnDef in this.PrnSettings.AllPrinters)
                {
                    this.cboShelfMobilePrinter.Items.Add(prnDef.Name);
                }

                isPrinterMatchFound = false;
                this.PrnSettings.IsEnabledShelfLabelPrinter = false;

                if (this.cboShelfMobilePrinter.Items.Count > 0)
                {
                    for (var idx = 0; idx <= this.cboShelfMobilePrinter.Items.Count - 1; idx++)
                    {
                        if (this.cboShelfMobilePrinter.Items[idx].ToString() == this.PrnSettings.ShelfLabelPrinterName)
                        {
                            isPrinterMatchFound = true;
                            this.cboShelfMobilePrinter.SelectedIndex = idx;
                            this.PrnSettings.IsEnabledShelfLabelPrinter = true;
                            this.PrnSettings.ShelfLabelPrinterName = this.cboShelfMobilePrinter.Items[idx].ToString();
                            break;
                        }
                    }

                    if (!isPrinterMatchFound)
                    {
                        this.PrnSettings.ShelfLabelPrinterName = string.Empty;
                    }
                }

                this.chkEnableShelfMobilePrinter.IsToggled = this.PrnSettings.IsEnabledShelfLabelPrinter;

                this.cboProductMobilePrinter.Items.Clear();

                foreach (PrinterSettings.PrinterInfo prnDef in this.PrnSettings.AllPrinters)
                {
                    this.cboProductMobilePrinter.Items.Add(prnDef.Name);
                }

                isPrinterMatchFound = false;
                this.PrnSettings.IsEnabledProductLabelPrinter = false;

                if (this.cboProductMobilePrinter.Items.Count > 0)
                {
                    for (var idx = 0; idx <= this.cboProductMobilePrinter.Items.Count - 1; idx++)
                    {
                        if (this.cboProductMobilePrinter.Items[idx].ToString() == this.PrnSettings.ProductLabelPrinterName)
                        {
                            isPrinterMatchFound = true;
                            this.cboProductMobilePrinter.SelectedIndex = idx;
                            this.PrnSettings.IsEnabledProductLabelPrinter = true;
                            this.PrnSettings.ProductLabelPrinterName = this.cboProductMobilePrinter.Items[idx].ToString();
                            break;
                        }
                    }

                    if (!isPrinterMatchFound)
                    {
                        this.PrnSettings.ProductLabelPrinterName = string.Empty;
                    }
                }

                this.chkEnableProductMobilePrinter.IsToggled = this.PrnSettings.IsEnabledProductLabelPrinter;
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
            try
            {
                // Disconnected Mode
                if (sender == this.chkDisconnectMode)
                {
                    // Cannot turn off disconnected mode if there are unsubmitted transactions
                    var isUnSubmittedTrx = false;
                    var rowsAffected = 0;

                    rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tActivity WHERE ActivityStatus <> ?", Convert.ToInt32(SysCommon.ActStatus.NoChange)));

                    if (rowsAffected > 0)
                    {
                        isUnSubmittedTrx = true;
                    }

                    if (!isUnSubmittedTrx)
                    {
                        rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tActivityItem WHERE ActivityItemStatus <> ?", Convert.ToInt32(SysCommon.ActStatus.NoChange)));

                        if (rowsAffected > 0)
                        {
                            isUnSubmittedTrx = true;
                        }
                    }

                    if (!isUnSubmittedTrx)
                    {
                        rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tActivityDetail WHERE ActivityDetailStatus <> ?", Convert.ToInt32(SysCommon.ActStatus.NoChange)));

                        if (rowsAffected > 0)
                        {
                            isUnSubmittedTrx = true;
                        }
                    }

                    if (isUnSubmittedTrx)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errorReport.ShowDialog();
                });
            }
        }

        private void ChkEnableShippingMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private async void ChkDisconnectMode_Toggled(object sender, ToggledEventArgs e)
        {
            await this.ValidateEntry(sender, new CancelEventArgs());
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void ChkDisconnectedModeLoadDetail_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void ChkVerifyLocationWhenProductChanged_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void ChkPrintProductLabelsAsBatch_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void ChkEnableShelfMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void ChkEnableProductMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                await this.Close();
            }
        }

        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Theme)this.themePicker.SelectedItem != this.SysEnv.SysSettings.Theme)
            {
                this.IdentifyChangesMade(sender, EventArgs.Empty);

                var picker = sender as Picker;
                var theme = (Theme)picker.SelectedItem;

                var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

                if (mergedDictionaries != null)
                {
                    mergedDictionaries.Clear();

                    switch (theme)
                    {
                        case Theme.Dark:
                            mergedDictionaries.Add(new DarkTheme());
                            this.SysEnv.SysSettings.Theme = Theme.Dark;
                            break;
                        case Theme.Light:
                        default:
                            mergedDictionaries.Add(new LightTheme());
                            this.SysEnv.SysSettings.Theme = Theme.Light;
                            break;
                    }
                }
            }
        }
    }
}