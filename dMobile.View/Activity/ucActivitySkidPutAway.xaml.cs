using dMobile.View.CustomPages;
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using System.Threading.Tasks;
using Android.Views;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivitySkidPutAway : DMobileContentPage
    {
        #region Fields

        private ProductInfo origProdInfoOnSkid;
        private ActDetail actDetailOnSkid = null;
        private ActDetail actDetail = null;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private LocationMap locMap;
        private BarCodeParse barCodeParse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private SysCommon.ActSubTypes actSubType;
        private int totalTicks = 0;
        private bool isControlLoaded = false;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        //private UserControl callingControl = new ucPurchaseOrdersMenu();
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        //private List<string> optionsMenuItems = new List<string>();

        //// cmdLocation
        //private string cmdLocationText;
        //private bool cmdLocationIsEnabled;

        //// cmdSelectSkid
        //private string cmdSelectSkidText;
        //private bool cmdSelectSkidIsEnabled;


        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivitySkidPutAway()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdLocation.Text = "Set To Location";
            this.cmdAddProduct.Text = "Add";
            this.cmdSelectSkid.Text = "Select Skid";

            this.cmdLocation.IsEnabled = false;
            this.cmdAddProduct.IsEnabled = false;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.DataGridMainActivityItemsList = new ObservableCollection<object>();
                this.ItemsList = new ObservableCollection<string>();
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();
                this.OrigProdInfoOnSkid = new ProductInfo();

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

                this.ActSubType = SysCommon.ActSubTypes.PutAwaySkid;
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;

                this.Activity = ActivityFacade.GetActivity(this.ActSubType);
                this.ClearContents();

                this.Title = this.GetControlDescription();
                //this.PopulateOptionsMenu();
            }
        }

        #endregion

        //protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    base.OnPropertyChanged(propertyName);

        //    if (
        //        propertyName == nameof(cmdLocationText) ||
        //        propertyName == nameof(cmdLocationIsEnabled) ||
        //        propertyName == nameof(cmdSelectSkidText) ||
        //        propertyName == nameof(cmdSelectSkidIsEnabled)
        //       )
        //    {
        //        this.PopulateOptionsMenu();
        //    }
        //}

        ///// <summary>
        ///// Populates a string array to be used in DisplayActionSheet
        ///// </summary>
        //private void PopulateOptionsMenu()
        //{
        //    this.optionsMenuItems.Clear();

        //    if (this.cmdLocationIsEnabled)
        //    {
        //        this.optionsMenuItems.Add(this.cmdLocationText);
        //    }

        //    if (this.cmdSelectSkidIsEnabled)
        //    {
        //        this.optionsMenuItems.Add(this.cmdSelectSkidText);
        //    }
        //}

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.Del || keycode == Keycode.Clear || keycode == Keycode.Back)
                    {
                        if (this.DataGridMainActivityItemsList.Count > 0 && this.dataGridMainActivity.SelectedItem != null)
                        {
                            //DialogResult result = MessageBox.Show("Do you wish yo delete this record?", "Put Away", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (await DisplayAlert("Put Away", "Do you wish yo delete this record?", "Yes", "No"))
                            {
                                this.ActDetail.DataViewDetails[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)].Delete();

                                // Delete needs to rebind the the grid to the proper dataview depending on the record count
                                if (this.ActDetail.DataViewDetails.Count == 0)
                                {
                                    this.DataGridMainActivityItemsList = this.ActDetailOnSkid.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                                    //this.dataGridMainActivity.ForeColor = Color.Orange;
                                    //this.dataGridMainActivity.Refresh();
                                }
                            }
                        }
                    }
                }
            });

            try
            {
                this.ucActivitySkidPutAway_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucActivitySkidPutAway_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }


        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

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

        private ProductInfo OrigProdInfoOnSkid
        {
            get
            {
                return this.origProdInfoOnSkid;
            }

            set
            {
                this.origProdInfoOnSkid = value;
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

        private BarCodeParse BarCodeParse
        {
            get
            {
                return this.barCodeParse;
            }

            set
            {
                this.barCodeParse = value;
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

        /// <summary>
        /// Gets or Sets the singleton instance of location map.
        /// </summary>
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
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private ActDetail ActDetailOnSkid
        {
            get
            {
                return this.actDetailOnSkid;
            }

            set
            {
                this.actDetailOnSkid = value;
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

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSelectSkid_Click(object sender, EventArgs e)
        {
            try
            {
                this.ScannerDisable();
                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSkidInfo, this.Activity, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    XDocument xmlDoc = form.DisplayControlData;
                    this.ClearContents();

                    SysSkidInfo tmpSkidInfo = SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                    if (tmpSkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        this.Activity.SkidInfo = tmpSkidInfo;
                        this.cmdAddProduct.IsEnabled = true;

                        // TODO: Resource strings
                        this.cmdSelectSkid.Text = "Skid# " + this.Activity.SkidInfo.CurrSkid.SkidCode;

                        // TODO: Resource strings
                        this.ItemsList.Clear();
                        this.ItemsList.Add("Skid Product: " + ProductInfoTasks.GetCurrReadableProdBCUnit(this.Activity.SkidInfo.CurrSkid.ProdInfo));
                        this.ItemsList.Add("Skid Location: " + LocationMapTasks.GetCurrentReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap));
                        this.cmdLocation.IsEnabled = true;

                        // Set the global location to the skid location, 
                        // need to enforce that the skid and location warehouses match.
                        this.LocMap.CurrLocMap = this.Activity.SkidInfo.CurrSkid.LocMap;
                        this.LocMap.CurrLocMapRefno = this.Activity.SkidInfo.CurrSkid.LocMapRefno;
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap);

                        this.Activity.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                        this.Activity.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;

                        this.ActItem.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                        this.ActItem.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                        this.ActItem.LocMap = this.LocMap;
                        this.ActItem.ProdInfo = Activity.SkidInfo.CurrSkid.ProdInfo;
                        this.ActItem.ProdInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(this.ActItem.ProdInfo);

                        this.ActDetailOnSkid.LocMap = this.ActItem.LocMap;
                        SProductSchemeData tempSchemeData = this.ActDetailOnSkid.SchemeData;
                        tempSchemeData.LogisticUnitSkidNumber = this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        this.ActDetailOnSkid.SchemeData = tempSchemeData;
                        this.OrigProdInfoOnSkid = this.ActDetailOnSkid.ActItem.ProdInfo;

                        this.ActDetailOnSkid.Refresh();
                        //this.dataGridMainActivity.Refresh();
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
                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivitySkidPutAway_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                if (this.IsControlLoaded == false)
                {
                    this.IsControlLoaded = true;

                    //this.RefreshData();

                    if (this.Loading)
                    {
                        this.Loading = false;
                    }
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
        public async void cmdAddProduct_Click(object sender, EventArgs e)
        {
            if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.ScannerDisable();
                this.ActItem.ProdInfo = this.OrigProdInfoOnSkid;

                //DialogResult dlgResult = MessageBox.Show("Select a different unit of measure?", "Select UOM",MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                //if (dlgResult == DialogResult.Yes)
                // TODO: Resource strings
                if (await DisplayAlert("Select UOM", "Select a different unit of measure?", "Yes", "No"))
                {
                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnitFilterToProduct, this.ActItem, this.ActSubType);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        XDocument xmlDoc = form.DisplayControlData;
                        int prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);
                    
                        if (prodUnitRefno != 0)
                        {
                            this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        }
                    }
                }

                // Clear this.ActDetail. If false is passed for parameter isAdd, attempt to read data from
                // DataViewDetail is made which has not data. = error.
                this.ActDetail = this.ActDetail.GetInfo(0, true);

                SysCommon.SFillQuantity fillQty = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActItem.ProdInfo, this.ActDetailOnSkid.Quantity);

                this.ActDetail.Quantity = fillQty.Quantity;
                this.ActDetail.BaseQuantity = fillQty.BaseQuantity;
                this.ActDetail.WeightQuantity = fillQty.WeightQuantity;
                this.ActDetail.SchemeID = this.ActDetailOnSkid.SchemeID;
                this.ActDetail.UniqueSchemeID = this.ActDetailOnSkid.UniqueSchemeID;
                this.ActDetail.SchemeData = this.ActDetailOnSkid.SchemeData;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime5 = new DateTime(1900,1,1);

                await this.InsertActivityDetail(this.ActDetail);
                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdLocation_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap.WhsLocMapRefno, true);
            await form.DisplayControl.ShowDialog();

            if (form.DialogResult == DialogResult.OK)
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    
                    if (this.ActDetail.DataViewDetails.Count > 0)
                    {
                        for (int i = 0; i <= this.ActDetail.DataViewDetails.Count - 1; i++)
                        {
                            this.ActDetail.DataViewDetails[i].BeginEdit();
                            this.ActDetail.DataViewDetails[i]["LocationMapRefno"] = this.LocMap.CurrLocMapRefno;

                            this.ActDetail.DataViewDetails[i]["ActivityDetailSchemeID"] = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                                this.ActDetailOnSkid.ActItem.ProdInfo.Code,
                                this.ActDetailOnSkid.ActItem.ProdInfo.Scheme,
                                this.ActDetailOnSkid.SchemeData);

                            this.ActDetail.DataViewDetails[i]["ActivityDetailUniqueSchemeID"] = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                                this.ActDetailOnSkid.ActItem.ProdInfo.Code,
                                this.ActDetailOnSkid.ActItem.ProdInfo.Scheme,
                                this.ActDetailOnSkid.SchemeData);

                            this.ActDetail.DataViewDetails[i].EndEdit();
                        }
                    }
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdLocation.Text = "Set Location";
                }
            }

            this.ScannerEnable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XDocument GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
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
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (this.IsReadyToClose())
                {
                    // Expect the control to close.
                    // Clear any properties.
                    if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        //if (MessageBox.Show("Do you wish to put away another skid?", "Skid Put Away",
                        if (await DisplayAlert("Skid Put Away", "Do you wish to put away another skid?", "Yes", "No"))
                        {
                            this.ClearContents();
                            cancelChanges = false;
                            //this.cmdSelectSkid.Focus();
                        }
                    }
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
                cancelChanges = true;
                
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
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Skid Put Away";
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
                if (isReadyToSave)
                {
                    if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                if (isReadyToSave)
                {
                    if (this.ActDetailOnSkid.DataViewDetails.Count == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                if (isReadyToSave)
                {
                    if (this.ActDetail.DataViewDetails.Count == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                // Check number of unts match, ignoring weight in comparison
                // Raise a warning if there is a mismatch
                if (isReadyToSave)
                {
                    if (this.ActItem.ProdInfo.UnitCode != "LB" && this.ActItem.ProdInfo.UnitCode != "KG")
                    {
                        decimal skidTotQty = 0;
                        
                        for (int idx = 0; idx <= this.ActDetailOnSkid.DataViewDetails.Count - 1; idx++)
                        {
                            skidTotQty += Convert.ToDecimal(this.ActDetailOnSkid.DataViewDetails[idx]["ActivityDetailQuantity"]);
                        }

                        decimal putAwaySkidTotQty = 0;
                        
                        for (int idx = 0; idx <= ActDetail.DataViewDetails.Count - 1; idx++)
                        {
                            putAwaySkidTotQty += Convert.ToDecimal(this.ActDetail.DataViewDetails[idx]["ActivityDetailQuantity"]);
                        }

                        if (skidTotQty != putAwaySkidTotQty)
                        {
                            // TODO: Resource strings
                            string msg = string.Format("Total quantity of {0} {2} to be put away does not match the current skid " + "quantity of {1} {2}. Do you wish to continue?", putAwaySkidTotQty, skidTotQty, this.ActItem.ProdInfo.UnitCode);

                            //if (MessageBox.Show(msg, "Skid Put Away", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            // TODO: Resource strings
                            if (!await DisplayAlert("Skid Put Away", msg, "Yes", "No"))
                            {
                                isReadyToSave = false;
                            }
                        }
                    }
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
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    string reqData = SysCommon.SubmitComm(this.MakeSkidPutAwaySubmitString());
                    
                    if (reqData.Length > 0)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);

                        // check if result is an error message
                        if (reqData.Contains("<message>ERROR</message>"))
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
                            //MessageBox.Show("Skid put away complete!", "Skid Put Away", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                            await DisplayAlert("Skid Put Away", "Skid put away complete!", "Ok");
                        }

                        this.ClearContents();
                    }

                    saveComplete = true;

                    //if (MessageBox.Show("Do you wish to put away any more skids?", "Skid Put Away", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    // TODO: Resource strings
                    if (await DisplayAlert("Skid Put Away", "Do you wish to put away any more skids?", "Yes", "No"))
                    {
                        saveComplete = false;
                        this.ScannerEnable();
                    }
                }
                else
                {
                    //MessageBox.Show("Data Failed Validation. Check Your Data.");
                    // TODO: Resource strings
                    await DisplayAlert("Skid Put Away", "Data Failed Validation. Check Your Data.", "OK");
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
        /// <remarks></remarks>
        public void ClearContents()
        {
            this.ItemsList.Clear();

            this.cmdAddProduct.IsEnabled = false;
            this.cmdLocation.IsEnabled = false;
            // TODO: Resource strings
            this.cmdSelectSkid.Text = "Select Skid";
            this.cmdLocation.Text = "Set Location";

            this.Activity.SkidInfo = new SysSkidInfo();
            this.Activity.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
            this.Activity.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;

            this.ActItem = ActItemFacade.GetActItem(this.Activity, this.ActSubType);
            this.ActItem.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
            this.ActItem.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;

            this.ActDetailOnSkid = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.SkidInfo);
            this.ActDetailOnSkid.ActItem = this.ActItem;
            this.ActDetailOnSkid.DataViewDetails = this.ActDetailOnSkid.GetDataView(false);

            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
            this.ActDetail.DataViewDetails = this.ActDetail.GetDataView(false);
            this.ActDetail.ActItem = this.ActItem;

            this.DataGridMainActivityItemsList = this.ActDetailOnSkid.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
            //this.dataGridMainActivity.ForeColor = Color.Orange;
            //this.dataGridMainActivity.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string MakeProductLabelsSubmitString(int idx)
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)detail[idx]["ProductUnitRefno"]);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRINT PRODUCT LABEL</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodInfo.Code + "</prod_id>")
                + ("<is_bc>" + prodInfo.UnitIsBC + "</is_bc>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                + ("<pono>" + this.ActItem.ActHdr.PONO + "</pono>")
                + ("<alt_qty>" + detail[idx]["ActivityDetailQuantity"] + "</alt_qty>")
                + ("<alt_unit>" + detail[idx]["UnitCode"] + "</alt_unit>")
                + ("<base_qty>" + detail[idx]["ActivityDetailBaseQuantity"] + "</base_qty>")
                + ("<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>")
                + ("<wt_qty>" + detail[idx]["ActivityDetailWeightQuantity"] + "</wt_qty>")
                + ("<wt_unit>" + prodInfo.WeightUnitCode + "</wt_unit>")
                + ("<serial>" + detail[idx]["ActivityDetailSingleTradeItemSerialNumber"] + "</serial>")
                + ("<lot_no>" + detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"] + "</lot_no>")
                + ("<pdate>" + production + "</pdate>")
                + ("<bdate>" + bestBefore + "</bdate>")
                + ("<variant_no>" + detail[idx]["ProductSchemeVariantNo"] + "</variant_no>")
                + ("<variant_code>" + detail[idx]["ProductSchemeVariantCode"] + "</variant_code>")
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MakeSkidPutAwaySubmitString()
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            DataView skidDetail = this.ActDetailOnSkid.DataViewDetails;
            StringBuilder request = new StringBuilder();
            SSkidInfo currSkid = this.Activity.SkidInfo.CurrSkid;
            DataView detail = ActDetail.DataViewDetails;

            request.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            request.Append("<imclient>");
            request.Append("<command><action>SUBMIT PUT AWAY SKID</action></command>");
            request.Append(SysCommon.GetRequestDefaults(false));

            for (int idx = 0; idx <= skidDetail.Count - 1; idx++)
            {
                if (currSkid.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    bestBefore = Convert.ToDateTime(skidDetail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
                }

                if (currSkid.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    production = Convert.ToDateTime(skidDetail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
                }

                request.Append("<from>");
                request.AppendFormat("<whs_id>{0}</whs_id>", currSkid.LocMap.WhsFKRefno);
                request.AppendFormat("<sect_refno>{0}</sect_refno>", currSkid.LocMap.SectFKRefno);
                request.AppendFormat("<slot_refno>{0}</slot_refno>", currSkid.LocMap.SlotFKRefno);
                request.AppendFormat("<refno></refno>", this.ActItem.ActHdr.FKRefno);
                request.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
                request.AppendFormat("<prod_id>{0}</prod_id>", skidDetail[idx]["ProductCode"]);
                request.AppendFormat("<bc>{0}</bc>", skidDetail[idx]["ProductUnitBC"]);
                request.AppendFormat("<alt_unit>{0}</alt_unit>", skidDetail[idx]["UnitCode"]);
                request.AppendFormat("<wt_unit>{0}</wt_unit>", currSkid.ProdInfo.WeightUnitCode);
                request.AppendFormat("<serial>{0}</serial>", skidDetail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
                request.AppendFormat("<pdate>{0}</pdate>", production);
                request.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                request.AppendFormat("<lot_no>{0}</lot_no>", skidDetail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
                request.AppendFormat("<variant_code>{0}</variant_code>", skidDetail[idx]["ProductSchemeVariantCode"]);
                request.AppendFormat("<variant_no>{0}</variant_no>", skidDetail[idx]["ProductSchemeVariantNo"]);
                request.AppendFormat("<alt_qty>{0}</alt_qty>", Convert.ToDecimal(skidDetail[idx]["ActivityDetailQuantity"]) * -1);
                request.AppendFormat("<wt_qty>{0}</wt_qty>", Convert.ToDecimal(skidDetail[idx]["ActivityDetailWeightQuantity"]) * -1);
                request.AppendFormat("<base_qty>{0}</base_qty>", Convert.ToDecimal(skidDetail[idx]["ActivityDetailBaseQuantity"]) * -1);
                request.AppendFormat("<skid_fk_refno>{0}</skid_fk_refno>", currSkid.SkidFKRefno);
                request.AppendFormat("<extra1>{0}</extra1>", this.ActDetailOnSkid.ActExtra.Text1);
                request.AppendFormat("<extra2>{0}</extra2>", this.ActDetailOnSkid.ActExtra.Text2);
                request.AppendFormat("<extra3>{0}</extra3>", this.ActDetailOnSkid.ActExtra.Text3);
                request.AppendFormat("<extra4>{0}</extra4>", this.ActDetailOnSkid.ActExtra.Text4);
                request.AppendFormat("<extra5>{0}</extra5>", this.ActDetailOnSkid.ActExtra.DateTime5);
                request.Append("</from>");
            }

            for (int idx = 0; idx <= detail.Count - 1; idx++)
            {
                if (currSkid.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    bestBefore = Convert.ToDateTime(detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
                }

                if (currSkid.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    production = Convert.ToDateTime(detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
                }

                request.Append("<to>");
                request.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
                request.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
                request.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
                request.AppendFormat("<refno></refno>", this.ActItem.ActHdr.FKRefno);
                request.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
                request.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
                request.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
                request.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
                request.AppendFormat("<wt_unit>{0}</wt_unit>", currSkid.ProdInfo.WeightUnitCode);
                request.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
                request.AppendFormat("<pdate>{0}</pdate>", production);
                request.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                request.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
                request.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
                request.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNo"]);
                request.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
                request.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
                request.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
                request.AppendFormat("<skid_fk_refno>{0}</skid_fk_refno>", currSkid.SkidFKRefno);
                request.AppendFormat("<extra1>{0}</extra1>", this.ActDetail.ActExtra.Text1);
                request.AppendFormat("<extra2>{0}</extra2>", this.ActDetail.ActExtra.Text2);
                request.AppendFormat("<extra3>{0}</extra3>", this.ActDetail.ActExtra.Text3);
                request.AppendFormat("<extra4>{0}</extra4>", this.ActDetail.ActExtra.Text4);
                request.AppendFormat("<extra5>{0}</extra5>", this.ActDetail.ActExtra.DateTime5);
                request.Append("</to>");
            }

            request.Append("</imclient>");

            return request.ToString();
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

        #region Private Members

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

        ///// <summary>
        /////
        ///// </summary>
        ///// <remarks></remarks>
        //private void RefreshData()
        //{
            ////this.ScannerDisable()

            ////this.pbarStatus.Visible = True

            ////this.lstDisplay.Items.Clear()
            ////this.lstDisplay.Items.Add("Loading item detail(s). Please wait...")
            ////this.lstDisplay.Invalidate()
            ////this.lstDisplay.Update()

            ////this.lstDisplay.Items.Clear()

            ////this.pbarStatus.Visible = False

            ////this.ScannerEnable()
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="actDetail"></param>
        /// <remarks></remarks>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            XDocument xmlDoc = null;
            SProductSchemeData schemeData = actDetail.SchemeData;
            //int idx = dataGridMainActivity.CurrentRowIndex;
            int idx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

            xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<serial>" + actDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + actDetail.SchemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + actDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + actDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + actDetail.SchemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + actDetail.SchemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + actDetail.Quantity + "</alt_qty>")
                + ("<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + actDetail.BaseQuantity + "</base_qty>")
                + "</params>");

            bool isInsertDetailReady = false;
            bool isDuplicateSchemeId = false;

            // Check for missing tracking components, if any missing invoke the edit detail form
            schemeData = actDetail.SchemeData;
            schemeData.LogisticUnitSkidNumber = Activity.SkidInfo.CurrSkid.SkidCode;

            // Check for duplicate occurs in form
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailEdit, actDetail, this.ActSubType);

            if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
            {
                xmlDoc = form.DisplayControlData;
                actDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
                actDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
                actDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);
                schemeData.SingleTradeItemSerialNumber = xmlDoc.Root.Element("serial").Value;
                schemeData.LogisticUnitProductionDate = DateTime.Parse(xmlDoc.Root.Element("pdate").Value);
                schemeData.LogisticUnitBatchLotNumber = xmlDoc.Root.Element("lot_no").Value;
                schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(xmlDoc.Root.Element("bdate").Value);
                schemeData.VariantCode = xmlDoc.Root.Element("variant_code").Value;
                schemeData.VariantNumber = int.Parse(xmlDoc.Root.Element("variant_no").Value);

                actDetail.ActExtra.Text1 = xmlDoc.Root.Element("extra1").Value;
                actDetail.ActExtra.Text2 = xmlDoc.Root.Element("extra2").Value;
                actDetail.ActExtra.Text3 = xmlDoc.Root.Element("extra3").Value;
                actDetail.ActExtra.Text4 = xmlDoc.Root.Element("extra4").Value;
                actDetail.ActExtra.DateTime5 = DateTime.Parse(xmlDoc.Root.Element("extra5").Value);

                isInsertDetailReady = true;
            }

            actDetail.SchemeData = schemeData;
            actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);
            actDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);

            if (isInsertDetailReady)
            {
                if (!isDuplicateSchemeId)
                {
                    actDetail.Insert(xmlDoc);

                    if (actDetail.DataViewDetails.Count > 0)
                    {
                        this.DataGridMainActivityItemsList = actDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                        //this.dataGridMainActivity.Focus();
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[actDetail.DataViewDetails.Count - 1];
                    }
                    else
                    {
                        this.DataGridMainActivityItemsList = this.ActDetailOnSkid.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                        //this.dataGridMainActivity.ForeColor = Color.Orange;
                        //this.dataGridMainActivity.Refresh();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            int prodUnitRefno = 0;
            int svdActDetailFKRefno = this.ActDetailOnSkid.FKRefno;

            try
            {
                this.BarCodeParse = scanArgs.ParseInfo;

                this.ScannerDisable();

                if (!string.IsNullOrEmpty(this.BarCodeParse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarCodeParse.LocationIdentifier));
                    
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdLocation.Text = "Set To Location";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                    {
                        if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Location must be set first.");
                        }
                        else
                        {
                            if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno == 0)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("A skid must be selected first before scanning any product detail.");
                            }

                            prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarCodeParse, prodUnitRefno);
                            
                            if (this.Activity.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Identified scanned product does not match the selected skid product.");
                            }

                            await this.GatherProductData(scanArgs.ParseInfo);
                        }
                    }
                    else if (!string.IsNullOrEmpty(BarCodeParse.ActFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an activity header reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(BarCodeParse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an activity item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarCodeParse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ClearContents();

                        this.Activity.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(scanArgs.ParseInfo.SSCC18);

                        if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Skid does not exist.");
                        }

                        if (this.Activity.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.cmdAddProduct.IsEnabled = true;

                            // TODO: Resource strings
                            this.cmdSelectSkid.Text = "Skid# " + Activity.SkidInfo.CurrSkid.SkidCode;

                            this.ItemsList.Clear();
                            
                            // TODO: Resource strings
                            this.ItemsList.Add("Skid Product: " + ProductInfoTasks.GetCurrReadableProdBCUnit(this.Activity.SkidInfo.CurrSkid.ProdInfo));
                            this.ItemsList.Add("Skid Location: " + LocationMapTasks.GetCurrentReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap));

                            this.cmdLocation.IsEnabled = true;
                            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap);

                            this.Activity.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                            this.Activity.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;

                            this.ActItem.FKRefno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                            this.ActItem.Refno = this.Activity.SkidInfo.CurrSkid.SkidFKRefno;
                            this.ActItem.LocMap = this.LocMap;
                            this.ActItem.ProdInfo = this.Activity.SkidInfo.CurrSkid.ProdInfo;
                            this.ActItem.ProdInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(this.ActItem.ProdInfo);

                            this.OrigProdInfoOnSkid = this.ActItem.ProdInfo;

                            this.ActDetailOnSkid.LocMap = this.ActItem.LocMap;
                            SProductSchemeData tempSchemeData = this.ActDetailOnSkid.SchemeData;
                            tempSchemeData.LogisticUnitSkidNumber = this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            this.ActDetailOnSkid.SchemeData = tempSchemeData;

                            this.ActDetailOnSkid.Refresh();
                            //this.dataGridMainActivity.Refresh();
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.cmdSelectSkid.Text = "Select Skid";
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanned bar code type is not supported here.");
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);

                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Select Skid";
                }

                this.ActDetailOnSkid.FKRefno = svdActDetailFKRefno;
            }
            catch (IsReadyToApplyException ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Select Skid";
                }

                this.ActDetailOnSkid.FKRefno = svdActDetailFKRefno;

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        await errorReport.ShowDialog();
                    });
                }
            }
            catch (GatherProductDataException ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Select Skid";
                }

                this.ActDetailOnSkid.FKRefno = svdActDetailFKRefno;

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        await errorReport.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdSelectSkid.Text = "Select Skid";
                }

                this.ActDetailOnSkid.FKRefno = svdActDetailFKRefno;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();
            }
        }

        private async Task GatherProductData(BarCodeParse barParse)
        {
            SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
            bool isInsertReady = false;

            isInsertReady = await SysShared.GatherProductData(barParse, this.ActSubType, this.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

            if (isInsertReady)
            {
                XDocument xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + ("<itm_refno>" + this.ActDetail.ActItem.FKRefno + "</itm_refno>")
                    + ("<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>")
                    + ("<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                    + "</imclient>");

                this.ActDetail.IsReadyToApply(xmlParam);

                //this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);

                if (this.ActDetail.DataViewDetails.Count == 0)
                {
                    this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                    //this.dataGridMainActivity.ForeColor = Color.Black;
                    //this.dataGridMainActivity.Refresh();
                }

                //if (this.ActDetail.DataViewDetails.Count > 0)
                //{
                //    this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);
                //}

                XDocument xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + ("<serial>" + this.ActDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                    + ("<pdate>" + this.ActDetail.SchemeData.LogisticUnitProductionDate + "</pdate>")
                    + ("<lot_no>" + this.ActDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                    + ("<bdate>" + this.ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                    + ("<variant_code>" + this.ActDetail.SchemeData.VariantCode + "</variant_code>")
                    + ("<variant_no>" + this.ActDetail.SchemeData.VariantNumber + "</variant_no>")
                    + ("<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>")
                    + ("<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>")
                    + ("<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>")
                    + ("<is_batch_print_product_label>" + false + "</is_batch_print_product_label>")
                    + "</params>");

                this.ActDetail.Insert(xmlDoc);

                if (this.ActDetail.DataViewDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                {
                    //this.dataGridMainActivity.Focus();
                    //this.dataGridMainActivity.CurrentRowIndex = this.ActDetail.DataViewDetails.Count - 1;
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.ActDetail.DataViewDetails.Count - 1];
                }
                else
                {
                    this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                    //this.dataGridMainActivity.ForeColor = Color.Orange;
                    //this.dataGridMainActivity.Refresh();
                }
            }
        }

        #endregion

        //private async void OptionsMenu_Clicked(object sender, EventArgs e)
        //{
        //    if (this.Loading)
        //    {
        //        return;
        //    }

        //    // TODO: Resource strings
        //    string result = await DisplayActionSheet("Skid Put Away", "Close", null, this.optionsMenuItems.ToArray());

        //    if (result == this.cmdLocationText && this.cmdLocationIsEnabled)
        //    {
        //        await this.cmdLocation_Click(sender, e);
        //    }
        //    else if (result == this.cmdSelectSkidText && this.cmdSelectSkidIsEnabled)
        //    {
        //        await this.cmdSelectSkid_Click(sender, e);
        //    }
        //}
    }
}