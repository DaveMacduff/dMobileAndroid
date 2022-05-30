// <copyright file="BarCodeExceptionReason.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Detailed account of what went wrong in a parsing.
    /// </summary>
    public enum BarCodeExceptionReason
    {
        /// <summary>
        /// Couldn't parse "Best Before Sell By Date" - Invalid Date
        /// </summary>
        BestBeforeSellByDate_InvalidDate,

        /// <summary>
        /// Couldn't parse "Expiry Use By Date" - Invalid Date
        /// </summary>
        ExpiryUseByDate_InvalidDate,

        /// <summary>
        /// Couldn't parse "Due Date" - Invalid Date
        /// </summary>
        DueDate_InvalidDate,

        /// <summary>
        /// Couldn't parse "Production Date" - Invalid Date
        /// </summary>
        ProductionDate_InvalidDate,

        /// <summary>
        /// Couldn't parse "Packaging Date" - Invalid Date
        /// </summary>
        PackagingDate_InvalidDate,

        /// <summary>
        /// Date failed to parse. Would normally be "Packaging Date",
        /// but there is a rule to use it as a "Production Date".
        /// </summary>
        PackagingDateAsProductionDate_InvalidDate,

        /// <summary>
        /// Couldn't parse "Product Variant" - Invalid Integer
        /// </summary>
        ProductVariant_InvalidInteger,

        /// <summary>
        /// Couldn't parse "Count of Trade Items" - Invalid Integer
        /// </summary>
        CountOfTradeItems_InvalidInteger,

        /// <summary>
        /// Couldn't parse "Variable Count" - Invalid Integer
        /// </summary>
        VariableCount_InvalidInteger,

        /// <summary>
        /// Couldn't parse "Batch lot number" - Invalid format.
        /// </summary>
        BatchLotNumber_InvalidFormat,

        /// <summary>
        /// Couldn't parse "Serial Number"
        /// </summary>
        SingleTradeItemSerialNumber,

        /// <summary>
        /// Couldn't parse "Serial Number" - Invalid Format
        /// </summary>
        SingleTradeItemSerialNumber_InvalidFormat,

        /// <summary>
        /// Couldn't parse "Activity FK Refno" - Invalid Format
        /// </summary>
        ActivityFKRefno_InvalidFormat,

        /// <summary>
        /// Couldn't parse "Activity Item FK Refno" - Invalid Format
        /// </summary>
        ActivityItemFKRefno_InvalidFormat,

        /// <summary>
        /// Couldn't parse "Activity Detail FK Refno" - Invalid Format
        /// </summary>
        ActivityDetailFKRefno_InvalidFormat,

        /// <summary>
        /// Couldn't parse "Dimension Meters" - Invalid Decimal
        /// </summary>
        DimensionMeters_InvalidDecimal,

        /// <summary>
        /// Couldn't parse "Dimension Meters" - Unknown Decimal Places
        /// </summary>
        DimensionMeters_UnknownDecimalPlaces,

        /// <summary>
        /// Couldn't parse "Metric Net Weight KG" - Invalid Decimal
        /// </summary>
        NetWeightMetricKG_InvalidDecimal,

        /// <summary>
        /// Couldn't parse "Metric Net Weight KG" - Unknown Decimal Places
        /// </summary>
        NetWeightMetricKG_UnknownDecimalPlaces,

        /// <summary>
        /// Couldn't parse "Non Metric Net Weight LB" - Invalid Decimal
        /// </summary>
        NetWeightNonMetricLB_InvalidDecimal,

        /// <summary>
        /// Couldn't parse "Non Metric Net Weight LB" - Unknown Decimal Places
        /// </summary>
        NetWeightNonMetricLB_UnknownDecimalPlaces,

        /// <summary>
        /// Couldn't parse "Metric Gross Weight KG" - Invalid Decimal
        /// </summary>
        GrossWeightMetricKG_InvalidDecimal,

        /// <summary>
        /// Couldn't parse "Metric Gross Weight KG" - Unknown Decimal Places
        /// </summary>
        GrossWeightMetricKG_UnknownDecimalPlaces,

        /// <summary>
        /// Couldn't parse "Non Metric Gross Weight LB" - Invalid Decimal
        /// </summary>
        GrossWeightNonMetricLB_InvalidDecimal,

        /// <summary>
        /// Couldn't parse "Non Metric Gross Weight LB" - Unknown Decimal Places
        /// </summary>
        GrossWeightNonMetricLB_UnknownDecimalPlaces,

        /// <summary>
        /// Catch all failure type.
        /// </summary>
        InvalidFormat,
    }
}
