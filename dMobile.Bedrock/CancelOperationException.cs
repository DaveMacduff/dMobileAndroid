// <copyright file="CancelOperationException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CancelOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        public CancelOperationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public CancelOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public CancelOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="suppressError">.</param>
        public CancelOperationException(string message, bool suppressError)
            : base(message)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        /// <param name="suppressError">.</param>
        public CancelOperationException(string message, Exception inner, bool suppressError)
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
