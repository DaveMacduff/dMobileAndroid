using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.CustomPages;
using System.Xml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using SQLite;
using Android.Views;
using System.Collections.ObjectModel;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    public partial class ucActivityConvertProduct : DMobileContentPage, IAppArea
    {
        #region Fields

        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private int fromProdRefno = 0;
        private int toProdRefno = 0;
        private ProductInfo fromProductInfo;
        private ProductInfo toProductInfo;
        private SysCommon.ActSubTypes actSubType;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private List<string> optionsMenuItems = new List<string>();
        private string saveText;
        private string cancelText;
        private bool isFirstAppearance = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ucActivityConvertProduct()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectToProduct.Text = "Select To Product";
            this.cmdSelectFromProduct.Text = "Select From Product";
            this.cmdDetail.Text = "Detail";
            this.saveText = "Save";
            this.cancelText = "Cancel";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

                InitBarcodeModifier();

                this.Title = this.GetControlDescription();
                this.PopulateOptionsMenu();
            }
        }

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();

            this.OptionsMenuItems.Add(this.saveText);
            this.OptionsMenuItems.Add(this.cancelText);
        }

        /// <summary>
        /// Initializes the barcode modifier.
        /// </summary>
        private void InitBarcodeModifier()
        {
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
                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertProduct);
                    this.Activity.SkidInfo = new SysSkidInfo();
                    this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertProduct;

                    this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.ConvertProduct);
                    this.ActItem.ActSubType = this.Activity.ActSubType;
                    await this.ActItem.GetActivityItemList(false);

                    this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertProduct);
                    this.ActDetail.ActSubType = this.ActItem.ActSubType;

                    await this.ActDetail.GetActivityDetailList(false);
                }

                await this.ucActivityConvertUnit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityConvertUnit_GotFocus threw an error " + ex, "OK"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await this.CancelChanges();
                base.OnBackButtonPressed();
                await this.Close();
            });

            return true;
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

        private int FromProdRefno
        {
            get
            {
                return this.fromProdRefno;
            }

            set
            {
                this.fromProdRefno = value;
                (this.ActItem as ConvertProductActItem).FromProdRefno = value;
            }
        }

        private int ToProdRefno
        {
            get
            {
                return this.toProdRefno;
            }

            set
            {
                this.toProdRefno = value;
                (this.ActItem as ConvertProductActItem).ToProdRefno = value;
            }
        }

        private ProductInfo FromProdInfo
        {
            get
            {
                return this.fromProductInfo;
            }
            set
            {
                this.fromProductInfo = value;
                this.FromProdRefno = value.Refno;
            }
        }


        private ProductInfo ToProdInfo
        {
            get
            {
                return this.toProductInfo;
            }
            set
            {
                this.toProductInfo = value;
                this.ToProdRefno = value.Refno;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public List<string> OptionsMenuItems { get => optionsMenuItems; set => optionsMenuItems = value; }

        #endregion

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
                await AssertLocationSet();
                AssertFromProductSet();

                AssertToProductSet();
                AssertDetailsSet();
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
            return "Convert Product";
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
                    if (this.FromProdRefno != 0 && this.ActItem.ListItems.Count != 0 && this.ActDetail.ListDetails.Count != 0)
                    {
                        saveComplete = true;
                        this.IsChangesMade = false;
                    }
                    else
                    {
                        await AssertLocationSet();
                        AssertFromProductSet();
                        AssertToProductSet();
                        AssertDetailsSet();
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
                    this.ClearContents();
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
        public async Task<XDocument> GetXML()
        {
            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<from_prod_fk_refno>" + ((this.FromProdInfo != null) ? this.FromProdInfo.Refno.ToString() : string.Empty) + "</from_prod_fk_refno>"
                + "<to_prod_fk_refno>" + ((this.ToProdInfo != null) ? this.ToProdInfo.Refno.ToString() : string.Empty) + "</to_prod_fk_refno>"
                + "<location_map_refno>" + this.LocMap.CurrLocMapRefno + "</location_map_refno>"
                + "</params>");

            return xmlDoc;
        }

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
            return; // no barcode handler
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ClearContents()
        {
            this.FromProdRefno = 0;
            this.ToProdRefno = 0;
            this.LocMap.CurrLocMap = new SLocMap();
            this.LocMap = LocationMap.GetInstance();
            this.ActItem.Delete();
            this.ActDetail.Delete();
            this.cmdLocation.Text = "Set Location";
            this.cmdSelectFromProduct.Text = "Select From Product";
            this.cmdSelectToProduct.Text = "Select To Product";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private async Task<string> MakeProductLabelsSubmitString(int idx)
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            List<object> detail = this.ActDetail.ListDetails;

            prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(detail[idx].GetType().GetProperty("ProductUnitRefno")?.GetValue(detail[idx])));
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")?.GetValue(detail[idx])).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")?.GetValue(detail[idx])).ToString("MM/dd/yyyy");
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
            req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx].GetType().GetProperty("ProductCode")?.GetValue(detail[idx]));
            req.AppendFormat("<bc>{0}</bc>", detail[idx].GetType().GetProperty("ProductUnitBC")?.GetValue(detail[idx]));
            req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx].GetType().GetProperty("UnitCode")?.GetValue(detail[idx]));
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", detail[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(detail[idx]));
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(detail[idx]));
            req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(detail[idx]));
            req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx].GetType().GetProperty("ProductSchemeVariantNumber")?.GetValue(detail[idx]));
            req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(detail[idx]));
            req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(detail[idx]));
            req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(detail[idx]));
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
            await ShowSelectLocationForm();
        }

        /// <summary>
        /// Shows the select location form.
        /// </summary>
        private async Task ShowSelectLocationForm()
        {
            await this.ScannerDisable();
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
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

            await this.ScannerEnable();
            DisplaySummary();
        }




        /// <summary>
        /// Throws an exception if Location is not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private async Task AssertLocationSet()
        {
            if (!await IsLocationSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Location must be set.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }

        }

        /// <summary>
        /// Checks if the location is selected.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsLocationSet()
        {
            return await LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno);
        }


        /// <summary>
        /// Throws an exception if FROM Product is not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertFromProductSet()
        {
            if (!IsFromProductSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Please set the from product unit first.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }


        /// <summary>
        /// Checks if the TO product matches the FROM product.
        /// If they match, throw a Exceptions.MessageOnlyException.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException"/>
        private void AssertToDoesNotEqualFrom()
        {
            if (this.ToProdRefno == this.FromProdRefno)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("TO Product cannot be the same as FROM Product.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }

        /// <summary>
        /// Checks if the FROM product is selected.
        /// </summary>
        /// <returns></returns>
        private bool IsFromProductSet()
        {
            return this.FromProdRefno != 0;
        }

        /// <summary>
        /// Checks if the convert TO product item is set.
        /// Throws if not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertToProductSet()
        {

            if (!IsToProductSet())
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("No convert to product has be set.", null, SysCommon.ShowErrorMessageType.MessageBox);
            }
        }
        /// <summary>
        /// Checks if the TO product is selected.
        /// </summary>
        /// <returns></returns>
        private bool IsToProductSet()
        {
            return this.ToProdRefno != 0;
        }

        /// <summary>
        /// Checks if the convert TO product item details is set.
        /// Throws if not set.
        /// </summary>
        /// <exception cref="Exceptions.MessageOnlyException" />
        private void AssertDetailsSet()
        {
            if (0 >= this.ActDetail.ListDetails.Count)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("No convert to product details have be set.");
            }
        }


        /// <summary>
        /// Selects a Product and returns the ProdRefno if found.
        /// </summary>
        /// <param name="filterAllowProductChange">True to filter the list to only products with AllowChangeProduct</param>
        /// <returns>ProdRefno if found; otherwise 0.</returns>
        private async Task<int> SelectProduct(bool filterAllowChangeProduct)
        {
            try
            {
                FormDialogContainer frmSelectProduct = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProduct, SysCommon.ActSubTypes.ConvertProduct);

                if (filterAllowChangeProduct)
                {
                    (frmSelectProduct.DisplayControl as ucSelectProduct).FilterOnAllowConvertProduct = true;
                }

                if (await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    XDocument xmlDocProduct = await frmSelectProduct.GetXML();
                    return Convert.ToInt32(xmlDocProduct.Root.Element("ProductRefno").Value);
                }
            }
            catch (FormatException)
            {
                // failed to convert to an integer
                // This is ignored and returned as a -1
            }
            catch (OverflowException)
            {
                // failed to convert to an integer.
                // This is ignored and returned as a -1
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            await ShowSelectFromProductForm();
        }

        /// <summary>
        /// Shows the Select FROM Product Form.
        /// </summary>
        private async Task ShowSelectFromProductForm()
        {
            await this.ScannerDisable();

            try
            {
                await AssertLocationSet();

                this.FromProdRefno = await SelectProduct(true);

                if (this.FromProdRefno != 0)
                {
                    ProductInfo prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);
                    prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.FromProdRefno);
                    FromProdInfo = prodInfo;

                    if (!FromProdInfo.IsAllowConvertProduct)
                    {
                        FromProdRefno = 0;
                        FromProdInfo = new ProductInfo();

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Cannot convert a product without the Product Flag: Convert Label Adjustment Allow Product Change.", null, SysCommon.ShowErrorMessageType.MessageBox);
                    }

                    await AssignFromProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            await this.ScannerEnable();
            DisplaySummary();
        }

        /// <summary>
        /// Assigns the FROM Product button text to reflect the product selected.
        /// </summary>
        private async Task AssignFromProductText()
        {
            string query = "SELECT tProduct.ProductCode,"
                + "tProduct.ProductDescription,"
                + "tProductUnit.ProductUnitBC,"
                + "tUnit.UnitCode "
                + "FROM tProductUnit "
                + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                + "WHERE tProductUnit.ProductUnitRefno = ?";

            List<ProductUnit> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnit>(query, this.FromProdInfo.UnitRefno);

            if (results?.Count > 0)
            {
                await Device.InvokeOnMainThreadAsync(() => this.cmdSelectFromProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " + Convert.ToString(results[0].ProductDescription).TrimEnd());

            }
        }


        /// <summary>
        /// Assigns the TO Product button text to reflect the product selected.
        /// </summary>
        private async Task AssignToProductText()
        {
            string query = "SELECT tProduct.ProductCode,"
            + "tProduct.ProductDescription,"
            + "tProductUnit.ProductUnitBC,"
            + "tUnit.UnitCode "
            + "FROM tProductUnit "
            + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
            + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
            + "WHERE tProductUnit.ProductUnitRefno = ?";

            List<ProductUnit> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnit>(query, this.ToProdInfo.UnitRefno);

            if (results?.Count > 0)
            {
                await Device.InvokeOnMainThreadAsync(() => this.cmdSelectToProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " + Convert.ToString(results[0].ProductDescription).TrimEnd());

            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectToUnit_Click(object sender, EventArgs e)
        {
            await ShowSelectToProductForm();
        }

        /// <summary>
        /// Shows the Select TO Product Form.
        /// </summary>
        private async Task ShowSelectToProductForm()
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl causes a click event to fire if it is the next control
                await this.ScannerDisable();

                await AssertLocationSet();

                this.ToProdRefno = await SelectProduct(false);

                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                if (this.ToProdRefno != 0)
                {
                    ProductInfo prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    ToProdInfo = prodInfo;

                    await AssignToProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            DisplaySummary();
        }


        /// <summary>
        /// Launches the ConvertProductDetails Form.
        /// </summary>
        private async Task ShowConvertProductDetailsForm()
        {
            try
            {
                await this.ScannerDisable();
                await AssertLocationSet();
                AssertFromProductSet();
                AssertToProductSet();

                AssertToDoesNotEqualFrom();

                await this.ActItem.Delete();

                this.ActItem.LocMap = this.LocMap;
                this.ActItem.ProdInfo = this.ToProdInfo;
                await this.ActItem.Insert(await GetXML(), false);
                this.ActDetail = await this.ActDetail.GetInfo(0, true);
                this.ActDetail.ActItem = this.ActItem;

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityConvertProductDetail, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    // Detail Save
                    // If save, we send to comm server
                }
                else
                {
                    // Detail Cancel
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            finally
            {
                await this.ScannerEnable();
            }

            DisplaySummary();
        }

        /// <summary>
        /// Displays the listbox summary data.
        /// </summary>
        private void DisplaySummary()
        {
            //DataRowCollection rows = this.ActDetail.DataViewDetails.Table.Rows;
            //SysCommon.SFillQuantity fillQty = GetActivityItemQuantity();

            this.ItemsList.Clear();

            if (FromProdInfo != null)
            {
                // TODO: Resource strings
                this.ItemsList.Add(string.Format("From {0} {1} {2}", FromProdInfo.BaseUnitCode, FromProdInfo.UnitCode, FromProdInfo.UnitSizeDesc));
            }

            if (ToProdInfo != null)
            {
                // TODO: Resource strings
                this.ItemsList.Add(string.Format("To {0} {1} {2}", ToProdInfo.BaseUnitCode, ToProdInfo.UnitCode, ToProdInfo.UnitSizeDesc));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private SysCommon.SFillQuantity GetActivityItemQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();
            List<object> detailList = this.ActDetail.ListDetails;

            if (detailList != null)
            {
                int count = detailList.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        fillQty.Quantity += Convert.ToDecimal(detailList[i].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(detailList[i]));
                        fillQty.BaseQuantity += Convert.ToDecimal(detailList[i].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(detailList[i]));
                        fillQty.WeightQuantity += Convert.ToDecimal(detailList[i].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(detailList[i]));
                    }
                }
            }

            return fillQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityConvertUnit_GotFocus(object sender, EventArgs e)
        {
            await Load();
        }

        /// <summary>
        /// Enables the form to be edited and loads.
        /// </summary>
        private async Task Load()
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

                this.DisplaySummary();
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

        private async void cmdDetail_Click(object sender, EventArgs e)
        {
            await ShowConvertProductDetailsForm();
        }

        private void updnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
        }

        private async void optionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            // TODO: Resource strings
            string result = await DisplayActionSheet("Convert Product Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.saveText)
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
                }
            }
            else if (result == this.cancelText)
            {
                await this.CancelChanges();
                await this.Close();
            }
        }
    }
}