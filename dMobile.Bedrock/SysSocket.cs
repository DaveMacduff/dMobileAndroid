namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Handles Communication via a socket to send and receive data
    /// </summary>
    /// <remarks></remarks>
    public class SysSocket : ISysSocketEventPublisher
    {
        #region Fields

        private string serverIP;
        private string sendEOM = "</WAIT_EOM>" + (char)20;
        private string receiveEOM = "</EOM>" + (char)20;
        private int recvBuffSize = 32768;
        private int connTotalRetries = 3;
        private int sendTotalRetries = 3;
        private int recvTotalRetries = 3;
        private int defPort = 10000;
        private int basePort = 10000;
        private int maxPorts = 1;
        private int sendTimeOutMilliseconds = 4101;
        private int recvTimeOutMilliseconds = 4102;
        private SysEnvironment sysEnv = SysEnvironment.GetInstance();

        #endregion

        #region Delegates

        private delegate void OnSysSocketStatusOutEventHandler(
            ISysSocketEventPublisher sysSocketEventPublisher,
            SysSocketEventArgs sysSocketEventArgs);

        #endregion

        #region Events

        private event OnSysSocketStatusOutEventHandler OnSysSocketStatusOut;

        #endregion

        #region Properties

        public string ServerIP
        {
            get
            {
                return this.serverIP;
            }

            set
            {
                this.serverIP = value;
            }
        }

        public string SendEOM
        {
            get
            {
                return this.sendEOM;
            }

            set
            {
                this.sendEOM = value;
            }
        }

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

        public int RecvBuffSize
        {
            get
            {
                return this.recvBuffSize;
            }

            set
            {
                this.recvBuffSize = value;
            }
        }

        public int ConnTotalRetries
        {
            get
            {
                return this.connTotalRetries;
            }

            set
            {
                this.connTotalRetries = value;
            }
        }

        public int SendTotalRetries
        {
            get
            {
                return this.sendTotalRetries;
            }

            set
            {
                this.sendTotalRetries = value;
            }
        }

        public int RecvTotalRetries
        {
            get
            {
                return this.recvTotalRetries;
            }

            set
            {
                this.recvTotalRetries = value;
            }
        }

        public int DefSocketPort
        {
            get
            {
                return this.defPort;
            }

            set
            {
                this.defPort = value;
            }
        }

        public int BasePort
        {
            get
            {
                return this.basePort;
            }

            set
            {
                this.basePort = value;
            }
        }

        public int MaxPorts
        {
            get
            {
                return this.maxPorts;
            }

            set
            {
                this.maxPorts = value;
            }
        }

        public int SendTimeOutMilliseconds
        {
            get
            {
                return this.sendTimeOutMilliseconds;
            }

            set
            {
                this.sendTimeOutMilliseconds = value;
            }
        }

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

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Create and return a connected socket object
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Socket Connect()
        {
            Socket socket = null;
            int port = this.DefSocketPort;
            Exception exConn = null;
            SysSocketEventArgs sockEventArgs = new SysSocketEventArgs();

            for (int attempt = 1; attempt <= this.ConnTotalRetries; attempt++)
            {
                try
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.IpAddress), this.ServerIP);
#else
                        sockEventArgs.StatusOutMsg = string.Format("IP address {0}", this.ServerIP);
#endif
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.ServerIP), port);
                    Socket tempSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    tempSocket.Blocking = true;

                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptPort), attempt, port);
#else
                        sockEventArgs.StatusOutMsg = string.Format("Connection attempt {0} port {1}.", attempt, port);
#endif
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    tempSocket.Connect(endPoint);

                    if (tempSocket.Connected)
                    {
                        socket = tempSocket;
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionSuccessful);
#else
                            sockEventArgs.StatusOutMsg = "Connection successful.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }
                    }

                    if (socket == null)
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptFailedOnPort), port);
#else
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt failed on port {0}.", port);
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
                        // RaiseEvent OnSysSocketStatusOut(Me, EventArgs.Empty, "Settting Send Timeout")
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.ReceiveTimeout, 
                        // SysEnv.SysSocketInstance.iReceiveTimeOutMilliseconds)
                        // RaiseEvent OnSysSocketStatusOut(Me, EventArgs.Empty, "Settting Send Timeout")
                        break;
                    }
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (ex.ErrorCode == 10060)
                    {
                        exConn = ex;
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptTimedOut);
#else
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptRefused);
#else
                            sockEventArgs.StatusOutMsg = "Connection attempt refused.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SocketExceptionOccurred);
