// <copyright file="SysSocketConnectionManager.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Manages a persistent, reusable TCP connection to the comm server, reconnecting
    /// transparently when the connection is lost or detected as stale.
    /// </summary>
    public sealed class SysSocketConnectionManager : IDisposable
    {
        private readonly SysSocket sysSock;
        private readonly object connLock = new object();
        private Socket persistentSocket;
        private int currentPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSocketConnectionManager"/> class.
        /// </summary>
        /// <param name="sysSock">The system socket.</param>
        public SysSocketConnectionManager(SysSocket sysSock)
        {
            this.sysSock = sysSock;
        }

        /// <summary>
        /// Gets a connected socket, reusing the existing one if still alive,
        /// or establishing a new one (with the existing port-fallback retry logic)
        /// if it is not.
        /// </summary>
        /// <returns>A connected socket.</returns>
        public Socket GetConnectedSocket()
        {
            lock (this.connLock)
            {
                if (this.persistentSocket != null && this.IsSocketHealthy(this.persistentSocket))
                {
                    return this.persistentSocket;
                }

                this.CloseExistingSocket();

                // Reuse existing Connect(ref Socket, port, ref bool) retry/port-fallback logic.
                Socket newSocket = null;
                var port = this.currentPort == 0 ? this.sysSock.DefSocketPort : this.currentPort;
                var tryNewPort = false;
                this.sysSock.Connect(ref newSocket, port, ref tryNewPort);

                this.persistentSocket = newSocket;
                this.currentPort = port;
                return this.persistentSocket;
            }
        }

        /// <summary>
        /// Call when a send/receive operation fails, forcing reconnect on next use.
        /// </summary>
        public void InvalidateConnection()
        {
            lock (this.connLock)
            {
                this.CloseExistingSocket();
            }
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
                // Poll returns true if readable AND zero bytes available => remote closed.
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
