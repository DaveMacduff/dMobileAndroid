// <copyright file="ISysSocket.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    public interface ISysSocketEventListener
    {
        void OnSysSocketStatusOut(ISysSocketEventPublisher sockPublisher,
            SysSocketEventArgs sockArgs);
    }

    public interface ISysSocketEventPublisher
    {
        void RegSockListenerStatusOut(ISysSocketEventListener sockListener);

        void UnregSockListenerStatusOut(ISysSocketEventListener sockListener);
    }
}
