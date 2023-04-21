// <copyright file="ActivityRemoveFromSkid.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Detail;

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="ActivityRemoveFromSkid" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityRemoveFromSkid : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityRemoveFromSkid"/> class.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail controller.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        public ActivityRemoveFromSkid(ActDetail actDetail, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();
            this.lblQty.Text = "Quantity";
            this.lblUnit.Text = "BX";
            this.lblBaseQty.Text = "Base Quantity";
            this.lblBaseUnit.Text = "BX";
            this.lblWtUnit.Text = "KG";
            this.lblWeight.Text = "Weight";
            this.btnSave.Text = "Save";
            this.btnCancel.Text = "Cancel";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();

                this.ActSubType = actSubTypes;
                this.ActDetailEdit = new ActDetailEdit();
                this.ActDetail = actDetail;
                this.ActDetail.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActSubType = this.ActSubType;
                this.ActDetail.ActItem.ActHdr.ActSubType = this.ActSubType;

                this.ActDetailEdit.SchemeData = this.ActDetail.SchemeData;
                this.ActDetailEdit.AltQuantity = this.ActDetail.Quantity;
                this.ActDetailEdit.BaseQuantity = this.ActDetail.BaseQuantity;
                this.ActDetailEdit.WeightQuantity = this.ActDetail.WeightQuantity;
            }
        }

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the ActDetailEdit.
        /// </summary>
        private ActDetailEdit ActDetailEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus..
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsChangesMade.
        /// </summary>
        private bool IsChangesMade { get; set; } = false;

        /// <summary>
        /// Gets or sets the ActDetail.
        /// </summary>
        private ActDetail ActDetail { get; set; } = null;

        /// <summary>
        /// Gets or sets the bar code scanner engine..
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets the ExtraInfoItems.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler is registered..
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the bar code parse data..
        /// </summary>
        private BarCodeParse Parse { get; set; } = null;

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
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
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// The IsReadyToSave.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = false;

            try
            {
                isReadyToSave = true;
                await ValidateQuantity();
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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

            //////////////////////////////////////////////////////////////////////////
            async Task ValidateQuantity()
            {
                // Round captured screen quantity to 3 decimals.
                this.ActDetailEdit.AltQuantity = Math.Round(decimal.Parse(this.txtAltQty.Text), 3);
                this.ActDetailEdit.BaseQuantity = Math.Round(decimal.Parse(this.txtBaseQty.Text), 3);
                this.ActDetailEdit.WeightQuantity = Math.Round(decimal.Parse(this.txtWtQty.Text), 3);

                // Check if UOM's are the same quantity should be the same.
                var prodInfo = this.ActDetail.ActItem.ProdInfo;
                CheckEqualQuantity(prodInfo);

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
                    expectedWeightQty = Math.Round(this.ActDetailEdit.BaseQuantity * prodInfo.UnitGrossWeight, 3);
                }

                if (Math.Sign(this.ActDetailEdit.AltQuantity) == 0 &&
                    (this.ActDetail.ActSubType == SysCommon.ActSubTypes.Adjustment ||
                        this.ActDetail.ActSubType == SysCommon.ActSubTypes.TicketMaterial))
                {
                    // Alt quantity is not validated.
                    await ValidateBaseAndWeightAsync(prodInfo, expectedWeightQty);
                }
                else
                {
                    await ValidateAltBaseAndWeightAsync(prodInfo, expectedWeightQty);
                }

                //////////////////////////////////////////////////////////////////////////
                // Check the quantity if the UOM match.
                void CheckEqualQuantity(ProductInfo prodInfo)
                {
                    if (prodInfo.UnitCode == prodInfo.BaseUnitCode &&
                        this.ActDetailEdit.AltQuantity != this.ActDetailEdit.BaseQuantity)
                    {
                        throw new MessageOnlyException("Quantity and base quantity are expected" +
                            " to be the same, because the units are the same.");
                    }

                    if (prodInfo.UnitCode == prodInfo.WeightUnitCode &&
                        this.ActDetailEdit.AltQuantity != this.ActDetailEdit.WeightQuantity)
                    {
                        throw new MessageOnlyException("Quantity and weight quantity are expected" +
                            " to be the same, because the units are the same.");
                    }

                    if (prodInfo.BaseUnitCode == prodInfo.WeightUnitCode &&
                        this.ActDetailEdit.BaseQuantity != this.ActDetailEdit.WeightQuantity)
                    {
                        throw new MessageOnlyException("Base quantity and weight quantity are expected" +
                            " to be the same, because the units are the same.");
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task ValidateBaseAndWeightAsync(ProductInfo prodInfo, decimal expectedWeightQty)
                {
                    // Base and weight quantity sign check.
                    const int TotalSignQtyBaseWeight = 2;
                    var totalSign = Math.Sign(this.ActDetailEdit.BaseQuantity) +
                                    Math.Sign(this.ActDetailEdit.WeightQuantity);
                    if (totalSign != TotalSignQtyBaseWeight && totalSign != -TotalSignQtyBaseWeight)
                    {
                        throw new MessageOnlyException(
                            "The sign of the base and weight quantity must be the same.");
                    }

                    if (this.SysEnv.SysSettings.IsIntegerCheckOverride)
                    {
                        // If the alt unit and base unit are the same. Quantity check was already done.
                        // If the base and weight unit are the same. Quantity check was already done.
                        if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                            else if (prodInfo.IsCatchWeight)
                            {
                                // TODO: Add weight tolerance check warning.
                            }
                            else if (prodInfo.IsCatchCount)
                            {
                                throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                    $" catch count and the base unit code be a weighted unit.");
                            }
                        }
                        else if (prodInfo.IsCatchWeight)
                        {
                            throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
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
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                        }
                    }
                    else
                    {
                        if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                            else if (prodInfo.IsCatchWeight)
                            {
                                // TODO: Add weight tolerance check warning.
                            }
                            else if (prodInfo.IsCatchCount)
                            {
                                throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                    $" catch count and the base unit code be a weighted unit.");
                            }
                        }
                        else if (prodInfo.IsCatchWeight)
                        {
                            throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
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
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task ValidateAltBaseAndWeightAsync(ProductInfo prodInfo, decimal expectedWeightQty)
                {
                    // Alt, base and weight quantity sign check.
                    const int TotalSignQtyBaseWeight = 3;
                    var totalSign = Math.Sign(this.ActDetailEdit.AltQuantity) +
                                    Math.Sign(this.ActDetailEdit.BaseQuantity) +
                                    Math.Sign(this.ActDetailEdit.WeightQuantity);
                    if (totalSign != TotalSignQtyBaseWeight && totalSign != -TotalSignQtyBaseWeight)
                    {
                        throw new MessageOnlyException(
                            "The sign of the quantity, base, and weight quantity must be the same.");
                    }

                    if (this.SysEnv.SysSettings.IsIntegerCheckOverride)
                    {
                        // If the alt unit and base unit are the same. Quantity check was already done.
                        // If the base and weight unit are the same. Quantity check was already done.
                        if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                        }

                        if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                            else if (prodInfo.IsCatchCount)
                            {
                                throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                    $" catch count and the base unit code be a weighted unit.");
                            }
                        }
                        else if (prodInfo.IsCatchWeight)
                        {
                            throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                $" catch weight and the base unit code not be a weighted unit.");
                        }
                        else if (prodInfo.IsCatchCount)
                        {
                            // Any alt quantity is valid.
                            // Any base quantity is valid.
                            // Weight must be match calculated.
                            if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
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
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                        }
                    }
                    else
                    {
                        // If the alt unit and base unit are the same. Quantity check was already done.
                        // If the base and weight unit are the same. Quantity check was already done.
                        if (prodInfo.UnitCode == "LB" || prodInfo.UnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                            else if (prodInfo.IsCatchWeight)
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

                        if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
                        {
                            // Any base quantity value is acceptable.
                            // Integer check override does not apply to weighted units.
                            if (!prodInfo.IsCatchWeight && expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                            else if (prodInfo.IsCatchWeight)
                            {
                                // TODO: Add weight tolerance check warning.
                            }
                            else if (prodInfo.IsCatchCount)
                            {
                                throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                    $" catch count and the base unit code be a weighted unit.");
                            }
                        }
                        else if (prodInfo.IsCatchWeight)
                        {
                            throw new NotSupportedException($"Product '{prodInfo.Code}' cannot be flagged as" +
                                $" catch weight and the base unit code not be a weighted unit.");
                        }
                        else if (prodInfo.IsCatchCount)
                        {
                            // Any alt quantity is valid.
                            // Any base quantity is valid.
                            // Weight must be match calculated.
                            if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
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
                                this.ActDetailEdit.AltQuantity / prodInfo.Unit2BaseUnitFactor, 3);
                            if (this.ActDetailEdit.BaseQuantity != expectedBaseQty)
                            {
                                throw new MessageOnlyException($"Base quantity {this.ActDetailEdit.BaseQuantity}" +
                                    $" {prodInfo.BaseUnitCode} does not match the expected base quantity" +
                                    $"{expectedBaseQty} {prodInfo.BaseUnitCode}.");
                            }

                            // Base unit is not a weighted unit.
                            // Any quantity value is acceptable.
                            // Weight quantity must match the expected.
                            if (expectedWeightQty != this.ActDetailEdit.WeightQuantity)
                            {
                                throw new MessageOnlyException($"Weight quantity {this.ActDetailEdit.WeightQuantity}" +
                                    $" {prodInfo.WeightUnitCode} does not match the expected weight quantity" +
                                    $"{expectedWeightQty} {prodInfo.WeightUnitCode}.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The IsReadyToClose.
        /// </summary>
        /// <returns>.</returns>
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
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public string GetControlDescription()
        {
            return "Remove From Skid";
        }

        /// <summary>
        /// The SaveChanges.
        /// </summary>
        /// <returns>.</returns>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    saveComplete = await this.IsValidData();
                    this.IsChangesMade = false;
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
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
        /// The CancelChanges.
        /// </summary>
        /// <returns>.</returns>
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
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// The GetXML.
        /// </summary>
        /// <returns>.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                          + "<params>"
                                          + "<alt_qty>" + this.ActDetailEdit.AltQuantity + "</alt_qty>"
                                          + "<base_qty>" + this.ActDetailEdit.BaseQuantity + "</base_qty>"
                                          + "<wt_qty>" + this.ActDetailEdit.WeightQuantity + "</wt_qty>"
                                          + "</params>");

            return request;
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
                SubscribeKeyUpHandlers();
                this.IsFirstAppearance = true;
                this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.BaseUnitCode;
                this.lblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    // disable when not catch weight, or when qty is LB/KG
                    this.txtWtQty.IsEnabled = false;
                }

                this.LoadData();
                this.DisplaySummary();
                _ = this.txtAltQty.Focus();
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
                    var msg = $"Unable to load skid info.{Environment.NewLine}" +
                        $"General exception.{Environment.NewLine}" +
                        $"Detail:{ex.Message}{Environment.NewLine}" +
                        "Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (!this.Loading)
                    {
                        if (keycode == Keycode.F1 || keycode == Keycode.Enter)
                        {
                            if (await this.SaveChanges())
                            {
                                this.DialogResult = DialogResult.OK;
                                await this.Close();
                            }
                        }
                    }
                });
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
        }

        /// <summary>
        /// The OnBackButtonPressed.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
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
                else if (await this.CancelChanges())
                {
                    this.DialogResult = DialogResult.None;
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Sender bar code scanner event arguments.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            try
            {
                this.Loading = true;
                this.Parse = scanArgs.ParseInfo;
                await this.ScannerDisable();
            }
            catch (TaskCanceledException)
            {
            }
            catch (IsReadyToApplyException)
            {
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (MessageOnlyException msgEx)
            {
                var msg = (msgEx.Message.IndexOf("MSGBOX-", StringComparison.Ordinal) == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
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
                this.Loading = false;
            }
        }

        /// <summary>
        /// The IsValidData.
        /// </summary>
        /// <returns>.</returns>
        private async Task<bool> IsValidData()
        {
            var isRetVal = false;

            var qty = decimal.Parse(this.txtAltQty.Text);
            var lineQty = this.ActDetail.Quantity;

            // We assign how much we want to remove
            var newQty = qty;
            var baseUnit = this.ActDetail.ActItem.ProdInfo.BaseUnitCode;
            var newWtQty = decimal.Parse(this.txtWtQty.Text);
            var newBaseQty = this.ActDetail.BaseQuantity;

            if (baseUnit == "LB" || baseUnit == "KG")
            {
                newBaseQty = newWtQty;
            }
            else
            {
                if (this.ActDetailEdit.AltQuantity != 0 && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                {
                    newBaseQty = newQty * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                }
                else
                {
                    newBaseQty = newQty;
                }
            }

            if (qty == lineQty)
            {
                this.ActDetailEdit.AltQuantity = newQty;
                this.ActDetailEdit.WeightQuantity = newWtQty;
                this.ActDetailEdit.BaseQuantity = newBaseQty;

                isRetVal = true;
            }
            else if (qty == 0)
            {
                // Exit without doing anything
                isRetVal = true;
            }
            else if (qty > lineQty)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    this.GetControlDescription(),
                    "Cannot remove more than exists on skid!",
                    "OK"));
            }
            else if (qty < 0)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    this.GetControlDescription(),
                    "Cannot remove negative quantity!",
                    "OK"));
            }
            else
            {
                if (Convert.ToInt32(qty) != qty)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "Cannot remove partial boxes!",
                        "OK"));
                }
                else
                {
                    // Good
                    this.ActDetailEdit.AltQuantity = newQty;
                    this.ActDetailEdit.WeightQuantity = newBaseQty;
                    this.ActDetailEdit.BaseQuantity = newBaseQty;
                    isRetVal = true;
                }
            }

            return isRetVal;
        }

        /// <summary>
        /// The PromptConsumeSkid.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task PromptConsumeSkid()
        {
            if (await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                this.GetControlDescription(),
                "Do you wish to consume this entire skid?",
                "Yes",
                "No")))
            {
                await this.ActDetail.Delete();

                if (this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid.IsConsumed)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                        this.GetControlDescription(),
                        "This skid is now consumed.",
                        "OK"));

                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;
                        await this.Close();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.None;
                        _ = await this.CancelChanges();
                    }
                }
            }
        }

        /// <summary>
        /// The DisplaySummary.
        /// </summary>
        private void DisplaySummary()
        {
            var skid = this.ActDetail.ActItem.ActHdr.SkidInfo.CurrSkid;

            this.ExtraInfoItems.Clear();
            this.ExtraInfoItems.Add("Product: " + this.ActDetail.ActItem.ProdInfo.Code +
                this.ActDetail.ActItem.ProdInfo.UnitBC + "  " +
                this.ActDetail.ActItem.ProdInfo.Desc);
            this.ExtraInfoItems.Add($"Production Date: {this.ActDetail.SchemeData.LogisticUnitProductionDate}");
            this.ExtraInfoItems.Add(
                $"Quantity {this.ActDetail.Quantity} {skid.ProdInfo.UnitCode} @ {this.ActDetail.WeightQuantity} {skid.ProdInfo.WeightUnitCode}");
            this.ExtraInfoItems.Add("SIZE: " + this.ActDetail.ActItem.ProdInfo.UnitSizeDesc);
        }

        /// <summary>
        /// The IdentifyChangesMade.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsFirstAppearance)
            {
                this.IsChangesMade = true;
            }
        }

        /// <summary>
        /// The LoadData.
        /// </summary>
        private void LoadData()
        {
            this.ActDetailEdit.AltQuantity = 0;
            this.ActDetailEdit.BaseQuantity = 0;
            this.ActDetailEdit.WeightQuantity = 0;

            this.txtAltQty.Text = "0";
            this.txtBaseQty.Text = "0";
            this.txtWtQty.Text = "0";
        }

        /// <summary>
        /// The ucActivityRemoveFromSkid_GotFocus.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void ActivityRemoveFromSkidGotFocus(object sender, EventArgs e)
        {
            if (!this.IsFirstAppearance)
            {
                this.IsFirstAppearance = true;
                this.lblUnit.Text = this.ActDetail.ActItem.ProdInfo.UnitCode;
                this.lblWtUnit.Text = this.ActDetail.ActItem.ProdInfo.WeightUnitCode;

                if (!this.ActDetail.ActItem.ProdInfo.IsCatchWeight ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "LB" ||
                    this.ActDetail.ActItem.ProdInfo.UnitCode == "KG")
                {
                    // disable when not catch weight, or when qty is LB/KG
                    this.txtWtQty.IsEnabled = false;
                }

                this.LoadData();
                this.DisplaySummary();
                _ = this.txtAltQty.Focus();
            }
        }

        /// <summary>
        /// The txtAltQty_TextChanged.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void TxtAltQtyTextChanged(object sender, TextChangedEventArgs e)
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
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3));
            }
            else if (prodInfo.BaseUnitCode == "LB" || prodInfo.BaseUnitCode == "KG")
            {
                baseQty = altQtyEntered * prodInfo.UnitGrossWeight;
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3));
            }
            else
            {
                baseQty = altQtyEntered * prodInfo.Unit2BaseUnitFactor;
                this.txtBaseQty.Text = Convert.ToString(Math.Round(baseQty, 3));
            }

            decimal weightQty;
            if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = altQtyEntered;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3));
            }
            else if (prodInfo.BaseUnitCode == prodInfo.WeightUnitCode)
            {
                weightQty = baseQty;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3));
            }
            else
            {
                weightQty = altQtyEntered * prodInfo.UnitGrossWeight;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3));
            }
        }

        /// <summary>
        /// The txtBaseQty_TextChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/>.</param>
        private void TxtBaseQtyTextChanged(object sender, TextChangedEventArgs e)
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
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3));
            }
            else
            {
                weightQty = baseQtyEntered * prodInfo.UnitGrossWeight;
                this.txtWtQty.Text = Convert.ToString(Math.Round(weightQty, 3));
            }

            decimal altQty;
            if (prodInfo.UnitCode == prodInfo.BaseUnitCode)
            {
                altQty = baseQtyEntered;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3));
            }
            else if (prodInfo.UnitCode == prodInfo.WeightUnitCode)
            {
                altQty = weightQty;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3));
            }
            else if (!(prodInfo.IsCatchWeight || prodInfo.IsCatchCount))
            {
                altQty = baseQtyEntered / prodInfo.Unit2BaseUnitFactor;
                this.txtAltQty.Text = Convert.ToString(Math.Round(altQty, 3));
            }
        }

        /// <summary>
        /// The txtWtQty_TextChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/>.</param>
        private void TxtWtQtyTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// The txtAltQty_Focused.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="FocusEventArgs"/>.</param>
        private void TxtAltQtyFocused(object sender, FocusEventArgs e)
        {
            if (this.txtAltQty.Text != null)
            {
                this.txtAltQty.CursorPosition = 0;
                this.txtAltQty.SelectionLength = this.txtAltQty.Text.Length;
            }
        }

        /// <summary>
        /// The txtAltQty_Focused.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="FocusEventArgs"/>.</param>
        private void TxtBaseQtyFocused(object sender, FocusEventArgs e)
        {
            if (this.txtBaseQty.Text != null)
            {
                this.txtBaseQty.CursorPosition = 0;
                this.txtBaseQty.SelectionLength = this.txtBaseQty.Text.Length;
            }
        }

        /// <summary>
        /// The txtWtQty_Focused.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="FocusEventArgs"/>.</param>
        private void TxtWtQtyFocused(object sender, FocusEventArgs e)
        {
            if (this.txtWtQty.Text != null)
            {
                this.txtWtQty.CursorPosition = 0;
                this.txtWtQty.SelectionLength = this.txtWtQty.Text.Length;
            }
        }

        /// <summary>
        /// The ButtonSaveClicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void CmdSaveClicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
        }

        /// <summary>
        /// The btnCancel_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void CmdCancelClicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            _ = await this.CancelChanges();
        }
    }
}
