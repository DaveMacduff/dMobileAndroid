using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using static Android.Views.View;
using dMobile.View.CustomPages.Modal;
using System.Collections.ObjectModel;
using Android.Views;
using dMobile.Resources.Types;
using System.IO;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityDetail : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private SysEnvironment sysEnv;
        private bool isHandlerRegistered = false;
        private LocationMap locMap;
        private SysCommon.ActSubTypes actSubTypes = SysCommon.ActSubTypes.NotApplicable;
        private ActDetail actDetail = null;
        private bool isControlLoaded = false;
        private int totalTicks = 0;
        private BarCodeParse parse = null;
        private BarCodeScanner.BarCodeScanner scanner;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;

        #endregion

        #region Constructors

        public ucActivityDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItem"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityDetail(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.cmdLocation.IsEnabled = false;
            this.cmdSelectSkid.IsEnabled = false;
            this.cmdAddSkid.IsEnabled = false;
            this.cmdAddDetail.IsEnabled = false;

            // TODO: Resource strings
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectSkid.Text = "Select Skid";
            this.cmdAddSkid.Text = "Add Skid";
            this.cmdAddDetail.Text = "Add Detail";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.DataGridMainActivityItemsList = new ObservableCollection<object>();
                this.ItemsList = new ObservableCollection<string>();

                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;

                if (actItem.ActHdr.SkidInfo == null)
                {
                    actItem.ActHdr.SkidInfo = new SysSkidInfo();
                }

                this.ActSubType = actSubTypes;
                this.ActDetail = ActDetailFacade.GetActDetail(actItem, actSubTypes);
                this.ActDetail.ActItem = actItem;

                this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(this.ActDetail.ActItem.ProdInfo);
            }
        }

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Del || keycode == Keycode.Clear)
                {
                    await this.dataGridMainActivity_KeyUpDelete(this, new EventArgs());
                }
                else if (keycode == Keycode.Escape)
                {
                    if (this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                // TODO: remove this
                else if (keycode == Keycode.Grave)
                {
                    if (this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await this.Close();
                    }
                }
                else if (keycode == Keycode.F1)
                {
                    if (this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.F3)
                {
                    this.KeyUpF3_ShowTotalQuantity(this, new EventArgs());
                    this.dataGridMainActivity.Focus();
                }
                else if (keycode == Keycode.F4)
                {
                    this.KeyUpF4_ConfirmDetail(this, new EventArgs());
                    this.dataGridMainActivity.Focus();
                }
            });

            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await this.ucActivityDetail_GotFocus(this, new EventArgs());
                });
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucActivityDetail_GotFocus/tmrActivityDetailLoaded_Tick threw an error " + ex, "OK");
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

        private BarCodeScanner.BarCodeScanner Scanner
        {
            get
            {
                return this.scanner;
            }

            set
            {
                this.scanner = value;
            }
        }

        private BarCodeParse Parse
        {
            get
            {
                return this.parse;
            }

            set
            {
                this.parse = value;
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

        private bool IsHandlerRegistered
        {
            get
            {
                return this.isHandlerRegistered;
            }

            set
            {
                this.isHandlerRegistered = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubTypes;
            }

            set
            {
                this.actSubTypes = value;
            }
        }

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
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
        /// Unregister the bar code scan event handler and disable the scanner
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsHandlerRegistered)
            {
                this.IsHandlerRegistered = true;
                this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (this.Scanner != null)
            {
                this.ScannerDisable();
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = true;

            return isReadyToSave;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    ucOptionsMenu displayControl = new ucOptionsMenu();
        //    return displayControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            string desc = string.Empty;


            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.Adjustment:
                    // TODO: Resource strings
                    desc = string.Format("Adjustment # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;


                case SysCommon.ActSubTypes.CycleCounts:
                case SysCommon.ActSubTypes.PhysicalCounts:
                    // TODO: Resource strings
                    desc = string.Format("Count # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;


                case SysCommon.ActSubTypes.Picking:
                case SysCommon.ActSubTypes.PickingSingle:
                    // TODO: Resource strings
                    desc = string.Format("Picking Order # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    // TODO: Resource strings
                    desc = string.Format("Receive # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Returns:
                    // TODO: Resource strings
                    desc = string.Format("Return # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Add-Product # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Finished # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Material # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    // TODO: Resource strings
                    desc = string.Format("Transfer from # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    // TODO: Resource strings
                    desc = string.Format("Transfer to # {0}", this.ActDetail.ActItem.ActHdr.FKRefno);
                    break;

                default:
                    // TODO: Resource strings
                    desc = "Activity";
                    break;
            }

            return desc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            // TODO: Ensure correct value is returned
            bool isSaveComplete = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                    this.SysEnv.SysSettings.CanPrintProductLabelReceiving &&
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                    !this.SysEnv.SysSettings.IsDisconnectedMode &&
                    this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                {
                    if (this.ActDetail.DataViewDetails.Count > 0)
                    {
                        if (await DisplayAlert("Print Label", "Print Product Labels", "Yes", "No"))
                        {
                            await this.PrintBatchedProductLabels();
                        }
                    }
                }
            });

            return isSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (this.IsReadyToClose())
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
                    // this.LoadData();
                    cancelChanges = false;
                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                Device.BeginInvokeOnMainThread(async () =>
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
        public XDocument GetXML()
        {
            SysCommon.SFillQuantity fillQty = this.ActDetail.GetDataViewTotalQuantity();

            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<qty>" + fillQty.Quantity + "</qty>")
                + ("<base_qty>" + fillQty.BaseQuantity + "</base_qty>")
                + ("<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>")
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
        private async Task PrintBatchedProductLabels()
        {
            // For each detail marked for printing
            string sendReq = string.Empty;
            string errDetail = string.Empty;
            string data = string.Empty;
            DataView detail = this.ActDetail.DataViewDetails;

            for (int rowIdx = 0; rowIdx <= detail.Count - 1; rowIdx++)
            {
                if (Convert.ToBoolean(this.ActDetail.DataViewDetails[rowIdx]["IsBatchProductLabel"]))
                {
                    try
                    {
                        XDocument request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + ("<serial>" + detail[rowIdx]["ActivityDetailSingleTradeItemSerialNumber"] + "</serial>")
                            + ("<pdate>" + detail[rowIdx]["ActivityDetailLogisticUnitProductionDate"] + "</pdate>")
                            + ("<lot_no>" + detail[rowIdx]["ActivityDetailLogisticUnitBatchLotNumber"] + "</lot_no>")
                            + ("<bdate>" + detail[rowIdx]["ActivityDetailLogisticUnitBestBeforeSellByDate"] + "</bdate>")
                            + ("<variant_code>" + detail[rowIdx]["ProductSchemeVariantCode"] + "</variant_code>")
                            + ("<variant_no>" + detail[rowIdx]["ProductSchemeVariantNo"] + "</variant_no>")
                            + ("<alt_qty>" + detail[rowIdx]["ActivityDetailQuantity"] + "</alt_qty>")
                            + ("<wt_qty>" + detail[rowIdx]["ActivityDetailBaseQuantity"] + "</wt_qty>")
                            + ("<base_qty>" + detail[rowIdx]["ActivityDetailWeightQuantity"] + "</base_qty>")
                            + "</params>");

                        // CreateSubmitProductLabelXML assumes actItem and 
                        // Loc.sSelectedLocationMap is correct
                        sendReq = this.ActDetail.CreateSubmitProductLabelXML(request);
                        errDetail = string.Empty;
                        data = SysCommon.SubmitComm(sendReq);

                        if (data.Length > 0)
                        {
                            if (data.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument docError = new XmlDocument();
                                docError.LoadXml(data);
                                string errorDetail = docError.DocumentElement["detail"].InnerText;

                                if (errorDetail.Contains("MSGBOX-"))
                                {
                                    throw new Exceptions.MessageOnlyException(errorDetail);
                                }
                                else
                                {
                                    throw new Exception(errorDetail);
                                }
                            }
                        }
                        else
                        {
                            // TODO: Resource strings
                            errDetail = "Unexpected print product label(s) error.";
                        }

                        if (string.IsNullOrEmpty(errDetail))
                        {
                            detail[rowIdx].BeginEdit();
                            detail[rowIdx]["IsBatchProductLabel"] = Convert.ToBoolean(false);
                            detail[rowIdx].EndEdit();
                        }
                        else
                        {
                            //DialogResult result = MessageBox.Show(errDetail, "Printing Label(s)", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                            if (await DisplayAlert("Printing Label(s)", errDetail, "Retry", "Abort"))
                            {
                                rowIdx--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Print product label(s) failed. " + Environment.NewLine + ex.Message, "Print Label(s)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        await DisplayAlert("Print Label(s)", "Print product label(s) failed. " + Environment.NewLine + ex.Message, "OK");
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await form.ShowDialog() == DialogResult.OK)
                {
                    if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                }
                else
                {
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                }

                this.ScannerEnable();

                this.dataGridMainActivity.Focus();
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private async Task tmrActivityDetailLoaded_Tick(object sender, EventArgs e)
        //{

        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityDetail_LostFocus(object sender, EventArgs e)
        {
            //if (this.tmrActivityDetailLoaded.Enabled == false)
            //{
            this.ScannerDisable();
            //}
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchDetail_GotFocus(object sender, EventArgs e)
        {
            // TODO: Text selection 
            //this.txtSearchDetail.SelectAll();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSkidInfo, this.ActDetail.ActItem, this.ActSubType);
                await form.DisplayControl.ShowDialog();
                XDocument xmlDoc = form.DisplayControlData;
                this.ActDetail.ActItem.ActHdr.SkidInfo = SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    SSkidInfo currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    if (this.ActDetail.ActItem.ProdInfo.Code == currSkid.ProdInfo.Code && this.ActDetail.ActItem.ProdInfo.UnitBC == currSkid.ProdInfo.UnitBC && this.ActDetail.ActItem.ProdInfo.UnitCode == currSkid.ProdInfo.UnitCode)
                    {
                        this.cmdAddSkid.IsEnabled = true;
                        // TODO: Resource strings
                        this.cmdSelectSkid.Text = "Skid# " + currSkid.SkidCode;
                        this.LocMap.CurrLocMap = currSkid.LocMap;

                        this.cmdLocation.IsEnabled = false;
                    }
                    else
                    {
                        // TODO: Resource strings
                        //MessageBox.Show("Skid product must match the selected line item product.", "Skid", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        await DisplayAlert("Skid", "Skid product must match the selected line item product.", "Ok");

                        // TODO: Resource strings
                        this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        this.cmdSelectSkid.Text = "Select Skid";
                        this.cmdAddSkid.IsEnabled = false;
                    }
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Select Skid";
                    this.cmdAddSkid.IsEnabled = false;
                    this.cmdLocation.IsEnabled = true;
                }
            }

            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdAddDetail_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                LocationMap locMap = LocationMap.GetInstance();

                this.ScannerDisable();

                if (!LocationMapTasks.IsCompleteLocMapRefno(locMap.CurrLocMapRefno))
                {
                    // TODO: Resource strings
                    //MessageBox.Show("Location must be set first.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    await DisplayAlert(this.GetControlDescription(), "Location must be set first.", "OK");
                }
                else
                {
                    try
                    {
                        bool isAllowedAddManualDetail = true;

                        if (sysEnv.SysSettings.IsPickAddManualDetailPasswordRequired && (this.ActSubType == SysCommon.ActSubTypes.PickingSingle || this.ActSubType == SysCommon.ActSubTypes.Picking))
                        {
                            // TODO: Do form
                            //FormAddManualDetailOverride form = new FormAddManualDetailOverride();

                            //if (await form.ShowDialog() == DialogResult.OK)
                            //{
                            //    isAllowedAddManualDetail = true;
                            //}
                            //else
                            //{
                            //isAllowedAddManualDetail = false;
                            //}
                        }

                        if (isAllowedAddManualDetail)
                        {
                            // Not allowed to count individual boxes on a skid. Skids must be
                            // scanned as a whole unit or built in skid maintenance.

                            // DEV-827 This change to allow counting partial skids is a bad idea.                        
                            ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                            ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                            ////{
                            ////    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                            ////    {
                            ////        this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                            ////    }
                            ////}

                            BarCodeParse parse = BarCodeParseFacade.ParseData(string.Empty);
                            bool isInsertReady = false;

                            this.ActDetail.Quantity = 0;
                            this.ActDetail.BaseQuantity = 0;
                            this.ActDetail.WeightQuantity = 0;
                            this.ActDetail.SchemeID = string.Empty;
                            this.ActDetail.UniqueSchemeID = string.Empty;
                            this.ActDetail.ActExtra.Text1 = string.Empty;
                            this.ActDetail.ActExtra.Text2 = string.Empty;
                            this.ActDetail.ActExtra.Text3 = string.Empty;
                            this.ActDetail.ActExtra.Text4 = string.Empty;
                            this.ActDetail.ActExtra.Text5 = string.Empty;

                            // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                            // and the fact is isn't human readable friendly.
                            SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                            SProductSchemeData schemeData = actDetail.SchemeData;

                            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                            {
                                schemeData.LogisticUnitSkidNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                                schemeData.LogisticUnitBatchLotNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                                schemeData.LogisticUnitBestBeforeSellByDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                                schemeData.LogisticUnitProductionDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                                schemeData.VariantCode = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                                schemeData.VariantNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                                schemeData.VariantDescription = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                            }

                            tempItemProdSchemeData.ProdSchemeData = schemeData;
                            actDetail.SchemeData = schemeData;

                            isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, true);
                            this.InsertActivityDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem), this.ActDetail, parse, isInsertReady);
                            this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                        }
                    }
                    catch (GatherProductDataException ex)
                    {
                        if (!ex.SuppressError)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                                await errRpt.ShowDialog();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }

                this.DisplayData();
                this.ScannerEnable();
                this.dataGridMainActivity.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAddSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                LocationMap locMap = LocationMap.GetInstance();

                this.ScannerDisable();

                try
                {
                    bool isAllowedAddManualDetail = true;

                    if (sysEnv.SysSettings.IsPickAddManualDetailPasswordRequired && (this.ActSubType == SysCommon.ActSubTypes.PickingSingle || this.ActSubType == SysCommon.ActSubTypes.Picking))
                    {
                        if (!sysEnv.ContactLog.IsAllowedPickingModuleOverrideAddManualDetail)
                        {
                            FormAddManualDetailOverride form = new FormAddManualDetailOverride();

                            // TODO: Do form FormAddManualDetailOverride
                            //if (await form.ShowDialog() == DialogResult.OK)
                            //{
                            //    isAllowedAddManualDetail = true;
                            //}
                            //else
                            //{
                            //    isAllowedAddManualDetail = false;
                            //}
                        }
                    }

                    if (isAllowedAddManualDetail)
                    {
                        //if (MessageBox.Show("Add the ENTIRE skid?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        if (await DisplayAlert(this.GetControlDescription(), "Add the ENTIRE skid?", "Yes", "No"))
                        {
                            if (this.ActDetail.DataViewDetails.Count > 0)
                            {
                                //this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);
                            }

                            this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ActDetail.ActItem.ProdInfo.Code, this.ActDetail.ActItem.ProdInfo.Scheme, this.ActDetail.SchemeData);

                            // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                            XDocument xmlParam = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<imclient>"
                                + ("<itm_refno>" + this.ActDetail.ActItem.FKRefno + "</itm_refno>")
                                + ("<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                                + ("<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                                + ("<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                                + "</imclient>");

                            this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                            this.ActDetail.InsertSkid(this.ActDetail.ActItem.ActHdr.SkidInfo);
                            this.RefreshData();
                            SysCommon.SFillQuantity fillQty = this.ActDetail.GetDataViewTotalQuantity();
                            this.ActDetail.ActItem.UpdateDataViewQuantity(this.ActDetail.ActItem.CurrRowIdx, fillQty.Quantity, fillQty.BaseQuantity, fillQty.WeightQuantity);

                            this.dataGridMainActivity.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }

                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityDetail_GotFocus(object sender, EventArgs e)
        {
            this.Loading = true;

            await Task.Run(() =>
            {
                if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
                {
                    this.ScannerEnable();

                    if (this.IsControlLoaded == false)
                    {
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            string getReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            Device.BeginInvokeOnMainThread(() => this.cmdLocation.Text = getReadableLocation);
                        }
                        else
                        {
                            // TODO: Resource strings
                            Device.BeginInvokeOnMainThread(() => this.cmdLocation.Text = "Set Location");
                        }

                        if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            // TODO: Resource strings
                            Device.BeginInvokeOnMainThread(() => this.cmdSelectSkid.Text = "Skid# " + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                        }
                        else
                        {
                            // TODO: Resource strings
                            Device.BeginInvokeOnMainThread(() => this.cmdSelectSkid.Text = "Select Skid");
                        }
                    }
                }

                this.RefreshData();

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (this.actDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        this.cmdLocation.IsEnabled = true;
                    }
                    else
                    {
                        cmdLocation.IsEnabled = false;
                    }

                    this.cmdSelectSkid.IsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                    this.cmdAddDetail.IsEnabled = true;

                });

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.IsControlLoaded = true;

            });

            this.Loading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ActDetail.Search(this.txtSearchDetail.Text);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void dataGridMainActivity_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    //this.dataGridMainActivity.Select(this.dataGridMainActivity.CurrentRowIndex);
        //    this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
        //    this.DisplayData();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task dataGridMainActivity_KeyUpDelete(object sender, EventArgs e)
        {
            try
            {
                this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.ScannerDisable();

                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                {
                    //DialogResult result = MessageBox.Show("Do you wish to delete this record?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    if (await DisplayAlert(this.GetControlDescription(), "Do you wish to delete this record?", "Yes", "No"))
                    {
                        int actDetailFKRefno = this.ActDetail.GetFKRefno(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));

                        this.ActDetail = this.ActDetail.GetInfo(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem), false);
                        this.ActDetail.ActItem.UpdateDataViewQuantity(this.ActDetail.ActItem.CurrRowIdx, this.ActDetail.Quantity * -1, this.ActDetail.BaseQuantity * -1, this.ActDetail.WeightQuantity * -1);
                        this.ActDetail.Delete();

                        if (this.ActDetail.DataViewDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                        {
                            //this.dataGridMainActivity.Select(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                            this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                        }
                        else
                        {
                            // this.lstDisplay will be cleared in SelectActivityDetail
                            this.ActDetail.SelectDetail(-1);
                        }

                        this.DisplayData();
                    }
                }
                else
                {
                    if (0 <= this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem))
                    {
                        int currIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
                        SysSkidInfo skidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode((string)this.ActDetail.DataViewDetails[currIdx]["ActivityDetailLogisticUnitSkidNumber"]);
                        //DialogResult result = MessageBox.Show(string.Format("Do you wish to delete this entire skid \'{0}\'?", skidInfo.CurrSkid.SkidCode), this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                        if (await DisplayAlert(this.GetControlDescription(), string.Format("Do you wish to delete this entire skid \'{0}\'?", skidInfo.CurrSkid.SkidCode), "Yes", "No"))
                        {
                            this.ActDetail.DeleteSkid(skidInfo);
                            this.RefreshData();
                            SysCommon.SFillQuantity fillQty = this.ActDetail.GetDataViewTotalQuantity();
                            this.ActDetail.ActItem.UpdateDataViewQuantity(this.ActDetail.ActItem.CurrRowIdx, fillQty.Quantity, fillQty.BaseQuantity, fillQty.WeightQuantity);
                        }
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.dataGridMainActivity.Focus();
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void dataGridMainActivity_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (this.IsControlLoaded)
        //    {
        //        if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Clear || e.KeyCode == Keys.Back)
        //        {
        //            this.dataGridMainActivity_KeyUpDelete(sender, e);
        //        }
        //        else if (e.KeyCode == Keys.Escape)
        //        {
        //            if (this.CancelChanges())
        //            {
        //                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //        else if (e.KeyCode == Keys.F1)
        //        {
        //            if (this.SaveChanges())
        //            {
        //                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //        else if (e.KeyCode == Keys.F3)
        //        {
        //            this.KeyUpF3_ShowTotalQuantity(sender, e);
        //            this.dataGridMainActivity.Focus();
        //        }
        //        else if (e.KeyCode == Keys.F4)
        //        {
        //            this.KeyUpF4_ConfirmDetail(sender, e);
        //            this.dataGridMainActivity.Focus();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void KeyUpHandler(object sender, KeyEventArgs e)
        //{
        //    if (this.IsControlLoaded)
        //    {
        //        if (e.KeyCode == Keys.Escape)
        //        {
        //            if (this.CancelChanges())
        //            {
        //                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //        else if (e.KeyCode == Keys.F1)
        //        {
        //            if (this.SaveChanges())
        //            {
        //                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
        //            }
        //        }
        //        else if (e.KeyCode == Keys.F3)
        //        {
        //            this.KeyUpF3_ShowTotalQuantity(sender, e);
        //        }
        //        else if (e.KeyCode == Keys.F4)
        //        {
        //            this.KeyUpF4_ConfirmDetail(sender, e);
        //        }
        //    }
        //}

        /// <summary>
        /// Mark selected line detail as confirmed.
        /// </summary>
        /// <remarks>
        // Dec 22 / 2014
        // Discussed with John agreed that splitting of quantity for confirmation is not going to happen.
        // The user must manually confirm the entire quantity and not a partial.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyUpF4_ConfirmDetail(object sender, EventArgs e)
        {
            if (this.ActSubType == SysCommon.ActSubTypes.Picking)
            {
                // Send the Activity Detail FKRefno to the comm server.
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);

                XDocument request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT SALES ORDER DETAIL CONFIRM</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.ActDetail.FKRefno + "</refno>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                try
                {
                    string reqData = SysCommon.SubmitComm(xmlString.ToString());

                    if (reqData.Length > 0)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            string errorDetail = docError.DocumentElement["detail"].InnerText;

                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new Exceptions.MessageOnlyException(errorDetail);
                            }
                            else
                            {
                                throw new Exception(errorDetail);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected confirm detail error.");
                    }

                    DataRowView dr = this.ActDetail.DataViewDetails[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)];
                    dr.BeginEdit();
                    dr["ActivityDetailIsConfirmed"] = true;
                    dr["ActivityDetailIsConfirmedString"] = "C";
                    dr.EndEdit();
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyUpF3_ShowTotalQuantity(object sender, EventArgs e)
        {
            try
            {
                this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.ScannerDisable();

                string msg = this.ActDetail.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    // TODO: Resource strings
                    //MessageBox.Show(msg, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    DisplayAlert(this.GetControlDescription(), msg, "Ok");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs"></param>
        private void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            int prodUnitRefno = 0;
            int currRowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
            int svdActDetailFKRefno = this.ActDetail.FKRefno;

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    this.Parse = scanArgs.ParseInfo;

                    this.ScannerDisable();

                    if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.Parse.LocationIdentifier));

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.cmdLocation.Text = "Set Location";
                        }
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                        {
                            if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                            {
                                // TODO: Resource strings
                                //MessageBox.Show("Location must be set first.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                                await DisplayAlert(this.GetControlDescription(), "Location must be set first.", "Ok");
                            }
                            else
                            {
                                SysShared.IdentifyProductScanned(this.ActSubType, this.Parse, ref prodUnitRefno);

                                if (prodUnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Scanned product does not match the selected product.");
                                }

                                // Not allowed to count individual boxes on a skid. Skids must be
                                // scanned as a whole unit or built in skid maintenance.

                                // DEV-827 This change to allow counting partial skids is a bad idea.                        
                                ////                            if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                                ////                                || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                                ////                            {
                                ////                                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                                ////                                {
                                ////                                    this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                                ////#if MULTILANGUAGE
                                ////                  this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                                ////#else
                                ////                                    this.cmdSelectSkid.Text = "Select Skid";
                                ////#endif
                                ////                                }
                                ////                            }

                                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                                {
                                    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("Identified line item product does not match the selected skid product.");
                                    }
                                }
                                else if (this.ActDetail.ActItem.ProdInfo.UnitRefno != prodUnitRefno)
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Identified line item product does not match the selected product.");
                                }

                                // Get the activity item refno from the identified location refno 
                                // and product refno
                                int tempActItemRefno = this.ActDetail.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);

                                if (tempActItemRefno == 0)
                                {
                                    // Location and product was not found on this activity
                                    ProductInfo prodInfo = new ProductInfo();
                                    prodInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                                    // TODO: Resource strings
                                    //MessageBox.Show((string)this.ActDetail.ActItem.GetItemNotFoundMessage(this.LocMap, prodInfo), this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                    await DisplayAlert(this.GetControlDescription(), Convert.ToString(this.ActDetail.ActItem.GetItemNotFoundMessage(this.LocMap, prodInfo)), "Ok");

                                }

                                // If an activity line item was found
                                if (tempActItemRefno != 0)
                                {
                                    //if (this.ActDetail.DataViewDetails.Count > 0)
                                    //{
                                    //dataGridMainActivity.UnSelect(dataGridMainActivity.CurrentRowIndex);
                                    //}

                                    bool isInsertReady = false;

                                    SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                                    SProductSchemeData schemeData = actDetail.SchemeData;

                                    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                                    {
                                        schemeData.LogisticUnitSkidNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                                        schemeData.LogisticUnitBatchLotNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                                        schemeData.LogisticUnitBestBeforeSellByDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                                        schemeData.LogisticUnitProductionDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                                        schemeData.VariantCode = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                                        schemeData.VariantNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                                        schemeData.VariantDescription = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                                    }

                                    tempItemProdSchemeData.ProdSchemeData = schemeData;
                                    actDetail.SchemeData = schemeData;

                                    isInsertReady = await SysShared.GatherProductData(this.Parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);
                                    this.InsertActivityDetail(currRowIdx, this.ActDetail, this.Parse, isInsertReady);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
                        {
                            // TODO: Resource strings
                            //MessageBox.Show("Scanning a header reference # is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            await DisplayAlert(this.GetControlDescription(), "Scanning a header reference # is not supported here.", "Ok");
                        }
                        else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
                        {
                            // TODO: Resource strings
                            //MessageBox.Show("Scanning an item reference # is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            await DisplayAlert(this.GetControlDescription(), "Scanning an item reference # is not supported here.", "Ok");
                        }
                        else if (!string.IsNullOrEmpty(this.Parse.ActDetailFKRefno))
                        {
                            // TODO: Resource strings
                            //MessageBox.Show("Scanning a detail reference # is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            await DisplayAlert(this.GetControlDescription(), "Scanning a detail reference # is not supported here.", "Ok");
                        }
                        else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
                        {
                            if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                            {
                                // TODO: Resource strings
                                throw new Exception("Scanning a shipping container bar code is not enabled.");
                            }

                            if (this.SysEnv.SysSettings.IsDisconnectedMode)
                            {
                                // TODO: Resource strings
                                throw new Exception("Scanning a shipping container bar code is not supported in disconnected mode.");
                            }

                            this.ActDetail.ActItem.ActHdr.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(scanArgs.ParseInfo.SSCC18);

                            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                            {
                                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Identified line item product does not match the selected skid product.");
                                }

                                // TODO: Resource strings
                                this.cmdSelectSkid.Text = "Skid# " + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            }
                            else
                            {
                                // TODO: Resource strings
                                this.cmdSelectSkid.Text = "Select Skid";
                            }

                            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                            {
                                // TODO: Resource strings
                                throw new Exception("Skid does not exist.");
                            }

                            if (this.ActDetail.ActItem.LocMap.CurrLocMap.WhsLocMapRefno != this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap.WhsLocMapRefno)
                            {
                                // TODO: Resource strings
                                throw new Exception("Skid warehouse does not match the selected warehouse.");
                            }

                            if (this.ActDetail.ActItem.ProdInfo.UnitRefno != this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno)
                            {
                                // TODO: Resource strings
                                throw new Exception("Skid product does not match the line item product.");
                            }

                            // Insert a new activity detail record
                            //if (this.ActDetail.DataViewDetails.Count > 0)
                            //{
                            //    this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);
                            //}

                            this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ActDetail.ActItem.ProdInfo.Code, this.ActDetail.ActItem.ProdInfo.Scheme, this.ActDetail.SchemeData);

                            // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                            XDocument xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + ("<itm_refno>" + this.ActDetail.ActItem.FKRefno + "</itm_refno>")
                            + ("<alt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                            + ("<wt_qty>" + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                            + ("<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                            + "</imclient>");

                            this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                            this.ActDetail.InsertSkid(this.ActDetail.ActItem.ActHdr.SkidInfo);

                            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                            {
                                this.cmdAddSkid.IsEnabled = true;

                                // TODO: Resource strings
                                this.cmdSelectSkid.Text = "Skid # " + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            }
                            else
                            {
                                this.cmdAddSkid.IsEnabled = false;

                                // TODO: Resource strings
                                this.cmdSelectSkid.Text = "Select Skid";
                            }

                            this.RefreshData();
                            SysCommon.SFillQuantity fillQty = this.ActDetail.GetDataViewTotalQuantity();
                            this.ActDetail.ActItem.UpdateDataViewQuantity(this.ActDetail.ActItem.CurrRowIdx, fillQty.Quantity, fillQty.BaseQuantity, fillQty.WeightQuantity);
                        }
                        else
                        {
                            // TODO: Resource strings
                            //MessageBox.Show("Scanned bar code type is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here.", "Ok");
                        }
                    }
                }
                catch (IsReadyToApplyException)
                {
                    this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);
                }
                catch (GatherProductDataException ex)
                {
                    this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                    if (!ex.SuppressError)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.ScannerEnable();
                }
            });
        }

        /// <summary>
        /// Used to move the grid row to the correct position, And set the state of some controls.
        /// </summary>
        /// <param name="svdActDetailFKRefno"></param>
        /// <param name="currRowIdx"></param>
        private void CleanUpBarCodeHandler(int svdActDetailFKRefno, int currRowIdx)
        {
            if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.cmdAddSkid.IsEnabled = true;

                // TODO: Resource strings
                this.cmdSelectSkid.Text = "Skid # " + this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
            }
            else
            {
                this.cmdAddSkid.IsEnabled = false;

                // TODO: Resource strings
                this.cmdSelectSkid.Text = "Select Skid";
            }

            this.ActDetail.FKRefno = svdActDetailFKRefno;

            if (this.ActDetail.DataViewDetails.Count > 0 && currRowIdx + 1 <= this.ActDetail.DataViewDetails.Count)
            {
                //this.dataGridMainActivity.CurrentRowIndex = currRowIdx;
                this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[currRowIdx];
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshData()
        {
            //this.TotalTicks = this.TotalTicks + this.tmrActivityDetailLoaded.Interval;

            this.ScannerDisable();

            //this.pbarStatus.Visible = true;
            //this.lstDisplay.Items.Clear();
            //this.lstDisplay.Items.Add("Loading item detail(s). Please wait...");
            //this.lstDisplay.Invalidate();
            //this.lstDisplay.Update();

            try
            {
                //this.pbarStatus.Minimum = 0;
                //this.pbarStatus.Maximum = 100;
                //this.pbarStatus.Value = 0;
                this.ActDetail.Refresh();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                this.ItemsList.Clear();
                this.ActDetail.DataViewDetails = this.ActDetail.GetDataView(false);

                this.DesignGrid();

                //this.dataGridMainActivity.ItemsSource = this.ActDetail.DataViewDetails;
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                if (this.ActDetail.DataViewDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                {
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                    this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                    this.DisplayData();
                    this.dataGridMainActivity.Focus();
                }
                else
                {
                    this.ActDetail.SelectDetail(-1);
                    this.DisplayData();
                }

                this.ScannerEnable();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="parse"></param>
        /// <param name="isInsertDetailReady"></param>
        private void InsertActivityDetail(int currRowIdx, ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                actDetail.FKRefno = 0;
                XDocument xmlDoc = actDetail.GetXML(actDetail);
                actDetail.Insert(xmlDoc);
                actDetail.ActItem.UpdateDataViewQuantity(actDetail.ActItem.CurrRowIdx, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
            }

            if (actDetail.DataViewDetails.Count > 0)
            {
                //this.dataGridMainActivity.CurrentRowIndex = actDetail.DataViewDetails.Count - 1;
                //this.dataGridMainActivity.Select(dataGridMainActivity.CurrentRowIndex);
                this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.Count - 1];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayData()
        {
            int detailIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

            switch (this.ActDetail.ActSubType)
            {
                case SysCommon.ActSubTypes.Adjustment:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"].ToString());
                    }

                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = this.ActDetail.DataViewDetails[detailIdx]["Location"].ToString();
                        string locCode = this.ActDetail.DataViewDetails[detailIdx]["LocationCode"].ToString();
                        string locName = this.ActDetail.DataViewDetails[detailIdx]["LocationName"].ToString();

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    this.ItemsList.Clear();

                    if (0 >= this.ActDetail.ActItem.ProdInfo.Refno)
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(string.Format("PRODUCT: {0}", "All"));
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(string.Format("PRODUCT: {0}", ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActDetail.ActItem.ProdInfo)));
                    }

                    // TODO: Resource strings
                    this.ItemsList.Add(string.Format("LOCATION: {0}", LocationMapTasks.GetCurrentReadableLocation()));
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = this.ActDetail.DataViewDetails[detailIdx]["Location"].ToString();
                        string locCode = this.ActDetail.DataViewDetails[detailIdx]["LocationCode"].ToString();
                        string locName = this.ActDetail.DataViewDetails[detailIdx]["LocationName"].ToString();

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.Picking:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));

                        DataView dataViewItems = this.ActDetail.ActItem.DataViewItems;
                        int itemIdx = this.ActDetail.ActItem.CurrRowIdx;
                        decimal qty = Convert.ToDecimal(dataViewItems[itemIdx]["ActivityItemPickingQuantityShipped"]);
                        decimal wtQty = Convert.ToDecimal(dataViewItems[itemIdx]["ActivityItemPickingWeightQuantityShipped"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("Total: " + SysCommon.NiceQuantityReadableString(this.ActDetail.ActItem.ProdInfo, qty, wtQty));
                    }

                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));

                        DataView dataViewItems = this.ActDetail.ActItem.DataViewItems;
                        int itemIdx = this.ActDetail.ActItem.CurrRowIdx;
                        decimal qty = Convert.ToDecimal(dataViewItems[itemIdx]["ActivityItemPickingQuantityShipped"]);
                        decimal wtQty = Convert.ToDecimal(dataViewItems[itemIdx]["ActivityItemPickingWeightQuantityShipped"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("Total: " + SysCommon.NiceQuantityReadableString(this.ActDetail.ActItem.ProdInfo, qty, wtQty));
                    }

                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    break;

                case SysCommon.ActSubTypes.Returns:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                        this.ItemsList.Add("PRODUCT: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + " " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
                    }

                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    this.ItemsList.Clear();

                    if (detailIdx >= 0)
                    {
                        string loc = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["Location"]);
                        string locCode = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationCode"]);
                        string locName = Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["LocationName"]);

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + loc + ": " + locCode + " " + locName);
                        this.ItemsList.Add(Convert.ToString(this.ActDetail.DataViewDetails[detailIdx]["ActivityDetailSchemeID"]));
                    }

                    break;
            }
        }

        private void DataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActDetail.SelectDetail(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
            this.DisplayData();
            // TODO: Apply filter
        }

        private void DesignGrid()
        {
            switch (this.ActDetail.ActSubType)
            {
                case SysCommon.ActSubTypes.Adjustment:
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    break;

                case SysCommon.ActSubTypes.Picking:
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    break;

                case SysCommon.ActSubTypes.Receiving:
                       // Header grid
                    Grid gridHeaderReceiving = new Grid
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        }
                    };

                    gridHeaderReceiving.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

                    // TODO: Resource strings
                    Label labelActivityDetailQuantityReceiveDetailHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
                    Label labelActivityDetailWeightQuantityReceiveDetailHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
                    Label labelUnitCodeReceiveDetailHeader = new Label { Text = " ", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
                    Label labelActivityDetailSchemeIDReceiveDetailHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

                    gridHeaderReceiving.Children.Add(labelActivityDetailQuantityReceiveDetailHeader);
                    gridHeaderReceiving.Children.Add(labelActivityDetailWeightQuantityReceiveDetailHeader);
                    gridHeaderReceiving.Children.Add(labelUnitCodeReceiveDetailHeader);
                    gridHeaderReceiving.Children.Add(labelActivityDetailSchemeIDReceiveDetailHeader);
                        
                    Grid.SetColumn(labelActivityDetailQuantityReceiveDetailHeader, 0);
                    Grid.SetColumn(labelActivityDetailWeightQuantityReceiveDetailHeader, 1);
                    Grid.SetColumn(labelUnitCodeReceiveDetailHeader, 2);
                    Grid.SetColumn(labelActivityDetailSchemeIDReceiveDetailHeader, 3);

                    Grid.SetRow(gridHeaderReceiving, 0);

                    this.dataGridMainActivityHeader.Children.Add(gridHeaderReceiving);

                    // Data grid
                    DataTemplate dataTemplateReceiving = new DataTemplate(() =>
                    {
                        Grid gridData = new Grid
                        {
                            Padding = new Thickness(5),
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                            }
                        };

                        Label labelActivityDetailQuantityReceiveDetail = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                        Label labelActivityDetailWeightQuantityReceiveDetail = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                        Label labelUnitCodeReceiveDetail = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                        Label labelActivityDetailSchemeIDReceiveDetail = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                        labelActivityDetailQuantityReceiveDetail.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity"));
                        labelActivityDetailWeightQuantityReceiveDetail.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity"));
                        labelUnitCodeReceiveDetail.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                        labelActivityDetailSchemeIDReceiveDetail.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                        gridData.Children.Add(labelActivityDetailQuantityReceiveDetail);
                        gridData.Children.Add(labelActivityDetailWeightQuantityReceiveDetail);
                        gridData.Children.Add(labelUnitCodeReceiveDetail);
                        gridData.Children.Add(labelActivityDetailSchemeIDReceiveDetail);

                        Grid.SetColumn(labelActivityDetailQuantityReceiveDetail, 0);
                        Grid.SetColumn(labelActivityDetailWeightQuantityReceiveDetail, 1);
                        Grid.SetColumn(labelUnitCodeReceiveDetail, 2);
                        Grid.SetColumn(labelActivityDetailSchemeIDReceiveDetail, 3);

                        return new ViewCell { View = gridData };
                    });

                    this.dataGridMainActivity.ItemTemplate = dataTemplateReceiving;
                    this.DataGridMainActivityItemsList.Clear();

                    foreach (DataRow row in this.ActDetail.DataViewDetails.ToTable().Rows)
                    {
                        ReceiveDetail receiveDetail = new ReceiveDetail
                        {
                            ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"]),
                            LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                            ActivityDetailReceivingFKRefno = Convert.ToInt32(row["ActivityDetailReceivingFKRefno"]),
                            UnitCode = Convert.ToString(row["UnitCode"]),
                            WtUnitCode = Convert.ToInt32(row["WtUnitCode"]),
                            BaseUnitCode = Convert.ToInt32(row["BaseUnitCode"]),
                            ProductCode = Convert.ToString(row["ProductCode"]),
                            ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                            ProductDescription = Convert.ToString(row["ProductDescription"]),
                            ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                            ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                            ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                            ActivityDetailQuantity = Convert.ToInt32(row["ActivityDetailQuantity"]),
                            ActivityDetailBaseQuantity = Convert.ToInt32(row["ActivityDetailBaseQuantity"]),
                            ActivityDetailWeightQuantity = Convert.ToInt32(row["ActivityDetailWeightQuantity"]),
                            ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"]),
                            ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"]),
                            ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"]),
                            ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"]),
                            ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"]),
                            ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"]),
                            ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"]),
                            ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"]),
                            LocationCode = Convert.ToInt32(row["LocationCode"]),
                            LocationName = Convert.ToString(row["LocationName"]),
                            Location = Convert.ToString(row["Location"]),
                            ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"]),
                            ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"]),
                            ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"]),
                            ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"]),
                            IsBatchProductLabel = Convert.ToBoolean(row["IsBatchProductLabel"]),
                            ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"]),
                            ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"]),
                            ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"]),
                            ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"]),
                            ActivityDetailExtra5Text = Convert.ToString(row["ActivityDetailExtra5Text"])
                        };

                        this.DataGridMainActivityItemsList.Add(receiveDetail);
                    }
                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    break;

                case SysCommon.ActSubTypes.Returns:
                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    break;
            }
        }

        private void ApplySearch()
        {
            switch (this.ActDetail.ActSubType)
            {
                case SysCommon.ActSubTypes.Adjustment:
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    break;

                case SysCommon.ActSubTypes.Picking:
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    break;

                case SysCommon.ActSubTypes.Returns:
                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    break;
            }
        }

        #endregion
    }
}