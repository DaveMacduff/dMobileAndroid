// <copyright file="SysSocket.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Handles Communication via a socket to send and receive data.
    /// </summary>
    public class SysSocket : ISysSocketEventPublisher
    {
        /// <summary>
        /// Defines the receiveEOM.
        /// </summary>
        private string receiveEOM = "</EOM>" + (char)20;

        /// <summary>
        /// Defines the recvTimeOutMilliseconds.
        /// </summary>
        private int recvTimeOutMilliseconds = 4102;

        /// <summary>
        /// The OnSysSocketStatusOutEventHandler.
        /// </summary>
        /// <param name="sysSocketEventPublisher">The sysSocketEventPublisher<see cref="ISysSocketEventPublisher"/>.</param>
        /// <param name="sysSocketEventArgs">The sysSocketEventArgs<see cref="SysSocketEventArgs"/>.</param>
        private delegate void OnSysSocketStatusOutEventHandler(
            ISysSocketEventPublisher sysSocketEventPublisher,
            SysSocketEventArgs sysSocketEventArgs);

        /// <summary>
        /// Defines the OnSysSocketStatusOut.
        /// </summary>
        private event OnSysSocketStatusOutEventHandler OnSysSocketStatusOut;

        /// <summary>
        /// Gets or sets the ServerIP.
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Gets or sets the SendEOM.
        /// </summary>
        public string SendEOM { get; set; } = "</WAIT_EOM>" + (char)20;

        /// <summary>
        /// Gets or sets the ReceiveEOM.
        /// </summary>
        public string ReceiveEOM
        {
            get
            {
                return this.receiveEOM;
            }

            set
            {
                this.receiveEOM = value;
            }
        }

        /// <summary>
        /// Gets or sets the RecvBuffSize.
        /// </summary>
        public int RecvBuffSize { get; set; } = 32768;

        /// <summary>
        /// Gets or sets the ConnTotalRetries.
        /// </summary>
        public int ConnTotalRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the SendTotalRetries.
        /// </summary>
        public int SendTotalRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the RecvTotalRetries.
        /// </summary>
        public int RecvTotalRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the DefSocketPort.
        /// </summary>
        public int DefSocketPort { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the BasePort.
        /// </summary>
        public int BasePort { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the MaxPorts.
        /// </summary>
        public int MaxPorts { get; set; } = 1;

        /// <summary>
        /// Gets or sets the SendTimeOutMilliseconds.
        /// </summary>
        public int SendTimeOutMilliseconds { get; set; } = 4101;

        /// <summary>
        /// Gets or sets the ReceiveTimeOutMilliseconds.
        /// </summary>
        public int ReceiveTimeOutMilliseconds
        {
            get
            {
                return this.recvTimeOutMilliseconds;
            }

            set
            {
                this.recvTimeOutMilliseconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Create and return a connected socket object.
        /// </summary>
        /// <returns><see cref="Socket"/> Connect socket instance or null.</returns>
        public Socket Connect()
        {
            Socket socket = null;
            var port = this.DefSocketPort;
            Exception exConn = null;
            var sockEventArgs = new SysSocketEventArgs();

            for (var attempt = 1; attempt <= this.ConnTotalRetries; attempt++)
            {
                try
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sockEventArgs.StatusOutMsg = string.Format("IP address {0}", this.ServerIP);
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    var endPoint = new IPEndPoint(IPAddress.Parse(this.ServerIP), port);
                    var tempSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    tempSocket.Blocking = true;

                    if (this.OnSysSocketStatusOut != null)
                    {
                        sockEventArgs.StatusOutMsg = string.Format("Connection attempt {0} port {1}.", attempt, port);
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    tempSocket.Connect(endPoint);

                    if (tempSocket.Connected)
                    {
                        socket = tempSocket;

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection successful.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }
                    }

                    if (socket == null)
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt failed on port {0}.", port);
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;

                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }
                    }
                    else
                    {
                        // Dim SysEnv As SysEnvironment = SysEnvironment.GetInstance()
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.SendTimeout,
                        // SysEnv.SysSocketInstance.iSendTimeOutMilliseconds)
                        // RaiseEvent OnSysSocketStatusOut(Me, EventArgs.Empty, "Setting Send Timeout")
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.ReceiveTimeout,
                        // SysEnv.SysSocketInstance.iReceiveTimeOutMilliseconds)
                        // RaiseEvent OnSysSocketStatusOut(Me, EventArgs.Empty, "Setting Send Timeout")
                        break;
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10060)
                    {
                        exConn = ex;
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";

                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;
                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }

                        continue;
                    }
                    else if (ex.ErrorCode == 10061)
                    {
                        exConn = ex;
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt refused.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;
                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }

                        continue;
                    }
                    else
                    {
                        exConn = ex;

                        // Raised multiple events to keep message size down
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket exception occurred.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("Code {0}.", ex.ErrorCode);
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Msg " + ex.Message;
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        continue;
                    }
                }
            }

            if (socket == null & exConn != null)
            {
                throw exConn;
            }

            return socket;
        }

        /// <summary>
        /// Create and return a connected socket object.
        /// </summary>
        /// <returns>.</returns>
        public Socket ConnectWithHostEntry()
        {
            Socket socket = null;
            SocketException exConn = null;
            var sockEventArgs = new SysSocketEventArgs();

            // Get host related information.
            var ipHostEntry = Dns.GetHostEntry(this.ServerIP);

            if (this.OnSysSocketStatusOut != null)
            {
                sockEventArgs.StatusOutMsg = "Getting IP host entry";
                this.OnSysSocketStatusOut(this, sockEventArgs);
            }

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host host IP Address is not compatible with the
            // address family (typical in the IPv6 case).
            IPAddress ipAddress = null;
            var port = this.DefSocketPort;

            for (var attempt = 1; attempt <= this.ConnTotalRetries; attempt++)
            {
                try
                {
                    foreach (var ipAddressLoopVariable in ipHostEntry.AddressList)
                    {
                        ipAddress = ipAddressLoopVariable;
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("IP address {0} ", ipAddress.ToString());
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        var endPoint = new IPEndPoint(ipAddress, port);
                        var tempSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        tempSocket.Blocking = true;

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt {0} port {1}.", attempt, port);
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        tempSocket.Connect(endPoint);

                        if (tempSocket.Connected)
                        {
                            socket = tempSocket;
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection successful.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }
                        }
                    }

                    if (socket == null)
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt failed on port {0}.", port);
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;
                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }
                    }
                    else
                    {
                        // Dim SysEnv As SysEnvironment = SysEnvironment.GetInstance()
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.SendTimeout,
                        // SysEnv.SysSocketInstance.iSendTimeOutMilliseconds)
                        // RaiseEvent StatusOut(Me, EventArgs.Empty, "Setting Send Timeout")
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.ReceiveTimeout,
                        // SysEnv.SysSocketInstance.iReceiveTimeOutMilliseconds)
                        // RaiseEvent StatusOut(Me, EventArgs.Empty, "Setting Send Timeout")
                        break;
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10060)
                    {
                        exConn = ex;

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;

                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }

                        continue;
                    }
                    else if (ex.ErrorCode == 10061)
                    {
                        exConn = ex;

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt refused.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port += 1;

                        if (port > this.BasePort + this.MaxPorts - 1)
                        {
                            port = this.BasePort;
                        }

                        continue;
                    }
                    else
                    {
                        exConn = ex;

                        // Raised multiple events to keep message size down
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket exception occurred.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = string.Format("Code {0}.", ex.ErrorCode);
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Msg " + ex.Message;
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        continue;
                    }
                }
            }

            if (socket == null & exConn != null)
            {
                throw exConn;
            }

            return socket;
        }

        /// <summary>
        /// Send data out via a socket.
        /// </summary>
        /// <param name="socket">.</param>
        /// <param name="request">.</param>
        /// <returns>Total number of bytes successfully sent.</returns>
        public int SendRequest(Socket socket, string request)
        {
            var attempt = 1;
            var unicode = Encoding.UTF8;
            var totalBytes2Send = unicode.GetBytes(request + this.SendEOM);

            // Add the EOF id
            var bytesRemaining2BeSent = totalBytes2Send.Length;
            var byteStartIndex = 0;
            var totalBytesSent = 0;
            var bytesSent = 0;
            var sysSocketEventArgs = new SysSocketEventArgs();

            if (this.OnSysSocketStatusOut != null)
            {
                sysSocketEventArgs.StatusOutMsg = "Sending request.";
                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
            }

            if (this.OnSysSocketStatusOut != null)
            {
                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes to send {0} ", totalBytes2Send.Length);
                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
            }

            // Send request to the server.
            while (bytesRemaining2BeSent > 0 & attempt <= this.SendTotalRetries)
            {
                try
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = string.Format("Send attempt {0} of {1} ", attempt, this.SendTotalRetries);
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    while (bytesRemaining2BeSent > 0)
                    {
                        if (socket.Poll((this.SysEnv.SysSock.SendTimeOutMilliseconds * 1000) + 1, SelectMode.SelectWrite))
                        {
                            var byteBuffer2Send = new byte[bytesRemaining2BeSent + 1];

                            if (this.OnSysSocketStatusOut != null)
                            {
                                sysSocketEventArgs.StatusOutMsg = string.Format("Sending bytes {0} ", bytesRemaining2BeSent);
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            Array.Copy(totalBytes2Send, byteStartIndex, byteBuffer2Send, 0, bytesRemaining2BeSent);

                            bytesSent = socket.Send(byteBuffer2Send, byteBuffer2Send.Length, 0);

                            if (this.OnSysSocketStatusOut != null)
                            {
                                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes sent {0} ", bytesSent);
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            byteStartIndex = byteStartIndex + bytesSent;

                            bytesRemaining2BeSent = bytesRemaining2BeSent - bytesSent;

                            if (this.OnSysSocketStatusOut != null)
                            {
                                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes remaining {0} ", bytesRemaining2BeSent);
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            totalBytesSent = totalBytesSent + bytesSent;
                        }
                        else
                        {
                            // connection timed out
                            throw new SocketException(10060);
                        }
                    }

                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = "Request sent.";
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }
                }
                catch (SocketException ex)
                {
                    // Raised multiple events to keep message size down
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = "Socket exception occurred.";
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = string.Format("Code {0}", ex.ErrorCode);
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = "Msg " + ex.Message;
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    attempt += 1;

                    // If we've reached the retry limit, throw the Exception
                    if (attempt < this.SendTotalRetries)
                    {
                        // In the event of a socket error data may still have been sent so adjust pointers
                        byteStartIndex = byteStartIndex + bytesSent;
                        bytesRemaining2BeSent = bytesRemaining2BeSent - bytesSent;
                        totalBytesSent = totalBytesSent + bytesSent;
                        continue;
                    }
                    else
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sysSocketEventArgs.StatusOutMsg = "Total send attempts exceeded.";
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        socket.Disconnect(true);
                        socket.Close();
                        throw ex;
                    }
                }
            }

            return totalBytesSent;
        }

        /// <summary>
        /// Read data from a socket.
        /// </summary>
        /// <param name="socket">.</param>
        /// <returns>A list of bytes from the socket.</returns>
        public List<byte> ReceiveResponse(Socket socket)
        {
            var bytesReceived = default(int);
            var attempt = 1;
            var bytes = new byte[this.RecvBuffSize + 1];

            // Read the bytes.
            var page = new List<byte>();
            var sysSocketEventArgs = new SysSocketEventArgs();

            do
            {
                // We will try the Receive, the RetryLimit times
                while (attempt <= this.RecvTotalRetries)
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = string.Format("Receive attempt {0} of {1} ", attempt, this.RecvTotalRetries);
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    try
                    {
                        if (socket.Poll((this.SysEnv.SysSock.ReceiveTimeOutMilliseconds * 1000) + 1, SelectMode.SelectRead))
                        {
                            bytesReceived = socket.Receive(bytes, bytes.Length, SocketFlags.None);

                            if (this.OnSysSocketStatusOut != null)
                            {
                                sysSocketEventArgs.StatusOutMsg = string.Format("Received {0} bytes", bytesReceived);
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            break;
                        }
                        else
                        {
                            // connection timed out
                            throw new SocketException(10060);
                        }
                    }
                    catch (SocketException ex)
                    {
                        // Raised multiple events to keep message size down
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sysSocketEventArgs.StatusOutMsg = "Socket exception occurred.";
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sysSocketEventArgs.StatusOutMsg = string.Format("Code {0}", ex.ErrorCode);
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
                            sysSocketEventArgs.StatusOutMsg = "Msg " + ex.Message;
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        attempt += 1;

                        // If we've reached the retry limit, throw the Exception
                        if (attempt < this.RecvTotalRetries)
                        {
                            continue;
                        }
                        else
                        {
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sysSocketEventArgs.StatusOutMsg = "Total receive attempts exceeded.";
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            socket.Disconnect(true);
                            socket.Close();
                            throw ex;
                        }
                    }
                }

                // Resize to what we received
                var bytesActuallyReceived = new byte[bytesReceived];
                Array.Copy(bytes, 0, bytesActuallyReceived, 0, bytesReceived);
                page.AddRange(bytesActuallyReceived);

                // Check for EOM
                var batch = Encoding.UTF8.GetString(bytesActuallyReceived, 0, bytesReceived);

                // If we have an EOM specified, check it
                if (!string.IsNullOrEmpty(this.ReceiveEOM))
                {
                    if (batch.EndsWith(this.ReceiveEOM))
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sysSocketEventArgs.StatusOutMsg = "EOM received.";
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        break;
                    }
                }
            }
            while (bytesReceived > 0);

            // Remove the EOM
            var eomBytData = System.Text.Encoding.UTF8.GetBytes(this.ReceiveEOM);

            // Note: The line below doesn't work
            // Dim eomIndex As int = Array.IndexOf(bytes.ToArray(), eomBytData.ToArray(), 0, bytes.Count)
            var eomIndex = page.LastIndexOf(eomBytData[eomBytData.Length - 1]);

            if (eomIndex >= 0)
            {
                // Determine start of EOM so we can trim it off
                var eomStart = eomIndex - eomBytData.Length;

                // Remove the EOM
                page.RemoveRange(eomStart + 1, eomBytData.Length);
            }

            return page;
        }

        /// <summary>
        /// The RegSockListenerStatusOut.
        /// </summary>
        /// <param name="sysSocketEventListener">The sysSocketEventListener<see cref="ISysSocketEventListener"/>.</param>
        public void RegSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut += new OnSysSocketStatusOutEventHandler(sysSocketEventListener.OnSysSocketStatusOut);
        }

        /// <summary>
        /// The UnregSockListenerStatusOut.
        /// </summary>
        /// <param name="sysSocketEventListener">The sysSocketEventListener<see cref="ISysSocketEventListener"/>.</param>
        public void UnregSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut -= new OnSysSocketStatusOutEventHandler(sysSocketEventListener.OnSysSocketStatusOut);
        }
    }
}