#else
                            sockEventArgs.StatusOutMsg = "Socket exception occurred.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Codezzz), ex.ErrorCode);
#else
                            sockEventArgs.StatusOutMsg = string.Format("Code {0}.", ex.ErrorCode);
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Msg) + ex.Message;
#else
                            sockEventArgs.StatusOutMsg = "Msg " + ex.Message;
#endif
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
        /// Create and return a connected socket object
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Socket ConnectWithHostEntry()
        {
            Socket socket = null;
            IPHostEntry ipHostEntry = null;
            SocketException exConn = null;
            SysSocketEventArgs sockEventArgs = new SysSocketEventArgs();

            // Get host related information.
            ipHostEntry = System.Net.Dns.GetHostEntry(this.ServerIP);

            if (this.OnSysSocketStatusOut != null)
            {
#if MULTILANGUAGE
        sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.GettingIpHostEntry);
#else
                sockEventArgs.StatusOutMsg = "Getting IP host entry";
#endif
                this.OnSysSocketStatusOut(this, sockEventArgs);
            }

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host host IP Address is not compatible with the 
            // address family (typical in the IPv6 case).
            IPAddress ipAddress = null;
            int port = this.DefSocketPort;

            for (int attempt = 1; attempt <= this.ConnTotalRetries; attempt++)
            {
                try
                {
                    foreach (IPAddress ipAddressLoopVariable in ipHostEntry.AddressList)
                    {
                        ipAddress = ipAddressLoopVariable;
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.IpAddressz), ipAddress.ToString());
#else
                            sockEventArgs.StatusOutMsg = string.Format("IP address {0} ", ipAddress.ToString());
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                        Socket tempSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        tempSocket.Blocking = true;

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptPort), attempt, port);
#else
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt {0} port {1}.", attempt, port);
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        tempSocket.Connect(endPoint);

                        if (tempSocket.Connected)
                        {
                            socket = tempSocket;
                            if (this.OnSysSocketStatusOut != null)
                            {
#if MULTILANGUAGE
                sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionSuccessful);
#else
                                sockEventArgs.StatusOutMsg = "Connection successful.";
#endif
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }
                        }
                    }

                    if (socket == null)
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptFailedOnPort), port);
#else
                            sockEventArgs.StatusOutMsg = string.Format("Connection attempt failed on port {0}.", port);
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
                        // RaiseEvent StatusOut(Me, EventArgs.Empty, "Settting Send Timeout")
                        // socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.ReceiveTimeout, 
                        // SysEnv.SysSocketInstance.iReceiveTimeOutMilliseconds)
                        // RaiseEvent StatusOut(Me, EventArgs.Empty, "Settting Send Timeout")
                        break;
                    }
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (ex.ErrorCode == 10060)
                    {
                        exConn = ex;
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptTimedOut);
#else
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.ConnectionAttemptRefused);
#else
                            sockEventArgs.StatusOutMsg = "Connection attempt refused.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        port = port + 1;
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
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SocketExceptionOccurred);
#else
                            sockEventArgs.StatusOutMsg = "Socket exception occurred.";
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Codezzz), ex.ErrorCode);
#else
                            sockEventArgs.StatusOutMsg = string.Format("Code {0}.", ex.ErrorCode);
#endif
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sockEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Msg) + ex.Message;
#else
                            sockEventArgs.StatusOutMsg = "Msg " + ex.Message;
