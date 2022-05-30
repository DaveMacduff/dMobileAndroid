// <copyright file="clsReleaseByOrderActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="ReleaseByOrderActivityFactory" />.
    /// </summary>
    public class ReleaseByOrderActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>The <see cref="Activity"/>.</returns>
        public override Activity GetActivity()
        {
            return new ReleaseByOrderActivity();
        }
    }
}
