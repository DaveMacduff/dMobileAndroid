// <copyright file="ActivityHdrPickingFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using CustomPages.Modal;
    using dMobile.Bedrock.Settings;
    using Forms;
    using Resources.Enums;
    using Resources.Models;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrPickingFilter : DMobileModal, IAppArea
    {
        private bool isFirstAppearance = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ucActivityHdrPickingFilter" /> class.
        /// </summary>
        public ucActivityHdrPickingFilter()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.lblApplyRouteCode.Text = "Apply Route Code Filter";
            this.lblApplyDeliveryDate.Text = "Apply Delivery Date Filter";
            this.lblDeliveryDate.Text = "Delivered";
            this.lblApplyOrderedDate.Text = "Apply Ordered Date Filter";
            this.lblOrderedDate.Text = "Ordered";
            this.lblRoute.Text = "Route";
            this.lblSortBy.Text = "Sort By";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.PickSettings = new PickingSettings();
                this.Filters = new Dictionary<string, Sort>();
                this.RoutesList = new ObservableCollection<object>();
            }
        }

        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.PickingFilter;

        private ArrayList SortBy1 { get; } = new ArrayList();

        private ArrayList SortBy2 { get; } = new ArrayList();

        private ArrayList SortBy3 { get; } = new ArrayList();

        private ArrayList SortBy4 { get; } = new ArrayList();

        private bool IsControlLoaded { get; set; }

        private PickingSettings PickSettings { get; set; }

        private string SQLSortBy { get; set; } = string.Empty;

        private bool IsChangesMade { get; set; }

        public Dictionary<string, Sort> Filters { get; set; }
        public ObservableCollection<object> RoutesList { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.PickSettings.RouteCodeSelectedIndex = this.cboRoute.SelectedIndex;

                    if (this.chkApplyRouteCode.IsToggled && this.cboRoute.SelectedIndex >= 0)
                    {
                        var selectedItem = this.cboRoute.SelectedItem as dynamic;

                        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.RouteCode))
                        {
                            this.PickSettings.RouteCodeSelectedValue = Convert.ToString(selectedItem.RouteCode);
                        }
                    }
                    else
                    {
                        this.PickSettings.RouteCodeSelectedIndex = -1;
                        this.PickSettings.RouteCodeSelectedValue = string.Empty;
                    }

                    if (this.chkApplyDeliveryDate.IsToggled)
                    {
                        this.PickSettings.DeliveryDateFilter = this.dtpDeliveryDate.Date;
                    }

                    if (this.chkApplyOrderedDate.IsToggled)
                    {
                        this.PickSettings.OrderedDateFilter = this.dtpOrderedDate.Date;
                    }

                    this.PickSettings.IsApplyRouteCodeFilter = this.chkApplyRouteCode.IsToggled;
                    this.PickSettings.IsApplyDeliveryDateFilter = this.chkApplyDeliveryDate.IsToggled;
                    this.PickSettings.IsApplyOrderedDateFilter = this.chkApplyOrderedDate.IsToggled;

                    this.PickSettings.SortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.PickSettings.SortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.PickSettings.SortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.PickSettings.SortBy4Index = this.cboSortBy4.SelectedIndex;
                    this.MakeSQLSortBy();

                    var tempPickSettings = this.PickSettings;
                    _ = this.PickSettings.SaveData(ref tempPickSettings);
                    this.PickSettings = tempPickSettings;

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new MessageOnlyException("Data failed validation. Check your data.");
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    await this.LoadData();

                    this.chkApplyRouteCode.IsToggled = this.PickSettings.IsApplyRouteCodeFilter;
                    this.cboRoute.SelectedIndex = this.PickSettings.RouteCodeSelectedIndex;
                    this.cboRoute.SelectedItem = this.PickSettings.RouteCodeSelectedValue;

                    this.chkApplyDeliveryDate.IsToggled = this.PickSettings.IsApplyDeliveryDateFilter;
                    this.dtpDeliveryDate.Date = this.PickSettings.DeliveryDateFilter;

                    this.chkApplyOrderedDate.IsToggled = this.PickSettings.IsApplyOrderedDateFilter;
                    this.dtpOrderedDate.Date = this.PickSettings.OrderedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.PickSettings.SortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.PickSettings.SortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.PickSettings.SortBy3Index;
                    this.cboSortBy4.SelectedIndex = this.PickSettings.SortBy4Index;
                    this.MakeSQLSortBy();

                    cancelChanges = false;
                    this.IsChangesMade = false;

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
                    await this.LoadData();
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<XDocument> GetXml()
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<delivery_date>" + this.PickSettings.DeliveryDateFilter.ToShortDateString() + "</delivery_date>"
                + "<ordered_date>" + this.PickSettings.OrderedDateFilter.ToShortDateString() + "</ordered_date>"
                + "<route_code>" + this.PickSettings.RouteCodeSelectedValue + "</route_code>"
                + "<sortby>" + this.SQLSortBy + "</sortby>"
                + "</params>");

            return Task.FromResult(request);
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.F1)
                        {
                            if (await this.SaveChanges())
                            {
                                this.DialogResult = DialogResult.OK;
                                await this.Close();
                            }
                        }
                    });

                    if (this.isFirstAppearance)
                    {
                        this.isFirstAppearance = false;
                        this.cboRoute.ItemsSource = await this.GetPickingRouteObservableCollection();
                        this.cboRoute.ItemDisplayBinding = new Binding("RouteCode");

                        // At least one sort option must be selected
                        this.SortBy1.Clear();
                        _ = this.SortBy1.Add("Order # Ascending");
                        _ = this.SortBy1.Add("Order # Descending");
                        _ = this.SortBy1.Add("Route # Ascending");
                        _ = this.SortBy1.Add("Route # Descending");
                        _ = this.SortBy1.Add("Ordered Date Ascending");
                        _ = this.SortBy1.Add("Ordered Date Descending");
                        _ = this.SortBy1.Add("Delivery Ascending");
                        _ = this.SortBy1.Add("Delivery Descending");

                        this.cboSortBy1.ItemsSource = this.SortBy1;
                        this.cboSortBy1.SelectedIndex = this.PickSettings.SortBy1Index;

                        this.SortBy2.Clear();
                        _ = this.SortBy2.Add(string.Empty);
                        _ = this.SortBy2.Add("Order # Ascending");
                        _ = this.SortBy2.Add("Order # Descending");
                        _ = this.SortBy2.Add("Route # Ascending");
                        _ = this.SortBy2.Add("Route # Descending");
                        _ = this.SortBy2.Add("Ordered Date Ascending");
                        _ = this.SortBy2.Add("Ordered Date Descending");
                        _ = this.SortBy2.Add("Delivery Ascending");
                        _ = this.SortBy2.Add("Delivery Descending");

                        this.cboSortBy2.ItemsSource = this.SortBy2;
                        this.cboSortBy2.SelectedIndex = this.PickSettings.SortBy2Index;

                        this.SortBy3.Clear();
                        _ = this.SortBy3.Add(string.Empty);
                        _ = this.SortBy3.Add("Order # Ascending");
                        _ = this.SortBy3.Add("Order # Descending");
                        _ = this.SortBy3.Add("Route # Ascending");
                        _ = this.SortBy3.Add("Route # Descending");
                        _ = this.SortBy3.Add("Ordered Date Ascending");
                        _ = this.SortBy3.Add("Ordered Date Descending");
                        _ = this.SortBy3.Add("Delivery Ascending");
                        _ = this.SortBy3.Add("Delivery Descending");

                        this.cboSortBy3.ItemsSource = this.SortBy3;
                        this.cboSortBy3.SelectedIndex = this.PickSettings.SortBy3Index;

                        this.SortBy4.Clear();
                        _ = this.SortBy4.Add(string.Empty);
                        _ = this.SortBy4.Add("Order # Ascending");
                        _ = this.SortBy4.Add("Order # Descending");
                        _ = this.SortBy4.Add("Route # Ascending");
                        _ = this.SortBy4.Add("Route # Descending");
                        _ = this.SortBy4.Add("Ordered Date Ascending");
                        _ = this.SortBy4.Add("Ordered Date Descending");
                        _ = this.SortBy4.Add("Delivery Ascending");
                        _ = this.SortBy4.Add("Delivery Descending");

                        this.cboSortBy4.ItemsSource = this.SortBy4;
                        this.cboSortBy4.SelectedIndex = this.PickSettings.SortBy4Index;

                        this.Filters.Add(string.Empty, null);
                        this.Filters.Add("Order # Ascending", new Sort
                            { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
                        this.Filters.Add("Order # Descending", new Sort
                            { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
                        this.Filters.Add("Route # Ascending", new Sort
                            { SortValue = "RouteCode", SortOrder = SortOrder.ASC });
                        this.Filters.Add("Route # Descending", new Sort
                            { SortValue = "RouteCode", SortOrder = SortOrder.DESC });
                        this.Filters.Add("Ordered Date Ascending", new Sort
                            { SortValue = "ActivityPickingOrderedDateTime", SortOrder = SortOrder.ASC });
                        this.Filters.Add("Ordered Date Descending", new Sort
                            { SortValue = "ActivityPickingOrderedDateTime", SortOrder = SortOrder.DESC });
                        this.Filters.Add("Delivery Ascending", new Sort
                            { SortValue = "ActivityPickingDeliveryDateTime", SortOrder = SortOrder.ASC });
                        this.Filters.Add("Delivery Descending", new Sort
                            { SortValue = "ActivityPickingDeliveryDateTime", SortOrder = SortOrder.DESC });
                    }

                    await this.ucActivityPickingFilter_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error", "this.ucActivityPickingFilter_GotFocus threw an error " + ex,
                            "OK"));
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                _ = await this.CancelChanges();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Picking Filter";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<ObservableCollection<object>> GetPickingRouteObservableCollection()
        {
            var results = await DatabaseConnection.GetInstance().SqlConn
                .QueryAsync<RouteModel>("SELECT DISTINCT RouteCode, RouteRefno FROM tRoute ORDER BY RouteCode ASC");

            if (results?.Count > 0)
            {
                foreach (var routeModel in results)
                {
                    object route = new
                    {
                        routeModel.RouteRefno,
                        routeModel.RouteCode
                    };
                    this.RoutesList.Add(route);
                }
            }

            return this.RoutesList;
        }

        /// <summary>
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
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

                // Do not set the IsChangesMade. MessageOnlyException is really not and error.
                // Allow the validation to handle the setting for IsChangesMade.
                _ = Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityPickingFilter_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                // Reload PickSettings because intial values will be scewed because of SelectedIndexChanged events firing
                await Task.Run(async () => await this.LoadData());
                await Device.InvokeOnMainThreadAsync(() => this.DisplayData());

                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// </summary>
        private void DisplayData()
        {
            this.chkApplyRouteCode.IsToggled = this.PickSettings.IsApplyRouteCodeFilter;

            if (this.PickSettings.RouteCodeSelectedIndex >= 0 &&
                this.PickSettings.RouteCodeSelectedIndex < this.cboRoute.Items.Count)
            {
                this.cboRoute.SelectedIndex = this.PickSettings.RouteCodeSelectedIndex;
            }
            else
            {
                this.cboRoute.SelectedIndex = -1;
            }

            this.chkApplyDeliveryDate.IsToggled = this.PickSettings.IsApplyDeliveryDateFilter;
            this.dtpDeliveryDate.Date = this.PickSettings.DeliveryDateFilter;

            this.chkApplyOrderedDate.IsToggled = this.PickSettings.IsApplyOrderedDateFilter;
            this.dtpOrderedDate.Date = this.PickSettings.OrderedDateFilter;

            this.cboSortBy1.SelectedIndex = this.PickSettings.SortBy1Index;
            this.cboSortBy2.SelectedIndex = this.PickSettings.SortBy2Index;
            this.cboSortBy3.SelectedIndex = this.PickSettings.SortBy3Index;
            this.cboSortBy4.SelectedIndex = this.PickSettings.SortBy4Index;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.SQLSortBy = this.Filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.PickSettings.SortBy1Value = this.Filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.PickSettings.SortBy2Value = this.Filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.PickSettings.SortBy2Value != null &&
                    !string.IsNullOrEmpty(this.PickSettings.SortBy2Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.PickSettings.SortBy3Value = this.Filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.PickSettings.SortBy3Value != null &&
                    !string.IsNullOrEmpty(this.PickSettings.SortBy3Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.SortBy3Value;
                }
            }

            if (this.cboSortBy4.SelectedIndex >= 0)
            {
                this.PickSettings.SortBy4Value = this.Filters[this.cboSortBy4.Items[this.cboSortBy4.SelectedIndex]];

                if (this.PickSettings.SortBy4Value != null &&
                    !string.IsNullOrEmpty(this.PickSettings.SortBy4Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.SortBy4Value;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            this.PickSettings = this.PickSettings.LoadData();

            // Default route selection.
            if (this.PickSettings.RouteCodeSelectedIndex <= this.cboRoute.Items.Count)
            {
                _ = await Device.InvokeOnMainThreadAsync(() =>
                    this.cboRoute.SelectedIndex = this.PickSettings.RouteCodeSelectedIndex);
            }

            // Default delivery date.
            if (SysCommon.IsEmptyDate(this.PickSettings.DeliveryDateFilter))
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.PickSettings.DeliveryDateFilter = DateTime.Now);
            }

            // Default ordered date.
            if (SysCommon.IsEmptyDate(this.PickSettings.OrderedDateFilter))
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.PickSettings.OrderedDateFilter = DateTime.Now);
            }
        }

        private void chkApplyRouteCode_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, EventArgs.Empty);
        }

        private void chkApplyIssuedDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, EventArgs.Empty);
        }

        private void chkApplyOrderedDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, EventArgs.Empty);
        }

        private void chkApplyDeliveryDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, EventArgs.Empty);
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }
    }
}