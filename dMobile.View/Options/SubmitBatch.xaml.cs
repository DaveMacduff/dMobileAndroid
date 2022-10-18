// <copyright file="SubmitBatch.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;
using dMobile.Controller.Activity_Detail;
using dMobile.Controller.Activity_Item;

namespace dMobile.View.Options
{
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
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmitBatch : DMobileContentPage, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitBatch"/> class.
        /// </summary>
        public SubmitBatch()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.cmdSubmit.Text = "Submit Selected";
            this.cmdFilter.Text = "Filter";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.SubmitBatchSettings = new SubmitBatchSettings().LoadData();
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

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
                            _ = await this.CancelChanges();
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
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "this.ucSubmitBatch_GotFocus threw an error " + ex, "OK"));
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing.
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
                _ = base.OnBackButtonPressed();

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

        private Controller.Activity.Activity Activity { get; set; } = null;

        private bool IsChangesMade { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private SysEnvironment SysEnv { get; set; }

        private SubmitBatchSettings SubmitBatchSettings { get; set; }

        public ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

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
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
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
            var isReadyToClose = true;

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
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
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
            return "dMobile Submit Batch";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

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
                    throw new MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
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
            var cancelChanges = true;

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
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SubmitBatch, ex);
                    _ = await errRpt.ShowDialog();
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
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

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
                        for (var curExNdx = 0; curExNdx < ex.Data.Count; curExNdx++)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Submit Batch", Convert.ToString("Error " + ex.Data[curExNdx].ToString() + '\r'), "OK"));
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", "Submit Batch", ex.Message, "OK"));
                    }

                    this.Activity.ListHdrs = await this.Activity.GetActivityList(true);

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        this.MainActivityItems = this.Activity.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                        if (this.Activity.ListHdrs.Count > 0 && this.MainActivityItems.Count > 0)
                        {
                            this.ListViewMainActivity.SelectedItem = this.MainActivityItems[0];
                            await this.Activity.SelectActivity(this.MainActivityItems[0]);
                        }
                    });

                    this.IsControlLoaded = true;
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", ex.Message, "OK"));
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
            var form = new FormDialogContainer(EDialogTypes.SubmitBatchFilter, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                var xmlDoc = await form.GetXML();
                this.SubmitBatchSettings = this.SubmitBatchSettings.LoadData();
                this.ExtraInfoItems.Clear();
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
            var actRefno = 0;
            var actItemRefno = 0;
            ActItem actItem = null;
            ActDetail actDetail = null;
            var locMap = LocationMap.GetInstance();

            await Task.Run(async () =>
            {
                try
                {
                    this.Loading = true;

                    if (this.Activity.ListHdrs.Count > 0)
                    {
                        for (var actRow = 0; actRow <= this.Activity.ListHdrs.Count - 1; actRow++)
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
                                for (var itemIdx = 0; itemIdx <= actItem.ListItems.Count - 1; itemIdx++)
                                {
                                    await actItem.AssignActivityItem(actItem.ListItems[itemIdx]);
                                    actItemRefno = actItem.ItemRefno;

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
                                        var i = (BarCodeSymbologies)Convert.ToInt32((actItem.ListItems[itemIdx] as dynamic).ProductUnitBarCodeSymbologyEnum);
                                        var symbologyPrefix = clsCommon.GetSymbologyPrefixFromSymbologyEnum(i);

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
                                                + "<refno>" + (actItem.ListItems[itemIdx] as dynamic).ActivityItemFKRefno
                                                + "</refno>"
                                                + "<itm_refno>" + (actItem.ListItems[itemIdx] as dynamic).ActivityItemFKRefno
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
                                            for (var detailIdx = 0; detailIdx <= actDetail.ListDetails.Count - 1; detailIdx++)
                                            {
                                                var detail = (ActivityDetail)actDetail.ListDetails[detailIdx];
                                                await actDetail.AssignThisFromDetailAsync(detail, false);
                                                await actDetail.Submit(actDetail.ListDetails[detailIdx], null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Submit Batch", "Submission Complete.", "OK"));
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Submit Batch", ex.Message, "OK"));
                }
                finally
                {
                    // this.pbarStatus.Maximum = 100;
                    // this.pbarStatus.Visible = false;
                    await this.LoadData();

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.ExtraInfoItems.Clear();
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
            await this.LoadData();
        }
    }
}