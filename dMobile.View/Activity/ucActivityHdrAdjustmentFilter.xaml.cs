using Android.Views;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
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
    public partial class ucActivityHdrAdjustmentFilter : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private AdjustmentSettings adjSettings = new AdjustmentSettings();
        private ArrayList sortBy1 = new ArrayList();
        private ArrayList sortBy2 = new ArrayList();
        private ArrayList sortBy3 = new ArrayList();
        private ArrayList adjTypes = new ArrayList();
        private string sqlSortBy = string.Empty;
        private bool enterTrapped = false;
        private bool isControlLoaded = false;
        private string fullClassName = "dMobile.ucActivityHdrAdjustmentFilter";
        private bool changesMade = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private Dictionary<string, string> filters = new Dictionary<string, string>();
        private Dictionary<string, string> adjustmentTypeFilters = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="callingControl">Control that called this one.</param>
        public ucActivityHdrAdjustmentFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
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
            // TODO: Resource strings
            this.SortBy1.Clear();
            this.SortBy1.Add(new ComboBoxMembers("Adjustment # Ascending", "ActivityAdjustmentFKRefno ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Adjustment # Descending", "ActivityAdjustmentFKRefno DESC"));
            this.SortBy1.Add(new ComboBoxMembers("Adjustment Type # Ascending", "ActivityAdjustmentType ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Adjustment Type # Descending", "ActivityAdjustmentType DESC"));
            this.SortBy1.Add(new ComboBoxMembers("Adjustment Date Ascending", "ActivityAdjustmentDateTime ASC"));
            this.SortBy1.Add(new ComboBoxMembers("Adjustment Date Descending", "ActivityAdjustmentDateTime DESC"));
            this.cboSortBy1.ItemsSource = this.SortBy1;
            this.cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy1.SelectedIndex = this.AdjSettings.SortBy1Index;

            // TODO: Resource strings
            this.SortBy2.Clear();
            this.SortBy2.Add(new ComboBoxMembers(string.Empty, string.Empty));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment # Ascending", "ActivityAdjustmentFKRefno ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment # Descending", "ActivityAdjustmentFKRefno DESC"));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment Type # Ascending", "ActivityAdjustmentType ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment Type # Descending", "ActivityAdjustmentType DESC"));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment Date Ascending", "ActivityAdjustmentDateTime ASC"));
            this.SortBy2.Add(new ComboBoxMembers("Adjustment Date Descending", "ActivityAdjustmentDateTime DESC"));
            this.cboSortBy2.ItemsSource = this.SortBy2;
            this.cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy2.SelectedIndex = this.AdjSettings.SortBy2Index;

            // TODO: Resource strings
            this.SortBy3.Clear();
            this.SortBy3.Add(new ComboBoxMembers(string.Empty, string.Empty));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment # Ascending", "ActivityAdjustmentFKRefno ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment # Descending", "ActivityAdjustmentFKRefno DESC"));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment Type # Ascending", "ActivityAdjustmentType ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment Type # Descending", "ActivityAdjustmentType DESC"));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment Date Ascending", "ActivityAdjustmentDateTime ASC"));
            this.SortBy3.Add(new ComboBoxMembers("Adjustment Date Descending", "ActivityAdjustmentDateTime DESC"));
            this.cboSortBy3.ItemsSource = this.SortBy3;
            this.cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
            this.cboSortBy3.SelectedIndex = this.AdjSettings.SortBy3Index;

            // TODO: Resource strings
            this.filters.Clear();
            this.filters.Add(string.Empty, string.Empty);
            this.filters.Add("Adjustment # Ascending", "ActivityAdjustmentFKRefno ASC");
            this.filters.Add("Adjustment # Descending", "ActivityAdjustmentFKRefno DESC");
            this.filters.Add("Adjustment Type # Ascending", "ActivityAdjustmentType ASC");
            this.filters.Add("Adjustment Type # Descending", "ActivityAdjustmentType DESC");
            this.filters.Add("Adjustment Date Ascending", "ActivityAdjustmentDateTime ASC");
            this.filters.Add("Adjustment Date Descending", "ActivityAdjustmentDateTime DESC");
        }

        /// <summary>
        /// Adds values to the adjustment type combo box.
        /// </summary>
        private void FillAdjustmentTypeComboBox()
        {
            // KLUDGE: these choices must be in the same order as the ActivityAdjustmentTypes definition.
            // The clsAdjustmentActivity.SelectActivity Method set the oAdjustmentSettings.iAdjustmentTypeIdx property
            // based on the enumeration value
            
            // TODO: Resource strings
            this.AdjTypes.Clear();
            this.AdjTypes.Add(new ComboBoxMembers("Inventory", Convert.ToInt32(SysCommon.AdjTypes.Inventory).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Unit Converstion", Convert.ToInt32(SysCommon.AdjTypes.UnitConversion).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Rework", Convert.ToInt32(SysCommon.AdjTypes.Rework).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Partial Rework", Convert.ToInt32(SysCommon.AdjTypes.PartialRework).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Production", Convert.ToInt32(SysCommon.AdjTypes.Production).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Partial Production", Convert.ToInt32(SysCommon.AdjTypes.PartialProduction).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Disposal", Convert.ToInt32(SysCommon.AdjTypes.Disposal).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Fresh To Frozen", Convert.ToInt32(SysCommon.AdjTypes.Fresh2Frozen).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Transfer", Convert.ToInt32(SysCommon.AdjTypes.Transfer).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Pack Off", Convert.ToInt32(SysCommon.AdjTypes.PackOff).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Lost & Found", Convert.ToInt32(SysCommon.AdjTypes.LostFound).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Cycle Count", Convert.ToInt32(SysCommon.AdjTypes.CycleCount).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Move To Deli", Convert.ToInt32(SysCommon.AdjTypes.MoveToDeli).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Periodic Count", Convert.ToInt32(SysCommon.AdjTypes.PeriodicCount).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Manufacture", Convert.ToInt32(SysCommon.AdjTypes.Manufacture).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Requisition", Convert.ToInt32(SysCommon.AdjTypes.Requisition).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Auto Convert To", Convert.ToInt32(SysCommon.AdjTypes.AutoConvertTo).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Auto Create From", Convert.ToInt32(SysCommon.AdjTypes.AutoCreateFrom).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Skid Putaway", Convert.ToInt32(SysCommon.AdjTypes.SkidPutaway).ToString()));

            // <WO ID="14748" comment="Added convert product due to aforementioned KLUDGE at top of function. Having to have same order of enumerations. So to play it safe I put debox and convert product in.>

            this.AdjTypes.Add(new ComboBoxMembers("Convert Product", Convert.ToInt32(SysCommon.AdjTypes.ConvertProduct).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Debox", Convert.ToInt32(SysCommon.AdjTypes.Debox).ToString()));
            this.AdjTypes.Add(new ComboBoxMembers("Convert Skid", Convert.ToInt32(SysCommon.AdjTypes.ConvertSkid).ToString()));
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
                    if (keycode == Keycode.Escape || keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                        else
                        {
                            await this.CancelChanges();
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
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityHdrAdjustmentFilter_GotFocus threw an error " + ex, "OK");
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

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

        #region Events

        public event ReqCloseHandler ReqClose
        {
            add
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(this.ReqCloseEvent, value);
            }

            remove
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(this.ReqCloseEvent, value);
            }
        }

        public event ReqSaveEnableHandler ReqSaveEnable
        {
            add
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Combine(this.ReqSaveEnableEvent, value);
            }

            remove
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Remove(this.ReqSaveEnableEvent, value);
            }
        }

        public event ReqSaveDisableHandler ReqSaveDisable
        {
            add
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Combine(this.ReqSaveDisableEvent, value);
            }

            remove
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Remove(this.ReqSaveDisableEvent, value);
            }
        }

        public event ReqCancelEnableHandler ReqCancelEnable
        {
            add
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Combine(this.ReqCancelEnableEvent, value);
            }

            remove
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Remove(this.ReqCancelEnableEvent, value);
            }
        }

        public event ReqCancelDisableHandler ReqCancelDisable
        {
            add
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Combine(this.ReqCancelDisableEvent, value);
            }

            remove
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Remove(this.ReqCancelDisableEvent, value);
            }
        }

        #endregion

        #region Properties

        public ArrayList AdjTypes
        {
            get
            {
                return this.adjTypes;
            }

            set
            {
                this.adjTypes = value;
            }
        }

        public ArrayList SortBy1
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

        public ArrayList SortBy2
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

        public ArrayList SortBy3
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

        public bool EnterTrapped
        {
            get
            {
                return this.enterTrapped;
            }

            set
            {
                this.enterTrapped = value;
            }
        }

        public bool IsControlLoaded
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

        public AdjustmentSettings AdjSettings
        {
            get
            {
                return this.adjSettings;
            }

            set
            {
                this.adjSettings = value;
            }
        }

        public string SQLSortBy
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

        public string FullClassName
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

        public bool ChangesMade
        {
            get
            {
                return this.changesMade;
            }

            set
            {
                this.changesMade = value;
            }
        }

        private ReqCloseHandler ReqCloseEvent
        {
            get
            {
                return this.reqCloseEvent;
            }

            set
            {
                this.reqCloseEvent = value;
            }
        }

        private ReqSaveEnableHandler ReqSaveEnableEvent
        {
            get
            {
                return this.reqSaveEnableEvent;
            }

            set
            {
                this.reqSaveEnableEvent = value;
            }
        }

        private ReqSaveDisableHandler ReqSaveDisableEvent
        {
            get
            {
                return this.reqSaveDisableEvent;
            }

            set
            {
                this.reqSaveDisableEvent = value;
            }
        }

        private ReqCancelEnableHandler ReqCancelEnableEvent
        {
            get
            {
                return this.reqCancelEnableEvent;
            }

            set
            {
                this.reqCancelEnableEvent = value;
            }
        }

        private ReqCancelDisableHandler ReqCancelDisableEvent
        {
            get
            {
                return this.reqCancelDisableEvent;
            }

            set
            {
                this.reqCancelDisableEvent = value;
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
        public void ucActivityHdrAdjustmentFilter_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
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

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
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
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
            // TODO: Resource strings
            return "dMobile Activity Adjustment Filter";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public XDocument GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                ("<adjustment_date>" + this.AdjSettings.AdjustmentDate.ToShortDateString() + "</adjustment_date>") +
                ("<adjustment_type>" + Convert.ToInt32(this.AdjSettings.AdjustmentType) + "</adjustment_type>") +
                ("<negate_scan_quantity>" + this.AdjSettings.NegateScanQuantity + "</negate_scan_quantity>") +
                ("<sortby>" + this.SQLSortBy + "</sortby>") + "</params>");

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
                    await errRpt.ShowDialog();
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
                        //this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)(Convert.ToInt32(((ComboBoxMembers)(this.cboAdjustmentType.Items[this.cboAdjustmentType.SelectedIndex])).ValueMember));
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
                    this.AdjSettings.SaveData(ref tempAdjSettings);
                    this.AdjSettings = tempAdjSettings;

                    saveComplete = true;
                    this.ChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Data Failed Validation. Check Your Data.", "OK");
                }
            }
            catch (Exception ex)
            {
                if (SysCommon.ErrorReportAndRequestContinue(this.FullClassName, "SaveChanges", ex) == false)
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
                    //this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)(Convert.ToInt32(((ComboBoxMembers)(this.cboAdjustmentType.Items[this.cboAdjustmentType.SelectedIndex])).ValueMember));
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
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }
        }

        #region IAppPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            //((ComboBoxMembers)this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex]).ValueMember.Trim();
            this.SQLSortBy = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            this.AdjSettings.SortBy1Value = this.filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (this.cboSortBy2.SelectedIndex >= 0)
            {
                this.AdjSettings.SortBy2Value =  this.filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();
                
                if (!string.IsNullOrEmpty(this.AdjSettings.SortBy2Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.AdjSettings.SortBy2Value;
                }
            }

            if (this.cboSortBy3.SelectedIndex >= 0)
            {
                this.AdjSettings.SortBy3Value =  this.filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();
                if (!string.IsNullOrEmpty(this.AdjSettings.SortBy3Value))
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

        #endregion
        
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void btnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}