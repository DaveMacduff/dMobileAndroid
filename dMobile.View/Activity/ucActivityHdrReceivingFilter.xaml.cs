// <copyright file="ucActivityHdrReceivingFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrReceivingFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityHdrReceivingFilter"/> class.
        /// </summary>
        public ucActivityHdrReceivingFilter()
        {
            this.InitializeComponent();

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
                this.cboSortBy1.SelectedIndex = this.RecvSettings.iSortBy1Index;

                this.aSortBy2.Clear();
                _ = this.aSortBy2.Add(string.Empty);
                _ = this.aSortBy2.Add("Purchase Order # Ascending");
                _ = this.aSortBy2.Add("Purchase Order # Descending");
                _ = this.aSortBy2.Add("Issued Date Ascending");
                _ = this.aSortBy2.Add("Issued Date Descending");
                _ = this.aSortBy2.Add("Arrival Ascending");
                _ = this.aSortBy2.Add("Arrival Descending");

                this.cboSortBy2.ItemsSource = this.aSortBy2;
                this.cboSortBy2.SelectedIndex = this.RecvSettings.iSortBy2Index;

                this.aSortBy3.Clear();
                _ = this.aSortBy3.Add(string.Empty);
                _ = this.aSortBy3.Add("Purchase Order # Ascending");
                _ = this.aSortBy3.Add("Purchase Order # Descending");
                _ = this.aSortBy3.Add("Issued Date Ascending");
                _ = this.aSortBy3.Add("Issued Date Descending");
                _ = this.aSortBy3.Add("Arrival Ascending");
                _ = this.aSortBy3.Add("Arrival Descending");

                this.cboSortBy3.ItemsSource = this.aSortBy3;
                this.cboSortBy3.SelectedIndex = this.RecvSettings.iSortBy3Index;

                this.filters.Add(string.Empty, null);
                this.filters.Add("Purchase Order # Ascending", new Sort() { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.ASC });
                this.filters.Add("Purchase Order # Descending", new Sort() { SortValue = "ActivityReceivingPONumber", SortOrder = SortOrder.DESC });
                this.filters.Add("Issued Date Ascending", new Sort() { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.ASC });
                this.filters.Add("Issued Date Descending", new Sort() { SortValue = "ActivityReceivingIssuedDateTime", SortOrder = SortOrder.DESC });
                this.filters.Add("Arrival Ascending", new Sort() { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.ASC });
                this.filters.Add("Arrival Descending", new Sort() { SortValue = "ActivityReceivingArrivalDateTime", SortOrder = SortOrder.DESC });
            }
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

            try
            {
                await this.ucActivityReceivingFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityReceivingFilter_GotFocus threw an error " + ex, "OK"));
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

        public ArrayList aSortBy1 { get; set; } = new ArrayList();
        public ArrayList aSortBy2 { get; set; } = new ArrayList();
        public ArrayList aSortBy3 { get; set; } = new ArrayList();

        public bool IsControlLoaded { get; set; } = false;
        public ReceivingSettings RecvSettings { get; set; } = new ReceivingSettings();
        public string strSQLSortBy { get; set; } = string.Empty;
        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrReceivingFilter";
        public bool bChangesMade { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.RecvSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.RecvSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.RecvSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.RecvSettings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.RecvSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.RecvSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.RecvSettings.SortBy3Value != null && !string.IsNullOrEmpty(this.RecvSettings.SortBy3Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.RecvSettings.SortBy3Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.RecvSettings = this.RecvSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.RecvSettings.dtArrivalDateFilter))
            {
                this.RecvSettings.dtArrivalDateFilter = DateTime.Now;
            }

            if (SysCommon.IsEmptyDate(this.RecvSettings.dtIssuedDateFilter))
            {
                this.RecvSettings.dtIssuedDateFilter = DateTime.Now;
            }

            this.IsControlLoaded = true;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
/// <summary>
///
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
/// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ucActivityReceivingFilter_GotFocus(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                // Reload oReceivingSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!this.IsControlLoaded)
                {
                    this.LoadData();
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.chkApplyArrivalDate.IsToggled = this.RecvSettings.IsApplyArrivalDateFilter;
                    this.dtpArrivalDate.Date = this.RecvSettings.dtArrivalDateFilter;

                    this.chkApplyIssuedDate.IsToggled = this.RecvSettings.IsApplyIssuedDateFilter;
                    this.dtpIssuedDate.Date = this.RecvSettings.dtIssuedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.RecvSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RecvSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RecvSettings.iSortBy3Index;
                });
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool IsReadyToSave = true;

            try
            {
            }
            catch (Exception)
            {
                IsReadyToSave = false;
            }

            return IsReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            return !this.bChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Activity Receiving Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;
            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyArrivalDate.IsToggled)
                    {
                        this.RecvSettings.dtArrivalDateFilter = this.dtpArrivalDate.Date;
                    }

                    this.RecvSettings.IsApplyArrivalDateFilter = this.chkApplyArrivalDate.IsToggled;

                    if (this.chkApplyIssuedDate.IsToggled)
                    {
                        this.RecvSettings.dtIssuedDateFilter = this.dtpIssuedDate.Date;
                    }

                    this.RecvSettings.IsApplyIssuedDateFilter = this.chkApplyIssuedDate.IsToggled;

                    this.RecvSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.RecvSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.RecvSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    ReceivingSettings tempRecvSettings = this.RecvSettings;
                    _ = this.RecvSettings.SaveData(ref tempRecvSettings);
                    this.RecvSettings = tempRecvSettings;

                    bSaveComplete = true;
                    this.bChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Validation Failed", "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception)
            {
                bSaveComplete = false;
            }

            return bSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool bCancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();

                    this.chkApplyArrivalDate.IsToggled = this.RecvSettings.IsApplyArrivalDateFilter;
                    this.dtpArrivalDate.Date = this.RecvSettings.dtArrivalDateFilter;

                    this.chkApplyIssuedDate.IsToggled = this.RecvSettings.IsApplyIssuedDateFilter;
                    this.dtpIssuedDate.Date = this.RecvSettings.dtIssuedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.RecvSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.RecvSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.RecvSettings.iSortBy3Index;
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
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                      "<params>" +
                      "<arrival_date>" + this.RecvSettings.dtArrivalDateFilter.ToShortDateString() + "</arrival_date>" +
                      "<issued_date>" + this.RecvSettings.dtIssuedDateFilter.ToShortDateString() + "</issued_date>" +
                      "<sortby>" + this.strSQLSortBy + "</sortby>" +
                      "</params>");

            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.bChangesMade = true;
                }
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