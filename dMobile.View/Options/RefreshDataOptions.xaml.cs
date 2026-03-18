// <copyright file="RefreshDataOptions.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using CustomPages;
    using dMobile.View.Pages;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="RefreshDataOptions" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RefreshDataOptions : DMobileContentPage, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshDataOptions"/> class.
        /// </summary>
        public RefreshDataOptions()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();

                try
                {
                    this.ucRefreshDataOptions_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => this.DisplayAlert(
                        "Error",
                        "ucRefreshDataOptions_GotFocus exception: " + ex.Message,
                        "OK"));
                }
            }

            this.cmdRefreshData.Text = Resource.RefreshData;
            this.cmdDataStatus.Text = Resource.DataStatus;
            this.lblSelectAll.Text = Resource.SelectDeselectAll;
            this.lblSystemData.Text = Resource.SystemData;
            this.lblAdjustments.Text = Resource.Adjustments;
            this.lblPurchaseOrders.Text = Resource.PurchaseOrders;
            this.lblSalesOrders.Text = Resource.SalesOrders;
            this.lblProductionTickets.Text = Resource.ProductionTickets;
            this.lblReplenishment.Text = Resource.Replenishment;
            this.lblCounts.Text = Resource.Counts;
            this.lblTransfers.Text = Resource.Transfers;
            this.lblPurchaseOrderWorkSheet.Text = Resource.PurchaseOrderWorkSheet;
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;
                base.OnAppearing();

                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async (keyPressed) =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.F1 &&
                            await this.SaveChanges())
                        {
                            await this.Close();
                        }
                    });

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
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
        /// The OnBackButtonPressed
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
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

        /// <summary>
        /// Gets or sets the SysEnv
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDisplayShutDownMsg
        /// </summary>
        private bool IsDisplayShutDownMsg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRefreshComplete
        /// </summary>
        private bool IsRefreshComplete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded
        /// </summary>
        private bool IsControlLoaded { get; set; }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The IsReadyToSave
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                // if select all make sure all items are selected. including disconnected mode activities if applicable
                if (this.chkSelectAll.IsToggled &&
                    this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    isReadyToSave = isReadyToSave && this.chkSystemData.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkAdjustments.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkCounts.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkProductionTickets.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkPurchaseOrders.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkReplenishment.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkSalesOrders.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkTransfers.IsToggled;
                    isReadyToSave = isReadyToSave && this.chkPurchaseOrderWorkSheet.IsToggled;
                }

                // if we are not in disconnected mode do not allow activity refreshing
                if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    isReadyToSave = isReadyToSave && !this.chkAdjustments.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkCounts.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkProductionTickets.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkPurchaseOrders.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkReplenishment.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkSalesOrders.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkTransfers.IsToggled;
                    isReadyToSave = isReadyToSave && !this.chkPurchaseOrderWorkSheet.IsToggled;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// The IsReadyToClose
        /// </summary>
        /// <returns></returns>
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
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        public async Task RefreshComplete()
        {
            if (this.IsRefreshComplete &&
                this.IsDisplayShutDownMsg)
            {
                this.IsDisplayShutDownMsg = false;

                // Attempt to record logout
                if (!this.SysEnv.SysSettings.IsDisconnectedMode &&
                    this.SysEnv.CommSetup.HostIpAddressComponent1 +
                    this.SysEnv.CommSetup.HostIpAddressComponent2 +
                    this.SysEnv.CommSetup.HostIpAddressComponent3 !=
                    0)
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
                        catch (SocketException ex)
                        {
                            Console.WriteLine(ex);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            Device.BeginInvokeOnMainThread(async () => await this.DisplayAlert(
                                this.GetControlDescription(),
                                "Please log back in for changes to be made in effect.",
                                "OK"));
                            _ = await Device.InvokeOnMainThreadAsync(() =>
                                Application.Current.MainPage = new NavigationPage(new SplashStartup()));
                        }
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "Please log back in for changes to be made in effect.",
                        "OK"));
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        Application.Current.MainPage = new NavigationPage(new SplashStartup()));
                }
            }
        }

        /// <summary>
        /// The GetControlDescription
        /// </summary>
        /// <returns></returns>
        public string GetControlDescription()
        {
            return Resource.RefreshDataOptions;
        }

        /// <summary>
        /// The SaveChanges
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SysEnv.RefreshOpt.SystemData = this.chkSystemData.IsToggled;
                    this.SysEnv.RefreshOpt.Adjustments = Convert.ToBoolean(
                        this.chkAdjustments.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.PurchaseOrders = Convert.ToBoolean(
                        this.chkPurchaseOrders.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.SalesOrders = Convert.ToBoolean(
                        this.chkSalesOrders.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.ProductionTickets = Convert.ToBoolean(
                        this.chkProductionTickets.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Replenishment = Convert.ToBoolean(
                        this.chkReplenishment.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Counts = Convert.ToBoolean(
                        this.chkCounts.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.Transfers = Convert.ToBoolean(
                        this.chkTransfers.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);
                    this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet = Convert.ToBoolean(
                        this.chkPurchaseOrderWorkSheet.IsToggled && this.SysEnv.SysSettings.IsDisconnectedMode);

                    var tempRefreshOpt = this.SysEnv.RefreshOpt;
                    saveComplete = this.SysEnv.RefreshOpt.SaveData(ref tempRefreshOpt);

                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        Resource.DataFailedValidationCheckYourData,
                        Resource.OK));
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// The CancelChanges
        /// </summary>
        /// <returns></returns>
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
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// The GetXml
        /// </summary>
        /// <returns></returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// The ucRefreshDataOptions_GotFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucRefreshDataOptions_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.LoadData();
                this.IsControlLoaded = true;
                _ = this.cmdRefreshData.Focus();
            }
        }

        /// <summary>
        /// The cmdRefreshData_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdRefreshData_Click(object sender, EventArgs e)
        {
            var isContinueRefresh = false;

            if (this.SysEnv.SysSettings.IsDisconnectedMode &&
                await SysCommon.CheckForUnSubmittedData())
            {
                if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                          this.GetControlDescription(),
                          Resource.WarningUnsubmittedDataExistsAndWillBeLostContinueWithRefreshData,
                          Resource.Yes,
                          Resource.No)) &&
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        Resource.ConfirmRefreshData,
                        Resource.Yes,
                        Resource.No)))
                {
                    isContinueRefresh = true;
                }
            }
            else if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                           this.GetControlDescription(),
                           "Confirm refresh data?",
                           Resource.Yes,
                           Resource.No)))
            {
                isContinueRefresh = true;
            }

            if (isContinueRefresh && await this.SaveChanges())
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var form = new FormDialogContainer(
                        EDialogTypes.RefreshDataStatus,
                        SysCommon.ActSubTypes.RefreshData);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var xmlDoc = await form.GetXml();
                        this.IsRefreshComplete = bool.Parse(xmlDoc.Root.Element("IsRefreshComplete").Value);
                        await this.RefreshComplete();
                    }
                });
            }
        }

        /// <summary>
        /// The cmdDataStatus_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdDataStatus_Click(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                var form = new FormDialogContainer(EDialogTypes.RefreshDataStatus, SysCommon.ActSubTypes.DataStatus);
                _ = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
            }
        }

        /// <summary>
        /// The chkSelectAll_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void chkSelectAll_Click(object sender, EventArgs e)
        {
            this.chkSystemData.IsToggled = this.chkSelectAll.IsToggled && await GroundWork.Common.IsDataBaseDataExist();

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.chkAdjustments.IsToggled = this.chkSelectAll.IsToggled;
                this.chkPurchaseOrders.IsToggled = this.chkSelectAll.IsToggled;
                this.chkSalesOrders.IsToggled = this.chkSelectAll.IsToggled;
                this.chkProductionTickets.IsToggled = this.chkSelectAll.IsToggled;
                this.chkReplenishment.IsToggled = this.chkSelectAll.IsToggled;
                this.chkCounts.IsToggled = this.chkSelectAll.IsToggled;
                this.chkTransfers.IsToggled = this.chkSelectAll.IsToggled;
                this.chkPurchaseOrderWorkSheet.IsToggled = this.chkSelectAll.IsToggled;
            }
        }

        /// <summary>
        /// The IsAllItemsSelected
        /// </summary>
        /// <returns></returns>
        private bool IsAllItemsSelected()
        {
            var isAllSelected = this.chkSystemData.IsToggled &&
                                Convert.ToBoolean(
                                    this.chkAdjustments.IsToggled &&
                                    this.chkPurchaseOrders.IsToggled &&
                                    this.chkSalesOrders.IsToggled &&
                                    this.chkProductionTickets.IsToggled &&
                                    this.chkReplenishment.IsToggled &&
                                    this.chkCounts.IsToggled &&
                                    this.chkTransfers.IsToggled &&
                                    this.chkPurchaseOrderWorkSheet.IsToggled
                                        ? true
                                        : false);
            return isAllSelected;
        }

        /// <summary>
        /// The ClickHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClickHandler(object sender, EventArgs e)
        {
            if (!this.chkSelectAll.IsToggled &&
                this.IsAllItemsSelected())
            {
                this.chkSelectAll.IsToggled = await GroundWork.Common.IsDataBaseDataExist();
            }
            else if (this.chkSelectAll.IsToggled &&
                     !this.IsAllItemsSelected())
            {
                this.chkSelectAll.IsToggled = false;
            }
        }

        /// <summary>
        /// The IdentifyChangesMade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void IdentifyChangesMade(object sender, ToggledEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    if (sender == this.cmdDataStatus)
                    {
                    }
                    else if (sender == this.cmdRefreshData)
                    {
                    }
                    else if (sender == this.chkAdjustments)
                    {
                    }
                    else if (sender == this.chkCounts)
                    {
                    }
                    else if (sender == this.chkProductionTickets)
                    {
                    }
                    else if (sender == this.chkPurchaseOrders)
                    {
                    }
                    else if (sender == this.chkPurchaseOrderWorkSheet)
                    {
                    }
                    else if (sender == this.chkReplenishment)
                    {
                    }
                    else if (sender == this.chkSalesOrders)
                    {
                    }
                    else if (sender == this.chkSelectAll)
                    {
                    }
                    else if (sender == this.chkSystemData)
                    {
                    }
                    else if (sender == this.chkTransfers)
                    {
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

                // Do not set the IsChangesMade. MessageOnlyException is really not and error.
                // Allow the validation to handle the setting for IsChangesMade.
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
            }
        }

        /// <summary>
        /// The ValidateEntry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                if (sender == this.cmdDataStatus)
                {
                }
                else if (sender == this.cmdRefreshData)
                {
                }
                else if (sender == this.chkAdjustments)
                {
                }
                else if (sender == this.chkCounts)
                {
                }
                else if (sender == this.chkProductionTickets)
                {
                }
                else if (sender == this.chkPurchaseOrders)
                {
                }
                else if (sender == this.chkPurchaseOrderWorkSheet)
                {
                }
                else if (sender == this.chkReplenishment)
                {
                }
                else if (sender == this.chkSalesOrders)
                {
                }
                else if (sender == this.chkSelectAll)
                {
                }
                else if (sender == this.chkSystemData)
                {
                }
                else if (sender == this.chkTransfers)
                {
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

                // Do not allow the exit of control.
                e.Cancel = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Do not allow the exit of control.
                e.Cancel = true;
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.RefreshData, ex);
                _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
            }
        }

        /// <summary>
        /// The LoadData
        /// </summary>
        private void LoadData()
        {
            _ = this.SysEnv.RefreshOpt.LoadData();

            this.chkSystemData.IsToggled = this.SysEnv.RefreshOpt.SystemData;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.chkAdjustments.IsToggled = this.SysEnv.RefreshOpt.Adjustments;
                this.chkCounts.IsToggled = this.SysEnv.RefreshOpt.Counts;
                this.chkProductionTickets.IsToggled = this.SysEnv.RefreshOpt.ProductionTickets;
                this.chkPurchaseOrders.IsToggled = this.SysEnv.RefreshOpt.PurchaseOrders;
                this.chkReplenishment.IsToggled = this.SysEnv.RefreshOpt.Replenishment;
                this.chkSalesOrders.IsToggled = this.SysEnv.RefreshOpt.SalesOrders;
                this.chkTransfers.IsToggled = this.SysEnv.RefreshOpt.Transfers;
                this.chkPurchaseOrderWorkSheet.IsToggled = this.SysEnv.RefreshOpt.PurchaseOrderWorkSheet;
            }
            else
            {
                this.chkAdjustments.IsEnabled = false;
                this.chkCounts.IsEnabled = false;
                this.chkProductionTickets.IsEnabled = false;
                this.chkPurchaseOrders.IsEnabled = false;
                this.chkReplenishment.IsEnabled = false;
                this.chkSalesOrders.IsEnabled = false;
                this.chkTransfers.IsEnabled = false;
                this.chkPurchaseOrderWorkSheet.IsEnabled = false;
            }
        }
    }
}
