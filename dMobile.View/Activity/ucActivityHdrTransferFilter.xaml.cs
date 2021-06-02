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
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrTransferFilter : DMobileModal, IAppArea
    {
        #region Fields

        private TransferSettings transSettings = new TransferSettings();
        private ArrayList m_aSortBy1 = new ArrayList();
        private ArrayList m_aSortBy2 = new ArrayList();
        private ArrayList m_aSortBy3 = new ArrayList();
        private ArrayList m_aSortBy4 = new ArrayList();
        private string m_strSQLSortBy = string.Empty;
        private bool m_bEnterTrapped = false;
        private bool isControlLoaded = false;
        private string m_strFullClassName = "dMobile.ucActivityHdrTransferFilter";
        private bool m_bChangesMade = false;
        private Dictionary<string, Sort> filters = new Dictionary<string, Sort>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ucCallingControl"></param>
        /// <remarks></remarks>
        public ucActivityHdrTransferFilter()
        {
            InitializeComponent();

            // TODO: Resource strings
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

            //At least one sort option must be selected
            // TODO: Resource strings
            aSortBy1.Clear();
            aSortBy1.Add("Transfer # Ascending");
            aSortBy1.Add("Transfer # Descending");
            aSortBy1.Add("Date Ascending");
            aSortBy1.Add("Date Descending");
            aSortBy1.Add("Whs # Ascending");
            aSortBy1.Add("Whs # Descending");
            aSortBy1.Add("Arrival Ascending");
            aSortBy1.Add("Arrival Descending");
            this.cboSortBy1.ItemsSource = this.aSortBy1;
            this.cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;

            // TODO: Resource strings
            aSortBy2.Clear();
            aSortBy2.Add(string.Empty);
            aSortBy2.Add("Transfer # Ascending");
            aSortBy2.Add("Transfer # Descending");
            aSortBy2.Add("Date Ascending");
            aSortBy2.Add("Date Descending");
            aSortBy2.Add("Whs # Ascending");
            aSortBy2.Add("Whs # Descending");
            aSortBy2.Add("Arrival Ascending");
            aSortBy2.Add("Arrival Descending");
            this.cboSortBy2.ItemsSource = this.aSortBy2;
            this.cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;

            // TODO: Resource strings
            aSortBy3.Clear();
            aSortBy3.Add(string.Empty);
            aSortBy3.Add("Transfer # Ascending");
            aSortBy3.Add("Transfer # Descending");
            aSortBy3.Add("Date Ascending");
            aSortBy3.Add("Date Descending");
            aSortBy3.Add("Whs # Ascending");
            aSortBy3.Add("Whs # Descending");
            aSortBy3.Add("Arrival Ascending");
            aSortBy3.Add("Arrival Descending");
            this.cboSortBy3.ItemsSource = this.aSortBy3;
            this.cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;

            // TODO: Resource strings
            aSortBy4.Clear();
            aSortBy4.Add(string.Empty);
            aSortBy4.Add("Transfer # Ascending");
            aSortBy4.Add("Transfer # Descending");
            aSortBy4.Add("Date Ascending");
            aSortBy4.Add("Date Descending");
            aSortBy4.Add("Whs # Ascending");
            aSortBy4.Add("Whs # Descending");
            aSortBy4.Add("Arrival Ascending");
            aSortBy4.Add("Arrival Descending");
            this.cboSortBy4.ItemsSource = this.aSortBy4;
            this.cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;

            this.filters.Add(string.Empty, null);
            this.filters.Add("Transfer # Ascending", new Sort() { SortValue = "ActivityTransferFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Transfer # Descending", new Sort() { SortValue = "ActivityTransferFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Date Ascending", new Sort() { SortValue = "ActivityTransferDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Date Descending", new Sort() { SortValue = "ActivityTransferDateTime", SortOrder = SortOrder.DESC });
            this.filters.Add("Whs # Ascending", new Sort() { SortValue = "LocationMapWarehouseFKRefno", SortOrder = SortOrder.ASC });
            this.filters.Add("Whs # Descending", new Sort() { SortValue = "LocationMapWarehouseFKRefno", SortOrder = SortOrder.DESC });
            this.filters.Add("Arrival Ascending", new Sort() { SortValue = "LocationMapArrivalDateTime", SortOrder = SortOrder.ASC });
            this.filters.Add("Arrival Descending", new Sort() { SortValue = "LocationMapArrivalDateTime", SortOrder = SortOrder.DESC });
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
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await CancelChanges();
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
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityFilter_GotFocus threw an error " + ex, "OK"));
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

        public ArrayList aSortBy4
        {
            get
            {
                return m_aSortBy4;
            }
            set
            {
                m_aSortBy4 = value;
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

        public TransferSettings TransSettings
        {
            get
            {
                return transSettings;
            }
            set
            {
                transSettings = value;
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

        private void MakeSQLSortBy()
        {
            strSQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].SortValue;
            TransSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]];

            if (cboSortBy2.SelectedIndex >= 0)
            {
                TransSettings.SortBy2Value = this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]];

                if (TransSettings.SortBy2Value != null && !string.IsNullOrEmpty(TransSettings.SortBy2Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.SortBy2Value;
                }
            }

            if (cboSortBy3.SelectedIndex >= 0)
            {
                TransSettings.SortBy3Value = this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]];

                if (TransSettings.SortBy3Value != null && !string.IsNullOrEmpty(TransSettings.SortBy3Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.SortBy3Value;
                }
            }

            if (cboSortBy4.SelectedIndex >= 0)
            {
                TransSettings.SortBy4Value = this.filters[cboSortBy4.Items[cboSortBy4.SelectedIndex]];

                if (TransSettings.SortBy4Value != null && !string.IsNullOrEmpty(TransSettings.SortBy4Value.SortValue))
                {
                    strSQLSortBy = strSQLSortBy + ", " + TransSettings.SortBy4Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            TransSettings = TransSettings.LoadData();

            if (SysCommon.IsEmptyDate(TransSettings.TransferDateFilter))
            {
                TransSettings.TransferDateFilter = DateTime.Now;
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                //Reload oTransferSettings because intial values will be scewed because of SelectedIndexChanged events firing
                if (!IsControlLoaded)
                {
                    LoadData();
                }

                chkApplyDate.IsToggled = TransSettings.IsApplyTransferDateFilter;
                dtpDate.Date = TransSettings.TransferDateFilter;

                cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;
                cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;
                cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;
                cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;
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
                if (await IsReadyToSave())
                {
                    if (chkApplyDate.IsToggled)
                    {
                        TransSettings.TransferDateFilter = dtpDate.Date;
                    }

                    TransSettings.IsApplyTransferDateFilter = chkApplyDate.IsToggled;

                    TransSettings.iSortBy1Index = cboSortBy1.SelectedIndex;
                    TransSettings.iSortBy2Index = cboSortBy2.SelectedIndex;
                    TransSettings.iSortBy3Index = cboSortBy3.SelectedIndex;
                    TransSettings.iSortBy4Index = cboSortBy4.SelectedIndex;
                    MakeSQLSortBy();

                    TransferSettings temp_oTransferSettings = TransSettings;
                    TransSettings.SaveData(ref temp_oTransferSettings);
                    TransSettings = temp_oTransferSettings;

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

                    cboSortBy1.SelectedIndex = TransSettings.iSortBy1Index;
                    cboSortBy2.SelectedIndex = TransSettings.iSortBy2Index;
                    cboSortBy3.SelectedIndex = TransSettings.iSortBy3Index;
                    cboSortBy4.SelectedIndex = TransSettings.iSortBy4Index;
                    MakeSQLSortBy();

                    dtpDate.Date = TransSettings.TransferDateFilter;
                    chkApplyDate.IsToggled = TransSettings.IsApplyTransferDateFilter;

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
        public async Task<XDocument> GetXML()
        {
            XDocument request = System.Xml.Linq.XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                      "<params>" +
                      ("<transfer_date>" + TransSettings.TransferDateFilter.ToShortDateString() + "</transfer_date>") +
                      ("<sortby>" + strSQLSortBy + "</sortby>") +
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
                if (IsControlLoaded)
                {
                    bChangesMade = true;
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
    }
}