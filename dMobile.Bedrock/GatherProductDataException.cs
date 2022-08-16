// <copyright file="GatherProductDataException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="GatherProductDataException" />.
    /// </summary>
    public class GatherProductDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatherProductDataException"/> class.
        /// </summary>
        public GatherProductDataException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatherProductDataException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public GatherProductDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatherProductDataException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public GatherProductDataException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatherProductDataException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="suppressError">.</param>
        public GatherProductDataException(string message, bool suppressError)
            : base(message)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatherProductDataException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        /// <param name="suppressError">.</param>
        public GatherProductDataException(string message, Exception inner, bool suppressError)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SuppressError.
        /// </summary>
        public bool SuppressError { get; set; } = false;
    }
}
