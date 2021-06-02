using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Themes;
using System.Collections.ObjectModel;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSetupDefaults : DMobileContentPage
    {
        private bool isControlLoaded;
        private bool isChangesMade;
        public ObservableCollection<Theme> themesList;
        private SysEnvironment sysEnv;
        private PrinterSettings prnSettings;
        private SysCommon.ActSubTypes actSubType;

        public ucSetupDefaults()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.IsChangesMade = false;
            this.IsControlLoaded = false;

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();
                prnSettings = new PrinterSettings().LoadData();
                actSubType = SysCommon.ActSubTypes.SetupDefaults;
                themesList = new ObservableCollection<Theme>();

                foreach (Theme theme in (Theme[])Enum.GetValues(typeof(Theme)))
                {
                    this.themesList.Add(theme);
                }

                try
                {
                    this.ucSetupDefaults_GotFocus(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "ucSetupDefaults_GotFocus exception: " + ex.Message, "OK"));
                }
            }

            // TODO: Resource strings
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
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter)
                {
                    this.BtnSave_Clicked(this, new EventArgs());
                }
                else if (keycode == Keycode.F1)
                {
                    this.BtnSave_Clicked(this, new EventArgs());
                }
            });

            this.themePicker.ItemsSource = this.themesList;
            this.themePicker.SelectedItem = this.SysEnv.SysSettings.Theme;
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

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private PrinterSettings PrnSettings
        {
            get
            {
                return this.prnSettings;
            }

            set
            {
                this.prnSettings = value;
            }
        }

        public bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
                OnPropertyChanged();
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
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this, */ex);
                    await errRpt.ShowDialog();
                });
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
            bool isReadyToClose = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
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
            // TODO: Resource Strings
            return "Setup Defaults";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    if (this.IsChangesMade && !(this.chkDisconnectMode.IsToggled == this.SysEnv.SysSettings.IsDisconnectedMode && this.chkDisconnectedModeLoadDetail.IsToggled == this.SysEnv.SysSettings.IsDisconnectedModeLoadDetail))
                    {
                        await DisplayAlert(this.GetControlDescription(), "Confirm disconnected mode changes?" + Environment.NewLine + "This will purge any unsubmitted data.", "Yes", "No").ContinueWith(async (result) =>
                        {
                            if (result.Result)
                            {
                                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucRefreshDataStatus, SysCommon.ActSubTypes.PurgeActivityData);
                                await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                                XDocument xmlDoc = await form.GetXML();
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

                    SystemSettings tempSystemSettings = this.SysEnv.SysSettings;
                    SystemSettingsTasks.SaveData(ref tempSystemSettings);
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

                    PrinterSettings tempPrnSettings = this.PrnSettings;
                    this.PrnSettings.SaveData(ref tempPrnSettings);
                    this.PrnSettings = tempPrnSettings;

                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource Strings
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
                });
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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errorReport.ShowDialog();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSetupDefaults_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.LoadData();
                this.IsControlLoaded = true;
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
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. MessageOnlyException is really not and error. 
                // Allow the validation to handle the setting for IsChangesMade.
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
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

                bool isPrinterMatchFound = false;
                this.chkEnableShippingMobilePrinter.IsToggled = false;

                if (this.cboShippingMobilePrinter.Items.Count > 0 && !string.IsNullOrEmpty(this.PrnSettings.ShippingLabelPrinterName))
                {
                    for (int idx = 0; idx <= this.cboShippingMobilePrinter.Items.Count - 1; idx++)
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
                    for (int idx = 0; idx <= this.cboShelfMobilePrinter.Items.Count - 1; idx++)
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
                    for (int idx = 0; idx <= this.cboProductMobilePrinter.Items.Count - 1; idx++)
                    {
                        if (this.cboProductMobilePrinter.Items[idx].ToString() == this.PrnSettings.ProductLabelPrinterName)
                        {
                            isPrinterMatchFound = true;
                            cboProductMobilePrinter.SelectedIndex = idx;
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
                    bool isUnSubmittedTrx = false;
                    int rowsAffected = 0;

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
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errorReport.ShowDialog();
                });
            }
        }

        private void ChkEnableShippingMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private async void ChkDisconnectMode_Toggled(object sender, ToggledEventArgs e)
        {
            await this.ValidateEntry(sender, new CancelEventArgs());
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void ChkDisconnectedModeLoadDetail_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void ChkVerifyLocationWhenProductChanged_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void ChkPrintProductLabelsAsBatch_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void ChkEnableShelfMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void ChkEnableProductMobilePrinter_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
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
                this.IdentifyChangesMade(sender, new EventArgs());

                Picker picker = sender as Picker;
                Theme theme = (Theme)picker.SelectedItem;

                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;

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