#endif
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
        /// Send data out via a socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="request"></param>
        /// <returns>Total number of bytes successfully sent</returns>
        /// <remarks></remarks>
        public int SendRequest(Socket socket, string request)
        {
            int attempt = 1;
            Encoding unicode = Encoding.UTF8;
            byte[] totalBytes2Send = unicode.GetBytes(request + this.SendEOM);

            // Add the EOF id
            int bytesRemaining2BeSent = totalBytes2Send.Length;
            int byteStartIndex = 0;
            int totalBytesSent = 0;
            int bytesSent = 0;
            SysSocketEventArgs sysSocketEventArgs = new SysSocketEventArgs();

            if (this.OnSysSocketStatusOut != null)
            {
#if MULTILANGUAGE
        sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SendingRequestz);
#else
                sysSocketEventArgs.StatusOutMsg = "Sending request.";
#endif
                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
            }

            if (this.OnSysSocketStatusOut != null)
            {
#if MULTILANGUAGE
        sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.BytesToSend), totalBytes2Send.Length);
#else
                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes to send {0} ", totalBytes2Send.Length);
#endif
                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
            }

            // Send request to the server.
            while (bytesRemaining2BeSent > 0 & attempt <= this.SendTotalRetries)
            {
                try
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SendAttemptOf), attempt, this.SendTotalRetries);
#else
                        sysSocketEventArgs.StatusOutMsg = string.Format("Send attempt {0} of {1} ", attempt, this.SendTotalRetries);
#endif
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    while (bytesRemaining2BeSent > 0)
                    {
                        if (socket.Poll((this.SysEnv.SysSock.SendTimeOutMilliseconds * 1000) + 1, SelectMode.SelectWrite))
                        {
                            byte[] byteBuffer2Send = new byte[bytesRemaining2BeSent + 1];

                            if (this.OnSysSocketStatusOut != null)
                            {
#if MULTILANGUAGE
                sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SendingBytes), bytesRemaining2BeSent);
#else
                                sysSocketEventArgs.StatusOutMsg = string.Format("Sending bytes {0} ", bytesRemaining2BeSent);
#endif
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            Array.Copy(totalBytes2Send, byteStartIndex, byteBuffer2Send, 0, bytesRemaining2BeSent);

                            bytesSent = socket.Send(byteBuffer2Send, byteBuffer2Send.Length, 0);

                            if (this.OnSysSocketStatusOut != null)
                            {
#if MULTILANGUAGE
                sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.BytesSentz), bytesSent);
#else
                                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes sent {0} ", bytesSent);
#endif
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            byteStartIndex = byteStartIndex + bytesSent;

                            bytesRemaining2BeSent = bytesRemaining2BeSent - bytesSent;
                            if (this.OnSysSocketStatusOut != null)
                            {
#if MULTILANGUAGE
                sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.BytesRemaining),  bytesRemaining2BeSent);
#else
                                sysSocketEventArgs.StatusOutMsg = string.Format("Bytes remaining {0} ", bytesRemaining2BeSent);
#endif
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
#if MULTILANGUAGE
            sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.RequestSent);
#else
                        sysSocketEventArgs.StatusOutMsg = "Request sent.";
#endif
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }
                }
                catch (SocketException ex)
                {
                    // Raised multiple events to keep message size down
                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SocketExceptionOccurred);
#else
                        sysSocketEventArgs.StatusOutMsg = "Socket exception occurred.";
#endif
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Codezzzz), ex.ErrorCode);
#else
                        sysSocketEventArgs.StatusOutMsg = string.Format("Code {0}", ex.ErrorCode);
#endif
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    if (this.OnSysSocketStatusOut != null)
                    {
#if MULTILANGUAGE
            sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Msg) + ex.Message;
#else
                        sysSocketEventArgs.StatusOutMsg = "Msg " + ex.Message;
#endif
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
#if MULTILANGUAGE
              sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.TotalSendAttemptsExceeded);
#else
                            sysSocketEventArgs.StatusOutMsg = "Total send attempts exceeded.";
