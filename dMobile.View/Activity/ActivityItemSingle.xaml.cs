// <copyright file="ActivityItemSingle.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using dMobile.Controller.Activity;
    using Forms;
    using Resources.Enums;
    using Resources.Types.Items;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ActivityItemSingle" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItemSingle : IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItemSingle"/> class.
        /// </summary>
        /// <param name="activity">Activity header.</param>
        /// <param name="actSubType">Activity subtype.</param>
        public ActivityItemSingle(Activity activity, SysCommon.ActSubTypes actSubType)
        {
            this.InitializeComponent();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.ScannerDisable().FireAndForgetSafeAsync();

            this.IsIndicatorRunning = false;
            this.LocMap = LocationMap.GetInstance();
            this.SysEnv = SysEnvironment.GetInstance();
            actSubType = actSubType == SysCommon.ActSubTypes.PickingSingle ? SysCommon.ActSubTypes.Picking : actSubType;
            this.ActSubType = actSubType;

            // Init activity header object
            this.ActHdr = activity;
            this.ActHdr.SkidInfo ??= new SysSkidInfo();

            // Init activity item object
            this.ActItem = ActItemFacade.GetActItem(this.ActHdr, this.ActSubType);

            // Init activity detail object
            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

            InitBarCodeModifiers();
            this.Title = this.GetControlDescription();
            this.LoadAndBind().FireAndForgetSafeAsync();
            this.SearchTimer = new SysTimer(this.TimerTickCallback);
            return;

            ///////////////////////////////////////////////////////////
            void InitBarCodeModifiers()
            {
                // Init bar code modifier
                this.BarCodeModifier1Picker.Items.Clear();
                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.BarCodeModifier1Picker.Items.Add(string.Empty);
                    var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
                    foreach (var item in modifiers)
                    {
                        this.BarCodeModifier1Picker.Items.Add(item);
                    }
                }
                else
                {
                    this.BarCodeModifier1Picker.IsVisible = false;
                    this.BarCodeModifier1Picker.IsEnabled = false;
                }

                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                // Init scanner
                this.Scanner = ScannerEngine.GetInstance().Scanner;
            }
        }

        private bool IsFirstAppearance { get; set; } = true;

        private BarCodeScanner Scanner { get; set; }

        private LocationMap LocMap { get; }

        private Activity ActHdr { get; }

        private ActItem ActItem { get; }

        private ActDetail ActDetail { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; }

        private bool IsDataLoading { get; set; }

        private SysEnvironment SysEnv { get; }

        private SysTimer SearchTimer { get; }

        private List<object> ListItems { get; set; } = new List<object>();

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
            if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            {
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            }

            await this.Scanner.EnableScanning();
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public Task<bool> IsReadyToSave()
        {
            const bool isReadyToSave = true;
            return Task.FromResult(isReadyToSave);
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public Task<bool> IsReadyToClose()
        {
            const bool isReadyToClose = true;
            return Task.FromResult(isReadyToClose);
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public Task<bool> SaveChanges()
        {
            const bool isSaveComplete = true;
            return Task.FromResult(isSaveComplete);
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public Task<bool> CancelChanges()
        {
            const bool cancelChanges = true;
            return Task.FromResult(cancelChanges);
        }

        /// <summary>
        /// The GetXml.
        /// </summary>
        /// <returns>.</returns>
        public Task<XDocument> GetXml()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>");
            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.ScannerEnable();
                    this.SearchTimer.Start();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
        }

        /// <summary>
        /// On disappearing event handler.
        /// </summary>
        protected override async void OnDisappearing()
        {
            try
            {
                MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
                base.OnDisappearing();
                this.IsIndicatorRunning = false;
                this.SearchTimer.Stop();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
        }

        /// <summary>
        /// On back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();
#pragma warning disable LindhartAnalyserMissingAwaitWarning
                Task.Run(
                    async () =>
                    {
                        await this.ScannerDisable();
                        if (await this.CancelChanges())
                        {
                            this.DialogResult = DialogResult.None;
                            await this.Close();
                        }
                    });
#pragma warning restore LindhartAnalyserMissingAwaitWarning
            }
        }

        private string GetControlDescription()
        {
            var desc = this.ActSubType switch
            {
                SysCommon.ActSubTypes.PhysicalCounts => $"Count # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.CycleCounts => $"Count # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Receiving => $"Receive # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Picking => $"Pick # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Returns => $"Return # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.TransferFrom => $"Transfer from # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.TransferTo => $"Transfer to # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Adjustment => $"Adjustment # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.TicketFinished => $"Ticket Finished # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.TicketMaterial => $"Ticket Material # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.TicketAddProduct => $"Ticket Add-Product # {this.ActItem.ActHdr.HdrFkRefno}",
                _ => "Activity Items"
            };

            return desc;
        }

        private void TimerTickCallback()
        {
            if (!this.IsIndicatorRunning)
            {
                this.SearchTimer.Stop();
                this.CmdAddItem.IsEnabled = this.ActItem.IsAllowAddItem;
            }
        }

        private async Task LoadAndBind()
        {
            if (!this.IsDataLoading)
            {
                this.IsIndicatorRunning = true;
                this.IsDataLoading = true;

                try
                {
                    // If not in disconnected mode, refresh data from the communication server.
                    if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        await this.ActItem.Refresh(this.ActItem.ActHdr.XmlDataCriteria);
                    }

                    this.ListItems = await this.ActItem.GetActivityItemList(isSubmitBatch: false);
                    this.ActItem.SelectedItem = this.ListItems.FirstOrDefault();
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    this.ActItem.ItemComment = await this.ActItem.GetComment(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
                catch (CommunicationException commEx)
                {
                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var msg = $"Unable to load activity items.{Environment.NewLine}" +
                                      $"Because of a communication problem.{Environment.NewLine}" +
                                      $"Detail:{commEx.Message}{Environment.NewLine}" +
                                      "Please exit and try again.";

                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK);
                        });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var msg = $"Unable to load activity items.{Environment.NewLine}" +
                                      $"General exception.{Environment.NewLine}" +
                                      $"Detail:{ex.Message}{Environment.NewLine}" +
                                      "Please exit and try again.";
                            var errorReport = new FormErrorReport(msg, ex);
                            _ = await errorReport.ShowDialog();
                        });
                }
                finally
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        private void DisplayCmdSetSkidData()
        {
            this.CmdSetSkid.IsEnabled = true;
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
            {
                this.CmdClearSkid.IsEnabled = true;
                var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                this.CmdSetSkid.Text = "Skid# " + currSkid.SkidCode;
            }
            else
            {
                this.CmdClearSkid.IsEnabled = false;
                this.CmdSetSkid.Text = "Select Skid";
            }
        }

        private void DisplayCmdSetLocationData()
        {
            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
            {
                // If the skid is a new skid or empty, the location is enabled.
                // There is no new skid flag, so we check if the quantity is greater than 0.
                var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                this.CmdSetLocation.IsEnabled = currSkid.Quantity == 0;
            }
            else
            {
                this.CmdSetLocation.IsEnabled = true;
            }

            this.CmdSetLocation.Text = this.LocMap.CurrLocMap.WhsLocMapRefno > 0
                ? LocationMapTasks.GetCurrentReadableLocation() : "Select Location";
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string barMsg = "Scanner event fired but no data was received. Try scanning someone's" +
                                          " forehead.";
                    throw new MessageOnlyException(barMsg);
                }

                var isFkRefnoScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.ActFkRefno);
                if (isFkRefnoScanned)
                {
                    const string msg = "Scanning an activity header bar code is not supported here.";
                    throw new MessageOnlyException(msg);
                }

                var isFkDetailRefnoScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.ActDetailFkRefno);
                if (isFkDetailRefnoScanned)
                {
                    const string msg = "Scanning an activity detail bar code is not supported here.";
                    throw new MessageOnlyException(msg);
                }

                var isFkItemRefnoScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.ActItemFkRefno);
                if (isFkItemRefnoScanned)
                {
                    await BarCodeHandlerLineItemIdentifier(scanArgs.ParseInfo);
                }

                var isLocationBarCodeScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.LocationIdentifier);
                if (isLocationBarCodeScanned)
                {
                    await BarCodeHandlerLocationIdentifier(scanArgs.ParseInfo);
                }

                var isSkidScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.Sscc18);
                if (isSkidScanned)
                {
                    await FindOrCreateSkidLineItem(scanArgs.ParseInfo);
                }

                var isProductScanned = !string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier);
                if (isProductScanned)
                {
                    await FindOrCreateProductLineItem(scanArgs.ParseInfo);
                    await CreateDetail(scanArgs.ParseInfo);
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (IsReadyToApplyException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return;

            async Task FindOrCreateSkidLineItem(BarCodeParse parse)
            {
                if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                {
                    const string msg = "Scanning a shipping container bar code is not enabled.";
                    throw new MessageOnlyException(msg);
                }

                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    const string msg = "Scanning a shipping container bar code is not supported in disconnected mode.";
                    throw new MessageOnlyException(msg);
                }

                var tempSkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(parse.Sscc18);

                // The current location should supersede the skid location on counts.
                // Otherwise, the skid location will override the current location.
                // Remember the skid location may not be set because of lacking inventory or transactions.
                var tempLocMap = this.LocMap.CurrLocMap;
                if (!(this.ActItem.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                      this.ActItem.ActSubType == SysCommon.ActSubTypes.CycleCounts))
                {
                    // Only assign the skid location if it is complete. It may be incomplete because of
                    // lacking inventory or transactions.
                    if (await LocationMapTasks.IsCompleteLocMap(tempSkidInfo.CurrSkid.LocMap))
                    {
                        tempLocMap = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap;
                    }
                }

                if (!await LocationMapTasks.IsCompleteLocMap(tempLocMap))
                {
                    const string msg = "The current location must be complete.";
                    throw new MessageOnlyException(msg);
                }

                var tempProdInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo;
                if (tempProdInfo.UnitRefno == 0)
                {
                    tempProdInfo = this.ActItem.ProdInfo;
                }

                tempSkidInfo.CurrSkid.ProdInfo = tempProdInfo;
                tempSkidInfo.CurrSkid.LocMap = tempLocMap;
                this.LocMap.CurrLocMap = tempLocMap;
                this.ActHdr.SkidInfo = tempSkidInfo;

                // Does the skid product and location exist as a line item.
                var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno,
                    this.ActHdr.SkidInfo.CurrSkid.LocMap);
                if (actItemFoundRefno > 0)
                {
                    await this.AssignAndDisplayLineItem(actItemFoundRefno);
                }
                else
                {
                    var prodInfo = this.ActHdr.SkidInfo.CurrSkid.ProdInfo;
                    if (this.ActItem.IsAllowAddItem)
                    {
                        if (await this.AskToAddNewItem(prodInfo))
                        {
                            var data = ProductInfoTasks.GetXml(prodInfo);

                            // Additional found matching line items are not allowed to be created during
                            // scanning and inserting. I am not expecting matches to be found because
                            // that check has already been performed at this point.
                            // These are assigned in the Insert method.
                            // this.ActItem.ProdInfo
                            // this.ActItem.ProdInfo.Scheme
                            // this.ActItem.ItemRefno
                            // this.ActItem.ItemFkRefno
                            await this.ActItem.Insert(data);
                            await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);
                        }
                        else
                        {
                            this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                            this.DisplayCmdSetSkidData();
                            await this.DisplaySkidLocationData();
                            throw new GatherProductDataException(
                                this.ActItem.GetItemNotFoundMessage(this.LocMap, prodInfo),
                                true);
                        }
                    }
                    else
                    {
                        // Because a new line item is not allowed to be created, the skid will be cleared.
                        var msg = this.ActItem.GetItemNotFoundMessage(this.LocMap, prodInfo);
                        this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                        this.DisplayCmdSetSkidData();
                        await this.DisplaySkidLocationData();
                        throw new MessageOnlyException(msg);
                    }
                }
            }

            async Task BarCodeHandlerLineItemIdentifier(BarCodeParse parse)
            {
                object foundItem = null;
                var itemFkRefno = Convert.ToInt32(parse.ActItemFkRefno);
                foreach (var item in this.ListItems.Where(item => ((ActItem)item).ItemFkRefno == itemFkRefno))
                {
                    foundItem = item;
                    await this.AssignAndDisplayLineItem(itemFkRefno);
                    break;
                }

                if (foundItem == null)
                {
                    var msg = this.ActItem.GetUnableToFindItemFkRefnoMessage(scanArgs.ParseInfo.ActItemFkRefno);
                    throw new MessageOnlyException(msg);
                }
            }

            async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
            {
                // If section filtering was set in the activity header. Do we want to enforce
                // the scanned section location?
                var locMapRefno =
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier);
                var scannedLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                // Special validation for picking and filtering by section.
                // Check if the scanned section matched the filtered one.
                if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    // This is just a warning and the scan will be accepted.
                    await CheckIfScannedSectionMatchesFilteredOne(scannedLoc);
                }

                this.LocMap.CurrLocMap = scannedLoc;
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                await this.DisplayLocationData();
                return;

                async Task CheckIfScannedSectionMatchesFilteredOne(SLocMap scannedLocMap)
                {
                    var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                    if (sectRefno != 0 && scannedLocMap.SectLocMapRefno != sectRefno)
                    {
                        var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                        var msg = $"Warning scanned location section '{scannedLocMap.SectName}' does" +
                                  $" not match the filtered picking section '{sectName}'.";
                        await Device.InvokeOnMainThreadAsync(
                            async () => await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                Resource.OK));
                    }
                }
            }

            async Task<bool> AskToAssignProductToSkid(ProductInfo prodInfo)
            {
                var isAssignProductToSkid = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var msg = "The current selected skid does not have a product assigned to it yet." +
                                  $"Do you wish to assign '{prodInfo.Code} {prodInfo.UnitBc.Trim()}" +
                                  $" {prodInfo.UnitCode}' to the skid?";
                        if (await this.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "Yes",
                                "No"))
                        {
                            isAssignProductToSkid = true;
                        }

                        return Task.CompletedTask;
                    });

                return isAssignProductToSkid;
            }

            async Task CheckIfSkidProductMatchesScanned(int prodUnitRefno, ProductInfo scannedProdInfo)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0)
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                    {
                        const string msg = "Selected skid product does not match" +
                                           " the scanned product. Please scan the skid assigned" +
                                           " product or clear the selected skid.";
                        throw new MessageOnlyException(msg);
                    }
                }
                else
                {
                    // If the skid product is not set, then set it.
                    if (!await AskToAssignProductToSkid(scannedProdInfo))
                    {
                        const string msg = "Assigning product to a skid cancelled. Scan aborted.";
                        throw new MessageOnlyException(msg);
                    }

                    this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo = scannedProdInfo;
                    SysSkidInfo.SendUpdate(this.ActItem.ActHdr.SkidInfo);
                }
            }

            async Task FindOrCreateProductLineItem(BarCodeParse parse)
            {
                {
                    // Identify the product from the parse info.
                    var prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);
                    var scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                    // If there is a skid selected, then the product scanned must match the skid product.
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        await CheckIfSkidProductMatchesScanned(prodUnitRefno, scannedProdInfo);
                    }

                    await this.SetDefaultLocationBeforeKnownActivityItem(scannedProdInfo);

                    // Attempt to find the activity line item.
                    var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno, this.LocMap.CurrLocMap);
                    if (actItemFoundRefno > 0)
                    {
                        await this.AssignAndDisplayLineItem(actItemFoundRefno);
                    }
                    else
                    {
                        if (!this.ActItem.IsAllowAddItem)
                        {
                            throw new MessageOnlyException(
                                this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo));
                        }

                        if (!await this.AskToAddNewItem(scannedProdInfo))
                        {
                            throw new GatherProductDataException(
                                this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo),
                                true);
                        }

                        // Additional found matching line items are not allowed to be created during
                        // scanning and inserting. I am not expecting matches to be found because
                        // that check has already been performed at this point.
                        await this.ActItem.Insert(ProductInfoTasks.GetXml(scannedProdInfo));
                        await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);
                    }
                }
            }

            ProductSchemeData DefaultSchemeDataFromCurrentSkidSchemeDetail(ActDetail actDetail)
            {
                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                // because of the length of the code and the fact isn't human-readable friendly.
                var schemeData = actDetail.SchemeData;
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    var curSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                    schemeData.LogisticUnitSkidNumber = curSkid.SkidCode;
                    schemeData.LogisticUnitBatchLotNumber = curSkid.SchemeData.LogisticUnitBatchLotNumber;
                    schemeData.LogisticUnitBestBeforeSellByDate = curSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                    schemeData.LogisticUnitProductionDate = curSkid.SchemeData.LogisticUnitProductionDate;
                    schemeData.VariantCode = curSkid.SchemeData.VariantCode;
                    schemeData.VariantNumber = curSkid.SchemeData.VariantNumber;
                    schemeData.VariantDescription = curSkid.SchemeData.VariantDescription;
                }

                return schemeData;
            }

            async Task CreateDetail(BarCodeParse parse)
            {
                {
                    var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                    var schemeData = DefaultSchemeDataFromCurrentSkidSchemeDetail(actDetail);
                    actDetail.SchemeData = schemeData;
                    var tempItemProdSchemeData = new ActItemSchemeData
                    {
                        ProdSchemeData = schemeData
                    };

                    var isInsertReady = await SysShared.GatherProductData(
                        parse,
                        this.ActSubType,
                        this.ActItem,
                        tempItemProdSchemeData,
                        actDetail);
                    if (isInsertReady)
                    {
                        var xmlDoc = actDetail.GetXml(actDetail);
                        await actDetail.Insert(xmlDoc);
                        this.ActItem.AddToQuantityInActivityItem(
                            this.ActItem.SelectedItem,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);
                        await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);
                        await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);
                    }
                }
            }
        }

        private async Task<bool> AskToAddNewItem(ProductInfo prodInfo)
        {
            var isAddNewItem = false;
            if (this.ActItem.IsAskCreateNewItem)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var msg = "An item does not exist for this warehouse " +
                                  $"{this.LocMap.CurrLocMap.WhsCode} and product " +
                                  $"'{ProductInfoTasks.GetCurrReadableProdBcUnit(prodInfo)}'." +
                                  Environment.NewLine +
                                  "Do you wish to create one?";
                        if (await this.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                msg,
                                "Yes",
                                "No"))
                        {
                            isAddNewItem = true;
                        }

                        return Task.CompletedTask;
                    });
            }
            else
            {
                isAddNewItem = true;
            }

            return isAddNewItem;
        }

        private async Task DisplayLocationData()
        {
            var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                this.ActItem.LocMapRefno,
                this.ActItem.ProdInfo.Refno);
            var selectLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
            this.LblDefaultLocationData.Text =
                LocationMapTasks.GetReadableLocation(selectLocMap, isSepSlotCoords: false);
        }

        private async Task DisplaySkidLocationData()
        {
            var locMapRefno = this.ActHdr.SkidInfo.CurrSkid.LocMapRefno;
            if (locMapRefno == 0)
            {
                this.LblSkidLocationData.Text = "Unknown";
            }
            else
            {
                var skidLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                this.LblSkidLocationData.Text = LocationMapTasks.GetReadableLocation(
                    skidLocMap,
                    isSepSlotCoords: false);
            }
        }

        private async Task DisplayData()
        {
            await Device.InvokeOnMainThreadAsync(
                async () =>
                {
                    this.DisplayCmdSetLocationData();
                    await this.DisplayLocationData();
                    await this.DisplaySkidLocationData();
                    this.DisplayCmdSetSkidData();
                    DisplayProductData();
                    DisplayQuantityData();
                    DisplayItemIndex();
                    this.LblCommentData.Text = this.ActItem.ItemComment;
                });
            return;

            void DisplayQuantityData()
            {
                this.LblOrderedExpectedOnHand.Text = this.ActItem.GetOrderedExpectedOnHandTitle();
                this.LblOrderedExpectedOnHandData.Text = this.ActItem.GetOrderedExpectedOnHandQuantity();
                this.LblShippedReceivedCounted.Text = this.ActItem.GetShippedReceivedCountedTitle();
                if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    if (this.ActItem.SelectedItem != null &&
                        ((PickingItem)this.ActItem.SelectedItem).ActivityItemPickingQuantityOrdered ==
                        ((PickingItem)this.ActItem.SelectedItem).ActivityItemPickingQuantityShipped)
                    {
                        this.LblShippedReceivedCountedData.TextColor = Color.Green;
                    }
                    else
                    {
                        this.LblShippedReceivedCountedData.TextColor = Color.Red;
                    }
                }

                this.LblShippedReceivedCountedData.Text = this.ActItem.GetShippedReceivedCountedQuantity();
            }

            void DisplayItemIndex()
            {
                this.LblItemCountData.Text = this.ActItem.ListItems.Count > 0
                    ? $"{this.ActItem.SelectedListIdx + 1} of {this.ActItem.ListItems.Count}" : "0 of 0";
            }

            void DisplayProductData()
            {
                if (this.ActItem.SelectedItem == null || this.ActItem.ProdInfo == null)
                {
                    this.LblProductBcUnitData.Text = "Unknown";
                    this.LblDescriptionSizeData.Text = "Unknown";
                    this.LblBrandData.Text = "Unknown";
                }
                else
                {
                    var prodBcUn = ProductInfoTasks.GetCurrReadableProdBcUnit(this.ActItem.ProdInfo);
                    this.LblProductBcUnitData.Text = prodBcUn;
                    var prodDescSize = this.ActItem.ProdInfo.Desc.TrimEnd() + " " + this.ActItem.ProdInfo.UnitSizeDesc;
                    this.LblDescriptionSizeData.Text = prodDescSize;
                    this.LblBrandData.Text = this.ActItem.ProdInfo.BrandDesc;
                }
            }
        }

        private async void CmdDetails_Click(object sender, EventArgs e)
        {
            try
            {
                this.CmdDetails.IsEnabled = false;
                await this.ScannerDisable();
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityDetail,
                    this.ActItem,
                    this.ActSubType);
                _ = await form.DisplayControl.ShowDialog();
                var xmlDoc = await form.GetXml();
                await AssignAndDisplayDetail();

                return;

                ///////////////////////////////////////////////////////////
                async Task AssignAndDisplayDetail()
                {
                    this.ActItem.ReplaceQuantityInActivityItem(
                        this.ActItem.SelectedItem,
                        Math.Round(decimal.Parse(xmlDoc.Root!.Element("qty") !.Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root!.Element("base_qty") !.Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root!.Element("wt_qty") !.Value), 3));
                    await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.CmdDetails.IsEnabled = true;
                await this.ScannerEnable();
            }
        }

        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }

        private async void CmdSetLocation_Click(object sender, EventArgs e)
        {
            try
            {
                this.CmdSetLocation.IsEnabled = false;
                await this.ScannerDisable();
                CheckLocationOverrideAllowed();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                    // Special validation for picking and filtering by section.
                    // Check if the scanned section matched the filtered one.
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking)
                    {
                        var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                        if (sectRefno != 0 &&
                            this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                        {
                            var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                            var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                        $" not match the filtered picking section '{sectName}'.";

                            await Device.InvokeOnMainThreadAsync(
                                async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActItem.GetControlDescription(),
                                        msg,
                                        Resource.OK));
                        }
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
                this.CmdSetLocation.IsEnabled = true;
            }

            return;

            void CheckLocationOverrideAllowed()
            {
                var dspOverrideMsg = false;
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                         this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation) ||
                        (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                         this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                    {
                    }
                    else
                    {
                        dspOverrideMsg = true;
                    }
                }

                if (dspOverrideMsg)
                {
                    const string msg = "Default locations are enabled, location changes cannot be overridden here.";
                    throw new MessageOnlyException(msg);
                }
            }
        }

        private async void CmdSetSkid_Click(object sender, EventArgs e)
        {
            try
            {
                this.CmdSetSkid.IsEnabled = false;
                var form = new FormDialogContainer(
                    EDialogTypes.SkidInfo,
                    this.ActItem.ActHdr,
                    this.ActSubType,
                    SysShared.ActivityType.Items);
                var result = await form.DisplayControl.ShowDialog();
                if (result != DialogResult.OK)
                {
                    throw new CancelOperationException();
                }

                // Current location was assigned in the form.
                var xmlDoc = await form.GetXml();
                var selectedSkidInfo = await SysSkidInfo.GetSkidInfoFromXml(xmlDoc);
                if (selectedSkidInfo.CurrSkid.SkidFkRefno == 0)
                {
                    const string msg = "Expected a selected skid to contain a valid skid foreign key reference number.";
                    throw new Exception(msg);
                }

                await AssignSelectedSkidTheCurrentLocationIfNoneAssigned(selectedSkidInfo);
                await AbortIfSelectedSkidLocationIsIncomplete(selectedSkidInfo);
                await CheckIfPickingSectionMatchesSkidSection(selectedSkidInfo.CurrSkid);
                var actItemFoundRefno = await AttemptToFindMatchingLineItemForSkidProdAndLocation(selectedSkidInfo);
                if (actItemFoundRefno > 0)
                {
                    this.ActItem.ActHdr.SkidInfo = selectedSkidInfo;
                    await this.AssignAndDisplayLineItem(actItemFoundRefno);
                }
                else
                {
                    if (this.ActItem.IsAllowAddItem)
                    {
                        if (await this.AskToAddNewItem(selectedSkidInfo.CurrSkid.ProdInfo))
                        {
                            this.ActItem.ActHdr.SkidInfo = selectedSkidInfo;
                            var prodInfo = ProductInfoTasks.GetXml(selectedSkidInfo.CurrSkid.ProdInfo);

                            // Additional found matching line items are not allowed to be created during
                            // scanning and inserting. I am not expecting matches to be found because
                            // that check has already been performed at this point.
                            // These are assigned in the Insert method.
                            // this.ActItem.ProdInfo
                            // this.ActItem.ProdInfo.Scheme
                            // this.ActItem.ItemRefno
                            // this.ActItem.ItemFkRefno
                            await this.ActItem.Insert(prodInfo);
                            await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);
                        }
                        else
                        {
                            var msg = this.ActItem.GetItemNotFoundMessage(this.LocMap, selectedSkidInfo.CurrSkid.ProdInfo);
                            this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                            this.DisplayCmdSetSkidData();
                            await this.DisplaySkidLocationData();
                            throw new MessageOnlyException(msg);
                        }
                    }
                    else
                    {
                        // Because a new line item is not allowed to be created, the skid will be cleared.
                        var msg = this.ActItem.GetItemNotFoundMessage(this.LocMap, selectedSkidInfo.CurrSkid.ProdInfo);
                        this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                        this.DisplayCmdSetSkidData();
                        await this.DisplaySkidLocationData();
                        throw new MessageOnlyException(msg);
                    }
                }
            }
            catch (CancelOperationException)
            {
                // Do nothing, user cancelled the operation.
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.CmdSetSkid.IsEnabled = true;
            }

            return;

            async Task AssignSelectedSkidTheCurrentLocationIfNoneAssigned(SysSkidInfo selectedSkidInfo)
            {
                if (selectedSkidInfo.CurrSkid.LocMapRefno != 0)
                {
                    return;
                }

                if (await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    selectedSkidInfo.CurrSkid.LocMap = this.LocMap.CurrLocMap;
                }
            }

            static async Task AbortIfSelectedSkidLocationIsIncomplete(SysSkidInfo selectedSkidInfo)
            {
                if (!await LocationMapTasks.IsCompleteLocMap(selectedSkidInfo.CurrSkid.LocMap))
                {
                    const string msg = "The current location must be complete.";
                    throw new MessageOnlyException(msg);
                }
            }

            async Task CheckIfPickingSectionMatchesSkidSection(SSkidInfo skidInfo)
            {
                // Special validation for picking and filtering by section.
                // Check if the skid section matched the activity filtered one.
                if (this.ActSubType != SysCommon.ActSubTypes.Picking)
                {
                    return;
                }

                var header = (PickingActivity)this.ActItem.ActHdr;
                var filterSectRefno = header.PickingLocationMapFilter.SectLocMapRefno;
                if (filterSectRefno != 0)
                {
                    var skidLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(skidInfo.LocMapRefno);
                    if (skidLoc.SectLocMapRefno > 0 && skidLoc.SectLocMapRefno != filterSectRefno)
                    {
                        var sectName = header.PickingLocationMapFilter.SectName;
                        var msg = $"Warning the skid section '{skidLoc.SectName}' does not match the filtered" +
                                  $" picking section '{sectName}'.";
                        throw new MessageOnlyException(msg);
                    }
                }
            }

            async Task<int> AttemptToFindMatchingLineItemForSkidProdAndLocation(SysSkidInfo selectedSkidInfo)
            {
                return await this.ActItem.GetActivityItemRefno(
                    selectedSkidInfo.CurrSkid.ProdInfo.UnitRefno,
                    selectedSkidInfo.CurrSkid.LocMap);
            }
        }

        private async void CmdFirst_Click(object sender, EventArgs e)
        {
            try
            {
                await PerformGotoFirst();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return;

            async Task PerformGotoFirst()
            {
                if (this.ActItem.ListItems.Count > 0)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                    this.ActItem.SelectedItem = this.ActItem.ListItems[0];
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
            }
        }

        private async void CmdLast_Click(object sender, EventArgs e)
        {
            try
            {
                await PerformGotoLast();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () => await this.DisplayAlert("Error", ex.Message, "OK"));
            }

            return;

            async Task PerformGotoLast()
            {
                if (this.ActItem.ListItems.Count > 0)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                    this.ActItem.SelectedItem = this.ActItem.ListItems[^1];
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
            }
        }

        private async void CmdNext_Click(object sender, EventArgs e)
        {
            try
            {
                await PerformGotoNext();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return;

            async Task PerformGotoNext()
            {
                if (this.ActItem.ListItems.Count > 0 && this.ActItem.SelectedListIdx < this.ActItem.ListItems.Count - 1)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                    this.ActItem.SelectedItem = this.ActItem.ListItems[this.ActItem.SelectedListIdx + 1];
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
            }
        }

        private async void CmdPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                await PerformGotoPrevious();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return;

            async Task PerformGotoPrevious()
            {
                if (this.ActItem.ListItems.Count > 0 && this.ActItem.SelectedListIdx > 0)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                    this.ActItem.SelectedItem = this.ActItem.ListItems[this.ActItem.SelectedListIdx - 1];
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                    await this.DisplayData();
                }
            }
        }

        private async void CmdItems_Click(object sender, EventArgs e)
        {
            try
            {
                this.CmdInvInquiry.IsEnabled = false;
                await this.ScannerDisable();
                var form = new FormDialogContainer(
                    EDialogTypes.SelectActivityItem,
                    this.ActItem.ActHdr,
                    this.ActSubType);
                _ = await form.DisplayControl.ShowDialog();
                var xmlDoc = await form.GetXml();
                var itmRefno = Convert.ToInt32(xmlDoc.Root!.Element("ActivityItemRefno") !.Value);
                if (itmRefno > 0)
                {
                    await this.AssignAndDisplayLineItem(itmRefno);

                    // if there is a skid selected keep it if the product and location matches.
                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ActItem.ProdInfo.UnitRefno ||
                            this.ActHdr.SkidInfo.CurrSkid.LocMapRefno != this.LocMap.CurrLocMapRefno)
                        {
                            this.ActHdr.SkidInfo = new SysSkidInfo();
                            this.DisplayCmdSetSkidData();
                            await this.DisplaySkidLocationData();
                        }
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.CmdInvInquiry.IsEnabled = true;
                await this.ScannerEnable();
            }
        }

        private async void CmdAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.CmdAddItem.IsEnabled = false;
                await this.ScannerDisable();
                await PerformAddProductItem();
            }
            catch (CancelOperationException)
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                            return Task.CompletedTask;
                        });
                }

                await this.LoadAndBind();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.CmdAddItem.IsEnabled = true;
                await this.ScannerEnable();
            }

            return;

            async Task PerformAddProductItem()
            {
                var prodInfoToAdd = await AskForProductToAdd();
                this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        this.DisplayCmdSetSkidData();
                        await this.DisplaySkidLocationData();
                        return Task.CompletedTask;
                    });

                // Get the location used in finding match line item.
                await this.SetDefaultLocationBeforeKnownActivityItem(prodInfoToAdd);
                await this.CheckCurrentLocationMatchDefaultProductLocation(
                    prodInfoToAdd,
                    LocationMap.GetInstance().CurrLocMap);
                this.DisplayCmdSetLocationData();
                await this.DisplayLocationData();

                await FindOrCreateLineItem(prodInfoToAdd);
                await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);
            }

            async Task<ProductInfo> AskForProductToAdd()
            {
                var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);
                if (await form.DisplayControl.ShowDialog() != DialogResult.OK)
                {
                    throw new CancelOperationException();
                }

                var xmlDoc = await form.GetXml();
                if (xmlDoc.Root == null)
                {
                    const string msg = "Product selected returned an invalid selection.";
                    throw new Exception(msg);
                }

                var prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno")?.Value);
                if (prodUnitRefno == 0)
                {
                    throw new MessageOnlyException("A product unit must be chosen.");
                }

                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                if (prodInfo.UnitRefno == 0)
                {
                    const string msg = "Product selected reference number is not valid.";
                    throw new Exception(msg);
                }

                return prodInfo;
            }

            async Task FindOrCreateLineItem(ProductInfo prodInfoToAdd)
            {
                var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                    prodInfoToAdd.UnitRefno,
                    LocationMap.GetInstance().CurrLocMap);
                if (actItemFoundRefno > 0)
                {
                    await this.AssignAndDisplayLineItem(actItemFoundRefno);

                    const string msg = "This location and product already exists. Moving to item.";
                    throw new MessageOnlyException(msg);
                }

                // Insert into activity item table, and add new activity item to the list.
                // Inserted item uses the current location.
                // this.ActItem.ItemRefno assigned in Insert();
                // this.ActItem.ItemFKRefno assigned in Insert();
                // this.ActItem.ProdInfo assigned in Insert();
                // this.ActItem.ProdInfo.Scheme assigned in Insert();
                var unitSizeDesc = SysCommon.AddXmlReservedChars(prodInfoToAdd.UnitSizeDesc);
                var xmlString = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <params>
                    <ProductUnitRefno>{prodInfoToAdd.UnitRefno}</ProductUnitRefno>
                    <ProductRefno>{prodInfoToAdd.Refno}</ProductRefno>
                    <ProductCode>{prodInfoToAdd.Code}</ProductCode>
                    <ProductDescription>{SysCommon.AddXmlReservedChars(prodInfoToAdd.Desc)}</ProductDescription>
                    <UnitCode>{prodInfoToAdd.UnitCode}</UnitCode>
                    <BaseUnitCode>{prodInfoToAdd.BaseUnitCode}</BaseUnitCode>
                    <ProductUnitBC>{prodInfoToAdd.UnitBc}</ProductUnitBC>
                    <ProductUnitSizeDescription>{unitSizeDesc}</ProductUnitSizeDescription>
                    </params>";
                var xmlDoc = XDocument.Parse(xmlString);
                await this.ActItem.Insert(xmlDoc);
            }
        }

        private async Task AssignAndDisplayLineItem(int itemRefno)
        {
            this.ActItem.SetSelectedActivityItemFromList(itemRefno);
            if (this.ActItem.SelectedItem == null)
            {
                throw new Exception("Failed to set the new selected item in the list.");
            }

            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
            await this.DisplayData();
        }

        private async Task SetDefaultLocationBeforeKnownActivityItem(ProductInfo prodInfo)
        {
            // Need to get a warehouse to determine default location if turned on, we don't
            // have the line item warehouse yet. We could assume the header warehouse or even
            // the login warehouse.
            // We can't determine the default location until after identifying the line item.
            var assignDefaultLocation = true;
            await IfSkidAssignedAndSubTypeCountUseSkidLocation();

            if (assignDefaultLocation)
            {
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // This will be the default location if the line item location is not complete.
                    var whsLocMapRefno = await LocationMapTasks.GetWhsLocMapRefnoFromLocMapRefno(
                        this.LocMap.CurrLocMapRefno);
                    var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(whsLocMapRefno, prodInfo.Refno);

                    if (IsAllowedDefaultLocationOverride())
                    {
                        await UseChosenLocationIfComplete(defLocMapRefno);
                    }
                    else
                    {
                        await SetLocationIfComplete(defLocMapRefno);
                    }
                }
                else
                {
                    SetCurrentLocationWithChosen();
                }
            }

            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
            {
                const string msg = "Location must be set first!";
                throw new MessageOnlyException(msg);
            }

            return;

            void SetCurrentLocationWithChosen()
            {
                // This should not be needed, current location should have been already set using the location picker.
                if (this.LocMap.CurrLocMapRefno != this.LocMap.ChosenLocMapRefno)
                {
                    // Set the current location to the chosen one, overriding and defaulting location behaviour.
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                }
            }

            bool IsAllowedDefaultLocationOverride()
            {
                var isAllowedLocationOverride = false;
                var useProductDefaultLocation = this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
                if (useProductDefaultLocation)
                {
                    var allowLocationOverridePhysical = this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                                                        this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation;
                    var allowLocationOverrideCycle = this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                                                     this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation;
                    if (allowLocationOverridePhysical || allowLocationOverrideCycle)
                    {
                        isAllowedLocationOverride = true;
                    }
                }

                return isAllowedLocationOverride;
            }

            async Task IfSkidAssignedAndSubTypeCountUseSkidLocation()
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    if (!(this.ActItem.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                          this.ActItem.ActSubType == SysCommon.ActSubTypes.CycleCounts))
                    {
                        if (await LocationMapTasks.IsCompleteLocMap(this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap))
                        {
                            this.LocMap.CurrLocMap = this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap;
                            assignDefaultLocation = false;
                        }
                    }
                }
            }

            async Task UseChosenLocationIfComplete(int defLocMapRefno)
            {
                // If we are allowed to override the default location, then we can use the chosen location
                // if it is complete.
                if (await LocationMapTasks.IsCompleteLocMap(this.LocMap.ChosenLocMap))
                {
                    SetCurrentLocationWithChosen();
                }
                else
                {
                    // if the chosen location is not complete, then we must use the default location.
                    await SetLocationIfComplete(defLocMapRefno);
                }
            }

            async Task SetLocationIfComplete(int locMapRefno)
            {
                if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
                {
                    const string msg = "Default locations is enabled, and for this activity and product" +
                                       " the location is not fully complete.";
                    throw new MessageOnlyException(msg);
                }

                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
            }
        }

        private async Task CheckCurrentLocationMatchDefaultProductLocation(ProductInfo prodInfo, SLocMap locMap)
        {
            var isLocationComplete = await LocationMapTasks.IsCompleteLocMap(locMap);
            var useSelectedLocation = !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc;
            if (isLocationComplete && useSelectedLocation)
            {
                // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                var isVerifyProductLocation = this.SysEnv.SysSettings.IsVerifyProductLocation;
                var isActivityTypeToVerify = this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                             this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                                             this.ActSubType == SysCommon.ActSubTypes.TransferTo;
                if (isVerifyProductLocation && isActivityTypeToVerify)
                {
                    // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                    // Picking activity items only have the warehouse, not section or slot.
                    if (locMap.WhsLocMapRefno <= 0)
                    {
                        throw new Exception("Invalid warehouse on activity item. LocationMapRefno == 0");
                    }

                    if (prodInfo.Refno <= 0)
                    {
                        throw new Exception("Invalid product on activity item. ProductRefno == 0");
                    }

                    var defLocMapRefno =
                        await this.ActItem.GetDefaultLocMapRefno(locMap.WhsLocMapRefno, prodInfo.Refno);
                    if (LocationMap.GetInstance().CurrLocMapRefno != defLocMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The current location does not match the product home location.");
                    }
                }
            }
        }

        /// <summary>
        ///     Command add detail click event handler.
        /// </summary>
        /// <param name="sender">Object that generated this event.</param>
        /// <param name="e">Event argument data.</param>
        private async void CmdAddDetail_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.CmdAddDetail.IsEnabled = false;
                await this.ScannerDisable();
                await PerformAddProductDetail();
            }
            catch (GatherProductDataException gatherEx)
            {
                if (!gatherEx.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, gatherEx);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
                this.CmdAddDetail.IsEnabled = true;
            }

            return;

            async Task PerformAddProductDetail()
            {
                if (!await CheckAllowedToAddDetail())
                {
                    const string msg = "Permission denied to add detail.";
                    throw new MessageOnlyException(msg);
                }

                var locMap = LocationMap.GetInstance();
                if (!await LocationMapTasks.IsCompleteLocMap(locMap.CurrLocMapRefno))
                {
                    const string msg = "Location must be set first!";
                    throw new MessageOnlyException(msg);
                }

                if (locMap.CurrLocMap.WhsLocMapRefno != this.ActItem.LocMap.WhsLocMapRefno)
                {
                    const string msg = "Detail warehouse must match the line item warehouse.";
                    throw new MessageOnlyException(msg);
                }

                await PerformInsertDetail();
                this.ActItem.AddToQuantityInActivityItem(
                    this.ActItem.SelectedItem,
                    this.ActDetail.Quantity,
                    this.ActDetail.BaseQuantity,
                    this.ActDetail.WeightQuantity);
                await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);
                await this.DisplayData();
            }

            async Task<bool> CheckAllowedToAddDetail()
            {
                var isAllowedAddManualDetail = true;
                if (this.SysEnv.SysSettings.IsPickAddManualDetailPasswordRequired &&
                    this.ActSubType == SysCommon.ActSubTypes.Picking)
                {
                    var form = new FormAddManualDetailOverride();
                    isAllowedAddManualDetail = await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await form.ShowDialog() == DialogResult.OK);
                }

                return isAllowedAddManualDetail;
            }

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
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1900, 1, 1);

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of
                // the length of the code and the fact isn't human-readable friendly.
                var tempItemProdSchemeData = new ActItemSchemeData();
                var schemeData = this.ActDetail.SchemeData;
                var skidInfo = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (skidInfo.SkidFkRefno == 0)
                {
                    // Set to empty values, otherwise the detail edit screen may not be shown.
                    schemeData.LogisticUnitSkidNumber = string.Empty;
                    schemeData.LogisticUnitBatchLotNumber = string.Empty;
                    ////schemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
                    ////schemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
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

            async Task PerformInsertDetail()
            {
                var gatherActDetail = ActDetailFacade.GetActDetail(this.ActDetail.ActItem, this.ActSubType);

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
                var tempItemProdSchemeData = InitDetailBeforeGather();

                var isInsertReady = await SysShared.GatherProductData(
                    parse,
                    this.ActSubType,
                    this.ActDetail.ActItem,
                    tempItemProdSchemeData,
                    gatherActDetail,
                    true);

                if (isInsertReady)
                {
                    gatherActDetail.DetailFkRefno = 0;
                    gatherActDetail.ActStatus = SysCommon.ActStatus.Inserted;
                    this.ActDetail.Quantity = gatherActDetail.Quantity;
                    this.ActDetail.BaseQuantity = gatherActDetail.BaseQuantity;
                    this.ActDetail.WeightQuantity = gatherActDetail.WeightQuantity;

                    // actDetail.DetailFkRefno and actDetail.DetailRefno is assigned in Insert().
                    var xmlDoc = gatherActDetail.GetXml(gatherActDetail);
                    await gatherActDetail.Insert(xmlDoc);
                }
            }
        }

        private async void CmdClearSkid_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.CmdClearSkid.IsEnabled = false;
                this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                await this.DisplayData();
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                this.CmdClearSkid.IsEnabled = true;
            }
        }

        private async void CmdAddSkid_Clicked(object sender, EventArgs e)
        {
            try
            {
                this.CmdAddSkid.IsEnabled = false;
                await this.ScannerEnable();
                await PerformAddSkid();
            }
            catch (CancelOperationException)
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
            finally
            {
                await this.ScannerEnable();
                this.CmdAddSkid.IsEnabled = true;
            }

            return;

            async Task PerformAddSkid()
            {
                var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                if (currSkid == null || currSkid.SkidFkRefno == 0)
                {
                    const string msg = "Please select a skid to add.";
                    throw new MessageOnlyException(msg);
                }

                if (!await this.DisplayAlert(
                        "Add Skid",
                        "Add the available skid quantity to this activity?",
                        "Yes",
                        "No"))
                {
                    throw new CancelOperationException();
                }

                await FindOrCreateLineItem(currSkid);
                await this.AssignAndDisplayLineItem(this.ActItem.ItemRefno);

                await PerformAddDetail();
                this.ActItem.AddToQuantityInActivityItem(
                    this.ActItem.SelectedItem,
                    this.ActDetail.Quantity,
                    this.ActDetail.BaseQuantity,
                    this.ActDetail.WeightQuantity);
                await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);
                await this.DisplayData();
            }

            async Task FindOrCreateLineItem(SSkidInfo currSkid)
            {
                // Check if the line item exists already.
                var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                    currSkid.ProdInfo.UnitRefno,
                    currSkid.LocMap);
                if (actItemFoundRefno > 0)
                {
                    this.ActItem.ItemRefno = actItemFoundRefno;
                }
                else
                {
                    // Check rule for default location and product default location must match.
                    await this.CheckCurrentLocationMatchDefaultProductLocation(currSkid.ProdInfo, currSkid.LocMap);

                    // Insert into activity item table, and add new activity item to the list.
                    // Inserted item uses the current location.
                    // this.ActItem.ItemRefno assigned in Insert();
                    // this.ActItem.ItemFKRefno assigned in Insert();
                    // this.ActItem.ProdInfo assigned in Insert();
                    // this.ActItem.ProdInfo.Scheme assigned in Insert();
                    var xmlDoc = ProductInfoTasks.GetXml(currSkid.ProdInfo);
                    await this.ActItem.Insert(xmlDoc);
                }
            }

            async Task PerformAddDetail()
            {
                this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                this.ActDetail.ActItem = this.ActItem;

                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                // because of the length of the code and the fact isn't human-readable friendly.
                var schemeData = this.ActDetail.SchemeData;
                schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                this.ActDetail.SchemeData = schemeData;
                this.ActDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                    this.ActItem.ProdInfo.Code,
                    this.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);
                this.ActDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                    this.ActItem.ProdInfo.Code,
                    this.ActItem.ProdInfo.Scheme,
                    this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate
                // checks and quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<imclient>" +
                    "<itm_refno> +" + this.ActItem.ItemFkRefno + "</itm_refno>" +
                    "<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>" +
                    "<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>" +
                    "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId + "</ActivityDetailUniqueSchemeID>" +
                    "</imclient>");

                await this.ActItem.IsReadyToApply(xmlParam);
                await this.ActDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);
            }
        }

        private async void CmdInvInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                var actDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.InventoryInquiry);
                actDetail.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    this.ActItem.LocMap.WhsLocMapRefno);
                var form = new FormDialogContainer(
                    EDialogTypes.ActivityInventoryInquiryDetail,
                    actDetail,
                    SysCommon.ActSubTypes.InventoryInquiry);
                await form.DisplayControl.ShowDialog();
                if (await form.DisplayControl.ShowDialog() != DialogResult.OK)
                {
                    throw new CancelOperationException();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(ex);
                        _ = await errRpt.ShowDialog();
                    });
            }
        }
    }
}