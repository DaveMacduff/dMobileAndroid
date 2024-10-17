// <copyright file="BarCodeParseErrorDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using dMobile.Bedrock;

    /// <summary>
    /// Represents details of a barcode parsing error.
    /// </summary>
    public class BarCodeParseErrorDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeParseErrorDetail"/> class.
        /// </summary>
        /// <param name="reason">The reason for the barcode parsing error.</param>
        /// <param name="exception">The exception that was thrown during parsing.</param>
        public BarCodeParseErrorDetail(BarCodeExceptionReason reason, BarCodeParseException exception)
        {
            this.Reason = reason;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the reason for the barcode parsing error.
        /// </summary>
        public BarCodeExceptionReason Reason { get; }

        /// <summary>
        /// Gets the exception that was thrown during parsing.
        /// </summary>
        public BarCodeParseException Exception { get; }
    }
}