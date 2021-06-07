using dMobile.View.CustomPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityMoveInventory : DMobileContentPage, IAppArea, IDataLayerEventListener
    {
        #region Fields

        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private BarCodeParse barCodeParse = null;
        private int prodUnitRefno = 0;
        private int prodUnitBarCodeRefno = 0;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private SLocMap selFromLocMap;
        private int selFromCurrLocMapRefno = 0;
        private SLocMap selToLocMap;
        private int selToCurrLocMapRefno = 0;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.MoveInventory;
        private ObservableCollection<string> itemsList;
        private ObservableCollection<object> dataGridMainActivityItemsList;
        private List<string> optionsMenuItems = new List<string>();

        #endregion

        #region Constructors

        public ucActivityMoveInventory()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityMoveInventory(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdFromLocation.Text = "Set From Location";
            this.cmdAdd.Text = "Add";
            this.cmdToLocation.Text = "Set To Location";
            this.lblScanToLocation.Text = "Scan To Loc.";

            this.DataGridMainActivityItemsList = new ObservableCollection<object>();
            this.ItemsList = new ObservableCollection<string>();

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SelFromLocMap = new SLocMap();
            this.SelToLocMap = new SLocMap();

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

            this.ActSubType = actSubType;
            this.Activity = ActivityFacade.GetActivity(actSubType);
            this.Activity.SkidInfo = new SysSkidInfo();
            this.ActItem = ActItemFacade.GetActItem(this.Activity, actSubType);
            this.ActItem.ActHdr = Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, actSubType);
            this.ActDetail.ActItem = ActItem;
            this.ActDetail.ActExtra = new ActDetailExtra();

            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.Title = this.GetControlDescription();

            this.optionsMenuItems.Add("Save");
            this.optionsMenuItems.Add("Cancel");
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

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Del || keycode == Keycode.Clear || keycode == Keycode.Back)
                {
                    // TODO: Resource strings
                    if (this.dataGridMainActivity.SelectedItem != null && this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) != -1 && await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Do you wish to delete this record?", "Yes", "No")))
                    {
                        // TODO: Test
                        this.ActDetail.ListDetails.RemoveAt(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem));
                    }
                }
            });

            try
            {
                this.Loading = true;
                await Task.Run(async () => await this.ucActivityMoveInventory_GotFocus());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityMoveInventory_GotFocus threw an error " + ex, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await ScannerDisable();

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

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

        private int SelToCurrLocMapRefno
        {
            get
            {
                return this.selToCurrLocMapRefno;
            }

            set
            {
                this.selToCurrLocMapRefno = value;
            }
        }

        private int SelFromCurrLocMapRefno
        {
            get
            {
                return this.selFromCurrLocMapRefno;
            }
            set
            {
                this.selFromCurrLocMapRefno = value;
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
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
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

        private SLocMap SelToLocMap
        {
            get
            {
                return this.selToLocMap;
            }

            set
            {
                this.selToLocMap = value;
            }
        }

        private SLocMap SelFromLocMap
        {
            get
            {
                return this.selFromLocMap;
            }

            set
            {
                this.selFromLocMap = value;
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

        private int ProdUnitRefno
        {
            get
            {
                return this.prodUnitRefno;
            }

            set
            {
                this.prodUnitRefno = value;
            }
        }

        private int ProdUnitBarCodeRefno
        {
            get
            {
                return this.prodUnitBarCodeRefno;
            }

            set
            {
                this.prodUnitBarCodeRefno = value;
            }
        }

        private BarCodeParse BarCodeParse
        {
            get
            {
                return this.barCodeParse;
            }

            set
            {
                this.barCodeParse = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
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
            bool isReadyToSave = true;

            if (ActDetail.ListDetails.Count > 0)
            {
                if (!await LocationMapTasks.IsCompleteLocMapRefno(SelFromLocMap))
                {
                    isReadyToSave = false;

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Send Moves", "Invalid Move \'From\' Location", "OK"));

                }
                else if (!await LocationMapTasks.IsCompleteLocMapRefno(SelToLocMap))
                {
                    isReadyToSave = false;

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Send Moves", "Invalid Move \'To\' Location", "OK"));

                }
                else if (SelFromLocMap.WhsLocMapRefno != SelToLocMap.WhsLocMapRefno)
                {
                    isReadyToSave = false;

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Send Moves", "The \'From\' And \'To\' Warehouse(s) Must Be The Same", "OK"));
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
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (Scanner != null)
            {
                await ScannerDisable();
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Move Inventory";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool bSaveComplete = false;

            try
            {
                this.Loading = true;

                await Task.Run(async () =>
                {
                    if (await IsReadyToSave())
                    {
                        if (ActDetail.ListDetails != null && ActDetail.ListDetails.Count > 0)
                        {
                            for (int idx = 0; idx < ActDetail.ListDetails.Count; idx++)
                            {
                                try
                                {
                                    if (ActDetail.ListDetails.Count > 0)
                                    {
                                        ActItem.FKRefno = 0;
                                        ActItem.Refno = 0;
                                        ProductInfo oProductInfo = ActItem.ProdInfo;
                                        oProductInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(ActDetail.ListDetails[idx].GetType().GetProperty("ProductUnitRefno")?.GetValue(ActDetail.ListDetails[idx])));
                                        oProductInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(oProductInfo.Refno);
                                        ActItem.ProdInfo = oProductInfo;
                                        ActDetail = await ActDetail.GetInfo(ActDetail.ListDetails[idx], false);
                                        XDocument xmlParam = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<param>" + ("<whs_id>" + SelFromLocMap.WhsFKRefno.ToString() + "</whs_id>") + ("<sect_refno>" + SelFromLocMap.SectFKRefno.ToString() + "</sect_refno>") + ("<slot_refno>" + SelFromLocMap.SlotFKRefno.ToString() + "</slot_refno>") + ("<to_sect_refno>" + SelToLocMap.SectFKRefno.ToString() + "</to_sect_refno>") + ("<to_slot_refno>" + SelToLocMap.SlotFKRefno.ToString() + "</to_slot_refno>") + "</param>");

                                        await ActDetail.Submit(ActDetail.ListDetails[idx], xmlParam);
                                        idx--;
                                    }
                                    else
                                    {
                                        goto endOfForLoop;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    // TODO: Resource strings
                                    bool result = await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Move Submission", Convert.ToString("Submit Move Failed" + Environment.NewLine + ex.Message), "Cancel", "Retry"));

                                    if (result)
                                    {
                                        bSaveComplete = false;
                                        goto endOfForLoop;

                                    }
                                    else
                                    {
                                        if (ActDetail.ListDetails.Count > 0)
                                        {
                                            idx--;
                                        }
                                    }
                                }
                            }

                        endOfForLoop:

                            if (ActDetail.ListDetails.Count > 0)
                            {
                                bSaveComplete = false;
                            }
                            else
                            {
                                // TODO: Resource strings
                                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Move Submission", "Finished Moves?", "Yes", "No")))
                                {
                                    bSaveComplete = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Validation", "Data Failed Validation. Check Your Data.", "OK"));
                    }
                });
            }
            catch (Exception ex)
            {
                bSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
            }

            return bSaveComplete;
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
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.MoveInventory, ex);
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
            XDocument xmlDoc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<params>" + "</params>");
            return xmlDoc;
        }

        #endregion

        #region IDataLayerEventListener Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            this.ItemsList.Clear();

            //Tokenize data longer than 30 characters for display, wrap to next line
            const int tokenLength = 30;

            if (dataLayerEventArgs.StatusOutMsg.Length > tokenLength)
            {
                int iTokens = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dataLayerEventArgs.StatusOutMsg.Length / tokenLength)));
                int iTokenLength = 0;

                for (var iTokenIdx = 0; iTokenIdx <= (iTokens * tokenLength) - 1; iTokenIdx++)
                {
                    iTokenLength = Convert.ToInt32(dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx > tokenLength ? tokenLength : dataLayerEventArgs.StatusOutMsg.Length - iTokenIdx);
                    this.ItemsList.Add(dataLayerEventArgs.StatusOutMsg.Substring(Convert.ToInt32(iTokenIdx), iTokenLength));
                    iTokenIdx += tokenLength;
                }
            }
            else
            {
                this.ItemsList.Add(dataLayerEventArgs.StatusOutMsg);
            }
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

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdFromLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    this.SelFromLocMap = LocMap.CurrLocMap;
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "Error selecting to location." + Environment.NewLine + ex.Message, "OK"));
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.SelToLocMap = this.LocMap.CurrLocMap;
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "Error selecting to location." + Environment.NewLine + ex.Message, "OK"));
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
        private async Task ucActivityMoveInventory_GotFocus()
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await Device.InvokeOnMainThreadAsync(() => this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);

                if (this.IsControlLoaded == false)
                {
                    this.SelFromLocMap = this.LocMap.CurrLocMap;
                    this.SelFromCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                    }

                    this.SelToLocMap = this.LocMap.CurrLocMap;
                    this.SelToCurrLocMapRefno = this.LocMap.CurrLocMapRefno;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                    }

                    await this.RefreshData();
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
        private async void cmdAdd_Click(object sender, EventArgs e)
        {
            int prodUnitRefno = 0;

            try
            {
                this.Loading = true;
                await this.ScannerDisable();

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    XDocument xmlDoc = await form.GetXML();
                    prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                    object infoObject = null;

                    await this.ActItem.Insert(xmlDoc, false);

                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        infoObject = this.ActDetail.ListDetails[0];
                    }

                    this.ActDetail = await this.ActDetail.GetInfo(infoObject, true);

                    SysCommon.SFillQuantity fillQty = SysCommon.FillQuantityStructureFromQuantity((ProductInfo)this.ActItem.ProdInfo, 1);
                    SActItemSchemeData tempItemSchemeData = new SActItemSchemeData
                    {
                        ActDetailQuantity = fillQty.Quantity,
                        ActDetailBaseQuantity = fillQty.BaseQuantity,
                        ActDetailWeightQuantity = fillQty.WeightQuantity
                    };

                    bool isInsertReady = false;
                    int foundRowIdx = -1;

                    if (this.ActDetail.ListDetails.Count > 0)
                    {
                        foundRowIdx = this.ActDetail.ListDetails.Count - 1;
                    }

                    this.BarCodeParse = BarCodeParseFacade.ParseData(string.Empty);

                    this.ActDetail.Quantity = 0;
                    this.ActDetail.BaseQuantity = 0;
                    this.ActDetail.WeightQuantity = 0;
                    this.ActDetail.SchemeID = string.Empty;
                    this.ActDetail.UniqueSchemeID = string.Empty;
                    this.ActDetail.ActExtra.Text1 = string.Empty;
                    this.ActDetail.ActExtra.Text2 = string.Empty;
                    this.ActDetail.ActExtra.Text3 = string.Empty;
                    this.ActDetail.ActExtra.Text4 = string.Empty;
                    this.ActDetail.ActExtra.DateTime5 = new DateTime(1980, 1, 1);

                    isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, true);
                    this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                        await errorReport.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });
            }
            finally
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                });

                await this.ScannerEnable();
                this.Loading = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iCurrentRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="oBarCodeParse"></param>
        /// <remarks></remarks>
        private void InsertActivityDetail(int rowIdx, ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);
                this.ActDetail.Insert(xmlDoc);

                if (this.ActDetail.ListDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > this.ActDetail.ListDetails.Count - 1)
                {
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.ActDetail.ListDetails.Count - 1];
                }
            }
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
            XDocument xmlDoc = null;
            int prodUnitRefno = 0;
            //int rowIdx = this.dataGridMainActivity.CurrentRowIndex;
            int rowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);

            try
            {
                this.BarCodeParse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                if (!string.IsNullOrEmpty(barCodeParse.LocationIdentifier))
                {
                    if (this.chkScanToLocation.IsChecked)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));
                        this.SelToLocMap = LocMap.CurrLocMap;

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "To: " + LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdToLocation.Text = "Set To Location");
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));
                        this.SelFromLocMap = this.LocMap.CurrLocMap;

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "From: " + LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(() => this.cmdFromLocation.Text = "Set From Location");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                    {
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.BarCodeParse, prodUnitRefno);

                        xmlDoc = XDocument.Parse(
                              "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                              + "<params>"
                              + ("<ProductUnitRefno>" + prodUnitRefno + "</ProductUnitRefno>")
                              + "</params>");

                        await this.ActItem.Insert(xmlDoc, false);

                        object infoObject = null;

                        if (this.ActDetail.ListDetails.Count > 0)
                        {
                            infoObject = this.ActDetail.ListDetails[0];
                        }

                        this.ActDetail = await this.ActDetail.GetInfo(infoObject, true);

                        SActItemSchemeData tempItemSchemeData = new SActItemSchemeData();
                        bool isInsertReady = false;

                        int foundRowIdx = -1;

                        if (this.ActDetail.ListDetails.Count > 0)
                        {
                            foundRowIdx = this.ActDetail.ListDetails.Count - 1;
                        }

                        isInsertReady = await SysShared.GatherProductData(this.BarCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, this.ActDetail, false);
                        this.InsertActivityDetail(foundRowIdx, this.ActDetail, this.BarCodeParse, isInsertReady);

                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.ActFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an header reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code # is not supported here.");
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning bar code type is not supported here.");
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);

                if (this.ActDetail.ListDetails.Count > 0 && rowIdx >= 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[rowIdx]);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(this.ActSubType, ex);
                    await errorReport.ShowDialog();
                });

                if (this.ActDetail.ListDetails.Count > 0 && rowIdx >= 0)
                {
                    await Device.InvokeOnMainThreadAsync(() => this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[rowIdx]);
                }
            }
            finally
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                });

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
            await this.ScannerDisable();
            await this.ActDetail.GetActivityDetailList(false);

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                this.ItemsList.Clear();

                this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                if (this.ActDetail.ListDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                {
                    this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                    await this.ActDetail.SelectActivityDetail(this.ActDetail.ListDetails[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)]);
                }
            });

            await this.ScannerEnable();
        }

        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            // TODO: Resource strings
            string result = await DisplayActionSheet("Move Inventory Options", "Close", null, this.optionsMenuItems.ToArray());

            if (result == "Save")
            {
                if (await this.SaveChanges())
                {
                    await this.Close();
                }
            }
            else if (result == "Cancel")
            {
                if (await this.CancelChanges())
                {
                    await this.Close();
                }
            }
        }

        #endregion
    }
}