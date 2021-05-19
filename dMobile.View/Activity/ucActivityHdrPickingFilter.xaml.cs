using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Enums;
using Android.Views;
using SQLite;
using System.Collections.ObjectModel;
using dMobile.View.Forms;
using dMobile.Resources.Models;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrPickingFilter : DMobileModal, IAppArea, IAppPublisher
    {
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.PickingFilter;
        private PickingSettings pickSettings;
        private ArrayList sortBy1 = new ArrayList();
        private ArrayList sortBy2 = new ArrayList();
        private ArrayList sortBy3 = new ArrayList();
        private ArrayList sortBy4 = new ArrayList();
        private string sqlSortBy = string.Empty;
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private DataView dataViewRoutes = new DataView();
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqCloseHandler reqCloseEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private Dictionary<string, string> filters;
        ObservableCollection<object> routesList;
        private bool isFirstAppearance = true;

        #region Constructors

        public ucActivityHdrPickingFilter()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblApplyRouteCode.Text = "Apply Route Code Filter";
            this.lblApplyDeliveryDate.Text = "Apply Delivery Date Filter";
            this.lblDeliveryDate.Text = "Delivered";
            this.lblApplyOrderedDate.Text = "Apply Ordered Date Filter";
            this.lblOrderedDate.Text = "Ordered";
            this.lblRoute.Text = "Route";
            this.lblSortBy.Text = "Sort By";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.PickSettings = new PickingSettings();
                this.Filters = new Dictionary<string, string>();
                this.RoutesList = new ObservableCollection<object>();
            }
        }

        #endregion

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

                if (keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
            });

            try
            {
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    this.cboRoute.ItemsSource = await this.GetPickingRouteObservableCollection();
                    this.cboRoute.ItemDisplayBinding = new Binding("RouteCode");

                    // At least one sort option must be selected
                    // TODO: Resource strings
                    this.SortBy1.Clear();
                    this.SortBy1.Add(new ComboBoxMembers("Order # Ascending", "ActivityPickingFKRefno ASC"));
                    this.SortBy1.Add(new ComboBoxMembers("Order # Descending", "ActivityPickingFKRefno DESC"));
                    this.SortBy1.Add(new ComboBoxMembers("Route # Ascending", "RouteCode ASC"));
                    this.SortBy1.Add(new ComboBoxMembers("Route # Descending", "RouteCode DESC"));
                    this.SortBy1.Add(new ComboBoxMembers("Ordered Date Ascending", "ActivityPickingOrderedDateTime ASC"));
                    this.SortBy1.Add(new ComboBoxMembers("Ordered Date Descending", "ActivityPickingOrderedDateTime DESC"));
                    this.SortBy1.Add(new ComboBoxMembers("Delivery Ascending", "ActivityPickingDeliveryDateTime ASC"));
                    this.SortBy1.Add(new ComboBoxMembers("Delivery Descending", "ActivityPickingDeliveryDateTime DESC"));

                    this.cboSortBy1.ItemsSource = this.SortBy1;
                    this.cboSortBy1.ItemDisplayBinding = new Binding("DisplayMember");
                    this.cboSortBy1.SelectedIndex = this.PickSettings.iSortBy1Index;

                    // TODO: Resource strings
                    this.SortBy2.Clear();
                    this.SortBy2.Add(new ComboBoxMembers(string.Empty, string.Empty));
                    this.SortBy2.Add(new ComboBoxMembers("Order # Ascending", "ActivityPickingFKRefno ASC"));
                    this.SortBy2.Add(new ComboBoxMembers("Order # Descending", "ActivityPickingFKRefno DESC"));
                    this.SortBy2.Add(new ComboBoxMembers("Route # Ascending", "RouteCode ASC"));
                    this.SortBy2.Add(new ComboBoxMembers("Route # Descending", "RouteCode DESC"));
                    this.SortBy2.Add(new ComboBoxMembers("Ordered Date Ascending", "ActivityPickingOrderedDateTime ASC"));
                    this.SortBy2.Add(new ComboBoxMembers("Ordered Date Descending", "ActivityPickingOrderedDateTime DESC"));
                    this.SortBy2.Add(new ComboBoxMembers("Delivery Ascending", "ActivityPickingDeliveryDateTime ASC"));
                    this.SortBy2.Add(new ComboBoxMembers("Delivery Descending", "ActivityPickingDeliveryDateTime DESC"));

                    this.cboSortBy2.ItemsSource = this.SortBy2;
                    this.cboSortBy2.ItemDisplayBinding = new Binding("DisplayMember");
                    this.cboSortBy2.SelectedIndex = this.PickSettings.iSortBy2Index;

                    // TODO: Resource strings
                    this.SortBy3.Clear();
                    this.SortBy3.Add(new ComboBoxMembers(string.Empty, string.Empty));
                    this.SortBy3.Add(new ComboBoxMembers("Order # Ascending", "ActivityPickingFKRefno ASC"));
                    this.SortBy3.Add(new ComboBoxMembers("Order # Descending", "ActivityPickingFKRefno DESC"));
                    this.SortBy3.Add(new ComboBoxMembers("Route # Ascending", "RouteCode ASC"));
                    this.SortBy3.Add(new ComboBoxMembers("Route # Descending", "RouteCode DESC"));
                    this.SortBy3.Add(new ComboBoxMembers("Ordered Date Ascending", "ActivityPickingOrderedDateTime ASC"));
                    this.SortBy3.Add(new ComboBoxMembers("Ordered Date Descending", "ActivityPickingOrderedDateTime DESC"));
                    this.SortBy3.Add(new ComboBoxMembers("Delivery Ascending", "ActivityPickingDeliveryDateTime ASC"));
                    this.SortBy3.Add(new ComboBoxMembers("Delivery Descending", "ActivityPickingDeliveryDateTime DESC"));

                    this.cboSortBy3.ItemsSource = this.SortBy3;
                    this.cboSortBy3.ItemDisplayBinding = new Binding("DisplayMember");
                    this.cboSortBy3.SelectedIndex = this.PickSettings.iSortBy3Index;

                    // TODO: Resource strings
                    this.SortBy4.Clear();
                    this.SortBy4.Add(new ComboBoxMembers(string.Empty, string.Empty));
                    this.SortBy4.Add(new ComboBoxMembers("Order # Ascending", "ActivityPickingFKRefno ASC"));
                    this.SortBy4.Add(new ComboBoxMembers("Order # Descending", "ActivityPickingFKRefno DESC"));
                    this.SortBy4.Add(new ComboBoxMembers("Route # Ascending", "RouteCode ASC"));
                    this.SortBy4.Add(new ComboBoxMembers("Route # Descending", "RouteCode DESC"));
                    this.SortBy4.Add(new ComboBoxMembers("Ordered Date Ascending", "ActivityPickingOrderedDateTime ASC"));
                    this.SortBy4.Add(new ComboBoxMembers("Ordered Date Descending", "ActivityPickingOrderedDateTime DESC"));
                    this.SortBy4.Add(new ComboBoxMembers("Delivery Ascending", "ActivityPickingDeliveryDateTime ASC"));
                    this.SortBy4.Add(new ComboBoxMembers("Delivery Descending", "ActivityPickingDeliveryDateTime DESC"));

                    this.cboSortBy4.ItemsSource = this.SortBy4;
                    this.cboSortBy4.ItemDisplayBinding = new Binding("DisplayMember");
                    this.cboSortBy4.SelectedIndex = this.PickSettings.iSortBy4Index;

                    Filters.Add("", "");
                    Filters.Add("Order # Ascending", "ActivityPickingFKRefno ASC");
                    Filters.Add("Order # Descending", "ActivityPickingFKRefno DESC");
                    Filters.Add("Route # Ascending", "RouteCode ASC");
                    Filters.Add("Route # Descending", "RouteCode DESC");
                    Filters.Add("Ordered Date Ascending", "ActivityPickingOrderedDateTime ASC");
                    Filters.Add("Ordered Date Descending", "ActivityPickingOrderedDateTime DESC");
                    Filters.Add("Delivery Ascending", "ActivityPickingDeliveryDateTime ASC");
                    Filters.Add("Delivery Descending", "ActivityPickingDeliveryDateTime DESC");
                }

                await this.ucActivityPickingFilter_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityPickingFilter_GotFocus threw an error " + ex, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());

            base.OnDisappearing();
        }

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

        private ArrayList SortBy4
        {
            get
            {
                return this.sortBy4;
            }

            set
            {
                this.sortBy4 = value;
            }
        }

        private DataView DataViewRoutes
        {
            get
            {
                return this.dataViewRoutes;
            }

            set
            {
                this.dataViewRoutes = value;
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

        private PickingSettings PickSettings
        {
            get
            {
                return this.pickSettings;
            }

            set
            {
                this.pickSettings = value;
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

        public Dictionary<string, string> Filters { get => filters; set => filters = value; }
        public ObservableCollection<object> RoutesList { get => routesList; set => routesList = value; }

        #endregion

        #region Public Members

        #region IAppArea

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
            return "Picking Filter";
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
                    this.PickSettings.iRouteCodeSelectedIndex = this.cboRoute.SelectedIndex;

                    if (this.chkApplyRouteCode.IsToggled && this.cboRoute.SelectedIndex >= 0)
                    {
                        var selectedItem = this.cboRoute.SelectedItem as dynamic;

                        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.RouteCode))
                        {
                            this.PickSettings.strRouteCodeSelectedValue = Convert.ToString(selectedItem.RouteCode);
                        }
                    }
                    else
                    {
                        this.PickSettings.iRouteCodeSelectedIndex = -1;
                        this.PickSettings.strRouteCodeSelectedValue = string.Empty;
                    }

                    if (this.chkApplyDeliveryDate.IsToggled)
                    {
                        this.PickSettings.dtDeliveryDateFilter = this.dtpDeliveryDate.Date;
                    }

                    if (this.chkApplyOrderedDate.IsToggled)
                    {
                        this.PickSettings.dtOrderedDateFilter = this.dtpOrderedDate.Date;
                    }

                    this.PickSettings.IsApplyRouteCodeFilter = this.chkApplyRouteCode.IsToggled;
                    this.PickSettings.IsApplyDeliveryDateFilter = this.chkApplyDeliveryDate.IsToggled;
                    this.PickSettings.IsApplyOrderedDateFilter = this.chkApplyOrderedDate.IsToggled;

                    this.PickSettings.iSortBy1Index = this.cboSortBy1.SelectedIndex;
                    this.PickSettings.iSortBy2Index = this.cboSortBy2.SelectedIndex;
                    this.PickSettings.iSortBy3Index = this.cboSortBy3.SelectedIndex;
                    this.PickSettings.iSortBy4Index = this.cboSortBy4.SelectedIndex;
                    this.MakeSQLSortBy();

                    PickingSettings tempPickSettings = this.PickSettings;
                    this.PickSettings.SaveData(ref tempPickSettings);
                    this.PickSettings = tempPickSettings;

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
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    this.LoadData();

                    this.chkApplyRouteCode.IsToggled = this.PickSettings.IsApplyRouteCodeFilter;
                    this.cboRoute.SelectedIndex = this.PickSettings.iRouteCodeSelectedIndex;
                    this.cboRoute.SelectedItem = this.PickSettings.strRouteCodeSelectedValue;

                    this.chkApplyDeliveryDate.IsToggled = this.PickSettings.IsApplyDeliveryDateFilter;
                    this.dtpDeliveryDate.Date = this.PickSettings.dtDeliveryDateFilter;

                    this.chkApplyOrderedDate.IsToggled = this.PickSettings.IsApplyOrderedDateFilter;
                    this.dtpOrderedDate.Date = this.PickSettings.dtOrderedDateFilter;

                    this.cboSortBy1.SelectedIndex = this.PickSettings.iSortBy1Index;
                    this.cboSortBy2.SelectedIndex = this.PickSettings.iSortBy2Index;
                    this.cboSortBy3.SelectedIndex = this.PickSettings.iSortBy3Index;
                    this.cboSortBy4.SelectedIndex = this.PickSettings.iSortBy4Index;
                    this.MakeSQLSortBy();

                    cancelChanges = false;
                    this.IsChangesMade = false;

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
                + ("<delivery_date>" + this.PickSettings.dtDeliveryDateFilter.ToShortDateString() + "</delivery_date>")
                + ("<ordered_date>" + this.PickSettings.dtOrderedDateFilter.ToShortDateString() + "</ordered_date>")
                + ("<route_code>" + this.PickSettings.strRouteCodeSelectedValue + "</route_code>")
                + ("<sortby>" + this.SQLSortBy + "</sortby>")
                + "</params>");

            return request;
        }

        #endregion

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
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<ObservableCollection<object>> GetPickingRouteObservableCollection()
        {
            List<tRouteModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tRouteModel>("SELECT DISTINCT RouteCode, RouteRefno FROM tRoute ORDER BY RouteCode ASC");

            if (results?.Count > 0)
            {
                foreach (tRouteModel routeModel in results)
                {
                    object route = new { RouteRefno = routeModel.RouteRefno, RouteCode = routeModel.RouteCode };
                    this.RoutesList.Add(route);
                }
            }

            return this.RoutesList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
                // Allow the validation to handle the setting for IsChangesMade.
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityPickingFilter_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                // Reload PickSettings because intial values will be scewed because of SelectedIndexChanged events firing
                await Task.Run(() => this.LoadData());
                await Device.InvokeOnMainThreadAsync(() => this.DisplayData());

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayData()
        {
            this.chkApplyRouteCode.IsToggled = this.PickSettings.IsApplyRouteCodeFilter;

            if (this.PickSettings.iRouteCodeSelectedIndex >= 0 && this.PickSettings.iRouteCodeSelectedIndex < this.cboRoute.Items.Count)
            {
                this.cboRoute.SelectedIndex = this.PickSettings.iRouteCodeSelectedIndex;
            }
            else
            {
                this.cboRoute.SelectedIndex = -1;
            }

            this.chkApplyDeliveryDate.IsToggled = this.PickSettings.IsApplyDeliveryDateFilter;
            this.dtpDeliveryDate.Date = this.PickSettings.dtDeliveryDateFilter;

            this.chkApplyOrderedDate.IsToggled = this.PickSettings.IsApplyOrderedDateFilter;
            this.dtpOrderedDate.Date = this.PickSettings.dtOrderedDateFilter;

            this.cboSortBy1.SelectedIndex = this.PickSettings.iSortBy1Index;
            this.cboSortBy2.SelectedIndex = this.PickSettings.iSortBy2Index;
            this.cboSortBy3.SelectedIndex = this.PickSettings.iSortBy3Index;
            this.cboSortBy4.SelectedIndex = this.PickSettings.iSortBy4Index;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void MakeSQLSortBy()
        {
            //ComboBoxMembers members = (ComboBoxMembers)this.cboSortBy1.Items[this.cboSortBy1.SelectedIndex];
            this.SQLSortBy = this.Filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();
            this.PickSettings.strSortBy1Value = this.Filters[cboSortBy1.Items[cboSortBy1.SelectedIndex]].Trim();

            if (0 <= cboSortBy2.SelectedIndex)
            {
                //members = (ComboBoxMembers)this.cboSortBy2.Items[this.cboSortBy2.SelectedIndex];
                this.PickSettings.strSortBy2Value = this.Filters[cboSortBy2.Items[cboSortBy2.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.PickSettings.strSortBy2Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.strSortBy2Value;
                }
            }

            if (0 <= cboSortBy3.SelectedIndex)
            {
                //members = (ComboBoxMembers)this.cboSortBy3.Items[this.cboSortBy3.SelectedIndex];
                this.PickSettings.strSortBy3Value = this.Filters[cboSortBy3.Items[cboSortBy3.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.PickSettings.strSortBy3Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.strSortBy3Value;
                }
            }

            if (0 <= cboSortBy4.SelectedIndex)
            {
                //members = (ComboBoxMembers)this.cboSortBy4.Items[this.cboSortBy4.SelectedIndex];
                this.PickSettings.strSortBy4Value = this.Filters[cboSortBy4.Items[cboSortBy4.SelectedIndex]].Trim();

                if (!string.IsNullOrEmpty(this.PickSettings.strSortBy4Value))
                {
                    this.SQLSortBy = this.SQLSortBy + ", " + this.PickSettings.strSortBy4Value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.PickSettings = this.PickSettings.LoadData();

            // Default route selection.
            if (this.PickSettings.iRouteCodeSelectedIndex <= this.cboRoute.Items.Count)
            {
                this.cboRoute.SelectedIndex = this.PickSettings.iRouteCodeSelectedIndex;
            }

            // Default delivery date.
            if (SysCommon.IsEmptyDate(this.PickSettings.dtDeliveryDateFilter))
            {
                this.PickSettings.dtDeliveryDateFilter = DateTime.Now;
            }

            // Default ordered date.
            if (SysCommon.IsEmptyDate(this.PickSettings.dtOrderedDateFilter))
            {
                this.PickSettings.dtOrderedDateFilter = DateTime.Now;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ValidateEntry(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = false;

        //    try
        //    {
        //        if (sender == this.tabOptions)
        //        {
        //        }

        //        if (sender == this.chkApplyDeliveryDate)
        //        {
        //        }

        //        if (sender == this.chkApplyOrderedDate)
        //        {
        //        }

        //        if (sender == this.chkApplyRouteCode)
        //        {
        //        }

        //        if (sender == this.cboRoute)
        //        {
        //        }

        //        if (sender == this.cboSortBy1)
        //        {
        //        }

        //        if (sender == this.cboSortBy3)
        //        {
        //        }

        //        if (sender == this.cboSortBy3)
        //        {
        //        }

        //        if (sender == this.cboSortBy4)
        //        {
        //        }

        //        if (sender == this.dtpDeliveryDate)
        //        {
        //        }

        //        if (sender == this.dtpOrderedDate)
        //        {
        //        }
        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        // Do not allow the exit of control.
        //        e.Cancel = true;
        //        ex.Show(this, this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Do not allow the exit of control.
        //        e.Cancel = true;
        //        using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
        //        {
        //            errRpt.ShowDialog();
        //        }
        //    }
        //}

        #endregion

        private void chkApplyRouteCode_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, new EventArgs());
        }

        private void chkApplyIssuedDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, new EventArgs());
        }

        private void chkApplyOrderedDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, new EventArgs());
        }

        private void chkApplyDeliveryDate_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(this, new EventArgs());
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}