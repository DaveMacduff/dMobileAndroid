using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.CustomPages.Modal;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityRemoveFromSkid : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        private ActDetailEdit actDetailEdit;
        private DataTable dataTableActDetail;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        //private UserControl callingControl = null;
        private bool isEnterTrapped = false;
        private DataView dataViewVariants = null;
        private DataView dataViewLots = null;
        private DataView dataViewSerialNumbers = null;
        private bool isNumericTrapped = false;
        private bool isEscapeTrapped = false;
        private ActDetail actDetail = null;
        private bool isEditTrapped = false;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityRemoveFromSkid(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.lblWtUnit.Text = "KG";
            this.lblUnit.Text = "Bx";
            this.lblWeight.Text = "Weight";
            this.lblQty.Text = "Quantity";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();

                this.ActSubType = actSubTypes;
                this.ActDetailEdit = new ActDetailEdit();
                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }
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
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await this.CancelChanges();
                        }

                        await this.Close();
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await this.CancelChanges();
                        }
                    }
                }
            });

            try
            {
                this.ucActivityRemoveFromSkid_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityRemoveFromSkid_GotFocus threw an error " + ex, "OK");
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

        private bool IsEditTrapped
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

        private bool IsEscapeTrapped
        {
            get
            {
                return this.isEscapeTrapped;
            }

            set
            {
                this.isEscapeTrapped = value;
            }
        }

        private bool IsNumericTrapped
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

        private DataView DataViewSerialNumbers
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

        private DataView DataViewLots
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

        private DataView DataViewVariants
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

        private DataTable DataTableActDetail
        {
            get
            {
                return this.dataTableActDetail;
            }

            set
            {
                this.dataTableActDetail = value;
            }
        }

        private ActDetailEdit ActDetailEdit
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }

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
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;// (decimal.Parse(this.txtAltQty.Text) != 0) && this.IsValidData();
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
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
            return "Remove From Skid";
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
                    saveComplete = this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    await DisplayAlert(this.GetControlDescription(), "Data failed validation. Check your data.", "OK");
                }
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
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

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
        public XDocument GetXML()
        {
            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<alt_qty>" + this.ActDetailEdit.Quantity + "</alt_qty>")
                + ("<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>")
                + "</params>");

            return request;
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
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            bool isRetVal = false;

            decimal qty = decimal.Parse(this.txtAltQty.Text);
            decimal lineQty = this.ActDetail.Quantity;
            // We assign how much we want to remove
            decimal newQty = qty;
            string baseUnit = this.ActDetail.ActItem.ProdInfo.BaseUnitCode;
            decimal newWtQty = decimal.Parse(this.txtWtQty.Text);
            decimal newBaseQty = this.ActDetail.BaseQuantity;

            if (baseUnit == "LB" || baseUnit == "KG")
            {
                newBaseQty = newWtQty;
            }
            else
            {
                if (this.ActDetailEdit.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    newBaseQty = newQty * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    newBaseQty = newQty;
                }
            }

            if (qty == lineQty)
            {
                this.ActDetailEdit.Quantity = newQty;
                this.ActDetailEdit.WeightQuantity = newWtQty;
                this.ActDetailEdit.BaseQuantity = newBaseQty;

                isRetVal = true;
            }
            else if (qty == 0)
            {
                // Exit without doing anything
                isRetVal = true;
            }
            else if (qty > lineQty)
            {
                DisplayAlert(this.GetControlDescription(), "Cannot remove more than exists on skid!", "OK");
            }
            else if (qty < 0)
            {
                DisplayAlert(this.GetControlDescription(), "Cannot remove negative quantity!", "OK");
            }
            else
            {
                if (Convert.ToInt32(qty) != qty)
                {
                    DisplayAlert(this.GetControlDescription(), "Cannot remove partial boxes!", "OK");
                }
                else
                {
                    // Good
                    this.ActDetailEdit.Quantity = newQty;
                    this.ActDetailEdit.WeightQuantity = newBaseQty;
                    this.ActDetailEdit.BaseQuantity = newBaseQty;
                    isRetVal = true;
                }
            }

            return isRetVal;
        }

        private async Task PromptConsumeSkid()
        {
            // TODO: Resource strings
            if (await DisplayAlert(this.GetControlDescription(), "Do You Wish To Consume This Entire Skid?", "Yes", "No"))
            {
                this.ActDetail.Delete();

                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.IsConsumed)
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), "This skid is now consumed.", "OK");

                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                        await this.CancelChanges();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySummary()
        {
            SSkidInfo skid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

            // TODO: Resource strings
            this.ItemsList.Clear();
            this.ItemsList.Add("Product: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " + this.ActDetail.ActItem.ProdInfo.Desc);
            this.ItemsList.Add(string.Format("Production Date: {0}", this.ActDetail.SchemeData.LogisticUnitProductionDate));
            this.ItemsList.Add(string.Format("Quantity {0} {1} @ {2} {3}", this.ActDetail.Quantity, skid.ProdInfo.UnitCode, this.ActDetail.WeightQuantity, skid.ProdInfo.WeightUnitCode));
            this.ItemsList.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            this.ActDetailEdit.Quantity = 0;
            this.ActDetailEdit.BaseQuantity = 0;

            this.txtAltQty.Text = "0";
            this.txtWtQty.Text = "0";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityRemoveFromSkid_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                    this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                    this.lblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                    if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight ||
                        (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" ||
                        this.ActDetail.ActItem.ProdInfo.UnitCode == "KG"))
                    {
                        // disable when not catch weight, or when qty is LB/KG
                        this.txtWtQty.IsEnabled = false;
                    }

                    this.LoadData();
                    this.DisplaySummary();

                    this.txtAltQty.Focus();
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtAltQty_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            decimal qtyEntered = 0;
            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQty.Text))
                {
                    qtyEntered = Convert.ToDecimal(this.txtAltQty.Text);
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

        #endregion

        private void txtWtQty_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {

        }

        private void txtAltQty_Focused(object sender, FocusEventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        private void txtWtQty_Focused(object sender, FocusEventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }
    }
}