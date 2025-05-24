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
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using Forms;
    using Resources.Enums;
    using Resources.Types.Items;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Activity convert product detail.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertProductDetail : IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityConvertProductDetail" /> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Convert product activity detail.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity sub types.</param>
        public ActivityConvertProductDetail(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.CmdAdd.Text = "Add";
            this.SysEnv = SysEnvironment.GetInstance();
            this.Title = this.GetControlDescription();

            this.ActSubType = actSubTypes;
            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.LoadAndBind().FireAndForgetSafeAsync();
        }

        private int FromProdRefno { get; set; }

        private int ToProdRefno { get; set; }

        private ProductInfo ToProduct { get; set; }

        private ProductInfo FromProduct { get; set; }

        private SysEnvironment SysEnv { get; }

        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        private SysCommon.ActSubTypes ActSubType { get; }

        private bool IsDataLoading { get; set; }

        private bool IsChangesMade { get; set; }

        private ActDetail ActDetail { get; }

        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

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
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        /// <summary>
        ///     Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool" /> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            const bool isReadyToClose = true;
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

            return isReadyToClose;
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
        ///     Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
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
        ///     Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var unqScheme = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var scheme = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
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
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandlerCallback);

            async void KeyUpHandlerCallback(string keyPressed)
            {
                try
                {
                    await this.ScannerDisable();
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (!this.IsIndicatorRunning)
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
        }

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformBackPress);
            return true;

            async void PerformBackPress()
            {
                _ = await this.CancelChanges();
                await this.ScannerDisable();
                _ = base.OnBackButtonPressed();
                await this.Close();
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async Task LoadAndBind()
        {
            try
            {
                this.IsDataLoading = true;
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
                this.IsDataLoading = false;
            }
        }

        private async Task AddManually()
        {
            var form = new FormDialogContainer(
                EDialogTypes.ActivityDetailEdit,
                this.ActDetail,
                SysCommon.ActSubTypes.ConvertProduct);
            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                var xmlDoc = await form.GetXml();
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

        private string GetControlDescription()
        {
            return "Convert Products";
        }

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

                this.IsDataLoading = true;
                var parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    throw new MessageOnlyException(
                        "Scanning a global location identifier bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product identifier bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(parse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning a line item bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(parse.ActDetailFkRefno))
                {
                    throw new MessageOnlyException("Scanning a line item detail bar code is not supported here.");
                }

                if (!string.IsNullOrEmpty(parse.Sscc18))
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
            finally
            {
                await this.ScannerEnable();
                this.IsDataLoading = false;
            }
        }

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

        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            this.IsChangesMade = true;
        }

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

                var detail = this.ActDetail.GatherDetail();
                this.ActDetail.ListDetails.Add(detail);
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
                this.MainActivityItems =
                    this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
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

        private bool NoData()
        {
            return this.ActDetail.ListDetails.Count == 0;
        }

        private bool HasData()
        {
            return !this.NoData();
        }

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