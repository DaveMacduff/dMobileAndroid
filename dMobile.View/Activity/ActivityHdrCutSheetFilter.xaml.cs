// <copyright file="ActivityHdrCutSheetFilter.xaml.cs" company="InterSect Business Systems Inc">
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

    /// <summary>
    /// Header cut sheet filter modal dialog.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrCutSheetFilter : DMobileModal, IAppArea
    {
        private readonly Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrCutSheetFilter" /> class.
        /// </summary>
        public ActivityHdrCutSheetFilter()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.lblApplyCutSheetDate.Text = "Apply CutSheet Date Filter";
            this.lblAdjustmentDate.Text = "Date";
            this.lblSortBy.Text = "Sort By";
            this.lblSortBy1.Text = "1";
            this.lblSortBy2.Text = "2";
            this.lblSortBy3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            this.aSortBy1.Clear();
            _ = this.aSortBy1.Add("CutSheet # Ascending");
            _ = this.aSortBy1.Add("CutSheet # Descending");
            _ = this.aSortBy1.Add("CutSheet Date Ascending");
            _ = this.aSortBy1.Add("CutSheet Date Descending");
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.SelectedIndex = this.Settings.SortBy1Index;

            this.aSortBy2.Clear();
            _ = this.aSortBy2.Add(string.Empty);
            _ = this.aSortBy2.Add("CutSheet # Ascending");
            _ = this.aSortBy2.Add("CutSheet # Descending");
            _ = this.aSortBy2.Add("CutSheet Date Ascending");
            _ = this.aSortBy2.Add("CutSheet Date Descending");
            this.cboSortBy2.ItemsSource = this.aSortBy2;
            this.cboSortBy2.SelectedIndex = this.Settings.SortBy2Index;

            this.aSortBy3.Clear();
            _ = this.aSortBy3.Add(string.Empty);
            _ = this.aSortBy3.Add("CutSheet # Ascending");
            _ = this.aSortBy3.Add("CutSheet # Descending");
            _ = this.aSortBy3.Add("CutSheet Date Ascending");
            _ = this.aSortBy3.Add("CutSheet Date Descending");
            this.cboSortBy3.ItemsSource = this.aSortBy3;
            this.cboSortBy3.SelectedIndex = this.Settings.SortBy3Index;

            this.filters.Add("CutSheet # Ascending", new Sort
                { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("CutSheet # Descending", new Sort
                { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("CutSheet Date Ascending", new Sort
                { SortValue = "ActivityCutSheetDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("CutSheet Date Descending", new Sort
                { SortValue = "ActivityCutSheetDateTime", SortOrder = SortOrder.DESC });
        }

        public CutSheetSettings Settings { get; set; } = new CutSheetSettings();

        public ArrayList aSortBy1 { get; set; } = new ArrayList();

        public ArrayList aSortBy2 { get; set; } = new ArrayList();

        public ArrayList aSortBy3 { get; set; } = new ArrayList();

        public bool bEnterTrapped { get; set; } = false;

        public bool IsControlLoaded { get; set; }

        public string strSQLSortBy { get; set; } = string.Empty;

        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrCutSheetFilter";

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
            var isReadyToSave = true;

            try
            {
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IsReadyToSave", ex) == false)
                {
                }

                isReadyToSave = false;
            }

            return Task.FromResult(isReadyToSave);
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
        public Task<XDocument> GetXml()
        {
            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                          "<params>" +
                                          "<production_date>" +
                                          this.Settings.CutSheetDateFilter.ToShortDateString() +
                                          "</production_date>" +
                                          "<sortby>" + this.strSQLSortBy + "</sortby>" +
                                          "</params>");

            return Task.FromResult(request);
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

                    this.dtpCutSheetDate.Date = this.Settings.CutSheetDateFilter;
                    this.chkApplyCutSheetDate.IsToggled = this.Settings.IsApplyCutSheetDateFilter;

                    this.cboSortBy1.SelectedIndex = this.Settings.SortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.Settings.SortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.Settings.SortBy3Index;
                    this.MakeSQLSortBy();

                    bCancelChanges = false;
                    this.bChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "CancelChanges", ex) == false)
                {
                }

                bCancelChanges = false;
            }

            return bCancelChanges;
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
                    if (this.chkApplyCutSheetDate.IsToggled)
                    {
                        this.Settings.CutSheetDateFilter = this.dtpCutSheetDate.Date;
                    }

                    this.Settings.IsApplyCutSheetDateFilter = this.chkApplyCutSheetDate.IsToggled;

                    this.Settings.SortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.Settings.SortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.Settings.SortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    var tempSettings = this.Settings;
                    _ = this.Settings.SaveData(ref tempSettings);
                    this.Settings = tempSettings;

                    bSaveComplete = true;
                    this.bChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "SaveChanges", ex) == false)
                {
                }

                bSaveComplete = false;
            }

            return bSaveComplete;
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

                        if (!this.IsIndicatorRunning && keycode == Keycode.F1)
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
                    });

                    this.ucActivityHdrCutSheetFilter_GotFocus();
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                        "this.ucActivityHdrCutSheetFilter_GotFocus threw an error " + ex, "OK"));
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
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
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
        private void LoadData()
        {
            this.Settings = this.Settings.LoadData();

            if (SysCommon.IsEmptyDate(this.Settings.CutSheetDateFilter))
            {
                this.Settings.CutSheetDateFilter = DateTime.Now;
            }

            this.IsControlLoaded = true;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.Settings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.Settings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.Settings.SortBy2Value != null &&
                    !string.IsNullOrEmpty(this.Settings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.Settings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.Settings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.Settings.SortBy3Value != null &&
                    !string.IsNullOrEmpty(this.Settings.SortBy3Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.Settings.SortBy3Value;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityHdrCutSheetFilter_GotFocus()
        {
            this.LoadData();
            this.dtpCutSheetDate.Date = this.Settings.CutSheetDateFilter;
            this.chkApplyCutSheetDate.IsToggled = this.Settings.IsApplyCutSheetDateFilter;

            this.cboSortBy1.SelectedIndex = this.Settings.SortBy1Index;
            this.cboSortBy2.SelectedIndex = this.Settings.SortBy2Index;
            this.cboSortBy3.SelectedIndex = this.Settings.SortBy3Index;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Activity CutSheet Filter";
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
                if (this.IsControlLoaded)
                {
                    this.bChangesMade = true;
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.strFullClassName, "IdentifyChangesMade", ex) == false)
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

        private void chkApplyCutSheetDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }

        private void dtpCutSheetDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.IdentifyChangesMade(sender, EventArgs.Empty);
        }
    }
}