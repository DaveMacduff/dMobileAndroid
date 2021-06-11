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
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItemInventoryInquiry : DMobileModal, IAppArea
    {
        #region Fields

        private ActItem actItem;
        private SysCommon.ActSubTypes actSubTypes;
        private BarCodeParse barParse;
        private BarCodeScanner.BarCodeScanner barScanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private bool isEnterTrapped = false;
        private int totalTicks = 0;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityItemInventoryInquiry(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.SysEnv = SysEnvironment.GetInstance();
            this.BarScanner = BarCodeSingleton.GetInstance().Scanner;

            this.ActSubType = actSubTypes;
            this.ActItem = actItem;

            this.Title = this.GetControlDescription();
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        ProductInfo svdProdInfo = this.ActItem.ProdInfo;

                        try
                        {
                            this.IsEnterTrapped = true;
                            this.IsEnabled = false;

                            await this.ScannerDisable();
                            int index = 0;

                            if (this.dataGridMainActivity.SelectedItem != null && this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) >= 0)
                            {
                                index = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

                                // TODO: test
                                this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromCodeBCUnit(Convert.ToString((this.ActItem.ListItems[index] as dynamic).productcode),
                                    Convert.ToString((this.ActItem.ListItems[index] as dynamic).productunitbc),
                                    Convert.ToString((this.ActItem.ListItems[index] as dynamic).unitcode));

                                ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.InventoryInquiry);

                                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityInventoryInquiryDetail, actDetail, this.ActSubType);
                                await form.DisplayControl.ShowDialog();
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
                            this.ActItem.ProdInfo = svdProdInfo;
                            await this.ScannerEnable();

                            this.IsEnabled = true;
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
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ActItemInventoryInquiry_GotFocus threw an error " + ex, "OK"));
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
                    await this.CancelChanges();
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

        public bool IsEnterTrapped
        {
            get
            {
                return this.isEnterTrapped;
            }

            set
            {
                this.isEnterTrapped = value;
            }
        }

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

        private int TotalTicks
        {
            get
            {
                return this.totalTicks;
            }

            set
            {
                this.totalTicks = value;
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
            await this.ScannerDisable();
            await Device.InvokeOnMainThreadAsync(() => this.ItemsList.Clear());

            try
            {
                XDocument xmldocParams = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "</params>");

                await this.ActItem.Refresh(xmldocParams);
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.DataGridMainActivityItemsList = this.ActItem.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            if (this.ActItem.ListItems.Count > 0)
            {
                await this.ActItem.SelectActivityItem(this.ActItem.ListItems[0]);
            }

            await this.ScannerEnable();
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
    }
}