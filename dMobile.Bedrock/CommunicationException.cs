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
        /// Initializes a new instance of the <see cref="CommunicationException"/> class.
        /// </summary>
        public CommunicationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public CommunicationException(string message)
            : base(ModifyMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public CommunicationException(string message, Exception inner)
            : base(ModifyMessage(message), inner)
        {
        }

        /// <summary>
        /// The ModifyMessage.
        /// </summary>
        /// <param name="message">.</param>
        /// <returns>.</returns>
        private static string ModifyMessage(string message)
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