#endif
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        socket.Close();
                        throw ex;
                    }
                }
            }

            return totalBytesSent;
        }

        /// <summary>
        /// Read data from a socket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns>A list of bytes from the socket</returns>
        /// <remarks></remarks>
        public List<byte> ReceiveResponse(Socket socket)
        {
            int bytesReceived = default(int);
            int attempt = 1;
            byte[] bytes = new byte[this.RecvBuffSize + 1];

            // Read the bytes.
            List<byte> page = new List<byte>();
            SysSocketEventArgs sysSocketEventArgs = new SysSocketEventArgs();

            do
            {
                // We will try the Receive, the RetryLimit times
                while (attempt <= this.RecvTotalRetries)
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sysSocketEventArgs.StatusOutMsg = string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.ReceiveAttemptOf), 
#else
"Receive attempt {0} of {1} ",
#endif
 attempt,
                            this.RecvTotalRetries);
                        this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                    }

                    try
                    {
                        if (socket.Poll((this.SysEnv.SysSock.ReceiveTimeOutMilliseconds * 1000) + 1, SelectMode.SelectRead))
                        {
                            bytesReceived = socket.Receive(bytes, bytes.Length, SocketFlags.None);

                            if (this.OnSysSocketStatusOut != null)
                            {
#if MULTILANGUAGE
                sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReceivedBytes), bytesReceived);
#else
                                sysSocketEventArgs.StatusOutMsg = string.Format("Received {0} bytes", bytesReceived);
#endif
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
#if MULTILANGUAGE
              sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.SocketExceptionOccurred);
#else
                            sysSocketEventArgs.StatusOutMsg = "Socket exception occurred.";
#endif
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sysSocketEventArgs.StatusOutMsg = string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Codezzzz), ex.ErrorCode);
#else
                            sysSocketEventArgs.StatusOutMsg = string.Format("Code {0}", ex.ErrorCode);
#endif
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Msg) + ex.Message;
#else
                            sysSocketEventArgs.StatusOutMsg = "Msg " + ex.Message;
#endif
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
#if MULTILANGUAGE
                sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.TotalReceiveAttemptsExceeded);
#else
                                sysSocketEventArgs.StatusOutMsg = "Total receive attempts exceeded.";
#endif
                                this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                            }

                            socket.Close();
                            throw ex;
                        }
                    }
                }

                // Resize to what we received
                byte[] bytesActuallyReceived = new byte[bytesReceived];
                Array.Copy(bytes, 0, bytesActuallyReceived, 0, bytesReceived);
                page.AddRange(bytesActuallyReceived);

                // Check for EOM
                string batch = Encoding.UTF8.GetString(bytesActuallyReceived, 0, bytesReceived);

                // If we have an EOM specified, check it
                if (!string.IsNullOrEmpty(this.ReceiveEOM))
                {
                    if (batch.EndsWith(this.ReceiveEOM))
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
#if MULTILANGUAGE
              sysSocketEventArgs.StatusOutMsg = Translator.GetInstance().Translate(TranslatorVocabulary.EomReceived);
#else
                            sysSocketEventArgs.StatusOutMsg = "EOM received.";
#endif
                            this.OnSysSocketStatusOut(this, sysSocketEventArgs);
                        }

                        break;
                    }
                }
            }
            while (bytesReceived > 0);

            // Remove the EOM
            byte[] eomBytData = System.Text.Encoding.UTF8.GetBytes(this.ReceiveEOM);

            // Note: The line below doesn't work
            // Dim eomIndex As int = Array.IndexOf(bytes.ToArray(), eomBytData.ToArray(), 0, bytes.Count)
            int eomIndex = page.LastIndexOf(eomBytData[eomBytData.Length - 1]);

            if (eomIndex >= 0)
            {
                // Determine start of EOM so we can trim it off
                int eomStart = eomIndex - eomBytData.Length;

                // Remove the EOM
                page.RemoveRange(eomStart + 1, eomBytData.Length);
            }

            return page;
        }

        #region ISysSocketEventPublisher Members

        public void RegSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut += new OnSysSocketStatusOutEventHandler(sysSocketEventListener.OnSysSocketStatusOut);
        }

        public void UnregSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut -= new OnSysSocketStatusOutEventHandler(sysSocketEventListener.OnSysSocketStatusOut);
        }

        #endregion

        #endregion
    }
}
