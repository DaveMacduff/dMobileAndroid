// <copyright file="IsReadyToApplyException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="IsReadyToApplyException" />.
    /// </summary>
    public class IsReadyToApplyException : Exception
    {
        /// <summary>
        /// Defines the isOverContinue.
        /// </summary>
        private bool isOverContinue;

        /// <summary>
        /// Gets or sets a value indicating whether SuppressError.
        /// </summary>
        public bool SuppressError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        public IsReadyToApplyException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public IsReadyToApplyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public IsReadyToApplyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="suppressError">.</param>
        public IsReadyToApplyException(string message, bool suppressError)
            : base(message)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="suppressError">.</param>
        /// <param name="isOverContinue">.</param>
        public IsReadyToApplyException(string message, bool suppressError, bool isOverContinue)
            : base(message)
        {
            this.SuppressError = suppressError;
            this.IsOverContinue = isOverContinue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        /// <param name="suppressError">.</param>
        public IsReadyToApplyException(string message, Exception inner, bool suppressError)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsReadyToApplyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        /// <param name="suppressError">.</param>
        /// <param name="isOverContinue">.</param>
        public IsReadyToApplyException(string message, Exception inner, bool suppressError, bool isOverContinue)
            : base(message, inner)
        {
            this.SuppressError = suppressError;
            this.IsOverContinue = isOverContinue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsOverContinue.
        /// </summary>
        public bool IsOverContinue
        {
            get => this.isOverContinue;

            set => this.isOverContinue = value;
        }
    }
}
