// <copyright file="Message.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Represents a message with an index and value.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the index of the message.
        /// </summary>
        public int MessageIndex { get; set; }

        /// <summary>
        /// Gets or sets the value of the message.
        /// </summary>
        public string MessageValue { get; set; }
    }
}