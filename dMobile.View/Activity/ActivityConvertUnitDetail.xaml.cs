// <copyright file="ActivityConvertUnitDetail.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages.Modal;
    using Forms;
    using Helpers;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Activity detail control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertUnitDetail : DMobileModal, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityConvertUnitDetail" /> class.
        /// </summary>
        /// <param name="actItem"></param>
        /// <param name="actSubTypes"></param>
        public ActivityConvertUnitDetail(ActItem actItem, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.MainActivityItems = new ObservableCollection<object>();
            this.ExtraInfoItems = new ObservableCollection<string>();
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Title = this.ActDetail.GetControlDescription();

            actItem.ActHdr.SkidInfo ??= new SysSkidInfo();

            this.ActSubType = actSubTypes;
            this.ActDetail = ActDetailFacade.GetActDetail(actItem, actSubTypes);
            this.ActDetail.ActItem = actItem;
            this.ScannerEnable().FireAndForgetSafeAsync();
        }

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; }

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; }

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets or sets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether to show a data valid message or raise an error.
        /// </summary>
        private bool IsShowDataValidMsg { get; set; } = true;

        /// <summary>
        ///     Gets or sets the selected activity detail foreign key reference number.
        /// </summary>
        private int SelectedActivityDetailFkRefno { get; set; }

        /// <summary>
        ///     Gets or sets the ExtraInfoItems
        ///     Gets or sets a list of extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; }

        /// <summary>
        ///     Gets or sets the MainActivityItems
        ///     Gets or sets a list of data grid activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; }

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            this.Scanner.ClearBarCodeScanEventHandlers();
            await this.Scanner.DisableScanning();
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            await this.Scanner.EnableScanning();
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            }
        }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = true;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (await this.IsReadyToSave())
                {
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException(
                            "Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///     Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

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
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity detail return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity detail return information.</returns>
        public async Task<XDocument> GetXml()
        {
            var fillQty = await this.ActDetail.GetTotalQuantityFromListDetail();

            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<qty>" + fillQty.Quantity + "</qty>"
                + "<base_qty>" + fillQty.BaseQuantity + "</base_qty>"
                + "<wt_qty>" + fillQty.WeightQuantity + "</wt_qty>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///     Data layer controller event handler. Used to handle status information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher" />.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs" />.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        ///     Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher" />.</param>
        /// <param name="dataLayerEventArgs">The dataLayerEventArgs<see cref="DataLayerEventArgs" />.</param>
        public void OnLayerProgressOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
        }

        /// <summary>
        ///     Get the control description.
        /// </summary>
        /// <returns><see cref="string" /> Control description.</returns>
        private string GetControlDescription()
        {
            return "Activity Detail";
        }

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    // Set the dialog result to OK to force the line item total to recalculate.
                    // Even though the back button is meant to be a cancel we need to update
                    // because data still could have changed.
                    this.DialogResult = DialogResult.OK;
                    _ = base.OnBackButtonPressed();
                    await this.ActDisappearing();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    // Note: Do we want to disable the scanner here. It may not even be enabled before at all.
                    // It is my understanding that the OnAppearing event only occurs once when the control is loading.
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    // NOTE: Not sure if this is needed at this anymore.
                    this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor =
                        await ProductInfoTasks.GetUnitRelationFactor(
                            this.ActDetail.ActItem.ProdInfo);

                    this.ActDetail.RegLayerListenerProgOut(this);
                    await this.RefreshBySubType();
                    this.ActDetail.UnregLayerListenerProgOut(this);
                    await this.DesignGrid();
                    this.CmdAddDetail.IsEnabled = true;

                    await this.ActAppearing();

                    // No need to invoke DisplayData() selected item will do so, unless the list is empty.
                    if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                    {
                        await this.DisplayDataItem();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = "This activity unexpectedly failed to initialize correctly." +
                                  " Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                    ////await Task.Run(() =>
                    ////{
                    ////    // Explicit GC, Xamarin forms not disposing of objects properly
                    ////    GC.Collect();
                    ////    GC.WaitForPendingFinalizers();
                    ////});
                }
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ActDisappearing();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Activity appearing.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task ActAppearing()
        {
            await this.ScannerEnable();
            SubscribeKeyUpHandlers();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

            await this.FindSelectedItemAsync();
            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async keyPressed =>
                    {
                        if (this.IsIndicatorRunning)
                        {
                            return;
                        }

                        // TODO: Verify key codes.
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Del || keycode == Keycode.Clear)
                        {
                            await OnAppearingDelKey();
                        }
                        else if (keycode == Keycode.F1)
                        {
                            await OnAppearingF1Key();
                        }
                        else if (keycode == Keycode.F3)
                        {
                            await OnAppearingF3Key();
                        }
                        else if (keycode == Keycode.DpadDown)
                        {
                            OnAppearingDpadDownKey();
                        }
                        else if (keycode == Keycode.DpadUp)
                        {
                            OnAppearingDpadUp();
                        }
                    });
                return;

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingDelKey()
                {
                    try
                    {
                        // Check if there is an item in the data grid selected to delete.
                        IsItemSelected();

                        // Ask if the deletion is a entire skid or item.
                        var isDelItem = false;
                        if (await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                "Do you wish to delete this single detail?",
                                "Yes",
                                "No"))
                        {
                            isDelItem = true;
                        }

                        // Invoke the deletion.
                        if (isDelItem)
                        {
                            await OnAppearingDelKeyDetail();
                        }

                        await this.FindSelectedItemAsync();
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                msg,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }

                    return;

                    //////////////////////////////////////////////////////////////////////////
                    void IsItemSelected()
                    {
                        var isItemSelected = this.ListViewMainActivity.SelectedItem != null
                                             && Convert.ToInt32(this.MainActivityItems.IndexOf(
                                                 this.ListViewMainActivity.SelectedItem)) >= 0;

                        if (!isItemSelected)
                        {
                            throw new MessageOnlyException("Nothing selected to delete.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task OnAppearingDelKeyDetail()
                    {
                        // Read the selected detail.
                        await this.ActDetail.AssignActivityDetail(
                            (Resources.Types.Details.ActivityDetail)this.ListViewMainActivity.SelectedItem);

                        // Submit the request, and delete it from the data table.
                        _ = await this.ActDetail.Delete();

                        // Update the activity line item quantities.
                        this.ActDetail.ActItem.AddToQuantityInActivityItem(
                            this.ActDetail.ActItem.SelectedItem,
                            this.ActDetail.Quantity * -1,
                            this.ActDetail.BaseQuantity * -1,
                            this.ActDetail.WeightQuantity * -1);

                        // Update the table item quantity.
                        await this.ActDetail.ActItem.UpdateItemTableQuantity(this.ActDetail.ActItem.SelectedItem);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF1Key()
                {
                    try
                    {
                        await this.ActDisappearing();
                        if (await this.SaveChanges())
                        {
                            this.DialogResult = DialogResult.OK;

                            // TODO: DEV-5619
                            // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                            await this.Close();
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                    finally
                    {
                        await this.ActAppearing();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF3Key()
                {
                    // TODO: DEV-5619
                    // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile
                    try
                    {
                        await this.ActDisappearing();
                        var msg = await this.ActDetail.ActItem.GetTotalQuantity();
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                "OK"));
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                msg,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                    finally
                    {
                        await this.ActAppearing();
                        _ = this.ListViewMainActivity.Focus();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadDownKey()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) <
                        this.MainActivityItems.Count - 1)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) + 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadUp()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > 0)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) - 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }
            }
        }

        /// <summary>
        ///     Activity disappearing.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
        }

        /// <summary>
        ///     Search text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        private async void TxtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                await this.ActDisappearing();
                await this.DesignGrid();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ActAppearing();
            }
        }

        /// <summary>
        ///     Data grid activity detail selected detail event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs" />.</param>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActDetail.AssignActivityDetail(e.SelectedItem);
            this.ActDetail.SelectedListIdx = e.SelectedItemIndex;
            this.SelectedActivityDetailFkRefno = this.ActDetail.DetailFkRefno;
            await this.DisplayDataItem();
        }

        /// <summary>
        ///     Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Scan event arguments.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;
                var parse = scanArgs.ParseInfo;
                await this.ActDisappearing();

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    throw new MessageOnlyException("Scanning a location bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    // Check if location is set, if defaulting location is not enabled.
                    if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                        !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                    {
                        throw new MessageOnlyException("Location must be set first!");
                    }

                    var prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);

                    var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                    // Check if the scanned product matches the select activity line item product.
                    if (prodUnitRefno != this.ActDetail.ActItem.ProdInfo.UnitRefno)
                    {
                        throw new Exception("Scanned product does not match the" +
                                            " selected line item product.");
                    }

                    // If there is a selected skid check if the scanned product matches.
                    var currSkid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                    if (currSkid.SkidFkRefno != 0)
                    {
                        if (currSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            throw new Exception("Scanned product does not match the selected skid product.");
                        }
                    }

                    // Get the activity item reference number from the identified location reference number
                    // and product reference number.
                    var tempActItemRefno = await this.ActDetail.ActItem.GetActivityItemRefno(
                        prodUnitRefno, LocationMap.GetInstance().CurrLocMap);
                    if (tempActItemRefno == 0)
                    {
                        // Location and product was not found on this activity
                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            Convert.ToString(this.ActDetail.ActItem.GetItemNotFoundMessage(
                                this.LocMap,
                                prodInfo)),
                            "OK"));
                    }
                    else
                    {
                        // A skid may contain 'default' scheme data, fill the activity detail
                        // with the default skid schema.
                        var tempItemProdSchemeData = new ActItemSchemeData();
                        var schemeData = this.ActDetail.SchemeData;
                        if (currSkid.SkidFkRefno != 0)
                        {
                            schemeData.LogisticUnitSkidNumber = currSkid.SkidCode;
                            schemeData.LogisticUnitBatchLotNumber = currSkid.SchemeData.LogisticUnitBatchLotNumber;
                            schemeData.LogisticUnitBestBeforeSellByDate =
                                currSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                            schemeData.LogisticUnitProductionDate = currSkid.SchemeData.LogisticUnitProductionDate;
                            schemeData.VariantCode = currSkid.SchemeData.VariantCode;
                            schemeData.VariantNumber = currSkid.SchemeData.VariantNumber;
                            schemeData.VariantDescription = currSkid.SchemeData.VariantDescription;
                        }

                        tempItemProdSchemeData.ProdSchemeData = schemeData;
                        this.ActDetail.SchemeData = schemeData;

                        // Gather data is the meat and potatoes of collecting and assembling
                        // the product scheme data.
                        var isInsertReady = await SysShared.GatherProductData(
                            parse,
                            this.ActSubType,
                            this.ActDetail.ActItem,
                            tempItemProdSchemeData,
                            this.ActDetail);
                        if (isInsertReady)
                        {
                            await this.InsertActivityDetail(this.ActDetail);
                        }

                        await this.FindSelectedItemAsync();
                        ////if (this.MainActivityItems.Count > currRowIdx)
                        ////{
                        ////    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[currRowIdx];
                        ////}
                    }
                }
                else if (!string.IsNullOrEmpty(parse.ActFkRefno))
                {
                    throw new MessageOnlyException("Scanning a header reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning an item reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.ActDetailFkRefno))
                {
                    throw new MessageOnlyException("Scanning a detail reference # is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                }
                else
                {
                    throw new MessageOnlyException("Scanned bar code type is not supported here.");
                }
            }
            catch (TaskCanceledException)
            {
                if (scanArgs != null)
                {
                    await this.FindSelectedItemAsync();
                }
            }
            catch (IsReadyToApplyException)
            {
                await this.FindSelectedItemAsync();
            }
            catch (GatherProductDataException ex)
            {
                await this.FindSelectedItemAsync();

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await this.FindSelectedItemAsync();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.IsIndicatorRunning = false;
                await this.ActAppearing();
            }
        }

        /// <summary>
        ///     Call refresh data, unique to the activity sub type.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task RefreshBySubType()
        {
            try
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ActDetail.UnregLayerListenerProgOut(this);
                    await this.ActDetail.Refresh();
                    await Device.InvokeOnMainThreadAsync(() => this.ActDetail.ListDetails.Clear());
                    this.ActDetail.ListDetails = await this.ActDetail.GetActivityDetailList(false);
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(
                        this.ActSubType,
                        ex,
                        this.ActDetail.GetControlDescription(),
                        "Refresh");
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ActDetail.UnregLayerListenerProgOut(this);
                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }
        }

        /// <summary>
        ///     Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActDetail.ListDetails.Count > 0)
            {
                // I am thinking this iteration will be fast even with 100+ records.
                // We are not triggering any UI refreshing, so it should be.
                this.ActDetail.SelectedListIdx = -1;
                object selectedItem = null;
                if (this.SelectedActivityDetailFkRefno != 0)
                {
                    foreach (var item in this.ActDetail.ListDetails)
                    {
                        if (this.SelectedActivityDetailFkRefno == this.ActDetail.GetFkRefno(item))
                        {
                            selectedItem = item;
                            this.ActDetail.SelectedListIdx++;
                            break;
                        }
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActDetail.SelectedListIdx = 0;
                    selectedItem = this.ActDetail.ListDetails[this.ActDetail.SelectedListIdx];
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFkRefno = this.ActDetail.DetailFkRefno;
                }
                else
                {
                    await this.ActDetail.AssignActivityDetail(selectedItem);
                    this.SelectedActivityDetailFkRefno = this.ActDetail.DetailFkRefno;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = selectedItem;
                    this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActDetail.SelectedListIdx = -1;
                this.ActDetail.SelectedDetail = null;
                this.ActDetail.DetailFkRefno = 0;
                this.ActDetail.DetailRefno = 0;
                this.SelectedActivityDetailFkRefno = 0;
                Device.BeginInvokeOnMainThread(() => { this.ListViewMainActivity.SelectedItem = null; });
            }
        }

        /// <summary>
        ///     Design the main activity grid / list. Also perform text search filtering.
        /// </summary>
        /// <returns>The <see cref="Task" />. Task operation.</returns>
        private async Task DesignGrid()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                // Redesign the grid / list and rebind it to the refreshed list items.
                this.MainActivityItems = this.ActDetail.DesignGrid(
                    ref this.GridMainActivity,
                    ref this.ListViewMainActivity);

                if (!string.IsNullOrEmpty(this.TxtSearchDetail.Text))
                {
                    this.MainActivityItems = this.ActDetail.Search(this.TxtSearchDetail.Text);
                }

                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            });
        }

        /// <summary>
        ///     Insert an activity detail.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            this.ActDetail.ActStatus = SysCommon.ActStatus.Inserted;
            actDetail.DetailFkRefno = 0;
            var xmlDoc = actDetail.GetXml(actDetail);
            await actDetail.Insert(xmlDoc);

            actDetail.DetailRefno = this.ActDetail.FindListDetailRefno(actDetail.DetailFkRefno);
            if (actDetail.DetailRefno < 0)
            {
                var inserted = this.ActDetail.GatherDetail();
                this.ActDetail.ListDetails.Add(inserted);
                this.SelectedActivityDetailFkRefno = inserted.ActivityDetailFKRefno;
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ListViewMainActivity.ItemTemplate = this.ActDetail.DesignGrid(
                        ref this.GridMainActivity,
                        inserted,
                        this.MainActivityItems);
                });
                await this.FindSelectedItemAsync();
            }
        }

        /// <summary>
        ///     Display the item list data and set other controls.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task DisplayDataItem()
        {
            await Device.InvokeOnMainThreadAsync(
                () =>
            {
                this.ExtraInfoItems.Clear();
                return Task.CompletedTask;
            });
        }

        /// <summary>
        ///     Set add detail command button click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender object event arguments.</param>
        private async void CmdAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsIndicatorRunning = true;
                if (await CheckAllowedToAddDetail())
                {
                    var locMap = LocationMap.GetInstance();
                    if (!await LocationMapTasks.IsCompleteLocMap(locMap.CurrLocMapRefno))
                    {
                        throw new MessageOnlyException("Location must be set first.");
                    }

                    if (locMap.CurrLocMap.WhsLocMapRefno != this.ActDetail.ActItem.LocMap.WhsLocMapRefno)
                    {
                        throw new MessageOnlyException("Detail warehouse must match the line item warehouse.");
                    }

                    await PerformAddDetail();
                }
                else
                {
                    throw new MessageOnlyException("Permission denied to add detail.");
                }
            }
            catch (GatherProductDataException gatherEx)
            {
                if (!gatherEx.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, gatherEx);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error", ex.Message, "OK"));
            }
            finally
            {
                await this.DisplayDataItem();
                this.IsIndicatorRunning = false;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            ActItemSchemeData InitDetailBeforeGather()
            {
                this.ActDetail.Quantity = 0;
                this.ActDetail.BaseQuantity = 0;
                this.ActDetail.WeightQuantity = 0;
                this.ActDetail.SchemeId = string.Empty;
                this.ActDetail.UniqueSchemeId = string.Empty;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of
                // the length of the code and the fact is isn't human readable friendly.
                var tempItemProdSchemeData = new ActItemSchemeData();
                var schemeData = this.ActDetail.SchemeData;
                var skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (skidInfo.SkidFkRefno == 0)
                {
                    schemeData.LogisticUnitSkidNumber = string.Empty;
                    schemeData.LogisticUnitBatchLotNumber = string.Empty;
                    schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                    schemeData.LogisticUnitProductionDate = DateTime.Now;
                    schemeData.VariantCode = string.Empty;
                    schemeData.VariantNumber = 0;
                    schemeData.VariantDescription = string.Empty;
                }
                else
                {
                    schemeData.LogisticUnitSkidNumber = skidInfo.SkidCode;
                    schemeData.LogisticUnitBatchLotNumber = skidInfo.SchemeData.LogisticUnitBatchLotNumber;
                    schemeData.LogisticUnitBestBeforeSellByDate = skidInfo.SchemeData.LogisticUnitBestBeforeSellByDate;
                    schemeData.LogisticUnitProductionDate = skidInfo.SchemeData.LogisticUnitProductionDate;
                    schemeData.VariantCode = skidInfo.SchemeData.VariantCode;
                    schemeData.VariantNumber = skidInfo.SchemeData.VariantNumber;
                    schemeData.VariantDescription = skidInfo.SchemeData.VariantDescription;
                }

                tempItemProdSchemeData.ProdSchemeData = schemeData;
                this.ActDetail.SchemeData = schemeData;
                return tempItemProdSchemeData;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> CheckAllowedToAddDetail()
            {
                var isAllowedAddManualDetail = true;
                if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                    (this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                     this.ActSubType == SysCommon.ActSubTypes.Picking))
                {
                    var form = new FormAddManualDetailOverride();

                    isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(async () =>
                        await form.ShowDialog() == DialogResult.OK);
                }

                return isAllowedAddManualDetail;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task PerformAddDetail()
            {
                // Not allowed to count individual boxes on a skid. Skids must be
                // scanned as a whole unit or built in skid maintenance.

                // DEV-827 This change to allow counting partial skids is a bad idea.
                ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                ////{
                ////    if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                ////    {
                ////        this.ActDetail.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                ////    }
                ////}

                var parse = BarCodeParseFacade.ParseData(string.Empty);

                try
                {
                    await this.ActDisappearing();
                    var tempItemProdSchemeData = InitDetailBeforeGather();
                    var isInsertReady = await SysShared.GatherProductData(
                        parse,
                        this.ActSubType,
                        this.ActDetail.ActItem,
                        tempItemProdSchemeData,
                        this.ActDetail);

                    if (isInsertReady)
                    {
                        await this.InsertActivityDetail(this.ActDetail);
                    }
                }
                finally
                {
                    await this.ActAppearing();
                }
            }
        }
    }
}