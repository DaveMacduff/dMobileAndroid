// <copyright file="ActivityHdrTransferFilter.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class ActivityHdrTransferFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdrTransferFilter"/> class.
        /// </summary>
        public ActivityHdrTransferFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblTransferDate.Text = "Transfer Date";
            this.lblApplyDate.Text = "Apply Transfer Date Filter";
            this.lblSort.Text = "SortBy";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.lbl3.Text = "3";
            this.lbl4.Text = "4";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            this.aSortBy1.Clear();
            _ = this.aSortBy1.Add("Transfer # Ascending");
            _ = this.aSortBy1.Add("Transfer # Descending");
            _ = this.aSortBy1.Add("Date Ascending");
            _ = this.aSortBy1.Add("Date Descending");
            _ = this.aSortBy1.Add("Whs # Ascending");
            _ = this.aSortBy1.Add("Whs # Descending");
            _ = this.aSortBy1.Add("Arrival Ascending");
            _ = this.aSortBy1.Add("Arrival Descending");
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.SelectedIndex = this.TransSettings.iSortBy1Index;

            this.aSortBy2.Clear();
            _ = this.aSortBy2.Add(string.Empty);
            _ = this.aSortBy2.Add("Transfer # Ascending");
            _ = this.aSortBy2.Add("Transfer # Descending");
            _ = this.aSortBy2.Add("Date Ascending");
            _ = this.aSortBy2.Add("Date Descending");
            _ = this.aSortBy2.Add("Whs # Ascending");
            _ = this.aSortBy2.Add("Whs # Descending");
            _ = this.aSortBy2.Add("Arrival Ascending");
            _ = this.aSortBy2.Add("Arrival Descending");
            this.cboSortBy2.ItemsSource = this.aSortBy2;
            this.cboSortBy2.SelectedIndex = this.TransSettings.iSortBy2Index;

            this.aSortBy3.Clear();
            _ = this.aSortBy3.Add(string.Empty);
            _ = this.aSortBy3.Add("Transfer # Ascending");
            _ = this.aSortBy3.Add("Transfer # Descending");
            _ = this.aSortBy3.Add("Date Ascending");
            _ = this.aSortBy3.Add("Date Descending");
            _ = this.aSortBy3.Add("Whs # Ascending");
            _ = this.aSortBy3.Add("Whs # Descending");
            _ = this.aSortBy3.Add("Arrival Ascending");
            _ = this.aSortBy3.Add("Arrival Descending");
            this.cboSortBy3.ItemsSource = this.aSortBy3;
            this.cboSortBy3.SelectedIndex = this.TransSettings.iSortBy3Index;

            this.aSortBy4.Clear();
            _ = this.aSortBy4.Add(string.Empty);
            _ = this.aSortBy4.Add("Transfer # Ascending");
            _ = this.aSortBy4.Add("Transfer # Descending");
            _ = this.aSortBy4.Add("Date Ascending");
            _ = this.aSortBy4.Add("Date Descending");
            _ = this.aSortBy4.Add("Whs # Ascending");
            _ = this.aSortBy4.Add("Whs # Descending");
            _ = this.aSortBy4.Add("Arrival Ascending");
            _ = this.aSortBy4.Add("Arrival Descending");
            this.cboSortBy4.ItemsSource = this.aSortBy4;
            this.cboSortBy4.SelectedIndex = this.TransSettings.iSortBy4Index;

            this.filters.Add(string.Empty, null);
            this.filters.Add("Transfer # Ascending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Transfer # Descending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Date Ascending", new Sort() { SortValue = "ActivityTransferDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Date Descending", new Sort() { SortValue = "ActivityTransferDateTime", SortOrder = SortOrder.DESC });
            this.filters.Add("Whs # Ascending", new Sort() { SortValue = "LocationMapWarehouseFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Whs # Descending", new Sort() { SortValue = "LocationMapWarehouseFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Arrival Ascending", new Sort() { SortValue = "LocationMapArrivalDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Arrival Descending", new Sort() { SortValue = "LocationMapArrivalDateTime", SortOrder = SortOrder.DESC });
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
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            _ = await this.CancelChanges();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityFilter_GotFocus threw an error " + ex, "OK"));
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

        public ArrayList aSortBy1 { get; set; } = new ArrayList();

        public ArrayList aSortBy2 { get; set; } = new ArrayList();

        public ArrayList aSortBy3 { get; set; } = new ArrayList();

        public ArrayList aSortBy4 { get; set; } = new ArrayList();

        public bool bEnterTrapped { get; set; } = false;

        public bool IsControlLoaded { get; set; } = false;

        public TransferSettings TransSettings { get; set; } = new TransferSettings();

        public string strSQLSortBy { get; set; } = string.Empty;

        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrTransferFilter";

        public bool bChangesMade { get; set; } = false;

        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.TransSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.TransSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.TransSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.TransSettings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.TransSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.TransSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.TransSettings.SortBy3Value != null && !string.IsNullOrEmpty(this.TransSettings.SortBy3Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.TransSettings.SortBy3Value;
                }
            }

            if (this.cboSortBy4.SelectedIndex >= 0)
            {
                this.TransSettings.SortBy4Value = this.filters[this.cboSortBy4.Items[this.cboSortBy4.SelectedIndex]];

                if (this.TransSettings.SortBy4Value != null && !string.IsNullOrEmpty(this.TransSettings.SortBy4Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.TransSettings.SortBy4Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.TransSettings = this.TransSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.TransSettings.TransferDateFilter))
            {
                this.TransSettings.TransferDateFilter = DateTime.Now;
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
        public void ucActivityFilter_GotFocus(object sender, EventArgs e)
        {
            // Reload oTransferSettings because intial values will be scewed because of SelectedIndexChanged events firing
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.chkApplyDate.IsToggled = this.TransSettings.IsApplyTransferDateFilter;
            this.dtpDate.Date = this.TransSettings.TransferDateFilter;

            this.cboSortBy1.SelectedIndex = this.TransSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.TransSettings.iSortBy2Index;
            this.cboSortBy3.SelectedIndex = this.TransSettings.iSortBy3Index;
            this.cboSortBy4.SelectedIndex = this.TransSettings.iSortBy4Index;
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
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IsReadyToSave", Ex) == false)
                {
                }

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
            return "Activity Transfer Filter";
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
                    if (this.chkApplyDate.IsToggled)
                    {
                        this.TransSettings.TransferDateFilter = this.dtpDate.Date;
                    }

                    this.TransSettings.IsApplyTransferDateFilter = this.chkApplyDate.IsToggled;

                    this.TransSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.TransSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.TransSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.TransSettings.iSortBy4Index = this.cboSortBy4.SelectedIndex;
                    this.MakeSQLSortBy();

                    TransferSettings temp_oTransferSettings = this.TransSettings;
                    _ = this.TransSettings.SaveData(ref temp_oTransferSettings);
                    this.TransSettings = temp_oTransferSettings;

                    bSaveComplete = true;
                    this.bChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "SaveChanges", Ex) == false)
                {
                }

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

                    this.cboSortBy1.SelectedIndex = this.TransSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.TransSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.TransSettings.iSortBy3Index;
                    this.cboSortBy4.SelectedIndex = this.TransSettings.iSortBy4Index;
                    this.MakeSQLSortBy();

                    this.dtpDate.Date = this.TransSettings.TransferDateFilter;
                    this.chkApplyDate.IsToggled = this.TransSettings.IsApplyTransferDateFilter;

                    bCancelChanges = false;
                    this.bChangesMade = false;
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "CancelChanges", Ex) == false)
                {
                }

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
                      "<transfer_date>" + this.TransSettings.TransferDateFilter.ToShortDateString() + "</transfer_date>" +
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
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                }
            }
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
            _ = await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}