// <copyright file="ActivityInventoryInquiry.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using dMobile.Controller.Activity;
    using Controller.Activity_Item;
    using Resources.Enums;
    using Resources.Services;
    using Resources.Types;
    using Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Location = Resources.Types.Location;

    /// <summary>
    /// Activity inventory inquiry.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityInventoryInquiry : IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityInventoryInquiry"/> class.
        /// </summary>
        public ActivityInventoryInquiry()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;

            // Initialize activity header instance.
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.Activity.SkidInfo ??= new SysSkidInfo();

            // Initialize activity item instance.
            this.ActItem = ActItemFacade.GetActItem(
                this.Activity,
                SysCommon.ActSubTypes.InventoryInquiry);
            this.ActItem.ProdInfo = new ProductInfo();
            this.ActItem.LocMap = this.LocMap.CurrLocMap;
            this.ActItem.SysEnv = this.SysEnv;

            this.InitializeBarCodeModifierControl();

            // Set control texts.
            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";
            this.Title = this.GetControlDescription();
            this.IsFirstAppearance = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityInventoryInquiry"/> class.
        /// </summary>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product information instance.</param>
        public ActivityInventoryInquiry(ProductInfo prodInfo)
        {
            this.InitializeComponent();

            // Initialize
            this.IsIndicatorRunning = false;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;

            // Initialize activity header instance.
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.Activity.SkidInfo ??= new SysSkidInfo();

            // Initialize activity item instance.
            this.ActItem = ActItemFacade.GetActItem(
                this.Activity,
                SysCommon.ActSubTypes.InventoryInquiry);
            this.ActItem.ProdInfo = prodInfo;
            this.ActItem.LocMap = this.LocMap.CurrLocMap;
            this.ActItem.SysEnv = this.SysEnv;

            this.InitializeBarCodeModifierControl();

            // Set control texts.
            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";
            this.cmdSelectProduct.Text = prodInfo.Code;
            this.Title = this.GetControlDescription();

            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            this.cmdLocation.IsEnabled = true;
            this.cmdSelectProduct.IsEnabled = true;
            this.cmdDisplayDefault.IsEnabled = true;
            this.BarCodeModifier1Picker.IsEnabled = true;
            this.IsFirstAppearance = true;
        }

        /// <summary>
        /// Gets or sets the bar code parse information.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        /// Gets or sets the scanner engine instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the activity item.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        /// Gets or sets the activity header.
        /// </summary>
        private Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        private List<object> ListItems { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the data grid main extra information items.
        /// </summary>
        private ObservableCollection<object> DataGridMainExtraInfoItems { get; set; } =
            new ObservableCollection<object>();

        /// <summary>
        ///     Gets or sets a value indicating whether this is the first appearance of the form.
        /// </summary>
        private bool IsFirstAppearance { get; set; }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered &&
                this.Scanner != null)
            {
                await this.Scanner.EnableScanning();
                this.Scanner.BarCodeScan +=
                    new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -=
                    new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.DisableScanning();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> The control description.</returns>
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
        }

        /// <summary>
        /// Get the activity result.
        /// </summary>
        /// <returns><see cref="XDocument"/> XML data.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// Check if is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(
                    async () => await this.DisplayAlert(
                        "Inventory Inquiry",
                        "Quantity inquiry is not supported in disconnected mode.",
                        "OK"));
            }

            return isReadyToSave;
        }

        /// <summary>
        /// Check if the form is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if ready to close, else false.</returns>
        public Task<bool> IsReadyToClose()
        {
            const bool isReadyToClose = true;
            return Task.FromResult(isReadyToClose);
        }

        /// <summary>
        /// Save changes, launch the inventory inquiry summary screen.
        /// </summary>
        /// <returns><see cref="bool"/> True for success, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            try
            {
                await this.ScannerDisable();
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityInventoryInquirySummary,
                    this.ActItem,
                    this.ActSubType);
                _ = await form.DisplayControl.ShowDialog();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return false;
        }

        /// <summary>
        /// Cancel changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if cancelled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                cancelChanges = false;
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        dMobile.Resources.Properties.Resources.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                    return Task.CompletedTask;
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// Location click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        public async void CmdLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(
                    EDialogTypes.SelectLocationMap,
                    SysCommon.ActSubTypes.NotApplicable);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                    {
                        this.LocMap.CurrLocMap =
                            await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                this.LocMap.LoginWhsLocMapRefno);
                    }

                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.LocMap.CurrLocMap =
                        await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            this.LocMap.LoginWhsLocMapRefno);
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }

                this.ActItem.LocMap = this.LocMap.CurrLocMap;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        "OK"));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     Select product click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        public async void CmdSelectProduct_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                var form = new FormDialogContainer(
                    EDialogTypes.SelectProduct,
                    SysCommon.ActSubTypes.NotApplicable);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlArgs = await form.GetXml();
                    var prodInfo = await ProductInfoTasks.GetInfoFromRefno(
                        Convert.ToInt32(xmlArgs.Root?.Element("ProductRefno")?.Value));
                    this.cmdSelectProduct.Text = prodInfo.Code;
                    this.ActItem.ProdInfo = prodInfo;
                }
                else
                {
                    this.cmdSelectProduct.Text = "Select Product";
                    this.ActItem.ProdInfo = new ProductInfo();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        "OK"));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// The on appearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();
            this.dataGridMain.ItemsSource = this.DataGridMainExtraInfoItems;
            if (this.IsFirstAppearance)
            {
                this.IsFirstAppearance = false;
                await this.ScannerEnable();
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    KeyUpHandlerCallback);
                return;

                ///////////////////////////////////////////////////////////
                async void KeyUpHandlerCallback(string keyPressed)
                {
                    try
                    {
                        await this.ScannerDisable();
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.Enter)
                            {
                                if (await this.SaveChanges())
                                {
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                            }
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "OK"));
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
            }
        }

        /// <summary>
        /// The on disappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc/>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonCallback()
            {
                if (await this.CancelChanges())
                {
                    await this.ScannerDisable();
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
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
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            "OK"));
                    throw new TaskCanceledException();
                }

                this.BarParse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new Exception("Scanning an activity reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new Exception(
                        "Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    var prodUnitRefno = 0;
                    prodUnitRefno = await SysShared.IdentifyProductScanned(
                        this.ActSubType,
                        this.BarParse);

                    if (prodUnitRefno != 0)
                    {
                        var tempProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        this.ActItem.ProdInfo = tempProdInfo;

                        // <issue="13609" author="RD" date="2016/02/22"/>
                        SysShared.AssertSuppressedErrorsAgainstScheme(
                            this.ActItem.ProdInfo,
                            this.BarParse);
                        _ = await Device.InvokeOnMainThreadAsync(
                            () => this.cmdSelectProduct.Text = this.ActItem.ProdInfo.Code);
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(
                            () => this.cmdSelectProduct.Text = "Select Product");
                    }
                }
                else if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new Exception(
                        "Scanning a shipping container bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                            this.BarParse.LocationIdentifier));
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(
                            () => this.cmdLocation.Text =
                                LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(
                            () => this.cmdLocation.Text = "Set Location");
                    }
                }
                else
                {
                    throw new Exception("Scanned bar code type is not supported here.");
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
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
        /// Initialize bar code modifier control.
        /// </summary>
        private void InitializeBarCodeModifierControl()
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

            this.BarCodeModifier1Picker.SelectedItem =
                Convert.ToString(this.SysEnv.CurrentBarCodeModifier1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        private void DesignDataGridProductOnly()
        {
            this.DataGridMainExtraInfoItems = new ObservableCollection<object>();

            foreach (ProductLocationMapType location in this.ListItems.Cast<ProductLocationMapType>())
            {
                this.DataGridMainExtraInfoItems.Add(location);
            }

            var labelProductCodeHeader = new Label
            {
                Text = "Product ID", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelLocationMapTypeHeader = new Label
            {
                Text = "Default", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelProductDescriptionHeader = new Label
            {
                Text = "Description", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelProductBrandDescriptionHeader = new Label
            {
                Text = "Brand", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var columnWidth0 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelProductCodeHeader.Text);
            var columnWidth1 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelLocationMapTypeHeader.Text);
            var columnWidth2 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelProductDescriptionHeader.Text);
            var columnWidth3 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelProductBrandDescriptionHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelProductCodeHeader);
            gridHeader.Children.Add(labelLocationMapTypeHeader);
            gridHeader.Children.Add(labelProductDescriptionHeader);
            gridHeader.Children.Add(labelProductBrandDescriptionHeader);

            Grid.SetColumn(labelProductCodeHeader, 0);
            Grid.SetColumn(labelLocationMapTypeHeader, 1);
            Grid.SetColumn(labelProductDescriptionHeader, 2);
            Grid.SetColumn(labelProductBrandDescriptionHeader, 3);

            Grid.SetRow(gridHeader, 0);

            this.dataGridMainHeader.Children.Add(gridHeader);

            foreach (var location in this.DataGridMainExtraInfoItems.Cast<ProductLocationMapType>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(location.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(location.LocationMapType);
                var detailWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(location.ProductDescription);
                var detailWidth3 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(location.ProductBrandDescription);

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            // Data grid
            var dataTemplate = new DataTemplate(
                () =>
                {
                    // Header grid
                    var gridData = new Grid()
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                        }
                    };

                    var labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelProductDescription = new Label
                        { VerticalOptions = LayoutOptions.Center };
                    var labelProductBrandDescription = new Label
                        { VerticalOptions = LayoutOptions.Center };

                    labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                    labelLocationMapType.SetBinding(
                        Label.TextProperty,
                        new Binding("LocationMapType"));
                    labelProductDescription.SetBinding(
                        Label.TextProperty,
                        new Binding("ProductDescription"));
                    labelProductBrandDescription.SetBinding(
                        Label.TextProperty,
                        new Binding("ProductBrandDescription"));

                    gridData.Children.Add(labelProductCode);
                    gridData.Children.Add(labelLocationMapType);
                    gridData.Children.Add(labelProductDescription);
                    gridData.Children.Add(labelProductBrandDescription);

                    Grid.SetColumn(labelProductCode, 0);
                    Grid.SetColumn(labelLocationMapType, 1);
                    Grid.SetColumn(labelProductDescription, 2);
                    Grid.SetColumn(labelProductBrandDescription, 3);

                    return new ViewCell { View = gridData };
                });

            this.dataGridMain.ItemTemplate = dataTemplate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        private void DesignDataGridLocationOnly()
        {
            this.DataGridMainExtraInfoItems = new ObservableCollection<object>();

            foreach (Location location in this.ListItems.Cast<Location>())
            {
                this.DataGridMainExtraInfoItems.Add(location);
            }

            var labelLocationMapWarehouseCodeHeader = new Label
            {
                Text = "Whs", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelLocationMapSectionNameHeader = new Label
            {
                Text = "Section", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelLocationMapSlotNameHeader = new Label
            {
                Text = "Slot", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            var labelLocationMapTypeHeader = new Label
            {
                Text = "Default", VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };

            var columnWidth0 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelLocationMapWarehouseCodeHeader.Text);
            var columnWidth1 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelLocationMapSectionNameHeader.Text);
            var columnWidth2 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelLocationMapSlotNameHeader.Text);
            var columnWidth3 = DependencyService.Get<IGridService>()
                .CalculateTextWidth(labelLocationMapTypeHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition
                        { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelLocationMapWarehouseCodeHeader);
            gridHeader.Children.Add(labelLocationMapSectionNameHeader);
            gridHeader.Children.Add(labelLocationMapSlotNameHeader);
            gridHeader.Children.Add(labelLocationMapTypeHeader);

            Grid.SetColumn(labelLocationMapWarehouseCodeHeader, 0);
            Grid.SetColumn(labelLocationMapSectionNameHeader, 1);
            Grid.SetColumn(labelLocationMapSlotNameHeader, 2);
            Grid.SetColumn(labelLocationMapTypeHeader, 3);

            Grid.SetRow(gridHeader, 0);

            this.dataGridMainHeader.Children.Add(gridHeader);

            foreach (dynamic location in this.DataGridMainExtraInfoItems)
            {
                double detailWidth0 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(location.LocationMapWarehouseCode));
                double detailWidth1 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(location.LocationMapSectionName));
                double detailWidth2 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(location.LocationMapSlotName));
                double detailWidth3 = DependencyService.Get<IGridService>()
                    .CalculateTextWidth(Convert.ToString(location.LocationMapType));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }
            }

            // Data grid
            var dataTemplate = new DataTemplate(
                () =>
                {
                    // Header grid
                    var gridData = new Grid()
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition
                                { Width = new GridLength(columnWidth3, GridUnitType.Absolute) }
                        }
                    };

                    var labelLocationMapWarehouseCode = new Label
                        { VerticalOptions = LayoutOptions.Center };
                    var labelLocationMapSectionName = new Label
                        { VerticalOptions = LayoutOptions.Center };
                    var labelLocationMapSlotName = new Label
                        { VerticalOptions = LayoutOptions.Center };
                    var labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };

                    labelLocationMapWarehouseCode.SetBinding(
                        Label.TextProperty,
                        new Binding("LocationMapWarehouseCode"));
                    labelLocationMapSectionName.SetBinding(
                        Label.TextProperty,
                        new Binding("LocationMapSectionName"));
                    labelLocationMapSlotName.SetBinding(
                        Label.TextProperty,
                        new Binding("LocationMapSlotName"));
                    labelLocationMapType.SetBinding(
                        Label.TextProperty,
                        new Binding("LocationMapType"));

                    gridData.Children.Add(labelLocationMapWarehouseCode);
                    gridData.Children.Add(labelLocationMapSectionName);
                    gridData.Children.Add(labelLocationMapSlotName);
                    gridData.Children.Add(labelLocationMapType);

                    Grid.SetColumn(labelLocationMapWarehouseCode, 0);
                    Grid.SetColumn(labelLocationMapSectionName, 1);
                    Grid.SetColumn(labelLocationMapSlotName, 2);
                    Grid.SetColumn(labelLocationMapType, 3);

                    return new ViewCell { View = gridData };
                });

            this.dataGridMain.ItemTemplate = dataTemplate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetDataViewProductOnlyPickingQuery()
        {
            var sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked &&
                this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode," +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE tProductLocationMapType.LocationMapRefno = " +
                      this.LocMap.CurrLocMapRefno +
                      " AND tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Picking;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked &&
                     this.LocMap.CurrLocMap.SlotLocMapRefno <= 0)
            {
                // find all the slots for the selected section
                var sub = "(SELECT tLocationMapSlot.LocationMapRefno " +
                          "FROM tLocationMapSlot " +
                          "WHERE (tLocationMapSlot.SectionLocationMapRefno = " +
                          this.LocMap.CurrLocMapRefno +
                          "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode," +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE (tProductLocationMapType.LocationMapRefno IN " +
                      sub +
                      ") " +
                      "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Picking +
                      ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked &&
                     !this.LocMap.CurrLocMap.IsSlotTracked &&
                     this.LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode, " +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE tProductLocationMapType.LocationMapRefno = " +
                      this.LocMap.CurrLocMapRefno +
                      " AND tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Picking;
            }

            if (this.ActItem.ProdInfo.Refno > 0)
            {
                sql += " AND tProductLocationMapType.ProductRefno = " +
                       this.ActItem.ProdInfo.Refno;
            }

            return sql;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetDataViewProductOnlyReceivingQuery()
        {
            var sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked &&
                this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode," +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE tProductLocationMapType.LocationMapRefno = " +
                      this.LocMap.CurrLocMapRefno +
                      " AND tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Receiving;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked &&
                     this.LocMap.CurrLocMap.SlotLocMapRefno <= 0)
            {
                // find all the slots for the selected section
                var sub = "(SELECT tLocationMapSlot.LocationMapRefno " +
                          "FROM tLocationMapSlot " +
                          "WHERE (tLocationMapSlot.SectionLocationMapRefno = " +
                          this.LocMap.CurrLocMapRefno +
                          "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode," +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE (tProductLocationMapType.LocationMapRefno IN " +
                      sub +
                      ") " +
                      "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Receiving +
                      ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked &&
                     !this.LocMap.CurrLocMap.IsSlotTracked &&
                     this.LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode," +
                      "tProduct.ProductDescription," +
                      "tProduct.ProductBrandDescription " +
                      "FROM tProductLocationMapType " +
                      "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno " +
                      "WHERE tProductLocationMapType.LocationMapRefno = " +
                      this.LocMap.CurrLocMapRefno +
                      " AND tProductLocationMapType.ProductLocationMapTypeEnum = " +
                      (int)SysCommon.ProdLocMapTypes.Receiving;
            }

            if (this.ActItem.ProdInfo.Refno > 0)
            {
                sql += " AND tProductLocationMapType.ProductRefno = " +
                       this.ActItem.ProdInfo.Refno;
            }

            return sql;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewProductOnly()
        {
            var listLocationMapTypes = new List<ProductLocationMapType>();
            var queryPickingOnly = this.GetDataViewProductOnlyPickingQuery();

            if (!string.IsNullOrEmpty(queryPickingOnly))
            {
                var resultsPickingProductLocationMapType = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<ProductLocationMapType>(queryPickingOnly);

                if (resultsPickingProductLocationMapType?.Count > 0)
                {
                    foreach (var item in resultsPickingProductLocationMapType)
                    {
                        item.LocationMapType = "Picking";
                    }

                    listLocationMapTypes.AddRange(resultsPickingProductLocationMapType);
                }
            }

            var queryReceivingOnly = this.GetDataViewProductOnlyReceivingQuery();

            if (!string.IsNullOrEmpty(queryReceivingOnly))
            {
                var resultsReceivingProductLocationMapType = await DatabaseConnection.GetInstance()
                    .SqlConn.QueryAsync<ProductLocationMapType>(queryReceivingOnly);

                if (resultsReceivingProductLocationMapType?.Count > 0)
                {
                    foreach (var item in resultsReceivingProductLocationMapType)
                    {
                        item.LocationMapType = "Receiving";
                    }

                    listLocationMapTypes.AddRange(resultsReceivingProductLocationMapType);
                }
            }

            listLocationMapTypes = listLocationMapTypes.OrderBy(obj => obj.ProductCode)
                .ThenBy(obj => obj.LocationMapType)
                .ToList();
            this.ListItems = listLocationMapTypes.Cast<object>().ToList();

            ////this.ListItems = (List<object>)listLocationMapTypes
            ////    .OrderBy(locMapType => locMapType.ProductCode)
            ////    .ThenBy(locMapType => locMapType.LocationMapType);
            return this.ListItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewLocationOnly()
        {
            var locations = new List<Location>();
            var locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                this.LocMap.CurrLocMap.WhsLocMapRefno,
                this.ActItem.ProdInfo.Refno,
                SysCommon.ProdLocMapTypes.Picking);
            var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            var pickingLocation = new Location()
            {
                LocationMapWarehouseCode = locMap.WhsCode,
                LocationMapSectionName = locMap.SectName,
                LocationMapSlotName = locMap.SlotName,
                LocationMapType = "Picking"
            };

            locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                this.LocMap.CurrLocMap.WhsLocMapRefno,
                this.ActItem.ProdInfo.Refno,
                SysCommon.ProdLocMapTypes.Receiving);
            locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            var receivingLocation = new Location()
            {
                LocationMapWarehouseCode = locMap.WhsCode,
                LocationMapSectionName = locMap.SectName,
                LocationMapSlotName = locMap.SlotName,
                LocationMapType = "Receiving"
            };

            locations.Add(pickingLocation);
            locations.Add(receivingLocation);

            // this.DataViewItems = dataTableItems.DefaultView;
            // this.DataViewItems.Table.TableName = this.MainGridMappingName;
            // this.DataViewItems.AllowNew = false;
            // this.DataViewItems.AllowEdit = false;
            // this.DataViewItems.AllowDelete = false;
            // this.DataViewItems.Sort = "LocationMapWarehouseCode,LocationMapSectionName,LocationMapSlotName,LocationMapType";
            locations = locations.OrderBy(obj => obj.LocationMapWarehouseCode)
                .ThenBy(obj => obj.LocationMapSectionName)
                .ThenBy(obj => obj.LocationMapSlotName)
                .ThenBy(obj => obj.LocationMapType)
                .ToList();
            this.ListItems = locations.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CmdDisplayDefault_Click(object sender, EventArgs e)
        {
            await this.DisplayLocationDefaults();
        }

        /// <summary>
        ///
        /// </summary>
        private async Task DisplayLocationDefaults()
        {
            await Task.Run(
                async () =>
                {
                    try
                    {
                        if (this.ActItem.ProdInfo.Refno == 0)
                        {
                            this.ActItem.ListItems = await this.GetDataViewLocationOnly();

                            await Device.InvokeOnMainThreadAsync(
                                () =>
                                {
                                    this.DesignDataGridLocationOnly();
                                    this.dataGridMain.ItemsSource = this.DataGridMainExtraInfoItems;
                                });
                        }
                        else
                        {
                            // A warehouse only query will result in to much data. Restrict it to at least a section.
                            if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                            {
                                throw new Exception(
                                    "The set location must have a warehouse selected.");
                            }

                            if (this.LocMap.CurrLocMap.IsSectionTracked &&
                                this.LocMap.CurrLocMap.SectLocMapRefno <= 0)
                            {
                                throw new Exception(
                                    "The set location must have a section selected.");
                            }

                            this.ActItem.ListItems = await this.GetDataViewProductOnly();

                            await Device.InvokeOnMainThreadAsync(
                                () =>
                                {
                                    this.DesignDataGridProductOnly();
                                    this.dataGridMain.ItemsSource = this.DataGridMainExtraInfoItems;
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
                            {
                                var errRpt = new FormErrorReport(this.ActSubType, ex);
                                _ = await errRpt.ShowDialog();
                            });
                    }
                });
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 =
                Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }

        /// <summary>
        ///     View button click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        private async void CmdView_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}