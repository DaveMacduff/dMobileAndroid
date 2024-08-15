// <copyright file="SysSocketInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System.Net.Sockets;

    /// <summary>
    ///     Socket wrapper class to hold socket instance and try again flag.
    /// </summary>
    public class SysSocketInfo
    {
        /// <summary>
        ///    Gets or sets the socket.
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        ///    Gets or sets a value indicating whether to try to connect to a new port.
        /// </summary>
        public bool TryNewPort { get; set; }
    }
}
