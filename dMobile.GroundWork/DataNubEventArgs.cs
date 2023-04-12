// <copyright file="DataNubEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;

    public class DataNubEventArgs : EventArgs
    {

        /// <summary>
        ///
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="DataNubEventArgs"/> class.
        /// </summary>
        public DataNubEventArgs()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msgData"></param>
        /// <summary>
        /// Initializes a new instance of the <see cref="DataNubEventArgs"/> class.
        /// </summary>
        /// <param name="msgData"></param>
        public DataNubEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        public string StatusOutMsg { get; set; } = string.Empty;
    }
}
