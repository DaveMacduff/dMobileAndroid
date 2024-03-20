// <copyright file="ActivityMoveSkid.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using CustomPages;
    using Forms;
    using Helpers;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Move skid activity.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityMoveSkid : DMobileContentPage, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityMoveSkid" /> class.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity subtypes.</param>
        public ActivityMoveSkid(SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.cmdToLocation.Text = "Set To Location";
            this.lblProductBCUnit.Text = "Product";
            this.lblSkidCode.Text = "Skid #";

            this.Title = this.GetControlDescription();
            this.LocMap = LocationMap.GetInstance();
            this.SkidInfo = new SysSkidInfo();
            this.ActSubType = actSubType;
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.SkidInfo);
            this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.SkidInfo);
            this.ActItem.ActHdr = this.Activity;
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.SkidInfo);
            this.ActDetail.ActItem = this.ActItem;
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.SkidCodeEnter().FireAndForgetSafeAsync();
        }

        /// <summary>
        ///     Gets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets or sets the skid information.
        /// </summary>
        private SysSkidInfo SkidInfo { get; set; }

        /// <summary>
        ///     Gets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; }

        /// <summary>
        ///     Gets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets the activity item.
        /// </summary>
        private ActItem ActItem { get; }

        /// <summary>
        ///     Gets the activity header.
        /// </summary>
        private Activity Activity { get; }

        /// <summary>
        ///     Gets the location map.
        /// </summary>
        private LocationMap LocMap { get; }

        /// <summary>
        ///     Gets the items list.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets a value indicating whether data is loading.
        /// </summary>
        private bool IsDataLoading { get; set; } = true;

        /// <summary>
        ///     Un-register the bar code scan event handler and disable the scanner.
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
                this.Scanner.BarCodeScan += this.BarCodeHandler;
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
                isReadyToSave = await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno);
                isReadyToSave = isReadyToSave && this.SkidInfo.CurrSkid.SkidFkRefno != 0;
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
            return true;
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
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
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
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
        public async Task<XDocument> GetXml()
        {
            XDocument xmlDoc;
            if (this.SkidInfo.CurrSkid.SkidFkRefno == 0)
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno></skid_fk_refno>"
                    + "<chep></chep>"
                    + "<hold></hold>"
                    + "<base_quantity></base_quantity>"
                    + "<cube></cube>"
                    + "<height></height>"
                    + "<quantity></quantity>"
                    + "<weight_quantity></weight_quantity>"
                    + "<hi></hi>"
                    + "<ti></ti>"
                    + "<cube_unit></cube_unit>"
                    + "<height_unit></height_unit>"
                    + "<logistic_unit_skid_code></logistic_unit_skid_code>"
                    + "<skid_code></skid_code>"
                    + "<location_map_refno></location_map_refno>"
                    + "<product_unit_refno></product_unit_refno>"
                    + "</params>");
            }
            else
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno>" + this.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
                    + "<chep>" + this.SkidInfo.CurrSkid.IsChep + "</chep>"
                    + "<hold>" + this.SkidInfo.CurrSkid.IsHold + "</hold>"
                    + "<base_quantity>" + this.SkidInfo.CurrSkid.BaseQuantity + "</base_quantity>"
                    + "<cube>" + this.SkidInfo.CurrSkid.Cube + "</cube>"
                    + "<height>" + this.SkidInfo.CurrSkid.Height + "</height>"
                    + "<quantity>" + this.SkidInfo.CurrSkid.Quantity + "</quantity>"
                    + "<weight_quantity>" + this.SkidInfo.CurrSkid.WeightQuantity + "</weight_quantity>"
                    + "<hi>" + this.SkidInfo.CurrSkid.Hi + "</hi>"
                    + "<ti>" + this.SkidInfo.CurrSkid.Ti + "</ti>"
                    + "<cube_unit>" + this.SkidInfo.CurrSkid.CubeUnit + "</cube_unit>"
                    + "<height_unit>" + this.SkidInfo.CurrSkid.HeightUnit + "</height_unit>"
                    + "<logistic_unit_skid_code>" + this.SkidInfo.CurrSkid.LogisticUnitSkidCode +
                    "</logistic_unit_skid_code>"
                    + "<skid_code>" + this.SkidInfo.CurrSkid.SkidCode + "</skid_code>"
                    + "<location_map_refno>" + this.SkidInfo.CurrSkid.LocMapRefno + "</location_map_refno>"
                    + "<product_unit_refno>" + this.SkidInfo.CurrSkid.ProdInfo.UnitRefno + "</product_unit_refno>"
                    + "</params>");
            }

            return xmlDoc;
        }

        /// <summary>
        ///     Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SkidInfo = await SysSkidInfo.Move(this.SkidInfo, this.LocMap.CurrLocMap);

                    if (await Device.InvokeOnMainThreadAsync(async () => !await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Do you wish to move any more skid(s)?",
                            "Yes",
                            "No")))
                    {
                        saveComplete = true;
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///     Get the activity description.
        /// </summary>
        /// <returns><see cref="string" /> Activity description.</returns>
        public string GetControlDescription()
        {
            return "Move Skid";
        }

        /// <summary>
        ///     Set to destination location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        public async void CmdToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new FormDialogContainer(
                    EDialogTypes.SelectLocationMap,
                    SysCommon.ActSubTypes.NotApplicable);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    if (this.LocMap.ChosenLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdToLocation.Text = LocationMapTasks.GetChosenReadableLocation();
                    }
                    else
                    {
                        this.cmdToLocation.Text = "Set Location";
                    }
                }
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
                    var msg = "This activity unexpectedly failed to initialize correctly." +
                              " Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();
            this.lstSkidStats.ItemsSource = this.ExtraInfoItems;
            this.txtSkidCode.Focus();
            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandler);
                return;

                //////////////////////////////////////////////////////////////////////////
                async void KeyUpHandler(string keyPressed)
                {
                    if (this.IsIndicatorRunning)
                    {
                        return;
                    }

                    try
                    {
                        await this.ScannerDisable();
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);
                        switch (keycode)
                        {
                            case Keycode.F1:
                                await AppearingF1Key();
                                break;

                            case Keycode.Enter:
                                await this.SkidCodeEnter();
                                break;
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
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
                        await this.ScannerEnable();
                    }

                    return;

                    //////////////////////////////////////////////////////////////////////////
                    async Task AppearingF1Key()
                    {
                        try
                        {
                            if (await this.SaveChanges())
                            {
                                await this.ScannerDisable();
                                await this.Close();
                            }
                            else
                            {
                                _ = await this.CancelChanges();
                                await this.ScannerDisable();
                                await this.Close();
                            }
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
                    }
                }
            }
        }

        /// <summary>
        ///     Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonHandler);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void BackButtonHandler()
            {
                try
                {
                    _ = base.OnBackButtonPressed();

                    if (await this.SaveChanges())
                    {
                        await this.ScannerDisable();
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                    else if (await this.CancelChanges())
                    {
                        await this.ScannerDisable();
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
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
            }
        }

        /// <summary>
        ///     Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Scan event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                var parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier);

                    if (locMapRefno > 0)
                    {
                        this.LocMap.ChosenLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                    }

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        if (this.LocMap.ChosenLocMap.WhsLocMapRefno != 0)
                        {
                            this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                        }
                        else
                        {
                            this.cmdToLocation.Text = "Set Location";
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                    }

                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
                    }

                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                    }

                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.SSCC18);

                        if (this.SkidInfo.CurrSkid.SkidFkRefno != 0)
                        {
                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                this.SkidInfo.CurrSkid.LocMapRefno);
                        }

                        await Device.InvokeOnMainThreadAsync(this.DisplaySkidInfo);
                    }
                }
            }
            catch (TaskCanceledException)
            {
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
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     Display the identified skid to move information.
        /// </summary>
        private void DisplaySkidInfo()
        {
            this.ExtraInfoItems.Clear();

            if (this.SkidInfo.CurrSkid.SkidFkRefno != 0)
            {
                this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;

                var location = LocationMapTasks.GetReadableLocation(this.SkidInfo.CurrSkid.LocMap, true);
                this.ExtraInfoItems.Add($"Current: {location}");
                this.lblProductBCUnit.Text = $"Product: {this.SkidInfo.CurrSkid.ProdInfo.Code}" +
                                             $" {this.SkidInfo.CurrSkid.ProdInfo.UnitBc}" +
                                             $" {this.SkidInfo.CurrSkid.ProdInfo.UnitCode}";
                this.ExtraInfoItems.Add($"Bar Code: {this.SkidInfo.CurrSkid.LogisticUnitSkidCode}");

                var qty = $"Qty: {Math.Round(this.SkidInfo.CurrSkid.Quantity, 3)}" +
                          $" {this.SkidInfo.CurrSkid.ProdInfo.UnitCode}";
                this.ExtraInfoItems.Add(qty);

                this.ExtraInfoItems.Add($"Weight: {Math.Round(this.SkidInfo.CurrSkid.WeightQuantity, 3)}" +
                                        $" {this.SkidInfo.CurrSkid.ProdInfo.WeightUnitCode}");
                this.ExtraInfoItems.Add($"Ti: {this.SkidInfo.CurrSkid.Ti} Hi: {this.SkidInfo.CurrSkid.Hi}");
                this.ExtraInfoItems.Add($"Height: {this.SkidInfo.CurrSkid.Height} {this.SkidInfo.CurrSkid.HeightUnit}");
                this.ExtraInfoItems.Add($"Cube: {this.SkidInfo.CurrSkid.Cube} {this.SkidInfo.CurrSkid.CubeUnit}");
                this.ExtraInfoItems.Add($"CHEP: {this.SkidInfo.CurrSkid.IsChep}");
                this.ExtraInfoItems.Add($"Hold: {this.SkidInfo.CurrSkid.IsHold}");
            }
            else
            {
                this.ExtraInfoItems.Add("Skid does not exist.");
            }
        }

        /// <summary>
        ///     Handle Enter press.
        /// </summary>
        private async Task SkidCodeEnter()
        {
            try
            {
                this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(this.txtSkidCode.Text);
                if (this.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        this.SkidInfo.CurrSkid.LocMapRefno);
                }

                await Device.InvokeOnMainThreadAsync(this.DisplaySkidInfo);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    this.GetControlDescription(),
                    ex.Message,
                    "OK"));
            }
        }

        /// <summary>
        ///     Skid code completed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void TxtSkidCode_Completed(object sender, EventArgs e)
        {
            await this.SkidCodeEnter();
        }
    }
}