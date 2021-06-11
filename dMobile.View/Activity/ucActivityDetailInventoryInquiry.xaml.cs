using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using Java.Util.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityDetailInventoryInquiry : DMobileModal, IAppArea
    {
        #region Fields

        private ActDetail actDetail;
        private SysCommon.ActSubTypes actSubType;
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
        /// <param name="actDetail"></param>
        /// <param name="actSubType"></param>
        public ucActivityDetailInventoryInquiry(ActDetail actDetail, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.SysEnv = SysEnvironment.GetInstance();
            this.BarScanner = BarCodeSingleton.GetInstance().Scanner;
            this.ActSubType = actSubType;
            this.ActDetail = actDetail;
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

            try
            {
                await this.ucActivityDetailInventoryInquiry_GotFocus(this, new EventArgs());
                await this.tmrActivityItemInventoryInquiry_Tick(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: logging
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "ucActivityDetailInventoryInquiry_GotFocus/tmrActivityItemInventoryInquiry_Tick error: " + ex.Message, "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ScannerDisable();
            base.OnDisappearing();
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

        public SysCommon.ActSubTypes ActSubType
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

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
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
            return "Inventory Inquiry Detail";
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
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
                    // this.IsChangesMade = false;
                }
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;
                    await this.ScannerDisable();
                    await Device.InvokeOnMainThreadAsync(() => this.ItemsList.Clear());

                    await this.ActDetail.Refresh();

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                        if (this.ActDetail.ListDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                            await this.ActDetail.SelectActivityDetail(this.ActDetail.ListDetails[0]);
                        }
                    });
                }
                catch (NotImplementedException)
                {
                    // Handle this.ActDetail.SelectActivityDetail not implemented
                    // TODO: Logging
                    Console.WriteLine(this.GetControlDescription() + " Not implemented exception");
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
        /// <remarks></remarks>
        public async Task tmrActivityItemInventoryInquiry_Tick(object sender, EventArgs e)
        {
            await this.RefreshData();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs"></param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            try
            {
                this.Loading = true;
                this.BarParse = scanArgs.ParseInfo;
                await this.ScannerDisable();

                //<issue="13609" author="RD" date="2016/02/22"/>
                if (ActDetail != null && ActDetail.ActItem != null)
                {
                    SysShared.AssertSuppressedErrorsAgainstScheme(ActDetail.ActItem.ProdInfo, this.BarParse);
                }

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
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
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
        /// <param name="e"></param>
        private async Task ucActivityDetailInventoryInquiry_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();
            }
        }

        #endregion

        private void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}