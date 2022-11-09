// <copyright file="ActivityHdrReturnsFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrReturnsFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdrReturnsFilter"/> class.
        /// </summary>
        public ActivityHdrReturnsFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblApplyReturnedDate.Text = "Apply Returned Date Filter";
            this.lblReturnedDate.Text = "Returned";
            this.lblApplyEnteredDate.Text = "Apply Entered Date Filter";
            this.lblEnteredDate.Text = "Entered";
            this.lblSort.Text = "Sort By";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            this.SortBy1.Clear();
            _ = this.SortBy1.Add("Order # Ascending");
            _ = this.SortBy1.Add("Order # Descending");
            _ = this.SortBy1.Add("Entered Date Ascending");
            _ = this.SortBy1.Add("Entered Date Descending");
            _ = this.SortBy1.Add("Returned Ascending");
            _ = this.SortBy1.Add("Returned Descending");
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;

            this.SortBy2.Clear();
            _ = this.SortBy2.Add(string.Empty);
            _ = this.SortBy2.Add("Order # Ascending");
            _ = this.SortBy2.Add("Order # Descending");
            _ = this.SortBy2.Add("Entered Date Ascending");
            _ = this.SortBy2.Add("Entered Date Descending");
            _ = this.SortBy2.Add("Returned Ascending");
            _ = this.SortBy2.Add("Returned Descending");
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;

            this.SortBy3.Clear();
            _ = this.SortBy3.Add(string.Empty);
            _ = this.SortBy3.Add("Order # Ascending");
            _ = this.SortBy3.Add("Order # Descending");
            _ = this.SortBy3.Add("Entered Date Ascending");
            _ = this.SortBy3.Add("Entered Date Descending");
            _ = this.SortBy3.Add("Returned Ascending");
            _ = this.SortBy3.Add("Returned Descending");
            this.cboSortBy3.ItemsSource = this.SortBy3;
            this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;

            this.filters.Add(string.Empty, null);
            this.filters.Add("Order # Ascending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Order # Descending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Entered Date Ascending", new Sort() { SortValue = "ActivityReturnsOrderedDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Entered Date Descending", new Sort() { SortValue = "ActivityReturnsOrderedDateTime", SortOrder = SortOrder.DESC });
            this.filters.Add("Returned Ascending", new Sort() { SortValue = "ActivityReturnsDeliveryDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Returned Descending", new Sort() { SortValue = "ActivityReturnsDeliveryDateTime", SortOrder = SortOrder.DESC });
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

                if (!this.Loading)
                {
                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityReturnsFilter_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityReturnsFilter_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        private ArrayList SortBy1 { get; set; } = new ArrayList();

        private ArrayList SortBy2 { get; set; } = new ArrayList();

        private ArrayList SortBy3 { get; set; } = new ArrayList();

        private bool IsEnterTrapped { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private ReturnsSettings RetSettings { get; set; } = new ReturnsSettings();

        private string SQLSortBy { get; set; } = string.Empty;

        private string FullClassName { get; set; } = "dMobile.ucActivityHdrReturnsFilter";

        private bool IsChangesMade { get; set; } = false;

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.Returns;

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
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
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
            return "Returns Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyReturnedDate.IsToggled)
                    {
                        this.RetSettings.ReturnedDateFilter = this.dtpReturnedDate.Date;
                    }

                    this.RetSettings.IsApplyReturnedDateFilter = this.chkApplyReturnedDate.IsToggled;

                    if (this.chkApplyEnteredDate.IsToggled)
                    {
                        this.RetSettings.ReturnedDateFilter = this.dtpEnteredDate.Date;
                    }

                    this.RetSettings.IsApplyEnteredDateFilter = this.chkApplyEnteredDate.IsToggled;

                    this.RetSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.RetSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.RetSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    var tempRetSettings = this.RetSettings;
                    _ = this.RetSettings.SaveData(ref tempRetSettings);
                    this.RetSettings = tempRetSettings;

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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
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
                    this.chkApplyReturnedDate.IsToggled = this.RetSettings.IsApplyReturnedDateFilter;
                    this.dtpReturnedDate.Date = this.RetSettings.ReturnedDateFilter;
                    this.chkApplyEnteredDate.IsToggled = this.RetSettings.IsApplyEnteredDateFilter;
                    this.dtpEnteredDate.Date = this.RetSettings.EnteredDateFilter;
                    this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;
                    this.MakeSQLSortBy();

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

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<returned_date>" + this.RetSettings.ReturnedDateFilter.ToShortDateString() + "</returned_date>"
                + "<entered_date>" + this.RetSettings.EnteredDateFilter.ToShortDateString() + "</entered_date>"
                + "<sortby>" + this.SQLSortBy + "</sortby>"
                + "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityReturnsFilter_GotFocus(object sender, EventArgs e)
        {
            // Reload oReturnsSettings because intial values will be scewed because of SelectedIndexChanged events firing
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.chkApplyEnteredDate.IsToggled = this.RetSettings.IsApplyEnteredDateFilter;
            this.dtpEnteredDate.Date = this.RetSettings.EnteredDateFilter;

            this.chkApplyReturnedDate.IsToggled = this.RetSettings.IsApplyReturnedDateFilter;
            this.dtpReturnedDate.Date = this.RetSettings.ReturnedDateFilter;

            this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;
            this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.SQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.RetSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.RetSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.RetSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.RetSettings.SortBy2Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.RetSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.RetSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.RetSettings.SortBy3Value != null && !string.IsNullOrEmpty(this.RetSettings.SortBy3Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.RetSettings.SortBy3Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.RetSettings = this.RetSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.RetSettings.EnteredDateFilter))
            {
                this.RetSettings.EnteredDateFilter = DateTime.Now;
            }

            if (SysCommon.IsEmptyDate(this.RetSettings.ReturnedDateFilter))
            {
                this.RetSettings.ReturnedDateFilter = DateTime.Now;
            }

            this.IsControlLoaded = true;
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }
    }
}