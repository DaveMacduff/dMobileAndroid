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
    using System.Threading;
    using System.Threading.Tasks;
    using Exception = System.Exception;

    /// <summary>
    ///     Handles Communication via a socket to send and receive data.
    /// </summary>
    public class SysSocket : ISysSocketEventPublisher
    {
        /// <summary>
        ///     The OnSysSocketStatusOutEventHandler.
        /// </summary>
        /// <param name="sysSocketEventPublisher">The sysSocketEventPublisher<see cref="ISysSocketEventPublisher" />.</param>
        /// <param name="sysSocketEventArgs">The sysSocketEventArgs<see cref="SysSocketEventArgs" />.</param>
        private delegate void SysSocketStatusOutEventHandler(
            ISysSocketEventPublisher sysSocketEventPublisher,
            SysSocketEventArgs sysSocketEventArgs);

        /// <summary>
        ///     Defines the OnSysSocketStatusOut.
        /// </summary>
        private event SysSocketStatusOutEventHandler OnSysSocketStatusOut;

        /// <summary>
        ///     Gets or sets the ServerIP.
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        ///     Gets or sets the ReceiveBuffSize.
        /// </summary>
        public int ReceiveBuffSize { get; set; } = 32768;

        /// <summary>
        ///     Gets or sets the ConnTotalRetries.
        /// </summary>
        public int ConnTotalRetries { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the SendTotalRetries.
        /// </summary>
        public int SendTotalRetries { get; set; } = 3;

        /// <summary>
        ///     Gets or sets the ReceiveTotalRetries.
        /// </summary>
        public int ReceiveTotalRetries { get; set; } = 3;

        /// <summary>
        ///     Gets or sets the DefSocketPort.
        /// </summary>
        public int DefSocketPort { get; set; } = 10000;

        /// <summary>
        ///     Gets or sets the BasePort.
        /// </summary>
        public int BasePort { get; set; } = 10000;

        /// <summary>
        ///     Gets or sets the MaxPorts.
        /// </summary>
        public int MaxPorts { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the SendTimeOutMilliseconds.
        /// </summary>
        public int SendTimeOutMilliseconds { get; set; } = 4101;

        /// <summary>
        ///     Gets or sets the ReceiveTimeOutMilliseconds.
        /// </summary>
        public int ReceiveTimeOutMilliseconds { get; set; } = 4102;

        /// <summary>
        ///     Gets the system environment.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets the SendEOM.
        /// </summary>
        private string SendEom { get; set; } = "</WAIT_EOM>" + (char)20;

        /// <summary>
        ///     Gets or sets the ReceiveEOM.
        /// </summary>
        private string ReceiveEom { get; set; } = "</EOM>" + (char)20;

        /// <summary>
        ///     The UnregSockListenerStatusOut.
        /// </summary>
        /// <param name="sysSocketEventListener">The sysSocketEventListener<see cref="ISysSocketEventListener" />.</param>
        public void UnregSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut -= sysSocketEventListener.OnSysSocketStatusOut;
        }

        /// <summary>
        ///     The RegSockListenerStatusOut.
        /// </summary>
        /// <param name="sysSocketEventListener">The sysSocketEventListener<see cref="ISysSocketEventListener" />.</param>
        public void RegSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut += sysSocketEventListener.OnSysSocketStatusOut;
        }

        /// <summary>
        ///     Disconnect the socket.
        /// </summary>
        /// <param name="socket">The socket<see cref="Socket" />.</param>
        /// <remarks>
        /// Most of the socket errors are currently caught and rethrown.
        /// Handled errors close the socket, shutdown is not invoked.
        /// And then rethrow the error.
        /// Non-blocking sockets are not currently supported by dMobile.
        /// </remarks>
        public void Disconnect(Socket socket)
        {
            _ = socket ?? throw new ArgumentNullException(nameof(socket));
            const int delayBetweenAttempts = 1000;
            var sockEventArgs = new SysSocketEventArgs();
            for (var attempt = 1; attempt <= this.ConnTotalRetries; attempt++)
            {
                try
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sockEventArgs.StatusOutMsg = "Shutting down socket.";
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                }
                catch (SocketException sockEx)
                {
                    switch ((SocketError)sockEx.ErrorCode)
                    {
                        case SocketError.AccessDenied:
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // An attempt was made to access a Socket in a way that is
                            // forbidden by its access permissions.
                            // Action: Not possible to close the socket.
                            throw;

                        case SocketError.AddressAlreadyInUse:
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address already in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Only one use of an address is normally permitted.
                            // Action: Not possible to close the socket.
                            throw;

                        case SocketError.AddressFamilyNotSupported:
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // The address family specified is not supported. This error is
                            // returned if the IPv6 address family was specified and the IPv6
                            // stack is not installed on the local machine. This error is
                            // returned if the IPv4 address family was specified and the
                            // IPv4 stack is not installed on the local machine.
                            // Action: Not possible to close the socket.
                            throw;

                        case SocketError.AddressNotAvailable:
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address not available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // The selected IP address is not valid in this context.
                            // Action: Not possible to close the socket.
                            throw;

                        case SocketError.AlreadyInProgress:
                            // The nonblocking socket already has an operation in progress.
                            // Action: This error only occurs when using a non-blocking socket.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket operation already in progress.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Non-blocking sockets are not supported by dMobile.
                            // Attempt to close again after a delay just because.
                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.ConnectionAborted:
                            // The connection was aborted by the .NET Framework or the
                            // underlying socket provider.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection aborted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.ConnectionRefused:
                            // The remote host is actively refusing a connection.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection refused.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.ConnectionReset:
                            // The connection was reset by the remote peer.
                            // Action: Continue to clean up the socket.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection reset by remote peer.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.DestinationAddressRequired:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket destination address required.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Disconnecting:
                            // A graceful shutdown is in progress.
                            // There as a current connection and the socket is being closed.
                            // Action: Sleep and then shutdown and close the socket.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection shutting down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.Fault:
                            // An invalid pointer address was detected by the underlying socket
                            // provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid pointer address.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostDown:
                            // The operation failed because the remote host is down.
                            // Action: Wait, and then try again.
                            // This error is returned if the remote host is down, or if the
                            // remote host is unreachable.
                            // Changing the port will not help.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket remote host is down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.HostNotFound:
                            // No such host is known. The name is not an official host name or
                            // alias.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket host is unknown.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.HostUnreachable:
                            // There is no network route to the specified host.
                            // Action: Wait, and then try again.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.InProgress:
                            // A blocking operation is currently executing.
                            // Socket is currently in use. An asynchronous operation
                            // to connect again is occurring.
                            // Leave the port alone and try a different port.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.Interrupted:
                            // A blocking operation was canceled.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket blocking operation canceled.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.InvalidArgument:
                            // An invalid argument was supplied to a Socket member.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid member argument.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.IOPending:
                            // The application has initiated an overlapped operation that
                            // cannot be completed immediately.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection overlapped operation.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.IsConnected:
                            // The Socket is connected.
                            if (!socket.Connected)
                            {
                                // Expected the socket connected flag to be true.
                                // Action: Quit and throw exception.
                                if (this.OnSysSocketStatusOut != null)
                                {
                                    sockEventArgs.StatusOutMsg = "Connection state mismatch.";
                                    this.OnSysSocketStatusOut(this, sockEventArgs);
                                }
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.MessageSize:
                            // The datagram is too long.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket datagram is too big.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkDown:
                            // The network is down.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.NetworkReset:
                            // The connection has been broken due to keep-alive activity
                            // that has already timed out.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection reset.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.NetworkUnreachable:
                            // No route to the remote host exists.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket network unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoBufferSpaceAvailable:
                            // No free buffer space is available for a Socket operation.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket no buffer space available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoData:
                            // The requested name or IP address was not found on the name server.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket IP address not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoRecovery:
                            // The error is unrecoverable or the requested database cannot
                            // be located.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket unrecoverable error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotConnected:
                            // The application tried to send or receive data, and the Socket
                            // is not connected.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not connected.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            socket.Close();
                            break;

                        case SocketError.NotInitialized:
                            // The underlying socket provider has not been initialized.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not initialized.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotSocket:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address omitted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.OperationAborted:
                            // The overlapped operation was aborted due to the closure of the
                            // Socket.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation reported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            // Socket is already closed. Continue to clean up the socket.
                            socket.Close();
                            break;

                        case SocketError.OperationNotSupported:
                            // The address family is not supported by the protocol family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProcessLimit:
                            // Too many processes are using the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket process limit.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolFamilyNotSupported:
                            // The protocol family is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket protocol family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolNotSupported:
                            // The protocol is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolOption:
                            // An unknown, invalid, or unsupported option or level was used
                            // with a socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid protocol option.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolType:
                            // The protocol type is incorrect for this Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol type incorrect.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Shutdown:
                            // A request to send or receive data was disallowed because the
                            // Socket has already been closed.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket already closed.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            break;

                        case SocketError.SocketError:
                            // An unspecified Socket error has occurred.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection socket error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.SocketNotSupported:
                            // The support for the specified socket type does not exist in this
                            // address family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Success:
                            // The Socket operation succeeded.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation succeeded.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            break;

                        case SocketError.SystemNotReady:
                            // The network subsystem is unavailable.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket system not ready.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TimedOut:
                            // The connection attempt timed out, or the connected host has
                            // failed to respond.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.TooManyOpenSockets:
                            // There are too many open sockets in the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Too many open sockets.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TryAgain:
                            // The name of the host could not be resolved. Try again later.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unresolved.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        case SocketError.TypeNotFound:
                            // The specified class was not found.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.VersionNotSupported:
                            // The version of the underlying socket provider is out of range.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket provider version out of range.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.WouldBlock:
                            // The nonblocking Socket already has an operation in progress.
                            // This error should not occur because we are not using a
                            // non-blocking socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket would block.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            Thread.Sleep(delayBetweenAttempts);
                            break;

                        default:
                            var msg = $"Unhandled socket error.{Environment.NewLine}" +
                                      $"Code: {sockEx.ErrorCode + " " + sockEx.Message}";
                            throw new Exception(msg);
                    }
                }
                finally
                {
                    socket.Close();
                }

                if (!socket.Connected)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Create and return a connection socket object.
        /// </summary>
        /// <param name="port"><see cref="int" /> Socket port to use.</param>
        /// <param name="timeout"><see cref="TimeSpan" /> Timeout period.</param>
        /// <returns>Connected socket instance.</returns>
        public async Task<SysSocketInfo> Connect(int port, TimeSpan timeout)
        {
            port = (port == 0) ? this.DefSocketPort : port;
            const int defaultSecondsTimeout = 10;
            timeout = (timeout == TimeSpan.Zero)
                ? new TimeSpan(0, 0, defaultSecondsTimeout)
                : timeout;
            Socket socket = null;
            const int delayBetweenAttempts = 1000;
            var sockInfo = new SysSocketInfo();
            var sockEventArgs = new SysSocketEventArgs();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            try
            {
                // Create a Task to connect
                var endPoint = new IPEndPoint(IPAddress.Parse(this.ServerIp), port);
                socket = new Socket(
                    endPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                sockInfo.Socket = socket;

                var connectTask = socket.ConnectAsync(endPoint);

                // Create a Task that completes when the timeout period elapses
                var timeoutTask = Task.Delay(timeout, cancellationTokenSource.Token);

                // Wait for either the connection to complete or the timeout
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                // If the completed task is the timeout task, throw a TimeoutException
                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException("Connection attempt timed out.");
                }

                // At this point, the connection has succeeded before the timeout.
                // Proceed with using the socket...
            }
            catch (TimeoutException timeEx)
            {
                if (this.OnSysSocketStatusOut != null)
                {
                    sockEventArgs.StatusOutMsg = timeEx.Message;
                    this.OnSysSocketStatusOut(this, sockEventArgs);
                }
            }
            catch (SocketException sockEx)
            {
                switch ((SocketError)sockEx.ErrorCode)
                {
                    case SocketError.AccessDenied:
                        // An attempt was made to access a Socket in a way that is
                        // forbidden by its access permissions.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket access denied.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressAlreadyInUse:
                        // Only one use of an address is normally permitted.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address already in use.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressFamilyNotSupported:
                        // The address family specified is not supported. This error is
                        // returned if the IPv6 address family was specified and the IPv6
                        // stack is not installed on the local machine. This error is
                        // returned if the IPv4 address family was specified and the
                        // IPv4 stack is not installed on the local machine.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address family not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressNotAvailable:
                        // The selected IP address is not valid in this context.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address not available.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AlreadyInProgress:
                        // The nonblocking socket already has an operation in progress.
                        // Action: This error only occurs when using a non-blocking socket.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket operation already in progress.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ConnectionAborted:
                        // The connection was aborted by the .NET Framework or the
                        // underlying socket provider.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket connection aborted.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ConnectionRefused:
                        // The remote host is actively refusing a connection.
                        // Action: Increment port and try again. We could try the same port
                        // again, but it is likely that the same error will occur.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection refused.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        sockInfo.TryNewPort = true;
                        break;

                    case SocketError.ConnectionReset:
                        // The connection was reset by the remote peer.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection reset by remote peer.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.DestinationAddressRequired:
                        // A required address was omitted from an operation on a Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket destination address required.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Disconnecting:
                        // A graceful shutdown is in progress.
                        // There as a current connection and the socket is being closed.
                        // Action: Wait, and then try again with the same port.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection shutting down.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        sockInfo.TryNewPort = true;
                        break;

                    case SocketError.Fault:
                        // An invalid pointer address was detected by the underlying socket
                        // provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid pointer address.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.HostDown:
                        // The operation failed because the remote host is down.
                        // Action: Wait, and then try again.
                        // This error is returned if the remote host is down, or if the
                        // remote host is unreachable.
                        // Changing the port will not help.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket remote host is down.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.HostNotFound:
                        // No such host is known. The name is not an official host name or
                        // alias.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket host is unknown.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.HostUnreachable:
                        // There is no network route to the specified host.
                        // Action: Wait, and then try again.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection host unreachable.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.InProgress:
                        // A blocking operation is currently executing.
                        // Socket is currently in use. An asynchronous operation
                        // to connect again is occurring.
                        // Leave the port alone and try a different port.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection in use.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        sockInfo.TryNewPort = true;
                        break;

                    case SocketError.Interrupted:
                        // A blocking operation was canceled.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket blocking operation canceled.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.InvalidArgument:
                        // An invalid argument was supplied to a Socket member.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid member argument.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.IOPending:
                        // The application has initiated an overlapped operation that
                        // cannot be completed immediately.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection overlapped operation.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.IsConnected:
                        // The Socket is already connected.
                        if (!socket!.Connected)
                        {
                            // Expected the socket connected flag to be true.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection state mismatch.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;
                        }

                        break;

                    case SocketError.MessageSize:
                        // The datagram is too long.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket datagram is too big.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkDown:
                        // The network is down.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket access denied.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkReset:
                        // The connection has been broken due to keep-alive activity
                        // that has already timed out.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket connection reset.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkUnreachable:
                        // No route to the remote host exists.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket network unreachable.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoBufferSpaceAvailable:
                        // No free buffer space is available for a Socket operation.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket no buffer space available.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoData:
                        // The requested name or IP address was not found on the name server.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket IP address not found.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoRecovery:
                        // The error is unrecoverable or the requested database cannot
                        // be located.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket unrecoverable error.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotConnected:
                        // The application tried to send or receive data, and the Socket
                        // is not connected.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket not connected.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotInitialized:
                        // The underlying socket provider has not been initialized.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket not initialized.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotSocket:
                        // A required address was omitted from an operation on a Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address omitted.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.OperationAborted:
                        // The overlapped operation was aborted due to the closure of the
                        // Socket.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation reported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.OperationNotSupported:
                        // The address family is not supported by the protocol family.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProcessLimit:
                        // Too many processes are using the underlying socket provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket process limit.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolFamilyNotSupported:
                        // The protocol family is not implemented or has not been configured.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket protocol family not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolNotSupported:
                        // The protocol is not implemented or has not been configured.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket protocol not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolOption:
                        // An unknown, invalid, or unsupported option or level was used
                        // with a socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid protocol option.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolType:
                        // The protocol type is incorrect for this Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket protocol type incorrect.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Shutdown:
                        // A request to send or receive data was disallowed because the
                        // Socket has already been closed.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket already closed.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.SocketError:
                        // An unspecified Socket error has occurred.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection socket error.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.SocketNotSupported:
                        // The support for the specified socket type does not exist in this
                        // address family.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket type not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Success:
                        // The Socket operation succeeded.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation succeeded.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        break;

                    case SocketError.SystemNotReady:
                        // The network subsystem is unavailable.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket system not ready.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.TimedOut:
                        // The connection attempt timed out, or the connected host has
                        // failed to respond.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        ////Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.TooManyOpenSockets:
                        // There are too many open sockets in the underlying socket provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Too many open sockets.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.TryAgain:
                        // The name of the host could not be resolved. Try again later.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection host unresolved.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.TypeNotFound:
                        // The specified class was not found.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket type not found.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.VersionNotSupported:
                        // The version of the underlying socket provider is out of range.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket provider version out of range.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.WouldBlock:
                        // The nonblocking Socket already has an operation in progress.
                        // This error should not occur because we are not using a
                        // non-blocking socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket would block.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    default:
                        var msg = $"Unhandled socket error.{Environment.NewLine}" +
                                  $"Code: {sockEx.ErrorCode + " " + sockEx.Message}";
                        throw new Exception(msg);
                }
            }

            return sockInfo;
        }

        /// <summary>
        ///     Create and return a connected socket object.
        /// </summary>
        /// <param name="socket">The socket<see cref="Socket" />.</param>
        public void Connect(ref Socket socket)
        {
            const int delayBetweenAttempts = 1000;
            var port = this.DefSocketPort;
            var tryNewPort = false;
            this.Connect(ref socket, port, ref tryNewPort);
            Thread.Sleep(delayBetweenAttempts);
        }

        /// <summary>
        ///     Create and return a connected socket object.
        /// </summary>
        /// <param name="socket">The socket<see cref="Socket" />.</param>
        /// <param name="port"><see cref="int" /> Socket port to use.</param>
        /// <param name="tryNewPort"><see cref="bool" /> Try a new port.</param>
        public void Connect(ref Socket socket, int port, ref bool tryNewPort)
        {
            const int delayBetweenAttempts = 1000;
            var sockEventArgs = new SysSocketEventArgs();
            if (this.OnSysSocketStatusOut != null)
            {
                sockEventArgs.StatusOutMsg = $"IP address {this.ServerIp}";
                this.OnSysSocketStatusOut(this, sockEventArgs);
            }

            if (this.OnSysSocketStatusOut != null)
            {
                sockEventArgs.StatusOutMsg =
                    $"Delay between connection tries: {delayBetweenAttempts}";
                this.OnSysSocketStatusOut(this, sockEventArgs);
            }

            var endPoint = new IPEndPoint(IPAddress.Parse(this.ServerIp), port);
            socket = new Socket(
                endPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket = socket ?? throw new ArgumentNullException(nameof(socket));

            try
            {
                tryNewPort = false;
                socket.Connect(endPoint);
                if (socket.Connected)
                {
                    if (this.OnSysSocketStatusOut != null)
                    {
                        sockEventArgs.StatusOutMsg = "Connection successful.";
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }
                }
            }
            catch (SocketException sockEx)
            {
                switch ((SocketError)sockEx.ErrorCode)
                {
                    case SocketError.AccessDenied:
                        // An attempt was made to access a Socket in a way that is
                        // forbidden by its access permissions.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket access denied.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressAlreadyInUse:
                        // Only one use of an address is normally permitted.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address already in use.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressFamilyNotSupported:
                        // The address family specified is not supported. This error is
                        // returned if the IPv6 address family was specified and the IPv6
                        // stack is not installed on the local machine. This error is
                        // returned if the IPv4 address family was specified and the
                        // IPv4 stack is not installed on the local machine.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address family not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AddressNotAvailable:
                        // The selected IP address is not valid in this context.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address not available.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.AlreadyInProgress:
                        // The nonblocking socket already has an operation in progress.
                        // Action: This error only occurs when using a non-blocking socket.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket operation already in progress.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ConnectionAborted:
                        // The connection was aborted by the .NET Framework or the
                        // underlying socket provider.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket connection aborted.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ConnectionRefused:
                        // The remote host is actively refusing a connection.
                        // Action: Increment port and try again. We could try the same port
                        // again, but it is likely that the same error will occur.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection refused.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        tryNewPort = true;
                        break;

                    case SocketError.ConnectionReset:
                        // The connection was reset by the remote peer.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection reset by remote peer.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.DestinationAddressRequired:
                        // A required address was omitted from an operation on a Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket destination address required.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Disconnecting:
                        // A graceful shutdown is in progress.
                        // There as a current connection and the socket is being closed.
                        // Action: Wait, and then try again with the same port.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection shutting down.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        tryNewPort = true;
                        break;

                    case SocketError.Fault:
                        // An invalid pointer address was detected by the underlying socket
                        // provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid pointer address.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.HostDown:
                        // The operation failed because the remote host is down.
                        // Action: Wait, and then try again.
                        // This error is returned if the remote host is down, or if the
                        // remote host is unreachable.
                        // Changing the port will not help.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket remote host is down.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.HostNotFound:
                        // No such host is known. The name is not an official host name or
                        // alias.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket host is unknown.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.HostUnreachable:
                        // There is no network route to the specified host.
                        // Action: Wait, and then try again.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection host unreachable.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.InProgress:
                        // A blocking operation is currently executing.
                        // Socket is currently in use. An asynchronous operation
                        // to connect again is occurring.
                        // Leave the port alone and try a different port.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection in use.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        tryNewPort = true;
                        break;

                    case SocketError.Interrupted:
                        // A blocking operation was canceled.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket blocking operation canceled.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.InvalidArgument:
                        // An invalid argument was supplied to a Socket member.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid member argument.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.IOPending:
                        // The application has initiated an overlapped operation that
                        // cannot be completed immediately.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection overlapped operation.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.IsConnected:
                        // The Socket is already connected.
                        if (!socket.Connected)
                        {
                            // Expected the socket connected flag to be true.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection state mismatch.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;
                        }

                        break;

                    case SocketError.MessageSize:
                        // The datagram is too long.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket datagram is too big.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkDown:
                        // The network is down.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket access denied.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkReset:
                        // The connection has been broken due to keep-alive activity
                        // that has already timed out.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket connection reset.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NetworkUnreachable:
                        // No route to the remote host exists.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket network unreachable.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoBufferSpaceAvailable:
                        // No free buffer space is available for a Socket operation.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket no buffer space available.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoData:
                        // The requested name or IP address was not found on the name server.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket IP address not found.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NoRecovery:
                        // The error is unrecoverable or the requested database cannot
                        // be located.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket unrecoverable error.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotConnected:
                        // The application tried to send or receive data, and the Socket
                        // is not connected.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket not connected.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotInitialized:
                        // The underlying socket provider has not been initialized.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket not initialized.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.NotSocket:
                        // A required address was omitted from an operation on a Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket address omitted.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.OperationAborted:
                        // The overlapped operation was aborted due to the closure of the
                        // Socket.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation reported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.OperationNotSupported:
                        // The address family is not supported by the protocol family.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProcessLimit:
                        // Too many processes are using the underlying socket provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket process limit.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolFamilyNotSupported:
                        // The protocol family is not implemented or has not been configured.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket protocol family not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolNotSupported:
                        // The protocol is not implemented or has not been configured.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket protocol not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolOption:
                        // An unknown, invalid, or unsupported option or level was used
                        // with a socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket invalid protocol option.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.ProtocolType:
                        // The protocol type is incorrect for this Socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket protocol type incorrect.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Shutdown:
                        // A request to send or receive data was disallowed because the
                        // Socket has already been closed.
                        // This error should not occur when attempting a connection.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket already closed.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.SocketError:
                        // An unspecified Socket error has occurred.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection socket error.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.SocketNotSupported:
                        // The support for the specified socket type does not exist in this
                        // address family.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket type not supported.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.Success:
                        // The Socket operation succeeded.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket operation succeeded.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        break;

                    case SocketError.SystemNotReady:
                        // The network subsystem is unavailable.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket system not ready.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.TimedOut:
                        // The connection attempt timed out, or the connected host has
                        // failed to respond.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        ////Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.TooManyOpenSockets:
                        // There are too many open sockets in the underlying socket provider.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Too many open sockets.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.TryAgain:
                        // The name of the host could not be resolved. Try again later.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Connection host unresolved.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        Thread.Sleep(delayBetweenAttempts);
                        break;

                    case SocketError.TypeNotFound:
                        // The specified class was not found.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket type not found.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.VersionNotSupported:
                        // The version of the underlying socket provider is out of range.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg =
                                "Socket provider version out of range.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    case SocketError.WouldBlock:
                        // The nonblocking Socket already has an operation in progress.
                        // This error should not occur because we are not using a
                        // non-blocking socket.
                        // Action: Quit and throw exception.
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "Socket would block.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        throw;

                    default:
                        var msg = $"Unhandled socket error.{Environment.NewLine}" +
                                  $"Code: {sockEx.ErrorCode + " " + sockEx.Message}";
                        throw new Exception(msg);
                }
            }
        }

        /// <summary>
        ///     Send the request to the server.
        /// </summary>
        /// <param name="socket">Socket instance.</param>
        /// <param name="request">Request data to send.</param>
        /// <returns>Total number of bytes sent.</returns>
        /// <remarks>
        /// Socket errors are currently all caught and rethrown. This may change in the future.
        /// The only socket error not rethrown is SocketError.Success.
        /// Non-blocking sockets are not currently supported by dMobile.
        /// If a socket error occurs, the socket port should be incremented and the connection retried.
        /// </remarks>
        public int SendRequest(Socket socket, string request)
        {
            ////const int delayBetweenAttempts = 1000;
            var unicode = Encoding.UTF8;
            var totalBytes2Send = unicode.GetBytes(request + this.SendEom);
            var bytesRemaining2BeSent = totalBytes2Send.Length;
            var byteStartIndex = 0;
            var totalBytesSent = 0;
            var sockEventArgs = new SysSocketEventArgs();

            // Poll timeout is in microseconds. 1000000 microseconds = 1 second.
            var pollTimeout = this.SysEnv.SysSock.SendTimeOutMilliseconds <= 0
                ? this.SysEnv.SysSock.SendTimeOutMilliseconds * 1000
                : 1000000;

            while (bytesRemaining2BeSent > 0)
            {
                try
                {
                    if (socket.Poll(pollTimeout, SelectMode.SelectError))
                    {
                        // SelectMode.SelectError does not provide specific error details directly.
                        // Typically, need to attempt a socket operation to get the error details.
                        // Send an empty byte array to get the error details.
                        socket.Send(Array.Empty<byte>());
                    }
                    else if (!socket.Poll(pollTimeout, SelectMode.SelectWrite))
                    {
                        // Socket is not ready for writing.
                        // SelectMode.SelectWrite does not provide specific error details directly.
                        // Typically, need to attempt a socket operation to get the error details.
                        // Send an empty byte array to get the error details.
                        socket.Send(Array.Empty<byte>());
                    }

                    var byteBuffer2Send = new byte[bytesRemaining2BeSent + 1];
                    Array.Copy(
                        totalBytes2Send,
                        byteStartIndex,
                        byteBuffer2Send,
                        0,
                        bytesRemaining2BeSent);
                    var bytesSent = socket.Send(byteBuffer2Send, byteBuffer2Send.Length, 0);
                    byteStartIndex += bytesSent;
                    bytesRemaining2BeSent -= bytesSent;
                    totalBytesSent += bytesSent;
                }
                catch (SocketException sockEx)
                {
                    switch ((SocketError)sockEx.ErrorCode)
                    {
                        case SocketError.AccessDenied:
                            // An attempt was made to access a Socket in a way that is
                            // forbidden by its access permissions.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressAlreadyInUse:
                            // Only one use of an address is normally permitted.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address already in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressFamilyNotSupported:
                            // The address family specified is not supported. This error is
                            // returned if the IPv6 address family was specified and the IPv6
                            // stack is not installed on the local machine. This error is
                            // returned if the IPv4 address family was specified and the
                            // IPv4 stack is not installed on the local machine.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressNotAvailable:
                            // The selected IP address is not valid in this context.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address not available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AlreadyInProgress:
                            // The nonblocking socket already has an operation in progress.
                            // Action: This error only occurs when using a non-blocking socket.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket operation already in progress.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionAborted:
                            // The connection was aborted by the .NET Framework or the
                            // underlying socket provider.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection aborted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionRefused:
                            // The remote host is actively refusing a connection.
                            // Action: Increment port and try again. We could try the same port
                            // again, but it is likely that the same error will occur.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection refused.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionReset:
                            // The connection was reset by the remote peer.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection reset by remote peer.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.DestinationAddressRequired:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket destination address required.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Disconnecting:
                            // A graceful shutdown is in progress.
                            // There as a current connection and the socket is being closed.
                            // Action: Wait, and then try again with the same port.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection shutting down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Fault:
                            // An invalid pointer address was detected by the underlying socket
                            // provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid pointer address.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostDown:
                            // The operation failed because the remote host is down.
                            // Action: Wait, and then try again.
                            // This error is returned if the remote host is down, or if the
                            // remote host is unreachable.
                            // Changing the port will not help.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket remote host is down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostNotFound:
                            // No such host is known. The name is not an official host name or
                            // alias.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket host is unknown.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostUnreachable:
                            // There is no network route to the specified host.
                            // Action: Wait, and then try again.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.InProgress:
                            // A blocking operation is currently executing.
                            // Socket is currently in use. An asynchronous operation
                            // to connect again is occurring.
                            // Leave the port alone and try a different port.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Interrupted:
                            // A blocking operation was canceled.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket blocking operation canceled.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.InvalidArgument:
                            // An invalid argument was supplied to a Socket member.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid member argument.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.IOPending:
                            // The application has initiated an overlapped operation that
                            // cannot be completed immediately.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection overlapped operation.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.IsConnected:
                            // The Socket is already connected.
                            if (!socket.Connected)
                            {
                                // Expected the socket connected flag to be true.
                                // Action: Quit and throw exception.
                                if (this.OnSysSocketStatusOut != null)
                                {
                                    sockEventArgs.StatusOutMsg = "Connection state mismatch.";
                                    this.OnSysSocketStatusOut(this, sockEventArgs);
                                }
                            }

                            throw;

                        case SocketError.MessageSize:
                            // The datagram is too long.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket datagram is too big.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkDown:
                            // The network is down.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkReset:
                            // The connection has been broken due to keep-alive activity
                            // that has already timed out.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection reset.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkUnreachable:
                            // No route to the remote host exists.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket network unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoBufferSpaceAvailable:
                            // No free buffer space is available for a Socket operation.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket no buffer space available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoData:
                            // The requested name or IP address was not found on the name server.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket IP address not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoRecovery:
                            // The error is unrecoverable or the requested database cannot
                            // be located.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket unrecoverable error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotConnected:
                            // The application tried to send or receive data, and the Socket
                            // is not connected.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not connected.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotInitialized:
                            // The underlying socket provider has not been initialized.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not initialized.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotSocket:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address omitted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.OperationAborted:
                            // The overlapped operation was aborted due to the closure of the
                            // Socket.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation reported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.OperationNotSupported:
                            // The address family is not supported by the protocol family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProcessLimit:
                            // Too many processes are using the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket process limit.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolFamilyNotSupported:
                            // The protocol family is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket protocol family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolNotSupported:
                            // The protocol is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolOption:
                            // An unknown, invalid, or unsupported option or level was used
                            // with a socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid protocol option.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolType:
                            // The protocol type is incorrect for this Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol type incorrect.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Shutdown:
                            // A request to send or receive data was disallowed because the
                            // Socket has already been closed.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket already closed.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.SocketError:
                            // An unspecified Socket error has occurred.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection socket error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.SocketNotSupported:
                            // The support for the specified socket type does not exist in this
                            // address family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Success:
                            // The Socket operation succeeded.
                            // Not sure why this is a socket error. Perhaps a Poll() call?
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation succeeded.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            break;

                        case SocketError.SystemNotReady:
                            // The network subsystem is unavailable.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket system not ready.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TimedOut:
                            // The connection attempt timed out, or the connected host has
                            // failed to respond.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TooManyOpenSockets:
                            // There are too many open sockets in the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Too many open sockets.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TryAgain:
                            // The name of the host could not be resolved. Try again later.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unresolved.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TypeNotFound:
                            // The specified class was not found.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.VersionNotSupported:
                            // The version of the underlying socket provider is out of range.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket provider version out of range.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.WouldBlock:
                            // The nonblocking Socket already has an operation in progress.
                            // This error should not occur because we are not using a
                            // non-blocking socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket would block.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        default:
                            var msg = $"Unhandled socket error.{Environment.NewLine}" +
                                      $"Code: {sockEx.ErrorCode + " " + sockEx.Message}";
                            throw new Exception(msg);
                    }
                }
            }

            return totalBytesSent;
        }

        /// <summary>
        ///     Read data from a socket.
        /// </summary>
        /// <param name="socket">.</param>
        /// <returns>A list of bytes from the socket.</returns>
        public List<byte> ReceiveResponse(Socket socket)
        {
            var bytes = new byte[this.ReceiveBuffSize + 1];
            var page = new List<byte>();
            var sockEventArgs = new SysSocketEventArgs();

            // Poll timeout is in microseconds. 1000000 microseconds = 1 second.
            var pollTimeout = this.SysEnv.SysSock.SendTimeOutMilliseconds <= 0
                ? this.SysEnv.SysSock.SendTimeOutMilliseconds * 1000
                : 1000000;

            var bytesReceived = default(int);
            do
            {
                try
                {
                    if (socket.Poll(pollTimeout, SelectMode.SelectError))
                    {
                        // SelectMode.SelectError does not provide specific error details directly.
                        // Typically, need to attempt a socket operation to get the error details.
                        // Send an empty byte array to get the error details.
                        socket.Send(Array.Empty<byte>());
                    }
                    else if (!socket.Poll(pollTimeout, SelectMode.SelectWrite))
                    {
                        // Socket is not ready for writing.
                        // SelectMode.SelectWrite does not provide specific error details directly.
                        // Typically, need to attempt a socket operation to get the error details.
                        // Send an empty byte array to get the error details.
                        socket.Send(Array.Empty<byte>());
                    }

                    bytesReceived = socket.Receive(bytes, bytes.Length, SocketFlags.None);

                    if (this.OnSysSocketStatusOut != null)
                    {
                        sockEventArgs.StatusOutMsg = $"Received {bytesReceived} bytes";
                        this.OnSysSocketStatusOut(this, sockEventArgs);
                    }

                    // Resize to what we received
                    var bytesActuallyReceived = new byte[bytesReceived];
                    Array.Copy(bytes, 0, bytesActuallyReceived, 0, bytesReceived);
                    page.AddRange(bytesActuallyReceived);

                    // Check for EOM
                    var batch = Encoding.UTF8.GetString(bytesActuallyReceived, 0, bytesReceived);
                    if (batch.EndsWith(this.ReceiveEom))
                    {
                        if (this.OnSysSocketStatusOut != null)
                        {
                            sockEventArgs.StatusOutMsg = "EOM received.";
                            this.OnSysSocketStatusOut(this, sockEventArgs);
                        }

                        break;
                    }
                }
                catch (SocketException sockEx)
                {
                    switch ((SocketError)sockEx.ErrorCode)
                    {
                        case SocketError.AccessDenied:
                            // An attempt was made to access a Socket in a way that is
                            // forbidden by its access permissions.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressAlreadyInUse:
                            // Only one use of an address is normally permitted.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address already in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressFamilyNotSupported:
                            // The address family specified is not supported. This error is
                            // returned if the IPv6 address family was specified and the IPv6
                            // stack is not installed on the local machine. This error is
                            // returned if the IPv4 address family was specified and the
                            // IPv4 stack is not installed on the local machine.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AddressNotAvailable:
                            // The selected IP address is not valid in this context.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address not available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.AlreadyInProgress:
                            // The nonblocking socket already has an operation in progress.
                            // Action: This error only occurs when using a non-blocking socket.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket operation already in progress.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionAborted:
                            // The connection was aborted by the .NET Framework or the
                            // underlying socket provider.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection aborted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionRefused:
                            // The remote host is actively refusing a connection.
                            // Action: Increment port and try again. We could try the same port
                            // again, but it is likely that the same error will occur.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection refused.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ConnectionReset:
                            // The connection was reset by the remote peer.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection reset by remote peer.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.DestinationAddressRequired:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket destination address required.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Disconnecting:
                            // A graceful shutdown is in progress.
                            // There as a current connection and the socket is being closed.
                            // Action: Wait, and then try again with the same port.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection shutting down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Fault:
                            // An invalid pointer address was detected by the underlying socket
                            // provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid pointer address.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostDown:
                            // The operation failed because the remote host is down.
                            // Action: Wait, and then try again.
                            // This error is returned if the remote host is down, or if the
                            // remote host is unreachable.
                            // Changing the port will not help.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket remote host is down.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostNotFound:
                            // No such host is known. The name is not an official host name or
                            // alias.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket host is unknown.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.HostUnreachable:
                            // There is no network route to the specified host.
                            // Action: Wait, and then try again.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.InProgress:
                            // A blocking operation is currently executing.
                            // Socket is currently in use. An asynchronous operation
                            // to connect again is occurring.
                            // Leave the port alone and try a different port.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection in use.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Interrupted:
                            // A blocking operation was canceled.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket blocking operation canceled.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.InvalidArgument:
                            // An invalid argument was supplied to a Socket member.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid member argument.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.IOPending:
                            // The application has initiated an overlapped operation that
                            // cannot be completed immediately.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection overlapped operation.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.IsConnected:
                            // The Socket is already connected.
                            if (!socket.Connected)
                            {
                                // Expected the socket connected flag to be true.
                                // Action: Quit and throw exception.
                                if (this.OnSysSocketStatusOut != null)
                                {
                                    sockEventArgs.StatusOutMsg = "Connection state mismatch.";
                                    this.OnSysSocketStatusOut(this, sockEventArgs);
                                }
                            }

                            throw;

                        case SocketError.MessageSize:
                            // The datagram is too long.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket datagram is too big.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkDown:
                            // The network is down.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket access denied.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkReset:
                            // The connection has been broken due to keep-alive activity
                            // that has already timed out.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket connection reset.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NetworkUnreachable:
                            // No route to the remote host exists.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket network unreachable.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoBufferSpaceAvailable:
                            // No free buffer space is available for a Socket operation.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket no buffer space available.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoData:
                            // The requested name or IP address was not found on the name server.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket IP address not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NoRecovery:
                            // The error is unrecoverable or the requested database cannot
                            // be located.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket unrecoverable error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotConnected:
                            // The application tried to send or receive data, and the Socket
                            // is not connected.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not connected.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotInitialized:
                            // The underlying socket provider has not been initialized.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket not initialized.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.NotSocket:
                            // A required address was omitted from an operation on a Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket address omitted.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.OperationAborted:
                            // The overlapped operation was aborted due to the closure of the
                            // Socket.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation reported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.OperationNotSupported:
                            // The address family is not supported by the protocol family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProcessLimit:
                            // Too many processes are using the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket process limit.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolFamilyNotSupported:
                            // The protocol family is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket protocol family not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolNotSupported:
                            // The protocol is not implemented or has not been configured.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolOption:
                            // An unknown, invalid, or unsupported option or level was used
                            // with a socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket invalid protocol option.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.ProtocolType:
                            // The protocol type is incorrect for this Socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket protocol type incorrect.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Shutdown:
                            // A request to send or receive data was disallowed because the
                            // Socket has already been closed.
                            // This error should not occur when attempting a connection.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket already closed.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.SocketError:
                            // An unspecified Socket error has occurred.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection socket error.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.SocketNotSupported:
                            // The support for the specified socket type does not exist in this
                            // address family.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not supported.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.Success:
                            // The Socket operation succeeded.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket operation succeeded.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            break;

                        case SocketError.SystemNotReady:
                            // The network subsystem is unavailable.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket system not ready.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TimedOut:
                            // The connection attempt timed out, or the connected host has
                            // failed to respond.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection attempt timed out.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TooManyOpenSockets:
                            // There are too many open sockets in the underlying socket provider.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Too many open sockets.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TryAgain:
                            // The name of the host could not be resolved. Try again later.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Connection host unresolved.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.TypeNotFound:
                            // The specified class was not found.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket type not found.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.VersionNotSupported:
                            // The version of the underlying socket provider is out of range.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg =
                                    "Socket provider version out of range.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        case SocketError.WouldBlock:
                            // The nonblocking Socket already has an operation in progress.
                            // This error should not occur because we are not using a
                            // non-blocking socket.
                            // Action: Quit and throw exception.
                            if (this.OnSysSocketStatusOut != null)
                            {
                                sockEventArgs.StatusOutMsg = "Socket would block.";
                                this.OnSysSocketStatusOut(this, sockEventArgs);
                            }

                            throw;

                        default:
                            var msg = $"Unhandled socket error.{Environment.NewLine}" +
                                      $"Code: {sockEx.ErrorCode + " " + sockEx.Message}";
                            throw new Exception(msg);
                    }
                }
            }
            while (bytesReceived > 0);

            // Remove the EOM
            var eomBytData = Encoding.UTF8.GetBytes(this.ReceiveEom);
            //// var eomIndex = page.LastIndexOf(eomBytData[eomBytData.Length - 1]);
            var eomIndex = page.LastIndexOf(eomBytData[^1]);
            if (eomIndex >= 0)
            {
                // Determine start of EOM so we can trim it off
                var eomStart = eomIndex - eomBytData.Length;

                // Remove the EOM
                page.RemoveRange(eomStart + 1, eomBytData.Length);
            }

            return page;
        }
    }
}