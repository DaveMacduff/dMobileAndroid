// <copyright file="BarCodeParseDetails.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System.Collections.Generic;
    using Bedrock;

    /// <summary>
    /// Class contains details of parsing.
    /// It contains fields that failed and their error messages.
    /// </summary>
    public class BarCodeParseDetails
    {
        /// <summary>
        /// List of all the error details.
        /// </summary>
        private readonly List<BarCodeParseErrorDetail> errorDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeParseDetails"/> class.
        /// </summary>
        public BarCodeParseDetails()
        {
            this.errorDetails = new List<BarCodeParseErrorDetail>();
        }

        /// <summary>
        /// Gets a value indicating whether there are any errors.
        /// </summary>
        public bool HasErrors => this.errorDetails.Count > 0;

        /// <summary>
        /// Gets the number of errors.
        /// </summary>
        public int ErrorCount => this.errorDetails.Count;

        /// <summary>
        /// Gets an enumerable for all parse error details.
        /// </summary>
        public IEnumerable<BarCodeParseErrorDetail> ErrorDetails
        {
            get
            {
                foreach (var detail in this.errorDetails)
                {
                    yield return detail;
                }
            }
        }

        /// <summary>
        /// Adds a detail of what went wrong with the parse.
        /// </summary>
        /// <param name="type">Field which failed the parse and the reason for it.</param>
        /// <param name="e">Exception containing the human readable details.</param>
        public void AddErrorDetail(BarCodeExceptionReason type, BarCodeParseException e)
        {
            this.errorDetails.Add(new BarCodeParseErrorDetail(type, e));
        }

        /// <summary>
        /// Gets the exception reason at the specified index.
        /// </summary>
        /// <param name="index">Index of the reason.</param>
        /// <returns>Reason if one is found, otherwise InvalidFormat.</returns>
        public BarCodeExceptionReason GetParseExceptionReasonAt(int index)
        {
            var detail = this.GetErrorDetailAt(index);

            if (detail == null)
            {
                return BarCodeExceptionReason.InvalidFormat;
            }

            return detail.Reason;
        }

        /// <summary>
        /// Removes the error from the list, then throws it.
        /// Ensures that it is only thrown once.
        /// </summary>
        /// <param name="index">Index of the error to throw.</param>
        /// <exception cref="BarCodeParseException">Throws an exception referring to the failed parsing of a field.</exception>
        public void ThrowOnlyOneError(int index)
        {
            var e = this.GetParseExceptionAt(index);
            if (e != null)
            {
                this.ClearErrors();
                throw e;
            }
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        public void ClearErrors()
        {
            this.errorDetails?.Clear();
        }

        /// <summary>
        /// Gets the exception at the specified index.
        /// </summary>
        /// <param name="index">Index of the exception.</param>
        /// <returns>Exception if one is found, otherwise null.</returns>
        private BarCodeParseException GetParseExceptionAt(int index)
        {
            var detail = this.GetErrorDetailAt(index);
            return detail?.Exception;
        }

        /// <summary>
        /// Gets the error detail at the specified index.
        /// Null if out of bounds.
        /// </summary>
        /// <param name="index">Index of the error detail.</param>
        /// <returns>ErrorDetail if one is found, otherwise null.</returns>
        private BarCodeParseErrorDetail GetErrorDetailAt(int index)
        {
            if (index >= 0 &&
                index < this.ErrorCount)
            {
                return this.errorDetails[index];
            }

            return null;
        }
    }
}