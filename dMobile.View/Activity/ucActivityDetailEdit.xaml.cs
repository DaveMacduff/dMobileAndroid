using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Android.Views;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.Resources.Enums;
using dMobile.Resources.Types;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityDetailEdit : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ActDetailEdit actDetailEdit;
        private DataTable dataTableActDetails;
        private int selectedIndex = 0;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        //private UserControl callingControl = null;
        private DataView dataViewVariants = null;
        private DataView dataViewLots = null;
        private DataView dataViewSerialNumbers = null;
        private string fullClassName = "dMobile.ucActivityDetailEdit";
        private bool isNumericTrapped = false;
        private ActDetail actDetail = null;
        private bool isEditTrapped = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.NotApplicable;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList;
        private bool printLabels;

        #endregion

        #region Constructors

        public ucActivityDetailEdit()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityDetailEdit(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.lblQty.Text = "Quantity";
            this.lblUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblWtUnit.Text = "KG";
            this.lblSerialNo.Text = "Serial No";
            this.lblProductionDate.Text = "Production";
            this.lblLotNo.Text = "Lot No";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblVariantCode.Text = "Variant";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.ItemsList = new ObservableCollection<string>();
                this.SysEnv = SysEnvironment.GetInstance();

                this.ActSubType = actSubTypes;

                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.ActDetailEdit = new ActDetailEdit();
                this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;

                this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;

                this.ActDetailEdit.ActExtra.Text1 = this.ActDetail.ActExtra.Text1;
                this.ActDetailEdit.ActExtra.Text2 = this.ActDetail.ActExtra.Text2;
                this.ActDetailEdit.ActExtra.Text3 = this.ActDetail.ActExtra.Text3;
                this.ActDetailEdit.ActExtra.Text4 = this.ActDetail.ActExtra.Text4;
                this.ActDetailEdit.ActExtra.DateTime5 = this.ActDetail.ActExtra.DateTime5;

                if (!this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTrackedDefault)
                {
                    // Default to current date
                    this.dtBestBeforeDate.Date = DateTime.Now;
                }

                if (!this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTrackedDefault)
                {
                    // Default to current date
                    this.dtProductionDate.Date = DateTime.Now;
                }

                this.Title = this.GetControlDescription();
                this.lbl1.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra1Caption;
                this.lbl2.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra2Caption;
                this.lbl3.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra3Caption;
                this.lbl4.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra4Caption;
                this.lbl5.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra5Caption;

                this.txtAltQty.TextChanged += this.txtAltQty_TextChanged;
            }
        }

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        if (this.printLabels)
                        {
                            if (!this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                            {
                                // TODO: Resource strings
                                if (await DisplayAlert(this.GetControlDescription(), "Print product labels?", "Yes", "No"))
                                {
                                    try
                                    {
                                        await this.PrintProductLabels();
                                    }
                                    catch (Exceptions.MessageOnlyException ex)
                                    {
                                        // Even though an error was raised, during label printing allow the control to close.
                                        ex.Show(this.ActSubType);
                                    }
                                    catch (Exception ex)
                                    {
                                        // Even though an error was raised, during label printing allow the control to close.

                                        await Device.InvokeOnMainThreadAsync(async () =>
                                        {
                                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                                            await errRpt.ShowDialog();
                                        });
                                    }
                                }
                            }
                        }

                        this.DialogResult = DialogResult.OK;
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await this.Close();
                    }
                }
            });

            try
            {
                await this.ucActivityDetailEdit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityDetailEdit_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.ScannerDisable();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        #region Delegate

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

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

        public SysEnvironment SysEnv
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

        public SysCommon.ActSubTypes ActSubType
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

        public bool IsEditTrapped
        {
            get
            {
                return this.isEditTrapped;
            }

            set
            {
                this.isEditTrapped = value;
            }
        }

        public bool IsNumericTrapped
        {
            get
            {
                return this.isNumericTrapped;
            }

            set
            {
                this.isNumericTrapped = value;
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

        public DataView DataViewSerialNumbers
        {
            get
            {
                return this.dataViewSerialNumbers;
            }

            set
            {
                this.dataViewSerialNumbers = value;
            }
        }

        public DataView DataViewLots
        {
            get
            {
                return this.dataViewLots;
            }

            set
            {
                this.dataViewLots = value;
            }
        }

        public DataView DataViewVariants
        {
            get
            {
                return this.dataViewVariants;
            }

            set
            {
                this.dataViewVariants = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
            }
        }

        public DataTable DataTableActDetails
        {
            get
            {
                return this.dataTableActDetails;
            }

            set
            {
                this.dataTableActDetails = value;
            }
        }

        public ActDetailEdit ActDetailEdit
        {
            get
            {
                return this.actDetailEdit;
            }

            set
            {
                this.actDetailEdit = value;
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

        public bool IsChangesMade
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

        //public UserControl CallingControl
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

        public ActDetail ActDetail
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

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
            bool isReadyToSave = true;

            try
            {
                await this.IsValidData();
            }
            catch (IsReadyToApplyException ex)
            {
                if (ex.SuppressError && ex.IsOverContinue)
                {
                    isReadyToSave = true;
                }
                else if (ex.SuppressError && !ex.IsOverContinue)
                {
                    isReadyToSave = false;
                }
                else
                {
                    isReadyToSave = false;

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                //ex.Show(this.ActSubType);
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.ActSubType.ToString(), ex.Message, "OK"));
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
                    isReadyToClose = false;
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return null;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Detail Edit";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = true;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.printLabels = false;

                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving && this.SysEnv.SysSettings.CanPrintProductLabelReceiving)
                    {
                        this.printLabels = true;
                    }
                    else if ((this.ActSubType == SysCommon.ActSubTypes.TransferFrom || this.ActSubType == SysCommon.ActSubTypes.TransferTo) && this.SysEnv.SysSettings.CanPrintProductLabelTransfers)
                    {
                        this.printLabels = true;
                    }

                    this.IsChangesMade = false;
                }
                else
                {
                    saveComplete = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                //ex.Show(this.ActSubType);
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.ActSubType.ToString(), ex.Message, "OK"));
            }
            catch (Exception ex)
            {
                saveComplete = false;

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
                    this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                    this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                    this.ActDetailEdit.ActExtra.Text1 = this.ActDetail.ActExtra.Text1;
                    this.ActDetailEdit.ActExtra.Text2 = this.ActDetail.ActExtra.Text2;
                    this.ActDetailEdit.ActExtra.Text3 = this.ActDetail.ActExtra.Text3;
                    this.ActDetailEdit.ActExtra.Text4 = this.ActDetail.ActExtra.Text4;
                    this.ActDetailEdit.ActExtra.DateTime5 = this.ActDetail.ActExtra.DateTime5;
                }
                else
                {
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
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
        public XDocument GetXML()
        {
            return this.ActDetailEdit.GetXMLFromActivityDetailEdit(this.ActDetailEdit);
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
        private async Task PrintProductLabels()
        {
            string sendReq = string.Empty;
            string reqData = string.Empty;

            if (this.ActDetail.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferFrom || this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferTo)
            {
                bool editLabelBeforePrint = !this.ActDetail.IsLabelSchemeComplete(this.actDetailEdit.SchemeData) || SysEnv.SysSettings.IsProductLabelCatchAsk;

                if (editLabelBeforePrint)
                {
                    ActDetail actDetailLabel = ActDetailFacade.GetActDetail(this.ActDetail.ActItem, this.ActSubType);

                    actDetailLabel.Refno = this.ActDetail.Refno;
                    actDetailLabel.FullClassName = this.ActDetail.FullClassName;
                    actDetailLabel.ActItem = this.ActDetail.ActItem;
                    actDetailLabel.ActSubType = this.ActSubType;
                    actDetailLabel.FKRefno = this.ActDetail.FKRefno;
                    actDetailLabel.SelectedLocMap = this.ActDetail.SelectedLocMap;

                    actDetailLabel.Quantity = this.ActDetailEdit.Quantity;
                    actDetailLabel.BaseQuantity = this.ActDetailEdit.BaseQuantity;
                    actDetailLabel.WeightQuantity = this.ActDetailEdit.WeightQuantity;
                    actDetailLabel.SchemeData = this.ActDetailEdit.SchemeData;
                    actDetailLabel.ActExtra = this.ActDetailEdit.ActExtra;

                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityProductLabel, actDetailLabel, this.ActSubType);
                    await form.DisplayControl.ShowDialog();
                    // Printing of product labels will be handled in ucActivityProductLabel
                }
                else
                {
                    sendReq = this.ActDetail.CreateSubmitProductLabelXML(this.ActDetailEdit.GetXMLFromActivityDetailEdit(this.ActDetailEdit));
                    reqData = SysCommon.SubmitComm(sendReq);

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
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            ProductInfo prodInfo = this.ActDetail.ActItem.ProdInfo;

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                if (!SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
                {
                    this.dtBestBeforeDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate;
                }
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                if (!SysCommon.IsEmptyDate(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
                {
                    this.dtProductionDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitProductionDate;
                }
            }

            if (prodInfo.Scheme.IsSerialTracked)
            {
                if (string.IsNullOrEmpty(this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber))
                {
                    SProductSchemeData schemeData = this.ActDetailEdit.SchemeData;
                    schemeData.SingleTradeItemSerialNumber = ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                    this.ActDetailEdit.SchemeData = schemeData;
                }

                this.cboSerialNo.Text = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;

                //if (this.cboSerialNo.See < 0)
                //{
                //    this.cboSerialNo.SelectedItem = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;
                //}
            }

            if (prodInfo.Scheme.IsLotTracked)
            {
                this.cboLotNo.SelectedItem = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;

                if (this.cboLotNo.SelectedIndex < 0)
                {
                    this.cboLotNo.SelectedItem = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;
                }
            }

            if (prodInfo.Scheme.IsVariantTracked)
            {
                this.cboVariantCode.SelectedItem = this.ActDetailEdit.SchemeData.VariantNumber;
                ////if (this.cboVariantCode.SelectedIndex < 0)
                ////{
                ////  this.cboVariantCode.Text = this.ActDetailEdit.SchemeData.VariantCode + " " 
                ////+ this.ActDetailEdit.SchemeData.VariantDescription;
                ////}
            }

            // default quantities if zero
            if (this.ActDetail.Quantity == 0)
            {
                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.WeightQuantity != 0)
                    {
                        this.ActDetailEdit.Quantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.Quantity = prodInfo.UnitWeight;
                    }
                }
                else
                {
                    this.ActDetailEdit.Quantity = 1;
                }
            }
            else
            {
                this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
            }

            if (prodInfo.IsCatchAskZero)
            {
                this.ActDetailEdit.Quantity = 0;
            }

            if (this.ActDetail.WeightQuantity == 0)
            {
                if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.Quantity != 0)
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.Quantity;
                    }
                    else
                    {
                        if (!(this.SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                        {
                            this.ActDetailEdit.WeightQuantity = prodInfo.UnitWeight;
                        }
                    }
                }
                else
                {
                    if (prodInfo.BaseUnitCode == "KG" || prodInfo.BaseUnitCode == "LB")
                    {
                        if (!(this.SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                        {
                            this.ActDetailEdit.WeightQuantity = prodInfo.UnitWeight;
                        }
                    }
                    else
                    {
                        if (prodInfo.Unit2BaseUnitFactor != 0)
                        {
                            if (!(this.SysEnv.SysSettings.IsSetCatchWeightZero && prodInfo.IsCatchWeight))
                            {
                                this.ActDetailEdit.WeightQuantity = prodInfo.Unit2BaseUnitFactor * prodInfo.UnitWeight;
                            }
                        }
                        else
                        {
                            this.ActDetailEdit.WeightQuantity = prodInfo.UnitWeight;
                        }
                    }
                }
            }
            else
            {
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }

            if (prodInfo.IsCatchAskZero) // always ask for weight if catchaskzero enabled
            {
                this.ActDetailEdit.WeightQuantity = 0;
            }

            if (this.ActDetail.BaseQuantity == 0)
            {
                if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.WeightQuantity;
                }
                else
                {
                    if (this.ActDetail.Quantity != 0 && prodInfo.Unit2BaseUnitFactor != 0)
                    {
                        if (this.SysEnv.SysSettings.IsSetCatchCountZero && prodInfo.IsCatchCount)
                        {
                            this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity * prodInfo.Unit2BaseUnitFactor;
                        }
                    }
                    else
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            }

            if (prodInfo.IsCatchAskZero) // always ask for weight if catchaskzero enabled
            {
                this.ActDetailEdit.BaseQuantity = 0;
            }

            this.ActDetailEdit.Quantity = decimal.Round(this.ActDetailEdit.Quantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);

            this.txtAltQty.Text = this.ActDetailEdit.Quantity.ToString();
            this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();

            if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text1))
            {
                this.txtExtraDetail1.Text = this.ActDetail.ActExtra.Text1;
            }

            if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text2))
            {
                this.txtExtraDetail2.Text = this.ActDetail.ActExtra.Text2;
            }

            if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text3))
            {
                this.txtExtraDetail3.Text = this.ActDetail.ActExtra.Text3;
            }

            if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text4))
            {
                this.txtExtraDetail4.Text = this.ActDetail.ActExtra.Text4;
            }

            if (!SysCommon.IsEmptyDate(this.ActDetail.ActExtra.DateTime5))
            {
                this.dtExtraDetail5.Date = this.ActDetail.ActExtra.DateTime5;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        private void GetProductVariantList(int prodRefno)
        {
            this.DataViewVariants = this.ActDetailEdit.GetVariantDataView(prodRefno);
            ObservableCollection<object> productVariantList = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewVariants.ToTable().Rows)
            {
                ProductVariant productVariant = new ProductVariant()
                {
                    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                    ProductSchemeVariantDisplay = Convert.ToString(row["ProductSchemeVariantDisplay"] != DBNull.Value ? row["ProductSchemeVariantDisplay"] : null),
                };

                productVariantList.Add(productVariant);
            }

            this.cboVariantCode.ItemDisplayBinding = new Binding("ProductSchemeVariantDisplay");
            this.cboVariantCode.ItemsSource = productVariantList;

            //this.cboVariantCode.ValueMember = "ProductSchemeVariantNo";
            //this.cboVariantCode.DisplayMember = "ProductSchemeVariantDisplay";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        private void GetLotNumberList(int prodRefno)
        {
            this.DataViewLots = this.ActDetailEdit.GetLotDataView(prodRefno);
            ObservableCollection<object> lotNumberList = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewLots.ToTable().Rows)
            {
                LotNumber lotNumber = new LotNumber()
                {
                    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null)
                };

                lotNumberList.Add(lotNumber);
            }

            this.cboLotNo.ItemDisplayBinding = new Binding("ActivityDetailLogisticUnitBatchLotNumber");
            this.cboLotNo.ItemsSource = lotNumberList;

            //this.cboLotNo.ValueMember = "ActivityDetailLogisticUnitBatchLotNumber";
            //this.cboLotNo.DisplayMember = "ActivityDetailLogisticUnitBatchLotNumber";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodRefno"></param>
        private void GetSerialNumberList(int prodRefno)
        {
            this.DataViewSerialNumbers = this.ActDetailEdit.GetSerialNumberDataView(prodRefno);
            ObservableCollection<SerialNumber> serialNumberList = new ObservableCollection<SerialNumber>();
            string selectedSerialNumberItem = this.cboSerialNo.Text;
            SerialNumber selectedSerialNumber = new SerialNumber();

            if (!string.IsNullOrEmpty(selectedSerialNumberItem))
            {
                selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(selectedSerialNumberItem);
                serialNumberList.Add(selectedSerialNumber);
            }

            foreach (DataRow row in this.DataViewSerialNumbers.ToTable().Rows)
            {
                SerialNumber serialNumber = new SerialNumber()
                {
                    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null)
                };

                serialNumberList.Add(serialNumber);
            }

            //this.cboSerialNo.ItemDisplayBinding = new Binding("ActivityDetailSingleTradeItemSerialNumber");
            //this.cboSerialNo.ItemsSource = serialNumberList;

            if (selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber != null)
            {
                this.cboSerialNo.Text = selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber;
            }

            //this.cboSerialNo.ValueMember = "ActivityDetailSingleTradeItemSerialNumber";
            //this.cboSerialNo.DisplayMember = "ActivityDetailSingleTradeItemSerialNumber";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task IsValidData()
        {
            SProductSchemeData s = this.ActDetailEdit.SchemeData;

            s.SingleTradeItemSerialNumber = this.cboSerialNo.Text;
            s.LogisticUnitBatchLotNumber = this.cboLotNo.Text;

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked)
            {
                ProductVariant variant = this.cboVariantCode != null ? this.cboVariantCode.SelectedItem as ProductVariant : null;
                s.VariantNumber = Convert.ToInt32(variant != null ? variant.ProductSchemeVariantNo : -1);
                s = this.ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(s);
            }

            s.LogisticUnitProductionDate = this.dtProductionDate.Date;
            s.LogisticUnitBestBeforeSellByDate = this.dtBestBeforeDate.Date;
            this.ActDetailEdit.SchemeData = s;

            this.ActDetailEdit.ActExtra.Text1 = this.txtExtraDetail1.Text ?? string.Empty;
            this.ActDetailEdit.ActExtra.Text2 = this.txtExtraDetail2.Text ?? string.Empty;
            this.ActDetailEdit.ActExtra.Text3 = this.txtExtraDetail3.Text ?? string.Empty;
            this.ActDetailEdit.ActExtra.Text4 = this.txtExtraDetail4.Text ?? string.Empty;
            this.ActDetailEdit.ActExtra.DateTime5 = this.dtExtraDetail5.Date;

            // Keyboard is numeric only
            //if (!Information.IsNumeric(txtAltQty.Text))
            //{
            //    // TODO: Resource strings
            //    throw new Exceptions.MessageOnlyException("Invalid quantity.");
            //}
            //else if (!Information.IsNumeric(txtWtQty.Text))
            //{
            //    // TODO: Resource strings
            //    throw new Exceptions.MessageOnlyException("Invalid weight quantity.");
            //}
            //else
            //{
            this.ActDetailEdit.Quantity = decimal.Parse(this.txtAltQty.Text);
            this.ActDetailEdit.WeightQuantity = decimal.Parse(this.txtWtQty.Text);

            if (0 != Math.Sign(this.ActDetailEdit.WeightQuantity) && 0 != Math.Sign(this.ActDetailEdit.Quantity))
            {
                if (Math.Sign(this.ActDetailEdit.Quantity) != Math.Sign(this.ActDetailEdit.WeightQuantity))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("The sign of quantities must be the same.");
                }
            }

            if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.WeightQuantity;
            }
            else
            {
                if (this.ActDetailEdit.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity;
                }
            }
            //}

            if (decimal.Parse(this.txtAltQty.Text) == 0 && decimal.Parse(this.txtWtQty.Text) == 0)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Both quantity and weight quantity cannot be zero.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsSerialTracked && this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber == string.Empty)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Serial number required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsLotTracked && this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Lot number required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked && SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Best before date required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked && this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                DateTime lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                DateTime upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                if (this.dtBestBeforeDate.Date < lower || this.dtBestBeforeDate.Date > upper)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("The entered best before date is outside an acceptable range.");
                }
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked && SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Production date required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked && this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                DateTime lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                DateTime upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                if (this.dtProductionDate.Date < lower || this.dtProductionDate.Date > upper)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("The entered production date is outside an acceptable range.");
                }
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked && this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Variant code required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.Extra1Type != (int)SysCommon.SchemeExtraTypes.Disabled && this.ActDetail.ActItem.ProdInfo.Scheme.Extra1IsRequired && string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text1))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException(this.ActDetail.ActItem.ProdInfo.Scheme.Extra1Caption.TrimEnd() + " is required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.Extra2Type != (int)SysCommon.SchemeExtraTypes.Disabled && this.ActDetail.ActItem.ProdInfo.Scheme.Extra2IsRequired && string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text2))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException(this.ActDetail.ActItem.ProdInfo.Scheme.Extra2Caption.TrimEnd() + " is required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.Extra3Type != (int)SysCommon.SchemeExtraTypes.Disabled && this.ActDetail.ActItem.ProdInfo.Scheme.Extra3IsRequired && string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text3))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException(this.ActDetail.ActItem.ProdInfo.Scheme.Extra3Caption.TrimEnd() + " is required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.Extra4Type != (int)SysCommon.SchemeExtraTypes.Disabled && this.ActDetail.ActItem.ProdInfo.Scheme.Extra4IsRequired && string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text4))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException(this.ActDetail.ActItem.ProdInfo.Scheme.Extra4Caption.TrimEnd() + " is required.");
            }

            if (this.ActDetail.ActItem.ProdInfo.Scheme.Extra5Type != (int)SysCommon.SchemeExtraTypes.Disabled && this.ActDetail.ActItem.ProdInfo.Scheme.Extra5IsRequired && SysCommon.IsEmptyDate(this.ActDetailEdit.ActExtra.DateTime5))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException(this.ActDetail.ActItem.ProdInfo.Scheme.Extra5Caption.TrimEnd() + " is required.");
            }

            this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            this.ActDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
            XDocument xmlParam = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + ("<itm_refno>" + this.ActDetail.ActItem.FKRefno + "</itm_refno>")
                + ("<alt_qty>" + this.ActDetailEdit.Quantity + "</alt_qty>")
                + ("<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>")
                + ("<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                + "</imclient>");

            await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            this.ItemsList.Clear();

            // TODO: Resource strings
            this.ItemsList.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ItemsList.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                this.Loading = true;

                if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
                {
                    if (!this.IsControlLoaded)
                    {
                        ProductInfo prodInfo = this.ActDetail.ActItem.ProdInfo;
                        ProdScheme scheme = prodInfo.Scheme;

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.lblUnit.Text = prodInfo.UnitCode;
                            this.lblWtUnit.Text = prodInfo.WeightUnitCode;
                        });

                        if (!scheme.IsBestBeforeDateTracked)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.dtBestBeforeDate.IsEnabled = false);
                        }

                        if (!scheme.IsProductionDateTracked)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.dtProductionDate.IsEnabled = false);
                        }

                        if (!scheme.IsLotTracked)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cboLotNo.IsEnabled = false);
                        }

                        if (!scheme.IsSerialTracked)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cboSerialNo.IsEnabled = false);
                        }

                        if (!scheme.IsVariantTracked)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cboVariantCode.IsEnabled = false);
                        }

                        if (!prodInfo.IsCatchWeight || (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG"))
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.txtWtQty.IsEnabled = false);
                        }

                        if (scheme.Extra1Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.txtExtraDetail1.IsEnabled = false);
                        }

                        if (scheme.Extra2Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.txtExtraDetail2.IsEnabled = false);
                        }

                        if (scheme.Extra3Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.txtExtraDetail3.IsEnabled = false);
                        }

                        if (scheme.Extra4Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.txtExtraDetail4.IsEnabled = false);
                        }

                        if (scheme.Extra5Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.dtExtraDetail5.IsEnabled = false);
                        }

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.LoadData();
                            this.ShowProductInfo();

                            this.GetProductVariantList(prodInfo.Refno);
                            this.GetSerialNumberList(prodInfo.Refno);
                            this.GetLotNumberList(prodInfo.Refno);
                        });

                        if (this.SysEnv.SysSettings.IsDetailEditScreenSetFocus2CatchWeight)
                        {
                            if (prodInfo.IsCatchWeight)
                            {
                                //this.txtWtQty.Focus();
                            }
                            else
                            {
                                //this.txtAltQty.Focus();
                            }
                        }
                        else
                        {
                            if (scheme.IsSerialTracked && prodInfo.IsCatchWeight)
                            {
                                //this.txtWtQty.Focus();
                            }
                            else
                            {
                                //this.txtAltQty.Focus();
                            }
                        }
                    }

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = true;
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void TxtAltQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void TxtWtQty_GotFocus(object sender, EventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            this.IsChangesMade = true;
        }

        #endregion

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                if (this.printLabels)
                {
                    if (!this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                    {
                        // TODO: Resource strings
                        if (await DisplayAlert(this.GetControlDescription(), "Print product labels?", "Yes", "No"))
                        {
                            try
                            {
                                await this.PrintProductLabels();
                            }
                            catch (Exceptions.MessageOnlyException ex)
                            {
                                // Even though an error was raised, during label printing allow the control to close.
                                ex.Show(this.ActSubType);
                            }
                            catch (Exception ex)
                            {
                                // Even though an error was raised, during label printing allow the control to close.

                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                                    await errRpt.ShowDialog();
                                });
                            }
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private void txtAltQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal qtyEntered = 0;
            decimal wtQty;

            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQty.Text))
                {
                    int.TryParse(this.txtAltQty.Text, out int result);
                    qtyEntered = result;
                }
            }
            catch
            {
            }

            if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
            {
                // if the unit code is in LBs or KGs then just use that value
                wtQty = qtyEntered;
            }
            else
            {
                // Otherwise the weigth = quantity * weightPerOneQty
                wtQty = qtyEntered * this.ActDetail.ActItem.ProdInfo.UnitWeight;
            }

            this.txtWtQty.Text = Convert.ToString(Math.Round(wtQty, 3));
        }
    }
}