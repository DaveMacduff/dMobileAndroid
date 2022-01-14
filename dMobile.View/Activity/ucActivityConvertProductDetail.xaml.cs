using dMobile.View.CustomPages;
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
using dMobile.BarCodeScanner;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using System.Collections.ObjectModel;
using Java.Security;
using dMobile.View.CustomPages.Modal;
using dMobile.Resources.Types;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertProductDetail : DMobileModal, IAppArea
    {
        #region Fields

        private int fromProductRefno;
        private int toProductRefno;
        private ProductInfo fromProduct;
        private ProductInfo toProduct;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private bool isControlLoaded = false;
        private ActDetail actDetail = null;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();
        private bool isFirstAppearance = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityConvertProductDetail(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            InitializeComponent();

            // TODO: Resource strings
            this.cmdAdd.Text = "Add";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;

                this.ActSubType = actSubTypes;
                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.Title = this.GetControlDescription();
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
                    if (keycode == Keycode.Enter)
                    {
                        await AddManually();
                    }
                    else if (keycode == Keycode.F1)
                    {
                        await this.CloseForm();
                    }
                    else if (keycode == Keycode.Del)
                    {
                        // Commented out because we currently dont support it
                        // but wanted to. Uncomment to add most of this functionality
                        // back in.
                        //DeleteAdjustment();
                    }
                }
            });

            try
            {
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    await this.ActDetail.GetActivityDetailList(false);

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                    });

                    this.LocMap = this.ActDetail.ActItem.LocMap;

                    this.ActDetail.ActItem.SelectedActivityItem = null;

                    if (this.ActDetail.ActItem.ListItems[0] is ConvertProductItem convertProductItem)
                    {
                        FromProdRefno = convertProductItem.From_prod_fk_refno;
                        ToProdRefno = convertProductItem.To_prod_fk_refno;
                    }

                    ProductInfo prodInfo = new ProductInfo();
                    ToProduct = await ProductInfoTasks.GetInfoFromRefno(ToProdRefno);
                    FromProduct = await ProductInfoTasks.GetInfoFromRefno(FromProdRefno);

                    // fill in all the activity data
                    this.ActDetail.ActItem.ProdInfo = ToProduct;
                }

                await this.ucActivityConvertProductDetail_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityConvertProductDetail_GotFocus threw an error " + ex, "OK"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await this.CancelChanges();
                base.OnBackButtonPressed();
                await this.CloseForm();
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

        /// <summary>
        /// The product we are changing out of.
        /// </summary>
        private int FromProdRefno
        {
            get
            {
                return this.fromProductRefno;
            }
            set
            {
                this.fromProductRefno = value;
            }
        }

        /// <summary>
        /// The desired product to change into.
        /// </summary>
        private int ToProdRefno
        {
            get
            {
                return this.toProductRefno;
            }
            set
            {
                this.toProductRefno = value;
            }
        }

        private ProductInfo ToProduct
        {
            get
            {
                return this.toProduct;
            }
            set
            {
                this.toProduct = value;
            }
        }

        private ProductInfo FromProduct
        {
            get
            {
                return this.fromProduct;
            }
            set
            {
                this.fromProduct = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.ActDetail.LocMap;
            }
            set
            {
                this.ActDetail.LocMap = value;
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
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

        #endregion

        #region Public Members

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
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = this.IsValidData();
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
            return "Convert Products";
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
                    if (HasData() &&
                        //this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&
                        this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch && // batch means print at save instead of by item
                        !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // weren't allowed to print on add or scan, so print now

                        // TODO: Resource strings
                        if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Print Label", "Print Product Labels?", "Yes", "No")))
                        {
                            await PrintBatchProductLabels();
                        }
                    }

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK"));
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
                    // Change any values back that you need
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
        public async Task<XDocument> GetXML()
        {
            string unqScheme = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            string scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            SProductSchemeData schemeData = this.ActDetail.SchemeData;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + schemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + schemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>")
                + ("<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>")
                + ("<scheme_id>" + scheme + "</scheme_id>")
                + ("<unq_scheme_id>" + unqScheme + "</unq_scheme_id>")
                + "</params>");

            return request;
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
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            BarCodeParse parse = null;
            bool isInsertReady = false;

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));
                }
                else
                {
                    AssertNotLineItem(parse);
                    AssertNotLineItemDetail(parse);
                    AssertNotShippingContainer(parse);

                    int prodUnitRefno = 0;
                    prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, prodUnitRefno);
                    // Check if product scanned matches the FROM product
                    // Location is set in ConvertProduct screen

                    // Product identifier
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        int tempFromProdUnitRefno = 0;

                        tempFromProdUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, tempFromProdUnitRefno);

                        ProductInfo scannedProduct = new ProductInfo();
                        scannedProduct = await ProductInfoTasks.GetInfoFromUnitRefno(tempFromProdUnitRefno);

                        //this.FromProdRefno = tempFromProdRefno;

                        // Insert a new activity detail record
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


                        // Assigns all data to the ActivityDetail
                        isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

                        // Confirm that scanned matches from data. If not, then what is wrong
                        if (scannedProduct.Code != FromProduct.Code)
                        {
                            throw new Exceptions.MessageOnlyException("Cannot scan a different \"From\" product than what was set.", null, SysCommon.ShowErrorMessageType.MessageBox);
                        }

                        XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                        await CreateAdjustment(xmlDoc);
                        // We check if we are allowed to print on scan
                        if (//this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct && // TODO possibly remove this.
                            !this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                            !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await PrintProductLabel();
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
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
                this.Loading = false;
            }
        }

        private static void AssertNotShippingContainer(BarCodeParse parse)
        {
            // Shipping container scanned, skid
            if (!string.IsNullOrEmpty(parse.SSCC18))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported here!");
            }
        }

        private static void AssertNotLineItemDetail(BarCodeParse parse)
        {
            // Activity detail fk refno
            if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here!");
            }
        }

        private static void AssertNotLineItem(BarCodeParse parse)
        {
            // Activity item fk refno
            if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Scanning a line item bar code is not supported here!");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            return true;
        }


        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySummary()
        {
            this.ItemsList.Clear();

            if (this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                // TODO: Resource strings
                this.ItemsList.Add("Batch printing enabled.");
            }
            else
            {
                // TODO: Resource strings
                this.ItemsList.Add("Print labels on scan.");
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
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
        }

        /// <summary>
        /// Adding products manually instead of scanning.
        /// </summary>
        private async Task AddManually()
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = await form.GetXML();

                try
                {
                    this.Loading = true;

                    await Task.Run(async () =>
                    {
                        await CreateAdjustment(xmlDoc);

                        // We check if we are allowed to print on scan
                        if (
                                //(this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&// TODO remove true || (workaround com server not updated with value yet)
                                !this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                                !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await PrintProductLabel();
                        }

                    });
                }
                catch (Exception ex)
                {
                    // TODO: logging
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", ex.Message, "OK"));
                }
                finally
                {
                    this.Loading = false;
                }
            }
        }

        /// <summary>
        /// Creates an adjustment for the product.
        /// </summary>
        /// <remarks>
        /// This sends what we wish to be an activity detail to the
        /// COM server. The COM server sees it doesn't exist and creates it,
        /// then it sends back three fk_refnos for activity, activity item, and activity detail.
        /// Once received it will then need to update the table values with the fk_refnos.
        /// </remarks>
        /// <param name="data">Data used for the row to be created.</param>
        private async Task CreateAdjustment(XDocument data)
        {
            try
            {
                // Create adjustment
                string request = await this.ActDetail.CreateSubmitXML(data);
                string response = SysCommon.SubmitComm(request);
                SysCommon.CheckCommResponseForErrors(response);

                // Parse response for the updated fk_refnos
                // Modify and change the ActivityDetail/Item/Header to be up to date with the reponse fk_refnos
                XDocument responseData = XDocument.Parse(response);
                this.ActDetail.ActItem.ActHdr.FKRefno = Convert.ToInt32(responseData.Root.Element("fk_refno").Value);
                this.ActDetail.ActItem.FKRefno = Convert.ToInt32(responseData.Root.Element("fkit_refno").Value);
                this.ActDetail.FKRefno = Convert.ToInt32(responseData.Root.Element("ser_refno").Value);

                data.Root.Add(new XElement("hdr_refno", this.ActDetail.ActItem.ActHdr.FKRefno));
                data.Root.Add(new XElement("itm_refno", this.ActDetail.ActItem.FKRefno));
                data.Root.Element("ser_refno").SetValue(this.ActDetail.FKRefno);

                await this.ActDetail.Insert(data);

                SelectRow(this.ActDetail.ListDetails.Count - 1);
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
        }


        /// <summary>
        /// Removes the adjustment from happening.
        /// </summary>
        /// <remarks>
        /// Sends the fk_refno of the detail to remove to the COM server.
        /// </remarks>
        private async Task DeleteAdjustment()
        {
            if (NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                XDocument data = new XDocument();
                // TODO send the fk_refno to delete over
                string sendRequest = await this.ActDetail.CreateDeleteXML(data);
                string response = SysCommon.SubmitComm(sendRequest);
                SysCommon.CheckCommResponseForErrors(response);

                // Remove from grid
                int selectedIndex = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

                this.ActDetail.ListDetails.RemoveAt(selectedIndex);
                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }

                if (this.ActDetail.ListDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > selectedIndex)
                {
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[selectedIndex];
                }
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// Prints all product labels if batch.
        /// </summary>
        private async Task PrintBatchProductLabels()
        {
            if (!this.sysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                return;
            }

            foreach (object obj in this.ActDetail.ListDetails)
            {
                // Set the detail, item, header, product, scheme, scheme data info
                await this.ActDetail.SelectActivityDetail(obj);

                // Print using that data
                await PrintProductLabel();
            }
        }

        /// <summary>
        /// Causes all current adjustments to pring prooduct labels.
        /// </summary>
        /// <remarks>
        /// Sends an xml string to the COM server to print the product labels.
        /// </remarks>
        private async Task PrintProductLabel()
        {
            if (NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                XDocument data = new XDocument();
                string sendRequest = await this.ActDetail.CreateSubmitProductLabelXML(data);
                string response = SysCommon.SubmitComm(sendRequest);
                SysCommon.CheckCommResponseForErrors(response);
            }
            catch (Exceptions.MessageOnlyException me)
            {
                me.Show(SysCommon.ActSubTypes.ConvertProduct);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// Checks if the Activity Detail has 0 data rows.
        /// </summary>
        /// <returns>True if there are no data rows.</returns>
        private bool NoData()
        {
            return this.ActDetail.ListDetails.Count == 0;
        }

        private bool HasData()
        {
            return !NoData();
        }

        #endregion

        /// <summary>
        /// Saves or cancels changes and exits.
        /// </summary>
        private async Task CloseForm()
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.None;
                await this.CancelChanges();
            }
        }

        /// <summary>
        /// Click handler for the Add button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            await AddManually();
        }

        private async Task ucActivityConvertProductDetail_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                }
            }
        }

        private void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectRow(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
        }

        private void SelectRow(int row)
        {
            if (row < 0)
            {
                return;
            }

            this.ActDetail.ActItem.SelectedActivityItem = this.ActDetail.ActItem.ListItems[row];
            this.DisplaySummary();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.DataGridMainActivityItemsList = this.ActDetail.Search(e.NewTextValue);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
        }
    }
}