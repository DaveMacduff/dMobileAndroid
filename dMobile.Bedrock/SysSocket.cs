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
    /// Provides methods and properties for managing TCP socket connections, including connecting, disconnecting,
    /// sending requests, and receiving responses. Supports configurable connection and retry parameters, and publishes
    /// socket status events to registered listeners.
    /// </summary>
    /// <remarks>The SysSocket class is designed for use in environments where robust socket communication and
    /// error handling are required. It allows for event-driven status reporting via ISysSocketEventPublisher and
    /// supports both synchronous and asynchronous connection workflows. Non-blocking sockets are not supported. Socket
    /// errors are reported through status events and, in most cases, rethrown to the caller. Thread safety is not
    /// guaranteed; callers should ensure appropriate synchronization if accessing instances from multiple
    /// threads.</remarks>
    public class SysSocket : ISysSocketEventPublisher
    {
        /// <summary>
        /// Represents the method that handles socket status output events published by an ISysSocketEventPublisher.
        /// </summary>
        /// <param name="sysSocketEventPublisher">The event publisher that raises the socket status event.</param>
        /// <param name="sysSocketEventArgs">The event data containing information about the socket status.</param>
        private delegate void SysSocketStatusOutEventHandler(
            ISysSocketEventPublisher sysSocketEventPublisher,
            SysSocketEventArgs sysSocketEventArgs);

        /// <summary>
        /// Represents the event that is raised when the system socket status changes.
        /// </summary>
        /// <remarks>Subscribe to this event to receive notifications about changes in the system socket's
        /// status. The event provides status information through the associated event handler delegate.</remarks>
        private event SysSocketStatusOutEventHandler OnSysSocketStatusOut;

        /// <summary>
        /// Gets or sets the IP address of the server to which the client will connect.
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// Gets or sets the size, in bytes, of the reception buffer used for incoming data.
        /// </summary>
        public int ReceiveBuffSize { get; set; } = 32768;

        /// <summary>
        /// Gets or sets the total number of retry attempts for a connection operation.
        /// </summary>
        /// <remarks>Set this property to control how many times a connection operation will be retried
        /// after a failure. A value less than 1 disables retries.</remarks>
        public int ConnTotalRetries { get; set; } = 1;

        /// <summary>
        /// Gets or sets the total number of times to retry sending a message after a failure.
        /// </summary>
        /// <remarks>Set this property to control how many retry attempts are made before the send
        /// operation is considered failed. A value of 0 disables retries.</remarks>
        public int SendTotalRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the total number of retry attempts allowed when receiving messages.
        /// </summary>
        public int ReceiveTotalRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the default port number used for socket connections.
        /// </summary>
        public int DefSocketPort { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the base network port number used for connections.
        /// </summary>
        /// <remarks>The base port typically serves as the starting point for allocating additional ports
        /// for related services or endpoints. Ensure that the specified port is not already in use by another
        /// application on the system.</remarks>
        public int BasePort { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the maximum number of ports that can be used.
        /// </summary>
        public int MaxPorts { get; set; } = 1;

        /// <summary>
        /// Gets or sets the timeout, in milliseconds, for sending data.
        /// </summary>
        public int SendTimeOutMilliseconds { get; set; } = 4101;

        /// <summary>
        /// Gets or sets the timeout period, in milliseconds, to wait for a reception operation to complete.
        /// </summary>
        /// <remarks>If a reception operation does not complete within the specified timeout period, it may
        /// be aborted or result in a timeout error, depending on the implementation. The default value is 4102
        /// milliseconds.</remarks>
        public int ReceiveTimeOutMilliseconds { get; set; } = 4102;

        /// <summary>
        /// Gets the singleton instance of the system environment configuration for the application.
        /// </summary>
        private SysEnvironment SysEnv { get; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets the end-of-message (EOM) marker used to indicate the end of a message in the communication
        /// protocol.
        /// </summary>
        /// <remarks>The EOM marker is typically appended to outgoing messages to signal message
        /// completion to the receiving system. Ensure that the value matches the expected format of the communication
        /// partner to avoid message parsing errors.</remarks>
        private string SendEom { get; } = "</WAIT_EOM>" + (char)20;

        /// <summary>
        /// Gets the end-of-message marker used to identify the conclusion of a received message.
        /// </summary>
        private string ReceiveEom { get; } = "</EOM>" + (char)20;

        /// <summary>
        /// Unregisters a socket event listener from receiving status output events.
        /// </summary>
        /// <remarks>After calling this method, the specified listener will no longer receive status
        /// output events from this instance. If the listener was not previously registered, this method has no
        /// effect.</remarks>
        /// <param name="sysSocketEventListener">The socket event listener to remove from the status output event
        /// notifications. Cannot be null.</param>
        public void UnregSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut -= sysSocketEventListener.OnSysSocketStatusOut;
        }

        /// <summary>
        /// Registers a listener to receive socket status update events.
        /// </summary>
        /// <remarks>Use this method to subscribe to socket status notifications. Multiple listeners can
        /// be registered by calling this method multiple times with different listener instances.</remarks>
        /// <param name="sysSocketEventListener">An object that implements the ISysSocketEventListener interface.
        /// The listener's OnSysSocketStatusOut method will be invoked when a socket status update occurs.</param>
        public void RegSockListenerStatusOut(ISysSocketEventListener sysSocketEventListener)
        {
            this.OnSysSocketStatusOut += sysSocketEventListener.OnSysSocketStatusOut;
        }

        /// <summary>
        /// Closes and disconnects the specified socket, attempting a graceful shutdown and handling common socket
        /// errors.
        /// </summary>
        /// <remarks>This method attempts to gracefully shut down the socket and will retry the operation
        /// multiple times based on the configured retry count. It handles a variety of socket errors, closing the
        /// socket as needed and reporting status messages via the OnSysSocketStatusOut event if it is subscribed. Some
        /// socket errors will result in exceptions being thrown, while others are handled internally. After the
        /// operation, the socket will be closed regardless of its previous state.
        /// Non-blocking sockets are not currently supported by dMobile.</remarks>
        /// <param name="socket">The socket to disconnect. Must not be null and should be in a connected or open state.</param>
        /// <exception cref="ArgumentNullException">Thrown if the socket parameter is null.</exception>
        /// <exception cref="Exception">Thrown if an unhandled or unexpected socket error occurs during the
        /// disconnect operation.</exception>
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
        /// Attempts to establish a TCP connection to the configured server using the specified port and timeout.
        /// </summary>
        /// <remarks>If the connection attempt fails due to a recoverable socket error, the method may set
        /// <see cref="SysSocketInfo.TryNewPort"/> to <see langword="true"/> to indicate that a new port should be
        /// tried. For unrecoverable errors, the method throws the corresponding exception. If a timeout occurs, the
        /// method does not throw but instead reports the status via the <see cref="OnSysSocketStatusOut"/> event, if
        /// subscribed. The method reports connection status and errors through the <see cref="OnSysSocketStatusOut"/>
        /// event handler, if it is set.</remarks>
        /// <param name="port">The port number to connect to. Specify 0 to use the default socket port.</param>
        /// <param name="timeout">The maximum duration to wait for the connection to succeed before timing out. Specify <see
        /// cref="TimeSpan.Zero"/> to use the default timeout of 10 seconds.</param>
        /// <returns>A <see cref="SysSocketInfo"/> object containing information about the connection attempt. The <see
        /// cref="SysSocketInfo.TryNewPort"/> property may be set to <see langword="true"/> if a new port should be
        /// tried due to certain connection errors.</returns>
        /// <exception cref="Exception">Thrown if an unhandled socket error occurs during the connection attempt.</exception>
        public async Task<SysSocketInfo> Connect(int port, TimeSpan timeout)
        {
            port = port == 0 ? this.DefSocketPort : port;
            const int defaultSecondsTimeout = 10;
            timeout = timeout == TimeSpan.Zero ? new TimeSpan(0, 0, defaultSecondsTimeout) : timeout;
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
        /// Establishes a connection using the specified socket and the default port.
        /// </summary>
        /// <remarks>This method attempts to connect the provided socket to the default port defined by
        /// the instance. If the connection fails, the method waits briefly before returning. The state of the socket
        /// after the call indicates whether the connection was successful.</remarks>
        /// <param name="socket">A reference to the <see cref="Socket"/> instance to use for the connection.
        /// The socket will be initialized and connected if the operation succeeds.</param>
        public void Connect(ref Socket socket)
        {
            const int delayBetweenAttempts = 1000;
            var port = this.DefSocketPort;
            var tryNewPort = false;
            this.Connect(ref socket, port, ref tryNewPort);
            Thread.Sleep(delayBetweenAttempts);
        }

        /// <summary>
        /// Attempts to establish a TCP connection to the server at the specified port, updating the provided socket and
        /// indicating whether a new port should be tried based on the connection result.
        /// </summary>
        /// <remarks>If the connection attempt fails due to certain recoverable socket errors (such as
        /// connection refused or host unreachable), <paramref name="tryNewPort"/> is set to <see langword="true"/> to
        /// indicate that the caller should attempt to connect using a different port. For unrecoverable or unexpected
        /// errors, an exception is thrown. The method may block while waiting between retry attempts. Status messages
        /// may be reported via an event if subscribed.</remarks>
        /// <param name="socket">A reference to the socket instance to use for the connection. On successful
        /// connection, this reference is updated to the connected socket. Cannot be null.</param>
        /// <param name="port">The port number on the server to which the connection attempt is made.</param>
        /// <param name="tryNewPort">A reference to a Boolean value that is set to <see langword="true"/> if the
        /// connection attempt failed and a new port should be tried; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="socket"/> is null.</exception>
        /// <exception cref="Exception">Thrown if an unhandled socket error occurs during the connection attempt.</exception>
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
        /// Sends a request string to the specified socket and returns the total number of bytes sent.
        /// </summary>
        /// <remarks>This method attempts to send the entire request, including an end-of-message marker,
        /// to the remote endpoint using the provided socket. If a <see cref="SocketException"/> occurs, it is rethrown
        /// after optionally notifying any registered status event handlers. The method does not perform any retries on
        /// failure. The caller is responsible for ensuring the socket is properly connected and disposed after
        /// use.
        /// Non-blocking sockets are not currently supported by dMobile.
        /// If a socket error occurs, the socket port should be incremented and the connection retried.
        /// </remarks>
        /// <param name="socket">The connected <see cref="Socket"/> instance to which the request will be sent.
        /// The socket must be writable and in a connected state.</param>
        /// <param name="request">The request string to send to the remote endpoint. The string is encoded as UTF-8
        /// and may not be null.</param>
        /// <returns>The total number of bytes sent to the socket, including any end-of-message marker appended to
        /// the request.</returns>
        /// <exception cref="Exception">Thrown if an unhandled socket error occurs that does not correspond to a
        /// specific <see cref="SocketError"/> value.</exception>
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
        /// Receives a response from the specified socket until the end-of-message (EOM) marker is detected.
        /// </summary>
        /// <remarks>This method reads data from the socket in a loop until the configured EOM marker is
        /// found in the received data. If a recognized socket error occurs, a corresponding SocketException is thrown.
        /// The method removes the EOM marker from the returned data. The socket must be connected and operational;
        /// otherwise, exceptions may be thrown. If a status event handler is registered, status messages are reported
        /// during the reception process.</remarks>
        /// <param name="socket">The connected socket from which to receive the response data. The socket must be in
        /// a connected state and ready for reading.</param>
        /// <returns>A list of bytes containing the received response data, excluding the EOM marker. The list will
        /// be empty if no data is received.</returns>
        /// <exception cref="Exception">Thrown if an unhandled or unknown socket error occurs during the reception
        /// operation.</exception>
        public List<byte> ReceiveResponse(Socket socket)
        {
            var bytes = new byte[this.ReceiveBuffSize + 1];
            var page = new List<byte>();
            var sockEventArgs = new SysSocketEventArgs();

            // Poll timeout is in microseconds. 1000000 microseconds = 1 second.
            var pollTimeout = this.SysEnv.SysSock.SendTimeOutMilliseconds <= 0
                ? this.SysEnv.SysSock.SendTimeOutMilliseconds * 1000
                : 1000000;

            var bytesReceived = 0;
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