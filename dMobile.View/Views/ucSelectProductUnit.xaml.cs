// <copyright file="ucSelectProductUnit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
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
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSelectProductUnit : DMobileModal, IAppArea
    {
        private bool isControlLoaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucSelectProductUnit"/> class.
        /// </summary>
        public ucSelectProductUnit()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ucSelectProductUnit"/> class.
        /// </summary>
        /// <param name="actSubType"></param>
        public ucSelectProductUnit(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ProductUnitsObservableCollection = new ObservableCollection<ProductUnit>();
            this.ActSubType = actSubType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ucSelectProductUnit"/> class.
        /// </summary>
        /// <param name="prodRefnoFilter"></param>
        /// <param name="prodUnitRefnoExcludeFilter"></param>
        /// <param name="actSubType"></param>
        public ucSelectProductUnit(int prodRefnoFilter, int prodUnitRefnoExcludeFilter, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;
            this.txtSearchProduct.Text = string.Empty;
            this.cboSearch.Items.Add("Code");
            this.cboSearch.Items.Add("Description");
            this.cboSearch.SelectedItem = "Code";

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ProductUnitsObservableCollection = new ObservableCollection<ProductUnit>();
            this.ProdRefnoFilter = prodRefnoFilter;
            this.ProdUnitRefnoFilter = prodUnitRefnoExcludeFilter;
            this.ActSubType = actSubType;
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (this.dgProducts.SelectedItem != null)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                    }
                }
                else if (keycode == Keycode.DpadDown)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductUnitsObservableCollection.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) < this.ProductUnitsObservableCollection.Count - 1)
                    {
                        this.dgProducts.SelectedItem = this.ProductUnitsObservableCollection[this.ProductUnitsObservableCollection.IndexOf(this.dgProducts.SelectedItem as ProductUnit) + 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductUnitsObservableCollection.IndexOf(this.dgProducts.SelectedItem as ProductUnit)) > 0)
                    {
                        this.dgProducts.SelectedItem = this.ProductUnitsObservableCollection[this.ProductUnitsObservableCollection.IndexOf(this.dgProducts.SelectedItem as ProductUnit) - 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
            });

            try
            {
                this.Loading = true;
                await this.ucSelectProduct_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "An error occurred loading data." + Environment.NewLine + ex.Message, "OK"));
            }
            finally
            {
                this.Loading = false;
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

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        private BarCodeScanner Scanner { get; set; }

        private SysEnvironment SysEnv { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.SelectProduct;

        private int ProdUnitRefnoFilter { get; set; } = 0;

        private int ProdRefnoFilter { get; set; } = 0;

        private string ProdUnitSizeDesc { get; set; } = string.Empty;

        private string ProdUnitBC { get; set; } = string.Empty;

        private bool ProdUnitIsBC { get; set; } = false;

        private string UnitCode { get; set; } = string.Empty;

        private string BaseUnitCode { get; set; } = string.Empty;

        private string WeightUnitCode { get; set; } = string.Empty;

        private string ProdDesc { get; set; } = string.Empty;

        private string ProdCode { get; set; } = string.Empty;

        private bool IsChangesMade { get; set; } = false;

        private List<ProductUnit> ListProdUnits { get; set; } = null;

        private List<ProductUnit> FilteredListProdUnits { get; set; } = null;

        private int ProdUnitRefno { get; set; } = 0;

        private int ProdRefno { get; set; } = 0;

        public ObservableCollection<ProductUnit> ProductUnitsObservableCollection { get; set; }

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
                isReadyToSave = true;
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
            bool isReadyToClose = !this.IsChangesMade;

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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
            return "Select Product";
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
                    await this.LoadData();
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<ProductUnitRefno>" + this.ProdUnitRefno + "</ProductUnitRefno>"
                + "<ProductRefno>" + this.ProdRefno + "</ProductRefno>"
                + "<ProductCode>" + this.ProdCode + "</ProductCode>"
                + "<ProductDescription>" + SysCommon.AddXMLReservedChars(this.ProdDesc) + "</ProductDescription>"
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
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucSelectProduct_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
            await Task.Run(async () => { await this.LoadData(); });
            _ = await Device.InvokeOnMainThreadAsync(() => this.cboSearch.SelectedIndex = 0);
            this.IsControlLoaded = true;
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
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%%" + search.Trim() + "%\' AND ProductUnitIsSellUnit AND ProductUnitIsBC";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList();
                            }

                            // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                            _ = this.FilteredListProdUnits.OrderByDescending(productUnit => productUnit.UnitCode).ThenBy(productUnit => productUnit.ProductUnitBC);
                        }
                        else
                        {
                            if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                            {
                                // ByCode
                                if (this.cboSearch.SelectedIndex == 0)
                                {
                                    // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                                    this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList();
                                }
                                else
                                {
                                    // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                                    this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit && productUnit.ProductUnitIsBC).ToList();
                                }
                            }
                            else
                            {
                                // By Code
                                if (this.cboSearch.SelectedIndex == 0)
                                {
                                    // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\'";
                                    this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList();
                                }
                                else
                                {
                                    // this.ListProdUnits.RowFilter = "ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\'";
                                    this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim())).ToList();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit).ToList();
                            }

                            // this.ListProdUnits.Sort = "UnitCode, ProductUnitBC DESC";
                            _ = this.FilteredListProdUnits.OrderByDescending(productUnit => productUnit.UnitCode).ThenBy(productUnit => productUnit.ProductUnitBC);
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductCode LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductDescription LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                        }
                    }
                }
                else
                {
                    // Product unit filter specified.
                    if (this.ProdUnitRefnoFilter != 0)
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter &&
                                productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter &&
                                productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) &&
                                productUnit.ProductUnitIsSellUnit &&
                                productUnit.ProductUnitIsBC).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit" + " AND ProductUnitIsBC";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter &&
                                productUnit.ProductUnitRefno == this.ProdUnitRefnoFilter &&
                                productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) &&
                                productUnit.ProductUnitIsSellUnit &&
                                productUnit.ProductUnitIsBC).ToList();
                            }
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductUnitRefno <> " + this.ProdUnitRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductUnitRefno != this.ProdUnitRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\' AND ProductUnitIsSellUnit";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim()) && productUnit.ProductUnitIsSellUnit).ToList();
                            }
                        }
                        else
                        {
                            // By Code
                            if (this.cboSearch.SelectedIndex == 0)
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductCode LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductCode.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                            else
                            {
                                // this.ListProdUnits.RowFilter = "ProductRefno = " + this.ProdRefnoFilter + " AND ProductDescription LIKE \'%" + search.Trim() + "%\'";
                                this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductRefno == this.ProdRefnoFilter && productUnit.ProductDescription.ToLower().Contains(search.ToLower().Trim())).ToList();
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.ActSubType != SysCommon.ActSubTypes.Receiving && this.ActSubType != SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                {
                    // this.ListProdUnits.RowFilter = "ProductUnitIsSellUnit";
                    this.FilteredListProdUnits = this.ListProdUnits.Where(productUnit => productUnit.ProductUnitIsSellUnit).ToList();
                }
                else
                {
                    this.FilteredListProdUnits = this.ListProdUnits;
                }
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
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            BarCodeParse parse = null;

            try
            {
                this.Loading = true;
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
                        int maxLen = parse.ProductIdentifier.Length > 10 ? 10 : parse.ProductIdentifier.Length;
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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            await this.LoadProdUnitList();
            this.ApplyFilter();
            await this.AddProductUnitToObservableCollection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadProdUnitList()
        {
            this.ListProdUnits = await ProductInfoTasks.GetProductUnitList();
            this.FilteredListProdUnits = this.ListProdUnits;
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
                    this.lblProductDescription.Text = this.ProdCode + "  " + this.ProdDesc;
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
            this.ProductUnitsObservableCollection = new ObservableCollection<ProductUnit>();

            foreach (ProductUnit prodUnit in this.FilteredListProdUnits)
            {
                // ProductUnit prodUnit = new ProductUnit
                // {
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]),
                //    ProductCode = Convert.ToString(row["ProductCode"]),
                //    UnitCode = Convert.ToString(row["UnitCode"]),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"]),
                //    WeightUnitCode = Convert.ToString(row["WeightUnitCode"]),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"]),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                //    ProductUnitIsSellUnit = Convert.ToBoolean(row["ProductUnitIsSellUnit"]),
                // };
                this.ProductUnitsObservableCollection.Add(prodUnit);
            }

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.dgProducts.ItemsSource = this.ProductUnitsObservableCollection;

                if (this.ProductUnitsObservableCollection.Count > 0)
                {
                    this.dgProducts.SelectedItem = this.ProductUnitsObservableCollection[0];
                }
                else
                {
                    this.dgProducts.SelectedItem = null;
                }
            });
        }

        private async void TxtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    Entry entry = sender as Entry;
                    int startLength = entry.Text.Length;

                    await Task.Delay(300);

                    if (startLength == entry.Text.Length)
                    {
                        this.ApplySearchFilter(this.txtSearchProduct.Text);
                        await this.AddProductUnitToObservableCollection();
                    }
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
        }

        private void TxtSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.txtSearchProduct.CursorPosition = 0;
            this.txtSearchProduct.SelectionLength = this.txtSearchProduct.Text.Length;
        }

        private async void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ProductUnit prodUnit = e.SelectedItem as ProductUnit;
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
