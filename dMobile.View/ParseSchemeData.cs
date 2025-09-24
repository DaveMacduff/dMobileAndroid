// <copyright file="ParseSchemeData.cs" company="InterSect Business Systems Inc">
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
    ///     Parse scheme data.
    /// </summary>
    public class ParseSchemeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseSchemeData"/> class.
        /// </summary>
        /// <param name="prodScheme">Product scheme.</param>
        /// <param name="barParse">Parsed Bar Code data or null.</param>
        /// <param name="defaultSchemeData">Default scheme data.</param>
        /// <param name="currSkid">Current skid information.</param>
        public ParseSchemeData(
            ProdScheme prodScheme,
            BarCodeParse barParse,
            ActItemSchemeData defaultSchemeData,
            SSkidInfo currSkid)
        {
            this.ProductSchemeInfo = prodScheme ?? throw new ArgumentNullException(nameof(prodScheme));
            ////this.BarCodeData = barParse ?? throw new ArgumentNullException(nameof(barParse));
            this.DefaultSchemeData = defaultSchemeData ?? throw new ArgumentNullException(nameof(defaultSchemeData));
            this.CurrentSkid = currSkid ?? throw new ArgumentNullException(nameof(currSkid));
        }

        private ProdScheme ProductSchemeInfo { get; } = new ProdScheme();

        private BarCodeParse BarCodeData { get; }

        private ActItemSchemeData DefaultSchemeData { get; } = new ActItemSchemeData();

        private SSkidInfo CurrentSkid { get; } = new SSkidInfo();

        private ProductSchemeData ProdSchemeData { get; set; } = new ProductSchemeData();

        private bool DefaultSchemeDataAssigned { get; set; } = false;

        private SQLiteAsyncConnection Conn { get; } = DatabaseConnection.GetInstance().SqlConn;

        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gather the scheme data from the activity detail and bar code parse.
        /// </summary>
        /// <param name="prodSchemeData">Product scheme data to be assigned.</param>
        /// <param name="defaultSchemeDataAssigned">Flag to show that default scheme data was assigned.</param>
        /// <returns>Scheme data collected.</returns>
        public ProductSchemeData GatherSchemeData(ProductSchemeData prodSchemeData, ref bool defaultSchemeDataAssigned)
        {
            this.ProdSchemeData = prodSchemeData ?? throw new ArgumentNullException(nameof(prodSchemeData));

            // Need local variable to track if default data was assigned.
            // Cannot use the defaultSchemeDataAssigned directly because it is a reference parameter.
            this.DefaultSchemeDataAssigned = false;

            NebraskBeefRule();

            // <issue="13609"/>
            this.AssertSuppressedErrorsAgainstScheme();
            AssignSkidNumberFromSelectedSkid();
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

            if (AssignSerialNumber())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            if (AssignVariant())
            {
                this.DefaultSchemeDataAssigned = true;
            }

            defaultSchemeDataAssigned = this.DefaultSchemeDataAssigned;
            return this.ProdSchemeData;
        }

        /// <summary>
        ///     <para>Checks to see if we have to throw any suppressed errors from Bar Code parsing.</para>
        ///     <para>
        ///         Rule Enabled
        ///         If an error is tracked in the scheme, throw first error from a tracked field.
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
        private void AssertSuppressedErrorsAgainstScheme()
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
                            if (this.ProductSchemeInfo.IsLotTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate:
                            if (this.ProductSchemeInfo.IsBestBeforeDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate:
                            break;

                        case BarCodeExceptionReason.ProductionDate_InvalidDate:
                            if (this.ProductSchemeInfo.IsProductionDateTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.ProductVariant_InvalidInteger:
                            if (this.ProductSchemeInfo.IsVariantTracked)
                            {
                                details.ThrowOnlyOneError(i);
                            }

                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber_InvalidFormat:
                            break;

                        case BarCodeExceptionReason.SingleTradeItemSerialNumber:
                            if (this.ProductSchemeInfo.IsSerialTracked)
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

            static bool IsNebraskaBeefRuleMet(BarCodeParse barParse)
            {
                return (barParse.Gtin.Contains("0740780")
                        || barParse.Gtin.Contains("19336"))
                        && !string.IsNullOrEmpty(barParse.LogisticUnitBatchLotNumber);
            }
        }

        private bool AssignBestBefore()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
            if (this.ProductSchemeInfo.IsBestBeforeDateTracked)
            {
                if (this.BarCodeData == null || SysCommon.IsEmptyDateTime(this.BarCodeData.LogisticUnitBestBeforeSellByDate))
                {
                    var defaultDate = this.DefaultSchemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate;
                    if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues &&
                        this.ProductSchemeInfo.IsBestBeforeDateTrackedDefault &&
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

        private bool AssignProductionDate()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
            if (this.ProductSchemeInfo.IsProductionDateTracked)
            {
                if (this.BarCodeData == null || SysCommon.IsEmptyDateTime(this.BarCodeData.LogisticUnitProductionDate))
                {
                    var defaultDate = this.DefaultSchemeData.ProdSchemeData.LogisticUnitProductionDate;
                    if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues &&
                        this.ProductSchemeInfo.IsProductionDateTrackedDefault &&
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

        private bool AssignLotNumber()
        {
            this.ProdSchemeData.LogisticUnitBatchLotNumber = string.Empty;
            var defaultDataAssigned = false;
            if (this.ProductSchemeInfo.IsLotTracked)
            {
                if (this.BarCodeData == null || this.BarCodeData.LogisticUnitBatchLotNumber?.Length == 0)
                {
                    this.ProdSchemeData.LogisticUnitBatchLotNumber =
                        this.DefaultSchemeData.ProdSchemeData.LogisticUnitBatchLotNumber;
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
                if (this.SysEnv.SysSettings.IsEnableDefaultDetailValues && this.ProductSchemeInfo.IsLotTrackedDefault)
                {
                    var lotCodeDefault = string.Empty;
                    if (this.BarCodeData != null)
                    {
                        lotCodeDefault = GetLotCodeDefaultFromSupplierCompanyPrefix(
                            this.ProductSchemeInfo.SchemeRefno,
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
                            if (globalTradeIdentifier.Contains(Convert.ToString(xRef.ProductSchemeLotNumberCode).TrimEnd()))
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

        private bool AssignSerialNumber()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.SingleTradeItemSerialNumber = string.Empty;
            if (this.ProductSchemeInfo.IsSerialTracked)
            {
                if (this.BarCodeData == null)
                {
                    this.ProdSchemeData.SingleTradeItemSerialNumber =
                         this.DefaultSchemeData.ProdSchemeData.SingleTradeItemSerialNumber;
                    defaultDataAssigned = true;
                }
                else if (this.BarCodeData.SingleTradeItemSerialNumber == null)
                {
                    if (this.BarCodeData.SingleTradeItemSerialNumber?.Length == 0)
                    {
                        this.ProdSchemeData.SingleTradeItemSerialNumber =
                            this.DefaultSchemeData.ProdSchemeData.SingleTradeItemSerialNumber;
                        defaultDataAssigned = true;
                    }
                    else
                    {
                        this.ProdSchemeData.SingleTradeItemSerialNumber = this.BarCodeData.SingleTradeItemSerialNumber;
                    }
                }
            }

            return defaultDataAssigned;
        }

        private bool AssignVariant()
        {
            var defaultDataAssigned = false;
            this.ProdSchemeData.VariantCode = string.Empty;
            this.ProdSchemeData.VariantDescription = string.Empty;
            this.ProdSchemeData.VariantNumber = 0;
            if (this.ProductSchemeInfo.IsVariantTracked)
            {
                if (this.BarCodeData == null)
                {
                    this.ProdSchemeData.VariantCode = this.DefaultSchemeData.ProdSchemeData.VariantCode;
                    this.ProdSchemeData.VariantDescription = this.DefaultSchemeData.ProdSchemeData.VariantDescription;
                    this.ProdSchemeData.VariantNumber = this.DefaultSchemeData.ProdSchemeData.VariantNumber;
                    defaultDataAssigned = true;
                }
                else
                {
                    if (this.BarCodeData.LogisticUnitProductVariant == 0)
                    {
                        this.ProdSchemeData.VariantCode = this.DefaultSchemeData.ProdSchemeData.VariantCode;
                        this.ProdSchemeData.VariantDescription = this.DefaultSchemeData.ProdSchemeData.VariantDescription;
                        this.ProdSchemeData.VariantNumber = this.DefaultSchemeData.ProdSchemeData.VariantNumber;
                        defaultDataAssigned = true;
                    }
                    else
                    {
                        this.ProdSchemeData.VariantNumber = this.BarCodeData.LogisticUnitProductVariant;
                        ActDetail.FillSchemeDataVarInfoFromSchemeDataVarNo(this.ProdSchemeData).GetAwaiter().GetResult();
                    }
                }
            }

            return defaultDataAssigned;
        }

        private void AssignSkidNumberFromSelectedSkid()
        {
            // Decided not to use the CurrSkid.LogisticUnitSkidCode because is is the full GS1 bar code. 18 digits.
            // Instead I am using the CurrSkid.SkidNo.
            // The SkidInfo.CurrSkid.SkidNo could have been assigned by manually selecting or creating a new skid.
            // Or it could have be scanned and the skid info looked up.
            this.ProdSchemeData.LogisticUnitSkidNumber = this.CurrentSkid.SkidCode;
        }
    }
}
