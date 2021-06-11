using dMobile.View.CustomPages.Modal;
using System;
using System.Data;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Types;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace dMobile.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSelectProduct : DMobileModal, IAppArea
    {
        #region Fields

        //private UserControl callingControl = null;
        private int prodRefno = 0;
        private bool isChangesMade = false;
        private List<Product> listProducts = null;
        private string prodCode = string.Empty;
        private string prodDesc = string.Empty;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private bool isControlLoaded = false;
        private bool filterOnAllowConvertProduct = false;
        private ObservableCollection<Product> productObservableCollection;
        private ObservableCollection<Product> productFilteredObservableCollection;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="callingControl"></param>
        /// <remarks></remarks>
        public ucSelectProduct()
        {
            this.InitializeComponent();

            // TODO: Resource strings
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

        #endregion

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

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
                // TODO: Logging
                Console.WriteLine("this.ucSelectProduct_GotFocus threw error " + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        #region Properties

        /// <summary>
        /// Sets a filter on product list for AllowConvertProduct.
        /// Will automatically load the data on the grid again if set to a different value.
        /// </summary>
        public bool FilterOnAllowConvertProduct
        {
            get
            {
                return filterOnAllowConvertProduct;
            }
            set
            {
                filterOnAllowConvertProduct = value;
                // we only load when value is true.
                // This allows the variable to be set back to false without causing infinite loop.
                if (value)
                {
                    // TODO: Test loading 
                    Task.Run(async () => await LoadData()).Wait();
                }
            }
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

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private string ProdDesc
        {
            get
            {
                return this.prodDesc;
            }

            set
            {
                this.prodDesc = value;
            }
        }

        private string ProdCode
        {
            get
            {
                return this.prodCode;
            }

            set
            {
                this.prodCode = value;
            }
        }

        private bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
            }
        }

        private List<Product> ListProducts
        {
            get
            {
                return this.listProducts;
            }

            set
            {
                this.listProducts = value;
            }
        }

        private int ProdRefno
        {
            get
            {
                return this.prodRefno;
            }

            set
            {
                this.prodRefno = value;
            }
        }

        private ObservableCollection<Product> ProductObservableCollection
        {
            get => productObservableCollection;
            set => productObservableCollection = value;
        }

        private ObservableCollection<Product> ProductFilteredObservableCollection
        {
            get => productFilteredObservableCollection;
            set => productFilteredObservableCollection = value;
        }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
        }

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
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            // TODO: Resource strings
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

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
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
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<params>" +
                ("<ProductRefno>" + this.ProdRefno + "</ProductRefno>") +
                ("<ProductCode>" + this.ProdCode + "</ProductCode>") +
                ("<ProductDescription>" + SysCommon.AddXMLReservedChars(this.ProdDesc) + "</ProductDescription>") +
                "</params>");

            return request;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucSelectProduct_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                if (!this.IsControlLoaded)
                {
                    this.Loading = true;

                    await Task.Run(() => this.LoadData()).ContinueWith(async (_) =>
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
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void textBoxSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsChangesMade = true;

            if (!string.IsNullOrEmpty(this.textBoxSearchProduct.Text) || (string.Compare(this.textBoxSearchProduct.Text.Trim(), string.Empty) > 0))
            {
                // By Code
                if (this.cboSearch.SelectedIndex == 0)
                {
                    //this.DataViewProducts.RowFilter = "ProductCode LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredObservableCollection = new ObservableCollection<Product>(this.ProductObservableCollection.Where((prod) => prod.ProductCode.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
                else
                {
                    //this.DataViewProducts.RowFilter = "ProductDescription LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredObservableCollection = new ObservableCollection<Product>(this.ProductObservableCollection.Where((prod) => prod.ProductDescription.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
            }
            else
            {
                this.ProductFilteredObservableCollection = this.ProductObservableCollection;
            }

            dgProducts.ItemsSource = this.ProductFilteredObservableCollection;

            if (this.ProductFilteredObservableCollection.Count > 0)
            {
                this.dgProducts.SelectedItem = this.ProductFilteredObservableCollection[0];
                await this.SelectProduct(this.ProductFilteredObservableCollection[0]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iSeletedIndex"></param>
        /// <remarks></remarks>
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
                // TODO: Logging
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            await this.LoadProductList();
            this.AddProductToObservableCollection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadProductList()
        {
            if (FilterOnAllowConvertProduct)
            {
                this.ListProducts = await ProductInfoTasks.GetProductListAllowConvertProduct();
            }
            else
            {
                this.ListProducts = await ProductInfoTasks.GetProductList();
            }
        }

        private void AddProductToObservableCollection()
        {
            this.ProductObservableCollection.Clear();

            foreach (Product product in this.ListProducts)
            {
                //Product prod = new Product
                //{
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                //    ProductDescription = Convert.ToString(row["ProductDescription"]),
                //    ProductCode = Convert.ToString(row["ProductCode"]),
                //};

                this.ProductObservableCollection.Add(product);
            }

            this.ProductFilteredObservableCollection = this.ProductObservableCollection;
        }

        private void TextBoxSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.textBoxSearchProduct.CursorPosition = 0;
            this.textBoxSearchProduct.SelectionLength = this.textBoxSearchProduct.Text != null ? this.textBoxSearchProduct.Text.Length : 0;
        }

        private async void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Product prod = e.SelectedItem as Product;
            await this.SelectProduct(prod);
        }


        private async void CmdSelect_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void CmdClose_Clicked(object sender, EventArgs e)
        {
            await this.CancelChanges();
            this.DialogResult = DialogResult.None;
            await this.Close();
        }
    }

    #endregion
}