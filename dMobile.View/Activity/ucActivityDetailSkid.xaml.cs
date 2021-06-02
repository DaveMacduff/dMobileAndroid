using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
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
using System.Xml;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityDetailSkid : DMobileModal, IAppArea
    {
        #region Fields

        /// <summary>
        /// Scanner can be toggled between add and remove mode.
        /// It defaults to Add Mode.
        /// </summary>
        private bool scannerModeAdd;
        private Color addModeHeaderColor, removeModeHeaderColor;

        private ActDetail actDetail;
        private List<object> listActDetails;
        private LocationMap locMap = LocationMap.GetInstance();
        private BarCodeParse parse = null;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private SysCommon.ActSubTypes actSubType;
        private int totalTicks = 0;
        private bool isControlLoaded = false;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();
        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_actItem"></param>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityDetailSkid(ActItem actItem, SysCommon.ActSubTypes actSubTypes, SysCommon.ActSubTypes callingActSubType)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdAdd.Text = "Add";

            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.ActSubType = actSubTypes;
            this.LocMap = actItem.LocMap;
            this.CallingActSubType = callingActSubType;

            this.ActDetail = ActDetailFacade.GetActDetail(actItem, this.ActSubType);
            this.ActDetail.LocMap = actItem.LocMap;
            SProductSchemeData tempSchemeData = this.ActDetail.SchemeData;
            tempSchemeData.LogisticUnitSkidNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
            this.ActDetail.SchemeData = tempSchemeData;

            this.addModeHeaderColor = Color.FromRgb(192, 255, 192);
            this.removeModeHeaderColor = Color.PaleVioletRed;
            this.ScannerModeAdd = true;
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
                    if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            this.DialogResult = DialogResult.None;
                            await this.CancelChanges();
                        }

                        await this.Close();
                    }
                    else if (keycode == Keycode.Del || keycode == Keycode.Clear)
                    {
                        await this.ScannerDisable();

                        // Instead of deleting the ActDetail, we are going to prompt the user how many boxes to remove.
                        // They remove all of them? Then delete the skid.
                        // Otherwise, need to call com server with how many were removed from the skid

                        if (this.dataGridMainActivity.SelectedItem == null)
                        {
                            return;
                        }

                        if (this.dataGridMainActivity.SelectedItem != null)
                        {
                            this.ActDetail = await this.ActDetail.GetInfo(this.dataGridMainActivity.SelectedItem, false);
                            await PromptRemove();
                        }

                        if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.IsConsumed)
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "This skid is now consumed.", "OK"));

                            if (await this.SaveChanges())
                            {
                                this.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                this.DialogResult = DialogResult.None;
                                await this.CancelChanges();
                            }
                        }
                    }
                    else if (keycode == Keycode.F5)
                    {
                        ToggleScannerMode();
                    }

                    // TODO: Add arrow key navigation. Window focus keeps dropping in this view for some reason, will figure out in a later build
                    //else if (keycode == Keycode.DpadDown)
                    //{
                    //    if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) < this.DataGridMainActivityItemsList.Count - 1)
                    //    {
                    //        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) + 1];
                    //        this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                    //    }
                    //}
                    //else if (keycode == Keycode.DpadUp)
                    //{
                    //    this.dataGridMainActivity.Focus();
                    //    if (this.dataGridMainActivity.SelectedItem != null && Convert.ToInt32(this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem)) > 0)
                    //    {
                    //        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem) - 1];
                    //        this.dataGridMainActivity.ScrollTo(this.dataGridMainActivity.SelectedItem, ScrollToPosition.MakeVisible, true);
                    //    }
                    //}
                }
            });

            try
            {
                await this.ucActivityDetail_GotFocus(this, new EventArgs());
                await this.tmrActivityDetailLoaded_Tick(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucActivityDetail_GotFocus/tmrActivityDetailLoaded_Tick threw an error " + ex, "OK"));
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

        private SysCommon.ActSubTypes CallingActSubType
        {
            get
            {
                return this.callingActSubType;
            }

            set
            {
                this.callingActSubType = value;
            }
        }

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

        private List<object> ListActivityDetails
        {
            get
            {
                return this.listActDetails;
            }

            set
            {
                this.listActDetails = value;
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

        /// <summary>
        /// Scanner is in Add Mode (true). Scanning a barcode will add it.
        /// Scanner is in Remove Mode (false). Scanning a barcode will remove it.
        /// </summary>
        private bool ScannerModeAdd
        {
            get
            {
                return scannerModeAdd;
            }
            set
            {
                if (value == scannerModeAdd)
                {
                    return;
                }

                scannerModeAdd = value;
                UpdateScannerMode();
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
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }

            return true;
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
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Skid Detail";
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
                    SysCommon.SFillQuantity fillQty = this.GetActivityItemQuantity();
                    SSkidInfo currSkidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    currSkidInfo.Quantity = fillQty.Quantity;
                    currSkidInfo.BaseQuantity = fillQty.BaseQuantity;
                    currSkidInfo.WeightQuantity = fillQty.WeightQuantity;
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid = currSkidInfo;
                    isSaveComplete = true;
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }

                this.DisplaySummaryData();
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

            return isSaveComplete;
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
                    SysCommon.SFillQuantity fillQty = this.GetActivityItemQuantity();
                    SSkidInfo currSkidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

                    currSkidInfo.Quantity = fillQty.Quantity;
                    currSkidInfo.BaseQuantity = fillQty.BaseQuantity;
                    currSkidInfo.WeightQuantity = fillQty.WeightQuantity;
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid = currSkidInfo;

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
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
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
            SysCommon.SFillQuantity fillQty = this.GetActivityItemQuantity();

            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<qty>" + fillQty.Quantity + "</qty>")
                + ("<base_qty>" + fillQty.BaseQuantity + "</base_qty>")
                + ("<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>")
                + "</params>");

            return xmldocParams;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// Used to move the grid row to the correct position, And set the state of some controls.
        /// </summary>
        /// <param name="svdActDetailFKRefno"></param>
        /// <param name="currRowIdx"></param>
        private void CleanUpBarCodeHandler(int svdActDetailFKRefno, int currRowIdx)
        {
            this.ActDetail.FKRefno = svdActDetailFKRefno;

            if (this.ActDetail.ListDetails.Count > 0 && currRowIdx + 1 <= this.ActDetail.ListDetails.Count && this.DataGridMainActivityItemsList.Count > currRowIdx)
            {
                //this.dataGridMainActivity.CurrentRowIndex = currRowIdx;
                this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[currRowIdx];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs"></param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            int prodUnitRefno = 0;
            int currRowIdx = this.DataGridMainActivityItemsList.IndexOf(this.dataGridMainActivity.SelectedItem);
            int svdActDetailFKRefno = this.ActDetail.FKRefno;

            try
            {
                this.Parse = scanArgs.ParseInfo;

                await this.ScannerDisable();

                if (this.CallingActSubType != SysCommon.ActSubTypes.SkidInfo)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Adding product to a skid can only be done in skid maintenance.");
                }

                if (!string.IsNullOrEmpty(this.Parse.LocationIdentifier))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning a location code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ActFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an activity header reference number is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an activity item reference number is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.SSCC18))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an shipping container code is not supported here.");
                }

                if (!string.IsNullOrEmpty(this.Parse.ProductIdentifier))
                {
                    if (!await LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Location must be set first.");
                    }

                    prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, this.Parse, prodUnitRefno);

                    if (prodUnitRefno != this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno)
                    {
                        // TODO: Resource strings
                        throw new Exception("Scanned product does not match selected skid product.");
                    }

                    SProductSchemeData schemeData = this.ActDetail.SchemeData;
                    schemeData.LogisticUnitSkidNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    schemeData.LogisticUnitBatchLotNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                    schemeData.LogisticUnitBestBeforeSellByDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                    schemeData.LogisticUnitProductionDate = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                    schemeData.VariantCode = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                    schemeData.VariantNumber = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                    schemeData.VariantDescription = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                    this.ActDetail.SchemeData = schemeData;

                    bool isReadyToInsertDetail = false;
                    SActItemSchemeData itemSchemeData = new SActItemSchemeData();
                    itemSchemeData.ProdSchemeData = schemeData;

                    isReadyToInsertDetail = await SysShared.GatherProductData(this.Parse, this.ActSubType, this.ActDetail.ActItem, itemSchemeData, isReadyToInsertDetail, this.ActDetail, false);

                    // If add mode / else remove mode
                    if (ScannerModeAdd)
                    {
                        await this.InsertActivityDetail(this.ActDetail, this.Parse, isReadyToInsertDetail);
                    }
                    else // Remove
                    {
                        await RemoveActivityDetail();
                    }

                    await this.ActDetail.SelectActivityDetail(this.DataGridMainActivityItemsList[currRowIdx]);

                    if (this.DataGridMainActivityItemsList.Count > currRowIdx)
                    {
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[currRowIdx];
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanned bar code type is not supported here.");
                }

                this.DisplaySummaryData();
            }
            catch (IsReadyToApplyException)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);
            }
            catch (GatherProductDataException ex)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                this.CleanUpBarCodeHandler(svdActDetailFKRefno, currRowIdx);

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
        /// <param name="currRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="parse"></param>
        /// <param name="isInsertDetailReady"></param>
        private async Task InsertActivityDetail(ActDetail actDetail, BarCodeParse parse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                actDetail.FKRefno = 0;
                XDocument xmlDoc = actDetail.GetXML(actDetail);
                await actDetail.Insert(xmlDoc);
            }

            if (actDetail.ListDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
            {
                this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[this.DataGridMainActivityItemsList.Count - 1];

            }
        }

        /// <summary>
        /// Remove a scanned activity detail.
        /// </summary>
        private async Task RemoveActivityDetail()
        {
            actDetail.FKRefno = 0;
            XDocument xmlDoc = actDetail.GetXML(actDetail);
            await actDetail.FillActDetailObjFromXML(xmlDoc);
            await actDetail.Delete();
        }

        /// <summary>
        /// Finds the index of the row matching the parse data.
        /// -1 if it does not exist.
        /// </summary>
        /// <param name="parse">BarCodeParse data</param>
        /// <returns>Row index if it exists; otherwise -1</returns>
        private int BarCodeToRowIndex(BarCodeParse parse)
        {
            // Search through the data grid, looking to match barcode parse

            return -1;
        }


        /// <summary>
        /// Updates the screen based on the scanner mode.
        /// </summary>
        private void UpdateScannerMode()
        {
            if (ScannerModeAdd)
            {
                // TODO: Resource strings
                this.scannerModeText.Text = "Adding";
                this.cmdAdd.Text = "Add";
            }
            else
            {
                // TODO: Resource strings
                this.scannerModeText.Text = "Removing";
                this.cmdAdd.Text = "Remove";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private SysCommon.SFillQuantity GetActivityItemQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();

            if (this.ListActivityDetails != null)
            {
                if (this.ListActivityDetails.Count > 0)
                {
                    for (int i = 0; i <= this.ListActivityDetails.Count - 1; i++)
                    {
                        fillQty.Quantity += Convert.ToDecimal(this.ListActivityDetails[i].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(this.ListActivityDetails[i]));
                        fillQty.BaseQuantity += Convert.ToDecimal(this.ListActivityDetails[i].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(this.ListActivityDetails[i]));
                        fillQty.WeightQuantity += Convert.ToDecimal(this.ListActivityDetails[i].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(this.ListActivityDetails[i]));
                    }
                }
            }

            return fillQty;
        }

        /// <summary>
        /// Adds data to the bottom list box to show
        /// Product ID, Qty and Weight.
        /// </summary>
        private void DisplaySummaryData()
        {
            SSkidInfo skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
            SysCommon.SFillQuantity fillQty = this.GetActivityItemQuantity();

            // TODO: Resource strings
            this.ItemsList.Clear();
            this.ItemsList.Add(string.Format("Product ID {0}", skidInfo.ProdInfo.Code));
            this.ItemsList.Add(string.Format("Quantity {0} {1} @ {2} {3}", fillQty.Quantity, skidInfo.ProdInfo.UnitCode, fillQty.WeightQuantity, skidInfo.ProdInfo.WeightUnitCode));
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task RefreshData()
        {
            try
            {
                await this.ScannerDisable();

                this.ListActivityDetails = await this.ActDetail.GetActivityDetailList(false);
                await this.ActDetail.Refresh();

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.DataGridMainActivityItemsList = this.ActDetail.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                    this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
                    this.ItemsList.Clear();

                    if (this.ListActivityDetails.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                    {
                        this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                        await this.ActDetail.SelectActivityDetail(this.dataGridMainActivity.SelectedItem);
                    }

                    this.DisplaySummaryData();
                });
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), ex.Message, "OK"));
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
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityDetail_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                UpdateScannerMode();

                if (this.IsControlLoaded == false)
                {
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
        private void txtSearchHdr_TextChanged(object sender, EventArgs e)
        {
            this.DataGridMainActivityItemsList = this.ActDetail.Search(this.txtSearchDetail.Text);
            this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;
        }

        private async Task PromptRemove()
        {
            if (this.ActDetail.Quantity == 1 ||
                // picking not allowed to remove boxes from skid
                this.CallingActSubType == SysCommon.ActSubTypes.Picking ||
                this.CallingActSubType == SysCommon.ActSubTypes.PickingSingle)
            {
                if (await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(this.GetControlDescription(), "Do You Wish To Remove This Record From This Skid?", "Yes", "No")))
                {
                    try
                    {
                        // special case: only 1, so remove!
                        SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity
                        {
                            Quantity = 0,
                            BaseQuantity = 0,
                            WeightQuantity = 0
                        };

                        await UpdateQty(fillQty, false);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "Error removing record from skid." + Environment.NewLine + ex.Message, "OK"));
                    }
                }
            }
            else
            {
                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityRemoveFromSkid, actDetail, actDetail.ActSubType);

                if (await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    try
                    {
                        XDocument xmlDoc = await form.GetXML();

                        // Create an activity detail for it now
                        SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity
                        {
                            Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value),
                            BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value),
                            WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value)
                        };

                        await UpdateQty(fillQty, true);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Resource strings
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "Error removing record from skid." + Environment.NewLine + ex.Message, "OK"));
                    }
                }
            }
        }

        /// <summary>
        /// Removes the quantity from a line.
        /// </summary>
        /// <param name="fillQty"></param>
        private async Task UpdateQty(SysCommon.SFillQuantity fillQty, bool modifyQuantity)
        {
            XDocument data = new XDocument();
            string xmlReq = await this.ActDetail.CreateDeleteXML(data);

            // replace default fields with fillQty
            if (modifyQuantity)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xmlReq);
                xdoc.DocumentElement["alt_qty"].InnerText = fillQty.Quantity.ToString();
                xdoc.DocumentElement["base_qty"].InnerText = fillQty.BaseQuantity.ToString();
                xdoc.DocumentElement["wt_qty"].InnerText = fillQty.WeightQuantity.ToString();

                xmlReq = xdoc.OuterXml;
            }
            string reqData = SysCommon.SubmitComm(xmlReq);

            if (reqData.Length > 0)
            {
                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    XmlDocument xmlError = new XmlDocument();
                    xmlError.LoadXml(reqData);
                    string errorDetail = xmlError.DocumentElement["detail"].InnerText;
                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new Exceptions.MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
            }

            await ShowUpdateOnGrid(fillQty);
        }

        /// <summary>
        /// Updates the grid to show the updated values.
        /// </summary>
        /// <param name="fillQty"></param>
        private async Task ShowUpdateOnGrid(SysCommon.SFillQuantity fillQty)
        {
            dynamic selectedItem = this.dataGridMainActivity.SelectedItem;

            if (selectedItem == null)
            {
                return;
            }

            decimal currQty = Convert.ToDecimal(selectedItem.ActivityDetailQuantity);
            decimal currBaseQty = Convert.ToDecimal(selectedItem.ActivityDetailBaseQuantity);
            decimal currWtQty = Convert.ToDecimal(selectedItem.ActivityDetailWeightQuantity);

            selectedItem.ActivityDetailQuantity = Convert.ToString(currQty - fillQty.Quantity);
            selectedItem.ActivityDetailBaseQuantity = Convert.ToString(currBaseQty - fillQty.BaseQuantity);
            selectedItem.ActivityDetailWeightQuantity = Convert.ToString(currWtQty - fillQty.WeightQuantity);

            await this.RefreshData();
            this.DisplaySummaryData();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchDetail_GotFocus(object sender, EventArgs e)
        {
            if (this.txtSearchDetail.Text != null)
            {
                this.txtSearchDetail.CursorPosition = 0;
                this.txtSearchDetail.SelectionLength = this.txtSearchDetail.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task tmrActivityDetailLoaded_Tick(object sender, EventArgs e)
        {
            await this.RefreshData();

            if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo)
            {
                this.cmdAdd.IsEnabled = true;
            }
            else
            {
                this.cmdAdd.IsEnabled = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void AddActivityDetail(object sender, EventArgs e)
        {
            if (!ScannerModeAdd)
            {
                await RemoveActivityDetail();
                return;
            }

            // Pass 0 as the current selected row on an add, true forces it as an add not an edit
            this.ActDetail = await this.ActDetail.GetInfo(this.DataGridMainActivityItemsList[0], true);

            await this.ScannerDisable();

            try
            {
                BarCodeParse parse = BarCodeParseFacade.ParseData(string.Empty);
                bool isInsertReady = false;

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

                SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                SProductSchemeData schemeData = this.ActDetail.SchemeData;
                SSkidInfo currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                // Did not use currSkid.LogisticUnitSkidCode because of the len of the skid code 
                // and the fact is isn't human readable friendly.
                schemeData.LogisticUnitSkidNumber = currSkid.SkidCode;
                schemeData.LogisticUnitBatchLotNumber = currSkid.SchemeData.LogisticUnitBatchLotNumber;
                schemeData.LogisticUnitBestBeforeSellByDate = currSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                schemeData.LogisticUnitProductionDate = currSkid.SchemeData.LogisticUnitProductionDate;
                schemeData.VariantCode = currSkid.SchemeData.VariantCode;
                schemeData.VariantNumber = currSkid.SchemeData.VariantNumber;
                schemeData.VariantDescription = currSkid.SchemeData.VariantDescription;

                tempItemProdSchemeData.ProdSchemeData = schemeData;
                this.ActDetail.SchemeData = schemeData;

                isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActDetail.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, true);

                await this.InsertActivityDetail(this.ActDetail, parse, isInsertReady);
                await this.ActDetail.SelectActivityDetail(this.dataGridMainActivity.SelectedItem);

                DisplaySummaryData();
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

        #endregion

        private async void dataGridMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.SelectActivityDetail(e.SelectedItem);
            this.DisplaySummaryData();
        }

        private void ToggleScannerMode()
        {
            // WO 14031 disabled until com server is updated.
            // bug in "REQUEST SKID DETAIL"
            // Note for David: do we want to see transactions on the skid detail?
            //ScannerModeAdd = !ScannerModeAdd;
        }

        private void scannerModeText_Clicked(object sender, EventArgs e)
        {
            ToggleScannerMode();
        }
    }
}