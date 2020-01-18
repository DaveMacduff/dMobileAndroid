using System;
using System.Collections.Generic;
using System.Text;
using dMobile.Bedrock;

namespace dMobile.BarCodeScanner
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
        BatchLotNumber_InvalidFormat,
        SingleTradeItemSerialNumber,

        /// <summary>
        /// Couldn't parse "Serial Number" - Invalid Format
        /// </summary>
        SerialNumber_InvalidFormat,

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

    /// <summary>
    /// Class contains details of parsing.
    /// It contains fields that failed and their error messages.
    /// </summary>
    public class BarCodeParseDetails
    {

        /// <summary>
        /// List of all the error details.
        /// </summary>
        List<BarCodeParseErrorDetail> errorDetails;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BarCodeParseDetails()
        {
            errorDetails = new List<BarCodeParseErrorDetail>();
        }

        /// <summary>
        /// Adds a detail of what went wrong with the parse.
        /// </summary>
        /// <param name="type">Field which failed the parse and the reason for it.</param>
        /// <param name="e">Exception containing the human readable details.</param>
        public void AddErrorDetail(BarCodeExceptionReason type, BarCodeParseException e)
        {
            errorDetails.Add(new BarCodeParseErrorDetail(type, e));
        }


        /// <summary>
        /// Gets the exception at the specified index.
        /// </summary>
        /// <param name="index">Index of the exception.</param>
        /// <returns>Exception if one is found, otherwise null.</returns>
        public BarCodeParseException GetParseExceptionAt(int index)
        {
            BarCodeParseErrorDetail detail = GetErrorDetailAt(index);

            if (detail == null)
                return null;

            return detail.Exception;
        }

        /// <summary>
        /// Gets the exception reason at the specified index.
        /// </summary>
        /// <param name="index">Index of the reason.</param>
        /// <returns>Reason if one is found, otherwise InvalidFormat.</returns>
        public BarCodeExceptionReason GetParseExceptionReasonAt(int index)
        {
            BarCodeParseErrorDetail detail = GetErrorDetailAt(index);

            if (detail == null)
                return BarCodeExceptionReason.InvalidFormat;

            return detail.Reason;
        }

        /// <summary>
        /// Gets the error detail at the specified index.
        /// Null if out of bounds.
        /// </summary>
        /// <param name="index">Index of the error detail.</param>
        /// <returns>ErrorDetail if one is found, otherwise null.</returns>
        public BarCodeParseErrorDetail GetErrorDetailAt(int index)
        {
            if (index >= 0 && index < ErrorCount)
                return errorDetails[index];
            return null;
        }

        /// <summary>
        /// Removes the error from the list, then throws it.
        /// Ensures that it is only thrown once.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="BarCodeParseException">Throws an exception referring to the failed parsing of a field.</exception>
        public void ThrowOnlyOneError(int index)
        {
            BarCodeParseException e = GetParseExceptionAt(index);
            if (e != null)
            {
                ClearErrors();
                throw e;
            }
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        public void ClearErrors()
        {
            if (errorDetails != null)
                errorDetails.Clear();
        }

        /// <summary>
        /// Gets an iterable for all parse error details.
        /// </summary>
        public IEnumerable<BarCodeParseErrorDetail> ErrorDetails
        {
            get
            {
                foreach (BarCodeParseErrorDetail detail in errorDetails)
                {
                    yield return detail;
                }
            }
        }

        /// <summary>
        /// Checks if there are any errors.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return errorDetails.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of errors.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return errorDetails.Count;
            }
        }
    }
}
