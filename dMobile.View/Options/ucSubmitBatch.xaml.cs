using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.View.CustomPages;
using System.Data.SQLite;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using System.Collections.ObjectModel;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSubmitBatch : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucOptionsMenu();
        private bool isControlLoaded = false;
        private SysEnvironment sysEnv;
        private bool isChangesMade = false;
        private bool isEnterTrapped = false;
        private SubmitBatchSettings submitBatchSettings;
        private Controller.Activity activity = null;
        private int totalTicks = 0;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        public ucSubmitBatch()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdSubmit.Text = "Submit Selected";
            this.cmdFilter.Text = "Filter";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.SubmitBatchSettings = new SubmitBatchSettings().LoadData();
            }
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

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
                            //this.CallingControl = new ucOptionsMenu();
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
                this.ucSubmitBatch_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucSubmitBatch_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ScannerDisable();

            base.OnDisappearing();
        }

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

        private Controller.Activity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
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

        //private UserControl CallingControl
        //{
        //    get
        //    {
        //        return this.callingControl;
        //    }

        //    set
        //    {
        //        this.callingControl = value;
        //    }
        //}

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

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        private SubmitBatchSettings SubmitBatchSettings
        {
            get
            {
                return this.submitBatchSettings;
            }

            set
            {
                this.submitBatchSettings = value;
            }
        }

        private int TotalTicks
        {
            get
            {
                return this.totalTicks;
            }

            set
            {
                this.totalTicks = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
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
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "dMobile Submit Batch";
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
                    this.IsChangesMade = false;
                    saveComplete = true;
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
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
        public XDocument GetXML()
        {
            XDocument xmldocParams = System.Xml.Linq.XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
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
        /// <remarks></remarks>
        private void LoadData()
        {
            this.Activity = null;

            switch (this.SubmitBatchSettings.eActivitySubType)
            {
                case SysCommon.ActSubTypes.Receiving:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Receiving);
                    break;

                case SysCommon.ActSubTypes.Picking:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Picking);
                    break;

                case SysCommon.ActSubTypes.Returns:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Returns);
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TransferFrom);
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.CycleCounts);
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.PhysicalCounts);
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Adjustment);
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketFinished);
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketMaterial);
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketAddProduct);
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                    break;

                default:
                    throw new NotSupportedException("Unhandled activity sub type.");
            }

            try
            {
                this.Activity.Refresh(null);
            }
            catch (SQLiteException ex)
            {
                for (int curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                {
                    // TODO: Resource strings
                    DisplayAlert("Submit Batch", Convert.ToString("Error " + ex.Data[curExNdx].ToString() + '\r'), "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Submit Batch", ex.Message, "OK");
            }

            this.Activity.DataViewHdrs = this.Activity.GetDataView(true);

            this.DataGridMainActivityItemsList = this.Activity.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

            if (Activity.DataViewHdrs.Count > 0)
            {
                //dataGridMainActivity.Select(0);
                Activity.SelectHdr(0);
            }

            this.IsControlLoaded = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdFilter_Click(object sender, EventArgs e)
        {
            this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSubmitBatchFilter, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                await form.SaveChanges(sender, e);
                XDocument xmlDoc = form.DisplayControlData;
                this.SubmitBatchSettings = this.SubmitBatchSettings.LoadData();
                this.ItemsList.Clear();
                this.LoadData();
            }

            this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSubmit_Click(object sender, EventArgs e)
        {
            int actRefno = 0;
            int actItemRefno = 0;
            ActItem actItem = null;
            ActDetail actDetail = null;
            LocationMap locMap = LocationMap.GetInstance();

            try
            {
                this.cmdSubmit.IsEnabled = false;
                //this.pbarStatus.Visible = true;

                if (this.Activity.DataViewHdrs.Count > 0)
                {
                    for (int actRow = 0; actRow <= this.Activity.DataViewHdrs.Count - 1; actRow++)
                    {
                        actRefno = this.Activity.GetRefnoFromRowIdx(actRow);

                        if (actItem == null)
                        {
                            switch (this.SubmitBatchSettings.eActivitySubType)
                            {
                                case SysCommon.ActSubTypes.Receiving:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Receiving);
                                    break;

                                case SysCommon.ActSubTypes.Picking:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Picking);
                                    break;

                                case SysCommon.ActSubTypes.Returns:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Returns);
                                    break;

                                case SysCommon.ActSubTypes.TransferFrom:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TransferFrom);
                                    break;

                                case SysCommon.ActSubTypes.CycleCounts:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.CycleCounts);
                                    break;

                                case SysCommon.ActSubTypes.PhysicalCounts:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.PhysicalCounts);
                                    break;

                                case SysCommon.ActSubTypes.BarCodeAssociation:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.BarCodeAssociation);
                                    break;

                                case SysCommon.ActSubTypes.Adjustment:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Adjustment);
                                    break;

                                case SysCommon.ActSubTypes.TicketFinished:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketFinished);
                                    break;

                                case SysCommon.ActSubTypes.TicketMaterial:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketMaterial);
                                    break;

                                case SysCommon.ActSubTypes.TicketAddProduct:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketAddProduct);
                                    break;

                                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                                    actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                    break;

                                default:
                                    throw new NotSupportedException("Unhandled activity sub type.");
                            }
                        }

                        actItem.DataViewItems = actItem.GetDataView(true);
                        //GC.Collect();
                        //GC.WaitForPendingFinalizers();

                        if (actItem.DataViewItems.Count > 0)
                        {
                            for (int itemIdx = 0; itemIdx <= actItem.DataViewItems.Count - 1; itemIdx++)
                            {
                                actItem.SelectItem(itemIdx);
                                actItemRefno = actItem.Refno;

                                if (actDetail == null
                                    && this.SubmitBatchSettings.eActivitySubType != SysCommon.ActSubTypes.BarCodeAssociation)
                                {
                                    switch (this.SubmitBatchSettings.eActivitySubType)
                                    {
                                        case SysCommon.ActSubTypes.Receiving:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Receiving);
                                            break;

                                        case SysCommon.ActSubTypes.Picking:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Picking);
                                            break;

                                        case SysCommon.ActSubTypes.Returns:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Returns);
                                            break;

                                        case SysCommon.ActSubTypes.TransferFrom:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TransferFrom);
                                            break;

                                        case SysCommon.ActSubTypes.CycleCounts:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.CycleCounts);
                                            break;

                                        case SysCommon.ActSubTypes.PhysicalCounts:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.PhysicalCounts);
                                            break;

                                        case SysCommon.ActSubTypes.Adjustment:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Adjustment);
                                            break;

                                        case SysCommon.ActSubTypes.TicketFinished:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketFinished);
                                            break;

                                        case SysCommon.ActSubTypes.TicketMaterial:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketMaterial);
                                            break;

                                        case SysCommon.ActSubTypes.TicketAddProduct:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketAddProduct);
                                            break;

                                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                                            actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                            break;

                                        default:
                                            throw new NotSupportedException("Unhandled activity sub type.");
                                    }
                                }

                                if (this.SubmitBatchSettings.eActivitySubType == SysCommon.ActSubTypes.BarCodeAssociation)
                                {
                                    SysCommon.BarCodeSymbologies i = (SysCommon.BarCodeSymbologies)actItem.DataViewItems[itemIdx]["ProductUnitBarCodeSymbologyEnum"];
                                    string symbologyPrefix = SysCommon.GetSymbologyPrefixFromSymbologyEnum(i);

                                    XDocument xmlParams = XDocument.Parse(
                                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                        + "<params>"
                                        + "<ProductUnitBarCodeRefno>" + actItem.DataViewItems[itemIdx]["ProductUnitBarCodeRefno"]
                                        + "</ProductUnitBarCodeRefno>"
                                        + "<ProductUnitRefno>" + actItem.DataViewItems[itemIdx]["ProductUnitRefno"]
                                        + "</ProductUnitRefno>"
                                        + "<ProductUnitBarCodeSymbologyPrefix>" + symbologyPrefix
                                        + "</ProductUnitBarCodeSymbologyPrefix>"
                                        + "<ProductUnitBarCodeCode>" + actItem.DataViewItems[itemIdx]["ProductUnitBarCodeCode"]
                                        + "</ProductUnitBarCodeCode>"
                                        + "<ProductUnitIsBC>" + actItem.DataViewItems[itemIdx]["ProductUnitIsBC"]
                                        + "</ProductUnitIsBC>"
                                        + "<ProductCode>" + actItem.DataViewItems[itemIdx]["ProductCode"] + "</ProductCode>"
                                        + "<UnitCode>" + actItem.DataViewItems[itemIdx]["UnitCode"] + "</UnitCode>"
                                        + "<ProductUnitBarCodeModifier1>"
                                        + actItem.DataViewItems[itemIdx]["ProductUnitBarCodeModifier1"]
                                        + "</ProductUnitBarCodeModifier1>"
                                        + "</params>");

                                    actItem.Submit(xmlParams, itemIdx);
                                }
                                else if (this.SubmitBatchSettings.eActivitySubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                                {
                                    XDocument xmlParams = XDocument.Parse(
                                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                        + "<params>"
                                        + "<ProductUnitRefno>" + actItem.DataViewItems[itemIdx]["ProductUnitRefno"]
                                        + "</ProductUnitRefno>"
                                        + "<ProductCode>" + actItem.DataViewItems[itemIdx]["ProductCode"] + "</ProductCode>"
                                        + "<UnitCode>" + actItem.DataViewItems[itemIdx]["UnitCode"] + "</UnitCode>"
                                        + "<ProductUnitBarCodeCode></ProductUnitBarCodeCode>"
                                        + "<ProductUnitIsBC>" + false + "</ProductUnitIsBC>"
                                        + "<ProductUnitBarCodeSymbologyPrefix></ProductUnitBarCodeSymbologyPrefix>"
                                        + "<alt_qty>"
                                        + actItem.DataViewItems[itemIdx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]
                                        + "</alt_qty>"
                                        + "<base_qty>"
                                        + actItem.DataViewItems[itemIdx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]
                                        + "</base_qty>"
                                        + "<wt_qty>"
                                        + actItem.DataViewItems[itemIdx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]
                                        + "</wt_qty>"
                                        + "<whs_id>" + actItem.DataViewItems[itemIdx]["LocationCode"] + "</whs_id>"
                                        + "<refno>" + actItem.DataViewItems[itemIdx]["ActivityItemPurchaseOrderWorkSheetFKRefno"]
                                        + "</refno>"
                                        + "<itm_refno>" + actItem.DataViewItems[itemIdx]["ActivityItemPurchaseOrderWorkSheetFKRefno"]
                                        + "</itm_refno>"
                                        + "<alt_unit>" + actItem.DataViewItems[itemIdx]["UnitCode"] + "</alt_unit>"
                                        + "<base_unit>" + actItem.ProdInfo.BaseUnitCode + "</base_unit>"
                                        + "<wt_unit>" + actItem.ProdInfo.WeightUnitCode + "</wt_unit>"
                                        + "</params>");

                                    actItem.Submit(xmlParams, itemIdx);
                                    //GC.Collect();
                                    //GC.WaitForPendingFinalizers();
                                    break;
                                }
                                else
                                {
                                    actDetail.DataViewDetails = actDetail.GetDataView(true);

                                    if (actDetail.DataViewDetails.Count > 0)
                                    {
                                        //this.pbarStatus.Maximum = actDetail.DataViewDetails.Count;
                                        //this.pbarStatus.Minimum = 0;

                                        for (int detailIdx = 0; detailIdx <= actDetail.DataViewDetails.Count - 1; detailIdx++)
                                        {
                                            actDetail.GetInfo(detailIdx, false);
                                            actDetail.Submit(detailIdx, null);

                                            //if (detailIdx % 100 == 0)
                                            //{
                                            //    this.pbarStatus.Value = detailIdx;
                                            //}
                                        }
                                    }
                                }

                                //GC.Collect();
                                //GC.WaitForPendingFinalizers();
                            }
                        }
                    }
                }

                // TODO: Resource strings
                await DisplayAlert("Submit Batch", "Submission Complete.", "OK");
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Submit Batch", ex.Message, "OK");
            }
            finally
            {
                //this.pbarStatus.Maximum = 100;
                //this.pbarStatus.Visible = false;
                this.ItemsList.Clear();
                this.LoadData();
                this.cmdSubmit.IsEnabled = true;

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucSubmitBatch_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.LoadData();
            }
        }

        #endregion
    }
}