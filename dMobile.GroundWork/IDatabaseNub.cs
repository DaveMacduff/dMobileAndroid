// <copyright file="IDatabaseNub.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    public interface IDatabaseNub
    {
        void OnNubStatOut(IDataNubPublisher dataNubPublisher, DataNubEventArgs dataNubArgs);
    }

    public interface IDataNubPublisher
    {
        void RegNubListenerStatOut(IDatabaseNub databaseNub);

        void UnregNubListenerStatOut(IDatabaseNub databaseNub);
    }
}
