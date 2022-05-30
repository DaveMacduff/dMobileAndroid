// <copyright file="IDatabaseNub.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    public interface IDataNubListener
    {
        void OnNubStatOut(IDataNubPublisher dataNubPublisher, DataNubEventArgs dataNubArgs);
    }

    public interface IDataNubPublisher
    {
        void RegNubListenerStatOut(IDataNubListener dataNubListener);

        void UnregNubListenerStatOut(IDataNubListener dataNubListener);
    }
}
