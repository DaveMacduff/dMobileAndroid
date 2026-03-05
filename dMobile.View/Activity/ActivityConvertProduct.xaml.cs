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
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Forms;
    using Resources.Enums;
    using Resources.Types.Products;
    using SQLite;
    using Views;
    using Xamarin.Forms;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Activity convert product.
    /// </summary>
    public partial class ActivityConvertProduct : IAppArea
    {
        private const string SaveText = "Save";
        private const string CancelText = "Cancel";
        private int fromProdRefno;
        private ProductInfo fromProductInfo;
        private int toProdRefno;
        private ProductInfo toProductInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityConvertProduct"/> class.
        /// </summary>
        public ActivityConvertProduct()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.CmdLocation.Text = "Set Location";
            this.CmdSelectToProduct.Text = "Select To Product";
            this.CmdSelectFromProduct.Text = "Select From Product";
            this.CmdDetail.Text = "Detail";

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();

            ParseAndSeedBarCodeModifier();

            this.Title = GetControlDescription();
            this.PopulateOptionsMenu();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

            // Initialize the activity.
            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertProduct);
            this.Activity.SkidInfo = new SysSkidInfo();
            this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertProduct;

            // Initialize the activity item.
            this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.ConvertProduct);
            this.ActItem.ActSubType = this.Activity.ActSubType;
            this.ActItem.GetActivityItemList(false).FireAndForgetSafeAsync();

            // Initialize the activity detail.
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertProduct);
            this.ActDetail.ActSubType = this.ActItem.ActSubType;
            this.ActDetail.GetActivityDetailList(false).FireAndForgetSafeAsync();

            this.DataCollector = new DataCollector(this.ActSubType, this.ActItem);

            this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                this.CmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            }
            else
            {
                this.CmdLocation.Text = "Set Location";
            }

            this.DisplaySummary();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.IsFirstAppearance = true;
            return;

            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();
                if (!this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    return;
                }

                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
                foreach (var item in modifiers)
                {
                    this.BarCodeModifier1Picker.Items.Add(item);
                }

                if (this.BarCodeModifier1Picker.Items.Count > 0)
                {
                    this.BarCodeModifier1Picker.IsVisible = true;
                    this.BarCodeModifier1Picker.IsEnabled = true;
                }

                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
            }
        }

        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.ConvertProduct;

        private ActDetail ActDetail { get; set; }

        private ActItem ActItem { get; set; }

        private Activity Activity { get; set; }

        private LocationMap LocMap { get; set; }

        private SysEnvironment SysEnv { get; }

        private bool IsDataLoading { get; set; }

        private bool IsChangesMade { get; set; }

        private int FromProdRefno
        {
            get => this.fromProdRefno;

            set
            {
                this.fromProdRefno = value;
                ((ConvertProductActItem)this.ActItem).FromProdRefno = value;
            }
        }

        private int ToProdRefno
        {
            get => this.toProdRefno;

            set
            {
                this.toProdRefno = value;
                ((ConvertProductActItem)this.ActItem).ToProdRefno = value;
            }
        }

        private ProductInfo FromProdInfo
        {
            get => this.fromProductInfo;
            set
            {
                this.fromProductInfo = value;
                this.FromProdRefno = value.Refno;
            }
        }

        private ProductInfo ToProdInfo
        {
            get => this.toProductInfo;
            set
            {
                this.toProductInfo = value;
                this.ToProdRefno = value.Refno;
            }
        }

        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        private List<string> OptionsMenuItems { get; } = new List<string>();

        private bool IsFirstAppearance { get; set; }

        private DataCollector DataCollector { get; set; }

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            await this.AssertLocationSet();
            this.AssertFromProductSet();
            this.AssertToProductSet();
            this.AssertDetailsSet();
            return true;
        }

        /// <summary>
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity item is ready to close, else false.</returns>
        public Task<bool> IsReadyToClose()
        {
            // Since this method does not perform any asynchronous operations,
            // it does not need to be marked as 'async'. Removing 'async' resolves the CS1998 warning.
            return Task.FromResult(!this.IsChangesMade);
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                await this.IsReadyToSave();
                if (this.FromProdRefno != 0 && this.ActItem.ListItems.Count != 0 &&
                    this.ActDetail.ListDetails.Count != 0)
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
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                await this.IsReadyToClose();
                await this.ClearContents();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
        public async Task<XDocument> GetXml()
        {
            return await Task.FromResult(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<from_prod_fk_refno>" +
                (this.FromProdInfo != null ? this.FromProdInfo.Refno.ToString() : string.Empty) +
                "</from_prod_fk_refno>"
                + "<to_prod_fk_refno>" + (this.ToProdInfo != null ? this.ToProdInfo.Refno.ToString() : string.Empty) +
                "</to_prod_fk_refno>"
                + "<location_map_refno>" + this.LocMap.CurrLocMapRefno + "</location_map_refno>"
                + "</params>"));
        }

        /// <summary>
        /// Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();
            if (this.IsFirstAppearance)
            {
                this.IsFirstAppearance = false;
                await this.ScannerEnable();
            }

            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, nameof(EventTypes.OnKeyUp), KeyUpHandlerCallback);
            }

            async void KeyUpHandlerCallback(string keyPressed)
            {
                if (this.IsDataLoading)
                {
                    return;
                }

                var isClosing = false;
                try
                {
                    await this.ScannerDisable();
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);
                    switch (keycode)
                    {
                        case Keycode.Enter:
                            if (await this.SaveChanges())
                            {
                                await this.ScannerDisable();
                                isClosing = true;
                                await this.Close();
                            }

                            break;

                        case Keycode.F1:
                            if (await this.SaveChanges())
                            {
                                await this.ScannerDisable();
                                isClosing = true;
                                await this.Close();
                            }

                            break;
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    if (!isClosing)
                    {
                        await this.ScannerEnable();
                    }
                }
            }
        }

        /// <summary>
        /// Back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> Always true.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonPressedCallback);
            return true;

            async void BackButtonPressedCallback()
            {
                _ = await this.CancelChanges();
                await this.ScannerDisable();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }

        /// <summary>
        /// Fires just before the page disappears.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, nameof(EventTypes.OnKeyUp));
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static string GetControlDescription()
        {
            return "Convert Product";
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsDataLoading = true;
                var parse = scanArgs.ParseInfo;
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier(parse);
                }
                else
                {
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        var prodUnitRefno = await this.DataCollector.IdentifyProductScanned(parse);
                        var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        scannedProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(scannedProdInfo.Refno);

                        await this.SetDefaultLocationBeforeKnownActivityItem(scannedProdInfo);
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() =>
                                this.CmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocation.Text = "Set Location");
                        }

                        var result = await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                "Are you scanning the 'To' product?",
                                "Yes", "No"));
                        if (result)
                        {
                            this.ToProdInfo = scannedProdInfo;
                            await this.AssignToProductText();
                        }
                        else
                        {
                            if (!scannedProdInfo.IsAllowConvertProduct)
                            {
                                this.FromProdRefno = 0;
                                this.FromProdInfo = new ProductInfo();
                                const string msg = "Cannot convert a product without the product plag: Convert Label" +
                                    " Adjustment Allow Product Change enabled.";
                                throw new MessageOnlyException(msg);
                            }

                            this.FromProdInfo = scannedProdInfo;
                            await this.AssignFromProductText();
                        }
                    }

                    if (!string.IsNullOrEmpty(parse.ActItemFkRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here.");
                    }

                    if (!string.IsNullOrEmpty(parse.ActDetailFkRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item detail bar code is not supported here.");
                    }

                    if (!string.IsNullOrEmpty(parse.Sscc18))
                    {
                        throw new MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.

                // Raised if not bar code was scanned.
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (GatherProductDataException prodDataEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, prodDataEx);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (IsReadyToApplyException isReadyEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, isReadyEx);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.IsDataLoading = false;
                await this.ScannerEnable();
            }

            return;

            async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
            {
                // If section filtering was set in the activity header. Do we want to enforce
                // the scanned section location.
                var tmpLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier));
                this.LocMap.CurrLocMap = tmpLoc;
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.CmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocation.Text = "Set Location");
                }
            }
        }

        /// <summary>
        ///     Set the activity item location for adding new or identify line items.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo" /> Product information.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SetDefaultLocationBeforeKnownActivityItem(ProductInfo prodInfo)
        {
            // Need to get a warehouse to determine default location if turned on, we don't
            // have the line item warehouse yet. We could assume the header warehouse or even
            // the login warehouse.
            // We can't determine the default location until after identifying the line item.
            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                // This will be the default location if the line item location is not complete.
                var whsLocMapRefno = await LocationMapTasks.GetWhsLocMapRefnoFromLocMapRefno(this.LocMap.CurrLocMapRefno);
                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(whsLocMapRefno, prodInfo.Refno);
                if (this.IsAllowedDefaultLocationOverride())
                {
                    if (await LocationMapTasks.IsCompleteLocMap(this.LocMap.ChosenLocMap))
                    {
                        SetCurrentLocationWithChosen();
                    }
                    else
                    {
                        if (!await LocationMapTasks.IsCompleteLocMap(defLocMapRefno))
                        {
                            const string msg = "Default locations is enabled, and for this activity and product" +
                                                " the location is not fully complete.";
                            throw new MessageOnlyException(msg);
                        }

                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                        this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                    }
                }
                else
                {
                    if (!await LocationMapTasks.IsCompleteLocMap(defLocMapRefno))
                    {
                        const string msg = "Default locations is enabled, and for this activity and product" +
                                            " the location is not fully complete.";
                        throw new MessageOnlyException(msg);
                    }

                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                }
            }
            else
            {
                SetCurrentLocationWithChosen();
            }

            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
            {
                throw new MessageOnlyException("Location must be set first!");
            }

            return;

            // Set the current location with the chosen location.
            void SetCurrentLocationWithChosen()
            {
                // This should not be needed, current location should have been already set using the location picker.
                if (this.LocMap.CurrLocMapRefno != this.LocMap.ChosenLocMapRefno)
                {
                    // Set the current location to the chosen one, overriding and defaulting location behaviour.
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                }
            }
        }

        /// <summary>
        ///     Check if default location is allowed to be overridden.
        /// </summary>
        /// <returns><see cref="bool" /> True if the location override is allowed else false.</returns>
        private bool IsAllowedDefaultLocationOverride()
        {
            return this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
        }

        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();
            this.OptionsMenuItems.Add(SaveText);
            this.OptionsMenuItems.Add(CancelText);
        }

        private async Task ClearContents()
        {
            this.FromProdRefno = 0;
            this.ToProdRefno = 0;
            this.LocMap = null;
            this.LocMap = LocationMap.GetInstance();
            await this.ActItem.Delete();
            await this.ActDetail.Delete();

            this.CmdLocation.Text = "Set Location";
            this.CmdSelectToProduct.Text = "Select To Product";
            this.CmdSelectFromProduct.Text = "Select From Product";
            this.CmdDetail.Text = "Detail";
        }

        private async Task<string> MakeProductLabelsSubmitString(int listDetailIndex)
        {
            var detail = this.ActDetail.ListDetails;
            var prodUnitRefno = Convert.ToInt32(detail[listDetailIndex].GetType().GetProperty("ProductUnitRefno")
                ?.GetValue(detail[listDetailIndex]));
            var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);
            var bestBefore = string.Empty;
            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(detail[listDetailIndex].GetType()
                        .GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")
                        ?.GetValue(detail[listDetailIndex]))
                    .ToString("MM/dd/yyyy");
            }

            var production = string.Empty;
            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(detail[listDetailIndex].GetType()
                        .GetProperty("ActivityDetailLogisticUnitProductionDate")
                        ?.GetValue(detail[listDetailIndex]))
                    .ToString("MM/dd/yyyy");
            }

            var prodId = detail[listDetailIndex].GetType().GetProperty("ProductCode")
                ?.GetValue(detail[listDetailIndex]);
            var prodUnitBc = detail[listDetailIndex].GetType().GetProperty("ProductUnitBC")
                ?.GetValue(detail[listDetailIndex]);
            var unitCode = detail[listDetailIndex].GetType().GetProperty("UnitCode")?.GetValue(detail[listDetailIndex]);
            var serialNumber = detail[listDetailIndex].GetType()
                .GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(detail[listDetailIndex]);
            var lotNumber = detail[listDetailIndex].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")
                ?.GetValue(detail[listDetailIndex]);
            var variantCode = detail[listDetailIndex].GetType().GetProperty("ProductSchemeVariantCode")
                ?.GetValue(detail[listDetailIndex]);
            var variantNumber = detail[listDetailIndex].GetType().GetProperty("ProductSchemeVariantNumber")
                ?.GetValue(detail[listDetailIndex]);
            var detailQuantity = detail[listDetailIndex].GetType().GetProperty("ActivityDetailQuantity")
                ?.GetValue(detail[listDetailIndex]);
            var weightQuantity = detail[listDetailIndex].GetType().GetProperty("ActivityDetailWeightQuantity")
                ?.GetValue(detail[listDetailIndex]);
            var baseQuantity = detail[listDetailIndex].GetType().GetProperty("ActivityDetailBaseQuantity")
                ?.GetValue(detail[listDetailIndex]);

            var req = new StringBuilder();
            _ = req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            _ = req.Append("<imclient>");
            _ = req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            _ = req.Append(await SysCommon.GetRequestDefaults(false));
            _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFkRefno);
            _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFkRefno);
            _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFkRefno);
            _ = req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.HdrFkRefno);
            _ = req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFkRefno);
            _ = req.AppendFormat("<prod_id>{0}</prod_id>", prodId);
            _ = req.AppendFormat("<bc>{0}</bc>", prodUnitBc);
            _ = req.AppendFormat("<alt_unit>{0}</alt_unit>", unitCode);
            _ = req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            _ = req.AppendFormat("<serial>{0}</serial>", serialNumber);
            _ = req.AppendFormat("<pdate>{0}</pdate>", production);
            _ = req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            _ = req.AppendFormat("<lot_no>{0}</lot_no>", lotNumber);
            _ = req.AppendFormat("<variant_code>{0}</variant_code>", variantCode);
            _ = req.AppendFormat("<variant_no>{0}</variant_no>", variantNumber);
            _ = req.AppendFormat("<alt_qty>{0}</alt_qty>", detailQuantity);
            _ = req.AppendFormat("<wt_qty>{0}</wt_qty>", weightQuantity);
            _ = req.AppendFormat("<base_qty>{0}</base_qty>", baseQuantity);
            _ = req.Append("</imclient>");

            return req.ToString();
        }

        private async void CmdLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.CmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocation.Text = "Set Location");
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.DisplaySummary();
            }
        }

        private async Task AssertLocationSet()
        {
            if (!await IsLocationSet())
            {
                throw new MessageOnlyException("Location must be set.");
            }

            return;

            async Task<bool> IsLocationSet()
            {
                return await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno);
            }
        }

        private void AssertFromProductSet()
        {
            if (!this.IsFromProductSet())
            {
                throw new MessageOnlyException("Please set the from product first.");
            }
        }

        private bool IsFromProductSet()
        {
            return this.FromProdRefno != 0;
        }

        private void AssertToProductSet()
        {
            if (!this.IsToProductSet())
            {
                throw new MessageOnlyException("No convert to product has be set.");
            }
        }

        private bool IsToProductSet()
        {
            return this.ToProdRefno != 0;
        }

        private void AssertDetailsSet()
        {
            if (this.ActDetail.ListDetails.Count == 0)
            {
                throw new MessageOnlyException("No convert to product details have be set.");
            }
        }

        private async Task<int> SelectProduct(bool filterAllowChangeProduct)
        {
            var prodRefno = 0;

            try
            {
                var frmSelectProduct = new FormDialogContainer(
                    EDialogTypes.SelectProduct,
                    SysCommon.ActSubTypes.ConvertProduct);

                if (filterAllowChangeProduct)
                {
                    ((SelectProduct)frmSelectProduct.DisplayControl).FilterOnAllowConvertProduct = true;
                }

                if (await frmSelectProduct.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDocProduct = await frmSelectProduct.GetXml();
                    if (xmlDocProduct.Root == null)
                    {
                        throw new NullReferenceException("xmlDocProduct.Root == null");
                    }

                    prodRefno = Convert.ToInt32(xmlDocProduct.Root.Element("ProductRefno") !.Value);
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return prodRefno;
        }

        private async void CmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                await this.AssertLocationSet();

                this.FromProdRefno = await this.SelectProduct(true);
                if (this.FromProdRefno == 0)
                {
                    this.FromProdInfo = new ProductInfo();
                    this.CmdSelectFromProduct.Text = "Select From Product";
                }
                else
                {
                    var prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);
                    prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.FromProdRefno);
                    this.FromProdInfo = prodInfo;

                    if (!this.FromProdInfo.IsAllowConvertProduct)
                    {
                        this.FromProdRefno = 0;
                        this.FromProdInfo = new ProductInfo();

                        throw new MessageOnlyException(
                            "Cannot convert a product without the Product Flag: Convert Label Adjustment Allow Product Change.");
                    }

                    await this.AssignFromProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.DisplaySummary();
            }
        }

        private async Task AssignFromProductText()
        {
            const string query = "SELECT tProduct.ProductCode,"
                                 + "tProduct.ProductDescription,"
                                 + "tProductUnit.ProductUnitBC,"
                                 + "tUnit.UnitCode "
                                 + "FROM tProductUnit "
                                 + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                 + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                 + "WHERE tProductUnit.ProductUnitRefno = ?";

            var results = await DatabaseConnection.GetInstance()
                .SqlConn.QueryAsync<ProductUnit>(query, this.FromProdInfo.UnitRefno);

            if (results?.Count > 0)
            {
                _ = await Device.InvokeOnMainThreadAsync(() =>
                    this.CmdSelectFromProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " +
                                                     Convert.ToString(results[0].ProductDescription).TrimEnd());
            }
        }

        private async void CmdSelectToUnit_Click(object sender, EventArgs e)
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl causes a click event to fire if it is the next control
                await this.ScannerDisable();

                await this.AssertLocationSet();

                this.ToProdRefno = await this.SelectProduct(false);

                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                    this.ActItem.ProdInfo.Refno);

                if (this.ToProdRefno == 0)
                {
                    this.ToProdInfo = new ProductInfo();
                    this.CmdSelectToProduct.Text = "Select To Product";
                }
                else
                {
                    var prodInfo = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    this.ToProdInfo = prodInfo;
                    await this.AssignToProductText();
                }
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            this.DisplaySummary();
        }

        private async Task AssignToProductText()
        {
            const string query = "SELECT tProduct.ProductCode,"
                                 + "tProduct.ProductDescription,"
                                 + "tProductUnit.ProductUnitBC,"
                                 + "tUnit.UnitCode "
                                 + "FROM tProductUnit "
                                 + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                 + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                 + "WHERE tProductUnit.ProductUnitRefno = ?";

            var results = await DatabaseConnection.GetInstance().SqlConn
                .QueryAsync<ProductUnit>(query, this.ToProdInfo.UnitRefno);

            if (results?.Count > 0)
            {
                _ = await Device.InvokeOnMainThreadAsync(() =>
                    this.CmdSelectToProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " +
                                                   Convert.ToString(results[0].ProductDescription).TrimEnd());
            }
        }

        private void DisplaySummary()
        {
            // DataRowCollection rows = this.ActDetail.DataViewDetails.Table.Rows;
            // SFillQuantity fillQty = GetActivityItemQuantity();
            this.ExtraInfoItems.Clear();

            if (this.FromProdInfo != null)
            {
                this.ExtraInfoItems.Add(
                    $"From {this.FromProdInfo.BaseUnitCode} {this.FromProdInfo.UnitCode} {this.FromProdInfo.UnitSizeDesc}");
            }

            if (this.ToProdInfo != null)
            {
                this.ExtraInfoItems.Add(
                    $"To {this.ToProdInfo.BaseUnitCode} {this.ToProdInfo.UnitCode} {this.ToProdInfo.UnitSizeDesc}");
            }
        }

        private SFillQuantity GetActivityItemQuantity()
        {
            var fillQty = new SFillQuantity
            {
                Quantity = 0,
                BaseQuantity = 0,
                WeightQuantity = 0
            };
            var detailList = this.ActDetail.ListDetails;

            if (detailList != null)
            {
                var count = detailList.Count;

                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var detailQuantity = detailList[i].GetType().GetProperty("ActivityDetailQuantity")
                            ?.GetValue(detailList[i]);
                        var baseQuantity = detailList[i].GetType().GetProperty("ActivityDetailBaseQuantity")
                            ?.GetValue(detailList[i]);
                        var weightQuantity = detailList[i].GetType().GetProperty("ActivityDetailWeightQuantity")
                            ?.GetValue(detailList[i]);

                        fillQty.Quantity += Convert.ToDecimal(detailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(baseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(weightQuantity);
                    }
                }
            }

            return fillQty;
        }

        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsDataLoading)
            {
                this.IsChangesMade = true;

                if (sender == this.BarCodeModifier1Picker)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
                }
            }
        }

        private async void CmdDetail_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                await ValidateCanPerformClick();
                await FillActivityItem();
                await CreateAndFillActivityDetail();

                var form = new FormDialogContainer(
                    EDialogTypes.ActivityConvertProductDetail,
                    this.ActDetail,
                    SysCommon.ActSubTypes.ConvertProduct);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    // Detail was already submitted to the comm server or saved in the local tables.
                }

                // Detail was already submitted to the comm server or saved in the local tables.
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.DisplaySummary();
            }

            return;

            // Validate whether allowed to perform the click detail event.
            async Task ValidateCanPerformClick()
            {
                await this.AssertLocationSet();
                this.AssertFromProductSet();
                this.AssertToProductSet();
                AssertToDoesNotEqualFrom();
            }

            // Check if the 'to' product and 'from' product are NOT the same.
            void AssertToDoesNotEqualFrom()
            {
                if (this.ToProdRefno == this.FromProdRefno)
                {
                    throw new MessageOnlyException("TO Product cannot be the same as the FROM Product.");
                }
            }

            // Fill the activity item.
            async Task FillActivityItem()
            {
                await this.ActItem.Delete();
                this.ActItem.LocMap = this.LocMap.CurrLocMap;
                this.ActItem.ProdInfo = this.ToProdInfo;
                await this.ActItem.Insert(await this.GetXml());
            }

            // Create new detail instance and fill it.
            Task CreateAndFillActivityDetail()
            {
                this.ActDetail.ClearProperties();
                this.ActDetail.ActItem = this.ActItem;
                return Task.CompletedTask;
            }
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            var result = await this.DisplayActionSheet(
                "Convert Product Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            switch (result)
            {
                case SaveText:
                    {
                        if (await this.SaveChanges())
                        {
                            await this.ScannerDisable();
                            await this.Close();
                        }

                        break;
                    }

                case CancelText:
                    {
                        _ = await this.CancelChanges();
                        await this.ScannerDisable();
                        await this.Close();
                        break;
                    }
            }
        }
    }
}
