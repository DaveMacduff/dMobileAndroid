// <copyright file="ActivityConvertUnitEditCatch.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Convert unit edit catch quantity.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertUnitEditCatch : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityConvertUnitEditCatch"/> class.
        /// </summary>
        /// <param name="actDetail"> Activity detail.</param>
        /// <param name="actSubTypes"> Activity sub types.</param>
        public ActivityConvertUnitEditCatch(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.LblBaseUnit.Text = "KG";
            this.LblBaseQty.Text = "Base Quantity";
            this.LblWtUnit.Text = "KG";
            this.LblWeight.Text = "Weight";
            this.LblEquals.Text = "Equals";
            this.LblFromUnitDesc.Text = "From Unit";
            this.CmdSave.Text = "Save";
            this.CmdCancel.Text = "Cancel";

            this.ActSubType = actSubTypes;
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
            };
        }

        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        private SysCommon.ActSubTypes ActSubType { get; set; }

        private DataView DataViewSerialNumbers { get; set; } = null;

        private DataView DataViewLots { get; set; } = null;

        private DataView DataViewVariants { get; set; } = null;

        private DataTable DataTableActDetail { get; set; }

        private ActDetailEdit ActDetailEdit { get; set; }

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private ActDetail ActDetail { get; set; } = null;

        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        private decimal AltQuantityEntered { get; set; } = 0;

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        public async Task ScannerDisable()
        {
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        public async Task ScannerEnable()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                await IsValidData();
                isReadyToSave = true;
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
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

            //////////////////////////////////////////////////////////////////////////
            async Task IsValidData()
            {
                await ValidateQuantity();

                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                this.ActDetail.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                    prodInfo.Code,
                    prodInfo.Scheme,
                    this.ActDetail.SchemeData);

                this.ActDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                    prodInfo.Code,
                    prodInfo.Scheme,
                    this.ActDetail.SchemeData);

                // IsReadyToApply will perform any validations, such as duplicate checks and
                // quantity validations.
                var xmlParam = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<itm_refno>" + this.ActDetail.ActItem.ItemFKRefno + "</itm_refno>"
                    + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                    + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                    + "<ActivityDetailUniqueSchemeID>" +
                        this.ActDetail.UniqueSchemeID +
                        "</ActivityDetailUniqueSchemeID>"
                    + "</imclient>");

                await this.ActDetail.ActItem.IsReadyToApply(xmlParam);

                //////////////////////////////////////////////////////////////////////////
                async Task ValidateQuantity()
                {
                    // Round captured screen quantity to 3 decimals.
                    this.ActDetailEdit.AltQuantity = this.AltQuantityEntered;
                    this.ActDetailEdit.BaseQuantity = Math.Round(decimal.Parse(this.TxtBaseQty.Text), 3);
                    this.ActDetailEdit.WeightQuantity = Math.Round(decimal.Parse(this.TxtWtQty.Text), 3);

                    // Check if Units of measure's are the same quantity should be the same.
                    var productInfo = this.ActDetail.ActItem.ProdInfo;
                    CheckEqualQuantity(productInfo);

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
                    void CheckEqualQuantity(ProductInfo prodQtyCheck)
                    {
                        if (prodQtyCheck.UnitCode == prodQtyCheck.BaseUnitCode &&
                            this.ActDetailEdit.AltQuantity != this.ActDetailEdit.BaseQuantity)
                        {
                            throw new MessageOnlyException("Quantity and base quantity are expected" +
                                " to be the same, because the units are the same.");
                        }

                        if (prodQtyCheck.UnitCode == prodQtyCheck.WeightUnitCode &&
                            this.ActDetailEdit.AltQuantity != this.ActDetailEdit.WeightQuantity)
                        {
                            throw new MessageOnlyException("Quantity and weight quantity are expected" +
                                " to be the same, because the units are the same.");
                        }

                        if (prodQtyCheck.BaseUnitCode == prodQtyCheck.WeightUnitCode &&
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
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = false;

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
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                // Non message only exceptions will allow the close to continue.
                // We do not want to get caught not being able to 'escape' the control.
                isReadyToClose = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Convert Units Edit";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            var unqScheme = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData);

            var scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActDetail.ActItem.ProdInfo.Code,
                this.ActDetail.ActItem.ProdInfo.Scheme,
                this.ActDetail.SchemeData); var schemeData = this.ActDetailEdit.SchemeData;

            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                          + "<params>"
                                          + "<serial>" + schemeData.SingleTradeItemSerialNumber + "</serial>"
                                          + "<pdate>" + schemeData.LogisticUnitProductionDate + "</pdate>"
                                          + "<lot_no>" + schemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                                          + "<bdate>" + schemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                                          + "<variant_code>" + schemeData.VariantCode + "</variant_code>"
                                          + "<variant_no>" + schemeData.VariantNumber + "</variant_no>"
                                          + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                                          + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                                          + "<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>"
                                          + "<scheme_id>" + scheme + "</scheme_id>"
                                          + "<unq_scheme_id>" + unqScheme + "</unq_scheme_id>"
                                          + "<split>" + false + "</split>"
                                          + "</params>");

            return request;
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
            });

            try
            {
                this.ucActivityDetailEdit_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.ucActivityDetailEdit_GotFocus threw an error " + ex,
                    "OK"));
            }
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    this.DialogResult = DialogResult.OK;
                    await this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    _ = await this.CancelChanges();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ShowProductInfo()
        {
            this.LblFromUnitDesc.Text = string.Format(
                "{0} {1} @ {2} {3}",
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).UnitCode,
                (this.ActDetail.ActItem.ListItems[0] as dynamic).ActivityDetailWeightQuantity,
                this.ActDetail.ActItem.ProdInfo.WeightUnitCode);

            this.ExtraInfoItems = new ObservableCollection<string>
            {
                "PROD: " + this.ActDetail.ActItem.ProdInfo.Code + this.ActDetail.ActItem.ProdInfo.UnitBC + "  " +
                    this.ActDetail.ActItem.ProdInfo.Desc,
                "SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc,
            };

            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
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
            // Default quantities if zero
            if (this.ActDetail.Quantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.WeightQuantity != 0)
                    {
                        this.ActDetailEdit.AltQuantity = this.ActDetail.WeightQuantity;
                    }
                    else
                    {
                        this.ActDetailEdit.AltQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
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

            if (this.ActDetail.WeightQuantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    if (this.ActDetail.Quantity != 0)
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.Quantity;
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                }
                else
                {
                    if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB")
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                    else
                    {
                        this.ActDetailEdit.WeightQuantity = this.ActDetail.ActItem.ProdInfo.UnitGrossWeight;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }

            if (this.ActDetail.BaseQuantity == 0)
            {
                if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
                {
                    this.ActDetailEdit.BaseQuantity = this.ActDetail.WeightQuantity;
                }
                else
                {
                    if (this.ActDetail.Quantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                    }
                    else
                    {
                        this.ActDetailEdit.BaseQuantity = this.ActDetail.Quantity;
                    }
                }
            }
            else
            {
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
            }

            this.ActDetailEdit.AltQuantity = decimal.Round(this.ActDetailEdit.AltQuantity, 3);
            this.ActDetailEdit.BaseQuantity = decimal.Round(this.ActDetailEdit.BaseQuantity, 3);
            this.ActDetailEdit.WeightQuantity = decimal.Round(this.ActDetailEdit.WeightQuantity, 3);

            this.TxtBaseQty.Text = this.ActDetailEdit.BaseQuantity.ToString(CultureInfo.InvariantCulture);
            this.TxtWtQty.Text = this.ActDetailEdit.WeightQuantity.ToString(CultureInfo.InvariantCulture);
        }

        private void ucActivityDetailEdit_GotFocus(object sender, EventArgs e)
        {
            if (!this.IsControlLoaded)
            {
                this.IsControlLoaded = true;
                this.LblBaseUnit.Text = this.ActDetail.ActItem.ProdInfo.BaseUnitCode;
                this.LblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    this.TxtWtQty.IsEnabled = false;
                }

                this.LoadData();
                this.ShowProductInfo();

                _ = this.TxtBaseQty.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void TxtBaseQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IdentifyChangesMade(sender, e);
            var prodInfo = this.ActDetail.ActItem.ProdInfo;

            decimal baseQtyEntered = 0;
            try
            {
                if (!string.IsNullOrEmpty(this.TxtBaseQty.Text))
                {
                    _ = decimal.TryParse(this.TxtBaseQty.Text, out var result);
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
                this.TxtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }
            else
            {
                weightQty = baseQtyEntered * prodInfo.UnitGrossWeight;
                this.TxtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3), CultureInfo.CurrentCulture);
            }

            // Calculate the alternate quantity, it cannot be entered by the user.
            var altQuantity = this.ActDetail.Quantity;
            if (prodInfo.UnitCode == prodInfo.BaseUnitCode)
            {
                altQuantity = baseQtyEntered;
                ////this.TxtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }
            else if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                altQuantity = weightQty;
                ////this.TxtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }
            else if (!(prodInfo.IsCatchWeight || prodInfo.IsCatchCount))
            {
                altQuantity = baseQtyEntered / prodInfo.Unit2BaseUnitFactor;
                ////this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3), CultureInfo.CurrentCulture);
            }

            this.AltQuantityEntered = altQuantity;
        }

        private async void CmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        private async void CmdCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            _ = await this.CancelChanges();
            await this.Close();
        }
    }
}