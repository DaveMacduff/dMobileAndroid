using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;
using dMobile.Bedrock;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrPhysicalCountFilter : DMobileModal, IAppArea
    {
        #region Fields

        private PhysicalCountSettings countSettings = new PhysicalCountSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrPhysicalCountFilter";
        private bool m_bChangesMade = false;
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        #endregion

        #region Constructors

        public ucActivityHdrPhysicalCountFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblSort.Text = "Sort By";
            this.lblApplyCountDate.Text = "Apply Physical Count Date Filter";
            this.lblDate.Text = "Count Date";
            this.lbl1.Text = "1";
            this.lbl2.Text = "2";
            this.btnSave.Text = "Save";
            this.btnClose.Text = "Close";

            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add("Count # Ascending");
            aSortBy1.Add("Count # Descending");
            aSortBy1.Add("Count Date Ascending");
            aSortBy1.Add("Count Date Descending");
            cboSortBy1.ItemsSource = aSortBy1;
            cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(string.Empty);
            aSortBy2.Add("Count # Ascending");
            aSortBy2.Add("Count # Descending");
            aSortBy2.Add("Count Date Ascending");
            aSortBy2.Add("Count Date Descending");
            cboSortBy2.ItemsSource = aSortBy2;
            cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;

            filters.Add(string.Empty, null);
            filters.Add("Count # Ascending", new Sort() { SortValue = "ActivityCountFKRefno", SortOrder = SortOrder.ASC });
            filters.Add("Count # Descending", new Sort() { SortValue = "ActivityCountFKRefno", SortOrder = SortOrder.DESC });
            filters.Add("Count Date Ascending", new Sort() { SortValue = "ActivityCountDateTime", SortOrder = SortOrder.ASC });
            filters.Add("Count Date Descending", new Sort() { SortValue = "ActivityCountDateTime", SortOrder = SortOrder.DESC });
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
                this.ucActivityHdrPhysicalCountFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdrPhysicalCountFilter_GotFocus threw an error " + ex, "OK"));
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

        public PhysicalCountSettings CountSettings
        {
            get
            {
                return countSettings;
            }
            set
            {
                countSettings = value;
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
        private void MakeSQLSortBy()
        {
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].SortValue;
            CountSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]];

            if (cboSortBy2.SelectedIndex >= 0)
            {
                CountSettings.SortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]];

                if (CountSettings.SortBy2Value != null && !string.IsNullOrEmpty(CountSettings.SortBy2Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + CountSettings.SortBy2Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            CountSettings = CountSettings.LoadData();

            // default count date to today's date if it is empty
            if (SysCommon.IsEmptyDate(CountSettings.dtCountDate))
            {
                CountSettings.dtCountDate = DateTime.Now;
            }

            IsControlLoaded = true;
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

        public void ucActivityHdrPhysicalCountFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload CountSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!IsControlLoaded)
                {
                    LoadData();
                }

                chkApplyCountDate.IsToggled = CountSettings.IsApplyCountDate;
                dtpCountDate.Date = CountSettings.dtCountDate;

                cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;
                MakeSQLSortBy();
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
            return "Physical Count Filter";
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
                    if (chkApplyCountDate.IsToggled)
                    {
                        CountSettings.dtCountDate = dtpCountDate.Date;
                    }

                    CountSettings.IsApplyCountDate = chkApplyCountDate.IsToggled;

                    CountSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    CountSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    MakeSQLSortBy();

                    PhysicalCountSettings tempCountSettings = CountSettings;
                    CountSettings.SaveData(ref tempCountSettings);
                    CountSettings = tempCountSettings;

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
                if (!await IsReadyToClose())
                {
                    LoadData();

                    chkApplyCountDate.IsToggled = CountSettings.IsApplyCountDate;
                    dtpCountDate.Date = CountSettings.dtCountDate;

                    cboSortBy1.SelectedIndex = CountSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = CountSettings.iSortBy2Index;
                    MakeSQLSortBy();

                    bCancelChanges = false;
                    bChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.NotApplicable, ex);
                    await errRpt.ShowDialog();
                });

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
            XDocument request = System.Xml.Linq.XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<count_date>" + CountSettings.dtCountDate.ToShortDateString() + "</count_date>") + ("<sortby>" + strSQLSortBy + "</sortby>") + "</params>");

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
                if (isControlLoaded)
                {
                    m_bChangesMade = true;
                }
            }
            catch (Exception Ex)
            {

                if (SysCommon.ErrorReportAndRequestContinue(m_strFullClassName, "IdentifyChangesMade", Ex) == false)
                {
                }
            }
        }

        #endregion

        private void cboSortBy1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboSortBy2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

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
            await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}