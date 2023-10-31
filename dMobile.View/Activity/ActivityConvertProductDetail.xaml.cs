// <copyright file="ActivityConvertProductDetail.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Controller.Activity_Item;
    using Resources.Enums;
    using Resources.Types;
    using CustomPages.Modal;
    using Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Activity convert product detail.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertProductDetail : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityConvertProductDetail"/> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Convert product activity detail.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub types.</param>
        public ActivityConvertProductDetail(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.CmdAdd.Text = "Add";
            this.SysEnv = SysEnvironment.GetInstance();
            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.Title = this.GetControlDescription();

            this.ActSubType = actSubTypes;
            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;
        }

        /// <summary>
        /// Gets or sets the product we are changing out of.
        /// </summary>
        private int FromProdRefno { get; set; }

        /// <summary>
        /// Gets or sets the desired product to change into.
        /// </summary>
        private int ToProdRefno { get; set; }

        /// <summary>
        /// Gets or set the to product information.
        /// </summary>
        private ProductInfo ToProduct { get; set; }

        /// <summary>
        /// Gets or sets the from product information.
        /// </summary>
        private ProductInfo FromProduct { get; set; }

        /// <summary>
        /// Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the bar code scanner engine.
        /// </summary>
        private BarCodeScanner Scanner { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control is loaded.
        /// </summary>
        private bool IsControlLoaded { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the convert product activity detail.
        /// </summary>
        private ActDetail ActDetail { get; set; } = null;

        /// <summary>
        /// Gets or sets the activity convert product extra info items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the main activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    await this.Scanner.Stop();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                // Currently no reason to validate detail, because the detail was already submitted.
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
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
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
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
                    // All user controls that initialize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    var isPrintLabels = this.HasData();
                    isPrintLabels = isPrintLabels && this.SysEnv.SysSettings.CanPrintProductLabelConvertProduct;
                    isPrintLabels = isPrintLabels && this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch;
                    isPrintLabels = isPrintLabels && !this.SysEnv.SysSettings.IsDisconnectedMode;
                    if (isPrintLabels)
                    {
                        // weren't allowed to print on add or scan, so print now
                        if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Print Label",
                                "Print Product Labels?",
                                "Yes",
                                "No")))
                        {
                            await this.PrintBatchProductLabels();
                        }
                    }

                    saveComplete = true;
                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Data Validation",
                        "Data failed validation. Check your data.",
                        "OK"));
                }
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
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (await this.IsReadyToClose())
                {
                    // Change any values back that you need
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
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
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var schemeData = this.ActDetail.SchemeData;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>"
                + "<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                + "<variant_code>" + schemeData.VariantCode + "</variant_code>"
                + "<variant_no>" + schemeData.VariantNumber + "</variant_no>"
                + "<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>"
                + "<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>"
                + "<scheme_id>" + scheme + "</scheme_id>"
                + "<unq_scheme_id>" + unqScheme + "</unq_scheme_id>"
                + "</params>");

            return request;
        }

        /// <summary>
        /// Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandlerCallback);

            try
            {
                if (this.IsFirstAppearance)
                {
                    await ScannerStart();
                    this.IsFirstAppearance = false;
                    _ = await this.ActDetail.GetActivityDetailList(false);

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.MainActivityItems = this.ActDetail.DesignGrid(
                            ref this.GridMainActivity,
                            ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                    });

                    this.ActDetail.ActItem.SelectedItem = this.ActDetail.ActItem.ListItems[0];
                    if (this.ActDetail.ActItem.ListItems[0] is ConvertProductItem convertProductItem)
                    {
                        this.FromProdRefno = convertProductItem.FromProdFKRefno;
                        this.ToProdRefno = convertProductItem.ToProdFKRefno;
                    }

                    var prodInfo = new ProductInfo();
                    this.ToProduct = await ProductInfoTasks.GetInfoFromRefno(this.ToProdRefno);
                    this.FromProduct = await ProductInfoTasks.GetInfoFromRefno(this.FromProdRefno);

                    // fill in all the activity data
                    this.ActDetail.ActItem.ProdInfo = this.ToProduct;
                }

                await this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    this.IsControlLoaded = true;
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task canceled by user
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
            catch (CommunicationException commEx)
            {
                this.IsFirstAppearance = true;

                // Question should a communication error, be reported as an error or just a messagebox?
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load activity details.{Environment.NewLine}" +
                        $"Because of a communication problem.{Environment.NewLine}" +
                        $"Detail:{commEx.Message}{Environment.NewLine}" +
                        "Please exit and try again.";

                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
                        msg,
                        Resource.OK);
                });
            }
            catch (Exception ex)
            {
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = $"Unable to load activity details.{Environment.NewLine}" +
                        $"General exception.{Environment.NewLine}" +
                        $"Detail:{ex.Message}{Environment.NewLine}" +
                        "Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task ScannerStart()
            {
                if (this.Scanner != null)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async void KeyUpHandlerCallback(string keyPressed)
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.IsPageLoading)
                {
                    if (keycode == Keycode.Enter)
                    {
                        await this.AddManually();
                    }
                    else if (keycode == Keycode.F1)
                    {
                        await this.CloseForm();
                    }
                    else if (keycode == Keycode.Del)
                    {
                        // Commented out because we currently don't support it
                        // but wanted to. Uncomment to add most of this functionality
                        // back in.
                        // DeleteAdjustment();
                    }
                }
            }
        }

        /// <summary>
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformBackPress);
            return true;

            //////////////////////////////////////////////////////////////////////////
            async void PerformBackPress()
            {
                _ = await this.CancelChanges();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }

        /// <summary>
        /// Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Add manually new activity detail.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task AddManually()
        {
            var form = new FormDialogContainer(
                EDialogTypes.ActivityDetailEdit,
                this.ActDetail,
                SysCommon.ActSubTypes.ConvertProduct);
            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                var xmlDoc = await form.GetXML();
                await Task.Run(async () =>
                {
                    await this.CreateAdjustment(xmlDoc);

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.MainActivityItems = this.ActDetail.DesignGrid(
                            ref this.GridMainActivity,
                            ref this.ListViewMainActivity);
                        this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                        this.SelectRow(this.ActDetail.ListDetails.Count - 1);
                    });

                    // We check if we are allowed to print on scan
                    if (this.SysEnv.SysSettings is
                        {
                            CanPrintProductLabelConvertProduct: true,
                            IsPrintProductLabelsAsBatch: false,
                            IsDisconnectedMode: false
                        })
                    {
                        await this.PrintProductLabel();
                    }
                });
            }
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        private string GetControlDescription()
        {
            return "Convert Products";
        }

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated the event.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Sender bar code scanner event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsPageLoading)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));
            this.IsPageLoading = true;

            try
            {
                var parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    throw new MessageOnlyException("Scanning a global location identifier bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product identifier bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                {
                    throw new MessageOnlyException("Scanning a line item bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                {
                    throw new MessageOnlyException("Scanning a line item detail bar code is not supported here.");
                }
                else if (!string.IsNullOrEmpty(parse.SSCC18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here.");
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.
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
            catch (GatherProductDataException prodDataEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, prodDataEx);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (IsReadyToApplyException isReadyEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, isReadyEx);
                    _ = await errRpt.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            this.IsPageLoading = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplaySummary()
        {
            this.ExtraInfoItems.Clear();

            if (this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                this.ExtraInfoItems.Add("Batch printing enabled.");
            }
            else
            {
                this.ExtraInfoItems.Add("Print labels on scan.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
        }

        /// <summary>
        /// Creates an adjustment for the product.
        /// </summary>
        /// <remarks>
        /// This sends what we wish to be an activity detail to the
        /// COM server. The COM server sees it doesn't exist and creates it,
        /// then it sends back three fk_refnos for activity, activity item, and activity detail.
        /// Once received it will then need to update the table values with the fk_refnos.
        /// </remarks>
        /// <param name="data">Data used for the row to be created.</param>
        private async Task CreateAdjustment(XDocument data)
        {
            try
            {
                // Create adjustment
                var request = await this.ActDetail.CreateSubmitXml(data);
                var response = SysCommon.SubmitComm(request);

                // Parse response for the updated fk_refnos
                // Modify and change the ActivityDetail/Item/Header to be up to date with the response fk_refnos
                var responseData = XDocument.Parse(response);
                this.ActDetail.ActItem.ActHdr.HdrFkRefno = Convert.ToInt32(responseData.Root.Element("fk_refno").Value);
                this.ActDetail.ActItem.ItemFkRefno = Convert.ToInt32(responseData.Root.Element("fkit_refno").Value);
                this.ActDetail.DetailFkRefno = Convert.ToInt32(responseData.Root.Element("ser_refno").Value);

                data.Root.Add(new XElement("hdr_refno", this.ActDetail.ActItem.ActHdr.HdrFkRefno));
                data.Root.Add(new XElement("itm_refno", this.ActDetail.ActItem.ItemFkRefno));
                data.Root.Element("ser_refno").SetValue(this.ActDetail.DetailFkRefno);

                var detail = this.ActDetail.InsertListDetail(this.MainActivityItems);
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

        /// <summary>
        /// Removes the adjustment from happening.
        /// </summary>
        /// <remarks>
        /// Sends the fk_refno of the detail to remove to the COM server.
        /// </remarks>
        private async Task DeleteAdjustment()
        {
            if (this.NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                var data = new XDocument();

                // TODO send the fk_refno to delete over
                var sendRequest = await this.ActDetail.CreateDeleteXml(data);
                _ = SysCommon.SubmitComm(sendRequest);

                // Remove from grid
                var selectedIndex = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);

                this.ActDetail.ListDetails.RemoveAt(selectedIndex);
                this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }

                if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > selectedIndex)
                {
                    this.ListViewMainActivity.SelectedItem = this.MainActivityItems[selectedIndex];
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

        /// <summary>
        /// Prints all product labels if batch.
        /// </summary>
        private async Task PrintBatchProductLabels()
        {
            if (!this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
            {
                return;
            }

            foreach (var obj in this.ActDetail.ListDetails)
            {
                // Set the detail, item, header, product, scheme, scheme data info
                await this.ActDetail.AssignActivityDetail(obj);

                // Print using that data
                await this.PrintProductLabel();
            }
        }

        /// <summary>
        /// Causes all current adjustments to print product labels.
        /// </summary>
        /// <remarks>
        /// Sends an xml string to the COM server to print the product labels.
        /// </remarks>
        private async Task PrintProductLabel()
        {
            if (this.NoData())
            {
                return;
            }

            try
            {
                // Create COM request to remove
                var data = new XDocument();
                var sendRequest = await this.ActDetail.CreateSubmitProductLabelXml(data);
                _ = SysCommon.SubmitComm(sendRequest);
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

        /// <summary>
        /// Checks if the Activity Detail has 0 data rows.
        /// </summary>
        /// <returns>True if there are no data rows.</returns>
        private bool NoData()
        {
            return this.ActDetail.ListDetails.Count == 0;
        }

        /// <summary>
        /// Check if there is any activity detail exists.
        /// </summary>
        /// <returns><see cref="bool"/> True if activity detail exists.</returns>
        private bool HasData()
        {
            return !this.NoData();
        }

        /// <summary>
        /// Saves or cancels changes and exits.
        /// </summary>
        private async Task CloseForm()
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.None;
                _ = await this.CancelChanges();
            }
        }

        /// <summary>
        /// Click handler for the Add button.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                await this.AddManually();
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
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
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.ucActivityConvertProductDetail_GotFocus threw an error " + ex,
                    "OK"));
            }
        }

        private void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.SelectRow(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem));
        }

        private void SelectRow(int row)
        {
            this.ActDetail.SelectedListIdx = row;
            if (row < 0)
            {
                return;
            }

            this.MainActivityItems[row] = this.ActDetail.ListDetails[row];
            this.ActDetail.SelectedDetail = this.MainActivityItems[row];
            this.ListViewMainActivity.SelectedItem = this.ActDetail.SelectedDetail;

            Device.BeginInvokeOnMainThread(() =>
            {
                this.ListViewMainActivity.ScrollTo(
                    this.ActDetail.SelectedDetail,
                    ScrollToPosition.MakeVisible,
                    false);
            });


            this.DisplaySummary();

        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.MainActivityItems = this.ActDetail.Search(e.NewTextValue);
            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
        }
    }
}