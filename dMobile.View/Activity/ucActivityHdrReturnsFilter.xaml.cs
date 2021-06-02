using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.CustomPages.Modal;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrReturnsFilter : DMobileModal, IAppArea
    {
        #region Fields

        private ReturnsSettings retSettings = new ReturnsSettings();
        private ArrayList sortBy1 = new ArrayList();
        private ArrayList sortBy2 = new ArrayList();
        private ArrayList sortBy3 = new ArrayList();
        private string sqlSortBy = string.Empty;
        private bool isEnterTrapped = false;
        private bool isControlLoaded = false;
        private string fullClassName = "dMobile.ucActivityHdrReturnsFilter";
        private bool isChangesMade = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.Returns;
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        public ucActivityHdrReturnsFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
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
            // TODO: Resource strings
            this.SortBy1.Clear();
            this.SortBy1.Add("Order # Ascending");
            this.SortBy1.Add("Order # Descending");
            this.SortBy1.Add("Entered Date Ascending");
            this.SortBy1.Add("Entered Date Descending");
            this.SortBy1.Add("Returned Ascending");
            this.SortBy1.Add("Returned Descending");
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.SelectedIndex = this.RetSettings.iSortBy1Index;

            // TODO: Resource strings
            this.SortBy2.Clear();
            this.SortBy2.Add(string.Empty);
            this.SortBy2.Add("Order # Ascending");
            this.SortBy2.Add("Order # Descending");
            this.SortBy2.Add("Entered Date Ascending");
            this.SortBy2.Add("Entered Date Descending");
            this.SortBy2.Add("Returned Ascending");
            this.SortBy2.Add("Returned Descending");
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.SelectedIndex = this.RetSettings.iSortBy2Index;

            // TODO: Resource strings
            this.SortBy3.Clear();
            this.SortBy3.Add(string.Empty);
            this.SortBy3.Add("Order # Ascending");
            this.SortBy3.Add("Order # Descending");
            this.SortBy3.Add("Entered Date Ascending");
            this.SortBy3.Add("Entered Date Descending");
            this.SortBy3.Add("Returned Ascending");
            this.SortBy3.Add("Returned Descending");
            this.cboSortBy3.ItemsSource = this.SortBy3;
            this.cboSortBy3.SelectedIndex = this.RetSettings.iSortBy3Index;

            filters.Add(string.Empty, null);
            this.filters.Add("Order # Ascending", new Sort() { SortValue = "ActivityReturnsFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Order # Descending", new Sort() { SortValue = "ActivityReturnsFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Entered Date Ascending", new Sort() { SortValue = "ActivityReturnsOrderedDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Entered Date Descending", new Sort() { SortValue = "ActivityReturnsOrderedDateTime", SortOrder = SortOrder.DESC });
            this.filters.Add("Returned Ascending", new Sort() { SortValue = "ActivityReturnsDeliveryDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Returned Descending", new Sort() { SortValue = "ActivityReturnsDeliveryDateTime", SortOrder = SortOrder.DESC });
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

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
                this.ucActivityReturnsFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityReturnsFilter_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
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
                base.OnBackButtonPressed();

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

        #endregion

        #region Properties

        private ArrayList SortBy1
        {
            get
            {
                return this.sortBy1;
            }

            set
            {
                this.sortBy1 = value;
            }
        }

        private ArrayList SortBy2
        {
            get
            {
                return this.sortBy2;
            }

            set
            {
                this.sortBy2 = value;
            }
        }

        private ArrayList SortBy3
        {
            get
            {
                return this.sortBy3;
            }

            set
            {
                this.sortBy3 = value;
            }
        }

        private bool IsEnterTrapped
        {
            get
            {
                return this.isEnterTrapped;
            }

            set
            {
                this.isEnterTrapped = value;
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private ReturnsSettings RetSettings
        {
            get
            {
                return this.retSettings;
            }

            set
            {
                this.retSettings = value;
            }
        }

        private string SQLSortBy
        {
            get
            {
                return this.sqlSortBy;
            }

            set
            {
                this.sqlSortBy = value;
            }
        }

        private string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        private bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        #endregion

        #region Public Members

        #region IAppArea Members

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
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            bool isReadyToClose = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            // TODO: Resource strings
            return "Returns Filter";
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

                    ReturnsSettings tempRetSettings = this.RetSettings;
                    this.RetSettings.SaveData(ref tempRetSettings);
                    this.RetSettings = tempRetSettings;

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            bool cancelChanges = true;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<returned_date>" + this.RetSettings.ReturnedDateFilter.ToShortDateString() + "</returned_date>")
                + ("<entered_date>" + this.RetSettings.EnteredDateFilter.ToShortDateString() + "</entered_date>")
                + ("<sortby>" + this.SQLSortBy + "</sortby>")
                + "</params>");

            return request;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityReturnsFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
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
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            this.SQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].SortValue;
            this.RetSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]];

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.RetSettings.SortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]];

                if (this.RetSettings.SortBy2Value != null && !string.IsNullOrEmpty(this.RetSettings.SortBy2Value.SortValue))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.RetSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.RetSettings.SortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]];

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

        #endregion
    }
}