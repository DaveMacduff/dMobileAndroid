// <copyright file="MoveSkidActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="MoveSkidActivityFactory" />.
    /// </summary>
    public class MoveSkidActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>.</returns>
        public override Activity GetActivity()
        {
            return new MoveSkidActivity();
        }
    }
}
