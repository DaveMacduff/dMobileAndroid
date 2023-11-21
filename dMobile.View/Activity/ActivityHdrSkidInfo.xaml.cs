// <copyright file="ActivityHdrSkidInfo.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Item;
    using CustomPages.Modal;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using static Android.Views.View;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityHdrSkidInfo" /> Skid information maintenance header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrSkidInfo : DMobileModal, IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        public ActivityHdrSkidInfo()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            this.Title = this.GetControlDescription();
            this.CallingActSubType = SysCommon.ActSubTypes.SkidInfo;
            this.IsNewSkid = true;

            this.ParseAndSeedBarCodeModifier();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        /// <param name="callingActSubType"><see cref="SysCommon.ActSubTypes" /> Calling activity sub type.</param>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        public ActivityHdrSkidInfo(SysCommon.ActSubTypes callingActSubType, Activity activity)
        {
            this.InitializeComponent();

            // Default control titles.
            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            // Load control data.
            this.ParseAndSeedBarCodeModifier();

            // Init objects.
            this.CallingActSubType = callingActSubType;
            this.CallingActFKRefno = activity.HdrFkRefno;

            // Set default form flags.
            this.IsNewSkid = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        /// <param name="callingActSubType"><see cref="SysCommon.ActSubTypes" /> Calling activity sub type.</param>
        /// <param name="actItem"><see cref="ActItem" /> Activity item.</param>
        public ActivityHdrSkidInfo(SysCommon.ActSubTypes callingActSubType, ActItem actItem)
        {
            this.InitializeComponent();

            // Default control titles.
            this.IsIndicatorRunning = false;
            this.Title = this.GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            // Load control data.
            this.ParseAndSeedBarCodeModifier();

            // Init objects.
            this.ActItem = actItem;

            // Save the current location to restore it upon canceling.
            this.CallingActSubType = callingActSubType;
            this.CallingActFKRefno = actItem.ActHdr.HdrFkRefno;
            this.CallingActItemFKRefno = actItem.ItemFkRefno;

            this.IsNewSkid = true;
        }

        /// <summary>
        ///     Gets or sets the calling activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes CallingActSubType { get; } = SysCommon.ActSubTypes.SkidInfo;

        /// <summary>
        ///     Gets or sets a value indicating whether the skid code has changed.
        /// </summary>
        private bool IsSkidCodeChanged { get; set; }

        /// <summary>
        ///     Gets or sets the calling activity foreign key reference number.
        /// </summary>
        private int CallingActFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the calling activity item foreign key reference number.
        /// </summary>
        private int CallingActItemFKRefno { get; set; }

        /// <summary>
        ///     Gets or sets the bar code scanner.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        ///     Gets or sets a value indicating whether the skid is new.
        /// </summary>
        private bool IsNewSkid { get; set; } = true;

        /// <summary>
        ///     Gets or sets the saved skid information.
        /// </summary>
        private SysSkidInfo SavedSkidInfo { get; set; } = new SysSkidInfo();

        /// <summary>
        ///     Gets or sets the saved location map.
        /// </summary>
        private SLocMap LocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the saved location map reference number.
        /// </summary>
        private int LocMapRefno { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the product unit reference number.
        /// </summary>
        private int ProdUnitRefno { get; set; }

        /// <summary>
        ///     Gets or sets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets a value indicating whether the bar code handler event is registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets the skid information.
        /// </summary>
        private SysSkidInfo SkidInfo { get; set; } = new SysSkidInfo();

        /// <summary>
        ///     Gets or sets the items list.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

        /// <summary>
        ///     Gets or sets the main activity items list.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } =
            new ObservableCollection<object>();

        /// <summary>
        ///     Gets or sets the activity item.
        /// </summary>
        private ActItem ActItem { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether is first appearance.
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
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.BarCodeScan -= this.BarCodeHandler;
                    await this.Scanner.Stop();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += this.BarCodeHandler;
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        ///     Check if the form data is in a state that can be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if ready to save else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            if (!this.IsChangesMade)
            {
                return isReadyToSave;
            }

            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap))
            {
                throw new MessageOnlyException("The skid location must be set.");
            }

            if (this.ProdUnitRefno <= 0)
            {
                throw new MessageOnlyException("A product must be assigned to a skid.");
            }

            if (this.SysEnv.SysSettings.IsPickDisableMakeSkid && this.IsNewSkid &&
                (this.CallingActSubType == SysCommon.ActSubTypes.Picking ||
                 this.CallingActSubType == SysCommon.ActSubTypes.PickingSingle))
            {
                throw new MessageOnlyException("Making a new skid has been disabled.");
            }

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if the form can be closed.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form can be closed.</returns>
        public Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            if (this.IsChangesMade)
            {
                // Perform any checks or changes needed to allow a rollback or form close.
                // Set IsReadyToClose = false if checks fail.
            }

            if (isReadyToClose)
            {
                if (this.SkidInfo.Equals(this.SavedSkidInfo))
                {
                    this.IsChangesMade = false;
                }
            }

            return Task.FromResult(isReadyToClose);
        }

        /// <summary>
        ///     Cancel any pending form data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form changes where cancelled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                await this.ScannerDisable();

                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    await this.InitSkidInfo();
                    this.DisplayData();
                    this.IsChangesMade = false;

                    // Expect the control to close.
                }
                else
                {
                    cancelChanges = false;
                    this.IsChangesMade = false;
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
                    var errorReport = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity skid return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity skid return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var skidInfo = this.SkidInfo.CurrSkid;

            XDocument xmlDoc;
            if (skidInfo.SkidFKRefno == 0)
            {
                var dt = new DateTime(1900, 1, 1);
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno>0</skid_fk_refno>"
                    + "<chep>false</chep>"
                    + "<hold>false</hold>"
                    + "<base_quantity>0</base_quantity>"
                    + "<cube>0</cube>"
                    + "<height>0</height>"
                    + "<quantity>0</quantity>"
                    + "<weight_quantity>0</weight_quantity>"
                    + "<hi>0</hi>"
                    + "<ti>0</ti>"
                    + "<cube_unit></cube_unit>"
                    + "<height_unit></height_unit>"
                    + "<logistic_unit_skid_code></logistic_unit_skid_code>"
                    + "<skid_code></skid_code>"
                    + "<location_map_refno>0</location_map_refno>"
                    + "<product_unit_refno>0</product_unit_refno>"
                    + "<completed>false</completed>"
                    + "<consumed>false</consumed>"
                    + "<putaway>false</putaway>"
                    + "<logistic_unit_batch_lot_number></logistic_unit_batch_lot_number>"
                    + "<logistic_unit_best_before_sell_by_date>" + dt + "</logistic_unit_best_before_sell_by_date>"
                    + "<logistic_unit_production_date>" + dt + "</logistic_unit_production_date>"
                    + "<product_scheme_variant_no>0</product_scheme_variant_no>"
                    + "<product_scheme_variant_code></product_scheme_variant_code>"
                    + "<product_scheme_variant_description></product_scheme_variant_description>"
                    + "<act_sub_type>" + (int)this.CallingActSubType + "</act_sub_type>"
                    + "<act_fk_refno>0</act_fk_refno>"
                    + "<actitem_fk_refno>0</actitem_fk_refno>"
                    + "</params>");
            }
            else
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno>" + skidInfo.SkidFKRefno + "</skid_fk_refno>"
                    + "<chep>" + skidInfo.IsCHEP + "</chep>"
                    + "<hold>" + skidInfo.IsHold + "</hold>"
                    + "<base_quantity>" + skidInfo.BaseQuantity + "</base_quantity>"
                    + "<cube>" + skidInfo.Cube + "</cube>"
                    + "<height>" + skidInfo.Height + "</height>"
                    + "<quantity>" + skidInfo.Quantity + "</quantity>"
                    + "<weight_quantity>" + skidInfo.WeightQuantity + "</weight_quantity>"
                    + "<hi>" + skidInfo.Hi + "</hi>"
                    + "<ti>" + skidInfo.Ti + "</ti>"
                    + "<cube_unit>" + skidInfo.CubeUnit + "</cube_unit>"
                    + "<height_unit>" + skidInfo.HeightUnit + "</height_unit>"
                    + "<logistic_unit_skid_code>" + skidInfo.LogisticUnitSkidCode + "</logistic_unit_skid_code>"
                    + "<skid_code>" + skidInfo.SkidCode + "</skid_code>"
                    + "<location_map_refno>" + skidInfo.LocMapRefno + "</location_map_refno>"
                    + "<product_unit_refno>" + skidInfo.ProdInfo.UnitRefno + "</product_unit_refno>"
                    + "<completed>" + skidInfo.IsCompleted + "</completed>"
                    + "<consumed>" + skidInfo.IsConsumed + "</consumed>"
                    + "<putaway>" + skidInfo.IsPutAway + "</putaway>"
                    + "<logistic_unit_batch_lot_number>" + skidInfo.SchemeData.LogisticUnitBatchLotNumber
                    + "</logistic_unit_batch_lot_number>"
                    + "<logistic_unit_best_before_sell_by_date>" + skidInfo.SchemeData.LogisticUnitBestBeforeSellByDate
                    + "</logistic_unit_best_before_sell_by_date>"
                    + "<logistic_unit_production_date>" + skidInfo.SchemeData.LogisticUnitProductionDate
                    + "</logistic_unit_production_date>"
                    + "<product_scheme_variant_no>" + skidInfo.SchemeData.VariantNumber + "</product_scheme_variant_no>"
                    + "<product_scheme_variant_code>" + skidInfo.SchemeData.VariantCode +
                    "</product_scheme_variant_code>"
                    + "<product_scheme_variant_description>" + skidInfo.SchemeData.VariantDescription
                    + "</product_scheme_variant_description>"
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
                await this.ScannerDisable();
                if (await this.IsReadyToSave())
                {
                    this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().ChosenLocMap.Clone();
                    await this.SaveSkid();
                    this.DisplayData();
                    saveComplete = true;
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
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            return saveComplete;
        }

        /// <summary>
        ///     Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += this.BarCodeHandler;
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///     Get the activity description.
        /// </summary>
        /// <returns><see cref="string" /> Activity description.</returns>
        public string GetControlDescription()
        {
            return "Skid Info";
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
                    this.IsIndicatorRunning = true;
                    base.OnAppearing();
                    this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;

                    if (this.IsFirstAppearance)
                    {
                        this.IsFirstAppearance = false;
                        await this.ScannerStart();
                        SubscribeKeyUpHandlers();
                        await this.InitSkidInfo();
                    }

                    ////if (!string.IsNullOrEmpty(this.txtSkidCode.Text))
                    ////{
                    ////    await this.SetupNewSkidFromSkidCode();
                    ////}

                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);

                    await Device.InvokeOnMainThreadAsync(() => this.DisplayData());
                }
                catch (CommunicationException commEx)
                {
                    this.IsFirstAppearance = true;

                    // Question should a communication error, be reported as an error or just a messagebox?
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var msg = $"Unable to load skid info.{Environment.NewLine}" +
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
                    this.IsFirstAppearance = true;
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

                    if (!this.IsIndicatorRunning)
                    {
                        if (keycode == Keycode.F1)
                        {
                            if (await this.SaveChanges())
                            {
                                this.DialogResult = DialogResult.OK;
                                await this.Close();
                            }
                        }
                        else if (keycode == Keycode.F5)
                        {
                            try
                            {
                                await this.ScannerDisable();
                                if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                        this.GetControlDescription(),
                                        "Do you wish to break this skid?",
                                        "Yes",
                                        "No")))
                                {
                                    SysSkidInfo.Break(this.SkidInfo);

                                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                        this.GetControlDescription(),
                                        "This skid is now broken.",
                                        "OK"));

                                    await this.InitSkidInfo();
                                    this.IsNewSkid = true;
                                }
                            }
                            catch (MessageOnlyException msgEx)
                            {
                                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                    this.GetControlDescription(),
                                    msg,
                                    "OK"));
                            }
                            catch (Exception ex)
                            {
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    var errorReport = new FormErrorReport(
                                        SysCommon.ActSubTypes.SkidInfo,
                                        ex);
                                    _ = await errorReport.ShowDialog();
                                });
                            }
                            finally
                            {
                                await this.ScannerEnable();
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        ///     Initialize a new skid information.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task InitSkidInfo()
        {
            if (this.IsFirstAppearance)
            {
                this.IsFirstAppearance = false;
                await this.ScannerStart();

                if (this.ActItem == null)
                {
                    this.IsNewSkid = true;
                    this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    var tempSkidInfo = new SSkidInfo();
                    tempSkidInfo.ProdInfo = new ProductInfo();
                    tempSkidInfo.LocMap = this.LocMap.Clone();
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                }
                else
                {
                    // No active skid selected
                    if (this.ActItem.ActHdr.SkidInfo == null ||
                        this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        this.IsNewSkid = true;
                        this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                        var tempSkidInfo = new SSkidInfo();
                        tempSkidInfo.LocMap = this.LocMap.Clone();

                        if (this.ActItem.SelectedItem != null && this.ActItem.ListItems != null)
                        {
                            this.ProdUnitRefno = Convert.ToInt32((
                                this.ActItem.SelectedItem as dynamic).ProductUnitRefno);
                            tempSkidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                            tempSkidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                                tempSkidInfo.ProdInfo.Refno);
                        }
                        else
                        {
                            tempSkidInfo.ProdInfo = new ProductInfo();
                        }

                        this.SkidInfo.CurrSkid = tempSkidInfo;
                    }
                    else
                    {
                        try
                        {
                            this.IsNewSkid = false;
                            this.SavedSkidInfo = SysSkidInfo.DeepCopy(this.ActItem.ActHdr.SkidInfo);

                            // Retrieve the skid info from the comm server. e.g. Bucket totals may have been updated.
                            this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                                this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode.TrimEnd());
                            this.SavedSkidInfo = this.SkidInfo;

                            await Device.InvokeOnMainThreadAsync(() => this.EnableControls());
                        }
                        catch (MessageOnlyException msgEx)
                        {
                            var msg = SysCommon.GetMessageFromMessageException(msgEx);
                            this.SkidInfo = this.ActItem.ActHdr.SkidInfo;
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.GetControlDescription(),
                                    msg,
                                    Resource.OK));
                        }
                        catch (Exception ex)
                        {
                            this.SkidInfo = this.ActItem.ActHdr.SkidInfo;
                            var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                            _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                        }

                        if (this.ActItem.ActHdr.SkidInfo == null ||
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        }
                    }
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
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The OnBackButtonPressed.
        /// </summary>
        /// <returns><see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            var isClose = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await this.ScannerDisable();
                    if (string.IsNullOrEmpty(this.txtSkidCode.Text.TrimEnd()))
                    {
                        _ = base.OnBackButtonPressed();
                        this.DialogResult = DialogResult.None;
                        await this.Close();
                    }
                    else
                    {
                        if (this.IsChangesMade)
                        {
                            if (await this.DisplayAlert("Save Changes", "Would you like to save any changes?", "Yes",
                                    "No"))
                            {
                                if (await this.SaveChanges())
                                {
                                    _ = base.OnBackButtonPressed();
                                    this.DialogResult = DialogResult.OK;
                                    await this.Close();
                                }
                            }
                            else
                            {
                                if (await this.CancelChanges())
                                {
                                    throw new MessageOnlyException("Changes reverted.");
                                }
                            }
                        }
                        else
                        {
                            // Do nothing keep selected skid state.
                            this.IsChangesMade = false;
                            _ = base.OnBackButtonPressed();
                            this.DialogResult = DialogResult.OK;
                            await this.Close();
                        }
                    }
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    isClose = false;
                    _ = Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    isClose = false;
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                }
                finally
                {
                    await this.ScannerEnable();
                }
            });

            return isClose;
        }

        /// <summary>
        ///     Initialize the bar code modifier items.
        /// </summary>
        private void ParseAndSeedBarCodeModifier()
        {
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
        }

        /// <summary>
        ///     Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="scanArgs">
        ///     <see cref="BarCodeScannerEventArgs" /> Sender bar code scanner
        ///     event arguments.
        /// </param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.IsIndicatorRunning || scanArgs == null)
            {
                return;
            }

            BarCodeParse codeParse = null;

            // Added this task run to fix socket disposal errors when scanning a skid.
            // Thought that scanning a location and or product may have the same issue or similar.
            // Did not test scanning a product or location if they had the same disposal issue.
            await Task.Run(async () =>
            {
                try
                {
                    this.IsIndicatorRunning = true;
                    ////this.IsSvdBarScannerEnabled = this.IsBarCodeHandlerRegistered;
                    await this.ScannerDisable();

                    codeParse = scanArgs.ParseInfo;

                    // global location identifier scanned
                    if (!string.IsNullOrEmpty(codeParse.LocationIdentifier))
                    {
                        if (this.IsNewSkid)
                        {
                            var locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                                codeParse.LocationIdentifier);

                            this.IsChangesMade = true;

                            if (locMapRefno != 0)
                            {
                                this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                            }
                            else
                            {
                                this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                            }

                            await Device.InvokeOnMainThreadAsync(() => this.DisplayData());
                        }
                        else
                        {
                            throw new MessageOnlyException(
                                "Scanning a location identifier can only be performed on a new skid.");
                        }
                    }

                    // Activity item foreign reference number
                    if (!string.IsNullOrEmpty(codeParse.ActItemFKRefno))
                    {
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
                    }

                    // Activity detail foreign reference number
                    if (!string.IsNullOrEmpty(codeParse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException(
                            "Scanning an line item detail bar code is not supported here!");
                    }

                    // Shipping container scanned, skid
                    if (!string.IsNullOrEmpty(codeParse.SSCC18))
                    {
                        try
                        {
                            this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(codeParse.SSCC18.TrimEnd());
                            this.LocMap = this.SkidInfo.CurrSkid.LocMap.Clone();
                            this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                            this.IsNewSkid = false;
                            await Device.InvokeOnMainThreadAsync(() => this.EnableControls());

                            // Set changes have not been made flag. Loading a skid information should not trigger the
                            // save changes dialog in the IdentifyChangesMade event.
                            this.IsChangesMade = false;
                        }
                        catch (Exception ex)
                        {
                            this.IsChangesMade = true;
                            this.IsNewSkid = true;
                            await this.InitSkidInfo();

                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                this.GetControlDescription(),
                                ex.Message + Environment.NewLine + "A new skid will be created upon saving.",
                                "OK"));
                        }

                        await Device.InvokeOnMainThreadAsync(() => this.DisplayData());
                    }

                    // Product identifier
                    if (!string.IsNullOrEmpty(codeParse.ProductIdentifier))
                    {
                        if (this.IsNewSkid)
                        {
                            this.ProdUnitRefno = await SysShared.IdentifyProductScanned(
                                this.CallingActSubType,
                                codeParse);

                            if (this.ProdUnitRefno != 0)
                            {
                                this.IsChangesMade = true;
                                var tempSkidInfo = this.SkidInfo.CurrSkid;
                                tempSkidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                                tempSkidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                                    tempSkidInfo.ProdInfo.Refno);
                                this.SkidInfo.CurrSkid = tempSkidInfo;

                                // <issue="13609" author="RD" date="2016/02/22"/>
                                SysShared.AssertSuppressedErrorsAgainstScheme(this.SkidInfo.CurrSkid.ProdInfo,
                                    codeParse);
                            }

                            await Device.InvokeOnMainThreadAsync(() => this.DisplayData());
                        }
                        else
                        {
                            throw new MessageOnlyException(
                                "Scanning a product identifier can only be done with a new skid.");
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
                }
                catch (Exception ex)
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                }
                finally
                {
                    await this.ScannerEnable();
                    this.IsIndicatorRunning = false;
                }
            });
        }

        /// <summary>
        ///     Print skid label click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void PictureBoxPrintLabel_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                if (await this.IsReadyToSave())
                {
                    await this.SaveSkid();
                    await Device.InvokeOnMainThreadAsync(() => this.DisplayData());

                    if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                            this.GetControlDescription(),
                            "Print skid label?",
                            "Yes",
                            "No")))
                    {
                        SysSkidInfo.PrintLabel(this.SkidInfo);
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
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = await errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     Perform save skid information.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SaveSkid()
        {
            if (this.IsChangesMade)
            {
                var tempSkidInfo = this.SkidInfo.CurrSkid;

                // Important to not assign a current exiting SkidNo to a new skid.
                if (!this.IsNewSkid)
                {
                    tempSkidInfo.SkidCode = this.txtSkidCode.Text;
                }

                tempSkidInfo.Hi = string.IsNullOrEmpty(this.txtHi.Text) ? 0 : int.Parse(this.txtHi.Text);
                tempSkidInfo.Ti = string.IsNullOrEmpty(this.txtTi.Text) ? 0 : int.Parse(this.txtTi.Text);
                tempSkidInfo.Cube = string.IsNullOrEmpty(this.txtCube.Text) ? 0 : decimal.Parse(this.txtCube.Text);
                tempSkidInfo.Height =
                    string.IsNullOrEmpty(this.txtHeight.Text) ? 0 : decimal.Parse(this.txtHeight.Text);
                tempSkidInfo.IsPutAway = this.chkIsPutAway.IsToggled;
                tempSkidInfo.LocMap = this.LocMap.Clone();
                tempSkidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                    tempSkidInfo.ProdInfo.Refno);

                if (this.IsNewSkid)
                {
                    tempSkidInfo.SkidCode = this.txtSkidCode.Text;
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.SkidInfo = await SysSkidInfo.Create(SysSkidInfo.SkidInfoToXML(this.SkidInfo));
                }
                else
                {
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    SysSkidInfo.SendUpdate(this.SkidInfo);
                }

                this.IsChangesMade = false;
                this.IsSkidCodeChanged = false;
            }
        }

        /// <summary>
        ///     Enable all form controls.
        /// </summary>
        private void EnableControls()
        {
            if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo && this.SkidInfo.CurrSkid.Quantity == 0 &&
                this.SkidInfo.CurrSkid.BaseQuantity == 0)
            {
                this.cmdSkidProduct.IsEnabled = true;
                this.BarCodeModifier1Picker.IsEnabled = true;
            }

            if (this.IsNewSkid || (this.SkidInfo.CurrSkid.Quantity == 0 && this.SkidInfo.CurrSkid.BaseQuantity == 0))
            {
                this.cmdLocation.IsEnabled = true;
            }

            this.txtTi.IsEnabled = true;
            this.txtHi.IsEnabled = true;
            this.txtHeight.IsEnabled = true;
            this.txtCube.IsEnabled = true;
            this.cmdDetail.IsEnabled = true;
            this.cmdComplete.IsEnabled = true;
            this.pictureBoxPrintLabel.IsEnabled = true;
        }

        /// <summary>
        ///     Disable the form controls.
        /// </summary>
        private void DisableControls()
        {
            if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo && this.SkidInfo.CurrSkid.Quantity == 0 &&
                this.SkidInfo.CurrSkid.BaseQuantity == 0)
            {
            }
            else
            {
                // this.cmdSkidProduct.IsEnabled = false;
                // this.CmdLocation.IsEnabled = false;
                this.txtTi.IsEnabled = false;
                this.txtHi.IsEnabled = false;
                this.txtHeight.IsEnabled = false;
                this.txtCube.IsEnabled = false;
            }

            this.BarCodeModifier1Picker.IsEnabled = false;
            this.cmdDetail.IsEnabled = false;
            this.cmdComplete.IsEnabled = false;
            this.pictureBoxPrintLabel.IsEnabled = false;
        }

        /// <summary>
        ///     Display data.
        /// </summary>
        private void DisplayData()
        {
            this.txtSkidCode.Text = string.Empty;
            this.txtTi.Text = string.Empty;
            this.txtHi.Text = string.Empty;
            this.txtHeight.Text = string.Empty;
            this.txtCube.Text = string.Empty;
            this.chkIsPutAway.IsToggled = false;
            this.lblCubeUOM.Text = string.Empty;
            this.lblHeightUOM.Text = string.Empty;

            this.cmdLocation.Text = "Set Location";

            if (this.SkidInfo.CurrSkid.LocMapRefno > 0)
            {
                this.cmdLocation.Text = LocationMapTasks.GetReadableLocation(this.SkidInfo.CurrSkid.LocMap);
            }

            this.cmdSkidProduct.Text = "Select Product";

            if (this.SkidInfo.CurrSkid.ProdInfo != null)
            {
                if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0)
                {
                    this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(
                        this.SkidInfo.CurrSkid.ProdInfo);
                }
            }

            this.cmdDetail.IsEnabled = false;
            this.cmdComplete.IsEnabled = false;

            this.ExtraInfoItems.Clear();

            if (this.SkidInfo != null && this.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;
                this.txtTi.Text = this.SkidInfo.CurrSkid.Ti.ToString();
                this.txtHi.Text = this.SkidInfo.CurrSkid.Hi.ToString();
                this.txtHeight.Text = this.SkidInfo.CurrSkid.Height.ToString();
                this.txtCube.Text = this.SkidInfo.CurrSkid.Cube.ToString();
                this.chkIsPutAway.IsToggled = this.SkidInfo.CurrSkid.IsPutAway;
                this.lblCubeUOM.Text = this.SkidInfo.CurrSkid.CubeUnit;
                this.lblHeightUOM.Text = this.SkidInfo.CurrSkid.HeightUnit;
                this.cmdLocation.Text = LocationMapTasks.GetReadableLocation(this.SkidInfo.CurrSkid.LocMap);
                this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(
                    this.SkidInfo.CurrSkid.ProdInfo);
                this.cmdDetail.IsEnabled = true;

                if (this.SkidInfo.CurrSkid.IsCompleted)
                {
                    this.cmdComplete.IsEnabled = false;
                }
                else
                {
                    this.cmdComplete.IsEnabled = true;
                }

                this.ExtraInfoItems.Add(string.Format(
                    "Quantity {0} {1} @ {2} {3}",
                    this.SkidInfo.CurrSkid.Quantity,
                    this.SkidInfo.CurrSkid.ProdInfo.UnitCode,
                    this.SkidInfo.CurrSkid.WeightQuantity,
                    this.SkidInfo.CurrSkid.ProdInfo.WeightUnitCode));
            }
        }

        /// <summary>
        ///     Set location click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (!this.IsNewSkid)
                {
                    throw new MessageOnlyException("A location can only be assigned to a new skid.");
                }

                var form = new FormDialogContainer(
                    EDialogTypes.SelectLocationMap,
                    SysCommon.ActSubTypes.NotApplicable);

                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    this.LocMap = LocationMap.GetInstance().ChosenLocMap.Clone();
                    var tempSkidInfo = this.SkidInfo.CurrSkid;
                    tempSkidInfo.LocMap = this.LocMap.Clone();
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.IsChangesMade = true;
                }

                _ = await Device.InvokeOnMainThreadAsync(() =>
                    Task.FromResult(this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation()));
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
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = await errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///     Skid detail click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsIndicatorRunning = true;
                await this.ScannerDisable();

                if (this.SkidInfo.CurrSkid.SkidFKRefno == 0)
                {
                    throw new MessageOnlyException("A skid code is required to see it's details.");
                }

                if (this.IsChangesMade)
                {
                    if (!await this.SaveChanges())
                    {
                        throw new MessageOnlyException("Unable to save the skid information before seeing details.");
                    }
                }

                LocationMap.GetInstance().CurrLocMap = this.LocMap;

                // Create the activity skid detail, activity item.
                var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.SkidInfo);
                activity.HdrFkRefno = this.SkidInfo.CurrSkid.SkidFKRefno;
                activity.HdrRefno = this.SkidInfo.CurrSkid.SkidFKRefno;
                activity.SkidInfo = this.SkidInfo;

                var actItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.SkidInfo);
                actItem.ItemFkRefno = this.SkidInfo.CurrSkid.SkidFKRefno;
                actItem.ItemRefno = this.SkidInfo.CurrSkid.SkidFKRefno;

                actItem.LocMap = this.LocMap;

                actItem.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                actItem.ProdInfo.Unit2BaseUnitFactor = await ProductInfoTasks.GetUnitRelationFactor(
                    actItem.ProdInfo);

                var form = new FormDialogContainer(
                    EDialogTypes.ActivityDetailSkid,
                    actItem,
                    SysCommon.ActSubTypes.SkidInfo,
                    this.CallingActSubType);

                if (await Device.InvokeOnMainThreadAsync(
                        async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    var xmlDoc = await form.GetXML();
                    this.IsIndicatorRunning = true;
                    this.IsChangesMade = false;

                    if (this.SkidInfo.CurrSkid.IsConsumed)
                    {
                        await this.InitSkidInfo();
                        this.IsNewSkid = true;
                    }
                }

                await Device.InvokeOnMainThreadAsync(() => this.DisplayData());
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
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = await errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Complete the skid click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdComplete_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                this.SkidInfo = SysSkidInfo.Complete(this.SkidInfo);

                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    this.GetControlDescription(),
                    "Skid completed.",
                    "OK"));
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
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = await errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     Select the skid product click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdSkidProduct_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (!this.IsNewSkid)
                {
                    throw new MessageOnlyException("A product can only be assigned to a skid.");
                }

                var form = new FormDialogContainer(
                    EDialogTypes.SelectProductUnit,
                    SysCommon.ActSubTypes.SkidInfo);

                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXML();
                    this.ProdUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);
                    var tempSkidInfo = this.SkidInfo.CurrSkid;
                    tempSkidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                    tempSkidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                        tempSkidInfo.ProdInfo.Refno);
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.IsChangesMade = true;
                }

                this.cmdSkidProduct.Text = "Select Product";

                if (this.SkidInfo.CurrSkid.ProdInfo != null)
                {
                    if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != 0)
                    {
                        this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(
                            this.SkidInfo.CurrSkid.ProdInfo);
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
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = await errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        ///     The IdentifyChangesMade.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Object that generated the event.</param>
        /// <param name="e"><see cref="TextChangedEventArgs" /> Text changed event arguments.</param>
        private void IdentifyChangesMade(object sender, TextChangedEventArgs e)
        {
            try
            {
                ////Device.BeginInvokeOnMainThread(async () => await this.ScannerDisable());

                // Set as changed only after form has been loaded.
                if (!this.IsIndicatorRunning)
                {
                    if (sender == this.txtSkidCode && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;

                        // Device.BeginInvokeOnMainThread(() => this.DisableControls());
                        this.IsSkidCodeChanged = true;
                    }

                    if (sender == this.txtCube && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;
                    }

                    if (sender == this.txtHeight && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;
                    }

                    if (sender == this.txtHi && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;
                    }

                    if (sender == this.txtTi && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;
                    }

                    if (sender == this.chkIsPutAway)
                    {
                        this.IsChangesMade = true;
                    }

                    if (sender == this.BarCodeModifier1Picker && e.NewTextValue != e.OldTextValue)
                    {
                        this.IsChangesMade = true;
                    }

                    ValidateEntry(sender, new CancelEventArgs());
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);

                // Do not set the IsChangesMade. MessageOnlyException is really not and error.
                // Allow the validation to handle the setting for IsChangesMade.
                ////this.IsChangesMade = false;
                _ = Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msg,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async void ValidateEntry(object sender, CancelEventArgs e)
            {
                e.Cancel = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        if (sender == this.txtSkidCode)
                        {
                            // Key down event fires before
                            if (this.SkidInfo.CurrSkid.SkidFKRefno == 0 || string.IsNullOrEmpty(this.txtSkidCode.Text))
                            {
                                this.IsNewSkid = true;
                            }
                            else
                            {
                                this.IsNewSkid = false;
                            }
                        }

                        if (sender == this.txtCube)
                        {
                            var isDecimal = decimal.TryParse(this.txtCube.Text, out var result);

                            if (isDecimal && result < 0)
                            {
                                e.Cancel = true;
                                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                    this.GetControlDescription(),
                                    "Cube value must be greater than or equal to zero.",
                                    "OK"));
                            }
                        }

                        if (sender == this.txtHeight)
                        {
                        }

                        if (sender == this.txtHi)
                        {
                        }

                        if (sender == this.txtTi)
                        {
                        }

                        if (sender == this.chkIsPutAway)
                        {
                        }

                        if (sender == this.BarCodeModifier1Picker)
                        {
                        }
                    }
                    catch (Exception)
                    {
                        e.Cancel = false;
                        throw;
                    }
                });
            }
        }

        /// <summary>
        ///     Bar code modifier selected item changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void UpdnBarCodeModifier1_SelectedItemChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
        }

        /// <summary>
        ///     Setup a new skid from skid code or assigned from.
        /// </summary>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SetupNewSkidFromSkidCode()
        {
            // SkidInfo may be a new skid, skid numbers can be created from the comm server.
            this.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(this.txtSkidCode.Text.TrimEnd());

            if (this.txtSkidCode.Text.TrimEnd() != this.SkidInfo.CurrSkid.SkidCode.TrimEnd())
            {
                this.IsNewSkid = true;
            }

            // Location may have been defaulted or already set.
            this.LocMap = this.SkidInfo.CurrSkid.LocMap;
            this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;

            if (this.ProdUnitRefno == 0)
            {
                this.IsNewSkid = true;
            }
        }

        /// <summary>
        ///     The txtSkidCode_KeyUpEnter.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="KeyEventArgs" /> Key event arguments.</param>
        private async void TxtSkidCode_KeyUpEnter(object sender, KeyEventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                this.IsNewSkid = false;

                try
                {
                    await SetupNewSkid();
                    this.EnableControls();
                    this.DisplayData();
                }
                catch (Exception ex)
                {
                    this.IsNewSkid = true;
                    await this.InitSkidInfo();

                    await this.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message + Environment.NewLine + "A new Skid will be created upon saving.",
                        "OK");

                    this.EnableControls();
                }

                Device.BeginInvokeOnMainThread(() => { this.DisplayData(); });
                this.IsSkidCodeChanged = false;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
                if (!this.IsNewSkid)
                {
                    this.IsChangesMade = false;
                }
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task SetupNewSkid()
            {
                if (string.IsNullOrEmpty(this.txtSkidCode.Text))
                {
                    SetupNewSkid();
                }
                else
                {
                    await this.SetupNewSkidFromSkidCode();
                }

                return;

                //////////////////////////////////////////////////////////////////////////
                void SetupNewSkid()
                {
                    this.IsNewSkid = true;
                    this.SkidInfo = new SysSkidInfo();
                    this.ProdUnitRefno = 0;
                    this.LocMap = LocationMap.GetInstance().CurrLocMap;
                }
            }
        }

        /// <summary>
        ///     The cmdNewSkid_Click.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private async void CmdNewSkid_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                this.IsNewSkid = true;

                // Clearing the skid code text box is needed to handle a new skid in this.SaveChanges().
                this.txtSkidCode.Text = string.Empty;
                this.IsSkidCodeChanged = false;
                this.DisableControls();

                var tempSkidInfo = new SSkidInfo();
                tempSkidInfo.LocMap = this.LocMap.Clone();
                tempSkidInfo.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                    tempSkidInfo.ProdInfo.Refno);
                this.SkidInfo.CurrSkid = tempSkidInfo;

                // this.IsChangesMade = true, Needed for SaveChanges to make the create new skid call.
                this.IsChangesMade = true;

                if (await this.IsReadyToSave())
                {
                    await this.SaveSkid();
                    this.IsNewSkid = false;
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        "Skid created in DTEC.",
                        Resource.OK);
                }
                else
                {
                    // If save fails just display the new skid detail.
                    throw new MessageOnlyException("Failed to create skid in DTEC.");
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
            finally
            {
                await this.ScannerEnable();
                this.DisplayData();
                this.EnableControls();
            }
        }

        /// <summary>
        ///     The txtSkidCode_Completed.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void TxtSkidCode_Completed(object sender, EventArgs e)
        {
            this.TxtSkidCode_KeyUpEnter(sender, new KeyEventArgs(
                false,
                Keycode.Enter,
                new KeyEvent(KeyEventActions.Up, Keycode.Enter)));
        }

        /// <summary>
        ///     The BarCodeModifier1Picker_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, new TextChangedEventArgs(string.Empty, string.Empty));
        }

        /// <summary>
        ///     The chkIsPutAway_Toggled.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Event arguments.</param>
        private void ChkIsPutAway_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new TextChangedEventArgs(string.Empty, string.Empty));
        }
    }
}