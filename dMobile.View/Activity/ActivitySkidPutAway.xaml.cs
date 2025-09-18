// <copyright file="ActivitySkidPutAway.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
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

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivitySkidPutAway : DMobileContentPage
    {
        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivitySkidPutAway" /> class.
        /// </summary>
        public ActivitySkidPutAway()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.cmdLocation.Text = "Set To Location";
            this.cmdAddProduct.Text = "Add";
            this.cmdSelectSkid.Text = "Select Skid";

            this.cmdLocation.IsEnabled = false;
            this.cmdAddProduct.IsEnabled = false;

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.MainActivityItems = new ObservableCollection<object>();
                this.ExtraInfoItems = new ObservableCollection<string>();
                this.SysEnv = SysEnvironment.GetInstance();
                this.LocMap = LocationMap.GetInstance();
                this.OrigProdInfoOnSkid = new ProductInfo();

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

                this.ActSubType = SysCommon.ActSubTypes.PutAwaySkid;
                this.Scanner = ScannerEngine.GetInstance().Scanner;
                this.Activity = ActivityFacade.GetActivity(this.ActSubType);
                this.DataCollector = new DataCollector(this.ActSubType);
                this.Title = this.GetControlDescription();
            }
        }

        private ProductInfo OrigProdInfoOnSkid { get; set; }

        private BarCodeParse BarCodeParse { get; set; }

        private BarCodeScanner Scanner { get; }

        private ActDetail ActDetail { get; set; }

        private Activity Activity { get; }

        private ActItem ActItem { get; set; }

        private SysEnvironment SysEnv { get; }

        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or Sets the singleton instance of location map.
        /// </summary>
        private LocationMap LocMap { get; }

        private SysCommon.ActSubTypes ActSubType { get; }

        private ActDetail ActDetailOnSkid { get; set; }

        private bool IsControlLoaded { get; set; }

        public ObservableCollection<string> ExtraInfoItems { get; set; }

        public ObservableCollection<object> MainActivityItems { get; set; }

        private DataCollector DataCollector { get; set; } = null;

        /// <summary>
        ///     The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();

                    MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async keyPressed =>
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.IsIndicatorRunning)
                        {
                            if (keycode == Keycode.F1)
                            {
                                if (await this.SaveChanges())
                                {
                                    await this.Close();
                                }
                            }
                            else if (keycode == Keycode.Del || keycode == Keycode.Clear)
                            {
                                if (this.MainActivityItems.Count > 0 && this.ListViewMainActivity.SelectedItem != null)
                                {
                                    if (await Device.InvokeOnMainThreadAsync(async () =>
                                            await this.DisplayAlert("Put Away", "Do you wish to delete this record?",
                                                "Yes",
                                                "No")))
                                    {
                                        _ = this.ActDetail.ListDetails.Remove(this.ListViewMainActivity.SelectedItem);

                                        // Delete needs to rebind the the grid to the proper data view depending on the record count
                                        await Device.InvokeOnMainThreadAsync(() =>
                                        {
                                            this.MainActivityItems = this.ActDetailOnSkid.DesignGrid(
                                                ref this.GridMainActivity,
                                                ref this.ListViewMainActivity);
                                            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                                        });
                                    }
                                }
                            }
                        }
                    });

                    await this.ClearContents();
                    await this.ucActivitySkidPutAway_GotFocus(this, EventArgs.Empty);
                }
                catch (CommunicationException commEx)
                {
                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load skid info.{Environment.NewLine}" +
                                  $"Because of a communication problem.{Environment.NewLine}" +
                                  $"Detail:{commEx.Message}{Environment.NewLine}" +
                                  "Please exit and try again.";

                        await Application.Current.MainPage.DisplayAlert(
                            this.Activity.GetControlDescription(),
                            msg,
                            Resource.OK);
                    });
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load skid info.{Environment.NewLine}" +
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
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <inheritdoc />
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

        /// <summary>
        ///     Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.ClearBarCodeScanEventHandlers();
                await this.Scanner.DisableScanning();
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
                await this.Scanner.EnableScanning();
                if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
                {
                    this.Scanner.OnBarCodeScan += this.BarCodeHandler;
                }

                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdSelectSkid_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                var form = new FormDialogContainer(EDialogTypes.SkidInfo, this.Activity, this.ActSubType);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXml();
                    await this.ClearContents();

                    var tmpSkidInfo = await SysSkidInfo.GetSkidInfoFromXml(xmlDoc);

                    if (tmpSkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        this.Activity.SkidInfo = tmpSkidInfo;
                        this.cmdAddProduct.IsEnabled = true;

                        this.cmdSelectSkid.Text = "Skid# " + this.Activity.SkidInfo.CurrSkid.SkidCode;

                        this.ExtraInfoItems.Clear();
                        this.ExtraInfoItems.Add("Skid Product: " +
                                                ProductInfoTasks.GetCurrReadableProdBcUnit(this.Activity.SkidInfo
                                                    .CurrSkid.ProdInfo));
                        this.ExtraInfoItems.Add("Skid Location: " +
                                                LocationMapTasks.GetReadableLocation(this.Activity.SkidInfo.CurrSkid
                                                    .LocMap));
                        this.cmdLocation.IsEnabled = true;

                        // Set the global location to the skid location,
                        // need to enforce that the skid and location warehouses match.
                        this.LocMap.CurrLocMap = this.Activity.SkidInfo.CurrSkid.LocMap;
                        this.cmdLocation.Text =
                            LocationMapTasks.GetReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap);

                        this.Activity.HdrFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                        this.Activity.HdrRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;

                        this.ActItem.ItemFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                        this.ActItem.ItemRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                        this.ActItem.LocMap = this.LocMap.CurrLocMap;
                        this.ActItem.ProdInfo = this.Activity.SkidInfo.CurrSkid.ProdInfo;
                        this.ActItem.ProdInfo.Unit2BaseUnitFactor =
                            await ProductInfoTasks.GetUnitRelationFactor(this.ActItem.ProdInfo);

                        this.ActDetailOnSkid.SelectedLocMap = this.ActItem.LocMap;
                        var tempSchemeData = this.ActDetailOnSkid.SchemeData;
                        tempSchemeData.LogisticUnitSkidNumber =
                            this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        this.ActDetailOnSkid.SchemeData = tempSchemeData;
                        this.OrigProdInfoOnSkid = this.ActDetailOnSkid.ActItem.ProdInfo;

                        await this.ActDetailOnSkid.Refresh();
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ucActivitySkidPutAway_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();

            this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            if (this.IsControlLoaded == false)
            {
                this.IsControlLoaded = true;

                if (this.IsIndicatorRunning)
                {
                    this.IsIndicatorRunning = false;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    this.IsIndicatorRunning = true;
                    await this.ScannerDisable();
                    this.ActItem.ProdInfo = this.OrigProdInfoOnSkid;

                    if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert("Select UOM", "Select a different unit of measure?", "Yes", "No")))
                    {
                        var form = new FormDialogContainer(
                            EDialogTypes.SelectProductUnitFilterToProduct,
                            this.ActItem,
                            this.ActSubType);

                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                await form.DisplayControl.ShowDialog() == DialogResult.OK))
                        {
                            var xmlDoc = await form.GetXml();
                            var prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                            if (prodUnitRefno != 0)
                            {
                                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            }
                        }
                    }

                    // Clear the quantities and scheme components, defaulting dates.
                    this.ActDetail.ClearProperties();
                    var fillQty = SysCommon.FillQuantityStructureFromQuantity(
                        this.ActItem.ProdInfo,
                        this.ActDetailOnSkid.Quantity);

                    this.ActDetail.Quantity = fillQty.Quantity;
                    this.ActDetail.BaseQuantity = fillQty.BaseQuantity;
                    this.ActDetail.WeightQuantity = fillQty.WeightQuantity;
                    this.ActDetail.SchemeId = this.ActDetailOnSkid.SchemeId;
                    this.ActDetail.UniqueSchemeId = this.ActDetailOnSkid.UniqueSchemeId;
                    this.ActDetail.SchemeData = this.ActDetailOnSkid.SchemeData;
                    this.ActDetail.ActExtra.Text1 = string.Empty;
                    this.ActDetail.ActExtra.Text2 = string.Empty;
                    this.ActDetail.ActExtra.Text3 = string.Empty;
                    this.ActDetail.ActExtra.Text4 = string.Empty;
                    this.ActDetail.ActExtra.DateTime1 = new DateTime(1900, 1, 1);

                    await this.InsertActivityDetail(this.ActDetail);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert("Error",
                    "An error occurred while adding a product." + Environment.NewLine + ex.Message, "OK"));
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public async void cmdLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                var form = new FormDialogContainer(EDialogTypes.SelectLocationMap,
                    this.ActItem.ActHdr.SkidInfo.CurrSkid.LocMap.WhsLocMapRefno, true);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());

                        if (this.ActDetail.ListDetails.Count > 0)
                        {
                            for (var i = 0; i <= this.ActDetail.ListDetails.Count - 1; i++)
                            {
                                (this.ActDetail.ListDetails[i] as dynamic).LocationMapRefno =
                                    this.LocMap.CurrLocMapRefno;

                                (this.ActDetail.ListDetails[i] as dynamic).ActivityDetailSchemeID =
                                    ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                                        this.ActDetailOnSkid.ActItem.ProdInfo.Code,
                                        this.ActDetailOnSkid.ActItem.ProdInfo.Scheme,
                                        this.ActDetailOnSkid.SchemeData);

                                (this.ActDetail.ListDetails[i] as dynamic).ActivityDetailUniqueSchemeID =
                                    ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                                        this.ActDetailOnSkid.ActItem.ProdInfo.Code,
                                        this.ActDetailOnSkid.ActItem.ProdInfo.Scheme,
                                        this.ActDetailOnSkid.SchemeData);
                            }
                        }
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
                    }
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert("Error", "Error setting location." + Environment.NewLine + ex.Message,
                        "OK"));
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public Task<XDocument> GetXML()
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return Task.FromResult(xmldocParams);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
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
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                    // Clear any properties.
                    if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno != 0)
                    {
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert("Skid Put Away", "Do you wish to put away another skid?", "Yes",
                                    "No")))
                        {
                            await this.ClearContents();
                            cancelChanges = false;
                        }
                    }
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
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Skid Put Away";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                if (isReadyToSave)
                {
                    if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                if (isReadyToSave)
                {
                    if (this.ActDetailOnSkid.ListDetails.Count == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                if (isReadyToSave)
                {
                    if (this.ActDetail.ListDetails.Count == 0)
                    {
                        isReadyToSave = false;
                    }
                }

                // Check number of unts match, ignoring weight in comparison
                // Raise a warning if there is a mismatch
                if (isReadyToSave)
                {
                    if (this.ActItem.ProdInfo.UnitCode != "LB" && this.ActItem.ProdInfo.UnitCode != "KG")
                    {
                        decimal skidTotQty = 0;

                        foreach (dynamic detail in this.ActDetailOnSkid.ListDetails)
                        {
                            skidTotQty += Convert.ToDecimal(detail.ActivityDetailQuantity);
                        }

                        decimal putAwaySkidTotQty = 0;

                        foreach (dynamic detail in this.ActDetail.ListDetails)
                        {
                            putAwaySkidTotQty += Convert.ToDecimal(detail.ActivityDetailQuantity);
                        }

                        if (skidTotQty != putAwaySkidTotQty)
                        {
                            var msg = string.Format(
                                "Total quantity of {0} {2} to be put away does not match the current skid quantity of {1} {2}. Do you wish to continue?",
                                putAwaySkidTotQty, skidTotQty, this.ActItem.ProdInfo.UnitCode);

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                    !await this.DisplayAlert("Skid Put Away", msg, "Yes", "No")))
                            {
                                isReadyToSave = false;
                            }
                        }
                    }
                }
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    await Task.Run(() =>
                    {
                        // Explicit GC, Xamarin forms not disposing of objects properly
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    });

                    var request = await this.MakeSkidPutAwaySubmitString();
                    var responseData = SysCommon.SubmitComm(request);
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseData);
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        "Skid Put Away",
                        "Skid put away complete!",
                        "OK"));
                    await this.ClearContents();
                    saveComplete = true;

                    if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            "Skid Put Away",
                            "Do you wish to put away any more skids?",
                            "Yes",
                            "No")))
                    {
                        saveComplete = false;
                        await this.ScannerEnable();
                    }
                }

                // await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Skid Put Away", "Data Failed Validation. Check Your Data.", "OK"));
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        public async Task ClearContents()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.ExtraInfoItems.Clear();
                this.cmdAddProduct.IsEnabled = false;
                this.cmdLocation.IsEnabled = false;

                this.cmdSelectSkid.Text = "Select Skid";
                this.cmdLocation.Text = "Set Location";
            });

            this.Activity.SkidInfo = new SysSkidInfo();
            this.Activity.HdrFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
            this.Activity.HdrRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;

            this.ActItem = ActItemFacade.GetActItem(this.Activity, this.ActSubType);
            this.ActItem.ItemFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
            this.ActItem.ItemRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;

            this.ActDetailOnSkid = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.SkidInfo);
            this.ActDetailOnSkid.ActItem = this.ActItem;
            this.ActDetailOnSkid.ListDetails = await this.ActDetailOnSkid.GetActivityDetailList(false);

            this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
            this.ActDetail.ListDetails = await this.ActDetail.GetActivityDetailList(false);
            this.ActDetail.ActItem = this.ActItem;

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.MainActivityItems =
                    this.ActDetailOnSkid.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            });
        }

        // TODO: Check if this is called
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="idx"></param>
        ///// <returns></returns>
        // public string MakeProductLabelsSubmitString(int idx)
        // {
        //    string production = string.Empty;
        //    string bestBefore = string.Empty;
        //    ProductInfo prodInfo = new ProductInfo();
        //    DataView detail = this.ActDetail.DataViewDetails;
        //    StringBuilder xmlString = new StringBuilder();
        //    XmlWriter xml = XmlWriter.Create(xmlString);

        // prodInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)detail[idx]["ProductUnitRefno"]);
        //    prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

        // if (prodInfo.Scheme.IsBestBeforeDateTracked)
        //    {
        //        bestBefore = Convert.ToDateTime(detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
        //    }

        // if (prodInfo.Scheme.IsProductionDateTracked)
        //    {
        //        production = Convert.ToDateTime(
        //            detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
        //    }

        // XDocument request = XDocument.Parse(
        //        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
        //        + "<imclient>"
        //        + "<command>"
        //        + "<action>SUBMIT PRINT PRODUCT LABEL</action>"
        //        + "<whs_id>0</whs_id>"
        //        + "<sect_refno>0</sect_refno>"
        //        + "</command>"
        //        + await SysCommon.GetRequestDefaults(false)
        //        + ("<prod_id>" + prodInfo.Code + "</prod_id>")
        //        + ("<is_bc>" + prodInfo.UnitIsBc + "</is_bc>")
        //        + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
        //        + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
        //        + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
        //        + ("<refno>" + this.ActItem.ActHdr.FkRefno + "</refno>")
        //        + ("<itm_refno>" + this.ActItem.FkRefno + "</itm_refno>")
        //        + ("<pono>" + this.ActItem.ActHdr.PONO + "</pono>")
        //        + ("<alt_qty>" + detail[idx]["ActivityDetailQuantity"] + "</alt_qty>")
        //        + ("<alt_unit>" + detail[idx]["UnitCode"] + "</alt_unit>")
        //        + ("<base_qty>" + detail[idx]["ActivityDetailBaseQuantity"] + "</base_qty>")
        //        + ("<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>")
        //        + ("<wt_qty>" + detail[idx]["ActivityDetailWeightQuantity"] + "</wt_qty>")
        //        + ("<wt_unit>" + prodInfo.WeightUnitCode + "</wt_unit>")
        //        + ("<serial>" + detail[idx]["ActivityDetailSingleTradeItemSerialNumber"] + "</serial>")
        //        + ("<lot_no>" + detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"] + "</lot_no>")
        //        + ("<pdate>" + production + "</pdate>")
        //        + ("<bdate>" + bestBefore + "</bdate>")
        //        + ("<variant_no>" + detail[idx]["ProductSchemeVariantNo"] + "</variant_no>")
        //        + ("<variant_code>" + detail[idx]["ProductSchemeVariantCode"] + "</variant_code>")
        //        + "</imclient>");

        // request.Save(xml);
        //    await xml.FlushAsync();
        //    xml.Close();

        // return xmlString.ToString();
        // }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<string> MakeSkidPutAwaySubmitString()
        {
            var production = string.Empty;
            var bestBefore = string.Empty;

            var skidDetail = this.ActDetailOnSkid.ListDetails;
            var request = new StringBuilder();
            var currSkid = this.Activity.SkidInfo.CurrSkid;
            var detail = this.ActDetail.ListDetails;

            _ = request.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            _ = request.Append("<imclient>");
            _ = request.Append("<command><action>SUBMIT PUT AWAY SKID</action></command>");
            var reqDef = await SysCommon.GetRequestDefaults(false);
            _ = request.Append(reqDef);

            for (var idx = 0; idx <= skidDetail.Count - 1; idx++)
            {
                if (currSkid.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    bestBefore = Convert
                        .ToDateTime(skidDetail[idx].GetType()
                            .GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")?.GetValue(skidDetail[idx]))
                        .ToString("MM/dd/yyyy");
                }

                if (currSkid.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    production = Convert
                        .ToDateTime(skidDetail[idx].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")
                            ?.GetValue(skidDetail[idx])).ToString("MM/dd/yyyy");
                }

                _ = request.Append("<from>");
                _ = request.AppendFormat("<whs_id>{0}</whs_id>", currSkid.LocMap.WhsFkRefno);
                _ = request.AppendFormat("<sect_refno>{0}</sect_refno>", currSkid.LocMap.SectFkRefno);
                _ = request.AppendFormat("<slot_refno>{0}</slot_refno>", currSkid.LocMap.SlotFkRefno);
                _ = request.AppendFormat("<refno></refno>", this.ActItem.ActHdr.HdrFkRefno);
                _ = request.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFkRefno);
                _ = request.AppendFormat("<prod_id>{0}</prod_id>",
                    skidDetail[idx].GetType().GetProperty("ProductCode")?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<bc>{0}</bc>",
                    skidDetail[idx].GetType().GetProperty("ProductUnitBC")?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<alt_unit>{0}</alt_unit>",
                    skidDetail[idx].GetType().GetProperty("UnitCode")?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<wt_unit>{0}</wt_unit>", currSkid.ProdInfo.WeightUnitCode);
                _ = request.AppendFormat("<serial>{0}</serial>",
                    skidDetail[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")
                        ?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<pdate>{0}</pdate>", production);
                _ = request.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                _ = request.AppendFormat("<lot_no>{0}</lot_no>",
                    skidDetail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")
                        ?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<variant_code>{0}</variant_code>",
                    skidDetail[idx].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<variant_no>{0}</variant_no>",
                    skidDetail[idx].GetType().GetProperty("ProductSchemeVariantNo")?.GetValue(skidDetail[idx]));
                _ = request.AppendFormat("<alt_qty>{0}</alt_qty>",
                    Convert.ToDecimal(skidDetail[idx].GetType().GetProperty("ActivityDetailQuantity")
                        ?.GetValue(skidDetail[idx])) * -1);
                _ = request.AppendFormat("<wt_qty>{0}</wt_qty>",
                    Convert.ToDecimal(skidDetail[idx].GetType().GetProperty("ActivityDetailWeightQuantity")
                        ?.GetValue(skidDetail[idx])) * -1);
                _ = request.AppendFormat("<base_qty>{0}</base_qty>",
                    Convert.ToDecimal(skidDetail[idx].GetType().GetProperty("ActivityDetailBaseQuantity")
                        ?.GetValue(skidDetail[idx])) * -1);
                _ = request.AppendFormat("<skid_fk_refno>{0}</skid_fk_refno>", currSkid.SkidFkRefno);
                _ = request.AppendFormat("<extra1>{0}</extra1>", this.ActDetailOnSkid.ActExtra.Text1);
                _ = request.AppendFormat("<extra2>{0}</extra2>", this.ActDetailOnSkid.ActExtra.Text2);
                _ = request.AppendFormat("<extra3>{0}</extra3>", this.ActDetailOnSkid.ActExtra.Text3);
                _ = request.AppendFormat("<extra4>{0}</extra4>", this.ActDetailOnSkid.ActExtra.Text4);
                _ = request.AppendFormat("<extra5>{0}</extra5>", this.ActDetailOnSkid.ActExtra.DateTime1);
                _ = request.Append("</from>");
            }

            for (var idx = 0; idx <= detail.Count - 1; idx++)
            {
                if (currSkid.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    bestBefore = Convert
                        .ToDateTime(detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")
                            .GetValue(detail[idx])).ToString("MM/dd/yyyy");
                }

                if (currSkid.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    production = Convert
                        .ToDateTime(detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")
                            .GetValue(detail[idx])).ToString("MM/dd/yyyy");
                }

                _ = request.Append("<to>");
                _ = request.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFkRefno);
                _ = request.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFkRefno);
                _ = request.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFkRefno);
                _ = request.AppendFormat("<refno></refno>", this.ActItem.ActHdr.HdrFkRefno);
                _ = request.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFkRefno);
                _ = request.AppendFormat("<prod_id>{0}</prod_id>",
                    detail[idx].GetType().GetProperty("ProductCode").GetValue(detail[idx]));
                _ = request.AppendFormat("<bc>{0}</bc>",
                    detail[idx].GetType().GetProperty("ProductUnitBC").GetValue(detail[idx]));
                _ = request.AppendFormat("<alt_unit>{0}</alt_unit>",
                    detail[idx].GetType().GetProperty("UnitCode").GetValue(detail[idx]));
                _ = request.AppendFormat("<wt_unit>{0}</wt_unit>", currSkid.ProdInfo.WeightUnitCode);
                _ = request.AppendFormat("<serial>{0}</serial>",
                    detail[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")
                        .GetValue(detail[idx]));
                _ = request.AppendFormat("<pdate>{0}</pdate>", production);
                _ = request.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                _ = request.AppendFormat("<lot_no>{0}</lot_no>",
                    detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")
                        .GetValue(detail[idx]));
                _ = request.AppendFormat("<variant_code>{0}</variant_code>",
                    detail[idx].GetType().GetProperty("ProductSchemeVariantCode").GetValue(detail[idx]));
                _ = request.AppendFormat("<variant_no>{0}</variant_no>",
                    detail[idx].GetType().GetProperty("ProductSchemeVariantNo").GetValue(detail[idx]));
                _ = request.AppendFormat("<alt_qty>{0}</alt_qty>",
                    detail[idx].GetType().GetProperty("ActivityDetailQuantity").GetValue(detail[idx]));
                _ = request.AppendFormat("<wt_qty>{0}</wt_qty>",
                    detail[idx].GetType().GetProperty("ActivityDetailWeightQuantity").GetValue(detail[idx]));
                _ = request.AppendFormat("<base_qty>{0}</base_qty>",
                    detail[idx].GetType().GetProperty("ActivityDetailBaseQuantity").GetValue(detail[idx]));
                _ = request.AppendFormat("<skid_fk_refno>{0}</skid_fk_refno>", currSkid.SkidFkRefno);
                _ = request.AppendFormat("<extra1>{0}</extra1>", this.ActDetail.ActExtra.Text1);
                _ = request.AppendFormat("<extra2>{0}</extra2>", this.ActDetail.ActExtra.Text2);
                _ = request.AppendFormat("<extra3>{0}</extra3>", this.ActDetail.ActExtra.Text3);
                _ = request.AppendFormat("<extra4>{0}</extra4>", this.ActDetail.ActExtra.Text4);
                _ = request.AppendFormat("<extra5>{0}</extra5>", this.ActDetail.ActExtra.DateTime1);
                _ = request.Append("</to>");
            }

            _ = request.Append("</imclient>");

            return request.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="actDetail"></param>
        /// <remarks></remarks>
        private async Task InsertActivityDetail(ActDetail actDetail)
        {
            XDocument xmlDoc = null;
            var schemeData = actDetail.SchemeData;

            xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<serial>" + actDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<pdate>" + actDetail.SchemeData.LogisticUnitProductionDate + "</pdate>"
                + "<lot_no>" + actDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<bdate>" + actDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                + "<variant_code>" + actDetail.SchemeData.VariantCode + "</variant_code>"
                + "<variant_no>" + actDetail.SchemeData.VariantNumber + "</variant_no>"
                + "<alt_qty>" + actDetail.Quantity + "</alt_qty>"
                + "<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + actDetail.BaseQuantity + "</base_qty>"
                + "</params>");

            var isInsertDetailReady = false;
            var isDuplicateSchemeId = false;

            // Check for missing tracking components, if any missing invoke the edit detail form
            schemeData = actDetail.SchemeData;
            schemeData.LogisticUnitSkidNumber = this.Activity.SkidInfo.CurrSkid.SkidCode;

            // Check for duplicate occurs in form
            var form = new FormDialogContainer(EDialogTypes.ActivityDetailEdit, actDetail, this.ActSubType);

            if (await Device.InvokeOnMainThreadAsync(async () =>
                    await form.DisplayControl.ShowDialog() == DialogResult.OK))
            {
                xmlDoc = await form.GetXml();
                actDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
                actDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
                actDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);
                schemeData.SingleTradeItemSerialNumber = xmlDoc.Root.Element("serial").Value;
                schemeData.LogisticUnitProductionDate = DateTime.Parse(xmlDoc.Root.Element("pdate").Value);
                schemeData.LogisticUnitBatchLotNumber = xmlDoc.Root.Element("lot_no").Value;
                schemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(xmlDoc.Root.Element("bdate").Value);
                schemeData.VariantCode = xmlDoc.Root.Element("variant_code").Value;
                schemeData.VariantNumber = int.Parse(xmlDoc.Root.Element("variant_no").Value);

                actDetail.ActExtra.Text1 = xmlDoc.Root.Element("extra1").Value;
                actDetail.ActExtra.Text2 = xmlDoc.Root.Element("extra2").Value;
                actDetail.ActExtra.Text3 = xmlDoc.Root.Element("extra3").Value;
                actDetail.ActExtra.Text4 = xmlDoc.Root.Element("extra4").Value;
                actDetail.ActExtra.DateTime1 = DateTime.Parse(xmlDoc.Root.Element("extra5").Value);

                isInsertDetailReady = true;
            }

            actDetail.SchemeData = schemeData;
            actDetail.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);
            actDetail.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme, actDetail.SchemeData);

            if (isInsertDetailReady)
            {
                if (!isDuplicateSchemeId)
                {
                    await actDetail.Insert(xmlDoc);

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        if (actDetail.ListDetails.Count > 0)
                        {
                            this.MainActivityItems = actDetail.DesignGrid(ref this.GridMainActivity,
                                ref this.ListViewMainActivity);
                            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                            this.ListViewMainActivity.SelectedItem =
                                this.MainActivityItems[actDetail.ListDetails.Count - 1];
                        }
                        else
                        {
                            this.MainActivityItems = this.ActDetailOnSkid.DesignGrid(ref this.GridMainActivity,
                                ref this.ListViewMainActivity);
                            this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            var prodUnitRefno = 0;
            var svdActDetailFKRefno = this.ActDetailOnSkid.DetailFkRefno;
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
                    throw new TaskCanceledException();
                }

                this.IsIndicatorRunning = true;
                this.BarCodeParse = scanArgs.ParseInfo;

                if (!string.IsNullOrEmpty(this.BarCodeParse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(this.BarCodeParse
                            .LocationIdentifier));

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() =>
                            this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                    }
                    else
                    {
                        _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set To Location");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.BarCodeParse.ProductIdentifier))
                    {
                        if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                        {
                            throw new MessageOnlyException("Location must be set first.");
                        }

                        if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno == 0)
                        {
                            throw new MessageOnlyException(
                                "A skid must be selected first before scanning any product detail.");
                        }

                        prodUnitRefno = await this.DataCollector.IdentifyProductScanned(this.BarCodeParse);
                        if (this.Activity.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            throw new MessageOnlyException(
                                "Identified scanned product does not match the selected skid product.");
                        }

                        await this.GatherProductData(scanArgs.ParseInfo);
                    }
                    else if (!string.IsNullOrEmpty(this.BarCodeParse.ActFkRefno))
                    {
                        throw new MessageOnlyException(
                            "Scanning an activity header reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarCodeParse.ActItemFkRefno))
                    {
                        throw new MessageOnlyException(
                            "Scanning an activity item reference # is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(this.BarCodeParse.Sscc18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        await this.ClearContents();

                        this.Activity.SkidInfo =
                            await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(scanArgs.ParseInfo.Sscc18);

                        if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno == 0)
                        {
                            throw new MessageOnlyException("Skid does not exist.");
                        }

                        if (this.Activity.SkidInfo.CurrSkid.SkidFkRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdAddProduct.IsEnabled = true);

                            _ = await Device.InvokeOnMainThreadAsync(() =>
                                this.cmdSelectSkid.Text = "Skid# " + this.Activity.SkidInfo.CurrSkid.SkidCode);

                            await Device.InvokeOnMainThreadAsync(() => this.ExtraInfoItems.Clear());

                            this.ExtraInfoItems.Add("Skid Product: " +
                                                    ProductInfoTasks.GetCurrReadableProdBcUnit(this.Activity
                                                        .SkidInfo.CurrSkid.ProdInfo));
                            this.ExtraInfoItems.Add("Skid Location: " +
                                                    LocationMapTasks.GetReadableLocation(this.Activity.SkidInfo
                                                        .CurrSkid.LocMap));

                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.IsEnabled = true);
                            _ = await Device.InvokeOnMainThreadAsync(() =>
                                this.cmdLocation.Text =
                                    LocationMapTasks.GetReadableLocation(this.Activity.SkidInfo.CurrSkid.LocMap));

                            this.Activity.HdrFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                            this.Activity.HdrRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;

                            this.ActItem.ItemFkRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                            this.ActItem.ItemRefno = this.Activity.SkidInfo.CurrSkid.SkidFkRefno;
                            this.ActItem.ProdInfo = this.Activity.SkidInfo.CurrSkid.ProdInfo;
                            this.ActItem.ProdInfo.Unit2BaseUnitFactor =
                                await ProductInfoTasks.GetUnitRelationFactor(this.ActItem.ProdInfo);

                            this.OrigProdInfoOnSkid = this.ActItem.ProdInfo;
                            var tempSchemeData = this.ActDetailOnSkid.SchemeData;
                            tempSchemeData.LogisticUnitSkidNumber =
                                this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            this.ActDetailOnSkid.SchemeData = tempSchemeData;

                            await this.ActDetailOnSkid.Refresh();
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectSkid.Text = "Select Skid");
                        }
                    }
                    else
                    {
                        throw new MessageOnlyException("Scanned bar code type is not supported here.");
                    }
                }
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

                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSelectSkid.Text =
                            "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectSkid.Text = "Select Skid");
                }

                this.ActDetailOnSkid.DetailFkRefno = svdActDetailFKRefno;
            }
            catch (IsReadyToApplyException ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSelectSkid.Text =
                            "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectSkid.Text = "Select Skid");
                }

                this.ActDetailOnSkid.DetailFkRefno = svdActDetailFKRefno;

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errorReport = new FormErrorReport(this.ActSubType, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
            }
            catch (GatherProductDataException ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSelectSkid.Text =
                            "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectSkid.Text = "Select Skid");
                }

                this.ActDetailOnSkid.DetailFkRefno = svdActDetailFKRefno;

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errorReport = new FormErrorReport(this.ActSubType, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                if (this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno != 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSelectSkid.Text =
                            "Skid# " + this.ActDetailOnSkid.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectSkid.Text = "Select Skid");
                }

                this.ActDetailOnSkid.DetailFkRefno = svdActDetailFKRefno;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        private async Task GatherProductData(BarCodeParse barParse)
        {
            var tempItemProdSchemeData = new ActItemSchemeData();
            var isInsertReady = await SysShared.GatherProductData(
                barParse,
                this.ActSubType,
                this.ActItem,
                tempItemProdSchemeData,
                this.ActDetail);

            if (isInsertReady)
            {
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + this.ActDetail.UniqueSchemeId +
                    "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await this.ActDetail.IsReadyToApply(xmlParam);

                var xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<serial>" + this.ActDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                    + "<pdate>" + this.ActDetail.SchemeData.LogisticUnitProductionDate + "</pdate>"
                    + "<lot_no>" + this.ActDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                    + "<bdate>" + this.ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                    + "<variant_code>" + this.ActDetail.SchemeData.VariantCode + "</variant_code>"
                    + "<variant_no>" + this.ActDetail.SchemeData.VariantNumber + "</variant_no>"
                    + "<alt_qty>" + this.ActDetail.Quantity + "</alt_qty>"
                    + "<wt_qty>" + this.ActDetail.WeightQuantity + "</wt_qty>"
                    + "<base_qty>" + this.ActDetail.BaseQuantity + "</base_qty>"
                    + "<is_batch_print_product_label>" + false + "</is_batch_print_product_label>"
                    + "</params>");

                await this.ActDetail.Insert(xmlDoc);

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.MainActivityItems =
                        this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.ListViewMainActivity);
                    this.ListViewMainActivity.ItemsSource = this.MainActivityItems;

                    if (this.ActDetail.ListDetails.Count > 0 && this.MainActivityItems.Count > 0)
                    {
                        this.ListViewMainActivity.SelectedItem = this.MainActivityItems.LastOrDefault();
                    }
                });
            }
        }
    }
}