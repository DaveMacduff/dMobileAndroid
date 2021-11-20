namespace dMobile.View.Activity
{
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomControls;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItem : DMobileModal, IAppArea, IDataLayerEventListener
    {
        private LocationMap locMap;
        private ActItem actItem;
        private SysCommon.ActSubTypes actSubTypes;
        private BarCodeScanner scanner = null;
        private bool isRefreshed = false;
        private int totalTicks = 0;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        private bool isShowDataValidMsg = true;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();
        private int selectedIndex;
        private List<string> optionsMenuItems = new List<string>();
        private bool firstLoad = true;
        private bool isAddingDetail = false;

        // For handling double click on listview item
        private int numTaps = 0;
        private object previousSender;

        // cmdLocation
        private string cmdLocationText;
        private bool cmdLocationIsEnabled;

        // cmdFilter
        private string cmdFilterText;
        private bool cmdFilterIsEnabled;

        // cmdSelectSkid
        private string cmdSelectSkidText;
        private bool cmdSelectSkidIsEnabled;

        // cmdAddProduct
        private string cmdAddProductText;
        private bool cmdAddProductIsEnabled;

        // cmdAddSkid
        private string cmdAddSkidText;
        private bool cmdAddSkidIsEnabled;

        public ucActivityItem()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="actSubTypes"></param>
        public ucActivityItem(Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Loading = false;

            // TODO: Resource strings
            this.CmdLocationText = "Set Location";
            this.CmdLocationIsEnabled = false;

            this.CmdFilterText = "Filter";
            this.CmdFilterIsEnabled = false;

            this.CmdSelectSkidText = "Select Skid";
            this.CmdSelectSkidIsEnabled = false;

            this.CmdAddProductText = "Add Prod";
            this.CmdAddProductIsEnabled = false;

            this.CmdAddSkidText = "Add Skid";
            this.CmdAddSkidIsEnabled = false;


            if (!DesignMode.IsDesignModeEnabled)
            {
                this.LocMap = LocationMap.GetInstance();
                this.SysEnv = SysEnvironment.GetInstance();

                this.ActSubType = actSubTypes;
                this.ActItem = ActItemFacade.GetActItem(activity, this.ActSubType);
                this.ActItem.ActHdr = activity;

                if (activity.SkidInfo == null)
                {
                    activity.SkidInfo = new SysSkidInfo();
                }

                this.Title = this.GetControlDescription();

                // Parse and seed the modifiers control
                this.updnBarCodeModifier1.Items.Clear();

                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.updnBarCodeModifier1.Items.Add(string.Empty);
                    string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                    foreach (string item in modifiers)
                    {
                        this.updnBarCodeModifier1.Items.Add(item);
                    }
                }
                else
                {
                    this.updnBarCodeModifier1.IsVisible = false;
                    this.updnBarCodeModifier1.IsEnabled = false;
                }

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            }
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName == nameof(CmdLocationText) ||
                propertyName == nameof(CmdLocationIsEnabled) ||
                propertyName == nameof(CmdFilterText) ||
                propertyName == nameof(CmdFilterIsEnabled) ||
                propertyName == nameof(CmdSelectSkidText) ||
                propertyName == nameof(CmdSelectSkidIsEnabled) ||
                propertyName == nameof(CmdAddProductText) ||
                propertyName == nameof(CmdAddProductIsEnabled) ||
                propertyName == nameof(CmdAddSkidText) ||
                propertyName == nameof(CmdAddSkidIsEnabled)
               )
            {
                this.PopulateOptionsMenu();
            }


            if (propertyName == nameof(this.CmdSelectSkidText))
            {
                if (this.CmdSelectSkidText != "Select Skid")
                {
                    this.lblSkidNumber.Text = this.CmdSelectSkidText;
                    this.lblSkidNumber.IsVisible = true;
                }
                else
                {
                    this.lblSkidNumber.Text = string.Empty;
                    this.lblSkidNumber.IsVisible = false;
                }
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
                            await this.DataGridMainActivity_KeyUpEnter();
                        }
                    }
                    else if (keycode == Keycode.Clear)
                    {
                        if (await this.CancelChanges())
                        {
                            this.DialogResult = DialogResult.None;
                            
                            // TODO: DEV-5619
                            // Android dmobile F1 & F3 Button Not Working Same As Windows dmobile   
                            await this.Close();
                        }
                    }
                    else if (keycode == Keycode.Del)
                    {
                        await this.DataGridMainActivity_KeyUpDelete();
                    }
                    else if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;

                            // TODO: DEV-5619
                            // Android dmobile F1 & F3 Button Not Working Same As Windows dmobile   
                            await this.Close();
                        }
                    }
                    else if (keycode == Keycode.F2)
                    {
                        await this.DataGridMainActivity_KeyUpF2(this, new EventArgs());
                    }
                    else if (keycode == Keycode.F3)
                    {
                        await this.DataGridMainActivity_KeyUpF3(this, new EventArgs());
                    }
                    else if (keycode == Keycode.F4)
                    {
                        await this.DataGridMainActivity_KeyUpF4(this, new EventArgs());
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
                await this.ucActivityItem_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityItem_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ScannerDisable();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.dataGridMainActivity.ItemTapped -= this.DataGridMainActivity_ItemTapped;

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
                else if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
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
                        await this.DataGridMainActivity_KeyUpEnter();
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

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();

            if (this.CmdLocationIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdLocationText);
            }

            if (this.CmdFilterIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdFilterText);
            }

            if (this.CmdSelectSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdSelectSkidText);
            }

            if (this.CmdAddProductIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdAddProductText);
            }

            if (this.CmdAddSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.CmdAddSkidText);
            }
        }

        private bool IsShowDataValidMsg
        {
            get
            {
                return this.isShowDataValidMsg;
            }

            set
            {
                this.isShowDataValidMsg = value;
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

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private ActItem ActItem
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

        private bool IsRefreshed
        {
            get
            {
                return this.isRefreshed;
            }

            set
            {
                this.isRefreshed = value;
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

        private BarCodeScanner Scanner
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }
        public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }
        public List<string> OptionsMenuItems { get => optionsMenuItems; set => optionsMenuItems = value; }
        public string CmdLocationText { get => cmdLocationText; set { cmdLocationText = value; this.OnPropertyChanged(); } }
        public bool CmdLocationIsEnabled { get => cmdLocationIsEnabled; set { cmdLocationIsEnabled = value; this.OnPropertyChanged(); } }
        public string CmdFilterText { get => cmdFilterText; set { cmdFilterText = value; this.OnPropertyChanged(); } }
        public bool CmdFilterIsEnabled { get => cmdFilterIsEnabled; set { cmdFilterIsEnabled = value; this.OnPropertyChanged(); } }
        public string CmdSelectSkidText { get => cmdSelectSkidText; set { cmdSelectSkidText = value; this.OnPropertyChanged(); } }
        public bool CmdSelectSkidIsEnabled { get => cmdSelectSkidIsEnabled; set { cmdSelectSkidIsEnabled = value; this.OnPropertyChanged(); } }
        public string CmdAddProductText { get => cmdAddProductText; set { cmdAddProductText = value; this.OnPropertyChanged(); } }
        public bool CmdAddProductIsEnabled { get => cmdAddProductIsEnabled; set { cmdAddProductIsEnabled = value; this.OnPropertyChanged(); } }
        public string CmdAddSkidText { get => cmdAddSkidText; set { cmdAddSkidText = value; this.OnPropertyChanged(); } }
        public bool CmdAddSkidIsEnabled { get => cmdAddSkidIsEnabled; set { cmdAddSkidIsEnabled = value; this.OnPropertyChanged(); } }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= this.BarCodeHandler;
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
                this.Scanner.BarCodeScan += this.BarCodeHandler;
                await this.Scanner.Start();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            if (isReadyToSave)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    try
                    {
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (Exceptions.MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                // TODO: Resource strings
                                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Quantity Validation", "Continue?", "Yes", "No") == false))
                                {
                                    isReadyToSave = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                // TODO: Resource strings
                                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Quantity Validation", ex.Message, "OK"));
                                isReadyToSave = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:
                                // TODO: Resource strings
                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
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
                }
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

            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            try
            {
                await this.ScannerDisable();
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
            finally
            {
                await this.ScannerEnable();
            }

            // Prairie meats like Lekker requirement to check quantity under even though we are cancelling / closing.
            // See WO# 12080
            if (isReadyToClose)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    try
                    {
                        this.IsShowDataValidMsg = true;
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (Exceptions.MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                //if (MessageBox.Show(ex.Message + Environment.NewLine + "Continue?", "Quantity Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                // TODO: Resource strings
                                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No") == false))
                                {
                                    isReadyToClose = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                //MessageBox.Show(ex.Message, "Quantity Validation", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                // TODO: Resource strings
                                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Quantity Validation", ex.Message, "OK"));
                                isReadyToClose = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:
                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
                    }
                    catch (Exception ex)
                    {
                        isReadyToClose = true;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                    }
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
            bool isSaveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    ////if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    ////{
                    ////  if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    ////  {
                    ////    // update the header record as reviewed if all items were reviewed
                    ////    bool isReviewed = true;
                    ////    for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                    ////    {
                    ////      isReviewed = isReviewed && (bool)this.ActItem.DataViewItems[i]["ActivityItemPickingIsReviewed"];
                    ////    }

                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].BeginEdit();
                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityPickingIsReviewed"] = isReviewed;
                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].EndEdit();
                    ////  }

                    ////  if (!((PickingActivity)ActHdr).ClearSalesOrderBeingPicked(true))
                    ////  {
                    ////    MessageBox.Show("Unable to clear the current picker.", "Clear Picker",
                    ////                  MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    ////  }
                    ////}

                    ////ClearContents();

                    // Print all receiving skid labels.
                    await PrintSkidLabels();

                    this.IsChangesMade = false;
                    isSaveComplete = true;
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all 
                        // in the event that IsReadyToSave just returns false.

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isSaveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = false;

            try
            {
                if (await this.IsReadyToClose())
                {
                    ////if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    ////{
                    ////  // Lekker requirement, cancel will update the reviewed header status.
                    ////  if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    ////  {
                    ////    // update the header record as reviewed if all items were reviewed
                    ////    bool isReviewed = true;
                    ////    for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                    ////    {
                    ////      isReviewed = isReviewed && (bool)this.ActItem.DataViewItems[i]["ActivityItemPickingIsReviewed"];
                    ////    }

                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].BeginEdit();
                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityPickingIsReviewed"] = isReviewed;
                    ////    this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].EndEdit();
                    ////  }
                    ////}

                    // Clear any properties.
                    cancelChanges = true;

                    ////ClearContents();

                    ////if (!((PickingActivity)ActHdr).ClearSalesOrderBeingPicked(true))
                    ////{
                    ////  MessageBox.Show("Unable to clear the current picker.", "Clear Picker",
                    ////                MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    ////}

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
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
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
            string desc = string.Empty;

            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.PhysicalCounts:
                    // TODO: Resource strings
                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    // TODO: Resource strings
                    desc = string.Format("Receive # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Picking:
                    // TODO: Resource strings
                    desc = string.Format("Picking Order # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    // TODO: Resource strings
                    desc = string.Format("Transfer From # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    // TODO: Resource strings
                    desc = string.Format("Transfer To # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Returns:
                    // TODO: Resource strings
                    desc = string.Format("Return # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    // TODO: Resource strings
                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    // TODO: Resource strings
                    desc = string.Format("Adjustment # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Finished # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Material # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Add-Product # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    // TODO: Resource strings
                    desc = string.Format("Picking Order # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                default:
                    // TODO: Resource strings
                    desc = "Activity Items";
                    break;
            }

            return desc;
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
                + "<params>" + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            ////lstDisplay.Items.Clear();

            ////const int lineLength = 30;
            ////List<string> lines = SysCommon.WordWrap2List(dataLayerEventArgs.StatusOutMsg, lineLength);
            ////for (int i = 0; i < lines.Count; i++)
            ////{
            ////    lstDisplay.Items.Add(lines[i]);
            ////}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            if (dataLayerEventArgs.SetMinMaxValue)
            {
                //this.pbarStatus.Maximum = dataLayerEventArgs.MaxValue;
                //this.pbarStatus.Minimum = dataLayerEventArgs.MinValue;
            }

            //this.pbarStatus.Value = dataLayerEventArgs.CurrentValue;
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task PrintSkidLabels()
        {
            // Print all receiving skid labels.
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving && !this.SysEnv.SysSettings.IsDisconnectedMode && this.SysEnv.SysSettings.IsSkidTrackingModule && this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
            {
                if (await this.CheckIfSkids2Print())
                {
                    //if (MessageBox.Show("Do you wish to print all skid label(s)?", "Skid Labels", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    if (await DisplayAlert("Skid Labels", "Do you wish to print all skid label(s)?", "Yes", "No"))
                    {
                        StringBuilder xmlString = new StringBuilder();
                        XmlWriter xml = XmlWriter.Create(xmlString);
                        XDocument request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + await SysCommon.GetRequestDefaults(false)
                            + "<command>"
                            + "<action>SUBMIT PRINT ALL SKID LABELS</action>"
                            + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                            + ("<fk_refno>" + this.ActItem.ActHdr.FKRefno + "</fk_refno>")
                            + ("<fk_source>PO</fk_source>")
                            + "</command>"
                            + "</imclient>");

                        request.Save(xml);
                        xml.Flush();
                        xml.Close();

                        string reqData = SysCommon.SubmitComm(xmlString.ToString());

                        if (reqData.Length > 0)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(reqData);

                            if (reqData.Contains("<message>ERROR</message>"))
                            {
                                string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                                if (errorDetail.Contains("MSGBOX-"))
                                {
                                    // TODO: Resource strings
                                    throw new Exceptions.MessageOnlyException("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task<bool> CheckIfSkids2Print()
        {
            bool isExist2Print = false;

            // Print all receiving skid labels.
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving && !this.SysEnv.SysSettings.IsDisconnectedMode && this.SysEnv.SysSettings.IsSkidTrackingModule && this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
            {
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                XDocument request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<command>"
                    + "<action>REQUEST CHECK IF SKIDS 2 PRINT</action>"
                    + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<fk_refno>" + this.ActItem.ActHdr.FKRefno + "</fk_refno>")
                    + ("<fk_source>PO</fk_source>")
                    + "</command>"
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                string reqData = SysCommon.SubmitComm(xmlString.ToString());

                if (reqData.Length > 0)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reqData);

                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Check skids to print error." + Environment.NewLine + errorDetail);
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("Check skids to print error." + Environment.NewLine + errorDetail);
                        }
                    }
                    else if (reqData.Contains("<detail>True</detail>"))
                    {
                        isExist2Print = true;
                    }
                }
            }

            return isExist2Print;
        }

        /// <summary>
        /// 
        /// </summary>
        private void IsItemReviewed()
        {
            bool isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                if (this.ActItem.SelectedActivityItem != null)
                {
                    // TODO: test
                    isReviewed = Convert.ToBoolean((this.ActItem.SelectedActivityItem as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void IsAllItemsReviewed()
        {
            bool isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                for (int idx = 0; idx <= this.ActItem.ListItems.Count - 1; idx++)
                {
                    // TODO: test
                    isReviewed = isReviewed && Convert.ToBoolean((this.ActItem.ListItems[idx] as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for ALL line items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private void AllItemsQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                decimal available = 0;
                decimal shipped = 0;
                decimal ordered = 0;

                for (int currIdx = 0; currIdx <= this.ActItem.ListItems.Count - 1; currIdx++)
                {
                    // TODO: Test
                    dynamic selectedItem = this.ActItem.ListItems[currIdx];

                    available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                    shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                    ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);

                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (0 < available && shipped < ordered)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for one line item.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Line items must exist, one needs to be selected.</remarks>
        private void ItemQuantityUnderCheck()
        {
            if (this.ActItem.ListItems != null)
            {
                if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
                {
                    if (this.ActItem.SelectedActivityItem != null)
                    {
                        dynamic selectedItem = this.ActItem.SelectedActivityItem;
                        decimal available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                        decimal shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                        decimal ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);

                        if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                        {
                            // Do not warn if product is out of stock WO# 12054 was only applied to ucActivityItemSingle
                            // if (0 < available && shipped < ordered)
                            if (shipped < ordered)
                            {
                                string msg = string.Empty;

                                if (this.SysEnv.SysSettings.IsPickQtyUnderShowBaseQtyAvail)
                                {
                                    // TODO: Resource strings
                                    msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable), Convert.ToString(selectedItem.BaseUnitCode));
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityAvailable), Convert.ToString(selectedItem.UnitCode));
                                }

                                throw new Exceptions.MessageOnlyException(msg);
                            }
                        }
                        else
                        {
                            if (shipped < ordered)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("The quantity shipped is less than the quantity ordered.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityItem_GotFocus(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;
                    await this.ScannerDisable();

                    while (this.isAddingDetail)
                    {
                        // Delay on appearing until detail has been added
                    }

                    if (this.firstLoad)
                    {
                        this.firstLoad = false;
                        await this.ActItem.Refresh(this.ActItem.ActHdr.XMLDataCriteria);

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                        {
                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        }
                    }

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
                        this.CmdLocationIsEnabled = true;
                        this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                        this.CmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;
                    });

                    this.ActItem.RegLayerListenerProgOut(this);
                    await this.RefreshData();
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
                    if (this.ActItem != null)
                    {
                        this.ActItem.UnregLayerListenerProgOut(this);
                    }

                    await this.ScannerEnable();
                    this.IsControlLoaded = true;
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
        private async Task ucActivityItem_LostFocus(object sender, EventArgs e)
        {
            await this.ScannerDisable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void updnBarCodeModifier1_GotFocus(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdLocation_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();

                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                        {
                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        }
                    }
                    else
                    {
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                        {
                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                        }
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
                    await DisplayData();
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdSelectSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl in the keypress event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSkidInfo, this.ActItem, this.ActSubType);
                await form.DisplayControl.ShowDialog();

                XDocument xmlDoc = await form.GetXML();
                this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                {
                    // Needed to refresh the selected item location, called SelectItem.
                    await this.ActItem.SelectActivityItem(this.ActItem.SelectedActivityItem);
                }
                else
                {
                    int actItemFoundRefno = await this.ActItem.GetRefnoFromProdUnitRefno(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno);

                    // TODO: Sorting example
                    this.ActItem.ListItems = this.ActItem.ListItems.OrderBy(item => item.GetType().GetProperty("ActivityItemRefno")?.GetValue(item)).ToList();

                    if (actItemFoundRefno > 0)
                    {
                        //int foundRowIdx = this.ActItem.ListItems.Find(actItemFoundRefno);
                        object selectedItem = this.ActItem.ListItems.Where(act => Convert.ToString(act.GetType().GetProperty("ActivityItemRefno")?.GetValue(act)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();

                        if (selectedItem != null)
                        {
                            await this.ActItem.SelectActivityItem(selectedItem);
                        }
                    }
                }

                await this.DisplayData();
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdAddProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl in the keypress event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                int actItemFoundRefno = 0;
                int prodUnitRefno = 0;
                int foundRowIdx = 0;
                XDocument xmlDoc = null;

                try
                {
                    await this.ScannerDisable();

                    // Not allowed to count individual boxes on a skid. Skids must be
                    // scanned as a whole unit or built in skid maintenance.

                    // DEV-827 This change to allow counting partial skids is a bad idea.                        
                    ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                    ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                    ////{
                    ////    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    ////    {
                    ////        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                    ////        this.cmdSelectSkid.Text = string.Empty;
                    ////    }
                    ////}

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        // activity item object needs to check if a new location and product can be added, if not add a detail
                        if (!await LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Location must be set first.");
                        }

                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            xmlDoc = await form.GetXML();
                            prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                        }
                    }
                    else
                    {
                        prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        this.LocMap.CurrLocMap = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap;

                        ProductInfo prodInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo;
                        string unitSizeDesc = SysCommon.AddXMLReservedChars(prodInfo.UnitSizeDesc);
                        xmlDoc = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + ("<ProductUnitRefno>" + prodInfo.UnitRefno + "</ProductUnitRefno>")
                            + ("<ProductRefno>" + prodInfo.Refno + "</ProductRefno>")
                            + ("<ProductCode>" + prodInfo.Code + "</ProductCode>")
                            + ("<ProductDescription>" + SysCommon.AddXMLReservedChars(prodInfo.Desc) + "</ProductDescription>")
                            + ("<UnitCode>" + prodInfo.UnitCode + "</UnitCode>")
                            + ("<BaseUnitCode>" + prodInfo.BaseUnitCode + "</BaseUnitCode>")
                            + ("<ProductUnitBC>" + prodInfo.UnitBC + "</ProductUnitBC>")
                            + ("<ProductUnitSizeDescription>" + unitSizeDesc + "</ProductUnitSizeDescription>")
                            + "</params>");
                    }

                    if (prodUnitRefno != 0)
                    {
                        // Check if the selected product to add already exists on the document.
                        actItemFoundRefno = await this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);

                        if (actItemFoundRefno > 0)
                        {
                            // TODO: Fix sorting
                            //string oldSort = this.ActItem.DataViewItems.Sort;
                            //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            object selectedItem = this.ActItem.ListItems.Where(act => Convert.ToString(act.GetType().GetProperty("ActivityItemRefno")?.GetValue(act)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();

                            this.ActItem.SelectedActivityItem = selectedItem;

                            // TODO: Fix sorting
                            //this.ActItem.DataViewItems.Sort = oldSort;
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(xmlDoc, false);
                            }
                            else
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Product rule says that it must exist on this document already.");
                            }
                        }

                        // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation && (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.TransferFrom))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = await this.ActItem.GetDefaultLocMapRefno(Convert.ToInt32((this.ActItem.ListItems[foundRowIdx] as dynamic).LocationMapRefno), Convert.ToInt32((this.ActItem.ListItems[foundRowIdx] as dynamic).ProductRefno));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("The current location does not match the product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////{
                        ////    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////    {
                        ////        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////    }
                        ////}

                        ////BarCodeParse parse = BarCodeParseFacade.ParseData(string.Empty);
                        ////bool isInsertReady = false;
                        ////ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

                        ////// Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        ////// and the fact is isn't human readable friendly.
                        ////SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                        ////SProductSchemeData schemeData = actDetail.SchemeData;

                        ////if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////{
                        ////  schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        ////  schemeData.LogisticUnitBatchLotNumber 
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                        ////  schemeData.LogisticUnitBestBeforeSellByDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                        ////  schemeData.LogisticUnitProductionDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                        ////  schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                        ////  schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                        ////  schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        ////}

                        ////tempItemProdSchemeData.ProdSchemeData = schemeData;
                        ////actDetail.SchemeData = schemeData;

                        ////SysShared.GatherProductData(
                        ////    parse,
                        ////    this.ActSubType,
                        ////    this.ActItem,
                        ////    tempItemProdSchemeData,
                        ////    this.CallingControl,
                        ////    ref isInsertReady,
                        ////    actDetail,
                        ////    true);

                        ////this.InsertActivityDetail(ref foundRowIdx, actDetail, parse, isInsertReady);
                    }
                }
                catch (GatherProductDataException ex)
                {
                    if (!ex.SuppressError)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                    }

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            await this.RefreshData();
                            break;
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

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            await this.RefreshData();
                            break;
                    }
                }
                finally
                {
                    if (this.ActItem.ListItems.Count > 0)
                    {
                        this.ActItem.SelectedActivityItem = this.ActItem.ListItems.LastOrDefault();
                    }

                    await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList?.LastOrDefault());
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayData());
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdAddSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl in the keypress event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                int actItemFoundRefno = 0;
                int prodUnitRefno = 0;
                object selectedItem = null;

                try
                {
                    await this.ScannerDisable();

                    if (await DisplayAlert("Add Skid", "Add the ENTIRE skid to this activity?", "Yes", "No"))
                    {
                        prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        if (actItemFoundRefno > 0)
                        {
                            //foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            selectedItem = this.ActItem.ListItems.Where(actItem => Convert.ToString(actItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(actItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx will be the last one
                                //foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                                selectedItem = this.ActItem.ListItems.LastOrDefault();
                                actItemFoundRefno = this.ActItem.Refno;
                            }
                            else
                            {
                                throw new Exceptions.MessageOnlyException(this.ActItem.GetItemNotFoundMessage(this.LocMap, this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }
                        }

                        this.ActItem.SelectedActivityItem = selectedItem;

                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        // and the fact is isn't human readable friendly.
                        SProductSchemeData schemeData = actDetail.SchemeData;
                        schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        actDetail.SchemeData = schemeData;
                        actDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);
                        actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        XDocument xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                            + ("<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                            + ("<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                            + ("<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                            + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        this.ActItem.UpdateListItemQuantity(this.ActItem.SelectedActivityItem, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
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
                    await DisplayData();
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataGridMainActivity_KeyUpEnter()
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;
                    this.isAddingDetail = true;
                    await this.ScannerDisable();

                    if (0 < this.ActItem.ListItems.Count)
                    {
                        await this.ActItem.SelectActivityItem(this.dataGridMainActivity.SelectedItem);

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            SSkidInfo currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                            if (currSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Selected line item product must match the selected skid product.");
                            }
                        }

                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetail, this.ActItem, this.ActSubType);

                        if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            XDocument xmlDoc = await form.GetXML();
                            this.ActItem.RefreshQuantity(
                                this.dataGridMainActivity.SelectedItem,
                                Math.Round(decimal.Parse(xmlDoc.Root.Element("qty").Value), 3),
                                Math.Round(decimal.Parse(xmlDoc.Root.Element("base_qty").Value), 3),
                                Math.Round(decimal.Parse(xmlDoc.Root.Element("wt_qty").Value), 3));
                            await this.ActItem.UpdateQuantityFromActivityItemList(this.dataGridMainActivity.SelectedItem, this.ActItem.Refno);
                        }
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
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await this.DisplayData();
                    });

                    await this.ScannerEnable();
                    this.isAddingDetail = false;
                    // Do not need to set this.Loading = false here, onAppearing takes care of it
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DataGridMainActivity_KeyUpDelete()
        {
            try
            {
                this.dataGridMainActivity.IsEnabled = false;
                await this.ScannerDisable();

                if (this.ActItem.ActHdr.SkidInfo != null)
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        //DialogResult result = MessageBox.Show(string.Format("Do you wish to delete this skid \'{0}\'?", this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode), "Delete Skid", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                        //if (result == DialogResult.Yes)

                        // TODO: Resource strings
                        if (await DisplayAlert("Delete Skid", string.Format("Do you wish to delete this skid \'{0}\'?", this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode), "Yes", "No"))
                        {
                            await this.ActItem.Delete();
                            await this.RefreshData();
                        }
                    }
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
                this.dataGridMainActivity.IsEnabled = true;
                await this.ScannerEnable();
                this.dataGridMainActivity.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task DataGridMainActivity_KeyUpF2(object sender, EventArgs e)
        {
            await this.KeyUpF2_ShowLineItemComments(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task DataGridMainActivity_KeyUpF3(object sender, EventArgs e)
        {
            await this.KeyUpF3_ShowTotalQuantity(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task DataGridMainActivity_KeyUpF4(object sender, EventArgs e)
        {
            await this.KeyUpF4_LaunchInvInquiry(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF4_LaunchInvInquiry(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                this.dataGridMainActivity.IsEnabled = false;

                await ShowInventoryInquirySummary();
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
                this.dataGridMainActivity.IsEnabled = true;
                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        private async Task ShowInventoryInquirySummary()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                // TODO: Resource strings
                throw new Exceptions.MessageOnlyException("Inventory Inquiry Summary is unavailable in disconnected mode.");
            }

            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityInventoryInquirySummary, this.ActItem, this.ActSubType);
            await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

            await this.ScannerEnable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF2_ShowLineItemComments(object sender, EventArgs e)
        {
            try
            {
                this.dataGridMainActivity.IsEnabled = false;

                await this.ScannerDisable();

                //if (0 > this.dataGridMainActivity.CurrentRowIndex)
                if (this.dataGridMainActivity.SelectedItem != null && 0 > this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Please select a line item to show the comments for.");
                }

                string comment = this.ActItem.GetComment((this.dataGridMainActivity.SelectedItem as dynamic).ActivityItemRefno);

                // TODO: test
                if (!string.IsNullOrEmpty(comment))
                {
                    // TODO: Resource strings
                    await DisplayAlert("Item Comment", comment, "OK");
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
                this.dataGridMainActivity.IsEnabled = true;
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF3_ShowTotalQuantity(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                string msg = await this.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), msg, "OK"));

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
                await this.ScannerEnable();
            }
        }

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

            this.Loading = true;
            await this.ScannerDisable();

            int prodUnitRefno = 0;
            object selectedItem = null;
            int actItemFoundRefno = 0;
            SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
            BarCodeParse parse = null;

            try
            {
                parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText = "Set Location");
                    }
                }
                else
                {
                    if (!await LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno)
                            && (!this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc
                            && !(this.ActSubType == SysCommon.ActSubTypes.Picking
                            || this.ActSubType == SysCommon.ActSubTypes.Receiving
                            || this.ActSubType == SysCommon.ActSubTypes.Receiving)))
                    {

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Location must be set first!");
                    }

                    // PRODUCT IDENTIFIER
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////                            || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, prodUnitRefno);
                        ProductInfo tempProdInfo = new ProductInfo();
                        tempProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        actItemFoundRefno = await this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        if (actItemFoundRefno > 0)
                        {
                            selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                bool isAddNewItem = false;

                                if (this.ActItem.IsAskCreateNewItem)
                                {
                                    await Device.InvokeOnMainThreadAsync(async () =>
                                    {
                                        if (await DisplayAlert(this.GetControlDescription(),
                                                    string.Format("A line item does not exist for this warehouse {0} and product \'{1}.\'" + Environment.NewLine + "Do you wish to create one?",
                                                    this.LocMap.CurrLocMap.WhsCode,
                                                    ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo)),
                                                    "Yes", "No"))
                                        {
                                            isAddNewItem = true;
                                        }
                                    });
                                }
                                else
                                {
                                    isAddNewItem = true;
                                }

                                if (isAddNewItem)
                                {
                                    await this.ActItem.Insert(ProductInfoTasks.GetXML(tempProdInfo), false);

                                    // This is a little dangerous we assume that the new row idx will be the last one
                                    selectedItem = this.ActItem.ListItems.LastOrDefault();
                                    actItemFoundRefno = this.ActItem.Refno;
                                }
                                else
                                {
                                    throw new GatherProductDataException(this.ActItem.GetItemNotFoundMessage(this.LocMap, tempProdInfo), true);
                                }
                            }
                            else
                            {
                                throw new Exception(this.ActItem.GetItemNotFoundMessage(this.LocMap, tempProdInfo));
                            }
                        }
                    }

                    // ACTIVITY ITEM FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        //this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();

                        if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty(this.ActItem.GetDataViewSortFKRefnoIdx())?.GetValue(activityItem)) == Convert.ToString(scanArgs.ParseInfo.ActItemFKRefno)).FirstOrDefault();

                            if (selectedItem != null)
                            {
                                await this.ActItem.SelectActivityItem(selectedItem);
                                actItemFoundRefno = this.ActItem.Refno;
                                //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                //foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                                selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                            }
                            else
                            {
                                throw new Exception(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a line item bar code is not supported here!");
                        }
                    }

                    // ACTIVITY DETAIL FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                        {
                            //this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();
                            tempItemProdSchemeData = await this.ActItem.GetProdSchemeDataFromDetailFKRefno(int.Parse(parse.ActDetailFKRefno));
                            //foundRowIdx = this.ActItem.DataViewItems.Find(tempItemProdSchemeData.ActItemFKRefno.ToString());

                            selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty(this.ActItem.GetDataViewSortFKRefnoIdx())?.GetValue(activityItem)) == Convert.ToString(tempItemProdSchemeData.ActItemFKRefno)).FirstOrDefault();

                            if (selectedItem != null)
                            {
                                await this.ActItem.SelectActivityItem(selectedItem);
                                actItemFoundRefno = this.ActItem.Refno;
                                //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                //foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);

                                selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                            }
                            else
                            {
                                throw new Exception(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                        }
                    }

                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno) && this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here! In Disconnected Mode");
                    }

                    // SHIPPING CONAINTER SCANNED, SKID
                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.SSCC18);

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Skid does not exist.");
                        }

                        prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        //this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        if (actItemFoundRefno > 0)
                        {
                            //foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            selectedItem = this.ActItem.ListItems.OfType<object>().Where(activityItem => Convert.ToString(activityItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(activityItem)) == Convert.ToString(actItemFoundRefno)).FirstOrDefault();
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx will be the last one
                                selectedItem = this.ActItem.ListItems.LastOrDefault();
                                actItemFoundRefno = this.ActItem.Refno;
                            }
                            else
                            {
                                throw new Exceptions.MessageOnlyException(this.ActItem.GetItemNotFoundMessage(this.LocMap, this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }
                        }
                    }

                    // IF AN ACTIVITY LINE ITEM WAS FOUND
                    if (actItemFoundRefno == 0 || selectedItem == null)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Line item not found.");
                    }
                    else
                    {
                        await this.ActItem.SelectActivityItem(selectedItem);
                        await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = selectedItem);
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayData());
                    }

                    if (!await LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Location must be set first.");
                    }

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ActItem.ProdInfo.UnitRefno)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Identified line item product does not match the selected skid product.");
                        }
                    }

                    // If NOT a container, evaluate
                    // WO 14171 if Skid is catch ask zero, then prompt for quantity.
                    if (string.IsNullOrEmpty(parse.SSCC18) || ActItem.IsCatchAskZero())
                    {
                        // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation && (this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.TransferFrom))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = await this.ActItem.GetDefaultLocMapRefno(Convert.ToInt32(selectedItem.GetType().GetProperty("LocationMapRefno")?.GetValue(selectedItem)), Convert.ToInt32(selectedItem.GetType().GetProperty("ProductRefno")?.GetValue(selectedItem)));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("The current location does not match the product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////                            || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        bool isInsertReady = false;
                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        // and the fact is isn't human readable friendly.
                        SProductSchemeData schemeData = actDetail.SchemeData;

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            schemeData.LogisticUnitBatchLotNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                            schemeData.LogisticUnitBestBeforeSellByDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                            schemeData.LogisticUnitProductionDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                            schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                            schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                            schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        }

                        tempItemProdSchemeData.ProdSchemeData = schemeData;
                        actDetail.SchemeData = schemeData;

                        isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActItem, tempItemProdSchemeData, isInsertReady, actDetail, false);
                        await this.InsertActivityDetail(selectedItem, actDetail, parse, isInsertReady);
                    }
                    else
                    {
                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);
                        actDetail.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(this.ActItem.ProdInfo.Code, this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        XDocument xmlParam = XDocument.Parse(
                                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                    + "<imclient>"
                                    + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                                    + ("<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                                    + ("<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                                    + ("<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                                    + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);

                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        this.ActItem.UpdateListItemQuantity(selectedItem, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
                        await this.ActItem.UpdateQuantityFromActivityItemList(selectedItem, this.ActItem.Refno);
                    }

                    // foundRowIdx = this.dataGridMainActivity.CurrentRowIndex
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        });
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.CmdSelectSkidText = "Select Skid");
                    }
                }

                await this.RefreshData();
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being cancelled from IdentifyProductScanned when user declines to assign barcode to product
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = false;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Select Skid";
                        });
                }

                // Do not refresh product list for counts. See WO# 12222.
                // Reloading count items into the thousands is too slow.
                // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                // Refreshing will have to be accomplished by exiting the items activity.
                switch (this.ActSubType)
                {
                    case SysCommon.ActSubTypes.PhysicalCounts:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        break;

                    default:
                        // Performing a refresh data will remove and partially added new items or quantity.
                        await this.RefreshData();
                        break;
                }
            }
            catch (GatherProductDataException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = false;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Select Skid";
                        });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                }

                // Do not refresh product list for counts. See WO# 12222.
                // Reloading count items into the thousands is too slow.
                // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                // Refreshing will have to be accomplished by exiting the items activity.
                switch (this.ActSubType)
                {
                    case SysCommon.ActSubTypes.PhysicalCounts:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        break;

                    default:
                        // Performing a refresh data will remove and partially added new items or quantity.
                        await this.RefreshData();
                        break;
                }
            }
            catch (IsReadyToApplyException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = false;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Select Skid";
                        });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                            await errRpt.ShowDialog();
                        });
                }

                // Do not refresh product list for counts. See WO# 12222.
                // Reloading count items into the thousands is too slow.
                // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                // Refreshing will have to be accomplished by exiting the items activity.
                switch (this.ActSubType)
                {
                    case SysCommon.ActSubTypes.PhysicalCounts:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        break;

                    default:
                        // Performing a refresh data will remove and partially added new items or quantity.
                        await this.RefreshData();
                        break;
                }
            }
            catch (Exception ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.CmdAddSkidIsEnabled = false;

                            // TODO: Resource strings
                            this.CmdSelectSkidText = "Select Skid";
                        });
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });

                // Do not refresh product list for counts. See WO# 12222.
                // Reloading count items into the thousands is too slow.
                // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                // Refreshing will have to be accomplished by exiting the items activity.
                switch (this.ActSubType)
                {
                    case SysCommon.ActSubTypes.PhysicalCounts:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        break;

                    default:
                        // Performing a refresh data will remove and partially added new items or quantity.
                        await this.RefreshData();
                        break;
                }
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
        /// <param name="currRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="parse"></param>
        /// <param name="isInsertDetailReady"></param>
        private async Task<object> InsertActivityDetail(object selectedActivityItem, ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                XDocument xmlDoc = actDetail.GetXML(actDetail);
                await actDetail.Insert(xmlDoc);

                // TODO check if actually new, if not delete and change counter
                // Does the row exist? If not, then it has been removed due to not needing it
                // Skip update qty.

                if (selectedActivityItem != null)
                {
                    // Upon a save of completed data update the data view to to show new item and quantity change
                    this.ActItem.UpdateListItemQuantity(selectedActivityItem, actDetail.Quantity, actDetail.BaseQuantity, actDetail.WeightQuantity);
                    await this.ActItem.UpdateQuantityFromActivityItemList(selectedActivityItem, this.ActItem.Refno);
                }
            }

            return selectedActivityItem;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            try
            {
                this.ActItem.RegLayerListenerProgOut(this);

                this.ActItem.ListItems = null;
                this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.DataGridMainActivityItemsList = null;
                    this.dataGridMainActivity.ItemsSource = null;

                    this.DataGridMainActivityItemsList = this.ActItem.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                    if (this.ActItem.ListItems?.Count > 0 && this.DataGridMainActivityItemsList?.Count > 0)
                    {
                        if (this.dataGridMainActivity.SelectedItem == null)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                        }
                        else
                        {
                            object selectedItem = this.DataGridMainActivityItemsList?.Where(item =>
                            Convert.ToInt32(item.GetType().GetProperty("ActivityItemRefno")?.GetValue(item)) == Convert.ToInt32(this.dataGridMainActivity.SelectedItem.GetType().GetProperty("ActivityItemRefno")?.GetValue(this.dataGridMainActivity.SelectedItem))).FirstOrDefault();
                            this.dataGridMainActivity.SelectedItem = null;
                            this.dataGridMainActivity.SelectedItem = selectedItem;
                            this.dataGridMainActivity. ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, false);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex, this.GetControlDescription(), "Refresh");
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ActItem.UnregLayerListenerProgOut(this);

                await Task.Run(() =>
                {
                    // Explicit GC, Xamarin forms not disposing of objects properly
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DisplayData()
        {
            string prodLocCode = string.Empty;
            string prodLocName = string.Empty;
            string defProdLoc = string.Empty;
            string prodDesc = string.Empty;
            string prodUnitSizeDesc = string.Empty;
            string prodBrandDesc = string.Empty;
            string prodCode = string.Empty;

            try
            {
                if (!this.Loading)
                {
                    this.Loading = true;
                }

                this.ItemsList.Clear();

                if (0 < this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno)
                {
                    this.CmdAddSkidIsEnabled = true;

                    // TODO: Resource strings
                    this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    this.CmdLocationIsEnabled = false;
                }
                else
                {
                    this.CmdAddSkidIsEnabled = false;
                    this.CmdLocationIsEnabled = true;

                    // TODO: Resource strings
                    this.CmdSelectSkidText = "Select Skid";
                }

                if (0 < this.LocMap.CurrLocMap.WhsLocMapRefno)
                {
                    this.CmdLocationText = "Location: " + Environment.NewLine + LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.CmdLocationText = "Set Location";
                }

                switch (this.ActItem.ActSubType)
                {
                    case SysCommon.ActSubTypes.Adjustment:
                        if (this.dataGridMainActivity.SelectedItem is AdjustmentItem adjustmentItem)
                        {
                            prodLocCode = Convert.ToString(adjustmentItem.LocationCode);
                            prodLocName = adjustmentItem.LocationName;
                            prodDesc = adjustmentItem.ProductDescription;
                            prodUnitSizeDesc = adjustmentItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.AssignDefaultLocation:
                        break;

                    case SysCommon.ActSubTypes.BarCodeAssociation:
                        break;

                    case SysCommon.ActSubTypes.BreakSkid:
                        break;

                    case SysCommon.ActSubTypes.ConvertUnit:
                        break;

                    case SysCommon.ActSubTypes.CycleCounts:
                        if (this.dataGridMainActivity.SelectedItem is CountItem cycleCountItem)
                        {
                            prodLocCode = cycleCountItem.LocationCode;
                            prodLocName = cycleCountItem.LocationName;
                            prodDesc = cycleCountItem.ProductDescription;
                            prodUnitSizeDesc = cycleCountItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.DataStatus:
                        break;

                    case SysCommon.ActSubTypes.InventoryInquiry:
                        this.ItemsList.Clear();

                        if (0 >= this.ActItem.ProdInfo.Refno)
                        {
                            // TODO: Resource strings
                            this.ItemsList.Add(string.Format("PRODUCT: {0}", "All"));
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.ItemsList.Add(string.Format("PRODUCT: {0}", ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo)));
                        }

                        // TODO: Resource strings
                        this.ItemsList.Add(string.Format("LOCATION: {0}", LocationMapTasks.GetCurrentReadableLocation()));
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
                        if (this.dataGridMainActivity.SelectedItem is CountItem physicalCountItem)
                        {
                            // Do not check if default location is enabled. Always show the default location.
                            SLocMap tempLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(physicalCountItem.LocationMapRefno));
                            int defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(tempLocMap.WhsLocMapRefno, Convert.ToInt32(physicalCountItem.ProductRefno), SysCommon.ProdLocMapTypes.Picking);
                            defProdLoc = LocationMapTasks.GetCurrentReadableLocation(await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                            prodDesc = physicalCountItem.ProductDescription.ToString();
                            prodUnitSizeDesc = physicalCountItem.ProductUnitSizeDescription.ToString();
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + defProdLoc.Trim());
                        break;

                    case SysCommon.ActSubTypes.Picking:
                        if (this.dataGridMainActivity.SelectedItem is PickingItem pickingItem)
                        {
                            prodLocCode = Convert.ToString(pickingItem.LocationCode);
                            prodLocName = pickingItem.LocationName;
                            prodCode = pickingItem.ProductCode;
                            prodDesc = pickingItem.ProductDescription;
                            prodUnitSizeDesc = pickingItem.ProductUnitSizeDescription;
                            prodBrandDesc = pickingItem.ProductBrandDescription;
                        }

                        this.ItemsList.Clear();

                        switch (this.SysEnv.SysSettings.PickItemDescOpt)
                        {
                            case 1:
                                // TODO: Resource strings
                                this.ItemsList.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " + prodBrandDesc.Trim());
                                break;

                            default:
                                // TODO: Resource strings
                                this.ItemsList.Add("DESC: " + prodDesc.Trim() + ", " + prodUnitSizeDesc.Trim());
                                break;
                        }

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.PickingSingle:
                        if (this.dataGridMainActivity.SelectedItem is PickingItem pickingItemSingle)
                        {
                            prodLocCode = Convert.ToString(pickingItemSingle.LocationCode);
                            prodLocName = pickingItemSingle.LocationName;
                            prodDesc = pickingItemSingle.ProductDescription;
                            prodUnitSizeDesc = pickingItemSingle.ProductUnitSizeDescription;
                            prodBrandDesc = pickingItemSingle.ProductBrandDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.TicketAddProduct:
                        if (this.dataGridMainActivity.SelectedItem is ProductionTicketByProductItem productionTicketByProductItem)
                        {
                            prodLocCode = Convert.ToString(productionTicketByProductItem.LocationCode);
                            prodLocName = productionTicketByProductItem.LocationName;
                            prodDesc = productionTicketByProductItem.ProductDescription;
                            prodUnitSizeDesc = productionTicketByProductItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.TicketFinished:
                        if (this.dataGridMainActivity.SelectedItem is ProductionTicketFinishedItem productionTicketFinishedItem)
                        {
                            prodLocCode = Convert.ToString(productionTicketFinishedItem.LocationCode);
                            prodLocName = productionTicketFinishedItem.LocationName;
                            prodDesc = productionTicketFinishedItem.ProductDescription;
                            prodUnitSizeDesc = productionTicketFinishedItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        //this.lstDisplay.Items.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.TicketMaterial:
                        if (this.dataGridMainActivity.SelectedItem is ProductionTicketMaterialItem productionTicketMaterialItem)
                        {
                            prodLocCode = Convert.ToString(productionTicketMaterialItem.LocationCode);
                            prodLocName = productionTicketMaterialItem.LocationName;
                            prodDesc = productionTicketMaterialItem.ProductDescription;
                            prodUnitSizeDesc = productionTicketMaterialItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                        if (this.dataGridMainActivity.SelectedItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
                        {
                            prodLocCode = Convert.ToString(purchaseOrderWorkSheetItem.LocationCode);
                            prodLocName = purchaseOrderWorkSheetItem.LocationName;
                            prodDesc = purchaseOrderWorkSheetItem.ProductDescription;
                            prodUnitSizeDesc = purchaseOrderWorkSheetItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.PurgeActivityData:
                        break;

                    case SysCommon.ActSubTypes.PutAwaySkid:
                        break;

                    case SysCommon.ActSubTypes.Receiving:
                        if (this.dataGridMainActivity.SelectedItem is ReceiveItem receiveItem)
                        {
                            prodLocCode = Convert.ToString(receiveItem.LocationCode);
                            prodLocName = receiveItem.LocationName;
                            prodDesc = receiveItem.ProductDescription;
                            prodUnitSizeDesc = receiveItem.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.RefreshData:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByItemSection:
                        break;

                    case SysCommon.ActSubTypes.ReleaseByOrder:
                        break;

                    case SysCommon.ActSubTypes.Returns:
                        if (this.dataGridMainActivity.SelectedItem is ReturnItem returnItem)
                        {
                            prodLocCode = Convert.ToString(returnItem.LocationCode);
                            prodLocName = returnItem.LocationName;
                            prodCode = returnItem.ProductCode;
                            prodDesc = returnItem.ProductDescription;
                            prodUnitSizeDesc = returnItem.ProductUnitSizeDescription;
                            prodBrandDesc = returnItem.ProductBrandDescription;
                        }

                        this.ItemsList.Clear();

                        // TODO: Resource strings
                        this.ItemsList.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " + prodBrandDesc.Trim());
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.SelectLocation:
                        break;

                    case SysCommon.ActSubTypes.SelectProduct:
                        break;

                    case SysCommon.ActSubTypes.SkidInfo:
                        break;

                    case SysCommon.ActSubTypes.TransferFrom:
                        if (this.dataGridMainActivity.SelectedItem is TransferItem transferItemFrom)
                        {
                            prodLocCode = Convert.ToString(transferItemFrom.LocationCode);
                            prodLocName = transferItemFrom.LocationName;
                            prodDesc = transferItemFrom.ProductDescription;
                            prodUnitSizeDesc = transferItemFrom.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;

                    case SysCommon.ActSubTypes.TransferTo:
                        if (this.dataGridMainActivity.SelectedItem is TransferItem transferItemTo)
                        {
                            prodLocCode = Convert.ToString(transferItemTo.LocationCode);
                            prodLocName = transferItemTo.LocationName;
                            prodDesc = transferItemTo.ProductDescription;
                            prodUnitSizeDesc = transferItemTo.ProductUnitSizeDescription;
                        }

                        this.ItemsList.Clear();
                        this.ItemsList.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                        // TODO: Resource strings
                        this.ItemsList.Add("LOCATION: " + prodLocCode.Trim() + " " + prodLocName.Trim());
                        break;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Display Data", ex.Message, "OK"));
            }
            finally
            {
                if (this.Loading)
                {
                    this.Loading = false;
                }
            }
        }

        private async void TxtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.DataGridMainActivityItemsList = null;
                this.dataGridMainActivity.ItemsSource = null;

                this.DataGridMainActivityItemsList = this.ActItem.Search(this.txtSearchItem.Text);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.dataGridMainActivity.SelectedItem = null;
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                    if (this.DataGridMainActivityItemsList?.Count > 0)
                    {
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                    }
                });
            }
            catch (NotImplementedException ex)
            {
                // Handle not implemented
                // TODO: Logging
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Not implemented", ex.Message, "OK"));
                this.txtSearchItem.Text = string.Empty;
                this.txtSearchItem.IsEnabled = false;
            }
        }

        private void TxtSearchItem_Focused(object sender, FocusEventArgs e)
        {
            this.txtSearchItem.CursorPosition = 0;
            this.txtSearchItem.SelectionLength = this.txtSearchItem.Text != null && this.txtSearchItem.Text.Length > 0 ? this.txtSearchItem.Text.Length : 0;
        }

        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = this.updnBarCodeModifier1.SelectedItem.ToString();
        }

        private async void DataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.SelectedIndex = e.SelectedItemIndex;
            await this.ActItem.SelectActivityItem(e.SelectedItem);
            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayData());
        }

        private async Task CmdFilter_Clicked(object sender, EventArgs e)
        {
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            // TODO: Resource strings
            string result = await DisplayActionSheet("ActivityItem Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.CmdLocationText && this.CmdLocationIsEnabled)
            {
                await this.cmdLocation_Click(sender, e);
            }
            else if (result == this.CmdFilterText && this.CmdFilterIsEnabled)
            {
                await this.CmdFilter_Clicked(sender, e);
            }
            else if (result == this.CmdSelectSkidText && this.CmdSelectSkidIsEnabled)
            {
                await this.cmdSelectSkid_Click(sender, e);
            }
            else if (result == this.CmdAddProductText && this.CmdAddProductIsEnabled)
            {
                await this.cmdAddProduct_Click(sender, e);
            }
            else if (result == this.CmdAddSkidText && this.CmdAddSkidIsEnabled)
            {
                await this.cmdAddSkid_Click(sender, e);
            }
        }
    }
}
