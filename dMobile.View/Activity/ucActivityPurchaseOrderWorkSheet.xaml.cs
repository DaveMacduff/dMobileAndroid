namespace dMobile.View.Activity
{
    using dMobile.View.CustomPages;
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.Resources.Enums;
    using dMobile.View.Forms;
    using System.Data.SQLite;
    using Android.Views;
    using System.Collections.ObjectModel;
    using dMobile.Resources.Types;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityPurchaseOrderWorkSheet : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        private Activity activity;
        private ActItem actItem;
        private ActDetail actDetail;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.PurchaseOrderWorkSheet;
        private BarCodeParse barParse;
        private BarCodeScanner scanner;
        private int totalTicks = 0;
        private bool isBarCodeRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        //private UserControl callingControl = null;
        private string itemSortByIdx = "ActivityItemPurchaseOrderWorkSheetFKRefno";
        private int prodUnitRefno = 0;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        #endregion

        #region Constructors

        public ucActivityPurchaseOrderWorkSheet()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdAdd.Text = "Add";

            this.ItemsList = new ObservableCollection<string>();
            this.DataGridMainActivityItemsList = new ObservableCollection<object>();
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

            this.Title = this.GetControlDescription();

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

            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
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

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape)
                    {
                        if (await SaveChanges())
                        {
                            ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                        else
                        {
                            await CancelChanges();
                        }
                    }
                    else if (keycode == Keycode.DpadDown)
                    {
                        if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) < this.DataGridMainActivityItemsList.Count - 1)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) + 1];
                            this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) > 0)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) - 1];
                            this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
            });

            try
            {
                this.Loading = true;
                await Task.Run(async () => await this.ucActivityPurchaseOrderWorkSheet_GotFocus(this, new EventArgs()));
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK");
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ucActivityHdr_LostFocus(this, new EventArgs());

            base.OnDisappearing();
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

        private BarCodeParse BarParse
        {
            get
            {
                return this.barParse;
            }

            set
            {
                this.barParse = value;
            }
        }

        private BarCodeScanner Scanner
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

        private string ItemSortByIdx
        {
            get
            {
                return this.itemSortByIdx;
            }

            set
            {
                this.itemSortByIdx = value;
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

        private bool IsBarCodeRegistered
        {
            get
            {
                return this.isBarCodeRegistered;
            }

            set
            {
                this.isBarCodeRegistered = value;
            }
        }

        private Activity Activity
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

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            this.Loading = true;

            await Task.Run(async () =>
            {
                string message = string.Empty;
                //int prodUnitRefno = 0;
                int actItemFoundRefno = 0;
                var currentRowIdx = Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                int foundRowIdx = 0;

                this.BarParse = scanArgs.ParseInfo;

                this.ScannerDisable();

                try
                {
                    if (!string.IsNullOrEmpty(BarParse.ActFKRefno))
                    {
                        //MessageBox.Show("Scanning an Activity Item Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(BarParse.ActItemFKRefno))
                    {
                        //MessageBox.Show("Scanning an Activity Item Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(BarParse.ActDetailFKRefno))
                    {
                        //MessageBox.Show("Scanning an Activity Detail Reference # Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an Activity Detail Reference # Is Not Supported Here", "OK"));

                    }
                    else if (!string.IsNullOrEmpty(BarParse.SSCC18))
                    {
                        //MessageBox.Show("Scanning a shipping container bar code is not supported here!", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(BarParse.LocationIdentifier))
                    {
                        //MessageBox.Show("Scanning a Location Identifier Bar Code Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a Location Identifier Bar Code Is Not Supported Here", "OK"));
                    }
                    else if (!string.IsNullOrEmpty(BarParse.ProductIdentifier))
                    {
                        //if (ActItem.DataViewItems.Count > 0 && currentRowIdx < ActItem.DataViewItems.Count)
                        //{
                        //    this.dataGridMainActivity.UnSelect(currentRowIdx);
                        //}

                        XDocument xmlDocProduct = null;

                        if (await IdentifyProductScanned(BarParse, xmlDocProduct))
                        {
                            ProductInfo oProductInfo = new ProductInfo();
                            oProductInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            actItemFoundRefno = ActItem.GetRefnoFromProdUnitRefno(this.prodUnitRefno);
                            //<issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(oProductInfo, this.BarParse);
                        }

                        if (actItemFoundRefno == 0 && this.prodUnitRefno != 0)
                        {
                            ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(ActItem.ProdInfo.Refno);

                            //<issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(this.ActItem.ProdInfo, this.BarParse);

                            if (ActItem.IsAllowAddItem)
                            {
                                Activity.FKRefno = 0;
                                ActItem.FKRefno = 0;

                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + 0 + "</alt_qty>") + ("<base_qty>" + 0 + "</base_qty>") + ("<wt_qty>" + 0 + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + Activity.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                await ActItem.Insert(request, false);
                                actItemFoundRefno = ActItem.Refno;
                            }
                            else
                            {
                                // TODO: Resource Strings
                                message = string.Format("Item Not Found For " + Environment.NewLine + "Location {0} " + Environment.NewLine + "Product {1} BC {2} Unit {3}", LocationMapTasks.GetCurrentReadableLocation(), ActItem.ProdInfo.Code, ActItem.ProdInfo.UnitBC, ActItem.ProdInfo.UnitCode);
                                throw (new Exception(message));
                            }
                        }

                        if (actItemFoundRefno == 0)
                        {
                            // TODO: Resource strings
                            throw (new Exception("Item Not Found."));
                        }
                        else
                        {
                            string strOldSort = ActItem.DataViewItems.Sort;
                            ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            foundRowIdx = ActItem.DataViewItems.Find(actItemFoundRefno);

                            if (foundRowIdx < 0)
                            {
                                // TODO: Resource strings
                                throw (new Exception("Unable To Indentify Item"));
                            }

                            ActItem.SelectItem(foundRowIdx);
                            ActItem.DataViewItems.Sort = strOldSort;
                        }

                        DialogResult dialogResult = DialogResult.None;
                        XDocument xmlDocEdit = null;

                        if (actItemFoundRefno > 0)
                        {
                            //Ask for new quantity
                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit, ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = form.DisplayControlData;
                                string strOldSort = ActItem.DataViewItems.Sort;
                                ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                foundRowIdx = ActItem.DataViewItems.Find(actItemFoundRefno);

                                if (foundRowIdx < 0)
                                {
                                    // TODO: Resource strings
                                    throw (new Exception("Unable To Indentify Item"));
                                }

                                decimal decQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("alt_qty").Value)) + Convert.ToDecimal(ActItem.DataViewItems[foundRowIdx]["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"]), 3);
                                decimal decBaseQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("base_qty").Value)) + Convert.ToDecimal(ActItem.DataViewItems[foundRowIdx]["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"]), 3);
                                decimal decWeightQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("wt_qty").Value)) + Convert.ToDecimal(ActItem.DataViewItems[foundRowIdx]["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"]), 3);
                                ActItem.DataViewItems.Sort = strOldSort;

                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + decQty + "</alt_qty>") + ("<base_qty>" + decBaseQty + "</base_qty>") + ("<wt_qty>" + decWeightQty + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                ActItem.Update(request);
                            }
                        }
                        else
                        {
                            ActItem.FKRefno = 0;

                            // Reset values
                            ActDetail.Quantity = 0;
                            ActDetail.BaseQuantity = 0;
                            ActDetail.WeightQuantity = 0;
                            ActDetail.SchemeID = string.Empty;
                            ActDetail.UniqueSchemeID = string.Empty;

                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit, ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = form.DisplayControlData;
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                await ActItem.Insert(request, false);
                            }
                        }

                        ActItem.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                        foundRowIdx = ActItem.DataViewItems.Find(ActItem.FKRefno);

                        if (foundRowIdx < 0)
                        {
                            // TODO: Resource strings
                            throw (new Exception("Unable To Indentify Item"));
                        }

                        //if (ActItem.DataViewItems.Count > 0 && currentRowIdx < ActItem.DataViewItems.Count)
                        //{
                        //    dataGridMainActivity.CurrentRowIndex = foundRowIdx;
                        //}

                        //dataGridMainActivity.Select(foundRowIdx);
                    }
                    else
                    {
                        //MessageBox.Show("Scanned Bar Code Type Is Not Supported Here", GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanned Bar Code Type Is Not Supported Here", "OK"));
                    }
                }
                catch (Exception ex)
                {
                    //if (ActItem.DataViewItems.Count > 0 && currentRowIdx < ActItem.DataViewItems.Count)
                    //{
                    //    dataGridMainActivity.CurrentRowIndex = currentRowIdx;
                    //}

                    //MessageBox.Show(ex.Message, "Activity Item", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Activity Item", ex.Message, "OK"));
                }
                finally
                {
                    ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oBarCodeParse"></param>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<bool> IdentifyProductScanned(BarCodeParse oBarCodeParse, XDocument xmlDocProduct)
        {
            bool IsProductIdentified = false;
            string strMessage = string.Empty;
            int iProductUnitBarCodeRefno = 0;
            Activity oActivityBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            ActItem actItemBarCode = ActItemFacade.GetActItem(oActivityBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
            ActDetail actDetailBarCode = ActDetailFacade.GetActDetail(actItemBarCode, SysCommon.ActSubTypes.BarCodeAssociation);

            //by reference params initialize
            this.prodUnitRefno = 0;

            using (SQLiteCommand cmndProductUnitBarCode = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProductUnitBarCode.CommandType = CommandType.Text;
                cmndProductUnitBarCode.CommandText = "SELECT * FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = @productUnitBarCodeCode AND ProductUnitBarCodeModifier1 = @productUnitBarCodeModifier1";
                //cmndProductUnitBarCode.IndexName = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1";
                cmndProductUnitBarCode.Parameters.Add(new SQLiteParameter("@productUnitBarCodeCode", oBarCodeParse.ProductIdentifier));
                cmndProductUnitBarCode.Parameters.Add(new SQLiteParameter("@productUnitBarCodeModifier1", this.SysEnv.CurrentBarCodeModifier1));

                using (SQLiteDataReader drProductUnitBarCode = cmndProductUnitBarCode.ExecuteReader())
                {
                    //if (drProductUnitBarCode.Seek(DbSeekOptions.FirstEqual, new object[] { oBarCodeParse.ProductIdentifier.PadRight(20), SysEnv.CurrentBarCodeModifier1 }))
                    //{
                    if (drProductUnitBarCode.Read())
                    {
                        iProductUnitBarCodeRefno = Convert.ToInt32(drProductUnitBarCode["ProductUnitBarCodeRefno"]);
                        this.prodUnitRefno = Convert.ToInt32(drProductUnitBarCode["ProductUnitRefno"]);
                    }
                    //}
                }

                //IF THE SCANNED BARCODE WAS NOT FOUND IN THE DATABASE
                if (iProductUnitBarCodeRefno == 0)
                {
                    //IF YES TO JOIN BARCODE TO A PRODUCT
                    // TODO: Resource strings
                    strMessage = "Scanned Product Bar Code Is Not Associated To a Product." + Environment.NewLine + "Do You Wish To Associate It Now? ";

                    //if (MessageBox.Show(strMessage, "Activity Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Activity Item", strMessage, "Yes", "No")))
                    {
                        // CALL AND SELECTED A PRODUCT TO JOIN THE BAR CODE TOO
                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

                        if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            xmlDocProduct = form.DisplayControlData;
                            this.prodUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                            //IF A PRODUCT WAS SELECTED, CREATE THE XML FOR THE BAR CODE ACTIVITY INSERT
                            if (this.prodUnitRefno != 0)
                            {
                                XDocument xmlDoc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + prodUnitRefno.ToString() + "</ProductUnitRefno>") + ("<ProductUnitBarCodeRefno>" + iProductUnitBarCodeRefno.ToString() + "</ProductUnitBarCodeRefno>") + ("<ProductIdentifier>" + oBarCodeParse.ProductIdentifier + "</ProductIdentifier>") + ("<RawData>" + oBarCodeParse.Raw + "</RawData>") + ("<enumBarCodeSymbology>" + (Convert.ToInt32(oBarCodeParse.Symbology)) + "</enumBarCodeSymbology>") + ("<BarCodeSymbologyPrefix>" + oBarCodeParse.SymbologyPrefix + "</BarCodeSymbologyPrefix>") + ("<ProductUnitBarCodeModifier1>" + SysEnv.CurrentBarCodeModifier1 + "</ProductUnitBarCodeModifier1>") + "</params>");

                                //INSERT A NEW 'BAR CODE ASSOCIATION' ACTIVITY
                                await actItemBarCode.Insert(xmlDoc, false);

                                IsProductIdentified = true;
                            }
                        }
                    }
                }
                else
                {
                    IsProductIdentified = true;
                }
            }


            return IsProductIdentified;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            //pbarStatus.Visible = true;
            //lstDisplay.Items.Clear();
            //lstDisplay.Items.Add("Loading Work Sheet. Please Wait...");
            //lstDisplay.Invalidate();
            //lstDisplay.Update();

            this.Loading = true;

            try
            {
                Activity.Refresh(null);
                ActItem.Refresh(null);
            }
            catch (SQLiteException ex)
            {
                for (int curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                {
                    //MessageBox.Show((string)("Error " + ex.Data[curExNdx].ToString() + '\r'));
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error ", ex.Data[curExNdx].ToString() + '\r', "OK"));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Refresh", ex.Message, "OK"));
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                //lstDisplay.Items.Clear();
                this.ItemsList.Clear();
                //ActItem.DesignDataGrid(dataGridMainActivity);

                ActItem.DataViewItems = ActItem.GetDataView(false);
                this.DataGridMainActivityItemsList = this.ActItem.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                //dataGridMainActivity.ItemsSource = ActItem.DataViewItems;
                dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                if (ActItem.DataViewItems.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                {
                    //dataGridMainActivity.Select(0);
                    //ActItem.SelectItem(dataGridMainActivity.CurrentRowIndex);
                    await Task.Run(() => ActItem.SelectItem(0));
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                }
            });

            this.Loading = false;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (isBarCodeRegistered)
            {
                Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(BarCodeHandler);
                Scanner.Stop();
                isBarCodeRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!isBarCodeRegistered)
            {
                isBarCodeRegistered = true;
                Scanner.Start();
                Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(BarCodeHandler);
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

            return IsReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (Scanner != null)
            {
                ScannerDisable();
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
        //    UserControl ucControl = new ucPurchaseOrdersMenu();
        //    return ucControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return Activity.GetControlDescription();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            return true;
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
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.PurchaseOrderWorkSheet, ex);
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
            XDocument xmldocParams = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");
            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async Task ucActivityPurchaseOrderWorkSheet_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                ScannerEnable();

                await Device.InvokeOnMainThreadAsync(() => updnBarCodeModifier1.SelectedItem = SysEnv.CurrentBarCodeModifier1);

                ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocMap.LoginWhsLocMapRefno);
                await RefreshData();
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void dataGridMainActivity_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    dataGridMainActivity.Select(dataGridMainActivity.CurrentRowIndex);
        //    ActItem.SelectItem(dataGridMainActivity.CurrentRowIndex);
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void tmrActivityLoaded_Tick(object sender, EventArgs e)
        //{
        //    TotalTicks = TotalTicks + tmrActivityLoaded.Interval;

        //    if (this.Enabled && TotalTicks >= 100)
        //    {
        //        tmrActivityLoaded.Enabled = false;
        //        //LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocMap.LoginWhsLocMapRefno);
        //        //RefreshData();
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityHdr_LostFocus(object sender, EventArgs e)
        {
            //if (tmrActivityLoaded.Enabled == false)
            //{
            if (Scanner != null)
            {
                ScannerDisable();
            }
            //}
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdd_Click(object sender, EventArgs e)
        {
            XDocument xmlDocProduct = null;
            XDocument xmlDocEdit = null;
            int iActivityItemFoundRefno = 0;
            DialogResult dialogResult = DialogResult.None;
            string strProductUnitBarCodeCode = string.Empty;
            string strBarCodeSymbologyPrefix = string.Empty;

            ScannerDisable();

            try
            {
                // activity item object needs to check if a new location and product can be added, if not add a detail
                if (LocMap.CurrLocMap.WhsFKRefno == 0 && !SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    //MessageBox.Show("You Have To Set Location!");
                    await DisplayAlert("Set Location", "Location Must Be Set!", "OK");
                }
                else
                {
                    FormDialogContainer formSelectProd = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

                    dialogResult = await formSelectProd.DisplayControl.ShowDialog();
                    await formSelectProd.SaveChanges(this, new EventArgs());

                    if (dialogResult == DialogResult.OK)
                    {
                        xmlDocProduct = formSelectProd.DisplayControlData;
                        ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));
                        ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(ActItem.ProdInfo.Refno);
                        iActivityItemFoundRefno = ActItem.GetRefnoFromProdUnitRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));

                        if (iActivityItemFoundRefno > 0)
                        {
                            //Ask for new quantity
                            FormDialogContainer formPurchaseOrderWorkSheetEdit = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit, ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await formPurchaseOrderWorkSheetEdit.DisplayControl.ShowDialog();
                            await formPurchaseOrderWorkSheetEdit.SaveChanges(this, new EventArgs());

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = formPurchaseOrderWorkSheetEdit.DisplayControlData;
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + xmlDocProduct.Root.Element("ProductUnitRefno").Value + "</ProductUnitRefno>") + ("<ProductCode>" + xmlDocProduct.Root.Element("ProductCode").Value + "</ProductCode>") + ("<ProductUnitIsBC>" + xmlDocProduct.Root.Element("ProductUnitIsBC").Value + "</ProductUnitIsBC>") + ("<UnitCode>" + xmlDocProduct.Root.Element("UnitCode").Value + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                ActItem.Update(request);
                            }
                        }
                        else
                        {
                            ActItem.FKRefno = 0;

                            // Reset values
                            ActDetail.Quantity = 0;
                            ActDetail.BaseQuantity = 0;
                            ActDetail.WeightQuantity = 0;
                            ActDetail.SchemeID = string.Empty;
                            ActDetail.UniqueSchemeID = string.Empty;

                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit, ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await form.DisplayControl.ShowDialog();

                            if (dialogResult == DialogResult.OK)
                            {
                                await form.SaveChanges(this, new EventArgs());
                                xmlDocEdit = form.DisplayControlData;
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");

                                //if (ActItem.DataViewItems.Count > 0)
                                //{
                                //    dataGridMainActivity.UnSelect(dataGridMainActivity.CurrentRowIndex);
                                //}

                                await ActItem.Insert(request, false);

                                if (ActItem.DataViewItems.Count > 0)
                                {
                                    string strOldSort = ActItem.DataViewItems.Sort;
                                    ActItem.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                                    int iRowIdx = ActItem.DataViewItems.Find(new object[] { ActItem.FKRefno });

                                    if (iRowIdx == -1)
                                    {
                                        // TODO: Resource strings
                                        throw (new Exception("Unable to Identify Activity Items"));
                                    }

                                    //dataGridMainActivity.Select(iRowIdx);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                //MessageBox.Show(ex.Message, "Activity Work Sheet", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Activity Work Sheet", ex.Message, "OK"));
            }

            ScannerEnable();
        }

        #endregion

        #region IAppPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        #endregion

        private void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //dataGridMainActivity.Select(dataGridMainActivity.CurrentRowIndex);
            ActItem.SelectItem(e.SelectedItemIndex);
        }
    }
}