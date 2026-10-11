// <copyright file="ActivityScanBarCodes.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using Android.App;
    using Android.Content;
    using Android.Views;
    using BarCodeScanner;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Item;
    using Forms;
    using GroundWork;
    using Resources.Enums;
    using Resources.Models.Products;
    using Resources.QueryResults;
    using Resources.Types.Products;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Content.PM;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Define the bar code scanning activity.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [Activity(
        Label = "dMobile",
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(
        new[] { "com.dmobile.ACTION" },
        Categories = new[] { Intent.CategoryDefault })]
    public partial class ActivityScanBarCodes : IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityScanBarCodes" /> class.
        /// </summary>
        public ActivityScanBarCodes()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();

            this.cmdAssignProduct.Text = "Assign Scan To Product";
            this.lblProduct.Text = "Product:";
            this.lblProductData.Text = "Unidentified";

            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ExtraInfoItems = new ObservableCollection<string>();

            this.LoadAndBindData().FireAndForgetSafeAsync();
            ParseAndSeedBarCodeModifier();
            this.ScannerEnable().FireAndForgetSafeAsync();
            return;

            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();
                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
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
        }

        private readonly ZebraIntentScannerService _scanner = ZebraIntentScannerService.Instance;

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.BarCodeAssociation;

        /// <summary>
        ///     Gets or sets the bar code parse instance.
        /// </summary>
        private BarCodeParse Parse { get; set; }

        /// <summary>
        ///     Gets or sets the activity item bar code instance.
        /// </summary>
        private ActItem ActItemBarCode { get; set; }

        /// <summary>
        ///     Gets or sets the activity bar code instance.
        /// </summary>
        private Controller.Activity.Activity ActBarCode { get; set; }

        /// <summary>
        ///     Gets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets the system environment instance.
        /// </summary> 
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets the items list collection.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; }

        /// <summary>
        ///     Gets or sets the product unit reference number.
        /// </summary>
        private int ProductUnitRefno { get; set; }

        /// <summary>
        ///     Gets or sets the product unit bar code reference number.
        /// </summary>
        private int ProductUnitBarCodeRefno { get; set; }

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
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                await this.Scanner.EnableScanning();
            }
        }

        /// <summary>
        ///     Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if the data is ready to be saved else false.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> IsReadyToSave()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }

        /// <summary>
        ///     Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form is ready to close else false.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> IsReadyToClose()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }

        /// <summary>
        ///     Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool" /> True if a save was performed successfully else false.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> SaveChanges()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
       }

        /// <summary>
        ///     Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool" />. True if the changes where canceled, else false.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> CancelChanges()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }

        /// <summary>
        ///     Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation containing an XDocument.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmlDoc;
        }

        /// <summary>
        ///     Get the activity description.
        /// </summary>
        /// <returns><see cref="string" /> Activity description.</returns>
        public string GetControlDescription()
        {
            return "Bar Code Association";
        }

        /// <summary>
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async void OnAppearing()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();
            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpCallback);
                return;

                async void KeyUpCallback(string keyPressed)
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.F1 && await this.SaveChanges())
                    {
                        await this.ScannerDisable();
                        await this.Close();
                    }
                }
            }
        }

        /// <summary>
        ///     On disappearing event handler.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            SysCommon.ShutdownConnection();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            async void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.ScannerDisable();
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.ScannerDisable();
                    await this.Close();
                }
            }
        }

        /// <summary>
        /// Load and bind the data to the form.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        private async Task LoadAndBindData()
        {
            this.lstScannedData.ItemsSource = this.ExtraInfoItems;
            var tableActivity = new TableActivity();
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await tableActivity.PurgeData(SysCommon.ActSubTypes.BarCodeAssociation);
            }

            this.ActBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
            await this.ActBarCode.Create(SysCommon.ActSubTypes.BarCodeAssociation);
            this.ActItemBarCode = ActItemFacade.GetActItem(
                this.ActBarCode,
                SysCommon.ActSubTypes.BarCodeAssociation);
        }

        /// <summary>
        ///     Add the scan message to the items list.
        /// </summary>
        /// <param name="msg"><see cref="string" /> Message to add.</param>
        private async Task AddScanMsg(string msg)
        {
            await Device.InvokeOnMainThreadAsync(() => { this.ExtraInfoItems.Add(msg); });
        }

        /// <summary>
        ///     Bar code event handler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Bar code information.</param>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            var isSerialTracked = false;
            var isProductionDateTracked = false;
            var isLotTracked = false;
            var isBestBeforeDateTracked = false;
            var isVariantTracked = false;
            ////var isLotUniqueComponent = false;
            ////var isProductionDateUniqueComponent = false;
            ////var isBestBeforeDateUniqueComponent = false;
            ////var isVariantUniqueComponent = false;
            var prodSchemeRefno = 0;
            var prodCode = string.Empty;
            var prodInfo = new ProductInfo();
            var currLocMap = LocationMap.GetInstance().CurrLocMap;

            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.Parse = scanArgs.ParseInfo;
                await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

                // Lookup product
                this.ProductUnitRefno = 0;
                this.ProductUnitBarCodeRefno = 0;

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                {
                    var prodIdentifier = scanArgs.ParseInfo.ProductIdentifier;
                    var resultsProductUnitBarcode = await FindProductUnit(prodIdentifier);

                    if (resultsProductUnitBarcode?.Count > 0)
                    {
                        this.ProductUnitRefno = resultsProductUnitBarcode[0].ProductUnitRefno;
                        this.ProductUnitBarCodeRefno = resultsProductUnitBarcode[0].ProductUnitBarCodeRefno;
                        prodSchemeRefno = resultsProductUnitBarcode[0].ProductSchemeRefno;
                        prodCode = resultsProductUnitBarcode[0].ProductCode;
                        prodInfo = await ProductInfoTasks.GetInfoFromRefno(resultsProductUnitBarcode[0].ProductRefno);
                        await DisplayProductAssocInfo(resultsProductUnitBarcode);
                    }

                    ////if (this.ProductUnitRefno == 0)
                    ////{
                    ////    await BarCodeHandlerProductIdentifierRuleSumwaDuck();
                    ////}

                    if (this.ProductUnitRefno == 0)
                    {
                        try
                        {
                            ////var unitRefnoFromBarcode = await BarCodeCommon.GetDeliProdInfoFromBarCode(
                            ////    prodIdentifier,
                            ////    this.SysEnv.CurrentBarCodeModifier1,
                            ////    this.Parse.Symbology,
                            ////    this.Parse.DeliScaleAmount);
                            ////this.ProductUnitRefno = unitRefnoFromBarcode.ProdInfo.UnitRefno;
                            ////var deliScaleWeight = unitRefnoFromBarcode.DeliScaleWeight;
                            ////var deliScaleWeightUnit = unitRefnoFromBarcode.DeliScaleWeightUnit;

                            var deliScaleWeight = 0;
                            var deliScaleWeightUnit = 0;

                            if (this.ProductUnitRefno == 0)
                            {
                                await this.AddScanMsg("Not associated to a product");

                                await Device.InvokeOnMainThreadAsync(() =>
                                {
                                    this.lblProductData.TextColor = Color.Red;

                                    this.lblProductData.Text = "Unidentified";
                                });
                            }
                            else
                            {
                                var queryProductUnit = @"
                                    select tProductUnit.ProductUnitRefno
                                            ,tLocationMapWarehouseProductScheme.ProductSchemeRefno AS WarehouseProductSchemeRefno
                                            ,tProduct.ProductCode
                                            ,tProductUnit.ProductUnitBC
                                            ,tUnit.UnitCode
                                        from tProductUnit 
                                        inner join tProduct on tProduct.ProductRefno = tProductUnit.ProductRefno 
                                        inner join tUnit on tUnit.UnitRefno = tProductUnit.UnitRefno 
                                        inner join tLocationMapWarehouseProductScheme on
                                            tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno";
                                queryProductUnit += " where tProductUnit.ProductUnitRefno = ?";
                                queryProductUnit +=
                                    " and tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ?";

                                var resultsProductUnit =
                                    await DatabaseConnection.GetInstance().SqlConn
                                        .QueryAsync<ProductUnitBarcodeQueryResult>(
                                            queryProductUnit,
                                            this.ProductUnitRefno,
                                            currLocMap.WhsLocMapRefno);

                                if (resultsProductUnit?.Count > 0)
                                {
                                    this.ProductUnitRefno = Convert.ToInt32(resultsProductUnit[0].ProductUnitRefno);
                                    this.ProductUnitBarCodeRefno = 0;
                                    prodSchemeRefno = Convert.ToInt32(resultsProductUnit[0].ProductSchemeRefno);
                                    prodCode = resultsProductUnit[0].ProductCode;

                                    // If product was found associated display it.  'ProductID+BC+UOM'
                                    await this.AddScanMsg("Product Associated To Bar Code");

                                    var prodBcUnit = resultsProductUnit[0].ProductCode + " " +
                                                     resultsProductUnit[0].ProductUnitBC + " " +
                                                     resultsProductUnit[0].UnitCode;
                                    await this.AddScanMsg(prodBcUnit);

                                    if (this.Parse.DeliScaleAmount != 0 &&
                                        (this.Parse.Symbology == BarCodeSymbology.UCC_12_UPC_E ||
                                         this.Parse.Symbology == BarCodeSymbology.EAN_UCC_13))
                                    {
                                        await this.AddScanMsg(
                                            $"Product Deli Scale Amount {this.Parse.DeliScaleAmount}");
                                        await this.AddScanMsg(
                                            $"Product Deli Scale Weight {deliScaleWeight} {deliScaleWeightUnit}");

                                        if (this.SysEnv.BarCodeSettings
                                            .IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount)
                                        {
                                            await this.AddScanMsg("Barcode Rule: 4 digit amount");
                                        }
                                    }

                                    await Device.InvokeOnMainThreadAsync(() =>
                                    {
                                        this.lblProductData.TextColor = Color.Black;
                                        this.lblProductData.Text = prodBcUnit;
                                    });
                                }
                            }
                        }
                        catch (Exception)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                await this.AddScanMsg("Product unidentified");

                                this.lblProductData.TextColor = Color.Red;

                                this.lblProductData.Text = "Unidentified";
                            });
                        }
                    }

                    // get product tracking scheme
                    if (prodSchemeRefno != 0)
                    {
                        const string queryProductScheme = @"
                            select tProductScheme.ProductSchemeIsSerialTracked,
                                    tProductScheme.ProductSchemeIsProductionDateTracked,
                                    tProductScheme.ProductSchemeIsLotTracked,
                                    tProductScheme.ProductSchemeIsBestBeforeDateTracked,
                                    tProductScheme.ProductSchemeIsVariantTracked,
                                    tProductScheme.ProductSchemeIsLotUniqueComponent,
                                    tProductScheme.ProductSchemeIsProductionDateUniqueComponent,
                                    tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent,
                                    tProductScheme.ProductSchemeIsVariantUniqueComponent
                                from tProductScheme
                                where tProductScheme.ProductSchemeRefno = ?";

                        var resultsProductScheme =
                            await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductSchemeModel>(
                                queryProductScheme,
                                prodSchemeRefno);

                        if (resultsProductScheme?.Count > 0)
                        {
                            isSerialTracked = resultsProductScheme[0].ProductSchemeIsSerialTracked;
                            isProductionDateTracked = resultsProductScheme[0].ProductSchemeIsProductionDateTracked;
                            isLotTracked = resultsProductScheme[0].ProductSchemeIsLotTracked;
                            isBestBeforeDateTracked = resultsProductScheme[0].ProductSchemeIsBestBeforeDateTracked;
                            isVariantTracked = resultsProductScheme[0].ProductSchemeIsVariantTracked;
                            ////isLotUniqueComponent = resultsProductScheme[0].ProductSchemeIsLotUniqueComponent;
                            ////isProductionDateUniqueComponent =
                            ////    resultsProductScheme[0].ProductSchemeIsProductionDateUniqueComponent;
                            ////isBestBeforeDateUniqueComponent =
                            ////    resultsProductScheme[0].ProductSchemeIsBestBeforeDateUniqueComponent;
                            ////isVariantUniqueComponent = resultsProductScheme[0].ProductSchemeIsVariantUniqueComponent;
                        }
                        else
                        {
                            throw new Exception($"Unable to locate the product scheme for product \'{prodCode}\'");
                        }
                    }
                }

                // do not allow product bar code reassignment
                if (this.ProductUnitRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => { this.cmdAssignProduct.IsEnabled = true; });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() => { this.cmdAssignProduct.IsEnabled = false; });
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.Sscc18))
                {
                    await BarCodeHandlerSscc18();
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier();
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.Gtin))
                {
                    await this.AddScanMsg("GTIN: " + scanArgs.ParseInfo.Gtin);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.GtinLogisticUnit))
                {
                    await this.AddScanMsg("GTIN in a Logistic Unit: " + scanArgs.ParseInfo.GtinLogisticUnit);
                }

                if (scanArgs.ParseInfo.PackagingType != 0)
                {
                    await this.AddScanMsg("Packaging Type: " + scanArgs.ParseInfo.PackagingType);
                }

                if (scanArgs.ParseInfo.NumberSystemCharacter != 0)
                {
                    await this.AddScanMsg("Number System Character: " + scanArgs.ParseInfo.NumberSystemCharacter);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                {
                    await this.AddScanMsg("Product Identifier: " + scanArgs.ParseInfo.ProductIdentifier);
                }

                // TODO: Verify data is accurate/in correct position
                // DEV-5602 Brand Field Not Displaying on dMobile Bar Code Association Window
                if (!string.IsNullOrEmpty(prodInfo?.BrandDesc))
                {
                    await this.AddScanMsg("Brand Description: " + prodInfo.BrandDesc);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ItemReference))
                {
                    await this.AddScanMsg("Item Reference: " + scanArgs.ParseInfo.ItemReference);
                }

                await BarCodeHandlerScheme();

                // AI 91 Activity Item FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActItemFkRefno))
                {
                    await this.AddScanMsg("Activity Item FK Ref#: " + scanArgs.ParseInfo.ActItemFkRefno);
                }

                // AI 92 Activity FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActFkRefno))
                {
                    await this.AddScanMsg("Activity FK Ref#: " + scanArgs.ParseInfo.ActFkRefno);
                }

                // AI 93 Activity Detail FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActDetailFkRefno))
                {
                    await this.AddScanMsg("Activity Detail FK Ref#: " + scanArgs.ParseInfo.ActDetailFkRefno);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.Remainder))
                {
                    await this.AddScanMsg("Unidentifiable Data");
                    await this.AddScanMsg(scanArgs.ParseInfo.Remainder);
                }

                // Symbology ID
                await this.AddScanMsg("Symbology: " + scanArgs.ParseInfo.Symbology);

                // Symbology Prefix
                await this.AddScanMsg("Symbology Prefix: " + scanArgs.ParseInfo.SymbologyPrefix);

                // Length
                await this.AddScanMsg($"Length: {scanArgs.ParseInfo.Length} prefix excluded");

                // Raw Bar Code Data
                await this.AddScanMsg("Bar Code Raw");

                const int lineLength = 30;
                var rawDataLine = SysCommon.WordWrap2List(scanArgs.ParseInfo.Raw, lineLength);

                for (var i = 0; i < rawDataLine.Count; i++)
                {
                    await this.AddScanMsg(rawDataLine[i]);
                }

                // <issue="13609" author="RD" date="2016/02/22">
                // All errors written to screen instead of throwing an error.
                if (this.Parse.HasErrors())
                {
                    if (this.Parse.Details.ErrorCount == 1)
                    {
                        await this.AddScanMsg("1 Error");
                    }
                    else
                    {
                        await this.AddScanMsg($"{this.Parse.Details.ErrorCount} Errors");
                    }

                    foreach (var errorDetail in this.Parse.Details.ErrorDetails)
                    {
                        var lines = SysCommon.WordWrap2List(errorDetail.Exception.Message, lineLength);

                        for (var i = 0; i < lines.Count; ++i)
                        {
                            await this.AddScanMsg(lines[i]);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                await this.LoadAndBindData();
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerSscc18()
            {
                await this.AddScanMsg("SSCC-18: " + scanArgs.ParseInfo.Sscc18);

                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    await this.AddScanMsg("Disconnected mode, unable to lookup SSCC-18.");
                }
                else
                {
                    try
                    {
                        this.lblProductData.Text = "Unidentified";
                        var skid = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                            scanArgs.ParseInfo.Sscc18);

                        if (skid.CurrSkid.SkidFkRefno > 0)
                        {
                            //// AddScanMsg(sender, BarCodeScanArgs, "Skid identified")
                            await this.AddScanMsg($"Code: {skid.CurrSkid.LogisticUnitSkidCode}");
                            await this.AddScanMsg($"Refno: {skid.CurrSkid.SkidFkRefno}");
                            await this.AddScanMsg($"Number: {skid.CurrSkid.SkidCode}");

                            var locReadable = LocationMapTasks.GetReadableLocation(
                                skid.CurrSkid.LocMap);

                            var skidQty =
                                $"Qty: {skid.CurrSkid.Quantity} {skid.CurrSkid.ProdInfo.UnitCode} @" +
                                $" {skid.CurrSkid.WeightQuantity} {skid.CurrSkid.ProdInfo.WeightUnitCode}";

                            await this.AddScanMsg($"Location: {locReadable}");
                            await this.AddScanMsg($"Product: {ProductInfoTasks.GetCurrReadableProdBcUnit(skid.CurrSkid.ProdInfo)}");
                            await this.AddScanMsg(skidQty);
                            await this.AddScanMsg($"Put Away: {skid.CurrSkid.IsPutAway}");
                            await this.AddScanMsg($"On Hold: {skid.CurrSkid.IsHold}");
                            await this.AddScanMsg($"Cube: {skid.CurrSkid.Cube} {skid.CurrSkid.CubeUnit}");
                            await this.AddScanMsg($"Height: {skid.CurrSkid.Height} {skid.CurrSkid.HeightUnit}");
                            await this.AddScanMsg($"Ti/Hi: {skid.CurrSkid.Ti}/{skid.CurrSkid.Hi}");
                            await this.AddScanMsg($"CHEP: {skid.CurrSkid.IsChep}");
                            await this.AddScanMsg($"Completed: {skid.CurrSkid.IsCompleted}");
                            await this.AddScanMsg($"Consumed: {skid.CurrSkid.IsConsumed}");
                        }
                        else
                        {
                            throw new Exception("Not associated to a skid.");
                        }
                    }
                    catch (Exception)
                    {
                        await this.AddScanMsg("Skid unidentified.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier()
            {
                await this.AddScanMsg("Location identifier: " + scanArgs.ParseInfo.LocationIdentifier);

                var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                    scanArgs.ParseInfo.LocationIdentifier);

                if (locMapRefno > 0)
                {
                    var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                    await this.AddScanMsg($"Warehouse code: {locMap.WhsCode}");
                    await this.AddScanMsg($"Warehouse refno: {locMap.WhsFkRefno}");
                    await this.AddScanMsg($"Warehouse name: {locMap.WhsName}");
                    await this.AddScanMsg($"Section code: {locMap.SectCode}");
                    await this.AddScanMsg($"Section refno: {locMap.SectFkRefno}");
                    await this.AddScanMsg($"Section name: {locMap.SectName}");
                    await this.AddScanMsg($"Slot code: {locMap.SlotCode}");
                    await this.AddScanMsg($"Slot refno: {locMap.SlotFkRefno}");
                    await this.AddScanMsg($"Slot name: {locMap.SlotName}");
                }
                else
                {
                    await this.AddScanMsg("Not associated to a location.");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerScheme()
            {
                await DisplayParsedSchemeData();
                await DisplayParsedWeights();

                if (scanArgs.ParseInfo.LogisticUnitCountOfTradeItems != 0)
                {
                    await this.AddScanMsg("Count of trade items: " + scanArgs.ParseInfo.LogisticUnitCountOfTradeItems);
                }

                if (scanArgs.ParseInfo.LogisticUnitVariableCount != 0)
                {
                    await this.AddScanMsg("Variable Count: " + scanArgs.ParseInfo.LogisticUnitVariableCount);
                }

                if (scanArgs.ParseInfo.LengthOr1StDimensionMeters != 0)
                {
                    await this.AddScanMsg("Length Meters: " + scanArgs.ParseInfo.LengthOr1StDimensionMeters);
                }

                return;

                async Task DisplayParsedWeights()
                {
                    if (scanArgs.ParseInfo.NetWeightMetricKgFoundAi)
                    {
                        await this.AddScanMsg("Net weight metric KG: " + scanArgs.ParseInfo.NetWeightMetricKg);
                    }

                    if (scanArgs.ParseInfo.NetWeightNonMetricLbFoundAi)
                    {
                        await this.AddScanMsg("Net weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLb);
                    }

                    if (scanArgs.ParseInfo.GrossWeightMetricKgFoundAi)
                    {
                        await this.AddScanMsg("Gross weight metric KG: " + scanArgs.ParseInfo.GrossWeightMetricKg);
                    }

                    if (scanArgs.ParseInfo.GrossWeightNonMetricLbFoundAi)
                    {
                        await this.AddScanMsg("Gross weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLb);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task DisplayParsedSchemeData()
                {
                    if (scanArgs.ParseInfo.ManufacturerSerialNumber != 0)
                    {
                        await this.AddScanMsg("Manufacturer Serial Number: " + scanArgs.ParseInfo.ManufacturerSerialNumber);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.SingleTradeItemSerialNumber))
                    {
                        if (isSerialTracked)
                        {
                            await this.AddScanMsg("(Tracked) Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                        else
                        {
                            await this.AddScanMsg("Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                    }
                    else
                    {
                        if (isSerialTracked)
                        {
                            await this.AddScanMsg("(Tracked) Serial # Missing");
                        }
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LogisticUnitBatchLotNumber))
                    {
                        if (isLotTracked)
                        {
                            await this.AddScanMsg("(Tracked) Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                        else
                        {
                            await this.AddScanMsg("Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                    }
                    else
                    {
                        if (isLotTracked)
                        {
                            await this.AddScanMsg("(Tracked) Batch / Lot Number Missing.");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitProductionDate))
                    {
                        if (isProductionDateTracked)
                        {
                            var msg = "(Tracked) Production Date: " +
                                      scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString();
                            await this.AddScanMsg(msg);
                        }
                        else
                        {
                            await this.AddScanMsg("Production Date: " +
                                                  scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString());
                        }
                    }
                    else
                    {
                        if (isProductionDateTracked)
                        {
                            await this.AddScanMsg("(Tracked) Production Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate))
                    {
                        if (isBestBeforeDateTracked)
                        {
                            var msg = "(Tracked) Best Before Date: " +
                                      scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString();
                            await this.AddScanMsg(msg);
                        }
                        else
                        {
                            var msg = "Best Before Date: " +
                                      scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString();
                            await this.AddScanMsg(msg);
                        }
                    }
                    else
                    {
                        if (isBestBeforeDateTracked)
                        {
                            await this.AddScanMsg("(Tracked) Best Before Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitExpiryUseByDate))
                    {
                        var msg = "Expiry Date: " + scanArgs.ParseInfo.LogisticUnitExpiryUseByDate.ToShortDateString();
                        await this.AddScanMsg(msg);
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitDueDate))
                    {
                        var msg = "Due Date: " + scanArgs.ParseInfo.LogisticUnitDueDate.ToShortDateString();
                        await this.AddScanMsg(msg);
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitPackagingDate))
                    {
                        var msg = "Packaging Date: " + scanArgs.ParseInfo.LogisticUnitPackagingDate.ToShortDateString();
                        await this.AddScanMsg(msg);
                    }

                    if (scanArgs.ParseInfo.LogisticUnitProductVariant != 0)
                    {
                        if (isVariantTracked)
                        {
                            var msg = "(Tracked) Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant;
                            await this.AddScanMsg(msg);
                        }
                        else
                        {
                            var msg = "Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant;
                            await this.AddScanMsg(msg);
                        }
                    }
                    else
                    {
                        if (isVariantTracked)
                        {
                            await this.AddScanMsg("(Tracked) Product variant missing.");
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerProductIdentifierRuleSumwaDuck()
            {
                // Do not look up gtin in bar code database for this rule, product will be identified by a range
                // of fixed weights.
                if (this.SysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct &&
                    this.Parse.ProductIdentifier == "90695955110022")
                {
                    BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct(this.Parse);

                    prodInfo = await ProductInfoTasks.GetInfoFromCodeBcUnit(
                        this.Parse.ProductIdentifier,
                        string.Empty,
                        "CS");
                    this.ProductUnitRefno = prodInfo.UnitRefno;
                }

                return;

                static void BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIdProduct(BarCodeParse barCodeParse)
                {
                    if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(11.00) &&
                        barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(11.99))
                    {
                        barCodeParse.ProductIdentifier = "MC10448";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.00) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.49))
                    {
                        barCodeParse.ProductIdentifier = "MC10443";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(12.50) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(12.99))
                    {
                        barCodeParse.ProductIdentifier = "MC10441";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.00) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.49))
                    {
                        barCodeParse.ProductIdentifier = "MC10442";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(13.50) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(13.99))
                    {
                        barCodeParse.ProductIdentifier = "MC10440";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.00) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.49))
                    {
                        barCodeParse.ProductIdentifier = "MC10445";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(14.50) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(14.99))
                    {
                        barCodeParse.ProductIdentifier = "MC10446";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.00) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(15.49))
                    {
                        barCodeParse.ProductIdentifier = "MC10447";
                    }
                    else if (barCodeParse.NetWeightMetricKg >= Convert.ToDecimal(15.50) &&
                             barCodeParse.NetWeightMetricKg <= Convert.ToDecimal(16.00))
                    {
                        barCodeParse.ProductIdentifier = "MC10450";
                    }
                    else
                    {
                        throw new Exception("Unable to identify product scanned." + Environment.NewLine +
                                            "ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct enabled.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<List<ProductUnitBarcodeQueryResult>> FindProductUnit(string prodIdentifier)
            {
                var queryProductUnitBarcode = @"
                    select tProductUnit.ProductUnitRefno,
                            tProductUnitBarCode.ProductUnitBarCodeRefno,
                            tLocationMapWarehouseProductScheme.ProductSchemeRefno AS WarehouseProductSchemeRefno,
                            tProduct.ProductSchemeRefno,
                            tProduct.ProductCode,
                            tProduct.ProductRefno,
                            tProduct.ProductDescription,
                            tProductUnit.ProductUnitBC,
                            tUnit.UnitCode
                        from tProductUnitBarCode
                        inner join tProductUnit on tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno
                        inner join tProduct on tProduct.ProductRefno = tProductUnit.ProductRefno
                        inner join tUnit on tUnit.UnitRefno = tProductUnit.UnitRefno
                        inner join tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno
                        where tProductUnitBarCode.ProductUnitBarCodeCode = ?
                            and tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ?
                            and tProductUnitBarCode.ProductUnitBarCodeModifier1 = ?";

                var resultsProductUnitBarcode =
                    await DatabaseConnection.GetInstance().SqlConn.QueryAsync<ProductUnitBarcodeQueryResult>(
                        queryProductUnitBarcode,
                        prodIdentifier,
                        currLocMap.WhsLocMapRefno,
                        this.SysEnv.CurrentBarCodeModifier1);
                return resultsProductUnitBarcode;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayProductAssocInfo(IReadOnlyList<ProductUnitBarcodeQueryResult> resultsProductUnitBarcode)
            {
                // If product was found associated display it.  'ProductID+BC+UOM'
                await this.AddScanMsg("Product associated to bar code");

                var prodBcUnit = resultsProductUnitBarcode[0].ProductCode + " " +
                                 resultsProductUnitBarcode[0].ProductUnitBC + " " +
                                 resultsProductUnitBarcode[0].UnitCode;

                await this.AddScanMsg(prodBcUnit);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblProductData.TextColor = Color.Black;
                    this.lblProductData.Text = prodBcUnit;
                });

                await this.AddScanMsg(resultsProductUnitBarcode[0].ProductDescription);
            }
        }

        /// <summary>
        ///     Bar code modifier change made event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void BarCodeModifierChangesMade(object sender, EventArgs e)
        {
            try
            {
                this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(ModifierChangeError);

                //////////////////////////////////////////////////////////////////////////
                async void ModifierChangeError()
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                }
            }
        }

        /// <summary>
        ///     Assign product click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdAssignProduct_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (this.Parse != null)
                {
                    var isAllowedAssocProdBarCode = await CheckAllowedToAssignProduct();
                    if (isAllowedAssocProdBarCode)
                    {
                        await AssignProduct();
                        await this.LoadAndBindData();
                    }

                    // Just return without a message because the user will
                    // already see a permission denied if they failed to enter override code.
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
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
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.Parse.ClearResetBarCodeProperties();
                });

                await this.ScannerEnable();
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> CheckAllowedToAssignProduct()
            {
                // Changed functionality to disallow product scanning everywhere.
                // However, always prompt for password if not allowed to associate.
                var isAllowedAssocProdBarCode = this.SysEnv.ContactLog.IsAllowedAssociateProductBarCode;
                if (!isAllowedAssocProdBarCode)
                {
                    // Prompt for password
                    var formBarcodeOverride = new FormAssociateBarCodeOverride();

                    if (await formBarcodeOverride.ShowDialog() == DialogResult.OK)
                    {
                        isAllowedAssocProdBarCode = true;
                    }
                }

                return isAllowedAssocProdBarCode;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task AssignProduct()
            {
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXml();
                    this.ProductUnitRefno = int.Parse(xmlDoc.Root!.Element("ProductUnitRefno") !.Value);

                    if (this.ProductUnitRefno != 0)
                    {
                        await AssocBarCodeToProduct();
                        var productUnit = await FindAssocProductInfo();
                        await DisplayProductAssocData(productUnit);
                    }
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                async Task DisplayProductAssocData(List<ProductUnit> productUnit)
                {
                    var productDataText = string.Empty;
                    if (productUnit?.Count > 0)
                    {
                        productDataText = Convert.ToString(productUnit[0].ProductCode).TrimEnd() +
                                          (" " + Convert.ToString(productUnit[0].ProductUnitBC)).TrimEnd() + " " +
                                          Convert.ToString(productUnit[0].UnitCode);
                    }

                    if (!string.IsNullOrEmpty(productDataText))
                    {
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.lblProductData.TextColor = Color.Black;
                            this.lblProductData.Text = productDataText;
                        });
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task<List<ProductUnit>> FindAssocProductInfo()
                {
                    var queryProductUnit = @"
                        select tProduct.ProductCode,
                                tProductUnit.ProductUnitBC,
                                tUnit.UnitCode
                            from tProductUnit
                            inner join tProduct on tProduct.ProductRefno = tProductUnit.ProductRefno
                            inner join tUnit on tUnit.UnitRefno = tProductUnit.UnitRefno
                            where tProductUnit.ProductUnitRefno = ?";

                    var productUnit = await DatabaseConnection.GetInstance().SqlConn
                        .QueryAsync<ProductUnit>(queryProductUnit, this.ProductUnitRefno);
                    return productUnit;
                }

                //////////////////////////////////////////////////////////////////////////
                async Task AssocBarCodeToProduct()
                {
                    var svdProductIdentifier = this.Parse.ProductIdentifier;
                    var svdSymbology = this.Parse.Symbology;
                    var svdSymbologyPrefix = this.Parse.SymbologyPrefix;

                    var xmlDoc = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<params>"
                        + "<ProductUnitRefno>" + this.ProductUnitRefno + "</ProductUnitRefno>"
                        + "<ProductUnitBarCodeRefno>" + this.ProductUnitBarCodeRefno + "</ProductUnitBarCodeRefno>"
                        + "<ProductIdentifier>" + svdProductIdentifier + "</ProductIdentifier>"
                        + "<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1
                        + "</ProductUnitBarCodeModifier1>"
                        + "<RawData></RawData>"
                        + "<enumBarCodeSymbology>" + (int)svdSymbology + "</enumBarCodeSymbology>"
                        + "<BarCodeSymbologyPrefix>" + svdSymbologyPrefix + "</BarCodeSymbologyPrefix>"
                        + "</params>");
                    await this.ActItemBarCode.Insert(xmlDoc);

                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.ExtraInfoItems[0] = "Product associated to bar code");
                }
            }
        }


    }
}