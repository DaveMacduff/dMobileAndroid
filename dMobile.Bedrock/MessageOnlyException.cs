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
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        public MessageOnlyException(string message)
            : base(ModifyMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class.
        /// </summary>
        /// <param name="message">.</param>
        /// <param name="inner">.</param>
        public MessageOnlyException(string message, Exception inner)
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
                errMsg = message.Substring(message.IndexOf(msgToken) + msgToken.Length);
            }
            else
            {
                errMsg = message;
            }

            return errMsg;
        }
    }
}
