// <copyright file="ContactLoginEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="ContactLoginEventArgs" />.
    /// </summary>
    public class ContactLoginEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactLoginEventArgs"/> class.
        /// </summary>
        public ContactLoginEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactLoginEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public ContactLoginEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the StatusOutMsg.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;
    }
}
