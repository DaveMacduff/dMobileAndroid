// <copyright file="clsMoveInventoryActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="MoveInvActivityFactory" />.
    /// </summary>
    public class MoveInvActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>.</returns>
        public override Activity GetActivity()
        {
            return new MoveInvActivity();
        }
    }
}
