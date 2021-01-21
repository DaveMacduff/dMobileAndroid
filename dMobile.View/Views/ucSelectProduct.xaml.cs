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

namespace dMobile.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSelectProduct : DMobileModal, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = null;
        private int prodRefno = 0;
        private bool isChangesMade = false;
        private DataView dataViewProducts = null;
        private string prodCode = string.Empty;
        private string prodDesc = string.Empty;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private bool isControlLoaded = false;
        private bool filterOnAllowConvertProduct = false;
        private ObservableCollection<Product> productList;
        private ObservableCollection<Product> productFilteredList;

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
                this.ProductList = new ObservableCollection<Product>();
                this.ProductFilteredList = new ObservableCollection<Product>();
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
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        await this.Close();
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.DpadDown)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductFilteredList.IndexOf(this.dgProducts.SelectedItem as Product)) < this.ProductFilteredList.Count - 1)
                    {
                        this.dgProducts.SelectedItem = this.ProductFilteredList[this.ProductFilteredList.IndexOf(this.dgProducts.SelectedItem as Product) + 1];
                        this.dgProducts.ScrollTo(this.dgProducts.SelectedItem, ScrollToPosition.MakeVisible, true);
                    }
                }
                else if (keycode == Keycode.DpadUp)
                {
                    if (this.dgProducts.SelectedItem != null && Convert.ToInt32(this.ProductFilteredList.IndexOf(this.dgProducts.SelectedItem as Product)) > 0)
                    {
                        this.dgProducts.SelectedItem = this.ProductFilteredList[this.ProductFilteredList.IndexOf(this.dgProducts.SelectedItem as Product) - 1];
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
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

        #region Events

        public event ReqCloseHandler ReqClose
        {
            add
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(this.ReqCloseEvent, value);
            }

            remove
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(this.ReqCloseEvent, value);
            }
        }

        public event ReqSaveEnableHandler ReqSaveEnable
        {
            add
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Combine(this.ReqSaveEnableEvent, value);
            }

            remove
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Remove(this.ReqSaveEnableEvent, value);
            }
        }

        public event ReqSaveDisableHandler ReqSaveDisable
        {
            add
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Combine(this.ReqSaveDisableEvent, value);
            }

            remove
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Remove(this.ReqSaveDisableEvent, value);
            }
        }

        public event ReqCancelEnableHandler ReqCancelEnable
        {
            add
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Combine(this.ReqCancelEnableEvent, value);
            }

            remove
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Remove(this.ReqCancelEnableEvent, value);
            }
        }

        public event ReqCancelDisableHandler ReqCancelDisable
        {
            add
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Combine(this.ReqCancelDisableEvent, value);
            }

            remove
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Remove(this.ReqCancelDisableEvent, value);
            }
        }

        #endregion

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
                    LoadData();
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

        private DataView DataViewProducts
        {
            get
            {
                return this.dataViewProducts;
            }

            set
            {
                this.dataViewProducts = value;
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

        //private UserControl CallingControl
        //{
        //    get
        //    {
        //        return this.callingControl;
        //    }

        //    set
        //    {
        //        this.callingControl = value;
        //    }
        //}

        private ReqCloseHandler ReqCloseEvent
        {
            get
            {
                return this.reqCloseEvent;
            }

            set
            {
                this.reqCloseEvent = value;
            }
        }

        private ReqSaveEnableHandler ReqSaveEnableEvent
        {
            get
            {
                return this.reqSaveEnableEvent;
            }

            set
            {
                this.reqSaveEnableEvent = value;
            }
        }

        private ReqSaveDisableHandler ReqSaveDisableEvent
        {
            get
            {
                return this.reqSaveDisableEvent;
            }

            set
            {
                this.reqSaveDisableEvent = value;
            }
        }

        private ReqCancelEnableHandler ReqCancelEnableEvent
        {
            get
            {
                return this.reqCancelEnableEvent;
            }

            set
            {
                this.reqCancelEnableEvent = value;
            }
        }

        private ReqCancelDisableHandler ReqCancelDisableEvent
        {
            get
            {
                return this.reqCancelDisableEvent;
            }

            set
            {
                this.reqCancelDisableEvent = value;
            }
        }

        private ObservableCollection<Product> ProductList
        {
            get => productList;
            set => productList = value;
        }
        private ObservableCollection<Product> ProductFilteredList
        {
            get => productFilteredList;
            set => productFilteredList = value;
        }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
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

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return this.CallingControl;
        //}

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
                    ResetFilterAllowProductChange();
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
                    // If filter on AllowProductChanges, reset that
                    ResetFilterAllowProductChange();

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
                    this.LoadData();
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
        /// Resets the data so that we can load fresh the regular data again.
        /// </summary>
        /// <returns></returns>
        private void ResetFilterAllowProductChange()
        {
            if (FilterOnAllowConvertProduct)
            {
                ////Products.GetInstance().ClearData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public XDocument GetXML()
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

        #region IAppPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
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
                            this.dgProducts.ItemsSource = this.ProductFilteredList;

                            if (this.ProductFilteredList.Count > 0)
                            {
                                this.dgProducts.SelectedItem = this.ProductFilteredList[0];
                            }

                            this.Loading = false;
                        });
                    });

                    this.lblProductDescription.Text = string.Empty;
                    this.cboSearch.SelectedIndex = 0;

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.IsControlLoaded = true;
                }
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void ValidateEntry(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = false;

        //    try
        //    {
        //        if (sender == this.textBoxSearchProduct)
        //        {
        //        }

        //        if (sender == this.cboSearch)
        //        {
        //        }

        //        if (sender == this.dgProducts)
        //        {
        //        }
        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        e.Cancel = true;
        //        ex.Show(this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Cancel = false;

        //        Device.BeginInvokeOnMainThread(async () =>
        //        {
        //            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
        //            await errRpt.ShowDialog();
        //        });
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void dgProducts_Select(object sender, EventArgs e)
        //{
        //    this.SelectProduct(this.dgProducts.CurrentRowIndex);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void textBoxSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsChangesMade = true;

            if (!string.IsNullOrEmpty(this.textBoxSearchProduct.Text) || (string.Compare(this.textBoxSearchProduct.Text.Trim(), string.Empty) > 0))
            {
                // By Code
                if (this.cboSearch.SelectedIndex == 0)
                {
                    //this.DataViewProducts.RowFilter = "ProductCode LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredList = new ObservableCollection<Product>(this.ProductList.Where((prod) => prod.ProductCode.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
                else
                {
                    //this.DataViewProducts.RowFilter = "ProductDescription LIKE \'" + this.textBoxSearchProduct.Text.Trim() + "%\'";
                    this.ProductFilteredList = new ObservableCollection<Product>(this.ProductList.Where((prod) => prod.ProductDescription.ToLower().Contains(this.textBoxSearchProduct.Text.ToLower().Trim())));
                }
            }
            else
            {
                this.ProductFilteredList = this.ProductList;
            }

            dgProducts.ItemsSource = this.ProductFilteredList;

            if (this.ProductFilteredList.Count > 0)
            {
                this.dgProducts.SelectedItem = this.ProductFilteredList[0];
                this.SelectProduct(this.ProductFilteredList[0]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iSeletedIndex"></param>
        /// <remarks></remarks>
        private void SelectProduct(Product prod)
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

                Device.BeginInvokeOnMainThread(() =>
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
        private void LoadData()
        {
            this.LoadProductList();
            this.AddProductToObservableCollection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadProductList()
        {
            DataTable dataTableProducts = new DataTable();

            if (FilterOnAllowConvertProduct)
            {
                ProductInfoTasks.GetProductListAllowConvertProduct(ref dataTableProducts);
            }
            else
            {
                ProductInfoTasks.GetProductList(ref dataTableProducts);
            }

            dataTableProducts.TableName = "Products";
            this.DataViewProducts = dataTableProducts.DefaultView;
        }

        private void AddProductToObservableCollection()
        {
            this.ProductList.Clear();

            foreach (DataRow row in this.DataViewProducts.ToTable().Rows)
            {
                Product prod = new Product
                {
                    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                    ProductDescription = Convert.ToString(row["ProductDescription"]),
                    ProductCode = Convert.ToString(row["ProductCode"]),
                };

                this.ProductList.Add(prod);
            }

            this.ProductFilteredList = this.ProductList;
        }

        private void TextBoxSearchProduct_Focused(object sender, FocusEventArgs e)
        {
            this.textBoxSearchProduct.CursorPosition = 0;
            this.textBoxSearchProduct.SelectionLength = this.textBoxSearchProduct.Text != null ? this.textBoxSearchProduct.Text.Length : 0;
        }

        private void DgProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Product prod = e.SelectedItem as Product;
            this.SelectProduct(prod);
        }


        private async void CmdSelect_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        private async void CmdClose_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                await this.Close();
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void IdentifyChangesMade(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (this.IsControlLoaded)
        //        {
        //            this.IsChangesMade = true;

        //            if (sender == this.textBoxSearchProduct)
        //            {
        //                // Handled in this.textBoxSearchProduct_TextChanged
        //            }

        //            if (sender == this.cboSearch)
        //            {
        //            }

        //            if (sender == this.dgProducts)
        //            {
        //            }
        //        }
        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
        //        // Allow the validation to handle the setting for IsChangesMade.
        //        ex.Show(this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        Device.BeginInvokeOnMainThread(async () =>
        //        {
        //            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
        //            await errRpt.ShowDialog();
        //        });
        //    }
    }

    #endregion
}