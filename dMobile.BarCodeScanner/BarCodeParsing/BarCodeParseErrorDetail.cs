// <copyright file="BarCodeParseErrorDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using dMobile.Bedrock;

    public class BarCodeParseErrorDetail
    {
        private BarCodeParseException exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeParseErrorDetail"/> class.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="exception"></param>
        public BarCodeParseErrorDetail(BarCodeExceptionReason reason, BarCodeParseException exception)
        {
            this.Reason = reason;
            this.exception = exception;
        }

        /// <summary>
        /// Gets the Bar Code Exception type.
        /// </summary>
        public BarCodeExceptionReason Reason { get; }

        /// <summary>
        /// Gets the Bar Code Parse Exception.
        /// </summary>
        public BarCodeParseException Exception
        {
            get
            {
                return this.exception;
            }
        }
    }
}
