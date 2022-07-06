// <copyright file="ActivityHdrProductionTicketFilter.xaml.cs" company="InterSect Business Systems Inc">
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
    public partial class ActivityHdrProductionTicketFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdrProductionTicketFilter"/> class.
        /// </summary>
        public ActivityHdrProductionTicketFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblApplyProductionDate.Text = "Apply Production Date Filter";
            this.lblAdjustmentDate.Text = "Date";
            this.lblSortBy.Text = "Sort By";
            this.lblSortBy1.Text = "1";
            this.lblSortBy2.Text = "2";
            this.lblSortBy3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            this.aSortBy1.Clear();
            _ = this.aSortBy1.Add("Production # Ascending");
            _ = this.aSortBy1.Add("Production # Descending");
            _ = this.aSortBy1.Add("Production Date Ascending");
            _ = this.aSortBy1.Add("Production Date Descending");
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.SelectedIndex = this.TicketSettings.iSortBy1Index;

            this.aSortBy2.Clear();
            _ = this.aSortBy2.Add(string.Empty);
            _ = this.aSortBy2.Add("Production # Ascending");
            _ = this.aSortBy2.Add("Production # Descending");
            _ = this.aSortBy2.Add("Production Date Ascending");
            _ = this.aSortBy2.Add("Production Date Descending");
            this.cboSortBy2.ItemsSource = this.aSortBy2;
            this.cboSortBy2.SelectedIndex = this.TicketSettings.iSortBy2Index;

            this.aSortBy3.Clear();
            _ = this.aSortBy3.Add(string.Empty);
            _ = this.aSortBy3.Add("Production # Ascending");
            _ = this.aSortBy3.Add("Production # Descending");
            _ = this.aSortBy3.Add("Production Date Ascending");
            _ = this.aSortBy3.Add("Production Date Descending");
            this.cboSortBy3.ItemsSource = this.aSortBy3;
            this.cboSortBy3.SelectedIndex = this.TicketSettings.iSortBy3Index;

            this.filters.Add("Production # Ascending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Production # Descending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Production Date Ascending", new Sort() { SortValue = "ActivityProductionDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Production Date Descending", new Sort() { SortValue = "ActivityProductionDateTime", SortOrder = SortOrder.DESC });
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
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            _ = await this.CancelChanges();
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityHdrProductionFilter_GotFocus();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdrProductionFilter_GotFocus threw an error " + ex, "OK"));
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
                else
                {
                    this.DialogResult = DialogResult.None;
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        public ProductionTicketSettings TicketSettings { get; set; } = new ProductionTicketSettings();

        public ArrayList aSortBy1 { get; set; } = new ArrayList();

        public ArrayList aSortBy2 { get; set; } = new ArrayList();

        public ArrayList aSortBy3 { get; set; } = new ArrayList();

        public bool bEnterTrapped { get; set; } = false;

        public bool IsControlLoaded { get; set; } = false;

        public string strSQLSortBy { get; set; } = string.Empty;

        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrProductionTicketFilter";

        public bool bChangesMade { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.TicketSettings = this.TicketSettings.LoadData();

            if (SysCommon.IsEmptyDate(this.TicketSettings.ProductionDateFilter))
            {
                this.TicketSettings.ProductionDateFilter = DateTime.Now;
            }

            this.IsControlLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.TicketSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.TicketSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.TicketSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.TicketSettings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.TicketSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.TicketSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.TicketSettings.SortBy3Value != null && !string.IsNullOrEmpty(this.TicketSettings.SortBy3Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.TicketSettings.SortBy3Value;
                }
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityHdrProductionFilter_GotFocus()
        {
            this.LoadData();
            this.dtpProductionDate.Date = this.TicketSettings.ProductionDateFilter;
            this.chkApplyProductionDate.IsToggled = this.TicketSettings.IsApplyProductionDateFilter;

            this.cboSortBy1.SelectedIndex = this.TicketSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.TicketSettings.iSortBy2Index;
            this.cboSortBy3.SelectedIndex = this.TicketSettings.iSortBy3Index;
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
            return "Activity Production Filter";
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
                      "<production_date>" + this.TicketSettings.ProductionDateFilter.ToShortDateString() + "</production_date>" +
                      "<sortby>" + this.strSQLSortBy + "</sortby>" +
                      "</params>");

            return request;
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

                    this.dtpProductionDate.Date = this.TicketSettings.ProductionDateFilter;
                    this.chkApplyProductionDate.IsToggled = this.TicketSettings.IsApplyProductionDateFilter;

                    this.cboSortBy1.SelectedIndex = this.TicketSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.TicketSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.TicketSettings.iSortBy3Index;
                    this.MakeSQLSortBy();

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
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyProductionDate.IsToggled)
                    {
                        this.TicketSettings.ProductionDateFilter = this.dtpProductionDate.Date;
                    }

                    this.TicketSettings.IsApplyProductionDateFilter = this.chkApplyProductionDate.IsToggled;

                    this.TicketSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.TicketSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.TicketSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    ProductionTicketSettings tempTicketSettings = this.TicketSettings;
                    _ = this.TicketSettings.SaveData(ref tempTicketSettings);
                    this.TicketSettings = tempTicketSettings;

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

        private void chkApplyProductionDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }

        private void dtpProductionDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.IdentifyChangesMade(sender, new EventArgs());
        }
    }
}