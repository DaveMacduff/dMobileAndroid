// <copyright file="RuleViolationException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner
{
    using System;

    /// <summary>
    /// Defines the <see cref="RuleViolationException" />.
    /// </summary>
    public class RuleViolationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViolationException"/> class.
        /// </summary>
        public RuleViolationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViolationException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Rule validation message.</param>
        public RuleViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViolationException"/> class.
        /// </summary>
        /// <param name="message"><see cref="string"/> Rule validation message.</param>
        /// <param name="inner"><see cref="Exception"/> Inner exception.</param>
        public RuleViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
