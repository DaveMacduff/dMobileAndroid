// <copyright file="ActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// This is the abstract activity class factory.
    /// </summary>
    public abstract class ActivityFactory
    {
        /// <summary>
        /// Get an activity instance.
        /// </summary>
        /// <returns><see cref="Activity"/> Activity instance.</returns>
        public abstract Activity GetActivity();
    }
}
