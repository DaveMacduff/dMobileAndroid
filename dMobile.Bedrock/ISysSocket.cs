// <copyright file="ISysSocket.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    ///     System socket event listener interface.
    /// </summary>
    public interface ISysSocketEventListener
    {
        /// <summary>
        ///     On system socket status in event.
        /// </summary>
        /// <param name="sockPublisher">System socket event publisher.</param>
        /// <param name="sockArgs">System socket event arguments.</param>
        void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher, SysSocketEventArgs sockArgs);
    }

    /// <summary>
    ///     System socket event publisher interface.
    /// </summary>
    public interface ISysSocketEventPublisher
    {
        /// <summary>
        ///     Register system socket listener status out event.
        /// </summary>
        /// <param name="sockListener">System socket event listener.</param>
        void RegSockListenerStatusOut(ISysSocketEventListener sockListener);

        /// <summary>
        ///     Unregister system socket listener status out event.
        /// </summary>
        /// <param name="sockListener">System socket event listener.</param>
        void UnregSockListenerStatusOut(ISysSocketEventListener sockListener);
    }
}
