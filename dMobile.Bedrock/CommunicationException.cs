// <copyright file="CommunicationException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="CommunicationException" />.
    /// </summary>
    public class CommunicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public CommunicationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CommunicationException(string message)
            : base(ModifyMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no
        /// inner exception is specified.</param>
        public CommunicationException(string message, Exception inner)
            : base(ModifyMessage(message), inner)
        {
        }

        /// <summary>
        /// Extracts the error message from the specified message string by removing a known prefix if present.
        /// </summary>
        /// <param name="message">The message string to process. If the string contains the prefix "MSGBOX-", the
        /// prefix and any preceding text are removed.</param>
        /// <returns>A string containing the error message without the "MSGBOX-" prefix, or the original message
        /// if the prefix is not found.</returns>
        private static string ModifyMessage(string message)
        {
            const string msgToken = "MSGBOX-";
            string errMsg;
            if (message.Contains(msgToken))
            {
                errMsg = message.Substring(message.IndexOf(msgToken, StringComparison.Ordinal) + msgToken.Length);
            }
            else
            {
                errMsg = message;
            }

            return errMsg;
        }
    }
}
