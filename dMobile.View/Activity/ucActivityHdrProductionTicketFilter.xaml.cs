using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.Resources.Types;
using dMobile.View.CustomPages.Modal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrProductionTicketFilter : DMobileModal, IAppArea
    {
        #region Fields

        private ProductionTicketSettings ticketSettings = new ProductionTicketSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private ArrayList m_aSortBy3 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrProductionTicketFilter";
        private bool m_bChangesMade = false;
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        #endregion

        #region Constructors

        public ucActivityHdrProductionTicketFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyProductionDate.Text = "Apply Production Date Filter";
            this.lblAdjustmentDate.Text = "Date";
            this.lblSortBy.Text = "Sort By";
            this.lblSortBy1.Text = "1";
            this.lblSortBy2.Text = "2";
            this.lblSortBy3.Text = "3";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            //At least one sort option must be selected
            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add("Production # Ascending");
            aSortBy1.Add("Production # Descending");
            aSortBy1.Add("Production Date Ascending");
            aSortBy1.Add("Production Date Descending");
            cboSortBy1.ItemsSource = aSortBy1;
            cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(string.Empty);
            aSortBy2.Add("Production # Ascending");
            aSortBy2.Add("Production # Descending");
            aSortBy2.Add("Production Date Ascending");
            aSortBy2.Add("Production Date Descending");
            cboSortBy2.ItemsSource = aSortBy2;
            cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;

            // TODO: Resource strings
            aSortBy3.Clear();
            aSortBy3.Add(string.Empty);
            aSortBy3.Add("Production # Ascending");
            aSortBy3.Add("Production # Descending");
            aSortBy3.Add("Production Date Ascending");
            aSortBy3.Add("Production Date Descending");
            cboSortBy3.ItemsSource = aSortBy3;
            cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;

            // TODO: Resource strings
            this.filters.Add("Production # Ascending", new Sort() { SortValue = "ActivityProductionFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Production # Descending", new Sort() { SortValue = "ActivityProductionFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Production Date Ascending", new Sort() { SortValue = "ActivityProductionDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Production Date Descending", new Sort() { SortValue = "ActivityProductionDateTime", SortOrder = SortOrder.DESC });
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
                        if (await SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await CancelChanges();
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
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdrProductionFilter_GotFocus threw an error " + ex, "OK"));
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

                if (await SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    await CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

        public ProductionTicketSettings TicketSettings
        {
            get
            {
                return ticketSettings;
            }
            set
            {
                ticketSettings = value;
            }
        }

        public ArrayList aSortBy1
        {
            get
            {
                return m_aSortBy1;
            }
            set
            {
                m_aSortBy1 = value;
            }
        }

        public ArrayList aSortBy2
        {
            get
            {
                return m_aSortBy2;
            }
            set
            {
                m_aSortBy2 = value;
            }
        }

        public ArrayList aSortBy3
        {
            get
            {
                return m_aSortBy3;
            }
            set
            {
                m_aSortBy3 = value;
            }
        }

        public bool bEnterTrapped
        {
            get
            {
                return m_bEnterTrapped;
            }
            set
            {
                m_bEnterTrapped = value;
            }
        }

        public bool IsControlLoaded
        {
            get
            {
                return isControlLoaded;
            }
            set
            {
                isControlLoaded = value;
            }
        }

        public string strSQLSortBy
        {
            get
            {
                return m_strSQLSortBy;
            }
            set
            {
                m_strSQLSortBy = value;
            }
        }

        public string strFullClassName
        {
            get
            {
                return m_strFullClassName;
            }
            set
            {
                m_strFullClassName = value;
            }
        }

        public bool bChangesMade
        {
            get
            {
                return m_bChangesMade;
            }
            set
            {
                m_bChangesMade = value;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.TicketSettings = TicketSettings.LoadData();

            if (SysCommon.IsEmptyDate(TicketSettings.ProductionDateFilter))
            {
                TicketSettings.ProductionDateFilter = DateTime.Now;
            }

            IsControlLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].SortValue;
            TicketSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]];

            if (cboSortBy2.SelectedIndex >= 0)
            {
                TicketSettings.SortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]];

                if (TicketSettings.SortBy2Value != null && !string.IsNullOrEmpty(TicketSettings.SortBy2Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TicketSettings.SortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                TicketSettings.SortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]];

                if (TicketSettings.SortBy3Value != null && !string.IsNullOrEmpty(TicketSettings.SortBy3Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TicketSettings.SortBy3Value;
                }
            }
        }

        #endregion

        #region Public Members

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
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                LoadData();
                dtpProductionDate.Date = TicketSettings.ProductionDateFilter;
                chkApplyProductionDate.IsToggled = TicketSettings.IsApplyProductionDateFilter;

                cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;
                cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;
            }
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
                if (SysCommon.ErrorReportAndRequestContinue(strFullClassName, "IsReadyToSave", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
            return !bChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Activity Production Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument request = System.Xml.Linq.XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                      "<params>" +
                      ("<production_date>" + TicketSettings.ProductionDateFilter.ToShortDateString() + "</production_date>") +
                      ("<sortby>" + strSQLSortBy + "</sortby>") +
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
                if (!await IsReadyToClose())
                {
                    LoadData();

                    dtpProductionDate.Date = TicketSettings.ProductionDateFilter;
                    chkApplyProductionDate.IsToggled = TicketSettings.IsApplyProductionDateFilter;

                    cboSortBy1.SelectedIndex = TicketSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = TicketSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = TicketSettings.iSortBy3Index;
                    MakeSQLSortBy();

                    bCancelChanges = false;
                    bChangesMade = false;
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "CancelChanges", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
                if (await IsReadyToSave())
                {
                    if (chkApplyProductionDate.IsToggled)
                    {
                        TicketSettings.ProductionDateFilter = dtpProductionDate.Date;
                    }

                    TicketSettings.IsApplyProductionDateFilter = chkApplyProductionDate.IsToggled;

                    TicketSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    TicketSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    TicketSettings.iSortBy3Index = cboSortBy3.SelectedIndex;
                    MakeSQLSortBy();

                    ProductionTicketSettings tempTicketSettings = TicketSettings;
                    TicketSettings.SaveData(ref tempTicketSettings);
                    TicketSettings = tempTicketSettings;

                    bSaveComplete = true;
                    bChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK"));
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "SaveChanges", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
                if (isControlLoaded)
                {
                    m_bChangesMade = true;
                }
            }
            catch (Exception Ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }
        }

        #endregion

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (await SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            await CancelChanges();
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