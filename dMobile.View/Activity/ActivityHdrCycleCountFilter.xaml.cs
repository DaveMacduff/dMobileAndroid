// <copyright file="ActivityHdrCycleCountFilter.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
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

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrCycleCountFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityHdrCycleCountFilter"/> class.
        /// </summary>
        public ucActivityHdrCycleCountFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblApplyCountDate.Text = "Apply Date Filter";
            this.lblDate.Text = "Count Date";
            this.lblSort.Text = "Sort By";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // At least one sort option must be selected
            this.SortBy1.Clear();
            _ = this.SortBy1.Add("Count # Ascending");
            _ = this.SortBy1.Add("Count # Descending");
            _ = this.SortBy1.Add("Count Date Ascending");
            _ = this.SortBy1.Add("Count Date Descending");
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;

            this.SortBy2.Clear();
            _ = this.SortBy2.Add(string.Empty);
            _ = this.SortBy2.Add("Count # Ascending");
            _ = this.SortBy2.Add("Count # Descending");
            _ = this.SortBy2.Add("Count Date Ascending");
            _ = this.SortBy2.Add("Count Date Descending");
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;

            this.filters.Add(string.Empty, null);
            this.filters.Add("Count # Ascending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Count # Descending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Count Date Ascending", new Sort() { SortValue = "ActivityCountDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Count Date Descending", new Sort() { SortValue = "ActivityCountDateTime", SortOrder = SortOrder.DESC });
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
                this.ucActivityHdrCycleCountFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdrCycleCountFilter_GotFocus threw an error " + ex, "OK"));
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

        private ArrayList SortBy1 { get; set; } = new ArrayList();

        private ArrayList SortBy2 { get; set; } = new ArrayList();

        private bool EnterTrapped { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private CycleCountSettings CountSettings { get; set; } = new CycleCountSettings();

        private string SQLSortBy { get; set; } = string.Empty;

        private string FullClassName { get; set; } = "dMobile.ucActivityHdrCycleCountFilter";

        private bool ChangesMade { get; set; } = false;

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
        public void ucActivityHdrCycleCountFilter_GotFocus(object sender, EventArgs e)
        {
            // Reload oCycleCountSettings because intial values will be scewed because of SelectedIndexChanged events firing
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;
            this.dtpCountDate.Date = this.CountSettings.dtCountDate;

            this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "IsReadyToSave", ex) == false)
                {
                }

                isReadyToSave = false;
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
            return !this.ChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Cycle Count Filter";
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
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyCountDate.IsToggled)
                    {
                        this.CountSettings.dtCountDate = this.dtpCountDate.Date;
                    }

                    this.CountSettings.IsApplyCountDate = this.chkApplyCountDate.IsToggled;

                    this.CountSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.CountSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;

                    this.MakeSQLSortBy();

                    CycleCountSettings tempCountSettings = this.CountSettings;
                    _ = this.CountSettings.SaveData(ref tempCountSettings);
                    this.CountSettings = tempCountSettings;

                    saveComplete = true;
                    this.ChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "SaveChanges", ex) == false)
                {
                }

                saveComplete = false;
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
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();

                    this.dtpCountDate.Date = this.CountSettings.dtCountDate;
                    this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;

                    this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;
                    this.MakeSQLSortBy();

                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    _ = await errRpt.ShowDialog();
                });

                cancelChanges = false;
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
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "<count_date>" + this.CountSettings.dtCountDate.ToShortDateString() + "</count_date>" +
                "<sortby>" + this.SQLSortBy + "</sortby>" +
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
                    this.ChangesMade = true;
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "IdentifyChangesMade", ex) == false)
                {
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            // (ComboBoxMembers)this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex];
            this.SQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.CountSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.CountSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.CountSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.CountSettings.SortBy2Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.CountSettings.SortBy2Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.CountSettings = this.CountSettings.LoadData();

            // default count date to today's date if it is empty
            if (SysCommon.IsEmptyDate(this.CountSettings.dtCountDate))
            {
                this.CountSettings.dtCountDate = DateTime.Now;
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
            this.DialogResult = DialogResult.None;
            _ = await this.CancelChanges();
            await this.Close();
        }
    }
}