// <copyright file="ActivityConvertProduct.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    public partial class ActivityConvertProduct : DMobileContentPage, IAppArea
    {
        private int fromProdRefno = 0;
        private int toProdRefno = 0;
        private ProductInfo fromProductInfo;
        private ProductInfo toProductInfo;
        private string saveText;
        private string cancelText;
        private bool isFirstAppearance = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivityConvertProduct()
        {
            this.InitializeComponent();

            this.cmdLocation.Text = "Set Location";
            this.cmdSelectToProduct.Text = "Select To Product";
            this.cmdSelectFromProduct.Text = "Select From Product";
            this.cmdDetail.Text = "Detail";
            this.saveText = "Save";
            this.cancelText = "Cancel";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

                this.InitBarcodeModifier();

                this.Title = this.GetControlDescription();
                this.PopulateOptionsMenu();
            }
        }

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet.
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
            this.BarCodeModifier1Picker.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (string item in modifiers)
                {
                    this.BarCodeModifier1Picker.Items.Add(item);
                }
            }
            else
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
            }

            this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
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
                    _ = await this.ActItem.GetActivityItemList(false);

                    this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertProduct);
                    this.ActDetail.ActSubType = this.ActItem.ActSubType;

                    _ = await this.ActDetail.GetActivityDetailList(false);
                }

                await this.ucActivityConvertUnit_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityConvertUnit_GotFocus threw an error " + ex, "OK"));
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

        private BarCodeScanner Scanner { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private ActDetail ActDetail { get; set; } = null;

        private ActItem ActItem { get; set; } = null;

        private Activity Activity { get; set; } = null;

        private LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        private SysEnvironment SysEnv { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

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

        public ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();
        public List<string> OptionsMenuItems { get; set; } = new List<string>();

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
        /// <remarks>Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        /// Must have a try catch block.</remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                await this.AssertLocationSet();
                this.AssertFromProductSet();

                this.AssertToProductSet();
                this.AssertDetailsSet();
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
                isReadyToClose = false;

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
                        await this.AssertLocationSet();
                        this.AssertFromProductSet();
                        this.AssertToProductSet();
                        this.AssertDetailsSet();
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
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
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    await this.ClearContents();
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
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
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
        private async Task ClearContents()
        {
            this.FromProdRefno = 0;
            this.ToProdRefno = 0;
            this.LocMap = null;
            this.LocMap = LocationMap.GetInstance();
            await this.ActItem.Delete();
            await this.ActDetail.Delete();
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
            _ = req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            _ = req.Append("<imclient>");
            _ = req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            _ = req.Append(await SysCommon.GetRequestDefaults(false));
            _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            _ = req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.HdrFKRefno);
            _ = req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFKRefno);
            _ = req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx].GetType().GetProperty("ProductCode")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<bc>{0}</bc>", detail[idx].GetType().GetProperty("ProductUnitBC")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx].GetType().GetProperty("UnitCode")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            _ = req.AppendFormat("<serial>{0}</serial>", detail[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<pdate>{0}</pdate>", production);
            _ = req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            _ = req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx].GetType().GetProperty("ProductSchemeVariantNumber")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(detail[idx]));
            _ = req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(detail[idx]));
            _ = req.Append("</imclient>");

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
            await this.ShowSelectLocationForm();
        }

        /// <summary>
        /// Shows the select location form.
        /// </summary>
        private async Task ShowSelectLocationForm()
        {
            await this.ScannerDisable();
            FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
                }
            }

            await this.ScannerEnable();
            this.DisplaySummary();
        }

        /// <summary>
        /// Throws an exception if Location is not set.
        /// </summary>
        /// <exception cref="MessageOnlyException" />
        private async Task AssertLocationSet()
        {
            if (!await this.IsLocationSet())
            {
                throw new MessageOnlyException("Location must be set.");
            }
        }

        /// <summary>
        /// Checks if the location is selected.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsLocationSet()
        {
            return await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno);
        }

        /// <summary>
        /// Throws an exception if FROM Product is not set.
        /// </summary>
        /// <exception cref="MessageOnlyException" />
        private void AssertFromProductSet()
        {
            if (!this.IsFromProductSet())
            {
                throw new MessageOnlyException("Please set the from product unit first.");
            }
        }

        /// <summary>
        /// Checks if the TO product matches the FROM product.
        /// If they match, throw a MessageOnlyException.
        /// </summary>
        /// <exception cref="MessageOnlyException"/>
        private void AssertToDoesNotEqualFrom()
        {
            if (this.ToProdRefno == this.FromProdRefno)
            {
                throw new MessageOnlyException("TO Product cannot be the same as FROM Product.");
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
        /// <exception cref="MessageOnlyException" />
        private void AssertToProductSet()
        {
            if (!this.IsToProductSet())
            {
                throw new MessageOnlyException("No convert to product has be set.");
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
        /// <exception cref="MessageOnlyException" />
        private void AssertDetailsSet()
        {
            if (this.ActDetail.ListDetails.Count <= 0)
            {
                throw new MessageOnlyException("No convert to product details have be set.");
            }
        }

        /// <summary>
        /// Selects a Product and returns the ProdRefno if found.
        /// </summary>
        /// <param name="filterAllowProductChange">True to filter the list to only products with AllowChangeProduct.</param>
        /// <returns>ProdRefno if found; otherwise 0.</returns>
        private async Task<int> SelectProduct(bool filterAllowChangeProduct)
        {
            try
            {
                FormDialogContainer frmSelectProduct = new FormDialogContainer(EDialogTypes.SelectProduct, SysCommon.ActSubTypes.ConvertProduct);

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
                    _ = await errRpt.ShowDialog();
                });
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
            await this.ShowSelectFromProductForm();
        }

        /// <summary>
        /// Shows the Select FROM Product Form.
        /// </summary>
        private async Task ShowSelectFromProductForm()
        {
            await this.ScannerDisable();

            try
            {
                await this.AssertLocationSet();

                this.FromProdRefno = await this.SelectProduct(true);

                if (this.FromProdRefno != 0)
                {
                    ProductInfo prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);
                    prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.FromProdRefno);
                    this.FromProdInfo = prodInfo;

                    if (!this.FromProdInfo.IsAllowConvertProduct)
                    {
                        this.FromProdRefno = 0;
                        this.FromProdInfo = new ProductInfo();

                        throw new MessageOnlyException("Cannot convert a product without the Product Flag: Convert Label Adjustment Allow Product Change.");
                    }

                    await this.AssignFromProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
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

            await this.ScannerEnable();
            this.DisplaySummary();
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
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectFromProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " + Convert.ToString(results[0].ProductDescription).TrimEnd());
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
                _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectToProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " + Convert.ToString(results[0].ProductDescription).TrimEnd());
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
            await this.ShowSelectToProductForm();
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

                await this.AssertLocationSet();

                this.ToProdRefno = await this.SelectProduct(false);

                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                if (this.ToProdRefno != 0)
                {
                    ProductInfo prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    this.ToProdInfo = prodInfo;

                    await this.AssignToProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
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
            }

            this.DisplaySummary();
        }

        /// <summary>
        /// Launches the ConvertProductDetails Form.
        /// </summary>
        private async Task ShowConvertProductDetailsForm()
        {
            try
            {
                await this.ScannerDisable();
                await this.AssertLocationSet();
                this.AssertFromProductSet();
                this.AssertToProductSet();

                this.AssertToDoesNotEqualFrom();

                await this.ActItem.Delete();

                this.ActItem.LocMap = this.LocMap.CurrLocMap;
                this.ActItem.ProdInfo = this.ToProdInfo;
                await this.ActItem.Insert(await this.GetXML(), false);
                this.ActDetail = await this.ActDetail.GetInfo(0, true);
                this.ActDetail.ActItem = this.ActItem;

                FormDialogContainer form = new FormDialogContainer(EDialogTypes.ActivityConvertProductDetail, this.ActDetail, SysCommon.ActSubTypes.ConvertProduct);

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
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            finally
            {
                await this.ScannerEnable();
            }

            this.DisplaySummary();
        }

        /// <summary>
        /// Displays the listbox summary data.
        /// </summary>
        private void DisplaySummary()
        {
            // DataRowCollection rows = this.ActDetail.DataViewDetails.Table.Rows;
            // SFillQuantity fillQty = GetActivityItemQuantity();
            this.ExtraInfoItems.Clear();

            if (this.FromProdInfo != null)
            {
                this.ExtraInfoItems.Add(string.Format("From {0} {1} {2}", this.FromProdInfo.BaseUnitCode, this.FromProdInfo.UnitCode, this.FromProdInfo.UnitSizeDesc));
            }

            if (this.ToProdInfo != null)
            {
                this.ExtraInfoItems.Add(string.Format("To {0} {1} {2}", this.ToProdInfo.BaseUnitCode, this.ToProdInfo.UnitCode, this.ToProdInfo.UnitSizeDesc));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private SFillQuantity GetActivityItemQuantity()
        {
            SFillQuantity fillQty = new SFillQuantity();
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
            await this.Load();
        }

        /// <summary>
        /// Enables the form to be edited and loads.
        /// </summary>
        private async Task Load()
        {
            await this.ScannerEnable();

            if (!this.IsControlLoaded)
            {
                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = "Set Location";
                }

                this.IsControlLoaded = true;
            }

            this.DisplaySummary();
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

                if (sender == this.BarCodeModifier1Picker)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
                }
            }
        }

        private async void cmdDetail_Click(object sender, EventArgs e)
        {
            await this.ShowConvertProductDetailsForm();
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
        }

        private async void optionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            string result = await this.DisplayActionSheet("Convert Product Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.saveText)
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
                }
            }
            else if (result == this.cancelText)
            {
                _ = await this.CancelChanges();
                await this.Close();
            }
        }
    }
}