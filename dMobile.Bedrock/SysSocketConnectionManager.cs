// <copyright file="SysSocketConnectionManager.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Manages a persistent, reusable TCP connection to the comm server, reconnecting
    /// transparently when the connection is lost or detected as stale. Also cycles
    /// through the configured port range when the server actively refuses connections
    /// on the current port.
    /// </summary>
    public sealed class SysSocketConnectionManager : IDisposable
    {
        private readonly SysSocket sysSock;
        private readonly object connLock = new object();
        private Socket persistentSocket;
        private int currentPort;
        private bool forceNewPortOnNextConnect;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSocketConnectionManager"/> class.
        /// </summary>
        /// <param name="sysSock">The system socket.</param>
        public SysSocketConnectionManager(SysSocket sysSock)
        {
            this.sysSock = sysSock;
            this.currentPort = sysSock.DefSocketPort;
        }

        /// <summary>
        /// Gets a connected socket, reusing the existing one if still alive,
        /// or establishing a new one (cycling to the next port if the previous
        /// attempt was invalidated due to a connection-level error) if it is not.
        /// </summary>
        /// <returns>A connected socket.</returns>
        /// <exception cref="SocketException">Thrown if the underlying connect attempt fails
        /// with an unrecoverable socket error.</exception>
        public Socket GetConnectedSocket()
        {
            lock (this.connLock)
            {
                if (this.persistentSocket != null && this.IsSocketHealthy(this.persistentSocket))
                {
                    return this.persistentSocket;
                }

                this.CloseExistingSocket();

                if (this.forceNewPortOnNextConnect)
                {
                    this.currentPort = this.NextPort(this.currentPort);
                    this.forceNewPortOnNextConnect = false;
                }

                Socket newSocket = null;
                var tryNewPort = false;
                this.sysSock.Connect(ref newSocket, this.currentPort, ref tryNewPort);

                if (tryNewPort || newSocket == null || !newSocket.Connected)
                {
                    // Connect() did not succeed (e.g. refused/shutting down); remember to
                    // advance to the next port on the following attempt instead of
                    // repeatedly hammering the same dead port.
                    this.forceNewPortOnNextConnect = true;
                    newSocket?.Close();
                    return null;
                }

                this.persistentSocket = newSocket;
                return this.persistentSocket;
            }
        }

        /// <summary>
        /// Call when a send/receive operation fails, forcing reconnect on next use.
        /// </summary>
        /// <param name="cycleToNextPort">If <see langword="true"/>, the next connection attempt
        /// will use the next port in the configured range instead of retrying the current one.</param>
        public void InvalidateConnection(bool cycleToNextPort = false)
        {
            lock (this.connLock)
            {
                this.CloseExistingSocket();
                this.forceNewPortOnNextConnect |= cycleToNextPort;
            }
        }

        /// <summary>
        /// Computes the next port to try, wrapping around within the configured port range.
        /// </summary>
        /// <param name="port">The current port.</param>
        /// <returns>The next port number to attempt.</returns>
        private int NextPort(int port)
        {
            if (this.sysSock.MaxPorts <= 1)
            {
                return this.sysSock.DefSocketPort;
            }

            var offset = port - this.sysSock.BasePort + 1;
            offset %= this.sysSock.MaxPorts;
            return this.sysSock.BasePort + offset;
        }

        /// <summary>
        /// Cheap liveness check without consuming data: detects RST/FIN from peer.
        /// </summary>
        /// <param name="socket">The socket to check.</param>
        /// <returns>True if the socket is healthy; otherwise, false.</returns>
        private bool IsSocketHealthy(Socket socket)
        {
            if (!(socket is { Connected: true }))
            {
                return false;
            }

            try
            {
                var readable = socket.Poll(0, SelectMode.SelectRead);
                return !(readable && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        /// <summary>
        /// Close the existing persistent socket, if any, and discard it. Best-effort cleanup; exceptions are ignored.
        /// </summary>
        private void CloseExistingSocket()
        {
            if (this.persistentSocket == null)
            {
                return;
            }

            try
            {
                this.sysSock.Disconnect(this.persistentSocket);
            }
            catch
            {
                // Best-effort cleanup; socket is being discarded regardless.
            }

            this.persistentSocket = null;
        }

        public void Dispose() => this.InvalidateConnection();
    }
}