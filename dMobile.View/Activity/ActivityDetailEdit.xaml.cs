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
    using BarCodeScanner.BarCodeScanners;
    using Bedrock;
    using Controller.Activity_Detail;
    using CustomPages.Modal;
    using Forms;
    using Resources.Enums;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Define the <see cref="ActivityDetailEdit"/> activity detail edit.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetailEdit : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetailEdit"/> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail object.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub types.</param>
        public ActivityDetailEdit(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.ExtraInfoItems = new ObservableCollection<string>();
            this.SysEnv = SysEnvironment.GetInstance();
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
            this.dtBestBeforeDate.Date = DateTime.Now;
            this.dtProductionDate.Date = DateTime.Now;
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

            this.ActDetail = actDetail;
            this.ActDetail.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActSubType = this.ActSubType;
            this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

            this.ActDetailEdit = new ActDetailEdit
            {
                SchemeData = this.ActDetail.SchemeData,
                AltQuantity = this.ActDetail.Quantity,
                BaseQuantity = this.ActDetail.BaseQuantity,
                WeightQuantity = this.ActDetail.WeightQuantity,
                ActExtra =
                {
                    Text1 = this.ActDetail.ActExtra.Text1,
                    Text2 = this.ActDetail.ActExtra.Text2,
                    Text3 = this.ActDetail.ActExtra.Text3,
                    Text4 = this.ActDetail.ActExtra.Text4,
                    DateTime1 = this.ActDetail.ActExtra.DateTime1,
                },
            };

            ////if (!this.ActDetail.ActItem.ProdInfo.Scheme.IsBestBeforeDateTrackedDefault)
            ////{
            ////    this.dtBestBeforeDate.Date = DateTime.Now;
            ////}

            ////if (!this.ActDetail.ActItem.ProdInfo.Scheme.IsProductionDateTrackedDefault)
            ////{
            ////    this.dtProductionDate.Date = DateTime.Now;
            ////}

            this.Title = this.ActDetail.GetControlDescription();
            this.lbl1.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra1Caption;
            this.lbl2.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra2Caption;
            this.lbl3.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra3Caption;
            this.lbl4.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra4Caption;
            this.lbl5.Text = this.ActDetail.ActItem.ProdInfo.Scheme.Extra5Caption;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to print product labels.
        /// </summary>
        private bool AskToPrintProductLabels { get; set; } = false;

        /// <summary>
        /// Gets or sets the system environment object.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the activity detail edit.
        /// </summary>
        private ActDetailEdit ActDetailEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the activity detail.
        /// </summary>
        private ActDetail ActDetail { get; set; }

        /// <summary>
        /// Gets or sets the activity detail edit items list.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler has been registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

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
                await IsValidData();
            }
            catch (IsReadyToApplyException ex)
            {
                if (ex.SuppressError && ex.IsOverContinue)
                {
                    isReadyToSave = true;
                }
                else if (ex.SuppressError && !ex.IsOverContinue)
                {
                    isReadyToSave = false;
                }
                else
                {
                    isReadyToSave = false;
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                isReadyToSave = false;
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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

            //////////////////////////////////////////////////////////////////////////
            async Task IsValidData()
            {
                await ValidateQuantity();
                await ValidateScheme();
                ValidateSchemeExtra();

                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var uniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                    prodInfo.Code,
                    prodInfo.Scheme,
                    this.ActDetailEdit.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and
                // quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + this.ActDetail.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                    + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" + uniqueSchemeId + "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                //////////////////////////////////////////////////////////////////////////
                async Task ValidateQuantity()
                {
                    // Round captured screen quantity to 3 decimals.
                    this.ActDetailEdit.AltQuantity = Math.Round(decimal.Parse(this.txtAltQty.Text), 3);
                    this.ActDetailEdit.BaseQuantity = Math.Round(decimal.Parse(this.txtBaseQty.Text), 3);
                    this.ActDetailEdit.WeightQuantity = Math.Round(decimal.Parse(this.txtWtQty.Text), 3);

                    // Check if Units of measure's are the same quantity should be the same.
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

                    // Alt quantity equal to zero will be considered valid for these activity sub types.
                    decimal expectedWeightQty = 0;
                    if (this.ActDetail.ActItem.ProdInfo.IsBaseWeightUnit)
                    {
                        expectedWeightQty = this.ActDetailEdit.BaseQuantity;
                    }
                    else
                    {
                        expectedWeightQty = Math.Round(this.ActDetailEdit.BaseQuantity * productInfo.UnitGrossWeight, 3);
                    }

                    if (Math.Sign(this.ActDetailEdit.AltQuantity) == 0 &&
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

                    //////////////////////////////////////////////////////////////////////////
                    // Check the quantity if the UOM match.
                    void CheckEqualQuantity()
                    {
                        if (productInfo.UnitCode == productInfo.BaseUnitCode &&
                            this.ActDetailEdit.AltQuantity != this.ActDetailEdit.BaseQuantity)
                        {
                            throw new MessageOnlyException("Quantity and base quantity are expected" +
                                " to be the same, because the units are the same.");
                        }

                        if (productInfo.UnitCode == productInfo.WeightUnitCode &&
                            this.ActDetailEdit.AltQuantity != this.ActDetailEdit.WeightQuantity)
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

                    //////////////////////////////////////////////////////////////////////////
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
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                                else if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                        $" catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                    $" catch weight and the base unit code not be a weighted unit.");
                            }
                            else
                            {
                                // Catch count is validate the same as here.
                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                        else
                        {
                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                                else if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                        $" catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                    $" catch weight and the base unit code not be a weighted unit.");
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
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task ValidateAltBaseAndWeightAsync()
                    {
                        // Alt, base and weight quantity sign check.
                        const int totalSignQtyBaseWeight = 3;
                        var totalSign = Math.Sign(this.ActDetailEdit.AltQuantity) +
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
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }

                            if (productInfo.BaseUnitCode == "LB" || productInfo.BaseUnitCode == "KG")
                            {
                                // Any base quantity value is acceptable.
                                // Integer check override does not apply to weighted units.
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                        $" catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                    $" catch weight and the base unit code not be a weighted unit.");
                            }
                            else if (productInfo.IsCatchCount)
                            {
                                // Any alt quantity is valid.
                                // Any base quantity is valid.
                                // Weight must be match calculated.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                            else
                            {
                                // Base unit is not a weighted unit.
                                // Any quantity value is acceptable.
                                // Weight quantity must match the expected.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
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
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                                else if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                            }
                            else if (Math.Abs(this.ActDetailEdit.AltQuantity) != this.ActDetailEdit.AltQuantity)
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
                                if (!productInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                                else if (productInfo.IsCatchWeight)
                                {
                                    // TODO: Add weight tolerance check warning.
                                }
                                else if (productInfo.IsCatchCount)
                                {
                                    throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                        $" catch count and the base unit code be a weighted unit.");
                                }
                            }
                            else if (productInfo.IsCatchWeight)
                            {
                                throw new NotSupportedException($"Product '{productInfo.Code}' cannot be flagged as" +
                                    $" catch weight and the base unit code not be a weighted unit.");
                            }
                            else if (productInfo.IsCatchCount)
                            {
                                // Any alt quantity is valid.
                                // Any base quantity is valid.
                                // Weight must be match calculated.
                                if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                                {
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
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
                                    this.ActDetailEdit.AltQuantity / productInfo.Unit2BaseUnitFactor, 3);
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
                                    throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                        $" {productInfo.WeightUnitCode} does not match the expected weight quantity" +
                                        $"{expectedWeightQty} {productInfo.WeightUnitCode}.");
                                }
                            }
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task ValidateScheme()
                {
                    await AssignScheme();
                    ValidateSchemeSerialTracked();
                    ValidateSchemeLotTracked();
                    ValidateSchemeBestBeforeDateTracked();
                    ValidateSchemeProductionDateTracked();
                    ValidateSchemeIsVariantTracked();

                    //////////////////////////////////////////////////////////////////////////
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

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeSerialTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsSerialTracked &&
                            this.ActDetailEdit.SchemeData.SingleTradeItemSerialNumber == string.Empty)
                        {
                            throw new MessageOnlyException("Serial number required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeLotTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsLotTracked &&
                            this.ActDetailEdit.SchemeData.LogisticUnitBatchLotNumber == string.Empty)
                        {
                            throw new MessageOnlyException("Lot number required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
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
                    }

                    //////////////////////////////////////////////////////////////////////////
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
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeIsVariantTracked()
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked &&
                            this.ActDetailEdit.SchemeData.VariantCode == string.Empty)
                        {
                            throw new MessageOnlyException("Variant code required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    async Task<ProductSchemeData> AssignSchemeVariant(ProductSchemeData scheme)
                    {
                        if (this.ActDetail.ActItem.ProdInfo.Scheme.IsVariantTracked)
                        {
                            scheme.VariantNumber = Convert.ToInt32(this.txtVariantCode.Text);
                            scheme = await this.ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(scheme);
                        }

                        return scheme;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void ValidateSchemeExtra()
                {
                    var scheme = this.ActDetail.ActItem.ProdInfo.Scheme;

                    AssignSchemeExtraDefault();

                    ValidateSchemeExtra1Required();
                    ValidateSchemeExtra2Required();
                    ValidateSchemeExtra3Required();
                    ValidateSchemeExtra4Required();
                    ValidateSchemeExtra5Required();

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeExtra1Required()
                    {
                        if (scheme.Extra1Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra1IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text1))
                        {
                            throw new MessageOnlyException(scheme.Extra1Caption.TrimEnd() + " is required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeExtra2Required()
                    {
                        if (scheme.Extra2Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra2IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text2))
                        {
                            throw new MessageOnlyException(scheme.Extra2Caption.TrimEnd() + " is required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeExtra3Required()
                    {
                        if (scheme.Extra3Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra3IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text3))
                        {
                            throw new MessageOnlyException(scheme.Extra3Caption.TrimEnd() + " is required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeExtra4Required()
                    {
                        if (scheme.Extra4Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra4IsRequired &&
                            string.IsNullOrEmpty(this.ActDetailEdit.ActExtra.Text4))
                        {
                            throw new MessageOnlyException(scheme.Extra4Caption.TrimEnd() + " is required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
                    void ValidateSchemeExtra5Required()
                    {
                        if (scheme.Extra5Type != (int)SysCommon.SchemeExtraTypes.Disabled && scheme.Extra5IsRequired &&
                            SysCommon.IsEmptyDate(this.ActDetailEdit.ActExtra.DateTime1))
                        {
                            throw new MessageOnlyException(scheme.Extra5Caption.TrimEnd() + " is required.");
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////
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
        /// Check if the activity detail edit is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity detail edit is ready to close,
        /// else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            try
            {
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Save any activity detail edit data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = true;

            try
            {
                if (await this.IsReadyToSave())
                {
                    var uniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                        this.ActDetail.ActItem.ProdInfo.Code,
                        this.ActDetail.ActItem.ProdInfo.Scheme,
                        this.ActDetailEdit.SchemeData);

                    var schemeId = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                        this.ActDetail.ActItem.ProdInfo.Code,
                        this.ActDetail.ActItem.ProdInfo.Scheme,
                        this.ActDetailEdit.SchemeData);

                    this.ActDetail.UniqueSchemeId = uniqueSchemeId;
                    this.ActDetail.SchemeId = schemeId;
                    this.ActDetail.SchemeData = this.ActDetailEdit.SchemeData;
                    this.ActDetail.Quantity = this.ActDetailEdit.AltQuantity;
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Cancel any pending activity detail edit changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (await this.IsReadyToClose())
                {
                    this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                    this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// Get the activity detail edit return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
        public async Task<XDocument> GetXML()
        {
            return await this.ActDetailEdit.GetXmlFromActivityDetailEdit(this.ActDetailEdit);
        }

        /// <summary>
        /// On appearing event handler.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Note: Do we want to disable the scanner here. It may not even be enabled before at all.
            // It is my understanding that the OnAppearing event only occurs once when the control
            // is loading.
            await this.ScannerDisable();
            SubscribeKeyUpHandlers();
            this.txtAltQty.TextChanged += this.TxtAltQty_TextChanged;
            this.txtBaseQty.TextChanged += this.TxtBaseQty_TextChanged;

            try
            {
                this.Loading = true;
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    ////this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                    await InitControlState();
                    await ScannerStart();
                    await LoadData();
                    await ShowProductInfo();

                    ////await SetStartingFocus();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(
                        "Error",
                        "this.ucActivityDetailEdit_GotFocus threw an error " + ex,
                        "OK"));
            }
            finally
            {
                this.Loading = false;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task ScannerStart()
            {
                if (this.Scanner != null)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += this.BarCodeHandler;
                    this.IsBarCodeHandlerRegistered = true;
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task ShowProductInfo()
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.ExtraInfoItems.Clear();

                    this.ExtraInfoItems.Add("PROD: " + this.ActDetail.ActItem.ProdInfo.Code +
                        this.ActDetail.ActItem.ProdInfo.UnitBC + "  " +
                        this.ActDetail.ActItem.ProdInfo.Desc);
                    this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
                });
            }

            //////////////////////////////////////////////////////////////////////////
            async Task InitControlState()
            {
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                var scheme = prodInfo.Scheme;

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblUnit.Text = prodInfo.UnitCode;
                    this.lblBaseUnit.Text = prodInfo.BaseUnitCode;
                    this.lblWtUnit.Text = prodInfo.WeightUnitCode;
                    if (!prodInfo.IsCatchWeight || prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                    {
                        this.txtWtQty.IsEnabled = false;
                    }

                    InitControlStateScheme();
                    InitControlStateExtra();
                });

                //////////////////////////////////////////////////////////////////////////
                // Enable scheme controls state.
                void InitControlStateScheme()
                {
                    if (!scheme.IsBestBeforeDateTracked)
                    {
                        this.dtBestBeforeDate.IsEnabled = false;
                    }

                    if (!scheme.IsProductionDateTracked)
                    {
                        this.dtProductionDate.IsEnabled = false;
                    }

                    if (!scheme.IsLotTracked)
                    {
                        this.txtLotNo.IsEnabled = false;
                    }

                    if (!scheme.IsSerialTracked)
                    {
                        this.txtSerialNo.IsEnabled = false;
                    }

                    if (!scheme.IsVariantTracked)
                    {
                        this.txtVariantCode.IsEnabled = false;
                    }
                }

                /////////////////////////////////////////////////////////////////////////
                // Enable extra data controls state.
                void InitControlStateExtra()
                {
                    if (scheme.Extra1Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.txtExtraDetail1.IsEnabled = false;
                    }

                    if (scheme.Extra2Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.txtExtraDetail2.IsEnabled = false;
                    }

                    if (scheme.Extra3Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.txtExtraDetail3.IsEnabled = false;
                    }

                    if (scheme.Extra4Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.txtExtraDetail4.IsEnabled = false;
                    }

                    if (scheme.Extra5Type == (int)SysCommon.SchemeExtraTypes.Disabled)
                    {
                        this.dtExtraDetail5.IsEnabled = false;
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task LoadData()
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    LoadQuantityDetailAltQuantity();
                    LoadQuantityDetailBaseQuantity();
                    LoadQuantityDetailWeightQuantity();
                    LoadSchemeDetail();
                    LoadExtraDetail();
                });

                //////////////////////////////////////////////////////////////////////////
                // Load the extra detail controls with the activity detail extra values.
                void LoadExtraDetail()
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

                //////////////////////////////////////////////////////////////////////////
                // Load the scheme controls with the activity detail scheme values.
                void LoadSchemeDetail()
                {
                    var schemeData = this.ActDetailEdit.SchemeData;
                    var prodInfo = this.ActDetail.ActItem.ProdInfo;

                    if (prodInfo.Scheme.IsBestBeforeDateTracked)
                    {
                        if (!SysCommon.IsEmptyDateTime(schemeData.LogisticUnitBestBeforeSellByDate))
                        {
                            this.dtBestBeforeDate.Date = schemeData.LogisticUnitBestBeforeSellByDate;
                        }
                    }

                    if (prodInfo.Scheme.IsProductionDateTracked)
                    {
                        if (!SysCommon.IsEmptyDate(this.ActDetailEdit.SchemeData.LogisticUnitProductionDate))
                        {
                            this.dtProductionDate.Date = schemeData.LogisticUnitProductionDate;
                        }
                    }

                    if (prodInfo.Scheme.IsSerialTracked)
                    {
                        GetNextDefaultSerialNumber(schemeData);
                        this.txtSerialNo.Text = schemeData.SingleTradeItemSerialNumber;
                    }

                    if (prodInfo.Scheme.IsLotTracked)
                    {
                        this.txtLotNo.Text = schemeData.LogisticUnitBatchLotNumber;
                    }

                    if (prodInfo.Scheme.IsVariantTracked)
                    {
                        this.txtVariantCode.Text = schemeData.VariantNumber.ToString();
                    }

                    //////////////////////////////////////////////////////////////////////////
                    // Get the default serial number and assign it to the activity edit detail scheme data.
                    void GetNextDefaultSerialNumber(ProductSchemeData schemeData)
                    {
                        if (string.IsNullOrEmpty(schemeData.SingleTradeItemSerialNumber))
                        {
                            schemeData.SingleTradeItemSerialNumber =
                                ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                            this.ActDetailEdit.SchemeData = schemeData;
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void LoadQuantityDetailAltQuantity()
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
                        this.ActDetailEdit.AltQuantity = 0;
                    }
                    else if (isSetCatchWeightZero)
                    {
                        this.ActDetailEdit.AltQuantity = 0;
                    }
                    else if (isSetCatchCountZero)
                    {
                        this.ActDetailEdit.AltQuantity = 0;
                    }
                    else if (this.ActDetail.Quantity == 0)
                    {
                        if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                        {
                            this.ActDetailEdit.AltQuantity = this.ActDetail.WeightQuantity != 0
                                ? this.ActDetail.WeightQuantity
                                : prodInfo.UnitGrossWeight;
                        }
                        else if (prodInfo.UnitCode == prodInfo.BaseUnitCode && this.ActDetailEdit.BaseQuantity != 0)
                        {
                            this.ActDetailEdit.AltQuantity = this.ActDetailEdit.BaseQuantity;
                        }
                        else
                        {
                            this.ActDetailEdit.AltQuantity = 1;
                        }
                    }
                    else
                    {
                        this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
                    }

                    this.ActDetailEdit.AltQuantity = decimal.Round(this.ActDetailEdit.AltQuantity, 3);
                    this.txtAltQty.Text = this.ActDetailEdit.AltQuantity.ToString(CultureInfo.CurrentCulture);
                }

                //////////////////////////////////////////////////////////////////////////
                void LoadQuantityDetailBaseQuantity()
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
                        else if (prodInfo.UnitCode == prodInfo.BaseUnitCode && this.ActDetailEdit.AltQuantity != 0)
                        {
                            this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity;
                        }
                        else
                        {
                            if (this.ActDetailEdit.AltQuantity != 0 && prodInfo.Unit2BaseUnitFactor != 0)
                            {
                                this.ActDetailEdit.BaseQuantity =
                                    this.ActDetailEdit.AltQuantity * prodInfo.Unit2BaseUnitFactor;
                            }
                            else
                            {
                                this.ActDetailEdit.BaseQuantity = this.ActDetailEdit.AltQuantity;
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

                //////////////////////////////////////////////////////////////////////////
                void LoadQuantityDetailWeightQuantity()
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
                        else if (this.ActDetailEdit.AltQuantity != 0 &&
                            (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG"))
                        {
                            this.ActDetailEdit.WeightQuantity = this.ActDetailEdit.AltQuantity;
                        }
                        else if (this.ActDetailEdit.BaseQuantity != 0)
                        {
                            this.ActDetailEdit.WeightQuantity =
                                this.ActDetailEdit.BaseQuantity * prodInfo.UnitGrossWeight;
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

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async (keyPressed) =>
                    {
                        if (this.Loading)
                        {
                            return;
                        }

                        var svdIsScanReg = this.IsBarCodeHandlerRegistered;
                        await this.ScannerDisable();

                        // TODO: Verify key codes.
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (!this.Loading)
                        {
                            if (keycode == Keycode.F1 || keycode == Keycode.Enter)
                            {
                                await AppearingEnterKey();
                            }
                        }

                        if (svdIsScanReg)
                        {
                            await this.ScannerEnable();
                        }
                    });

                //////////////////////////////////////////////////////////////////////////
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
                        await this.Close();
                    }

                    //////////////////////////////////////////////////////////////////////////
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
                                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1)
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

            //////////////////////////////////////////////////////////////////////////
            async Task SetStartingFocus()
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    var prodInfo = this.ActDetail.ActItem.ProdInfo;
                    _ = this.SysEnv.SysSettings.IsDetailEditScreenSetFocus2CatchWeight
                        ? prodInfo.IsCatchWeight ? this.txtWtQty.Focus() : this.txtAltQty.Focus()
                        : prodInfo.Scheme.IsSerialTracked && prodInfo.IsCatchWeight ?
                            this.txtWtQty.Focus() : this.txtAltQty.Focus();
                });
            }
        }

        /// <summary>
        /// Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            this.txtAltQty.TextChanged -= this.TxtAltQty_TextChanged;
            this.txtBaseQty.TextChanged -= this.TxtBaseQty_TextChanged;
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.ScannerDisable();

            base.OnDisappearing();
        }

        /// <summary>
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        private static string GetControlDescription()
        {
            return "Activity Detail Editing";
        }

        /// <summary>
        /// Print product labels.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task PrintProductLabels()
        {
            if (this.ActDetail.ActSubType == SysCommon.ActSubTypes.Receiving ||
                this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                this.ActDetail.ActSubType == SysCommon.ActSubTypes.TransferTo)
            {
                var editLabelBeforePrint = !this.ActDetail.IsLabelSchemeComplete(
                    this.ActDetailEdit.SchemeData) || this.SysEnv.SysSettings.IsProductLabelCatchAsk;
                if (editLabelBeforePrint)
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

                    actDetailLabel.Quantity = this.ActDetailEdit.AltQuantity;
                    actDetailLabel.BaseQuantity = this.ActDetailEdit.BaseQuantity;
                    actDetailLabel.WeightQuantity = this.ActDetailEdit.WeightQuantity;
                    actDetailLabel.SchemeData = this.ActDetailEdit.SchemeData;
                    actDetailLabel.ActExtra = this.ActDetailEdit.ActExtra;

                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityProductLabel,
                        actDetailLabel,
                        this.ActSubType);
                    _ = await form.DisplayControl.ShowDialog();

                    // Printing of product labels will be handled in ucActivityProductLabel
                }
                else
                {
                    var sendReq = await this.ActDetail.CreateSubmitProductLabelXml(
                        await this.ActDetailEdit.GetXmlFromActivityDetailEdit(this.ActDetailEdit));
                    _ = SysCommon.SubmitComm(sendReq);
                }
            }
        }

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Sender bar code scanner
        /// event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading)
            {
                return;
            }

            _ = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            // Allowing scanning does not make sense here. Unless you are scanning a secondary
            // supporting bar code with other AI's.
            this.Loading = true;

            await this.ScannerDisable();
        }

        /// <summary>
        /// Button save click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                if (this.AskToPrintProductLabels)
                {
                    if (!this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch)
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
                                var msg = (msgEx.Message.IndexOf("MSGBOX-",
                                               StringComparison.Ordinal) ==
                                           -1)
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
                }

                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        /// Button cancel click handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            if (await this.CancelChanges())
            {
                this.DialogResult = DialogResult.None;
                await this.Close();
            }
        }

        /// <summary>
        /// Text box alt quantity changed handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Event arguments.</param>
        /// <remarks>Catch weight or catch ask is checked upon save validation.</remarks>
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

        /// <summary>
        /// Text box base quantity changed handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Event arguments.</param>
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