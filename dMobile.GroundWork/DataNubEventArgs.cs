// <copyright file="DataNubEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;

    /// <summary>
    /// Represents event data for DataNub events.
    /// </summary>
    public class DataNubEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataNubEventArgs"/> class.
        /// </summary>
        public DataNubEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataNubEventArgs"/> class with a specified message.
        /// </summary>
        /// <param name="msgData">The message data.</param>
        public DataNubEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the status output message.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;
    }
}