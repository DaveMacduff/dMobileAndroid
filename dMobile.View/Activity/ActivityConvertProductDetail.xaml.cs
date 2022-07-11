// <copyright file="ActivityConvertProductDetail.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertProductDetail : DMobileModal, IAppArea
    {
        private bool isFirstAppearance = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actDetail"></param>
        /// <param name="actSubTypes"></param>
        public ActivityConvertProductDetail(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.cmdAdd.Text = "Add";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.Scanner = ScannerEngine.GetInstance().Scanner;

                this.ActSubType = actSubTypes;
                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        await this.AddManually();
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
                        // DeleteAdjustment();
                    }
                }
            });

            try
            {
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;
                    _ = await this.ActDetail.GetActivityDetailList(false);

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                    });

                    this.LocMap = this.ActDetail.ActItem.LocMap;

                    this.ActDetail.ActItem.SelectedActivityItem = null;

                    if (this.ActDetail.ActItem.ListItems[0] is ConvertProductItem convertProductItem)
                    {
                        this.FromProdRefno = convertProductItem.From_prod_fk_refno;
                        this.ToProdRefno = convertProductItem.To_prod_fk_refno;
                    }

                    ProductInfo prodInfo = new ProductInfo();
                    this.ToProduct = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    this.FromProduct = await ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);

                    // fill in all the activity data
                    this.ActDetail.ActItem.ProdInfo = this.ToProduct;
                }

                await this.ucActivityConvertProductDetail_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityConvertProductDetail_GotFocus threw an error " + ex, "OK"));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = await this.CancelChanges();
                _ = base.OnBackButtonPressed();
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        /// <summary>
        /// The product we are changing out of.
        /// </summary>
        private int FromProdRefno { get; set; }

        /// <summary>
        /// The desired product to change into.
        /// </summary>
        private int ToProdRefno { get; set; }

        private ProductInfo ToProduct { get; set; }

        private ProductInfo FromProduct { get; set; }

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

        private SysEnvironment SysEnv { get; set; }

        private BarCodeScanner Scanner { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private ActDetail ActDetail { get; set; } = null;

        public ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
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
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
                    if (this.HasData() &&
                        // this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&
                        this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch && // batch means print at save instead of by item
                        !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // weren't allowed to print on add or scan, so print now
                        if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Print Label", "Print Product Labels?", "Yes", "No")))
                        {
                            await this.PrintBatchProductLabels();
                        }
                    }

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Data Validation", "Data failed validation. Check your data.", "OK"));
                }
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
                    _ = await errRpt.ShowDialog();
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
            string unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
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
                + "<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>"
                + "<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                + "<variant_code>" + schemeData.VariantCode + "</variant_code>"
                + "<variant_no>" + schemeData.VariantNumber + "</variant_no>"
                + "<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>"
                + "<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>"
                + "<scheme_id>" + scheme + "</scheme_id>"
                + "<unq_scheme_id>" + unqScheme + "</unq_scheme_id>"
                + "</params>");

            return request;
        }

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

                        // this.FromProdRefno = tempFromProdRefno;

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
                        this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                        // Assigns all data to the ActivityDetail
                        isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

                        // Confirm that scanned matches from data. If not, then what is wrong
                        if (scannedProduct.Code != this.FromProduct.Code)
                        {
                            throw new MessageOnlyException("Cannot scan a different \"From\" product than what was set.");
                        }

                        XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                        await this.CreateAdjustment(xmlDoc);
                        // We check if we are allowed to print on scan
                        if (// this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct && // TODO possibly remove this.
                            !this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                            !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await this.PrintProductLabel();
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
                throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
            }
        }

        private static void AssertNotLineItemDetail(BarCodeParse parse)
        {
            // Activity detail fk refno
            if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
            {
                throw new MessageOnlyException("Scanning an line item detail bar code is not supported here!");
            }
        }

        private static void AssertNotLineItem(BarCodeParse parse)
        {
            // Activity item fk refno
            if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
            {
                throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
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
            this.ExtraInfoItems.Clear();

            if (this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                this.ExtraInfoItems.Add("Batch printing enabled.");
            }
            else
            {
                this.ExtraInfoItems.Add("Print labels on scan.");
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
            FormDialogContainer form = new FormDialogContainer(EDialogTypes.ActivityDetailEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = await form.GetXML();

                try
                {
                    this.Loading = true;

                    await Task.Run(async () =>
                    {
                        await this.CreateAdjustment(xmlDoc);

                        // We check if we are allowed to print on scan
                        if (
                                // (this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct &&// TODO remove true || (workaround com server not updated with value yet)
                                !this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch &&
                                !this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            await this.PrintProductLabel();
                        }
                    });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", ex.Message, "OK"));
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
                this.ActDetail.ActItem.ActHdr.HdrFKRefno = Convert.ToInt32(responseData.Root.Element("fk_refno").Value);
                this.ActDetail.ActItem.ItemFKRefno = Convert.ToInt32(responseData.Root.Element("fkit_refno").Value);
                this.ActDetail.DetailFKRefno = Convert.ToInt32(responseData.Root.Element("ser_refno").Value);

                data.Root.Add(new XElement("hdr_refno", this.ActDetail.ActItem.ActHdr.HdrFKRefno));
                data.Root.Add(new XElement("itm_refno", this.ActDetail.ActItem.ItemFKRefno));
                data.Root.Element("ser_refno").SetValue(this.ActDetail.DetailFKRefno);

                await this.ActDetail.Insert(data);

                this.SelectRow(this.ActDetail.ListDetails.Count - 1);
            }
            catch (MessageOnlyException me)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        me.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
            if (this.NoData())
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
                int selectedIndex = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);

                this.ActDetail.ListDetails.RemoveAt(selectedIndex);
                this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > selectedIndex)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[selectedIndex];
                }
            }
            catch (MessageOnlyException me)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        me.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// Prints all product labels if batch.
        /// </summary>
        private async Task PrintBatchProductLabels()
        {
            if (!this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                return;
            }

            foreach (object obj in this.ActDetail.ListDetails)
            {
                // Set the detail, item, header, product, scheme, scheme data info
                await this.ActDetail.SelectActivityDetail(obj);

                // Print using that data
                await this.PrintProductLabel();
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
            if (this.NoData())
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
            catch (MessageOnlyException me)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        me.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
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
            return !this.NoData();
        }

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
                _ = await this.CancelChanges();
            }
        }

        /// <summary>
        /// Click handler for the Add button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            await this.AddManually();
        }

        private async Task ucActivityConvertProductDetail_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();

            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
            }
        }

        private void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.SelectRow(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem));
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
            this.MainActivityItems = this.ActDetail.Search(e.NewTextValue);
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
        }
    }
}