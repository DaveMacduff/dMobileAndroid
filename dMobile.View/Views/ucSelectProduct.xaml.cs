// <copyright file="ucSelectProduct.xaml.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Bedrock;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ucSelectProduct" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSelectProduct : DMobileModal, IAppArea
    {
        /// <summary>
        /// Defines the filterOnAllowConvertProduct.
        /// </summary>
        private bool filterOnAllowConvertProduct = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucSelectProduct"/> class.
        /// </summary>
        public ucSelectProduct()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblSearch.Text = "Search";
            this.lblProductDescription.Text = string.Empty;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.cboSearch.Items.Add("Code");
                this.cboSearch.Items.Add("Description");
                this.cboSearch.SelectedItem = "Code";
                this.SysEnv = SysEnvironment.GetInstance();
                this.ProductObservableCollection = new ObservableCollection<Product>();
                this.ProductFilteredObservableCollection = new ObservableCollection<Product>();
            }
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
                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        await this.Close();
                    }
                }
                else if (keycode == Keycode.DpadDown)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductFilteredObservableCollection.IndexOf(this.dgProducts.SelectedItem as Product)) < this.ProductFilteredObservableCollection.Count - 1)
                    {
                        this.dgProducts.SelectedItem = this.ProductFilteredObservableCollection[this.ProductFilteredObservableCollection.IndexOf(this.dgProducts.SelectedItem as Product) + 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductFilteredObservableCollection.IndexOf(this.dgProducts.SelectedItem as Product)) > 0)
                    {
                        this.dgProducts.SelectedItem = this.ProductFilteredObservableCollection[this.ProductFilteredObservableCollection.IndexOf(this.dgProducts.SelectedItem as Product) - 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
            });

            try
            {
                await this.ucSelectProduct_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Console.WriteLine("this.ucSelectProduct_GotFocus threw error " + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    _ = await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether FilterOnAllowConvertProduct
        /// Sets a filter on product list for AllowConvertProduct.
        /// Will automatically load the data on the grid again if set to a different value..
        /// </summary>
        public bool FilterOnAllowConvertProduct
        {
            get
            {
                return this.filterOnAllowConvertProduct;
            }
            set
            {
                this.filterOnAllowConvertProduct = value;
                // we only load when value is true.
                // This allows the variable to be set back to false without causing infinite loop.
                if (value)
                {
                    // TODO: Test loading
                    Task.Run(async () => await this.LoadData()).Wait();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsControlLoaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the ProdDesc.
        /// </summary>
        private string ProdDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ProdCode.
        /// </summary>
        private string ProdCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the ListProducts.
        /// </summary>
        private List<Product> ListProducts { get; set; } = null;

        /// <summary>
        /// Gets or sets the ProdRefno.
        /// </summary>
        private int ProdRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ProductObservableCollection.
        /// </summary>
        private ObservableCollection<Product> ProductObservableCollection { get; set; }

        /// <summary>
        /// Gets or sets the ProductFilteredObservableCollection.
        /// </summary>
        private ObservableCollection<Product> ProductFilteredObservableCollection { get; set; }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

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
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            bool isReadyToClose = true;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
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
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Select Product";
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
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
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    this.ProdRefno = 0;
                    this.ProdDesc = string.Empty;
                    this.ProdCode = string.Empty;

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
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
        public async Task<XDocument> GetXML()
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                "<ProductRefno>" + this.ProdRefno + "</ProductRefno>" +
                "<ProductCode>" + this.ProdCode + "</ProductCode>" +
                "<ProductDescription>" + SysCommon.AddXMLReservedChars(this.ProdDesc) + "</ProductDescription>" +
                "</params>");

            return request;
        }

        /// <summary>
        /// The ucSelectProduct_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ucSelectProduct_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.Loading = true;

                _ = await Task.Run(() => this.LoadData()).ContinueWith(async (_) =>
                    {
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.dgProducts.ItemsSource = this.ProductFilteredObservableCollection;

                            if (this.ProductFilteredObservableCollection.Count > 0)
                            {
                                this.dgProducts.SelectedItem = this.ProductFilteredObservableCollection[0];
                            }

                            this.Loading = false;
                        });
                    });

                this.lblProductDescription.Text = string.Empty;
                this.cboSearch.SelectedIndex = 0;
                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        /// The textBoxSearchProduct_TextChanged.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void textBoxSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsChangesMade = true;

            if (!string.IsNullOrEmpty(this.textBoxSearchProduct.Text) || (string.Compare(this.textBoxSearchProduct.Text.Trim(), string.Empty) > 0))
            {
                // By Code
                if (this.cboSearch.SelectedIndex == 0)
                {
                    // this.DataViewProducts.RowFilter = "ProductCode LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredObservableCollection = new ObservableCollection<Product>(this.ProductObservableCollection.Where((prod) => prod.ProductCode.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
                else
                {
                    // this.DataViewProducts.RowFilter = "ProductDescription LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredObservableCollection = new ObservableCollection<Product>(this.ProductObservableCollection.Where((prod) => prod.ProductDescription.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
            }
            else
            {
                this.ProductFilteredObservableCollection = this.ProductObservableCollection;
            }

            this.dgProducts.ItemsSource = this.ProductFilteredObservableCollection;

            if (this.ProductFilteredObservableCollection.Count > 0)
            {
                this.dgProducts.SelectedItem = this.ProductFilteredObservableCollection[0];
                await this.SelectProduct(this.ProductFilteredObservableCollection[0]);
            }
        }

        /// <summary>
        /// The SelectProduct.
        /// </summary>
        /// <param name="prod">The prod<see cref="Product"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task SelectProduct(Product prod)
        {
            if (prod == null)
            {
                return;
            }

            try
            {
                this.ProdRefno = Convert.ToInt32(prod.ProductRefno);
                this.ProdDesc = Convert.ToString(prod.ProductDescription);
                this.ProdCode = Convert.ToString(prod.ProductCode);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblProductDescription.Text = this.ProdCode + "  " + this.ProdDesc;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task LoadData()
        {
            await this.LoadProductList();
            this.AddProductToObservableCollection();
        }

        /// <summary>
        /// The LoadProductList.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task LoadProductList()
        {
            if (this.FilterOnAllowConvertProduct)
            {
                this.ListProducts = await ProductInfoTasks.GetProductListAllowConvertProduct();
            }
            else
            {
                this.ListProducts = await ProductInfoTasks.GetProductList();
            }
        }

        /// <summary>
        /// The AddProductToObservableCollection.
        /// </summary>
        private void AddProductToObservableCollection()
        {
            this.ProductObservableCollection.Clear();

            foreach (Product product in this.ListProducts)
            {
                this.ProductObservableCollection.Add(product);
            }

            this.ProductFilteredObservableCollection = this.ProductObservableCollection;
        }

        /// <summary>
        /// The TextBoxSearchProduct_Focused.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="FocusEventArgs"/>.</param>
        private void TextBoxSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.textBoxSearchProduct.CursorPosition = 0;
            this.textBoxSearchProduct.SelectionLength = this.textBoxSearchProduct.Text != null ? this.textBoxSearchProduct.Text.Length : 0;
        }

        /// <summary>
        /// The DgProducts_ItemSelected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private async void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Product prod = e.SelectedItem as Product;
            await this.SelectProduct(prod);
        }

        /// <summary>
        /// The CmdSelect_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void CmdSelect_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        /// The CmdClose_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void CmdClose_Clicked(object sender, EventArgs e)
        {
            _ = await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }
}
