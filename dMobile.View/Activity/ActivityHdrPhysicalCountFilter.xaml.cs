// <copyright file="ActivityHdrPhysicalCountFilter.xaml.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ActivityHdrPhysicalCountFilter" /> Physical count filter options.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrPhysicalCountFilter : DMobileModal, IAppArea
    {

        /// <summary>
        /// Defines the filters.
        /// </summary>
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityHdrPhysicalCountFilter"/> class.
        /// </summary>
        public ActivityHdrPhysicalCountFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblSort.Text = "Sort By";
            this.lblApplyCountDate.Text = "Apply Physical Count Date Filter";
            this.lblDate.Text = "Count Date";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            this.aSortBy1.Clear();
            _ = this.aSortBy1.Add("Count # Ascending");
            _ = this.aSortBy1.Add("Count # Descending");
            _ = this.aSortBy1.Add("Count Date Ascending");
            _ = this.aSortBy1.Add("Count Date Descending");
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;

            this.aSortBy2.Clear();
            _ = this.aSortBy2.Add(string.Empty);
            _ = this.aSortBy2.Add("Count # Ascending");
            _ = this.aSortBy2.Add("Count # Descending");
            _ = this.aSortBy2.Add("Count Date Ascending");
            _ = this.aSortBy2.Add("Count Date Descending");
            this.cboSortBy2.ItemsSource = this.aSortBy2;
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

                if (!this.IsPageLoading)
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
                this.ucActivityHdrPhysicalCountFilter_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdrPhysicalCountFilter_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            _ = this.ScannerDisable();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
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
        /// Gets or sets the aSortBy1.
        /// </summary>
        public ArrayList aSortBy1 { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets the aSortBy2.
        /// </summary>
        public ArrayList aSortBy2 { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets a value indicating whether bEnterTrapped.
        /// </summary>
        public bool bEnterTrapped { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        public bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the CountSettings.
        /// </summary>
        public PhysicalCountSettings CountSettings { get; set; } = new PhysicalCountSettings();

        /// <summary>
        /// Gets or sets the strSQLSortBy.
        /// </summary>
        public string strSQLSortBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the strFullClassName.
        /// </summary>
        public string strFullClassName { get; set; } = "dMobile.ucActivityHdrPhysicalCountFilter";

        /// <summary>
        /// Gets or sets a value indicating whether bChangesMade.
        /// </summary>
        public bool bChangesMade { get; set; } = false;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ScannerDisable()
        {
            throw new NotImplementedException("Bar code scanning is not implemented here.");
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ScannerEnable()
        {
            throw new NotImplementedException("Bar code scanning is not implemented here.");
        }

        /// <summary>
        /// The ucActivityHdrPhysicalCountFilter_GotFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        public void ucActivityHdrPhysicalCountFilter_GotFocus(object sender, EventArgs e)
        {
            // Reload CountSettings because initial values will be screwed because of SelectedIndexChanged events firing
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;
            this.dtpCountDate.Date = this.CountSettings.dtCountDate;

            this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;
            this.MakeSQLSortBy();
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var IsReadyToSave = true;

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
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            return !this.bChangesMade;
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Physical Count Filter";
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var bSaveComplete = false;
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

                    var tempCountSettings = this.CountSettings;
                    _ = this.CountSettings.SaveData(ref tempCountSettings);
                    this.CountSettings = tempCountSettings;

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
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var bCancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();

                    this.chkApplyCountDate.IsToggled = this.CountSettings.IsApplyCountDate;
                    this.dtpCountDate.Date = this.CountSettings.dtCountDate;

                    this.cboSortBy1.SelectedIndex = this.CountSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.CountSettings.iSortBy2Index;
                    this.MakeSQLSortBy();

                    bCancelChanges = false;
                    this.bChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    _ = await errRpt.ShowDialog();
                });

                bCancelChanges = false;
            }

            return bCancelChanges;
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
        public async Task<XDocument> GetXML()
        {
            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "<count_date>" + this.CountSettings.dtCountDate.ToShortDateString() + "</count_date>" + "<sortby>" + this.strSQLSortBy + "</sortby>" + "</params>");

            return request;
        }

        /// <summary>
        /// The IdentifyChangesMade.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
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

        /// <summary>
        /// The MakeSQLSortBy.
        /// </summary>
        private void MakeSQLSortBy()
        {
            this.strSQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.CountSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.CountSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.CountSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.CountSettings.SortBy2Value.SortValue))
                {
                    this.strSQLSortBy = this.strSQLSortBy + ", " + this.CountSettings.SortBy2Value;
                }
            }
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
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

        /// <summary>
        /// The cboSortBy1_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void cboSortBy1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The cboSortBy2_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void cboSortBy2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The btnSave_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        /// The btnClose_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            _ = await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}
