// <copyright file="SelectProductUnit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Perform selecting a product unit.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectProductUnit : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectProductUnit"/> class.
        /// </summary>
        public SelectProductUnit()
        {
            this.InitializeComponent();
            this.IsPageLoading = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectProductUnit"/> class.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public SelectProductUnit(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsPageLoading = false;
            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ProductUnits = new ObservableCollection<ProductUnit>();
            this.ActSubType = actSubType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectProductUnit"/> class.
        /// </summary>
        /// <param name="prodRefnoFilter"><see cref="int"/> Product reference number filter.</param>
        /// <param name="prodUnitRefnoExcludeFilter"><see cref="int"/> Product unit reference number exclude filter.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public SelectProductUnit(int prodRefnoFilter, int prodUnitRefnoExcludeFilter, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsPageLoading = false;
            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");
            this.cboSearch.SelectedItem = "Code";
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ProductUnits = new ObservableCollection<ProductUnit>();
            this.ProdRefnoFilter = prodRefnoFilter;
            this.ProdUnitRefnoFilter = prodUnitRefnoExcludeFilter;
            this.ActSubType = actSubType;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code scanner engine.
        /// </summary>
        private dMobile.BarCodeScanner.BarCodeScanners.BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.SelectProduct;

        /// <summary>
        /// Gets or sets the product unit reference number filter.
        /// </summary>
        private int ProdUnitRefnoFilter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product reference number filter.
        /// </summary>
        private int ProdRefnoFilter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product unit size description.
        /// </summary>
        private string ProdUnitSizeDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit bc string.
        /// </summary>
        private string ProdUnitBC { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the product unit is broken case.
        /// </summary>
        private bool ProdUnitIsBC { get; set; } = false;

        /// <summary>
        /// Gets or sets the product unit code.
        /// </summary>
        private string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product base unit code.
        /// </summary>
        private string BaseUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product weight unit code.
        /// </summary>
        private string WeightUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        private string ProdDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        private string ProdCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product brand.
        /// </summary>
        private string ProdBrand { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets a list of product units.
        /// </summary>
        private List<ProductUnit> ListProdUnits { get; set; } = null;

        /// <summary>
        /// Gets or sets the filtered list of product units.
        /// </summary>
        private List<ProductUnit> FilteredListProdUnits { get; set; } = null;

        /// <summary>
        /// Gets or sets the product unit reference number.
        /// </summary>
        private int ProdUnitRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product reference number.
        /// </summary>
        private int ProdRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a list of product units.
        /// </summary>
        private ObservableCollection<ProductUnit> ProductUnits { get; set; }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
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
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initialize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return "Select Product";
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.IsChangesMade = false;
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all
                    // in the event that IsReadyToSave just returns false.
                    throw new Exception("Data failed validation. Check your data.");
                }
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
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties if needed.
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
                    // await this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;

                    // Re-enable the scanner, IsReadToClose calls this.ScannerDisable().
                    await this.ScannerEnable();
                }
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
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<ProductUnitRefno>" + this.ProdUnitRefno + "</ProductUnitRefno>"
                + "<ProductRefno>" + this.ProdRefno + "</ProductRefno>"
                + "<ProductCode>" + this.ProdCode + "</ProductCode>"
                + "<ProductDescription>" + SysCommon.AddXMLReservedChars(this.ProdDesc) + "</ProductDescription>"
                + "<ProductBrand>" + this.ProdBrand + "</ProductBrand>"
                + "<UnitCode>" + this.UnitCode + "</UnitCode>"
                + "<BaseUnitCode>" + this.BaseUnitCode + "</BaseUnitCode>"
                + "<WeightUnitCode>" + this.WeightUnitCode + "</WeightUnitCode>"
                + "<ProductUnitBC>" + this.ProdUnitBC + "</ProductUnitBC>"
                + "<ProductUnitIsBC>" + this.ProdUnitIsBC + "</ProductUnitIsBC>"
                + "<ProductUnitSizeDescription>" + SysCommon.AddXMLReservedChars(this.ProdUnitSizeDesc)
                + "</ProductUnitSizeDescription>"
                + "</params>");

            return request;
        }

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
                    this.Scanner.BarCodeScan -=
                        new dMobile.BarCodeScanner.BarCodeScanners.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
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
                    this.Scanner.BarCodeScan +=
                        new dMobile.BarCodeScanner.BarCodeScanners.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// On appearing event handler.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsPageLoading)
            {
                try
                {
                    this.IsPageLoading = true;
                    base.OnAppearing();
                    await Task.Run(async () => { await this.LoadData(); });
                    if (this.IsFirstAppearance)
                    {
                        this.IsFirstAppearance = false;
                        await ScannerStart();
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cboSearch.SelectedIndex = 0);
                        await this.ActAppearing();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    this.IsFirstAppearance = true;
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    this.IsFirstAppearance = true;
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        const string msg = "This activity unexpectedly failed to initialize correctly." +
                                           " Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                    this.IsPageLoading = false;
                    ////await Task.Run(() =>
                    ////{
                    ////    // Explicit GC, Xamarin forms not disposing of objects properly
                    ////    GC.Collect();
                    ////    GC.WaitForPendingFinalizers();
                    ////});
                }
            }

            //////////////////////////////////////////////////////////////////////////
            // Registers the bar code scan event handler and enables the scanner.
            async Task ScannerStart()
            {
                if (this.Scanner != null)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan +=
                        new dMobile.BarCodeScanner.BarCodeScanners.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// The on disappearing event handler.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ActDisappearing();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// On back button pressed event handler.
        /// </summary>
        /// <returns><see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);

            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();

                _ = await this.CancelChanges();
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }

        /// <summary>
        /// Activity appearing.
        /// </summary>
        private async Task ActAppearing()
        {
            SubscribeKeyUpHandlers();
            await this.ScannerEnable();

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    switch (keycode)
                    {
                        case Keycode.DpadDown:
                            {
                                if (this.dgProducts.SelectedItem != null && Convert.ToInt32(
                                        this.ProductUnits.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) <
                                    this.ProductUnits.Count - 1)
                                {
                                    this.dgProducts.SelectedItem = this.ProductUnits[
                                        this.ProductUnits.IndexOf(this.dgProducts.SelectedItem as ProductUnit) + 1];
                                    this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                                }

                                break;
                            }

                        case Keycode.DpadUp:
                            {
                                if (this.dgProducts.SelectedItem != null && Convert.ToInt32(
                                        this.ProductUnits.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) > 0)
                                {
                                    this.dgProducts.SelectedItem = this.ProductUnits[
                                        this.ProductUnits.IndexOf(this.dgProducts.SelectedItem as ProductUnit) - 1];
                                    this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                                }

                                break;
                            }

                        case Keycode.Enter:
                        case Keycode.F1:
                            {
                                if (this.dgProducts.SelectedItem != null)
                                {
                                    if (await this.SaveChanges())
                                    {
                                        this.DialogResult = DialogResult.OK;
                                        await this.Close();
                                    }
                                }

                                break;
                            }

                    }
                });
            }
        }

        /// <summary>
        /// Activity disappearing.
        /// </summary>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="search"></param>
        private void ApplySearchFilter(string search)
        {
            if (!string.IsNullOrEmpty(search) || (string.Compare(search.Trim(), string.Empty) > 0))
            {
                if (this.ProdRefnoFilter == 0)
                {
                    // Product unit filter specified.
                    if (this.ProdUnitRefnoFilter != 0)
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                            this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                        productUnit.ProductCode.ToLower()
                                            .Contains(search.ToLower()
                                                .Trim()) &&
                                        productUnit.ProductUnitIsSellUnit &&
                                        productUnit.ProductUnitIsBC)
                                    .ToList()
                                :

                                this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                        productUnit.ProductDescription.ToLower()
                                            .Contains(search.ToLower()
                                                .Trim()) &&
                                        productUnit.ProductUnitIsSellUnit &&
                                        productUnit.ProductUnitIsBC)
                                    .ToList();

                            // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                            _ = this.FilteredListProdUnits.OrderByDescending(productUnit => productUnit.UnitCode)
                                .ThenBy(productUnit => productUnit.ProductUnitBC);
                        }
                        else
                        {
                            if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                                this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                            {
                                // ByCode
                                this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                    ? this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                        productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) &&
                                        productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList()
                                    :
                                    this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                        productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) &&
                                        productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList();
                            }
                            else
                            {
                                // By Code
                                this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                    ? this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit
                                            .ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList()
                                    :
                                    this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit
                                            .ProductDescription.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                            this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) &&
                                    productUnit.ProductUnitIsSellUnit).ToList()
                                :
                                this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) &&
                                    productUnit.ProductUnitIsSellUnit).ToList();

                            // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                            _ = this.FilteredListProdUnits.OrderByDescending(productUnit => productUnit.UnitCode)
                                .ThenBy(productUnit => productUnit.ProductUnitBC);
                        }
                        else
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList()
                                :
                                this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()))
                                    .ToList();
                        }
                    }
                }
                else
                {
                    // Product unit filter specified.
                    if (this.ProdUnitRefnoFilter != 0)
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                            this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter &&
                                productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) &&
                                productUnit.ProductUnitIsSellUnit &&
                                productUnit.ProductUnitIsBC).ToList();
                            }
                            else
                            {
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter &&
                                productUnit.ProductUnitRefno == this.ProdUnitRefnoFilter &&
                                productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) &&
                                productUnit.ProductUnitIsSellUnit &&
                                productUnit.ProductUnitIsBC).ToList();
                            }
                        }
                        else
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter &&
                                    productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                    productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList()
                                :
                                this.ListProdUnits.Where(productUnit =>
                                        productUnit.ProductRefno == this.ProdRefnoFilter &&
                                        productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                        productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()))
                                    .ToList();
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                            this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter &&
                                    productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) &&
                                    productUnit.ProductUnitIsSellUnit).ToList()
                                : this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter &&
                                    productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) &&
                                    productUnit.ProductUnitIsSellUnit).ToList();
                        }
                        else
                        {
                            // By Code
                            this.FilteredListProdUnits = this.cboSearch.SelectedIndex == 0
                                ? this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductCode
                                        .ToLower().Contains(search.ToLower().Trim())).ToList()
                                : this.ListProdUnits.Where(productUnit =>
                                    productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductDescription
                                        .ToLower().Contains(search.ToLower().Trim())).ToList();
                        }
                    }
                }

                ////if (this.ActSubType != SysCommon.ActSubTypes.Receiving &&
                ////    this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                ////{
                ////    this.FilteredListProdUnits = this.ListProdUnits
                ////        .Where(productUnit => productUnit.ProductUnitIsSellUnit).ToList();
                ////}
                ////else
                ////{
                ////    ////this.FilteredListProdUnits = this.ListProdUnits;
                ////}
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsPageLoading || scanArgs == null)
            {
                return;
            }

            BarCodeParse parse = null;

            try
            {
                await this.ScannerDisable();

                parse = scanArgs.ParseInfo;

                // Global location identifier
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    throw new Exception("Scanning a global location identifier is not supported here!");
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(parse.ActFKRefno))
                {
                    throw new Exception("Scanning an activity bar code is not supported here!");
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                {
                    throw new Exception("Scanning a line item bar code is not supported here!");
                }

                // Activity detail fk refno
                if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                {
                    throw new Exception("Scanning a line item detail bar code is not supported here!");
                }

                // Shipping container scanned, skid
                if (!string.IsNullOrEmpty(parse.SSCC18))
                {
                    throw new Exception("Scanning a shipping container bar code is not supported here!");
                }

                // Product identifier
                if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.cboSearch.SelectedIndex = 0;
                        var maxLen = parse.ProductIdentifier.Length > 10 ? 10 : parse.ProductIdentifier.Length;
                        this.txtSearchProduct.Text = parse.ProductIdentifier.Substring(0, maxLen);
                        this.txtSearchProduct.CursorPosition = 0;
                        this.txtSearchProduct.SelectionLength = this.txtSearchProduct.Text.Length;
                        _ = this.txtSearchProduct.Focus();
                    });
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
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            this.ListProdUnits = await ProductInfoTasks.GetProductUnitList();
            this.FilteredListProdUnits = this.ListProdUnits;
            this.ApplyFilter();
            await this.AddProductUnitToObservableCollection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idx"></param>
        private async Task SelectProdUnit(ProductUnit prodUnit)
        {
            if (prodUnit == null)
            {
                return;
            }

            try
            {
                var prod = await ProductInfoTasks.GetInfoFromRefno(prodUnit.ProductRefno);
                this.ProdBrand = Convert.ToString(prod.BrandDesc);

                this.ProdUnitRefno = Convert.ToInt32(prodUnit.ProductUnitRefno);
                this.ProdRefno = Convert.ToInt32(prodUnit.ProductRefno);
                this.ProdDesc = Convert.ToString(prodUnit.ProductDescription);
                this.ProdCode = Convert.ToString(prodUnit.ProductCode);
                this.UnitCode = Convert.ToString(prodUnit.UnitCode);
                this.BaseUnitCode = Convert.ToString(prodUnit.BaseUnitCode);
                this.WeightUnitCode = Convert.ToString(prodUnit.WeightUnitCode);
                this.ProdUnitBC = Convert.ToString(prodUnit.ProductUnitBC);
                this.ProdUnitIsBC = Convert.ToBoolean(prodUnit.ProductUnitIsBC);
                this.ProdUnitSizeDesc = Convert.ToString(prodUnit.ProductUnitSizeDescription);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblProductDescription.Text = this.ProdCode + "  " + this.ProdDesc + " " + this.ProdBrand;
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("SelectProdUnit", ex.Message, "OK"));
            }
        }

        private void ApplyFilter()
        {
            if (this.ProdRefnoFilter != 0)
            {
                if (this.ProdUnitRefnoFilter != 0)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter;
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductRefno == this.ProdRefnoFilter && prodUnit.ProductUnitRefno != this.ProdUnitRefnoFilter).ToList();
                    }
                    else
                    {
                        // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductRefno == this.ProdRefnoFilter && prodUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && prodUnit.ProductUnitIsSellUnit && prodUnit.ProductUnitIsBC).ToList();

                        // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                        _ = this.FilteredListProdUnits.OrderByDescending(prodUnit => prodUnit.UnitCode).ThenBy(prodUnit => prodUnit.ProductUnitBC);
                    }
                }
                else
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter;
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductRefno == this.ProdRefnoFilter).ToList();
                    }
                    else
                    {
                        // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitIsSellUnit";
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductRefno == this.ProdRefnoFilter && prodUnit.ProductUnitIsSellUnit).ToList();

                        // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                        _ = this.FilteredListProdUnits.OrderByDescending(prodUnit => prodUnit.UnitCode).ThenBy(prodUnit => prodUnit.ProductUnitBC);
                    }
                }
            }
            else
            {
                if (this.ProdUnitRefnoFilter != 0)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter;
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductUnitRefno != this.ProdUnitRefnoFilter).ToList();
                    }
                    else
                    {
                        // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter.ToString() + " AND ProductUnitIsSellUnit AND ProductUnitIsBC";
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && prodUnit.ProductUnitIsSellUnit && prodUnit.ProductUnitIsBC).ToList();
                    }
                }
                else
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving || this.ActSubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                    {
                        // this.ListProdUnits.RowFilter = string.Empty;
                        this.FilteredListProdUnits = this.ListProdUnits;
                    }
                    else
                    {
                        // this.ListProdUnits.RowFilter = "ProductUnitIsSellUnit";
                        this.FilteredListProdUnits = this.ListProdUnits.Where(prodUnit => prodUnit.ProductUnitIsSellUnit).ToList();
                    }
                }
            }
        }

        private async Task AddProductUnitToObservableCollection()
        {
            this.ProductUnits = new ObservableCollection<ProductUnit>();

            foreach (var prodUnit in this.FilteredListProdUnits)
            {
                this.ProductUnits.Add(prodUnit);
            }

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.dgProducts.ItemsSource = this.ProductUnits;

                if (this.ProductUnits.Count > 0)
                {
                    this.dgProducts.SelectedItem = this.ProductUnits[0];
                }
                else
                {
                    this.dgProducts.SelectedItem = null;
                }
            });
        }

        private async void TxtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ListProdUnits != null)
            {
                try
                {
                    this.IsChangesMade = true;

                    var entry = sender as Entry;
                    var startLength = entry.Text.Length;

                    await Task.Delay(300);

                    if (startLength == entry.Text.Length)
                    {
                        this.ApplySearchFilter(this.txtSearchProduct.Text);
                        await this.AddProductUnitToObservableCollection();
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
            }
        }

        private void TxtSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.txtSearchProduct.CursorPosition = 0;
            this.txtSearchProduct.SelectionLength = this.txtSearchProduct.Text.Length;
        }

        private async void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var prodUnit = e.SelectedItem as ProductUnit;
            await this.SelectProdUnit(prodUnit);
        }

        private async void CmdSelect_Clicked(object sender, EventArgs e)
        {
            if (this.dgProducts.SelectedItem != null && await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void CmdClose_Clicked(object sender, EventArgs e)
        {
            _ = await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}
