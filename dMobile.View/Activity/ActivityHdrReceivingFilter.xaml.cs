// <copyright file="ActivityHdrReceivingFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using Bedrock;
    using CustomPages.Modal;
    using dMobile.Bedrock.Settings;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrReceivingFilter : DMobileModal, IAppArea
    {
        private readonly Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrReceivingFilter" /> class.
        /// </summary>
        public ActivityHdrReceivingFilter()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.lblApplyIssuedDate.Text = "Apply Issued Date Filter";
            this.lblPODate.Text = "Issued";
            this.lblApplyArrivalDate.Text = "Apply Arrival Date Filter";
            this.lblArrivalDate.Text = "Arrival";
            this.lblSort.Text = "Sort";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";

            if (!DesignMode.IsDesignModeEnabled)
            {
                // At least one sort option must be selected
                this.aSortBy1.Clear();
                _ = this.aSortBy1.Add("Purchase Order # Ascending");
                _ = this.aSortBy1.Add("Purchase Order # Descending");
                _ = this.aSortBy1.Add("Issued Date Ascending");
                _ = this.aSortBy1.Add("Issued Date Descending");
                _ = this.aSortBy1.Add("Arrival Ascending");
                _ = this.aSortBy1.Add("Arrival Descending");

                this.cboSortBy1.ItemsSource = this.aSortBy1;
                this.cboSortBy1.SelectedIndex = this.RecvSettings.SortBy1Index;

                this.aSortBy2.Clear();
                _ = this.aSortBy2.Add(string.Empty);
                _ = this.aSortBy2.Add("Purchase Order # Ascending");
                _ = this.aSortBy2.Add("Purchase Order # Descending");
                _ = this.aSortBy2.Add("Issued Date Ascending");
                _ = this.aSortBy2.Add("Issued Date Descending");
                _ = this.aSortBy2.Add("Arrival Ascending");
                _ = this.aSortBy2.Add("Arrival Descending");

                this.cboSortBy2.ItemsSource = this.aSortBy2;
                this.cboSortBy2.SelectedIndex = this.RecvSettings.SortBy2Index;

                this.aSortBy3.Clear();
                _ = this.aSortBy3.Add(string.Empty);
                _ = this.aSortBy3.Add("Purchase Order # Ascending");
                _ = this.aSortBy3.Add("Purchase Order # Descending");
                _ = this.aSortBy3.Add("Issued Date Ascending");
                _ = this.aSortBy3.Add("Issued Date Descending");
                _ = this.aSortBy3.Add("Arrival Ascending");
                _ = this.aSortBy3.Add("Arrival Descending");

                this.cboSortBy3.ItemsSource = this.aSortBy3;
                this.cboSortBy3.SelectedIndex = this.RecvSettings.SortBy3Index;

                this.filters.Add(string.Empty, null);
                this.filters.Add("Purchase Order # Ascending", new Sort
                    { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.ASC });
                this.filters.Add("Purchase Order # Descending", new Sort
                    { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.DESC });
                this.filters.Add("Issued Date Ascending", new Sort
                    { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.ASC });
                this.filters.Add("Issued Date Descending", new Sort
                    { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.DESC });
                this.filters.Add("Arrival Ascending", new Sort
                    { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.ASC });
                this.filters.Add("Arrival Descending", new Sort
                    { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.DESC });
            }
        }

        public ArrayList aSortBy1 { get; set; } = new ArrayList();
        public ArrayList aSortBy2 { get; set; } = new ArrayList();
        public ArrayList aSortBy3 { get; set; } = new ArrayList();
        public ReceivingSettings RecvSettings { get; set; } = new ReceivingSettings();
        public string strSQLSortBy { get; set; } = string.Empty;
        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrReceivingFilter";
        public bool bChangesMade { get; set; }

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
        public Task<bool> IsReadyToSave()
        {
            var IsReadyToSave = true;

            try
            {
            }
            catch (Exception)
            {
                IsReadyToSave = false;
            }

            return Task.FromResult(IsReadyToSave);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(!this.bChangesMade);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> SaveChanges()
        {
            var bSaveComplete = false;
            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyArrivalDate.IsToggled)
                    {
                        this.RecvSettings.ArrivalDateFilter = this.dtpArrivalDate.Date;
                    }

                    this.RecvSettings.IsApplyArrivalDateFilter = this.chkApplyArrivalDate.IsToggled;

                    if (this.chkApplyIssuedDate.IsToggled)
                    {
                        this.RecvSettings.IssuedDateFilter = this.dtpIssuedDate.Date;
                    }

                    this.RecvSettings.IsApplyIssuedDateFilter = this.chkApplyIssuedDate.IsToggled;

                    this.RecvSettings.SortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.RecvSettings.SortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.RecvSettings.SortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    var tempRecvSettings = this.RecvSettings;
                    _ = this.RecvSettings.SaveData(ref tempRecvSettings);
                    this.RecvSettings = tempRecvSettings;

                    bSaveComplete = true;
                    this.bChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Validation Failed", "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception)
            {
                bSaveComplete = false;
            }

            return bSaveComplete;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public async Task<bool> CancelChanges()
        {
            var bCancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();

                    this.chkApplyArrivalDate.IsToggled = this.RecvSettings.IsApplyArrivalDateFilter;
                    this.dtpArrivalDate.Date = this.RecvSettings.ArrivalDateFilter;

                    this.chkApplyIssuedDate.IsToggled = this.RecvSettings.IsApplyIssuedDateFilter;
                    this.dtpIssuedDate.Date = this.RecvSettings.IssuedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.RecvSettings.SortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RecvSettings.SortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RecvSettings.SortBy3Index;
                    this.MakeSQLSortBy();

                    bCancelChanges = false;
                    this.bChangesMade = false;
                }
            }
            catch (Exception)
            {
                bCancelChanges = false;
            }

            return bCancelChanges;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <inheritdoc />
        public Task<XDocument> GetXml()
        {
            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                          "<params>" +
                                          "<arrival_date>" + this.RecvSettings.ArrivalDateFilter.ToShortDateString() +
                                          "</arrival_date>" +
                                          "<issued_date>" + this.RecvSettings.IssuedDateFilter.ToShortDateString() +
                                          "</issued_date>" +
                                          "<sortby>" + this.strSQLSortBy + "</sortby>" +
                                          "</params>");

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

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.Grave)
                            {
                                if (await this.SaveChanges())
                                {
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                                else
                                {
                                    this.DialogResult = DialogResult.None;
                                    _ = await this.CancelChanges();
                                }

                                await this.Close();
                            }
                        }
                    });

                    await this.ucActivityReceivingFilter_GotFocus(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                        "this.ucActivityReceivingFilter_GotFocus threw an error " + ex, "OK"));
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

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.RecvSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.RecvSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.RecvSettings.SortBy2Value != null &&
                    !string.IsNullOrEmpty(this.RecvSettings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.RecvSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.RecvSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.RecvSettings.SortBy3Value != null &&
                    !string.IsNullOrEmpty(this.RecvSettings.SortBy3Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.RecvSettings.SortBy3Value;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.RecvSettings = this.RecvSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.RecvSettings.ArrivalDateFilter))
            {
                this.RecvSettings.ArrivalDateFilter = DateTime.Now;
            }

            if (SysCommon.IsEmptyDate(this.RecvSettings.IssuedDateFilter))
            {
                this.RecvSettings.IssuedDateFilter = DateTime.Now;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ucActivityReceivingFilter_GotFocus(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            await Task.Run(async () =>
            {
                // Reload oReceivingSettings because intial values will be scewed because of SelectedIndexChanged events firing
                this.LoadData();

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.chkApplyArrivalDate.IsToggled = this.RecvSettings.IsApplyArrivalDateFilter;
                    this.dtpArrivalDate.Date = this.RecvSettings.ArrivalDateFilter;

                    this.chkApplyIssuedDate.IsToggled = this.RecvSettings.IsApplyIssuedDateFilter;
                    this.dtpIssuedDate.Date = this.RecvSettings.IssuedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.RecvSettings.SortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RecvSettings.SortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RecvSettings.SortBy3Index;
                });
            });
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Activity Receiving Filter";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                this.bChangesMade = true;
            }
            catch (Exception)
            {
            }
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
            this.DialogResult = DialogResult.None;
            _ = await this.CancelChanges();
            await this.Close();
        }

        private void dtpArrivalDate_DateSelected(object sender, DateChangedEventArgs e)
        {
        }
    }
}