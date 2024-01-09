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
    using Helpers;
    using Resources.Enums;
    using Resources.Types;
    using SQLite;
    using Views;
    using Xamarin.Forms;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Activity convert product.
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
        ///     Initializes a new instance of the <see cref="ActivityConvertProduct" /> class.
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
            this.ScannerEnable().FireAndForgetSafeAsync();
            return;

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.Items.Clear();

                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.BarCodeModifier1Picker.Items.Add(string.Empty);
                    var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                    foreach (var item in modifiers)
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
        }

        /// <summary>
        ///     Gets the bar code scanner engine instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.ConvertProduct;

        /// <summary>
        ///     Gets or sets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; set; }

        /// <summary>
        ///     Gets or sets the activity item.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        ///     Gets or sets the activity.
        /// </summary>
        private Activity Activity { get; set; }

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        ///     Gets or sets the system environment settings.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the data is loaded.
        /// </summary>
        private bool IsDataLoading { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether changes to data has been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets the convert from product reference number.
        /// </summary>
        private int FromProdRefno
        {
            get => this.fromProdRefno;

            set
            {
                this.fromProdRefno = value;
                ((ConvertProductActItem)this.ActItem).FromProdRefno = value;
            }
        }

        /// <summary>
        ///     Gets or sets the convert to product reference number.
        /// </summary>
        private int ToProdRefno
        {
            get => this.toProdRefno;

            set
            {
                this.toProdRefno = value;
                ((ConvertProductActItem)this.ActItem).ToProdRefno = value;
            }
        }

        /// <summary>
        ///     Gets or sets the convert from product information.
        /// </summary>
        private ProductInfo FromProdInfo
        {
            get => this.fromProductInfo;
            set
            {
                this.fromProductInfo = value;
                this.FromProdRefno = value.Refno;
            }
        }

        /// <summary>
        ///     Gets or sets the convert to product information.
        /// </summary>
        private ProductInfo ToProdInfo
        {
            get => this.toProductInfo;
            set
            {
                this.toProductInfo = value;
                this.ToProdRefno = value.Refno;
            }
        }

        /// <summary>
        ///     Gets the extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets the options menu items.
        /// </summary>
        private List<string> OptionsMenuItems { get; } = new List<string>();

        /// <summary>
        ///     Un-register the bar code scan event handler and disable the scanner.
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
            await this.Scanner.EnableScanning();
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                {
                    this.Scanner.BarCodeScan += this.BarCodeHandler;
                }
            }
        }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        /// <remarks>
        ///     Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        ///     Must have a try catch block.
        /// </remarks>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                await this.AssertLocationSet();
                this.AssertFromProductSet();

                this.AssertToProductSet();
                this.AssertDetailsSet();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
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
        ///     Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

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
                    //// this.IsChangesMade = false;
                }
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
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<from_prod_fk_refno>" +
                (this.FromProdInfo != null ? this.FromProdInfo.Refno.ToString() : string.Empty) +
                "</from_prod_fk_refno>"
                + "<to_prod_fk_refno>" + (this.ToProdInfo != null ? this.ToProdInfo.Refno.ToString() : string.Empty) +
                "</to_prod_fk_refno>"
                + "<location_map_refno>" + this.LocMap.CurrLocMapRefno + "</location_map_refno>"
                + "</params>");

            return xmlDoc;
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();
            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandlerCallback);
            }

            //////////////////////////////////////////////////////////////////////////
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
        ///     Back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> Always true.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonPressedCallback);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonPressedCallback()
            {
                _ = await this.CancelChanges();
                await this.ScannerDisable();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Get the control form description.
        /// </summary>
        /// <returns><see cref="string" /> Control form description.</returns>
        private static string GetControlDescription()
        {
            return "Convert Product";
        }

        /// <summary>
        ///     Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="scanArgs">
        ///     <see cref="BarCodeScannerEventArgs" /> Sender bar code scanner
        ///     event arguments.
        /// </param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsDataLoading)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            try
            {
                this.IsDataLoading = true;
                var parse = scanArgs.ParseInfo;
                await this.ScannerDisable();
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier(parse);
                }
                else
                {
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        throw new MessageOnlyException("Scanning a product identifier bar code is not supported here.");
                    }

                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here.");
                    }

                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item detail bar code is not supported here.");
                    }

                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        throw new MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.
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

            //////////////////////////////////////////////////////////////////////////
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
        ///     Populates a string array to be used in DisplayActionSheet.
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();
            this.OptionsMenuItems.Add(SaveText);
            this.OptionsMenuItems.Add(CancelText);
        }

        /// <summary>
        ///     Clear the move from and to settings.
        /// </summary>
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

        /// <summary>
        ///     Make the product label submit XML string.
        /// </summary>
        /// <param name="listDetailIndex"><see cref="int" /> List detail index.</param>
        /// <returns><see cref="string" /> Created product label request string.</returns>
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
            _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
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

        /// <summary>
        ///     Set location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
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
                await this.ScannerDisable();
                this.DisplaySummary();
            }
        }

        /// <summary>
        ///     Throws an exception if Location is not set.
        /// </summary>
        /// <exception cref="MessageOnlyException"> Raised if location is not set.</exception>
        private async Task AssertLocationSet()
        {
            if (!await IsLocationSet())
            {
                throw new MessageOnlyException("Location must be set.");
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsLocationSet()
            {
                return await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno);
            }
        }

        /// <summary>
        ///     Throws an exception if FROM Product is not set.
        /// </summary>
        /// <exception cref="MessageOnlyException"> Raised if the product unit is not set.</exception>
        private void AssertFromProductSet()
        {
            if (!this.IsFromProductSet())
            {
                throw new MessageOnlyException("Please set the from product first.");
            }
        }

        /// <summary>
        ///     Checks if the FROM product is selected.
        /// </summary>
        /// <returns><see cref="bool" /> True if the from product has been selected, else false.</returns>
        private bool IsFromProductSet()
        {
            return this.FromProdRefno != 0;
        }

        /// <summary>
        ///     Checks if the convert TO product item is set.
        ///     Throws if not set.
        /// </summary>
        /// <exception cref="MessageOnlyException"> Raised if convert to product is not set.</exception>
        private void AssertToProductSet()
        {
            if (!this.IsToProductSet())
            {
                throw new MessageOnlyException("No convert to product has be set.");
            }
        }

        /// <summary>
        ///     Checks if the TO product is selected.
        /// </summary>
        /// <returns><see cref="bool" /> True if the convert product has been set.</returns>
        private bool IsToProductSet()
        {
            return this.ToProdRefno != 0;
        }

        /// <summary>
        ///     Checks if the convert TO product item details is set.
        ///     Throws if not set.
        /// </summary>
        /// <exception cref="MessageOnlyException"> Raised if convert to product details exist.</exception>
        private void AssertDetailsSet()
        {
            if (this.ActDetail.ListDetails.Count <= 0)
            {
                throw new MessageOnlyException("No convert to product details have be set.");
            }
        }

        /// <summary>
        ///     Selects a Product and returns the ProdRefno if found.
        /// </summary>
        /// <param name="filterAllowChangeProduct">True to filter the list to only products with AllowChangeProduct.</param>
        /// <returns>ProdRefno if found; otherwise 0.</returns>
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
                    var xmlDocProduct = await frmSelectProduct.GetXML();
                    if (xmlDocProduct.Root == null)
                    {
                        throw new NullReferenceException("xmlDocProduct.Root == null");
                    }

                    prodRefno = Convert.ToInt32(xmlDocProduct.Root.Element("ProductRefno")!.Value);
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

        /// <summary>
        ///     Select from product unit click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
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

                    await AssignFromProductText();
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

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignFromProductText()
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
                    .SQLConn.QueryAsync<ProductUnit>(query, this.FromProdInfo.UnitRefno);

                if (results?.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.CmdSelectFromProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " +
                                                         Convert.ToString(results[0].ProductDescription).TrimEnd());
                }
            }
        }

        /// <summary>
        ///     Select To product unit click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
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
                    await AssignToProductText();
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
            return;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignToProductText()
            {
                const string query = "SELECT tProduct.ProductCode,"
                                     + "tProduct.ProductDescription,"
                                     + "tProductUnit.ProductUnitBC,"
                                     + "tUnit.UnitCode "
                                     + "FROM tProductUnit "
                                     + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                     + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                     + "WHERE tProductUnit.ProductUnitRefno = ?";

                var results = await DatabaseConnection.GetInstance().SQLConn
                    .QueryAsync<ProductUnit>(query, this.ToProdInfo.UnitRefno);

                if (results?.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.CmdSelectToProduct.Text = Convert.ToString(results[0].ProductCode).TrimEnd() + " " +
                                                       Convert.ToString(results[0].ProductDescription).TrimEnd());
                }
            }
        }

        /// <summary>
        ///     Displays the listbox summary data.
        /// </summary>
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

        /// <summary>
        ///     Get the activity item quantity.
        /// </summary>
        /// <returns>
        ///     <see cref="SFillQuantity" />
        /// </returns>
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

        /// <summary>
        ///     Identify changes made.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
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

        /// <summary>
        ///     Activity detail click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Events arguments.</param>
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

            //////////////////////////////////////////////////////////////////////////
            // Validate whether allowed to perform the click detail event.
            async Task ValidateCanPerformClick()
            {
                await this.AssertLocationSet();
                this.AssertFromProductSet();
                this.AssertToProductSet();
                AssertToDoesNotEqualFrom();
            }

            //////////////////////////////////////////////////////////////////////////
            // Check if the 'to' product and 'from' product are NOT the same.
            void AssertToDoesNotEqualFrom()
            {
                if (this.ToProdRefno == this.FromProdRefno)
                {
                    throw new MessageOnlyException("TO Product cannot be the same as the FROM Product.");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            // Fill the activity item.
            async Task FillActivityItem()
            {
                await this.ActItem.Delete();
                this.ActItem.LocMap = this.LocMap.CurrLocMap;
                this.ActItem.ProdInfo = this.ToProdInfo;
                await this.ActItem.Insert(await this.GetXml());
            }

            //////////////////////////////////////////////////////////////////////////
            // Create new detail instance and fill it.
            async Task CreateAndFillActivityDetail()
            {
                this.ActDetail.ClearProperties();
                this.ActDetail.ActItem = this.ActItem;
            }
        }

        /// <summary>
        ///     Bar code modifier picker selected index changed handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
        }

        /// <summary>
        ///     Options menu clicked event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
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