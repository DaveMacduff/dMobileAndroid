// <copyright file="ActivityPurchaseOrderWorkSheet.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Helpers;
    using Forms;
    using GroundWork;
    using Resources.Enums;
    using Resources.Types;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;
    using System.Text;
    using Resources.Types.Items;

    /// <summary>
    /// Activity purchase order work sheet.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityPurchaseOrderWorkSheet : IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityPurchaseOrderWorkSheet" /> class.
        /// </summary>
        /// <param name="actSubType"><see cref="ActSubType"/> Activity subtype.</param>
        public ActivityPurchaseOrderWorkSheet(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.cmdAdd.Text = "Add";
            this.IsIndicatorRunning = false;
            this.ExtraInfoItems = new ObservableCollection<string>();
            this.MainActivityItems = new ObservableCollection<object>();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, this.ActSubType);
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
            this.Title = this.GetControlDescription();

            ParseAndSeedBarCodeModifier();
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.LoadAndBindData().FireAndForgetSafeAsync();
            return;

            //////////////////////////////////////////////////////////////////////////
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

        /// <summary>
        ///    Gets or sets the bar code parse information.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        ///    Gets the bar code scanner engine.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets the activity detail instance.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets the location map instance.
        /// </summary>
        private LocationMap LocMap { get; }

        /// <summary>
        ///     Gets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Gets the activity header instance.
        /// </summary>
        private Activity Activity { get; }

        /// <summary>
        ///     Gets the activity item instance.
        /// </summary>
        private ActItem ActItem { get; }

        /// <summary>
        ///     Gets or sets the selected activity item foreign key reference number.
        /// </summary>
        private int SelectedActivityItemFkRefno { get; set; }

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets or sets the extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        ///     Gets or sets the main activity items collection.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
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
                this.Scanner.BarCodeScan += this.BarCodeHandler;
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            return await Task.Run(() => true);
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initialize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }

            return true;
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            return await Task.Run(() => true);
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
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.PurchaseOrderWorkSheet, ex);
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
            var xmldocParams = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                               "<params>" + "</params>");
            return xmldocParams;
        }

        /// <summary>
        ///     Get control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
        }

        /// <summary>
        ///     Command add click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated this event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event argument data.</param>
        public async void cmdAdd_Click(object sender, EventArgs e)
        {
            var dialogResult = DialogResult.None;
            var strProductUnitBarCodeCode = string.Empty;
            var strBarCodeSymbologyPrefix = string.Empty;

            try
            {
                await this.ScannerDisable();

                var productUnitRefno = 0;
                string productCode;
                bool productUnitIsBc;
                string unitCode;
                var formSelectProdUnit = new FormDialogContainer(EDialogTypes.SelectProductUnit,
                    SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                _ = await Device.InvokeOnMainThreadAsync(async () =>
                    dialogResult = await formSelectProdUnit.DisplayControl.ShowDialog());
                if (dialogResult == DialogResult.OK)
                {
                    var xmlDocProduct = await formSelectProdUnit.GetXml();
                    productUnitRefno = int.Parse(xmlDocProduct.Root!.Element("ProductUnitRefno") !.Value);
                    productCode = xmlDocProduct.Root!.Element("ProductCode") !.Value;
                    productUnitIsBc = bool.Parse(xmlDocProduct.Root!.Element("ProductUnitIsBC") !.Value);
                    unitCode = xmlDocProduct.Root!.Element("UnitCode") !.Value;
                    this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(productUnitRefno);
                    this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                        this.ActItem.ProdInfo.Refno);
                }
                else
                {
                    throw new TaskCanceledException();
                }

                var iActivityItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                    productUnitRefno,
                    LocationMap.GetInstance().CurrLocMap);
                if (iActivityItemFoundRefno > 0)
                {
                    var formPurchaseOrderWorkSheetEdit = new FormDialogContainer(
                        EDialogTypes.ActivityPurchaseOrderWorkSheetEdit,
                        this.ActDetail,
                        SysCommon.ActSubTypes.NotApplicable);

                    dialogResult = await formPurchaseOrderWorkSheetEdit.DisplayControl.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                    {
                        var xmlDocEdit = await formPurchaseOrderWorkSheetEdit.GetXml();
                        var altQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("alt_qty") !.Value), 3);
                        var baseQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("base_qty") !.Value), 3);
                        var wtQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("wt_qty") !.Value), 3);

                        var request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<ProductUnitRefno>" + productUnitRefno + "</ProductUnitRefno>" +
                            "<ProductCode>" + productCode + "</ProductCode>" +
                            "<ProductUnitIsBC>" + productUnitIsBc + "</ProductUnitIsBC>" +
                            "<UnitCode>" + unitCode + "</UnitCode>" +
                            "<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>" +
                            "<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix +
                            "</ProductUnitBarCodeSymbologyPrefix>" +
                            "<alt_qty>" + altQty + "</alt_qty>" +
                            "<base_qty>" + baseQty + "</base_qty>" +
                            "<wt_qty>" + wtQty + "</wt_qty>" +
                            "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" +
                            "<refno>" + this.ActItem.ItemFkRefno + "</refno>" +
                            "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                            "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" +
                            "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" +
                            "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" +
                            "</params>");
                        await this.ActItem.Update(request);
                    }
                }
                else
                {
                    this.ActItem.ItemFkRefno = 0;

                    // Reset values
                    this.ActDetail.Quantity = 0;
                    this.ActDetail.BaseQuantity = 0;
                    this.ActDetail.WeightQuantity = 0;
                    this.ActDetail.SchemeId = string.Empty;
                    this.ActDetail.UniqueSchemeId = string.Empty;
                    var formPurchaseOrderWorkSheetEdit = new FormDialogContainer(
                        EDialogTypes.ActivityPurchaseOrderWorkSheetEdit,
                        this.ActDetail,
                        SysCommon.ActSubTypes.NotApplicable);
                    dialogResult = await formPurchaseOrderWorkSheetEdit.DisplayControl.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        var xmlDocEdit = await formPurchaseOrderWorkSheetEdit.GetXml();
                        var altQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("alt_qty")!.Value), 3);
                        var baseQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("base_qty")!.Value), 3);
                        var wtQty = Math.Round(decimal.Parse(xmlDocEdit.Root!.Element("wt_qty")!.Value), 3);

                        var request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" +
                            "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" +
                            "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBc + "</ProductUnitIsBC>" +
                            "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" +
                            "<ProductUnitBarCodeCode>" + strProductUnitBarCodeCode + "</ProductUnitBarCodeCode>" +
                            "<ProductUnitBarCodeSymbologyPrefix>" + strBarCodeSymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" +
                            "<alt_qty>" + altQty + "</alt_qty>" +
                            "<base_qty>" + baseQty + "</base_qty>" +
                            "<wt_qty>" + wtQty + "</wt_qty>" +
                            "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" +
                            "<refno>" + this.ActItem.ItemFkRefno + "</refno>" +
                            "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                            "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" +
                            "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" +
                            "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" +
                            "</params>");
                        await this.ActItem.Insert(request);

                        if (this.ActItem.ListItems.Count > 0)
                        {
                            var purchaseOrderWorkSheetItem = this.ActItem.ListItems.OfType<PurchaseOrderWorkSheetItem>()
                                .FirstOrDefault(item => item.ActivityItemFKRefno == this.ActItem.ItemFkRefno);
                            if (purchaseOrderWorkSheetItem == null)
                            {
                                throw new Exception("Unable to identify the work sheet items");
                            }

                            this.ListViewMainActivity.SelectedItem = purchaseOrderWorkSheetItem;
                        }
                    }
                }

                await this.LoadAndBindData();
            }
            catch (TaskCanceledException)
            {
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
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.SubscribeKeyUpHandlers();
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);

            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            }
        }

        /// <summary>
        ///     Subscribe the key up handlers.
        /// </summary>
        private void SubscribeKeyUpHandlers()
        {
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), this.KeyUpHandlerCallback);
        }

        /// <summary>
        ///     Key up handler call back.
        /// </summary>
        /// <param name="keyPressed"><see cref="string" /> Key pressed value.</param>
        private void KeyUpHandlerCallback(string keyPressed)
        {
            _ = Enum.TryParse(keyPressed, out Keycode keycode);

            if (!this.IsIndicatorRunning)
            {
                if (keycode == Keycode.DpadDown)
                {
                    AppearingDpadDown();
                }
                else if (keycode == Keycode.DpadUp)
                {
                    AppearingDpadUp();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadDown()
            {
                if (this.ListViewMainActivity.SelectedItem != null && Convert.ToInt32(this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem)) < this.MainActivityItems.Count - 1)
                {
                    this.ListViewMainActivity.SelectedItem =
                        this.MainActivityItems[
                            this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) + 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            void AppearingDpadUp()
            {
                if (this.ListViewMainActivity.SelectedItem != null &&
                    Convert.ToInt32(this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem)) > 0)
                {
                    this.ListViewMainActivity.SelectedItem =
                        this.MainActivityItems[
                            this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem) - 1];
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        true);
                }
            }
        }

        /// <summary>
        ///     Load and Bind the data.
        /// </summary>
        /// <returns><see cref="Task"/> Task.</returns>
        private async Task LoadAndBindData()
        {
            try
            {
                this.IsIndicatorRunning = true;
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                await this.Activity.Refresh(null);
                await this.ActItem.Refresh(null);
                this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ExtraInfoItems.Clear();
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

                    this.MainActivityItems = this.ActItem.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                    this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
                    await this.FindSelectedItemAsync();
                });
            }
            catch (CommunicationException commEx)
            {
                // Question should a communication error, be reported as an error or just a messagebox?
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load worksheet.{Environment.NewLine}" +
                              $"Because of a communication problem.{Environment.NewLine}" +
                              $"Detail:{commEx.Message}{Environment.NewLine}" +
                              "Please exit and try again.";

                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK);
                });
            }
            catch (SQLiteException ex)
            {
                for (var curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert("Error ", ex.Data[curExNdx].ToString() + '\r', "OK"));
                }
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
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// Find the selected item.
        /// </summary>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActItem.ListItems.Count > 0 && this.MainActivityItems.Count > 0)
            {
                this.ActItem.SelectedListIdx = 0;
                object selectedItem = null;
                foreach (var item in this.ActItem.ListItems)
                {
                    if (this.SelectedActivityItemFkRefno == this.ActItem.GetFkRefno(item))
                    {
                        selectedItem = item;
                        break;
                    }

                    this.ActItem.SelectedListIdx++;
                }

                // Select the first header if no match was found, else select the item.
                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActItem.SelectedListIdx = 0;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems[this.ActItem.SelectedListIdx];
                    });
                    await this.ActItem.AssignActivityItem(this.ListViewMainActivity.SelectedItem);
                }
                else
                {
                    await this.ActItem.AssignActivityItem(selectedItem);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedItem;
                    });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActItem.SelectedListIdx = 0;
                    if (this.ActItem.SelectedListIdx <= this.MainActivityItems.Count - 1)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ExtraInfoItems.Clear();
                            this.ListViewMainActivity.SelectedItem =
                                this.MainActivityItems[this.ActItem.SelectedListIdx];
                        });
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = selectedItem; });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.ScrollTo(
                        this.ListViewMainActivity.SelectedItem,
                        ScrollToPosition.MakeVisible,
                        false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActItem.SelectedListIdx = -1;
                this.ActItem.SelectedItem = null;
                this.ActItem.ItemFkRefno = 0;
                this.ActItem.ItemRefno = 0;
                this.SelectedActivityItemFkRefno = 0;
                Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = null; });
            }
        }

        /// <summary>
        ///     Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Sender bar code scanner event arguments.</param>
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

                var actItemFoundRefno = 0;
                this.BarParse = scanArgs.ParseInfo;
                var isLocationBarCodeScanned = !string.IsNullOrEmpty(this.BarParse.LocationIdentifier);
                var isProductScanned = !string.IsNullOrEmpty(this.BarParse.ProductIdentifier);
                var isFkRefnoScanned = !string.IsNullOrEmpty(this.BarParse.ActFkRefno);
                var isFkItemRefnoScanned = !string.IsNullOrEmpty(this.BarParse.ActItemFkRefno);
                var isFkDetailRefnoScanned = !string.IsNullOrEmpty(this.BarParse.ActDetailFkRefno);
                var isSkidScanned = !string.IsNullOrEmpty(this.BarParse.Sscc18);

                if (isFkRefnoScanned)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                }
                else if (isFkItemRefnoScanned)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning an Activity Item Reference # Is Not Supported Here", "OK"));
                }
                else if (isFkDetailRefnoScanned)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning an Activity Detail Reference # Is Not Supported Here", "OK"));
                }
                else if (isSkidScanned)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning a shipping container bar code is not supported here!", "OK"));
                }
                else if (isLocationBarCodeScanned)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "Scanning a location identifier bar code is not supported here", "OK"));
                }
                else if (isProductScanned)
                {
                    var prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarParse);
                    var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    if (scannedProdInfo.Refno == 0)
                    {
                        throw new MessageOnlyException("Scanned product could not be identified.");
                    }

                    var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    await this.SetDefaultLocationBeforeKnownActivityItem(scannedProdInfo);

                    actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                        prodUnitRefno,
                        LocationMap.GetInstance().CurrLocMap);

                    if (actItemFoundRefno == 0 && prodUnitRefno != 0)
                    {
                        this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ActItem.ProdInfo.Scheme =
                            await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                        // <issue="13609" author="RD" date="2016/02/22"/>
                        SysShared.AssertSuppressedErrorsAgainstScheme(this.ActItem.ProdInfo, this.BarParse);

                        if (this.ActItem.IsAllowAddItem)
                        {
                            this.Activity.HdrFkRefno = 0;
                            this.ActItem.ItemFkRefno = 0;

                            var request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                "<params>" +
                                "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" +
                                "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" +
                                "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBc + "</ProductUnitIsBC>" +
                                "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" +
                                "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" +
                                "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" +
                                "<alt_qty>" + 0 + "</alt_qty>" +
                                "<base_qty>" + 0 + "</base_qty>" +
                                "<wt_qty>" + 0 + "</wt_qty>" +
                                "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" +
                                "<refno>" + this.Activity.HdrFkRefno + "</refno>" +
                                "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                                "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" +
                                "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" +
                                "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" +
                                "</params>");
                            await this.ActItem.Insert(request);
                            actItemFoundRefno = this.ActItem.ItemRefno;
                        }
                        else
                        {
                            var message = string.Format(
                                "Item Not Found For " + Environment.NewLine + "Location {0} " +
                                Environment.NewLine + "Product {1} BC {2} Unit {3}",
                                LocationMapTasks.GetCurrentReadableLocation(), this.ActItem.ProdInfo.Code,
                                this.ActItem.ProdInfo.UnitBc, this.ActItem.ProdInfo.UnitCode);
                            throw new Exception(message);
                        }
                    }

                    if (actItemFoundRefno == 0)
                    {
                        throw new Exception("Item Not Found.");
                    }

                    this.SelectedActivityItemFkRefno = this.ActItem.ItemFkRefno;
                    var foundSheetItem = this.ActItem.ListItems
                        .OfType<PurchaseOrderWorkSheetItem>().FirstOrDefault(
                            item => item.ActivityItemRefno == actItemFoundRefno);
                    if (foundSheetItem == null)
                    {
                        throw new Exception("Unable To Identify Item");
                    }

                    await this.ActItem.AssignActivityItem(foundSheetItem);
                    var dialogResult = DialogResult.None;
                    XDocument xmlDocEdit = null;
                    if (actItemFoundRefno > 0)
                    {
                        // Ask for new quantity
                        var form = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit,
                            this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                        dialogResult =
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog());

                        if (dialogResult == DialogResult.OK)
                        {
                            xmlDocEdit = await form.GetXml();

                            // TODO: Fix sorting
                            // string strOldSort = ActItem.DataViewItems.Sort;
                            // ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            var purchaseOrderWorkSheetItem2 = this.ActItem.ListItems
                                .OfType<PurchaseOrderWorkSheetItem>().FirstOrDefault(
                                    item => item.ActivityItemRefno == actItemFoundRefno);

                            if (purchaseOrderWorkSheetItem2 == null)
                            {
                                throw new Exception("Unable to identify item.");
                            }

                            ////var decQty =
                            ////    Math.Round(
                            ////        decimal.Parse(xmlDocEdit.Root.Element("alt_qty").Value) +
                            ////        Convert.ToDecimal(purchaseOrderWorkSheetItem2
                            ////            .ActivityItemPurchaseOrderWorkSheetQuantityOrdered), 3);
                            var decQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("alt_qty").Value), 3);
                            var decBaseQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("base_qty").Value), 3);
                            var decWeightQty = Math.Round(decimal.Parse(xmlDocEdit.Root.Element("wt_qty").Value), 3);

                            var request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                "<params>" +
                                "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" +
                                "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" +
                                "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBc + "</ProductUnitIsBC>" +
                                "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" +
                                "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" +
                                "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" +
                                "<alt_qty>" + decQty + "</alt_qty>" +
                                "<base_qty>" + decBaseQty + "</base_qty>" +
                                "<wt_qty>" + decWeightQty + "</wt_qty>" +
                                "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" +
                                "<refno>" + this.ActItem.ItemFkRefno + "</refno>" +
                                "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                                "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" +
                                "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" +
                                "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" +
                                "</params>");
                            await this.ActItem.Update(request);
                        }
                    }
                    else
                    {
                        this.ActItem.ItemFkRefno = 0;

                        // Reset values
                        this.ActDetail.Quantity = 0;
                        this.ActDetail.BaseQuantity = 0;
                        this.ActDetail.WeightQuantity = 0;
                        this.ActDetail.SchemeId = string.Empty;
                        this.ActDetail.UniqueSchemeId = string.Empty;

                        var form = new FormDialogContainer(EDialogTypes.ActivityPurchaseOrderWorkSheetEdit,
                            this.ActDetail, SysCommon.ActSubTypes.NotApplicable);

                        dialogResult =
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog());

                        if (dialogResult == DialogResult.OK)
                        {
                            xmlDocEdit = await form.GetXml();

                            var request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                "<params>" +
                                "<ProductUnitRefno>" + this.ActItem.ProdInfo.UnitRefno + "</ProductUnitRefno>" +
                                "<ProductCode>" + this.ActItem.ProdInfo.Code + "</ProductCode>" +
                                "<ProductUnitIsBC>" + this.ActItem.ProdInfo.UnitIsBc + "</ProductUnitIsBC>" +
                                "<UnitCode>" + this.ActItem.ProdInfo.UnitCode + "</UnitCode>" +
                                "<ProductUnitBarCodeCode>" + this.BarParse.ProductIdentifier + "</ProductUnitBarCodeCode>" +
                                "<ProductUnitBarCodeSymbologyPrefix>" + this.BarParse.SymbologyPrefix + "</ProductUnitBarCodeSymbologyPrefix>" +
                                "<alt_qty>" + xmlDocEdit.Root.Element("alt_qty").Value + "</alt_qty>" +
                                "<base_qty>" + xmlDocEdit.Root.Element("base_qty").Value + "</base_qty>" +
                                "<wt_qty>" + xmlDocEdit.Root.Element("wt_qty").Value + "</wt_qty>" +
                                "<whs_id>" + this.LocMap.LoginWhsLocMapRefno + "</whs_id>" +
                                "<refno>" + this.ActItem.ItemFkRefno + "</refno>" +
                                "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                                "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>" +
                                "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>" +
                                "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>" +
                                "</params>");
                            await this.ActItem.Insert(request);
                        }
                    }

                    // TODO: Fix sorting
                    // ActItem.DataViewItems.Sort = "ActivityItemFKRefno";
                    var purchaseOrderWorkSheetItem = this.ActItem.ListItems
                        .OfType<PurchaseOrderWorkSheetItem>().FirstOrDefault(
                            item => item.ActivityItemFKRefno == this.ActItem.ItemFkRefno);

                    if (purchaseOrderWorkSheetItem == null)
                    {
                        throw new Exception("Unable to identify the item.");
                    }

                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.ListViewMainActivity.SelectedItem = purchaseOrderWorkSheetItem);
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await this.DisplayAlert(this.GetControlDescription(),
                            "The scanned bar code type is not supported here.", "OK"));
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.

                // This exception is also caught if the scan event arguments are null.
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (IsReadyToApplyException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
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
                    await this.DisplayAlert("Activity Item", ex.Message, "OK"));
            }
            finally
            {
                if (scanArgs != null)
                {
                    await this.LoadAndBindData();
                }

                await this.ScannerEnable();
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
                var whsLocMapRefno =
                    await LocationMapTasks.GetWhsLocMapRefnoFromLocMapRefno(this.LocMap.CurrLocMapRefno);
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

            //////////////////////////////////////////////////////////////////////////
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
            var isAllowedLocationOverride = false;
            var useProductDefaultLocation = this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
            if (useProductDefaultLocation)
            {
                var allowLocationOverridePhysical = this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                                                    && this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation;
                var allowLocationOverrideCycle = this.ActSubType == SysCommon.ActSubTypes.CycleCounts
                                                 && this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation;
                if (allowLocationOverridePhysical || allowLocationOverrideCycle)
                {
                    isAllowedLocationOverride = true;
                }
            }

            return isAllowedLocationOverride;
        }

        /// <summary>
        ///    List view main activity item selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated this event.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs" /> Selected item changed event argument data.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await this.ActItem.AssignActivityItem(e.SelectedItem);
            }
        }
    }
}