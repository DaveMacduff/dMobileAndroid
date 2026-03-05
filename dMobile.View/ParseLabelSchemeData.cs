// <copyright file="ParseLabelSchemeData.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System;
    using dMobile.BarCodeScanner;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.Bedrock;
    using dMobile.Bedrock.Settings;
    using dMobile.Controller.Activity_Detail;
    using dMobile.Controller.Activity_Item;
    using dMobile.Resources.Models.Products;
    using SQLite;

    /// <summary>
    /// Parse label scheme data.
    /// </summary>
    public class ParseLabelSchemeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseLabelSchemeData"/> class.
        /// </summary>
        /// <param name="prodScheme">Product scheme.</param>
        /// <param name="barParse">Parsed Bar Code data.</param>
        /// <param name="defaultLabelSchemeData">Default label scheme data.</param>
        /// <param name="currSkid">Current skid information.</param>
        public ParseLabelSchemeData(
            ProdScheme prodScheme,
            BarCodeParse barParse,
            ActItemSchemeData defaultLabelSchemeData,
            SSkidInfo currSkid)
        {
            this.ProdScheme = prodScheme ?? throw new ArgumentNullException(nameof(prodScheme));

            // Bar code data can be null if parsing was invoked from a manual entry.
            this.BarCodeData = barParse;
            this.DefaultLabelSchemeData = defaultLabelSchemeData ?? throw new ArgumentNullException(nameof(defaultLabelSchemeData));
            this.CurrentSkid = currSkid ?? throw new ArgumentNullException(nameof(currSkid));
        }

        private ProdScheme ProdScheme { get; set; } = new ProdScheme();

        private BarCodeParse BarCodeData { get; }

        private ActItemSchemeData DefaultLabelSchemeData { get; } = new ActItemSchemeData();

        private SSkidInfo CurrentSkid { get; } = new SSkidInfo();

        private ProductSchemeData ProdSchemeData { get; set; } = new ProductSchemeData();

        private bool DefaultSchemeDataAssigned { get; set; }

        private SQLiteAsyncConnection Conn { get; } = DatabaseConnection.GetInstance().SqlConn;

        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gathers label scheme data based on the provided product scheme data and bar code parsing results.
        /// </summary>
        /// <param name="prodSchemeData">Initial product scheme data.</param>
        /// <param name="defaultSchemeDataAssigned">Default scheme data assigned.</param>
        /// <returns>Label scheme data.</returns>
        public ProductSchemeData GatherLabelSchemeData(
            ProductSchemeData prodSchemeData,
            ref bool defaultSchemeDataAssigned)
        {
            this.ProdSchemeData = prodSchemeData ?? throw new ArgumentNullException(nameof(prodSchemeData));

            // Need local variable to track if default data was assigned.
            // Cannot use the defaultSchemeDataAssigned directly because it is a reference parameter.
            this.DefaultSchemeDataAssigned = false;

            this.NebraskBeefRule();

            // <issue="13609"/>
            this.AssertSuppressedErrorsAgainstLabelScheme();
            this.AssignSkidNumberFromSelectedSkid();

            if (this.AssignBestBefore())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            if (this.AssignProductionDate())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            if (this.AssignLotNumber())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            if (this.AssignSerialNumber())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            if (this.AssignVariant())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            defaultSchemeDataAssigned = this.DefaultSchemeDataAssigned;
            return this.ProdSchemeData;
        }

        private void NebraskBeefRule()
        {
            if (this.BarCodeData != null)
            {
                // Decided to implement the bar code here. Could have made it a new BarCodeParseCode_128_NebraskaBeef class.
                // The bar code was well-formed except it being a non GS1 code 128.
                if (this.SysEnv.BarCodeSettings.IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 &&
                    IsNebraskaBeefRuleMet(this.BarCodeData))
                {
                    // Lot Number is '00000021724739' serial number should be '724739'
                    this.BarCodeData.LogisticUnitBatchLotNumber = this.BarCodeData.LogisticUnitBatchLotNumber.Remove(0, 8);
                    this.BarCodeData.SingleTradeItemSerialNumber = string.Empty;
                }
            }

            return;

            static bool IsNebraskaBeefRuleMet(BarCodeParse barParse)
            {
                return (barParse.Gtin.Contains("0740780")
                        || barParse.Gtin.Contains("19336"))
                       && !string.IsNullOrEmpty(barParse.LogisticUnitBatchLotNumber);
            }
        }

        private void AssignSkidNumberFromSelectedSkid()
        {
            // Decided not to use the CurrSkid.LogisticUnitSkidCode because is is the full GS1 bar code. 18 digits.
            // Instead I am using the CurrSkid.SkidNo.
            // The SkidInfo.CurrSkid.SkidNo could have been assigned by manually selecting or creating a new skid.
            // Or it could have be scanned and the skid info looked up.
            this.ProdSchemeData.LogisticUnitSkidNumber = this.CurrentSkid.SkidCode;
        }

        private bool AssignVariant()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.VariantCode = string.Empty;
            this.ProdSchemeData.VariantDescription = string.Empty;
            this.ProdSchemeData.VariantNumber = 0;
            if (this.ProdScheme.LabelInfo.IsVariantTracked)
            {
                if (this.BarCodeData == null || this.BarCodeData.LogisticUnitProductVariant == 0)
                {
                    this.ProdSchemeData.VariantCode = this.DefaultLabelSchemeData.ProdSchemeData.VariantCode;
                    this.ProdSchemeData.VariantDescription = this.DefaultLabelSchemeData.ProdSchemeData.VariantDescription;
                    this.ProdSchemeData.VariantNumber = this.DefaultLabelSchemeData.ProdSchemeData.VariantNumber;
                    defaultDataAssigned = true;
                }
                else
                {
                    this.ProdSchemeData.VariantNumber = this.BarCodeData.LogisticUnitProductVariant;
                    ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(this.ProdSchemeData).GetAwaiter().GetResult();
                }
            }

            return defaultDataAssigned;
        }

        private bool AssignSerialNumber()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.SingleTradeItemSerialNumber = string.Empty;
            if (this.ProdScheme.LabelInfo.IsSerialTracked)
            {
                if (this.BarCodeData == null || this.BarCodeData.SingleTradeItemSerialNumber?.Length == 0)
                {
                    this.ProdSchemeData.SingleTradeItemSerialNumber =
                        this.DefaultLabelSchemeData.ProdSchemeData.SingleTradeItemSerialNumber;
                    if (!this.SysEnv.SysSettings.IsDisconnectedMode
                        && string.IsNullOrEmpty(this.ProdSchemeData.SingleTradeItemSerialNumber))
                    {
                        this.ProdSchemeData.SingleTradeItemSerialNumber =
                            ProductInfoTasks.GetProdSchemeSingleTradeItemSerialNumber();
                    }

                    if (!string.IsNullOrEmpty(this.ProdSchemeData.SingleTradeItemSerialNumber))
                    {
                        defaultDataAssigned = true;
                    }
                }
                else
                {
                    this.ProdSchemeData.SingleTradeItemSerialNumber = this.BarCodeData.SingleTradeItemSerialNumber;
                }
            }

            return defaultDataAssigned;
        }

        private bool AssignLotNumber()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.LogisticUnitBatchLotNumber = string.Empty;
            if (this.ProdScheme.LabelInfo.IsLotTracked)
            {
                if (this.BarCodeData == null || this.BarCodeData.LogisticUnitBatchLotNumber?.Length == 0)
                {
                    this.ProdSchemeData.LogisticUnitBatchLotNumber =
                        this.DefaultLabelSchemeData.ProdSchemeData.LogisticUnitBatchLotNumber;
                    defaultDataAssigned = true;
                }
                else
                {
                    this.ProdSchemeData.LogisticUnitBatchLotNumber = this.BarCodeData.LogisticUnitBatchLotNumber;
                }

                // This code is expected to have a speed issue.
                // Comparing GTIN = '90000122345000' to "122345". The small string is on the right instead of the left.
                // A SQL command using '122345' LIKE %90000122345000% will of course not work.
                // Only way to find was to iterate through the table and use a 'contains' command.
                if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues && this.ProdScheme.IsLotTrackedDefault)
                {
                    var lotCodeDefault = string.Empty;
                    if (this.BarCodeData != null)
                    {
                        lotCodeDefault = GetLotCodeDefaultFromSupplierCompanyPrefix(
                            this.ProdScheme.LabelInfo.SchemeRefno,
                            this.BarCodeData.Gtin);
                    }

                    if (!string.IsNullOrEmpty(lotCodeDefault))
                    {
                        if (string.IsNullOrEmpty(this.ProdSchemeData.LogisticUnitBatchLotNumber))
                        {
                            this.ProdSchemeData.LogisticUnitBatchLotNumber = lotCodeDefault;
                            defaultDataAssigned = true;
                        }
                        else
                        {
                            if (!this.ProdSchemeData.LogisticUnitBatchLotNumber.Contains(lotCodeDefault.Trim()))
                            {
                                this.ProdSchemeData.LogisticUnitBatchLotNumber = lotCodeDefault + "-" +
                                    this.ProdSchemeData.LogisticUnitBatchLotNumber.TrimEnd();
                                defaultDataAssigned = true;
                            }
                        }
                    }
                }
            }

            return defaultDataAssigned;

            string GetLotCodeDefaultFromSupplierCompanyPrefix(int schemeReferenceNumber, string globalTradeIdentifier)
            {
                schemeReferenceNumber = schemeReferenceNumber <= 0
                    ? throw new ArgumentOutOfRangeException(nameof(schemeReferenceNumber))
                    : schemeReferenceNumber;
                var lotCodeDefault = string.Empty;

                if (!string.IsNullOrEmpty(globalTradeIdentifier))
                {
                    // Order by descending, we want to examine the longest string to compare for a match first.
                    const string query = @"
                        select tProductSchemeLotNumber.ProductSchemeLotNumberCode,
                                tProductSchemeLotNumber.ProductSchemeLotNumberCodeDefault
                            from tProductSchemeLotNumberXRef
                            inner join tProductSchemeLotNumber ON tProductSchemeLotNumber.ProductSchemeLotNumberRefno =
                                    tProductSchemeLotNumberXRef.ProductSchemeLotNumberRefno
                                where tProductSchemeLotNumberXRef.ProductSchemeRefno = ?
                            order by ProductSchemeLotNumberCode DESC";
                    var results = this.Conn.QueryAsync<ProductSchemeLotNumberModel>(query, schemeReferenceNumber)
                        .GetAwaiter()
                        .GetResult();

                    if (results?.Count > 0)
                    {
                        foreach (var xRef in results)
                        {
                            if (globalTradeIdentifier.Contains(Convert.ToString(xRef.ProductSchemeLotNumberCode)
                                    .TrimEnd()))
                            {
                                lotCodeDefault = Convert.ToString(xRef.ProductSchemeLotNumberCodeDefault).TrimEnd();
                                break;
                            }
                        }
                    }
                }

                return lotCodeDefault;
            }
        }

        private bool AssignProductionDate()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.LogisticUnitProductionDate = SysCommon.EmptyDateTime;
            if (this.ProdScheme.LabelInfo.IsProductionDateTracked)
            {
                if (this.BarCodeData == null || SysCommon.IsEmptyDateTime(this.BarCodeData.LogisticUnitProductionDate))
                {
                    var defaultDate = this.DefaultLabelSchemeData.ProdSchemeData.LogisticUnitProductionDate;
                    if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues &&
                        this.ProdScheme.IsProductionDateTrackedDefault &&
                        !SysCommon.IsEmptyDateTime(defaultDate))
                    {
                        this.ProdSchemeData.LogisticUnitProductionDate = defaultDate;
                        defaultDataAssigned = true;
                    }
                    else
                    {
                        this.ProdSchemeData.LogisticUnitProductionDate = DateTime.Now;
                    }
                }
                else
                {
                    this.ProdSchemeData.LogisticUnitProductionDate = this.BarCodeData.LogisticUnitProductionDate;
                }
            }

            return defaultDataAssigned;
        }

        private bool AssignBestBefore()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.LogisticUnitBestBeforeSellByDate = SysCommon.EmptyDateTime;
            if (this.ProdScheme.LabelInfo.IsBestBeforeDateTracked)
            {
                if (this.BarCodeData == null || SysCommon.IsEmptyDateTime(this.BarCodeData.LogisticUnitBestBeforeSellByDate))
                {
                    var defaultDate = this.DefaultLabelSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate;
                    if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues &&
                        this.ProdScheme.IsBestBeforeDateTrackedDefault &&
                        !SysCommon.IsEmptyDateTime(defaultDate))
                    {
                        this.ProdSchemeData.LogisticUnitBestBeforeSellByDate = defaultDate;
                        defaultDataAssigned = true;
                    }
                    else
                    {
                        this.ProdSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                    }
                }
                else
                {
                    this.ProdSchemeData.LogisticUnitBestBeforeSellByDate = this.BarCodeData.LogisticUnitBestBeforeSellByDate;
                }
            }

            return defaultDataAssigned;
        }

        /// <summary>
        ///     <para>Checks to see if we have to throw any suppressed errors from Bar Code parsing.</para>
        ///     <para>
        ///         Rule Enabled
        ///         If an error is tracked in the label printing scheme, throw first error from a tracked field.
        ///     </para>
        ///     <para>
        ///         Rule Disabled
        ///         If there are errors, throw the first error.
        ///     </para>
        ///     <para>
        ///         <see cref="BarCodeSettings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents" />.
        ///     </para>
        ///     <seealso cref="BarCodeParseUcc_EAN_128_Symbol.ParseBarCodeData" />.
        /// </summary>
        private void AssertSuppressedErrorsAgainstLabelScheme()
        {
            // Currently only UCC EAN 128 will create these errors
            if (this.BarCodeData?.Details.HasErrors != true)
            {
                // no errors!
                return;
            }

            // UCC EAN 128
            // Suppress Rule Enabled
            if (this.BarCodeData.Symbology == BarCodeSymbology.UCC_EAN_128_Symbol &&
                this.BarCodeData.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents)
            {
                // Check errors against scheme
                var details = this.BarCodeData.Details;
                var count = details.ErrorCount;
                for (var i = 0; i < count; ++i)
                {
                    // Default/unhandled will be thrown
                    switch (details.GetParseExceptionReasonAt(i))
                    {
                        case BarCodeExceptionReason.ActivityDetailFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.ActivityFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.ActivityItemFKRefno_InvalidFormat:
                        case BarCodeExceptionReason.CountOfTradeItems_InvalidInteger:
                        case BarCodeExceptionReason.DimensionMeters_InvalidDecimal:
                        case BarCodeExceptionReason.DimensionMeters_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.DueDate_InvalidDate:
                        case BarCodeExceptionReason.ExpiryUseByDate_InvalidDate:
                        case BarCodeExceptionReason.GrossWeightMetricKG_InvalidDecimal:
                        case BarCodeExceptionReason.GrossWeightMetricKG_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.GrossWeightNonMetricLB_InvalidDecimal:
                        case BarCodeExceptionReason.GrossWeightNonMetricLB_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.NetWeightMetricKG_InvalidDecimal:
                        case BarCodeExceptionReason.NetWeightMetricKG_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.NetWeightNonMetricLB_InvalidDecimal:
                        case BarCodeExceptionReason.NetWeightNonMetricLB_UnknownDecimalPlaces:
                        case BarCodeExceptionReason.PackagingDate_InvalidDate:
                        case BarCodeExceptionReason.VariableCount_InvalidInteger:
                            break;

                        case BarCodeExceptionReason.BatchLotNumber_InvalidFormat:
                            if (this.ProdScheme.LabelInfo.IsLotTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate:
                            if (this.ProdScheme.LabelInfo.IsBestBeforeDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.ProductionDate_InvalidDate:
                            if (this.ProdScheme.LabelInfo.IsProductionDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.ProductVariant_InvalidInteger:
                            if (this.ProdScheme.LabelInfo.IsVariantTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber:
                            if (this.ProdScheme.LabelInfo.IsSerialTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.InvalidFormat:
                            // Unknown error, throw
                            details.ThrowOnlyOneError(i);
                            break;
                    }
                }
            }
            else
            {
                // Suppress rule not enabled and/or not UCC EAN 128
                // Errors exist.
                // Throw first error
                this.BarCodeData.Details.ThrowOnlyOneError(0);
            }
        }
    }
}