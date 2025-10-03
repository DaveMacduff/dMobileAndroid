// <copyright file="MessageOnlyException.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="MessageOnlyException" />.
    /// </summary>
    public class MessageOnlyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        public MessageOnlyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public MessageOnlyException(string message)
            : base(RemoveMSGBOXToken(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public MessageOnlyException(string message, Exception inner)
            : base(RemoveMSGBOXToken(message), inner)
        {
        }

        /// <summary>
        /// Remove MSGBOX- token from message if it exists.
        /// </summary>
        /// <param name="message">Message to clean.</param>
        /// <returns>Cleaned message.</returns>
        private static string RemoveMSGBOXToken(string message)
        {
            var msgToken = "MSGBOX-";
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
