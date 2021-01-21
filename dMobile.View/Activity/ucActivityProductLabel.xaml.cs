using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityProductLabel : DMobileModal, IAppArea, IAppPublisher
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
        private bool isNumericTrapped = false;
        private ActDetail actDetail = null;

        /// <summary>
        /// Indicates ucActivityProductLabel was called from a activity item user control.
        /// </summary>
        private bool isEditTrapped = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.PrintProductLabel;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        #endregion

        #region Constructors

        public ucActivityProductLabel()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblVariantCode.Text = "Variant";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblLotNo.Text = "LotNo";
            this.lblProductionDate.Text = "Production";
            this.lblSerialNo.Text = "Serial No";
            this.cmdSelectProduct.Text = "Select Product";
            this.lblQuantity.Text = "Quantity";

            this.SysEnv = SysEnvironment.GetInstance();
            Controller.Activity act = ActivityFacade.GetActivity(this.ActSubType);
            ActItem actItem = ActItemFacade.GetActItem(act, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(actItem, this.ActSubType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityProductLabel(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblVariantCode.Text = "Variant";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblLotNo.Text = "LotNo";
            this.lblProductionDate.Text = "Production";
            this.lblSerialNo.Text = "Serial No";
            this.cmdSelectProduct.Text = "Select Product";
            this.lblQuantity.Text = "Quantity";

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
        }

        #endregion

        #region OnAppearing/OnDisappearing

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

                if (!this.Loading)
                {
                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                            await this.Close();
                        }
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
                        {
                            this.DialogResult = DialogResult.None;
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                }
            });

            try
            {
                this.ucActivityProductLabel_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityProductLabel_GotFocus threw an error " + ex, "OK");
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
                isReadyToSave = await this.IsValidData();
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
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
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
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
            return "Product Label Edit";
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
                    // TODO: Resource strings
                    if (await DisplayAlert(this.GetControlDescription(), "Print product labels?", "Yes", "No"))
                    {
                        string sendReq = this.ActDetail.CreateSubmitProductLabelXML(this.GetXML());
                        string strData = SysCommon.SubmitComm(sendReq);

                        if (strData.Length > 0)
                        {
                            if (strData.Contains("<message>ERROR</message>"))
                            {
                                XmlDocument docError = new XmlDocument();
                                docError.LoadXml(strData);
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

                        this.IsChangesMade = false;
                        saveComplete = true;
                    }
                }
                else
                {
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
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void IdentifyChangesMade(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (this.IsControlLoaded)
        //        {
        //            if (sender == this.txtAltQuantity)
        //            {
        //                this.isChangesMade = true;
        //            }

        //            if (sender == this.txtWtQty)
        //            {
        //                this.IsChangesMade = true;
        //            }

        //            if (sender == this.cboLotNo)
        //            {
        //                this.IsChangesMade = true;
        //            }

        //            if (sender == this.cboSerialNo)
        //            {
        //                this.IsChangesMade = true;
        //            }

        //            if (sender == this.cboVariantCode)
        //            {
        //                this.IsChangesMade = true;
        //            }

        //            if (sender == this.dtBestBeforeDate)
        //            {
        //                this.IsChangesMade = true;
        //            }

        //            if (sender == this.dtProductionDate)
        //            {
        //                this.IsChangesMade = true;
        //            }
        //        }
        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
        //        // Allow the validation to handle the setting for IsChangesMade.
        //        ex.Show(this, this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.IsChangesMade = true;
        //        using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
        //        {
        //            errRpt.ShowDialog();
        //        }
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtWtQty_GotFocus(object sender, EventArgs e)
        {
            //this.txtWtQty.SelectAll();

            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }

        private void txtQuantity_GotFocus(object sender, EventArgs e)
        {
            //this.txtAltQuantity.SelectAll();

            if (this.txtAltQuantity.Text != null)
            {
                this.txtAltQuantity.CursorPosition = 0;
                this.txtAltQuantity.SelectionLength = this.txtAltQuantity.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityProductLabel_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    ProductInfo prodInfo = this.ActDetail.ActItem.ProdInfo;
                    ProdScheme scheme = prodInfo.Scheme;

                    this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(prodInfo);

                    this.lblWtUnit.Text = prodInfo.WeightUnitCode;

                    if (!(scheme.IsBestBeforeDateTracked || scheme.LabelInfo.IsBestBeforeDateTracked))
                    {
                        this.dtBestBeforeDate.IsEnabled = false;
                    }

                    if (!(scheme.IsProductionDateTracked || scheme.LabelInfo.IsProductionDateTracked))
                    {
                        this.dtProductionDate.IsEnabled = false;
                    }

                    if (!(scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked))
                    {
                        this.cboLotNo.IsEnabled = false;
                    }

                    if (!(scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked))
                    {
                        this.cboSerialNo.IsEnabled = false;
                    }

                    if (!scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
                    {
                        this.cboVariantCode.IsEnabled = false;
                    }

                    if (!prodInfo.IsCatchWeight || (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG"))
                    {
                        this.txtWtQty.IsEnabled = false;
                    }

                    this.GetProductVariantList(prodInfo.Refno);
                    this.GetSerialNumberList(prodInfo.Refno);
                    this.GetLotNumberList(prodInfo.Refno);

                    this.LoadData();
                    this.ShowProductInfo();

                    this.IsControlLoaded = true;

                    this.txtAltQuantity.Focus();
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            ProductInfo prodInfo = this.ActDetail.ActItem.ProdInfo;
            ProdScheme scheme = prodInfo.Scheme;

            if (scheme.IsBestBeforeDateTracked || scheme.LabelInfo.IsBestBeforeDateTracked)
            {
                if (!SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
                {
                    this.dtBestBeforeDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate;
                }
            }

            if (scheme.IsProductionDateTracked || scheme.LabelInfo.IsProductionDateTracked)
            {
                if (!SysCommon.IsEmptyDate(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
                {
                    this.dtProductionDate.Date = this.ActDetailEdit.SchemeData.LogisticUnitProductionDate;
                }
            }

            if (scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked)
            {
                if (string.IsNullOrEmpty(this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber))
                {
                    SProductSchemeData schemeData = this.ActDetailEdit.SchemeData;
                    schemeData.SingleTradeItemSerialNumber =
                        ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                    this.ActDetailEdit.SchemeData = schemeData;
                }

                this.cboSerialNo.SelectedItem = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;
                
                //if (this.cboSerialNo.SelectedIndex < 0)
                //{
                //    this.cboSerialNo.SelectedItem = this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber;
                //}
            }

            if (scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked)
            {
                this.cboLotNo.SelectedItem = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;
                
                //if (this.cboLotNo.SelectedIndex < 0)
                //{
                //    this.cboLotNo.Text = this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber;
                //}
            }

            if (scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
            {
                this.cboVariantCode.SelectedItem = this.ActDetailEdit.SchemeData.VariantNumber;
                
                //if (this.cboVariantCode.SelectedIndex < 0)
                //{
                //    this.cboVariantCode.Text = this.ActDetailEdit.SchemeData.VariantNumber.ToString();
                //}
            }

            // Default quantities if zero
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
                        this.ActDetailEdit.WeightQuantity = prodInfo.UnitWeight;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
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

            this.ActDetailEdit.Quantity = decimal.Round(this.ActDetailEdit.Quantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);

            this.txtAltQuantity.Text = this.ActDetailEdit.Quantity.ToString();
            this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString();
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
            object selectedSerialNumberItem = this.cboSerialNo.SelectedItem;
            SerialNumber selectedSerialNumber = new SerialNumber();

            if (selectedSerialNumberItem != null)
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

            this.cboSerialNo.ItemDisplayBinding = new Binding("ActivityDetailSingleTradeItemSerialNumber");
            this.cboSerialNo.ItemsSource = serialNumberList;

            if (selectedSerialNumber.ActivityDetailSingleTradeItemSerialNumber != null)
            {
                this.cboSerialNo.SelectedItem = selectedSerialNumber;
            }

            //this.cboSerialNo.ValueMember = "ActivityDetailSingleTradeItemSerialNumber";
            //this.cboSerialNo.DisplayMember = "ActivityDetailSingleTradeItemSerialNumber";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<bool> IsValidData()
        {
            bool isRetVal = true;
            ProdScheme scheme = this.ActDetail.ActItem.ProdInfo.Scheme;

            SProductSchemeData s = this.ActDetailEdit.SchemeData;
            SerialNumber serialNumber = this.cboSerialNo.SelectedItem != null ? this.cboSerialNo.SelectedItem as SerialNumber : null;
            LotNumber lotNumber = this.cboLotNo.SelectedItem != null ? this.cboLotNo.SelectedItem as LotNumber : null; 

            s.SingleTradeItemSerialNumber = serialNumber != null ? serialNumber.ActivityDetailSingleTradeItemSerialNumber : string.Empty;
            s.LogisticUnitBatchLotNumber = lotNumber != null ? lotNumber.ActivityDetailLogisticUnitBatchLotNumber : string.Empty;

            if (scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked)
            {
                ProductVariant variant = this.cboVariantCode != null ? this.cboVariantCode.SelectedItem as ProductVariant : null;
                s.VariantNumber = Convert.ToInt32(variant != null ? variant.ProductSchemeVariantNo : -1);
                s = this.ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(s);
            }

            s.LogisticUnitProductionDate = this.dtProductionDate.Date;
            s.LogisticUnitBestBeforeSellByDate = this.dtBestBeforeDate.Date;
            this.ActDetailEdit.SchemeData = s;

            this.ActDetailEdit.Quantity = decimal.Parse(this.txtAltQuantity.Text);
            
            if (this.ActDetailEdit.Quantity == 0)
            {
                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Invalid Quantity.", "OK");
                isRetVal = false;
            }

            // handled in xaml Keyboard="Numeric"
            //if (!Information.IsNumeric(txtWtQty.Text))
            //{
            //    MessageBox.Show("Invalid weight quantity.",this.GetControlDescription(),MessageBoxButtons.OK,MessageBoxIcon.Hand,MessageBoxDefaultButton.Button1);
            //    isRetVal = false;
            //}
            //else
            //{
                this.ActDetailEdit.WeightQuantity = decimal.Parse(this.txtWtQty.Text);

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

            ////if (decimal.Parse(this.txtWtQty.Text) == 0)
            ////{
            ////  throw new Exception("Weight quantity cannot be zero.");
            ////}

            if (isRetVal)
            {
                if ((scheme.IsSerialTracked || scheme.LabelInfo.IsSerialTracked) && this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber == string.Empty)
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Serial number required.", "OK");
                    isRetVal = false;
                }
            }

            if (isRetVal)
            {
                if ((scheme.IsLotTracked || scheme.LabelInfo.IsLotTracked) && this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
                {   
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Lot number required.", "OK");
                    isRetVal = false;
                }
            }

            if (isRetVal)
            {
                if ((scheme.IsVariantTracked || scheme.LabelInfo.IsVariantTracked) && this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "Variant code required.", "OK");
                    isRetVal = false;
                }
            }

            if (scheme.IsBestBeforeDateTracked && SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Best before date required.");
            }

            if (this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                DateTime lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                DateTime upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);
                
                if (this.dtBestBeforeDate.Date < lower || this.dtBestBeforeDate.Date > upper)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("The entered best before date is outside an acceptable range.");
                }
            }

            if (scheme.IsProductionDateTracked && SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Production date required.");
            }

            if (this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
            {
                DateTime lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                DateTime upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);
                
                if (this.dtProductionDate.Date < lower || this.dtProductionDate.Date > upper)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("The entered production date is outside an acceptable range.");
                }
            }

            if (isRetVal)
            {
                this.ActDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Code,
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                XDocument xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + ("<itm_refno>" + this.ActDetail.ActItem.FKRefno + "</itm_refno>")
                    + ("<alt_qty>" + this.ActDetailEdit.Quantity + "</alt_qty>")
                    + ("<wt_qty>" + this.ActDetailEdit.Quantity + "</wt_qty>")
                    + ("<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                    + "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
            }

            return isRetVal;
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
        private async void cmdSelectProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl causes a click event to fire if it is the next control
                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == dMobile.Resources.Enums.DialogResult.OK)
                {
                    XDocument xmlDoc = form.DisplayControlData;
                    int prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);
                    
                    if (prodUnitRefno != 0)
                    {
                        ProductInfo prodInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ActDetail.ActItem.ProdInfo = prodInfo;

                        this.cmdSelectProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(prodInfo);
                    }
                }
            }
        }

        #endregion

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            this.IsChangesMade = true;

            decimal qtyEntered = 0;
            
            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQuantity.Text))
                {
                    qtyEntered = Convert.ToDecimal(this.txtAltQuantity.Text);
                }
            }
            catch
            {
            }

            decimal wtQty;
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

            this.txtWtQty.Text = Math.Round(wtQty, 3).ToString();
        }
    }
}