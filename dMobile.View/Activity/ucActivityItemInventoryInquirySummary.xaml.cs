using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Views;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.Resources.Enums;
using dMobile.Resources.Types;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItemInventoryInquirySummary : DMobileModal, IAppArea, IDataLayerEventListener
    {
        #region Fields

        private Controller.Activity activity;
        private ActItem actItem;
        private SysCommon.ActSubTypes actSubTypes;
        private BarCodeParse barParse;
        private BarCodeScanner.BarCodeScanner barScanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityItemInventoryInquirySummary(ActItem actItem)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.BarScanner = BarCodeSingleton.GetInstance().Scanner;
                this.ActSubType = SysCommon.ActSubTypes.InventoryInquirySummary;
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);

                if (this.Activity.SkidInfo == null)
                {
                    this.Activity.SkidInfo = new SysSkidInfo();
                }

                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.InventoryInquirySummary);
                this.ActItem.ProdInfo = actItem.ProdInfo;
                this.ActItem.LocMap = actItem.LocMap;
                this.ActItem.SysEnv = this.SysEnv;
            }
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
            this.lstDisplay.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        ProductInfo svdProdInfo = this.ActItem.ProdInfo;

                        try
                        {
                            this.IsEnabled = false;

                            await this.ScannerDisable();

                            int currentIndex = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

                            if (currentIndex < 0)
                            {
                                return;
                            }

                            // TODO: Test
                            this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromCodeBCUnit(
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as ActivityItem).ProductCode),
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as ActivityItem).ProductUnitBC),
                                Convert.ToString((this.ActItem.ListItems[currentIndex] as ActivityItem).UnitCode));

                            ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.InventoryInquiry);

                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityInventoryInquiryDetail, actDetail, this.ActSubType);
                                await form.DisplayControl.ShowDialog();
                            });
                        }
                        catch (Exception ex)
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                FormErrorReport errRpt = new FormErrorReport(ex);
                                await errRpt.ShowDialog();
                            });
                        }
                        finally
                        {
                            this.ActItem.ProdInfo = svdProdInfo;

                            await this.ScannerEnable();

                            this.IsEnabled = true;
                        }
                    }
                    else if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await this.CancelChanges();
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                await this.ActItemInventoryInquiry_GotFocus(this, new EventArgs());
                await this.RefreshData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActItemInventoryInquiry_GotFocus error: " + ex.Message);
            }
        }

        /// <summary>
        /// The OnDisappearing
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
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

        public SysEnvironment SysEnv
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

        public bool IsBarCodeHandlerRegistered
        {
            get
            {
                return this.isBarCodeHandlerRegistered;
            }

            set
            {
                this.isBarCodeHandlerRegistered = value;
            }
        }

        public ActItem ActItem
        {
            get
            {
                return this.actItem;
            }

            set
            {
                this.actItem = value;
            }
        }


        private Controller.Activity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
            }
        }

        public SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubTypes;
            }

            set
            {
                this.actSubTypes = value;
            }
        }

        private BarCodeParse BarParse
        {
            get
            {
                return this.barParse;
            }

            set
            {
                this.barParse = value;
            }
        }

        private BarCodeScanner.BarCodeScanner BarScanner
        {
            get
            {
                return this.barScanner;
            }

            set
            {
                this.barScanner = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

        #endregion

        #region Public Members

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.BarScanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.BarScanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                await this.BarScanner.Start();
                this.BarScanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
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
            return "Inventory Inquiry Summary";
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
            return this.IsValidData();
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
                if (this.BarScanner != null)
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
                this.IsEnabled = false;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsEnabled = true;
            }

            return true;
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
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

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
        /// <remarks></remarks>
        public async Task RefreshData()
        {
            try
            {
                this.Loading = true;

                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(() => this.ItemsList.Clear());

                this.ActItem.RegLayerListenerProgOut(this);
                XDocument xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "</params>");

                await this.ActItem.Refresh(xmldocParams);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.DataGridMainActivityItemsList = this.ActItem.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                    this.ItemsList.Clear();
                });

                if (this.DataGridMainActivityItemsList.Count > 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0]);
                    // TODO: test
                    await this.ActItem.SelectActivityItem(this.ActItem.ListItems[0]);
                }

                await Device.InvokeOnMainThreadAsync(() => DisplaySummaryData());
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ActItem.UnregLayerListenerProgOut(this);
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        /// Adds data to the bottom list box to show
        /// Product ID, Qty and Weight.
        /// </summary>
        private void DisplaySummaryData()
        {
            ActivityItem selectedItem = this.ActItem.SelectedActivityItem as ActivityItem;
            ProductInfo prodInfo = this.ActItem.ProdInfo;

            this.ItemsList.Clear();

            if (selectedItem != null)
            {
                // TODO: Resource strings
                this.ItemsList.Add(string.Format(
                    "Quantity {0} {1} @ {2} {3}",
                    selectedItem.ActivityItemQuantity,
                    selectedItem.UnitCode,
                    selectedItem.ActivityItemWeightQuantity,
                    selectedItem.WeightUnitCode));

                if (this.ActItem.LocMap.CurrLocMap.IsSectionTracked)
                {
                    if (this.ActItem.LocMap.CurrLocMap.IsSlotTracked)
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(string.Format("Section {0} Slot {1}", selectedItem.LocationMapSectionFKRefno, selectedItem.LocationMapSlotFKRefno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.ItemsList.Add(string.Format("Section {0}", selectedItem.LocationMapSectionFKRefno));
                    }
                }
            }
            else
            {
                // TODO: Resource strings
                this.ItemsList.Add("No product at selected location.");
            }

            // TODO: Resource strings
            this.ItemsList.Add(string.Format("Product ID {0}", prodInfo.Code));
            this.ItemsList.Add(string.Format("Whs {0} {1}", this.ActItem.LocMap.CurrLocMap.WhsFKRefno, this.ActItem.LocMap.CurrLocMap.WhsName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task ActItemInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsValidData()
        {
            return true;
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

            this.Loading = true;
            this.BarParse = scanArgs.ParseInfo;
            await this.ScannerDisable();

            try
            {
                if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning an activity reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning an activity item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning an product identifier is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a shipping container bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    // TODO: Resource strings
                    throw new Exception("Scanning a global location identifier is not supported here.");
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception("Scanned bar code type is not supported here.");
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        #endregion

        #region IDataLayerEventListener Members

        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        #endregion

        private void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.ActItem.SelectedActivityItem = e.SelectedItem;
            DisplaySummaryData();
        }
    }
}