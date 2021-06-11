using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.View.CustomPages;
using SQLite;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using Android.Views;
using System.Collections.ObjectModel;

namespace dMobile.View.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucSubmitBatch : DMobileContentPage, IAppArea
    {
        #region Fields

        private bool isControlLoaded = false;
        private SysEnvironment sysEnv;
        private bool isChangesMade = false;
        private SubmitBatchSettings submitBatchSettings;
        private Controller.Activity activity = null;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        #endregion

        #region Constructors

        public ucSubmitBatch()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdSubmit.Text = "Submit Selected";
            this.cmdFilter.Text = "Filter";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.SubmitBatchSettings = new SubmitBatchSettings().LoadData();
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
                            await this.Close();
                        }
                        else
                        {
                            await this.CancelChanges();
                            await this.Close();
                        }
                    }
                }
            });

            try
            {
                this.Loading = true;
                await this.ucSubmitBatch_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "this.ucSubmitBatch_GotFocus threw an error " + ex, "OK"));
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
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        #endregion

        #region Properties

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

        private SubmitBatchSettings SubmitBatchSettings
        {
            get
            {
                return this.submitBatchSettings;
            }

            set
            {
                this.submitBatchSettings = value;
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
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
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
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "dMobile Submit Batch";
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
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
                    await this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(SysCommon.ActSubTypes.SubmitBatch);
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
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
            XDocument xmldocParams = System.Xml.Linq.XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task LoadData()
        {
            await Task.Run(async () =>
            {
                try
                {
                    this.Activity = null;

                    switch (this.SubmitBatchSettings.eActivitySubType)
                    {
                        case SysCommon.ActSubTypes.Receiving:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Receiving);
                            break;

                        case SysCommon.ActSubTypes.Picking:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Picking);
                            break;

                        case SysCommon.ActSubTypes.Returns:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Returns);
                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TransferFrom);
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.CycleCounts);
                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.PhysicalCounts);
                            break;

                        case SysCommon.ActSubTypes.BarCodeAssociation:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
                            break;

                        case SysCommon.ActSubTypes.Adjustment:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.Adjustment);
                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketFinished);
                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketMaterial);
                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.TicketAddProduct);
                            break;

                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                            break;

                        default:
                            throw new NotSupportedException("Unhandled activity sub type.");
                    }

                    try
                    {
                        await this.Activity.Refresh(null);
                    }
                    catch (SQLiteException ex)
                    {
                        for (int curExNdx = 0; curExNdx < ex.Data.Count; curExNdx++)
                        {
                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Submit Batch", Convert.ToString("Error " + ex.Data[curExNdx].ToString() + '\r'), "OK"));
                        }
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", "Submit Batch", ex.Message, "OK"));
                    }

                    this.Activity.ListHdrs = await this.Activity.GetActivityList(true);

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.DataGridMainActivityItemsList = this.Activity.DesignGrid(ref this.dataGridMainActivityHeader, ref this.dataGridMainActivity);
                        this.dataGridMainActivity.ItemsSource = this.DataGridMainActivityItemsList;

                        if (Activity.ListHdrs.Count > 0 && this.DataGridMainActivityItemsList.Count > 0)
                        {
                            this.dataGridMainActivity.SelectedItem = this.DataGridMainActivityItemsList[0];
                            await Activity.SelectActivity(this.DataGridMainActivityItemsList[0]);
                        }
                    });

                    this.IsControlLoaded = true;
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Error", ex.Message, "OK"));
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdFilter_Click(object sender, EventArgs e)
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSubmitBatchFilter, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = await form.GetXML();
                this.SubmitBatchSettings = this.SubmitBatchSettings.LoadData();
                this.ItemsList.Clear();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSubmit_Click(object sender, EventArgs e)
        {
            int actRefno = 0;
            int actItemRefno = 0;
            ActItem actItem = null;
            ActDetail actDetail = null;
            LocationMap locMap = LocationMap.GetInstance();

            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    if (this.Activity.ListHdrs.Count > 0)
                    {
                        for (int actRow = 0; actRow <= this.Activity.ListHdrs.Count - 1; actRow++)
                        {
                            actRefno = this.Activity.GetRefnoFromRowIdx(this.Activity.ListHdrs[actRow]);

                            if (actItem == null)
                            {
                                switch (this.SubmitBatchSettings.eActivitySubType)
                                {
                                    case SysCommon.ActSubTypes.Receiving:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Receiving);
                                        break;

                                    case SysCommon.ActSubTypes.Picking:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Picking);
                                        break;

                                    case SysCommon.ActSubTypes.Returns:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Returns);
                                        break;

                                    case SysCommon.ActSubTypes.TransferFrom:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TransferFrom);
                                        break;

                                    case SysCommon.ActSubTypes.CycleCounts:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.CycleCounts);
                                        break;

                                    case SysCommon.ActSubTypes.PhysicalCounts:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.PhysicalCounts);
                                        break;

                                    case SysCommon.ActSubTypes.BarCodeAssociation:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.BarCodeAssociation);
                                        break;

                                    case SysCommon.ActSubTypes.Adjustment:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.Adjustment);
                                        break;

                                    case SysCommon.ActSubTypes.TicketFinished:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketFinished);
                                        break;

                                    case SysCommon.ActSubTypes.TicketMaterial:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketMaterial);
                                        break;

                                    case SysCommon.ActSubTypes.TicketAddProduct:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.TicketAddProduct);
                                        break;

                                    case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                                        actItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                        break;

                                    default:
                                        throw new NotSupportedException("Unhandled activity sub type.");
                                }
                            }

                            actItem.ListItems = await actItem.GetActivityItemList(true);

                            if (actItem.ListItems.Count > 0)
                            {
                                for (int itemIdx = 0; itemIdx <= actItem.ListItems.Count - 1; itemIdx++)
                                {
                                    await actItem.SelectActivityItem(actItem.ListItems[itemIdx]);
                                    actItemRefno = actItem.Refno;

                                    if (actDetail == null && this.SubmitBatchSettings.eActivitySubType != SysCommon.ActSubTypes.BarCodeAssociation)
                                    {
                                        switch (this.SubmitBatchSettings.eActivitySubType)
                                        {
                                            case SysCommon.ActSubTypes.Receiving:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Receiving);
                                                break;

                                            case SysCommon.ActSubTypes.Picking:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Picking);
                                                break;

                                            case SysCommon.ActSubTypes.Returns:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Returns);
                                                break;

                                            case SysCommon.ActSubTypes.TransferFrom:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TransferFrom);
                                                break;

                                            case SysCommon.ActSubTypes.CycleCounts:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.CycleCounts);
                                                break;

                                            case SysCommon.ActSubTypes.PhysicalCounts:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.PhysicalCounts);
                                                break;

                                            case SysCommon.ActSubTypes.Adjustment:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.Adjustment);
                                                break;

                                            case SysCommon.ActSubTypes.TicketFinished:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketFinished);
                                                break;

                                            case SysCommon.ActSubTypes.TicketMaterial:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketMaterial);
                                                break;

                                            case SysCommon.ActSubTypes.TicketAddProduct:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.TicketAddProduct);
                                                break;

                                            case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                                                actDetail = ActDetailFacade.GetActDetail(actItem, SysCommon.ActSubTypes.PurchaseOrderWorkSheet);
                                                break;

                                            default:
                                                throw new NotSupportedException("Unhandled activity sub type.");
                                        }
                                    }

                                    if (this.SubmitBatchSettings.eActivitySubType == SysCommon.ActSubTypes.BarCodeAssociation)
                                    {
                                        SysCommon.BarCodeSymbologies i = (SysCommon.BarCodeSymbologies)Convert.ToInt32((actItem.ListItems[itemIdx] as dynamic).ProductUnitBarCodeSymbologyEnum);
                                        string symbologyPrefix = SysCommon.GetSymbologyPrefixFromSymbologyEnum(i);

                                        XDocument xmlParams = XDocument.Parse(
                                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                                + "<params>"
                                                + "<ProductUnitBarCodeRefno>" + (actItem.ListItems[itemIdx] as dynamic).ProductUnitBarCodeRefno
                                                + "</ProductUnitBarCodeRefno>"
                                                + "<ProductUnitRefno>" + (actItem.ListItems[itemIdx] as dynamic).ProductUnitRefno
                                                + "</ProductUnitRefno>"
                                                + "<ProductUnitBarCodeSymbologyPrefix>" + symbologyPrefix
                                                + "</ProductUnitBarCodeSymbologyPrefix>"
                                                + "<ProductUnitBarCodeCode>" + (actItem.ListItems[itemIdx] as dynamic).ProductUnitBarCodeCode
                                                + "</ProductUnitBarCodeCode>"
                                                + "<ProductUnitIsBC>" + (actItem.ListItems[itemIdx] as dynamic).ProductUnitIsBC
                                                + "</ProductUnitIsBC>"
                                                + "<ProductCode>" + (actItem.ListItems[itemIdx] as dynamic).ProductCode + "</ProductCode>"
                                                + "<UnitCode>" + (actItem.ListItems[itemIdx] as dynamic).UnitCode + "</UnitCode>"
                                                + "<ProductUnitBarCodeModifier1>"
                                                + (actItem.ListItems[itemIdx] as dynamic).ProductUnitBarCodeModifier1
                                                + "</ProductUnitBarCodeModifier1>"
                                                + "</params>");

                                        await actItem.Submit(xmlParams, actItem.ListItems[itemIdx]);
                                    }
                                    else if (this.SubmitBatchSettings.eActivitySubType == SysCommon.ActSubTypes.PurchaseOrderWorkSheet)
                                    {
                                        XDocument xmlParams = XDocument.Parse(
                                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                                + "<params>"
                                                + "<ProductUnitRefno>" + (actItem.ListItems[itemIdx] as dynamic).ProductUnitRefno
                                                + "</ProductUnitRefno>"
                                                + "<ProductCode>" + (actItem.ListItems[itemIdx] as dynamic).ProductCode + "</ProductCode>"
                                                + "<UnitCode>" + (actItem.ListItems[itemIdx] as dynamic).UnitCode + "</UnitCode>"
                                                + "<ProductUnitBarCodeCode></ProductUnitBarCodeCode>"
                                                + "<ProductUnitIsBC>" + false + "</ProductUnitIsBC>"
                                                + "<ProductUnitBarCodeSymbologyPrefix></ProductUnitBarCodeSymbologyPrefix>"
                                                + "<alt_qty>"
                                                + (actItem.ListItems[itemIdx] as dynamic).ActivityItemPurchaseOrderWorkSheetQuantityOrdered
                                                + "</alt_qty>"
                                                + "<base_qty>"
                                                + (actItem.ListItems[itemIdx] as dynamic).ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered
                                                + "</base_qty>"
                                                + "<wt_qty>"
                                                + (actItem.ListItems[itemIdx] as dynamic).ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered
                                                + "</wt_qty>"
                                                + "<whs_id>" + (actItem.ListItems[itemIdx] as dynamic).LocationCode + "</whs_id>"
                                                + "<refno>" + (actItem.ListItems[itemIdx] as dynamic).ActivityItemPurchaseOrderWorkSheetFKRefno
                                                + "</refno>"
                                                + "<itm_refno>" + (actItem.ListItems[itemIdx] as dynamic).ActivityItemPurchaseOrderWorkSheetFKRefno
                                                + "</itm_refno>"
                                                + "<alt_unit>" + (actItem.ListItems[itemIdx] as dynamic).UnitCode + "</alt_unit>"
                                                + "<base_unit>" + actItem.ProdInfo.BaseUnitCode + "</base_unit>"
                                                + "<wt_unit>" + actItem.ProdInfo.WeightUnitCode + "</wt_unit>"
                                                + "</params>");

                                        await actItem.Submit(xmlParams, actItem.ListItems[itemIdx]);

                                        break;
                                    }
                                    else
                                    {
                                        actDetail.ListDetails = await actDetail.GetActivityDetailList(true);

                                        if (actDetail.ListDetails.Count > 0)
                                        {
                                            for (int detailIdx = 0; detailIdx <= actDetail.ListDetails.Count - 1; detailIdx++)
                                            {
                                                await actDetail.GetInfo(actDetail.ListDetails[detailIdx], false);
                                                await actDetail.Submit(actDetail.ListDetails[detailIdx], null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Submit Batch", "Submission Complete.", "OK"));
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(() => ex.Show(SysCommon.ActSubTypes.SubmitBatch));
                }
                catch (Exception ex)
                {
                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Submit Batch", ex.Message, "OK"));
                }
                finally
                {
                    //this.pbarStatus.Maximum = 100;
                    //this.pbarStatus.Visible = false;
                    await this.LoadData();

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.ItemsList.Clear();
                        this.cmdSubmit.IsEnabled = true;
                    });

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
        private async Task ucSubmitBatch_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                await this.LoadData();
            }
        }

        #endregion
    }
}