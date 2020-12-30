namespace dMobile.View.Activity
{
    using Android.Util;
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using static Android.Views.View;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdr : DMobileContentPage, IAppArea, IDataLayerEventListener, IAppPublisher
    {
        private Controller.Activity actHdr;
        private SysCommon.ActSubTypes actSubType;
        private BarCodeParse barParse;
        private BarCodeScanner barScanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();
        private List<string> optionsMenuItems = new List<string>();
        private string filterText;
        private bool filterIsEnabled;
        private bool isRefreshing;

        // For handling double click on listview item
        private int numTaps = 0;
        private object previousSender;

        private bool firstOnAppearing = true;

        public ucActivityHdr()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityHdr(SysCommon.ActSubTypes activitySubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdAdd.Text = "Add";
            this.FilterText = "Filter";
            
            this.cmdAdd.IsEnabled = false;
            this.FilterIsEnabled = true;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.BarScanner = BarCodeSingleton.GetInstance().Scanner;

                this.ActSubType = activitySubType;
                this.ActHdr = ActivityFacade.GetActivity(this.ActSubType);

                if (this.ActHdr.SkidInfo == null)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                }

                this.Title = this.GetControlDescription();
            }
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(FilterIsEnabled) || propertyName == nameof(FilterText))
            {
                this.PopulateOptionsMenu();
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
            this.dataGridMainActivity.ItemTapped += this.DataGridMainActivity_ItemTapped;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        if (this.dataGridMainActivity.SelectedItem != null && (Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) > -1))
                        {
                            await this.DataGridMainActivity_KeyUpEnter(this, null);
                        }
                    }
                    else if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.F4 && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // Release order.
                        this.DataGridMainActivity_KeyUpF4(this, null);
                    }
                    else if (keycode == Keycode.F5)
                    {
                        // Change pick selection.
                        this.DataGridMainActivity_KeyUpF5(this, null);
                    }
                    else if (keycode == Keycode.DpadDown)
                    {
                        if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) < this.DataGridMainActivityItemsList.Count - 1)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) + 1];
                            this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                    else if (keycode == Keycode.DpadUp)
                    {
                        if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) > 0)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) - 1];
                            this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                        }
                    }
                }
            });

            try
            {
                this.ucActivityHdr_GotFocus(this, new EventArgs());
                await this.tmrActivityLoaded_Tick(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityHdr_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.dataGridMainActivity.ItemTapped -= this.DataGridMainActivity_ItemTapped;
            this.ScannerDisable();

            base.OnDisappearing();
        }

        /// <summary>
        /// Handle double click on listview item
        /// </summary>
        private async void DataGridMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            this.numTaps++;

            if (this.previousSender == e.Item)
            {
                if (this.numTaps == 2)
                {
                    this.numTaps = 0;

                    if (this.dataGridMainActivity.SelectedItem != null && (Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) > -1))
                    {
                        await this.DataGridMainActivity_KeyUpEnter(this, null);
                    }
                }
            }
            else
            {
                this.previousSender = e.Item;
                this.numTaps = 1;
            }

            await Task.Delay(500);
            this.numTaps = 0;
        }

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

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

        private BarCodeScanner BarScanner
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

        private Controller.Activity ActHdr
        {
            get
            {
                return this.actHdr;
            }

            set
            {
                this.actHdr = value;
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }
        public List<string> OptionsMenuItems { get => optionsMenuItems; set => optionsMenuItems = value; }
        public string FilterText { get => filterText; set { filterText = value; this.OnPropertyChanged(); } }
        public bool FilterIsEnabled { get => filterIsEnabled; set { filterIsEnabled = value; this.OnPropertyChanged(); } }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.BarScanner != null)
            {
                this.BarScanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.BarScanner.Stop();
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
                this.BarScanner.Start();
                this.BarScanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
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
            bool isReadyToClose = !this.IsChangesMade;

            try
            {
                if (isReadyToClose && (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
                {
                    if (this.SysEnv.SysSettings.IsPickConfirmExit)
                    {
                        //if (MessageBox.Show("Confirm exit?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        //{
                        //    isReadyToClose = false;
                        //}

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            // TODO: Resource strings
                            await DisplayAlert(this.GetControlDescription(), "Confirm exit?", "Yes", "No").ContinueWith(result =>
                            {
                                if (!result.Result)
                                {
                                    isReadyToClose = false;
                                }

                                if (isReadyToClose)
                                {
                                    // NOTE: This code is needed to clean up the scanner driver.
                                    // Symbol had threading issues without this call when dMobile was terminating.
                                    // All user controls that initalize a bar code need this code.
                                    // The parent form (not a user control) will call scanner.terminate and dispose
                                    // of the scanner instance upon exiting dMobile.
                                    this.ScannerDisable();
                                }
                            });
                        });
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <returns></returns>
        //        /// <remarks></remarks>
        //        public UserControl GetCallingControl()
        //        {
        //            UserControl control = null;

        //            switch (this.ActSubType)
        //            {
        //                case SysCommon.ActSubTypes.Adjustment:
        //                    bool controlSet = false;
        //                    AdjActivity adjAct = (actHdr as AdjActivity);

        //                    if (adjAct != null)
        //                    {
        //                        AdjustmentSettings settings = adjAct.AdjSettings;

        //                        if (settings != null && settings.AdjustmentType == SysCommon.AdjTypes.Debox)
        //                        {
        //                            control = new ucProductionTicketMenu();
        //                            controlSet = true;
        //                        }
        //                    }

        //                    if (!controlSet)
        //                        control = new ucInventoryAdjustmentsMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.PhysicalCounts:
        //                    control = new ucInventoryCountsMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.CycleCounts:
        //                    control = new ucInventoryCountsMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.Picking:
        //                    control = new ucSalesOrdersMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.PickingSingle:
        //                    control = new ucSalesOrdersMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.Returns:
        //                    control = new ucSalesOrdersMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.Receiving:
        //                    control = new ucPurchaseOrdersMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.Transfers:
        //                    control = new ucInventoryMovementMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.TicketFinished:
        //                    control = new ucProductionTicketMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.TicketMaterial:
        //                    control = new ucProductionTicketMenu();
        //                    break;

        //                case SysCommon.ActSubTypes.TicketAddProduct:
        //                    control = new ucProductionTicketMenu();
        //                    break;

        //            }

        //            if (control == null)
        //            {
        //#if MULTILANGUAGE
        //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.CaseStatementForActsubtype) + this.ActSubType
        //            + Translator.GetInstance().Translate(TranslatorVocabulary.WasNotDefinedInGetcallingcontrol));
        //#else
        //                throw new Exception("Case statement for ActSubType '" + this.ActSubType
        //                    + "' was not defined in GetCallingControl.");
        //#endif
        //            }

        //            return control;
        //        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return this.ActHdr.GetControlDescription();
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
                    saveComplete = true;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exception("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                saveComplete = false;

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
                    cancelChanges = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
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
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            //this.lstDisplay.Items.Clear();
            this.ItemsList.Clear();

            if (this.ActHdr.DataViewHdrs.Count > 0)
            {
                const int LineLength = 30;
                List<string> lines = SysCommon.WordWrap2List(dataLayerEventArgs.StatusOutMsg, LineLength);

                for (int i = 0; i < lines.Count; i++)
                {
                    //this.lstDisplay.Items.Add(lines[i]);
                    this.ItemsList.Add(lines[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            //if (dataLayerEventArgs.SetMinMaxValue)
            //{
            //    this.pbarStatus.Minimum = dataLayerEventArgs.MinValue;
            //    this.pbarStatus.Maximum = dataLayerEventArgs.MaxValue;
            //}

            //this.pbarStatus.Value = dataLayerEventArgs.CurrentValue;
        }


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
            string savedViewSort = string.Empty;
            this.BarParse = scanArgs.ParseInfo;

            await Task.Run(async () =>
            {
                this.Loading = true;

                this.ScannerDisable();

                try
                {
                    savedViewSort = this.ActHdr.DataViewHdrs.Sort;

                    if (!string.IsNullOrEmpty(this.BarParse.ActFKRefno))
                    {
                        // this.BarParse.ActFKRefno e.g. format is 'OE:12345678' 
                        // Start at 3th pos for the fk refno
                        string actHdrStringFKRefno = this.BarParse.ActFKRefno;

                        if (!string.IsNullOrEmpty(actHdrStringFKRefno))
                        {
                            this.ActHdr.DataViewHdrs.Sort = this.ActHdr.GetFKRefnoIdx();

                            if (this.ActHdr.DataViewHdrs.Count > 0)
                            {
                                //this.dataGridMainActivity.UnSelect(currRowIdx);
                            }

                            int rowIdx = this.ActHdr.DataViewHdrs.Find(actHdrStringFKRefno);

                            if (rowIdx >= 0)
                            {
                                //this.dataGridMainActivity.UnSelect(this.dataGridMainActivity.CurrentRowIndex);
                                //this.dataGridMainActivity.CurrentRowIndex = rowIdx;
                                this.ActHdr.Refno = this.ActHdr.GetRefnoFromRowIdx(rowIdx);

                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityItem, this.ActHdr, this.ActSubType);
                                    this.IsChangesMade = false;
                                    await form.DisplayControl.ShowDialog();
                                });

                                //this.dataGridMainActivity.Select(rowIdx);
                            }
                            else
                            {
                                this.ActHdr.FKRefno = 0;
                                this.ActHdr.Refno = 0;

                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException(string.Format("Unable to find reference # {0}", actHdrStringFKRefno));
                            }
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException(string.Format("Unable to parse reference # {0}", actHdrStringFKRefno));
                        }
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an activity item reference # is not supported here!");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a product bar code is not supported here!");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.SSCC18))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                    }
                    else if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a location identifier bar code is not supported here!");
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanned bar code type is not supported here!");
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(() => ex.Show(this.ActSubType));
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
                    this.Loading = false;
                    this.ActHdr.DataViewHdrs.Sort = savedViewSort;
                    this.ScannerEnable();
                }
            });
        }

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();
            
            if (this.FilterIsEnabled)
            {
                this.OptionsMenuItems.Add(this.FilterText);
            }
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            try
            {
                if (!this.isRefreshing)
                {
                    this.isRefreshing = true;

                    await Task.Run(async () =>
                    {
                        this.Loading = true;

                        try
                        {
                            this.ActHdr.RegLayerListenerProgOut(this);
                            this.ActHdr.Refresh(this.ActHdr.XMLDataCriteria);
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
                            this.ActHdr.UnregLayerListenerProgOut(this);
                        }

                        await Device.InvokeOnMainThreadAsync(() => this.ItemsList.Clear());
                        this.ActHdr.DataViewHdrs = this.ActHdr.GetDataView(false);

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.DataGridMainActivityItemsList = this.ActHdr.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);

                            if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                            {
                                this.DataGridMainActivityItemsList = this.ActHdr.Search(this.txtSearchHdr.Text);
                            }

                            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                            if (this.ActHdr.DataViewHdrs.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                            {
                                if (this.dataGridMainActivity.SelectedItem == null)
                                {
                                    this.ActHdr.SelectHdr(0);
                                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                                }
                                else
                                {
                                    this.ActHdr.SelectHdr(this.ActHdr.SelectedIndex);
                                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.ActHdr.SelectedIndex];
                                }

                                this.dataGridMainActivity.Focus();
                            }

                            this.DisplayData();
                        });

                        this.Loading = false;
                    });
                }
                else
                {
                    Console.WriteLine("activity is still refreshing...");
                }
            }
            catch (Exception ex)
            {
                // TODO: logging
                // TODO: Resource strings
                await DisplayAlert("Error", "An error occurred while refreshing data: " + ex.Message, "OK");
            }
            finally
            {
                this.isRefreshing = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityHdr_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task tmrActivityLoaded_Tick(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                bool performRefreshData = true;

                try
                {
                    this.ActHdr.XMLDataCriteria = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                                "<params>" + "<whs_id>0</whs_id>" + "<sect_refno>0</sect_refno>" + "</params>");

                    if ((this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle) && this.SysEnv.SysSettings.IsPrePickCrit && this.firstOnAppearing)
                    {
                        this.firstOnAppearing = false;
                        this.ScannerDisable();
                        int sectFKRefno = 0;

                        // Scanning in the picking selection criteria form is possible, which means the
                        // the last parse bar code data would be overwritten.
                        // However because the bar code parse object at this point does not contain any valuable data we do not
                        // need to preserve it before calling the picking selection form.
                        FormDialogContainer formPickSelect = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPickingSelectCriteria, this.ActSubType);

                        if (await Device.InvokeOnMainThreadAsync(async () => await formPickSelect.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            XDocument xmlResult = formPickSelect.GetXML();
                            LocationMap locMap = LocationMap.GetInstance();
                            SLocMap tmpLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(int.Parse(xmlResult.Root.Element("LocationMapRefno").Value));
                            locMap.CurrLocMap = tmpLocMap;
                            sectFKRefno = tmpLocMap.SectFKRefno;

                            this.ActHdr.XMLDataCriteria = XDocument.Parse(
                                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                        + "<params>"
                                        + ("<whs_id>" + tmpLocMap.WhsFKRefno + "</whs_id>")
                                        + ("<sect_refno>" + tmpLocMap.SectFKRefno + "</sect_refno>")
                                        + "</params>");
                        }

                        // See WO#12906 do not load any orders if no section was selected.
                        if (this.SysEnv.SysSettings.IsPickingModuleDisallowLoadingAllOrders && sectFKRefno == 0)
                        {
                            performRefreshData = false;
                        }

                        this.ScannerEnable();
                    }

                    if (performRefreshData)
                    {
                        await this.RefreshData();
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
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
                finally
                {
                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.isControlLoaded = true;

                    // See WO#12906 do not load any orders if no section was selected.
                    // Close picking activity.
                    if (!performRefreshData)
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayData()
        {
            int idx = this.ActHdr.SelectedIndex;
            DataView dataViewHdrs = this.ActHdr.DataViewHdrs;

            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.Adjustment:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add(dataViewHdrs[idx]["ActivityAdjustmentType"].ToString());
                            this.ItemsList.Add("WHS:  " + dataViewHdrs[idx]["LocationMapWarehouseName"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            // TODO: Resource strings
                            this.ItemsList.Add(dataViewHdrs[idx]["ProductUnitBarCodeCode"].ToString().TrimEnd());
                            this.ItemsList.Add(dataViewHdrs[idx]["ProductUnitBarCodeModifier1"].ToString().TrimEnd());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ItemsList.Clear();

                        if (dataViewHdrs.Count > 0)
                        {
                            const int LineLength = 30;
                            List<string> lines = SysCommon.WordWrap2List(dataViewHdrs[idx]["ActivityCountDescription"].ToString().TrimEnd(), LineLength);

                            for (int i = 0; i < lines.Count; i++)
                            {
                                this.ItemsList.Add(lines[i]);
                            }
                        }
                    });

                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ItemsList.Clear();

                        if (dataViewHdrs.Count > 0)
                        {
                            const int LineLength = 30;
                            List<string> lines = SysCommon.WordWrap2List(dataViewHdrs[idx]["ActivityCountDescription"].ToString().TrimEnd(), LineLength);

                            for (int i = 0; i < lines.Count; i++)
                            {
                                this.ItemsList.Add(lines[i]);
                            }
                        }
                    });

                    break;

                case SysCommon.ActSubTypes.Picking:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add("CUSTOMER: " + dataViewHdrs[idx]["ActivityPickingCustomerContactName"].ToString());
                            this.ItemsList.Add("SHIPTO:   " + dataViewHdrs[idx]["ActivityPickingCustomerShipToInfo"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add("CUSTOMER: " + dataViewHdrs[idx]["ActivityPickingCustomerContactName"].ToString());
                            this.ItemsList.Add("SHIPTO:   " + dataViewHdrs[idx]["ActivityPickingCustomerShipToInfo"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add(dataViewHdrs[idx]["ActivityProductionDescription"].ToString());
                            this.ItemsList.Add("WHS:  " + dataViewHdrs[idx]["LocationMapWarehouseName"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add(dataViewHdrs[idx]["ActivityProductionDescription"].ToString());
                            this.ItemsList.Add("WHS:  " + dataViewHdrs[idx]["LocationMapWarehouseName"].ToString());

                        });
                    }

                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add(dataViewHdrs[idx]["ActivityProductionDescription"].ToString());
                            this.ItemsList.Add("WHS:  " + dataViewHdrs[idx]["LocationMapWarehouseName"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add("VENDOR:  " + dataViewHdrs[idx]["ActivityReceivingVendorContactName"].ToString());
                            this.ItemsList.Add("SUPPLIER:  " + dataViewHdrs[idx]["ActivityReceivingVendorSupplierContactName"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add("CUSTOMER: " + dataViewHdrs[idx]["ActivityPickingCustomerContactName"].ToString());
                            this.ItemsList.Add("SHIPTO:   " + dataViewHdrs[idx]["ActivityPickingCustomerShipToInfo"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.Returns:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add("CUSTOMER: " + dataViewHdrs[idx]["ActivityReturnsCustomerContactName"].ToString());
                            this.ItemsList.Add("SHIPTO:   " + dataViewHdrs[idx]["ActivityReturnsCustomerShipToInfo"].ToString());
                        });
                    }

                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    if (dataViewHdrs.Count > 0)
                    {
                        // TODO: Resource strings
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ItemsList.Add(dataViewHdrs[idx]["ActivityTransferDescription"].ToString());
                            this.ItemsList.Add("TO WHS:  " + dataViewHdrs[idx]["LocationMapWarehouseName"].ToString());
                        });
                    }

                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdFilter_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    FormDialogContainer.EDialogTypes dlgType = 0;

                    this.ScannerDisable();

                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.Picking:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrPickingFilter;
                            break;

                        case SysCommon.ActSubTypes.PickingSingle:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrPickingFilter;
                            break;

                        case SysCommon.ActSubTypes.Receiving:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrReceivingFilter;
                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrTransferFilter;
                            break;

                        case SysCommon.ActSubTypes.TransferTo:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrTransferFilter;
                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrPhysicalCountFilter;
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrCycleCountFilter;
                            break;

                        case SysCommon.ActSubTypes.Adjustment:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityAdjustmentFilter;
                            break;

                        case SysCommon.ActSubTypes.Returns:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityHdrReturnsFilter;
                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityProductionTicketFinishedFilter;
                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityProductionTicketMaterialFilter;
                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            dlgType = FormDialogContainer.EDialogTypes.ucActivityProductionTicketByProductFilter;
                            break;

                        default:
                            throw new NotImplementedException("eActivitySubType");
                    }

                    FormDialogContainer form = new FormDialogContainer(dlgType, this.ActSubType);
                    DialogResult dialogResult = await form.DisplayControl.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                    {
                        await form.SaveChanges(this, new EventArgs());
                        XDocument xmlDoc = form.DisplayControlData;
                        this.ActHdr.DataViewHdrs.RowFilter = this.ActHdr.GetFilter(xmlDoc);

                        if (!string.IsNullOrEmpty(xmlDoc.Root.Element("sortby").Value))
                        {
                            this.ActHdr.DataViewHdrs.Sort = xmlDoc.Root.Element("sortby").Value;
                        }

                        if (!string.IsNullOrEmpty(this.txtSearchHdr.Text))
                        {
                            this.DataGridMainActivityItemsList = this.ActHdr.Search(this.txtSearchHdr.Text);
                        }
                    }

                    this.ScannerEnable();
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
                    // Allow the validation to handle the setting for IsChangesMade.
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
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    // Needed to check if the enter key was pressed last, because it seems that 
                    // SelectNextControl in the keypress event causes a click event to fire
                    // if a command button is the next control rather than just giving the button focus.
                    XDocument xmlDoc = null;
                    this.ActHdr.Insert(xmlDoc);
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
                    // Allow the validation to handle the setting for IsChangesMade.
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
            }
        }

        /// <summary>
        /// Change the pick selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataGridMainActivity_KeyUpF5(object sender, KeyEventArgs e)
        {
            await this.KeyUpF5_ChangePickingCriteria(sender, e);
        }

        /// <summary>
        /// Release order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataGridMainActivity_KeyUpF4(object sender, KeyEventArgs e)
        {
            await this.KeyUpF4_ReleaseOrder(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task DataGridMainActivity_KeyUpEnter(object sender, KeyEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.ScannerDisable();
                    this.ActHdr.Refno = this.ActHdr.GetRefnoFromRowIdx(Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)));

                    if (this.SysEnv.SysSettings.IsPickSingleProd && this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        bool isAllowedToPick = true;

                        if (await ((PickingActivity)this.ActHdr).IsSalesOrderBeingPicked())
                        {
                            isAllowedToPick = false;

                            if (this.SysEnv.ContactLog.IsAllowedPickingModuleOverridePicker)
                            {
                                //if (MessageBox.Show("Do you wish to clear the current picker?", "Picker Override", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)

                                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Picker Override", "Do you wish to clear the current picker?", "Yes", "No")))
                                {
                                    if (await ((PickingActivity)this.ActHdr).ClearSalesOrderBeingPicked(false))
                                    {
                                        //MessageBox.Show("Picker cleared.", "Picker Override", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Picker Override", "Picker cleared.", "OK"));
                                    }
                                    else
                                    {
                                        //MessageBox.Show("Unable to clear the current picker.", "Picker Override", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Picker Override", "Unable to clear the current picker.", "OK"));
                                    }
                                }
                            }
                        }

                        if (isAllowedToPick)
                        {
                            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityItemSingle, this.ActHdr, this.ActSubType);
                            this.IsChangesMade = false;
                            await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                        }
                    }
                    else
                    {
                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityItem, this.ActHdr, this.ActSubType);
                        this.IsChangesMade = false;
                        await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
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
                finally
                {
                    this.ScannerEnable();

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    this.Loading = false;
                }
            });
        }

        /// <summary>
        /// Release order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF4_ReleaseOrder(object sender, KeyEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    if (Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) < 0)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Please select a record to release.");
                    }

                    // Important! Need to ask for label quantity before the change status AND set this.ActHdr.Refno
                    this.ActHdr.Refno = this.ActHdr.GetRefnoFromRowIdx(Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)));

                    SysCommon.SLabelQuantity labelQuantity = new SysCommon.SLabelQuantity();

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle && this.SysEnv.SysSettings.CanPrintShippingLabel && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        labelQuantity = this.ActHdr.GetLabelQuantity(this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity);
                    }

                    if (await Device.InvokeOnMainThreadAsync(async () => await this.ActHdr.ChangeStatus(Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)))))
                    {
                        // Print labels after release
                        this.ScannerDisable();

                        // Lekker Foods wants to be ask to print labels after the order has been released.
                        // There is no opportunity to reprint after the release.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            // Shipping labels
                            if (this.SysEnv.SysSettings.CanPrintShippingLabel && !this.SysEnv.SysSettings.IsDisconnectedMode)
                            {
                                labelQuantity = await PrintLabelsAfterRelease(labelQuantity);
                            }

                            if (this.SysEnv.SysSettings.IsPickRefreshOrdAfterRel)
                            {
                                await this.RefreshData();
                            }
                            else
                            {
                                this.ActHdr.DataViewHdrs.Delete(Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(dataGridMainActivity.SelectedItem)));
                            }
                        }
                        else if (this.ActSubType == SysCommon.ActSubTypes.Adjustment)
                        {
                            // <WO ID="14748" comment="Printing after deboxing" />
                            // If DEBOX, prompt for label
                            AdjActivity adjustment = ActHdr as AdjActivity;

                            if (adjustment != null)
                            {
                                if (adjustment.AdjSettings != null && adjustment.AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox)
                                {
                                    // Show pop up screen
                                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDeboxLabelCount, ActSubType);

                                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                                    {
                                        await form.SaveChanges(sender, e);
                                        XDocument doc = form.GetXML();
                                        actHdr.PrintLabel(doc);
                                    }

                                }
                            }
                        }

                        this.ScannerEnable();
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
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
                finally
                {
                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            });
        }

        private async Task<SysCommon.SLabelQuantity> PrintLabelsAfterRelease(SysCommon.SLabelQuantity labelQuantity)
        {
            if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle || this.ActSubType == SysCommon.ActSubTypes.Picking)
            {
                if (this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity)
                {
                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityShippingLabelEdit, this.ActSubType, labelQuantity, this.ActHdr, SysCommon.ShippingLabelQuantitySelectionMode.RepackOnly);

                    if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                    {
                        XDocument request = form.GetXML();

                        // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                        // If itm_refno is zero then no line is updated as printed.
                        request.Element("params").Element("whs_id").AddAfterSelf(new XElement("itm_refno", 0));
                        this.ActHdr.PrintLabel(request);
                    }
                }
                else
                {
                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityShippingLabelEdit, this.ActSubType, labelQuantity, this.ActHdr, SysCommon.ShippingLabelQuantitySelectionMode.BothFullRepack);

                    if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                    {
                        XDocument request = form.GetXML();

                        // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                        // If itm_refno is zero then no line is updated as printed.
                        request.Element("params").Element("whs_id").AddAfterSelf(new XElement("itm_refno", 0));
                        this.ActHdr.PrintLabel(request);
                    }
                }
            }

            return labelQuantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF5_ChangePickingCriteria(object sender, KeyEventArgs e)
        {
            await Task.Run(async () =>
            {
                this.Loading = true;

                try
                {
                    this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                    if ((this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle) && this.SysEnv.SysSettings.IsPrePickCrit)
                    {
                        // NOTE: oBarCodeParse is not initialized yet so there is not reason to preserve its values
                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityPickingSelectCriteria, this.ActSubType);

                        if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            LocationMap locMap = LocationMap.GetInstance();

                            XDocument result = form.GetXML();

                            SLocMap sLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(int.Parse(result.Root.Element("LocationMapRefno").Value));
                            locMap.CurrLocMap = sLocMap;

                            this.ActHdr.XMLDataCriteria = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + ("<whs_id>" + sLocMap.WhsFKRefno + "</whs_id>")
                                + ("<sect_refno>" + sLocMap.SectFKRefno + "</sect_refno>")
                                + "</params>");
                        }

                        await this.RefreshData();
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
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
                finally
                {
                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.Loading = false;
                }
            });
        }

        private void DataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            int index = this.DataGridMainActivityItemsList.IndexOf(e.SelectedItem);

            if (index != -1)
            {
                this.ActHdr.SelectHdr(Convert.ToInt32(index));
                this.DisplayData();
            }
        }

        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;
                    this.DataGridMainActivityItemsList = this.ActHdr.Search(this.txtSearchHdr.Text);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
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
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }
            
            // TODO: Resource strings
            string result = await DisplayActionSheet("Activity Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.FilterText && this.FilterIsEnabled)
            {
                this.cmdFilter_Click(sender, e);
            }
        }
    }
}
