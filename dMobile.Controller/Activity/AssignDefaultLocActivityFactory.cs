// <copyright file="AssignDefaultLocActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="AssignDefaultLocActivityFactory" />.
    /// </summary>
    public class AssignDefaultLocActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>.</returns>
        public override Activity GetActivity()
        {
            return new AssignDefaultLocActivity();
        }
    }
}
