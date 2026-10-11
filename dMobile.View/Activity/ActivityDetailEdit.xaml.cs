// <copyright file="ActivityDetailEdit.xaml.cs" company="InterSect Business Systems Inc">
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
    using Controller.Activity_Detail;
    using dMobile.Controller.Activity_Item;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    ///     Define the <see cref="ActivityDetailEdit" /> activity detail edit.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetailEdit : IAppArea
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityDetailEdit" /> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail object.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes" /> Activity subtypes.</param>
        public ActivityDetailEdit(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.IsDataLoading = false;
            this.IsIndicatorRunning = false;
            this.IsFirstAppearance = true;
            this.ExtraInfoItems = new ObservableCollection<string>();
            this.ActSubType = actSubTypes;

            this.lblQty.Text = "Quantity";
            this.lblUnit.Text = "KG";
            this.lblBaseQty.Text = "Base Qty";
            this.lblBaseUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.lblWtUnit.Text = "KG";
            this.lblSerialNo.Text = "Serial No";
            this.lblProductionDate.Text = "Production";
            this.lblLotNo.Text = "Lot No";
            this.lblBestBeforeDate.Text = "Best Before";
            this.lblVariantCode.Text = "Variant";

            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit = new ActDetailEdit
            {
                SchemeData = this.ActDetail.SchemeData,
                Quantity = this.ActDetail.Quantity,
                BaseQuantity = this.ActDetail.BaseQuantity,
                WeightQuantity = this.ActDetail.WeightQuantity,
                ActExtra =
                {
                    Text1 = this.ActDetail.ActExtra.Text1,
                    Text2 = this.ActDetail.ActExtra.Text2,
                    Text3 = this.ActDetail.ActExtra.Text3,
                    Text4 = this.ActDetail.ActExtra.Text4,
                    DateTime1 = this.ActDetail.ActExtra.DateTime1
                }
            };

            InitControlState();
            this.ScannerEnable().FireAndForgetSafeAsync();
            this.LoadAndBindData().FireAndForgetSafeAsync();
            return;

            void InitControlState()
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var scheme = prodInfo.Scheme;

                this.Title = this.ActDetail.GetControlDescription();
                this.lblUnit.Text = prodInfo.UnitCode;
                this.lblBaseUnit.Text = prodInfo.BaseUnitCode;
                this.lblWtUnit.Text = prodInfo.WeightUnitCode;

                this.lbl1.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra1Caption;
                this.lbl2.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra2Caption;
                this.lbl3.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra3Caption;
                this.lbl4.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra4Caption;
                this.lbl5.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra5Caption;

                if (!prodInfo.IsCatchWeight || prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                {
                    this.txtWtQty.IsEnabled = false;
                }

                InitControlStateScheme();
                InitControlStateExtra();
                return;

                // Enable scheme controls state.
                void InitControlStateScheme()
                {
                    if (scheme.IsBestBeforeDateTracked)
                    {
                        this.lblBestBeforeDate.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        this.lblBestBeforeDate.FontAttributes = FontAttributes.None;
                        this.dtBestBeforeDate.IsEnabled = false;
                    }

                    if (scheme.IsProductionDateTracked)
                    {
                       this.lblProductionDate.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        this.lblProductionDate.FontAttributes = FontAttributes.None;
                        this.dtProductionDate.IsEnabled = false;
                    }

                    this.dtBestBeforeDate.Date = DateTime.Now;
                    this.dtProductionDate.Date = DateTime.Now;
                    ////this.dtBestBeforeDate.Date = SysCommon.EmptyDateTime;
                    ////this.dtProductionDate.Date = SysCommon.EmptyDateTime;
                    this.dtExtraDetail5.Date = DateTime.Now;
                    if (this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
                    {
                        var lower = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                        this.dtBestBeforeDate.MinimumDate = lower;
                        this.dtProductionDate.MinimumDate = lower;
                        this.dtExtraDetail5.MinimumDate = lower;

                        var upper = DateTime.Now.AddYears(this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);
                        this.dtBestBeforeDate.MaximumDate = upper;
                        this.dtProductionDate.MaximumDate = upper;
                        this.dtExtraDetail5.MaximumDate = upper;
                    }

                    if (scheme.IsLotTracked)
                    {
                        this.lblLotNo.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        this.lblLotNo.FontAttributes = FontAttributes.None;
                        this.txtLotNo.IsEnabled = false;
                    }

                    if (scheme.IsSerialTracked)
                    {
                       this.lblSerialNo.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        this.lblSerialNo.FontAttributes = FontAttributes.None;
                        this.txtSerialNo.IsEnabled = false;
                    }

                    if (scheme.IsVariantTracked)
                    {
                        this.lblVariantCode.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        this.lblVariantCode.FontAttributes = FontAttributes.None;
                        this.txtVariantCode.IsEnabled = false;
                    }
                }

                // Enable extra data controls state.
                void InitControlStateExtra()
                {
                    if (scheme.Extra1Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.lbl1.FontAttributes = FontAttributes.None;
                        this.txtExtraDetail1.IsEnabled = false;
                    }
                    else
                    {
                        this.lbl1.FontAttributes = FontAttributes.Bold;
                    }

                    if (scheme.Extra2Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.lbl2.FontAttributes = FontAttributes.None;
                        this.txtExtraDetail2.IsEnabled = false;
                    }
                    else
                    {
                        this.lbl2.FontAttributes = FontAttributes.Bold;
                    }

                    if (scheme.Extra3Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.lbl3.FontAttributes = FontAttributes.None;
                        this.txtExtraDetail3.IsEnabled = false;
                    }
                    else
                    {
                        this.lbl3.FontAttributes = FontAttributes.Bold;
                    }

                    if (scheme.Extra4Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.lbl4.FontAttributes = FontAttributes.None;
                        this.txtExtraDetail4.IsEnabled = false;
                    }
                    else
                    {
                        this.lbl4.FontAttributes = FontAttributes.Bold;
                    }

                    if (scheme.Extra5Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.lbl5.FontAttributes = FontAttributes.None;
                        this.dtExtraDetail5.IsEnabled = false;
                    }
                    else
                    {
                        this.lbl5.FontAttributes = FontAttributes.Bold;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the data is being loaded.
        /// </summary>
        private bool IsDataLoading { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to print product labels.
        /// </summary>
        private bool AskToPrintProductLabels { get; set; }

        /// <summary>
        ///     Gets the system environment object.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets the activity subtype.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; }

        /// <summary>
        ///     Gets the activity detail edit.
        /// </summary>
        private ActDetailEdit ActDetailEdit { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this is the first form appearance.
        /// </summary>
        private bool IsFirstAppearance { get; }

        /// <summary>
        ///     Gets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; }

        /// <summary>
        ///     Gets the activity detail edit items list.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; }

        /// <summary>
        ///     Gets or sets the bar code parsed data.
        /// </summary>
        private BarCodeParse BarParse { get; set; }

        /// <summary>
        ///     Gets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; } = ScannerEngine.GetInstance().Scanner;

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
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
            ////if (!this.Scanner.IsBarCodeScanEventHandlerAdded())
            ////{
            ////    this.Scanner.OnBarCodeScan += this.BarCodeHandler;
            ////}

            ////await this.Scanner.EnableScanning();
        }

        /// <summary>
        ///     Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool" /> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                await IsValidData();
                this.SysEnv.LastEnteredSchemeData = (ProductSchemeData)this.ActDetailEdit.SchemeData.Clone();
            }
            catch (IsReadyToApplyException applyEx)
            {
                if (applyEx.SuppressError && applyEx.IsOverContinue)
                {
                    this.SysEnv.LastEnteredSchemeData = (ProductSchemeData)this.ActDetailEdit.SchemeData.Clone();
                    isReadyToSave = true;
                }
                else if (applyEx.SuppressError && !applyEx.IsOverContinue)
                {
                    isReadyToSave = false;
                }
                else
                {
                    isReadyToSave = false;
                    var msg = SysCommon.GetMessageFromIsReadyToApplyException(applyEx);
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msg,
                            Resource.OK));
                }
            }
            catch (MessageOnlyException msgEx)
            {
                isReadyToSave = false;
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

            async Task IsValidData()
            {
                await ValidateQuantity();
                await ValidateScheme();
                ValidateSchemeExtra();

                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var uniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                    prodInfo.Code,
                    prodInfo.Scheme,
                    this.ActDetailEdit.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and
                // quantity validations.
                var xml = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                    <itm_refno>{this.ActDetail.ActItem.ItemFkRefno}</itm_refno>
                    <alt_qty>{this.ActDetailEdit.Quantity}</alt_qty>
                    <wt_qty>{this.ActDetailEdit.WeightQuantity}</wt_qty>
                    <ActivityDetailUniqueSchemeID>{uniqueSchemeId}</ActivityDetailUniqueSchemeID>
                    </imclient>";

                var xmlParam = XDocument.Parse(xml);
                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);
                return;

                async Task ValidateQuantity()
                {
                    // Round captured screen quantity to 3 decimals.
                    this.ActDetailEdit.Quantity = Math.Round(decimal.Parse(this.txtAltQty.Text), 3);
                    this.ActDetailEdit.BaseQuantity = Math.Round(decimal.Parse(this.txtBaseQty.Text), 3);
                    this.ActDetailEdit.WeightQuantity = Math.Round(decimal.Parse(this.txtWtQty.Text), 3);

                    // Check if Units of measures are the same quantity should be the same.
                    var productInfo = this.ActDetail.ActItem.ProdInfo;
                    CheckEqualQuantity();

                    if (this.ActDetailEdit.BaseQuantity == 0)
                    {
                        throw new MessageOnlyException("Base quantity is expected to have a value not equal to zero.");
                    }

                    if (this.ActDetailEdit.WeightQuantity == 0)
                    {
                        throw new MessageOnlyException("Weight quantity is expected to have a value not equal to zero.");
                    }

                    // Alt quantity equal to zero will be considered valid for these activity subtypes.
                    decimal expectedWeightQty = 0;
                    if (this.ActDetail.ActItem.ProdInfo.IsBaseWeightUnit)
                    {
                        expectedWeightQty = this.ActDetailEdit.BaseQuantity;
                    }
                    else
                    {
                        ////expectedWeightQty = Math.Round(this.ActDetailEdit.BaseQuantity * productInfo.UnitGrossWeight, 3);
                        expectedWeightQty = Math.Round(this.ActDetailEdit.Quantity * productInfo.UnitGrossWeight, 3);
                    }

                    if (Math.Sign(this.ActDetailEdit.Quantity) == 0 &&
                        (this.ActDetail.ActSubType == SysCommon.ActSubTypes.Adjustment ||
                         this.ActDetail.ActSubType == SysCommon.ActSubTypes.TicketMaterial))
                    {
                        // Alt quantity is not validated.
                        await ValidateBaseAndWeightAsync();
                    }
                    else
                    {
                        await ValidateAltBaseAndWeightAsync();
                    }

                    return;

                    // Check the quantity if the UOM match.
                    void CheckEqualQuantity()
                    {
                        if (productInfo.UnitCode == productInfo.BaseUnitCode &&
                            this.ActDetailEdit.Quantity != this.ActDetailEdit.BaseQuantity)
                        {
                            throw new MessageOnlyException("Quantity and base quantity are expected" +
                                                           " to be the same, because the units are the same.");
                        }

                        if (productInfo.UnitCode == productInfo.WeightUnitCode &&
                            this.ActDetailEdit.Quantity != this.ActDetailEdit.WeightQuantity)
                        {
                            throw new MessageOnlyException("Quantity and weight quantity are expected" +
                                                           " to be the same, because the units are the same.");
                        }

                        if (productInfo.BaseUnitCode == productInfo.WeightUnitCode &&
                            this.ActDetailEdit.BaseQuantity != this.ActDetailEdit.WeightQuantity)
                        {
                            throw new MessageOnlyException("Base quantity and weight quantity are expected" +
                                                           " to be the same, because the units are the same.");
                        }
                    }

                    async Task ValidateBaseAndWeightAsync()
                    {
                        // Base and weight quantity sign check.
                        const int totalSignQtyBaseWeight = 2;
                        var totalSign = Math.Sign(this.ActDetailEdit.BaseQuantity) +
                                        Math.Sign(this.ActDetailEdit.WeightQuantity);
                        if (totalSign != totalSignQtyBaseWeight && totalSign != -totalSignQtyBaseWeight)
                        {
                            throw new MessageOnlyException(
                                "The sign of the base and weight quantity must be the same.");
                        }

                        if (this.SysEnv.SysSettings.IsIntegerCheckOverride)
                        {
                            // If the alt unit and base unit are the same. Quantity check was already done.
                            // If the base and weight unit are the same. Quantity check was already done.
                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }

                                if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException(
                                        $"Product '{productInfo.Code}' cannot be flagged as" +
                                        " catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException(
                                    $"Product '{productInfo.Code}' cannot be flagged as" +
                                    " catch weight and the base unit code not be a weighted unit.");
                            }
                            else
                            {
                                // Catch count is validate the same as here.
                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $" {expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                        else
                        {
                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight &&
                                    expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }

                                if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException(
                                        $"Product '{productInfo.Code}' cannot be flagged as" +
                                        " catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                                                " catch weight and the base unit code not be a weighted unit.");
                            }
                            else
                            {
                                // Catch count is validate the same as here.
                                // Integer check is performed, yet any whole number is valid because we have no
                                // Alt quantity to base a validation on.
                                if (Math.Abs(this.ActDetailEdit.BaseQuantity) != this.ActDetailEdit.BaseQuantity)
                                {
                                    // Warn with message box, do not raise message info exception.
                                    await Device.InvokeOnMainThreadAsync(async () =>
                                        await Application.Current.MainPage.DisplayAlert(
                                            this.ActDetail.GetControlDescription(),
                                            "Warning! Base quantity is expected to be a whole number.",
                                            Resource.OK));
                                }

                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $" {expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                    }

                    async Task ValidateAltBaseAndWeightAsync()
                    {
                        // Alt, base and weight quantity sign check.
                        const int totalSignQtyBaseWeight = 3;
                        var totalSign = Math.Sign(this.ActDetailEdit.Quantity) +
                                        Math.Sign(this.ActDetailEdit.BaseQuantity) +
                                        Math.Sign(this.ActDetailEdit.WeightQuantity);
                        if (totalSign != totalSignQtyBaseWeight && totalSign != -totalSignQtyBaseWeight)
                        {
                            throw new MessageOnlyException(
                                "The sign of the quantity, base, and weight quantity must be the same.");
                        }

                        if (this.SysEnv.SysSettings.IsIntegerCheckOverride)
                        {
                            // If the alt unit and base unit are the same. Quantity check was already done.
                            // If the base and weight unit are the same. Quantity check was already done.
                            if (productInfo.UnitCode == "LB" || productInfo.UnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight &&
                                    expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }

                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight &&
                                    expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }

                                if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException(
                                        $"Product '{productInfo.Code}' cannot be flagged as" +
                                        " catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                                                " catch weight and the base unit code not be a weighted unit.");
                            }
                            else if (productInfo.IsCatchCount)
                            {
                                // Any alt quantity is valid.
                                // Any base quantity is valid.
                                // Weight must be match calculated.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $" {expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                            else
                            {
                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                        else
                        {
                            // If the alt unit and base unit are the same. Quantity check was already done.
                            // If the base and weight unit are the same. Quantity check was already done.
                            if (productInfo.UnitCode == "LB" || productInfo.UnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight &&
                                    expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }

                                if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                            }
                            else if (Math.Abs(this.ActDetailEdit.Quantity) != this.ActDetailEdit.Quantity)
                            {
                                // Warn with message box, do not raise message info exception.
                                await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActDetail.GetControlDescription(),
                                        "Warning! Quantity is expected to be a whole number.",
                                        Resource.OK));
                            }

                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight &&
                                    expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }

                                if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException(
                                        $"Product '{productInfo.Code}' cannot be flagged as" +
                                        " catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                                                " catch weight and the base unit code not be a weighted unit.");
                            }
                            else if (productInfo.IsCatchCount)
                            {
                                // Any alt quantity is valid.
                                // Any base quantity is valid.
                                // Weight must be match calculated.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                            else if (Math.Abs(this.ActDetailEdit.BaseQuantity) != this.ActDetailEdit.BaseQuantity)
                            {
                                // Warn with message box, do not raise message info exception.
                                await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.ActDetail.GetControlDescription(),
                                        "Warning! Base quantity is expected to be a whole number.",
                                        Resource.OK));
                            }
                            else
                            {
                                var expectedBaseQty = Math.Round(
                                    this.ActDetailEdit.Quantity * productInfo.Unit2BaseUnitFactor, 3);
                                if (this.ActDetailEdit.BaseQuantity != expectedBaseQty)
                                {
                                    throw new MessageOnlyException($"Base quantity {this.ActDetailEdit.BaseQuantity}" +
                                                                   $" {productInfo.BaseUnitCode} does not match the expected base quantity" +
                                                                   $"{expectedBaseQty} {productInfo.BaseUnitCode}.");
                                }

                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException(
                                        $"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $" {expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                    }
                }

                async Task ValidateScheme()
                {
                    await AssignScheme();
                    ValidateSchemeSerialTracked();
                    ValidateSchemeLotTracked();
                    ValidateSchemeBestBeforeDateTracked();
                    ValidateSchemeProductionDateTracked();
                    ValidateSchemeIsVariantTracked();
                    return;

                    async Task AssignScheme()
                    {
                        var scheme = this.ActDetailEdit.SchemeData;
                        var schemeSingleTradeItemSerialNumber = this.txtSerialNo.Text;
                        if (schemeSingleTradeItemSerialNumber != null)
                        {
                            scheme.SingleTradeItemSerialNumber = schemeSingleTradeItemSerialNumber;
                        }

                        var schemeLogisticUnitBatchLotNumber = this.txtLotNo.Text;
                        if (schemeLogisticUnitBatchLotNumber != null)
                        {
                            scheme.LogisticUnitBatchLotNumber = schemeLogisticUnitBatchLotNumber;
                        }

                        scheme.LogisticUnitProductionDate = this.dtProductionDate.Date;
                        scheme.LogisticUnitBestBeforeSellByDate = this.dtBestBeforeDate.Date;
                        scheme = await AssignSchemeVariant(scheme);
                        this.ActDetailEdit.SchemeData = scheme;
                    }

                    void ValidateSchemeSerialTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsSerialTracked &&
                            this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber == string.Empty)
                        {
                            throw new MessageOnlyException("Serial number required.");
                        }
                    }

                    void ValidateSchemeLotTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsLotTracked &&
                            this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
                        {
                            throw new MessageOnlyException("Lot number required.");
                        }
                    }

                    void ValidateSchemeBestBeforeDateTracked()
                    {
                        // Validate best before date empty.
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                            SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitBestBeforeSellByDate))
                        {
                            throw new MessageOnlyException("Best before date required.");
                        }

                        // Validate best before date range.
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                            this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
                        {
                            var lower = DateTime.Now.AddYears(
                                this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                            var upper = DateTime.Now.AddYears(
                                this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                            if (this.dtBestBeforeDate.Date < lower || this.dtBestBeforeDate.Date > upper)
                            {
                                throw new MessageOnlyException(
                                    "The entered best before date is outside an acceptable range.");
                            }
                        }

                        this.CheckBestBeforeAcceptedDays();
                    }

                    void ValidateSchemeProductionDateTracked()
                    {
                        // Validate production date empty.
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                            SysCommon.IsEmptyDateTime(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
                        {
                            throw new MessageOnlyException("Production date required.");
                        }

                        // Validate production date range.
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                            this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange != 0)
                        {
                            var lower = DateTime.Now.AddYears(
                                this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange * -1);
                            var upper = DateTime.Now.AddYears(
                                this.SysEnv.SysSettings.DetailDateMinMaxValidYearRange);

                            if (this.dtProductionDate.Date < lower || this.dtProductionDate.Date > upper)
                            {
                                throw new MessageOnlyException(
                                    "The entered production date is outside an acceptable range.");
                            }
                        }

                        this.CheckProductionAcceptedDays();
                    }

                    void ValidateSchemeIsVariantTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked &&
                            this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
                        {
                            throw new MessageOnlyException("Variant code required.");
                        }
                    }

                    async Task<ProductSchemeData> AssignSchemeVariant(ProductSchemeData scheme)
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked)
                        {
                            scheme.VariantNumber = Convert.ToInt32(this.txtVariantCode.Text);
                            scheme = await ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(scheme);
                        }

                        return scheme;
                    }
                }

                void ValidateSchemeExtra()
                {
                    var scheme = this.ActDetail.ActItem.ProdInfo.Scheme;

                    AssignSchemeExtraDefault();

                    ValidateSchemeExtra1Required();
                    ValidateSchemeExtra2Required();
                    ValidateSchemeExtra3Required();
                    ValidateSchemeExtra4Required();
                    ValidateSchemeExtra5Required();
                    return;

                    void ValidateSchemeExtra1Required()
                    {
                        if (scheme.Extra1Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra1IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text1))
                        {
                            throw new MessageOnlyException(scheme.Extra1Caption.TrimEnd() + " is required.");
                        }
                    }

                    void ValidateSchemeExtra2Required()
                    {
                        if (scheme.Extra2Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra2IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text2))
                        {
                            throw new MessageOnlyException(scheme.Extra2Caption.TrimEnd() + " is required.");
                        }
                    }

                    void ValidateSchemeExtra3Required()
                    {
                        if (scheme.Extra3Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra3IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text3))
                        {
                            throw new MessageOnlyException(scheme.Extra3Caption.TrimEnd() + " is required.");
                        }
                    }

                    void ValidateSchemeExtra4Required()
                    {
                        if (scheme.Extra4Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra4IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text4))
                        {
                            throw new MessageOnlyException(scheme.Extra4Caption.TrimEnd() + " is required.");
                        }
                    }

                    void ValidateSchemeExtra5Required()
                    {
                        if (scheme.Extra5Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra5IsRequired &&
                            SysCommon.IsEmptyDate(this.ActDetailEdit.ActExtra.DateTime1))
                        {
                            throw new MessageOnlyException(scheme.Extra5Caption.TrimEnd() + " is required.");
                        }
                    }

                    void AssignSchemeExtraDefault()
                    {
                        this.ActDetailEdit.ActExtra.Text1 = this.txtExtraDetail1.Text ?? string.Empty;
                        this.ActDetailEdit.ActExtra.Text2 = this.txtExtraDetail2.Text ?? string.Empty;
                        this.ActDetailEdit.ActExtra.Text3 = this.txtExtraDetail3.Text ?? string.Empty;
                        this.ActDetailEdit.ActExtra.Text4 = this.txtExtraDetail4.Text ?? string.Empty;
                        this.ActDetailEdit.ActExtra.DateTime1 = this.dtExtraDetail5.Date;
                    }
                }
            }
        }

        /// <summary>
        ///     Check if the activity detail edit is ready to close.
        /// </summary>
        /// <returns>
        ///     <see cref="bool" /> True if the activity detail edit is ready to close,
        ///     else false.
        /// </returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        GetControlDescription(),
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
        ///     Save any activity detail edit data changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = true;

            try
            {
                if (await this.IsReadyToSave())
                {
                    var uniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                        this.ActDetail.ActItem.ProdInfo.Code,
                        this.ActDetail.ActItem.ProdInfo.Scheme,
                        this.ActDetailEdit.SchemeData);

                    var schemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                        this.ActDetail.ActItem.ProdInfo.Code,
                        this.ActDetail.ActItem.ProdInfo.Scheme,
                        this.ActDetailEdit.SchemeData);

                    this.ActDetail.UniqueSchemeId = uniqueSchemeId;
                    this.ActDetail.SchemeId = schemeId;
                    this.ActDetail.SchemeData = this.ActDetailEdit.SchemeData;
                    this.ActDetail.Quantity = this.ActDetailEdit.Quantity;
                    this.ActDetail.BaseQuantity = this.ActDetailEdit.BaseQuantity;
                    this.ActDetail.WeightQuantity = this.ActDetailEdit.WeightQuantity;
                    this.ActDetail.ActExtra.Text1 = this.ActDetailEdit.ActExtra.Text1;
                    this.ActDetail.ActExtra.Text2 = this.ActDetailEdit.ActExtra.Text2;
                    this.ActDetail.ActExtra.Text3 = this.ActDetailEdit.ActExtra.Text3;
                    this.ActDetail.ActExtra.Text4 = this.ActDetailEdit.ActExtra.Text4;
                    this.ActDetail.ActExtra.DateTime1 = this.ActDetailEdit.ActExtra.DateTime1;

                    this.AskToPrintProductLabels = false;

                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                        this.SysEnv.SysSettings.CanPrintProductLabelReceiving)
                    {
                        this.AskToPrintProductLabels = true;
                    }
                    else if ((this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                              this.ActSubType == SysCommon.ActSubTypes.TransferTo) &&
                             this.SysEnv.SysSettings.CanPrintProductLabelTransfers)
                    {
                        this.AskToPrintProductLabels = true;
                    }

                    this.IsChangesMade = false;
                }
                else
                {
                    saveComplete = false;
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = SysCommon.GetMessageFromMessageException(msgEx);
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActDetail.GetControlDescription(),
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
        ///     Cancel any pending activity detail edit changes.
        /// </summary>
        /// <returns><see cref="bool" /> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (await this.IsReadyToClose())
                {
                    this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                    this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                    this.ActDetailEdit.ActExtra.Text1 = this.ActDetail.ActExtra.Text1;
                    this.ActDetailEdit.ActExtra.Text2 = this.ActDetail.ActExtra.Text2;
                    this.ActDetailEdit.ActExtra.Text3 = this.ActDetail.ActExtra.Text3;
                    this.ActDetailEdit.ActExtra.Text4 = this.ActDetail.ActExtra.Text4;
                    this.ActDetailEdit.ActExtra.DateTime1 = this.ActDetail.ActExtra.DateTime1;
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

                cancelChanges = false;
            }

            return cancelChanges;
        }

        /// <summary>
        ///     Get the activity detail edit return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument" /> Activity item return information.</returns>
        public async Task<XDocument> GetXml()
        {
            return await SysShared.GetXmlFromActivityDetailEdit(this.ActDetailEdit);
        }

        /// <summary>
        ///     On appearing event handler.
        /// </summary>
        protected override async void OnAppearing()
        {
            ////this.IsIndicatorRunning = true;
            base.OnAppearing();
            SubscribeKeyUpHandlers();

            // Set the focus to the first control, if this is the first appearance.
            if (this.IsFirstAppearance)
            {
                try
                {
                    this.CheckProductionAcceptedDays();
                    this.CheckBestBeforeAcceptedDays();
                }
                catch (MessageOnlyException msgEx)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActDetail.GetControlDescription(),
                            msgEx.Message,
                            Resource.OK));
                }

                await this.SetStartingFocus();
            }

            return;

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), KeyUpHandlerCallback);
                return;

                async void KeyUpHandlerCallback(string keyPressed)
                {
                    if (this.IsDataLoading)
                    {
                        return;
                    }

                    await this.ScannerDisable();

                    // TODO: Verify key codes.
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.F1 || keycode == Keycode.Enter)
                    {
                        await AppearingEnterKey();
                    }

                    await this.ScannerEnable();
                }

                async Task AppearingEnterKey()
                {
                    if (await this.SaveChanges())
                    {
                        if (this.AskToPrintProductLabels &&
                            !this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                        {
                            await AskToPrintProductLabels();
                        }

                        this.DialogResult = DialogResult.OK;
                        await this.ScannerDisable();
                        await this.Close();
                    }

                    return;

                    async Task AskToPrintProductLabels()
                    {
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    this.ActDetail.GetControlDescription(),
                                    "Print product labels?",
                                    "Yes",
                                    "No")))
                        {
                            try
                            {
                                await this.PrintProductLabels();
                            }
                            catch (MessageOnlyException msgEx)
                            {
                                var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                                    ? msgEx.Message
                                    : msgEx.Message["MSGBOX-".Length..];
                                await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        GetControlDescription(),
                                        msg,
                                        Resource.OK));
                            }
                            catch (Exception ex)
                            {
                                // Even though an error was raised, during label printing
                                // allow the control to close.
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    var errRpt = new FormErrorReport(
                                        this.ActSubType,
                                        ex);
                                    _ = await errRpt.ShowDialog();
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ScannerDisable();
            SysCommon.ShutdownConnection();
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        ///     On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool" /> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(CancelAndClose);
            return true;

            async void CancelAndClose()
            {
                if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.ScannerDisable();
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            }
        }

        private static string GetControlDescription()
        {
            return "Activity Detail Editing";
        }

        private async Task LoadAndBindData()
        {
            try
            {
                if (!this.IsDataLoading)
                {
                    this.IsDataLoading = true;
                    this.IsIndicatorRunning = true;
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        AssignExtraInfoControl();
                        AssignAltQuantityToControls();
                        AssignBaseQuantityToControls();
                        AssignWeightQuantityToControls();
                        AssignSchemeDataToControls();
                        AssignExtraDetailToControls();
                    });
                }
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
                this.IsIndicatorRunning = false;
                this.IsDataLoading = false;
            }

            return;

            void AssignExtraInfoControl()
            {
                this.ExtraInfoItems.Clear();

                this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code +
                                        this.ActDetail.ActItem.ProdInfo.UnitBc + "  " +
                                        this.ActDetail.ActItem.ProdInfo.Desc);
                this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
                this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            }

            // Load the extra detail controls with the activity detail extra values.
            void AssignExtraDetailToControls()
            {
                if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text1))
                {
                    this.txtExtraDetail1.Text = this.ActDetail.ActExtra.Text1;
                }

                if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text2))
                {
                    this.txtExtraDetail2.Text = this.ActDetail.ActExtra.Text2;
                }

                if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text3))
                {
                    this.txtExtraDetail3.Text = this.ActDetail.ActExtra.Text3;
                }

                if (!string.IsNullOrEmpty(this.ActDetail.ActExtra.Text4))
                {
                    this.txtExtraDetail4.Text = this.ActDetail.ActExtra.Text4;
                }

                if (!SysCommon.IsEmptyDate(this.ActDetail.ActExtra.DateTime1))
                {
                    this.dtExtraDetail5.Date = this.ActDetail.ActExtra.DateTime1;
                }
            }

            // Load the scheme controls with the activity detail scheme values.
            void AssignSchemeDataToControls()
            {
                var editSchemeData = this.ActDetailEdit.SchemeData;
                var prodInfo = this.ActDetail.ActItem.ProdInfo;

                if (prodInfo.Scheme.IsBestBeforeDateTracked)
                {
                    if (editSchemeData.LogisticUnitBestBeforeSellByDate.Date == DateTime.Now.Date &&
                        !SysCommon.IsEmptyDate(this.SysEnv.LastEnteredSchemeData.LogisticUnitBestBeforeSellByDate))
                    {
                        editSchemeData.LogisticUnitBestBeforeSellByDate = this.SysEnv.LastEnteredSchemeData.LogisticUnitBestBeforeSellByDate;
                        this.dtBestBeforeDate.Date = editSchemeData.LogisticUnitBestBeforeSellByDate;
                    }
                    else
                    {
                        this.dtBestBeforeDate.Date = editSchemeData.LogisticUnitBestBeforeSellByDate;
                    }
                }

                if (prodInfo.Scheme.IsProductionDateTracked)
                {
                    if (editSchemeData.LogisticUnitProductionDate.Date == DateTime.Now.Date &&
                        !SysCommon.IsEmptyDate(this.SysEnv.LastEnteredSchemeData.LogisticUnitProductionDate))
                    {
                        editSchemeData.LogisticUnitProductionDate = this.SysEnv.LastEnteredSchemeData.LogisticUnitProductionDate;
                        this.dtProductionDate.Date = editSchemeData.LogisticUnitProductionDate;
                    }
                    else
                    {
                        this.dtProductionDate.Date = editSchemeData.LogisticUnitProductionDate;
                    }
                }

                if (prodInfo.Scheme.IsSerialTracked)
                {
                    if (string.IsNullOrEmpty(editSchemeData.SingleTradeItemSerialNumber))
                    {
                        editSchemeData.SingleTradeItemSerialNumber =
                            ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                        this.ActDetailEdit.SchemeData = editSchemeData;
                    }

                    this.txtSerialNo.Text = editSchemeData.SingleTradeItemSerialNumber;
                }

                if (prodInfo.Scheme.IsLotTracked)
                {
                    if (string.IsNullOrEmpty(editSchemeData.LogisticUnitBatchLotNumber) &&
                        !string.IsNullOrEmpty(this.SysEnv.LastEnteredSchemeData.LogisticUnitBatchLotNumber))
                    {
                        editSchemeData.LogisticUnitBatchLotNumber = this.SysEnv.LastEnteredSchemeData.LogisticUnitBatchLotNumber;
                        this.txtLotNo.Text = editSchemeData.LogisticUnitBatchLotNumber;
                    }
                    else
                    {
                        this.txtLotNo.Text = editSchemeData.LogisticUnitBatchLotNumber;
                    }
                }

                if (prodInfo.Scheme.IsVariantTracked)
                {
                    if (string.IsNullOrEmpty(editSchemeData.VariantCode) &&
                        !string.IsNullOrEmpty(this.SysEnv.LastEnteredSchemeData.VariantCode))
                    {
                        editSchemeData.VariantNumber = this.SysEnv.LastEnteredSchemeData.VariantNumber;
                        editSchemeData.VariantDescription = this.SysEnv.LastEnteredSchemeData.VariantDescription;
                        editSchemeData.VariantCode = this.SysEnv.LastEnteredSchemeData.VariantCode;
                        this.txtLotNo.Text = editSchemeData.VariantCode;
                    }
                    else
                    {
                        this.txtVariantCode.Text = editSchemeData.VariantCode;
                    }
                }
            }

            void AssignAltQuantityToControls()
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var isSetCatchWeightZero = this.ActDetail.ActItem.ProdInfo.IsCatchWeight
                                           && this.SysEnv.SysSettings.IsSetCatchWeightZero &&
                                           (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG");
                var isSetCatchCountZero = this.ActDetail.ActItem.ProdInfo.IsCatchCount &&
                                          this.SysEnv.SysSettings.IsSetCatchCountZero &&
                                          prodInfo.UnitCode == prodInfo.BaseUnitCode;

                if (this.ActDetail.ActItem.IsCatchAskZero())
                {
                    this.ActDetailEdit.Quantity = 0;
                }
                else if (isSetCatchWeightZero)
                {
                    this.ActDetailEdit.Quantity = 0;
                }
                else if (isSetCatchCountZero)
                {
                    this.ActDetailEdit.Quantity = 0;
                }
                else if (this.ActDetail.Quantity == 0)
                {
                    if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                    {
                        this.ActDetailEdit.Quantity = this.ActDetail.WeightQuantity != 0
                            ? this.ActDetail.WeightQuantity
                            : prodInfo.UnitGrossWeight;
                    }
                    else if (prodInfo.UnitCode == prodInfo.BaseUnitCode && this.ActDetailEdit.BaseQuantity != 0)
                    {
                        this.ActDetailEdit.Quantity = this.ActDetailEdit.BaseQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.Quantity = 1;
                    }
                }
                else
                {
                    this.ActDetailEdit.Quantity = this.ActDetail.Quantity;
                }

                this.ActDetailEdit.Quantity = decimal.Round(this.ActDetailEdit.Quantity, 3);
                this.txtAltQty.Text = this.ActDetailEdit.Quantity.ToString(CultureInfo.CurrentCulture);
            }

            void AssignBaseQuantityToControls()
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                if (this.ActDetail.ActItem.IsCatchAskZero())
                {
                    this.ActDetailEdit.BaseQuantity = 0;
                }
                else if (this.ActDetail.ActItem.ProdInfo.IsCatchWeight &&
                         this.SysEnv.SysSettings.IsSetCatchWeightZero &&
                         (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG"))
                {
                    this.ActDetailEdit.BaseQuantity = 0;
                }
                else if (this.ActDetail.ActItem.ProdInfo.IsCatchCount &&
                         this.SysEnv.SysSettings.IsSetCatchCountZero &&
                         prodInfo.UnitCode == prodInfo.BaseUnitCode)
                {
                    this.ActDetailEdit.BaseQuantity = 0;
                }
                else if (this.ActDetail.BaseQuantity == 0)
                {
                    if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.WeightQuantity != 0
                            ? this.ActDetail.WeightQuantity
                            : prodInfo.UnitGrossWeight;
                    }
                    else if (prodInfo.UnitCode == prodInfo.BaseUnitCode && this.ActDetailEdit.Quantity != 0)
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity;
                    }
                    else
                    {
                        if (this.ActDetailEdit.Quantity != 0 && prodInfo.Unit2BaseUnitFactor != 0)
                        {
                            this.ActDetailEdit.BaseQuantity =
                                this.ActDetailEdit.Quantity * prodInfo.Unit2BaseUnitFactor;
                        }
                        else
                        {
                            this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.Quantity;
                        }
                    }
                }
                else
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                }

                this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
                this.txtBaseQty.Text = this.ActDetailEdit.BaseQuantity.ToString(CultureInfo.CurrentCulture);
            }

            void AssignWeightQuantityToControls()
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                if (this.ActDetail.ActItem.IsCatchAskZero())
                {
                    this.ActDetailEdit.WeightQuantity = 0;
                }
                else if (this.ActDetail.ActItem.ProdInfo.IsCatchWeight &&
                         this.SysEnv.SysSettings.IsSetCatchWeightZero &&
                         (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG"))
                {
                    this.ActDetailEdit.WeightQuantity = 0;
                }
                else if (this.ActDetail.WeightQuantity == 0)
                {
                    if (this.ActDetailEdit.BaseQuantity != 0 &&
                        (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG"))
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetailEdit.BaseQuantity;
                    }
                    else if (this.ActDetailEdit.Quantity != 0 &&
                             (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG"))
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetailEdit.Quantity;
                    }
                    else if (this.ActDetailEdit.BaseQuantity != 0)
                    {
                        ////this.ActDetailEdit.WeightQuantity =
                        ////    this.ActDetailEdit.BaseQuantity * prodInfo.UnitGrossWeight;
                        this.ActDetailEdit.WeightQuantity =
                            this.ActDetailEdit.Quantity * prodInfo.UnitGrossWeight;
                    }
                }
                else
                {
                    this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
                }

                this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);
                this.txtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString(CultureInfo.CurrentCulture);
            }
        }

        private async Task SetStartingFocus()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                _ = this.SysEnv.SysSettings.IsDetailEditScreenSetFocus2CatchWeight
                    ? prodInfo.IsCatchWeight ? this.txtWtQty.Focus() : this.txtAltQty.Focus()
                    : prodInfo.Scheme.IsSerialTracked && prodInfo.IsCatchWeight
                        ? this.txtBaseQty.Focus()
                        : this.txtAltQty.Focus();
            });
        }

        private async Task PrintProductLabels()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (this.ActDetail.ActSubType == SysCommon.ActSubTypes.Receiving ||
                this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferTo)
            {
                var actDetailLabel = ActDetailFacade.GetActDetail(
                    this.ActDetail.ActItem,
                    this.ActSubType);

                actDetailLabel.DetailRefno = this.ActDetail.DetailRefno;
                actDetailLabel.FullClassName = this.ActDetail.FullClassName;
                actDetailLabel.ActItem = this.ActDetail.ActItem;
                actDetailLabel.ActSubType = this.ActSubType;
                actDetailLabel.DetailFkRefno = this.ActDetail.DetailFkRefno;
                actDetailLabel.SelectedLocMap = this.ActDetail.SelectedLocMap;

                actDetailLabel.Quantity = this.ActDetailEdit.Quantity;
                actDetailLabel.BaseQuantity = this.ActDetailEdit.BaseQuantity;
                actDetailLabel.WeightQuantity = this.ActDetailEdit.WeightQuantity;
                actDetailLabel.ActExtra = this.ActDetailEdit.ActExtra;

                var defaultSchemeData = new ActItemSchemeData();
                var defaultSchemeDataAssigned = false;

                var labelSchemeData = new ParseLabelSchemeData(
                    this.ActDetail.ActItem.ProdInfo.Scheme,
                    this.BarParse,
                    defaultSchemeData,
                    this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid);
                actDetailLabel.SchemeData = labelSchemeData.GatherLabelSchemeData(
                    actDetailLabel.SchemeData,
                    ref defaultSchemeDataAssigned);

                var editLabelBeforePrint = !this.ActDetail.IsLabelSchemeComplete(
                    this.ActDetailEdit.SchemeData) || this.SysEnv.SysSettings.IsProductLabelCatchAsk;
                if (editLabelBeforePrint)
                {
                    // Merge of scheme data and label scheme data.
                    actDetailLabel.SchemeData = this.ActDetailEdit.SchemeData;
                    actDetailLabel.LabelSchemeData = this.ActDetailEdit.SchemeData;

                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityProductLabel,
                        actDetailLabel,
                        this.ActSubType);
                    _ = await form.DisplayControl.ShowDialog();

                    // Printing of product labels will be handled in ucActivityProductLabel
                }
                else
                {
                    ////var form = new FormDialogContainer(
                    ////    EDialogTypes.ActivityProductLabelQuantity,
                    ////    this.ActDetail,
                    ////    this.ActSubType);
                    ////_ = await form.DisplayControl.ShowDialog();

                    var sendReq = await this.ActDetail.CreateSubmitProductLabelXml(
                        await SysShared.GetXmlFromActivityDetailEdit(this.ActDetailEdit));
                    _ = SysCommon.SubmitComm(sendReq);
                }
            }
        }

        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            var message = string.Empty;

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

                this.BarParse = scanArgs.ParseInfo;
                if (!string.IsNullOrEmpty(this.BarParse.ActFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity header reference # is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ActItemFkRefno))
                {
                    throw new MessageOnlyException("Scanning an activity item reference # is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.ProductIdentifier))
                {
                    throw new MessageOnlyException("Scanning a product bar code is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.Sscc18))
                {
                    throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                }

                if (!string.IsNullOrEmpty(this.BarParse.LocationIdentifier))
                {
                    throw new MessageOnlyException("Scanning a location identifier bar code is not supported here!");
                }

                throw new MessageOnlyException("Scanned bar code type is not supported here!");
            }
            catch (TaskCanceledException)
            {
                // Do nothing.
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
                await this.ScannerEnable();
            }
        }

        private void CheckProductionAcceptedDays()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                this.ActDetail.ActItem.ProdInfo.ProductAcceptDays > 0 &&
                this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                this.ActDetail.SchemeData.LogisticUnitProductionDate > new DateTime(1900, 1, 1))
            {
                var acceptDate = this.ActDetail.SchemeData.LogisticUnitProductionDate.AddDays(
                    this.ActDetail.ActItem.ProdInfo.ProductAcceptDays);
                if (acceptDate < DateTime.Now.Date)
                {
                    var msg = $"Production date {acceptDate:MM/dd/yyyy} must be greater than {DateTime.Now.Date:MM/dd/yyyy}.";
                    throw new MessageOnlyException(msg);
                }
            }
        }

        private void CheckBestBeforeAcceptedDays()
        {
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                this.ActDetail.ActItem.ProdInfo.ProductAcceptDays > 0 &&
                this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                this.ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate > new DateTime(1900, 1, 1))
            {
                var acceptDate = DateTime.Now.AddDays(this.ActDetail.ActItem.ProdInfo.ProductAcceptDays);
                if (this.ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate <= acceptDate)
                {
                    var msg = $"Best before date {this.ActDetail.SchemeData.LogisticUnitBestBeforeSellByDate:MM/dd/yyyy}" +
                              $" must be greater than {acceptDate:MM/dd/yyyy}.";
                    throw new MessageOnlyException(msg);
                }
            }
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                if (this.AskToPrintProductLabels && !this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
                {
                    if (await Device.InvokeOnMainThreadAsync(async () =>
                            await this.DisplayAlert(
                                this.ActDetail.GetControlDescription(),
                                "Print product labels?",
                                "Yes",
                                "No")))
                    {
                        try
                        {
                            await this.PrintProductLabels();
                        }
                        catch (MessageOnlyException msgEx)
                        {
                            var msg = msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1
                                ? msgEx.Message
                                : msgEx.Message["MSGBOX-".Length..];

                            // Even though an error was raised, during label printing allow the control to close.
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    GetControlDescription(),
                                    msg,
                                    Resource.OK));
                        }
                        catch (Exception ex)
                        {
                            // Even though an error was raised, during label printing allow the control to close.
                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                var errRpt = new FormErrorReport(this.ActSubType, ex);
                                _ = await errRpt.ShowDialog();
                            });
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                await this.ScannerDisable();
                await this.Close();
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.ScannerDisable();
                await this.Close();
            }
        }

        private void TxtAltQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            var prodInfo = this.ActDetail.ActItem.ProdInfo;

            decimal altQtyEntered = 0;
            try
            {
                if (!string.IsNullOrEmpty(this.txtAltQty.Text))
                {
                    _ = decimal.TryParse(this.txtAltQty.Text, out var result);
                    altQtyEntered = result;
                }
            }
            catch
            {
                altQtyEntered = 0;
            }

            decimal baseQty;
            if (prodInfo.UnitCode == prodInfo.BaseUnitCode && prodInfo.Unit2BaseUnitFactor <= 1)
            {
                baseQty = altQtyEntered;
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3), CultureInfo.CurrentCulture);
            }
            else if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
            {
                baseQty = altQtyEntered * prodInfo.UnitGrossWeight;
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3), CultureInfo.CurrentCulture);
            }
            else
            {
                baseQty = altQtyEntered * prodInfo.Unit2BaseUnitFactor;
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3), CultureInfo.CurrentCulture);
            }

            decimal weightQty;
            if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = altQtyEntered;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }
            else if (prodInfo.BaseUnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = baseQty;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }
            else
            {
                weightQty = altQtyEntered * prodInfo.UnitGrossWeight;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }
        }

        private void TxtBaseQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            var prodInfo = this.ActDetail.ActItem.ProdInfo;

            decimal baseQtyEntered = 0;
            try
            {
                if (!string.IsNullOrEmpty(this.txtBaseQty.Text))
                {
                    _ = decimal.TryParse(this.txtBaseQty.Text, out var result);
                    baseQtyEntered = result;
                }
            }
            catch
            {
                baseQtyEntered = 0;
            }

            decimal weightQty;
            if (prodInfo.WeightUnitCode == prodInfo.BaseUnitCode)
            {
                weightQty = baseQtyEntered;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }
            else
            {
                weightQty = baseQtyEntered * prodInfo.UnitGrossWeight;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }

            decimal altQty;
            if (prodInfo.UnitCode == prodInfo.BaseUnitCode)
            {
                altQty = baseQtyEntered;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }
            else if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                altQty = weightQty;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }
            else if (!(prodInfo.IsCatchWeight || prodInfo.IsCatchCount))
            {
                altQty = baseQtyEntered / prodInfo.Unit2BaseUnitFactor;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }
        }
    }
}