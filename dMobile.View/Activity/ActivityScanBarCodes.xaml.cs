// <copyright file="ActivityScanBarCodes.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.GroundWork;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Models;
    using dMobile.Resources.QueryResults;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Color = Xamarin.Forms.Color;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Define the bar code scanning activity.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityScanBarCodes : DMobileContentPage, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityScanBarCodes"/> class.
        /// </summary>
        public ActivityScanBarCodes()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            this.cmdAssignProduct.Text = "Assign Scan To Product";
            this.lblProduct.Text = "Product:";
            this.lblProductData.Text = "Unidentified";

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ExtraInfoItems = new ObservableCollection<string>();

            ParseAndSeedBarCodeModifier();
            ////this.BarCodeModifier1Picker.Items.Clear();

            ////if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            ////{
            ////    this.BarCodeModifier1Picker.Items.Add(string.Empty);
            ////    string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

            ////    foreach (string item in modifiers)
            ////    {
            ////        this.BarCodeModifier1Picker.Items.Add(item);
            ////    }
            ////}
            ////else
            ////{
            ////    this.BarCodeModifier1Picker.IsVisible = false;
            ////    this.BarCodeModifier1Picker.IsEnabled = false;
            ////}

            ////this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;

                // Parse and seed the modifiers control
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

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.BarCodeAssociation;

        /// <summary>
        /// Gets or sets the bar code parse instance.
        /// </summary>
        private BarCodeParse Parse { get; set; } = null;

        /// <summary>
        /// Gets or sets the activity item bar code instance.
        /// </summary>
        private ActItem ActItemBarCode { get; set; } = null;

        /// <summary>
        /// Gets or sets the activity bar code instance.
        /// </summary>
        private Activity ActBarCode { get; set; } = null;

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets or sets the items list collection.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler has be registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the activity has been initialized.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether any changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the product unit reference number.
        /// </summary>
        private int ProductUnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product unit bar code reference number.
        /// </summary>
        private int ProductUnitBarCodeRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the bar code scan parse instance has data.
        /// </summary>
        private bool ScanHasData { get; set; } = false;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    await this.Scanner.Stop();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Check if the activity header data is ready to be saved.
        /// </summary>
        /// <returns><see cref="bool"/> True if the data is ready to be saved else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
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
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// Check if this form is ready to close, that is can be closed.
        /// </summary>
        /// <returns><see cref="Task{bool}"/> True if the form is ready to close else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initialize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
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
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Get the activity description.
        /// </summary>
        /// <returns><see cref="string"/> Activity description.</returns>
        public string GetControlDescription()
        {
            return "Bar Code Association";
        }

        /// <summary>
        /// Check if a save can be performed and do so.
        /// </summary>
        /// <returns><see cref="bool"/> True if a save was performed successfully else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    // ClearContents();
                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new MessageOnlyException("Data failed validation. Check your data.");
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
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// Check if there are any pending changes, if so cancel them.
        /// </summary>
        /// <returns><see cref="bool"/>. True if the changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    ////ClearProperties();
                    ////DisplayData();
                    ////this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
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
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// Assemble a XML string of activity data to return to the calling parent form.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation containing an XDocument.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmlDoc;
        }

        /// <summary>
        /// Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                SubscribeKeyUpHandlers();

                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    this.lstScannedData.ItemsSource = this.ExtraInfoItems;
                    var tableActItemBarCodeXRef =
                        new TableActivityItemBarCodeAssociationXRef();
                    var tableActItem = new TableActivityItem();
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

                    await this.ScannerStart();
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
                this.Loading = false;

                await Task.Run(() =>
                {
                    // Explicit GC, Xamarin forms not disposing of objects properly
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                });
            }

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            await this.Close();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// On disappearing event handler.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        /// <summary>
        /// Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
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
            });

            return true;
        }

        /// <summary>
        /// Add the scan message to the items list.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        /// <param name="msg"><see cref="string"/> Message to add.</param>
        private async Task AddScanMsg(object sender, EventArgs e, string msg)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.ExtraInfoItems.Add(msg);
            });
        }

        /// <summary>
        /// Bar code event handler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Bar code information.</param>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            this.Loading = true;

            var isSerialTracked = false;
            var isProductionDateTracked = false;
            var isLotTracked = false;
            var isBestBeforeDateTracked = false;
            var isVariantTracked = false;
            var isLotUniqueComponent = false;
            var isProductionDateUniqueComponent = false;
            var isBestBeforeDateUniqueComponent = false;
            var isVariantUniqueComponent = false;
            var prodSchemeRefno = 0;
            var prodCode = string.Empty;
            var prodInfo = new ProductInfo();
            var currLocMap = LocationMap.GetInstance().CurrLocMap;

            try
            {
                await this.ScannerDisable();

                this.Parse = scanArgs.ParseInfo;
                if (this.Parse != null)
                {
                    this.ScanHasData = true;
                }

                await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

                // Lookup product
                this.ProductUnitRefno = 0;
                this.ProductUnitBarCodeRefno = 0;

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                {
                    var prodIdentifier = scanArgs.ParseInfo.ProductIdentifier;
                    var resultsProductUnitBarcode = await FindProductUnit(currLocMap, prodIdentifier);

                    if (resultsProductUnitBarcode?.Count > 0)
                    {
                        this.ProductUnitRefno = resultsProductUnitBarcode[0].ProductUnitRefno;
                        this.ProductUnitBarCodeRefno = resultsProductUnitBarcode[0].ProductUnitBarCodeRefno;
                        prodSchemeRefno = resultsProductUnitBarcode[0].ProductSchemeRefno;
                        prodCode = resultsProductUnitBarcode[0].ProductCode;

                        await DisplayProductAssocInfo(sender, scanArgs, resultsProductUnitBarcode);
                    }

                    if (this.ProductUnitRefno == 0)
                    {
                        prodInfo = await BarCodeHandlerProductIdentifierRuleSumwaDuck(prodInfo);
                    }

                    if (this.ProductUnitRefno == 0)
                    {
                        try
                        {
                            var unitRefnoFromBarcode = await clsCommon.GetDeliProdInfoFromBarCode(
                                prodIdentifier,
                                this.SysEnv.CurrentBarCodeModifier1,
                                this.Parse.Symbology,
                                this.Parse.DeliScaleAmount);
                            this.ProductUnitRefno = unitRefnoFromBarcode.ProdInfo.UnitRefno;
                            var deliScaleWeight = unitRefnoFromBarcode.DeliScaleWeight;
                            var deliScaleWeightUnit = unitRefnoFromBarcode.DeliScaleWeightUnit;

                            if (this.ProductUnitRefno == 0)
                            {
                                await this.AddScanMsg(sender, scanArgs, "Not associated to a product");

                                await Device.InvokeOnMainThreadAsync(() =>
                                {
                                    this.lblProductData.TextColor = Color.Red;

                                    this.lblProductData.Text = "Unidentified";
                                });
                            }
                            else
                            {
                                var queryProductUnit = "SELECT tProductUnit.ProductUnitRefno,"
                                                       + "tLocationMapWarehouseProductScheme.ProductSchemeRefno AS WarehouseProductSchemeRefno,"
                                                       + "tProduct.ProductCode,"
                                                       + "tProductUnit.ProductUnitBC,"
                                                       + "tUnit.UnitCode "
                                                       + "FROM tProductUnit "
                                                       + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                                       + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                                       + "INNER JOIN tLocationMapWarehouseProductScheme ON" +
                                                       " tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno "
                                                       + "WHERE tProductUnit.ProductUnitRefno = ? "
                                                       + "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ?";

                                var resultsProductUnit =
                                    await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnitBarcodeQueryResult>(
                                        queryProductUnit,
                                        this.ProductUnitRefno,
                                        currLocMap.WhsLocMapRefno);

                                if (resultsProductUnit?.Count > 0)
                                {
                                    this.ProductUnitRefno = Convert.ToInt32(resultsProductUnit[0].ProductUnitRefno);
                                    this.ProductUnitBarCodeRefno = 0;
                                    prodSchemeRefno = Convert.ToInt32(resultsProductUnit[0].ProductSchemeRefno);
                                    prodCode = resultsProductUnit[0].ProductCode.ToString();

                                    // If product was found associated display it.  'ProductID+BC+UOM'
                                    await this.AddScanMsg(sender, scanArgs, "Product Associated To Bar Code");

                                    var prodbcuom = resultsProductUnit[0].ProductCode.ToString() + " " +
                                                    resultsProductUnit[0].ProductUnitBC.ToString() + " " +
                                                    resultsProductUnit[0].UnitCode.ToString();
                                    await this.AddScanMsg(sender, scanArgs, prodbcuom);

                                    if (this.Parse.DeliScaleAmount != 0 &&
                                        (this.Parse.Symbology == BarCodeSymbologies.UCC_12_UPC_E ||
                                        this.Parse.Symbology == BarCodeSymbologies.EAN_UCC_13))
                                    {
                                        await this.AddScanMsg(sender, scanArgs, string.Format(
                                            "Product Deli Scale Amount {0}",
                                            this.Parse.DeliScaleAmount));
                                        await this.AddScanMsg(sender, scanArgs, string.Format(
                                            "Product Deli Scale Weight {0} {1}",
                                            deliScaleWeight,
                                            deliScaleWeightUnit));

                                        if (this.SysEnv.BarCodeSettings.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount)
                                        {
                                            await this.AddScanMsg(sender, scanArgs, "Barcode Rule: 4 digit amount");
                                        }
                                    }

                                    await Device.InvokeOnMainThreadAsync(() =>
                                    {
                                        this.lblProductData.TextColor = Color.Black;
                                        this.lblProductData.Text = prodbcuom;
                                    });
                                }
                            }
                        }
                        catch (Exception)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                await this.AddScanMsg(sender, scanArgs, "Product unidentified");

                                this.lblProductData.TextColor = Color.Red;

                                this.lblProductData.Text = "Unidentified";
                            });
                        }
                    }

                    // get product tracking scheme
                    if (prodSchemeRefno != 0)
                    {
                        var queryProductScheme = "SELECT tProductScheme.ProductSchemeIsSerialTracked,"
                                                 + "tProductScheme.ProductSchemeIsProductionDateTracked,"
                                                 + "tProductScheme.ProductSchemeIsLotTracked,"
                                                 + "tProductScheme.ProductSchemeIsBestBeforeDateTracked,"
                                                 + "tProductScheme.ProductSchemeIsVariantTracked,"
                                                 + "tProductScheme.ProductSchemeIsLotUniqueComponent,"
                                                 + "tProductScheme.ProductSchemeIsProductionDateUniqueComponent,"
                                                 + "tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent,"
                                                 + "tProductScheme.ProductSchemeIsVariantUniqueComponent "
                                                 + "FROM tProductScheme "
                                                 + "WHERE tProductScheme.ProductSchemeRefno = ?";

                        var resultsProductScheme =
                            await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductSchemeModel>(
                                queryProductScheme,
                                prodSchemeRefno);

                        if (resultsProductScheme?.Count > 0)
                        {
                            isSerialTracked = resultsProductScheme[0].ProductSchemeIsSerialTracked;
                            isProductionDateTracked = resultsProductScheme[0].ProductSchemeIsProductionDateTracked;
                            isLotTracked = resultsProductScheme[0].ProductSchemeIsLotTracked;
                            isBestBeforeDateTracked = resultsProductScheme[0].ProductSchemeIsBestBeforeDateTracked;
                            isVariantTracked = resultsProductScheme[0].ProductSchemeIsVariantTracked;
                            isLotUniqueComponent = resultsProductScheme[0].ProductSchemeIsLotUniqueComponent;
                            isProductionDateUniqueComponent =
                                resultsProductScheme[0].ProductSchemeIsProductionDateUniqueComponent;
                            isBestBeforeDateUniqueComponent =
                                resultsProductScheme[0].ProductSchemeIsBestBeforeDateUniqueComponent;
                            isVariantUniqueComponent = resultsProductScheme[0].ProductSchemeIsVariantUniqueComponent;
                        }
                        else
                        {
                            throw new Exception(string.Format(
                                "Unable to locate the product scheme for product \'{0}\'",
                                prodCode));
                        }
                    }
                }

                // do not allow product bar code reassignment
                if (this.ProductUnitRefno == 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.cmdAssignProduct.IsEnabled = true;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.cmdAssignProduct.IsEnabled = false;
                    });
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.SSCC18))
                {
                    await BarCodeHandlerSSCC18(sender, scanArgs);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier(sender, scanArgs);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.GTIN))
                {
                    await this.AddScanMsg(sender, scanArgs, "GTIN: " + scanArgs.ParseInfo.GTIN);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.GTINLogisticUnit))
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "GTIN in a Logistic Unit: " + scanArgs.ParseInfo.GTINLogisticUnit);
                }

                if (scanArgs.ParseInfo.PackagingType != 0)
                {
                    await this.AddScanMsg(sender, scanArgs, "Packaging Type: " + scanArgs.ParseInfo.PackagingType);
                }

                if (scanArgs.ParseInfo.NumberSystemCharacter != 0)
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Number System Character: " + scanArgs.ParseInfo.NumberSystemCharacter);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Product Identifier: " + scanArgs.ParseInfo.ProductIdentifier);
                }

                // TODO: Verify data is accurate/in correct position
                // DEV-5602 Brand Field Not Displaying on dMobile Bar Code Association Window
                if (!string.IsNullOrEmpty(prodInfo?.BrandDesc))
                {
                    await this.AddScanMsg(sender, scanArgs, "Brand Description: " + prodInfo.BrandDesc);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ItemReference))
                {
                    await this.AddScanMsg(sender, scanArgs, "Item Reference: " + scanArgs.ParseInfo.ItemReference);
                }

                await BarCodeHandlerScheme(
                    sender,
                    scanArgs,
                    isSerialTracked,
                    isProductionDateTracked,
                    isLotTracked,
                    isBestBeforeDateTracked,
                    isVariantTracked,
                    prodInfo);

                // AI 91 Activity Item FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActItemFKRefno))
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Activity Item FK Ref#: " + scanArgs.ParseInfo.ActItemFKRefno);
                }

                // AI 92 Activity FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActFKRefno))
                {
                    await this.AddScanMsg(sender, scanArgs, "Activity FK Ref#: " + scanArgs.ParseInfo.ActFKRefno);
                }

                // AI 93 Activity Detail FK Refno
                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActDetailFKRefno))
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Activity Detail FK Ref#: " + scanArgs.ParseInfo.ActDetailFKRefno);
                }

                if (!string.IsNullOrEmpty(scanArgs.ParseInfo.Remainder))
                {
                    await this.AddScanMsg(sender, scanArgs, "Unidentifiable Data");
                    await this.AddScanMsg(sender, scanArgs, scanArgs.ParseInfo.Remainder);
                }

                // Symbology ID
                await this.AddScanMsg(sender, scanArgs, "Symbology: " + scanArgs.ParseInfo.Symbology);

                // Symbology Prefix
                await this.AddScanMsg(sender, scanArgs, "Symbology Prefix: " + scanArgs.ParseInfo.SymbologyPrefix);

                // Length
                await this.AddScanMsg(
                    sender,
                    scanArgs,
                    string.Format("Length: {0} prefix excluded", scanArgs.ParseInfo.Length));

                // Raw Bar Code Data
                await this.AddScanMsg(sender, scanArgs, "Bar Code Raw");

                const int LineLength = 30;
                var rawDataLine = SysCommon.WordWrap2List(scanArgs.ParseInfo.Raw, LineLength);

                for (var i = 0; i < rawDataLine.Count; i++)
                {
                    await this.AddScanMsg(sender, scanArgs, rawDataLine[i]);
                }

                // <issue="13609" author="RD" date="2016/02/22">
                // All errors written to screen instead of throwing an error.
                if (this.Parse.HasErrors())
                {
                    if (this.Parse.Details.ErrorCount == 1)
                    {
                        await this.AddScanMsg(sender, scanArgs, "1 Error");
                    }
                    else
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            string.Format("{0} Errors", this.Parse.Details.ErrorCount));
                    }

                    var count = this.Parse.Details.ErrorCount;

                    foreach (var errorDetail in this.Parse.Details.ErrorDetails)
                    {
                        var lines = SysCommon.WordWrap2List(errorDetail.Exception.Message, LineLength);

                        for (var i = 0; i < lines.Count; ++i)
                        {
                            await this.AddScanMsg(sender, scanArgs, lines[i]);
                        }
                    }
                }
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
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerSSCC18(object sender, BarCodeScannerEventArgs scanArgs)
            {
                await this.AddScanMsg(sender, scanArgs, "SSCC-18: " + scanArgs.ParseInfo.SSCC18);

                if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    try
                    {
                        var skid = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                            scanArgs.ParseInfo.SSCC18);

                        if (skid.CurrSkid.SkidFKRefno > 0)
                        {
                            //// AddScanMsg(sender, BarCodeScanArgs, "Skid identified")
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Code: {0}", skid.CurrSkid.LogisticUnitSkidCode));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Refno: {0}", skid.CurrSkid.SkidFKRefno));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Number: {0}", skid.CurrSkid.SkidCode));

                            var locReadable = LocationMapTasks.GetReadableLocation(
                                skid.CurrSkid.LocMap);

                            var skidQty = string.Format(
                                "Qty: {0} {1} @ {2} {3}",
                                skid.CurrSkid.Quantity,
                                skid.CurrSkid.ProdInfo.UnitCode,
                                skid.CurrSkid.WeightQuantity,
                                skid.CurrSkid.ProdInfo.WeightUnitCode);

                            await this.AddScanMsg(sender, scanArgs, string.Format("Location: {0}", locReadable));
                            await this.AddScanMsg(sender, scanArgs, string.Format(
                                "Product: {0}",
                                ProductInfoTasks.GetCurrReadableProdBCUnit(skid.CurrSkid.ProdInfo)));
                            await this.AddScanMsg(sender, scanArgs, skidQty);
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Put Away: {0}", skid.CurrSkid.IsPutAway));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("On Hold: {0}", skid.CurrSkid.IsHold));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Cube: {0} {1}", skid.CurrSkid.Cube, skid.CurrSkid.CubeUnit));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Height: {0} {1}", skid.CurrSkid.Height, skid.CurrSkid.HeightUnit));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Ti/Hi: {0}/{1}", skid.CurrSkid.Ti, skid.CurrSkid.Hi));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("CHEP: {0}", skid.CurrSkid.IsCHEP));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Completed: {0}", skid.CurrSkid.IsCompleted));
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                string.Format("Consumed: {0}", skid.CurrSkid.IsConsumed));
                        }
                        else
                        {
                            throw new Exception("Not associated to a skid.");
                        }
                    }
                    catch (Exception)
                    {
                        await this.AddScanMsg(sender, scanArgs, "Skid unidentified.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier(object sender, BarCodeScannerEventArgs scanArgs)
            {
                await this.AddScanMsg(
                    sender,
                    scanArgs,
                    "Location identifier: " + scanArgs.ParseInfo.LocationIdentifier);

                var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                    scanArgs.ParseInfo.LocationIdentifier);

                if (locMapRefno > 0)
                {
                    var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                    await this.AddScanMsg(sender, scanArgs, string.Format("Warehouse code: {0}", locMap.WhsCode));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Warehouse refno: {0}", locMap.WhsFKRefno));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Warehouse name: {0}", locMap.WhsName));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Section code: {0}", locMap.SectCode));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Section refno: {0}", locMap.SectFKRefno));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Section name: {0}", locMap.SectName));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Slot code: {0}", locMap.SlotCode));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Slot refno: {0}", locMap.SlotFKRefno));
                    await this.AddScanMsg(sender, scanArgs, string.Format("Slot name: {0}", locMap.SlotName));
                }
                else
                {
                    await this.AddScanMsg(sender, scanArgs, "Not associated to a location.");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerScheme(
                object sender,
                BarCodeScannerEventArgs scanArgs,
                bool isSerialTracked,
                bool isProductionDateTracked,
                bool isLotTracked,
                bool isBestBeforeDateTracked,
                bool isVariantTracked,
                ProductInfo prodInfo)
            {
                await DisplayParsedSchemeData(
                    sender,
                    scanArgs,
                    isSerialTracked,
                    isProductionDateTracked,
                    isLotTracked,
                    isBestBeforeDateTracked,
                    isVariantTracked);

                await DisplayParsedWeights(sender, scanArgs);

                if (scanArgs.ParseInfo.LogisticUnitCountOfTradeItems != 0)
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Count of trade items: " + scanArgs.ParseInfo.LogisticUnitCountOfTradeItems);
                }

                if (scanArgs.ParseInfo.LogisticUnitVariableCount != 0)
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Variable Count: " + scanArgs.ParseInfo.LogisticUnitVariableCount);
                }

                if (scanArgs.ParseInfo.LengthOr1stDimensionMeters != 0)
                {
                    await this.AddScanMsg(
                        sender,
                        scanArgs,
                        "Length Meters: " + scanArgs.ParseInfo.LengthOr1stDimensionMeters);
                }

                async Task DisplayParsedWeights(object sender, BarCodeScannerEventArgs scanArgs)
                {
                    if (scanArgs.ParseInfo.NetWeightMetricKGFoundAI)
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            "Net weight metric KG: " + scanArgs.ParseInfo.NetWeightMetricKG);
                    }

                    if (scanArgs.ParseInfo.NetWeightNonMetricLBFoundAI)
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            "Net weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLB);
                    }

                    if (scanArgs.ParseInfo.GrossWeightMetricKGFoundAI)
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            "Gross weight metric KG: " + scanArgs.ParseInfo.GrossWeightMetricKG);
                    }

                    if (scanArgs.ParseInfo.GrossWeightNonMetricLBFoundAI)
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            "Gross weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLB);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task DisplayParsedSchemeData(
                    object sender,
                    BarCodeScannerEventArgs scanArgs,
                    bool isSerialTracked,
                    bool isProductionDateTracked,
                    bool isLotTracked,
                    bool isBestBeforeDateTracked,
                    bool isVariantTracked)
                {
                    if (scanArgs.ParseInfo.ManufacturerSerialNumber != 0)
                    {
                        await this.AddScanMsg(
                            sender,
                            scanArgs,
                            "Manufacturer Serial Number: " + scanArgs.ParseInfo.ManufacturerSerialNumber);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.SingleTradeItemSerialNumber))
                    {
                        if (isSerialTracked)
                        {
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                "(Tracked) Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                        else
                        {
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                "Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                    }
                    else
                    {
                        if (isSerialTracked)
                        {
                            await this.AddScanMsg(sender, scanArgs, "(Tracked) Serial # Missing");
                        }
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LogisticUnitBatchLotNumber))
                    {
                        if (isLotTracked)
                        {
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                "(Tracked) Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                        else
                        {
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                "Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                    }
                    else
                    {
                        if (isLotTracked)
                        {
                            await this.AddScanMsg(sender, scanArgs, "(Tracked) Batch / Lot Number Missing.");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitProductionDate))
                    {
                        if (isProductionDateTracked)
                        {
                            var msg = "(Tracked) Production Date: " +
                                      scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString();
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                msg);
                        }
                        else
                        {
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                "Production Date: " + scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString());
                        }
                    }
                    else
                    {
                        if (isProductionDateTracked)
                        {
                            await this.AddScanMsg(sender, scanArgs, "(Tracked) Production Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate))
                    {
                        if (isBestBeforeDateTracked)
                        {
                            var msg = "(Tracked) Best Before Date: " +
                                      scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString();
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                msg);
                        }
                        else
                        {
                            var msg = "Best Before Date: " +
                                      scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString();
                            await this.AddScanMsg(
                                sender,
                                scanArgs,
                                msg);
                        }
                    }
                    else
                    {
                        if (isBestBeforeDateTracked)
                        {
                            await this.AddScanMsg(sender, scanArgs, "(Tracked) Best Before Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitExpiryUseByDate))
                    {
                        var msg = "Expiry Date: " + scanArgs.ParseInfo.LogisticUnitExpiryUseByDate.ToShortDateString();
                        await this.AddScanMsg(sender, scanArgs, msg);
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitDueDate))
                    {
                        var msg = "Due Date: " + scanArgs.ParseInfo.LogisticUnitDueDate.ToShortDateString();
                        await this.AddScanMsg(sender, scanArgs, msg);
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitPackagingDate))
                    {
                        var msg = "Packaging Date: " + scanArgs.ParseInfo.LogisticUnitPackagingDate.ToShortDateString();
                        await this.AddScanMsg(sender, scanArgs, msg);
                    }

                    if (scanArgs.ParseInfo.LogisticUnitProductVariant != 0)
                    {
                        if (isVariantTracked)
                        {
                            var msg = "(Tracked) Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant;
                            await this.AddScanMsg(sender, scanArgs, msg);
                        }
                        else
                        {
                            var msg = "Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant;
                            await this.AddScanMsg(sender, scanArgs, msg);
                        }
                    }
                    else
                    {
                        if (isVariantTracked)
                        {
                            await this.AddScanMsg(sender, scanArgs, "(Tracked) Product variant missing.");
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<ProductInfo> BarCodeHandlerProductIdentifierRuleSumwaDuck(ProductInfo prodInfo)
            {
                // Do not look up gtin in bar code database for this rule, product will be identified by a range
                // of fixed weights.
                if (this.SysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct &&
                    this.Parse.ProductIdentifier == "90695955110022")
                {
                    SysShared.BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIDProduct(this.Parse);

                    prodInfo = await ProductInfoTasks.GetInfoFromCodeBCUnit(
                        this.Parse.ProductIdentifier,
                        string.Empty,
                        "CS");
                    this.ProductUnitRefno = prodInfo.UnitRefno;
                }

                return prodInfo;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<List<ProductUnitBarcodeQueryResult>> FindProductUnit(SLocMap currLocMap, string prodIdentifier)
            {
                var queryProductUnitBarcode = "select tProductUnit.ProductUnitRefno,"
                                              + "tProductUnitBarCode.ProductUnitBarCodeRefno,"
                                              + "tLocationMapWarehouseProductScheme.ProductSchemeRefno AS WarehouseProductSchemeRefno,"
                                              + "tProduct.ProductSchemeRefno,"
                                              + "tProduct.ProductCode,"
                                              + "tProduct.ProductDescription,"
                                              + "tProductUnit.ProductUnitBC,"
                                              + "tUnit.UnitCode "
                                              + "FROM tProductUnitBarCode "
                                              + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno "
                                              + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                              + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                              + "INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno "
                                              + "WHERE tProductUnitBarCode.ProductUnitBarCodeCode = ? "
                                              + "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = ? "
                                              + "AND tProductUnitBarCode.ProductUnitBarCodeModifier1 = ?";

                var resultsProductUnitBarcode =
                    await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnitBarcodeQueryResult>(
                        queryProductUnitBarcode,
                        prodIdentifier,
                        currLocMap.WhsLocMapRefno,
                        this.SysEnv.CurrentBarCodeModifier1);
                return resultsProductUnitBarcode;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task DisplayProductAssocInfo(
                object sender,
                BarCodeScannerEventArgs scanArgs,
                List<ProductUnitBarcodeQueryResult> resultsProductUnitBarcode)
            {
                // If product was found associated display it.  'ProductID+BC+UOM'
                await this.AddScanMsg(sender, scanArgs, "Product associated to bar code");

                var prodBCUnit = resultsProductUnitBarcode[0].ProductCode.ToString() + " " +
                                 resultsProductUnitBarcode[0].ProductUnitBC.ToString() + " " +
                                 resultsProductUnitBarcode[0].UnitCode.ToString();

                await this.AddScanMsg(sender, scanArgs, prodBCUnit);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblProductData.TextColor = Color.Black;
                    this.lblProductData.Text = prodBCUnit;
                });

                await this.AddScanMsg(
                    sender,
                    scanArgs,
                    resultsProductUnitBarcode[0].ProductDescription.ToString());
            }
        }

        /// <summary>
        /// Bar code modifier change made event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private void BarCodeModifierChangesMade(object sender, EventArgs e)
        {
            try
            {
                this.IsChangesMade = true;
                this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        /// Assign product click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
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
                        await AssignProduct(sender);
                    }
                    else
                    {
                        // Just return without a message because the user will
                        // already see a permission denied if they failed to enter override code.
                    }
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
                Device.BeginInvokeOnMainThread(async () =>
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
                    this.ScanHasData = false;
                });

                await this.ScannerEnable();
            }

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
            async Task AssignProduct(object sender)
            {
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXML();
                    this.ProductUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                    if (this.ProductUnitRefno != 0)
                    {
                        await AssocBarCodeToProduct();
                        var productUnit = await FindAssocProductInfo();
                        await DisplayProductAssocData(productUnit);
                    }
                }

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
                    var queryProductUnit = "SELECT tProduct.ProductCode,"
                                           + "tProductUnit.ProductUnitBC,"
                                           + "tUnit.UnitCode "
                                           + "FROM tProductUnit "
                                           + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                           + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                           + "WHERE tProductUnit.ProductUnitRefno = ?";

                    var productUnit = await DatabaseConnection.GetInstance().SQLConn.
                        QueryAsync<ProductUnit>(queryProductUnit, this.ProductUnitRefno);
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
                    await this.ActItemBarCode.Insert(xmlDoc, true);

                    _ = await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems[0] = "Product associated to bar code");
                }
            }
        }
    }
}