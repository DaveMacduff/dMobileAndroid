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
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity sub types.</param>
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
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the saved bar code scanner enabled status.
        /// </summary>
        private bool IsSvdBarScannerEnabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether bar code handler is registered / enabled.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; } = SysCommon.ActSubTypes.SkidInfo;

        /// <summary>
        ///     Gets or sets the skid information.
        /// </summary>
        private SysSkidInfo SkidInfo { get; set; } = new SysSkidInfo();

        /// <summary>
        ///     Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets or sets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets or sets the activity item.
        /// </summary>
        private ActItem ActItem { get; }

        /// <summary>
        ///     Gets or sets the activity header.
        /// </summary>
        private Activity Activity { get; }

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        private LocationMap LocMap { get; } = LocationMap.GetInstance();

        /// <summary>
        ///     Gets or sets the items list.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= this.BarCodeHandler;
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += this.BarCodeHandler;
                this.IsBarCodeHandlerRegistered = true;
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
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
                isReadyToSave = isReadyToSave && await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno);
                isReadyToSave = isReadyToSave && this.SkidInfo.CurrSkid.SkidFKRefno != 0;
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
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
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }

            return isReadyToClose;
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
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

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
                    //// this.IsChangesMade = false;
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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
        public async Task<XDocument> GetXML()
        {
            XDocument xmlDoc;
            if (this.SkidInfo.CurrSkid.SkidFKRefno == 0)
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
                    + "<skid_fk_refno>" + this.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                    + "<chep>" + this.SkidInfo.CurrSkid.IsCHEP + "</chep>"
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
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
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
        ///     Set to destination location click event hander.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        public async void CmdToLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                await this.ScannerDisable();

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
            finally
            {
                if (this.IsSvdBarScannerEnabled)
                {
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///     Customize behavior immediately prior to the 'Page' becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    await this.ScannerDisable();
                    SubscribeKeyUpHandlers();

                    this.IsIndicatorRunning = true;
                    this.lstSkidStats.ItemsSource = this.ExtraInfoItems;
                    if (this.IsFirstAppearance)
                    {
                        this.IsFirstAppearance = false;
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
                        const string msg = "This activity unexpectedly failed to initialize correctly." +
                                           " Please exit and try again.";
                        var errorReport = new FormErrorReport(msg, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                    await this.ScannerEnable();
                    ////await Task.Run(() =>
                    ////{
                    ////    // Explicit GC, Xamarin forms not disposing of objects properly
                    ////    GC.Collect();
                    ////    GC.WaitForPendingFinalizers();
                    ////});
                }
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                {
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
                });
                return;

                //////////////////////////////////////////////////////////////////////////
                async Task AppearingF1Key()
                {
                    try
                    {
                        this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                        await this.ScannerDisable();

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
                        if (this.IsSvdBarScannerEnabled)
                        {
                            await this.ScannerEnable();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Clean up before the 'Page' becomes invisible.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            Device.BeginInvokeOnMainThread(async () => { await this.ScannerDisable(); });
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     Back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    // Disable the scanner even though the on disappearing event will turn it off also
                    // upon closing. CancelChanges may show dialogs.
                    this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();
                    _ = base.OnBackButtonPressed();

                    if (await this.SaveChanges())
                    {
                        _ = base.OnBackButtonPressed();
                        await this.Close();
                    }
                    else if (await this.CancelChanges())
                    {
                        _ = base.OnBackButtonPressed();
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
                    if (this.IsSvdBarScannerEnabled)
                    {
                        await this.ScannerEnable();
                    }
                }
            });

            return true;
        }

        /// <summary>
        ///     Bar code scan handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs" /> Scan event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsIndicatorRunning)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            this.IsIndicatorRunning = true;
            await this.ScannerDisable();
            var parse = scanArgs.ParseInfo;

            try
            {
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
                    // PRODUCT IDENTIFIER
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                    }

                    // ACTIVITY ITEM FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
                    }

                    // ACTIVITY DETAIL FK REFNO
                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                    }

                    // SHIPPING CONAINTER SCANNED, SKID
                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.SSCC18);

                        if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                this.SkidInfo.CurrSkid.LocMapRefno);
                        }

                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            this.DisplaySkidInfo();
                            ////if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            ////{
                            ////    this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                            ////}
                            ////else
                            ////{
                            ////    this.cmdToLocation.Text = "Set Location";
                            ////}
                        });
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Display the identified skid to move information.
        /// </summary>
        private void DisplaySkidInfo()
        {
            this.ExtraInfoItems.Clear();

            if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;

                var location = LocationMapTasks.GetReadableLocation(this.SkidInfo.CurrSkid.LocMap, true);
                this.ExtraInfoItems.Add($"Current: {location}");
                this.lblProductBCUnit.Text = $"Product: {this.SkidInfo.CurrSkid.ProdInfo.Code}" +
                                             $" {this.SkidInfo.CurrSkid.ProdInfo.UnitBC}" +
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
                this.ExtraInfoItems.Add($"CHEP: {this.SkidInfo.CurrSkid.IsCHEP}");
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
                if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
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

            await Device.InvokeOnMainThreadAsync(() =>
            {
                ////if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                ////{
                ////    this.cmdToLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                ////}
                ////else
                ////{
                ////    this.cmdToLocation.Text = "Set Location";
                ////}
            });
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