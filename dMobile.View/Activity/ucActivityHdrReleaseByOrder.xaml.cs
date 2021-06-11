using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Views;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrReleaseByOrder : DMobileContentPage, IAppArea
    {
        #region Fields

        private Controller.Activity activity = null;
        private SysCommon.ActSubTypes actSubTypes = SysCommon.ActSubTypes.ReleaseByOrder;
        private BarCodeParse parse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        private List<string> optionsMenuItems = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityHdrReleaseByOrder(SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.cmdFilter.Text = "Filter";
            this.cmdAdd.Text = "Add";
            this.cmdAdd.IsEnabled = false;

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;

            this.ActSubType = actSubTypes;
            this.Activity = ActivityFacade.GetActivity(this.ActSubType);
            this.Activity.ActSubType = this.ActSubType;

            this.Title = this.GetControlDescription();
            this.ItemsList = new ObservableCollection<string>();
            this.DataGridMainActivityItemsList = new ObservableCollection<object>();
            this.optionsMenuItems.Add("Release order");
        }

        #endregion

        #region keyEvents

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.F4 && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        try
                        {
                            await ReleaseOrder();
                        }
                        catch (Exception ex)
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Change Status", ex.Message, "OK"));
                        }
                    }
                }
            });

            try
            {
                await this.ucActivityHdrReleaseByOrder_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityHdrReleaseByOrder_GotFocus threw an error " + ex, "OK"));
            }
        }

        private async Task ReleaseOrder()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
            {
                await this.ScannerDisable();

                // SHIPPING LABELS
                if (this.SysEnv.SysSettings.CanPrintShippingLabel && !this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    // Load activity properties, activity is an argument for the ShippingLabelControl
                    // SelectedIdx is not refreshed.
                    this.Activity.Refno = this.Activity.GetRefnoFromRowIdx(this.dataGridMainActivity.SelectedItem);

                    SysCommon.SLabelQuantity labelQuantity = new SysCommon.SLabelQuantity();
                    labelQuantity = await this.Activity.GetLabelQuantity();

                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityShippingLabelEdit, this.ActSubType, labelQuantity, this.Activity, SysCommon.ShippingLabelQuantitySelectionMode.BothFullRepack);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        XDocument request = await form.GetXML();

                        // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                        // If itm_refno is zero then no line is updated as printed.
                        request.Element("params").Element("whs_id").AddAfterSelf(new XElement("itm_refno", 0));
                        await this.Activity.PrintLabel(request);
                    }
                }

                if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                {
                    await this.RefreshData();
                }
                else
                {
                    this.Activity.ListHdrs.Remove(this.dataGridMainActivity.SelectedItem);

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.ItemsList.Clear();
                        this.DataGridMainActivityItemsList = this.Activity.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);

                        if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                        {
                            this.DataGridMainActivityItemsList = this.Activity.Search(this.txtSearchHdr.Text);
                        }

                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                        if (this.Activity.ListHdrs.Count > 0 && this.DataGridMainActivityItemsList.Count > 0 && this.DataGridMainActivityItemsList[0] != null)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                            await this.Activity.SelectActivity(this.dataGridMainActivity.SelectedItem);
                        }
                    });
                }

                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ucActivityHdrReleaseByOrder_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else
                {
                    await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

        private BarCodeParse Parse
        {
            get
            {
                return this.parse;
            }

            set
            {
                this.parse = value;
            }
        }

        private BarCodeScanner.BarCodeScanner Scanner
        {
            get
            {
                return this.scanner;
            }

            set
            {
                this.scanner = value;
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

        private bool IsBarCodeHandlerRegistered
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

        private SysCommon.ActSubTypes ActSubType
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
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
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
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            if (this.Scanner != null)
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                await this.ScannerDisable();
            }

            return true;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    UserControl control = new ucSalesOrdersMenu();

        //    return control;
        //}

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
        public async Task<bool> SaveChanges()
        {
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
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
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
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async Task ucActivityHdrReleaseByOrder_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.ScannerEnable();
                await this.RefreshData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.DataGridMainActivityItemsList = this.Activity.Search(txtSearchHdr.Text);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void DataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.Activity.SelectActivity(e.SelectedItem);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdFilter_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityHdrPickingFilter, this.ActSubType);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = await form.GetXML();
                this.Activity.GetFilter(xmlDoc);

                // TODO: Fix filtering
                //this.Activity.DataViewHdrs.RowFilter = filter;

                if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                {
                    this.Activity.Search(txtSearchHdr.Text);
                }

                if (!string.IsNullOrEmpty(xmlDoc.Root.Element("sortby").Value))
                {
                    // TODO: Fix sorting
                    //this.Activity.DataViewHdrs.Sort = xmlDoc.Root.Element("sortby").Value;
                }
            }

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async Task ucActivityHdrReleaseByOrder_LostFocus(object sender, EventArgs e)
        {
            //if (this.tmrActivityLoaded.Enabled == false)
            //{
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }
            //}
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAdd_Click(object sender, EventArgs e)
        {
            XDocument xmlDoc = null;
            await this.Activity.Insert(xmlDoc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void txtSearchHdr_GotFocus(object sender, EventArgs e)
        {
            if (this.txtSearchHdr.Text != null)
            {
                this.txtSearchHdr.CursorPosition = 0;
                this.txtSearchHdr.SelectionLength = this.txtSearchHdr.Text.Length;
            }
        }

        #endregion

        #region Private Members

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

            try
            {
                this.Loading = true;

                string message = string.Empty;
                //int currRowIdx = this.dataGridMainActivity.CurrentRowIndex;
                //int currRowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
                string savedViewSort = string.Empty;

                this.Parse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
                {
                    // this.Parse.ActivityFKRefno e.g. format is 'OE:12345678' start at 4th pos for the fk refno
                    string actStringFKRefno = this.Parse.ActFKRefno;

                    if (!string.IsNullOrEmpty(actStringFKRefno))
                    {
                        // TODO: Fix sorting
                        //savedViewSort = this.Activity.DataViewHdrs.Sort;
                        //this.Activity.DataViewHdrs.Sort = this.Activity.GetFKRefnoIdx();

                        object selectedActivity = this.Activity.ListHdrs.OfType<object>().Where(activity => Convert.ToString(activity.GetType().GetProperty(this.Activity.GetFKRefnoIdx()).GetValue(activity)) == actStringFKRefno).FirstOrDefault();

                        // TODO: Fix sorting
                        //this.Activity.DataViewHdrs.Sort = savedViewSort;

                        if (selectedActivity != null)
                        {
                            //this.dataGridMainActivity.CurrentRowIndex = rowIdx;
                            this.Activity.GetRefnoFromRowIdx(selectedActivity);

                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityItem, this.Activity, this.ActSubType);
                            await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                        }
                        else
                        {
                            this.Activity.FKRefno = 0;
                            this.Activity.Refno = 0;

                            // TODO: Resource strings
                            message = string.Format("Unable To Find Reference# {0}", actStringFKRefno);
                            //MessageBox.Show(message, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), message, "OK"));
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        message = string.Format("Unable To Parse Reference# {0}", actStringFKRefno);
                        //MessageBox.Show(message, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), message, "OK"));
                    }
                }
                else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
                {
                    //MessageBox.Show("Scanning an activity item reference # is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning an activity item reference # is not supported here.", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    //MessageBox.Show("Scanning a product bar code is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a product bar code is not supported here.", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
                {
                    //MessageBox.Show("Scanning a shipping container bar code is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK"));
                }
                else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    //MessageBox.Show("Scanning a location identifier bar code is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanning a location identifier bar code is not supported here.", "OK"));
                }
                else
                {
                    //MessageBox.Show("Scanned bar code type is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here.", "OK"));
                }
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error scanning barcode", ex.Message, "OK"));
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
        private async Task RefreshData()
        {
            this.Loading = true;

            await Task.Run(async () =>
            {
                try
                {
                    await this.Activity.Refresh(null);
                }
                catch (SQLiteException ex)
                {
                    for (int curExNdx = 0; curExNdx <= ex.Data.Count; curExNdx++)
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), Convert.ToString("Error " + Convert.ToString(ex.Data[curExNdx])), "OK"));
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), ex.Message, "OK"));
                }
                finally
                {
                    this.Loading = false;
                }

                this.Activity.ListHdrs = await this.Activity.GetActivityList(false);

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ItemsList.Clear();
                    this.DataGridMainActivityItemsList = this.Activity.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);

                    if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                    {
                        this.DataGridMainActivityItemsList = this.Activity.Search(this.txtSearchHdr.Text);
                    }

                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                    if (this.Activity.ListHdrs.Count > 0 && this.DataGridMainActivityItemsList.Count > 0 && this.DataGridMainActivityItemsList[0] != null)
                    {
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                        await this.Activity.SelectActivity(this.dataGridMainActivity.SelectedItem);
                    }
                });
            });

            this.Loading = false;
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            // TODO: Resource strings
            string result = await DisplayActionSheet("Release Order Options", "Close", null, this.optionsMenuItems.ToArray());

            if (result == "Release order")
            {
                await this.ReleaseOrder();
            }
        }

        #endregion
    }
}