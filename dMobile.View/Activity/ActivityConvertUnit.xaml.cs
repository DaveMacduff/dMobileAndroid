using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.View.Forms;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using dMobile.Resources.Types;
using dMobile.View.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertUnit : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucInventoryMenu();
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private int fromProdUnitRefno = 0;
        private int toProdUnitRefno = 0;
        //private bool isNumericTrapped = false;
        //private bool isEscapeTrapped = false;
        //private bool isEditTrapped = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.ConvertUnit;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public ucActivityConvertUnit()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectToUnit.Text = "Select To Unit";
            this.cmdSelectFromUnit.Text = "Select From Unit";
            this.lblConvertedUnitInfo.Text = "Converted Qty 0 UOM @ 0 KG/LB";
            this.lblFullUnitInfo.Text = "Qty 0 UOM @ 0 KG/LB";
            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

                this.updnBarCodeModifier1.Items.Clear();

                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.updnBarCodeModifier1.Items.Add(string.Empty);
                    string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                    foreach (string item in modifiers)
                    {
                        this.updnBarCodeModifier1.Items.Add(item);
                    }
                }
                else
                {
                    this.updnBarCodeModifier1.IsVisible = false;
                    this.updnBarCodeModifier1.IsEnabled = false;
                }

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertUnit);
                this.Activity.SkidInfo = new SysSkidInfo();
                this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertUnit;

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.ConvertUnit);
                this.ActItem.ActSubType = this.Activity.ActSubType;
                this.ActItem.GetDataView(false);

                this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertUnit);
                this.ActDetail.ActSubType = this.ActItem.ActSubType;

                this.DesignObservableCollection();

                //this.txtGridEdit.IsVisible = false;
                //this.dgUnitWeight.Controls.Add(this.txtGridEdit);

                this.Title = this.GetControlDescription();
            }
        }

        private void DesignObservableCollection()
        {
            if (this.ActDetail.DataViewDetails == null)
            {
                this.ActDetail.GetDataView(false);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dgUnitWeight);
                this.dgUnitWeight.ItemsSource = this.DataGridMainActivityItemsList;

                if (this.DataGridMainActivityItemsList.Count > 0)
                {
                    this.dgUnitWeight.SelectedItem = this.DataGridMainActivityItemsList[0];
                }
            });
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

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await this.Close();
                    }
                }
                else if (keycode == Keycode.F2)
                {
                    if (this.dgUnitWeight.SelectedItem is ConvertUnitDetail convertUnitDetail)
                    {
                        ucActivityConvertUnitWeightEdit activityConvertUnitWeightEdit = new ucActivityConvertUnitWeightEdit(convertUnitDetail);

                        if (await activityConvertUnitWeightEdit.ShowDialog() == DialogResult.OK && activityConvertUnitWeightEdit.WeightHasChanged)
                        {
                            int index = this.DataGridMainActivityItemsList.IndexOf(this.dgUnitWeight.SelectedItem);
                            this.ActDetail.DataViewDetails[index]["ActivityDetailWeightQuantity"] = activityConvertUnitWeightEdit.EditedWeight;
                            this.DisplayData();
                            this.DesignObservableCollection();
                        }
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    await this.CancelChanges();
                }
            });

            try
            {
                await this.ucActivityConvertUnit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityConvertUnit_GotFocus threw an error " + ex, "OK"));
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

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

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

        private ActItem ActItem
        {
            get
            {
                return this.actItem;
            }

            set
            {
                this.actItem = value;
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

        private bool IsBarCodeHandlerRegistered
        {
            get
            {
                return this.isBarCodeHandlerRegistered;
            }

            set
            {
                this.isBarCodeHandlerRegistered = value;
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

        private int FromProdUnitRefno
        {
            get
            {
                return this.fromProdUnitRefno;
            }

            set
            {
                this.fromProdUnitRefno = value;
            }
        }

        private int ToProdUnitRefno
        {
            get
            {
                return this.toProdUnitRefno;
            }

            set
            {
                this.toProdUnitRefno = value;
            }
        }

        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

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

        //private bool IsNumericTrapped
        //{
        //    get
        //    {
        //        return this.isNumericTrapped;
        //    }

        //    set
        //    {
        //        this.isNumericTrapped = value;
        //    }
        //}

        //private bool IsEscapeTrapped
        //{
        //    get
        //    {
        //        return this.isEscapeTrapped;
        //    }

        //    set
        //    {
        //        this.isEscapeTrapped = value;
        //    }
        //}

        //private bool IsEditTrapped
        //{
        //    get
        //    {
        //        return this.isEditTrapped;
        //    }

        //    set
        //    {
        //        this.isEditTrapped = value;
        //    }
        //}

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        /// Must have a try catch block.</remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Location must be set.");
                }

                if (0 >= this.FromProdUnitRefno)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Convert from product must be set.");
                }

                if (0 >= this.ActItem.DataViewItems.Count)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("No convert to items have be defined.");
                }

                if (0 >= this.ActDetail.DataViewDetails.Count)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("No convert to item details have be defined.");
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
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
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
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
                {
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
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
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
        //    return new ucInventoryMenu();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Convert Unit";
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
                    if (this.FromProdUnitRefno != 0 && this.ActItem.DataViewItems.Count != 0 && this.ActDetail.DataViewDetails.Count != 0)
                    {
                        string dataReq = SysCommon.SubmitComm(this.MakeConvertUnitSubmitString());

                        if (dataReq.Length > 0)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(dataReq);

                            if (dataReq.Contains("<message>ERROR</message>"))
                            {
                                string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                                if (errorDetail.Contains("MSGBOX-"))
                                {
                                    throw new Exceptions.MessageOnlyException(errorDetail);
                                }
                                else
                                {
                                    throw new Exception(errorDetail);
                                }
                            }
                            else
                            {
                                if (this.SysEnv.SysSettings.CanPrintProductLabelConvertUnits)
                                {
                                    //if (MessageBox.Show("Unit conversion complete." + Environment.NewLine + "Do you wish to print product labels?", "Unit Conversion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                    if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Unit Conversion", "Unit conversion complete." + Environment.NewLine + "Do you wish to print product labels?", "Yes", "No")))
                                    {
                                        for (int idx = 0; idx < ActDetail.DataViewDetails.Count; idx++)
                                        {
                                            dataReq = SysCommon.SubmitComm(this.MakeProductLabelsSubmitString(idx));

                                            if (dataReq.Contains("<message>ERROR</message>"))
                                            {
                                                xmlDoc.LoadXml(dataReq);
                                                string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

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
                                else
                                {
                                    // TODO: Resource strings
                                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Unit Conversion", "Unit conversion complete.", "OK"));
                                }
                            }

                            await this.ClearContents();
                        }

                        saveComplete = true;
                        this.IsChangesMade = false;

                        //if (MessageBox.Show("Do you wish to convert any more unit(s)?", "Convert Unit(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        // TODO: Resource strings
                        if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Convert Unit(s)", "Do you wish to convert any more unit(s)?", "Yes", "No")))
                        {
                            saveComplete = false;
                        }
                    }
                    else
                    {
                        if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                        {
                            //MessageBox.Show("Location must be set.", "Convert Unit(s)", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Convert Unit(s)", "Location must be set.", "OK"));
                        }
                        else if (this.ActItem.DataViewItems.Count == 0)
                        {
                            //MessageBox.Show("Convert from unit must be set.", "Convert Unit(s)", MessageBoxButtons.OK, MessageBoxIcon.Hand,MessageBoxDefaultButton.Button1);

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Convert Unit(s)", "Convert from unit must be set.", "OK"));
                        }
                        else if (this.ActDetail.DataViewDetails.Count == 0)
                        {
                            //MessageBox.Show("Convert to unit(s) must be set.", "Convert Unit(s)", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Convert Unit(s)", "Convert to unit(s) must be set.", "OK"));
                        }
                    }
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
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
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
                    // this.ProdUnitRefno = 0;

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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public XDocument GetXML()
        {
            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmlDoc;
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
        private void DisplayData()
        {
            if (this.ActItem.DataViewItems.Count > 0)
            {
                ProductInfo fromProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);

                // TODO: Resource strings
                this.lblFullUnitInfo.Text = string.Format("Qty {0} {1} @ {2} {3}",
                                            this.ActItem.DataViewItems[0]["ActivityDetailQuantity"],
                                            fromProdInfo.UnitCode,
                                            Math.Round(Convert.ToDecimal(this.ActItem.DataViewItems[0]["ActivityDetailWeightQuantity"]), 3),
                                            fromProdInfo.WeightUnitCode);
            }
            else
            {
                // TODO: Resource strings
                this.lblFullUnitInfo.Text = "Qty 0 UOM @ 0 KG/LB";
            }

            this.DisplayDataConvertedUnitInfo();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
            {
                this.Loading = true;
                BarCodeParse parse = null;
                SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();

                try
                {
                    await this.ScannerDisable();

                    parse = scanArgs.ParseInfo;

                    // GLOBAL LOCATION IDENTIFIER SCANNED
                    if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                    {
                        this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
                        }
                    }
                    else
                    {
                        // Activity item fk refno
                        if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a line item bar code is not supported here!");
                        }

                        // Activity detail fk refno
                        if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                        }

                        // Shipping container scanned, skid
                        if (!string.IsNullOrEmpty(parse.SSCC18))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                        }

                        // Product identifier
                        if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                        {
                            if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno) && !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Location Must Be Set First");
                            }

                            await this.ClearContents();

                            int tempFromProdUnitRefno = this.FromProdUnitRefno;

                            tempFromProdUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, tempFromProdUnitRefno);

                            this.FromProdUnitRefno = tempFromProdUnitRefno;
                            this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                            this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                            // Insert a new activity detail record
                            this.ActDetail.ActItem = this.ActItem;

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
                            this.ActDetail.ActExtra.DateTime5 = new DateTime(1980, 1, 1);

                            isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

                            XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                            await this.ActItem.Insert(xmlDoc, false);

                            this.DisplayData();

                            if (this.FromProdUnitRefno > 0)
                            {
                                using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                {
                                    cmndProdBCUnit.CommandType = CommandType.Text;
                                    cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode," +
                                        " tProductUnit.ProductUnitBC," +
                                        " tUnit.UnitCode" +
                                        " FROM tProductUnit" +
                                        " INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno" +
                                        " INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno" +
                                        " WHERE tProductUnit.ProductUnitRefno = " + this.FromProdUnitRefno.ToString();

                                    using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                                    {
                                        if (drProdBCUnit.Read())
                                        {
                                            this.cmdSelectFromUnit.Text = Convert.ToString(drProdBCUnit["ProductCode"])
                                                + " " + Convert.ToString(drProdBCUnit["ProductUnitBC"]) + " "
                                                + Convert.ToString(drProdBCUnit["UnitCode"]);
                                        }

                                        drProdBCUnit.Close();
                                    }
                                }
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
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplayDataConvertedUnitInfo()
        {
            if (this.ActDetail.DataViewDetails.Count > 0)
            {
                decimal qty = 0;
                decimal wtQty = 0;

                for (int item = 0; item <= this.ActDetail.DataViewDetails.Count - 1; item++)
                {
                    qty += Convert.ToDecimal(this.ActDetail.DataViewDetails[item]["ActivityDetailQuantity"]);
                    wtQty += Convert.ToDecimal(this.ActDetail.DataViewDetails[item]["ActivityDetailWeightQuantity"]);
                }

                ProductInfo toProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);
                this.lblConvertedUnitInfo.Text = string.Format("Qty {0} {1} @ {2} {3}", Math.Round(qty, 3), toProdInfo.UnitCode, Math.Round(wtQty, 3), toProdInfo.WeightUnitCode);
            }
            else
            {
                // TODO: Resource strings
                this.lblConvertedUnitInfo.Text = "Converted Qty 0 UOM @ 0 KG/LB";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                // buttons ignore cancel = true.
                if (sender == this.cmdLocation)
                {
                }

                if (sender == this.cmdSelectFromUnit)
                {
                }

                if (sender == this.cmdSelectToUnit)
                {
                }

                if (sender == this.updnBarCodeModifier1)
                {
                }

                if (sender == this.dgUnitWeight)
                {
                }

                //if (sender == this.txtGridEdit)
                //{
                //    DataGridCell cell = dgUnitWeight.CurrentCell;
                //    this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber] = this.txtGridEdit.Text;

                //    if (this.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActItem.ProdInfo.BaseUnitCode == "KG")
                //    {
                //        this.ActDetail.DataViewDetails[cell.RowNumber]["ActivityDetailBaseQuantity"] = (decimal)this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber];
                //    }

                //    if (this.ActItem.ProdInfo.UnitCode == "LB" || this.ActItem.ProdInfo.UnitCode == "KG")
                //    {
                //        this.ActDetail.DataViewDetails[cell.RowNumber]["ActivityDetailQuantity"] = (decimal)this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber];
                //    }

                //    this.DisplayDataConvertedUnitInfo();
                //    this.txtGridEdit.Visible = false;
                //    this.txtGridEdit.Enabled = false;
                //}
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task ClearContents()
        {
            this.FromProdUnitRefno = 0;
            this.ToProdUnitRefno = 0;
            this.ActItem.Delete();
            this.ActDetail.Delete();

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.cmdSelectFromUnit.Text = "Select From Unit";
                this.cmdSelectToUnit.Text = "Select To Unit";
                this.lblFullUnitInfo.Text = "Qty 0 UOM @ KG/LB";
                this.lblConvertedUnitInfo.Text = "Qty 0 UOM @ KG/LB";
            });

            this.DesignObservableCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private string MakeProductLabelsSubmitString(int idx)
        {
            PrinterSettings prnSettings = new PrinterSettings().LoadData();
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(detail[idx]["ProductUnitRefno"]));
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
            req.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
            req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
            req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNumber"]);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
            req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
            req.AppendFormat("<{0}>{1}</{0}>", "mobile_printer_name", (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty));
            req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string MakeConvertUnitSubmitString()
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;
            DataView items = this.ActDetail.ActItem.DataViewItems;

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(items[0]["ProductUnitRefno"]));
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(items[0]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(items[0]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT CONVERT UNIT</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.Append("<from>");
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", items[0]["ProductCode"]);
            req.AppendFormat("<bc>{0}</bc>", items[0]["ProductUnitBC"]);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", items[0]["UnitCode"]);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", items[0]["ActivityDetailSingleTradeItemSerialNumber"]);
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", items[0]["ActivityDetailLogisticUnitBatchLotNumber"]);
            req.AppendFormat("<variant_code>{0}</variant_code>", items[0]["ProductSchemeVariantCode"]);
            req.AppendFormat("<variant_no>{0}</variant_no>", items[0]["ProductSchemeVariantNumber"]);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", items[0]["ActivityDetailQuantity"]);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", items[0]["ActivityDetailWeightQuantity"]);
            req.AppendFormat("<base_qty>{0}</base_qty>", items[0]["ActivityDetailBaseQuantity"]);
            req.AppendFormat("<base_unit>{0}</base_unit>", prodInfo.BaseUnitCode);
            req.Append("</from>");

            for (int idx = 0; idx <= this.ActDetail.DataViewDetails.Count - 1; idx++)
            {
                ProductInfo toProdInfo = new ProductInfo();
                toProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(detail[idx]["ProductUnitRefno"]));

                req.Append("<to>");
                req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
                req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
                req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
                req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
                req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
                req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
                req.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
                req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
                req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
                req.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
                req.AppendFormat("<pdate>{0}</pdate>", production);
                req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
                req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
                req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNumber"]);
                req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
                req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
                req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
                req.AppendFormat("<base_unit>{0}</base_unit>", toProdInfo.BaseUnitCode);
                req.Append("</to>");
            }

            req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
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

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            this.cmdSelectFromUnit.IsEnabled = false;
            await this.ScannerDisable();

            if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
            {
                //MessageBox.Show("Your location must be set.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Your location must be set.", "OK"));
            }
            else
            {
                FormDialogContainer frmSelectProduct = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, SysCommon.ActSubTypes.ConvertUnit);

                if (await Device.InvokeOnMainThreadAsync(async () => await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    await this.ClearContents();
                    XDocument xmlDocProduct = frmSelectProduct.DisplayControlData;
                    this.FromProdUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                    if (this.FromProdUnitRefno != 0)
                    {
                        this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                        this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);
                        this.ActItem.LocMap = this.LocMap;

                        // Pass 0 as the current selected row on an add, true forces it as an add not an edit
                        this.ActDetail = this.ActDetail.GetInfo(0, true);

                        try
                        {
                            this.ActDetail.Quantity = 0;
                            this.ActDetail.BaseQuantity = 0;
                            this.ActDetail.WeightQuantity = 0;
                            this.ActDetail.SchemeID = string.Empty;
                            this.ActDetail.UniqueSchemeID = string.Empty;
                            this.ActDetail.ActExtra.Text1 = string.Empty;
                            this.ActDetail.ActExtra.Text2 = string.Empty;
                            this.ActDetail.ActExtra.Text3 = string.Empty;
                            this.ActDetail.ActExtra.Text4 = string.Empty;
                            this.ActDetail.ActExtra.DateTime5 = new DateTime(1980, 1, 1);

                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertUnit);

                            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                            {
                                XDocument xmlDoc = form.DisplayControlData;
                                await this.ActItem.Insert(xmlDoc, false);

                                using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                {
                                    cmndProdBCUnit.CommandType = CommandType.Text;
                                    cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode,"
                                        + "tProductUnit.ProductUnitBC,"
                                        + "tUnit.UnitCode "
                                        + "FROM tProductUnit "
                                        + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                        + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                        + "WHERE tProductUnit.ProductUnitRefno = " + this.FromProdUnitRefno;

                                    using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                                    {
                                        if (drProdBCUnit.Read())
                                        {
                                            this.cmdSelectFromUnit.Text =
                                                Convert.ToString(drProdBCUnit["ProductCode"]).TrimEnd()
                                                + (" " + Convert.ToString(drProdBCUnit["ProductUnitBC"])).TrimEnd()
                                                + " " + Convert.ToString(drProdBCUnit["UnitCode"]);

                                            this.DisplayData();
                                        }

                                        drProdBCUnit.Close();
                                    }
                                }
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                                await errorReport.ShowDialog();
                            });
                        }
                        catch (Exceptions.MessageOnlyException ex)
                        {
                            ex.Show(this.ActSubType);
                        }
                        catch (Exception ex)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                                await errorReport.ShowDialog();
                            });
                        }
                    }
                }

                //this.cmdSelectToUnit.Refresh();
            }

            await this.ScannerEnable();

            this.cmdSelectFromUnit.IsEnabled = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectToUnit_Click(object sender, EventArgs e)
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl causes a click event to fire if it is the next control
                await this.ScannerDisable();

                if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                {
                    //MessageBox.Show("Your location must be set.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Your location must be set.", "OK"));
                }
                else if (this.FromProdUnitRefno == 0 || this.ActItem.DataViewItems.Count <= 0)
                {
                    //MessageBox.Show("Please set the from product unit first.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Please set the from product unit first.", "OK"));
                }
                else
                {
                    this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                    this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                    decimal decFromBaseFactor = ProductInfoTasks.GetUnitRelationFactor((ProductInfo)this.ActItem.ProdInfo);

                    FormDialogContainer frmSelectProduct = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnitFilterToProduct, this.ActItem, SysCommon.ActSubTypes.ConvertUnit);

                    if (await Device.InvokeOnMainThreadAsync(async () => await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK))
                    {
                        XDocument xmlDocProduct = frmSelectProduct.DisplayControlData;
                        this.ToProdUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                        if (this.ToProdUnitRefno != 0)
                        {
                            this.ActItem.LocMap = this.LocMap;
                            this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);
                            this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                            SysCommon.SFillQuantity fillQty = this.RequestConvertUnitResult(
                                this.FromProdUnitRefno,
                                this.ToProdUnitRefno,
                                Convert.ToDecimal(this.ActItem.DataViewItems[0]["ActivityDetailQuantity"]),
                                Convert.ToDecimal(this.ActItem.DataViewItems[0]["ActivityDetailWeightQuantity"]));

                            this.ActDetail.Quantity = fillQty.Quantity;
                            this.ActDetail.BaseQuantity = fillQty.BaseQuantity;
                            this.ActDetail.WeightQuantity = fillQty.WeightQuantity;
                            this.ActDetail.SchemeID = Convert.ToString(this.ActItem.DataViewItems[0]["ActivityDetailSchemeID"]);
                            this.ActDetail.UniqueSchemeID = Convert.ToString(this.ActItem.DataViewItems[0]["ActivityDetailUniqueSchemeID"]);

                            SProductSchemeData prodSchemeData = new SProductSchemeData
                            {
                                LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitBestBeforeSellByDate"]),
                                LogisticUnitProductionDate = Convert.ToDateTime(this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitProductionDate"]),
                                VariantNumber = Convert.ToInt32(this.ActItem.DataViewItems[0]["ProductSchemeVariantNumber"]),
                                LogisticUnitBatchLotNumber = Convert.ToString(this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitBatchLotNumber"]),
                                LogisticUnitSkidNumber = string.Empty,
                                SingleTradeItemSerialNumber = Convert.ToString(this.ActItem.DataViewItems[0]["ActivityDetailSingleTradeItemSerialNumber"]),
                                VariantCode = Convert.ToString(this.ActItem.DataViewItems[0]["ProductSchemeVariantCode"])
                            };

                            this.ActDetail.SchemeData = prodSchemeData;

                            try
                            {
                                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityConvertUnitEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertUnit);

                                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                                {
                                    XDocument xmlDoc = form.DisplayControlData;
                                    this.cmdSelectToUnit.Text = string.Format("{0} {1}\'S ({2} per {3})",
                                        this.ActItem.ProdInfo.UnitBC,
                                        this.ActItem.ProdInfo.UnitCode,
                                        Math.Round(this.ActDetail.Quantity, 3),
                                        Convert.ToString(this.ActItem.DataViewItems[0]["UnitCode"]));

                                    this.ActDetail.Delete();
                                    this.ActDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
                                    this.ActDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
                                    this.ActDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);

                                    if (bool.Parse(xmlDoc.Root.Element("split").Value))
                                    {
                                        if (this.ActItem.ProdInfo.UnitCode == "LB" || this.ActItem.ProdInfo.UnitCode == "KG")
                                        {
                                            this.ActDetail.Insert(xmlDoc);
                                        }
                                        else
                                        {
                                            decimal altQty = this.ActDetail.Quantity;
                                            decimal baseQty = this.ActDetail.BaseQuantity;
                                            decimal wt = this.ActDetail.WeightQuantity;
                                            decimal wtQty = altQty == 0 ? wt : wt / altQty;

                                            for (int unitQty = 1; unitQty <= altQty; unitQty++)
                                            {
                                                this.ActDetail.Quantity = 1;
                                                this.ActDetail.WeightQuantity = wtQty;

                                                if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
                                                {
                                                    this.ActDetail.BaseQuantity = this.ActDetail.WeightQuantity;
                                                }
                                                else
                                                {
                                                    if (this.ActDetail.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                                                    {
                                                        this.ActDetail.BaseQuantity = this.ActDetail.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                                                    }
                                                    else
                                                    {
                                                        this.ActDetail.BaseQuantity = this.ActDetail.Quantity;
                                                    }
                                                }

                                                this.ActDetail.Insert(xmlDoc);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.ActDetail.Insert(xmlDoc);
                                    }
                                }

                                await Device.InvokeOnMainThreadAsync(() =>
                                {
                                    this.DisplayData();
                                    this.DesignObservableCollection();
                                });
                            }
                            catch (SQLiteException ex)
                            {
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                                    await errorReport.ShowDialog();
                                });
                            }
                            catch (Exceptions.MessageOnlyException ex)
                            {
                                ex.Show(this.ActSubType);
                            }
                            catch (Exception ex)
                            {
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                                    await errorReport.ShowDialog();
                                });
                            }
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromProdUnitRefno"></param>
        /// <param name="toProdUnitRefno"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private SysCommon.SFillQuantity RequestConvertUnitResult(int fromProdUnitRefno, int toProdUnitRefno, decimal quantity, decimal weightQuantity)
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity
            {
                Quantity = quantity
            };

            string request = this.CreateConvertUnitResultXML(fromProdUnitRefno, toProdUnitRefno, quantity, weightQuantity);
            string dataReq = SysCommon.SubmitComm(request);

            if (dataReq.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(dataReq);

                if (dataReq.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new Exceptions.MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }

                if (xmlDoc.SelectSingleNode("/VFPData/Quantity").HasChildNodes)
                {
                    fillQty.Quantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/Quantity").FirstChild.Value);
                }

                if (xmlDoc.SelectSingleNode("/VFPData/BaseQuantity").HasChildNodes)
                {
                    fillQty.BaseQuantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/BaseQuantity").FirstChild.Value);
                }

                if (xmlDoc.SelectSingleNode("/VFPData/WeightQuantity").HasChildNodes)
                {
                    fillQty.WeightQuantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/WeightQuantity").FirstChild.Value);
                }
            }

            return fillQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateConvertUnitResultXML(int fromProdUnitRefno, int toProdUnitRefno, decimal quantity, decimal weightQuantity)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            ProductInfo fromProd = ProductInfoTasks.GetInfoFromUnitRefno(fromProdUnitRefno);
            ProductInfo toProd = ProductInfoTasks.GetInfoFromUnitRefno(toProdUnitRefno);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CONVERT UNIT RESULT</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + fromProd.Code + "</prod_id>")
                + ("<from_bc>" + fromProd.UnitBC + "</from_bc>")
                + ("<from_alt_qty>" + quantity + "</from_alt_qty>")
                + ("<from_alt_unit>" + fromProd.UnitCode + "</from_alt_unit>")
                + ("<from_wt_qty>" + weightQuantity + "</from_wt_qty>")
                + ("<to_bc>" + toProd.UnitBC + "</to_bc>")
                + ("<to_alt_unit>" + toProd.UnitCode + "</to_alt_unit>")
                + ("<to_base_unit>" + toProd.BaseUnitCode + "</to_base_unit>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void NumericKeyPressHandler(object sender, KeyPressEventArgs e)
        //{
        //    if (this.IsEditTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsEditTrapped = false;
        //    }
        //    else if (this.IsNumericTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsNumericTrapped = false;
        //    }
        //    else if (this.IsEscapeTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsEscapeTrapped = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void updnBarCodeModifier1_GotFocus(object sender, EventArgs e)
        //{
        //    SysCommon.SelectValueInDomainUpDown((UpDownBase)sender);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityConvertUnit_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdLocation.Text = "Set Location";
                    }

                    this.IsControlLoaded = true;
                }

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void unitWeight_DoubleClick(object sender, EventArgs e)
        //{
        //    DataGridCell currCell = dgUnitWeight.CurrentCell;

        //    if (this.dgUnitWeight.TableStyles["tActivityDetail"].GridColumnStyles[currCell.ColumnNumber].MappingName == "ActivityDetailWeightQuantity")
        //    {
        //        this.txtGridEdit.Bounds = this.dgUnitWeight.GetCellBounds(currCell.RowNumber, currCell.ColumnNumber);
        //        this.txtGridEdit.Text = this.dgUnitWeight[currCell.RowNumber, currCell.ColumnNumber].ToString();
        //        this.txtGridEdit.SelectAll();
        //        this.txtGridEdit.Visible = true;
        //        this.txtGridEdit.Enabled = true;
        //        this.txtGridEdit.Focus();
        //    }
        //}

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

                if (sender == this.updnBarCodeModifier1)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
                }
            }
        }

        #endregion

        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            await this.CancelChanges();
            await this.Close();
        }

        private void dgUnitWeight_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}