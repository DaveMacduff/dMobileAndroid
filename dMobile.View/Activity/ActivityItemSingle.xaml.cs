// <copyright file="ActivityItemSingle.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using dMobile.Controller.Activity;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using static Android.Views.View;
    using Resource = dMobile.Resources.Properties.Resources;

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
        /// <param name="actSubTypes">Activity subtype.</param>
        public ActivityItemSingle(Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.LocMap = LocationMap.GetInstance();
            this.SysEnv = SysEnvironment.GetInstance();
            this.ActSubType = actSubTypes;

            // Init activity header object
            this.ActHdr = activity;
            this.ActHdr.SkidInfo ??= new SysSkidInfo();

            // Init activity item object
            this.ActItem = ActItemFacade.GetActItem(this.ActHdr, this.ActSubType);
            this.ActItem.ActHdr = this.ActHdr;

            InitBarCodeModifiers();
            this.Title = this.GetControlDescription();
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

        /// <summary>
        /// Gets or sets the Scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the LocMap.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the ActHdr.
        /// </summary>
        private Activity ActHdr { get; set; }

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this controls data is being loaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

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
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;
            return isReadyToSave;
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;
            return isReadyToClose;
        }

        /// <summary>
        /// Get the control description based on the activity subtype.
        /// </summary>
        /// <returns>.</returns>
        private string GetControlDescription()
        {
            var desc = this.ActSubType switch
            {
                SysCommon.ActSubTypes.PhysicalCounts => $"Count # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.CycleCounts => $"Count # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Receiving => $"Receive # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.Picking => $"Pick # {this.ActItem.ActHdr.HdrFkRefno}",
                SysCommon.ActSubTypes.PickingSingle => $"Pick # {this.ActItem.ActHdr.HdrFkRefno}",
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

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;
            return isSaveComplete;
        }

        /// <summary>
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = false;
            return cancelChanges;
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
        /// The BarCodeHandler.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="scanArgs">The scanArgs<see cref="BarCodeScannerEventArgs"/>.</param>
        /// <exception cref="MessageOnlyException"></exception>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string barMsg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
                            this.ActHdr.GetControlDescription(),
                            barMsg,
                            Resource.OK));
                    throw new TaskCanceledException();
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
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
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

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(BackButtonCallback);
            return true;

            async void BackButtonCallback()
            {
                _ = base.OnBackButtonPressed();

                if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            }
        }

        /// <summary>
        /// The DisplayData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task DisplayData()
        {
        }

        /// <summary>
        /// The RefreshData.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RefreshData()
        {
            try
            {
                await this.ScannerDisable();
                await this.ActItem.Refresh(this.ActHdr.XmlDataCriteria);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(
                    async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        "OK"));
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// The ucActivityItem_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task ucActivityItem_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();
        }

        /// <summary>
        /// The CmdLocation_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdLocation_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();
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
            }
        }

        /// <summary>
        /// The cmdSelectSkid_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdSelectSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();

                    var form = new FormDialogContainer(EDialogTypes.SkidInfo, this.ActItem, this.ActSubType);
                    {
                        _ = await form.DisplayControl.ShowDialog();
                        var xmlDoc = await form.GetXml();
                        this.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXml(xmlDoc);

                        ////if (this.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                        ////{
                        ////    this.cmdAddSkidIsEnabled = true;

                        ////    this.cmdSelectSkidText =
                        ////        Convert.ToString("Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode);

                        ////    this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;

                        ////    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        ////    _ = await Device.InvokeOnMainThreadAsync(
                        ////        () =>
                        ////            this.cmdLocationText =
                        ////                "Location: " + Environment.NewLine + currentReadableLocation);

                        ////    this.cmdLocationIsEnabled = false;
                        ////}
                        ////else
                        ////{
                        ////    this.cmdAddSkidIsEnabled = false;

                        ////    this.cmdSelectSkidText = "Select Skid";

                        ////    this.cmdLocationIsEnabled = true;
                        ////}
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
            }
        }

        /// <summary>
        /// The cmdAddProduct_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdAddProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var itemFoundRefno = 0;
                    var prodUnitRefno = 0;

                    await this.ScannerDisable();

                    // Not allowed to count individual boxes on a skid. Skids must be
                    // scanned as a whole unit or built in skid maintenance.

                    // DEV-827 This change to allow counting partial skids is a bad idea.
                    ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                    ////            this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                    ////{
                    ////    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    ////    {
                    ////        this.ActHdr.SkidInfo = new SysSkidInfo();
                    ////        this.cmdSelectSkid.Text = string.Empty;
                    ////    }
                    ////}

                    XDocument xmlDoc = null;

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFkRefno == 0)
                    {
                        // activity item object needs to check if a new location and product can be added, if not add a detail
                        if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                        {
                            throw new MessageOnlyException("The location must be set.");
                        }

                        var form = new FormDialogContainer(EDialogTypes.SelectProductUnit, this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            xmlDoc = await form.GetXml();
                            prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        var info = this.ActHdr.SkidInfo.CurrSkid.ProdInfo;

                        xmlDoc = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<params>" +
                            "<ProductUnitRefno>" +
                            info.UnitRefno +
                            "</ProductUnitRefno>" +
                            "<ProductRefno>" +
                            info.Refno +
                            "</ProductRefno>" +
                            "<ProductCode>" +
                            info.Code +
                            "</ProductCode>" +
                            "<ProductDescription>" +
                            SysCommon.AddXmlReservedChars(info.Desc) +
                            "</ProductDescription>" +
                            "<UnitCode>" +
                            info.UnitCode +
                            "</UnitCode>" +
                            "<ProductUnitBC>" +
                            info.UnitBc +
                            "</ProductUnitBC>" +
                            "<ProductUnitSizeDescription>" +
                            SysCommon.AddXmlReservedChars(info.UnitSizeDesc) +
                            "</ProductUnitSizeDescription>" +
                            "</params>");
                    }

                    if (prodUnitRefno != 0)
                    {
                        itemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            prodUnitRefno,
                            LocationMap.GetInstance().CurrLocMap);

                        if (itemFoundRefno > 0)
                        {
                            // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            // foundRowIdx = this.ActItem.DataViewItems.Find(itemFoundRefno);
                            // this.ActItem.CurrRowIdx = foundRowIdx;
                            this.ActItem.SelectedItem = this.ActItem.ListItems.OfType<object>()
                                .FirstOrDefault(
                                    activityItem => Convert.ToString(
                                                        activityItem.GetType()
                                                            .GetProperty("ActivityItemRefno")
                                                            ?.GetValue(activityItem)) ==
                                                    Convert.ToString(itemFoundRefno));
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(xmlDoc);

                                // This is a little dangerous we assume that the new row idx
                                // will be the last one
                                this.ActItem.SelectedItem = this.ActItem.ListItems.Last();
                                itemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                throw new MessageOnlyException(
                                    "Product rule says that it must exist on" +
                                    " this document already.");
                            }
                        }

                        // Rule: Check if the current location and product home location are the same.
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                            (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                             this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                             this.ActSubType == SysCommon.ActSubTypes.TransferTo))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = this.ActItem.GetDefaultLocMapRefno(
                                Convert.ToInt32(((dynamic)this.ActItem.SelectedItem)?.LocationMapRefno),
                                Convert.ToInt32(((dynamic)this.ActItem.SelectedItem)?.ProductRefno));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                throw new MessageOnlyException(
                                    "The current location does not match the" +
                                    " product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                        ////                                      this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
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

                        ////BarCodeParse barCodeParse = BarCodeParseFacade.ParseData(string.Empty);
                        ////bool isInsertReady = false;
                        ////ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        ////actDetail.Quantity = 0;
                        ////actDetail.BaseQuantity = 0;
                        ////actDetail.WeightQuantity = 0;
                        ////actDetail.SchemeID = string.Empty;
                        ////actDetail.UniqueSchemeID = string.Empty;
                        ////actDetail.ActExtra.Text1 = string.Empty;
                        ////actDetail.ActExtra.Text2 = string.Empty;
                        ////actDetail.ActExtra.Text3 = string.Empty;
                        ////actDetail.ActExtra.Text4 = string.Empty;
                        ////actDetail.ActExtra.Text5 = string.Empty;

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
                        ////    barCodeParse,
                        ////    this.ActSubType,
                        ////    this.ActItem,
                        ////    tempItemProdSchemeData,
                        ////    this.CallingControl,
                        ////    ref isInsertReady,
                        ////    actDetail,
                        ////    true);

                        ////this.InsertActivityDetail(ref foundRowIdx, actDetail, barCodeParse, isInsertReady);
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

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as MessageOnlyException vs Exception.
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
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(ex);
                            _ = await errRpt.ShowDialog();
                        });

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            await this.RefreshData();
                            break;
                    }
                }
                finally
                {
                    await this.DisplayData();
                    await this.ScannerEnable();
                }
            }
        }

        /// <summary>
        /// The cmdAddSkid_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task cmdAddSkid_Click(object sender, EventArgs e)
        {
            // TODO: Test
            if (this.IsControlLoaded)
            {
                try
                {
                    var actItemFoundRefno = 0;
                    var prodUnitRefno = 0;
                    object foundObject = 0;
                    var message = string.Empty;

                    await this.ScannerDisable();

                    if (await Device.InvokeOnMainThreadAsync(
                            async () => await this.DisplayAlert(
                                "Skid",
                                "Add the ENTIRE skid to this activity?",
                                "Yes",
                                "No")))
                    {
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            prodUnitRefno,
                            LocationMap.GetInstance().CurrLocMap);
                        if (actItemFoundRefno > 0)
                        {
                            foundObject = this.ActItem.ListItems.OfType<object>()
                                .FirstOrDefault(
                                    activityItem =>
                                        Convert.ToString(
                                            activityItem.GetType()
                                                .GetProperty("ActivityItemRefno")
                                                ?.GetValue(activityItem)) ==
                                        Convert.ToString(actItemFoundRefno));
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(
                                    ProductInfoTasks.GetXml(this.ActHdr.SkidInfo.CurrSkid.ProdInfo));

                                // This is a little dangerous we assume that the new row idx
                                // will be the last one
                                foundObject = this.ActItem.ListItems.Last();
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                message = string.Format(
                                    "Item not found for " +
                                    Environment.NewLine +
                                    "Location {0} " +
                                    Environment.NewLine +
                                    "Product {1} BC {2} Unit {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Code,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitBc,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                                throw new MessageOnlyException(message);
                            }
                        }

                        this.ActItem.SelectedItem = foundObject;

                        // Insert a new activity detail record
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        var scheme = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        var unqScheme = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                            "<imclient>" +
                            "<itm_refno>" +
                            this.ActItem.ItemFkRefno +
                            "</itm_refno>" +
                            "<alt_qty>" +
                            this.ActHdr.SkidInfo.CurrSkid.Quantity +
                            "</alt_qty>" +
                            "<wt_qty>" +
                            this.ActHdr.SkidInfo.CurrSkid.WeightQuantity +
                            "</wt_qty>" +
                            "<ActivityDetailUniqueSchemeID>" +
                            actDetail.UniqueSchemeId +
                            "</ActivityDetailUniqueSchemeID>" +
                            "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActHdr.SkidInfo);

                        // upon a save of completed data update the data view to to show new item
                        // and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            foundObject,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        await this.ActItem.UpdateItemTableQuantity(foundObject);
                        await this.DisplayData();
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
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
            }
        }

        /// <summary>
        /// The CmdDetails_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void CmdDetails_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    await this.ScannerDisable();
                    await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);

                    var currentSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        if (currentSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                        {
                            throw new MessageOnlyException(
                                "Selected line item product must match the" +
                                " selected skid product.");
                        }
                    }

                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityDetail,
                        this.ActItem,
                        this.ActSubType);
                    _ = await form.DisplayControl.ShowDialog();
                    var xmlDoc = await form.GetXml();

                    this.ActItem.ReplaceQuantityInActivityItem(
                        this.ActItem.SelectedItem,
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("base_qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("wt_qty").Value), 3));

                    await this.ActItem.UpdateItemTableQuantity(this.ActItem.SelectedItem);

                    ////if (currentSkid.SkidFkRefno != 0)
                    ////{
                    ////    this.cmdAddSkidIsEnabled = true;
                    ////    this.cmdSelectSkidText = "Skid# " + currentSkid.SkidCode;
                    ////    this.cmdLocationIsEnabled = false;
                    ////}
                    ////else
                    ////{
                    ////    this.cmdAddSkidIsEnabled = false;

                    ////    this.cmdSelectSkidText = "Select Skid";
                    ////    this.cmdLocationIsEnabled = true;
                    ////}

                    await this.DisplayData();
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
            }
        }

        /// <summary>
        /// The CmdNext_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void CmdNext_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var isValid = true;
                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        if (this.ActItem.SelectedItem != null)
                        {
                            try
                            {
                            }
                            catch (Exception ex)
                            {
                                switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                                {
                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                        if (await Device.InvokeOnMainThreadAsync(
                                                async () =>
                                                    !await this.DisplayAlert(
                                                        "Quantity Validation",
                                                        ex.Message + Environment.NewLine + "Continue?",
                                                        "Yes",
                                                        "No")))
                                        {
                                            isValid = false;
                                        }

                                        break;

                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                        await Device.InvokeOnMainThreadAsync(
                                            async () =>
                                                await this.DisplayAlert(
                                                    "Quantity Validation",
                                                    ex.Message,
                                                    "OK"));
                                        isValid = false;

                                        break;
                                }
                            }
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(
                                async () => await this.DisplayAlert(
                                    this.GetControlDescription(),
                                    "Last Item",
                                    "OK"));
                        }
                    }

                    if (isValid)
                    {
                        if (this.ActItem.SelectedItem != null)
                        {
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            ////if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            ////{
                            ////    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            ////    _ = await Device.InvokeOnMainThreadAsync(
                            ////        () =>
                            ////            this.cmdLocationText =
                            ////                "Location: " + Environment.NewLine + currentReadableLocation);
                            ////}
                            ////else
                            ////{
                            ////    this.cmdLocationText = "Set Location";
                            ////}
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
                    await this.DisplayData();
                }
            }
        }

        /// <summary>
        /// The CmdPrevious_Click.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private async void CmdPrevious_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    var isValid = true;

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                            {
                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                    if (await Device.InvokeOnMainThreadAsync(
                                            async () => !await this.DisplayAlert(
                                                "Quantity Validation",
                                                ex.Message + Environment.NewLine + "Continue?",
                                                "Yes",
                                                "No")))
                                    {
                                        isValid = false;
                                    }

                                    break;

                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                    await Device.InvokeOnMainThreadAsync(
                                        async () => await this.DisplayAlert(
                                            "Quantity Validation",
                                            ex.Message,
                                            "OK"));

                                    isValid = false;
                                    break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        if (this.ActItem.SelectedItem != null)
                        {
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedItem);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            ////if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            ////{
                            ////    var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            ////    _ = await Device.InvokeOnMainThreadAsync(
                            ////        () =>
                            ////            this.cmdLocationText =
                            ////                "Location: " + Environment.NewLine + currentReadableLocation);
                            ////}
                            ////else
                            ////{
                            ////    this.cmdLocationText = "Set Location";
                            ////}
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
                    await this.DisplayData();
                }
            }
        }

        /// <summary>
        /// The KeyUpF2_ShowLineItemComment.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task KeyUpF2_ShowLineItemComment(object sender, KeyEventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (this.ActItem.SelectedItem == null)
                {
                    throw new MessageOnlyException("Please select and line item to show the comments for.");
                }

                string comment =
                    await this.ActItem.GetComment(
                        Convert.ToInt32((this.ActItem.SelectedItem as dynamic).ActivityItemRefno));

                if (!string.IsNullOrEmpty(comment))
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            "Item Comment",
                            comment,
                            "OK"));
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
        }

        /// <summary>
        /// The KeyUpF3_ShowTotalQuantity.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task KeyUpF3_ShowTotalQuantity(object sender, KeyEventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var msg = await this.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            "OK"));
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
        }

        /// <summary>
        /// The BarCodeModifier1Picker_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BarCodeModifier1Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }

        private void CmdSetLocation_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdSetSkid_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdFirst_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdLast_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdItems_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdAddItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CmdAddDetail_Clicked(object sender, EventArgs e)
        {
        }

        private void CmdClearSkid_Clicked(object sender, EventArgs e)
        {

        }
    }
}