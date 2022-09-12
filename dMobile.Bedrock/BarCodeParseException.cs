// <copyright file="BarCodeParseException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Define the bar code parse exception.
    /// </summary>
    public class BarCodeParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeParseException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Exception message.</param>
        /// <param name="field"><see cref="string"/> Field name.</param>
        /// <param name="innerException"><see cref="Exception"/> Inner exception.</param>
        public BarCodeParseException(string message, string field, Exception innerException)
            : base(
                $"Could not parse '{field}'. {message}",
                innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeParseException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Exception message.</param>
        /// <param name="innerException"><see cref="Exception"/> Inner exception.</param>
        public BarCodeParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
