// <copyright file="ActivityHdrAdjustmentFilter.xaml.cs" company="InterSect Business Systems Inc">
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
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrAdjustmentFilter : DMobileModal, IAppArea
    {
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();
        private Dictionary<string, string> adjustmentTypeFilters = new Dictionary<string, string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="callingControl">Control that called this one.</param>
        public ucActivityHdrAdjustmentFilter()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblSortBy.Text = "Sort By";
            this.lblApplyAdjustmentType.Text = "Apply Adjustment Type Filter";
            this.lblApplyAdjustmentDate.Text = "Apply Adjustment Date Filter";
            this.lblAdjustmentDate.Text = "Date";
            this.lblAdjustmentType.Text = "Type";
            this.lblNegateScanQuantity.Text = "Negate Scan Quantity";
            this.lblSortBy1.Text = "1";
            this.lblSortBy2.Text = "2";
            this.lblSortBy3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            this.FillAdjustmentTypeComboBox();

            this.cboAdjustmentType.ItemsSource = this.AdjTypes;
            this.cboAdjustmentType.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboAdjustmentType.SelectedIndex = Convert.ToInt32(this.AdjSettings.AdjustmentType);

            // At least one sort option must be selected
            this.SortBy1.Clear();
            _ = this.SortBy1.Add("Adjustment # Ascending");
            _ = this.SortBy1.Add("Adjustment # Descending");
            _ = this.SortBy1.Add("Adjustment Type # Ascending");
            _ = this.SortBy1.Add("Adjustment Type # Descending");
            _ = this.SortBy1.Add("Adjustment Date Ascending");
            _ = this.SortBy1.Add("Adjustment Date Descending");
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.SelectedIndex = this.AdjSettings.SortBy1Index;

            this.SortBy2.Clear();
            _ = this.SortBy2.Add(string.Empty);
            _ = this.SortBy2.Add("Adjustment # Ascending");
            _ = this.SortBy2.Add("Adjustment # Descending");
            _ = this.SortBy2.Add("Adjustment Type # Ascending");
            _ = this.SortBy2.Add("Adjustment Type # Descending");
            _ = this.SortBy2.Add("Adjustment Date Ascending");
            _ = this.SortBy2.Add("Adjustment Date Descending");
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.SelectedIndex = this.AdjSettings.SortBy2Index;

            this.SortBy3.Clear();
            _ = this.SortBy3.Add(string.Empty);
            _ = this.SortBy3.Add("Adjustment # Ascending");
            _ = this.SortBy3.Add("Adjustment # Descending");
            _ = this.SortBy3.Add("Adjustment Type # Ascending");
            _ = this.SortBy3.Add("Adjustment Type # Descending");
            _ = this.SortBy3.Add("Adjustment Date Ascending");
            _ = this.SortBy3.Add("Adjustment Date Descending");
            this.cboSortBy3.ItemsSource = this.SortBy3;
            this.cboSortBy3.SelectedIndex = this.AdjSettings.SortBy3Index;

            this.filters.Clear();
            this.filters.Add(string.Empty, null);
            this.filters.Add("Adjustment # Ascending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Adjustment # Descending", new Sort() { SortValue = "ActivityFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Adjustment Type # Ascending", new Sort() { SortValue = "ActivityAdjustmentType", SortOrder = SortOrder.ASC });
            this.filters.Add("Adjustment Type # Descending", new Sort() { SortValue = "ActivityAdjustmentType", SortOrder = SortOrder.DESC });
            this.filters.Add("Adjustment Date Ascending", new Sort() { SortValue = "ActivityAdjustmentDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Adjustment Date Descending", new Sort() { SortValue = "ActivityAdjustmentDateTime", SortOrder = SortOrder.DESC });
        }

        /// <summary>
        /// Adds values to the adjustment type combo box.
        /// </summary>
        private void FillAdjustmentTypeComboBox()
        {
            // KLUDGE: these choices must be in the same order as the ActivityAdjustmentTypes definition.
            // The clsAdjustmentActivity.SelectActivity Method set the oAdjustmentSettings.iAdjustmentTypeIdx property
            // based on the enumeration value
            this.AdjTypes.Clear();
            _ = this.AdjTypes.Add(new ComboBoxMembers("Inventory", Convert.ToInt32(SysCommon.AdjTypes.Inventory).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Unit Converstion", Convert.ToInt32(SysCommon.AdjTypes.UnitConversion).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Rework", Convert.ToInt32(SysCommon.AdjTypes.Rework).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Partial Rework", Convert.ToInt32(SysCommon.AdjTypes.PartialRework).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Production", Convert.ToInt32(SysCommon.AdjTypes.Production).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Partial Production", Convert.ToInt32(SysCommon.AdjTypes.PartialProduction).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Disposal", Convert.ToInt32(SysCommon.AdjTypes.Disposal).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Fresh To Frozen", Convert.ToInt32(SysCommon.AdjTypes.Fresh2Frozen).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Transfer", Convert.ToInt32(SysCommon.AdjTypes.Transfer).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Pack Off", Convert.ToInt32(SysCommon.AdjTypes.PackOff).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Lost & Found", Convert.ToInt32(SysCommon.AdjTypes.LostFound).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Cycle Count", Convert.ToInt32(SysCommon.AdjTypes.CycleCount).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Move To Deli", Convert.ToInt32(SysCommon.AdjTypes.MoveToDeli).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Periodic Count", Convert.ToInt32(SysCommon.AdjTypes.PeriodicCount).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Manufacture", Convert.ToInt32(SysCommon.AdjTypes.Manufacture).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Requisition", Convert.ToInt32(SysCommon.AdjTypes.Requisition).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Auto Convert To", Convert.ToInt32(SysCommon.AdjTypes.AutoConvertTo).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Auto Create From", Convert.ToInt32(SysCommon.AdjTypes.AutoCreateFrom).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Skid Putaway", Convert.ToInt32(SysCommon.AdjTypes.SkidPutaway).ToString()));

            // <WO ID="14748" comment="Added convert product due to aforementioned KLUDGE at top of function. Having to have same order of enumerations. So to play it safe I put debox and convert product in.>
            _ = this.AdjTypes.Add(new ComboBoxMembers("Convert Product", Convert.ToInt32(SysCommon.AdjTypes.ConvertProduct).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Debox", Convert.ToInt32(SysCommon.AdjTypes.Debox).ToString()));
            _ = this.AdjTypes.Add(new ComboBoxMembers("Convert Skid", Convert.ToInt32(SysCommon.AdjTypes.ConvertSkid).ToString()));
            // </WO>
            this.adjustmentTypeFilters.Clear();
            this.adjustmentTypeFilters.Add("Inventory", Convert.ToInt32(SysCommon.AdjTypes.Inventory).ToString());
            this.adjustmentTypeFilters.Add("Unit Converstion", Convert.ToInt32(SysCommon.AdjTypes.UnitConversion).ToString());
            this.adjustmentTypeFilters.Add("Rework", Convert.ToInt32(SysCommon.AdjTypes.Rework).ToString());
            this.adjustmentTypeFilters.Add("Partial Rework", Convert.ToInt32(SysCommon.AdjTypes.PartialRework).ToString());
            this.adjustmentTypeFilters.Add("Production", Convert.ToInt32(SysCommon.AdjTypes.Production).ToString());
            this.adjustmentTypeFilters.Add("Partial Production", Convert.ToInt32(SysCommon.AdjTypes.PartialProduction).ToString());
            this.adjustmentTypeFilters.Add("Disposal", Convert.ToInt32(SysCommon.AdjTypes.Disposal).ToString());
            this.adjustmentTypeFilters.Add("Fresh To Frozen", Convert.ToInt32(SysCommon.AdjTypes.Fresh2Frozen).ToString());
            this.adjustmentTypeFilters.Add("Transfer", Convert.ToInt32(SysCommon.AdjTypes.Transfer).ToString());
            this.adjustmentTypeFilters.Add("Pack Off", Convert.ToInt32(SysCommon.AdjTypes.PackOff).ToString());
            this.adjustmentTypeFilters.Add("Lost & Found", Convert.ToInt32(SysCommon.AdjTypes.LostFound).ToString());
            this.adjustmentTypeFilters.Add("Cycle Count", Convert.ToInt32(SysCommon.AdjTypes.CycleCount).ToString());
            this.adjustmentTypeFilters.Add("Move To Deli", Convert.ToInt32(SysCommon.AdjTypes.MoveToDeli).ToString());
            this.adjustmentTypeFilters.Add("Periodic Count", Convert.ToInt32(SysCommon.AdjTypes.PeriodicCount).ToString());
            this.adjustmentTypeFilters.Add("Manufacture", Convert.ToInt32(SysCommon.AdjTypes.Manufacture).ToString());
            this.adjustmentTypeFilters.Add("Requisition", Convert.ToInt32(SysCommon.AdjTypes.Requisition).ToString());
            this.adjustmentTypeFilters.Add("Auto Convert To", Convert.ToInt32(SysCommon.AdjTypes.AutoConvertTo).ToString());
            this.adjustmentTypeFilters.Add("Auto Create From", Convert.ToInt32(SysCommon.AdjTypes.AutoCreateFrom).ToString());
            this.adjustmentTypeFilters.Add("Skid Putaway", Convert.ToInt32(SysCommon.AdjTypes.SkidPutaway).ToString());
            this.adjustmentTypeFilters.Add("Convert Product", Convert.ToInt32(SysCommon.AdjTypes.ConvertProduct).ToString());
            this.adjustmentTypeFilters.Add("Debox", Convert.ToInt32(SysCommon.AdjTypes.Debox).ToString());
            this.adjustmentTypeFilters.Add("Convert Skid", Convert.ToInt32(SysCommon.AdjTypes.ConvertSkid).ToString());

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
                            _ = await this.CancelChanges();
                            this.DialogResult = DialogResult.None;
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityHdrAdjustmentFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityHdrAdjustmentFilter_GotFocus threw an error " + ex, "OK"));
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
                    _ = await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        public ArrayList AdjTypes { get; set; } = new ArrayList();

        public ArrayList SortBy1 { get; set; } = new ArrayList();

        public ArrayList SortBy2 { get; set; } = new ArrayList();

        public ArrayList SortBy3 { get; set; } = new ArrayList();

        public bool EnterTrapped { get; set; } = false;

        public bool IsControlLoaded { get; set; } = false;

        public AdjustmentSettings AdjSettings { get; set; } = new AdjustmentSettings();

        public string SQLSortBy { get; set; } = string.Empty;

        public string FullClassName { get; set; } = "dMobile.ucActivityHdrAdjustmentFilter";

        public bool ChangesMade { get; set; } = false;

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
        public void ucActivityHdrAdjustmentFilter_GotFocus(object sender, EventArgs e)
        {
            // Reload oAdjustmentSettings because intial values will be scewed because of SelectedIndexChanged events firing
            if (!this.IsControlLoaded)
            {
                this.LoadData();
            }

            this.chkApplyAdjustmentType.IsToggled = this.AdjSettings.IsApplyAdjustmentType;
            this.cboAdjustmentType.SelectedIndex = (int)this.AdjSettings.AdjustmentType;

            this.chkApplyAdjustmentDate.IsToggled = this.AdjSettings.IsApplyAdjustmentDate;
            this.dtpAdjustmentDate.Date = this.AdjSettings.AdjustmentDate;

            this.chkNegateScanQuantity.IsEnabled = SysCommon.IsAllowedChangeNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
            this.chkNegateScanQuantity.IsToggled = SysCommon.DefaultNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);

            this.cboSortBy1.SelectedIndex = this.AdjSettings.SortBy1Index;
            this.cboSortBy2.SelectedIndex = this.AdjSettings.SortBy2Index;
            this.cboSortBy3.SelectedIndex = this.AdjSettings.SortBy3Index;
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
            return "dMobile Activity Adjustment Filter";
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
                "<adjustment_date>" + this.AdjSettings.AdjustmentDate.ToShortDateString() + "</adjustment_date>" +
                "<adjustment_type>" + Convert.ToInt32(this.AdjSettings.AdjustmentType) + "</adjustment_type>" +
                "<negate_scan_quantity>" + this.AdjSettings.NegateScanQuantity + "</negate_scan_quantity>" +
                "<sortby>" + this.SQLSortBy + "</sortby>" + "</params>");

            return request;
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

                    this.chkApplyAdjustmentType.IsToggled = this.AdjSettings.IsApplyAdjustmentType;

                    // oAdjustmentSettings.eAdjustmentType is assigned in the cboAdjustmentType_SelectedIndexChanged event
                    this.cboAdjustmentType.SelectedIndex = Convert.ToInt32(this.AdjSettings.AdjustmentType);

                    this.chkApplyAdjustmentDate.IsToggled = this.AdjSettings.IsApplyAdjustmentDate;
                    this.dtpAdjustmentDate.Date = this.AdjSettings.AdjustmentDate;

                    this.chkNegateScanQuantity.IsToggled = this.AdjSettings.NegateScanQuantity;

                    this.cboSortBy1.SelectedIndex = this.AdjSettings.SortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.AdjSettings.SortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.AdjSettings.SortBy3Index;
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
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.chkApplyAdjustmentType.IsToggled && this.cboAdjustmentType.SelectedIndex > -1)
                    {
                        this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)Convert.ToInt32(this.adjustmentTypeFilters[this.cboAdjustmentType.Items[this.cboAdjustmentType.SelectedIndex]]);
                    }

                    this.AdjSettings.IsApplyAdjustmentType = this.chkApplyAdjustmentType.IsToggled;
                    this.AdjSettings.NegateScanQuantity = this.chkNegateScanQuantity.IsToggled;
                    this.AdjSettings.IsApplyAdjustmentDate = this.chkApplyAdjustmentDate.IsToggled;

                    if (this.chkApplyAdjustmentDate.IsToggled)
                    {
                        this.AdjSettings.AdjustmentDate = this.dtpAdjustmentDate.Date;
                    }

                    this.AdjSettings.SortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.AdjSettings.SortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.AdjSettings.SortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.MakeSQLSortBy();

                    AdjustmentSettings tempAdjSettings = this.AdjSettings;
                    _ = this.AdjSettings.SaveData(ref tempAdjSettings);
                    this.AdjSettings = tempAdjSettings;

                    saveComplete = true;
                    this.ChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK"));
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void cboAdjustmentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    // Did not move this into SaveData because there is not way to assign oAdjustmentSetting.eAdjustmentType
                    // to the binding context in the LoadData or CancelData method easily for a default value
                    // this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)(Convert.ToInt32(((ComboBoxMembers)(this.cboAdjustmentType.Items[this.cboAdjustmentType.SelectedIndex])).ValueMember));
                    this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)Convert.ToInt32(this.adjustmentTypeFilters[this.cboAdjustmentType.Items[this.cboAdjustmentType.SelectedIndex]]);
                    this.chkNegateScanQuantity.IsToggled = SysCommon.DefaultNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
                    this.chkNegateScanQuantity.IsEnabled = SysCommon.IsAllowedChangeNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType);
                    this.ChangesMade = true;
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "cboAdjustmentType_SelectedIndexChanged", ex) == false)
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
            // ((ComboBoxMembers)this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]).ValueMember.Trim();
            this.SQLSortBy = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]].SortValue;
            this.AdjSettings.SortBy1Value = this.filters[this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.AdjSettings.SortBy2Value = this.filters[this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex]];

                if (this.AdjSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.AdjSettings.SortBy2Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.AdjSettings.SortBy2Value.SortValue;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.AdjSettings.SortBy3Value = this.filters[this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex]];

                if (this.AdjSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.AdjSettings.SortBy3Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.AdjSettings.SortBy3Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.AdjSettings = this.AdjSettings.LoadData();

            // default adjustment date to today's date if it is empty
            if (SysCommon.IsEmptyDate(this.AdjSettings.AdjustmentDate))
            {
                this.AdjSettings.AdjustmentDate = DateTime.Now;
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
            _ = await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}