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
    using SQLite;
    using Android.Views;
    using System.Collections.ObjectModel;
    using dMobile.Resources.Types;
    using dMobile.Resources.Models;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityPurchaseOrderWorkSheet : DMobileContentPage, IAppArea
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
        private string itemSortByIdx = "ActivityItemPurchaseOrderWorkSheetFKRefno";
        private int prodUnitRefno = 0;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        private bool isFirstAppearance = true;

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
                    if (keycode == Keycode.DpadDown)
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

                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                }

                await this.ucActivityPurchaseOrderWorkSheet_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ucActivityHdr_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

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
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            this.Loading = true;

            await Task.Run(async () =>
            {
                string message = string.Empty;
                //int prodUnitRefno = 0;
                int actItemFoundRefno = 0;
                var currentRowIdx = Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));

                this.BarParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

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
                        XDocument xmlDocProduct = null;

                        if (await IdentifyProductScanned(BarParse, xmlDocProduct))
                        {
                            ProductInfo oProductInfo = new ProductInfo();
                            oProductInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            actItemFoundRefno = await ActItem.GetRefnoFromProdUnitRefno(this.prodUnitRefno);
                            //<issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(oProductInfo, this.BarParse);
                        }

                        if (actItemFoundRefno == 0 && this.prodUnitRefno != 0)
                        {
                            ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.prodUnitRefno);
                            ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(ActItem.ProdInfo.Refno);

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
                            // TODO: Fix sorting
                            //string strOldSort = ActItem.DataViewItems.Sort;
                            //ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            //foundRowIdx = ActItem.DataViewItems.Find(actItemFoundRefno);

                            PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem1 = ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemRefno == actItemFoundRefno).FirstOrDefault();

                            if (purchaseOrderWorkSheetItem1 == null)
                            {
                                // TODO: Resource strings
                                throw (new Exception("Unable To Indentify Item"));
                            }

                            await ActItem.SelectActivityItem(purchaseOrderWorkSheetItem1);

                            // TODO: Fix sorting
                            //ActItem.DataViewItems.Sort = strOldSort;
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
                                xmlDocEdit = await form.GetXML();

                                // TODO: Fix sorting
                                //string strOldSort = ActItem.DataViewItems.Sort;
                                //ActItem.DataViewItems.Sort = "ActivityItemRefno";

                                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem2 = ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemRefno == actItemFoundRefno).FirstOrDefault();

                                if (purchaseOrderWorkSheetItem2 == null)
                                {
                                    // TODO: Resource strings
                                    throw (new Exception("Unable To Indentify Item"));
                                }

                                decimal decQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("alt_qty").Value)) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetQuantityOrdered), 3);
                                decimal decBaseQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("base_qty").Value)) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered), 3);
                                decimal decWeightQty = Math.Round((decimal.Parse(xmlDocEdit.Root.Element("wt_qty").Value)) + Convert.ToDecimal(purchaseOrderWorkSheetItem2.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered), 3);

                                // TODO: Fix sorting
                                // ActItem.DataViewItems.Sort = strOldSort;

                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + decQty + "</alt_qty>") + ("<base_qty>" + decBaseQty + "</base_qty>") + ("<wt_qty>" + decWeightQty + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                await ActItem.Update(request);
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
                                xmlDocEdit = await form.GetXML();
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                await ActItem.Insert(request, false);
                            }
                        }

                        // TODO: Fix sorting
                        // ActItem.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";

                        PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemPurchaseOrderWorkSheetFKRefno == ActItem.FKRefno).FirstOrDefault();

                        if (purchaseOrderWorkSheetItem == null)
                        {
                            // TODO: Resource strings
                            throw (new Exception("Unable To Indentify Item"));
                        }

                        await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = purchaseOrderWorkSheetItem);
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanned Bar Code Type Is Not Supported Here", "OK"));
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Activity Item", ex.Message, "OK"));
                }
                finally
                {
                    await ScannerEnable();
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

            List<tProductUnitBarCodeModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductUnitBarCodeModel>("SELECT * FROM tProductUnitBarCode WHERE ProductUnitBarCodeCode = ? AND ProductUnitBarCodeModifier1 = ?", oBarCodeParse.ProductIdentifier, this.SysEnv.CurrentBarCodeModifier1);

            if (results?.Count > 0)
            {
                iProductUnitBarCodeRefno = results[0].ProductUnitBarCodeRefno;
                this.prodUnitRefno = results[0].ProductUnitRefno;
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
                        xmlDocProduct = await form.GetXML();
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

            return IsProductIdentified;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            try
            {
                this.Loading = true;
                await Activity.Refresh(null);
                await ActItem.Refresh(null);

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ItemsList.Clear();

                    ActItem.ListItems = await ActItem.GetActivityItemList(false);
                    this.DataGridMainActivityItemsList = this.ActItem.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                    if (ActItem.ListItems.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                    {
                        await ActItem.SelectActivityItem(this.DataGridMainActivityItemsList[0]);
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                    }
                });
            }
            catch (SQLiteException ex)
            {
                for (int curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                {
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
            finally
            {
                this.Loading = false;
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
            if (isBarCodeRegistered)
            {
                Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(BarCodeHandler);
                await Scanner.Stop();
                isBarCodeRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
            if (!isBarCodeRegistered)
            {
                isBarCodeRegistered = true;
                await Scanner.Start();
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
                await ScannerDisable();
            }

            return true;
        }

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
        public async Task<XDocument> GetXML()
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
                await ScannerEnable();
                await Device.InvokeOnMainThreadAsync(() => updnBarCodeModifier1.SelectedItem = SysEnv.CurrentBarCodeModifier1);
                LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(LocMap.LoginWhsLocMapRefno);
                await RefreshData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async Task ucActivityHdr_LostFocus(object sender, EventArgs e)
        {
            //if (tmrActivityLoaded.Enabled == false)
            //{
            if (Scanner != null)
            {
                await ScannerDisable();
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

            await ScannerDisable();

            try
            {
                // activity item object needs to check if a new location and product can be added, if not add a detail
                if (LocMap.CurrLocMap.WhsFKRefno == 0 && !SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    //MessageBox.Show("You Have To Set Location!");
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Set Location", "Location Must Be Set!", "OK"));
                }
                else
                {
                    FormDialogContainer formSelectProd = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);

                    await Device.InvokeOnMainThreadAsync(async () => dialogResult = await formSelectProd.DisplayControl.ShowDialog());

                    if (dialogResult == DialogResult.OK)
                    {
                        xmlDocProduct = await formSelectProd.GetXML();
                        ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));
                        ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(ActItem.ProdInfo.Refno);
                        iActivityItemFoundRefno = await ActItem.GetRefnoFromProdUnitRefno(int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value));

                        if (iActivityItemFoundRefno > 0)
                        {
                            //Ask for new quantity
                            FormDialogContainer formPurchaseOrderWorkSheetEdit = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPurchaseOrderWorkSheetEdit, ActDetail, SysCommon.ActSubTypes.NotApplicable);

                            dialogResult = await formPurchaseOrderWorkSheetEdit.DisplayControl.ShowDialog();

                            if (dialogResult == DialogResult.OK)
                            {
                                xmlDocEdit = await formPurchaseOrderWorkSheetEdit.GetXML();
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + xmlDocProduct.Root.Element("ProductUnitRefno").Value + "</ProductUnitRefno>") + ("<ProductCode>" + xmlDocProduct.Root.Element("ProductCode").Value + "</ProductCode>") + ("<ProductUnitIsBC>" + xmlDocProduct.Root.Element("ProductUnitIsBC").Value + "</ProductUnitIsBC>") + ("<UnitCode>" + xmlDocProduct.Root.Element("UnitCode").Value + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");
                                await ActItem.Update(request);
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
                                xmlDocEdit = await form.GetXML();
                                XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + ("<ProductUnitRefno>" + ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>") + ("<ProductCode>" + ActItem.ProdInfo.Code + "</ProductCode>") + ("<ProductUnitIsBC>" + ActItem.ProdInfo.UnitIsBC + "</ProductUnitIsBC>") + ("<UnitCode>" + ActItem.ProdInfo.UnitCode + "</UnitCode>") + ("<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>") + ("<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>") + ("<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>") + ("<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>") + ("<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>") + ("<whs_id>" + LocMap.LoginWhsLocMapRefno + "</whs_id>") + ("<refno>" + ActItem.FKRefno + "</refno>") + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>") + ("<alt_unit>" + ActItem.ProdInfo.UnitCode + "</alt_unit>") + ("<base_unit>" + ActItem.ProdInfo.BaseUnitCode + "</base_unit>") + ("<wt_unit>" + ActItem.ProdInfo.WeightUnitCode + "</wt_unit>") + "</params>");

                                await ActItem.Insert(request, false);

                                if (ActItem.ListItems.Count > 0)
                                {
                                    // TODO: Fix sorting
                                    //string strOldSort = ActItem.DataViewItems.Sort;
                                    //ActItem.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";

                                    PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(item => item.ActivityItemPurchaseOrderWorkSheetFKRefno == ActItem.FKRefno).FirstOrDefault();

                                    if (purchaseOrderWorkSheetItem == null)
                                    {
                                        // TODO: Resource strings
                                        throw (new Exception("Unable to Identify Activity Items"));
                                    }

                                    this.dataGridMainActivity.SelectedItem = purchaseOrderWorkSheetItem;
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

            await ScannerEnable();
        }

        #endregion

        private async void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await ActItem.SelectActivityItem(e.SelectedItem);
            }
        }
    }
}