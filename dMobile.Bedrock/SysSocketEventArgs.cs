// <copyright file="SysSocketEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="SysSocketEventArgs" />.
    /// </summary>
    public class SysSocketEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysSocketEventArgs"/> class.
        /// </summary>
        public SysSocketEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSocketEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public SysSocketEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        // PROPERTIES
        /// <summary>
        /// Gets or sets the StatusOutMsg.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;
    }
}
