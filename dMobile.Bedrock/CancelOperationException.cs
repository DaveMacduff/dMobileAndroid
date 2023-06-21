// <copyright file="CancelOperationException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Cancel operation exception.
    /// </summary>
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
        /// <param name="message"> Cancellation reason message.</param>
        public CancelOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOperationException"/> class.
        /// </summary>
        /// <param name="message"> Cancellation reason message.</param>
        /// <param name="inner"> Inner exception.</param>
        public CancelOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
