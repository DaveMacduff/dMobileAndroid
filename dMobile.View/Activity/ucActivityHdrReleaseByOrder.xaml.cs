using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
    public partial class ucActivityHdrReleaseByOrder : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        private Controller.Activity activity = null;
        private SysCommon.ActSubTypes actSubTypes = SysCommon.ActSubTypes.ReleaseByOrder;
        private BarCodeParse parse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private int totalTicks = 0;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;

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
                            if (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                            {
                                this.ScannerDisable();

                                // SHIPPING LABELS
                                if (this.SysEnv.SysSettings.CanPrintShippingLabel && !this.SysEnv.SysSettings.IsDisconnectedMode)
                                {
                                    // Load activity properties, activity is an argument for the ShippingLabelControl
                                    // SelectedIdx is not refreshed.
                                    this.Activity.Refno = this.Activity.GetRefnoFromRowIdx(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));

                                    SysCommon.SLabelQuantity labelQuantity = new SysCommon.SLabelQuantity();
                                    labelQuantity = this.Activity.GetLabelQuantity();

                                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityShippingLabelEdit, this.ActSubType, labelQuantity, this.Activity, SysCommon.ShippingLabelQuantitySelectionMode.BothFullRepack);
                                    
                                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                                    {
                                        await form.SaveChanges(this, new EventArgs());
                                        XDocument request = form.GetXML();

                                        // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                        // If itm_refno is zero then no line is updated as printed.
                                        request.Element("params").Element("whs_id").AddAfterSelf(new XElement("itm_refno", 0));
                                        this.Activity.PrintLabel(request);
                                    }
                                }

                                if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                                {
                                    await this.RefreshData();
                                }
                                else
                                {
                                    this.Activity.DataViewHdrs.Delete(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                                }

                                this.ScannerEnable();
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: Resource strings
                            await DisplayAlert("Change Status", ex.Message, "OK");
                        }
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                        else
                        {
                            await this.CancelChanges();
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
                await DisplayAlert("Error", "this.ucActivityHdrReleaseByOrder_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ucActivityHdrReleaseByOrder_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

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
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                this.Scanner.Start();
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
                this.ScannerDisable();
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
        public XDocument GetXML()
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
                this.ScannerEnable();
                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

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
        public void DataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            int index = this.DataGridMainActivityItemsList.IndexOf(e.SelectedItem);

            if (index != -1)
            {
                this.Activity.SelectHdr(Convert.ToInt32(index));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdFilter_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityHdrPickingFilter, this.ActSubType);
            
            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                await form.SaveChanges(sender, e);
                XDocument xmlDoc = form.DisplayControlData;
                string filter = this.Activity.GetFilter(xmlDoc);

                this.Activity.DataViewHdrs.RowFilter = filter;

                // TODO: search
                if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                {
                    this.Activity.Search(txtSearchHdr.Text);
                }

                if (!string.IsNullOrEmpty(xmlDoc.Root.Element("sortby").Value))
                {
                    this.Activity.DataViewHdrs.Sort = xmlDoc.Root.Element("sortby").Value;
                }
            }

            this.ScannerEnable();
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //public void tmrActivityLoaded_Tick(object sender, EventArgs e)
        //{
        //    this.TotalTicks = this.TotalTicks + this.tmrActivityLoaded.Interval;

        //    if (this.Enabled && this.TotalTicks >= 1000)
        //    {
        //        this.tmrActivityLoaded.Enabled = false;

        //        // Moved to gotFocus
        //        //this.RefreshData();
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void ucActivityHdrReleaseByOrder_LostFocus(object sender, EventArgs e)
        {
            //if (this.tmrActivityLoaded.Enabled == false)
            //{
            if (this.Scanner != null)
            {
                this.ScannerDisable();
            }
            //}
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public void cmdAdd_Click(object sender, EventArgs e)
        {
            XDocument xmlDoc = null;
            this.Activity.Insert(xmlDoc);
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
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            string message = string.Empty;
            //int currRowIdx = this.dataGridMainActivity.CurrentRowIndex;
            int currRowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
            string savedViewSort = string.Empty;

            this.Parse = scanArgs.ParseInfo;

            this.ScannerDisable();

            if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
            {
                // this.Parse.ActivityFKRefno e.g. format is 'OE:12345678' start at 4th pos for the fk refno
                string actStringFKRefno = this.Parse.ActFKRefno;

                if (!string.IsNullOrEmpty(actStringFKRefno))
                {
                    savedViewSort = this.Activity.DataViewHdrs.Sort;
                    this.Activity.DataViewHdrs.Sort = this.Activity.GetFKRefnoIdx();

                    //if (this.Activity.DataViewHdrs.Count > 0)
                    //{
                    //    this.dataGridMainActivity.UnSelect(currRowIdx);
                    //}

                    int rowIdx = this.Activity.DataViewHdrs.Find(actStringFKRefno);
                    this.Activity.DataViewHdrs.Sort = savedViewSort;

                    if (rowIdx >= 0)
                    {
                        //this.dataGridMainActivity.CurrentRowIndex = rowIdx;
                        this.Activity.GetRefnoFromRowIdx(rowIdx);

                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityItem, this.Activity, this.ActSubType);
                        await form.DisplayControl.ShowDialog();
                    }
                    else
                    {
                        this.Activity.FKRefno = 0;
                        this.Activity.Refno = 0;

                        // TODO: Resource strings
                        message = string.Format("Unable To Find Reference# {0}", actStringFKRefno);
                        //MessageBox.Show(message, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        // TODO: Resource strings
                        await DisplayAlert(this.GetControlDescription(), message, "OK");
                    }
                }
                else
                {
                    // TODO: Resource strings
                    message = string.Format("Unable To Parse Reference# {0}", actStringFKRefno);
                    //MessageBox.Show(message, this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), message, "OK");
                }
            }
            else if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
            {
                //MessageBox.Show("Scanning an activity item reference # is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Scanning an activity item reference # is not supported here.", "OK");
            }
            else if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
            {
                //MessageBox.Show("Scanning a product bar code is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Scanning a product bar code is not supported here.", "OK");
            }
            else if (!string.IsNullOrEmpty(this.Parse.SSCC18))
            {
                //MessageBox.Show("Scanning a shipping container bar code is not supported here!", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Scanning a shipping container bar code is not supported here!", "OK");
            }
            else if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
            {
                //MessageBox.Show("Scanning a location identifier bar code is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Scanning a location identifier bar code is not supported here.", "OK");
            }
            else
            {
                //MessageBox.Show("Scanned bar code type is not supported here.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Scanned bar code type is not supported here.", "OK");
            }

            this.ScannerEnable();
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
                    this.Activity.Refresh(null);
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

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ItemsList.Clear();

                    this.Activity.DataViewHdrs = this.Activity.GetDataView(false);
                    this.DataGridMainActivityItemsList = this.Activity.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity, ref dataGridWidth);

                    if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                    {
                        this.DataGridMainActivityItemsList = this.Activity.Search(this.txtSearchHdr.Text);
                    }

                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                    if (this.Activity.DataViewHdrs.Count > 0 && this.DataGridMainActivityItemsList.Count > 0 && this.DataGridMainActivityItemsList[0] != null)
                    {
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                        this.Activity.SelectHdr(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                    }
                });
            });

            this.Loading = false;
        }

        #endregion
    }
}