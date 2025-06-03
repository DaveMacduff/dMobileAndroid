// <copyright file="ActivityHdrSkidInfo.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using BarCodeScanner.BarCodeParsing;
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity;
    using Controller.Activity_Item;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Defines the <see cref="ActivityHdrSkidInfo" /> Skid information maintenance header.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityHdrSkidInfo : IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        /// <param name="activityType"><see cref="ActivityType" /> Activity type.</param>
        public ActivityHdrSkidInfo(SysShared.ActivityType activityType)
        {
            this.InitializeComponent();
            this.InitControlText();

            // Set changes have not been made flag. Loading a skid information should not trigger the
            // save changes dialog in the IdentifyChangesMade event.
            this.IsChangesMade = false;
            this.IsCallerSkidLoaded = false;
            this.IsNewSkid = false;
            this.CallingActSubType = SysCommon.ActSubTypes.SkidInfo;
            this.ActivityType = activityType;
            this.SkidInfo = new SysSkidInfo();
            this.ProdInfo = new ProductInfo();
            this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
            this.SkidInfo.CurrSkid.LocMap = this.LocMap;
            this.SkidInfo.CurrSkid.ProdInfo = this.ProdInfo;
            this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
            this.CallingWarehouseLocMapRefno = this.LocMap.WhsLocMapRefno;

            this.ParseAndSeedBarCodeModifier();
            _ = Device.InvokeOnMainThreadAsync(
                () => this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);
            this.DisplaySkidInfo().FireAndForgetSafeAsync();
            this.ScannerEnable().FireAndForgetSafeAsync();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        /// <param name="callingActSubType"><see cref="SysCommon.ActSubTypes" /> Calling activity subtype.</param>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        /// <param name="activityType"><see cref="ActivityType" /> Activity type.</param>
        public ActivityHdrSkidInfo(
            SysCommon.ActSubTypes callingActSubType,
            Activity activity,
            SysShared.ActivityType activityType)
        {
            this.InitializeComponent();
            this.InitControlText();

            // Set changes have not been made flag. Loading a skid information should not trigger the
            // save changes dialog in the IdentifyChangesMade event.
            this.CallingActSubType = callingActSubType;
            this.ActivityType = activityType;
            if (activity.SkidInfo.CurrSkid.SkidFkRefno <= 0)
            {
                this.IsChangesMade = true;
                this.IsCallerSkidLoaded = false;
                this.IsNewSkid = true;
                this.SkidInfo = new SysSkidInfo();
                this.ProdInfo = new ProductInfo();
                this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                this.SkidInfo.CurrSkid.LocMap = this.LocMap;
                this.SkidInfo.CurrSkid.ProdInfo = this.ProdInfo;
            }
            else
            {
                this.IsChangesMade = false;
                this.IsCallerSkidLoaded = true;
                this.IsNewSkid = false;
                this.SkidInfo = activity.SkidInfo;
                this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                this.LocMap = this.SkidInfo.CurrSkid.LocMap;
            }

            this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
            this.CallingWarehouseLocMapRefno = this.LocMap.WhsLocMapRefno;

            // Load control data.
            this.ParseAndSeedBarCodeModifier();
            _ = Device.InvokeOnMainThreadAsync(
                () => this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);
            this.DisplaySkidInfo().FireAndForgetSafeAsync();
            this.ScannerEnable().FireAndForgetSafeAsync();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityHdrSkidInfo" /> class.
        /// </summary>
        /// <param name="callingActSubType"><see cref="SysCommon.ActSubTypes" /> Calling activity subtype.</param>
        /// <param name="actItem"><see cref="ActItem" /> Activity item.</param>
        /// <param name="activityType"><see cref="SysShared.ActivityType" /> Activity type.</param>
        public ActivityHdrSkidInfo(
            SysCommon.ActSubTypes callingActSubType,
            ActItem actItem,
            SysShared.ActivityType activityType)
        {
            this.InitializeComponent();
            this.InitControlText();

            // Set changes have not been made flag. Loading a skid information should not trigger the
            // save changes dialog in the IdentifyChangesMade event.
            this.CallingActSubType = callingActSubType;
            this.ActivityType = activityType;
            if (actItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno <= 0)
            {
                this.IsChangesMade = true;
                this.IsCallerSkidLoaded = false;
                this.IsNewSkid = true;
                this.SkidInfo = new SysSkidInfo();
                this.ProdInfo = actItem.ProdInfo;
                this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                this.SkidInfo.CurrSkid.ProdInfo = this.ProdInfo;
                this.SkidInfo.CurrSkid.LocMap = this.LocMap;
                this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
            }
            else
            {
                this.IsChangesMade = false;
                this.IsNewSkid = false;
                this.IsCallerSkidLoaded = true;
                this.SkidInfo = actItem.ActHdr.SkidInfo;
                this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
                this.SetIsNewSkid(this.SkidInfo.CurrSkid.SkidCode).FireAndForgetSafeAsync();
            }

            this.CallingWarehouseLocMapRefno = this.LocMap.WhsLocMapRefno;

            this.ParseAndSeedBarCodeModifier();
            _ = Device.InvokeOnMainThreadAsync(
                () => this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1);
            this.DisplaySkidInfo().FireAndForgetSafeAsync();
            this.ScannerEnable().FireAndForgetSafeAsync();
        }

        private SysShared.ActivityType ActivityType { get; }

        private SysCommon.ActSubTypes CallingActSubType { get; }

        private bool IsSkidHasQuantity => this.SkidInfo.CurrSkid.Quantity > 0;

        private int CallingWarehouseLocMapRefno { get; }

        private SysSkidInfo CallingSkidInfo { get; set; }

        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

        private bool IsCallerSkidLoaded { get; set; }

        private bool IsNewSkid { get; set; }

        private SLocMap LocMap { get; set; }

        private ProductInfo ProdInfo { get; set; }

        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        private bool IsChangesMade { get; set; }

        private SysSkidInfo SkidInfo { get; set; }

        private ObservableCollection<string> ExtraInfoItems { get; } = new ObservableCollection<string>();

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
                this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            }
        }

        /// <summary>
        ///     Check if the form data is in a state that can be saved.
        /// </summary>
        /// <returns><see cref="bool" /> True if ready to save else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            const bool isReadyToSave = true;

            if (!this.IsChangesMade)
            {
                const string msg = "No changes have been made to save.";
                throw new MessageOnlyException(msg);
            }

            if (this.ProdInfo.UnitRefno <= 0)
            {
                throw new MessageOnlyException("A product must be assigned to the skid.");
            }

            ////if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap))
            ////{
            ////    throw new MessageOnlyException("A location must be assigned to the skid.");
            ////}

            if (this.SysEnv.SysSettings.IsPickDisableMakeSkid &&
                this.CallingActSubType == SysCommon.ActSubTypes.Picking)
            {
                throw new MessageOnlyException("Making a new skid is disabled during picking.");
            }

            this.SkidInfo.CurrSkid = await this.FillSkidInfo();

            return isReadyToSave;
        }

        /// <summary>
        ///     Check if the form can be closed.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form can be closed.</returns>
        public async Task<bool> IsReadyToClose()
        {
            this.SkidInfo.CurrSkid = await this.FillSkidInfo();
            return this.SkidInfo.Equals(this.CallingSkidInfo);
        }

        /// <summary>
        ///     Cancel any pending form data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the form changes where cancelled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = false;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                    cancelChanges = true;
                }
                else
                {
                    if (await this.DisplayAlert(
                            GetControlDescription(),
                            "Changes have not been saved. Close anyway?",
                            "Yes",
                            "No"))
                    {
                        cancelChanges = true;
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(this.CallingActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity skid return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity skid return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXml()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var skidInfo = this.SkidInfo.CurrSkid;

            XDocument xmlDoc;
            if (skidInfo.SkidFkRefno == 0)
            {
                var dt = new DateTime(1900, 1, 1);
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<params>" +
                    "<skid_fk_refno>0</skid_fk_refno>" +
                    "<chep>false</chep>" +
                    "<hold>false</hold>" +
                    "<base_quantity>0</base_quantity>" +
                    "<cube>0</cube>" +
                    "<height>0</height>" +
                    "<quantity>0</quantity>" +
                    "<weight_quantity>0</weight_quantity>" +
                    "<hi>0</hi>" +
                    "<ti>0</ti>" +
                    "<cube_unit></cube_unit>" +
                    "<height_unit></height_unit>" +
                    "<logistic_unit_skid_code></logistic_unit_skid_code>" +
                    "<skid_code></skid_code>" +
                    "<location_map_refno>0</location_map_refno>" +
                    "<product_unit_refno>0</product_unit_refno>" +
                    "<completed>false</completed>" +
                    "<consumed>false</consumed>" +
                    "<putaway>false</putaway>" +
                    "<logistic_unit_batch_lot_number></logistic_unit_batch_lot_number>" +
                    "<logistic_unit_best_before_sell_by_date>" + dt + "</logistic_unit_best_before_sell_by_date>" +
                    "<logistic_unit_production_date>" + dt + "</logistic_unit_production_date>" +
                    "<product_scheme_variant_no>0</product_scheme_variant_no>" +
                    "<product_scheme_variant_code></product_scheme_variant_code>" +
                    "<product_scheme_variant_description></product_scheme_variant_description>" +
                    "<act_sub_type>" + (int)this.CallingActSubType + "</act_sub_type>" +
                    "<act_fk_refno>0</act_fk_refno>" +
                    "<actitem_fk_refno>0</actitem_fk_refno>" +
                    "</params>");
            }
            else
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                    "<params>" +
                    "<skid_fk_refno>" + skidInfo.SkidFkRefno + "</skid_fk_refno>" +
                    "<chep>" + skidInfo.IsChep + "</chep>" +
                    "<hold>" + skidInfo.IsHold + "</hold>" +
                    "<base_quantity>" + skidInfo.BaseQuantity + "</base_quantity>" +
                    "<cube>" + skidInfo.Cube + "</cube>" +
                    "<height>" + skidInfo.Height + "</height>" +
                    "<quantity>" + skidInfo.Quantity + "</quantity>" +
                    "<weight_quantity>" + skidInfo.WeightQuantity + "</weight_quantity>" +
                    "<hi>" + skidInfo.Hi + "</hi>" +
                    "<ti>" + skidInfo.Ti + "</ti>" +
                    "<cube_unit>" + skidInfo.CubeUnit + "</cube_unit>" +
                    "<height_unit>" + skidInfo.HeightUnit + "</height_unit>" +
                    "<logistic_unit_skid_code>" + skidInfo.LogisticUnitSkidCode + "</logistic_unit_skid_code>" +
                    "<skid_code>" + skidInfo.SkidCode + "</skid_code>" +
                    "<location_map_refno>" + skidInfo.LocMapRefno + "</location_map_refno>" +
                    "<product_unit_refno>" + skidInfo.ProdInfo.UnitRefno + "</product_unit_refno>" +
                    "<completed>" + skidInfo.IsCompleted + "</completed>" +
                    "<consumed>" + skidInfo.IsConsumed + "</consumed>" +
                    "<putaway>" + skidInfo.IsPutAway + "</putaway>" +
                    "<logistic_unit_batch_lot_number>" + skidInfo.SchemeData.LogisticUnitBatchLotNumber + "</logistic_unit_batch_lot_number>" +
                    "<logistic_unit_best_before_sell_by_date>" + skidInfo.SchemeData.LogisticUnitBestBeforeSellByDate + "</logistic_unit_best_before_sell_by_date>" +
                    "<logistic_unit_production_date>" + skidInfo.SchemeData.LogisticUnitProductionDate + "</logistic_unit_production_date>" +
                    "<product_scheme_variant_no>" + skidInfo.SchemeData.VariantNumber + "</product_scheme_variant_no>" +
                    "<product_scheme_variant_code>" + skidInfo.SchemeData.VariantCode + "</product_scheme_variant_code>" +
                    "<product_scheme_variant_description>" + skidInfo.SchemeData.VariantDescription + "</product_scheme_variant_description>" +
                    "</params>");
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
                    await this.SaveSkid(this.SkidInfo.CurrSkid);
                    saveComplete = true;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                saveComplete = false;
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
                            msg,
                            Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                    {
                        var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                        _ = await errRpt.ShowDialog();
                    });
            }

            return saveComplete;
        }

        /// <summary>
        ///     Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            SubscribeKeyUpHandlers();

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    KeyUpHandlerCallback);
                return;

                async void KeyUpHandlerCallback(string keyPressed)
                {
                    try
                    {
                        await this.ScannerDisable();
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);
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
                            await BreakSkid();
                        }
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx);
                        await Device.InvokeOnMainThreadAsync(
                            async () => await this.DisplayAlert(
                                GetControlDescription(),
                                msg,
                                "OK"));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(
                            async () =>
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

            async Task BreakSkid()
            {
                if (string.IsNullOrEmpty(this.SkidInfo.CurrSkid.SkidCode))
                {
                    const string msg = "No skid code has been entered to be broken.";
                    throw new MessageOnlyException(msg);
                }

                if (await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            GetControlDescription(),
                            $"Do you wish to break this skid '{this.SkidInfo.CurrSkid.SkidCode}'?",
                            "Yes",
                            "No")))
                {
                    if (!this.IsNewSkid)
                    {
                        SysSkidInfo.Break(this.SkidInfo.CurrSkid);
                    }

                    await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            GetControlDescription(),
                            $"This skid '{this.SkidInfo.CurrSkid.SkidCode}' is now broken.",
                            "OK"));

                    this.SkidInfo.CurrSkid = new SSkidInfo();
                    this.ProdInfo = new ProductInfo();
                    this.LocMap = new SLocMap();
                    this.IsCallerSkidLoaded = false;
                    this.IsNewSkid = false;
                    this.IsChangesMade = false;
                    await this.DisplaySkidInfo();
                }
            }
        }

        /// <summary>
        ///     The OnDisappearing.
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     The OnBackButtonPressed.
        /// </summary>
        /// <returns><see cref="bool" /> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(PerformCancelChanges);
            return true;

            async void PerformCancelChanges()
            {
                try
                {
                    if (await this.CancelChanges())
                    {
                        // Set the dialog result to OK to force the line item total to recalculate.
                        // Even though the back button is meant to be a cancel we need to update
                        // because data still could have changed.
                        this.DialogResult = DialogResult.Cancel;
                        BackButtonCallback();
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }

            async void BackButtonCallback()
            {
                try
                {
                    this.SysEnv.SysSettings.IsShowActivityFilterSelection = true;
                    this.SysEnv.SysSettings.IsNewActivityDetailsAdded = false;
                    await this.ScannerDisable();
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                        {
                            var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                            _ = await errRpt.ShowDialog();
                        });
                }
            }
        }

        private static string GetControlDescription()
        {
            return "Skid Info";
        }

        private async Task SetIsNewSkid(string skidCode)
        {
            try
            {
                _ = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(skidCode);
                this.IsNewSkid = false;
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx).ToUpper();
                this.IsNewSkid = msg.Contains("DOES NOT EXIST.");
            }
            catch
            {
                this.IsNewSkid = true;
            }
        }

        private async Task InitSkidInfo()
        {
            if (this.SkidInfo.CurrSkid.SkidFkRefno <= 0)
            {
                this.IsCallerSkidLoaded = false;
                this.SkidInfo.CurrSkid = new SSkidInfo();
                this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                this.ProdInfo.Scheme =
                    await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                this.ProdInfo.Unit2BaseUnitFactor =
                    await ProductInfoTasks.GetUnitRelationFactor(this.ProdInfo);
                this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                this.CallingSkidInfo = (SysSkidInfo)this.SkidInfo.ShallowCopy();
            }
            else
            {
                this.CallingSkidInfo = (SysSkidInfo)this.SkidInfo.ShallowCopy();
                switch (this.ActivityType)
                {
                    case SysShared.ActivityType.Header:
                        this.IsCallerSkidLoaded = true;
                        this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                        this.ProdInfo.Scheme =
                            await ProductInfoTasks.GetSchemeComponentsFromRefno(
                                this.ProdInfo.Refno);
                        this.ProdInfo.Unit2BaseUnitFactor =
                            await ProductInfoTasks.GetUnitRelationFactor(this.ProdInfo);
                        this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                        break;

                    case SysShared.ActivityType.Items:
                        this.IsCallerSkidLoaded = true;
                        this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                        this.ProdInfo.Scheme =
                            await ProductInfoTasks.GetSchemeComponentsFromRefno(
                                this.ProdInfo.Refno);
                        this.ProdInfo.Unit2BaseUnitFactor =
                            await ProductInfoTasks.GetUnitRelationFactor(this.ProdInfo);
                        this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                        break;

                    case SysShared.ActivityType.Details:
                        this.IsCallerSkidLoaded = true;
                        this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                        this.ProdInfo.Scheme =
                            await ProductInfoTasks.GetSchemeComponentsFromRefno(
                                this.ProdInfo.Refno);
                        this.ProdInfo.Unit2BaseUnitFactor =
                            await ProductInfoTasks.GetUnitRelationFactor(this.ProdInfo);
                        this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                        break;

                    case SysShared.ActivityType.DetailsEdit:
                        throw new NotImplementedException(
                            "ActivityHdrSkidInfo: ActivityType.DetailsEdit not implemented.");

                    case SysShared.ActivityType.HeaderFilter:
                        throw new NotImplementedException(
                            "ActivityHdrSkidInfo: ActivityType.HeaderFilter not implemented.");

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Task DisplaySkidInfo()
        {
            switch (this.ActivityType)
            {
                case SysShared.ActivityType.Header:
                    _ = Device.InvokeOnMainThreadAsync(
                        () =>
                        {
                            this.txtSkidCode.IsEnabled = true;
                            this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;

                            if (this.ProdInfo.Refno > 0)
                            {
                                this.cmdSkidProduct.IsEnabled = false;
                                this.cmdSkidProduct.Text =
                                    ProductInfoTasks.GetCurrReadableProdBcUnit(this.ProdInfo);
                            }
                            else
                            {
                                this.cmdSkidProduct.IsEnabled = true;
                                this.cmdSkidProduct.Text = "Select Product";
                            }

                            this.cmdSaveSkid.IsEnabled = true;
                            this.cmdDetail.IsEnabled = true;
                            this.cmdComplete.IsEnabled = true;

                            if (this.SkidInfo.CurrSkid.LocMapRefno > 0)
                            {
                                this.cmdLocation.Text =
                                    LocationMapTasks.GetReadableLocation(this.LocMap);
                            }
                            else
                            {
                                this.cmdLocation.Text = "Set Location";
                            }

                            ////if (this.IsSkidHasQuantity)
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = true;
                            ////}
                            ////else
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = false;
                            ////}

                            this.txtCube.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Cube, 3),
                                CultureInfo.CurrentCulture);
                            this.lblCubeUOM.Text = this.SkidInfo.CurrSkid.CubeUnit;
                            this.txtHeight.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Height, 3),
                                CultureInfo.CurrentCulture);
                            this.lblHeightUOM.Text = this.SkidInfo.CurrSkid.HeightUnit;
                            this.txtHi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Hi,
                                CultureInfo.CurrentCulture);
                            this.txtTi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Ti,
                                CultureInfo.CurrentCulture);

                            this.ExtraInfoItems.Clear();
                            this.ExtraInfoItems.Add(
                                $"Quantity {this.SkidInfo.CurrSkid.Quantity} {this.SkidInfo.CurrSkid.ProdInfo?.UnitCode}" +
                                $" @ {this.SkidInfo.CurrSkid.WeightQuantity} {this.SkidInfo.CurrSkid.ProdInfo?.WeightUnitCode}");
                        });

                    break;

                case SysShared.ActivityType.Items:
                    _ = Device.InvokeOnMainThreadAsync(
                        () =>
                        {
                            this.txtSkidCode.IsEnabled = true;
                            this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;
                            this.cmdSaveSkid.IsEnabled = true;

                            if (this.ProdInfo.Refno > 0)
                            {
                                this.cmdSkidProduct.Text =
                                    ProductInfoTasks.GetCurrReadableProdBcUnit(this.ProdInfo);
                                this.cmdSkidProduct.IsEnabled = false;
                            }
                            else
                            {
                                this.cmdSkidProduct.IsEnabled = true;
                                this.cmdSkidProduct.Text = "SELECT PRODUCT";
                            }

                            this.cmdDetail.IsEnabled = true;
                            this.cmdComplete.IsEnabled = true;

                            if (this.SkidInfo.CurrSkid.LocMapRefno > 0)
                            {
                                this.cmdLocation.Text =
                                    LocationMapTasks.GetReadableLocation(this.LocMap);
                            }
                            else
                            {
                                this.cmdLocation.Text = "Set Location";
                            }

                            ////if (this.SkidInfo.CurrSkid.Quantity > 0)
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = true;
                            ////}
                            ////else
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = false;
                            ////}

                            this.txtCube.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Cube, 3),
                                CultureInfo.CurrentCulture);
                            this.lblCubeUOM.Text = this.SkidInfo.CurrSkid.CubeUnit;
                            this.txtHeight.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Height, 3),
                                CultureInfo.CurrentCulture);
                            this.lblHeightUOM.Text = this.SkidInfo.CurrSkid.HeightUnit;
                            this.txtHi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Hi,
                                CultureInfo.CurrentCulture);
                            this.txtTi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Ti,
                                CultureInfo.CurrentCulture);
                            this.ExtraInfoItems.Clear();
                            this.ExtraInfoItems.Add(
                                $"Quantity {this.SkidInfo.CurrSkid.Quantity} {this.SkidInfo.CurrSkid.ProdInfo?.UnitCode}" +
                                $" @ {this.SkidInfo.CurrSkid.WeightQuantity} {this.SkidInfo.CurrSkid.ProdInfo?.WeightUnitCode}");
                        });

                    break;

                case SysShared.ActivityType.Details:
                    _ = Device.InvokeOnMainThreadAsync(
                        () =>
                        {
                            this.txtSkidCode.IsEnabled = true;
                            this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;
                            this.cmdSaveSkid.IsEnabled = true;
                            if (this.ProdInfo.Refno > 0)
                            {
                                this.cmdSkidProduct.Text =
                                    ProductInfoTasks.GetCurrReadableProdBcUnit(this.ProdInfo);
                                this.cmdSkidProduct.IsEnabled = false;
                            }
                            else
                            {
                                this.cmdSkidProduct.Text = "SELECT PRODUCT";
                                this.cmdSkidProduct.IsEnabled = true;
                            }

                            this.cmdDetail.IsEnabled = true;
                            this.cmdComplete.IsEnabled = true;

                            if (this.SkidInfo.CurrSkid.LocMapRefno > 0)
                            {
                                this.cmdLocation.Text =
                                    LocationMapTasks.GetReadableLocation(this.LocMap);
                            }
                            else
                            {
                                this.cmdLocation.Text = "Set Location";
                            }

                            ////if (this.SkidInfo.CurrSkid.Quantity > 0)
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = true;
                            ////}
                            ////else
                            ////{
                            ////    this.pictureBoxPrintLabel.IsEnabled = false;
                            ////}

                            this.txtCube.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Cube, 3),
                                CultureInfo.CurrentCulture);
                            this.lblCubeUOM.Text = this.SkidInfo.CurrSkid.CubeUnit;
                            this.txtHeight.Text = Convert.ToString(
                                Math.Round(this.SkidInfo.CurrSkid.Height, 3),
                                CultureInfo.CurrentCulture);
                            this.lblHeightUOM.Text = this.SkidInfo.CurrSkid.HeightUnit;
                            this.txtHi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Hi,
                                CultureInfo.CurrentCulture);
                            this.txtTi.Text = Convert.ToString(
                                this.SkidInfo.CurrSkid.Ti,
                                CultureInfo.CurrentCulture);
                            this.ExtraInfoItems.Clear();
                            this.ExtraInfoItems.Add(
                                $"Quantity {this.SkidInfo.CurrSkid.Quantity} {this.SkidInfo.CurrSkid.ProdInfo?.UnitCode}" +
                                $" @ {this.SkidInfo.CurrSkid.WeightQuantity} {this.SkidInfo.CurrSkid.ProdInfo?.WeightUnitCode}");
                        });

                    break;

                case SysShared.ActivityType.DetailsEdit:
                    throw new NotImplementedException(
                        "ActivityHdrSkidInfo: ActivityType.DetailsEdit not implemented.");
                    break;

                case SysShared.ActivityType.HeaderFilter:
                    throw new NotImplementedException(
                        "ActivityHdrSkidInfo: ActivityType.HeaderFilter not implemented.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

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

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            try
            {
                await this.ScannerDisable();
                if (scanArgs == null)
                {
                    const string msg = "Scanner event fired but no data was received.";
                    await Device.InvokeOnMainThreadAsync(
                        async () => await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
                            msg,
                            Resource.OK));
                    throw new TaskCanceledException();
                }

                var codeParse = scanArgs.ParseInfo;
                var isLocationBarCodeScanned = !string.IsNullOrEmpty(codeParse.LocationIdentifier);
                var isProductScanned = !string.IsNullOrEmpty(codeParse.ProductIdentifier);
                var isFkRefnoScanned = !string.IsNullOrEmpty(codeParse.ActFkRefno);
                var isFkItemRefnoScanned = !string.IsNullOrEmpty(codeParse.ActItemFkRefno);
                var isFkDetailRefnoScanned = !string.IsNullOrEmpty(codeParse.ActDetailFkRefno);
                var isSkidScanned = !string.IsNullOrEmpty(codeParse.Sscc18);

                if (isLocationBarCodeScanned)
                {
                    await HandleLocationScanned(codeParse);
                }

                if (isFkRefnoScanned)
                {
                    const string msg = "Scanning a header bar code is not supported here!";
                    throw new MessageOnlyException(msg);
                }

                if (isFkItemRefnoScanned)
                {
                    const string msg = "Scanning a line item bar code is not supported here!";
                    throw new MessageOnlyException(msg);
                }

                // Activity detail foreign reference number
                if (isFkDetailRefnoScanned)
                {
                    const string msg =
                        "Scanning an line item detail bar code is not supported here!";
                    throw new MessageOnlyException(msg);
                }

                // Shipping container scanned, skid
                if (isSkidScanned)
                {
                    await HandleSkidScanned(codeParse);
                }

                if (isProductScanned)
                {
                    await HandleProductScanned(codeParse);
                }

                await this.DisplaySkidInfo();
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task cancelled by user
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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
            }

            return;

            async Task HandleSkidScanned(BarCodeParse codeParse)
            {
                try
                {
                    var scannedSkidInfo =
                        await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(codeParse.Sscc18.TrimEnd());
                    switch (this.ActivityType)
                    {
                        case SysShared.ActivityType.Header:
                            break;

                        case SysShared.ActivityType.Items:
                            if (scannedSkidInfo.CurrSkid.SkidFkRefno <= 0)
                            {
                                const string msg =
                                    "This skid does not currently exist. Also creating a new skid" +
                                    " can on be performed on the line item detail level.";
                                throw new MessageOnlyException(msg);
                            }

                            // If the skid scanned is not the same as the current skid.
                            if (this.SkidInfo.CurrSkid.SkidFkRefno > 0 &&
                                scannedSkidInfo.CurrSkid.SkidFkRefno !=
                                this.SkidInfo.CurrSkid.SkidFkRefno)
                            {
                                // If the skid scanned is not in the current warehouse location.
                                if (scannedSkidInfo.CurrSkid.LocMap.WhsLocMapRefno > 0 &&
                                    this.LocMap.WhsLocMapRefno > 0 &&
                                    scannedSkidInfo.CurrSkid.LocMap.WhsLocMapRefno !=
                                    this.LocMap.WhsLocMapRefno)
                                {
                                    const string msg =
                                        "The skid scanned is not in the current warehouse location.";
                                    throw new MessageOnlyException(msg);
                                }

                                // ProdInfo was initially set at the start of the form.
                                // It could be the activity item product or the initial skid product.
                                if (this.IsSkidHasQuantity &&
                                    scannedSkidInfo.CurrSkid.Quantity > 0)
                                {
                                    if (this.ProdInfo.Refno !=
                                        this.SkidInfo.CurrSkid.ProdInfo.Refno)
                                    {
                                        const string msg =
                                            "The scanned skid product does not match the product on the skid.";
                                        throw new MessageOnlyException(msg);
                                    }
                                }
                            }

                            break;

                        case SysShared.ActivityType.Details:
                            if (this.SysEnv.SysSettings.IsPickDisableMakeSkid &&
                                scannedSkidInfo.CurrSkid.SkidFkRefno <= 0 &&
                                this.CallingActSubType == SysCommon.ActSubTypes.Picking)
                            {
                                const string msg =
                                    "Creating a new skid is not allowed in this picking activity." +
                                    " Please check the system preference.";
                                throw new MessageOnlyException(msg);
                            }

                            break;

                        case SysShared.ActivityType.DetailsEdit:
                            break;

                        case SysShared.ActivityType.HeaderFilter:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    this.SkidInfo = scannedSkidInfo;

                    // Update the original product and location with the scanned skid information.
                    // Likely the product and location will be the same as the scanned skid.
                    this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                    this.ProdInfo.Scheme =
                        await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                    this.ProdInfo.Unit2BaseUnitFactor =
                        await ProductInfoTasks.GetUnitRelationFactor(this.ProdInfo);
                    this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = SysCommon.GetMessageFromMessageException(msgEx);
                    await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                GetControlDescription(),
                                msg,
                                Resource.OK));
                }
                catch (Exception ex)
                {
                    var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    _ = await Device.InvokeOnMainThreadAsync(async () => await errRpt.ShowDialog());
                }
            }

            async Task HandleLocationScanned(BarCodeParse codeParse)
            {
                var locMapRefno =
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        codeParse.LocationIdentifier);
                if (locMapRefno <= 0)
                {
                    const string msg = "The scanned location could not be identified.";
                    throw new MessageOnlyException(msg);
                }

                if (this.IsSkidHasQuantity &&
                    locMapRefno != this.SkidInfo.CurrSkid.LocMapRefno)
                {
                    const string msg =
                        "The scanned location must be the same as the skid location once it has quantity.";
                    throw new MessageOnlyException(msg);
                }

                if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
                {
                    const string msg = "The scanned location is not complete.";
                    throw new MessageOnlyException(msg);
                }

                var scannedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                if (scannedLocMap.WhsLocMapRefno != this.CallingWarehouseLocMapRefno)
                {
                    const string msg = "The scanned location must be in the current warehouse.";
                    throw new MessageOnlyException(msg);
                }

                this.LocMap = scannedLocMap;
                ////this.SkidInfo.CurrSkid.LocMap = scannedLocMap.Clone();
                this.IsChangesMade = true;
            }

            async Task HandleProductScanned(BarCodeParse codeParse)
            {
                var prodUnitRefno = await SysShared.IdentifyProductScanned(
                    this.CallingActSubType,
                    codeParse);
                if (prodUnitRefno <= 0)
                {
                    const string msg =
                        "The scanned bar code does not match any associated product.";
                    throw new MessageOnlyException(msg);
                }

                switch (this.ActivityType)
                {
                    case SysShared.ActivityType.Header:

                        // Quantity is considered, the product must match the skid / activity product
                        // otherwise it can be changed.
                        // A skid could be created with a product and then the product changed.
                        // Quantity is considered, the product must match the skid / activity product
                        if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0 &&
                            this.IsSkidHasQuantity &&
                            this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            const string msg =
                                "The product cannot be changed on an existing skid with quantity.";
                            throw new MessageOnlyException(msg);
                        }

                        break;

                    case SysShared.ActivityType.Items:

                        // Quantity is considered, the product must match the skid / activity product
                        // otherwise it can be changed.
                        // A skid could be created with a product and then the product changed.
                        // Quantity is considered, the product must match the skid / activity product
                        if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0 &&
                            this.IsSkidHasQuantity &&
                            this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            const string msg =
                                "The product cannot be changed on an existing skid with quantity.";
                            throw new MessageOnlyException(msg);
                        }

                        break;

                    case SysShared.ActivityType.Details:

                        // Do not consider quantity, the product must match the skid / activity product.
                        if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno > 0 &&
                            this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != prodUnitRefno)
                        {
                            const string msg =
                                "The scanned product does not match the product on the skid.";
                            throw new MessageOnlyException(msg);
                        }

                        break;

                    case SysShared.ActivityType.DetailsEdit:
                        throw new NotImplementedException(
                            "ActivityHdrSkidInfo: ActivityType.DetailsEdit not implemented.");

                    case SysShared.ActivityType.HeaderFilter:
                        throw new NotImplementedException(
                            "ActivityHdrSkidInfo: ActivityType.HeaderFilter not implemented.");

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Update the skid product even though there is no change.
                var prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                if (prodInfo.Refno <= 0)
                {
                    const string msg = "The scanned product could not be identified.";
                    throw new MessageOnlyException(msg);
                }

                prodInfo = this.ProdInfo;
                prodInfo.Scheme =
                    await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);
                if (prodInfo.Scheme.SchemeRefno <= 0)
                {
                    const string msg = "The scanned product scheme could not be identified.";
                    throw new MessageOnlyException(msg);
                }

                prodInfo.Unit2BaseUnitFactor =
                    await ProductInfoTasks.GetUnitRelationFactor(prodInfo);
                this.ProdInfo = ProductInfo.DeepCopy(prodInfo);
                this.SkidInfo.CurrSkid.ProdInfo = this.ProdInfo;
                this.IsChangesMade = true;
            }
        }

        private async void CmdLocation_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (this.IsSkidHasQuantity)
                {
                    throw new MessageOnlyException(
                        "A location can only be assigned to a skid without quantity.");
                }

                var form = new FormDialogContainer(
                    EDialogTypes.SelectLocationMap,
                    SysCommon.ActSubTypes.NotApplicable);

                if (await Device.InvokeOnMainThreadAsync(
                        async () =>
                            await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    var selectedLoc = LocationMap.GetInstance().ChosenLocMap.Clone();
                    if (!await LocationMapTasks.IsCompleteLocMap(selectedLoc.LocMapRefno))
                    {
                        const string msg = "The chosen location is not complete.";
                        throw new MessageOnlyException(msg);
                    }

                    if (selectedLoc.WhsLocMapRefno != this.CallingWarehouseLocMapRefno)
                    {
                        const string msg = "The chosen location must be in the current warehouse.";
                        throw new MessageOnlyException(msg);
                    }

                    if (this.IsSkidHasQuantity &&
                        selectedLoc.LocMapRefno != this.LocMap.LocMapRefno)
                    {
                        const string msg =
                            "The chosen location must be the same as the skid location once it has quantity.";
                        throw new MessageOnlyException(msg);
                    }

                    this.LocMap = selectedLoc.Clone();
                    this.IsChangesMade = true;
                }

                _ = await Device.InvokeOnMainThreadAsync(
                    () =>
                        Task.FromResult(
                            this.cmdLocation.Text =
                                LocationMapTasks.GetReadableLocation(this.LocMap)));
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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

        private async void PictureBoxPrintLabel_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                await this.SaveSkid(this.SkidInfo.CurrSkid);
                await this.InitSkidInfo();
                if (await Device.InvokeOnMainThreadAsync(
                        async () => await this.DisplayAlert(
                            GetControlDescription(),
                            "Print skid label?",
                            "Yes",
                            "No")))
                {
                    SysSkidInfo.PrintLabel(this.SkidInfo);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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

        private async Task<SSkidInfo> FillSkidInfo()
        {
            var tempSkidInfo = this.SkidInfo.CurrSkid;
            tempSkidInfo.SkidCode = this.txtSkidCode.Text;
            tempSkidInfo.Hi =
                string.IsNullOrEmpty(this.txtHi.Text) ? 0 : int.Parse(this.txtHi.Text);
            tempSkidInfo.Ti =
                string.IsNullOrEmpty(this.txtTi.Text) ? 0 : int.Parse(this.txtTi.Text);
            tempSkidInfo.Cube = string.IsNullOrEmpty(this.txtCube.Text)
                ? 0
                : decimal.Parse(this.txtCube.Text);
            tempSkidInfo.Height = string.IsNullOrEmpty(this.txtHeight.Text)
                ? 0
                : decimal.Parse(this.txtHeight.Text);
            tempSkidInfo.IsPutAway = this.chkIsPutAway.IsToggled;
            tempSkidInfo.ProdInfo = this.ProdInfo;
            tempSkidInfo.ProdInfo.Scheme =
                await ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo.ProdInfo.Refno);
            tempSkidInfo.ProdInfo.Unit2BaseUnitFactor =
                await ProductInfoTasks.GetUnitRelationFactor(tempSkidInfo.ProdInfo);
            tempSkidInfo.LocMap = this.LocMap;
            return tempSkidInfo;
        }

        /// <summary>
        ///     Perform save skid information.
        /// </summary>
        /// <param name="skidInfo"><see cref="SSkidInfo" /> Skid information</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        private async Task SaveSkid(SSkidInfo skidInfo)
        {
            if (this.IsChangesMade)
            {
                if (this.IsNewSkid)
                {
                    skidInfo.SkidCode = this.txtSkidCode.Text.Trim();
                    this.SkidInfo.CurrSkid = skidInfo;
                    this.SkidInfo =
                        await SysSkidInfo.Create(SysSkidInfo.SkidInfoToXml(this.SkidInfo));
                }
                else
                {
                    this.SkidInfo.CurrSkid = skidInfo;
                    SysSkidInfo.SendUpdate(this.SkidInfo);
                }

                this.IsChangesMade = false;

                // The calling skid info is updated to the current skid info.
                this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
            }
        }

        private async void CmdDetail_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                if (this.SkidInfo.CurrSkid.SkidFkRefno == 0)
                {
                    const string msg = "A skid code is required to see it's details.";
                    throw new MessageOnlyException(msg);
                }

                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap))
                {
                    const string msg = "Please set a complete location first.";
                    throw new MessageOnlyException(msg);
                }

                if (this.IsChangesMade ||
                    this.IsNewSkid)
                {
                    const string msg = "Please save the skid information before attempting to see the details.";
                    throw new MessageOnlyException(msg);
                }

                LocationMap.GetInstance().CurrLocMap = this.LocMap;

                // Create the activity skid detail, activity item.
                var activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.SkidInfo);
                activity.HdrFkRefno = this.SkidInfo.CurrSkid.SkidFkRefno;
                activity.HdrRefno = this.SkidInfo.CurrSkid.SkidFkRefno;
                activity.SkidInfo = this.SkidInfo;

                var actItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.SkidInfo);
                actItem.ItemFkRefno = this.SkidInfo.CurrSkid.SkidFkRefno;
                actItem.ItemRefno = this.SkidInfo.CurrSkid.SkidFkRefno;
                actItem.LocMap = this.SkidInfo.CurrSkid.LocMap;
                actItem.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;

                var form = new FormDialogContainer(
                    EDialogTypes.ActivityDetailSkid,
                    actItem,
                    SysCommon.ActSubTypes.SkidInfo,
                    this.CallingActSubType);

                if (await Device.InvokeOnMainThreadAsync(
                        async () => await form.DisplayControl.ShowDialog() == DialogResult.OK))
                {
                    await form.GetXml();
                    this.IsChangesMade = false;
                    if (this.SkidInfo.CurrSkid.IsConsumed)
                    {
                        this.IsCallerSkidLoaded = true;
                    }
                }

                await this.InitSkidInfo();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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

        private async void CmdComplete_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                this.SkidInfo = SysSkidInfo.Complete(this.SkidInfo);
                await Device.InvokeOnMainThreadAsync(
                    async () => await this.DisplayAlert(
                        GetControlDescription(),
                        "Skid completed.",
                        "OK"));
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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

        private async void CmdSkidProduct_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();

                // Only allow a product to be selected if the skid is new or has no quantity.
                if (this.IsCallerSkidLoaded &&
                    this.ProdInfo.UnitRefno > 0 &&
                    this.IsSkidHasQuantity)
                {
                    const string msg = "A product can only be assigned to a new skid.";
                    throw new MessageOnlyException(msg);
                }

                var form = new FormDialogContainer(
                    EDialogTypes.SelectProductUnit,
                    SysCommon.ActSubTypes.SkidInfo);
                if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                {
                    var xmlDoc = await form.GetXml();
                    if (xmlDoc.Root == null)
                    {
                        throw new MessageOnlyException(
                            "Unable to retrieve selected product information.");
                    }

                    var prodUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno") !.Value);
                    var tempSkidInfo = this.SkidInfo.CurrSkid;
                    tempSkidInfo.ProdInfo =
                        await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    tempSkidInfo.ProdInfo.Scheme =
                        await ProductInfoTasks.GetSchemeComponentsFromRefno(
                            tempSkidInfo.ProdInfo.Refno);
                    tempSkidInfo.ProdInfo.Unit2BaseUnitFactor =
                        await ProductInfoTasks.GetUnitRelationFactor(
                            tempSkidInfo.ProdInfo);
                    this.ProdInfo = tempSkidInfo.ProdInfo;
                    this.IsChangesMade = true;
                }

                this.cmdSkidProduct.Text = "Select Product";

                if (this.ProdInfo != null)
                {
                    if (this.ProdInfo.UnitRefno != 0)
                    {
                        this.cmdSkidProduct.Text =
                            ProductInfoTasks.GetCurrReadableProdBcUnit(this.ProdInfo);
                    }
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Device.InvokeOnMainThreadAsync(
                    async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            GetControlDescription(),
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

        private async void CmdSaveSkid_Click(object sender, EventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                this.IsNewSkid = string.IsNullOrEmpty(this.txtSkidCode.Text);
                if (await this.IsReadyToSave())
                {
                    var msg = this.IsNewSkid ? "Skid created." : "Skid updated.";

                    await this.SaveSkid(this.SkidInfo.CurrSkid);

                    this.IsNewSkid = false;

                    this.IsChangesMade = false;

                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
                        msg,
                        Resource.OK);
                }
                else
                {
                    throw new MessageOnlyException("Could not create a skid.");
                }

                await this.DisplaySkidInfo();
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
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
            }
        }

        private void InitControlText()
        {
            this.Title = GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdSaveSkid.Text = "Save";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";
        }

        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsChangesMade = true;
        }

        private void ChkIsPutAway_Toggled(object sender, ToggledEventArgs e)
        {
            this.IsChangesMade = true;
            this.SkidInfo.CurrSkid.IsPutAway = this.chkIsPutAway.IsToggled;
        }

        private void TxtSkidCode_OnFocused(object sender, FocusEventArgs e)
        {
        }

        private async void TxtSkidCode_OnUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                var skidCode = this.txtSkidCode.Text.Trim();
                if (!string.IsNullOrEmpty(skidCode))
                {
                    var foundSkid = new SysSkidInfo();
                    try
                    {
                        foundSkid = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(skidCode);
                        this.IsNewSkid = false;
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = SysCommon.GetMessageFromMessageException(msgEx).ToUpper();
                        this.IsNewSkid = msg.Contains("DOES NOT EXIST.");
                    }
                    catch
                    {
                        this.IsNewSkid = true;
                    }

                    if (!this.IsNewSkid)
                    {
                        this.SkidInfo = foundSkid;
                        this.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                        this.LocMap = this.SkidInfo.CurrSkid.LocMap;
                        this.CallingSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);
                        this.IsChangesMade = false;
                    }
                    else
                    {
                        // New skid information will be set in the SaveChanges method.
                        this.IsChangesMade = true;
                        if (this.ActivityType == SysShared.ActivityType.Items)
                        {
                            this.SkidInfo = new SysSkidInfo();
                            const string msg =
                                "New skids are not allowed to be created at the item level.";
                            throw new MessageOnlyException(msg);
                        }
                        else
                        {
                            var msg =
                                $"This skid code '{skidCode}' does not exist. A new skid will be created upon saving.";
                            throw new MessageOnlyException(msg);
                        }
                    }

                    await this.DisplaySkidInfo();
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
        }

        private void TxtTi_OnFocused(object sender, FocusEventArgs e)
        {
        }

        private async void TxtTi_OnUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                this.IsChangesMade = true;
                this.SkidInfo.CurrSkid.Ti = string.IsNullOrEmpty(this.txtTi.Text)
                    ? 0
                    : int.Parse(this.txtTi.Text);
                if (this.SkidInfo.CurrSkid.Ti < 0)
                {
                    this.SkidInfo.CurrSkid.Ti = 0;
                    const string msg = "The ti must be greater than or equal to zero.";
                    throw new MessageOnlyException(msg);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
        }

        private void TxtHi_OnFocused(object sender, FocusEventArgs e)
        {
        }

        private async void TxtHi_OnUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                this.IsChangesMade = true;
                this.SkidInfo.CurrSkid.Hi = string.IsNullOrEmpty(this.txtHi.Text)
                    ? 0
                    : int.Parse(this.txtHi.Text);
                if (this.SkidInfo.CurrSkid.Hi < 0)
                {
                    this.SkidInfo.CurrSkid.Hi = 0;
                    const string msg = "The hi must be greater than or equal to zero.";
                    throw new MessageOnlyException(msg);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
        }

        private void TxtHeight_OnFocused(object sender, FocusEventArgs e)
        {
        }

        private async void TxtHeight_OnUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                this.IsChangesMade = true;
                this.SkidInfo.CurrSkid.Height = string.IsNullOrEmpty(this.txtHeight.Text)
                    ? 0
                    : decimal.Parse(this.txtHeight.Text);
                if (this.SkidInfo.CurrSkid.Height < 0)
                {
                    this.SkidInfo.CurrSkid.Height = 0;
                    const string msg = "The height must be greater than or equal to zero.";
                    throw new MessageOnlyException(msg);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
        }

        private void TxtCube_OnFocused(object sender, FocusEventArgs e)
        {
        }

        private async void TxtCube_OnUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                this.IsChangesMade = true;
                this.SkidInfo.CurrSkid.Cube = string.IsNullOrEmpty(this.txtCube.Text)
                    ? 0
                    : decimal.Parse(this.txtCube.Text);
                if (this.SkidInfo.CurrSkid.Cube < 0)
                {
                    this.SkidInfo.CurrSkid.Cube = 0;
                    const string msg = "The cube must be greater than or equal to zero.";
                    throw new MessageOnlyException(msg);
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                await Application.Current.MainPage.DisplayAlert(
                    GetControlDescription(),
                    msg,
                    Resource.OK);
            }
            catch (Exception ex)
            {
                var errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                _ = errRpt.ShowDialog();
            }
        }
    }
}