// <copyright file="ActivityInventoryInquiry.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityInventoryInquiry : DMobileModal, IAppArea
    {

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityInventoryInquiry"/> class.
        /// </summary>
        public ActivityInventoryInquiry()
        {
            this.InitializeComponent();

            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

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

                this.BarCodeModifier1Picker.SelectedItem = Convert.ToString(this.SysEnv.CurrentBarCodeModifier1);

                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquiry);
                this.ActItem.ProdInfo = new ProductInfo();
                this.ActItem.LocMap = this.LocMap;
                this.ActItem.SysEnv = this.SysEnv;

                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
/// <summary>
/// Initializes a new instance of the <see cref="ActivityInventoryInquiry"/> class.
/// </summary>
/// <param name="prodInfo"></param>
        public ActivityInventoryInquiry(ProductInfo prodInfo)
        {
            this.InitializeComponent();

            this.cmdSelectProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.cmdDisplayDefault.Text = "Display Defaults";
            this.cmdView.Text = "View";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();

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

                this.BarCodeModifier1Picker.SelectedItem = Convert.ToString(this.SysEnv.CurrentBarCodeModifier1);

                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquiry;
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquiry);
                this.ActItem.ProdInfo = prodInfo;
                this.ActItem.LocMap = this.LocMap;
                this.ActItem.SysEnv = this.SysEnv;

                this.cmdSelectProduct.Text = prodInfo.Code;
                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.dataGridMain.ItemsSource = this.DataGridMainExtraInfoItems;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
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
            });

            try
            {
                await this.ucActivityInventoryInquiry_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucActivityInventoryInquiry_GotFocus threw an error " + ex, "OK"));
            }
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

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                _ = await this.CancelChanges();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        private bool IsControlLoaded { get; set; } = false;

        private BarCodeParse BarParse { get; set; }

        private BarCodeScanner Scanner { get; set; }

        private ActItem ActItem { get; set; }

        private LocationMap LocMap { get; set; }

        private bool IsEnterTrapped { get; set; } = false;

        private SysEnvironment SysEnv { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        private Activity Activity { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private string MainGridMappingName { get; set; } = "tLocationMapDefault";

        private List<object> ListItems { get; set; } = new List<object>();

        public ObservableCollection<object> DataGridMainExtraInfoItems { get; set; } = new ObservableCollection<object>();

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
        public string GetControlDescription()
        {
            return this.Activity.GetControlDescription();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Inventory Inquiry", "Quantity inquiry is not supported in disconnected mode.", "OK"));
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

            if (isReadyToClose)
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

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            try
            {
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(EDialogTypes.ActivityInventoryInquirySummary, this.ActItem, this.ActSubType);
                _ = await form.DisplayControl.ShowDialog();
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

            return false;
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
                    // this.ProdUnitRefno = 0;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                await LocationMapTasks.AssignLocationMapAsync();

                if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
            }
            else
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            }

            this.ActItem.LocMap = this.LocMap;

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSelectProduct_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            ProductInfo prodInfo = new ProductInfo();
            FormDialogContainer form = new FormDialogContainer(EDialogTypes.SelectProduct, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlArgs = await form.GetXML();
                prodInfo = await ProductInfoTasks.GetInfoFromRefno(Convert.ToInt32(xmlArgs.Root.Element("ProductRefno").Value));
                this.cmdSelectProduct.Text = prodInfo.Code;
            }
            else
            {
                this.cmdSelectProduct.Text = "Select Product";
            }

            this.ActItem.ProdInfo = prodInfo;
            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void tmrActivityInventoryInquiry_Tick(object sender, EventArgs e)
        {
            this.IsControlLoaded = true;

            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            this.cmdLocation.IsEnabled = true;
            this.cmdSelectProduct.IsEnabled = true;
            this.cmdDisplayDefault.IsEnabled = true;
            this.BarCodeModifier1Picker.IsEnabled = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs"></param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(async () =>
            {
                this.Loading = true;
                this.BarParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                try
                {
                    if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                    {
                        throw new Exception("Scanning an activity reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                    {
                        throw new Exception("Scanning an activity item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                    {
                        int prodUnitRefno = 0;
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarParse, prodUnitRefno);

                        if (prodUnitRefno != 0)
                        {
                            ProductInfo tempProdInfo = new ProductInfo();
                            tempProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ActItem.ProdInfo = tempProdInfo;
                            // <issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(this.ActItem.ProdInfo, this.BarParse);
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectProduct.Text = this.ActItem.ProdInfo.Code);
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectProduct.Text = "Select Product");
                        }
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                    {
                        throw new Exception("Scanning a shipping container bar code is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarParse.LocationIdentifier));

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
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
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task ucActivityInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            this.tmrActivityInventoryInquiry_Tick(sender, e);
            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        private void DesignDataGridProductOnly()
        {
            this.DataGridMainExtraInfoItems = new ObservableCollection<object>();

            foreach (ProductLocationMapType location in this.ListItems)
            {
                // ProductLocationMapType location = new ProductLocationMapType
                // {
                //    ProductCode = Convert.ToString(row["ProductCode"]),
                //    LocationMapType = Convert.ToString(row["LocationMapType"]),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"])
                // };
                this.DataGridMainExtraInfoItems.Add(location);
            }

            Label labelProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapTypeHeader = new Label { Text = "Default", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductDescriptionHeader = new Label { Text = "Description", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductBrandDescriptionHeader = new Label { Text = "Brand", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapTypeHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductDescriptionHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductBrandDescriptionHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                },
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

            foreach (ProductLocationMapType location in this.DataGridMainExtraInfoItems)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(location.LocationMapType);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductDescription);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(location.ProductBrandDescription);

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
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    },
                };

                Label labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductDescription = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductBrandDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelLocationMapType.SetBinding(Label.TextProperty, new Binding("LocationMapType"));
                labelProductDescription.SetBinding(Label.TextProperty, new Binding("ProductDescription"));
                labelProductBrandDescription.SetBinding(Label.TextProperty, new Binding("ProductBrandDescription"));

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

            foreach (Location location in this.ListItems)
            {
                // var location = new
                // {
                //    LocationMapWarehouseCode = Convert.ToInt32(row["LocationMapWarehouseCode"]),
                //    LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                //    LocationMapSlotName = Convert.ToString(row["LocationMapSlotName"]),
                //    LocationMapType = Convert.ToString(row["LocationMapType"])
                // };
                this.DataGridMainExtraInfoItems.Add(location);
            }

            Label labelLocationMapWarehouseCodeHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapSectionNameHeader = new Label { Text = "Section", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapSlotNameHeader = new Label { Text = "Slot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelLocationMapTypeHeader = new Label { Text = "Default", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapWarehouseCodeHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapSectionNameHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapSlotNameHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelLocationMapTypeHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                },
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
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapWarehouseCode));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapSectionName));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapSlotName));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(location.LocationMapType));

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
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    },
                };

                Label labelLocationMapWarehouseCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapSectionName = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapSlotName = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelLocationMapType = new Label { VerticalOptions = LayoutOptions.Center };

                labelLocationMapWarehouseCode.SetBinding(Label.TextProperty, new Binding("LocationMapWarehouseCode"));
                labelLocationMapSectionName.SetBinding(Label.TextProperty, new Binding("LocationMapSectionName"));
                labelLocationMapSlotName.SetBinding(Label.TextProperty, new Binding("LocationMapSlotName"));
                labelLocationMapType.SetBinding(Label.TextProperty, new Binding("LocationMapType"));

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
            string sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SlotLocMapRefno <= 0)
            {
                // find all the slots for the selected section
                string sub = "(SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE (tLocationMapSlot.SectionLocationMapRefno = " + this.LocMap.CurrLocMapRefno + "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE (tProductLocationMapType.LocationMapRefno IN " + sub + ") "
                    + "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking
                    + ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked && !this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode, "
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Picking;
            }

            return sql;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetDataViewProductOnlyReceivingQuery()
        {
            string sql = string.Empty;

            if (this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving;
            }
            else if (this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SlotLocMapRefno <= 0)
            {
                // find all the slots for the selected section
                string sub = "(SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE (tLocationMapSlot.SectionLocationMapRefno = " + this.LocMap.CurrLocMapRefno + "))";

                // find all the products for the selected section slot locations
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE (tProductLocationMapType.LocationMapRefno IN " + sub + ") "
                    + "AND (tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving
                    + ")";
            }
            else if (this.LocMap.CurrLocMap.IsSectionTracked && !this.LocMap.CurrLocMap.IsSlotTracked && this.LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                // find all the products for the selected slot location
                sql = "SELECT tProduct.ProductCode,"
                    + "tProduct.ProductDescription,"
                    + "tProduct.ProductBrandDescription "
                    + "FROM tProductLocationMapType "
                    + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductLocationMapType.ProductRefno "
                    + "WHERE tProductLocationMapType.LocationMapRefno = " + this.LocMap.CurrLocMapRefno
                    + " AND tProductLocationMapType.ProductLocationMapTypeEnum = " + (int)SysCommon.ProdLocMapTypes.Receiving;
            }

            return sql;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewProductOnly()
        {
            List<ProductLocationMapType> listLocationMapTypes = new List<ProductLocationMapType>();
            string queryPickingOnly = this.GetDataViewProductOnlyPickingQuery();

            if (!string.IsNullOrEmpty(queryPickingOnly))
            {
                List<ProductLocationMapType> resultsPickingProductLocationMapType = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductLocationMapType>(queryPickingOnly);

                if (resultsPickingProductLocationMapType?.Count > 0)
                {
                    listLocationMapTypes.AddRange(resultsPickingProductLocationMapType);

                    // foreach (ProductLocationMapType productLocationMapType in resultsPickingProductLocationMapType)
                    // {
                    //    DataRow row = dataTableItems.NewRow();
                    //    row["ProductCode"] = Convert.ToString(drProdLocType["ProductCode"]);
                    //    row["LocationMapType"] = "Picking";
                    //    row["ProductBrandDescription"] = Convert.ToString(drProdLocType["ProductBrandDescription"]);
                    //    row["ProductDescription"] = Convert.ToString(drProdLocType["ProductDescription"]);
                    //    dataTableItems.Rows.Add(row);
                    // }
                }
            }

            string queryReceivingOnly = this.GetDataViewProductOnlyReceivingQuery();

            if (!string.IsNullOrEmpty(queryReceivingOnly))
            {
                List<ProductLocationMapType> resultsReceivingProductLocationMapType = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductLocationMapType>(queryReceivingOnly);

                if (resultsReceivingProductLocationMapType?.Count > 0)
                {
                    listLocationMapTypes.AddRange(resultsReceivingProductLocationMapType);

                    // while (drProdLocType.Read())
                    // {
                    //    DataRow row = dataTableItems.NewRow();
                    //    row["ProductCode"] = Convert.ToString(drProdLocType["ProductCode"]);
                    //    row["LocationMapType"] = "Receiving";
                    //    row["ProductBrandDescription"] = Convert.ToString(drProdLocType["ProductBrandDescription"]);
                    //    row["ProductDescription"] = Convert.ToString(drProdLocType["ProductDescription"]);
                    //    dataTableItems.Rows.Add(row);
                    // }
                }
            }

            // this.DataViewItems = dataTableItems.DefaultView;
            // this.DataViewItems.Table.TableName = this.MainGridMappingName;
            // this.DataViewItems.AllowNew = false;
            // this.DataViewItems.AllowEdit = false;
            // this.DataViewItems.AllowDelete = false;
            // this.DataViewItems.Sort = "ProductCode,LocationMapType";
            this.ListItems = (List<object>)listLocationMapTypes.OrderBy(locMapType => locMapType.ProductCode).ThenBy(locMapType => locMapType.LocationMapType);
            return this.ListItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private async Task<List<object>> GetDataViewLocationOnly()
        {
            List<Location> locations = new List<Location>();
            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
            SLocMap locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            Location pickingLocation = new Location()
            {
                LocationMapWarehouseCode = locMap.WhsCode,
                LocationMapSectionName = locMap.SectName,
                LocationMapSlotName = locMap.SlotName,
                LocationMapType = "Picking",
            };

            locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);
            locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

            Location receivingLocation = new Location()
            {
                LocationMapWarehouseCode = locMap.WhsCode,
                LocationMapSectionName = locMap.SectName,
                LocationMapSlotName = locMap.SlotName,
                LocationMapType = "Receiving",
            };

            locations.Add(pickingLocation);
            locations.Add(receivingLocation);

            // this.DataViewItems = dataTableItems.DefaultView;
            // this.DataViewItems.Table.TableName = this.MainGridMappingName;
            // this.DataViewItems.AllowNew = false;
            // this.DataViewItems.AllowEdit = false;
            // this.DataViewItems.AllowDelete = false;
            // this.DataViewItems.Sort = "LocationMapWarehouseCode,LocationMapSectionName,LocationMapSlotName,LocationMapType";
            locations = locations.OrderBy(obj => obj.LocationMapWarehouseCode).ThenBy(obj => obj.LocationMapSectionName).ThenBy(obj => obj.LocationMapSlotName).ThenBy(obj => obj.LocationMapType).ToList();
            this.ListItems = locations.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdDisplayDefault_Click(object sender, EventArgs e)
        {
            await this.DisplayLocationDefaults();
        }

        /// <summary>
        ///
        /// </summary>
        private async Task DisplayLocationDefaults()
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    if (this.ActItem.ProdInfo.Refno > 0)
                    {
                        this.ActItem.ListItems = await this.GetDataViewLocationOnly();

                        await Device.InvokeOnMainThreadAsync(() =>
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
                            throw new Exception("The set location must have a warehouse selected.");
                        }

                        if (this.LocMap.CurrLocMap.IsSectionTracked && this.LocMap.CurrLocMap.SectLocMapRefno <= 0)
                        {
                            throw new Exception("The set location must have a section selected.");
                        }

                        this.ActItem.ListItems = await this.GetDataViewProductOnly();

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.DesignDataGridProductOnly();
                            this.dataGridMain.ItemsSource = this.DataGridMainExtraInfoItems;
                        });
                    }
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
                    this.Loading = false;
                }
            });
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }

        private async void cmdView_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}